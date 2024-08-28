namespace AIC.Core.Security.Cryptography.Symmetric.Contracts;

public interface ISymmetricCryptographyProvider
{
    byte[] Encrypt(byte[] data, byte[] key, byte[]? iv = null);
    byte[] Decrypt(byte[] data, byte[] key, byte[]? iv = null);
    string Encrypt(string data, byte[] key);
    string Decrypt(string data, byte[] key);
}