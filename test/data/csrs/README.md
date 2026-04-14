# 测试 CSR 说明

本目录包含用于测试和开发的 CSR (证书签名请求) 文件。

## ⚠️ 安全警告

**这些 CSR 仅用于测试和开发目的，切勿在生产环境中使用！**

## CSR 列表

### RSA 密钥的 CSR

| 文件名 | 密钥类型 | 描述 | CN | SAN |
|--------|----------|------|----|----|
| `rsa-2048-basic.csr` | RSA 2048 | 基本 CSR | test.example.com | 无 |
| `rsa-2048-san.csr` | RSA 2048 | 带 SAN 的 CSR | multi.example.com | 3 个 DNS + 1 个 IP |
| `rsa-3072-wildcard.csr` | RSA 3072 | 通配符域名 | *.example.com | 无 |
| `rsa-4096-long-dn.csr` | RSA 4096 | 长 DN 字段 | secure.technology-innovation.example.com | 无 |

### EC 密钥的 CSR (P-256)

| 文件名 | 密钥类型 | 描述 | CN | SAN |
|--------|----------|------|----|----|
| `ec-p256-basic.csr` | EC P-256 | 基本 CSR | ec-test.example.com | 无 |
| `ec-p256-san.csr` | EC P-256 | 带 SAN (含 IPv6) | ec-multi.example.com | 2 个 DNS + 2 个 IP |

### EC 密钥的 CSR (P-384)

| 文件名 | 密钥类型 | 描述 | CN | 签名算法 |
|--------|----------|------|----|----------|
| `ec-p384-basic.csr` | EC P-384 | 基本 CSR | p384.example.com | SHA-256 |
| `ec-p384-sha384.csr` | EC P-384 | 高安全性 | secure-p384.example.com | SHA-384 |

### EC 密钥的 CSR (P-521)

| 文件名 | 密钥类型 | 描述 | CN | 签名算法 |
|--------|----------|------|----|----------|
| `ec-p521-basic.csr` | EC P-521 | 基本 CSR | p521.example.com | SHA-256 |
| `ec-p521-sha512.csr` | EC P-521 | 最高安全性 | max-secure.example.com | SHA-512 |

### SM2 密钥的 CSR (国密)

| 文件名 | 密钥类型 | 描述 | CN | SAN |
|--------|----------|------|----|----|
| `sm2-basic.csr` | SM2 | 基本国密 CSR | sm2-test.example.cn | 无 |
| `sm2-san.csr` | SM2 | 带 SAN 的国密 CSR | sm2-multi.example.cn | 3 个 DNS |

### 特殊场景的 CSR

| 文件名 | 密钥类型 | 用途 | 特点 |
|--------|----------|------|------|
| `rsa-2048-ip-only.csr` | RSA 2048 | IP 证书 | 仅包含 IP 地址，无域名 |
| `rsa-2048-email.csr` | RSA 2048 | 邮件证书 | SAN 包含邮件地址 |
| `rsa-2048-codesign.csr` | RSA 2048 | 代码签名 | 用于代码签名证书 |
| `ec-p256-client.csr` | EC P-256 | 客户端认证 | 用于客户端证书 |
| `rsa-2048-multi-ou.csr` | RSA 2048 | 多层组织 | 包含多个 OU 字段 |
| `rsa-2048-idn.csr` | RSA 2048 | 国际化域名 | 支持中文域名 |
| `rsa-2048-basic.der` | RSA 2048 | DER 格式 | 二进制格式的 CSR |

## 重新生成 CSR

运行以下命令重新生成所有测试 CSR:

```bash
cd /path/to/crypto-utils
./cmd/generate-test-csrs.sh
```

## 查看 CSR 信息

### 查看 PEM 格式 CSR
```bash
openssl req -in rsa-2048-basic.csr -text -noout
```

### 查看 DER 格式 CSR
```bash
openssl req -in rsa-2048-basic.der -inform DER -text -noout
```

### 验证 CSR 签名
```bash
openssl req -in rsa-2048-basic.csr -verify -noout
```

### 查看 CSR 的公钥
```bash
openssl req -in rsa-2048-basic.csr -pubkey -noout
```

### 提取 CSR 的主题信息
```bash
openssl req -in rsa-2048-basic.csr -subject -noout
```

### 查看 SAN (Subject Alternative Names)
```bash
openssl req -in rsa-2048-san.csr -text -noout | grep -A 1 "Subject Alternative Name"
```

## CSR 格式说明

- **PEM 格式**: 文本格式，以 `-----BEGIN CERTIFICATE REQUEST-----` 开头
- **DER 格式**: 二进制格式，直接存储 ASN.1 结构
- **Subject DN**: 主题可分辨名称，包含 CN、O、OU、C、ST、L 等字段
- **SAN**: 主题备用名称，可以包含多个 DNS、IP、Email 等

## 使用场景

### 1. 服务器证书 CSR
- 用于 HTTPS/TLS 服务器
- 通常包含域名的 CN 和 SAN
- 示例: `rsa-2048-basic.csr`, `rsa-2048-san.csr`

### 2. 通配符证书 CSR
- 用于保护多个子域名
- CN 以 `*.` 开头
- 示例: `rsa-3072-wildcard.csr`

### 3. 客户端证书 CSR
- 用于客户端身份认证
- 示例: `ec-p256-client.csr`

### 4. 代码签名证书 CSR
- 用于软件、驱动程序签名
- 示例: `rsa-2048-codesign.csr`

### 5. 邮件证书 CSR
- 用于 S/MIME 邮件加密和签名
- 示例: `rsa-2048-email.csr`

## 提交 CSR 到 CA

生成的 CSR 可以提交给证书颁发机构 (CA) 以获取签名证书:

1. **公共 CA**: Let's Encrypt, DigiCert, GlobalSign 等
2. **企业 CA**: 企业内部 PKI 系统
3. **自签名**: 使用 OpenSSL 自行签名 (仅用于测试)

## 安全建议

1. 这些 CSR 仅用于本地开发和测试
2. 生产环境应使用新生成的私钥和 CSR
3. 私钥必须妥善保管，不要泄露
4. CSR 可以公开，但应确保其准确性

---

生成日期: $(date '+%Y-%m-%d %H:%M:%S')
生成脚本: cmd/generate-test-csrs.sh
依赖脚本: cmd/generate-test-keys.sh
