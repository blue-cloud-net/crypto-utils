import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

const STORAGE_KEY = 'crypto-utils-theme'

/** 主题模式：system=跟随系统；light=浅色；dark=深色 */
export type ThemeMode = 'system' | 'light' | 'dark'

const prefersDarkQuery = window.matchMedia('(prefers-color-scheme: dark)')

/** 主题状态：三态切换，默认跟随系统，localStorage 持久化 */
export const useThemeStore = defineStore('theme', () => {
  const theme = ref<ThemeMode>('system')
  /** 系统当前是否为深色（system 模式下生效） */
  const systemDark = ref(prefersDarkQuery.matches)

  /** 是否应用深色主题 */
  const isDark = computed(
    () => theme.value === 'dark' || (theme.value === 'system' && systemDark.value),
  )

  /** 将当前状态应用到 <html> 的 dark class（Element Plus 暗色模式依赖） */
  function apply(): void {
    document.documentElement.classList.toggle('dark', isDark.value)
  }

  /** 初始化：恢复持久化值 + 监听系统主题变化 */
  function init(): void {
    const saved = localStorage.getItem(STORAGE_KEY)
    if (saved === 'system' || saved === 'light' || saved === 'dark') {
      theme.value = saved
    }
    prefersDarkQuery.addEventListener('change', (event) => {
      systemDark.value = event.matches
      apply()
    })
    apply()
  }

  /** 切换主题并持久化 */
  function setTheme(next: ThemeMode): void {
    theme.value = next
    localStorage.setItem(STORAGE_KEY, next)
    apply()
  }

  return { theme, isDark, init, setTheme }
})
