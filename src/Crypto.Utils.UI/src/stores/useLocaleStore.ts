import { defineStore } from 'pinia'
import { ref } from 'vue'

const STORAGE_KEY = 'crypto-utils-locale'

/** 界面语言 */
export type Locale = 'zh-CN' | 'en-US'

/** 语言状态：默认跟随浏览器，localStorage 持久化 */
export const useLocaleStore = defineStore('locale', () => {
  const locale = ref<Locale>('zh-CN')

  /** 初始化：优先恢复持久化值，否则跟随浏览器语言 */
  function init(): void {
    const saved = localStorage.getItem(STORAGE_KEY)
    if (saved === 'zh-CN' || saved === 'en-US') {
      locale.value = saved
      return
    }
    locale.value = navigator.language.toLowerCase().startsWith('zh') ? 'zh-CN' : 'en-US'
  }

  /** 切换语言并持久化 */
  function setLocale(next: Locale): void {
    locale.value = next
    localStorage.setItem(STORAGE_KEY, next)
  }

  return { locale, init, setLocale }
})
