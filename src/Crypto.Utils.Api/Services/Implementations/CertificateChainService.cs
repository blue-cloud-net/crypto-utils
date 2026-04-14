using Crypto.Utils.Models.Requests;
using Crypto.Utils.Models.Responses;
using Crypto.Utils.X509;
using Microsoft.Extensions.Logging;

namespace Crypto.Utils.Services.Implementations;

/// <summary>
/// 证书链服务实现
/// </summary>
public class CertificateChainService : ICertificateChainService
{
    private readonly ILogger<CertificateChainService> _logger;

    public CertificateChainService(ILogger<CertificateChainService> logger)
    {
        _logger = logger;
    }

    public async Task<ChainBuildResponse> BuildChainAsync(ChainBuildRequest request)
    {
        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("构建证书链");

                // 加载目标证书
                var targetCert = request.Certificate.Contains("BEGIN")
                    ? Certificate.FromPem(request.Certificate)
                    : Certificate.FromDer(Convert.FromBase64String(request.Certificate));

                List<Certificate> chain = [targetCert];
                List<string> chainStrings = [targetCert.ToPem()];

                // 加载中间证书
                var intermediateCerts = new List<Certificate>();
                if (request.IntermediateCertificates != null)
                {
                    foreach (var certData in request.IntermediateCertificates)
                    {
                        var cert = certData.Contains("BEGIN")
                            ? Certificate.FromPem(certData)
                            : Certificate.FromDer(Convert.FromBase64String(certData));
                        intermediateCerts.Add(cert);
                    }
                }

                // 加载根证书
                var rootCerts = new List<Certificate>();
                if (request.RootCertificates != null)
                {
                    foreach (var certData in request.RootCertificates)
                    {
                        var cert = certData.Contains("BEGIN")
                            ? Certificate.FromPem(certData)
                            : Certificate.FromDer(Convert.FromBase64String(certData));
                        rootCerts.Add(cert);
                    }
                }

                // 构建证书链
                var currentCert = targetCert;
                var isComplete = false;

                while (!isComplete)
                {
                    // 检查是否是自签名证书（根证书）
                    if (currentCert.Subject == currentCert.Issuer)
                    {
                        isComplete = true;
                        break;
                    }

                    // 在中间证书中查找颁发者
                    var issuer = intermediateCerts.FirstOrDefault(c => c.Subject == currentCert.Issuer);

                    if (issuer == null)
                    {
                        // 在根证书中查找颁发者
                        issuer = rootCerts.FirstOrDefault(c => c.Subject == currentCert.Issuer);
                    }

                    if (issuer != null)
                    {
                        chain.Add(issuer);
                        chainStrings.Add(issuer.ToPem());
                        currentCert = issuer;

                        // 检查是否到达根证书
                        if (currentCert.Subject == currentCert.Issuer)
                        {
                            isComplete = true;
                        }
                    }
                    else
                    {
                        // 找不到颁发者，链不完整
                        break;
                    }
                }

                var response = new ChainBuildResponse
                {
                    CertificateChain = chainStrings,
                    ChainLength = chain.Count,
                    IsComplete = isComplete,
                    Message = isComplete ? "证书链构建完成" : $"证书链不完整，已构建 {chain.Count} 级"
                };

                _logger.LogInformation("证书链构建完成, 长度={Length}, 完整={IsComplete}",
                    response.ChainLength, response.IsComplete);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "构建证书链失败");
                throw new InvalidOperationException($"构建证书链失败: {ex.Message}", ex);
            }
        });
    }

    public async Task<ChainVerifyResponse> VerifyChainAsync(ChainVerifyRequest request)
    {
        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("验证证书链");

                if (request.CertificateChain == null || request.CertificateChain.Count == 0)
                {
                    throw new ArgumentException("证书链不能为空");
                }

                // 加载证书链
                var chain = new List<Certificate>();
                foreach (var certData in request.CertificateChain)
                {
                    var cert = certData.Contains("BEGIN")
                        ? Certificate.FromPem(certData)
                        : Certificate.FromDer(Convert.FromBase64String(certData));
                    chain.Add(cert);
                }

                // 加载信任的根证书
                var trustedRoots = new List<Certificate>();
                if (request.TrustedRoots != null)
                {
                    foreach (var certData in request.TrustedRoots)
                    {
                        var cert = certData.Contains("BEGIN")
                            ? Certificate.FromPem(certData)
                            : Certificate.FromDer(Convert.FromBase64String(certData));
                        trustedRoots.Add(cert);
                    }
                }

                var checkDate = request.CheckDate ?? DateTime.UtcNow;
                var details = new List<CertificateValidationDetail>();
                var isValid = true;

                // 验证每个证书
                for (var i = 0; i < chain.Count; i++)
                {
                    var cert = chain[i];
                    var detail = new CertificateValidationDetail
                    {
                        Subject = cert.Subject,
                        Issuer = cert.Issuer
                    };

                    // 检查有效期
                    if (cert.NotBefore > checkDate || cert.NotAfter < checkDate)
                    {
                        detail.IsValid = false;
                        detail.Message = $"证书不在有效期内（{cert.NotBefore} - {cert.NotAfter}）";
                        isValid = false;
                    }
                    // 检查签名
                    else if (i < chain.Count - 1)
                    {
                        // 使用下一级证书验证签名
                        var issuerCert = chain[i + 1];
                        if (cert.Issuer != issuerCert.Subject)
                        {
                            detail.IsValid = false;
                            detail.Message = "证书链不连续";
                            isValid = false;
                        }
                        else if (!cert.IsSignatureVerify(issuerCert))
                        {
                            detail.IsValid = false;
                            detail.Message = "签名验证失败";
                            isValid = false;
                        }
                        else
                        {
                            detail.IsValid = true;
                            detail.Message = "验证通过";
                        }
                    }
                    else
                    {
                        // 最后一个证书，应该是根证书
                        if (cert.Subject == cert.Issuer)
                        {
                            // 自签名证书，验证自身签名
                            if (!cert.IsSignatureVerify(cert))
                            {
                                detail.IsValid = false;
                                detail.Message = "根证书自签名验证失败";
                                isValid = false;
                            }
                            else
                            {
                                // 检查是否在信任的根证书列表中
                                if (trustedRoots.Count > 0)
                                {
                                    var isTrusted = trustedRoots.Any(r => r.Subject == cert.Subject && r.SerialNumber == cert.SerialNumber);
                                    if (!isTrusted)
                                    {
                                        detail.IsValid = false;
                                        detail.Message = "根证书不在信任列表中";
                                        isValid = false;
                                    }
                                    else
                                    {
                                        detail.IsValid = true;
                                        detail.Message = "根证书验证通过";
                                    }
                                }
                                else
                                {
                                    detail.IsValid = true;
                                    detail.Message = "根证书自签名验证通过（未检查信任列表）";
                                }
                            }
                        }
                        else
                        {
                            detail.IsValid = false;
                            detail.Message = "证书链不完整，缺少根证书";
                            isValid = false;
                        }
                    }

                    details.Add(detail);
                }

                var response = new ChainVerifyResponse
                {
                    IsValid = isValid,
                    Message = isValid ? "证书链验证通过" : "证书链验证失败",
                    CertificateDetails = details
                };

                _logger.LogInformation("证书链验证完成: {IsValid}", isValid);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "验证证书链失败");
                throw new InvalidOperationException($"验证证书链失败: {ex.Message}", ex);
            }
        });
    }
}
