using System.Text;
using Crypto.Utils.Crypto.Sm;
using Crypto.Utils.TestUtils;
using FluentAssertions;
using NUnit.Framework;

namespace Crypto.Utils.Core.Tests.Crypto.Sm;

/// <summary>
/// SM3 与 OpenSSL 互操作性测试
/// 需要系统安装 OpenSSL 且支持 SM3
/// </summary>
[TestFixture]
[Category("Integration")]
[Category("OpenSSL")]
public class SM3OpenSslInteropTests
{
    private OpenSslWrapper _openssl = null!;
    private string _tempDir = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _openssl = new OpenSslWrapper();

        // 检查 OpenSSL 版本
        var versionResult = await _openssl.GetVersionAsync();
        if (!versionResult.IsSuccess)
        {
            Assert.Ignore("OpenSSL is not available. Skipping tests.");
        }
    }

    [SetUp]
    public void SetUp()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"sm3_test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_tempDir))
        {
            try
            {
                Directory.Delete(_tempDir, true);
            }
            catch
            {
                // 忽略清理错误
            }
        }
    }

    #region 标准测试向量验证

    [Test]
    [TestCase("", "1AB21D8355CFA17F8E61194831E81A8F22BEC8C728FEFB747ED035EB5082AA2B")]
    [TestCase("abc", "66C7F0F462EEEDD9D1F2D46BDC10E4E24167C4875CF2F7A2297DA02B8F4BA8E0")]
    public async Task StandardTestVector_ShouldMatchOpenSSL(string input, string expectedHex)
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes(input);
        var dataPath = Path.Combine(_tempDir, "data.txt");
        await File.WriteAllBytesAsync(dataPath, data);

        // Act - 使用 SM3 类计算哈希
        var sm3Hash = SM3.HashData(data);

        // Act - 使用 OpenSSL 计算哈希
        var opensslResult = await _openssl.ExecuteCommandAsync($"dgst -sm3 -hex \"{dataPath}\"");

        // Assert
        if (!opensslResult.IsSuccess)
        {
            Assert.Ignore($"OpenSSL does not support SM3. Error: {opensslResult.StandardError}");
            return;
        }

        var expectedHash = Convert.FromHexString(expectedHex);
        sm3Hash.Should().Equal(expectedHash, "SM3 hash should match standard test vector");

        // 验证 OpenSSL 输出包含预期的哈希值
        var opensslOutput = opensslResult.StandardOutput.Trim();
        opensslOutput.Should().ContainEquivalentOf(expectedHex,
            "OpenSSL should produce the same hash");
    }

    #endregion

    #region 哈希一致性测试

    [Test]
    public async Task HashData_ShouldMatchOpenSSL()
    {
        // Arrange
        var testData = Encoding.UTF8.GetBytes("Hello SM3 from .NET!");
        var dataPath = Path.Combine(_tempDir, "test_data.txt");
        await File.WriteAllBytesAsync(dataPath, testData);

        // Act - 使用 SM3 类计算哈希
        var sm3Hash = SM3.HashData(testData);

        // Act - 使用 OpenSSL 计算哈希
        var opensslResult = await _openssl.ExecuteCommandAsync($"dgst -sm3 -binary -out \"{_tempDir}/hash.bin\" \"{dataPath}\"");

        if (!opensslResult.IsSuccess)
        {
            Assert.Ignore($"OpenSSL does not support SM3. Error: {opensslResult.StandardError}");
            return;
        }

        var opensslHash = await File.ReadAllBytesAsync(Path.Combine(_tempDir, "hash.bin"));

        // Assert
        sm3Hash.Should().Equal(opensslHash, "SM3 hash should match OpenSSL hash");
    }

    [Test]
    public async Task HashData_LargeData_ShouldMatchOpenSSL()
    {
        // Arrange
        var testData = new byte[10000];
        Random.Shared.NextBytes(testData);
        var dataPath = Path.Combine(_tempDir, "large_data.bin");
        await File.WriteAllBytesAsync(dataPath, testData);

        // Act - 使用 SM3 类计算哈希
        var sm3Hash = SM3.HashData(testData);

        // Act - 使用 OpenSSL 计算哈希
        var opensslResult = await _openssl.ExecuteCommandAsync($"dgst -sm3 -binary -out \"{_tempDir}/hash.bin\" \"{dataPath}\"");

        if (!opensslResult.IsSuccess)
        {
            Assert.Ignore($"OpenSSL does not support SM3. Error: {opensslResult.StandardError}");
            return;
        }

        var opensslHash = await File.ReadAllBytesAsync(Path.Combine(_tempDir, "hash.bin"));

        // Assert
        sm3Hash.Should().Equal(opensslHash, "SM3 hash should match OpenSSL hash for large data");
    }

    [Test]
    public async Task HashData_EmptyData_ShouldMatchOpenSSL()
    {
        // Arrange
        var testData = Array.Empty<byte>();
        var dataPath = Path.Combine(_tempDir, "empty_data.txt");
        await File.WriteAllBytesAsync(dataPath, testData);

        // Act - 使用 SM3 类计算哈希
        var sm3Hash = SM3.HashData(testData);

        // Act - 使用 OpenSSL 计算哈希
        var opensslResult = await _openssl.ExecuteCommandAsync($"dgst -sm3 -binary -out \"{_tempDir}/hash.bin\" \"{dataPath}\"");

        if (!opensslResult.IsSuccess)
        {
            Assert.Ignore($"OpenSSL does not support SM3. Error: {opensslResult.StandardError}");
            return;
        }

        var opensslHash = await File.ReadAllBytesAsync(Path.Combine(_tempDir, "hash.bin"));

        // Assert
        sm3Hash.Should().Equal(opensslHash, "SM3 hash should match OpenSSL hash for empty data");
    }

    #endregion

    #region 流哈希测试

    [Test]
    public async Task HashStream_ShouldMatchOpenSSL()
    {
        // Arrange
        var testData = Encoding.UTF8.GetBytes("Stream data for SM3 testing with OpenSSL compatibility");
        var dataPath = Path.Combine(_tempDir, "stream_data.txt");
        await File.WriteAllBytesAsync(dataPath, testData);

        // Act - 使用 SM3 类计算哈希（从流）
        byte[] sm3Hash;
        using (var stream = File.OpenRead(dataPath))
        {
            sm3Hash = SM3.HashData(stream);
        }

        // Act - 使用 OpenSSL 计算哈希
        var opensslResult = await _openssl.ExecuteCommandAsync($"dgst -sm3 -binary -out \"{_tempDir}/hash.bin\" \"{dataPath}\"");

        if (!opensslResult.IsSuccess)
        {
            Assert.Ignore($"OpenSSL does not support SM3. Error: {opensslResult.StandardError}");
            return;
        }

        var opensslHash = await File.ReadAllBytesAsync(Path.Combine(_tempDir, "hash.bin"));

        // Assert
        sm3Hash.Should().Equal(opensslHash, "SM3 stream hash should match OpenSSL hash");
    }

    #endregion

    #region 增量哈希测试

    [Test]
    public async Task IncrementalHash_ShouldMatchOpenSSL()
    {
        // Arrange
        var part1 = Encoding.UTF8.GetBytes("First part ");
        var part2 = Encoding.UTF8.GetBytes("Second part ");
        var part3 = Encoding.UTF8.GetBytes("Third part");

        var allData = part1.Concat(part2).Concat(part3).ToArray();
        var dataPath = Path.Combine(_tempDir, "full_data.txt");
        await File.WriteAllBytesAsync(dataPath, allData);

        // Act - 使用 SM3 类增量计算哈希
        byte[] sm3Hash;
        using (var sm3 = new SM3())
        {
            var outputBuffer = new byte[sm3.OutputBlockSize];
            sm3.TransformBlock(part1, 0, part1.Length, outputBuffer, 0);
            sm3.TransformBlock(part2, 0, part2.Length, outputBuffer, 0);
            sm3.TransformBlock(part3, 0, part3.Length, outputBuffer, 0);
            sm3.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            sm3Hash = sm3.Hash!;
        }

        // Act - 使用 OpenSSL 计算哈希
        var opensslResult = await _openssl.ExecuteCommandAsync($"dgst -sm3 -binary -out \"{_tempDir}/hash.bin\" \"{dataPath}\"");

        if (!opensslResult.IsSuccess)
        {
            Assert.Ignore($"OpenSSL does not support SM3. Error: {opensslResult.StandardError}");
            return;
        }

        var opensslHash = await File.ReadAllBytesAsync(Path.Combine(_tempDir, "hash.bin"));

        // Assert
        sm3Hash.Should().Equal(opensslHash, "SM3 incremental hash should match OpenSSL hash");
    }

    #endregion

    #region 不同数据类型测试

    [Test]
    [TestCase("Hello World")]
    [TestCase("中文测试")]
    [TestCase("Special chars: !@#$%^&*()")]
    [TestCase("Numbers: 0123456789")]
    public async Task HashData_VariousStrings_ShouldMatchOpenSSL(string input)
    {
        // Arrange
        var testData = Encoding.UTF8.GetBytes(input);
        var dataPath = Path.Combine(_tempDir, "string_data.txt");
        await File.WriteAllBytesAsync(dataPath, testData);

        // Act - 使用 SM3 类计算哈希
        var sm3Hash = SM3.HashData(testData);

        // Act - 使用 OpenSSL 计算哈希
        var opensslResult = await _openssl.ExecuteCommandAsync($"dgst -sm3 -binary -out \"{_tempDir}/hash.bin\" \"{dataPath}\"");

        if (!opensslResult.IsSuccess)
        {
            Assert.Ignore($"OpenSSL does not support SM3. Error: {opensslResult.StandardError}");
            return;
        }

        var opensslHash = await File.ReadAllBytesAsync(Path.Combine(_tempDir, "hash.bin"));

        // Assert
        sm3Hash.Should().Equal(opensslHash, $"SM3 hash should match OpenSSL hash for input: {input}");
    }

    [Test]
    public async Task HashData_BinaryData_ShouldMatchOpenSSL()
    {
        // Arrange
        var testData = new byte[256];
        for (var i = 0; i < testData.Length; i++)
        {
            testData[i] = (byte)i;
        }
        var dataPath = Path.Combine(_tempDir, "binary_data.bin");
        await File.WriteAllBytesAsync(dataPath, testData);

        // Act - 使用 SM3 类计算哈希
        var sm3Hash = SM3.HashData(testData);

        // Act - 使用 OpenSSL 计算哈希
        var opensslResult = await _openssl.ExecuteCommandAsync($"dgst -sm3 -binary -out \"{_tempDir}/hash.bin\" \"{dataPath}\"");

        if (!opensslResult.IsSuccess)
        {
            Assert.Ignore($"OpenSSL does not support SM3. Error: {opensslResult.StandardError}");
            return;
        }

        var opensslHash = await File.ReadAllBytesAsync(Path.Combine(_tempDir, "hash.bin"));

        // Assert
        sm3Hash.Should().Equal(opensslHash, "SM3 hash should match OpenSSL hash for binary data");
    }

    #endregion

    #region 性能和稳定性测试

    [Test]
    public async Task HashData_MultipleRuns_ShouldBeConsistent()
    {
        // Arrange
        var testData = Encoding.UTF8.GetBytes("Consistency test data");
        var dataPath = Path.Combine(_tempDir, "consistency_data.txt");
        await File.WriteAllBytesAsync(dataPath, testData);

        // Act - 多次计算哈希
        var sm3Hashes = new List<byte[]>();
        for (var i = 0; i < 5; i++)
        {
            sm3Hashes.Add(SM3.HashData(testData));
        }

        // Act - 使用 OpenSSL 计算哈希
        var opensslResult = await _openssl.ExecuteCommandAsync($"dgst -sm3 -binary -out \"{_tempDir}/hash.bin\" \"{dataPath}\"");

        if (!opensslResult.IsSuccess)
        {
            Assert.Ignore($"OpenSSL does not support SM3. Error: {opensslResult.StandardError}");
            return;
        }

        var opensslHash = await File.ReadAllBytesAsync(Path.Combine(_tempDir, "hash.bin"));

        // Assert
        foreach (var hash in sm3Hashes)
        {
            hash.Should().Equal(opensslHash, "All SM3 hashes should be consistent with OpenSSL");
        }

        // 所有哈希应该相同
        for (var i = 1; i < sm3Hashes.Count; i++)
        {
            sm3Hashes[i].Should().Equal(sm3Hashes[0], "All SM3 hashes should be identical");
        }
    }

    #endregion
}
