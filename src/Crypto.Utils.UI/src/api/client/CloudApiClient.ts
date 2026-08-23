/**
 * 云端 API 客户端
 * 基于 Axios，统一解包 ApiResponse<T>；失败抛 ApiError（message + errorCode）
 */
import axios, { AxiosError } from 'axios'
import type { ApiResponse } from '@/models/common/ApiResponse'

const http = axios.create({
  // 同源：开发环境由 Vite 代理到后端；生产由 Host 同源服务
  baseURL: '',
  timeout: 120000,
})

/** 统一的 API 业务错误 */
export class ApiError extends Error {
  /** 后端错误码（errorCode），可能为空 */
  errorCode: string | null

  constructor(message: string, errorCode: string | null = null) {
    super(message)
    this.name = 'ApiError'
    this.errorCode = errorCode
  }
}

/**
 * 发起 POST 请求并解包业务数据
 * @throws {ApiError} 业务失败或网络/HTTP 错误
 */
export async function post<T>(url: string, body: unknown): Promise<T> {
  try {
    const response = await http.post<ApiResponse<T>>(url, body)
    const payload = response.data
    if (payload.success && payload.data !== null) {
      return payload.data
    }
    throw new ApiError(payload.message || '操作失败', payload.errorCode)
  } catch (err) {
    if (err instanceof ApiError) {
      throw err
    }
    const axiosErr = err as AxiosError<ApiResponse>
    if (axiosErr.response?.data) {
      const payload = axiosErr.response.data
      throw new ApiError(
        payload.message || `请求失败（HTTP ${axiosErr.response.status}）`,
        payload.errorCode,
      )
    }
    throw new ApiError(axiosErr.message || '网络请求失败')
  }
}

export const CloudApiClient = { post }
