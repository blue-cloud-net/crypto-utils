using Crypto.Utils.Crypto;
using Crypto.Utils.TestSupport;
using Crypto.Utils.X509;
using Crypto.Utils.X509.Enums;
using Crypto.Utils.X509.Models;

namespace Crypto.Utils.Core.Tests.X509;

/// <summary>
/// 证书三类对照测试：
/// A. openssl/tongsuo 运行时生成 → 代码解析断言；
/// B. 代码生成 → openssl/tongsuo 解析断言；
/// C. 同一素材分别用代码与 openssl 解析，关键属性一致。
/// </summary>
[Trait("Category", "Integration")]
[Trait("Category", "OpenSSL")]
public class CertificateOpenSslInteropTests : IDisposable
{
    private readonly string _tempDir;

    public CertificateOpenSslInteropTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"cert_interop_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            try
            {
                Directory.Delete(_tempDir, true);
            }
            catch
            {
                // 忽略清理错误。
            }
        }
    }

    #region A. openssl 生成 → 代码解析

    [Fact]
    public async Task OpenSslGeneratedCert_ShouldBeParsedByCode()
    {
        CliToolGuard.EnsureToolAvailable(OpenSslCli.BinaryPath);

        // Arrange - 运行时生成密钥与含扩展自签名证书
        var keyPath = Path.Combine(_tempDir, "key.pem");
        var certPath = Path.Combine(_tempDir, "cert.pem");

        var genKey = await OpenSslCli.GenerateRsaKeyAsync(keyPath);
        genKey.IsSuccess.Should().BeTrue(genKey.FullOutput);

        var genCert = await OpenSslCli.GenerateSelfSignedCertificateAsync(
            keyPath,
            certPath,
            "/CN=interop.example.com",
            days: 365,
            subjectAlternativeNames: "DNS:interop.example.com,DNS:www.interop.example.com",
            keyUsage: "critical,digitalSignature,keyEncipherment",
            extendedKeyUsage: "serverAuth",
            includeSki: true,
            serialHex: "7777");
        genCert.IsSuccess.Should().BeTrue(genCert.FullOutput);

        // Act - 代码解析
        var cert = Certificate.FromPem(await File.ReadAllTextAsync(certPath));

        // Assert - 与生成参数一致
        cert.Subject.Should().Contain("CN=interop.example.com");
        cert.SerialNumber.Should().Be("7777");
        cert.KeyUsages.Should().HaveFlag(KeyUsage.DigitalSignature);
        cert.KeyUsages.Should().HaveFlag(KeyUsage.KeyEncipherment);
        cert.ExtendedKeyUsages.Should().HaveFlag(ExtendedKeyUsage.ServerAuthentication);
        cert.SubjectAlternativeNames.Should().Contain(n => n.Value == "interop.example.com");
        cert.SubjectAlternativeNames.Should().Contain(n => n.Value == "www.interop.example.com");
        cert.SubjectKeyIdentifier.Should().NotBeNullOrEmpty();
        cert.ValidityPeriod.Should().BeCloseTo(TimeSpan.FromDays(365), TimeSpan.FromHours(1));
    }

    #endregion

    #region B. 代码生成 → openssl 解析

    [Fact]
    public async Task CodeGeneratedCert_ShouldBeParsedByOpenSsl()
    {
        CliToolGuard.EnsureToolAvailable(OpenSslCli.BinaryPath);

        // Arrange - 代码生成含扩展自签名证书
        var keyPair = AsymmetricKeyPair.GenerateRsa(2048);
        var cert = Certificate.GenerateSelfSigned(
            "/C=CN/O=Interop Corp/CN=b2o.example.com",
            keyPair.PrivateKey,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(364),
            "8888",
            "SHA256WITHRSA",
            new X509ExtensionOptions
            {
                KeyUsages = KeyUsage.DigitalSignature | KeyUsage.KeyEncipherment,
                ExtendedKeyUsages = ExtendedKeyUsage.ServerAuthentication,
                SubjectAlternativeNames = [new GeneralName(GeneralNameType.DnsName, "b2o.example.com")],
                IncludeSubjectKeyIdentifier = true,
                IncludeAuthorityKeyIdentifier = true,
            });

        var certPath = Path.Combine(_tempDir, "b2o.pem");
        await File.WriteAllTextAsync(certPath, cert.ToPem());

        // Act - openssl 解析
        var result = await OpenSslCli.X509InfoAsync(certPath);

        // Assert - 输出包含预期字段
        result.IsSuccess.Should().BeTrue(result.FullOutput);
        result.StandardOutput.Should().Contain("CN = b2o.example.com");
        result.StandardOutput.Should().Contain("Subject Alternative Name");
        result.StandardOutput.Should().Contain("DNS:b2o.example.com");
        result.StandardOutput.Should().Contain("X509v3 Key Usage");
        result.StandardOutput.Should().Contain("Digital Signature");
        result.StandardOutput.Should().Contain("X509v3 Subject Key Identifier");
        result.StandardOutput.Should().Contain("X509v3 Authority Key Identifier");
    }

    #endregion

    #region C. 素材双方解析对照

    [Fact]
    public async Task Fixture_CodeAndOpenSsl_ShouldAgreeOnKeyFields()
    {
        CliToolGuard.EnsureToolAvailable(OpenSslCli.BinaryPath);

        // 同一素材
        var fixturePath = TestData.Certs("rsa-2048-selfsigned-ext.pem");

        // 代码解析
        var cert = Certificate.FromPem(File.ReadAllText(fixturePath));

        // openssl 解析
        var result = await OpenSslCli.X509InfoAsync(fixturePath);
        result.IsSuccess.Should().BeTrue(result.FullOutput);

        // 双方一致
        result.StandardOutput.Should().Contain("CN = test.example.com");
        result.StandardOutput.Should().Contain("DNS:test.example.com");
        result.StandardOutput.Should().Contain("Digital Signature");
        result.StandardOutput.Should().Contain("TLS Web Server Authentication");
        result.StandardOutput.Should().Contain("http://crl.example.com/test.crl");

        cert.Subject.Should().Contain("CN=test.example.com");
        cert.SerialNumber.Should().Be("1001");
        cert.KeyUsages.Should().HaveFlag(KeyUsage.DigitalSignature);
        cert.ExtendedKeyUsages.Should().HaveFlag(ExtendedKeyUsage.ServerAuthentication);
    }

    #endregion

    #region SM2（tongsuo）

    [Fact]
    public async Task TongsuoGeneratedSm2Cert_ShouldBeParsedByCode()
    {
        CliToolGuard.EnsureToolAvailable(TongsuoCli.BinaryPath);

        // Arrange - tongsuo 运行时生成 SM2 自签名证书
        var keyPath = Path.Combine(_tempDir, "sm2-key.pem");
        var certPath = Path.Combine(_tempDir, "sm2-cert.pem");

        var genKey = await TongsuoCli.GenerateSm2KeyPairAsync(keyPath);
        genKey.IsSuccess.Should().BeTrue(genKey.FullOutput);

        var genCert = await TongsuoCli.GenerateSm2SelfSignedCertificateAsync(
            keyPath,
            certPath,
            "/C=CN/O=SM2 Corp/CN=sm2-interop.example.cn",
            days: 365,
            subjectAlternativeNames: "DNS:sm2-interop.example.cn",
            keyUsage: "critical,digitalSignature",
            extendedKeyUsage: "serverAuth");
        genCert.IsSuccess.Should().BeTrue(genCert.FullOutput);

        // Act - 代码解析
        var cert = Certificate.FromPem(await File.ReadAllTextAsync(certPath));

        // Assert
        cert.Subject.Should().Contain("CN=sm2-interop.example.cn");
        cert.KeyUsages.Should().HaveFlag(KeyUsage.DigitalSignature);
        cert.SubjectAlternativeNames.Should().Contain(n => n.Value == "sm2-interop.example.cn");
        cert.IsSignatureVerify(cert).Should().BeTrue();
    }

    [Fact]
    public async Task CodeGeneratedSm2Cert_ShouldBeParsedByTongsuo()
    {
        CliToolGuard.EnsureToolAvailable(TongsuoCli.BinaryPath);

        // Arrange - 代码生成 SM2 自签名证书
        var keyPair = AsymmetricKeyPair.GenerateSm2();
        var cert = Certificate.GenerateSelfSigned(
            "/C=CN/O=SM2 Corp/CN=sm2-b2o.example.cn",
            keyPair.PrivateKey,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(364),
            "9001",
            "SM3WITHSM2",
            new X509ExtensionOptions
            {
                KeyUsages = KeyUsage.DigitalSignature,
                SubjectAlternativeNames = [new GeneralName(GeneralNameType.DnsName, "sm2-b2o.example.cn")],
                IncludeSubjectKeyIdentifier = true,
            });

        var certPath = Path.Combine(_tempDir, "sm2-b2o.pem");
        await File.WriteAllTextAsync(certPath, cert.ToPem());

        // Act - tongsuo 解析
        var result = await TongsuoCli.X509InfoAsync(certPath);

        // Assert
        result.IsSuccess.Should().BeTrue(result.FullOutput);
        result.StandardOutput.Should().Contain("CN=sm2-b2o.example.cn");
        result.StandardOutput.Should().Contain("DNS:sm2-b2o.example.cn");
        result.StandardOutput.Should().Contain("Digital Signature");
    }

    #endregion
}
