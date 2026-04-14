using Crypto.Utils.Models;
using Crypto.Utils.Models.Requests;
using Crypto.Utils.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Crypto.Utils.Controllers;

/// <summary>
/// 证书管理控制器
/// </summary>
[ApiController]
[Route("api/cert")]
[Produces("application/json")]
public class CertificateController : ControllerBase
{
    private readonly ICertificateService _certificateService;
    private readonly ILogger<CertificateController> _logger;

    public CertificateController(
        ILogger<CertificateController> logger,
        ICertificateService certificateService)
    {
        _certificateService = certificateService;
        _logger = logger;
    }

    /// <summary>
    /// 生成自签名证书
    /// </summary>
    /// <param name="request">自签名证书生成请求</param>
    /// <returns>证书数据</returns>
    [HttpPost("self-signed")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.CertificateResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateSelfSignedAsync([FromBody] SelfSignedCertificateRequest request)
    {
        _logger.LogInformation("生成自签名证书: CN={CN}", request.Subject.CN);
        var result = await _certificateService.GenerateSelfSignedCertificateAsync(request);
        return this.Ok(ApiResponse<Models.Responses.CertificateResponse>.Ok(result, "自签名证书生成成功"));
    }

    /// <summary>
    /// 基于 CSR 签发证书
    /// </summary>
    /// <param name="request">CSR 签发请求</param>
    /// <returns>签发的证书</returns>
    [HttpPost("sign-csr")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.CertificateResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SignCsrAsync([FromBody] SignCsrRequest request)
    {
        _logger.LogInformation("基于 CSR 签发证书");
        var result = await _certificateService.SignByCsrAsync(request);
        return this.Ok(ApiResponse<Models.Responses.CertificateResponse>.Ok(result, "证书签发成功"));
    }

    /// <summary>
    /// 基于公钥签发证书
    /// </summary>
    /// <param name="request">公钥签发请求</param>
    /// <returns>签发的证书</returns>
    [HttpPost("sign-publickey")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.CertificateResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SignPublicKeyAsync([FromBody] SignPublicKeyRequest request)
    {
        _logger.LogInformation("基于公钥签发证书: CN={CN}", request.Subject.CN);
        var result = await _certificateService.SignByPublicKeyAsync(request);
        return this.Ok(ApiResponse<Models.Responses.CertificateResponse>.Ok(result, "证书签发成功"));
    }

    /// <summary>
    /// 直接生成密钥和证书
    /// </summary>
    /// <param name="request">生成请求</param>
    /// <returns>证书和私钥</returns>
    [HttpPost("sign-generate")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.CertificateResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SignAndGenerateAsync([FromBody] SignGenerateRequest request)
    {
        _logger.LogInformation("直接生成密钥和证书: CN={CN}, Algorithm={Algorithm}", 
            request.Subject.CN, request.KeyAlgorithm);
        var result = await _certificateService.SignAndGenerateAsync(request);
        return this.Ok(ApiResponse<Models.Responses.CertificateResponse>.Ok(result, "证书和密钥生成成功"));
    }

    /// <summary>
    /// 解析证书
    /// </summary>
    /// <param name="request">证书解析请求</param>
    /// <returns>证书详细信息</returns>
    [HttpPost("parse")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.CertificateParseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ParseAsync([FromBody] CertificateParseRequest request)
    {
        var result = await _certificateService.ParseCertificateInfoAsync(request);
        return this.Ok(ApiResponse<Models.Responses.CertificateParseResponse>.Ok(result));
    }

    /// <summary>
    /// 验证证书
    /// </summary>
    /// <param name="request">证书验证请求</param>
    /// <returns>验证结果</returns>
    [HttpPost("verify")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.CertificateVerifyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyAsync([FromBody] CertificateVerifyRequest request)
    {
        var result = await _certificateService.VerifyCertificateAsync(request);
        return this.Ok(ApiResponse<Models.Responses.CertificateVerifyResponse>.Ok(result));
    }

    /// <summary>
    /// 证书格式转换
    /// </summary>
    /// <param name="request">格式转换请求</param>
    /// <returns>转换后的证书</returns>
    [HttpPost("convert")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.CertificateResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConvertAsync([FromBody] CertificateConvertRequest request)
    {
        _logger.LogInformation("证书格式转换: {SourceFormat} -> {TargetFormat}", 
            request.SourceFormat, request.TargetFormat);
        var result = await _certificateService.ConvertCertificateAsync(request);
        return this.Ok(ApiResponse<Models.Responses.CertificateResponse>.Ok(result, "证书格式转换成功"));
    }
}
