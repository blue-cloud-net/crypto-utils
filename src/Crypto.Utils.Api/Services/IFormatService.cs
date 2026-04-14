using Crypto.Utils.Models.Requests;
using Crypto.Utils.Models.Responses;

namespace Crypto.Utils.Services;

/// <summary>
/// 格式转换服务接口
/// 支持 KEY、CSR、CERT、CRL 的 PEM 和 DER 格式互转
/// </summary>
public interface IFormatService
{
    /// <summary>
    /// 密钥格式转换 (PEM ↔ DER)
    /// </summary>
    Task<KeyConvertResponse> ConvertKeyFormatAsync(KeyFormatConvertRequest request);

    /// <summary>
    /// CSR 格式转换 (PEM ↔ DER)
    /// </summary>
    Task<FormatConvertResponse> ConvertCsrFormatAsync(FormatConvertRequest request);

    /// <summary>
    /// 证书格式转换 (PEM ↔ DER)
    /// </summary>
    Task<FormatConvertResponse> ConvertCertificateFormatAsync(FormatConvertRequest request);

    /// <summary>
    /// CRL 格式转换 (PEM ↔ DER)
    /// </summary>
    Task<FormatConvertResponse> ConvertCrlFormatAsync(FormatConvertRequest request);

    /// <summary>
    /// 通用格式转换 - 自动检测类型
    /// </summary>
    Task<FormatConvertResponse> ConvertFormatAsync(FormatConvertRequest request);
}
