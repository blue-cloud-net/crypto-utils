namespace Crypto.Utils.Models.Responses;

/// <summary>
/// 密钥对响应
/// </summary>
public class KeyPairResponse
{
    /// <summary>
    /// 公钥
    /// </summary>
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>
    /// 私钥
    /// </summary>
    public string PrivateKey { get; set; } = string.Empty;

    /// <summary>
    /// 算法名称
    /// </summary>
    public string Algorithm { get; set; } = string.Empty;

    /// <summary>
    /// 密钥大小
    /// </summary>
    public int? KeySize { get; set; }

    /// <summary>
    /// 曲线名称（EC 密钥）
    /// </summary>
    public string? CurveName { get; set; }
}

/// <summary>
/// 密钥信息响应
/// </summary>
public class KeyInfoResponse
{
    /// <summary>
    /// 算法名称
    /// </summary>
    public string AlgorithmName { get; set; } = string.Empty;

    /// <summary>
    /// 是否为私钥
    /// </summary>
    public bool IsPrivate { get; set; }

    /// <summary>
    /// 是否为公钥
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// 密钥大小
    /// </summary>
    public int? KeySize { get; set; }

    /// <summary>
    /// 曲线 OID（EC 密钥）
    /// </summary>
    public string? CurveOid { get; set; }

    /// <summary>
    /// 曲线名称（EC 密钥）
    /// </summary>
    public string? CurveName { get; set; }

    /// <summary>
    /// 密钥数据（PEM 格式）
    /// </summary>
    public string KeyData { get; set; } = string.Empty;

   /// <summary>
    /// 指纹
    /// </summary>
    public Dictionary<string, string> Fingerprints { get; set; } = new();

    /// <summary>
    /// 算法参数字典
    /// 包含密钥的关键参数，如 RSA 的 Modulus 和 Exponent，EC 的 X 和 Y 坐标等
    /// </summary>
    public Dictionary<string, string>? Parameters { get; set; }
}

/// <summary>
/// 密钥转换响应
/// </summary>
public class KeyConvertResponse
{
    /// <summary>
    /// 转换后的密钥数据
    /// </summary>
    public string ConvertedKey { get; set; } = string.Empty;

    /// <summary>
    /// 目标格式
    /// </summary>
    public string Format { get; set; } = string.Empty;
}
