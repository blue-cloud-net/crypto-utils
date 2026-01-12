namespace Crypto.Utils.X509.Enums;

/// <summary>
/// X.509 证书扩展密钥用途（Extended Key Usage, EKU）
/// 定义证书的具体应用场景和使用目的，是对 Key Usage 的进一步细化。
/// EKU 提供应用层面的约束，限制证书在特定场景下的使用。
/// RFC 标准参考 <see href="https://datatracker.ietf.org/doc/html/rfc5280#section-4.2.1.12"/>
/// </summary>
[Flags]
public enum ExtendedKeyUsage
{
    [Display(Name = "ExtendedKeyUsage_None", ResourceType = typeof(EnumResources))]
    None = 0,

    [Display(Name = "ExtendedKeyUsage_ServerAuthentication", ResourceType = typeof(EnumResources))]
    ServerAuthentication = 1 << 0,

    [Display(Name = "ExtendedKeyUsage_ClientAuthentication", ResourceType = typeof(EnumResources))]
    ClientAuthentication = 1 << 1,

    [Display(Name = "ExtendedKeyUsage_CodeSigning", ResourceType = typeof(EnumResources))]
    CodeSigning = 1 << 2,

    [Display(Name = "ExtendedKeyUsage_EmailProtection", ResourceType = typeof(EnumResources))]
    EmailProtection = 1 << 3,

    [Display(Name = "ExtendedKeyUsage_IpsecEndSystem", ResourceType = typeof(EnumResources))]
    IpsecEndSystem = 1 << 4,

    [Display(Name = "ExtendedKeyUsage_IpsecTunnel", ResourceType = typeof(EnumResources))]
    IpsecTunnel = 1 << 5,

    [Display(Name = "ExtendedKeyUsage_IpsecUser", ResourceType = typeof(EnumResources))]
    IpsecUser = 1 << 6,

    [Display(Name = "ExtendedKeyUsage_TimeStamping", ResourceType = typeof(EnumResources))]
    TimeStamping = 1 << 7,

    [Display(Name = "ExtendedKeyUsage_OcspSigning", ResourceType = typeof(EnumResources))]
    OcspSigning = 1 << 8,

    [Display(Name = "ExtendedKeyUsage_Dvcs", ResourceType = typeof(EnumResources))]
    Dvcs = 1 << 9,

    [Display(Name = "ExtendedKeyUsage_SbgpCertificate", ResourceType = typeof(EnumResources))]
    SbgpCertificate = 1 << 10,

    [Display(Name = "ExtendedKeyUsage_EapOverPpp", ResourceType = typeof(EnumResources))]
    EapOverPpp = 1 << 11,

    [Display(Name = "ExtendedKeyUsage_EapOverLan", ResourceType = typeof(EnumResources))]
    EapOverLan = 1 << 12,

    [Display(Name = "ExtendedKeyUsage_SshClient", ResourceType = typeof(EnumResources))]
    SshClient = 1 << 13,

    [Display(Name = "ExtendedKeyUsage_SshServer", ResourceType = typeof(EnumResources))]
    SshServer = 1 << 14,

    [Display(Name = "ExtendedKeyUsage_DocumentSigning", ResourceType = typeof(EnumResources))]
    DocumentSigning = 1 << 15,

    [Display(Name = "ExtendedKeyUsage_AnyPurpose", ResourceType = typeof(EnumResources))]
    AnyPurpose = 1 << 16,

    [Display(Name = "ExtendedKeyUsage_SmartCardLogon", ResourceType = typeof(EnumResources))]
    SmartCardLogon = 1 << 17
}
