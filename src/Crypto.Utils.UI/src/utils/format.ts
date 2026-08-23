/**
 * 格式化工具
 */

/** 将 UTC ISO 字符串格式化为本地时间（YYYY-MM-DD HH:mm） */
export function formatDateTime(iso: string | null | undefined): string {
  if (!iso) {
    return '-'
  }
  const date = new Date(iso)
  if (Number.isNaN(date.getTime())) {
    return iso
  }
  const pad = (n: number): string => String(n).padStart(2, '0')
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())} ${pad(date.getHours())}:${pad(date.getMinutes())}`
}

/** 天数：保留 1 位小数，负数显示为 0 */
export function formatDays(days: number | null | undefined): string {
  if (days === null || days === undefined) {
    return '-'
  }
  return days.toFixed(1)
}

/** 指纹值按冒号分隔显示 */
export function formatFingerprint(value: string): string {
  return value
}

/** 根据 PEM 文本猜测输出扩展名（含换行折叠的 PEM） */
export function extensionFromPem(content: string): string {
  if (content.includes('ENCRYPTED PRIVATE KEY')) return 'pem'
  if (content.includes('CERTIFICATE')) return 'crt'
  if (content.includes('REQUEST')) return 'csr'
  if (content.includes('PRIVATE KEY')) return 'key'
  return 'pem'
}
