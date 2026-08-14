#!/bin/bash

# SM2 测试素材生成脚本（tongsuo）
# 生成国密 SM2 自签名证书与 CSR，用于固定测试。
# 输出:
# - tests/data/certs/sm2-selfsigned-ext.pem : SM2 自签名证书，serial 4001，CN=sm2-test.example.cn，含 SAN/KU/EKU
# - tests/data/csrs/sm2-ext.csr             : SM2 CSR，CN=sm2-test.example.cn，含 SAN
# 依赖: generate-test-keys.sh 生成的 SM2 密钥

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
KEYS_DIR="$PROJECT_ROOT/tests/data/keys"
CERTS_DIR="$PROJECT_ROOT/tests/data/certs"
CSRS_DIR="$PROJECT_ROOT/tests/data/csrs"
TONGSUO="${TONGSUO_PATH:-/opt/tongsuo/bin/tongsuo}"

if [ ! -x "$TONGSUO" ]; then
    echo "错误: tongsuo 不可用 ($TONGSUO)。请设置 TONGSUO_PATH 或安装 tongsuo。"
    exit 1
fi

echo "====================================="
echo "SM2 测试素材生成 (tongsuo)"
echo "====================================="

mkdir -p "$CERTS_DIR" "$CSRS_DIR"

if [ ! -f "$KEYS_DIR/sm2-pkcs8.pem" ]; then
    "$SCRIPT_DIR/generate-test-keys.sh"
fi

# 1. SM2 自签名证书（含扩展）
cat > "$CERTS_DIR/temp_sm2_ext.cnf" <<EOF
[req]
distinguished_name = dn
prompt = no
req_extensions = req_ext
x509_extensions = req_ext

[dn]

[req_ext]
keyUsage = critical,digitalSignature
extendedKeyUsage = serverAuth
subjectAltName = DNS:sm2-test.example.cn,DNS:www.sm2-test.example.cn
subjectKeyIdentifier = hash
EOF

"$TONGSUO" req -x509 -new -key "$KEYS_DIR/sm2-pkcs8.pem" \
    -out "$CERTS_DIR/sm2-selfsigned-ext.pem" \
    -days 365 -set_serial 0x4001 \
    -subj "/C=CN/O=SM2 Corp/CN=sm2-test.example.cn" \
    -config "$CERTS_DIR/temp_sm2_ext.cnf" 2>/dev/null
rm -f "$CERTS_DIR/temp_sm2_ext.cnf"
echo "  ✓ sm2-selfsigned-ext.pem"

# 2. SM2 CSR（含 SAN）
cat > "$CSRS_DIR/temp_sm2_csr.cnf" <<EOF
[req]
distinguished_name = dn
prompt = no
req_extensions = req_ext

[dn]

[req_ext]
subjectAltName = DNS:sm2-test.example.cn,DNS:api.sm2-test.example.cn
EOF

"$TONGSUO" req -new -key "$KEYS_DIR/sm2-pkcs8.pem" \
    -out "$CSRS_DIR/sm2-ext.csr" \
    -subj "/C=CN/O=SM2 Corp/CN=sm2-test.example.cn" \
    -config "$CSRS_DIR/temp_sm2_csr.cnf" 2>/dev/null
rm -f "$CSRS_DIR/temp_sm2_csr.cnf"
echo "  ✓ sm2-ext.csr"

echo "完成。"
