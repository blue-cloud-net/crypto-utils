using System.ComponentModel.DataAnnotations;

namespace Crypto.Utils.Models.Requests;

/// <summary>
/// 通用格式转换请求
/// </summary>
public class FormatConvertRequest
{
    /// <summary>
    /// 数据内容
    /// </summary>
    [Required]
    public string Data { get; set; } = string.Empty;

    /// <summary>
    /// 源格式 (PEM/DER)
    /// </summary>
    [Required]
    public string SourceFormat { get; set; } = string.Empty;

    /// <summary>
    /// 目标格式 (PEM/DER)
    /// </summary>
    [Required]
    public string TargetFormat { get; set; } = string.Empty;

    /// <summary>
    /// 数据类型（可选：KEY/CSR/CERT/CRL，不指定则自动检测）
    /// </summary>
    public string? DataType { get; set; }
}
