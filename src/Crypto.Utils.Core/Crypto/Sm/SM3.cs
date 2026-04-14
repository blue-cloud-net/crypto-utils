using System.Buffers;
using System.Diagnostics;

namespace Crypto.Utils.Crypto.Sm;

/// <summary>
/// SM3 哈希算法实现，基于 BouncyCastle
/// </summary>
public sealed class SM3 : HashAlgorithm
{

    /// <summary>
    /// SM3 算法产生的哈希大小（字节）
    /// </summary>
    public const int HashSizeInBytes = 32;

    /// <summary>
    /// SM3 算法产生的哈希大小（位）
    /// </summary>
    public const int HashSizeInBits = HashSizeInBytes * 8;

    private SM3Digest _digest;

    #region 构造函数

    /// <summary>
    /// 初始化 SM3 的新实例
    /// </summary>
    public SM3()
    {
        _digest = new SM3Digest();
        HashSizeValue = HashSizeInBits;
    }

    #endregion

    #region 属性

    /// <summary>
    /// 获取输入块的大小（字节）
    /// </summary>
    public override int InputBlockSize => 64;

    /// <summary>
    /// 获取输出块的大小（字节）
    /// </summary>
    public override int OutputBlockSize => HashSizeInBytes;

    #endregion

    #region 核心方法（重写基类）

    /// <summary>
    /// 初始化哈希算法
    /// </summary>
    public override void Initialize() => _digest.Reset();

    /// <summary>
    /// 计算输入数据的哈希值
    /// </summary>
    protected override void HashCore(byte[] array, int ibStart, int cbSize) =>
        _digest.BlockUpdate(array, ibStart, cbSize);

    /// <summary>
    /// 支持 Span 的哈希核心方法
    /// </summary>
    protected override void HashCore(ReadOnlySpan<byte> source)
    {
        if (source.Length == 0)
            return;

        var pool = ArrayPool<byte>.Shared;
        var data = pool.Rent(source.Length);
        try
        {
            source.CopyTo(data);
            _digest.BlockUpdate(data, 0, source.Length);
        }
        finally
        {
            pool.Return(data);
        }
    }

    /// <summary>
    /// 完成哈希计算并返回结果
    /// </summary>
    protected override byte[] HashFinal()
    {
        var result = new byte[this.OutputBlockSize];
        _ = _digest.DoFinal(result, 0);
        return result;
    }

    /// <summary>
    /// 尝试完成哈希计算
    /// </summary>
    protected override bool TryHashFinal(Span<byte> destination, out int bytesWritten)
    {
        if (destination.Length < this.OutputBlockSize)
        {
            bytesWritten = 0;
            return false;
        }

        _ = _digest.DoFinal(destination);
        bytesWritten = HashSizeInBytes;
        return true;
    }

    #endregion

    #region 资源管理

