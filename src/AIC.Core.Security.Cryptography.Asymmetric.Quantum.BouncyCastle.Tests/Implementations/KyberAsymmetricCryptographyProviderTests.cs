namespace AIC.Core.Security.Cryptography.Asymmetric.Quantum.BouncyCastle.Tests.Implementations;

using NUnit.Framework;
using Moq;
using System;
using AIC.Core.Security.Cryptography.Asymmetric.Quantum.BouncyCastle.Implementations;
using AIC.Core.Security.Cryptography.Symmetric.Contracts;
using Org.BouncyCastle.Pqc.Crypto.Crystals.Kyber;

[TestFixture]
public class KyberAsymmetricCryptographyProviderTests
{
    private Mock<ISymmetricCryptographyProvider> mockSymmetricCryptographyProvider;
    private KyberAsymmetricCryptographyProvider provider;

    [SetUp]
    public void SetUp()
    {
        this.mockSymmetricCryptographyProvider = new Mock<ISymmetricCryptographyProvider>();
        this.provider = new KyberAsymmetricCryptographyProvider(this.mockSymmetricCryptographyProvider.Object);
    }

    [Test]
    public void Constructor_WhenCalled_ShouldInitializeDependencies()
    {
        // Arrange & Act
        var result = new KyberAsymmetricCryptographyProvider(this.mockSymmetricCryptographyProvider.Object);

        // Assert
        Assert.IsNotNull(result);
    }

    [Test]
    public void Constructor_WhenSymmetricCryptographyProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new KyberAsymmetricCryptographyProvider(null));
    }

    [Test]
    public void GenerateKeyPair_WhenCalled_ShouldReturnValidKeyPair()
    {
        // Arrange & Act
        var result = this.provider.GenerateKeyPair();

        // Assert
        Assert.IsNotNull(result.privateKeyParameters);
        Assert.IsNotNull(result.publicKeyParameters);
        Assert.IsTrue(result.privateKeyParameters.Length > 0);
        Assert.IsTrue(result.publicKeyParameters.Length > 0);
    }

    [Test]
    public void GenerateKyberKeyPair_WhenCalled_ShouldReturnValidKyberKeyPair()
    {
        // Arrange & Act
        var result = this.provider.GenerateKyberKeyPair();

        // Assert
        Assert.IsNotNull(result.privateKeyParameters);
        Assert.IsNotNull(result.publicKeyParameters);
        Assert.IsInstanceOf<KyberPrivateKeyParameters>(result.privateKeyParameters);
        Assert.IsInstanceOf<KyberPublicKeyParameters>(result.publicKeyParameters);
    }

    [Test]
    public void Encrypt_WhenCalledWithByteArray_ShouldReturnEncryptedDataAndEncapsulatedKey()
    {
        // Arrange
        var data = new byte[] { 0x01, 0x02, 0x03 };
        var publicKey = new byte[] { 0x04, 0x05, 0x06 };
        var encapsulatedSharedKey = new byte[] { 0x07, 0x08 };

        var encryptedData = new byte[] { 0x09, 0x0A, 0x0B };

        this.mockSymmetricCryptographyProvider.Setup(s => s.Encrypt(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(encryptedData);

        // Act
        var result = this.provider.Encrypt(data, publicKey, out var encapsulatedKey);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(encryptedData, result);
        Assert.AreEqual(encapsulatedSharedKey.Length, encapsulatedKey.Length);
    }

    [Test]
    public void Decrypt_WhenCalledWithByteArray_ShouldReturnDecryptedData()
    {
        // Arrange
        var data = new byte[] { 0x01, 0x02, 0x03 };
        var privateKey = new byte[] { 0x04, 0x05, 0x06 };
        var encapsulatedSharedKey = new byte[] { 0x07, 0x08 };

        var decryptedData = new byte[] { 0x09, 0x0A, 0x0B };

        this.mockSymmetricCryptographyProvider.Setup(s => s.Decrypt(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(decryptedData);

        // Act
        var result = this.provider.Decrypt(data, privateKey, encapsulatedSharedKey);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(decryptedData, result);
    }
}
