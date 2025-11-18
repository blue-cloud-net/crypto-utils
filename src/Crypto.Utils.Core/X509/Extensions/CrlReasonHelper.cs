using CrlReason = Crypto.Utils.X509.Enums.CrlReason;

namespace Crypto.Utils.X509.Extensions;

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
    public static Org.BouncyCastle.Asn1.X509.CrlReason ToBouncyCastleFormat(CrlReason reason) => new((int)reason);

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
    public static CrlReason? FromInt(int reasonCode)
    {
        if (Enum.IsDefined(typeof(CrlReason), reasonCode))
        {
            return (CrlReason)reasonCode;
        }

        return null;
    }

    /// <summary>
    /// 获取所有定义的吊销原因
    /// </summary>
    /// <returns>吊销原因列表</returns>
    public static IEnumerable<CrlReason> GetAllReasons() => Enum.GetValues<CrlReason>();

    /// <summary>
    /// 获取所有吊销原因的友好名称字典
    /// </summary>
    /// <returns>吊销原因到友好名称的映射</returns>
    public static Dictionary<CrlReason, string> GetReasonNameDictionary()
    {
        return Enum.GetValues<CrlReason>()
            .ToDictionary(r => r, r => r.GetFriendlyName());
    }
}