    /// <summary>
    /// 释放资源
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _digest = null!;
        }
        base.Dispose(disposing);
    }

    #endregion

    #region 静态工厂方法

    /// <summary>
    /// 创建 SM3 的默认实现实例
    /// </summary>
    /// <returns>SM3 的新实例</returns>
    public static new SM3 Create() => new();

    #endregion

    #region 静态方法

    /// <summary>
    /// 使用 SM3 算法计算数据的哈希值
    /// </summary>
    /// <param name="source">要哈希的数据</param>
    /// <returns>哈希值</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="source" /> 为 <see langword="null" />
    /// </exception>
    public static byte[] HashData(byte[] source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return HashData(new ReadOnlySpan<byte>(source));
    }

    /// <summary>
    /// 使用 SM3 算法计算数据的哈希值
    /// </summary>
    /// <param name="source">要哈希的数据</param>
    /// <returns>哈希值</returns>
    public static byte[] HashData(ReadOnlySpan<byte> source)
    {
        var buffer = GC.AllocateUninitializedArray<byte>(HashSizeInBytes);

        var written = HashData(source, buffer.AsSpan());
        Debug.Assert(written == buffer.Length);

        return buffer;
    }

    /// <summary>
    /// 使用 SM3 算法计算数据的哈希值
    /// </summary>
    /// <param name="source">要哈希的数据</param>
    /// <param name="destination">接收哈希值的缓冲区</param>
    /// <returns>写入 <paramref name="destination" /> 的总字节数</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="destination"/> 中的缓冲区太小，无法容纳计算的哈希大小。
    /// SM3 算法始终产生 256 位哈希，即 32 字节。
    /// </exception>
    public static int HashData(ReadOnlySpan<byte> source, Span<byte> destination)
    {
        if (!TryHashData(source, destination, out var bytesWritten))
            throw new ArgumentException("Destination buffer is too short.", nameof(destination));

        return bytesWritten;
    }

    /// <summary>
    /// 尝试使用 SM3 算法计算数据的哈希值
    /// </summary>
    /// <param name="source">要哈希的数据</param>
    /// <param name="destination">接收哈希值的缓冲区</param>
    /// <param name="bytesWritten">
    /// 此方法返回时，包含写入 <paramref name="destination"/> 的总字节数
    /// </param>
    /// <returns>
    /// 如果 <paramref name="destination"/> 太小无法容纳计算的哈希，则为 <see langword="false"/>，
    /// 否则为 <see langword="true"/>
    /// </returns>
    public static bool TryHashData(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten)
    {
        if (destination.Length < HashSizeInBytes)
        {
            bytesWritten = 0;
            return false;
        }

        var digest = new SM3Digest();

        if (source.Length > 0)
        {
            var pool = ArrayPool<byte>.Shared;
            var sourceArray = pool.Rent(source.Length);
            try
            {
                source.CopyTo(sourceArray);
                digest.BlockUpdate(sourceArray, 0, source.Length);
            }
            finally
            {
                pool.Return(sourceArray);
            }
        }

        bytesWritten = digest.DoFinal(destination);
        Debug.Assert(bytesWritten == HashSizeInBytes);

        return true;
    }

    /// <summary>
    /// 使用 SM3 算法计算流的哈希值
    /// </summary>
    /// <param name="source">要哈希的流</param>
    /// <param name="destination">接收哈希值的缓冲区</param>
    /// <returns>写入 <paramref name="destination" /> 的总字节数</returns>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="source" /> 为 <see langword="null" />
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <p>
    ///   <paramref name="destination"/> 中的缓冲区太小，无法容纳计算的哈希大小。
    ///   SM3 算法始终产生 256 位哈希，即 32 字节。
    ///   </p>
    ///   <p>-或-</p>
    ///   <p>
    ///   <paramref name="source" /> 不支持读取。
    ///   </p>
    /// </exception>
    public static int HashData(Stream source, Span<byte> destination)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (destination.Length < HashSizeInBytes)
            throw new ArgumentException("Destination buffer is too short.", nameof(destination));

        if (!source.CanRead)
            throw new ArgumentException("Stream does not support reading.", nameof(source));

        var digest = new SM3Digest();
        var pool = ArrayPool<byte>.Shared;
        var buffer = pool.Rent(4096);
        try
        {
            int bytesRead;
            while ((bytesRead = source.Read(buffer, 0, 4096)) > 0)
            {
                digest.BlockUpdate(buffer, 0, bytesRead);
            }

            return digest.DoFinal(destination);
        }
        finally
        {
            pool.Return(buffer);
        }
    }

    /// <summary>
    /// 使用 SM3 算法计算流的哈希值
    /// </summary>
    /// <param name="source">要哈希的流</param>
    /// <returns>哈希值</returns>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="source" /> 为 <see langword="null" />
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <paramref name="source" /> 不支持读取。
    /// </exception>
    public static byte[] HashData(Stream source)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (!source.CanRead)
            throw new ArgumentException("Stream does not support reading.", nameof(source));

        var buffer = new byte[HashSizeInBytes];
        HashData(source, buffer);
        return buffer;
    }

    /// <summary>
    /// 异步计算流的 SM3 哈希值
    /// </summary>
    /// <param name="source">要哈希的流</param>
    /// <param name="cancellationToken">
    ///   用于监视取消请求的令牌。
    ///   默认值为 <see cref="System.Threading.CancellationToken.None" />
    /// </param>
    /// <returns>哈希值</returns>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="source" /> 为 <see langword="null" />
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <paramref name="source" /> 不支持读取。
    /// </exception>
    public static async ValueTask<byte[]> HashDataAsync(Stream source, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (!source.CanRead)
            throw new ArgumentException("Stream does not support reading.", nameof(source));

        var digest = new SM3Digest();
        var pool = ArrayPool<byte>.Shared;
        var buffer = pool.Rent(4096);
        try
        {
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer.AsMemory(0, 4096), cancellationToken).ConfigureAwait(false)) > 0)
            {
                digest.BlockUpdate(buffer, 0, bytesRead);
            }

            var result = new byte[HashSizeInBytes];
            digest.DoFinal(result, 0);
            return result;
        }
        finally
        {
            pool.Return(buffer);
        }
    }

    /// <summary>
    /// 异步计算流的 SM3 哈希值
    /// </summary>
    /// <param name="source">要哈希的流</param>
    /// <param name="destination">接收哈希值的缓冲区</param>
    /// <param name="cancellationToken">
    ///   用于监视取消请求的令牌。
    ///   默认值为 <see cref="System.Threading.CancellationToken.None" />
    /// </param>
    /// <returns>写入 <paramref name="destination" /> 的总字节数</returns>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="source" /> 为 <see langword="null" />
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <p>
    ///   <paramref name="destination"/> 中的缓冲区太小，无法容纳计算的哈希大小。
    ///   SM3 算法始终产生 256 位哈希，即 32 字节。
    ///   </p>
    ///   <p>-或-</p>
    ///   <p>
    ///   <paramref name="source" /> 不支持读取。
    ///   </p>
    /// </exception>
    public static async ValueTask<int> HashDataAsync(
        Stream source,
        Memory<byte> destination,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (destination.Length < HashSizeInBytes)
            throw new ArgumentException("Destination buffer is too short.", nameof(destination));

        if (!source.CanRead)
            throw new ArgumentException("Stream does not support reading.", nameof(source));

        var digest = new SM3Digest();
        var pool = ArrayPool<byte>.Shared;
        var buffer = pool.Rent(4096);
        try
        {
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer.AsMemory(0, 4096), cancellationToken).ConfigureAwait(false)) > 0)
            {
                digest.BlockUpdate(buffer, 0, bytesRead);
            }

            return digest.DoFinal(destination.Span);
        }
        finally
        {
            pool.Return(buffer);
        }
    }

    #endregion
}
