namespace Crypto.Utils.Models.Responses;

/// <summary>
/// 证书链构建响应
/// </summary>
public class ChainBuildResponse
{
    /// <summary>
    /// 证书链（从叶子到根）
    /// </summary>
    public List<string> CertificateChain { get; set; } = [];

    /// <summary>
    /// 链长度
    /// </summary>
    public int ChainLength { get; set; }

    /// <summary>
    /// 是否完整
    /// </summary>
    public bool IsComplete { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// 证书链验证响应
/// </summary>
public class ChainVerifyResponse
{
    /// <summary>
    /// 是否有效
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// 验证消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 各级证书验证详情
    /// </summary>
    public List<CertificateValidationDetail>? CertificateDetails { get; set; }
}

/// <summary>
/// 证书验证详情
/// </summary>
public class CertificateValidationDetail
{
    /// <summary>
    /// 证书主体
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// 证书颁发者
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// 是否有效
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// 验证消息
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
