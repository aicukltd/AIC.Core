namespace AIC.Core.Security.Cryptography.Symmetric.Contracts;

public interface IDoubleSymmetricCryptographyProvider
{
    byte[] Encrypt(byte[] data, byte[] primaryKey, byte[] secondaryKey, byte[] primaryIv = null, byte[] secondaryIv = null);
    byte[] Decrypt(byte[] data, byte[] primaryKey, byte[] secondaryKey, byte[] primaryIv = null, byte[] secondaryIv = null);
    string Encrypt(string data, byte[] primaryKey, byte[] secondaryKey);
    string Decrypt(string data, byte[] primaryKey, byte[] secondaryKey);
}