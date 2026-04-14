using CliWrap;
using CliWrap.Buffered;

namespace Crypto.Utils.TestUtils;

/// <summary>
/// OpenSSL 命令行工具封装类，用于测试场景中生成和验证密钥、签名等
/// </summary>
public class OpenSslWrapper
{
    private readonly string _openSslPath;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="openSslPath">OpenSSL 可执行文件路径，默认为 "openssl"</param>
    public OpenSslWrapper(
        string openSslPath = "openssl")
    {
        _openSslPath = openSslPath;
    }

    /// <summary>
    /// 执行 OpenSSL 命令
    /// </summary>
    /// <param name="arguments">命令参数</param>
    /// <param name="stdin">标准输入数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>执行结果</returns>
    private async Task<OpenSslResult> ExecuteAsync(
        string arguments,
        string? stdin = null,
        CancellationToken cancellationToken = default)
    {
        var cmd = Cli.Wrap(_openSslPath)
            .WithArguments(arguments)
            .WithValidation(CommandResultValidation.None);

        if (stdin != null)
        {
            cmd = cmd.WithStandardInputPipe(PipeSource.FromString(stdin));
        }

        var result = await cmd.ExecuteBufferedAsync(cancellationToken);

        return new OpenSslResult
        {
            ExitCode = result.ExitCode,
            StandardOutput = result.StandardOutput,
            StandardError = result.StandardError
        };
    }

    #region SM2 相关方法

