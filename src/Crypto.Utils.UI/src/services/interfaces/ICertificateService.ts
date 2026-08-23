import type {
  CertificateParseOptions,
  CertificateInfoResult,
  SelfSignedOptions,
  CertificateResult,
} from '@/models/certificate'

/** 证书服务接口 */
export interface ICertificateService {
  /** 解析证书 */
  parseCertificate(options: CertificateParseOptions): Promise<CertificateInfoResult>

  /** 生成自签名证书（最简表单） */
  generateSelfSigned(options: SelfSignedOptions): Promise<CertificateResult>
}
