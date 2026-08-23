<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import ModeNotice from '@/components/ModeNotice.vue'
import KeyTextArea from '@/components/KeyTextArea.vue'
import ResultPanel from '@/components/ResultPanel.vue'
import SampleFillButton from '@/components/SampleFillButton.vue'
import { ServiceFactory } from '@/services/ServiceFactory'
import { useAppStore } from '@/stores/useAppStore'
import type { CsrInfoResult } from '@/models/csr'
import { SAMPLE_CSR } from '@/utils/samples'

const { t } = useI18n()
const appStore = useAppStore()
const csrService = ServiceFactory.getCsrService(appStore.mode)

const csrData = ref('')
const loading = ref(false)
const error = ref<string | null>(null)
const result = ref<CsrInfoResult | null>(null)

function fillSample(): void {
  csrData.value = SAMPLE_CSR
  error.value = null
  result.value = null
}

async function handleParse(): Promise<void> {
  if (!csrData.value.trim()) {
    error.value = t('errors.emptyCsr')
    return
  }
  error.value = null
  loading.value = true
  try {
    result.value = await csrService.parseCsr({ csrData: csrData.value })
  } catch (err) {
    error.value = err instanceof Error ? err.message : String(err)
    result.value = null
  } finally {
    loading.value = false
  }
}

const extensionEntries = () => Object.entries(result.value?.extensions ?? {})
</script>

<template>
  <div class="page">
    <ModeNotice />

    <el-card>
      <div class="toolbar">
        <SampleFillButton @fill="fillSample" />
      </div>

      <KeyTextArea v-model="csrData" :placeholder="t('csr.csrData')" />

      <div class="actions">
        <el-button type="primary" :loading="loading" :icon="'Search'" @click="handleParse">
          {{ t('common.parse') }}
        </el-button>
      </div>

      <ResultPanel :error="error" />

      <template v-if="result">
        <el-divider content-position="left">{{ t('csr.parseResult') }}</el-divider>

        <el-descriptions :column="1" border>
          <el-descriptions-item :label="t('csr.subjectDn')">{{ result.subject }}</el-descriptions-item>
          <el-descriptions-item :label="t('csr.signatureAlgorithmName')">
            {{ result.signatureAlgorithmName }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('key.algorithmName')">
            {{ result.publicKey.algorithmName }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('key.keySizeLabel')">
            {{ result.publicKey.keySize ? `${result.publicKey.keySize} bit` : '-' }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('key.curve')">
            {{ result.publicKey.curveName ?? '-' }}
          </el-descriptions-item>
        </el-descriptions>

        <template v-if="extensionEntries().length">
          <h4 class="sub-title">{{ t('csr.extensions') }}</h4>
          <el-table :data="extensionEntries().map(([k, v]) => ({ k, v }))" border size="small" max-height="320">
            <el-table-column prop="k" label="Key" width="180" />
            <el-table-column prop="v" label="Value" show-overflow-tooltip />
          </el-table>
        </template>
      </template>
    </el-card>
  </div>
</template>

<style scoped>
.page {
  max-width: 860px;
  margin: 0 auto;
}
.toolbar {
  margin-bottom: 12px;
}
.actions {
  margin-top: 12px;
}
.sub-title {
  margin: 20px 0 8px;
  font-size: 14px;
  color: var(--el-text-color-primary);
}
</style>