    /// <summary>
    /// 生成 SM2 密钥对 (PEM 格式)
    /// </summary>
    /// <param name="privateKeyPath">私钥输出路径</param>
    /// <param name="publicKeyPath">公钥输出路径（可选）</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task<OpenSslResult> GenerateSm2KeyPairAsync(
        string privateKeyPath,
        string? publicKeyPath = null,
        CancellationToken cancellationToken = default)
    {
        // 使用 genpkey 生成私钥，避免生成 SM2 PARAMETERS 部分
        var genResult = await this.ExecuteAsync(
            $"genpkey -algorithm EC -pkeyopt ec_paramgen_curve:SM2 -out \"{privateKeyPath}\"",
            cancellationToken: cancellationToken);

        if (genResult.ExitCode != 0 || publicKeyPath == null)
            return genResult;

        // 从私钥导出公钥
        return await this.ExecuteAsync(
            $"pkey -in \"{privateKeyPath}\" -pubout -out \"{publicKeyPath}\"",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 使用 SM2 私钥对数据签名（计算 Z 值）
    /// </summary>
    /// <param name="dataPath">要签名的数据文件路径</param>
    /// <param name="privateKeyPath">私钥路径</param>
    /// <param name="signaturePath">签名输出路径</param>
    /// <param name="userId">用户ID（默认为 "1234567812345678"，与 BouncyCastle 一致）</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task<OpenSslResult> SignWithSm2Async(
        string dataPath,
        string privateKeyPath,
        string signaturePath,
        string userId = "1234567812345678",
        CancellationToken cancellationToken = default)
    {
        // 使用 dgst 命令配合 -sigopt distid 参数设置用户ID
        // OpenSSL 3.x 中 SM2 签名需要使用 dgst 命令而不是 pkeyutl
        var args = $"dgst -sm3 -sign \"{privateKeyPath}\" -sigopt distid:{userId} -out \"{signaturePath}\" \"{dataPath}\"";

        return await this.ExecuteAsync(args, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 使用 SM2 公钥验证签名（计算 Z 值）
    /// </summary>
    /// <param name="dataPath">原始数据文件路径</param>
    /// <param name="signaturePath">签名文件路径</param>
    /// <param name="publicKeyPath">公钥路径</param>
    /// <param name="userId">用户ID（默认为 "1234567812345678"，与 BouncyCastle 一致）</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task<OpenSslResult> VerifyWithSm2Async(
        string dataPath,
        string signaturePath,
        string publicKeyPath,
        string userId = "1234567812345678",
        CancellationToken cancellationToken = default)
    {
        // 使用 dgst 命令配合 -sigopt distid 参数设置用户ID
        // OpenSSL 3.x 中 SM2 验签需要使用 dgst 命令而不是 pkeyutl
        // 验签时必须使用与签名时相同的 distid
        var args = $"dgst -sm3 -verify \"{publicKeyPath}\" -signature \"{signaturePath}\" -sigopt distid:{userId} \"{dataPath}\"";

        return await this.ExecuteAsync(args, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 使用 SM2 公钥加密数据
    /// </summary>
    public async Task<OpenSslResult> EncryptWithSm2Async(
        string inputPath,
        string outputPath,
        string publicKeyPath,
        CancellationToken cancellationToken = default)
    {
        return await this.ExecuteAsync(
            $"pkeyutl -encrypt -in \"{inputPath}\" -pubin -inkey \"{publicKeyPath}\" -out \"{outputPath}\"",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 使用 SM2 私钥解密数据
    /// </summary>
    public async Task<OpenSslResult> DecryptWithSm2Async(
        string inputPath,
        string outputPath,
        string privateKeyPath,
        CancellationToken cancellationToken = default)
    {
        return await this.ExecuteAsync(
            $"pkeyutl -decrypt -in \"{inputPath}\" -inkey \"{privateKeyPath}\" -out \"{outputPath}\"",
            cancellationToken: cancellationToken);
    }

    #endregion

    #region 密钥相关方法

    /// <summary>
    /// 查看密钥详细信息
    /// </summary>
    /// <param name="keyPath">密钥文件路径</param>
    /// <param name="isPublicKey">是否为公钥</param>
    /// <param name="format">密钥格式（PEM 或 DER），默认为 PEM</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task<OpenSslResult> ViewKeyInfoAsync(
        string keyPath,
        bool isPublicKey = false,
        string format = "PEM",
        CancellationToken cancellationToken = default)
    {
        var pubinArg = isPublicKey ? "-pubin" : "";
        var informArg = format.Equals("DER", StringComparison.OrdinalIgnoreCase) ? "-inform DER" : "";
        return await this.ExecuteAsync(
            $"pkey {pubinArg} {informArg} -in \"{keyPath}\" -text -noout",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 将 PEM 格式私钥转换为 DER 格式 (PKCS#8)
    /// </summary>
    public async Task<OpenSslResult> ConvertPrivateKeyPemToDerAsync(
        string pemPath,
        string derPath,
        CancellationToken cancellationToken = default)
    {
        return await this.ExecuteAsync(
            $"pkcs8 -topk8 -nocrypt -in \"{pemPath}\" -out \"{derPath}\" -outform DER",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 将 DER 格式私钥转换为 PEM 格式
    /// </summary>
    public async Task<OpenSslResult> ConvertPrivateKeyDerToPemAsync(
        string derPath,
        string pemPath,
        CancellationToken cancellationToken = default)
    {
        return await this.ExecuteAsync(
            $"pkey -in \"{derPath}\" -inform DER -out \"{pemPath}\" -outform PEM",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 将 PEM 格式公钥转换为 DER 格式 (SPKI)
    /// </summary>
    public async Task<OpenSslResult> ConvertPublicKeyPemToDerAsync(
        string pemPath,
        string derPath,
        CancellationToken cancellationToken = default)
    {
        return await this.ExecuteAsync(
            $"ec -pubin -in \"{pemPath}\" -out \"{derPath}\" -outform DER",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 将 DER 格式公钥转换为 PEM 格式
    /// </summary>
    public async Task<OpenSslResult> ConvertPublicKeyDerToPemAsync(
        string derPath,
        string pemPath,
        CancellationToken cancellationToken = default)
    {
        return await this.ExecuteAsync(
            $"pkey -pubin -in \"{derPath}\" -inform DER -out \"{pemPath}\" -outform PEM",
            cancellationToken: cancellationToken);
    }

    #endregion

    #region 通用方法

    /// <summary>
    /// 执行通用的 OpenSSL 命令
    /// </summary>
    /// <param name="command">OpenSSL 命令（不包括 "openssl" 前缀）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>执行结果</returns>
    public async Task<OpenSslResult> ExecuteCommandAsync(
        string command,
        CancellationToken cancellationToken = default)
    {
        return await this.ExecuteAsync(command, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 获取 OpenSSL 版本信息
    /// </summary>
    public async Task<OpenSslResult> GetVersionAsync(CancellationToken cancellationToken = default)
    {
        return await this.ExecuteAsync("version", cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 检查 OpenSSL 是否支持 SM2
    /// </summary>
    public async Task<bool> IsSm2SupportedAsync(CancellationToken cancellationToken = default)
    {
        var result = await this.ExecuteAsync("ecparam -list_curves", cancellationToken: cancellationToken);
        return result.StandardOutput.Contains("SM2", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 检查 OpenSSL 是否支持 SM3
    /// </summary>
    public async Task<bool> IsSm3SupportedAsync(CancellationToken cancellationToken = default)
    {
        var result = await this.ExecuteAsync("dgst -list", cancellationToken: cancellationToken);
        return result.StandardOutput.Contains("sm3", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 检查 OpenSSL 是否支持 SM4
    /// </summary>
    public async Task<bool> IsSm4SupportedAsync(CancellationToken cancellationToken = default)
    {
        var result = await this.ExecuteAsync("enc -list", cancellationToken: cancellationToken);
        return result.StandardOutput.Contains("sm4", StringComparison.OrdinalIgnoreCase);
    }

    #endregion
}

/// <summary>
/// OpenSSL 命令执行结果
/// </summary>
public class OpenSslResult
{
    /// <summary>
    /// 退出码
    /// </summary>
    public int ExitCode { get; init; }

    /// <summary>
    /// 标准输出
    /// </summary>
    public string StandardOutput { get; init; } = string.Empty;

    /// <summary>
    /// 标准错误
    /// </summary>
    public string StandardError { get; init; } = string.Empty;

    /// <summary>
    /// 命令是否成功执行
    /// </summary>
    public bool IsSuccess => this.ExitCode == 0;

    /// <summary>
    /// 获取完整输出（标准输出 + 标准错误）
    /// </summary>
    public string FullOutput =>
        string.IsNullOrEmpty(this.StandardError)
            ? this.StandardOutput
            : $"{this.StandardOutput}\n{this.StandardError}";
}
