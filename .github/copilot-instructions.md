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

## 代码规范

1. **所有公开 `async` 方法**签名末尾必须包含 `CancellationToken ct = default`
2. 库代码 `await` 调用须追加 `.ConfigureAwait(false)`
3. **日志使用结构化日志**，参数化消息模板，**禁止**字符串拼接；正确：`logger.LogInformation("User {UserId} logged in", userId)`
4. 可空引用类型全量启用；**公共 API 必须在方法体内做参数校验**（使用 `ArgumentNullException.ThrowIfNull`）
5. 通过接口编程，使用内置 DI，对外暴露最小接口；Core 层暴露服务接口，实现放入 Services 目录
6. 不吞异常；捕获异常时必须显式处理或重新抛出，使用有意义的消息便于诊断
7. **公共 API 必须有 XML 注释**；实现类优先用 `<inheritdoc />`；注释中文，日志消息和异常消息英文，句尾加句号
8. **严禁硬编码密钥、凭证、Token**；敏感数据不写日志，不在异常消息中暴露；密钥操作后及时清理内存
9. 测试命名：`MethodName_Should_ExpectedBehavior_When_Condition`，AAA 模式，覆盖正常/边界/异常路径
10. **Conventional Commits 规范**：
    - 中文描述，一到三句话，首字母小写，句尾不加句号。
    - 以 `type(scope): description` 格式开头，`scope` 可选，描述修改的范围（如模块、层次等），`type` 必须是以下之一：
      - `feat: 新增功能` - 新增代码功能
      - `fix: 修复bug` - 缺陷修复
      - `refactor: 重构` - 重组代码逻辑、优化结构（不改变功能）
      - `docs: 文档` - 文档或注释更新
      - `style: 风格` - 代码格式调整（不改变功能）
      - `perf: 性能` - 性能优化
      - `test: 测试` - 添加或修改测试
      - `chore: 工具` - 构建、依赖、工具链等
11. 密钥导入/导出必须指定格式参数（PKCS#1/PKCS#8/SEC1 等），禁止使用默认格式
12. 签名验证失败时返回特定错误码，禁止泄露原始异常信息
13. 随机数生成使用 `SecureRandom`，禁止使用 `Random`；对称加密包含 IV/Nonce，禁止 ECB 模式
14. HTTP 状态码：200/201（成功）、400（参数错误）、422（验证失败）、500（服务错误）
15. 错误响应：`{ "success": false, "message": "...", "errorCode": "...", "data": null }`；文件上传限制大小和格式
16. 大字节数组操作使用 `Span<T>` 或 `Memory<T>`；PEM/DER 解析后立即释放原始字节数组
17. 长耗时操作（生成密钥对）提供进度回调或异步取消；错误消息通过资源文件定义，支持 i18n
18. 时间戳统一使用 UTC，展示时由前端转换为本地时区
19. `Directory.Packages.props` 统一管理包版本；BouncyCastle 大版本更新需评估 API 兼容性
20. 使用 `Path.Join()` 拼接路径（.NET 6+），兼容 Linux，禁止硬编码反斜杠
21. 导出文件操作使用 `try-finally` 或 `using` 快速释放资源
