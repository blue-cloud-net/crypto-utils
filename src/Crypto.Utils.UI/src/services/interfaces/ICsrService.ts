import type {
  CsrGenerateOptions,
  CsrResult,
  CsrParseOptions,
  CsrInfoResult,
} from '@/models/csr'

/** CSR 服务接口 */
export interface ICsrService {
  /** 生成 CSR */
  generateCsr(options: CsrGenerateOptions): Promise<CsrResult>

  /** 解析 CSR */
  parseCsr(options: CsrParseOptions): Promise<CsrInfoResult>
}
