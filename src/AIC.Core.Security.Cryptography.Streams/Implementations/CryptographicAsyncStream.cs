using AIC.Core.Security.Cryptography.Streams.Contracts;

namespace AIC.Core.Security.Cryptography.Streams.Implementations;

public class CryptographicAsyncStream : AsyncStream, ICryptographicAsyncStream
{

    public async Task InitializeEncryptionAsync(byte[] encryptionKey, byte[] signingPrivateKey)
    {
        this.ValidateStreamOperation();

        throw new NotImplementedException();
    }

    public async Task InitializeDecryptionAsync(byte[] decryptionKey, byte[] signingPublicKey)
    {
        this.ValidateStreamOperation();

        throw new NotImplementedException();
    }

    public async Task FinalizeEncryptionAsync()
    {
        this.ValidateStreamOperation();

        throw new NotImplementedException();
    }

    public async Task<bool> FinalizeDecryptionAsync()
    {
        this.ValidateStreamOperation();

        throw new NotImplementedException();
    }

    public async Task<int> EncryptAndWriteAsync(byte[] buffer, int count)
    {
        this.ValidateStreamOperation();

        throw new NotImplementedException();
    }

    public async Task<int> ReadAndDecryptAsync(byte[] buffer, int count)
    {
        this.ValidateStreamOperation();

        throw new NotImplementedException();
    }
}