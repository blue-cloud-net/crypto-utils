using Crypto.Utils.Models;
using Crypto.Utils.Models.Requests;
using Crypto.Utils.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Crypto.Utils.Controllers;

/// <summary>
/// 格式转换 API
/// 支持 KEY、CSR、CERT、CRL 的 PEM 和 DER 格式互转
/// </summary>
[ApiController]
[Route("api/format")]
[Produces("application/json")]
public class FormatController : ControllerBase
{
    private readonly IFormatService _formatService;
    private readonly ILogger<FormatController> _logger;

    public FormatController(IFormatService formatService, ILogger<FormatController> logger)
    {
        _formatService = formatService;
        _logger = logger;
    }

    /// <summary>
    /// 通用格式转换（自动检测类型）
    /// </summary>
    /// <param name="request">格式转换请求</param>
    /// <returns>转换后的数据</returns>
    [HttpPost("convert")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.FormatConvertResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Convert([FromBody] FormatConvertRequest request)
    {
        _logger.LogInformation("通用格式转换: {SourceFormat} -> {TargetFormat}, 类型: {DataType}", 
            request.SourceFormat, request.TargetFormat, request.DataType ?? "自动检测");
        
        var result = await _formatService.ConvertFormatAsync(request);
        return this.Ok(ApiResponse<Models.Responses.FormatConvertResponse>.Ok(result, "格式转换成功"));
    }

    /// <summary>
    /// 密钥格式转换 (PEM ↔ DER)
    /// </summary>
    /// <param name="request">密钥格式转换请求</param>
    /// <returns>转换后的密钥</returns>
    [HttpPost("key/convert")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.KeyConvertResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConvertKey([FromBody] KeyFormatConvertRequest request)
    {
        _logger.LogInformation("密钥格式转换: {SourceFormat} -> {TargetFormat}", 
            request.SourceFormat, request.TargetFormat);
        
        var result = await _formatService.ConvertKeyFormatAsync(request);
        return this.Ok(ApiResponse<Models.Responses.KeyConvertResponse>.Ok(result, "密钥格式转换成功"));
    }

    /// <summary>
    /// CSR 格式转换 (PEM ↔ DER)
    /// </summary>
    /// <param name="request">CSR 格式转换请求</param>
    /// <returns>转换后的 CSR</returns>
    [HttpPost("csr/convert")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.FormatConvertResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConvertCsr([FromBody] FormatConvertRequest request)
    {
        _logger.LogInformation("CSR 格式转换: {SourceFormat} -> {TargetFormat}", 
            request.SourceFormat, request.TargetFormat);
        
        var result = await _formatService.ConvertCsrFormatAsync(request);
        return this.Ok(ApiResponse<Models.Responses.FormatConvertResponse>.Ok(result, "CSR 格式转换成功"));
    }

    /// <summary>
    /// 证书格式转换 (PEM ↔ DER)
    /// </summary>
    /// <param name="request">证书格式转换请求</param>
    /// <returns>转换后的证书</returns>
    [HttpPost("certificate/convert")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.FormatConvertResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConvertCertificate([FromBody] FormatConvertRequest request)
    {
        _logger.LogInformation("证书格式转换: {SourceFormat} -> {TargetFormat}", 
            request.SourceFormat, request.TargetFormat);
        
        var result = await _formatService.ConvertCertificateFormatAsync(request);
        return this.Ok(ApiResponse<Models.Responses.FormatConvertResponse>.Ok(result, "证书格式转换成功"));
    }

    /// <summary>
    /// CRL 格式转换 (PEM ↔ DER)
    /// </summary>
    /// <param name="request">CRL 格式转换请求</param>
    /// <returns>转换后的 CRL</returns>
    [HttpPost("crl/convert")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.FormatConvertResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConvertCrl([FromBody] FormatConvertRequest request)
    {
        _logger.LogInformation("CRL 格式转换: {SourceFormat} -> {TargetFormat}", 
            request.SourceFormat, request.TargetFormat);
        
        var result = await _formatService.ConvertCrlFormatAsync(request);
        return this.Ok(ApiResponse<Models.Responses.FormatConvertResponse>.Ok(result, "CRL 格式转换成功"));
    }
}
