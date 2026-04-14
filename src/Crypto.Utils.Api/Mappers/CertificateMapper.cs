using Crypto.Utils.Models.Responses;
using Crypto.Utils.X509;
using Crypto.Utils.X509.Enums;
using Crypto.Utils.X509.Extensions;

namespace Crypto.Utils.Mappers;

/// <summary>
/// 证书映射扩展方法
/// </summary>
public static class CertificateMapper
{
    /// <summary>
    /// 将 Certificate 转换为 CertificateParseResponse
    /// </summary>
    /// <param name="certificate">证书对象</param>
    /// <returns>证书解析响应</returns>
    public static CertificateParseResponse ToCertificateParseResponse(this Certificate certificate)
    {
        if (certificate == null)
            throw new ArgumentNullException(nameof(certificate));

        var response = new CertificateParseResponse
        {
            Version = certificate.Version,
            SerialNumber = certificate.SerialNumber,
            Subject = certificate.Subject,
            Issuer = certificate.Issuer,
            NotBefore = certificate.NotBefore,
            NotAfter = certificate.NotAfter,
            Duration = certificate.ValidityPeriod.TotalDays,
            IsValid = certificate.IsValidNow,
            RemainingDays = Math.Round(certificate.TimeUntilExpiry.TotalDays, 1),
            SignatureAlgorithmOid = certificate.SignatureAlgorithmOid.ToString(),
            SignatureAlgorithmName = certificate.SignatureAlgorithmName,
            IsCA = certificate.IsCertificateAuthority,
            PathLengthConstraint = certificate.PathLengthConstraint,
            SubjectKeyIdentifier = certificate.SubjectKeyIdentifier,
            AuthorityKeyIdentifier = certificate.AuthorityKeyIdentifier,
            KeyUsage = certificate.KeyUsages.GetFriendlyNames(),
            ExtendedKeyUsage = certificate.ExtendedKeyUsages.GetFriendlyNames(),
            SubjectAlternativeNames = certificate.SubjectAlternativeNames?.Select(san => san.ToString()),
            CrlDistributionPoints = certificate.CrlDistributionPointUrls,
            CertificatePolicies = certificate.CertificatePolicyOids?.Select(p => p.Id),
            Extensions = certificate.Extensions.ToDictionary(
                ext => ext.Name,
                ext => (object)ext.ValueHex
            )
        };

        // 添加 Authority Information Access
        if (certificate.AuthorityInformationAccessOscp is not null
            || certificate.AuthorityInformationAccessCaIssuers is not null)
        {
            response.AuthorityInformationAccess = new AuthorityInformationAccessResponse
            {
                OcspUrls = certificate.AuthorityInformationAccessOscp ?? [],
                CaIssuers = certificate.AuthorityInformationAccessCaIssuers ?? []
            };
        }

        // 获取公钥
        var publicKey = certificate.GetPublicKey();
        response.PublicKey = publicKey.ToKeyInfoResponse();

        // 计算证书指纹
        string[] algorithms = ["sha1", "sha256"];
        if (response.PublicKey.AlgorithmName == "SM2")
        {
            // SM2算法的证书 支持使用 SM3 指纹
            algorithms = ["sm3", "sha1", "sha256"];
        }

        foreach (var algo in algorithms)
        {
            response.Fingerprints[algo] = certificate.ComputeFingerprint(algo);
        }

        return response;
    }
}