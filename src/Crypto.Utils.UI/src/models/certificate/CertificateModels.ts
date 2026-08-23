/**
 * 证书模块模型
 * 与后端 Models/Responses/CertificateResponses.cs 及 api-reference.md 对应
 */
import type { SubjectInfo } from '@/models/common/SubjectInfo'
import type { KeyInfoResult } from '@/models/key/KeyModels'

/** 解析证书请求参数 */
export interface CertificateParseOptions {
  certificateData: string
}

/** AIA（权威信息访问） */
export interface AuthorityInformationAccess {
  ocspUrls?: string[] | null
  caIssuers?: string[] | null
}

/** 证书解析结果（/api/cert/parse） */
export interface CertificateInfoResult {
  version: number
  serialNumber: string
  subject: string
  issuer: string
  /** UTC ISO 字符串 */
  notBefore: string
  /** UTC ISO 字符串 */
  notAfter: string
  /** 有效期天数 */
  duration: number
  /** 当前是否有效 */
  isValid: boolean
  /** 剩余有效天数 */
  remainingDays: number
  signatureAlgorithmOid: string
  signatureAlgorithmName: string
  fingerprints: Record<string, string>
  publicKey: KeyInfoResult
  isCA: boolean
  pathLengthConstraint?: number | null
  subjectKeyIdentifier?: string | null
  authorityKeyIdentifier?: string | null
  keyUsage?: string[] | null
  extendedKeyUsage?: string[] | null
  subjectAlternativeNames?: string[] | null
  certificatePolicies?: string[] | null
  crlDistributionPoints?: string[] | null
  authorityInformationAccess?: AuthorityInformationAccess | null
  extensions?: Record<string, unknown> | null
}

/** 生成自签名证书请求参数（首期最简：CN + 有效期 + 私钥 + 签名算法） */
export interface SelfSignedOptions {
  subject: SubjectInfo
  privateKey: string
  /** UTC ISO 字符串（可空，默认当前时间） */
  validFrom?: string | null
  /** UTC ISO 字符串（必填） */
  validTo: string
  signatureAlgorithm: string
  outputFormat: string
}

/** 证书生成/转换结果 */
export interface CertificateResult {
  certificateData: string
  format: string
  privateKey?: string | null
}

/** 支持的签名算法 */
export const SIGNATURE_ALGORITHMS = [
  'SHA256WITHRSA',
  'SHA384WITHRSA',
  'SHA512WITHRSA',
  'SHA256WITHECDSA',
  'SHA384WITHECDSA',
  'SHA512WITHECDSA',
  'SM3WITHSM2',
  'SHA256WITHDSA',
] as const
