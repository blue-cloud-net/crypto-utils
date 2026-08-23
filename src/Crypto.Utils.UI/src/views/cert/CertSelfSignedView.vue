<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import ModeNotice from '@/components/ModeNotice.vue'
import KeyTextArea from '@/components/KeyTextArea.vue'
import CopyableText from '@/components/CopyableText.vue'
import ResultPanel from '@/components/ResultPanel.vue'
import SampleFillButton from '@/components/SampleFillButton.vue'
import { ServiceFactory } from '@/services/ServiceFactory'
import { useAppStore } from '@/stores/useAppStore'
import { SIGNATURE_ALGORITHMS } from '@/models/certificate'
import type { CertificateResult } from '@/models/certificate'
import { SAMPLE_PRIVATE_KEY } from '@/utils/samples'
import { buildFilename, downloadBase64, downloadText } from '@/utils/download'

const { t } = useI18n()
const appStore = useAppStore()
const certService = ServiceFactory.getCertificateService(appStore.mode)

const cn = ref('')
const validFrom = ref<Date | null>(null)
const validTo = ref<Date>(defaultValidTo())
const privateKey = ref('')
const signatureAlgorithm = ref('SHA256WITHRSA')
const outputFormat = ref('PEM')
const loading = ref(false)
const error = ref<string | null>(null)
const result = ref<CertificateResult | null>(null)

function defaultValidTo(): Date {
  const date = new Date()
  date.setFullYear(date.getFullYear() + 1)
  return date
}

function fillPrivateKey(): void {
  privateKey.value = SAMPLE_PRIVATE_KEY
}

async function handleGenerate(): Promise<void> {
  if (!cn.value.trim()) {
    error.value = t('errors.emptyCn')
    return
  }
  if (!privateKey.value.trim()) {
    error.value = t('errors.emptyPrivateKey')
    return
  }
  if (!validTo.value) {
    error.value = t('errors.emptyValidTo')
    return
  }
  error.value = null
  loading.value = true
  try {
    result.value = await certService.generateSelfSigned({
      subject: { CN: cn.value.trim() },
      privateKey: privateKey.value,
      validFrom: validFrom.value ? new Date(validFrom.value).toISOString() : null,
      validTo: new Date(validTo.value).toISOString(),
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

function downloadCertificate(): void {
  if (!result.value) return
  const isDer = result.value.format.toUpperCase() === 'DER'
  const ext = isDer ? 'der' : 'crt'
  const filename = buildFilename('certificate', cn.value.trim(), ext)
  if (isDer) {
    // DER 输出为 Base64 文本，解码为二进制后下载
    downloadBase64(result.value.certificateData, filename)
  } else {
    downloadText(result.value.certificateData, filename)
  }
}
</script>

<template>
  <div class="page">
    <ModeNotice />

    <el-card>
      <el-form label-width="120px" @submit.prevent>
        <el-form-item :label="t('subject.cn')" required>
          <el-input v-model="cn" :placeholder="'example.com'" maxlength="128" />
        </el-form-item>

        <el-form-item :label="t('cert.validFrom')">
          <el-date-picker
            v-model="validFrom"
            type="datetime"
            :placeholder="t('cert.validityHint')"
            style="width: 260px"
          />
        </el-form-item>

        <el-form-item :label="t('cert.validTo')" required>
          <el-date-picker v-model="validTo" type="datetime" style="width: 260px" />
        </el-form-item>

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
          <el-button type="primary" :loading="loading" :icon="'CircleCheck'" @click="handleGenerate">
            {{ t('common.generate') }}
          </el-button>
        </el-form-item>
      </el-form>

      <ResultPanel :error="error" />

      <template v-if="result">
        <el-divider content-position="left">{{ t('cert.generateResult') }}</el-divider>
        <div class="result-head">
          <el-tag size="small" type="success">{{ result.format }}</el-tag>
          <el-button size="small" :icon="'Download'" @click="downloadCertificate">
            {{ t('common.download') }}
          </el-button>
        </div>
        <CopyableText :content="result.certificateData" :rows="12" />
      </template>
    </el-card>
  </div>
</template>

<style scoped>
.page {
  max-width: 860px;
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
