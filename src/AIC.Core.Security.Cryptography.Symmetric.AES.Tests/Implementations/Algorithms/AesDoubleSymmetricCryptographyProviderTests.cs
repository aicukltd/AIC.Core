namespace AIC.Core.Security.Cryptography.Symmetric.AES.Tests.Implementations.Algorithms;

using NUnit.Framework;
using Moq;
using System.Text;
using AIC.Core.Security.Cryptography.Symmetric.AES.Implementations.Algorithms;
using AIC.Core.Security.Cryptography.Symmetric.Contracts;

[TestFixture]
public class AesDoubleSymmetricCryptographyProviderTests
{
    private AesDoubleSymmetricCryptographyProvider provider;
    private Mock<ISymmetricCryptographyProvider> primaryCryptographyProviderMock;
    private Mock<ISymmetricCryptographyProvider> secondaryCryptographyProviderMock;

    [SetUp]
    public void SetUp()
    {
        this.primaryCryptographyProviderMock = new Mock<ISymmetricCryptographyProvider>();
        this.secondaryCryptographyProviderMock = new Mock<ISymmetricCryptographyProvider>();

        this.provider = new AesDoubleSymmetricCryptographyProvider(
            this.primaryCryptographyProviderMock.Object,
            this.secondaryCryptographyProviderMock.Object);
    }

    [Test]
    public void Encrypt_WithByteArray_ShouldCallBothProviders()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Test data");
        var primaryKey = new byte[16];
        var secondaryKey = new byte[16];
        var primaryIv = new byte[16];
        var secondaryIv = new byte[16];
        var primaryEncrypted = Encoding.UTF8.GetBytes("Primary Cipher");
        var secondaryEncrypted = Encoding.UTF8.GetBytes("Secondary Cipher");

        this.primaryCryptographyProviderMock
            .Setup(p => p.Encrypt(It.IsAny<byte[]>(), primaryKey, primaryIv))
            .Returns(primaryEncrypted);

        this.secondaryCryptographyProviderMock
            .Setup(p => p.Encrypt(It.IsAny<byte[]>(), secondaryKey, secondaryIv))
            .Returns(secondaryEncrypted);

        // Act
        var result = this.provider.Encrypt(data, primaryKey, secondaryKey, primaryIv, secondaryIv);

        // Assert
        this.primaryCryptographyProviderMock.Verify(p => p.Encrypt(data, primaryKey, primaryIv), Times.Once);
        this.secondaryCryptographyProviderMock.Verify(p => p.Encrypt(primaryEncrypted, secondaryKey, secondaryIv), Times.Once);
        Assert.AreEqual(secondaryEncrypted, result);
    }

    [Test]
    public void Decrypt_WithByteArray_ShouldCallBothProviders()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Test data");
        var primaryKey = new byte[16];
        var secondaryKey = new byte[16];
        var primaryIv = new byte[16];
        var secondaryIv = new byte[16];
        var primaryDecrypted = Encoding.UTF8.GetBytes("Primary Decrypted");
        var secondaryDecrypted = Encoding.UTF8.GetBytes("Secondary Decrypted");

        this.primaryCryptographyProviderMock
            .Setup(p => p.Decrypt(It.IsAny<byte[]>(), primaryKey, primaryIv))
            .Returns(primaryDecrypted);

        this.secondaryCryptographyProviderMock
            .Setup(p => p.Decrypt(It.IsAny<byte[]>(), secondaryKey, secondaryIv))
            .Returns(secondaryDecrypted);

        // Act
        var result = this.provider.Decrypt(data, primaryKey, secondaryKey, primaryIv, secondaryIv);

        // Assert
        this.primaryCryptographyProviderMock.Verify(p => p.Decrypt(data, primaryKey, primaryIv), Times.Once);
        this.secondaryCryptographyProviderMock.Verify(p => p.Decrypt(primaryDecrypted, secondaryKey, secondaryIv), Times.Once);
        Assert.AreEqual(secondaryDecrypted, result);
    }

    [Test]
    public void Encrypt_WithString_ShouldCallBothProviders()
    {
        // Arrange
        var data = "Test data";
        var primaryKey = new byte[16];
        var secondaryKey = new byte[16];
        var primaryEncrypted = "Primary Cipher";
        var secondaryEncrypted = "Secondary Cipher";

        this.primaryCryptographyProviderMock
            .Setup(p => p.Encrypt(It.IsAny<string>(), primaryKey))
            .Returns(primaryEncrypted);

        this.secondaryCryptographyProviderMock
            .Setup(p => p.Encrypt(It.IsAny<string>(), secondaryKey))
            .Returns(secondaryEncrypted);

        // Act
        var result = this.provider.Encrypt(data, primaryKey, secondaryKey);

        // Assert
        this.primaryCryptographyProviderMock.Verify(p => p.Encrypt(data, primaryKey), Times.Once);
        this.secondaryCryptographyProviderMock.Verify(p => p.Encrypt(primaryEncrypted, secondaryKey), Times.Once);
        Assert.AreEqual(secondaryEncrypted, result);
    }

    [Test]
    public void Decrypt_WithString_ShouldCallBothProviders()
    {
        // Arrange
        var data = "Test data";
        var primaryKey = new byte[16];
        var secondaryKey = new byte[16];
        var primaryDecrypted = "Primary Decrypted";
        var secondaryDecrypted = "Secondary Decrypted";

        this.primaryCryptographyProviderMock
            .Setup(p => p.Decrypt(It.IsAny<string>(), primaryKey))
            .Returns(primaryDecrypted);

        this.secondaryCryptographyProviderMock
            .Setup(p => p.Decrypt(It.IsAny<string>(), secondaryKey))
            .Returns(secondaryDecrypted);

        // Act
        var result = this.provider.Decrypt(data, primaryKey, secondaryKey);

        // Assert
        this.primaryCryptographyProviderMock.Verify(p => p.Decrypt(data, primaryKey), Times.Once);
        this.secondaryCryptographyProviderMock.Verify(p => p.Decrypt(primaryDecrypted, secondaryKey), Times.Once);
        Assert.AreEqual(secondaryDecrypted, result);
    }
}