# 系统架构

## 项目定位

Crypto Utils 是一个**无状态、无认证、无持久化**的纯工具型平台，提供密钥对管理、证书操作、CSR 处理、CRL 管理等密码学工具能力。

- 所有操作基于请求-响应模式，数据仅在内存中处理
- 无用户系统、无数据库、无文件系统存储
- 私钥等敏感数据不落盘、不写日志

---

## 系统分层

```
┌─────────────────────────────────────────────┐
│           Crypto.Utils.UI (Vue 3 SPA)        │
│           http://localhost:5173              │
└───────────────────┬─────────────────────────┘
                    │ HTTP (开发: SpaProxy 代理)
┌───────────────────▼─────────────────────────┐
│         Crypto.Utils.Host (ASP.NET Core)     │
│           http://localhost:5000              │
│  - 中间件注册（异常处理、HTTPS重定向）         │
│  - 路由映射（Controllers + /health）          │
│  - SpaProxy（开发环境）/ 静态文件（生产）      │
└───────────────────┬─────────────────────────┘
                    │ 依赖注入
┌───────────────────▼─────────────────────────┐
│         Crypto.Utils.Api (类库)              │
│  Controllers → Services → Mappers → Models  │
└───────────────────┬─────────────────────────┘
                    │ 引用
┌───────────────────▼─────────────────────────┐
│         Crypto.Utils.Core (类库)             │
│  密钥 / 证书 / CSR / CRL / SM算法封装        │
└─────────────────────────────────────────────┘
```

---

## 后端结构

### Crypto.Utils.Core

核心密码学功能封装，基于 BouncyCastle。

```
Crypto.Utils.Core/
├── Crypto/                         # 非对称密钥封装
│   ├── AsymmetricKeyPair.cs        # 密钥对（含 GenerateRsa/GenerateEc/GenerateDsa）
│   ├── AsymmetricKeyParameter.cs   # 密钥基类（PEM/DER 导入导出）
│   ├── AsymmetricPrivateKeyParameter.cs
│   ├── AsymmetricPublicKeyParameter.cs
│   └── Sm/                         # 商密算法
│       ├── SM2.cs                  # 基于 ECKeyParameters + sm2p256v1
│       ├── SM3.cs                  # 基于 SM3Digest
│       └── SM4.cs                  # 基于 SM4Engine
├── X509/                           # X.509 对象模型
│   ├── Certificate.cs              # 证书（版本/序列号/主体/颁发者/有效期/扩展/指纹）
│   ├── CertificateSigningRequest.cs
│   ├── CertificateRevocationList.cs
│   ├── Enums/                      # 枚举（KeyUsage/ExtendedKeyUsage/GeneralNameType 等）
│   ├── Extensions/                 # 辅助工具（FingerprintHelper/KeyUsageHelper 等）
│   ├── Models/                     # 值对象（CertificateExtension/GeneralName/X509DistinguishedName）
│   └── Utils/CertificateUtils.cs
├── BouncyCastle/                   # BouncyCastle 扩展
│   ├── Asn1/X509/                  # ASN.1/X.509 扩展方法
│   └── ObjectIdentifiers/          # OID 常量（CertificatePolicy/ExtendedKeyUsage）
└── Resources/                      # 枚举多语言资源（zh-hans / en-us）
```

### Crypto.Utils.Api

HTTP 层与业务逻辑层，以类库形式被 Host 引用。

```
Crypto.Utils.Api/
├── Controllers/              # HTTP 入口，仅做路由和参数绑定
│   ├── KeyController.cs          # /api/key/*
│   ├── CertificateController.cs  # /api/cert/*
│   ├── CsrController.cs          # /api/csr/*
│   ├── CrlController.cs          # /api/crl/*
│   ├── ChainController.cs        # /api/cert/chain/*
│   └── FormatController.cs       # /api/format/*
├── Services/                 # 业务逻辑
│   ├── IKeyService.cs / Implementations/KeyService.cs
│   ├── ICertificateService.cs / Implementations/CertificateService.cs
│   ├── ICsrService.cs / Implementations/CsrService.cs
│   ├── ICrlService.cs / Implementations/CrlService.cs
│   ├── ICertificateChainService.cs / Implementations/CertificateChainService.cs
│   └── IFormatService.cs / Implementations/FormatService.cs
├── Models/
│   ├── ApiResponse.cs            # 统一响应封装 ApiResponse<T>
│   ├── Requests/                 # 各模块请求模型（*Requests.cs）
│   └── Responses/                # 各模块响应模型（*Responses.cs）
├── Mappers/                  # 模型映射（Core 对象 ↔ API 模型）
│   ├── CertificateMapper.cs
│   ├── CrlMapper.cs
│   └── KeyParameterMapper.cs
├── Middlewares/
│   └── ExceptionHandlingMiddleware.cs  # 全局异常处理
└── Extensions/
    └── ServiceCollectionExtensions.cs  # DI 注册 + Swagger 配置
```

