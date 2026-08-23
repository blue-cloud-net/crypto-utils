import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { Mode } from '@/services/ServiceFactory'

const STORAGE_KEY = 'crypto-utils-mode'

/** 应用全局状态：当前处理模式（cloud/browser） */
export const useAppStore = defineStore('app', () => {
  const mode = ref<Mode>('cloud')

  /** 从 localStorage 恢复模式 */
  function init(): void {
    const saved = localStorage.getItem(STORAGE_KEY)
    if (saved === 'cloud' || saved === 'browser') {
      mode.value = saved
    }
  }

  /** 切换模式并持久化 */
  function setMode(next: Mode): void {
    mode.value = next
    localStorage.setItem(STORAGE_KEY, next)
  }

  return { mode, init, setMode }
})
