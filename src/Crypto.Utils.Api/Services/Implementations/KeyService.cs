using Crypto.Utils.Crypto;
using Crypto.Utils.Mappers;
using Crypto.Utils.Models.Requests;
using Crypto.Utils.Models.Responses;
using Microsoft.Extensions.Logging;

namespace Crypto.Utils.Services.Implementations;

/// <summary>
/// 密钥对服务实现
/// </summary>
public class KeyService : IKeyService
{
    private readonly ILogger<KeyService> _logger;
    private readonly IFormatService _formatService;

    public KeyService(
        ILogger<KeyService> logger,
         IFormatService formatService)
    {
        _logger = logger;
        _formatService = formatService;
    }

    public Task<KeyPairResponse> GenerateKeyPairAsync(KeyPairGenerateRequest request)
    {
        try
        {
            _logger.LogInformation("开始生成密钥对: 算法={Algorithm}, 密钥大小={KeySize}", request.Algorithm, request.KeySize);

            AsymmetricKeyPair keyPair;

            // 根据算法类型生成密钥对
            switch (request.Algorithm.ToUpper())
            {
                case "RSA":
                    var rsaKeySize = request.KeySize ?? 2048;
                    if (rsaKeySize != 2048 && rsaKeySize != 3072 && rsaKeySize != 4096)
                    {
                        throw new ArgumentException("RSA 密钥大小必须是 2048、3072 或 4096 位");
                    }
                    keyPair = AsymmetricKeyPair.GenerateRsa(rsaKeySize);
                    break;

                case "EC":
                case "ECDSA":
                    var curveName = request.CurveName ?? "secp256r1";
                    keyPair = AsymmetricKeyPair.GenerateEc(curveName);
                    break;

                case "SM2":
                    // SM2 使用固定的 sm2p256v1 曲线
                    keyPair = AsymmetricKeyPair.GenerateEc("sm2p256v1");
                    break;

                case "DSA":
                    var dsaKeySize = request.KeySize ?? 2048;
                    if (dsaKeySize != 2048 && dsaKeySize != 3072)
                    {
                        throw new ArgumentException("DSA 密钥大小必须是 2048 或 3072 位");
                    }
                    keyPair = AsymmetricKeyPair.GenerateDsa(dsaKeySize);
                    break;

                default:
                    throw new ArgumentException($"不支持的算法类型: {request.Algorithm}");
            }

            // 根据输出格式导出密钥
            string privateKey;
            string publicKey;

            if (request.OutputFormat.ToUpper() == "DER")
            {
                privateKey = Convert.ToBase64String(keyPair.PrivateKey.ToDer());
                publicKey = Convert.ToBase64String(keyPair.PublicKey.ToDer());
            }
            else // 默认 PEM
            {
                privateKey = keyPair.PrivateKey.ToPem();
                publicKey = keyPair.PublicKey.ToPem();
            }

            var response = new KeyPairResponse
            {
                PrivateKey = privateKey,
                PublicKey = publicKey,
                Algorithm = keyPair.Algorithm,
                KeySize = keyPair.KeySize,
                CurveName = keyPair.PrivateKey.CurveName
            };

            _logger.LogInformation("密钥对生成成功: 算法={Algorithm}, 大小={KeySize}", response.Algorithm, response.KeySize);

            return Task.FromResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成密钥对失败: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<KeyConvertResponse> ConvertFormatAsync(KeyFormatConvertRequest request)
    {
        _logger.LogInformation("密钥格式转换委托给 FormatService");
        return await _formatService.ConvertKeyFormatAsync(request);
    }

    public Task<KeyInfoResponse> ParseKeyInfoAsync(KeyParseRequest request)
    {
        try
        {
            _logger.LogInformation("开始解析密钥信息");

            AsymmetricKeyParameter key;

            // 尝试解析密钥（自动识别 PEM/DER 格式）
            try
            {
                // 先尝试作为 PEM 格式解析
                if (request.KeyData.Contains("BEGIN"))
                {
                    if (request.KeyData.Contains("PRIVATE KEY"))
                    {
                        key = AsymmetricPrivateKeyParameter.FromPem(request.KeyData);
                    }
                    else if (request.KeyData.Contains("PUBLIC KEY"))
                    {
                        key = AsymmetricPublicKeyParameter.FromPem(request.KeyData);
                    }
                    else
                    {
                        throw new ArgumentException("无法解析密钥数据，既不是有效的 PEM 格式，也不是有效的 DER 格式");
                    }
                }
                else
                {
                    // 尝试作为 Base64 编码的 DER 格式解析
                    var derBytes = Convert.FromBase64String(request.KeyData);

                    // 先尝试解析为私钥
                    try
                    {
                        key = AsymmetricPrivateKeyParameter.FromDer(derBytes);
                    }
                    catch (Exception privateKeyEx)
                    {
                        try
                        {
                            // 如果失败，尝试解析为公钥
                            key = AsymmetricPublicKeyParameter.FromDer(derBytes);
                        }
                        catch (Exception publicKeyEx)
                        {
                            throw new Exception("无法解析密钥数据，既不是有效的私钥，也不是有效的公钥",
                                new AggregateException(privateKeyEx, publicKeyEx));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"无法解析密钥数据: {ex.Message}", ex);
            }

            // 使用扩展方法转换密钥信息
            var response = key.ToKeyInfoResponse();

            _logger.LogInformation("密钥信息解析成功: 算法={Algorithm}, 类型={Type}",
                response.AlgorithmName,
                response.IsPrivate ? "私钥" : "公钥");

            return Task.FromResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "解析密钥信息失败: {Message}", ex.Message);
            throw;
        }
    }

    public Task<KeyConvertResponse> ConvertPkcsFormatAsync(PkcsConvertRequest request)
    {
        throw new NotImplementedException("PKCS 格式转换功能待实现");
    }

    public Task<KeyConvertResponse> EncryptPrivateKeyAsync(KeyEncryptRequest request)
    {
        throw new NotImplementedException("私钥加密功能待实现");
    }

    public Task<KeyConvertResponse> DecryptPrivateKeyAsync(KeyDecryptRequest request)
    {
        throw new NotImplementedException("私钥解密功能待实现");
    }
}
