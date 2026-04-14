# API 参考

## 统一响应格式

所有接口均返回 `ApiResponse<T>`：

```json
{
  "success": true,
  "message": "操作成功",
  "data": { ... },
  "errorCode": null,
  "timestamp": "2026-04-14T10:00:00Z"
}
```

失败时 `success` 为 `false`，`data` 为 `null`，`errorCode` 为错误标识字符串。

---

## 通用类型

### SubjectInfo（证书主体）

| 字段 | 类型    | 必填 | 说明                |
| ---- | ------- | ---- | ------------------- |
| CN   | string  | ✅   | Common Name         |
| O    | string? | —    | Organization        |
| OU   | string? | —    | Organizational Unit |
| C    | string? | —    | Country (2位)       |
| ST   | string? | —    | State/Province      |
| L    | string? | —    | Locality            |
| E    | string? | —    | Email               |

---

## 密钥模块 `/api/key`

### POST `/api/key/generate` — 生成密钥对

**请求**

| 字段         | 类型    | 必填 | 默认值  | 说明                            |
| ------------ | ------- | ---- | ------- | ------------------------------- |
| algorithm    | string  | ✅   | `"RSA"` | RSA / EC / SM2 / DSA            |
| keySize      | int?    | —    | —       | RSA: 2048/4096；EC: 256/384/521 |
| curveName    | string? | —    | —       | EC 曲线名称                     |
| outputFormat | string  | —    | `"PEM"` | PEM / DER                       |

**响应 `data`**

| 字段       | 类型    | 说明           |
| ---------- | ------- | -------------- |
| publicKey  | string  | 公钥数据       |
| privateKey | string  | 私钥数据       |
| algorithm  | string  | 算法名称       |
| keySize    | int?    | 密钥大小       |
| curveName  | string? | 曲线名称（EC） |

---

### POST `/api/key/parse` — 解析密钥信息

**请求**

| 字段    | 类型   | 必填 | 说明                |
| ------- | ------ | ---- | ------------------- |
| keyData | string | ✅   | 密钥数据（PEM/DER） |

**响应 `data`**

| 字段          | 类型    | 说明                                       |
| ------------- | ------- | ------------------------------------------ |
| algorithmName | string  | 算法名称                                   |
| isPrivate     | bool    | 是否为私钥                                 |
| isPublic      | bool    | 是否为公钥                                 |
| keySize       | int?    | 密钥大小                                   |
| curveOid      | string? | 曲线 OID（EC）                             |
| curveName     | string? | 曲线名称（EC）                             |
| keyData       | string  | PEM 格式密钥数据                           |
| fingerprints  | object  | 指纹字典（算法名 → 十六进制值）            |
| parameters    | object? | 密钥参数（RSA: Modulus/Exponent；EC: X/Y） |

---

### POST `/api/key/convert` — 格式转换 (PEM ↔ DER)

**请求**

| 字段         | 类型   | 必填 | 说明      |
| ------------ | ------ | ---- | --------- |
| keyData      | string | ✅   | 密钥数据  |
| sourceFormat | string | ✅   | PEM / DER |
| targetFormat | string | ✅   | PEM / DER |

**响应 `data`**

| 字段         | 类型   | 说明             |
| ------------ | ------ | ---------------- |
| convertedKey | string | 转换后的密钥数据 |
| format       | string | 目标格式         |

---

### POST `/api/key/pkcs-convert` — PKCS 格式转换

**请求**

| 字段         | 类型    | 必填 | 说明                          |
| ------------ | ------- | ---- | ----------------------------- |
| keyData      | string  | ✅   | 密钥数据                      |
| sourceFormat | string  | ✅   | PKCS1 / PKCS8                 |
| targetFormat | string  | ✅   | PKCS1 / PKCS8                 |
| password     | string? | —    | 加密密码（用于加密的 PKCS#8） |

**响应 `data`** — 同 `/api/key/convert`

---

### POST `/api/key/encrypt` — 加密私钥

**请求**

