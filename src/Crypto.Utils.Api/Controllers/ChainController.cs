using Crypto.Utils.Models;
using Crypto.Utils.Models.Requests;
using Crypto.Utils.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Crypto.Utils.Controllers;

/// <summary>
/// 证书链管理控制器
/// </summary>
[ApiController]
[Route("api/cert/chain")]
[Produces("application/json")]
public class ChainController : ControllerBase
{
    private readonly ILogger<ChainController> _logger;
    private readonly ICertificateChainService _chainService;

    public ChainController(
        ILogger<ChainController> logger,
        ICertificateChainService chainService)
    {
        _chainService = chainService;
        _logger = logger;
    }

    /// <summary>
    /// 构建证书链
    /// </summary>
    /// <param name="request">证书链构建请求</param>
    /// <returns>证书链</returns>
    [HttpPost("build")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.ChainBuildResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Build([FromBody] ChainBuildRequest request)
    {
        _logger.LogInformation("构建证书链");
        var result = await _chainService.BuildChainAsync(request);
        return this.Ok(ApiResponse<Models.Responses.ChainBuildResponse>.Ok(result));
    }

    /// <summary>
    /// 验证证书链
    /// </summary>
    /// <param name="request">证书链验证请求</param>
    /// <returns>验证结果</returns>
    [HttpPost("verify")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.ChainVerifyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Verify([FromBody] ChainVerifyRequest request)
    {
        _logger.LogInformation("验证证书链，链长度: {Length}", request.CertificateChain.Count);
        var result = await _chainService.VerifyChainAsync(request);
        return this.Ok(ApiResponse<Models.Responses.ChainVerifyResponse>.Ok(result));
    }
}
