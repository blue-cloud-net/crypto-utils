using Crypto.Utils.Crypto;
using Crypto.Utils.Mappers;
using Crypto.Utils.Models.Requests;
using Crypto.Utils.Models.Responses;
using Crypto.Utils.X509;
using Microsoft.Extensions.Logging;

namespace Crypto.Utils.Services.Implementations;

/// <summary>
/// CSR 服务实现
/// </summary>
public class CsrService : ICsrService
{
    private readonly ILogger<CsrService> _logger;

    public CsrService(
        ILogger<CsrService> logger)
    {
        _logger = logger;
    }

    public Task<CsrResponse> GenerateCsrAsync(CsrGenerateRequest request)
    {
        try
        {
            _logger.LogInformation("生成 CSR: Subject={Subject}", request.Subject.CN);

            // 加载私钥
            var privateKey = request.PrivateKey.Contains("BEGIN")
                ? AsymmetricPrivateKeyParameter.FromPem(request.PrivateKey)
                : AsymmetricPrivateKeyParameter.FromDer(Convert.FromBase64String(request.PrivateKey));

            // 构建 Subject DN
            var subjectBuilder = new System.Text.StringBuilder();
            if (!string.IsNullOrEmpty(request.Subject.CN))
                subjectBuilder.Append($"CN={request.Subject.CN}");
            if (!string.IsNullOrEmpty(request.Subject.O))
                subjectBuilder.Append($", O={request.Subject.O}");
            if (!string.IsNullOrEmpty(request.Subject.OU))
                subjectBuilder.Append($", OU={request.Subject.OU}");
            if (!string.IsNullOrEmpty(request.Subject.C))
                subjectBuilder.Append($", C={request.Subject.C}");
            if (!string.IsNullOrEmpty(request.Subject.ST))
                subjectBuilder.Append($", ST={request.Subject.ST}");
            if (!string.IsNullOrEmpty(request.Subject.L))
                subjectBuilder.Append($", L={request.Subject.L}");
            if (!string.IsNullOrEmpty(request.Subject.E))
                subjectBuilder.Append($", E={request.Subject.E}");

            var subjectDN = subjectBuilder.ToString();

            // 从私钥派生公钥
            var bcPrivateKey = privateKey.GetBouncyCastleKey();
            Org.BouncyCastle.Crypto.AsymmetricKeyParameter bcPublicKey;

            if (bcPrivateKey is Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters rsaPrivate)
            {
                bcPublicKey = new Org.BouncyCastle.Crypto.Parameters.RsaKeyParameters(false, rsaPrivate.Modulus, rsaPrivate.PublicExponent);
            }
            else if (bcPrivateKey is Org.BouncyCastle.Crypto.Parameters.ECPrivateKeyParameters ecPrivate)
            {
                var q = ecPrivate.Parameters.G.Multiply(ecPrivate.D);
                bcPublicKey = new Org.BouncyCastle.Crypto.Parameters.ECPublicKeyParameters(q, ecPrivate.Parameters);
            }
            else
            {
                throw new NotSupportedException($"不支持的私钥类型: {bcPrivateKey.GetType().Name}");
            }

            var publicKey = new AsymmetricPublicKeyParameter(bcPublicKey);

            // 生成 CSR
            var csr = CertificateSigningRequest.Generate(
                subjectDN,
                publicKey,
                privateKey,
                request.SignatureAlgorithm
            );

            var csrData = request.OutputFormat.ToUpperInvariant() == "DER"
                ? Convert.ToBase64String(csr.ToDer())
                : csr.ToPem();

            return Task.FromResult(new CsrResponse
            {
                CsrData = csrData,
                Format = request.OutputFormat
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成 CSR 失败");
            throw;
        }
    }

    public Task<CsrParseResponse> ParseCsrInfoAsync(CsrParseRequest request)
    {
        try
        {
            _logger.LogInformation("解析 CSR");

            var csr = request.CsrData.Contains("BEGIN")
                ? CertificateSigningRequest.FromPem(request.CsrData)
                : CertificateSigningRequest.FromDer(Convert.FromBase64String(request.CsrData));

            var response = new CsrParseResponse
            {
                Subject = csr.Subject,
                SignatureAlgorithmName = csr.SignatureAlgorithmName,
                PublicKey = csr.GetPublicKey().ToKeyInfoResponse(),
                Extensions = new Dictionary<string, object>()
            };

            return Task.FromResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "解析 CSR 失败");
            throw;
        }
    }

    public Task<CsrVerifyResponse> VerifyCsrAsync(CsrVerifyRequest request)
    {
        try
        {
            _logger.LogInformation("验证 CSR");

            var csr = request.CsrData.Contains("BEGIN")
                ? CertificateSigningRequest.FromPem(request.CsrData)
                : CertificateSigningRequest.FromDer(Convert.FromBase64String(request.CsrData));

            var isValid = csr.Verify();

            return Task.FromResult(new CsrVerifyResponse
            {
                IsValid = isValid,
                Message = isValid ? "CSR 签名验证成功" : "CSR 签名验证失败"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证 CSR 失败");
            return Task.FromResult(new CsrVerifyResponse
            {
                IsValid = false,
                Message = $"验证失败: {ex.Message}"
            });
        }
    }
}
