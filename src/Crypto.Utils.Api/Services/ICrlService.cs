using Crypto.Utils.Models.Requests;
using Crypto.Utils.Models.Responses;

namespace Crypto.Utils.Services;

/// <summary>
/// CRL 服务接口
/// </summary>
public interface ICrlService
{
    /// <summary>
    /// 生成 CRL
    /// </summary>
    Task<CrlResponse> GenerateCrlAsync(CrlGenerateRequest request);

    /// <summary>
    /// 解析 CRL 信息
    /// </summary>
    Task<CrlParseResponse> ParseCrlInfoAsync(CrlParseRequest request);

    /// <summary>
    /// 检查证书吊销状态
    /// </summary>
    Task<CrlCheckResponse> CheckRevocationAsync(CrlCheckRequest request);
}
