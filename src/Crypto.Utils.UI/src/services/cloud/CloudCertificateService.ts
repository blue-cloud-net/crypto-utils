import { post } from '@/api/client/CloudApiClient'
import type {
  CertificateParseOptions,
  CertificateInfoResult,
  SelfSignedOptions,
  CertificateResult,
} from '@/models/certificate'
import type { ICertificateService } from '@/services/interfaces/ICertificateService'

/** 云端证书服务：调用后端 /api/cert/* */
export class CloudCertificateService implements ICertificateService {
  parseCertificate(options: CertificateParseOptions): Promise<CertificateInfoResult> {
    return post<CertificateInfoResult>('/api/cert/parse', options)
  }

  generateSelfSigned(options: SelfSignedOptions): Promise<CertificateResult> {
    return post<CertificateResult>('/api/cert/self-signed', options)
  }
}
