using System.IO;

namespace AIC.Core.Security.Cryptography.Streams.Contracts;

public interface IAsyncStream
{
    /// <summary>
    /// Sets the stream to use for the duration of the operations.
    /// </summary>
    /// <param name="stream"></param>
    void SetStream(Stream stream);

    /// <summary>
    /// Reads a specified number of bytes from the stream object into memory starting at the current seek pointer.
    /// </summary>
    /// <param name="buffer">The buffer that will receive the data read from the stream.</param>
    /// <param name="count">The number of bytes to read from the stream object.</param>
    /// <returns>A task that represents the asynchronous read operation. The task result contains the actual number of bytes read.</returns>
    Task<int> ReadAsync(byte[] buffer, int count);

    /// <summary>
    /// Writes a specified number of bytes into the stream object starting at the current seek pointer.
    /// </summary>
    /// <param name="buffer">The buffer containing the data to write to the stream.</param>
    /// <param name="count">The number of bytes to write to the stream.</param>
    /// <returns>A task that represents the asynchronous write operation. The task result contains the actual number of bytes written.</returns>
    Task<int> WriteAsync(byte[] buffer, int count);

    /// <summary>
    /// Changes the seek pointer to a new location relative to the beginning of the stream, to the end of the stream, or to the current seek pointer.
    /// </summary>
    /// <param name="offset">The displacement to add to <paramref name="origin" />.</param>
    /// <param name="origin">The origin of the seek, which can be the beginning, current position, or end of the stream.</param>
    /// <returns>A task that represents the asynchronous seek operation. The task result contains the new position in the stream.</returns>
    Task<long> SeekAsync(long offset, SeekOrigin origin);

    /// <summary>
    /// Changes the size of the stream object.
    /// </summary>
    /// <param name="newSize">The new size of the stream as a number of bytes.</param>
    /// <returns>A task that represents the asynchronous resize operation.</returns>
    Task SetSizeAsync(long newSize);

    /// <summary>
    /// Copies a specified number of bytes from the current seek pointer in the stream to the current seek pointer in another stream.
    /// </summary>
    /// <param name="destination">The destination stream.</param>
    /// <param name="count">The number of bytes to copy from the source stream.</param>
    /// <returns>A task that represents the asynchronous copy operation. The task result contains a tuple with the number of bytes read and written.</returns>
    Task<(long bytesRead, long bytesWritten)> CopyToAsync(IAsyncStream destination, long count);

    /// <summary>
    /// Ensures that any changes made to a stream object that is open in transacted mode are reflected in the parent storage.
    /// </summary>
    /// <param name="commitFlags">Flags that control how the changes for the stream object are committed.</param>
    /// <returns>A task that represents the asynchronous commit operation.</returns>
    Task CommitAsync(int commitFlags);

    /// <summary>
    /// Discards all changes that have been made to a transacted stream since the last <see cref="CommitAsync" /> call.
    /// </summary>
    /// <returns>A task that represents the asynchronous revert operation.</returns>
    Task RevertAsync();

    /// <summary>
    /// Restricts access to a specified range of bytes in the stream.
    /// </summary>
    /// <param name="offset">The byte offset for the beginning of the range.</param>
    /// <param name="count">The length of the range, in bytes, to restrict.</param>
    /// <param name="lockType">The requested restrictions on accessing the range.</param>
    /// <returns>A task that represents the asynchronous lock operation.</returns>
    Task LockRegionAsync(long offset, long count, int lockType);

    /// <summary>
    /// Removes the access restriction on a range of bytes previously restricted with the <see cref="LockRegionAsync" /> method.
    /// </summary>
    /// <param name="offset">The byte offset for the beginning of the range.</param>
    /// <param name="count">The length, in bytes, of the range to unlock.</param>
    /// <param name="lockType">The access restrictions previously placed on the range.</param>
    /// <returns>A task that represents the asynchronous unlock operation.</returns>
    Task UnlockRegionAsync(long offset, long count, int lockType);

    /// <summary>
    /// Creates a new stream object with its own seek pointer that references the same bytes as the original stream.
    /// </summary>
    /// <returns>A task that represents the asynchronous clone operation. The task result contains the new stream object.</returns>
    Task<IAsyncStream> CloneAsync();
}