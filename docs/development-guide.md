# 开发规范

## C# 规范

### 命名空间

根命名空间为 `Crypto.Utils`（在 `Directory.Build.props` 中通过 `<RootNamespace>` 统一设定）。

各项目的命名空间约定：

| 项目              | 命名空间                                                                   |
| ----------------- | -------------------------------------------------------------------------- |
| Crypto.Utils.Core | `Crypto.Utils.*`（如 `Crypto.Utils.X509`、`Crypto.Utils.Crypto`）          |
| Crypto.Utils.Api  | `Crypto.Utils.Controllers`、`Crypto.Utils.Services`、`Crypto.Utils.Models` |
| Crypto.Utils.Host | `Crypto.Utils.Host`                                                        |

### 语言特性

`Directory.Build.props` 全局启用：

```xml
<LangVersion>latest</LangVersion>
<Nullable>enable</Nullable>
<ImplicitUsings>enable</ImplicitUsings>
```

- **Nullable**：所有可空引用类型必须显式标注 `?`，不得使用 `!` 强制忽略
- **Implicit Usings**：`System`、`System.Collections.Generic`、`System.Linq` 等已全局导入，无需再次 `using`
- **GlobalUsings.cs**：各项目的额外全局 `using` 统一放在项目根目录的 `GlobalUsings.cs`

### 文件与目录

- 一个文件一个类（或紧密相关的小类型组合，如 Request + 内嵌 DTO）
- 目录结构反映命名空间层级（如 `X509/Enums/KeyUsage.cs` → `Crypto.Utils.X509.Enums`）
- 枚举扩展方法单独放 `{EnumName}Extensions.cs`，辅助工具类放 `{Name}Helper.cs`

### 请求/响应模型

- 请求模型文件：`Models/Requests/{Module}Requests.cs`
- 响应模型文件：`Models/Responses/{Module}Responses.cs`
- 必填字段使用 `[Required]` 或 `required` 关键字
- 默认值在声明时赋值，不依赖构造函数

---

## TypeScript / Vue 规范

### 目录结构

```
src/
├── api/client/          # HTTP 客户端（CloudApiClient）
├── services/
│   ├── interfaces/      # 接口文件命名：I{Module}Service.ts
│   ├── cloud/           # 实现文件命名：Cloud{Module}Service.ts
│   └── browser/         # 实现文件命名：Browser{Module}Service.ts
├── models/              # 每个模块一个子目录，每类一个文件
│   └── {module}/
│       ├── {Model}.ts
│       └── index.ts     # 统一导出
├── views/               # 页面组件（与路由对应）
├── components/          # 通用/复用组件
└── stores/              # Pinia Store，命名：use{Module}Store
```

### 命名约定

| 类型        | 约定                     | 示例                        |
| ----------- | ------------------------ | --------------------------- |
| 服务接口    | `I{Module}Service`       | `ICertificateService`       |
| 云端实现    | `Cloud{Module}Service`   | `CloudCertificateService`   |
| 浏览器实现  | `Browser{Module}Service` | `BrowserCertificateService` |
| Pinia Store | `use{Module}Store`       | `useCertificateStore`       |
| 页面组件    | PascalCase               | `CertificateParser.vue`     |

### 服务接口规范

- 所有服务方法返回 `Promise<T>`（不包含 `ApiResponse` 包装层）
- 接口文件同时定义相关的 Options 和 Result 类型
- 错误通过 `throw` 传递，不在接口层吞掉

---

## API 设计规范

### 路由格式

```
POST /api/{module}/{action}
```

- 全部使用 **POST** 方法（操作型 API，请求体传参）
- 模块名全小写：`key`、`cert`、`csr`、`crl`、`format`
- 动作名使用连字符：`self-signed`、`sign-csr`、`pkcs-convert`

### 统一响应格式

所有接口返回 `ApiResponse<T>`：

```json
{
  "success": true,
  "message": "操作成功",
  "data": { ... },
  "errorCode": null,
  "timestamp": "2026-04-14T00:00:00Z"
}
```

失败时：

```json
{
  "success": false,
  "message": "错误描述",
  "errorCode": "ERR_INVALID_KEY",
  "data": null,
  "timestamp": "2026-04-14T00:00:00Z"
}
```

### 异常处理

- 所有未捕获异常由 `ExceptionHandlingMiddleware` 统一处理，返回标准 `ApiResponse` 错误格式
- Controller 内**不**使用 `try-catch` 包裹业务逻辑
- Service 层抛出有意义的异常（如 `ArgumentException`、`InvalidOperationException`），中间件负责转换
- 私钥、密码等敏感字段**不**写入日志（日志中仅记录算法、格式等非敏感信息）

### 输出格式参数

所有生成类接口接受 `OutputFormat` 参数（默认 `"PEM"`），支持 `"PEM"` / `"DER"`。

---

## 测试规范

### 命名约定

- 测试类：`{Subject}Tests`（如 `SM2Tests`、`SM2OpenSslInteropTests`）
- 测试方法：`{Method}_{Scenario}_{ExpectedResult}`（如 `Sign_WithValidKey_ShouldSucceed`）

### OpenSSL 互操作测试

使用 `Crypto.Utils.TestSupport.OpenSslCli` / `TongsuoCli` 静态类封装 OpenSSL / tongsuo CLI 调用（基于 `CliWrap`），用于验证本项目生成的密钥/证书与 OpenSSL / tongsuo 的互操作性。`TongsuoCli` 默认使用 `/opt/tongsuo/bin/tongsuo`（可通过环境变量 `TONGSUO_PATH` 覆盖），用于国密 SM2/SM3/SM4 相关互操作。

测试数据文件放在 `tests/data/`：

```
tests/data/
├── keys/     # 测试用密钥文件
├── csrs/     # 测试用 CSR 文件
├── certs/    # 测试用证书文件
├── crls/     # 测试用 CRL 文件
└── pfx/      # 测试用 PFX 文件
```

---

## 安全规范

- **私钥/密码不记录日志**：`LogInformation` 中只允许记录算法名称、格式、模块名等非敏感信息
- **内存处理**：所有敏感数据仅在请求处理期间驻留内存，不写文件系统
- **请求体限制**：生产部署时应配置 `MaxRequestBodySize`（建议 10MB）防止 DoS
- **HTTPS**：生产环境强制 HTTPS（`UseHsts` + `UseHttpsRedirection` 已在 Host 配置）
