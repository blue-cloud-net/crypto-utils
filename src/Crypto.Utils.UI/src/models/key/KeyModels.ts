/**
 * 密钥模块模型
 * 与后端 Models/Responses/KeyPairResponses.cs 及 api-reference.md 对应
 */

/** 生成密钥对请求参数 */
export interface KeyPairGenerateOptions {
  /** 算法：RSA / EC / SM2 / DSA */
  algorithm: string
  /** 密钥大小（RSA: 2048/3072/4096；DSA: 2048/3072） */
  keySize?: number | null
  /** EC 曲线名称 */
  curveName?: string | null
  /** 输出格式：PEM / DER */
  outputFormat: string
}

/** 生成密钥对结果 */
export interface KeyPairResult {
  publicKey: string
  privateKey: string
  algorithm: string
  keySize?: number | null
  curveName?: string | null
}

/** 密钥信息结果（/api/key/parse） */
export interface KeyInfoResult {
  algorithmName: string
  isPrivate: boolean
  isPublic: boolean
  keySize?: number | null
  curveOid?: string | null
  curveName?: string | null
  /** PEM 格式密钥数据 */
  keyData: string
  /** 指纹字典（算法名 → 十六进制值） */
  fingerprints: Record<string, string>
  /** 密钥参数（RSA: N/E；EC: X/Y 等） */
  parameters?: Record<string, string> | null
}

/** PEM ↔ DER 格式转换请求参数 */
export interface KeyConvertOptions {
  keyData: string
  sourceFormat: string
  targetFormat: string
}

/** PKCS#1 ↔ PKCS#8 格式转换请求参数 */
export interface PkcsConvertOptions {
  keyData: string
  sourceFormat: string
  targetFormat: string
  /** 加密密码（目标为加密 PKCS#8 时使用） */
  password?: string | null
}

/** 私钥加密请求参数 */
export interface KeyEncryptOptions {
  privateKey: string
  password: string
  /** 加密算法（默认 AES-256-CBC） */
  algorithm: string
}

/** 私钥解密请求参数 */
export interface KeyDecryptOptions {
  encryptedPrivateKey: string
  password: string
}

/** 密钥转换/加密/解密结果 */
export interface KeyConvertResult {
  convertedKey: string
  format: string
}

/** RSA / DSA 可选的密钥大小 */
export const RSA_KEY_SIZES = [2048, 3072, 4096] as const

/** DSA 可选的密钥大小 */
export const DSA_KEY_SIZES = [2048, 3072] as const

/** 支持的非对称算法 */
export const KEY_ALGORITHMS = ['RSA', 'EC', 'SM2', 'DSA'] as const

/** 支持的 EC 曲线（sm2p256v1 为 SM2） */
export const EC_CURVES = [
  { label: 'secp256r1 (P-256)', value: 'secp256r1' },
  { label: 'secp384r1 (P-384)', value: 'secp384r1' },
  { label: 'secp521r1 (P-521)', value: 'secp521r1' },
  { label: 'sm2p256v1 (SM2)', value: 'sm2p256v1' },
] as const

/** 私钥加密算法 */
export const PRIVATE_KEY_CIPHERS = ['AES-256-CBC', 'AES-128-CBC', 'AES-192-CBC', 'DES-EDE3-CBC'] as const
