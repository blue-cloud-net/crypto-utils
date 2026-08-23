<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAppStore } from '@/stores/useAppStore'
import { useThemeStore } from '@/stores/useThemeStore'
import { useLocaleStore } from '@/stores/useLocaleStore'
import type { ThemeMode } from '@/stores/useThemeStore'
import type { Locale } from '@/stores/useLocaleStore'
import type { Mode } from '@/services/ServiceFactory'

const { t } = useI18n()
const route = useRoute()
const router = useRouter()
const appStore = useAppStore()
const themeStore = useThemeStore()
const localeStore = useLocaleStore()

const collapsed = ref(false)
const activeMenu = computed(() => route.path)

interface MenuItem {
  path: string
  label: string
  icon: string
}

const menuGroups: { label: string; items: MenuItem[] }[] = [
  {
    label: 'nav.groupKey',
    items: [
      { path: '/key/generate', label: 'nav.keyGenerate', icon: 'Key' },
      { path: '/key/parse', label: 'nav.keyParse', icon: 'Search' },
      { path: '/key/convert', label: 'nav.keyConvert', icon: 'Refresh' },
    ],
  },
  {
    label: 'nav.groupCert',
    items: [
      { path: '/cert/parse', label: 'nav.certParse', icon: 'Document' },
      { path: '/cert/self-signed', label: 'nav.certSelfSigned', icon: 'CircleCheck' },
    ],
  },
  {
    label: 'nav.groupCsr',
    items: [
      { path: '/csr/generate', label: 'nav.csrGenerate', icon: 'EditPen' },
      { path: '/csr/parse', label: 'nav.csrParse', icon: 'View' },
    ],
  },
]

function go(path: string): void {
  router.push(path)
}

function handleMode(value: string | number | boolean): void {
  appStore.setMode(value as Mode)
}

function handleTheme(value: string | number | boolean): void {
  themeStore.setTheme(value as ThemeMode)
}

function handleLocale(value: string | number | boolean): void {
  localeStore.setLocale(value as Locale)
}
</script>

<template>
  <el-container class="app-layout">
    <el-aside :width="collapsed ? '64px' : '220px'" class="app-aside">
      <div class="logo" @click="go('/')">
        <el-icon :size="22" class="logo-icon"><Lock /></el-icon>
        <span v-if="!collapsed" class="logo-text">{{ t('common.appName') }}</span>
      </div>
      <el-menu :default-active="activeMenu" :collapse="collapsed" :collapse-transition="false" @select="go">
        <el-menu-item-group v-for="group in menuGroups" :key="group.label" :title="collapsed ? '' : t(group.label)">
          <el-menu-item v-for="item in group.items" :key="item.path" :index="item.path">
            <el-icon><component :is="item.icon" /></el-icon>
            <template #title>{{ t(item.label) }}</template>
          </el-menu-item>
        </el-menu-item-group>
      </el-menu>
    </el-aside>

    <el-container class="app-body">
      <el-header class="app-header" height="52px">
        <div class="header-left">
          <el-icon class="collapse-btn" :size="18" @click="collapsed = !collapsed">
            <Expand v-if="collapsed" />
            <Fold v-else />
          </el-icon>
          <span class="header-title">{{ t((route.meta.titleKey as string) ?? 'home.welcome') }}</span>
        </div>
        <div class="header-right">
          <el-radio-group :model-value="appStore.mode" size="small" @change="handleMode">
            <el-radio-button value="cloud">{{ t('mode.cloud') }}</el-radio-button>
            <el-radio-button value="browser">{{ t('mode.browser') }}</el-radio-button>
          </el-radio-group>

          <el-dropdown trigger="click" @command="handleTheme">
            <span class="header-action">
              <el-icon><Moon v-if="themeStore.isDark" /><Sunny v-else /></el-icon>
              <span>{{ t(`theme.${themeStore.theme}`) }}</span>
            </span>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item command="system">{{ t('theme.system') }}</el-dropdown-item>
                <el-dropdown-item command="light">{{ t('theme.light') }}</el-dropdown-item>
                <el-dropdown-item command="dark">{{ t('theme.dark') }}</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>

          <el-dropdown trigger="click" @command="handleLocale">
            <span class="header-action">
              <el-icon><ChatDotRound /></el-icon>
              <span>{{ localeStore.locale === 'zh-CN' ? '中' : 'EN' }}</span>
            </span>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item command="zh-CN">简体中文</el-dropdown-item>
                <el-dropdown-item command="en-US">English</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
      </el-header>

      <el-main class="app-main">
        <router-view />
      </el-main>
    </el-container>
  </el-container>
</template>

<style scoped>
.app-layout {
  height: 100%;
}
.app-aside {
  border-right: 1px solid var(--el-border-color-light);
  transition: width 0.2s;
  overflow: hidden;
}
.logo {
  display: flex;
  align-items: center;
  gap: 8px;
  height: 52px;
  padding: 0 16px;
  cursor: pointer;
  color: var(--el-color-primary);
  font-weight: 600;
  white-space: nowrap;
}
.logo-text {
  font-size: 16px;
}
.app-body {
  min-width: 0;
}
.app-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-bottom: 1px solid var(--el-border-color-light);
}
.header-left {
  display: flex;
  align-items: center;
  gap: 12px;
}
.collapse-btn {
  cursor: pointer;
  color: var(--el-text-color-secondary);
}
.header-title {
  font-size: 15px;
  font-weight: 600;
}
.header-right {
  display: flex;
  align-items: center;
  gap: 16px;
}
.header-action {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  cursor: pointer;
  color: var(--el-text-color-regular);
  font-size: 13px;
  outline: none;
}
.app-main {
  padding: 20px;
  overflow-y: auto;
}
</style>
