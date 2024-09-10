namespace AIC.Core.Security.Cryptography.Hashing.BouncyCastle.Tests.Implementations;

using AIC.Core.Security.Cryptography.Hashing.BouncyCastle.Implementations;
using NUnit.Framework;
using Org.BouncyCastle.Crypto.Digests;
using System.Text;
using System.Threading.Tasks;

[TestFixture]
public class Sha3HashingServiceTests
{
    private Sha3HashingService hashingService;

    [SetUp]
    public void SetUp()
    {
        this.hashingService = new Sha3HashingService();
    }

    [Test]
    public async Task HashAsync_WithValidData_ShouldReturnCorrectHashLength()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Hello, World!");

        // Act
        var hash = await this.hashingService.HashAsync(data);

        // Assert
        Assert.IsNotNull(hash);
        Assert.AreEqual(64, hash.Length); // 512 bits / 8 = 64 bytes
    }

    [Test]
    public async Task HashAsync_WithEmptyData_ShouldReturnCorrectHashLength()
    {
        // Arrange
        var data = new byte[0];

        // Act
        var hash = await this.hashingService.HashAsync(data);

        // Assert
        Assert.IsNotNull(hash);
        Assert.AreEqual(64, hash.Length); // 512 bits / 8 = 64 bytes
    }

    [Test]
    public async Task HashAsync_WithSameData_ShouldReturnSameHash()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Same data");

        // Act
        var hash1 = await this.hashingService.HashAsync(data);
        var hash2 = await this.hashingService.HashAsync(data);

        // Assert
        Assert.AreEqual(hash1, hash2);
    }

    [Test]
    public async Task HashAsync_WithDifferentData_ShouldReturnDifferentHashes()
    {
        // Arrange
        var data1 = Encoding.UTF8.GetBytes("Data 1");
        var data2 = Encoding.UTF8.GetBytes("Data 2");

        // Act
        var hash1 = await this.hashingService.HashAsync(data1);
        var hash2 = await this.hashingService.HashAsync(data2);

        // Assert
        Assert.AreNotEqual(hash1, hash2);
    }

    [Test]
    public async Task HashAsync_WithLargeData_ShouldReturnCorrectHashLength()
    {
        // Arrange
        var data = new byte[10000]; // Large data array
        new Random().NextBytes(data); // Fill with random bytes

        // Act
        var hash = await this.hashingService.HashAsync(data);

        // Assert
        Assert.IsNotNull(hash);
        Assert.AreEqual(64, hash.Length); // 512 bits / 8 = 64 bytes
    }
}
