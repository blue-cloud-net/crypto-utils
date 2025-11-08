using Cert.Utils.Common;
using CrlReason = Cert.Utils.X509.Enums.CrlReason;

namespace Cert.Utils.X509.Extensions;

/// <summary>
/// CRL 吊销原因辅助类
/// 提供吊销原因枚举和 BouncyCastle 格式之间的转换功能。
/// </summary>
public static class CrlReasonHelper
{
    /// <summary>
    /// 将 CrlReason 枚举转换为 BouncyCastle 的 CrlReason 格式
    /// </summary>
    /// <param name="reason">吊销原因枚举值</param>
    /// <returns>BouncyCastle CrlReason 对象</returns>
    public static Org.BouncyCastle.Asn1.X509.CrlReason ToBouncyCastleFormat(CrlReason reason)
    {
        return new Org.BouncyCastle.Asn1.X509.CrlReason((int)reason);
    }

    /// <summary>
    /// 从 BouncyCastle CrlReason 对象转换为 CrlReason 枚举
    /// </summary>
    /// <param name="bcReason">BouncyCastle CrlReason 对象</param>
    /// <returns>吊销原因枚举值</returns>
    public static CrlReason FromBouncyCastleFormat(Org.BouncyCastle.Asn1.X509.CrlReason bcReason)
    {
        var reasonValue = bcReason.IntValueExact;

        if (Enum.IsDefined(typeof(CrlReason), reasonValue))
        {
            return (CrlReason)reasonValue;
        }

        // 如果是未定义的值,返回 Unspecified
        return CrlReason.Unspecified;
    }

    /// <summary>
    /// 从整数值转换为 CrlReason 枚举
    /// </summary>
    /// <param name="reasonCode">吊销原因代码</param>
    /// <returns>吊销原因枚举值</returns>
    public static CrlReason FromInt(int reasonCode)
    {
        if (Enum.IsDefined(typeof(CrlReason), reasonCode))
        {
            return (CrlReason)reasonCode;
        }

        return CrlReason.Unspecified;
    }

    /// <summary>
    /// 获取吊销原因的友好名称
    /// </summary>
    /// <param name="reason">吊销原因枚举值</param>
    /// <returns>友好名称</returns>
    public static string GetFriendlyName(CrlReason reason)
    {
        return EnumDisplayNameCache<CrlReason>.GetDisplayName(reason);
    }

    /// <summary>
    /// 获取吊销原因的枚举名称
    /// </summary>
    /// <param name="reason">吊销原因枚举值</param>
    /// <returns>枚举名称</returns>
    public static string GetName(CrlReason reason)
    {
        return Enum.GetName(reason) ?? reason.ToString();
    }

    /// <summary>
    /// 检查吊销原因是否可以被撤销
    /// </summary>
    /// <param name="reason">吊销原因枚举值</param>
    /// <returns>如果可以被撤销返回 true</returns>
    /// <remarks>
    /// 只有 CertificateHold 状态可以通过 RemoveFromCrl 撤销
    /// </remarks>
    public static bool IsRevocable(CrlReason reason)
    {
        return reason == CrlReason.CertificateHold;
    }

    /// <summary>
    /// 检查吊销原因是否表示密钥泄露
    /// </summary>
    /// <param name="reason">吊销原因枚举值</param>
    /// <returns>如果表示密钥泄露返回 true</returns>
    public static bool IsKeyCompromise(CrlReason reason)
    {
        return reason == CrlReason.KeyCompromise
            || reason == CrlReason.CACompromise
            || reason == CrlReason.AACompromise;
    }

    /// <summary>
    /// 获取所有定义的吊销原因
    /// </summary>
    /// <returns>吊销原因列表</returns>
    public static IEnumerable<CrlReason> GetAllReasons()
    {
        return Enum.GetValues<CrlReason>();
    }

    /// <summary>
    /// 获取所有吊销原因的友好名称字典
    /// </summary>
    /// <returns>吊销原因到友好名称的映射</returns>
    public static Dictionary<CrlReason, string> GetReasonNameDictionary()
    {
        return Enum.GetValues<CrlReason>()
            .ToDictionary(r => r, r => GetFriendlyName(r));
    }
}
