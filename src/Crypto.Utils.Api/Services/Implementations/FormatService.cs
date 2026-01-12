using Crypto.Utils.Crypto;
using Crypto.Utils.Models.Requests;
using Crypto.Utils.Models.Responses;
using Crypto.Utils.X509;
using Microsoft.Extensions.Logging;

namespace Crypto.Utils.Services.Implementations;

/// <summary>
/// 格式转换服务实现
/// 支持 KEY、CSR、CERT、CRL 的 PEM 和 DER 格式互转
/// </summary>
public class FormatService : IFormatService
{
    private readonly ILogger<FormatService> _logger;

    public FormatService(
        ILogger<FormatService> logger)
    {
        _logger = logger;
    }

    public async Task<KeyConvertResponse> ConvertKeyFormatAsync(KeyFormatConvertRequest request)
    {
        return await Task.Run(() =>
        {
            try
            {
                var sourceFormat = request.SourceFormat.ToUpperInvariant();
                var targetFormat = request.TargetFormat.ToUpperInvariant();

                _logger.LogInformation("转换密钥格式: {SourceFormat} -> {TargetFormat}", sourceFormat, targetFormat);

                if (sourceFormat == targetFormat)
                {
                    return new KeyConvertResponse
                    {
                        ConvertedKey = request.KeyData,
                        Format = targetFormat
                    };
                }

                byte[] keyBytes;
                string convertedKey;

                if (sourceFormat == "PEM" && targetFormat == "DER")
                {
                    // PEM -> DER
                    // 尝试解析为私钥或公钥
                    try
                    {
                        var privateKey = AsymmetricPrivateKeyParameter.FromPem(request.KeyData);
                        keyBytes = privateKey.ToDer();
                    }
                    catch
                    {
                        // 如果不是私钥，尝试作为公钥
                        var publicKey = AsymmetricPublicKeyParameter.FromPem(request.KeyData);
                        keyBytes = publicKey.ToDer();
                    }
                    convertedKey = Convert.ToBase64String(keyBytes);
                }
                else if (sourceFormat == "DER" && targetFormat == "PEM")
                {
                    // DER -> PEM
                    keyBytes = Convert.FromBase64String(request.KeyData);
                    
                    try
                    {
                        var privateKey = AsymmetricPrivateKeyParameter.FromDer(keyBytes);
                        convertedKey = privateKey.ToPem();
                    }
                    catch
                    {
                        var publicKey = AsymmetricPublicKeyParameter.FromDer(keyBytes);
                        convertedKey = publicKey.ToPem();
                    }
                }
                else
                {
                    throw new ArgumentException($"不支持的格式转换: {sourceFormat} -> {targetFormat}");
                }

                return new KeyConvertResponse
                {
                    ConvertedKey = convertedKey,
                    Format = targetFormat
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "密钥格式转换失败");
                throw new InvalidOperationException($"密钥格式转换失败: {ex.Message}", ex);
            }
        });
    }

