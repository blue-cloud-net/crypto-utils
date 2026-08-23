<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import ModeNotice from '@/components/ModeNotice.vue'
import KeyTextArea from '@/components/KeyTextArea.vue'
import ResultPanel from '@/components/ResultPanel.vue'
import SampleFillButton from '@/components/SampleFillButton.vue'
import { ServiceFactory } from '@/services/ServiceFactory'
import { useAppStore } from '@/stores/useAppStore'
import type { CertificateInfoResult } from '@/models/certificate'
import { SAMPLE_CERTIFICATE } from '@/utils/samples'
import { formatDateTime, formatDays } from '@/utils/format'

const { t } = useI18n()
const appStore = useAppStore()
const certService = ServiceFactory.getCertificateService(appStore.mode)

const certData = ref('')
const loading = ref(false)
const error = ref<string | null>(null)
const result = ref<CertificateInfoResult | null>(null)

function fillSample(): void {
  certData.value = SAMPLE_CERTIFICATE
  error.value = null
  result.value = null
}

async function handleParse(): Promise<void> {
  if (!certData.value.trim()) {
    error.value = t('errors.emptyCertificate')
    return
  }
  error.value = null
  loading.value = true
  try {
    result.value = await certService.parseCertificate({ certificateData: certData.value })
  } catch (err) {
    error.value = err instanceof Error ? err.message : String(err)
    result.value = null
  } finally {
    loading.value = false
  }
}

const fingerprintEntries = () => Object.entries(result.value?.fingerprints ?? {})
const kuList = () => result.value?.keyUsage ?? []
const ekuList = () => result.value?.extendedKeyUsage ?? []
const sanList = () => result.value?.subjectAlternativeNames ?? []
const policyList = () => result.value?.certificatePolicies ?? []
const crldpList = () => result.value?.crlDistributionPoints ?? []
</script>

<template>
  <div class="page">
    <ModeNotice />

    <el-card>
      <div class="toolbar">
        <SampleFillButton @fill="fillSample" />
      </div>

      <KeyTextArea v-model="certData" :placeholder="t('cert.certificateData')" />

      <div class="actions">
        <el-button type="primary" :loading="loading" :icon="'Search'" @click="handleParse">
          {{ t('common.parse') }}
        </el-button>
      </div>

      <ResultPanel :error="error" />

      <template v-if="result">
        <el-divider content-position="left">{{ t('cert.parseResult') }}</el-divider>

        <el-descriptions :column="2" border>
          <el-descriptions-item :label="t('cert.version')">{{ `V${result.version}` }}</el-descriptions-item>
          <el-descriptions-item :label="t('cert.isCA')">
            <el-tag :type="result.isCA ? 'primary' : 'info'" size="small">
              {{ result.isCA ? 'Yes' : 'No' }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item :label="t('cert.serialNumber')" :span="2">
            <code>{{ result.serialNumber }}</code>
          </el-descriptions-item>
          <el-descriptions-item :label="t('cert.subject')" :span="2">{{ result.subject }}</el-descriptions-item>
          <el-descriptions-item :label="t('cert.issuer')" :span="2">{{ result.issuer }}</el-descriptions-item>
          <el-descriptions-item :label="t('cert.notBefore')">{{ formatDateTime(result.notBefore) }}</el-descriptions-item>
          <el-descriptions-item :label="t('cert.notAfter')">{{ formatDateTime(result.notAfter) }}</el-descriptions-item>
          <el-descriptions-item :label="t('cert.durationDays')">{{ formatDays(result.duration) }}</el-descriptions-item>
          <el-descriptions-item :label="t('cert.remainingDays')">
            <el-tag :type="result.isValid ? 'success' : 'danger'" size="small">
              {{ result.isValid ? t('cert.valid') : t('cert.invalid') }} · {{ formatDays(result.remainingDays) }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item :label="t('cert.signatureAlgorithm')">
            {{ result.signatureAlgorithmName }}
          </el-descriptions-item>
          <el-descriptions-item :label="'OID'">
            <code>{{ result.signatureAlgorithmOid }}</code>
          </el-descriptions-item>
        </el-descriptions>

        <template v-if="fingerprintEntries().length">
          <h4 class="sub-title">{{ t('cert.fingerprints') }}</h4>
          <el-descriptions :column="1" border>
            <el-descriptions-item v-for="[alg, value] in fingerprintEntries()" :key="alg" :label="alg">
              <code>{{ value }}</code>
            </el-descriptions-item>
          </el-descriptions>
        </template>

        <h4 class="sub-title">{{ t('cert.publicKey') }}</h4>
        <el-descriptions :column="2" border>
          <el-descriptions-item :label="t('key.algorithmName')">{{ result.publicKey.algorithmName }}</el-descriptions-item>
          <el-descriptions-item :label="t('key.keySizeLabel')">
            {{ result.publicKey.keySize ? `${result.publicKey.keySize} bit` : '-' }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('key.curve')" :span="2">
            {{ result.publicKey.curveName ?? '-' }}
          </el-descriptions-item>
        </el-descriptions>

        <template v-if="result.subjectKeyIdentifier">
          <h4 class="sub-title">{{ t('cert.subjectKeyIdentifier') }}</h4>
          <code>{{ result.subjectKeyIdentifier }}</code>
        </template>
        <template v-if="result.authorityKeyIdentifier">
          <h4 class="sub-title">{{ t('cert.authorityKeyIdentifier') }}</h4>
          <code>{{ result.authorityKeyIdentifier }}</code>
        </template>

        <template v-if="kuList().length">
          <h4 class="sub-title">{{ t('cert.keyUsage') }}</h4>
          <el-tag v-for="ku in kuList()" :key="ku" size="small" class="tag-item">{{ ku }}</el-tag>
        </template>
        <template v-if="ekuList().length">
          <h4 class="sub-title">{{ t('cert.extendedKeyUsage') }}</h4>
          <el-tag v-for="eku in ekuList()" :key="eku" size="small" type="success" class="tag-item">{{ eku }}</el-tag>
        </template>
        <template v-if="sanList().length">
          <h4 class="sub-title">{{ t('cert.subjectAlternativeNames') }}</h4>
          <ul class="text-list">
            <li v-for="(san, i) in sanList()" :key="i"><code>{{ san }}</code></li>
          </ul>
        </template>
        <template v-if="policyList().length">
          <h4 class="sub-title">{{ t('cert.certificatePolicies') }}</h4>
          <ul class="text-list">
            <li v-for="(p, i) in policyList()" :key="i"><code>{{ p }}</code></li>
          </ul>
        </template>
        <template v-if="crldpList().length">
          <h4 class="sub-title">{{ t('cert.crlDistributionPoints') }}</h4>
          <ul class="text-list">
            <li v-for="(u, i) in crldpList()" :key="i"><code>{{ u }}</code></li>
          </ul>
        </template>
      </template>
    </el-card>
  </div>
</template>

<style scoped>
.page {
  max-width: 960px;
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
.tag-item {
  margin-right: 8px;
  margin-bottom: 4px;
}
.text-list {
  margin: 0;
  padding-left: 20px;
  font-size: 13px;
  color: var(--el-text-color-regular);
}
</style>
