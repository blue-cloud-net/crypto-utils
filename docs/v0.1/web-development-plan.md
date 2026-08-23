# Crypto.Utils Web 首期（云模式）实施计划

> 本文件记录 Web（前端）+ Api 收尾的分阶段实施计划与执行结果，与 `web-roadmap.md`（进度总览）配套。
> 执行期间：2026-08-16

## 目标

完成 Api 层收尾（PKCS 格式转换、私钥加解密）并正式接通 Host；搭建前端工程（Vue 3 + Vite + TS + Element Plus + vue-i18n + Pinia + Vue Router + Axios）；实现「后端运算（云）」模式的密钥/证书/CSR 常用页面；支持中英双语与「浅色/深色/跟随系统」三态主题；预留双模式架构（Browser 二期实现）。

## 已确认决策

- 首期前端只做 **cloud（后端运算）** 模式；`ServiceFactory` + 接口已就位，browser 实现返回占位（抛 `BrowserNotImplementedError`），二期用 Web Crypto + node-forge 实现
- 首期功能范围：密钥（生成/解析/PEM↔DER 转换/PKCS#1↔PKCS#8/私钥加解密）、证书（解析/自签名生成）、CSR（生成/解析）
- SM2/SM3/SM4 首期仅云端支持；browser 模式页面显示「仅支持云端」降级提示
- 主题：`system / light / dark` 三态，默认跟随系统，localStorage 持久化
- 语言：中英双语（vue-i18n），默认跟随浏览器语言，localStorage 持久化
- 页面组织：模块内多路由独立页面（`/key/generate`、`/key/parse`、`/key/convert` …）
- 自签名证书表单为最简版（CN + 有效期 + 私钥 + 签名算法），KU/EKU/SAN/序列号二期
- 结果交互：展示 + 一键复制 + 下载文件 + 示例填充
- 开发端口：前端 Vite `5178`（避开 5173 的其它项目），后端 http `5072`

---

## 阶段 0：Api 收尾 + Host 接通（已完成）

| 步骤 | 内容 | 状态 |
|------|------|------|
| 0.1 | Core 新增 `AsymmetricPrivateKeyParameter.ToPkcs8Encrypted(password, algorithm)`：基于 PBES2 + PBKDF2（HMAC-SHA-256）+ AES-CBC 生成 `-----BEGIN ENCRYPTED PRIVATE KEY-----`，与 OpenSSL 互操作 | ✅ |
| 0.2 | 实现 `KeyService.ConvertPkcsFormatAsync`：PKCS#1↔PKCS#8（EC 映射 SEC1、DSA 传统格式），目标 PKCS#8 + 密码时走加密导出 | ✅ |
| 0.3 | 实现 `KeyService.EncryptPrivateKeyAsync`（传统加密 PEM）/ `DecryptPrivateKeyAsync` | ✅ |
| 0.4 | `Host/Program.cs` 接通 `AddCertificateServices`、`AddSwaggerDocumentation`、`UseExceptionHandling`、Swagger（开发）；`UseHttpsRedirection` 仅生产；SpaProxy 属性配置（`SpaProxyServerUrl`/`SpaProxyLaunchCommand`） | ✅ |

> 说明：`Microsoft.AspNetCore.SpaProxy` 7.0.20 通过 `[assembly: HostingStartup]`（`SpaHostingStartup`）自动注册 SPA 代理中间件，读取构建生成的 `spa.proxy.json`，**不需要**在 Program.cs 调用 `UseSpaDevelopmentServer`。`SpaProxyServerUrl`/`SpaProxyLaunchCommand` 是 **MSBuild 属性**（写入 spa.proxy.json），不是 appsettings 配置。

## 阶段 1：前端工程基建（已完成）

| 步骤 | 内容 | 状态 |
|------|------|------|
| 1.1 | 依赖：`element-plus`、`@element-plus/icons-vue`、`vue-router@4`、`pinia`、`axios`、`vue-i18n@9` | ✅ |
| 1.2 | `vite.config.ts`：`/api`、`/health` 代理 → `http://localhost:5072`；别名 `@` → `src`（同步 `tsconfig.app.json` paths）；端口 5178 | ✅ |
| 1.3 | 目录结构：`api/client`、`services/{interfaces,cloud,browser}`、`models/{common,key,certificate,csr}`、`views/{key,cert,csr}`、`components`、`layouts`、`router`、`stores`、`locales`、`utils` | ✅ |
| 1.4 | 类型模型：`ApiResponse<T>`、`SubjectInfo`（含 `normalizeSubject`）、密钥/证书/CSR 的 Options/Result 类型（字段对齐 `api-reference.md`） | ✅ |
| 1.5 | `CloudApiClient`：Axios 封装，统一解包 `ApiResponse<T>`，失败抛 `ApiError(message, errorCode)` | ✅ |
| 1.6 | 服务接口 `IKeyService` / `ICertificateService` / `ICsrService`（返回 `Promise<T>`） | ✅ |
| 1.7 | `CloudKeyService` / `CloudCertificateService` / `CloudCsrService` 实现 | ✅ |
| 1.8 | `ServiceFactory`：`Mode = 'cloud' | 'browser'`；browser 返回 Proxy 占位（方法抛 `BrowserNotImplementedError`） | ✅ |
| 1.9 | stores：`useAppStore`（模式）、`useThemeStore`（三态主题 + 系统监听）、`useLocaleStore`（语言） | ✅ |
| 1.10 | locales：`zh-CN` / `en-US` 语言包 + vue-i18n 实例；Element Plus 内置文案随语言切换（`ElConfigProvider`） | ✅ |
| 1.11 | utils：`download`（文本/Base64 二进制下载 + 文件名规范）、`format`（日期/天数格式化）、`samples`（示例素材，取自 tests/data） | ✅ |

