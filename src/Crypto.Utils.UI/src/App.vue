<script setup lang="ts">
import { computed, onMounted, watch } from 'vue'
import { ElConfigProvider } from 'element-plus'
import zhCn from 'element-plus/es/locale/lang/zh-cn'
import en from 'element-plus/es/locale/lang/en'
import { i18n } from '@/locales'
import { useAppStore } from '@/stores/useAppStore'
import { useThemeStore } from '@/stores/useThemeStore'
import { useLocaleStore } from '@/stores/useLocaleStore'

const appStore = useAppStore()
const themeStore = useThemeStore()
const localeStore = useLocaleStore()

// Element Plus 内置文案语言包
const elementLocale = computed(() => (localeStore.locale === 'zh-CN' ? zhCn : en))

onMounted(() => {
  appStore.init()
  localeStore.init()
  themeStore.init()
  i18n.global.locale.value = localeStore.locale
})

// 语言切换时同步 vue-i18n
watch(
  () => localeStore.locale,
  (value) => {
    i18n.global.locale.value = value
  },
)
</script>

<template>
  <el-config-provider :locale="elementLocale">
    <router-view />
  </el-config-provider>
</template>
