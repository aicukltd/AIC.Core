namespace AIC.Core.Security.Cryptography.Asymmetric.RSA.Tests.Implementations.Algorithms;

using NUnit.Framework;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AIC.Core.Security.Cryptography.Asymmetric.Extensions;
using Newtonsoft.Json;
using AIC.Core.Security.Cryptography.Asymmetric.Implementations;
using AIC.Core.Security.Cryptography.Asymmetric.RSA.Implementations.Algorithms;

[TestFixture]
public class RsaBouncyCastleAsymmetricCryptographyProviderTests
{
    private RsaBouncyCastleAsymmetricCryptographyProvider provider;

    [SetUp]
    public void SetUp()
    {
        this.provider = new RsaBouncyCastleAsymmetricCryptographyProvider();
    }

    [Test]
    public void GenerateKeyPair_WithValidKeySize_ShouldReturnValidJsonKeyPair()
    {
        // Arrange
        var keySize = 2048;

        // Act
        var (privateKeyJson, publicKeyJson) = this.provider.GenerateKeyPair(keySize);

        // Assert
        Assert.IsNotNull(privateKeyJson);
        Assert.IsNotNull(publicKeyJson);

        var privateKeyParameters = JsonConvert.DeserializeObject<AsymmetricPrivateKeyParameters>(privateKeyJson);
        var publicKeyParameters = JsonConvert.DeserializeObject<AsymmetricPublicKeyParameters>(publicKeyJson);

        Assert.IsNotNull(privateKeyParameters);
        Assert.IsNotNull(publicKeyParameters);
    }

    [Test]
    public void Encrypt_WithValidPublicKey_ShouldReturnEncryptedBase64String()
    {
        // Arrange
        var keySize = 2048;
        var (_, publicKeyJson) = this.provider.GenerateKeyPair(keySize);
        var plainText = "Hello, RSA with BouncyCastle!";

        // Act
        var encryptedData = this.provider.Encrypt(plainText, publicKeyJson);

        // Assert
        Assert.IsNotNull(encryptedData);
        Assert.IsTrue(Convert.FromBase64String(encryptedData).Length > 0);
    }

    [Test]
    public void Decrypt_WithValidPrivateKey_ShouldReturnOriginalPlainText()
    {
        // Arrange
        var keySize = 2048;
        var (privateKeyJson, publicKeyJson) = this.provider.GenerateKeyPair(keySize);
        var plainText = "Hello, RSA with BouncyCastle!";
        var encryptedData = this.provider.Encrypt(plainText, publicKeyJson);

        // Act
        var decryptedData = this.provider.Decrypt(encryptedData, privateKeyJson);

        // Assert
        Assert.AreEqual(plainText, decryptedData);
    }

    [Test]
    public void Sign_WithValidPrivateKey_ShouldReturnValidSignature()
    {
        // Arrange
        var keySize = 2048;
        var (privateKeyJson, _) = this.provider.GenerateKeyPair(keySize);
        var dataToSign = "Sign this data using RSA and BouncyCastle";

        // Act
        var signature = this.provider.Sign(dataToSign, privateKeyJson);

        // Assert
        Assert.IsNotNull(signature);
        Assert.IsTrue(Convert.FromBase64String(signature).Length > 0);
    }

    [Test]
    public void Verify_WithValidSignature_ShouldReturnTrue()
    {
        // Arrange
        var keySize = 2048;
        var (privateKeyJson, publicKeyJson) = this.provider.GenerateKeyPair(keySize);
        var dataToSign = "Verify this data using RSA and BouncyCastle";
        var signature = this.provider.Sign(dataToSign, privateKeyJson);

        // Act
        var isVerified = this.provider.Verify(dataToSign, signature, publicKeyJson);

        // Assert
        Assert.IsTrue(isVerified);
    }

    [Test]
    public void Verify_WithInvalidSignature_ShouldReturnFalse()
    {
        // Arrange
        var keySize = 2048;
        var (_, publicKeyJson) = this.provider.GenerateKeyPair(keySize);
        var dataToSign = "Verify this data using RSA and BouncyCastle";
        var invalidSignature = Convert.ToBase64String(Encoding.UTF8.GetBytes("InvalidSignature"));

        // Act
        var isVerified = this.provider.Verify(dataToSign, invalidSignature, publicKeyJson);

        // Assert
        Assert.IsFalse(isVerified);
    }
}