| 字段       | 类型   | 必填 | 默认值          | 说明     |
| ---------- | ------ | ---- | --------------- | -------- |
| privateKey | string | ✅   | —               | 私钥数据 |
| password   | string | ✅   | —               | 加密密码 |
| algorithm  | string | —    | `"AES-256-CBC"` | 加密算法 |

**响应 `data`** — 同 `/api/key/convert`

---

### POST `/api/key/decrypt` — 解密私钥

**请求**

| 字段                | 类型   | 必填 | 说明           |
| ------------------- | ------ | ---- | -------------- |
| encryptedPrivateKey | string | ✅   | 加密的私钥数据 |
| password            | string | ✅   | 解密密码       |

**响应 `data`** — 同 `/api/key/convert`

---

## 证书模块 `/api/cert`

### POST `/api/cert/self-signed` — 生成自签名证书

**请求**

| 字段                    | 类型        | 必填 | 默认值            | 说明             |
| ----------------------- | ----------- | ---- | ----------------- | ---------------- |
| subject                 | SubjectInfo | ✅   | —                 | 证书主体         |
| privateKey              | string      | ✅   | —                 | 私钥数据         |
| validFrom               | DateTime?   | —    | 当前时间          | 有效期开始       |
| validTo                 | DateTime    | ✅   | —                 | 有效期结束       |
| signatureAlgorithm      | string      | —    | `"SHA256WITHRSA"` | 签名算法         |
| serialNumber            | string?     | —    | 随机生成          | 序列号           |
| keyUsage                | string[]?   | —    | —                 | 密钥用途列表     |
| extendedKeyUsage        | string[]?   | —    | —                 | 扩展密钥用途列表 |
| subjectAlternativeNames | string[]?   | —    | —                 | SAN 列表         |
| outputFormat            | string      | —    | `"PEM"`           | PEM / DER        |

**响应 `data`**

| 字段            | 类型    | 说明                            |
| --------------- | ------- | ------------------------------- |
| certificateData | string  | 证书数据                        |
| format          | string  | 输出格式                        |
| privateKey      | string? | 私钥（仅 sign-generate 时返回） |

---

### POST `/api/cert/sign-csr` — 基于 CSR 签发证书

**请求**

| 字段          | 类型      | 必填 | 默认值   | 说明       |
| ------------- | --------- | ---- | -------- | ---------- |
| csr           | string    | ✅   | —        | CSR 数据   |
| caCertificate | string    | ✅   | —        | CA 证书    |
| caPrivateKey  | string    | ✅   | —        | CA 私钥    |
| validFrom     | DateTime? | —    | 当前时间 | 有效期开始 |
| validTo       | DateTime  | ✅   | —        | 有效期结束 |
| serialNumber  | string?   | —    | 随机生成 | 序列号     |
| extensions    | object?   | —    | —        | 证书扩展   |
| outputFormat  | string    | —    | `"PEM"`  | PEM / DER  |

**响应 `data`** — 同 `/api/cert/self-signed`

---

### POST `/api/cert/sign-publickey` — 基于公钥签发证书

**请求**

| 字段               | 类型        | 必填 | 默认值            | 说明       |
| ------------------ | ----------- | ---- | ----------------- | ---------- |
| publicKey          | string      | ✅   | —                 | 公钥数据   |
| subject            | SubjectInfo | ✅   | —                 | 证书主体   |
| caCertificate      | string      | ✅   | —                 | CA 证书    |
| caPrivateKey       | string      | ✅   | —                 | CA 私钥    |
| validFrom          | DateTime?   | —    | 当前时间          | 有效期开始 |
| validTo            | DateTime    | ✅   | —                 | 有效期结束 |
| signatureAlgorithm | string      | —    | `"SHA256WITHRSA"` | 签名算法   |
| serialNumber       | string?     | —    | 随机生成          | 序列号     |
| extensions         | object?     | —    | —                 | 证书扩展   |
| outputFormat       | string      | —    | `"PEM"`           | PEM / DER  |

**响应 `data`** — 同 `/api/cert/self-signed`

---

### POST `/api/cert/sign-generate` — 生成密钥并签发证书

**请求**

