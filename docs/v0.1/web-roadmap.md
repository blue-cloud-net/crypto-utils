# Crypto.Utils Web 开发路线图（v0.1）

> 本文件是 Web（前端）+ Api 收尾在 **v0.1** 版本的计划与进度记录，与 [roadmap.md](../roadmap.md)（全项目总览）保持同步。
> 更新日期：2026-08-16
> v0.1 已交付完毕；后续版本计划见 [v0.2](../v0.2/README.md) 与 [v1.x](../v1.x/README.md)。

## 定位

Web 是平台的浏览器端 SPA（Vue 3 + Vite + TypeScript + Element Plus），与后端 Api/Host 配合，提供密钥、证书、CSR 等常用密码学工具。支持**后端运算（cloud）**与**浏览器本地运算（browser）**两种模式（browser 二期实现），支持中英双语与浅色/深色/跟随系统主题。

---

## 当前状态（已完成）

### Api 收尾（后端）

- [x] `KeyService.ConvertPkcsFormatAsync`：PKCS#1 ↔ PKCS#8（EC→SEC1、DSA 传统），目标 PKCS#8 + 密码时加密
- [x] `KeyService.EncryptPrivateKeyAsync` / `DecryptPrivateKeyAsync`：私钥加密（传统加密 PEM）/ 解密
- [x] Core 新增 `AsymmetricPrivateKeyParameter.ToPkcs8Encrypted`：PBES2 + PBKDF2（HMAC-SHA-256）+ AES-CBC，与 OpenSSL 互操作
- [x] `Host/Program.cs` 接通 DI、异常中间件、Swagger（开发）、SpaProxy 属性；`UseHttpsRedirection` 仅生产

### 前端工程基建

- [x] Vue 3 + Vite + TS + Element Plus + vue-i18n + Pinia + Vue Router + Axios
- [x] `CloudApiClient`：统一解包 `ApiResponse<T>`，失败抛 `ApiError(message, errorCode)`
- [x] 服务接口 `IKeyService` / `ICertificateService` / `ICsrService` + Cloud 实现
- [x] `ServiceFactory` 双模式工厂（browser 占位，抛 `BrowserNotImplementedError`）
- [x] 类型模型（models：common / key / certificate / csr，字段对齐 api-reference.md）
- [x] 主题三态（system/light/dark，默认跟随系统，localStorage 持久化，监听 prefers-color-scheme）
- [x] 中英双语（vue-i18n，默认随浏览器语言，Element Plus 文案同步）
- [x] 通用组件：KeyTextArea / CopyableText / ResultPanel / DownloadButton / SampleFillButton / ModeBadge / ModeNotice

### 功能页面（cloud 模式）

- [x] 首页：模块导航卡片
- [x] 密钥：生成（RSA/EC/SM2/DSA）、解析（算法/指纹/参数）、格式转换（PEM↔DER / PKCS#1↔PKCS#8 / 私钥加解密）
- [x] 证书：解析（版本/有效期/指纹/公钥/扩展）、自签名生成（最简）
- [x] CSR：生成（Subject + 私钥）、解析

### 验证

- [x] `dotnet build crypto-utils.slnx`：0 错误 0 警告
- [x] `dotnet test`：341 用例全绿（net8.0 / net10.0）
- [x] 加密 PKCS#8 与 OpenSSL 互操作验证
- [x] 浏览器闭环：密钥/证书/CSR 各页面 + 主题三态 + 双语切换 + browser 降级提示

---

## 后续版本

- Web 在 v0.2 的页面任务（PFX 页面、CA 签发、证书链、自签名高级扩展、CSR 扩展与验证、HTTPS 检测、开发工具集、Api 测试项目）见 [v0.2 开发计划](../v0.2/development-plan.md)。
- **Browser 本地运算**、SM 浏览器端 WASM 及工程优化（UnoCSS、移动端响应式、Element Plus 按需引入、docker compose）见 [v1.x 规划](../v1.x/README.md)。

---

## 技术决策

- **双模式**：前端通过 `ServiceFactory` 按 `Mode` 返回服务实现；接口返回 `Promise<T>` 不含 HTTP 细节，Cloud/Browser 实现可互换
- **首期仅 cloud**：browser 模式显示降级提示「当前功能仅支持云端」；SM 相关功能 browser 模式不提供
- **主题**：三态 system/light/dark，默认跟随系统，localStorage 持久化（`html.dark` + Element Plus 暗色变量）
- **语言**：中英双语，默认随浏览器语言，localStorage 持久化
- **端口**：前端 5178、后端 http 5072 / https 7273（Vite 代理 `/api` → 5072）
- **SpaProxy**：Microsoft.AspNetCore.SpaProxy 通过 HostingStartup 自动注册，读取构建生成的 `spa.proxy.json`
- **下载**：PEM 文本下载；DER 以 Base64 解码为二进制下载
- **暂不支持**：HSM、EdDSA、JKS、时间戳服务
