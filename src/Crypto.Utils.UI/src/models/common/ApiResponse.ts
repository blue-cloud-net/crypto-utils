/**
 * 后端统一响应封装 ApiResponse<T>
 * 与后端 Models/ApiResponse.cs 对应（JSON camelCase）
 */
export interface ApiResponse<T = unknown> {
  /** 是否成功 */
  success: boolean
  /** 提示消息 */
  message: string
  /** 业务数据（失败时为 null） */
  data: T | null
  /** 错误标识（失败时非空） */
  errorCode: string | null
  /** 时间戳（UTC ISO 字符串） */
  timestamp: string
}
