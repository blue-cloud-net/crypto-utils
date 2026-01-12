# 证书工具 UI 项目设计文档

## 项目概述
一个基于 Vue 3 的证书管理和处理工具，专为开发测试环境设计，支持各种证书格式转换和处理操作。

## 技术栈
- **前端框架**: Vue 3 + TypeScript
- **构建工具**: Vite
- **包管理器**: pnpm (高效依赖管理)
- **UI 组件库**: Element Plus
- **CSS 框架**: UnoCSS
- **路由**: Vue Router
- **状态管理**: Pinia
- **国际化**: Vue I18n
- **HTTP 客户端**: Axios + 自定义API封装
- **处理模式**: 双模式架构 (云端处理优先 + 浏览器模式后续)
- **API架构**: 统一接口抽象，支持模式切换

## 核心功能模块

### 1. � 证书模块 (Certificate Module)

#### 🔍 证书解析 (高优先级)
- [ ] 拖拽上传证书文件 (PEM, DER, CRT, CER)
- [ ] 证书详细信息解析展示
- [ ] 证书指纹计算 (MD5, SHA1, SHA256)
- [ ] 证书扩展信息解析
- [ ] 证书有效期和状态检查

#### 🔧 证书生成 (高优先级)
- [ ] **自签名证书生成** - 生成根证书和私钥
- [ ] **公钥 + CA签发证书** - 使用已有公钥和CA证书签发
- [ ] **CSR + CA签发证书** - 使用CSR和CA证书签发
- [ ] **CA证书生成证书和私钥** - 完整的证书签发流程
- [ ] 批量证书生成和模板配置

#### 🔄 证书转换
- [ ] PEM ↔ DER 格式转换
- [ ] 批量格式转换
- [ ] 证书编码转换

#### ✅ 证书验证
- [ ] **证书链验证** - 完整证书链验证
- [ ] **证书与私钥验证** - 验证证书私钥匹配
- [ ] **证书与公钥验证** - 验证证书公钥匹配
- [ ] 证书有效性和用途验证

#### 🔐 PFX 处理
- [ ] **PFX合成/生成** - 证书+私钥打包为PFX
- [ ] **PFX解析/提取** - 从PFX提取证书和私钥
- [ ] PFX密码保护和解密

### 2. 📋 CSR模块 (Certificate Signing Request)

#### 🔧 CSR生成 (高优先级)
- [ ] 交互式CSR生成向导
- [ ] 自定义主题信息 (CN, O, OU, C等)
- [ ] SAN扩展配置 (多域名支持)
- [ ] 密钥算法选择 (RSA, ECDSA)

#### 🔍 CSR解析 (高优先级)
- [ ] CSR文件解析和信息展示
- [ ] CSR签名验证
- [ ] CSR扩展信息解析

#### ✅ CSR验证
- [ ] **CSR与私钥验证** - 验证CSR私钥匹配
- [ ] **CSR与公钥验证** - 验证CSR公钥匹配
- [ ] CSR格式和签名验证

### 3. 🗝️ 密钥模块 (Key Management)

#### 🔧 密钥生成
- [ ] RSA密钥对生成 (1024, 2048, 4096位)
- [ ] ECDSA密钥对生成 (P-256, P-384, P-521)
- [ ] Ed25519密钥对生成

#### 🔄 密钥转换
- [ ] **PKCS#1 ↔ PKCS#8** 格式转换
- [ ] 私钥密码保护和解密
- [ ] 密钥编码格式转换

#### 🔍 密钥解析
- [ ] **私钥解析** - 解析私钥信息并提取公钥
- [ ] **公钥解析** - 解析公钥信息和参数
- [ ] 密钥算法和长度识别

### 4. 🌐 HTTPS模块 (HTTPS Tools)

#### 🔍 HTTPS检测
- [ ] **HTTPS证书检测** - 在线检测网站证书
- [ ] **提取HTTPS证书** - 从HTTPS连接提取证书
- [ ] **补全证书链** - 自动补全缺失的中间证书
- [ ] SSL/TLS协议版本检测
- [ ] 证书到期监控和告警