### Crypto.Utils.Host

ASP.NET Core Web 宿主，负责应用启动配置。

- **开发模式**：启用 Swagger UI、注册异常处理中间件、SpaProxy（代理到 `:5173`）
- **生产模式**：HSTS + 静态文件服务（服务编译后的 UI dist）
- `/health` 健康检查端点（始终可用）

---

## 前端结构（规划）

技术栈：Vue 3 + Vite + TypeScript + Element Plus + UnoCSS

```
Crypto.Utils.UI/src/
├── api/
│   └── client/
│       └── CloudApiClient.ts     # Axios 封装，处理 ApiResponse<T>
├── services/
│   ├── interfaces/               # 服务接口（ICertificateService 等）
│   ├── cloud/                    # 云端模式实现（调用后端 API）
│   └── browser/                  # 浏览器模式实现（本地 WASM/node-forge，待开发）
├── models/                       # 数据模型（按模块分目录）
│   ├── certificate/
│   ├── csr/
│   ├── key/
│   ├── https/
│   └── network/
├── views/                        # 页面组件（按路由模块）
│   ├── certificate/
│   ├── csr/
│   ├── key/
│   ├── https/
│   └── devtools/
├── components/                   # 通用组件（FileDropZone/CopyableText 等）
└── stores/                       # Pinia 状态管理
```

### 双模式服务层

前端支持两种处理模式，通过 `ServiceFactory` 统一切换：

| 模式      | 说明                              | 状态             |
| --------- | --------------------------------- | ---------------- |
| `cloud`   | 调用后端 REST API 处理            | 规划中（主模式） |
| `browser` | 浏览器本地处理（node-forge/WASM） | 待开发           |

服务接口（`ICertificateService`、`IKeyService`、`ICsrService`、`IHttpsService`、`INetworkService`）返回 `Promise<T>`，不包含 HTTP 层细节，Cloud / Browser 实现可互换。

---

## 路由

### 后端 API

| 模块     | 路由前缀                 |
| -------- | ------------------------ |
| 密钥     | `POST /api/key/*`        |
| 证书     | `POST /api/cert/*`       |
| CSR      | `POST /api/csr/*`        |
| CRL      | `POST /api/crl/*`        |
| 证书链   | `POST /api/cert/chain/*` |
| 格式转换 | `POST /api/format/*`     |
| 健康检查 | `GET /health`            |

### 前端页面（规划）

```
/                    # 首页
/certificate/*       # 证书解析/生成/转换/验证/PFX
/csr/*               # CSR 生成/解析/验证
/key/*               # 密钥生成/转换/解析
/https/*             # HTTPS 检测/提取/证书链补全
/devtools/*          # JWT / 哈希 / 编解码 / UUID
/network/*           # DNS 查询 / HTTP 工具
```

---

## 核心依赖

| 依赖                               | 版本   | 用途                             |
| ---------------------------------- | ------ | -------------------------------- |
| BouncyCastle.Cryptography          | 2.6.2  | 密码学核心（含商密 SM2/SM3/SM4） |
| Swashbuckle.AspNetCore             | 6.6.2  | Swagger/OpenAPI 文档             |
| Swashbuckle.AspNetCore.Annotations | 6.6.2  | Swagger 注解                     |
| Microsoft.AspNetCore.OpenApi       | 8.0.0  | OpenAPI 集成                     |
| Microsoft.AspNetCore.SpaProxy      | 7.0.20 | 开发环境 SPA 代理                |
| NUnit                              | 4.2.2  | 单元测试                         |
| FluentAssertions                   | 6.12.1 | 断言库                           |
| CliWrap                            | 3.6.6  | OpenSSL 互操作测试               |
| Vue                                | 3.5.x  | 前端框架                         |
| Vite                               | —      | 前端构建工具                     |
