using Crypto.Utils.Models.Requests;
using Crypto.Utils.Models.Responses;

namespace Crypto.Utils.Services;

/// <summary>
/// 密钥对服务接口
/// </summary>
public interface IKeyService
{
    /// <summary>
    /// 生成密钥对
    /// </summary>
    Task<KeyPairResponse> GenerateKeyPairAsync(KeyPairGenerateRequest request);

    /// <summary>
    /// 密钥格式转换 (PEM/DER)
    /// </summary>
    Task<KeyConvertResponse> ConvertFormatAsync(KeyFormatConvertRequest request);

    /// <summary>
    /// 解析密钥信息
    /// </summary>
    Task<KeyInfoResponse> ParseKeyInfoAsync(KeyParseRequest request);

    /// <summary>
    /// PKCS 格式转换 (PKCS1/PKCS8)
    /// </summary>
    Task<KeyConvertResponse> ConvertPkcsFormatAsync(PkcsConvertRequest request);

    /// <summary>
    /// 加密私钥
    /// </summary>
    Task<KeyConvertResponse> EncryptPrivateKeyAsync(KeyEncryptRequest request);

    /// <summary>
    /// 解密私钥
    /// </summary>
    Task<KeyConvertResponse> DecryptPrivateKeyAsync(KeyDecryptRequest request);
}