| 字段               | 类型        | 必填 | 默认值            | 说明           |
| ------------------ | ----------- | ---- | ----------------- | -------------- |
| subject            | SubjectInfo | ✅   | —                 | 证书主体       |
| keyAlgorithm       | string      | ✅   | `"RSA"`           | RSA / EC / SM2 |
| keySize            | int?        | —    | —                 | 密钥大小       |
| curveName          | string?     | —    | —                 | EC 曲线名称    |
| caCertificate      | string      | ✅   | —                 | CA 证书        |
| caPrivateKey       | string      | ✅   | —                 | CA 私钥        |
| validFrom          | DateTime?   | —    | 当前时间          | 有效期开始     |
| validTo            | DateTime    | ✅   | —                 | 有效期结束     |
| signatureAlgorithm | string      | —    | `"SHA256WITHRSA"` | 签名算法       |
| serialNumber       | string?     | —    | 随机生成          | 序列号         |
| extensions         | object?     | —    | —                 | 证书扩展       |
| outputFormat       | string      | —    | `"PEM"`           | PEM / DER      |

**响应 `data`** — 同 `/api/cert/self-signed`（`privateKey` 字段有值）

---

### POST `/api/cert/parse` — 解析证书

**请求**

| 字段            | 类型   | 必填 | 说明                |
| --------------- | ------ | ---- | ------------------- |
| certificateData | string | ✅   | 证书数据（PEM/DER） |

**响应 `data`**

| 字段                       | 类型            | 说明                            |
| -------------------------- | --------------- | ------------------------------- |
| version                    | int             | 证书版本                        |
| serialNumber               | string          | 序列号                          |
| subject                    | string          | 主体 DN                         |
| issuer                     | string          | 颁发者 DN                       |
| notBefore                  | DateTime        | 有效期开始                      |
| notAfter                   | DateTime        | 有效期结束                      |
| duration                   | double          | 有效期天数                      |
| isValid                    | bool            | 当前是否有效                    |
| remainingDays              | double          | 剩余有效天数                    |
| signatureAlgorithmOid      | string          | 签名算法 OID                    |
| signatureAlgorithmName     | string          | 签名算法名称                    |
| fingerprints               | object          | 指纹字典                        |
| publicKey                  | KeyInfoResponse | 公钥详情（见密钥 parse 响应）   |
| isCA                       | bool            | 是否为 CA 证书                  |
| pathLengthConstraint       | int?            | 路径长度约束                    |
| subjectKeyIdentifier       | string?         | SKI                             |
| authorityKeyIdentifier     | string?         | AKI                             |
| keyUsage                   | string[]?       | 密钥用途                        |
| extendedKeyUsage           | string[]?       | 扩展密钥用途                    |
| subjectAlternativeNames    | string[]?       | SAN（格式：`类型: 值`）         |
| certificatePolicies        | string[]?       | 证书策略 OID 列表               |
| crlDistributionPoints      | string[]?       | CRL 分发点                      |
| authorityInformationAccess | object?         | AIA（`ocspUrls` + `caIssuers`） |
| extensions                 | object?         | 所有扩展字段                    |

---

### POST `/api/cert/verify` — 验证证书

**请求**

| 字段              | 类型      | 必填 | 说明                     |
| ----------------- | --------- | ---- | ------------------------ |
| certificate       | string    | ✅   | 待验证证书               |
| issuerCertificate | string?   | —    | 颁发者证书               |
| checkDate         | DateTime? | —    | 验证日期（默认当前时间） |

**响应 `data`**

| 字段           | 类型      | 说明           |
| -------------- | --------- | -------------- |
| signatureValid | bool      | 签名是否有效   |
| dateValid      | bool      | 是否在有效期内 |
| chainValid     | bool      | 证书链是否完整 |
| isValid        | bool      | 综合验证结果   |
| message        | string    | 验证说明       |
| details        | string[]? | 详细信息       |

---

### POST `/api/cert/convert` — 证书格式转换

**请求**

| 字段            | 类型    | 必填 | 说明              |
| --------------- | ------- | ---- | ----------------- |
| certificateData | string  | ✅   | 证书数据          |
| sourceFormat    | string  | ✅   | PEM / DER / PFX   |
| targetFormat    | string  | ✅   | PEM / DER / PFX   |
| password        | string? | —    | PFX 密码          |
| privateKey      | string? | —    | 转为 PFX 时的私钥 |

