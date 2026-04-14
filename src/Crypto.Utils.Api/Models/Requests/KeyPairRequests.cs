using System.ComponentModel.DataAnnotations;

namespace Crypto.Utils.Models.Requests;

/// <summary>
/// 生成密钥对请求
/// </summary>
public class KeyPairGenerateRequest
{
    /// <summary>
    /// 算法类型 (RSA/EC/SM2/DSA)
    /// </summary>
    [Required]
    public string Algorithm { get; set; } = "RSA";

    /// <summary>
    /// 密钥大小 (RSA: 2048/4096, EC: 256/384/521)
    /// </summary>
    public int? KeySize { get; set; }

    /// <summary>
    /// EC 曲线名称（可选）
    /// </summary>
    public string? CurveName { get; set; }

    /// <summary>
    /// 输出格式 (PEM/DER)
    /// </summary>
    public string OutputFormat { get; set; } = "PEM";
}

/// <summary>
/// 密钥格式转换请求
/// </summary>
public class KeyFormatConvertRequest
{
    /// <summary>
    /// 密钥数据
    /// </summary>
    public required string KeyData { get; set; }

    /// <summary>
    /// 源格式 (PEM/DER)
    /// </summary>
    public required string SourceFormat { get; set; }

    /// <summary>
    /// 目标格式 (PEM/DER)
    /// </summary>
    public required string TargetFormat { get; set; }
}

/// <summary>
/// PKCS 格式转换请求
/// </summary>
public class PkcsConvertRequest
{
    /// <summary>
    /// 密钥数据
    /// </summary>
    [Required]
    public string KeyData { get; set; } = string.Empty;

    /// <summary>
    /// 源格式 (PKCS1/PKCS8)
    /// </summary>
    [Required]
    public string SourceFormat { get; set; } = string.Empty;

    /// <summary>
    /// 目标格式 (PKCS1/PKCS8)
    /// </summary>
    [Required]
    public string TargetFormat { get; set; } = string.Empty;

    /// <summary>
    /// 加密密码（可选，用于加密的 PKCS#8）
    /// </summary>
    public string? Password { get; set; }
}

/// <summary>
/// 密钥加密请求
/// </summary>
public class KeyEncryptRequest
{
    /// <summary>
    /// 私钥数据
    /// </summary>
    [Required]
    public string PrivateKey { get; set; } = string.Empty;

    /// <summary>
    /// 加密密码
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 加密算法（默认 AES-256-CBC）
    /// </summary>
    public string Algorithm { get; set; } = "AES-256-CBC";
}

/// <summary>
/// 密钥解密请求
/// </summary>
public class KeyDecryptRequest
{
    /// <summary>
    /// 加密的私钥数据
    /// </summary>
    [Required]
    public string EncryptedPrivateKey { get; set; } = string.Empty;

    /// <summary>
    /// 解密密码
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// 密钥信息查询请求
/// </summary>
public class KeyParseRequest
{
    /// <summary>
    /// 密钥数据（PEM/DER）
    /// </summary>
    public required string KeyData { get; set; }
}
