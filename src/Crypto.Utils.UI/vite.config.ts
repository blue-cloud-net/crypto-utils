import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    port: 5178,
    // 开发环境将 API 请求代理到后端（http profile 端口 5072）
    proxy: {
      '/api': {
        target: 'http://localhost:5072',
        changeOrigin: true,
      },
      '/health': {
        target: 'http://localhost:5072',
        changeOrigin: true,
      },
    },
  },
})
