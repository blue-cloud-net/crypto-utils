namespace Crypto.Utils.Models.Responses;

/// <summary>
/// 格式转换响应
/// </summary>
public class FormatConvertResponse
{
    /// <summary>
    /// 转换后的数据
    /// </summary>
    public string ConvertedData { get; set; } = string.Empty;

    /// <summary>
    /// 目标格式
    /// </summary>
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// 数据类型（KEY/CSR/CERT/CRL）
    /// </summary>
    public string DataType { get; set; } = string.Empty;
}
