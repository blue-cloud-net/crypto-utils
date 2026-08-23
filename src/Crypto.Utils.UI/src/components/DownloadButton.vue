<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { downloadText } from '@/utils/download'

const props = defineProps<{
  content: string
  filename: string
  mime?: string
}>()

const { t } = useI18n()

function handleDownload(): void {
  downloadText(props.content, props.filename, props.mime)
}
</script>

<template>
  <el-button size="small" :icon="'Download'" @click="handleDownload">{{ t('common.download') }}</el-button>
</template>
