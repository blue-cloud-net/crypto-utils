using System.ComponentModel.DataAnnotations;

namespace Crypto.Utils.Models.Requests;

/// <summary>
/// 构建证书链请求
/// </summary>
public class ChainBuildRequest
{
    /// <summary>
    /// 目标证书
    /// </summary>
    [Required]
    public string Certificate { get; set; } = string.Empty;

    /// <summary>
    /// 中间证书列表
    /// </summary>
    public List<string>? IntermediateCertificates { get; set; }

    /// <summary>
    /// 根证书列表
    /// </summary>
    public List<string>? RootCertificates { get; set; }
}

/// <summary>
/// 验证证书链请求
/// </summary>
public class ChainVerifyRequest
{
    /// <summary>
    /// 证书链（从叶子到根）
    /// </summary>
    [Required]
    public List<string> CertificateChain { get; set; } = [];

    /// <summary>
    /// 信任的根证书列表
    /// </summary>
    public List<string>? TrustedRoots { get; set; }

    /// <summary>
    /// 检查日期（可选，默认当前时间）
    /// </summary>
    public DateTime? CheckDate { get; set; }
}
