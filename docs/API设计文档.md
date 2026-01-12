# 证书工具 API 项目设计文档

## 1. 项目概述

### 1.1 项目名称
证书工具 API (Certificate Utils API)

### 1.2 项目目标
构建一个功能完善的证书管理 Web API 服务，提供证书生成、解析、转换、验证等核心功能，支持多种证书格式和算法。

### 1.3 技术栈
- **框架**: ASP.NET Core 8.0 / Minimal API
- **加密库**: BouncyCastle
- **支持算法**: RSA、EC、DSA、**商密算法（SM2/SM3/SM4）**
- **证书标准**: X.509
- **服务类型**: 纯 API 服务（无 UI、无用户系统、无认证）
- **存储**: 无持久化存储（所有操作基于请求-响应模式）

---

## 2. 功能需求

### 2.1 密钥对管理

#### 2.1.1 生成密钥对
- **接口**: `POST /api/keypair/generate`
- **功能**: 生成非对称密钥对
- **参数**:
  - `algorithm`: 算法类型 (RSA/EC/SM2/DSA)
  - `keySize`: 密钥大小 (RSA: 2048/4096, EC: 256/384/521)
  - `curveName`: EC 曲线名称（可选）
- **返回**: 公钥和私钥（PEM/DER 格式）

#### 2.1.2 密钥格式转换
- **接口**: `POST /api/keypair/convert`
- **功能**: 转换密钥格式 (PEM ↔ DER)
- **参数**:
  - `keyData`: 密钥数据
  - `sourceFormat`: 源格式
  - `targetFormat`: 目标格式
- **返回**: 转换后的密钥

#### 2.1.3 密钥信息查询
- **接口**: `POST /api/keypair/info`
- **功能**: 解析并显示密钥信息
- **参数**:
  - `keyData`: 密钥数据（PEM/DER）
- **返回**: 密钥类型、算法、大小等信息

#### 2.1.4 密钥格式转换 (PKCS1/PKCS8)
- **接口**: `POST /api/keypair/pkcs-convert`
- **功能**: 在 PKCS#1 和 PKCS#8 格式之间转换
- **参数**:
  - `keyData`: 密钥数据
  - `sourceFormat`: 源格式 (PKCS1/PKCS8)
  - `targetFormat`: 目标格式 (PKCS1/PKCS8)
  - `password`: 加密密码（可选，用于加密的 PKCS#8）
- **返回**: 转换后的密钥

#### 2.1.5 密钥加密
- **接口**: `POST /api/keypair/encrypt`
- **功能**: 加密私钥（生成加密的 PKCS#8 格式）
- **参数**:
  - `privateKey`: 私钥数据
  - `password`: 加密密码
  - `algorithm`: 加密算法（默认 AES-256-CBC）
- **返回**: 加密后的私钥

#### 2.1.6 密钥解密
- **接口**: `POST /api/keypair/decrypt`
- **功能**: 解密私钥
- **参数**:
  - `encryptedPrivateKey`: 加密的私钥数据
  - `password`: 解密密码
- **返回**: 解密后的私钥

---

### 2.2 证书签名请求 (CSR)

#### 2.2.1 生成 CSR
- **接口**: `POST /api/csr/generate`
- **功能**: 生成证书签名请求
- **参数**:
  - `subject`: 证书主体信息 (DN)
    - CN (Common Name)
    - O (Organization)
    - OU (Organizational Unit)
    - C (Country)
    - ST (State)
    - L (Locality)
  - `privateKey`: 私钥数据
  - `signatureAlgorithm`: 签名算法
  - `extensions`: 扩展字段（可选）
- **返回**: CSR 数据（PEM/DER）

#### 2.2.2 解析 CSR
- **接口**: `POST /api/csr/parse`
- **功能**: 解析 CSR 内容
- **参数**:
  - `csrData`: CSR 数据
- **返回**: 主体信息、公钥、签名算法等

#### 2.2.3 验证 CSR
- **接口**: `POST /api/csr/verify`
- **功能**: 验证 CSR 签名
- **参数**:
  - `csrData`: CSR 数据
