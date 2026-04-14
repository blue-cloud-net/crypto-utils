using System.ComponentModel.DataAnnotations;

namespace Crypto.Utils.Models.Requests;

/// <summary>
/// 生成 CRL 请求
/// </summary>
public class CrlGenerateRequest
{
    /// <summary>
    /// 颁发者 DN
    /// </summary>
    [Required]
    public SubjectInfo Issuer { get; set; } = new();

    /// <summary>
    /// 吊销证书列表
    /// </summary>
    public List<RevokedCertificateInfo>? RevokedCertificates { get; set; }

    /// <summary>
    /// CA 私钥
    /// </summary>
    [Required]
    public string CaPrivateKey { get; set; } = string.Empty;

    /// <summary>
    /// 签名算法
    /// </summary>
    public string SignatureAlgorithm { get; set; } = "SHA256WITHRSA";

    /// <summary>
    /// 本次更新时间
    /// </summary>
    public DateTime? ThisUpdate { get; set; }

    /// <summary>
    /// 下次更新时间
    /// </summary>
    [Required]
    public DateTime NextUpdate { get; set; }

    /// <summary>
    /// 输出格式 (PEM/DER)
    /// </summary>
    public string OutputFormat { get; set; } = "PEM";
}

/// <summary>
/// 解析 CRL 请求
/// </summary>
public class CrlParseRequest
{
    /// <summary>
    /// CRL 数据
    /// </summary>
    [Required]
    public string CrlData { get; set; } = string.Empty;
}

/// <summary>
/// 检查证书吊销状态请求
/// </summary>
public class CrlCheckRequest
{
    /// <summary>
    /// 证书数据
    /// </summary>
    [Required]
    public string Certificate { get; set; } = string.Empty;

    /// <summary>
    /// CRL 数据
    /// </summary>
    [Required]
    public string CrlData { get; set; } = string.Empty;
}

/// <summary>
/// 吊销证书信息
/// </summary>
public class RevokedCertificateInfo
{
    /// <summary>
    /// 证书序列号
    /// </summary>
    [Required]
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>
    /// 吊销日期
    /// </summary>
    [Required]
    public DateTime RevocationDate { get; set; }

    /// <summary>
    /// 吊销原因
    /// </summary>
    public string? Reason { get; set; }
}
