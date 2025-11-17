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
    [Display(Name = "无")]
    None = 0,

    [Display(Name = "服务器身份验证")]
    ServerAuthentication = 1 << 0,

    [Display(Name = "客户端身份验证")]
    ClientAuthentication = 1 << 1,

    [Display(Name = "代码签名")]
    CodeSigning = 1 << 2,

    [Display(Name = "电子邮件保护")]
    EmailProtection = 1 << 3,

    [Display(Name = "IPsec 端系统")]
    IpsecEndSystem = 1 << 4,

    [Display(Name = "IPsec 隧道")]
    IpsecTunnel = 1 << 5,

    [Display(Name = "IPsec 用户")]
    IpsecUser = 1 << 6,

    [Display(Name = "时间戳")]
    TimeStamping = 1 << 7,

    [Display(Name = "OCSP 签名")]
    OcspSigning = 1 << 8,

    [Display(Name = "数据验证和认证服务")]
    Dvcs = 1 << 9,

    [Display(Name = "SBGP 证书")]
    SbgpCertificate = 1 << 10,

    [Display(Name = "EAP over PPP")]
    EapOverPpp = 1 << 11,

    [Display(Name = "EAP over LAN")]
    EapOverLan = 1 << 12,

    [Display(Name = "SSH 客户端")]
    SshClient = 1 << 13,

    [Display(Name = "SSH 服务器")]
    SshServer = 1 << 14,

    [Display(Name = "文档签名")]
    DocumentSigning = 1 << 15,

    [Display(Name = "任意用途")]
    AnyPurpose = 1 << 16,

    [Display(Name = "智能卡登录")]
    SmartCardLogon = 1 << 17
}
