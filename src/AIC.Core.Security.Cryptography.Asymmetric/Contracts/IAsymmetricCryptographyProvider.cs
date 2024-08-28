namespace AIC.Core.Security.Cryptography.Asymmetric.Contracts;

public interface IAsymmetricCryptographyProvider
{
    (string privateKeyParametersJson, string publicKeyParametersJson) GenerateKeyPair(int keySize);
    (string privateKeyParametersJson, string publicKeyParametersJson) GenerateKeyPair(int keySize, byte[] seed);
    (byte[] privateKeyParameters, byte[] publicKeyParameters) GenerateKeyPair();
    (byte[] privateKeyParameters, byte[] publicKeyParameters) GenerateKeyPair(byte[] seed);
    byte[] Encrypt(byte[] data, string publicKey);
    byte[] Decrypt(byte[] data, string privateKey);
    byte[] Encrypt(byte[] data, byte[] publicKey);
    byte[] Decrypt(byte[] data, byte[] privateKey);
    string Encrypt(string data, string publicKey);
    string Decrypt(string data, string privateKey);
    byte[] Sign(byte[] data, byte[] privateKey);
    byte[] Sign(byte[] data, string privateKey);
    bool Verify(byte[] data, byte[] signature, byte[] publicKey);
    bool Verify(byte[] data, byte[] signature, string publicKey);
    string Sign(string data, string privateKey);
    bool Verify(string data, string signature, string publicKey);
}