### 5. 🛠️ 开发工具集 (Developer Tools)
- [ ] JWT Token 生成和验证
- [ ] 各种哈希算法计算 (MD5, SHA1, SHA256, SHA512)
- [ ] Base64/Hex 编解码工具
- [ ] UUID 生成器
- [ ] 时间戳转换工具
- [ ] JSON 格式化和验证

### 6. 🌍 网络工具集 (Network Tools)

#### 🔍 DNS查询
- [ ] A/AAAA记录查询
- [ ] MX记录查询 (邮件服务器)
- [ ] TXT记录查询 (SPF, DKIM等)
- [ ] CNAME/NS记录查询
- [ ] 反向DNS查询

#### 📡 HTTP查询
- [ ] HTTP/HTTPS请求工具
- [ ] 请求头自定义
- [ ] 响应状态和头部分析
- [ ] 请求时间和性能分析

## 页面结构规划

```
/                    # 首页 - 功能导航面板
├── /certificate     # 证书模块
│   ├── /parse       # 证书解析 (高优先级)
│   ├── /generate    # 证书生成 (高优先级)
│   ├── /convert     # 证书转换
│   ├── /verify      # 证书验证
│   └── /pfx         # PFX处理
├── /csr            # CSR模块  
│   ├── /generate    # CSR生成 (高优先级)
│   ├── /parse       # CSR解析 (高优先级)
│   └── /verify      # CSR验证
├── /key            # 密钥模块
│   ├── /generate    # 密钥生成
│   ├── /convert     # 密钥转换
│   └── /parse       # 密钥解析
├── /https          # HTTPS模块
│   ├── /detect      # HTTPS检测
│   ├── /extract     # 证书提取
│   └── /chain       # 证书链补全
├── /devtools       # 开发工具集
│   ├── /jwt         # JWT工具
│   ├── /hash        # 哈希计算
│   ├── /encode      # 编解码
│   └── /uuid        # UUID生成
├── /network        # 网络工具集
│   ├── /dns         # DNS查询
│   └── /http        # HTTP工具
├── /projects       # 项目管理
└── /settings       # 设置页面
```

## 组件架构

### 布局组件
- `AppLayout` - 主应用布局 (头部+主体+底部)
- `AppHeader` - 固定头部，包含右上角菜单
- `AppFooter` - 底部版权信息
- `AppMenu` - 右上角下拉菜单
- `ThemeToggle` - 暗色模式切换
- `LanguageSwitch` - 语言切换器

### 核心业务组件

#### 证书模块组件
- `CertificateParser` - 证书解析主组件
- `CertificateGenerator` - 证书生成向导
- `CertificateConverter` - 证书格式转换
- `CertificateValidator` - 证书验证组件
- `PfxProcessor` - PFX处理组件

#### CSR模块组件  
- `CsrGenerator` - CSR生成向导
- `CsrParser` - CSR解析展示
- `CsrValidator` - CSR验证组件

#### 密钥模块组件
- `KeyGenerator` - 密钥对生成器
- `KeyConverter` - 密钥格式转换
- `KeyParser` - 密钥解析展示

#### HTTPS模块组件
- `HttpsDetector` - HTTPS证书检测
- `CertificateExtractor` - 证书提取工具
- `ChainCompleter` - 证书链补全

#### 工具模块组件
- `DevToolsHub` - 开发工具集合
- `NetworkTools` - 网络工具集合
- `DnsLookup` - DNS查询工具
- `HttpClient` - HTTP请求工具

### 通用 UI 组件
- `FileDropZone` - 拖拽上传区域
- `CodeEditor` - 代码编辑器 (Monaco Editor)
- `CopyableText` - 可复制文本组件
- `DownloadCard` - 文件下载卡片
- `ProcessingStatus` - 处理状态指示器
- `ResultCard` - 结果展示卡片

## 数据流设计

### 模型定义 (src/models/)

模型按功能模块分类，每个模型一个文件：

#### 证书模块 (certificate/)
- `Certificate.ts` - 证书基础模型、解析后的证书信息
- `PfxContainer.ts` - PFX 容器模型

#### CSR模块 (csr/)
- `CSR.ts` - CSR 模型、解析后的 CSR 信息

#### 密钥模块 (key/)
- `KeyPair.ts` - 密钥对模型

