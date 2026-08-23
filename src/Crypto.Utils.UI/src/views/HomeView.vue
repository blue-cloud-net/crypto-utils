<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'

const { t } = useI18n()
const router = useRouter()

interface ModuleCard {
  key: string
  titleKey: string
  descKey: string
  icon: string
  color: string
  routes: { path: string; labelKey: string }[]
}

const modules: ModuleCard[] = [
  {
    key: 'key',
    titleKey: 'home.moduleKey',
    descKey: 'home.moduleKeyDesc',
    icon: 'Key',
    color: '#409eff',
    routes: [
      { path: '/key/generate', labelKey: 'nav.keyGenerate' },
      { path: '/key/parse', labelKey: 'nav.keyParse' },
      { path: '/key/convert', labelKey: 'nav.keyConvert' },
    ],
  },
  {
    key: 'cert',
    titleKey: 'home.moduleCert',
    descKey: 'home.moduleCertDesc',
    icon: 'Document',
    color: '#67c23a',
    routes: [
      { path: '/cert/parse', labelKey: 'nav.certParse' },
      { path: '/cert/self-signed', labelKey: 'nav.certSelfSigned' },
    ],
  },
  {
    key: 'csr',
    titleKey: 'home.moduleCsr',
    descKey: 'home.moduleCsrDesc',
    icon: 'EditPen',
    color: '#e6a23c',
    routes: [
      { path: '/csr/generate', labelKey: 'nav.csrGenerate' },
      { path: '/csr/parse', labelKey: 'nav.csrParse' },
    ],
  },
]

function go(path: string): void {
  router.push(path)
}
</script>

<template>
  <div class="home-view">
    <div class="hero">
      <h1 class="hero-title">{{ t('home.welcome') }}</h1>
      <p class="hero-desc">{{ t('home.description') }}</p>
    </div>

    <h2 class="section-title">{{ t('home.modulesTitle') }}</h2>

    <el-row :gutter="20" class="module-grid">
      <el-col v-for="mod in modules" :key="mod.key" :xs="24" :sm="12" :lg="8">
        <el-card class="module-card" shadow="hover">
          <div class="module-header">
            <el-icon :size="28" :style="{ color: mod.color }">
              <component :is="mod.icon" />
            </el-icon>
            <span class="module-title">{{ t(mod.titleKey) }}</span>
          </div>
          <p class="module-desc">{{ t(mod.descKey) }}</p>
          <el-divider />
          <div class="module-routes">
            <el-button
              v-for="r in mod.routes"
              :key="r.path"
              size="small"
              plain
              @click="go(r.path)"
            >
              {{ t(r.labelKey) }} →
            </el-button>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<style scoped>
.home-view {
  max-width: 1080px;
  margin: 0 auto;
}
.hero {
  text-align: center;
  padding: 40px 0 24px;
}
.hero-title {
  font-size: 30px;
  margin: 0 0 12px;
  color: var(--el-text-color-primary);
}
.hero-desc {
  font-size: 14px;
  color: var(--el-text-color-secondary);
  max-width: 640px;
  margin: 0 auto;
  line-height: 1.8;
}
.section-title {
  font-size: 18px;
  margin: 8px 0 16px;
  color: var(--el-text-color-primary);
}
.module-card {
  height: 100%;
}
.module-header {
  display: flex;
  align-items: center;
  gap: 12px;
}
.module-title {
  font-size: 17px;
  font-weight: 600;
  color: var(--el-text-color-primary);
}
.module-desc {
  font-size: 13px;
  color: var(--el-text-color-secondary);
  margin: 12px 0 0;
  min-height: 40px;
  line-height: 1.6;
}
.module-routes {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}
</style>
