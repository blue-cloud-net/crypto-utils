using Crypto.Utils.X509.Enums;

namespace Crypto.Utils.Models.Responses;

/// <summary>
/// CRL 生成响应
/// </summary>
public class CrlResponse
{
    /// <summary>
    /// CRL 数据
    /// </summary>
    public string CrlData { get; set; } = string.Empty;

    /// <summary>
    /// 输出格式
    /// </summary>
    public string Format { get; set; } = string.Empty;
}

/// <summary>
/// CRL 解析响应
/// </summary>
public class CrlParseResponse
{
    /// <summary>
    /// 颁发者信息
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// 本次更新时间
    /// </summary>
    public DateTime ThisUpdate { get; set; }

    /// <summary>
    /// 下次更新时间
    /// </summary>
    public DateTime NextUpdate { get; set; }

    /// <summary>
    /// 签名算法
    /// </summary>
    public string SignatureAlgorithmName { get; set; } = string.Empty;

    /// <summary>
    /// 吊销证书列表
    /// </summary>
    public List<RevokedCertificateDetail>? RevokedCertificates { get; set; }
}

/// <summary>
/// CRL 检查响应
/// </summary>
public class CrlCheckResponse
{
    /// <summary>
    /// 是否被吊销
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// 吊销日期
    /// </summary>
    public DateTime? RevocationDate { get; set; }

    /// <summary>
    /// 吊销原因
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// 吊销证书详情
/// </summary>
public class RevokedCertificateDetail
{
    /// <summary>
    /// 证书序列号
    /// </summary>
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>
    /// 吊销日期
    /// </summary>
    public DateTime RevocationDate { get; set; }

    /// <summary>
    /// 吊销原因
    /// </summary>
    public CertificateRevocationReason? Reason { get; set; }
}