    public async Task<FormatConvertResponse> ConvertCsrFormatAsync(FormatConvertRequest request)
    {
        return await Task.Run(() =>
        {
            try
            {
                var sourceFormat = request.SourceFormat.ToUpperInvariant();
                var targetFormat = request.TargetFormat.ToUpperInvariant();

                _logger.LogInformation("转换 CSR 格式: {SourceFormat} -> {TargetFormat}", sourceFormat, targetFormat);

                if (sourceFormat == targetFormat)
                {
                    return new FormatConvertResponse
                    {
                        ConvertedData = request.Data,
                        Format = targetFormat,
                        DataType = "CSR"
                    };
                }

                string convertedData;

                if (sourceFormat == "PEM" && targetFormat == "DER")
                {
                    // PEM -> DER
                    var csr = CertificateSigningRequest.FromPem(request.Data);
                    var derBytes = csr.ToDer();
                    convertedData = Convert.ToBase64String(derBytes);
                }
                else if (sourceFormat == "DER" && targetFormat == "PEM")
                {
                    // DER -> PEM
                    var derBytes = Convert.FromBase64String(request.Data);
                    var csr = CertificateSigningRequest.FromDer(derBytes);
                    convertedData = csr.ToPem();
                }
                else
                {
                    throw new ArgumentException($"不支持的格式转换: {sourceFormat} -> {targetFormat}");
                }

                return new FormatConvertResponse
                {
                    ConvertedData = convertedData,
                    Format = targetFormat,
                    DataType = "CSR"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CSR 格式转换失败");
                throw new InvalidOperationException($"CSR 格式转换失败: {ex.Message}", ex);
            }
        });
    }

    public async Task<FormatConvertResponse> ConvertCertificateFormatAsync(FormatConvertRequest request)
    {
        return await Task.Run(() =>
        {
            try
            {
                var sourceFormat = request.SourceFormat.ToUpperInvariant();
                var targetFormat = request.TargetFormat.ToUpperInvariant();

                _logger.LogInformation("转换证书格式: {SourceFormat} -> {TargetFormat}", sourceFormat, targetFormat);

                if (sourceFormat == targetFormat)
                {
                    return new FormatConvertResponse
                    {
                        ConvertedData = request.Data,
                        Format = targetFormat,
                        DataType = "CERT"
                    };
                }

                string convertedData;

                if (sourceFormat == "PEM" && targetFormat == "DER")
                {
                    // PEM -> DER
                    var cert = Certificate.FromPem(request.Data);
                    var derBytes = cert.ToDer();
                    convertedData = Convert.ToBase64String(derBytes);
                }
                else if (sourceFormat == "DER" && targetFormat == "PEM")
                {
                    // DER -> PEM
                    var derBytes = Convert.FromBase64String(request.Data);
                    var cert = Certificate.FromDer(derBytes);
                    convertedData = cert.ToPem();
                }
                else
                {
                    throw new ArgumentException($"不支持的格式转换: {sourceFormat} -> {targetFormat}");
                }

                return new FormatConvertResponse
                {
                    ConvertedData = convertedData,
                    Format = targetFormat,
                    DataType = "CERT"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "证书格式转换失败");
                throw new InvalidOperationException($"证书格式转换失败: {ex.Message}", ex);
            }
        });
    }

    public async Task<FormatConvertResponse> ConvertCrlFormatAsync(FormatConvertRequest request)
    {
        return await Task.Run(() =>
        {
            try
            {
                var sourceFormat = request.SourceFormat.ToUpperInvariant();
                var targetFormat = request.TargetFormat.ToUpperInvariant();

                _logger.LogInformation("转换 CRL 格式: {SourceFormat} -> {TargetFormat}", sourceFormat, targetFormat);

                if (sourceFormat == targetFormat)
                {
                    return new FormatConvertResponse
                    {
                        ConvertedData = request.Data,
                        Format = targetFormat,
                        DataType = "CRL"
                    };
                }

                string convertedData;

                if (sourceFormat == "PEM" && targetFormat == "DER")
                {
                    // PEM -> DER
                    var crl = CertificateRevocationList.FromPem(request.Data);
                    var derBytes = crl.ToDer();
                    convertedData = Convert.ToBase64String(derBytes);
                }
                else if (sourceFormat == "DER" && targetFormat == "PEM")
                {
                    // DER -> PEM
                    var derBytes = Convert.FromBase64String(request.Data);
                    var crl = CertificateRevocationList.FromDer(derBytes);
                    convertedData = crl.ToPem();
                }
                else
                {
                    throw new ArgumentException($"不支持的格式转换: {sourceFormat} -> {targetFormat}");
                }

                return new FormatConvertResponse
                {
                    ConvertedData = convertedData,
                    Format = targetFormat,
                    DataType = "CRL"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CRL 格式转换失败");
                throw new InvalidOperationException($"CRL 格式转换失败: {ex.Message}", ex);
            }
        });
    }

    public async Task<FormatConvertResponse> ConvertFormatAsync(FormatConvertRequest request)
    {
        return await Task.Run(() =>
        {
            try
            {
                var dataType = request.DataType?.ToUpperInvariant();

                // 如果指定了数据类型，直接调用对应的转换方法
                if (!string.IsNullOrEmpty(dataType))
                {
                    return dataType switch
                    {
                        "KEY" => this.ConvertKeyFormatToGeneric(request),
                        "CSR" => this.ConvertCsrFormatAsync(request).Result,
                        "CERT" or "CERTIFICATE" => this.ConvertCertificateFormatAsync(request).Result,
                        "CRL" => this.ConvertCrlFormatAsync(request).Result,
                        _ => throw new ArgumentException($"不支持的数据类型: {dataType}")
                    };
                }

                // 自动检测数据类型
                _logger.LogInformation("自动检测数据类型并转换格式");
                
                var detectedType = this.DetectDataType(request.Data, request.SourceFormat);
                _logger.LogInformation("检测到数据类型: {DataType}", detectedType);

                return detectedType switch
                {
                    "KEY" => this.ConvertKeyFormatToGeneric(request),
                    "CSR" => this.ConvertCsrFormatAsync(request).Result,
                    "CERT" => this.ConvertCertificateFormatAsync(request).Result,
                    "CRL" => this.ConvertCrlFormatAsync(request).Result,
                    _ => throw new InvalidOperationException("无法识别数据类型")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "格式转换失败");
                throw new InvalidOperationException($"格式转换失败: {ex.Message}", ex);
            }
        });
    }

    /// <summary>
    /// 将密钥转换结果转换为通用格式响应
    /// </summary>
    private FormatConvertResponse ConvertKeyFormatToGeneric(FormatConvertRequest request)
    {
        var keyRequest = new KeyFormatConvertRequest
        {
            KeyData = request.Data,
            SourceFormat = request.SourceFormat,
            TargetFormat = request.TargetFormat
        };

        var keyResult = this.ConvertKeyFormatAsync(keyRequest).Result;

        return new FormatConvertResponse
        {
            ConvertedData = keyResult.ConvertedKey,
            Format = keyResult.Format,
            DataType = "KEY"
        };
    }

    /// <summary>
    /// 检测数据类型
    /// </summary>
    private string DetectDataType(string data, string format)
    {
        format = format.ToUpperInvariant();

        try
        {
            if (format == "PEM")
            {
                // 根据 PEM 标记检测类型
                if (data.Contains("BEGIN CERTIFICATE REQUEST") || data.Contains("BEGIN NEW CERTIFICATE REQUEST"))
                    return "CSR";
                if (data.Contains("BEGIN CERTIFICATE"))
                    return "CERT";
                if (data.Contains("BEGIN X509 CRL"))
                    return "CRL";
                if (data.Contains("BEGIN PRIVATE KEY") || data.Contains("BEGIN RSA PRIVATE KEY") || 
                    data.Contains("BEGIN EC PRIVATE KEY") || data.Contains("BEGIN PUBLIC KEY") ||
                    data.Contains("BEGIN RSA PUBLIC KEY"))
                    return "KEY";
            }
            else if (format == "DER")
            {
                // DER 格式需要尝试解析
                var derBytes = Convert.FromBase64String(data);

                // 尝试作为证书
                try
                {
                    Certificate.FromDer(derBytes);
                    return "CERT";
                }
                catch { }

                // 尝试作为 CSR
                try
                {
                    CertificateSigningRequest.FromDer(derBytes);
                    return "CSR";
                }
                catch { }

                // 尝试作为 CRL
                try
                {
                    CertificateRevocationList.FromDer(derBytes);
                    return "CRL";
                }
                catch { }

                // 尝试作为密钥
                try
                {
                    AsymmetricPrivateKeyParameter.FromDer(derBytes);
                    return "KEY";
                }
                catch
                {
                    try
                    {
                        AsymmetricPublicKeyParameter.FromDer(derBytes);
                        return "KEY";
                    }
                    catch { }
                }
            }

            throw new InvalidOperationException("无法识别数据类型");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "数据类型检测失败");
            throw new InvalidOperationException("无法识别数据类型", ex);
        }
    }
}
