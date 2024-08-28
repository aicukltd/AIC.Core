namespace AIC.Core.Security.Cryptography.Asymmetric.Quantum.BouncyCastle.Implementations;

using AIC.Core.Security.Cryptography.Asymmetric.Contracts;
using Org.BouncyCastle.Crypto;

public abstract class
    BaseAsymmetricCryptographyProvider<TAsymmetricCipherKeyPairGenerator> : IAsymmetricCryptographyProvider
    where TAsymmetricCipherKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
{
    public virtual (string privateKeyParametersJson, string publicKeyParametersJson) GenerateKeyPair(int keySize)
    {
        var keyPair = this.GenerateKeyPair();

        return
        (
            Convert.ToBase64String(keyPair.privateKeyParameters),
            Convert.ToBase64String(keyPair.publicKeyParameters)
        );
    }

    public virtual (string privateKeyParametersJson, string publicKeyParametersJson) GenerateKeyPair(int keySize,
        byte[] seed)
    {
        return this.GenerateKeyPair(keySize);
    }

    public abstract (byte[] privateKeyParameters, byte[] publicKeyParameters) GenerateKeyPair();


    public virtual (byte[] privateKeyParameters, byte[] publicKeyParameters) GenerateKeyPair(byte[] seed)
    {
        return this.GenerateKeyPair();
    }

    public virtual byte[] Encrypt(byte[] data, string publicKey)
    {
        throw new NotImplementedException();
    }

    public virtual byte[] Decrypt(byte[] data, string privateKey)
    {
        throw new NotImplementedException();
    }

    public virtual byte[] Encrypt(byte[] data, byte[] publicKey)
    {
        throw new NotImplementedException();
    }

    public virtual byte[] Decrypt(byte[] data, byte[] privateKey)
    {
        throw new NotImplementedException();
    }

    public virtual string Encrypt(string plainText, string publicKeyJson)
    {
        throw new NotImplementedException();
    }

    public virtual string Decrypt(string encryptedData, string privateKeyJson)
    {
        throw new NotImplementedException();
    }

    public virtual byte[] Sign(byte[] data, byte[] privateKey)
    {
        return null;
    }

    public virtual byte[] Sign(byte[] data, string privateKey)
    {
        var rawKey = Convert.FromBase64String(privateKey);

        return this.Sign(data, rawKey);
    }

    public virtual bool Verify(byte[] data, byte[] signature, byte[] publicKey)
    {
        return false;
    }

    public virtual bool Verify(byte[] data, byte[] signature, string publicKey)
    {
        var rawKey = Convert.FromBase64String(publicKey);

        return this.Verify(data, signature, rawKey);
    }

    public virtual string Sign(string data, string privateKey)
    {
        return Convert.ToBase64String(this.Sign(Convert.FromBase64String(data), privateKey));
    }

    public virtual bool Verify(string data, string signature, string publicKey)
    {
        return this.Verify(Convert.FromBase64String(data), Convert.FromBase64String(signature), publicKey);
    }

    protected abstract TAsymmetricCipherKeyPairGenerator InitialiseAlgorithm();
}