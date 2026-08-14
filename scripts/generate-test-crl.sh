#!/bin/bash

# CRL 测试素材生成脚本（openssl）
# 生成包含两个吊销条目（序列号 1111 / 2222，不同吊销原因）的 CRL。
# 输出: tests/data/crls/test.crl
#
# 生成参数（作为单元测试断言依据）：
# - 颁发者: CN=Test CRL CA
# - 吊销条目: serial 1111 (keyCompromise), serial 2222 (superseded)
# - 吊销时间由生成时刻决定，测试仅断言条目的存在与序列号

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
KEYS_DIR="$PROJECT_ROOT/tests/data/keys"
OUTPUT_DIR="$PROJECT_ROOT/tests/data/crls"

echo "====================================="
echo "CRL 测试素材生成"
echo "====================================="

mkdir -p "$OUTPUT_DIR"
TEMP_DIR="$OUTPUT_DIR/temp_ca_db"
rm -rf "$TEMP_DIR"
mkdir -p "$TEMP_DIR/newcerts"
touch "$TEMP_DIR/index.txt"
echo "unique_subject = no" > "$TEMP_DIR/index.txt.attr"
echo "01" > "$TEMP_DIR/serial"

if [ ! -f "$KEYS_DIR/rsa-2048-pkcs1.pem" ]; then
    "$SCRIPT_DIR/generate-test-keys.sh"
fi

# CA 配置
cat > "$TEMP_DIR/ca.cnf" <<EOF
[ ca ]
default_ca = CA_default

[ CA_default ]
dir = $TEMP_DIR
database = $TEMP_DIR/index.txt
new_certs_dir = $TEMP_DIR/newcerts
certificate = $TEMP_DIR/ca.crt
private_key = $TEMP_DIR/ca.key
serial = $TEMP_DIR/serial
default_md = sha256
default_days = 365
default_crl_days = 30
policy = policy_any

[ policy_any ]
commonName = supplied
EOF

# CA 自签名证书
openssl req -x509 -new -key "$KEYS_DIR/rsa-2048-pkcs1.pem" \
    -out "$TEMP_DIR/ca.crt" -days 3650 -set_serial 0xCA01 \
    -subj "/C=CN/O=CRL Org/CN=Test CRL CA" 2>/dev/null

# CA 私钥（供 openssl ca 使用）
cp "$KEYS_DIR/rsa-2048-pkcs1.pem" "$TEMP_DIR/ca.key"

# 生成两个叶子证书并吊销
for entry in "1111:keyCompromise" "2222:superseded"; do
    serial="${entry%%:*}"
    reason="${entry##*:}"
    cert="$TEMP_DIR/cert-$serial.pem"
    csr="$TEMP_DIR/cert-$serial.csr"

    openssl req -new -key "$KEYS_DIR/rsa-2048-pkcs1.pem" \
        -out "$csr" -subj "/C=CN/O=CRL Org/CN=revoked-$serial" 2>/dev/null

    # 指定序列号签发
    echo "$serial" > "$TEMP_DIR/serial"
    openssl ca -config "$TEMP_DIR/ca.cnf" -batch -in "$csr" -out "$cert" \
        -startdate 20250101000000Z -enddate 20350101000000Z 2>/dev/null

    openssl ca -config "$TEMP_DIR/ca.cnf" -batch -revoke "$cert" \
        -crl_reason "$reason" 2>/dev/null
done

# 生成 CRL
openssl ca -config "$TEMP_DIR/ca.cnf" -gencrl -out "$OUTPUT_DIR/test.crl" 2>/dev/null

rm -rf "$TEMP_DIR"

echo "  ✓ test.crl"
echo "完成。"