#### HTTPS模块 (https/)
- `HttpsInfo.ts` - HTTPS 检测信息模型

#### 网络模块 (network/)
- `DnsQuery.ts` - DNS 查询相关模型
- `HttpRequest.ts` - HTTP 请求响应模型

#### 项目管理模块 (project/)
- `Project.ts` - 项目模型
- `UserSettings.ts` - 用户设置模型
- `Notification.ts` - 通知模型
- `HistoryRecord.ts` - 历史记录模型

### 状态管理 (Pinia Stores)
- `useCertificateStore` - 证书数据和转换状态
- `useProjectStore` - 项目和工作区管理
- `useUserStore` - 用户设置和偏好
- `useThemeStore` - 主题和国际化状态
- `useNotificationStore` - 消息通知管理
- `useHistoryStore` - 操作历史记录

### API 服务层架构

#### 核心API客户端
- `CloudApiClient` - 云端处理模式API客户端（已整合 BaseApiClient）
- `BrowserApiClient` - 浏览器本地处理客户端（待开发）
- `ServiceFactory` - 服务工厂，根据模式选择实现

#### 统一功能接口 (src/services/interfaces/)

按功能模块拆分的服务接口，每个接口一个文件：

- `ICertificateService.ts` - 证书操作统一接口
  - 包含：CertGenerateOptions, SignOptions, GeneratedCertificate, ValidationResult, ChainValidationResult
  
- `ICsrService.ts` - CSR操作统一接口
  - 包含：CsrGenerateOptions, GeneratedCSR
  
- `IKeyService.ts` - 密钥操作统一接口
  - 包含：KeyGenerateOptions, KeyConvertOptions
  
- `IHttpsService.ts` - HTTPS检测统一接口
  - 包含：HttpsDetectOptions
  
- `INetworkService.ts` - 网络工具统一接口
  - 依赖模型：DnsRecordType, DnsQueryResult, HttpRequestOptions, HttpResponse

**接口设计原则**：
- 所有方法返回 `Promise<T>`，不包含 HTTP 层的 `ApiResponse`
- 接口只定义业务逻辑契约，不关心底层实现细节
- 这样的设计使得接口可以被云端模式和浏览器模式共享
- 接口相关的选项和结果类型定义在同一文件中

#### 具体实现类
**云端模式实现:**
- `CloudCertificateService` - 云端证书处理
- `CloudCsrService` - 云端CSR处理
- `CloudKeyService` - 云端密钥处理
- `CloudHttpsService` - 云端HTTPS检测（待开发）
- `CloudNetworkService` - 云端网络工具（待开发）

**浏览器模式实现:**
- `BrowserCertificateService` - 浏览器证书处理（待开发）
- `BrowserCsrService` - 浏览器CSR处理（待开发）
- `BrowserKeyService` - 浏览器密钥处理（待开发）
- `BrowserHttpsService` - 浏览器HTTPS检测（待开发）
- `BrowserNetworkService` - 浏览器网络工具（待开发）

### 数据格式

所有业务数据模型定义在 `src/models/` 中，按模块分类：