- **返回**: 验证结果（布尔值）

---

### 2.3 证书管理

#### 2.3.1 生成自签名证书
- **接口**: `POST /api/certificate/self-signed`
- **功能**: 生成自签名证书
- **参数**:
  - `subject`: 证书主体
  - `privateKey`: 私钥
  - `validFrom`: 有效期开始时间
  - `validTo`: 有效期结束时间
  - `keyUsage`: 密钥用途
  - `extendedKeyUsage`: 扩展密钥用途
  - `subjectAlternativeNames`: SAN（可选）
- **返回**: 证书数据（PEM/DER）

#### 2.3.2 签发证书（CA 签名）

##### 模式 1: 基于 CSR 签发
- **接口**: `POST /api/certificate/sign-csr`
- **功能**: 使用 CA 证书为 CSR 签发证书
- **参数**:
  - `csr`: CSR 数据
  - `caCertificate`: CA 证书
  - `caPrivateKey`: CA 私钥
  - `validFrom`: 有效期开始
  - `validTo`: 有效期结束
  - `serialNumber`: 序列号
  - `extensions`: 证书扩展
- **返回**: 签发的证书

##### 模式 2: 基于公钥签发
- **接口**: `POST /api/certificate/sign-publickey`
- **功能**: 使用 CA 证书为指定公钥签发证书
- **参数**:
  - `publicKey`: 公钥数据
  - `subject`: 证书主体信息
  - `caCertificate`: CA 证书
  - `caPrivateKey`: CA 私钥
  - `validFrom`: 有效期开始
  - `validTo`: 有效期结束
  - `serialNumber`: 序列号
  - `extensions`: 证书扩展
- **返回**: 签发的证书

##### 模式 3: 直接生成密钥和证书
- **接口**: `POST /api/certificate/sign-generate`
- **功能**: 直接生成密钥对并签发证书（一步完成）
- **参数**:
  - `subject`: 证书主体信息
  - `keyAlgorithm`: 密钥算法 (RSA/EC/SM2)
  - `keySize`: 密钥大小
  - `caCertificate`: CA 证书
  - `caPrivateKey`: CA 私钥
  - `validFrom`: 有效期开始
  - `validTo`: 有效期结束
  - `serialNumber`: 序列号
  - `extensions`: 证书扩展
- **返回**: 签发的证书和生成的私钥

#### 2.3.3 解析证书
- **接口**: `POST /api/certificate/parse`
- **功能**: 解析证书信息（包含指纹和公钥提取）
- **参数**:
  - `certificateData`: 证书数据
  - `includePublicKey`: 是否包含公钥详情（默认 true）
  - `fingerprintAlgorithms`: 指纹算法列表（默认 SHA-1, SHA-256）
- **返回**: 完整证书信息
  - 主体、颁发者
  - 序列号、有效期
  - 公钥信息（完整公钥数据）
  - 扩展字段
  - 签名算法
  - 证书指纹（SHA-1、SHA-256、MD5 等）

#### 2.3.4 验证证书
- **接口**: `POST /api/certificate/verify`
- **功能**: 验证证书有效性
- **参数**:
  - `certificate`: 待验证证书
  - `issuerCertificate`: 颁发者证书（可选）
  - `checkDate`: 检查日期（可选，默认当前时间）
- **返回**: 验证结果
  - 签名是否有效
  - 是否在有效期内
  - 证书链是否完整

#### 2.3.5 证书格式转换
- **接口**: `POST /api/certificate/convert`
- **功能**: 转换证书格式
- **参数**:
  - `certificateData`: 证书数据
  - `sourceFormat`: 源格式 (PEM/DER/PFX)
  - `targetFormat`: 目标格式
  - `password`: 密码（PFX 需要）
- **返回**: 转换后的证书

---

### 2.4 证书吊销列表 (CRL)

