# 网站证书说明

本目录包含从真实网站拉取的 SSL/TLS 证书，用于测试和开发。

## ⚠️ 注意事项

1. 这些证书是从公开网站拉取的真实证书
2. 证书可能会过期，需要定期更新
3. 仅用于测试和开发目的
4. 不要用于任何非法用途

## 文件命名规则

- `<hostname>_<port>_cert.pem` - 服务器证书（叶子证书）
- `<hostname>_<port>_chain.pem` - 完整证书链
- `<hostname>_<port>_chain_N.pem` - 中间证书（N 为序号）
- `<hostname>_<port>_info.txt` - 证书详细信息

## 证书链结构

一个典型的证书链包含:

1. **服务器证书（叶子证书）**: 网站服务器的证书
2. **中间证书**: 由中间 CA 颁发的证书
3. **根证书**: 受信任的根 CA 证书（通常不包含在链中）

## 重新拉取证书

运行以下命令重新拉取所有网站证书:

```bash
cd /path/to/crypto-utils
./cmd/pull-website-certs.sh
```

拉取特定网站证书:

```bash
./cmd/pull-website-certs.sh example.com
./cmd/pull-website-certs.sh example.com:8443
```

拉取多个网站:

```bash
./cmd/pull-website-certs.sh site1.com site2.com site3.com
```

## 查看证书信息

### 查看证书详细信息
```bash
openssl x509 -in <hostname>_<port>_cert.pem -text -noout
```

### 查看证书主题
```bash
openssl x509 -in <hostname>_<port>_cert.pem -noout -subject
```

### 查看证书有效期
```bash
openssl x509 -in <hostname>_<port>_cert.pem -noout -dates
```

### 验证证书链
```bash
openssl verify -CAfile <hostname>_<port>_chain.pem <hostname>_<port>_cert.pem
```

## 证书用途

这些证书可用于:

- 测试证书解析功能
- 测试证书链验证
- 测试证书格式转换
- 学习 X.509 证书结构
- 开发证书相关工具

## 安全建议

1. 定期更新证书（建议每月更新）
2. 不要在生产环境中使用这些证书
3. 注意证书的有效期
4. 不要修改证书内容

---

更新日期: $(date '+%Y-%m-%d %H:%M:%S')
生成脚本: cmd/pull-website-certs.sh
