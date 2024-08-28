namespace AIC.Core.Security.Cryptography.Symmetric.AES.Implementations.Algorithms;

using System.Text;
using AIC.Core.Security.Cryptography.Implementations;
using AIC.Core.Security.Cryptography.Symmetric.Contracts;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

public class AesBouncyCastleSymmetricCryptographyProvider : ISymmetricCryptographyProvider
{
    private const string Algorithm = "AES";

    private const byte AesIvSize = 16;
    private const byte GcmTagSize = 16; // in bytes

    private readonly string algorithm;

    private readonly CipherMode cipherMode;
    private readonly CipherPadding padding;

    public AesBouncyCastleSymmetricCryptographyProvider(CipherMode cipherMode = CipherMode.CBC,
        CipherPadding padding = CipherPadding.PKCS7PADDING)
    {
        this.cipherMode = cipherMode;
        this.padding = padding;
        this.algorithm = $"{AesBouncyCastleSymmetricCryptographyProvider.Algorithm}/{this.cipherMode}/{this.padding}";
    }

    public byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
    {
        var keyParameters =
            this.CreateKeyParameters(key, iv, AesBouncyCastleSymmetricCryptographyProvider.GcmTagSize * 8);
        var cipher = CipherUtilities.GetCipher(this.algorithm);
        cipher.Init(true, keyParameters);
        return cipher.DoFinal(data);
    }

    public byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
    {
        var keyParameters =
            this.CreateKeyParameters(key, iv, AesBouncyCastleSymmetricCryptographyProvider.GcmTagSize * 8);
        var cipher = CipherUtilities.GetCipher(this.algorithm);
        cipher.Init(false, keyParameters);
        return cipher.DoFinal(data);
    }

    public string Encrypt(string plainText, byte[] key)
    {
        var random = new SecureRandom();
        var plainTextData = Encoding.UTF8.GetBytes(plainText);
        var iv = random.GenerateSeed(AesBouncyCastleSymmetricCryptographyProvider.AesIvSize);
        var cipherText = this.Encrypt(plainTextData, key, iv);

        return this.PackCipherData(cipherText, iv);
    }

    public string Decrypt(string cipherText, byte[] key)
    {
        var (encryptedBytes, iv, tagSize) = this.UnpackCipherData(cipherText);
        var keyParameters = this.CreateKeyParameters(key, iv, tagSize * 8);
        var cipher = CipherUtilities.GetCipher(this.algorithm);
        cipher.Init(false, keyParameters);

        var decryptedData = cipher.DoFinal(encryptedBytes);
        return Encoding.UTF8.GetString(decryptedData);
    }

    private ICipherParameters CreateKeyParameters(byte[] key, byte[] iv, int macSize)
    {
        var keyParameter = ParameterUtilities.CreateKeyParameter("AES", key);
        if (this.cipherMode == CipherMode.CBC)
            return new ParametersWithIV(keyParameter, iv);
        if (this.cipherMode == CipherMode.GCM) return new AeadParameters(keyParameter, macSize, iv);

        throw new Exception("Unsupported cipher mode");
    }

    private string PackCipherData(byte[] encryptedBytes, byte[] iv)
    {
        var dataSize = encryptedBytes.Length + iv.Length + 1;
        if (this.cipherMode == CipherMode.GCM)
            dataSize += 1;

        var index = 0;
        var data = new byte[dataSize];
        data[index] = AesBouncyCastleSymmetricCryptographyProvider.AesIvSize;
        index += 1;
        if (this.cipherMode == CipherMode.GCM)
        {
            data[index] = AesBouncyCastleSymmetricCryptographyProvider.GcmTagSize;
            index += 1;
        }

        Array.Copy(iv, 0, data, index, iv.Length);
        index += iv.Length;
        Array.Copy(encryptedBytes, 0, data, index, encryptedBytes.Length);

        return Convert.ToBase64String(data);
    }

    private (byte[], byte[], byte) UnpackCipherData(string cipherText)
    {
        var index = 0;
        var cipherData = Convert.FromBase64String(cipherText);
        var ivSize = cipherData[index];
        index += 1;

        byte tagSize = 0;
        if (this.cipherMode == CipherMode.GCM)
        {
            tagSize = cipherData[index];
            index += 1;
        }

        var iv = new byte[ivSize];
        Array.Copy(cipherData, index, iv, 0, ivSize);
        index += ivSize;

        var encryptedBytes = new byte[cipherData.Length - index];
        Array.Copy(cipherData, index, encryptedBytes, 0, encryptedBytes.Length);
        return (encryptedBytes, iv, tagSize);
    }
}