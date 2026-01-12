using Crypto.Utils.Crypto;
using Crypto.Utils.Models.Responses;
using Crypto.Utils.X509.Extensions;

namespace Crypto.Utils.Mappers;

/// <summary>
/// 密钥参数扩展方法
/// </summary>
public static class KeyParameterMapper
{
    /// <summary>
    /// 将 AsymmetricKeyParameter 转换为 KeyInfoResponse
    /// </summary>
    /// <param name="key">密钥参数</param>
    /// <returns>密钥信息响应</returns>
    public static KeyInfoResponse ToKeyInfoResponse(this AsymmetricKeyParameter key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        var response = new KeyInfoResponse
        {
            AlgorithmName = key.AlgorithmName,
            IsPrivate = key.IsPrivate,
            IsPublic = key.IsPublic,
            KeySize = key.KeySize,
            CurveOid = key.CurveOid?.Id,
            CurveName = key.CurveName,
            KeyData = key.ToPem(),
            Parameters = key.Parameters
        };

        // 计算指纹
        var algorithms = new[] { "sha1", "sha256" };
        if (response.AlgorithmName == "SM2")
        {
            // SM2算法的密钥 支持使用 SM3 指纹
            algorithms = new[] { "sm3", "sha1", "sha256" };
        }

        response.Fingerprints = new Dictionary<string, string>();
        foreach (var algo in algorithms)
        {
            response.Fingerprints[algo] = FingerprintHelper.ComputeFingerprint(key.ToDer(), algo);
        }

        return response;
    }
}