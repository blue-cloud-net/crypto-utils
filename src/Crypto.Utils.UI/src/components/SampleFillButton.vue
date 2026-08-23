<script setup lang="ts">
import { useI18n } from 'vue-i18n'

defineProps<{
  disabled?: boolean
}>()

const emit = defineEmits<{ (e: 'fill'): void }>()
const { t } = useI18n()
</script>

<template>
  <el-button size="small" :icon="'MagicStick'" :disabled="disabled" @click="emit('fill')">
    {{ t('common.fillSample') }}
  </el-button>
</template>
