import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import AppLayout from '@/layouts/AppLayout.vue'
import { i18n } from '@/locales'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: AppLayout,
    children: [
      {
        path: '',
        name: 'home',
        component: () => import('@/views/HomeView.vue'),
        meta: { titleKey: 'home.welcome' },
      },
      // 密钥模块
      {
        path: 'key/generate',
        name: 'key-generate',
        component: () => import('@/views/key/KeyGenerateView.vue'),
        meta: { titleKey: 'key.generateTitle' },
      },
      {
        path: 'key/parse',
        name: 'key-parse',
        component: () => import('@/views/key/KeyParseView.vue'),
        meta: { titleKey: 'key.parseTitle' },
      },
      {
        path: 'key/convert',
        name: 'key-convert',
        component: () => import('@/views/key/KeyConvertView.vue'),
        meta: { titleKey: 'key.convertTitle' },
      },
      // 证书模块
      {
        path: 'cert/parse',
        name: 'cert-parse',
        component: () => import('@/views/cert/CertParseView.vue'),
        meta: { titleKey: 'cert.parseTitle' },
      },
      {
        path: 'cert/self-signed',
        name: 'cert-self-signed',
        component: () => import('@/views/cert/CertSelfSignedView.vue'),
        meta: { titleKey: 'cert.selfSignedTitle' },
      },
      // CSR 模块
      {
        path: 'csr/generate',
        name: 'csr-generate',
        component: () => import('@/views/csr/CsrGenerateView.vue'),
        meta: { titleKey: 'csr.generateTitle' },
      },
      {
        path: 'csr/parse',
        name: 'csr-parse',
        component: () => import('@/views/csr/CsrParseView.vue'),
        meta: { titleKey: 'csr.parseTitle' },
      },
    ],
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.afterEach((to) => {
  const titleKey = to.meta.titleKey as string | undefined
  const title = titleKey ? i18n.global.t(titleKey) : 'Crypto Utils'
  document.title = title ? `${title} · Crypto Utils` : 'Crypto Utils'
})

export default router
