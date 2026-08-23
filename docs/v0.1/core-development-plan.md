# Crypto.Utils.Core 实施计划

> 本文件记录 `Crypto.Utils.Core` 模块「继续完善」工作的分阶段实施计划与执行结果，与 `core-roadmap.md`（进度总览）配套。
> 执行期间：2026-08-14

## 目标

补齐 `Crypto.Utils.Core` 的扩展参数支持、PFX/PKCS#12 与完整测试体系（含离线素材 + 三类对照测试），并重构测试基础设施（TestSupport、xUnit）。

## 已确认决策

- `cmd/` → `scripts/`、`test/` → `tests/` 目录重命名
- `Directory.Packages.props` 启用 `ManagePackageVersionsCentrally`（新项目 `PackageReference` 不带版本）
- 删除 `Crypto.Utils.TestUtils`，新建 `tests/Crypto.Utils.TestSupport`（静态 CLI 封装）
- 测试框架：NUnit → **xUnit + FluentAssertions**
- SM2/3/4 互操作走 tongsuo（`/opt/tongsuo/bin/tongsuo`，`TONGSUO_PATH` 覆盖）；RSA/EC/DSA 走 openssl
- 工具缺失时测试**失败**（`CliToolGuard`），不跳过
- 素材：离线生成并提交（含 keys/pfx，密码 `test1234`），短有效期 + 定期重新生成；断言字段不依赖「当前有效」

---

## 阶段 0：目录重命名 + 包管理（已完成）

| 步骤 | 内容 | 状态 |
|------|------|------|
| 0.1 | `cmd/`→`scripts/`、`test/`→`tests/` | ✅ |
| 0.2 | 脚本内 `test/data`→`tests/data`、`cmd/`→`scripts/` 引用更新 | ✅ |
| 0.3 | `tests/data/*/README.md` 引用更新 | ✅ |
| 0.4 | `.gitignore` 移除对 `tests/data/keys` 的忽略（data 全部纳入版本控制） | ✅ |
| 0.5 | `crypto-utils.slnx` 路径 `test/`→`tests/` | ✅ |
| 0.6 | `docs/development-guide.md` 更新（`tests/data`、TestSupport） | ✅ |
| 0.7 | 确认 CPM 已启用 | ✅ |

## 阶段 1：测试基础设施（TestSupport 项目）（已完成）

| 步骤 | 内容 | 状态 |
|------|------|------|
| 1.1 | `tests/Crypto.Utils.TestSupport` 项目（`<PackageReference Include="CliWrap" />` 不带版本） | ✅ |
| 1.2 | 静态 `OpenSslCli`：密钥/证书/CSR/CRL/PFX 生成与解析、加解密与签名互操作 | ✅ |
| 1.3 | 静态 `TongsuoCli`：SM2 证书/CSR/密钥、SM2/3/4 签名/摘要/加解密 | ✅ |
| 1.4 | `CliToolGuard`：工具不可用则 `Assert.Fail` | ✅ |
| 1.5 | 迁移 3 个 SM 互操作测试到 `TongsuoCli` | ✅ |
| 1.6 | 删除 `tests/Crypto.Utils.TestUtils`；更新 `Core.Tests.csproj` 引用 | ✅ |

## 阶段 2：离线测试素材 + 生成脚本（已完成）

| 步骤 | 内容 | 状态 |
|------|------|------|
| 2.1 | `scripts/generate-test-certs.sh`：RSA/EC 含扩展自签名、CA+叶子链、极简自签名 | ✅ |
| 2.2 | `scripts/generate-test-crl.sh`：CRL（吊销条目 1111/2222 + 原因） | ✅ |
| 2.3 | `scripts/generate-test-pfx.sh`：私钥+证书、私钥+证书链（密码 test1234） | ✅ |
| 2.4 | `scripts/generate-test-sm-certs.sh`（tongsuo）：SM2 证书/CSR | ✅ |
| 2.5 | 每目录 README 记录精确生成参数作为断言依据 | ✅ |

