<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { useAppStore } from '@/stores/useAppStore'

const appStore = useAppStore()
const { t } = useI18n()
</script>

<template>
  <el-alert
    v-if="appStore.mode === 'browser'"
    type="warning"
    :title="t('mode.notice')"
    :closable="false"
    show-icon
    class="mode-notice"
  />
</template>

<style scoped>
.mode-notice {
  margin-bottom: 16px;
}
</style>
