import { post } from '@/api/client/CloudApiClient'
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
import type { IKeyService } from '@/services/interfaces/IKeyService'

/** 云端密钥服务：调用后端 /api/key/* */
export class CloudKeyService implements IKeyService {
  generateKeyPair(options: KeyPairGenerateOptions): Promise<KeyPairResult> {
    return post<KeyPairResult>('/api/key/generate', options)
  }

  parseKey(keyData: string): Promise<KeyInfoResult> {
    return post<KeyInfoResult>('/api/key/parse', { keyData })
  }

  convertKeyFormat(options: KeyConvertOptions): Promise<KeyConvertResult> {
    return post<KeyConvertResult>('/api/key/convert', options)
  }

  convertPkcsFormat(options: PkcsConvertOptions): Promise<KeyConvertResult> {
    return post<KeyConvertResult>('/api/key/pkcs-convert', options)
  }

  encryptPrivateKey(options: KeyEncryptOptions): Promise<KeyConvertResult> {
    return post<KeyConvertResult>('/api/key/encrypt', options)
  }

  decryptPrivateKey(options: KeyDecryptOptions): Promise<KeyConvertResult> {
    return post<KeyConvertResult>('/api/key/decrypt', options)
  }
}
