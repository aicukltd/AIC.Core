namespace AIC.Core.Security.Cryptography.Symmetric.AES.Implementations.Algorithms;

using System.Security.Cryptography;
using System.Text;
using AIC.Core.Extensions;
using AIC.Core.Security.Cryptography.Symmetric.Contracts;

public class AesSymmetricCryptographyProvider : ISymmetricCryptographyProvider
{
    public byte[] Encrypt(byte[] data, byte[] key, byte[]? iv)
    {
        using var aes = Aes.Create();

        aes.Padding = PaddingMode.PKCS7;
        aes.Mode = CipherMode.CBC;
        aes.KeySize = 256;

        aes.Key = key.Length > aes.Key.Length ? key.GetSubSetOfBytes(32, 0) : key;
        aes.IV = iv?.Length > aes.IV.Length ? iv.GetSubSetOfBytes(16, 0) : iv;


        var cryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV);

        return cryptoTransform.TransformFinalBlock(data, 0, data.Length);
    }

    public byte[] Decrypt(byte[] data, byte[] key, byte[]? iv)
    {
        using var aes = Aes.Create();

        aes.Padding = PaddingMode.PKCS7;
        aes.Mode = CipherMode.CBC;
        aes.KeySize = 256;

        aes.Key = key.Length > aes.Key.Length ? key.GetSubSetOfBytes(32, 0) : key;
        aes.IV = iv?.Length > aes.IV.Length ? iv.GetSubSetOfBytes(16, 0) : iv;

        var cryptoTransform = aes.CreateDecryptor(aes.Key, aes.IV);

        return cryptoTransform.TransformFinalBlock(data, 0, data.Length);
    }

    public string Encrypt(string plainText, byte[] key)
    {
        var clearBytes = Encoding.ASCII.GetBytes(plainText);
        var data = this.Encrypt(clearBytes, key, key);
        return Convert.ToBase64String(data);
    }

    public string Decrypt(string cipherText, byte[] key)
    {
        var data = Convert.FromBase64String(cipherText);
        var clearBytes = this.Decrypt(data, key, key);
        return Encoding.ASCII.GetString(clearBytes);
    }

    private byte[] Encrypt(string data, ICryptoTransform cryptoTransform)
    {
        if (data == null || data.Length <= 0)
            throw new ArgumentException(nameof(data));

        using var memoryStream = new MemoryStream();
        using (var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
        {
            using (var writer = new StreamWriter(cryptoStream))
            {
                writer.Write(data);
            }
        }

        return memoryStream.ToArray();
    }

    private string Decrypt(byte[] data, ICryptoTransform cryptoTransform)
    {
        if (data == null || data.Length <= 0)
            throw new ArgumentException(nameof(data));

        using var memoryStream = new MemoryStream(data);
        using var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read);
        using var reader = new StreamReader(cryptoStream);
        return reader.ReadToEnd();
    }
}