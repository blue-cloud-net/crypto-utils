# browser 服务实现（二期）

本目录存放「浏览器本地运算」的服务实现（`Browser{Module}Service.ts`），
二期将基于 **Web Crypto API + node-forge** 实现：

- Web Crypto：RSA/EC 加解密、签名、AES、哈希、PBKDF2
- node-forge：X.509 / CSR / PEM / ASN.1 编解码

> 说明：浏览器原生能力不支持 SM2/SM3/SM4，SM 相关功能在 browser 模式下显示「仅支持云端」。
> 当前首期仅提供 cloud 模式，本目录留空，ServiceFactory 对 browser 模式返回占位实现（抛 `BrowserNotImplementedError`）。
