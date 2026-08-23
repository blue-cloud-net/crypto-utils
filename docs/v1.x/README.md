# Crypto Utils v1.x（浏览器本地运算与扩展功能）

> v1.x 是平台在 v0.2 之后的未来版本，核心是 **Browser 本地运算**（v1.1）与按需扩展功能。
>
> 版本定义：v0.1（基础，已交付）→ v0.2（核心功能完整化，进行中）→ **v1.x（浏览器本地运算与扩展，未来）**。
>
> 更新日期：2026-08-23

## v1.1 Browser 本地运算

> 目标：基于 Web Crypto API + node-forge 实现浏览器本地运算，替换 `ServiceFactory` 的 browser 占位；敏感数据不离开浏览器。

- [ ] **Browser 本地运算**：基于 Web Crypto API + node-forge 实现 `Browser{Module}Service`（替换 `ServiceFactory` 占位）
- [ ] SM2 / SM3 / SM4 浏览器端支持（需 WASM 方案）
- [ ] 各页面 browser 模式真实实现 + 云端 / 本地结果一致性验证

## 其他扩展（按需）

- [ ] CRL 完整支持页面（生成 / 解析 / 吊销检查）
- [ ] 批量操作
- [ ] OCSP 支持
- [ ] DNS / HTTP 网络工具
- [ ] Docker 容器化部署
- [ ] 工程优化：Element Plus 按需引入、UnoCSS、移动端响应式细化

## 文档导航

- 全项目总览：[../roadmap.md](../roadmap.md)
- v0.1 交付记录：[../v0.1/](../v0.1/)
- v0.2 规划：[../v0.2/README.md](../v0.2/README.md)
