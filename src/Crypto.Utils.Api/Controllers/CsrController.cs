using Crypto.Utils.Models;
using Crypto.Utils.Models.Requests;
using Crypto.Utils.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Crypto.Utils.Controllers;

/// <summary>
/// CSR (证书签名请求) 控制器
/// </summary>
[ApiController]
[Route("api/csr")]
[Produces("application/json")]
public class CsrController : ControllerBase
{
    private readonly ICsrService _csrService;
    private readonly ILogger<CsrController> _logger;

    public CsrController(ICsrService csrService, ILogger<CsrController> logger)
    {
        _csrService = csrService;
        _logger = logger;
    }

    /// <summary>
    /// 生成 CSR
    /// </summary>
    /// <param name="request">CSR 生成请求</param>
    /// <returns>CSR 数据</returns>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.CsrResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Generate([FromBody] CsrGenerateRequest request)
    {
        _logger.LogInformation("生成 CSR: CN={CN}", request.Subject.CN);
        var result = await _csrService.GenerateCsrAsync(request);
        return this.Ok(ApiResponse<Models.Responses.CsrResponse>.Ok(result, "CSR 生成成功"));
    }

    /// <summary>
    /// 解析 CSR
    /// </summary>
    /// <param name="request">CSR 解析请求</param>
    /// <returns>CSR 信息</returns>
    [HttpPost("parse")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.CsrParseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Parse([FromBody] CsrParseRequest request)
    {
        _logger.LogInformation("解析 CSR");
        var result = await _csrService.ParseCsrInfoAsync(request);
        return this.Ok(ApiResponse<Models.Responses.CsrParseResponse>.Ok(result));
    }

    /// <summary>
    /// 验证 CSR
    /// </summary>
    /// <param name="request">CSR 验证请求</param>
    /// <returns>验证结果</returns>
    [HttpPost("verify")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.CsrVerifyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Verify([FromBody] CsrVerifyRequest request)
    {
        _logger.LogInformation("验证 CSR");
        var result = await _csrService.VerifyCsrAsync(request);
        return this.Ok(ApiResponse<Models.Responses.CsrVerifyResponse>.Ok(result));
    }
}
