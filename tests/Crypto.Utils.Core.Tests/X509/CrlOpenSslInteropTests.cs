using Crypto.Utils.Crypto;
using Crypto.Utils.TestSupport;
using Crypto.Utils.X509;
using CertificateRevocationReason = Crypto.Utils.X509.Enums.CertificateRevocationReason;

namespace Crypto.Utils.Core.Tests.X509;

/// <summary>
/// CRL 对照测试：代码生成 → openssl 解析、素材双方解析对照。
/// </summary>
[Trait("Category", "Integration")]
[Trait("Category", "OpenSSL")]
public class CrlOpenSslInteropTests : IDisposable
{
    private readonly string _tempDir;

    public CrlOpenSslInteropTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"crl_interop_{Guid.NewGuid():N}");
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
    public async Task CodeGeneratedCrl_ShouldBeParsedByOpenSsl()
    {
        CliToolGuard.EnsureToolAvailable(OpenSslCli.BinaryPath);

        // Arrange - 代码生成 CRL
        var keyPair = AsymmetricKeyPair.GenerateRsa(2048);
        var now = DateTime.UtcNow;
        var crl = CertificateRevocationList.Generate(
            "/C=CN/O=Interop Org/CN=Interop CA",
            keyPair.PrivateKey,
            [("AAAA", now, CertificateRevocationReason.KeyCompromise)],
            now,
            now.AddDays(30),
            "SHA256WITHRSA");

        var crlPath = Path.Combine(_tempDir, "interop.crl");
        await File.WriteAllTextAsync(crlPath, crl.ToPem());

        // Act - openssl 解析
        var result = await OpenSslCli.CrlInfoAsync(crlPath);

        // Assert
        result.IsSuccess.Should().BeTrue(result.FullOutput);
        result.StandardOutput.Should().Contain("CN = Interop CA");
        result.StandardOutput.Should().Contain("Serial Number: AAAA");
        result.StandardOutput.Should().Contain("Key Compromise");
    }

    [Fact]
    public async Task Fixture_CodeAndOpenSsl_ShouldAgree()
    {
        CliToolGuard.EnsureToolAvailable(OpenSslCli.BinaryPath);

        var fixturePath = TestData.Crls("test.crl");

        var crl = CertificateRevocationList.FromPem(File.ReadAllText(fixturePath));
        var result = await OpenSslCli.CrlInfoAsync(fixturePath);
        result.IsSuccess.Should().BeTrue(result.FullOutput);

        result.StandardOutput.Should().Contain("CN = Test CRL CA");
        result.StandardOutput.Should().Contain("Serial Number: 1111");
        result.StandardOutput.Should().Contain("Serial Number: 2222");

        crl.Issuer.Should().Contain("CN=Test CRL CA");
        crl.RevokedCertificatesCount.Should().Be(2);
        crl.IsRevoked("1111").Should().BeTrue();
    }
}
