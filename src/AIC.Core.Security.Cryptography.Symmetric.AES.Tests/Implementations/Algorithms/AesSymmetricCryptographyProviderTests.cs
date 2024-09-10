namespace AIC.Core.Security.Cryptography.Symmetric.AES.Tests.Implementations.Algorithms;

using AIC.Core.Security.Cryptography.Symmetric.AES.Implementations.Algorithms;
using NUnit.Framework;

using NUnit.Framework;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

[TestFixture]
public class AesSymmetricCryptographyProviderTests
{
    private AesSymmetricCryptographyProvider provider;

    [SetUp]
    public void SetUp()
    {
        this.provider = new AesSymmetricCryptographyProvider();
    }

    [Test]
    public void Encrypt_Decrypt_WithByteArray_ShouldReturnOriginalData()
    {
        // Arrange
        var data = Encoding.ASCII.GetBytes("Test data");
        var key = new byte[32];
        var iv = new byte[16];
        new Random().NextBytes(key);
        new Random().NextBytes(iv);

        // Act
        var encryptedData = this.provider.Encrypt(data, key, iv);
        var decryptedData = this.provider.Decrypt(encryptedData, key, iv);

        // Assert
        Assert.AreEqual(data, decryptedData);
    }

    [Test]
    public void Encrypt_Decrypt_WithString_ShouldReturnOriginalString()
    {
        // Arrange
        var plainText = "Hello, World!";
        var key = new byte[32];
        new Random().NextBytes(key);

        // Act
        var encryptedText = this.provider.Encrypt(plainText, key);
        var decryptedText = this.provider.Decrypt(encryptedText, key);

        // Assert
        Assert.AreEqual(plainText, decryptedText);
    }

    [Test]
    public void Encrypt_WithNullData_ShouldThrowArgumentNullException()
    {
        // Arrange
        string plainText = null;
        var key = new byte[32];
        new Random().NextBytes(key);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => this.provider.Encrypt(plainText, key));
    }

    [Test]
    public void Decrypt_WithNullData_ShouldThrowArgumentNullException()
    {
        // Arrange
        string cipherText = null;
        var key = new byte[32];
        new Random().NextBytes(key);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => this.provider.Decrypt(cipherText, key));
    }

    [Test]
    public void Encrypt_Decrypt_WithInvalidKey_ShouldThrowCryptographicException()
    {
        // Arrange
        var data = Encoding.ASCII.GetBytes("Test data");
        var key = new byte[32];
        var invalidKey = new byte[16]; // Key size is invalid
        var iv = new byte[16];
        new Random().NextBytes(key);
        new Random().NextBytes(iv);

        // Act & Assert
        Assert.Throws<CryptographicException>(() =>
        {
            var encryptedData = this.provider.Encrypt(data, key, iv);
            this.provider.Decrypt(encryptedData, invalidKey, iv);
        });
    }

    [Test]
    public void Encrypt_Decrypt_WithDifferentIv_ShouldNotReturnOriginalData()
    {
        // Arrange
        var data = Encoding.ASCII.GetBytes("Test data");
        var key = new byte[32];
        var iv1 = new byte[16];
        var iv2 = new byte[16];
        new Random().NextBytes(key);
        new Random().NextBytes(iv1);
        new Random().NextBytes(iv2);

        // Act
        var encryptedData = this.provider.Encrypt(data, key, iv1);
        var decryptedData = this.provider.Decrypt(encryptedData, key, iv2);

        // Assert
        Assert.AreNotEqual(data, decryptedData);
    }

    [Test]
    public void Encrypt_WithInvalidIv_ShouldThrowCryptographicException()
    {
        // Arrange
        var data = Encoding.ASCII.GetBytes("Test data");
        var key = new byte[32];
        var invalidIv = new byte[8]; // IV size is invalid
        new Random().NextBytes(key);

        // Act & Assert
        Assert.Throws<CryptographicException>(() =>
        {
            this.provider.Encrypt(data, key, invalidIv);
        });
    }

    [Test]
    public void Decrypt_WithInvalidIv_ShouldThrowCryptographicException()
    {
        // Arrange
        var data = Encoding.ASCII.GetBytes("Test data");
        var key = new byte[32];
        var invalidIv = new byte[8]; // IV size is invalid
        new Random().NextBytes(key);

        // Act & Assert
        Assert.Throws<CryptographicException>(() =>
        {
            this.provider.Decrypt(data, key, invalidIv);
        });
    }
}
