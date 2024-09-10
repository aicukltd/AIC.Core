using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIC.Core.Security.Cryptography.Streams.Contracts
{
    using System.Runtime.InteropServices.ComTypes;
    using System.Security.Cryptography;

    public interface ICryptographicAsyncStream : IAsyncStream
    {
        /// <summary>
        /// Initializes the stream for encryption, preparing it to encrypt data as it is written.
        /// </summary>
        /// <param name="encryptionKey">The key used for encrypting the data.</param>
        /// <param name="signingPrivateKey">The private key used for signing the data.</param>
        /// <returns>A task that represents the asynchronous initialization operation.</returns>
        Task InitializeEncryptionAsync(byte[] encryptionKey, byte[] signingPrivateKey);

        /// <summary>
        /// Initializes the stream for decryption, preparing it to decrypt data as it is read.
        /// </summary>
        /// <param name="decryptionKey">The key used for decrypting the data.</param>
        /// <param name="signingPublicKey">The public key used for verifying the data signature.</param>
        /// <returns>A task that represents the asynchronous initialization operation.</returns>
        Task InitializeDecryptionAsync(byte[] decryptionKey, byte[] signingPublicKey);

        /// <summary>
        /// Finalizes the encryption process, ensuring all data is encrypted and the signature is generated.
        /// </summary>
        /// <returns>A task that represents the asynchronous finalization operation.</returns>
        Task FinalizeEncryptionAsync();

        /// <summary>
        /// Finalizes the decryption process, ensuring all data is decrypted and the signature is verified.
        /// </summary>
        /// <returns>A task that represents the asynchronous finalization operation. The task result is true if the signature is valid; otherwise, false.</returns>
        Task<bool> FinalizeDecryptionAsync();

        /// <summary>
        /// Encrypts and writes data to the stream.
        /// </summary>
        /// <param name="buffer">The buffer containing the data to encrypt and write.</param>
        /// <param name="count">The number of bytes to encrypt and write to the stream.</param>
        /// <returns>A task that represents the asynchronous write operation. The task result contains the actual number of bytes written.</returns>
        Task<int> EncryptAndWriteAsync(byte[] buffer, int count);

        /// <summary>
        /// Reads and decrypts data from the stream.
        /// </summary>
        /// <param name="buffer">The buffer that will receive the decrypted data.</param>
        /// <param name="count">The number of bytes to read and decrypt from the stream.</param>
        /// <returns>A task that represents the asynchronous read operation. The task result contains the actual number of bytes read.</returns>
        Task<int> ReadAndDecryptAsync(byte[] buffer, int count);
    }
}
