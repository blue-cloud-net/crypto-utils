
using Crypto.Utils.Models.Responses;
using Crypto.Utils.X509;
using Crypto.Utils.X509.Enums;
using Crypto.Utils.X509.Models;
using System.Diagnostics.CodeAnalysis;

namespace Crypto.Utils.Mappers;

/// <summary>
/// CRL 映射扩展方法
/// </summary>
public static class CrlMapper
{
    /// <summary>
    /// 将 CertificateRevocationList 转换为 CrlParseResponse
    /// </summary>
    /// <param name="crl">CRL 对象</param>
    /// <returns>CRL 解析响应</returns>
    public static CrlParseResponse ToCrlParseResponse(this CertificateRevocationList crl)
    {
        if (crl == null)
            throw new ArgumentNullException(nameof(crl));

        return new CrlParseResponse
        {
            Issuer = crl.Issuer,
            ThisUpdate = crl.ThisUpdate,
            NextUpdate = crl.NextUpdate ?? DateTime.MinValue,
            SignatureAlgorithmName = crl.SignatureAlgorithmName,
            RevokedCertificates = crl.RevokedCertificates
                ?.Select(info => info.ToRevokedCertificateDetail())
                .ToList()
        };
    }

    /// <summary>
    /// 将吊销证书信息转换为 RevokedCertificateDetail
    /// </summary>
    /// <param name="revokedCert">吊销证书信息</param>
    /// <returns>吊销证书详情</returns>
    public static RevokedCertificateDetail ToRevokedCertificateDetail(this RevokedCertificateInfo revokedCert)
    {
        return new RevokedCertificateDetail
        {
            SerialNumber = revokedCert.SerialNumber,
            RevocationDate = revokedCert.RevocationDate,
            Reason = revokedCert.RevocationReason
        };
    }

    /// <summary>
    /// 将字符串转换为 CertificateRevocationReason 枚举
    /// 支持枚举名称、枚举值（数字）和友好名称
    /// </summary>
    /// <param name="reasonString">原因字符串</param>
    /// <returns>吊销原因枚举，如果无法解析则返回 null</returns>
    public static CertificateRevocationReason? ParseRevocationReason(string? reasonString)
    {
        if (string.IsNullOrWhiteSpace(reasonString))
            return null;

        reasonString = reasonString.Trim();

        // 1. 尝试按枚举值（数字）解析
        if (int.TryParse(reasonString, out var reasonValue) &&
            Enum.IsDefined(typeof(CertificateRevocationReason), reasonValue))
        {
            return (CertificateRevocationReason)reasonValue;
        }

        // 2. 尝试按枚举名称解析（不区分大小写）
        if (Enum.TryParse<CertificateRevocationReason>(reasonString, ignoreCase: true, out var parsedReason))
        {
            return parsedReason;
        }

        return null;
    }

    /// <summary>
    /// 将字符串转换为 CertificateRevocationReason 枚举（带默认值）
    /// </summary>
    /// <param name="reasonString">原因字符串</param>
    /// <param name="defaultReason">无法解析时的默认值</param>
    /// <returns>吊销原因枚举</returns>
    public static CertificateRevocationReason ParseRevocationReasonOrDefault(
        string? reasonString,
        CertificateRevocationReason defaultReason = CertificateRevocationReason.Unspecified)
    {
        return ParseRevocationReason(reasonString) ?? defaultReason;
    }

    /// <summary>
    /// 尝试将字符串转换为 CertificateRevocationReason 枚举
    /// </summary>
    /// <param name="reasonString">原因字符串</param>
    /// <param name="reason">输出的吊销原因枚举</param>
    /// <returns>是否成功解析</returns>
    public static bool TryParseRevocationReason(string? reasonString, [NotNullWhen(true)] out CertificateRevocationReason? reason)
    {
        var parsed = ParseRevocationReason(reasonString);
        if (parsed.HasValue)
        {
            reason = parsed.Value;
            return true;
        }

        reason = null;
        return false;
    }
}
