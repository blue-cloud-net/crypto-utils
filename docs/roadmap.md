# 开发路线图

## 当前状态（已完成）

### Crypto.Utils.Core

- [x] 非对称密钥封装：`AsymmetricKeyPair`、`AsymmetricPrivateKeyParameter`、`AsymmetricPublicKeyParameter`（RSA / EC / DSA，支持 PEM/DER 导出）
- [x] 商密算法：`SM2`、`SM3`、`SM4` 完整封装（基于 BouncyCastle）
- [x] X.509 对象模型：`Certificate`、`CertificateSigningRequest`、`CertificateRevocationList`
- [x] 枚举体系：`KeyUsage`、`ExtendedKeyUsage`、`GeneralNameType`、`CertificatePolicy`、`CertificateRevocationReason`（含中英文资源文件）
- [x] 辅助工具：`FingerprintHelper`、`KeyUsageHelper`、`ExtendedKeyUsageHelper`、`CertificatePolicyHelper` 等
- [x] BouncyCastle 扩展：ASN.1/X.509 扩展方法、OID 常量

### Crypto.Utils.Api

- [x] 六个模块的 Controller 骨架：`KeyController`、`CertificateController`、`CsrController`、`CrlController`、`ChainController`、`FormatController`
- [x] 对应 Service 接口 + 实现骨架（六个模块）
- [x] 完整 Request / Response 模型定义
- [x] `ApiResponse<T>` 统一响应封装
- [x] `ExceptionHandlingMiddleware` 全局异常处理中间件
- [x] `ServiceCollectionExtensions` DI 注册 + Swagger 配置
- [x] `CertificateMapper`、`CrlMapper`、`KeyParameterMapper` 映射器

### Crypto.Utils.Host

- [x] ASP.NET Core Web 宿主配置（SpaProxy / 静态文件）
- [x] `/health` 健康检查端点

### 测试

- [x] SM2 / SM3 / SM4 单元测试
- [x] SM2 / SM3 / SM4 OpenSSL 互操作测试

---

## Phase 1：Core 功能补全（Core 已完成，宿主待办）

> Core 模块的独立计划与进度详见 [core-roadmap.md](core-roadmap.md) 与 [core-development-plan.md](core-development-plan.md)。
> 测试框架已切换为 xUnit + FluentAssertions；互操作测试 SM 走 tongsuo、RSA/EC/DSA 走 openssl。

### 测试（优先）

- [x] RSA / EC / DSA 密钥单元测试（生成、PEM/DER 往返、参数提取）
- [x] RSA 加解密单元测试（OAEP-SHA256 + PKCS#1 v1.5）
- [x] RSA 签名/验签单元测试（PSS + PKCS#1 v1.5）
- [x] ECDSA 签名/验签单元测试；ECDH 密钥协商单元测试
- [x] DSA 签名/验签单元测试
- [x] `Certificate` 单元测试（自签名含扩展、SignCsr、SignPublicKey、PFX 往返）
- [x] `CertificateSigningRequest` 单元测试（生成含扩展、解析、验签）
- [x] `CertificateRevocationList` 单元测试（生成、解析、IsRevoked）
- [x] RSA / ECDSA OpenSSL 互操作测试（加解密 + 签名交叉验证）

### Crypto.Utils.Core 补全

- [x] `RsaCrypto`：RSA 加密/解密（OAEP-SHA256 默认 / PKCS#1 v1.5 可选）、签名/验签（PSS 默认 / PKCS#1 v1.5 可选）
- [x] `EcdsaCrypto`：ECDSA 签名/验签（DER 编码）、ECDH 共享密钥计算
- [x] `DsaCrypto`：DSA 签名/验签
- [x] `AesCrypto`：AES-CBC / AES-GCM 封装（含自动 IV 生成，禁用 ECB 模式）
- [x] `X509ExtensionOptions`：证书/CSR 扩展字段参数值对象
- [x] `Certificate.GenerateSelfSigned` / `SignCsr` / `SignPublicKey` 新增扩展字段参数（`KeyUsage`、`ExtendedKeyUsage`、`SubjectAlternativeNames`、`BasicConstraints`、SKI、AKI、CRL 分发点）
- [x] `CertificateSigningRequest.Generate` 新增扩展属性参数（SAN、KeyUsage、EKU）
- [x] `PfxUtils.ToPfx` / `FromPfx`：PFX/PKCS#12 导入导出（含密码保护与证书链）

### Crypto.Utils.Api 补全

- [x] `KeyService.ConvertPkcsFormatAsync`：PKCS#1 ↔ PKCS#8 格式转换
- [x] `KeyService.EncryptPrivateKeyAsync` / `DecryptPrivateKeyAsync`：私钥加密/解密
- [x] 补全其余 Service 实现逻辑（接入 Core 新增能力）

### 宿主

- [x] Host `Program.cs` 正式注册中间件与 Swagger
- [ ] Docker 容器化部署

---

## Phase 2：前端基础（Web 首期已完成）

> Web 模块的独立计划与进度详见 [web-roadmap.md](web-roadmap.md) 与 [web-development-plan.md](web-development-plan.md)。

### 前端工程

- [x] Element Plus 组件库集成
- [x] Vue Router 路由（模块内多路由独立页面）
- [x] Pinia 状态管理
- [x] Axios HTTP 客户端封装（`CloudApiClient`）
- [x] `ServiceFactory` 双模式工厂（cloud 实现 + browser 占位）
- [x] vue-i18n 中英双语（默认随浏览器语言）
- [x] 主题三态（system/light/dark，默认跟随系统）

### 功能页面（cloud 模式）

- [x] 密钥管理页面（生成 / 解析 / 格式转换 / PKCS 转换 / 私钥加解密）
- [x] 证书页面（解析 / 自签名生成（最简））
- [x] CSR 页面（生成 / 解析）
- [ ] 浏览器本地运算（Browser Service，基于 Web Crypto + node-forge，二期）
- [ ] 自签名证书高级扩展（KU/EKU/SAN/序列号，二期）

---

## Phase 3：核心功能完整化

- [ ] CSR 完整支持（生成含扩展 / 解析 / 验证页面）
- [ ] CA 签发证书三种模式（CSR 签发 / 公钥签发 / 直接生成）
- [ ] PFX / PKCS#12 支持页面（合成 / 提取 / 密码保护）
- [ ] 证书链构建与验证页面
- [ ] HTTPS 证书在线检测与提取
- [ ] 开发工具集（JWT / 哈希 / 编解码 / UUID）

---

## Phase 4：扩展功能（按需）

- [ ] CRL 完整支持（生成 / 解析 / 吊销检查页面）
- [ ] 批量操作
- [ ] 浏览器本地处理模式（Browser Service 实现，基于 node-forge）
- [ ] OCSP 支持
- [ ] DNS / HTTP 网络工具

---

## 已确认的技术决策

- **无持久化**：所有操作无状态，不使用数据库或文件存储
- **无认证**：纯工具 API，网络层安全由部署方决定
- **商密支持**：完整支持 SM2 / SM3 / SM4
- **算法范围**：RSA、EC、DSA、SM2；PEM / DER / PFX 格式
- **加密 padding**：RSA 加密默认 OAEP-SHA256，签名默认 PSS；保留 PKCS#1 v1.5 可选项
- **ECDH**：返回原始共享密钥字节，KDF 由调用方决定
- **DSA**：仅支持签名/验签，不支持加密（算法限制）
- **暂不支持**：HSM、EdDSA、JKS、时间戳服务
