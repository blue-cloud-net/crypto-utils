<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useAppStore } from '@/stores/useAppStore'

const appStore = useAppStore()
const { t } = useI18n()

const text = computed(() => (appStore.mode === 'cloud' ? t('mode.badgeCloud') : t('mode.badgeBrowser')))
</script>

<template>
  <el-tag :type="appStore.mode === 'cloud' ? 'primary' : 'warning'" size="small" effect="plain">
    {{ text }}
  </el-tag>
</template>
