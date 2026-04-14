using System.ComponentModel.DataAnnotations;

namespace Crypto.Utils.Models.Requests;

/// <summary>
/// 生成 CSR 请求
/// </summary>
public class CsrGenerateRequest
{
    /// <summary>
    /// 证书主体信息 (DN)
    /// </summary>
    [Required]
    public SubjectInfo Subject { get; set; } = new();

    /// <summary>
    /// 私钥数据
    /// </summary>
    [Required]
    public string PrivateKey { get; set; } = string.Empty;

    /// <summary>
    /// 签名算法
    /// </summary>
    public string SignatureAlgorithm { get; set; } = "SHA256WITHRSA";

    /// <summary>
    /// 扩展字段（可选）
    /// </summary>
    public Dictionary<string, string>? Extensions { get; set; }

    /// <summary>
    /// 输出格式 (PEM/DER)
    /// </summary>
    public string OutputFormat { get; set; } = "PEM";
}

/// <summary>
/// 解析 CSR 请求
/// </summary>
public class CsrParseRequest
{
    /// <summary>
    /// CSR 数据
    /// </summary>
    public required string CsrData { get; set; }
}

/// <summary>
/// 验证 CSR 请求
/// </summary>
public class CsrVerifyRequest
{
    /// <summary>
    /// CSR 数据
    /// </summary>
    [Required]
    public string CsrData { get; set; } = string.Empty;
}

/// <summary>
/// 证书主体信息
/// </summary>
public class SubjectInfo
{
    /// <summary>
    /// Common Name
    /// </summary>
    [Required]
    public string CN { get; set; } = string.Empty;

    /// <summary>
    /// Organization
    /// </summary>
    public string? O { get; set; }

    /// <summary>
    /// Organizational Unit
    /// </summary>
    public string? OU { get; set; }

    /// <summary>
    /// Country
    /// </summary>
    public string? C { get; set; }

    /// <summary>
    /// State
    /// </summary>
    public string? ST { get; set; }

    /// <summary>
    /// Locality
    /// </summary>
    public string? L { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public string? E { get; set; }
}
