/**
 * 文件下载工具
 */

/** 按 MIME 触发浏览器下载 */
export function downloadText(content: string, filename: string, mime = 'application/x-pem-file'): void {
  const blob = new Blob([content], { type: mime })
  triggerDownload(blob, filename)
}

/** 将 Base64 文本解码为二进制并下载（用于 DER 等二进制格式） */
export function downloadBase64(base64: string, filename: string, mime = 'application/octet-stream'): void {
  const binary = atob(base64.replace(/\s+/g, ''))
  const bytes = new Uint8Array(binary.length)
  for (let i = 0; i < binary.length; i++) {
    bytes[i] = binary.charCodeAt(i)
  }
  triggerDownload(new Blob([bytes], { type: mime }), filename)
}

function triggerDownload(blob: Blob, filename: string): void {
  const url = URL.createObjectURL(blob)
  const anchor = document.createElement('a')
  anchor.href = url
  anchor.download = filename
  document.body.appendChild(anchor)
  anchor.click()
  document.body.removeChild(anchor)
  URL.revokeObjectURL(url)
}

/** 生成下载文件名：{类型}-{标识}-{时间戳}.{扩展名} */
export function buildFilename(type: string, identifier: string, extension: string): string {
  const safeId = identifier.replace(/[^\w.-]+/g, '-').slice(0, 40) || 'unknown'
  const timestamp = new Date().toISOString().replace(/[:.]/g, '-').slice(0, 19)
  return `${type}-${safeId}-${timestamp}.${extension}`
}
