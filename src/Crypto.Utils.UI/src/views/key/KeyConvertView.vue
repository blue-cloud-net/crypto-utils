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
import { PRIVATE_KEY_CIPHERS } from '@/models/key'
import type { KeyConvertResult } from '@/models/key'
import { SAMPLE_PRIVATE_KEY } from '@/utils/samples'
import { buildFilename, downloadBase64, downloadText } from '@/utils/download'

const { t } = useI18n()
const appStore = useAppStore()
const keyService = ServiceFactory.getKeyService(appStore.mode)

const activeTab = ref('format')

const error = ref<string | null>(null)
const loading = ref(false)

// --- Tab 1: PEM ↔ DER ---
const convertKeyData = ref('')
const convertSource = ref('PEM')
const convertTarget = ref('DER')
const convertResult = ref<KeyConvertResult | null>(null)

async function handleConvert(): Promise<void> {
  if (!convertKeyData.value.trim()) {
    error.value = t('errors.emptyKeyData')
    return
  }
  if (convertSource.value === convertTarget.value) {
    error.value = t('errors.selectSourceTarget')
    return
  }
  error.value = null
  loading.value = true
  try {
    convertResult.value = await keyService.convertKeyFormat({
      keyData: convertKeyData.value,
      sourceFormat: convertSource.value,
      targetFormat: convertTarget.value,
    })
  } catch (err) {
    error.value = err instanceof Error ? err.message : String(err)
  } finally {
    loading.value = false
  }
}

// --- Tab 2: PKCS#1 ↔ PKCS#8 ---
const pkcsKeyData = ref('')
const pkcsSource = ref('PKCS1')
const pkcsTarget = ref('PKCS8')
const pkcsPassword = ref('')
const pkcsResult = ref<KeyConvertResult | null>(null)

async function handlePkcsConvert(): Promise<void> {
  if (!pkcsKeyData.value.trim()) {
    error.value = t('errors.emptyKeyData')
    return
  }
  error.value = null
  loading.value = true
  try {
    pkcsResult.value = await keyService.convertPkcsFormat({
      keyData: pkcsKeyData.value,
      sourceFormat: pkcsSource.value,
      targetFormat: pkcsTarget.value,
      password: pkcsTarget.value === 'PKCS8' && pkcsPassword.value ? pkcsPassword.value : null,
    })
  } catch (err) {
    error.value = err instanceof Error ? err.message : String(err)
  } finally {
    loading.value = false
  }
}

// --- Tab 3: 私钥加密 / 解密 ---
const encPrivateKey = ref('')
const encPassword = ref('')
const encAlgorithm = ref('AES-256-CBC')
const encResult = ref<KeyConvertResult | null>(null)
const decEncryptedKey = ref('')
const decPassword = ref('')
const decResult = ref<KeyConvertResult | null>(null)

async function handleEncrypt(): Promise<void> {
  if (!encPrivateKey.value.trim()) {
    error.value = t('errors.emptyPrivateKey')
    return
  }
  if (!encPassword.value) {
    error.value = t('errors.emptyPassword')
    return
  }
  error.value = null
  loading.value = true
  try {
    encResult.value = await keyService.encryptPrivateKey({
      privateKey: encPrivateKey.value,
      password: encPassword.value,
      algorithm: encAlgorithm.value,
    })
  } catch (err) {
    error.value = err instanceof Error ? err.message : String(err)
  } finally {
    loading.value = false
  }
}

async function handleDecrypt(): Promise<void> {
  if (!decEncryptedKey.value.trim()) {
    error.value = t('errors.emptyEncryptedKey')
    return
  }
  if (!decPassword.value) {
    error.value = t('errors.emptyPassword')
    return
  }
  error.value = null
  loading.value = true
  try {
    decResult.value = await keyService.decryptPrivateKey({
      encryptedPrivateKey: decEncryptedKey.value,
      password: decPassword.value,
    })
  } catch (err) {
    error.value = err instanceof Error ? err.message : String(err)
  } finally {
    loading.value = false
  }
}

function downloadResult(result: KeyConvertResult, type: string): void {
  const isDer = result.format.toUpperCase() === 'DER'
  const ext = isDer ? 'der' : 'pem'
  const filename = buildFilename(type, 'converted', ext)
  if (isDer) {
    downloadBase64(result.convertedKey, filename)
  } else {
    downloadText(result.convertedKey, filename)
  }
}

function fillConvertSample(): void {
  convertKeyData.value = SAMPLE_PRIVATE_KEY
}
function fillPkcsSample(): void {
  pkcsKeyData.value = SAMPLE_PRIVATE_KEY
}
function fillEncryptSample(): void {
  encPrivateKey.value = SAMPLE_PRIVATE_KEY
}
</script>

