namespace AIC.Core.Security.Cryptography.Symmetric.AES.Implementations.Algorithms;

using AIC.Core.Security.Cryptography.Symmetric.Contracts;

public class AesBouncyCastleDoubleSymmetricCryptographyProvider : IDoubleSymmetricCryptographyProvider
{
    private readonly ISymmetricCryptographyProvider primaryCryptographyProvider;
    private readonly ISymmetricCryptographyProvider secondaryCryptographyProvider;

    public AesBouncyCastleDoubleSymmetricCryptographyProvider(
        ISymmetricCryptographyProvider primaryCryptographyProvider,
        ISymmetricCryptographyProvider secondaryCryptographyProvider)
    {
        this.primaryCryptographyProvider = primaryCryptographyProvider;
        this.secondaryCryptographyProvider = secondaryCryptographyProvider;
    }

    public byte[] Encrypt(byte[] data, byte[] primaryKey, byte[] secondaryKey, byte[] primaryIv = null, byte[] secondaryIv = null)
    {
        var primaryCipherResult = this.primaryCryptographyProvider.Encrypt(data, primaryKey, primaryIv);
        var secondaryCipherResult =
            this.secondaryCryptographyProvider.Encrypt(primaryCipherResult, secondaryKey, secondaryIv);
        return secondaryCipherResult;
    }

    public byte[] Decrypt(byte[] data, byte[] primaryKey, byte[] secondaryKey, byte[] primaryIv = null, byte[] secondaryIv = null)
    {
        var primaryCipherResult = this.primaryCryptographyProvider.Decrypt(data, primaryKey, primaryIv);
        var secondaryCipherResult =
            this.secondaryCryptographyProvider.Decrypt(primaryCipherResult, secondaryKey, secondaryIv);
        return secondaryCipherResult;
    }

    public string Encrypt(string data, byte[] primaryKey, byte[] secondaryKey)
    {
        var primaryCipherResult = this.primaryCryptographyProvider.Encrypt(data, primaryKey);
        var secondaryCipherResult = this.secondaryCryptographyProvider.Encrypt(primaryCipherResult, secondaryKey);
        return secondaryCipherResult;
    }

    public string Decrypt(string data, byte[] primaryKey, byte[] secondaryKey)
    {
        var primaryCipherResult = this.primaryCryptographyProvider.Decrypt(data, primaryKey);
        var secondaryCipherResult = this.secondaryCryptographyProvider.Decrypt(primaryCipherResult, secondaryKey);
        return secondaryCipherResult;
    }
}