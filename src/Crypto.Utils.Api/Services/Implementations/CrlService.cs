using Crypto.Utils.Crypto;
using Crypto.Utils.Mappers;
using Crypto.Utils.Models.Requests;
using Crypto.Utils.Models.Responses;
using Crypto.Utils.X509;
using Microsoft.Extensions.Logging;

namespace Crypto.Utils.Services.Implementations;

/// <summary>
/// CRL 服务实现
/// </summary>
public class CrlService : ICrlService
{
    private readonly ILogger<CrlService> _logger;

    public CrlService(ILogger<CrlService> logger)
    {
        _logger = logger;
    }

    public async Task<CrlResponse> GenerateCrlAsync(CrlGenerateRequest request)
    {
        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("生成 CRL");

                // 解析 CA 私钥
                var caPrivateKey = request.CaPrivateKey.Contains("BEGIN")
                    ? AsymmetricPrivateKeyParameter.FromPem(request.CaPrivateKey)
                    : AsymmetricPrivateKeyParameter.FromDer(Convert.FromBase64String(request.CaPrivateKey));

                // 构建颁发者 DN
                var issuerDN = this.BuildDistinguishedName(request.Issuer);

                // 准备吊销证书列表，转换吊销原因字符串为枚举
                var revokedCerts = request.RevokedCertificates?
                    .Select(rc => (
                        SerialNumber: rc.SerialNumber,
                        RevocationDate: rc.RevocationDate,
                        Reason: CrlMapper.ParseRevocationReason(rc.Reason)))
                    .ToList();

                // 生成 CRL
                var thisUpdate = request.ThisUpdate ?? DateTime.UtcNow;
                var nextUpdate = request.NextUpdate;

                var crl = CertificateRevocationList.Generate(
                    issuerDN,
                    caPrivateKey,
                    revokedCerts ?? [],
                    thisUpdate,
                    nextUpdate,
                    request.SignatureAlgorithm
                );

                // 导出 CRL
                var crlData = request.OutputFormat.ToUpperInvariant() == "DER"
                    ? Convert.ToBase64String(crl.ToDer())
                    : crl.ToPem();

                _logger.LogInformation("CRL 生成成功");

                return new CrlResponse
                {
                    CrlData = crlData,
                    Format = request.OutputFormat.ToUpperInvariant()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成 CRL 失败");
                throw new InvalidOperationException($"生成 CRL 失败: {ex.Message}", ex);
            }
        });
    }

    public async Task<CrlParseResponse> ParseCrlInfoAsync(CrlParseRequest request)
    {
        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("解析 CRL");

                // 解析 CRL
                var crl = request.CrlData.Contains("BEGIN")
                    ? CertificateRevocationList.FromPem(request.CrlData)
                    : CertificateRevocationList.FromDer(Convert.FromBase64String(request.CrlData));

                // 使用 Mapper 转换为响应对象
                var response = crl.ToCrlParseResponse();

                _logger.LogInformation("CRL 解析成功，包含 {Count} 个吊销证书", response.RevokedCertificates?.Count ?? 0);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "解析 CRL 失败");
                throw new InvalidOperationException($"解析 CRL 失败: {ex.Message}", ex);
            }
        });
    }

    public async Task<CrlCheckResponse> CheckRevocationAsync(CrlCheckRequest request)
    {
        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("检查证书吊销状态");

                // 解析证书
                var certificate = request.Certificate.Contains("BEGIN")
                    ? Certificate.FromPem(request.Certificate)
                    : Certificate.FromDer(Convert.FromBase64String(request.Certificate));

                // 解析 CRL
                var crl = request.CrlData.Contains("BEGIN")
                    ? CertificateRevocationList.FromPem(request.CrlData)
                    : CertificateRevocationList.FromDer(Convert.FromBase64String(request.CrlData));

                // 检查证书是否在 CRL 中
                var isRevoked = crl.IsRevoked(certificate.SerialNumber);
                var revokedCert = crl.RevokedCertificates?
                    .FirstOrDefault(rc => rc.SerialNumber.Equals(certificate.SerialNumber, StringComparison.OrdinalIgnoreCase));

                var response = new CrlCheckResponse
                {
                    IsRevoked = isRevoked,
                    RevocationDate = revokedCert?.RevocationDate,
                    Reason = revokedCert?.RevocationReason?.ToString(),
                    Message = isRevoked ? "证书已被吊销" : "证书未被吊销"
                };

                _logger.LogInformation("证书吊销状态检查完成: {IsRevoked}", isRevoked);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查证书吊销状态失败");
                throw new InvalidOperationException($"检查证书吊销状态失败: {ex.Message}", ex);
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
}