<template>
  <div class="page">
    <ModeNotice />

    <el-card>
      <el-tabs v-model="activeTab">
        <!-- Tab 1: PEM ↔ DER -->
        <el-tab-pane :label="t('key.tabConvertFormat')" name="format">
          <div class="toolbar">
            <SampleFillButton @fill="fillConvertSample" />
            <span class="hint">{{ t('key.derNotice') }}</span>
          </div>
          <KeyTextArea v-model="convertKeyData" :placeholder="t('common.keyData')" />
          <div class="format-row">
            <el-select v-model="convertSource" style="width: 160px">
              <el-option value="PEM" :label="t('common.pem')" />
              <el-option value="DER" :label="t('common.der')" />
            </el-select>
            <el-icon><Right /></el-icon>
            <el-select v-model="convertTarget" style="width: 160px">
              <el-option value="PEM" :label="t('common.pem')" />
              <el-option value="DER" :label="t('common.der')" />
            </el-select>
            <el-button type="primary" :loading="loading" :icon="'Refresh'" @click="handleConvert">
              {{ t('common.convert') }}
            </el-button>
          </div>

          <ResultPanel :error="error" />

          <template v-if="convertResult">
            <el-divider content-position="left">{{ t('key.convertedKey') }}</el-divider>
            <div class="result-head">
              <el-tag size="small">{{ convertResult.format }}</el-tag>
              <el-button size="small" :icon="'Download'" @click="downloadResult(convertResult, 'key')">
                {{ t('common.download') }}
              </el-button>
            </div>
            <CopyableText :content="convertResult.convertedKey" :rows="10" />
          </template>
        </el-tab-pane>

        <!-- Tab 2: PKCS#1 ↔ PKCS#8 -->
        <el-tab-pane :label="t('key.tabPkcsConvert')" name="pkcs">
          <div class="toolbar">
            <SampleFillButton @fill="fillPkcsSample" />
            <span class="hint">{{ t('key.passwordHint') }}</span>
          </div>
          <KeyTextArea v-model="pkcsKeyData" :placeholder="t('common.keyData')" />
          <div class="format-row">
            <el-select v-model="pkcsSource" style="width: 160px">
              <el-option value="PKCS1" :label="t('key.pkcs1')" />
              <el-option value="PKCS8" :label="t('key.pkcs8')" />
            </el-select>
            <el-icon><Right /></el-icon>
            <el-select v-model="pkcsTarget" style="width: 160px">
              <el-option value="PKCS1" :label="t('key.pkcs1')" />
              <el-option value="PKCS8" :label="t('key.pkcs8')" />
            </el-select>
            <el-button type="primary" :loading="loading" :icon="'Refresh'" @click="handlePkcsConvert">
              {{ t('common.convert') }}
            </el-button>
          </div>
          <el-input
            v-if="pkcsTarget === 'PKCS8'"
            v-model="pkcsPassword"
            type="password"
            show-password
            :placeholder="t('common.password')"
            style="max-width: 320px; margin-top: 12px"
          />

          <ResultPanel :error="error" />

          <template v-if="pkcsResult">
            <el-divider content-position="left">{{ t('key.convertedKey') }}</el-divider>
            <div class="result-head">
              <el-tag size="small">{{ pkcsResult.format }}</el-tag>
              <el-button size="small" :icon="'Download'" @click="downloadResult(pkcsResult, 'key')">
                {{ t('common.download') }}
              </el-button>
            </div>
            <CopyableText :content="pkcsResult.convertedKey" :rows="10" />
          </template>
        </el-tab-pane>

        <!-- Tab 3: 私钥加密 / 解密 -->
        <el-tab-pane :label="t('key.tabEncryptDecrypt')" name="cipher">
          <el-row :gutter="24">
            <el-col :md="12">
              <h4 class="sub-title">{{ t('common.encrypt') }}</h4>
              <div class="toolbar">
                <SampleFillButton @fill="fillEncryptSample" />
              </div>
              <KeyTextArea v-model="encPrivateKey" :placeholder="t('common.privateKey')" :rows="6" />
              <div class="format-row">
                <el-input
                  v-model="encPassword"
                  type="password"
                  show-password
                  :placeholder="t('common.password')"
                  style="width: 200px"
                />
                <el-select v-model="encAlgorithm" style="width: 180px">
                  <el-option v-for="c in PRIVATE_KEY_CIPHERS" :key="c" :value="c" :label="c" />
                </el-select>
                <el-button type="primary" :loading="loading" :icon="'Lock'" @click="handleEncrypt">
                  {{ t('common.encrypt') }}
                </el-button>
              </div>
              <template v-if="encResult">
                <el-divider content-position="left">{{ t('key.encryptResult') }}</el-divider>
                <div class="result-head">
                  <el-tag size="small" type="warning">{{ encResult.format }}</el-tag>
                  <el-button size="small" :icon="'Download'" @click="downloadResult(encResult, 'private-key')">
                    {{ t('common.download') }}
                  </el-button>
                </div>
                <CopyableText :content="encResult.convertedKey" :rows="8" />
              </template>
            </el-col>

            <el-col :md="12">
              <h4 class="sub-title">{{ t('common.decrypt') }}</h4>
              <KeyTextArea v-model="decEncryptedKey" :placeholder="t('key.encryptedPrivateKey')" :rows="6" />
              <div class="format-row">
                <el-input
                  v-model="decPassword"
                  type="password"
                  show-password
                  :placeholder="t('common.password')"
                  style="width: 200px"
                />
                <el-button type="primary" :loading="loading" :icon="'Unlock'" @click="handleDecrypt">
                  {{ t('common.decrypt') }}
                </el-button>
              </div>
              <template v-if="decResult">
                <el-divider content-position="left">{{ t('key.decryptResult') }}</el-divider>
                <div class="result-head">
                  <el-tag size="small" type="success">{{ decResult.format }}</el-tag>
                  <el-button size="small" :icon="'Download'" @click="downloadResult(decResult, 'private-key')">
                    {{ t('common.download') }}
                  </el-button>
                </div>
                <CopyableText :content="decResult.convertedKey" :rows="8" />
              </template>
            </el-col>
          </el-row>
          <ResultPanel :error="error" />
        </el-tab-pane>
      </el-tabs>
    </el-card>
  </div>
</template>

<style scoped>
.page {
  max-width: 960px;
  margin: 0 auto;
}
.toolbar {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 12px;
}
.hint {
  font-size: 12px;
  color: var(--el-text-color-secondary);
}
.format-row {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-top: 12px;
}
.result-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 6px;
}
.sub-title {
  margin: 0 0 12px;
  font-size: 14px;
  color: var(--el-text-color-primary);
}
</style>