## 阶段 2：布局 + 主题 + i18n 骨架（已完成）

| 步骤 | 内容 | 状态 |
|------|------|------|
| 2.1 | `AppLayout.vue`：可折叠侧边栏（密钥/证书/CSR 分组）+ 顶栏（折叠、标题、模式 radio、主题下拉、语言下拉） | ✅ |
| 2.2 | 路由：`/` 首页 + 7 个工具页，`AppLayout` 为父布局；`document.title` 随 i18n 翻译 | ✅ |
| 2.3 | 主题：`html.dark` + Element Plus 暗色 CSS 变量；`system` 态监听 `prefers-color-scheme`；默认跟随系统；localStorage 持久化 | ✅ |
| 2.4 | 通用组件：`KeyTextArea`（拖拽/粘贴）、`CopyableText`、`ResultPanel`、`DownloadButton`、`SampleFillButton`、`ModeBadge`、`ModeNotice`（降级提示） | ✅ |

## 阶段 3：功能页面（cloud 模式，已完成）

| 步骤 | 内容 | 状态 |
|------|------|------|
| 3.1 | `KeyGenerateView`（RSA/EC/SM2/DSA + 大小/曲线 + PEM/DER + 私/公钥复制/下载） | ✅ |
| 3.2 | `KeyParseView`（算法/公私/大小/曲线/指纹/参数表 + 示例填充） | ✅ |
| 3.3 | `KeyConvertView`（三块：PEM↔DER、PKCS#1↔PKCS#8 带密码、私钥加解密） | ✅ |
| 3.4 | `CertParseView`（版本/序列号/主体/颁发者/有效期含剩余天数/指纹/公钥/扩展 SKI/AKI/KU/EKU/SAN/CRLDP） | ✅ |
| 3.5 | `CertSelfSignedView`（最简：CN + 有效期默认+1年 + 私钥 + 签名算法） | ✅ |
| 3.6 | `CsrGenerateView`（CN 必填 + O/OU/C/ST/L/E + 私钥 + 签名算法） | ✅ |
| 3.7 | `CsrParseView`（主体/签名算法/公钥/扩展） | ✅ |
| 3.8 | 每页统一：browser 模式显示 `ModeNotice`；提交 loading；错误走 `ResultPanel` | ✅ |

## 阶段 4：验证（已完成）

| 步骤 | 内容 | 状态 |
|------|------|------|
| 4.1 | `dotnet build crypto-utils.slnx`：0 错误 0 警告 | ✅ |
| 4.2 | `dotnet test`：341 用例全绿（net8.0 / net10.0） | ✅ |
| 4.3 | curl 验证：PKCS#8→PKCS#1、私钥加密、加密 PKCS#8（PBES2/AES-256）、私钥解密；加密 PKCS#8 经 `openssl pkey/pkcs8` 可解析（互操作） | ✅ |
| 4.4 | UI 浏览器闭环：密钥生成→解析、自签名证书生成、证书解析、CSR 生成，均正常 | ✅ |
| 4.5 | 主题：浅色/深色/跟随系统切换 + localStorage 持久化 + `html.dark` 应用 | ✅ |
| 4.6 | 语言：中/EN 切换即时生效，Element Plus 内置文案同步，持久化 | ✅ |
| 4.7 | 模式：切到 browser 显示降级提示，切回 cloud 恢复 | ✅ |

---

## 验证结果

- `dotnet build crypto-utils.slnx`：0 错误 0 警告（含前端 `vue-tsc -b` + `vite build`）
- `dotnet test tests/Crypto.Utils.Core.Tests`：**341 个用例全绿（net8.0 / net10.0）**
- 浏览器手动验证 7 个工具页 + 主题 + 双语 + 双模式降级提示全部通过

## 后续建议

- **Api 测试项目**：可新增 `tests/Crypto.Utils.Api.Tests` 覆盖 PKCS 转换/私钥加解密往返（本次以 `.http` + curl 手动验证）
- **Browser 模式（二期）**：基于 Web Crypto + node-forge 实现本地运算，替换 `ServiceFactory` 的占位
- **SM 浏览器端**：需 WASM 方案（首期仅云端）
- **二期待办**：PFX 页面、CA 签发（sign-csr/sign-publickey/sign-generate）、证书链构建/验证、CRL 页面、自签名高级扩展（KU/EKU/SAN/序列号）、CSR 扩展字段、HTTPS 检测、开发工具集、UnoCSS（如无原子化 CSS 需求可后置）