```typescript
// 证书模块 - src/models/certificate/
interface Certificate {
  id: string;
  name: string;
  type: 'X509' | 'PFX' | 'P12';
  format: 'PEM' | 'DER' | 'PFX' | 'P12';
  content: string;
  parsed: ParsedCertificate;
  createdAt: Date;
  projectId?: string;
  tags: string[];
}

interface ParsedCertificate {
  subject: CertificateSubject;
  issuer: CertificateIssuer;
  validity: {
    notBefore: Date;
    notAfter: Date;
    isExpired: boolean;
    daysUntilExpiry: number;
  };
  serialNumber: string;
  version: number;
  fingerprint: {
    md5: string;
    sha1: string;
    sha256: string;
  };
  publicKey: PublicKeyInfo;
  extensions: CertificateExtension[];
  signatureAlgorithm: string;
}

// CSR模块 - src/models/csr/
interface CertificateSigningRequest {
  id: string;
  name: string;
  content: string;
  parsed: ParsedCSR;
  createdAt: Date;
}

interface ParsedCSR {
  subject: CertificateSubject;
  publicKey: PublicKeyInfo;
  attributes: CSRAttribute[];
  signatureAlgorithm: string;
  extensions: CertificateExtension[];
}

// 密钥模块 - src/models/key/
interface KeyPair {
  id: string;
  name: string;
  algorithm: 'RSA' | 'ECDSA' | 'Ed25519';
  keySize: number;
  privateKey: string;
  publicKey: string;
  format: 'PKCS1' | 'PKCS8';
  createdAt: Date;
}

interface PublicKeyInfo {
  algorithm: string;
  keySize: number;
  curve?: string; // For ECDSA
  modulus?: string; // For RSA
  publicExponent?: string; // For RSA
}

// PFX模块 - src/models/certificate/
interface PfxContainer {
  certificates: Certificate[];
  privateKeys: string[];
  friendlyNames: string[];
  password?: string;
}

// HTTPS模块 - src/models/https/
interface HttpsInfo {
  domain: string;
  port: number;
  certificates: Certificate[];
  chain: Certificate[];
  protocols: string[];
  cipherSuites: string[];
  isValid: boolean;
  errors: string[];
  checkedAt: Date;
}

// 网络模块 - src/models/network/
type DnsRecordType = 'A' | 'AAAA' | 'CNAME' | 'MX' | 'TXT' | 'NS' | 'PTR';

interface DnsQueryResult {
  type: DnsRecordType;
  records: any[];
  ttl?: number;
}

interface HttpRequestOptions {
  method: 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH' | 'HEAD' | 'OPTIONS';
  url: string;
  headers?: Record<string, string>;
  body?: any;
  timeout?: number;
}

interface HttpResponse {
  status: number;
  statusText: string;
  headers: Record<string, string>;
  body: any;
  time: number; // 响应时间（毫秒）
}
```

**模型组织原则**：
- 按功能模块分文件夹：certificate, csr, key, https, network, project
- 每个模型一个独立文件，便于维护和查找
- 通过 `src/models/index.ts` 统一导出
- `src/types/index.ts` 重新导出 models，保持向后兼容


## UI/UX 设计规范

### 布局结构
```
┌─────────────────────────────────────────────────────────┐
│ Header (Fixed)                              Menu ▼     │
├─────────────────────────────────────────────────────────┤
│                                                         │
│                   Main Content Area                     │
│                                                         │
│                                                         │
├─────────────────────────────────────────────────────────┤
│ Footer - Copyright & Version Info                       │
└─────────────────────────────────────────────────────────┘
```

