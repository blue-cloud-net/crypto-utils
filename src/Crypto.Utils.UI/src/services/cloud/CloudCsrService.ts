import { post } from '@/api/client/CloudApiClient'
import type {
  CsrGenerateOptions,
  CsrResult,
  CsrParseOptions,
  CsrInfoResult,
} from '@/models/csr'
import type { ICsrService } from '@/services/interfaces/ICsrService'

/** 云端 CSR 服务：调用后端 /api/csr/* */
export class CloudCsrService implements ICsrService {
  generateCsr(options: CsrGenerateOptions): Promise<CsrResult> {
    return post<CsrResult>('/api/csr/generate', options)
  }

  parseCsr(options: CsrParseOptions): Promise<CsrInfoResult> {
    return post<CsrInfoResult>('/api/csr/parse', options)
  }
}