**响应 `data`** — 同 `/api/cert/self-signed`（无 `privateKey`）

---

## CSR 模块 `/api/csr`

### POST `/api/csr/generate` — 生成 CSR

**请求**

| 字段               | 类型        | 必填 | 默认值            | 说明      |
| ------------------ | ----------- | ---- | ----------------- | --------- |
| subject            | SubjectInfo | ✅   | —                 | 主体信息  |
| privateKey         | string      | ✅   | —                 | 私钥数据  |
| signatureAlgorithm | string      | —    | `"SHA256WITHRSA"` | 签名算法  |
| extensions         | object?     | —    | —                 | 扩展字段  |
| outputFormat       | string      | —    | `"PEM"`           | PEM / DER |

**响应 `data`**

| 字段    | 类型   | 说明     |
| ------- | ------ | -------- |
| csrData | string | CSR 数据 |
| format  | string | 输出格式 |

---

### POST `/api/csr/parse` — 解析 CSR

**请求**

| 字段    | 类型   | 必填 | 说明                |
| ------- | ------ | ---- | ------------------- |
| csrData | string | ✅   | CSR 数据（PEM/DER） |

**响应 `data`**

| 字段                   | 类型            | 说明         |
| ---------------------- | --------------- | ------------ |
| subject                | string          | 主体 DN      |
| signatureAlgorithmName | string          | 签名算法名称 |
| publicKey              | KeyInfoResponse | 公钥详情     |
| extensions             | object?         | 扩展字段     |

---

### POST `/api/csr/verify` — 验证 CSR 签名

**请求**

| 字段    | 类型   | 必填 | 说明     |
| ------- | ------ | ---- | -------- |
| csrData | string | ✅   | CSR 数据 |

**响应 `data`**

| 字段    | 类型   | 说明         |
| ------- | ------ | ------------ |
| isValid | bool   | 签名是否有效 |
| message | string | 验证说明     |

---

## CRL 模块 `/api/crl`

### POST `/api/crl/generate` — 生成 CRL

**请求**

| 字段                | 类型                      | 必填 | 默认值            | 说明         |
| ------------------- | ------------------------- | ---- | ----------------- | ------------ |
| issuer              | SubjectInfo               | ✅   | —                 | 颁发者信息   |
| revokedCertificates | RevokedCertificateInfo[]? | —    | —                 | 吊销证书列表 |
| caPrivateKey        | string                    | ✅   | —                 | CA 私钥      |
| signatureAlgorithm  | string                    | —    | `"SHA256WITHRSA"` | 签名算法     |
| thisUpdate          | DateTime?                 | —    | 当前时间          | 本次更新时间 |
| nextUpdate          | DateTime                  | ✅   | —                 | 下次更新时间 |
| outputFormat        | string                    | —    | `"PEM"`           | PEM / DER    |

**RevokedCertificateInfo**

| 字段           | 类型     | 必填 | 说明       |
| -------------- | -------- | ---- | ---------- |
| serialNumber   | string   | ✅   | 证书序列号 |
| revocationDate | DateTime | ✅   | 吊销日期   |
| reason         | string?  | —    | 吊销原因   |

**响应 `data`**

| 字段    | 类型   | 说明     |
| ------- | ------ | -------- |
| crlData | string | CRL 数据 |
| format  | string | 输出格式 |

---

### POST `/api/crl/parse` — 解析 CRL

**请求**

| 字段    | 类型   | 必填 | 说明                |
| ------- | ------ | ---- | ------------------- |
| crlData | string | ✅   | CRL 数据（PEM/DER） |

**响应 `data`**

| 字段                   | 类型                        | 说明                                   |
| ---------------------- | --------------------------- | -------------------------------------- |
| issuer                 | string                      | 颁发者 DN                              |
| thisUpdate             | DateTime                    | 本次更新时间                           |
| nextUpdate             | DateTime                    | 下次更新时间                           |
| signatureAlgorithmName | string                      | 签名算法名称                           |
| revokedCertificates    | RevokedCertificateDetail[]? | 吊销证书列表（含序列号/吊销日期/原因） |

