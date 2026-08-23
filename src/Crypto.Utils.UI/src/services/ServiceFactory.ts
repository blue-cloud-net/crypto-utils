import { CloudKeyService } from '@/services/cloud/CloudKeyService'
import { CloudCertificateService } from '@/services/cloud/CloudCertificateService'
import { CloudCsrService } from '@/services/cloud/CloudCsrService'
import type { IKeyService } from '@/services/interfaces/IKeyService'
import type { ICertificateService } from '@/services/interfaces/ICertificateService'
import type { ICsrService } from '@/services/interfaces/ICsrService'

/** 处理模式：cloud=后端运算；browser=浏览器本地运算（二期实现） */
export type Mode = 'cloud' | 'browser'

/** 浏览器模式未实现时抛出的错误 */
export class BrowserNotImplementedError extends Error {
  constructor(service: string) {
    super(`当前功能仅支持云端（${service} 浏览器模式尚未实现）`)
    this.name = 'BrowserNotImplementedError'
  }
}

/**
 * 生成浏览器模式的占位实现：任何方法调用都抛 BrowserNotImplementedError
 * 二期以 Web Crypto + node-forge 替换为真实实现
 */
function browserStub<T extends object>(service: string): T {
  return new Proxy({} as T, {
    get() {
      return () => {
        throw new BrowserNotImplementedError(service)
      }
    },
  })
}

/** 服务工厂：按当前模式返回对应的服务实现 */
export class ServiceFactory {
  static getKeyService(mode: Mode): IKeyService {
    return mode === 'cloud' ? new CloudKeyService() : browserStub<IKeyService>('IKeyService')
  }

  static getCertificateService(mode: Mode): ICertificateService {
    return mode === 'cloud' ? new CloudCertificateService() : browserStub<ICertificateService>('ICertificateService')
  }

  static getCsrService(mode: Mode): ICsrService {
    return mode === 'cloud' ? new CloudCsrService() : browserStub<ICsrService>('ICsrService')
  }
}
