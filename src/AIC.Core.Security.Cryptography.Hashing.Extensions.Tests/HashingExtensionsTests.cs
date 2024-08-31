namespace AIC.Core.Security.Cryptography.Hashing.Extensions.Tests;

using NUnit.Framework;
using Moq;
using System.Text;
using System.Threading.Tasks;
using AIC.Core.Security.Cryptography.Hashing.Contracts;

[TestFixture]
public class HashingExtensionsTests
{
    private Mock<IHashingService> mockHashingService;

    [SetUp]
    public void SetUp()
    {
        this.mockHashingService = new Mock<IHashingService>();
    }

    [Test]
    public async Task Hash_WithValidString_ShouldReturnBase64EncodedHash()
    {
        // Arrange
        var data = "Hello, World!";
        var expectedHash = new byte[] { 0x01, 0x02, 0x03 }; // Example byte array

        this.mockHashingService
            .Setup(s => s.HashAsync(It.IsAny<byte[]>()))
            .ReturnsAsync(expectedHash);

        // Act
        var result = await this.mockHashingService.Object.Hash(data);

        // Assert
        var expectedBase64Hash = Convert.ToBase64String(expectedHash);
        Assert.AreEqual(expectedBase64Hash, result);
    }

    [Test]
    public async Task Hash_WithEmptyString_ShouldReturnBase64EncodedHash()
    {
        // Arrange
        var data = string.Empty;
        var expectedHash = new byte[] { 0x04, 0x05, 0x06 }; // Example byte array

        this.mockHashingService
            .Setup(s => s.HashAsync(It.IsAny<byte[]>()))
            .ReturnsAsync(expectedHash);

        // Act
        var result = await this.mockHashingService.Object.Hash(data);

        // Assert
        var expectedBase64Hash = Convert.ToBase64String(expectedHash);
        Assert.AreEqual(expectedBase64Hash, result);
    }

    [Test]
    public async Task Hash_WithNullString_ShouldThrowArgumentNullException()
    {
        // Arrange
        string data = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => this.mockHashingService.Object.Hash(data));
    }

    [Test]
    public async Task Hash_WithDifferentStrings_ShouldReturnDifferentBase64EncodedHashes()
    {
        // Arrange
        var data1 = "String1";
        var data2 = "String2";

        var hash1 = new byte[] { 0x01, 0x02, 0x03 }; // Example byte array for string1
        var hash2 = new byte[] { 0x07, 0x08, 0x09 }; // Example byte array for string2

        this.mockHashingService
            .SetupSequence(s => s.HashAsync(It.IsAny<byte[]>()))
            .ReturnsAsync(hash1)
            .ReturnsAsync(hash2);

        // Act
        var result1 = await this.mockHashingService.Object.Hash(data1);
        var result2 = await this.mockHashingService.Object.Hash(data2);

        // Assert
        var expectedBase64Hash1 = Convert.ToBase64String(hash1);
        var expectedBase64Hash2 = Convert.ToBase64String(hash2);

        Assert.AreEqual(expectedBase64Hash1, result1);
        Assert.AreEqual(expectedBase64Hash2, result2);
        Assert.AreNotEqual(result1, result2);
    }
}