---

### POST `/api/crl/check` — 检查证书吊销状态

**请求**

| 字段        | 类型   | 必填 | 说明     |
| ----------- | ------ | ---- | -------- |
| certificate | string | ✅   | 证书数据 |
| crlData     | string | ✅   | CRL 数据 |

**响应 `data`**

| 字段           | 类型      | 说明       |
| -------------- | --------- | ---------- |
| isRevoked      | bool      | 是否被吊销 |
| revocationDate | DateTime? | 吊销日期   |
| reason         | string?   | 吊销原因   |
| message        | string    | 说明       |

---

## 证书链模块 `/api/cert/chain`

### POST `/api/cert/chain/build` — 构建证书链

**请求**

| 字段                     | 类型      | 必填 | 说明         |
| ------------------------ | --------- | ---- | ------------ |
| certificate              | string    | ✅   | 目标证书     |
| intermediateCertificates | string[]? | —    | 中间证书列表 |
| rootCertificates         | string[]? | —    | 根证书列表   |

**响应 `data`**

| 字段             | 类型     | 说明                 |
| ---------------- | -------- | -------------------- |
| certificateChain | string[] | 证书链（从叶子到根） |
| chainLength      | int      | 链长度               |
| isComplete       | bool     | 是否完整             |
| message          | string   | 说明                 |

---

### POST `/api/cert/chain/verify` — 验证证书链

**请求**

| 字段             | 类型      | 必填 | 说明                     |
| ---------------- | --------- | ---- | ------------------------ |
| certificateChain | string[]  | ✅   | 证书链（从叶子到根）     |
| trustedRoots     | string[]? | —    | 信任的根证书             |
| checkDate        | DateTime? | —    | 验证日期（默认当前时间） |

**响应 `data`**

| 字段               | 类型                           | 说明             |
| ------------------ | ------------------------------ | ---------------- |
| isValid            | bool                           | 是否有效         |
| message            | string                         | 说明             |
| certificateDetails | CertificateValidationDetail[]? | 各级证书验证详情 |

**CertificateValidationDetail**

| 字段    | 类型   | 说明       |
| ------- | ------ | ---------- |
| subject | string | 证书主体   |
| issuer  | string | 证书颁发者 |
| isValid | bool   | 是否有效   |
| message | string | 验证说明   |

---

## 格式转换模块 `/api/format`

### POST `/api/format/convert` — 通用格式转换（自动检测类型）

**请求**

| 字段         | 类型    | 必填 | 说明                                       |
| ------------ | ------- | ---- | ------------------------------------------ |
| data         | string  | ✅   | 数据内容                                   |
| sourceFormat | string  | ✅   | PEM / DER                                  |
| targetFormat | string  | ✅   | PEM / DER                                  |
| dataType     | string? | —    | KEY / CSR / CERT / CRL（不指定则自动检测） |

**响应 `data`**

| 字段          | 类型   | 说明                         |
| ------------- | ------ | ---------------------------- |
| convertedData | string | 转换后的数据                 |
| format        | string | 目标格式                     |
| dataType      | string | 数据类型（KEY/CSR/CERT/CRL） |

---

### POST `/api/format/key/convert` — 密钥格式转换

请求/响应同 `/api/key/convert`。

---

### POST `/api/format/csr/convert` — CSR 格式转换

请求使用 `FormatConvertRequest`（字段同通用转换），响应同通用格式转换。

---

### POST `/api/format/certificate/convert` — 证书格式转换

请求使用 `FormatConvertRequest`，响应同通用格式转换。

---

### POST `/api/format/crl/convert` — CRL 格式转换

请求使用 `FormatConvertRequest`，响应同通用格式转换。

---

## 健康检查

### GET `/health`

无请求体。

**响应**

```json
{
  "status": "Healthy",
  "service": "Crypto Utils API",
  "version": "1.0.0",
  "timestamp": "2026-04-14T10:00:00Z"
}
```