## 阶段 3：Core 功能补全（已完成）

| 步骤 | 内容 | 状态 |
|------|------|------|
| 3.1 | `X509/Models/X509ExtensionOptions.cs` + `BasicConstraintsOptions` | ✅ |
| 3.2 | `X509/Extensions/X509ExtensionBuilder.cs`（`Apply` + `BuildX509Extensions`） | ✅ |
| 3.3 | `Certificate.GenerateSelfSigned/SignCsr/SignPublicKey` 增加可选 `X509ExtensionOptions?` | ✅ |
| 3.4 | `CertificateSigningRequest.Generate` 增加可选 `X509ExtensionOptions?`（`extensionRequest`） | ✅ |
| 3.5 | `X509/Utils/PfxUtils.cs`（`ToPfx`/`FromPfx`）+ `X509/Models/PfxBundle.cs`（PFX 逻辑从 `Certificate` 拆离） | ✅ |

## 阶段 4：单元测试（已完成）

| 步骤 | 内容 | 状态 |
|------|------|------|
| 4.1 | `AsymmetricKeyPairTests`（生成、PEM/DER 往返、素材解析对照） | ✅ |
| 4.2 | `RsaCryptoTests` / `EcdsaCryptoTests` / `DsaCryptoTests` / `AesCryptoTests` | ✅ |
| 4.3 | `CertificateTests`（素材断言、扩展自签名、SignCsr/SignPublicKey、PFX 往返） | ✅ |
| 4.4 | `CertificateSigningRequestTests` / `CertificateRevocationListTests` | ✅ |

## 阶段 5：对照测试（已完成）

| 步骤 | 内容 | 状态 |
|------|------|------|
| 5.1 | A 工具生成→代码解析：`CertificateOpenSslInteropTests` / `CsrOpenSslInteropTests` | ✅ |
| 5.2 | B 代码生成→工具解析：openssl/tongsuo `x509/req/crl/pkcs12 -text` 断言 | ✅ |
| 5.3 | C 素材双方解析对照 | ✅ |
| 5.4 | `RsaEcdsaOpenSslInteropTests`（RSA/ECDSA 加解密+签名交叉验证） | ✅ |

## 阶段 6：代码质量收尾（已完成）

| 步骤 | 内容 | 状态 |
|------|------|------|
| 6.1 | 中文异常消息统一英文（句尾句号） | ✅ |
| 6.2 | 修复 `Certificate.AuthorityKeyIdentifier` 空 keyid NRE、`GeneralName` IP 编解码、`RevokedCertificate.RevocationReason`（DerEnumerated）、`RevokedCertificatesCount` 语义 | ✅ |
| 6.3 | `IsRevoked` 改用 `ArgumentNullException.ThrowIfNull`；补 CRL `SignatureAlgorithmOid` 文档 | ✅ |
| 6.4 | 修复 `AsymmetricPrivateKeyParameter.GetPublicKey` RSA 公钥指数 bug；清理未用字段；消除全部构建警告 | ✅ |

## 阶段 7：文档产出 + roadmap 同步（已完成）

| 步骤 | 内容 | 状态 |
|------|------|------|
| 7.1 | 创建 `docs/core-roadmap.md`（进度总览） | ✅ |
| 7.2 | 创建 `docs/core-development-plan.md`（本文件） | ✅ |
| 7.3 | 更新 `docs/roadmap.md`（勾选 Phase 1 Core 已完成项） | ✅ |

---

## 验证结果

- `dotnet build crypto-utils.slnx`：0 错误、0 警告
- `dotnet test tests/Crypto.Utils.Core.Tests`：**293 个用例全绿（net8.0 / net10.0）**，含素材解析 + 三类对照 + openssl/tongsuo 互操作
- 素材脚本可重复执行；README 参数与脚本参数一致

## 后续建议

- 依赖 Core 的 Api 层接入（`KeyService.ConvertPkcsFormatAsync`、私钥加解密等）
- Core 可选增强见 `core-roadmap.md`「待办计划」
