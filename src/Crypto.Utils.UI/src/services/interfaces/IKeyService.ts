import type {
  KeyPairGenerateOptions,
  KeyPairResult,
  KeyInfoResult,
  KeyConvertOptions,
  PkcsConvertOptions,
  KeyEncryptOptions,
  KeyDecryptOptions,
  KeyConvertResult,
} from '@/models/key'

/**
 * 密钥服务接口
 * 所有方法返回 Promise<T>（不含 ApiResponse 包装层）
 */
export interface IKeyService {
  /** 生成密钥对 */
  generateKeyPair(options: KeyPairGenerateOptions): Promise<KeyPairResult>

  /** 解析密钥信息 */
  parseKey(keyData: string): Promise<KeyInfoResult>

  /** PEM ↔ DER 格式转换 */
  convertKeyFormat(options: KeyConvertOptions): Promise<KeyConvertResult>

  /** PKCS#1 ↔ PKCS#8 格式转换 */
  convertPkcsFormat(options: PkcsConvertOptions): Promise<KeyConvertResult>

  /** 加密私钥（传统加密 PEM） */
  encryptPrivateKey(options: KeyEncryptOptions): Promise<KeyConvertResult>

  /** 解密私钥 */
  decryptPrivateKey(options: KeyDecryptOptions): Promise<KeyConvertResult>
}
