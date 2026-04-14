using Crypto.Utils.Models.Requests;
using Crypto.Utils.Models.Responses;

namespace Crypto.Utils.Services;

/// <summary>
/// 证书服务接口
/// </summary>
public interface ICertificateService
{
    /// <summary>
    /// 生成自签名证书
    /// </summary>
    Task<CertificateResponse> GenerateSelfSignedCertificateAsync(SelfSignedCertificateRequest request);

    /// <summary>
    /// 基于 CSR 签发证书
    /// </summary>
    Task<CertificateResponse> SignByCsrAsync(SignCsrRequest request);

    /// <summary>
    /// 基于公钥签发证书
    /// </summary>
    Task<CertificateResponse> SignByPublicKeyAsync(SignPublicKeyRequest request);

    /// <summary>
    /// 直接生成密钥和证书
    /// </summary>
    Task<CertificateResponse> SignAndGenerateAsync(SignGenerateRequest request);

    /// <summary>
    /// 解析证书（包含指纹和公钥提取）
    /// </summary>
    Task<CertificateParseResponse> ParseCertificateInfoAsync(CertificateParseRequest request);

    /// <summary>
    /// 验证证书
    /// </summary>
    Task<CertificateVerifyResponse> VerifyCertificateAsync(CertificateVerifyRequest request);

    /// <summary>
    /// 证书格式转换
    /// </summary>
    Task<CertificateResponse> ConvertCertificateAsync(CertificateConvertRequest request);
}
