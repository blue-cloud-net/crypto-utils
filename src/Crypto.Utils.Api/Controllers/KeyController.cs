using Crypto.Utils.Models;
using Crypto.Utils.Models.Requests;
using Crypto.Utils.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Crypto.Utils.Controllers;

/// <summary>
/// 密钥管理控制器
/// </summary>
[ApiController]
[Route("api/key")]
[Produces("application/json")]
public class KeyController : ControllerBase
{
    private readonly ILogger<KeyController> _logger;
    private readonly IKeyService _keyService;

    public KeyController(
        ILogger<KeyController> logger,
        IKeyService keyService)
    {
        _logger = logger;
        _keyService = keyService;
    }

    /// <summary>
    /// 生成密钥对
    /// </summary>
    /// <param name="request">生成密钥对请求</param>
    /// <returns>密钥对响应</returns>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.KeyPairResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Generate([FromBody] KeyPairGenerateRequest request)
    {
        _logger.LogInformation("生成密钥对: {Algorithm}, KeySize: {KeySize}", request.Algorithm, request.KeySize);
        var result = await _keyService.GenerateKeyPairAsync(request);
        return this.Ok(ApiResponse<Models.Responses.KeyPairResponse>.Ok(result, "密钥对生成成功"));
    }

    /// <summary>
    /// 密钥格式转换 (PEM/DER)
    /// </summary>
    /// <param name="request">格式转换请求</param>
    /// <returns>转换后的密钥</returns>
    [HttpPost("convert")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.KeyConvertResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Convert([FromBody] KeyFormatConvertRequest request)
    {
        _logger.LogInformation("密钥格式转换: {SourceFormat} -> {TargetFormat}", request.SourceFormat, request.TargetFormat);
        var result = await _keyService.ConvertFormatAsync(request);
        return this.Ok(ApiResponse<Models.Responses.KeyConvertResponse>.Ok(result, "格式转换成功"));
    }

    /// <summary>
    /// 解析密钥信息
    /// </summary>
    /// <param name="request">密钥信息请求</param>
    /// <returns>密钥信息</returns>
    [HttpPost("parse")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.KeyInfoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ParseInfo([FromBody] KeyParseRequest request)
    {
        var result = await _keyService.ParseKeyInfoAsync(request);
        return this.Ok(ApiResponse<Models.Responses.KeyInfoResponse>.Ok(result));
    }

    /// <summary>
    /// PKCS 格式转换 (PKCS1/PKCS8)
    /// </summary>
    /// <param name="request">PKCS 格式转换请求</param>
    /// <returns>转换后的密钥</returns>
    [HttpPost("pkcs-convert")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.KeyConvertResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConvertPkcs([FromBody] PkcsConvertRequest request)
    {
        this._logger.LogInformation("PKCS 格式转换: {SourceFormat} -> {TargetFormat}", request.SourceFormat, request.TargetFormat);
        var result = await this._keyService.ConvertPkcsFormatAsync(request);
        return this.Ok(ApiResponse<Models.Responses.KeyConvertResponse>.Ok(result, "PKCS 格式转换成功"));
    }

    /// <summary>
    /// 加密私钥
    /// </summary>
    /// <param name="request">加密请求</param>
    /// <returns>加密后的私钥</returns>
    [HttpPost("encrypt")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.KeyConvertResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Encrypt([FromBody] KeyEncryptRequest request)
    {
        this._logger.LogInformation("加密私钥");
        var result = await this._keyService.EncryptPrivateKeyAsync(request);
        return this.Ok(ApiResponse<Models.Responses.KeyConvertResponse>.Ok(result, "私钥加密成功"));
    }

    /// <summary>
    /// 解密私钥
    /// </summary>
    /// <param name="request">解密请求</param>
    /// <returns>解密后的私钥</returns>
    [HttpPost("decrypt")]
    [ProducesResponseType(typeof(ApiResponse<Models.Responses.KeyConvertResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Decrypt([FromBody] KeyDecryptRequest request)
    {
        this._logger.LogInformation("解密私钥");
        var result = await this._keyService.DecryptPrivateKeyAsync(request);
        return this.Ok(ApiResponse<Models.Responses.KeyConvertResponse>.Ok(result, "私钥解密成功"));
    }
}
