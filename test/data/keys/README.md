# 测试密钥说明

本目录包含用于测试和开发的密钥文件。

## ⚠️ 安全警告

**这些密钥仅用于测试和开发目的，切勿在生产环境中使用！**

## 密钥列表

### RSA 密钥

| 文件名 | 算法 | 密钥长度 | 格式 | 说明 |
|--------|------|----------|------|------|
| `rsa-2048-pkcs1.pem` | RSA | 2048 位 | PKCS#1 PEM | 传统格式私钥 |
| `rsa-2048-pkcs8.pem` | RSA | 2048 位 | PKCS#8 PEM | 标准格式私钥 |
| `rsa-2048-public.pem` | RSA | 2048 位 | PEM | 公钥 |
| `rsa-3072-pkcs1.pem` | RSA | 3072 位 | PKCS#1 PEM | 传统格式私钥 |
| `rsa-3072-pkcs8.pem` | RSA | 3072 位 | PKCS#8 PEM | 标准格式私钥 |
| `rsa-4096-pkcs1.pem` | RSA | 4096 位 | PKCS#1 PEM | 传统格式私钥 |
| `rsa-4096-pkcs8.pem` | RSA | 4096 位 | PKCS#8 PEM | 标准格式私钥 |
| `rsa-2048-pkcs8.der` | RSA | 2048 位 | PKCS#8 DER | 二进制格式私钥 |
| `rsa-2048-encrypted.pem` | RSA | 2048 位 | PKCS#1 PEM | 加密私钥 (密码: test1234) |

### EC 密钥 (椭圆曲线)

| 文件名 | 曲线 | 格式 | 说明 |
|--------|------|------|------|
| `ec-p256-pkcs8.pem` | secp256r1 (P-256) | PKCS#8 PEM | NIST P-256 私钥 |
| `ec-p256-public.pem` | secp256r1 (P-256) | PEM | NIST P-256 公钥 |
| `ec-p256-pkcs8.der` | secp256r1 (P-256) | PKCS#8 DER | NIST P-256 私钥 (二进制) |
| `ec-p384-pkcs8.pem` | secp384r1 (P-384) | PKCS#8 PEM | NIST P-384 私钥 |
| `ec-p384-public.pem` | secp384r1 (P-384) | PEM | NIST P-384 公钥 |
| `ec-p521-pkcs8.pem` | secp521r1 (P-521) | PKCS#8 PEM | NIST P-521 私钥 |
| `ec-p521-public.pem` | secp521r1 (P-521) | PEM | NIST P-521 公钥 |
| `ec-p256-encrypted.pem` | secp256r1 (P-256) | PKCS#8 PEM | 加密私钥 (密码: test1234) |

### SM2 密钥 (国密)

| 文件名 | 曲线 | 格式 | 说明 |
|--------|------|------|------|
| `sm2-sec1.pem` | SM2 | SEC1 PEM | 国密 SM2 私钥 (SEC1 格式) |
| `sm2-pkcs8.pem` | SM2 | PKCS#8 PEM | 国密 SM2 私钥 (PKCS#8 格式) |
| `sm2-public.pem` | SM2 | PEM | 国密 SM2 公钥 |
| `sm2-pkcs8.der` | SM2 | PKCS#8 DER | 国密 SM2 私钥 (二进制) |

> **注意**: SM2 密钥需要 OpenSSL 1.1.1+ 或带有国密支持的版本

### DSA 密钥

| 文件名 | 密钥长度 | 格式 | 说明 |
|--------|----------|------|------|
| `dsa-2048-params.pem` | 2048 位 | PEM | DSA 参数 |
| `dsa-2048-private.pem` | 2048 位 | PEM | DSA 私钥 |
| `dsa-2048-public.pem` | 2048 位 | PEM | DSA 公钥 |

## 重新生成密钥

运行以下命令重新生成所有测试密钥:

```bash
cd /path/to/crypto-utils
./cmd/generate-test-keys.sh
```

## 查看密钥信息

### 查看 RSA 密钥信息
```bash
openssl rsa -in rsa-2048-pkcs1.pem -text -noout
```

### 查看 EC 密钥信息
```bash
openssl ec -in ec-p256-pkcs8.pem -text -noout
```

### 查看 SM2 密钥信息
```bash
openssl ec -in sm2-pkcs8.pem -text -noout
```

### 查看 DSA 密钥信息
```bash
openssl dsa -in dsa-2048-private.pem -text -noout
```

## 文件格式说明

- **PEM**: 文本格式，Base64 编码，以 `-----BEGIN xxx-----` 开头
- **DER**: 二进制格式，直接存储 ASN.1 结构
- **PKCS#1**: RSA 私钥的传统格式
- **PKCS#8**: 通用私钥格式，支持多种算法

## 安全建议

1. 这些密钥仅用于本地开发和测试
2. 不要将这些密钥用于生产环境
3. 不要将这些密钥提交到公共代码仓库
4. 生产环境应使用安全的密钥管理系统 (如 HSM、KMS)

---

生成日期: $(date '+%Y-%m-%d %H:%M:%S')
生成脚本: cmd/generate-test-keys.sh
