using System.ComponentModel.DataAnnotations;

namespace Crypto.Utils.Models.Requests;

/// <summary>
/// 生成自签名证书请求
/// </summary>
public class SelfSignedCertificateRequest
{
    /// <summary>
    /// 证书主体
    /// </summary>
    [Required]
    public SubjectInfo Subject { get; set; } = new();

    /// <summary>
    /// 私钥数据
    /// </summary>
    [Required]
    public string PrivateKey { get; set; } = string.Empty;

    /// <summary>
    /// 有效期开始时间
    /// </summary>
    public DateTime? ValidFrom { get; set; }

    /// <summary>
    /// 有效期结束时间
    /// </summary>
    [Required]
    public DateTime ValidTo { get; set; }

    /// <summary>
    /// 签名算法
    /// </summary>
    public string SignatureAlgorithm { get; set; } = "SHA256WITHRSA";

    /// <summary>
    /// 序列号（可选）
    /// </summary>
    public string? SerialNumber { get; set; }

    /// <summary>
    /// 密钥用途
    /// </summary>
    public List<string>? KeyUsage { get; set; }

    /// <summary>
    /// 扩展密钥用途
    /// </summary>
    public List<string>? ExtendedKeyUsage { get; set; }

    /// <summary>
    /// 主题备用名称（SAN）
    /// </summary>
    public List<string>? SubjectAlternativeNames { get; set; }

    /// <summary>
    /// 输出格式 (PEM/DER)
    /// </summary>
    public string OutputFormat { get; set; } = "PEM";
}

/// <summary>
/// 基于 CSR 签发证书请求
/// </summary>
public class SignCsrRequest
{
    /// <summary>
    /// CSR 数据
    /// </summary>
    [Required]
    public string Csr { get; set; } = string.Empty;

    /// <summary>
    /// CA 证书
    /// </summary>
    [Required]
    public string CaCertificate { get; set; } = string.Empty;

    /// <summary>
    /// CA 私钥
    /// </summary>
    [Required]
    public string CaPrivateKey { get; set; } = string.Empty;

    /// <summary>
    /// 有效期开始时间
    /// </summary>
    public DateTime? ValidFrom { get; set; }

    /// <summary>
    /// 有效期结束时间
    /// </summary>
    [Required]
    public DateTime ValidTo { get; set; }

    /// <summary>
    /// 序列号（可选）
    /// </summary>
    public string? SerialNumber { get; set; }

    /// <summary>
    /// 证书扩展
    /// </summary>
    public Dictionary<string, object>? Extensions { get; set; }

    /// <summary>
    /// 输出格式 (PEM/DER)
    /// </summary>
    public string OutputFormat { get; set; } = "PEM";
}

/// <summary>
/// 基于公钥签发证书请求
/// </summary>
public class SignPublicKeyRequest
{
    /// <summary>
    /// 公钥数据
    /// </summary>
    [Required]
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>
    /// 证书主体信息
    /// </summary>
    [Required]
    public SubjectInfo Subject { get; set; } = new();

    /// <summary>
    /// CA 证书
    /// </summary>
    [Required]
    public string CaCertificate { get; set; } = string.Empty;

    /// <summary>
    /// CA 私钥
    /// </summary>
    [Required]
    public string CaPrivateKey { get; set; } = string.Empty;

    /// <summary>
    /// 有效期开始时间
    /// </summary>
    public DateTime? ValidFrom { get; set; }

    /// <summary>
    /// 有效期结束时间
    /// </summary>
    [Required]
    public DateTime ValidTo { get; set; }

    /// <summary>
    /// 签名算法
    /// </summary>
    public string SignatureAlgorithm { get; set; } = "SHA256WITHRSA";

    /// <summary>
    /// 序列号（可选）
    /// </summary>
    public string? SerialNumber { get; set; }

    /// <summary>
    /// 证书扩展
    /// </summary>
    public Dictionary<string, object>? Extensions { get; set; }

    /// <summary>
    /// 输出格式 (PEM/DER)
    /// </summary>
    public string OutputFormat { get; set; } = "PEM";
}

/// <summary>
/// 直接生成密钥和证书请求
/// </summary>
public class SignGenerateRequest
{
    /// <summary>
    /// 证书主体信息
    /// </summary>
    [Required]
    public SubjectInfo Subject { get; set; } = new();

    /// <summary>
    /// 密钥算法 (RSA/EC/SM2)
    /// </summary>
    [Required]
    public string KeyAlgorithm { get; set; } = "RSA";

    /// <summary>
    /// 密钥大小
    /// </summary>
    public int? KeySize { get; set; }

    /// <summary>
    /// EC 曲线名称（可选）
    /// </summary>
    public string? CurveName { get; set; }

    /// <summary>
    /// CA 证书
    /// </summary>
    [Required]
    public string CaCertificate { get; set; } = string.Empty;

    /// <summary>
    /// CA 私钥
    /// </summary>
    [Required]
    public string CaPrivateKey { get; set; } = string.Empty;

    /// <summary>
    /// 有效期开始时间
    /// </summary>
    public DateTime? ValidFrom { get; set; }

    /// <summary>
    /// 有效期结束时间
    /// </summary>
    [Required]
    public DateTime ValidTo { get; set; }

    /// <summary>
    /// 签名算法
    /// </summary>
    public string SignatureAlgorithm { get; set; } = "SHA256WITHRSA";

    /// <summary>
    /// 序列号（可选）
    /// </summary>
    public string? SerialNumber { get; set; }

    /// <summary>
    /// 证书扩展
    /// </summary>
    public Dictionary<string, object>? Extensions { get; set; }

    /// <summary>
    /// 输出格式 (PEM/DER)
    /// </summary>
    public string OutputFormat { get; set; } = "PEM";
}

/// <summary>
/// 解析证书请求
/// </summary>
public class CertificateParseRequest
{
    /// <summary>
    /// 证书数据
    /// </summary>
    public required string CertificateData { get; set; }
}

/// <summary>
/// 验证证书请求
/// </summary>
public class CertificateVerifyRequest
{
    /// <summary>
    /// 待验证证书
    /// </summary>
    [Required]
    public string Certificate { get; set; } = string.Empty;

    /// <summary>
    /// 颁发者证书（可选）
    /// </summary>
    public string? IssuerCertificate { get; set; }

    /// <summary>
    /// 检查日期（可选，默认当前时间）
    /// </summary>
    public DateTime? CheckDate { get; set; }
}

/// <summary>
/// 证书格式转换请求
/// </summary>
public class CertificateConvertRequest
{
    /// <summary>
    /// 证书数据
    /// </summary>
    [Required]
    public string CertificateData { get; set; } = string.Empty;

    /// <summary>
    /// 源格式 (PEM/DER/PFX)
    /// </summary>
    [Required]
    public string SourceFormat { get; set; } = string.Empty;

    /// <summary>
    /// 目标格式 (PEM/DER/PFX)
    /// </summary>
    [Required]
    public string TargetFormat { get; set; } = string.Empty;

    /// <summary>
    /// 密码（PFX 需要）
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 私钥（转换为 PFX 时需要）
    /// </summary>
    public string? PrivateKey { get; set; }
}
