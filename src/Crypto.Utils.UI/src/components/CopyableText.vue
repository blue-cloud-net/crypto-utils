<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'

const props = defineProps<{
  content: string
  label?: string
  rows?: number
}>()

const { t } = useI18n()
const copied = ref(false)

async function handleCopy(): Promise<void> {
  await navigator.clipboard.writeText(props.content)
  copied.value = true
  setTimeout(() => {
    copied.value = false
  }, 1500)
}
</script>

<template>
  <div class="copyable-text">
    <div v-if="label" class="copyable-label">{{ label }}</div>
    <el-input
      type="textarea"
      :model-value="content"
      :rows="rows ?? 8"
      readonly
      resize="none"
      class="pem-input"
    />
    <div class="copyable-actions">
      <el-button size="small" type="primary" plain :icon="copied ? 'Check' : 'CopyDocument'" @click="handleCopy">
        {{ copied ? t('common.copied') : t('common.copy') }}
      </el-button>
    </div>
  </div>
</template>

<style scoped>
.copyable-label {
  font-size: 13px;
  color: var(--el-text-color-secondary);
  margin-bottom: 6px;
}
.copyable-actions {
  margin-top: 6px;
}
</style>
