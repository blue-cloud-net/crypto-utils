using Crypto.Utils.Models.Requests;
using Crypto.Utils.Models.Responses;

namespace Crypto.Utils.Services;

/// <summary>
/// 证书链服务接口
/// </summary>
public interface ICertificateChainService
{
    /// <summary>
    /// 构建证书链
    /// </summary>
    Task<ChainBuildResponse> BuildChainAsync(ChainBuildRequest request);

    /// <summary>
    /// 验证证书链
    /// </summary>
    Task<ChainVerifyResponse> VerifyChainAsync(ChainVerifyRequest request);
}