### Element Plus 主题定制
- **主色调**: 技术蓝色系 (#409EFF)
- **成功色**: 绿色系 (#67C23A) 
- **警告色**: 橙色系 (#E6A23C)
- **错误色**: 红色系 (#F56C6C)
- **暗色模式**: 深色主题适配

### UnoCSS 工具类规范
- 响应式设计: `sm:` `md:` `lg:` `xl:`
- 间距系统: `m-*` `p-*` (4px倍数)
- 颜色系统: 与Element Plus保持一致
- 动画效果: `transition-*` `duration-*`

### 国际化支持
- **默认语言**: 中文 (zh-CN)
- **支持语言**: 英文 (en-US)
- **扩展语言**: 日文 (ja-JP), 韩文 (ko-KR)
- **RTL支持**: 预留阿拉伯语支持

### 响应式断点
- **手机**: < 768px
- **平板**: 768px - 1024px  
- **桌面**: > 1024px

## 技术实现方案

### 云端处理架构
```
Frontend (Vue 3)  ←→  Backend API  ←→  Certificate Engine
     ↓                    ↓                   ↓
 Element Plus        Express/Koa         OpenSSL/BoringSSL
 UnoCSS              Multer Upload       node-forge
 Vue Router          JWT Auth            Certificate Utils
 Pinia Store         Rate Limiting       Format Conversion
 Vue I18n            Error Handler       Validation Engine
```

### 核心依赖包
```json
{
  "vue": "^3.5.22",
  "element-plus": "^2.8.0", 
  "@unocss/preset-uno": "^0.62.0",
  "vue-router": "^4.5.1",
  "pinia": "^3.0.3",
  "vue-i18n": "^10.0.0",
  "axios": "^1.7.0",
  "@vueuse/core": "^11.0.0",
  "monaco-editor": "^0.50.0",
  "node-forge": "^1.3.1",
  "crypto-js": "^4.2.0"
}
```

### 包管理器配置
- **pnpm**: 使用 corepack 管理，支持 monorepo 和高效的依赖安装
- **lockfile**: pnpm-lock.yaml 确保依赖版本一致性
- **节点链接**: 硬链接和符号链接减少磁盘空间占用

### API架构设计

#### 项目结构
```
src/
├── api/                          # HTTP 客户端层
│   ├── client/
│   │   └── CloudApiClient.ts    # 云端 API 客户端
│   └── index.ts                  # API 客户端导出
│
├── services/                     # 业务服务层
│   ├── interfaces/              # 服务接口定义（按模块拆分）
│   │   ├── ICertificateService.ts
│   │   ├── ICsrService.ts
│   │   ├── IKeyService.ts
│   │   ├── IHttpsService.ts
│   │   ├── INetworkService.ts
│   │   └── index.ts
│   ├── ServiceFactory.ts        # 服务工厂（单例）
│   ├── cloud/                   # 云端模式实现
│   │   ├── CloudCertificateService.ts
│   │   ├── CloudCsrService.ts
│   │   └── CloudKeyService.ts
│   ├── browser/                 # 浏览器模式实现（待开发）
│   │   ├── BrowserCertificateService.ts
│   │   ├── BrowserCsrService.ts
│   │   └── BrowserKeyService.ts
│   └── index.ts                 # 服务层导出
│
├── models/                      # 数据模型（按模块分类）
│   ├── certificate/
│   │   ├── Certificate.ts
│   │   ├── PfxContainer.ts
│   │   └── index.ts
│   ├── csr/
│   │   ├── CSR.ts
│   │   └── index.ts
│   ├── key/
│   │   ├── KeyPair.ts
│   │   └── index.ts
│   ├── https/
│   │   ├── HttpsInfo.ts
│   │   └── index.ts
│   ├── network/
│   │   ├── DnsQuery.ts
│   │   ├── HttpRequest.ts
│   │   └── index.ts
│   ├── project/
│   │   ├── Project.ts
│   │   ├── UserSettings.ts
│   │   ├── Notification.ts
│   │   ├── HistoryRecord.ts
│   │   └── index.ts
│   └── index.ts                # 统一导出
│
└── types/
    ├── api.ts                  # API 相关类型
    └── index.ts                # 重新导出 models（向后兼容）
```

#### 统一接口抽象层
```typescript
// 核心API接口定义
interface IApiClient {
  get<T>(url: string, config?: AxiosRequestConfig): Promise<ApiResponse<T>>;
  post<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<ApiResponse<T>>;
  put<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<ApiResponse<T>>;
  delete<T>(url: string, config?: AxiosRequestConfig): Promise<ApiResponse<T>>;
  upload<T>(url: string, file: File, config?: AxiosRequestConfig): Promise<ApiResponse<T>>;
}

// API响应格式
interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: string;
  code?: number;
  message?: string;
}

// 处理模式枚举
enum ProcessingMode {
  CLOUD = 'cloud',     // 云端处理模式
  BROWSER = 'browser'  // 浏览器本地处理模式
}

// 功能服务统一接口示例 (src/services/interfaces/ICertificateService.ts)
interface ICertificateService {
  // 证书解析
  parseCertificate(file: File): Promise<ParsedCertificate>;
  parseCertificateFromText(content: string): Promise<ParsedCertificate>;
  
  // 证书生成
  generateSelfSignedCert(options: CertGenerateOptions): Promise<GeneratedCertificate>;
  generateCertFromCSR(csr: string, ca: Certificate, options: SignOptions): Promise<GeneratedCertificate>;
  generateCertFromPublicKey(publicKey: string, ca: Certificate, options: SignOptions): Promise<GeneratedCertificate>;
  
  // 证书验证
  validateCertificate(cert: Certificate): Promise<ValidationResult>;
  validateCertificateChain(certificates: Certificate[]): Promise<ChainValidationResult>;
  validateCertWithPrivateKey(cert: Certificate, privateKey: string): Promise<boolean>;
  
  // 证书转换
  convertCertificateFormat(cert: Certificate, targetFormat: CertFormat): Promise<string>;
  
  // PFX处理
  createPfx(cert: Certificate, privateKey: string, password?: string): Promise<ArrayBuffer>;
  parsePfx(pfxData: ArrayBuffer, password?: string): Promise<PfxContainer>;
}
```

**架构优势**：
- ✅ **模块化接口**：每个服务接口独立文件，职责清晰
- ✅ **模型分离**：业务模型统一在 models 中管理，便于复用
- ✅ **关注点分离**：HTTP 层（CloudApiClient）和业务层（Services）解耦
- ✅ **接口纯净**：服务接口返回 `Promise<T>`，不依赖 HTTP 实现细节
- ✅ **易于扩展**：新增模块只需添加对应的接口和实现文件
- ✅ **类型安全**：完整的 TypeScript 类型支持

#### 双模式实现架构
```typescript
// 服务工厂 - 根据处理模式自动选择实现
class ServiceFactory {
  private static instance: ServiceFactory;
  private currentMode: ProcessingMode = ProcessingMode.CLOUD;
  
  static getInstance(): ServiceFactory {
    if (!this.instance) {
      this.instance = new ServiceFactory();
    }
    return this.instance;
  }
  
  setMode(mode: ProcessingMode) {
    this.currentMode = mode;
  }
  
  createCertificateService(): ICertificateService {
    switch (this.currentMode) {
      case ProcessingMode.CLOUD:
        return new CloudCertificateService(new CloudApiClient());
      case ProcessingMode.BROWSER:
        return new BrowserCertificateService();
      default:
        throw new Error(`Unsupported processing mode: ${this.currentMode}`);
    }
  }
  
  // 其他服务工厂方法...
  createCsrService(): ICsrService { /* ... */ }
  createKeyService(): IKeyService { /* ... */ }
}

// 云端API客户端实现（已整合 BaseApiClient）
class CloudApiClient implements IApiClient {
  private axios: AxiosInstance;
  
  constructor() {
    this.axios = axios.create({
      baseURL: '/api/v1',
      timeout: 30000,
      headers: {
        'Content-Type': 'application/json',
      }
    });
    
    this.setupInterceptors();
  }
  
  private setupInterceptors() {
    // 请求拦截器：添加认证token、loading状态等
    this.axios.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('auth_token');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );
    
    // 响应拦截器：统一错误处理、消息提示
    this.axios.interceptors.response.use(
      (response) => ({
        success: true,
        data: response.data,
        code: response.status
      }),
      (error) => ({
        success: false,
        error: error.message,
        code: error.response?.status || 500,
        message: error.response?.data?.message || 'Unknown error'
      })
    );
  }
  
  async get<T>(url: string, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
    return this.axios.get(url, config);
  }
  
  async post<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
    return this.axios.post(url, data, config);
  }
  
  async upload<T>(url: string, file: File, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
    const formData = new FormData();
    formData.append('file', file);
    
    return this.axios.post(url, formData, {
      ...config,
      headers: {
        'Content-Type': 'multipart/form-data',
        ...config?.headers
      }
    });
  }
}

// 云端服务实现 - 使用 unwrapResponse 模式
class CloudCertificateService implements ICertificateService {
  constructor(private apiClient: IApiClient) {}

  // 辅助方法：将 ApiResponse<T> 解包为 T
  private async unwrapResponse<T>(promise: Promise<ApiResponse<T>>): Promise<T> {
    const response = await promise;
    if (response.success && response.data !== undefined) {
      return response.data;
    }
    throw new Error(response.error || response.message || 'Unknown error');
  }

  // 业务方法：返回纯业务数据
  async parseCertificate(file: File): Promise<ParsedCertificate> {
    return this.unwrapResponse(
      this.apiClient.upload<ParsedCertificate>('/certificate/parse', file)
    );
  }
  
  // ... 其他方法
}

// 浏览器本地处理客户端（待开发）
class BrowserCertificateService implements ICertificateService {
  async parseCertificate(file: File): Promise<ParsedCertificate> {
    // 使用 node-forge 在浏览器中解析证书
    const content = await file.text();
    const cert = forge.pki.certificateFromPem(content);
    
    return {
      subject: this.extractSubject(cert.subject),
      issuer: this.extractIssuer(cert.issuer),
      validity: this.extractValidity(cert.validity),
      // ... 其他字段
    };
  }
  
  // ... 其他方法实现
}
```

**实现要点**：
- 服务工厂根据用户设置的 processingMode 自动选择实现
- CloudApiClient 返回 `ApiResponse<T>`（HTTP 层）
- unwrapResponse() 将 `ApiResponse<T>` 转换为 `T`（业务层）
- 服务方法返回 `Promise<T>`，符合接口定义
- 错误通过 throw 传递，由调用方统一处理


### 安全策略
- **文件上传**: 大小限制、类型检查、病毒扫描
- **私钥处理**: 云端临时存储，处理完毕立即删除
- **数据传输**: HTTPS + JWT认证
- **访问控制**: API频率限制，IP白名单
- **日志审计**: 操作记录，敏感信息脱敏

### 性能优化
- **代码分割**: 路由级别懒加载
- **组件缓存**: Keep-alive缓存
- **API优化**: 请求去重，结果缓存
- **文件处理**: 大文件分片上传
- **CDN加速**: 静态资源加速

### 开发环境配置
- **开发服务器**: Vite dev server (HMR)
- **代码规范**: ESLint + Prettier
- **类型检查**: TypeScript strict mode
- **测试框架**: Vitest + Vue Test Utils
- **构建部署**: Docker + GitHub Actions

## 项目里程碑 (详细规划)

### Phase 1: 基础架构 (Week 1-2)

#### Week 1: 项目初始化
- [x] **1.1 项目脚手架搭建**
  - [x] Vue 3 + Vite + TypeScript 项目初始化
  - [x] ESLint + Prettier 代码规范配置
  - [x] Git 仓库和分支策略设置
  - [x] package.json 依赖包管理

- [x] **1.2 UI框架集成** 
  - [x] Element Plus 组件库集成
  - [x] UnoCSS 原子化CSS配置
  - [x] 自定义主题和CSS变量设置
  - [x] 响应式断点和工具类定义

#### Week 2: 核心架构
- [x] **2.1 路由和状态管理**
  - [x] Vue Router 路由配置和页面结构
  - [x] Pinia Store 状态管理架构
  - [x] 路由守卫和权限控制
  - [x] 页面布局组件开发

- [x] **2.2 国际化和主题**
  - [x] Vue I18n 国际化配置
  - [x] 中文/英文语言包
  - [x] 暗色/亮色主题切换
  - [x] 用户偏好设置存储

- [x] **2.3 API客户端架构**
  - [x] Axios HTTP客户端封装
  - [x] 请求拦截器和错误处理
  - [x] API统一接口定义 (ICertificateService等)
  - [x] 双模式工厂模式实现 (Cloud/Browser)

### Phase 2: 核心模块开发 - 云端模式优先 (Week 3-6)

#### Week 3: 证书模块基础 (云端模式)
- [x] **3.1 证书解析功能 (高优先级)** ✅
  - [x] 云端证书解析API对接
  - [x] 证书信息展示组件
  - [x] 文件上传和文本输入支持
  - [x] 详细的证书信息展示
  - [x] 错误处理和用户提示
  - [x] 国际化支持（中英文）

- [ ] **3.2 证书生成功能 (高优先级)**
  - [ ] 自签名证书生成向导
  - [ ] 证书配置表单组件
  - [ ] 云端证书生成实现
  - [ ] 证书预览和下载

#### Week 4: CSR和密钥模块
- [ ] **4.1 CSR模块 (高优先级)**
  - [ ] CSR生成向导界面
  - [ ] CSR解析和展示
  - [ ] 主题信息配置组件
  - [ ] SAN扩展配置
  - [ ] CSR与密钥验证

- [ ] **4.2 密钥管理模块**
  - [ ] 密钥对生成器 (RSA/ECDSA/Ed25519)
  - [ ] 密钥格式转换 (PKCS#1/PKCS#8)
  - [ ] 密钥解析和信息展示
  - [ ] 密钥安全存储

#### Week 5: 证书高级功能
- [ ] **5.1 证书验证和转换**
  - [ ] 证书链验证逻辑
  - [ ] 证书与密钥匹配验证
  - [ ] PEM/DER格式转换
  - [ ] 批量处理功能

- [ ] **5.2 PFX处理**
  - [ ] PFX合成和生成
  - [ ] PFX解析和提取
  - [ ] 密码保护处理
  - [ ] 证书链导入导出

#### Week 6: 证书生成完善
- [ ] **6.1 高级证书生成**
  - [ ] 公钥 + CA签发证书
  - [ ] CSR + CA签发证书  
  - [ ] CA证书生成证书和私钥
  - [ ] 批量证书生成

- [ ] **6.2 证书管理**
  - [ ] 证书项目组织
  - [ ] 标签分类系统
  - [ ] 收藏和快速访问
  - [ ] 使用统计和历史

### Phase 3: 扩展模块开发 (Week 7-8)

#### Week 7: HTTPS和网络工具
- [ ] **7.1 HTTPS检测模块**
  - [ ] 在线证书检测
  - [ ] HTTPS连接分析
  - [ ] 证书链补全算法
  - [ ] SSL/TLS协议检测
  - [ ] 到期监控告警

- [ ] **7.2 网络工具集**
  - [ ] DNS查询工具 (A/AAAA/MX/TXT/CNAME/NS)
  - [ ] HTTP/HTTPS请求工具
  - [ ] 请求头自定义界面
  - [ ] 响应分析和性能监控

#### Week 8: 开发工具和整合
- [ ] **8.1 开发工具集**
  - [ ] JWT Token工具
  - [ ] 哈希计算器
  - [ ] Base64/Hex编解码
  - [ ] UUID生成器
  - [ ] 时间戳转换
  - [ ] JSON格式化验证

- [ ] **8.2 模块整合测试**
  - [ ] 跨模块功能测试
  - [ ] API接口联调
  - [ ] 双模式切换测试
  - [ ] 数据流验证

### Phase 4: 浏览器模式开发 (Week 9)

#### Week 9: 浏览器本地处理实现
- [ ] **9.1 浏览器端证书处理**
  - [ ] node-forge 集成和配置
  - [ ] 浏览器证书解析实现 (BrowserCertificateService)
  - [ ] 浏览器证书生成实现
  - [ ] 浏览器PFX处理实现
  
- [ ] **9.2 浏览器端CSR和密钥处理**
  - [ ] 浏览器CSR生成和解析 (BrowserCsrService)
  - [ ] 浏览器密钥管理 (BrowserKeyService)
  - [ ] 加密算法本地实现
  - [ ] 安全性和性能优化

### Phase 5: 优化和部署 (Week 10)

#### Week 9: 性能优化
- [ ] **9.1 前端优化**
  - [ ] 代码分割和懒加载
  - [ ] 组件缓存优化
  - [ ] Bundle大小优化
  - [ ] 加载性能优化

- [ ] **9.2 API优化**
  - [ ] 请求去重和缓存
  - [ ] 大文件分片处理
  - [ ] 错误重试机制
  - [ ] 接口性能监控

#### Week 10: 测试和部署
- [ ] **10.1 测试完善**
  - [ ] 单元测试 (Vitest)
  - [ ] 组件测试 (Vue Test Utils)
  - [ ] E2E测试 (Cypress)
  - [ ] 浏览器兼容性测试

- [ ] **10.2 部署和文档**
  - [ ] Docker容器化
  - [ ] CI/CD流水线 (GitHub Actions)
  - [ ] 用户使用文档
  - [ ] API接口文档
  - [ ] 部署和运维指南

### 关键里程碑检查点
- **Week 2 End**: ✅ 基础架构完成，API双模式可切换
- **Week 4 End**: ✅ 证书解析、CSR生成核心功能完成
- **Week 6 End**: ✅ 证书模块所有功能完成  
- **Week 8 End**: ✅ 所有模块开发完成
- **Week 10 End**: ✅ 项目部署上线

---

*设计文档 v1.0 - 基于开发测试场景的证书工具需求*