namespace Crypto.Utils.X509.Enums;

public static class CrlReasonExtensions
{
    /// <summary>
    /// 吊销原因枚举值扩展方法
    /// </summary>
    /// <param name="reason">吊销原因枚举值</param>
    extension(CrlReason reason)
    {
        /// <summary>
        /// 检查吊销原因是否可以被撤销
        /// </summary>
        /// <returns>如果可以被撤销返回 true</returns>
        /// <remarks>
        /// 只有 CertificateHold 状态可以通过 RemoveFromCrl 撤销
        /// </remarks>
        public bool IsRevocable() => reason == CrlReason.CertificateHold;

        /// <summary>
        /// 检查吊销原因是否表示密钥泄露
        /// </summary>
        /// <returns>如果表示密钥泄露返回 true</returns>
        public bool IsKeyCompromise() =>
            reason is CrlReason.KeyCompromise
                    or CrlReason.CACompromise
                    or CrlReason.AACompromise;

        /// <summary>
        /// 获取吊销原因的枚举名称
        /// </summary>
        /// <returns>枚举名称</returns>
        public string GetName() => Enum.GetName(reason) ?? reason.ToString();

        /// <summary>
        /// 获取吊销原因的友好名称
        /// </summary>
        /// <returns>友好名称</returns>
        public string GetFriendlyName() =>
            EnumDisplayNameCache<CrlReason>.GetDisplayName(reason) ?? reason.GetName();
    }

}