# Crypto Utils v0.2 开发计划

> 与 [README.md](README.md)（版本总览 + 进度）配套。
> 目标：核心功能完整化 + v0.1 遗留基础能力接线。
> 更新日期：2026-08-23

## 定位与原则

- 纯 **cloud（后端运算）** 模式；browser 模式保持降级提示（v1.1 实现）
- 遵循 [.github/copilot-instructions.md](../../.github/copilot-instructions.md) 规范（CancellationToken、结构化日志、XML 注释、Conventional Commits 等）
- 后端接线遵循「Controller 薄、Service 接 Core」模式；前端遵循现有 `ServiceFactory` / Cloud 服务 / 通用组件模式

---

## 阶段 0：v0.1 遗留基础能力接线（后端）

> 目标：把 Core 已就绪、Api 请求模型已定义但未接线的能力接通。

| 步骤 | 内容 | 状态 |
|------|------|------|
| 0.1 | `CsrService.GenerateCsrAsync` 接线 `CsrGenerateRequest.Extensions` → `CertificateSigningRequest.Generate(..., extensions)` | ⬜ |
| 0.2 | `CsrService.ParseCsrInfoAsync` 输出扩展详情（SAN / KU / EKU）到 `CsrParseResponse` | ⬜ |
| 0.3 | `CertificateService.GenerateSelfSignedCertificateAsync` 接线 KU / EKU / SAN | ⬜ |
| 0.4 | `CertificateService.SignByCsrAsync` / `SignByPublicKeyAsync` / `SignAndGenerateAsync` 接线 `Extensions` | ⬜ |
| 0.5 | `CertificateService.ConvertCertificateAsync` 补齐 PFX 分支（PEM / DER / PFX 互转） | ⬜ |
| 0.6 | Core `X509ExtensionOptions` 补 `CertificatePolicies`、AIA（OCSP / CA Issuers）写支持（前置增强，供 0.1-0.4 使用） | ⬜ |

---

## 阶段 1：CSR 完整支持

> 依赖：阶段 0。

| 步骤 | 内容 | 状态 |
|------|------|------|
| 1.1 | `CsrGenerateView` 增加扩展字段表单（SAN / KU / EKU / CRLDP）；`CsrGenerateOptions` 加 `extensions` | ⬜ |
| 1.2 | 新增 `CsrVerifyView`（`/csr/verify`）；`ICsrService` 加 `verifyCsr` | ⬜ |
| 1.3 | `CsrParseView` 展示扩展详情 | ⬜ |
| 1.4 | 路由 + `AppLayout` 导航 + `HomeView` 卡片 + i18n（zh-CN / en-US） | ⬜ |

---

## 阶段 2：CA 签发三模式 + 自签名高级扩展

> 依赖：阶段 0。

| 步骤 | 内容 | 状态 |
|------|------|------|
| 2.1 | 新增 `CertCaSignView`（`/cert/ca-sign`），三模式 Tab（sign-csr / sign-publickey / sign-generate） | ⬜ |
| 2.2 | `CertSelfSignedView` 升级：支持高级扩展（KU / EKU / SAN / 序列号） | ⬜ |
| 2.3 | `ICertificateService` 增加三个签发方法；扩展字段模型与 CSR 复用 | ⬜ |
| 2.4 | 路由 + 导航 + 首页 + i18n | ⬜ |

---

## 阶段 3：PFX / PKCS#12 支持

> 依赖：阶段 0（0.5）。

**后端**

| 步骤 | 内容 | 状态 |
|------|------|------|
| 3.1 | 新增 `PfxCreateRequest`（证书 + 私钥 + 链 + 密码 + friendlyName）/ `PfxExtractRequest` 模型 | ⬜ |
| 3.2 | `CertificateService` 新增 `CreatePfxAsync` / `ExtractPfxAsync`（复用 Core `PfxUtils.ToPfx` / `FromPfx`） | ⬜ |
| 3.3 | 新增端点 `api/cert/pfx/create`、`api/cert/pfx/extract` | ⬜ |

**前端**

| 步骤 | 内容 | 状态 |
|------|------|------|
| 3.4 | 新增 `CertPfxView`（`/cert/pfx`），合成 / 提取两个 Tab | ⬜ |
| 3.5 | `ICertificateService` 增加 PFX 方法（或独立 `IPfxService`） | ⬜ |
| 3.6 | 路由 + 导航 + 首页 + i18n | ⬜ |

---

## 阶段 4：证书链构建与验证（PKIX 升级）

**Core**

| 步骤 | 内容 | 状态 |
|------|------|------|
| 4.1 | 新增 `CertificateChainUtils`：基于 BC `PkixBuilderParameters` / `TrustAnchor` / `CertPathBuilder` 构建 + `CertPathValidator` 验证（签名 / 有效期 / 路径长度 / 信任根） | ⬜ |
| 4.2 | 单元测试：多级链构建、各失败场景（过期 / 根不受信任 / 签名损坏 / 路径长度） | ⬜ |

**Api**

| 步骤 | 内容 | 状态 |
|------|------|------|
| 4.3 | `CertificateChainService.BuildChainAsync` / `VerifyChainAsync` 改用 Core 标准实现（保留现有响应模型，替换手写 Subject + Serial 匹配） | ⬜ |
| 4.4 | 链验证详情增强（每级签名 / 有效期 / 路径长度校验） | ⬜ |

**前端**

| 步骤 | 内容 | 状态 |
|------|------|------|
| 4.5 | 新增 `CertChainView`（`/cert/chain`），构建 / 验证两个 Tab（多证书输入） | ⬜ |
| 4.6 | 新增 `IChainService`（或并入 `ICertificateService`）+ Cloud 实现 | ⬜ |
| 4.7 | 路由 + 导航 + 首页 + i18n | ⬜ |

