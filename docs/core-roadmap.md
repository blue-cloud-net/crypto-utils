# Crypto.Utils.Core 开发路线图

> 本文件是 `Crypto.Utils.Core` 模块的独立计划与进度跟踪文档，与 `roadmap.md`（全项目）保持同步。
> 更新日期：2026-08-14

## 定位

`Crypto.Utils.Core` 是平台的密码学核心类库，基于 BouncyCastle 2.6.2，提供密钥、证书、CSR、CRL、商密算法等能力。本模块**无状态、无持久化**，敏感数据仅在内存中处理。

---

## 当前状态（已完成）

### 非对称密钥

- [x] `AsymmetricKeyPair`：RSA / EC / SM2 / DSA 密钥对生成
- [x] `AsymmetricKeyParameter` / `AsymmetricPrivateKeyParameter` / `AsymmetricPublicKeyParameter`：PEM/DER 导入导出、加密 PEM、公钥推导
- [x] DSA 参数生成（FIPS 186-3，L=2048/3072 配 SHA-256）

### 非对称/对称算法

- [x] `RsaCrypto`：加解密（默认 OAEP-SHA256，可选 PKCS#1 v1.5）；签名验签（默认 PSS，可选 PKCS#1 v1.5）
- [x] `EcdsaCrypto`：ECDSA 签名验签（DER 编码）；ECDH 共享密钥（同曲线校验）
- [x] `DsaCrypto`：DSA 签名验签（DER 编码）
- [x] `AesCrypto`：CBC / CFB / OFB（自动 IV）+ GCM 认证加密（12 字节 Nonce + 16 字节 Tag）；**禁用 ECB**
- [x] `SM2` / `SM3` / `SM4` 商密算法封装

### X.509 对象模型

- [x] `Certificate`：解析、指纹、有效期、扩展读取（SAN/KU/EKU/SKI/AKI/CRLDP/AIA/策略）
- [x] `Certificate` 生成：`GenerateSelfSigned` / `SignCsr` / `SignPublicKey`，支持可选 `X509ExtensionOptions`
- [x] `X509ExtensionOptions` 值对象：KU / EKU / SAN / BasicConstraints / SKI / AKI / CRL 分发点（含参数校验）
- [x] `X509ExtensionBuilder`：把扩展应用到证书生成器或构建 CSR 扩展集合
- [x] `CertificateSigningRequest`：生成（含扩展，经 `extensionRequest`）、解析、验签
- [x] `CertificateRevocationList`：生成（含吊销原因）、解析、`IsRevoked`
- [x] PFX / PKCS#12：`PfxUtils.ToPfx` / `FromPfx`（含私钥与证书链、密码保护、`PfxBundle`）
- [x] `X509NameParser`：兼容 OpenSSL 斜杠风格与 RFC 2253 风格的可分辨名称解析
- [x] 枚举体系与 Helpers：`KeyUsage`、`ExtendedKeyUsage`、`GeneralNameType`、`CertificatePolicy`、`CertificateRevocationReason`（含中英文资源）

### 测试基础设施

- [x] 目录重命名：`cmd/`→`scripts/`、`test/`→`tests/`
- [x] 测试框架：NUnit → **xUnit + FluentAssertions**
- [x] `tests/Crypto.Utils.TestSupport`：静态 `OpenSslCli`（openssl）、`TongsuoCli`（`/opt/tongsuo/bin/tongsuo`，`TONGSUO_PATH` 可覆盖）、`CliToolGuard`
- [x] 删除 `tests/Crypto.Utils.TestUtils`
- [x] 离线素材：`tests/data/{keys,certs,csrs,crls,pfx}`（openssl / tongsuo 生成，固定参数，含 README 断言依据）
- [x] 素材生成脚本：`scripts/generate-test-{keys,certs,crl,pfx,sm-certs}.sh`

### 测试

- [x] RSA / EC / DSA / SM2 密钥单元测试（生成、PEM/DER 往返、参数提取、素材解析对照）
- [x] RSA 加解密（OAEP + PKCS#1 v1.5）、签名验签（PSS + PKCS#1 v1.5）单元测试
- [x] ECDSA 签名、ECDH 共享密钥单元测试
- [x] DSA 签名验签单元测试
- [x] AES-CBC / AES-GCM 单元测试（含篡改失败、禁 ECB）
- [x] `Certificate` 单元测试（素材断言、含扩展自签名、SignCsr、SignPublicKey、PFX 往返）
- [x] `CertificateSigningRequest` / `CertificateRevocationList` 单元测试
- [x] 三类对照测试：A 工具生成→代码解析；B 代码生成→工具解析；C 素材双方解析对照
- [x] OpenSSL 互操作（RSA/ECDSA 加解密+签名）；Tongsuo 互操作（SM2/3/4）
- [x] 测试全绿：**293 个用例（net8.0 / net10.0）**

---

## 待办计划

### Core 增强（可选/后续）

- [ ] AES 更多模式互操作测试（CFB / OFB 与 openssl 对照）
- [ ] RSA-PSS 不同哈希组合的 OpenSSL 交叉验证（SHA-384/512）
- [ ] 大文件/流式哈希性能基准（`SM3.HashDataAsync` 等）
- [ ] `X509ExtensionOptions` 扩展更多字段（如 `CertificatePolicies`、AIA 的 OCSP/CA Issuers）

### 依赖 Core 的下一阶段（属于 Api / Host / UI，非 Core）

- [ ] `KeyService.ConvertPkcsFormatAsync`：PKCS#1 ↔ PKCS#8 格式转换（接入 Core 能力）
- [ ] `KeyService.EncryptPrivateKeyAsync` / `DecryptPrivateKeyAsync`：私钥加密/解密
- [ ] Api 层其余 Service 接入 Core 新增能力（扩展参数、PFX）

---

## 技术决策

- **无持久化**：所有操作无状态，不使用数据库或文件存储
- **加密 padding**：RSA 加密默认 OAEP-SHA256，签名默认 PSS；保留 PKCS#1 v1.5 可选项
- **ECDH**：返回原始共享密钥字节，KDF 由调用方决定
- **签名编码**：ECDSA / DSA 使用标准 DER 编码（与 OpenSSL 互操作）
- **DSA**：仅支持签名/验签（算法限制）；密钥长度 1024/2048/3072
- **AES**：禁用 ECB；GCM 使用 12 字节随机 Nonce + 16 字节认证标签
- **商密**：SM2/3/4 完整支持，互操作测试走 tongsuo
- **PFX**：基于 BouncyCastle `Pkcs12StoreBuilder`，默认含私钥与证书链
- **素材**：离线生成并提交（含私钥/PFX，密码 `test1234`），短有效期（约 1 年）+ 定期重新生成
- **暂不支持**：HSM、EdDSA、JKS、时间戳服务