#### 2.4.1 生成 CRL
- **接口**: `POST /api/crl/generate`
- **功能**: 生成证书吊销列表
- **参数**:
  - `issuer`: 颁发者 DN
  - `revokedCertificates`: 吊销证书列表
    - 序列号
    - 吊销日期
    - 吊销原因
  - `caPrivateKey`: CA 私钥
  - `thisUpdate`: 本次更新时间
  - `nextUpdate`: 下次更新时间
- **返回**: CRL 数据

#### 2.4.2 解析 CRL
- **接口**: `POST /api/crl/parse`
- **功能**: 解析 CRL 内容
- **参数**:
  - `crlData`: CRL 数据
- **返回**: CRL 详细信息

#### 2.4.3 检查证书吊销状态
- **接口**: `POST /api/crl/check`
- **功能**: 检查证书是否被吊销
- **参数**:
  - `certificate`: 证书数据
  - `crlData`: CRL 数据
- **返回**: 吊销状态

---

### 2.5 证书链管理

#### 2.5.1 构建证书链
- **接口**: `POST /api/chain/build`
- **功能**: 构建完整证书链
- **参数**:
  - `certificate`: 目标证书
  - `intermediateCertificates`: 中间证书列表
  - `rootCertificates`: 根证书列表
- **返回**: 证书链（从叶子到根）

#### 2.5.2 验证证书链
- **接口**: `POST /api/chain/verify`
- **功能**: 验证整个证书链
- **参数**:
  - `certificateChain`: 证书链
  - `trustedRoots`: 信任的根证书
- **返回**: 验证结果和详细信息

---

---

## 3. 非功能需求

### 3.1 性能要求
- [ ] API 响应时间 < 500ms（常规操作）
- [ ] 支持并发请求
- [ ] 大文件处理优化（文件上传限制）

### 3.2 安全性要求
- [ ] HTTPS 强制加密传输（生产环境）
- [ ] 敏感数据不记录日志（私钥、密码）
- [ ] ~~API 认证机制~~ **（无认证 - 纯工具 API）**
- [ ] ~~请求频率限制~~ **（可选，根据部署环境决定）**
- [ ] 输入验证和异常处理
- [ ] 请求数据大小限制（防止 DoS）
- [ ] 私钥等敏感数据仅在内存中处理，不落盘

### 3.3 可用性要求
- [ ] RESTful API 设计规范
- [ ] 统一错误响应格式
- [ ] API 文档（Swagger/OpenAPI）
- [ ] 详细的日志记录
- [ ] 健康检查端点

### 3.4 扩展性要求
- [ ] 模块化设计
- [ ] 插件化算法支持
- [ ] 配置外部化
- [ ] 容器化部署（Docker）

---

## 4. 数据模型（待补充）

### 4.1 请求/响应模型
- [ ] KeyPairRequest
- [ ] KeyEncryptRequest / KeyDecryptRequest
- [ ] PkcsConvertRequest
- [ ] CertificateRequest
- [ ] CsrRequest
- [ ] SignCsrRequest
- [ ] SignPublicKeyRequest
- [ ] SignGenerateRequest
- [ ] CertificateParseResponse (包含指纹和公钥)
- [ ] ApiResponse<T>
- [ ] ErrorResponse

### 4.2 配置模型
- [ ] 算法配置（支持的算法列表、默认参数）
- [ ] 安全配置（HTTPS、请求大小限制）
- [ ] 日志配置（日志级别、敏感数据过滤）
- [ ] 商密算法配置（SM2 曲线、SM3 参数、SM4 模式）

---

## 5. 部署和运维（待补充）

### 5.1 部署方式
- [ ] Docker 容器
- [ ] Kubernetes
- [ ] 传统部署

### 5.2 监控
- [ ] 应用程序监控
- [ ] 性能指标
- [ ] 错误追踪

---

## 6. 已确认事项

### 6.1 功能优先级
- [ ] 哪些功能是 MVP 阶段必需的？
- [ ] 哪些功能可以延后实现？

