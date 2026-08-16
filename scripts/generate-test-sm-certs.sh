#!/bin/bash

# SM2 测试素材生成脚本（tongsuo）
# 生成国密 SM2 自签名证书、CSR 与 CRL，用于固定测试。
# 输出:
# - tests/data/certs/sm2-selfsigned-ext.pem : SM2 自签名证书，serial 4001，CN=sm2-test.example.cn，含 SAN/KU/EKU
# - tests/data/csrs/sm2-ext.csr             : SM2 CSR，CN=sm2-test.example.cn，含 SAN
# - tests/data/crls/sm2.crl                 : SM2 CRL，颁发者 CN=SM2 Test CRL CA，吊销 serial 5A01(keyCompromise)
# 依赖: generate-test-keys.sh 生成的 SM2 密钥

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
KEYS_DIR="$PROJECT_ROOT/tests/data/keys"
CERTS_DIR="$PROJECT_ROOT/tests/data/certs"
CSRS_DIR="$PROJECT_ROOT/tests/data/csrs"
CRLS_DIR="$PROJECT_ROOT/tests/data/crls"
TONGSUO="${TONGSUO_PATH:-/opt/tongsuo/bin/tongsuo}"

if [ ! -x "$TONGSUO" ]; then
    echo "错误: tongsuo 不可用 ($TONGSUO)。请设置 TONGSUO_PATH 或安装 tongsuo。"
    exit 1
fi

echo "====================================="
echo "SM2 测试素材生成 (tongsuo)"
echo "====================================="

mkdir -p "$CERTS_DIR" "$CSRS_DIR" "$CRLS_DIR"

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

# 3. SM2 CRL（含吊销条目 5A01）
SM2_TEMP_DIR="$CRLS_DIR/temp_sm2_ca_db"
rm -rf "$SM2_TEMP_DIR"
mkdir -p "$SM2_TEMP_DIR/newcerts"
touch "$SM2_TEMP_DIR/index.txt"
echo "unique_subject = no" > "$SM2_TEMP_DIR/index.txt.attr"
echo "01" > "$SM2_TEMP_DIR/serial"

cat > "$SM2_TEMP_DIR/ca.cnf" <<EOF
[ ca ]
default_ca = CA_default

[ CA_default ]
dir = $SM2_TEMP_DIR
database = $SM2_TEMP_DIR/index.txt
new_certs_dir = $SM2_TEMP_DIR/newcerts
certificate = $SM2_TEMP_DIR/ca.crt
private_key = $SM2_TEMP_DIR/ca.key
serial = $SM2_TEMP_DIR/serial
default_md = sm3
default_days = 365
default_crl_days = 30
policy = policy_any

[ policy_any ]
commonName = supplied
EOF

"$TONGSUO" req -x509 -new -key "$KEYS_DIR/sm2-pkcs8.pem" \
    -out "$SM2_TEMP_DIR/ca.crt" -days 3650 -set_serial 0xCA01 \
    -subj "/C=CN/O=SM2 CRL Org/CN=SM2 Test CRL CA" 2>/dev/null
cp "$KEYS_DIR/sm2-pkcs8.pem" "$SM2_TEMP_DIR/ca.key"

# 生成叶子证书并吊销（序列号 5A01）
echo "5A01" > "$SM2_TEMP_DIR/serial"
"$TONGSUO" req -new -key "$KEYS_DIR/sm2-pkcs8.pem" \
    -out "$SM2_TEMP_DIR/cert-5A01.csr" \
    -subj "/C=CN/O=SM2 CRL Org/CN=revoked-5A01" 2>/dev/null
"$TONGSUO" ca -config "$SM2_TEMP_DIR/ca.cnf" -batch -in "$SM2_TEMP_DIR/cert-5A01.csr" \
    -out "$SM2_TEMP_DIR/cert-5A01.pem" \
    -startdate 20250101000000Z -enddate 20350101000000Z 2>/dev/null
"$TONGSUO" ca -config "$SM2_TEMP_DIR/ca.cnf" -batch -revoke "$SM2_TEMP_DIR/cert-5A01.pem" \
    -crl_reason keyCompromise 2>/dev/null

"$TONGSUO" ca -config "$SM2_TEMP_DIR/ca.cnf" -gencrl -out "$CRLS_DIR/sm2.crl" 2>/dev/null
rm -rf "$SM2_TEMP_DIR"
echo "  ✓ sm2.crl"

echo "完成。"
