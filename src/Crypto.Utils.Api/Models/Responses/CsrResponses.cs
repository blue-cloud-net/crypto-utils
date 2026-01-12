namespace Crypto.Utils.Models.Responses;

/// <summary>
/// CSR 生成响应
/// </summary>
public class CsrResponse
{
    /// <summary>
    /// CSR 数据
    /// </summary>
    public string CsrData { get; set; } = string.Empty;

    /// <summary>
    /// 输出格式
    /// </summary>
    public string Format { get; set; } = string.Empty;
}

/// <summary>
/// CSR 解析响应
/// </summary>
public class CsrParseResponse
{
    /// <summary>
    /// 主体信息
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// 签名算法
    /// </summary>
    public string SignatureAlgorithmName { get; set; } = string.Empty;

    /// <summary>
    /// 公钥信息
    /// </summary>
    public KeyInfoResponse PublicKey { get; set; } = new();

    /// <summary>
    /// 扩展字段
    /// </summary>
    public Dictionary<string, object>? Extensions { get; set; }
}

/// <summary>
/// CSR 验证响应
/// </summary>
public class CsrVerifyResponse
{
    /// <summary>
    /// 是否有效
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// 验证消息
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
