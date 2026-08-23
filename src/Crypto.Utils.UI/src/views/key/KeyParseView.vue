<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import ModeNotice from '@/components/ModeNotice.vue'
import KeyTextArea from '@/components/KeyTextArea.vue'
import ResultPanel from '@/components/ResultPanel.vue'
import SampleFillButton from '@/components/SampleFillButton.vue'
import { ServiceFactory } from '@/services/ServiceFactory'
import { useAppStore } from '@/stores/useAppStore'
import type { KeyInfoResult } from '@/models/key'
import { SAMPLE_PRIVATE_KEY } from '@/utils/samples'

const { t } = useI18n()
const appStore = useAppStore()
const keyService = ServiceFactory.getKeyService(appStore.mode)

const keyData = ref('')
const loading = ref(false)
const error = ref<string | null>(null)
const result = ref<KeyInfoResult | null>(null)

function fillSample(): void {
  keyData.value = SAMPLE_PRIVATE_KEY
  error.value = null
  result.value = null
}

async function handleParse(): Promise<void> {
  if (!keyData.value.trim()) {
    error.value = t('errors.emptyKeyData')
    return
  }
  error.value = null
  loading.value = true
  try {
    result.value = await keyService.parseKey(keyData.value)
  } catch (err) {
    error.value = err instanceof Error ? err.message : String(err)
    result.value = null
  } finally {
    loading.value = false
  }
}

const fingerprintEntries = () => Object.entries(result.value?.fingerprints ?? {})
const parameterEntries = () => Object.entries(result.value?.parameters ?? {})
</script>

<template>
  <div class="page">
    <ModeNotice />

    <el-card>
      <div class="toolbar">
        <SampleFillButton @fill="fillSample" />
      </div>

      <KeyTextArea v-model="keyData" :placeholder="t('common.keyData')" />

      <div class="actions">
        <el-button type="primary" :loading="loading" :icon="'Search'" @click="handleParse">
          {{ t('common.parse') }}
        </el-button>
      </div>

      <ResultPanel :error="error" />

      <template v-if="result">
        <el-divider content-position="left">{{ t('key.parseResult') }}</el-divider>

        <el-descriptions :column="2" border>
          <el-descriptions-item :label="t('key.algorithmName')">
            {{ result.algorithmName }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('key.isPrivate')">
            <el-tag v-if="result.isPrivate" type="warning" size="small">{{ t('key.private') }}</el-tag>
            <el-tag v-else type="success" size="small">{{ t('key.public') }}</el-tag>
          </el-descriptions-item>
          <el-descriptions-item :label="t('key.keySizeLabel')">
            {{ result.keySize ? `${result.keySize} bit` : '-' }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('key.curve')">
            {{ result.curveName ?? '-' }}
          </el-descriptions-item>
        </el-descriptions>

        <template v-if="fingerprintEntries().length">
          <h4 class="sub-title">{{ t('key.fingerprints') }}</h4>
          <el-descriptions :column="1" border>
            <el-descriptions-item v-for="[alg, value] in fingerprintEntries()" :key="alg" :label="alg">
              <code>{{ value }}</code>
            </el-descriptions-item>
          </el-descriptions>
        </template>

        <template v-if="parameterEntries().length">
          <h4 class="sub-title">{{ t('key.parameters') }}</h4>
          <el-table :data="parameterEntries().map(([k, v]) => ({ k, v }))" border size="small" max-height="320">
            <el-table-column prop="k" label="Key" width="140" />
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
