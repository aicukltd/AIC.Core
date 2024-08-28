namespace AIC.Core.Security.Cryptography.Asymmetric.RSA.Implementations.Algorithms;

using System.Text;
using AIC.Core.Security.Cryptography.Asymmetric.Contracts;
using AIC.Core.Security.Cryptography.Asymmetric.Extensions;
using AIC.Core.Security.Cryptography.Asymmetric.Implementations;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;

public class RsaBouncyCastleAsymmetricCryptographyProvider : IAsymmetricCryptographyProvider
{
    private const string Algorithm = "RSA/ECB/OAEPWithSHA256AndMGF1Padding";
    private const string SignatureAlgorithm = "SHA512WITHRSA";
    private const int DefaultRsaBlockSize = 190;

    public (string privateKeyParametersJson, string publicKeyParametersJson) GenerateKeyPair(int keySize)
    {
        var random = new SecureRandom();
        var keyGenerationParameters = new KeyGenerationParameters(random, keySize);
        var generator = new RsaKeyPairGenerator();
        generator.Init(keyGenerationParameters);

        var keyPair = generator.GenerateKeyPair();

        var privateKeyParametersJson = JsonConvert.SerializeObject(keyPair.Private.ToPrivateKeyParameters());
        var publicKeyParametersJson = JsonConvert.SerializeObject(keyPair.Public.ToPublicKeyParameters());
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
        var encryptionKey = JsonConvert.DeserializeObject<AsymmetricPublicKeyParameters>(publicKeyJson)
            .ToRsaKeyParameters();

        var cipher = CipherUtilities.GetCipher(RsaBouncyCastleAsymmetricCryptographyProvider.Algorithm);
        cipher.Init(true, encryptionKey);

        var dataToEncrypt = Encoding.UTF8.GetBytes(plainText);
        var encryptedData = this.ApplyCipher(dataToEncrypt, cipher,
            RsaBouncyCastleAsymmetricCryptographyProvider.DefaultRsaBlockSize);
        return Convert.ToBase64String(encryptedData);
    }

    public string Decrypt(string encryptedData, string privateKeyJson)
    {
        var decryptionKey = JsonConvert.DeserializeObject<AsymmetricPrivateKeyParameters>(privateKeyJson)
            .ToRsaPrivateCrtKeyParameters();

        var cipher = CipherUtilities.GetCipher(RsaBouncyCastleAsymmetricCryptographyProvider.Algorithm);
        cipher.Init(false, decryptionKey);

        var blockSize = decryptionKey.Modulus.BitLength / 8;

        var dataToDecrypt = Convert.FromBase64String(encryptedData);
        var decryptedData = this.ApplyCipher(dataToDecrypt, cipher, blockSize);
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
        var signatureKey = JsonConvert.DeserializeObject<AsymmetricPrivateKeyParameters>(privateKeyJson)
            .ToRsaPrivateCrtKeyParameters();

        var dataToSign = Encoding.UTF8.GetBytes(data);

        var signer = SignerUtilities.GetSigner(RsaBouncyCastleAsymmetricCryptographyProvider.SignatureAlgorithm);
        signer.Init(true, signatureKey);
        signer.BlockUpdate(dataToSign, 0, dataToSign.Length);

        var signature = signer.GenerateSignature();
        return Convert.ToBase64String(signature);
    }

    public bool Verify(string data, string signature, string publicKeyJson)
    {
        var signatureKey = JsonConvert.DeserializeObject<AsymmetricPublicKeyParameters>(publicKeyJson)
            .ToRsaKeyParameters();

        var dataToVerify = Encoding.UTF8.GetBytes(data);
        var binarySignature = Convert.FromBase64String(signature);

        var signer = SignerUtilities.GetSigner(RsaBouncyCastleAsymmetricCryptographyProvider.SignatureAlgorithm);
        signer.Init(false, signatureKey);
        signer.BlockUpdate(dataToVerify, 0, dataToVerify.Length);

        return signer.VerifySignature(binarySignature);
    }

    private byte[] ApplyCipher(byte[] data, IBufferedCipher cipher, int blockSize)
    {
        var inputStream = new MemoryStream(data);
        var outputBytes = new List<byte>();

        int index;
        var buffer = new byte[blockSize];
        while ((index = inputStream.Read(buffer, 0, blockSize)) > 0)
        {
            var cipherBlock = cipher.DoFinal(buffer, 0, index);
            outputBytes.AddRange(cipherBlock);
        }

        return outputBytes.ToArray();
    }
}