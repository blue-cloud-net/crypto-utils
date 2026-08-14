using Crypto.Utils.Crypto;
using Crypto.Utils.TestSupport;
using Crypto.Utils.X509;
using Crypto.Utils.X509.Enums;
using Crypto.Utils.X509.Models;

namespace Crypto.Utils.Core.Tests.X509;

/// <summary>
/// CSR 对照测试：openssl 生成 → 代码解析、代码生成 → openssl 解析、素材双方解析对照。
/// </summary>
[Trait("Category", "Integration")]
[Trait("Category", "OpenSSL")]
public class CsrOpenSslInteropTests : IDisposable
{
    private readonly string _tempDir;

    public CsrOpenSslInteropTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"csr_interop_{Guid.NewGuid():N}");
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

    [Fact]
    public async Task OpenSslGeneratedCsr_ShouldBeParsedByCode()
    {
        CliToolGuard.EnsureToolAvailable(OpenSslCli.BinaryPath);

        // Arrange - openssl 运行时生成密钥与 CSR
        var keyPath = Path.Combine(_tempDir, "key.pem");
        var csrPath = Path.Combine(_tempDir, "req.csr");
        await OpenSslCli.GenerateRsaKeyAsync(keyPath);

        var genCsr = await OpenSslCli.GenerateCsrAsync(
            keyPath,
            csrPath,
            "/CN=csr-interop.example.com",
            subjectAlternativeNames: "DNS:csr-interop.example.com,DNS:www.csr-interop.example.com",
            keyUsage: "critical,digitalSignature,keyEncipherment",
            extendedKeyUsage: "serverAuth");
        genCsr.IsSuccess.Should().BeTrue(genCsr.FullOutput);

        // Act - 代码解析
        var csr = CertificateSigningRequest.FromPem(await File.ReadAllTextAsync(csrPath));

        // Assert
        csr.Subject.Should().Contain("CN=csr-interop.example.com");
        csr.SignatureAlgorithmName.Should().BeEquivalentTo("SHA256WITHRSA");
        csr.Verify().Should().BeTrue();
        csr.KeyUsages.Should().NotBeNull();
        csr.KeyUsages!.Value.Should().HaveFlag(KeyUsage.DigitalSignature);
        csr.ExtendedKeyUsages.Should().NotBeNull();
        csr.ExtendedKeyUsages!.Value.Should().HaveFlag(ExtendedKeyUsage.ServerAuthentication);
        csr.SubjectAlternativeNames.Should().Contain(n => n.Value == "csr-interop.example.com");
        csr.SubjectAlternativeNames.Should().Contain(n => n.Value == "www.csr-interop.example.com");
    }

    [Fact]
    public async Task CodeGeneratedCsr_ShouldBeParsedByOpenSsl()
    {
        CliToolGuard.EnsureToolAvailable(OpenSslCli.BinaryPath);

        // Arrange - 代码生成含扩展 CSR
        var keyPair = AsymmetricKeyPair.GenerateRsa(2048);
        var csr = CertificateSigningRequest.Generate(
            "/C=CN/O=Interop Corp/CN=csr-b2o.example.com",
            keyPair,
            "SHA256WITHRSA",
            new X509ExtensionOptions
            {
                KeyUsages = KeyUsage.DigitalSignature | KeyUsage.KeyEncipherment,
                ExtendedKeyUsages = ExtendedKeyUsage.ServerAuthentication,
                SubjectAlternativeNames = [new GeneralName(GeneralNameType.DnsName, "csr-b2o.example.com")],
            });

        var csrPath = Path.Combine(_tempDir, "csr-b2o.csr");
        await File.WriteAllTextAsync(csrPath, csr.ToPem());

        // Act - openssl 解析并验证
        var result = await OpenSslCli.ReqInfoAsync(csrPath);

        // Assert
        result.IsSuccess.Should().BeTrue(result.FullOutput);
        result.StandardOutput.Should().Contain("CN = csr-b2o.example.com");
        result.StandardOutput.Should().Contain("DNS:csr-b2o.example.com");
        result.StandardOutput.Should().Contain("Digital Signature");
        result.StandardOutput.Should().Contain("TLS Web Server Authentication");
        result.FullOutput.Should().Contain("verify OK");
    }

    [Fact]
    public async Task Fixture_CodeAndOpenSsl_ShouldAgree()
    {
        CliToolGuard.EnsureToolAvailable(OpenSslCli.BinaryPath);

        var fixturePath = TestData.Csrs("rsa-2048-san.csr");

        var csr = CertificateSigningRequest.FromPem(File.ReadAllText(fixturePath));
        var result = await OpenSslCli.ReqInfoAsync(fixturePath);
        result.IsSuccess.Should().BeTrue(result.FullOutput);

        result.StandardOutput.Should().Contain("CN = multi.example.com");
        result.StandardOutput.Should().Contain("DNS:multi.example.com");
        result.FullOutput.Should().Contain("verify OK");

        csr.Subject.Should().Contain("CN=multi.example.com");
        csr.SubjectAlternativeNames.Should().Contain(n => n.Value == "multi.example.com");
        csr.Verify().Should().BeTrue();
    }
}
