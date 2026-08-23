<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import ModeNotice from '@/components/ModeNotice.vue'
import KeyTextArea from '@/components/KeyTextArea.vue'
import CopyableText from '@/components/CopyableText.vue'
import ResultPanel from '@/components/ResultPanel.vue'
import SampleFillButton from '@/components/SampleFillButton.vue'
import { ServiceFactory } from '@/services/ServiceFactory'
import { useAppStore } from '@/stores/useAppStore'
import { SIGNATURE_ALGORITHMS } from '@/models/certificate'
import type { CsrResult } from '@/models/csr'
import { createEmptySubject, normalizeSubject } from '@/models/common/SubjectInfo'
import { SAMPLE_PRIVATE_KEY } from '@/utils/samples'
import { buildFilename, downloadBase64, downloadText } from '@/utils/download'

const { t } = useI18n()
const appStore = useAppStore()
const csrService = ServiceFactory.getCsrService(appStore.mode)

const subject = reactive(createEmptySubject())
const privateKey = ref('')
const signatureAlgorithm = ref('SHA256WITHRSA')
const outputFormat = ref('PEM')
const loading = ref(false)
const error = ref<string | null>(null)
const result = ref<CsrResult | null>(null)

function fillPrivateKey(): void {
  privateKey.value = SAMPLE_PRIVATE_KEY
}

async function handleGenerate(): Promise<void> {
  if (!subject.CN.trim()) {
    error.value = t('errors.emptyCn')
    return
  }
  if (!privateKey.value.trim()) {
    error.value = t('errors.emptyPrivateKey')
    return
  }
  error.value = null
  loading.value = true
  try {
    result.value = await csrService.generateCsr({
      subject: normalizeSubject(subject),
      privateKey: privateKey.value,
      signatureAlgorithm: signatureAlgorithm.value,
      outputFormat: outputFormat.value,
    })
  } catch (err) {
    error.value = err instanceof Error ? err.message : String(err)
    result.value = null
  } finally {
    loading.value = false
  }
}

function downloadCsr(): void {
  if (!result.value) return
  const isDer = result.value.format.toUpperCase() === 'DER'
  const ext = isDer ? 'der' : 'csr'
  const filename = buildFilename('csr', subject.CN.trim(), ext)
  if (isDer) {
    // DER 输出为 Base64 文本，解码为二进制后下载
    downloadBase64(result.value.csrData, filename)
  } else {
    downloadText(result.value.csrData, filename)
  }
}
</script>

<template>
  <div class="page">
    <ModeNotice />

    <el-card>
      <el-form label-width="140px" @submit.prevent>
        <el-row :gutter="16">
          <el-col :md="12">
            <el-form-item :label="t('subject.cn')" required>
              <el-input v-model="subject.CN" :placeholder="'example.com'" maxlength="128" />
            </el-form-item>
          </el-col>
          <el-col :md="12">
            <el-form-item :label="t('subject.o')">
              <el-input v-model="subject.O" maxlength="128" />
            </el-form-item>
          </el-col>
          <el-col :md="12">
            <el-form-item :label="t('subject.ou')">
              <el-input v-model="subject.OU" maxlength="128" />
            </el-form-item>
          </el-col>
          <el-col :md="12">
            <el-form-item :label="t('subject.c')">
              <el-input v-model="subject.C" maxlength="2" :placeholder="'CN'" />
            </el-form-item>
          </el-col>
          <el-col :md="12">
            <el-form-item :label="t('subject.st')">
              <el-input v-model="subject.ST" maxlength="128" />
            </el-form-item>
          </el-col>
          <el-col :md="12">
            <el-form-item :label="t('subject.l')">
              <el-input v-model="subject.L" maxlength="128" />
            </el-form-item>
          </el-col>
          <el-col :md="12">
            <el-form-item :label="t('subject.e')">
              <el-input v-model="subject.E" maxlength="128" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item :label="t('cert.signatureAlgorithm')">
          <el-select v-model="signatureAlgorithm" style="width: 220px">
            <el-option v-for="alg in SIGNATURE_ALGORITHMS" :key="alg" :value="alg" :label="alg" />
          </el-select>
        </el-form-item>

        <el-form-item :label="t('common.outputFormat')">
          <el-radio-group v-model="outputFormat">
            <el-radio-button value="PEM">{{ t('common.pem') }}</el-radio-button>
            <el-radio-button value="DER">{{ t('common.der') }}</el-radio-button>
          </el-radio-group>
        </el-form-item>

        <el-form-item :label="t('common.privateKey')" required>
          <div class="key-field">
            <div class="toolbar">
              <SampleFillButton @fill="fillPrivateKey" />
            </div>
            <KeyTextArea v-model="privateKey" :placeholder="t('common.privateKey')" :rows="8" />
          </div>
        </el-form-item>

        <el-form-item>
          <el-button type="primary" :loading="loading" :icon="'EditPen'" @click="handleGenerate">
            {{ t('common.generate') }}
          </el-button>
        </el-form-item>
      </el-form>

      <ResultPanel :error="error" />

      <template v-if="result">
        <el-divider content-position="left">{{ t('csr.generateResult') }}</el-divider>
        <div class="result-head">
          <el-tag size="small" type="success">{{ result.format }}</el-tag>
          <el-button size="small" :icon="'Download'" @click="downloadCsr">
            {{ t('common.download') }}
          </el-button>
        </div>
        <CopyableText :content="result.csrData" :rows="10" />
      </template>
    </el-card>
  </div>
</template>

<style scoped>
.page {
  max-width: 900px;
  margin: 0 auto;
}
.key-field {
  width: 100%;
}
.toolbar {
  margin-bottom: 8px;
}
.result-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 6px;
}
</style>
