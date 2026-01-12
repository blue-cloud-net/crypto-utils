using Crypto.Utils.Models;
using Crypto.Utils.Models.Requests;
using Crypto.Utils.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Crypto.Utils.Controllers;

/// <summary>
/// CRL (证书吊销列表) 控制器
/// </summary>
[ApiController]
[Route("api/crl")]
[Produces("application/json")]
public class CrlController : ControllerBase
{
    private readonly ILogger<CrlController> _logger;
    private readonly ICrlService _crlService;

    public CrlController(
        ILogger<CrlController> logger,
        ICrlService crlService)
    {
        _crlService = crlService;
        _logger = logger;
    }

    /// <summary>
    /// 生成 CRL
    /// </summary>
    /// <param name="request">CRL 生成请求</param>
    /// <returns>CRL 数据</returns>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.CrlResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Generate([FromBody] CrlGenerateRequest request)
    {
        _logger.LogInformation("生成 CRL");
        var result = await _crlService.GenerateCrlAsync(request);
        return this.Ok(ApiResponse<Models.Responses.CrlResponse>.Ok(result, "CRL 生成成功"));
    }

    /// <summary>
    /// 解析 CRL
    /// </summary>
    /// <param name="request">CRL 解析请求</param>
    /// <returns>CRL 信息</returns>
    [HttpPost("parse")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.CrlParseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Parse([FromBody] CrlParseRequest request)
    {
        _logger.LogInformation("解析 CRL");
        var result = await _crlService.ParseCrlInfoAsync(request);
        return this.Ok(ApiResponse<Models.Responses.CrlParseResponse>.Ok(result));
    }

    /// <summary>
    /// 检查证书吊销状态
    /// </summary>
    /// <param name="request">吊销检查请求</param>
    /// <returns>吊销状态</returns>
    [HttpPost("check")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.CrlCheckResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckRevocation([FromBody] CrlCheckRequest request)
    {
        _logger.LogInformation("检查证书吊销状态");
        var result = await _crlService.CheckRevocationAsync(request);
        return this.Ok(ApiResponse<Models.Responses.CrlCheckResponse>.Ok(result));
    }
}