### 6.2 算法支持 ✅
- [x] **支持商密 SM2/SM3/SM4** - 完整支持
- [ ] 是否需要支持其他特殊算法？（如 EdDSA、X25519 等）

### 6.3 存储需求 ✅
- [x] **无持久化存储** - 所有操作基于请求-响应模式
- [x] **无数据库** - 纯无状态 API 服务
- [x] **无文件系统存储** - 数据仅在内存中处理

### 6.4 认证授权 ✅
- [x] **无用户系统** - 纯工具型 API
- [x] **无认证机制** - 开放式 API（由部署方决定网络层安全）
- [x] **无权限管理** - 所有接口均可直接访问

### 6.5 文件处理
- [ ] 最大请求体大小限制？（建议 10MB）
- [ ] 是否支持批量操作？
- [ ] 是否需要异步处理大型任务？

### 6.6 证书存储格式
- [ ] PFX/PKCS#12 支持优先级？（建议支持）
- [ ] JKS (Java KeyStore) 是否需要？（建议暂不支持）
- [x] **PEM、DER 基本支持** - 必需

### 6.7 UI 界面 ✅
- [x] **纯 API 服务** - 无 Web 前端
- [x] **Swagger UI** - 仅用于 API 文档和测试

### 6.8 特殊需求
- [ ] 是否需要支持硬件安全模块 (HSM)？（建议暂不支持）
- [ ] 是否需要 OCSP 支持？（建议延后）
- [ ] 是否需要时间戳服务？（建议延后）

---

## 7. 项目里程碑（待规划）

### Phase 1: MVP 核心功能
- [ ] 基础密钥对生成（RSA、EC、SM2）
- [ ] 密钥格式转换（PEM/DER、PKCS1/PKCS8）
- [ ] 密钥加密解密（含 SM4）
- [ ] 自签名证书生成
- [ ] 证书解析和验证（含指纹、公钥提取）
- [ ] 基本格式转换

### Phase 2: 高级功能
- [ ] CSR 完整支持（生成、解析、验证）
- [ ] CA 签发功能（三种模式）
- [ ] 证书链验证
- [ ] PFX/PKCS#12 支持

### Phase 3: 扩展功能（按需）
- [ ] CRL 支持
- [ ] 批量操作
- [ ] OCSP 支持
- [ ] 时间戳服务

---

## 8. 技术参考

### 8.1 已有代码结构
```
Cert.Utils.Core/X509/
- AsymmetricKeyParameter.cs (基类)
- AsymmetricPrivateKeyParameter.cs
- AsymmetricPublicKeyParameter.cs
- AsymmetricCipherKeyPair.cs
- Certificate.cs
- CertificateSigningRequest.cs
- CertificateRevocationList.cs
- X509Name.cs
- Enums/KeyUsage.cs
```

### 8.2 依赖库
- **BouncyCastle.Cryptography** - 核心加密库（支持商密算法）
- **Swashbuckle.AspNetCore** - Swagger/OpenAPI 文档

### 8.3 商密算法实现
- **SM2**: 使用 BouncyCastle 的 `ECKeyParameters` + `GMObjectIdentifiers.sm2p256v1`
- **SM3**: 使用 BouncyCastle 的 `SM3Digest`
- **SM4**: 使用 BouncyCastle 的 `SM4Engine`
- **签名算法**: SM3WithSM2Signer

---

## 变更记录

| 日期 | 版本 | 修改内容 | 修改人 |
|------|------|----------|--------|
| 2025-10-15 | v0.1 | 初始草案 | - |
| 2025-10-15 | v0.2 | 添加密钥加密解密、签发证书三种模式、整合指纹和公钥提取 | - |
| 2025-10-15 | v0.3 | 确认商密支持、无存储需求、无认证、纯API服务 | - |

---

## 备注

这是一个初步的需求文档草案，请根据实际需求：
1. 补充和完善各个功能模块的详细需求
2. 确认优先级和实施计划
3. 明确待确认事项
4. 添加具体的业务场景和用例
5. 确定技术选型细节

**请逐步审阅并提供反馈！**
