# Crypto Utils

密码学工具平台——无状态、无认证、无持久化的纯工具型 API + Vue 3 SPA。
提供密钥对管理、X.509 证书操作、CSR、CRL 等能力，支持 RSA / EC / DSA / SM2 / SM3 / SM4。

## 技术栈

| 层     | 技术                                        |
| ------ | ------------------------------------------- |
| 核心库 | .NET 10 + BouncyCastle 2.6.2                |
| API    | ASP.NET Core 10，Controllers 模式           |
| 前端   | Vue 3 + Vite + TypeScript + Element Plus    |
| 测试   | NUnit 4 + FluentAssertions + OpenSSL 互操作 |

## 文档

| 文档                                                 | 说明                                       |
| ---------------------------------------------------- | ------------------------------------------ |
| [architecture.md](../docs/architecture.md)           | 系统分层、后端/前端结构、依赖清单          |
| [development-guide.md](../docs/development-guide.md) | 命名规范、API 设计规则、测试规范、安全要求 |
| [roadmap.md](../docs/roadmap.md)                     | 当前完成状态、各阶段开发计划               |
| [api-reference.md](../docs/api-reference.md)         | 所有端点的请求/响应字段说明                |

## 关键约定

- **命名空间**：根命名空间 `Crypto.Utils`，项目分层为 `Crypto.Utils.Core / Api / Host / UI`
- **API 响应**：所有接口返回 `ApiResponse<T>`（`success`, `message`, `data`, `errorCode`, `timestamp`）
- **路由格式**：`POST /api/{module}/{action}`，全小写，action 用连字符
- **服务层**：Api 层 Controller 不写业务逻辑，只做路由绑定；Service 实现注入 Core 库
- **安全**：私钥、密码不写日志；敏感数据仅在内存中处理，不落盘
