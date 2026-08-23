/**
 * CSR 模块模型
 * 与后端 Models/Responses/CsrResponses.cs 及 api-reference.md 对应
 */
import type { SubjectInfo } from '@/models/common/SubjectInfo'
import type { KeyInfoResult } from '@/models/key/KeyModels'

/** 生成 CSR 请求参数 */
export interface CsrGenerateOptions {
  subject: SubjectInfo
  privateKey: string
  signatureAlgorithm: string
  outputFormat: string
}

/** CSR 生成结果 */
export interface CsrResult {
  csrData: string
  format: string
}

/** 解析 CSR 请求参数 */
export interface CsrParseOptions {
  csrData: string
}

/** CSR 解析结果（/api/csr/parse） */
export interface CsrInfoResult {
  subject: string
  signatureAlgorithmName: string
  publicKey: KeyInfoResult
  extensions?: Record<string, unknown> | null
}
