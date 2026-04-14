namespace Crypto.Utils.Models.Responses;

/// <summary>
/// 证书响应
/// </summary>
public class CertificateResponse
{
    /// <summary>
    /// 证书数据
    /// </summary>
    public string CertificateData { get; set; } = String.Empty;

    /// <summary>
    /// 输出格式
    /// </summary>
    public string Format { get; set; } = String.Empty;

    /// <summary>
    /// 私钥（仅在生成密钥和证书时返回）
    /// </summary>
    public string? PrivateKey { get; set; }
}

/// <summary>
/// 证书解析响应
/// </summary>
public class CertificateParseResponse
{
    /// <summary>
    /// 版本
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 序列号
    /// </summary>
    public string SerialNumber { get; set; } = String.Empty;

    /// <summary>
    /// 主体信息
    /// </summary>
    public string Subject { get; set; } = String.Empty;

    /// <summary>
    /// 颁发者信息
    /// </summary>
    public string Issuer { get; set; } = String.Empty;

    /// <summary>
    /// 有效期开始时间
    /// </summary>
    public DateTime NotBefore { get; set; }

    /// <summary>
    /// 有效期结束时间
    /// </summary>
    public DateTime NotAfter { get; set; }

    /// <summary>
    /// 有效期（天数）
    /// </summary>
    public double Duration { get; set; }

    /// <summary>
    /// 是否有效
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// 有效期剩余天数
    /// </summary>
    public double RemainingDays { get; set; }

    /// <summary>
    /// 签名算法Oid
    /// </summary>
    public string SignatureAlgorithmOid { get; set; } = String.Empty;

    /// <summary>
    /// 签名算法名称
    /// </summary>
    public string SignatureAlgorithmName { get; set; } = String.Empty;

    /// <summary>
    /// 指纹
    /// </summary>
    public Dictionary<string, string> Fingerprints { get; set; } = new();

    /// <summary>
    /// 公钥信息
    /// </summary>
    public KeyInfoResponse PublicKey { get; set; } = new();

    /// <summary>
    /// 是否为 CA 证书
    /// </summary>
    public bool IsCA { get; set; }

    /// <summary>
    /// 路径长度约束（仅对 CA 证书有效）
    /// </summary>
    public int? PathLengthConstraint { get; set; }

    /// <summary>
    /// 主题密钥标识符（Subject Key Identifier）
    /// </summary>
    public string? SubjectKeyIdentifier { get; set; }

    /// <summary>
    /// 颁发机构密钥标识符（Authority Key Identifier）
    /// </summary>
    public string? AuthorityKeyIdentifier { get; set; }

    /// <summary>
    /// 密钥用途
    /// </summary>
    public IEnumerable<string>? KeyUsage { get; set; }

    /// <summary>
    /// 扩展密钥用途
    /// </summary>
    public IEnumerable<string>? ExtendedKeyUsage { get; set; }

    /// <summary>
    /// 主题备用名称（格式：类型: 值，如 DNS: www.example.com）
    /// </summary>
    public IEnumerable<string>? SubjectAlternativeNames { get; set; }

    /// <summary>
    /// 证书策略 OID 列表
    /// </summary>
    public IEnumerable<string>? CertificatePolicies { get; set; }

    /// <summary>
    /// CRL 分发点
    /// </summary>
    public IEnumerable<string>? CrlDistributionPoints { get; set; }

    /// <summary>
    /// 颁发机构信息访问（Authority Information Access）
    /// </summary>
    public AuthorityInformationAccessResponse? AuthorityInformationAccess { get; set; }

    /// <summary>
    /// 扩展字段
    /// </summary>
    public Dictionary<string, object>? Extensions { get; set; }
}

/// <summary>
/// 颁发机构信息访问响应
/// </summary>
public class AuthorityInformationAccessResponse
{
    /// <summary>
    /// OCSP 服务器 URL 列表
    /// </summary>
    public IEnumerable<string> OcspUrls { get; set; } = [];

    /// <summary>
    /// CA 证书下载 URL 列表
    /// </summary>
    public IEnumerable<string> CaIssuers { get; set; } = [];
}

/// <summary>
/// 证书验证响应
/// </summary>
public class CertificateVerifyResponse
{
    /// <summary>
    /// 签名是否有效
    /// </summary>
    public bool SignatureValid { get; set; }

    /// <summary>
    /// 是否在有效期内
    /// </summary>
    public bool DateValid { get; set; }

    /// <summary>
    /// 证书链是否完整
    /// </summary>
    public bool ChainValid { get; set; }

    /// <summary>
    /// 总体是否有效
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// 验证消息
    /// </summary>
    public string Message { get; set; } = String.Empty;

    /// <summary>
    /// 详细信息
    /// </summary>
    public List<string>? Details { get; set; }
}
