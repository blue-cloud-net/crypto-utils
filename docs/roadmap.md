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

## Phase 1：后端完善 + 前端基础（进行中）

### 后端

- [ ] 补全 Service 实现逻辑（当前为骨架，核心业务调用 Core 库待接入）
- [ ] Host `Program.cs` 正式注册中间件与 Swagger
- [ ] Docker 容器化部署

### 前端（全部待建）

- [ ] Element Plus 组件库集成
- [ ] UnoCSS 样式配置
- [ ] Vue Router 路由
- [ ] Pinia 状态管理
- [ ] Axios HTTP 客户端封装（`CloudApiClient`）
- [ ] `ServiceFactory` 双模式工厂
- [ ] 密钥管理页面（生成 / 解析 / 格式转换）
- [ ] 证书页面（解析 / 自签名生成）

---

## Phase 2：核心功能完整化

- [ ] CSR 完整支持（生成 / 解析 / 验证页面）
- [ ] CA 签发证书三种模式（CSR 签发 / 公钥签发 / 直接生成）
- [ ] PFX / PKCS#12 支持（合成 / 提取 / 密码保护）
- [ ] 证书链构建与验证页面
- [ ] HTTPS 证书在线检测与提取
- [ ] 开发工具集（JWT / 哈希 / 编解码 / UUID）

---

## Phase 3：扩展功能（按需）

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
- **暂不支持**：HSM、EdDSA、JKS、时间戳服务
