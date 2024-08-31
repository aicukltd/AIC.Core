using AIC.Core.Security.Cryptography.Streams.Contracts;

namespace AIC.Core.Security.Cryptography.Streams.Implementations;

public class AsyncStream : IAsyncStream
{
    private Stream stream = null!;

    public void SetStream(Stream inputStream)
    {
        this.stream = inputStream;
    }

    protected void ValidateStreamOperation()
    {
        if (this.stream == null) throw new InvalidOperationException("The stream is null.");
    }

    public async Task<int> ReadAsync(byte[] buffer, int count)
    {
        this.ValidateStreamOperation();

        if (buffer == null) throw new ArgumentNullException(nameof(buffer));
        if (count < 0 || count > buffer.Length) throw new ArgumentOutOfRangeException(nameof(count));

        return await this.stream.ReadAsync(buffer, 0, count).ConfigureAwait(false);
    }

    public async Task<int> WriteAsync(byte[] buffer, int count)
    {
        this.ValidateStreamOperation();

        if (buffer == null) throw new ArgumentNullException(nameof(buffer));
        if (count < 0 || count > buffer.Length) throw new ArgumentOutOfRangeException(nameof(count));

        await this.stream.WriteAsync(buffer, 0, count).ConfigureAwait(false);
        return count;
    }

    public async Task<long> SeekAsync(long offset, SeekOrigin origin)
    {
        this.ValidateStreamOperation();

        return await Task.Run(() => this.stream.Seek(offset, origin)).ConfigureAwait(false);
    }

    public async Task SetSizeAsync(long newSize)
    {
        this.ValidateStreamOperation();

        if (this.stream.CanWrite)
        {
            await Task.Run(() => this.stream.SetLength(newSize)).ConfigureAwait(false);
        }
        else
        {
            throw new NotSupportedException("Stream does not support writing.");
        }
    }

    public async Task<(long bytesRead, long bytesWritten)> CopyToAsync(IAsyncStream destination, long count)
    {
        this.ValidateStreamOperation();

        if (destination == null) throw new ArgumentNullException(nameof(destination));

        var buffer = new byte[81920]; // 80KB buffer
        long totalBytesRead = 0;
        long totalBytesWritten = 0;

        while (count > 0)
        {
            var bytesToRead = (int)Math.Min(buffer.Length, count);
            var bytesRead = await this.stream.ReadAsync(buffer, 0, bytesToRead).ConfigureAwait(false);
            if (bytesRead == 0) break;

            await destination.WriteAsync(buffer, bytesRead).ConfigureAwait(false);
            totalBytesRead += bytesRead;
            totalBytesWritten += bytesRead;
            count -= bytesRead;
        }

        return (totalBytesRead, totalBytesWritten);
    }

    public async Task CommitAsync(int commitFlags)
    {
        this.ValidateStreamOperation();

        if (this.stream.CanWrite)
        {
            await Task.Run(() => this.stream.FlushAsync()).ConfigureAwait(false);
        }
        else
        {
            throw new NotSupportedException("Stream does not support writing.");
        }
    }

    public async Task RevertAsync()
    {
        this.ValidateStreamOperation();

        // For most streams, there isn't an explicit "revert" operation.
        // If the stream is transactional, you would implement this accordingly.
        // Here, we'll simply ensure that the stream is flushed if it's writable.
        if (this.stream.CanWrite)
        {
            await Task.Run(() => this.stream.FlushAsync()).ConfigureAwait(false);
        }
    }

    public async Task LockRegionAsync(long offset, long count, int lockType)
    {
        this.ValidateStreamOperation();

        // .NET streams do not support locking regions by default.
        // Implement if necessary, using a derived stream or other mechanism.
        await Task.CompletedTask;
        throw new NotSupportedException("Locking is not supported by this stream.");
    }

    public async Task UnlockRegionAsync(long offset, long count, int lockType)
    {
        this.ValidateStreamOperation();

        // .NET streams do not support unlocking regions by default.
        // Implement if necessary, using a derived stream or other mechanism.
        await Task.CompletedTask;
        throw new NotSupportedException("Unlocking is not supported by this stream.");
    }

    public async Task<IAsyncStream> CloneAsync()
    {
        this.ValidateStreamOperation();

        // Cloning is not supported for general streams.
        // Implement this method if using a specific stream that can be cloned.
        await Task.CompletedTask;
        throw new NotSupportedException("Cloning is not supported by this stream.");
    }
}