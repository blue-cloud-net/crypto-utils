using Crypto.Utils.Crypto;
using Crypto.Utils.Mappers;
using Crypto.Utils.Models.Requests;
using Crypto.Utils.Models.Responses;
using Crypto.Utils.X509;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace Crypto.Utils.Services.Implementations;

/// <summary>
/// 证书服务实现
/// </summary>
public class CertificateService : ICertificateService
{
    private readonly ILogger<CertificateService> _logger;
    private readonly IFormatService _formatService;

    public CertificateService(
        ILogger<CertificateService> logger,
         IFormatService formatService)
    {
        _logger = logger;
        _formatService = formatService;
    }

    public async Task<CertificateResponse> GenerateSelfSignedCertificateAsync(SelfSignedCertificateRequest request)
    {
        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("生成自签名证书: Subject={Subject}", request.Subject.CN);

                // 解析私钥
                var privateKey = request.PrivateKey.Contains("BEGIN")
                    ? AsymmetricPrivateKeyParameter.FromPem(request.PrivateKey)
                    : AsymmetricPrivateKeyParameter.FromDer(Convert.FromBase64String(request.PrivateKey));

                // 构建 Subject DN
                var subjectDN = this.BuildDistinguishedName(request.Subject);

                // 生成证书
                var validFrom = request.ValidFrom ?? DateTime.UtcNow;
                var validTo = request.ValidTo;
                var serialNumber = request.SerialNumber ?? this.GenerateSerialNumber();

                var certificate = Certificate.GenerateSelfSigned(
                    subjectDN,
                    privateKey,
                    validFrom,
                    validTo,
                    serialNumber,
                    request.SignatureAlgorithm
                );

                // 导出证书
                var certData = request.OutputFormat.ToUpperInvariant() == "DER"
                    ? Convert.ToBase64String(certificate.ToDer())
                    : certificate.ToPem();

                _logger.LogInformation("自签名证书生成成功");

                return new CertificateResponse
                {
                    CertificateData = certData,
                    Format = request.OutputFormat.ToUpperInvariant()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成自签名证书失败");
                throw new InvalidOperationException($"生成自签名证书失败: {ex.Message}", ex);
            }
        });
    }

    public async Task<CertificateResponse> SignByCsrAsync(SignCsrRequest request)
    {
        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("基于 CSR 签发证书");

                // 解析 CSR
                var csr = request.Csr.Contains("BEGIN")
                    ? CertificateSigningRequest.FromPem(request.Csr)
                    : CertificateSigningRequest.FromDer(Convert.FromBase64String(request.Csr));

                // 解析 CA 证书和私钥
                var caCert = request.CaCertificate.Contains("BEGIN")
                    ? Certificate.FromPem(request.CaCertificate)
                    : Certificate.FromDer(Convert.FromBase64String(request.CaCertificate));

                var caPrivateKey = request.CaPrivateKey.Contains("BEGIN")
                    ? AsymmetricPrivateKeyParameter.FromPem(request.CaPrivateKey)
                    : AsymmetricPrivateKeyParameter.FromDer(Convert.FromBase64String(request.CaPrivateKey));

                // 签发证书
                var validFrom = request.ValidFrom ?? DateTime.UtcNow;
                var validTo = request.ValidTo;
                var serialNumber = request.SerialNumber ?? this.GenerateSerialNumber();

                var certificate = Certificate.SignCsr(
                    csr,
                    caCert,
                    caPrivateKey,
                    validFrom,
                    validTo,
                    serialNumber
                );

                // 导出证书
                var certData = request.OutputFormat.ToUpperInvariant() == "DER"
                    ? Convert.ToBase64String(certificate.ToDer())
                    : certificate.ToPem();

                _logger.LogInformation("基于 CSR 签发证书成功");

                return new CertificateResponse
                {
                    CertificateData = certData,
                    Format = request.OutputFormat.ToUpperInvariant()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "基于 CSR 签发证书失败");
                throw new InvalidOperationException($"基于 CSR 签发证书失败: {ex.Message}", ex);
            }
        });
    }

    public async Task<CertificateResponse> SignByPublicKeyAsync(SignPublicKeyRequest request)
    {
        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("基于公钥签发证书: Subject={Subject}", request.Subject.CN);

                // 解析公钥
                var publicKey = request.PublicKey.Contains("BEGIN")
                    ? AsymmetricPublicKeyParameter.FromPem(request.PublicKey)
                    : AsymmetricPublicKeyParameter.FromDer(Convert.FromBase64String(request.PublicKey));

                // 解析 CA 证书和私钥
                var caCert = request.CaCertificate.Contains("BEGIN")
                    ? Certificate.FromPem(request.CaCertificate)
                    : Certificate.FromDer(Convert.FromBase64String(request.CaCertificate));

                var caPrivateKey = request.CaPrivateKey.Contains("BEGIN")
                    ? AsymmetricPrivateKeyParameter.FromPem(request.CaPrivateKey)
                    : AsymmetricPrivateKeyParameter.FromDer(Convert.FromBase64String(request.CaPrivateKey));

                // 构建 Subject DN
                var subjectDN = this.BuildDistinguishedName(request.Subject);

                // 签发证书
                var validFrom = request.ValidFrom ?? DateTime.UtcNow;
                var validTo = request.ValidTo;
                var serialNumber = request.SerialNumber ?? this.GenerateSerialNumber();

                var certificate = Certificate.SignPublicKey(
                    publicKey,
                    subjectDN,
                    caCert,
                    caPrivateKey,
                    validFrom,
                    validTo,
                    serialNumber,
                    request.SignatureAlgorithm
                );

                // 导出证书
                var certData = request.OutputFormat.ToUpperInvariant() == "DER"
                    ? Convert.ToBase64String(certificate.ToDer())
                    : certificate.ToPem();

                _logger.LogInformation("基于公钥签发证书成功");

                return new CertificateResponse
                {
                    CertificateData = certData,
                    Format = request.OutputFormat.ToUpperInvariant()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "基于公钥签发证书失败");
                throw new InvalidOperationException($"基于公钥签发证书失败: {ex.Message}", ex);
            }
        });
    }

    public async Task<CertificateResponse> SignAndGenerateAsync(SignGenerateRequest request)
    {
        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("直接生成密钥和证书: Subject={Subject}, 算法={Algorithm}",
                    request.Subject.CN, request.KeyAlgorithm);

                // 生成密钥对
                AsymmetricKeyPair keyPair;
                switch (request.KeyAlgorithm.ToUpper())
                {
                    case "RSA":
                        keyPair = AsymmetricKeyPair.GenerateRsa(request.KeySize ?? 2048);
                        break;
                    case "EC":
                        keyPair = AsymmetricKeyPair.GenerateEc(request.CurveName ?? "secp256r1");
                        break;
                    case "SM2":
                        keyPair = AsymmetricKeyPair.GenerateEc("sm2p256v1");
                        break;
                    default:
                        throw new ArgumentException($"不支持的算法: {request.KeyAlgorithm}");
                }

                // 解析 CA 证书和私钥
                var caCert = request.CaCertificate.Contains("BEGIN")
                    ? Certificate.FromPem(request.CaCertificate)
                    : Certificate.FromDer(Convert.FromBase64String(request.CaCertificate));

                var caPrivateKey = request.CaPrivateKey.Contains("BEGIN")
                    ? AsymmetricPrivateKeyParameter.FromPem(request.CaPrivateKey)
                    : AsymmetricPrivateKeyParameter.FromDer(Convert.FromBase64String(request.CaPrivateKey));

                // 构建 Subject DN
                var subjectDN = this.BuildDistinguishedName(request.Subject);

                // 签发证书
                var validFrom = request.ValidFrom ?? DateTime.UtcNow;
                var validTo = request.ValidTo;
                var serialNumber = request.SerialNumber ?? this.GenerateSerialNumber();

                var certificate = Certificate.SignPublicKey(
                    keyPair.PublicKey,
                    subjectDN,
                    caCert,
                    caPrivateKey,
                    validFrom,
                    validTo,
                    serialNumber,
                    request.SignatureAlgorithm
                );

                // 导出证书和私钥
                var certData = request.OutputFormat.ToUpperInvariant() == "DER"
                    ? Convert.ToBase64String(certificate.ToDer())
                    : certificate.ToPem();

                var privateKeyData = request.OutputFormat.ToUpperInvariant() == "DER"
                    ? Convert.ToBase64String(keyPair.PrivateKey.ToDer())
                    : keyPair.PrivateKey.ToPem();

                _logger.LogInformation("直接生成密钥和证书成功");

                return new CertificateResponse
                {
                    CertificateData = certData,
                    PrivateKey = privateKeyData,
                    Format = request.OutputFormat.ToUpperInvariant()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "直接生成密钥和证书失败");
                throw new InvalidOperationException($"直接生成密钥和证书失败: {ex.Message}", ex);
            }
        });
    }

    public async Task<CertificateParseResponse> ParseCertificateInfoAsync(CertificateParseRequest request)
    {
        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("解析证书");

                // 解析证书
                var certificate = request.CertificateData.Contains("BEGIN")
                    ? Certificate.FromPem(request.CertificateData)
                    : Certificate.FromDer(Convert.FromBase64String(request.CertificateData));

                // 使用 Mapper 转换为响应对象
                var response = certificate.ToCertificateParseResponse();

                _logger.LogInformation("证书解析成功");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "解析证书失败");
                throw new InvalidOperationException($"解析证书失败: {ex.Message}", ex);
            }
        });
    }

    public async Task<CertificateVerifyResponse> VerifyCertificateAsync(CertificateVerifyRequest request)
    {
        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("验证证书");

                // 解析证书
                var certificate = request.Certificate.Contains("BEGIN")
                    ? Certificate.FromPem(request.Certificate)
                    : Certificate.FromDer(Convert.FromBase64String(request.Certificate));

                var checkDate = request.CheckDate ?? DateTime.UtcNow;
                var response = new CertificateVerifyResponse
                {
                    Details = (List<string>)[]
                };

                // 验证有效期
                response.DateValid = certificate.NotBefore <= checkDate && checkDate <= certificate.NotAfter;
                if (!response.DateValid)
                {
                    response.Details.Add($"证书不在有效期内：{certificate.NotBefore} - {certificate.NotAfter}");
                }

                // 验证签名
                if (!string.IsNullOrEmpty(request.IssuerCertificate))
                {
                    var issuerCert = request.IssuerCertificate.Contains("BEGIN")
                        ? Certificate.FromPem(request.IssuerCertificate)
                        : Certificate.FromDer(Convert.FromBase64String(request.IssuerCertificate));

                    response.SignatureValid = certificate.IsSignatureVerify(issuerCert);
                    response.ChainValid = response.SignatureValid;

                    if (!response.SignatureValid)
                    {
                        response.Details.Add("证书签名验证失败");
                    }
                }
                else
                {
                    // 自签名证书
                    response.SignatureValid = certificate.IsSignatureVerify(certificate);
                    response.ChainValid = response.SignatureValid;

                    if (!response.SignatureValid)
                    {
                        response.Details.Add("自签名证书签名验证失败");
                    }
                }

                response.IsValid = response.DateValid && response.SignatureValid && response.ChainValid;
                response.Message = response.IsValid ? "证书有效" : "证书无效";

                _logger.LogInformation("证书验证完成: {IsValid}", response.IsValid);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "验证证书失败");
                throw new InvalidOperationException($"验证证书失败: {ex.Message}", ex);
            }
        });
    }

    public async Task<CertificateResponse> ConvertCertificateAsync(CertificateConvertRequest request)
    {
        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("转换证书格式: {SourceFormat} -> {TargetFormat}",
                    request.SourceFormat, request.TargetFormat);

                // 解析证书
                var certificate = request.CertificateData.Contains("BEGIN")
                    ? Certificate.FromPem(request.CertificateData)
                    : Certificate.FromDer(Convert.FromBase64String(request.CertificateData));

                // 转换格式
                var certData = request.TargetFormat.ToUpperInvariant() == "DER"
                    ? Convert.ToBase64String(certificate.ToDer())
                    : certificate.ToPem();

                _logger.LogInformation("证书格式转换成功");

                return new CertificateResponse
                {
                    CertificateData = certData,
                    Format = request.TargetFormat.ToUpperInvariant()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "转换证书格式失败");
                throw new InvalidOperationException($"转换证书格式失败: {ex.Message}", ex);
            }
        });
    }

    // 辅助方法
    private string BuildDistinguishedName(SubjectInfo subject)
    {
        var parts = new List<string>();

        if (!string.IsNullOrEmpty(subject.CN)) parts.Add($"CN={subject.CN}");
        if (!string.IsNullOrEmpty(subject.O)) parts.Add($"O={subject.O}");
        if (!string.IsNullOrEmpty(subject.OU)) parts.Add($"OU={subject.OU}");
        if (!string.IsNullOrEmpty(subject.L)) parts.Add($"L={subject.L}");
        if (!string.IsNullOrEmpty(subject.ST)) parts.Add($"ST={subject.ST}");
        if (!string.IsNullOrEmpty(subject.C)) parts.Add($"C={subject.C}");
        if (!string.IsNullOrEmpty(subject.E)) parts.Add($"E={subject.E}");

        return string.Join(", ", parts);
    }

    private string GenerateSerialNumber()
    {
        var bytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        // 确保是正数
        bytes[0] &= 0x7F;
        return Convert.ToHexString(bytes);
    }
}
