namespace AIC.Core.Security.Cryptography.Asymmetric.RSA.Implementations.Algorithms;

using System.Security.Cryptography;
using System.Text;
using AIC.Core.Security.Cryptography.Asymmetric.Contracts;
using AIC.Core.Security.Cryptography.Asymmetric.Extensions;
using AIC.Core.Security.Cryptography.Asymmetric.Implementations;
using Newtonsoft.Json;

public class RsaAsymmetricCryptographyProvider : IAsymmetricCryptographyProvider
{
    public (string privateKeyParametersJson, string publicKeyParametersJson) GenerateKeyPair(int keySize)
    {
        using var rsa = RSA.Create();
        rsa.KeySize = keySize;

        var privateKeyParameters = rsa.ExportParameters(true).ToPrivateKeyParameters();
        var publicKeyParameters = rsa.ExportParameters(false).ToPublicKeyParameters();

        var privateKeyParametersJson = JsonConvert.SerializeObject(privateKeyParameters);
        var publicKeyParametersJson = JsonConvert.SerializeObject(publicKeyParameters);
        return (privateKeyParametersJson, publicKeyParametersJson);
    }

    public (string privateKeyParametersJson, string publicKeyParametersJson) GenerateKeyPair(int keySize, byte[] seed)
    {
        throw new NotImplementedException();
    }

    public (byte[] privateKeyParameters, byte[] publicKeyParameters) GenerateKeyPair()
    {
        throw new NotImplementedException();
    }

    public (byte[] privateKeyParameters, byte[] publicKeyParameters) GenerateKeyPair(byte[] seed)
    {
        throw new NotImplementedException();
    }

    public byte[] Encrypt(byte[] data, string publicKey)
    {
        throw new NotImplementedException();
    }

    public byte[] Decrypt(byte[] data, string privateKey)
    {
        throw new NotImplementedException();
    }

    public byte[] Encrypt(byte[] data, byte[] publicKey)
    {
        throw new NotImplementedException();
    }

    public byte[] Decrypt(byte[] data, byte[] privateKey)
    {
        throw new NotImplementedException();
    }

    public string Encrypt(string plainText, string publicKeyJson)
    {
        using var rsa = RSA.Create();
        var rsaParameters = JsonConvert.DeserializeObject<AsymmetricPublicKeyParameters>(publicKeyJson)
            .ToRsaParameters();
        rsa.ImportParameters(rsaParameters);

        var dataToEncrypt = Encoding.UTF8.GetBytes(plainText);
        var encryptedData = rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.OaepSHA256);
        return Convert.ToBase64String(encryptedData);
    }

    public string Decrypt(string encryptedData, string privateKeyJson)
    {
        using var rsa = RSA.Create();
        var rsaParameters = JsonConvert.DeserializeObject<AsymmetricPrivateKeyParameters>(privateKeyJson)
            .ToRsaParameters();
        rsa.ImportParameters(rsaParameters);

        var dataToDecrypt = Convert.FromBase64String(encryptedData);
        var decryptedData = rsa.Decrypt(dataToDecrypt, RSAEncryptionPadding.OaepSHA256);
        return Encoding.UTF8.GetString(decryptedData);
    }

    public byte[] Sign(byte[] data, byte[] privateKey)
    {
        throw new NotImplementedException();
    }

    public byte[] Sign(byte[] data, string privateKey)
    {
        throw new NotImplementedException();
    }

    public bool Verify(byte[] data, byte[] signature, byte[] publicKey)
    {
        throw new NotImplementedException();
    }

    public bool Verify(byte[] data, byte[] signature, string publicKey)
    {
        throw new NotImplementedException();
    }

    public string Sign(string data, string privateKeyJson)
    {
        using var rsa = RSA.Create();
        var rsaParameters = JsonConvert.DeserializeObject<AsymmetricPrivateKeyParameters>(privateKeyJson)
            .ToRsaParameters();
        rsa.ImportParameters(rsaParameters);

        var dataToSign = Encoding.UTF8.GetBytes(data);
        var signature = rsa.SignData(dataToSign, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
        return Convert.ToBase64String(signature);
    }

    public bool Verify(string data, string signature, string publicKeyJson)
    {
        using var rsa = RSA.Create();
        var rsaParameters = JsonConvert.DeserializeObject<AsymmetricPublicKeyParameters>(publicKeyJson)
            .ToRsaParameters();
        rsa.ImportParameters(rsaParameters);

        var dataToVerify = Encoding.UTF8.GetBytes(data);
        var signatureData = Convert.FromBase64String(signature);
        return rsa.VerifyData(dataToVerify, signatureData, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
    }
}