using Crypto.Utils.Models.Requests;
using Crypto.Utils.Models.Responses;

namespace Crypto.Utils.Services;

/// <summary>
/// CSR 服务接口
/// </summary>
public interface ICsrService
{
    /// <summary>
    /// 生成 CSR
    /// </summary>
    Task<CsrResponse> GenerateCsrAsync(CsrGenerateRequest request);

    /// <summary>
    /// 解析 CSR 信息
    /// </summary>
    Task<CsrParseResponse> ParseCsrInfoAsync(CsrParseRequest request);

    /// <summary>
    /// 验证 CSR
    /// </summary>
    Task<CsrVerifyResponse> VerifyCsrAsync(CsrVerifyRequest request);
}