---

## 阶段 5：HTTPS 证书在线检测与提取（进阶版）

**Core**

| 步骤 | 内容 | 状态 |
|------|------|------|
| 5.1 | 新增 `RemoteCertificateFetcher`：`TcpClient` + `SslStream` 抓取远程证书链（host / port / SNI / 超时 / CancellationToken） | ⬜ |
| 5.2 | TLS 信息提取（`SslProtocol` 版本、加密套件） | ⬜ |
| 5.3 | 主机名匹配检查（输入 host vs SAN / CN） | ⬜ |
| 5.4 | 吊销状态：AIA 提取 OCSP URL（轻量单请求 + 超时保护）/ 展示 CRLDP | ⬜ |
| 5.5 | 单元测试：本地 HTTPS 测试端点 | ⬜ |

**Api**

| 步骤 | 内容 | 状态 |
|------|------|------|
| 5.6 | 新增 `RemoteCertificateService` + 端点 `api/network/certificate-check`（host / port / sni） | ⬜ |
| 5.7 | `RemoteCertificateCheckResponse`（证书链 + TLS 版本 / 套件 + 域名匹配 + 有效期 + 吊销状态） | ⬜ |
| 5.8 | 安全：端口白名单、超时上限、仅接受域名 / IP 禁 URL、SSRF 防护 | ⬜ |

**前端**

| 步骤 | 内容 | 状态 |
|------|------|------|
| 5.9 | 新增 `NetworkHttpsCheckView`（`/network/https-check`） | ⬜ |
| 5.10 | 新增 `INetworkService` + Cloud 实现 | ⬜ |
| 5.11 | 路由 + 导航 + 首页 + i18n | ⬜ |

---

## 阶段 6：开发工具集

**Core**

| 步骤 | 内容 | 状态 |
|------|------|------|
| 6.1 | `JwtUtils`：编解码（header / payload + Base64URL）+ HS256/384/512 签名验证（RS 系列可选扩展） | ⬜ |
| 6.2 | `HashUtils`：SHA-1/256/384/512、HMAC-SHA256/384/512、复用 SM3 | ⬜ |
| 6.3 | `CodecUtils`：Base64 / Base64URL / Hex / URL 编解码 | ⬜ |
| 6.4 | `UuidUtils`：UUID v4（`SecureRandom` 填充随机位） | ⬜ |
| 6.5 | 单元测试：各工具正常 / 边界 / 错误路径 | ⬜ |

**Api**

| 步骤 | 内容 | 状态 |
|------|------|------|
| 6.6 | 新增 `DevToolsService` + `DevToolsController`：`api/dev/jwt/{encode,decode,verify}`、`api/dev/hash`、`api/dev/codec`、`api/dev/uuid` | ⬜ |
| 6.7 | 请求 / 响应模型 + 参数校验 | ⬜ |

**前端**

| 步骤 | 内容 | 状态 |
|------|------|------|
| 6.8 | 新增 `DevToolsView`（`/dev/tools`），JWT / 哈希 / 编解码 / UUID 四 Tab | ⬜ |
| 6.9 | 新增 `IDevToolsService` + Cloud 实现 | ⬜ |
| 6.10 | 路由 + 导航 + 首页 + i18n | ⬜ |

---

## 阶段 7：测试工程与验证

| 步骤 | 内容 | 状态 |
|------|------|------|
| 7.1 | 搭建 `tests/Crypto.Utils.Api.Tests`（xUnit + FluentAssertions，Service 级单测为主） | ⬜ |
| 7.2 | 各功能 Api 测试：正常 / 边界 / 错误路径（往返、错误密码、非法输入、超时） | ⬜ |
| 7.3 | 更新 `api-reference.md`（新端点）并同步 [../roadmap.md](../roadmap.md)、[../v0.1/web-roadmap.md](../v0.1/web-roadmap.md) 勾选 | ⬜ |
| 7.4 | 验证：`dotnet build` 0 错误 0 警告；`dotnet test` 全绿；浏览器闭环各新页面（cloud 模式） | ⬜ |

---

## 验证标准

- `dotnet build crypto-utils.slnx`：0 错误 0 警告（含 `vue-tsc` + `vite build`）
- `dotnet test`：Core 341 例 + 新增用例全绿
- 新增 Api 测试项目跑通
- 浏览器手动闭环：CSR 含扩展生成 → 验证、CA 三模式签发 → 解析、PFX 合成 → 提取、证书链构建 / 验证、HTTPS 检测、开发工具各 Tab

## 技术决策（v0.2 补充）

- **开发工具集**：基础全覆盖（JWT HS256/384/512、SHA-1/256/384/512 + HMAC + SM3、Base64 / Base64URL / Hex / URL、UUID v4）；RS 系列列为可选扩展
- **HTTPS 检测**：进阶版（TLS 版本、加密套件、OCSP + CRLDP 吊销信息）；吊销状态为轻量 OCSP 单请求，与 v1.x 独立 OCSP 工具界定
- **证书链**：Core 新增标准 PKIX 实现（BC `PkixBuilderParameters` / `TrustAnchor` / `CertPathBuilder` / `CertPathValidator`），替换 Api 层手写实现
- **模块归属**：CSR 验证挂 csr 模块；CA 签发 / PFX / 证书链挂 cert 模块；HTTPS 检测挂新 network 模块；开发工具挂新 dev 模块
- **JWT**：仅 HS 系列（基础全覆盖），RS 列为可选扩展
