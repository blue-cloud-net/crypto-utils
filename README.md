# 🔐 Crypto Utils - 密码学工具平台

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet" alt=".NET 8.0"/>
  <img src="https://img.shields.io/badge/Vue-3.x-4FC08D?logo=vue.js" alt="Vue 3"/>
  <img src="https://img.shields.io/badge/TypeScript-5.x-3178C6?logo=typescript" alt="TypeScript"/>
  <img src="https://img.shields.io/badge/License-Apache_2.0-blue.svg" alt="License"/>
</p>

## 📖 项目简介

Crypto Utils 是一个功能全面、易用的密码学工具平台，专为开发者和测试人员设计，提供证书管理、密钥处理、CSR 生成、加解密等一站式密码学工具服务。本项目采用现代化的前后端分离架构，注重用户体验和数据安全，支持云端处理和本地浏览器模式双重选择，让您的敏感数据处理更加安全可控。

## ✨ 核心特性

### 🔑 密钥管理
- 支持 **RSA**、**ECDSA**、**SM2**（商密）、**DSA** 等多种非对称加密算法
- 密钥对生成，支持自定义密钥长度和椭圆曲线参数
- 密钥格式转换：**PKCS#1** ↔ **PKCS#8**，**PEM** ↔ **DER**
- 私钥解析并提取公钥，查看密钥详细信息

### 📜 证书服务
- **自签名证书生成**：快速创建测试用根证书
- **CA 签发证书**：支持 CSR 签发、公钥签发、完整证书链生成
- **证书解析**：详细展示证书信息、扩展字段、指纹计算
- **证书验证**：证书链验证、证书与密钥匹配验证
- **PFX 处理**：证书和私钥打包/解包，密码保护
- **格式转换**：PEM、DER、CRT、CER 等格式互转

### 📋 CSR（证书签名请求）
- **交互式生成向导**：图形化配置 CN、O、OU、SAN 等字段
- **CSR 解析**：查看 CSR 详细信息和扩展字段
- **CSR 验证**：验证 CSR 与私钥、公钥的匹配关系

### 🌐 HTTPS 工具
- **在线证书检测**：检查网站 HTTPS 证书配置
- **证书链补全**：自动下载并补全缺失的中间证书
- **SSL/TLS 分析**：协议版本、加密套件安全性评估

### 🛠️ 开发工具集
- 哈希计算：MD5、SHA-1、SHA-256、SHA-512、SM3
- 编解码工具：Base64、Hex、URL 编解码
- JWT 工具：生成、解析、验证 JSON Web Token

## 🏗️ 技术架构

### 后端
- **框架**：ASP.NET Core 8.0 Minimal API
- **加密库**：BouncyCastle（支持商密算法）
- **设计模式**：RESTful API、无状态设计
- **特性**：统一异常处理、API 响应封装、Swagger 文档

### 前端
- **框架**：Vue 3 + TypeScript + Vite
- **UI 组件库**：Element Plus
- **样式方案**：UnoCSS
- **状态管理**：Pinia
- **路由管理**：Vue Router
- **HTTP 客户端**：Axios
- **国际化**：Vue I18n（中英文）

### 部署
- **容器化**：Docker / Docker Compose
- **Web 服务器**：Kestrel / Nginx
- **跨平台**：支持 Linux / Windows / macOS

## 🚀 快速开始

### 环境要求
- .NET 8.0 SDK
- Node.js 18+ / pnpm
- Docker（可选）

### 启动后端
```bash
cd src/Crypto.Utils.Host
dotnet restore
dotnet run
```

访问 API：`http://localhost:5000`  
访问 Swagger：`http://localhost:5000/swagger`

### 启动前端
```bash
cd src/Crypto.Utils.UI
pnpm install
pnpm dev
```

访问应用：`http://localhost:5173`

### Docker 部署
```bash
docker-compose up -d
```

## 🔒 安全特性

- **双模式架构**：支持云端 API 处理和浏览器本地处理模式
- **数据不落盘**：API 无状态设计，不存储用户数据
- **隐私保护**：敏感操作可选择纯前端模式，数据不离开浏览器
- **HTTPS 传输**：生产环境强制使用 HTTPS 加密传输

## 📚 文档

- [API 设计文档](docs/API设计文档.md)
- [UI 设计文档](docs/UI设计文档.md)
- [项目路线图](docs/Roadmap.md)

## 🗺️ 开发路线

项目按照四个阶段逐步完善：
1. **第一阶段**：基础设施与核心功能（密钥、证书基础操作）
2. **第二阶段**：功能扩展（CSR、证书链、PFX、CRL）
3. **第三阶段**：商密算法与高级特性（SM2/SM3/SM4、HTTPS 工具）
4. **第四阶段**：生态完善（批量处理、监控、SDK、CLI）

详见 [Roadmap.md](docs/Roadmap.md)

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！

## 📄 开源协议

本项目采用 [Apache License 2.0](LICENSE) 开源协议。

## 🌟 致谢

- [BouncyCastle](https://www.bouncycastle.org/) - 强大的密码学库
- [Element Plus](https://element-plus.org/) - 优秀的 Vue 3 组件库
- [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet) - 现代化的 Web 框架

---

<p align="center">
  如果这个项目对您有帮助，请给我一个 ⭐ Star！
</p>
