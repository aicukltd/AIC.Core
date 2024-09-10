namespace AIC.Core.Security.Cryptography.Symmetric.AES.Tests.Implementations.Algorithms;

using AIC.Core.Security.Cryptography.Symmetric.AES.Implementations.Algorithms;
using NUnit.Framework;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Text;

[TestFixture]
public class AesBouncyCastleSymmetricCryptographyProviderTests
{
    private AesBouncyCastleSymmetricCryptographyProvider provider;

    [SetUp]
    public void SetUp()
    {
        this.provider = new AesBouncyCastleSymmetricCryptographyProvider();
    }

    [Test]
    public void Encrypt_Decrypt_WithByteArray_ShouldReturnOriginalData()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Test data");
        var key = new byte[16];
        var iv = new byte[16];
        new SecureRandom().NextBytes(key);
        new SecureRandom().NextBytes(iv);

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
        var key = new byte[16];
        new SecureRandom().NextBytes(key);

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
        var key = new byte[16];
        new SecureRandom().NextBytes(key);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => this.provider.Encrypt(plainText, key));
    }

    [Test]
    public void Decrypt_WithNullData_ShouldThrowArgumentNullException()
    {
        // Arrange
        string cipherText = null;
        var key = new byte[16];
        new SecureRandom().NextBytes(key);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => this.provider.Decrypt(cipherText, key));
    }

    [Test]
    public void Encrypt_Decrypt_WithInvalidKey_ShouldThrowCryptoException()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Test data");
        var key = new byte[16];
        var invalidKey = new byte[8];
        var iv = new byte[16];
        new SecureRandom().NextBytes(key);
        new SecureRandom().NextBytes(iv);

        // Act
        var encryptedData = this.provider.Encrypt(data, key, iv);

        // Assert
        Assert.Throws<CryptoException>(() => this.provider.Decrypt(encryptedData, invalidKey, iv));
    }

    [Test]
    public void Encrypt_Decrypt_WithDifferentIv_ShouldNotReturnOriginalData()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Test data");
        var key = new byte[16];
        var iv1 = new byte[16];
        var iv2 = new byte[16];
        new SecureRandom().NextBytes(key);
        new SecureRandom().NextBytes(iv1);
        new SecureRandom().NextBytes(iv2);

        // Act
        var encryptedData = this.provider.Encrypt(data, key, iv1);
        var decryptedData = this.provider.Decrypt(encryptedData, key, iv2);

        // Assert
        Assert.AreNotEqual(data, decryptedData);
    }

    [Test]
    public void CreateKeyParameters_WithUnsupportedCipherMode_ShouldThrowException()
    {
        // Arrange
        var key = new byte[16];
        var iv = new byte[16];
        var macSize = 128;

        // Act & Assert
        var ex = Assert.Throws<Exception>(() =>
            this.provider.GetType()
                .GetMethod("CreateKeyParameters",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(this.provider, new object[] { key, iv, macSize }));

        Assert.AreEqual("Unsupported cipher mode", ex.Message);
    }

    [Test]
    public void PackCipherData_UnpackCipherData_ShouldReturnOriginalData()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Test data");
        var iv = new byte[16];
        new SecureRandom().NextBytes(iv);

        var packCipherDataMethod = this.provider.GetType().GetMethod("PackCipherData",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var unpackCipherDataMethod = this.provider.GetType().GetMethod("UnpackCipherData",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Act
        var packedData = (string)packCipherDataMethod.Invoke(this.provider, new object[] { data, iv });
        var (unpackedData, unpackedIv, _) =
            ((byte[], byte[], byte))unpackCipherDataMethod.Invoke(this.provider, new object[] { packedData });

        // Assert
        Assert.AreEqual(data, unpackedData);
        Assert.AreEqual(iv, unpackedIv);
    }
}

