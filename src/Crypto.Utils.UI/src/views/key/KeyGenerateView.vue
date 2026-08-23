<script setup lang="ts">
import { computed, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import ModeNotice from '@/components/ModeNotice.vue'
import CopyableText from '@/components/CopyableText.vue'
import ResultPanel from '@/components/ResultPanel.vue'
import { ServiceFactory } from '@/services/ServiceFactory'
import { useAppStore } from '@/stores/useAppStore'
import { EC_CURVES, KEY_ALGORITHMS, RSA_KEY_SIZES, DSA_KEY_SIZES } from '@/models/key'
import type { KeyPairResult } from '@/models/key'
import { buildFilename, downloadText } from '@/utils/download'

const { t } = useI18n()
const appStore = useAppStore()
const keyService = ServiceFactory.getKeyService(appStore.mode)

const algorithm = ref<string>('RSA')
const keySize = ref<number>(2048)
const curveName = ref<string>('secp256r1')
const outputFormat = ref<string>('PEM')
const loading = ref(false)
const error = ref<string | null>(null)
const result = ref<KeyPairResult | null>(null)

const showKeySize = computed(() => algorithm.value === 'RSA' || algorithm.value === 'DSA')
const showCurve = computed(() => algorithm.value === 'EC')
const availableSizes = computed(() =>
  algorithm.value === 'DSA' ? [...DSA_KEY_SIZES] : [...RSA_KEY_SIZES],
)

function resetError(): void {
  error.value = null
}

async function handleGenerate(): Promise<void> {
  resetError()
  loading.value = true
  try {
    result.value = await keyService.generateKeyPair({
      algorithm: algorithm.value,
      keySize: showKeySize.value ? keySize.value : null,
      curveName: showCurve.value ? curveName.value : null,
      outputFormat: outputFormat.value,
    })
  } catch (err) {
    error.value = err instanceof Error ? err.message : String(err)
    result.value = null
  } finally {
    loading.value = false
  }
}

function downloadPrivate(): void {
  if (!result.value) return
  const ext = outputFormat.value === 'DER' ? 'der' : 'pem'
  const content = outputFormat.value === 'DER' ? result.value.privateKey : result.value.privateKey
  downloadText(content, buildFilename('private-key', result.value.algorithm, ext), ext === 'der' ? 'application/octet-stream' : 'application/x-pem-file')
}

function downloadPublic(): void {
  if (!result.value) return
  const ext = outputFormat.value === 'DER' ? 'der' : 'pem'
  downloadText(result.value.publicKey, buildFilename('public-key', result.value.algorithm, ext), ext === 'der' ? 'application/octet-stream' : 'application/x-pem-file')
}
</script>

<template>
  <div class="page">
    <ModeNotice />

    <el-card>
      <template #header>
        <div class="card-header">
          <span>{{ t('key.generateTitle') }}</span>
          <el-tag type="primary" size="small">{{ t('common.algorithm') }}</el-tag>
        </div>
      </template>

      <el-form label-width="110px" @submit.prevent>
        <el-form-item :label="t('common.algorithm')">
          <el-radio-group v-model="algorithm" @change="resetError">
            <el-radio-button v-for="alg in KEY_ALGORITHMS" :key="alg" :value="alg">
              {{ alg }}
            </el-radio-button>
          </el-radio-group>
        </el-form-item>

        <el-form-item v-if="showKeySize" :label="t('key.keySize')">
          <el-select v-model="keySize" style="width: 200px">
            <el-option v-for="size in availableSizes" :key="size" :value="size" :label="`${size} bit`" />
          </el-select>
        </el-form-item>

        <el-form-item v-if="showCurve" :label="t('key.curveName')">
          <el-select v-model="curveName" style="width: 240px">
            <el-option
              v-for="curve in EC_CURVES"
              :key="curve.value"
              :value="curve.value"
              :label="curve.label"
            />
          </el-select>
        </el-form-item>

        <el-form-item :label="t('common.outputFormat')">
          <el-radio-group v-model="outputFormat" @change="resetError">
            <el-radio-button value="PEM">{{ t('common.pem') }}</el-radio-button>
            <el-radio-button value="DER">{{ t('common.der') }}</el-radio-button>
          </el-radio-group>
        </el-form-item>

        <el-form-item>
          <el-button type="primary" :loading="loading" :icon="'MagicStick'" @click="handleGenerate">
            {{ t('common.generate') }}
          </el-button>
        </el-form-item>
      </el-form>

      <ResultPanel :error="error" />

      <template v-if="result">
        <el-divider content-position="left">{{ t('key.generateResult') }}</el-divider>
        <div class="key-block">
          <div class="key-block-head">
            <span>{{ t('common.privateKey') }}（{{ result.algorithm }}）</span>
            <el-button size="small" :icon="'Download'" @click="downloadPrivate">{{ t('common.download') }}</el-button>
          </div>
          <CopyableText :content="result.privateKey" :rows="10" />
        </div>
        <div class="key-block">
          <div class="key-block-head">
            <span>{{ t('common.publicKey') }}（{{ result.algorithm }}）</span>
            <el-button size="small" :icon="'Download'" @click="downloadPublic">{{ t('common.download') }}</el-button>
          </div>
          <CopyableText :content="result.publicKey" :rows="6" />
        </div>
      </template>
    </el-card>
  </div>
</template>

<style scoped>
.page {
  max-width: 860px;
  margin: 0 auto;
}
.card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.key-block {
  margin-top: 16px;
}
.key-block-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 6px;
  font-size: 13px;
  color: var(--el-text-color-secondary);
}
</style>
