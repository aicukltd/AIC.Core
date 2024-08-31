namespace AIC.Core.Security.Cryptography.Asymmetric.Certificates.Quantum.BouncyCastle.Tests.Implementations;

using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Contracts;
using AIC.Core.Security.Cryptography.Asymmetric.Certificates.Quantum.BouncyCastle.Contracts;
using AIC.Core.Security.Cryptography.Asymmetric.Certificates.Quantum.BouncyCastle.Implementations;
using AIC.Core.Security.Cryptography.Asymmetric.Quantum.BouncyCastle.Contracts;
using Org.BouncyCastle.Asn1.X509;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

[TestFixture]
public class DilithiumBouncyCastleAsymmetricCertificateProviderTests
{
    private Mock<IDilithiumAsymmetricCryptographyProvider> mockDilithiumAsymmetricCryptographyProvider;
    private Mock<IDilithiumAsymmetricCertificateProvider> mockDilithiumAsymmetricCertificateProvider;
    private DilithiumBouncyCastleAsymmetricCertificateProvider provider;

    [SetUp]
    public void SetUp()
    {
        this.mockDilithiumAsymmetricCryptographyProvider = new Mock<IDilithiumAsymmetricCryptographyProvider>();
        this.mockDilithiumAsymmetricCertificateProvider = new Mock<IDilithiumAsymmetricCertificateProvider>();
        this.provider = new DilithiumBouncyCastleAsymmetricCertificateProvider(
            this.mockDilithiumAsymmetricCryptographyProvider.Object,
            this.mockDilithiumAsymmetricCertificateProvider.Object);
    }

    [Test]
    public void Constructor_WhenCalled_ShouldInitializeDependencies()
    {
        // Arrange & Act
        var result = new DilithiumBouncyCastleAsymmetricCertificateProvider(
            this.mockDilithiumAsymmetricCryptographyProvider.Object,
            this.mockDilithiumAsymmetricCertificateProvider.Object);

        // Assert
        Assert.IsNotNull(result);
    }

    [Test]
    public void Constructor_WhenCryptographyProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new DilithiumBouncyCastleAsymmetricCertificateProvider(null, this.mockDilithiumAsymmetricCertificateProvider.Object));
    }

    [Test]
    public void Constructor_WhenCertificateProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new DilithiumBouncyCastleAsymmetricCertificateProvider(this.mockDilithiumAsymmetricCryptographyProvider.Object, null));
    }

    [Test]
    public async Task FromCertificateAsync_WhenCertificateIsProvided_ShouldReturnIX509Certificate3()
    {
        // Arrange
        var certificate = CreateSampleBouncyCastleCertificate();
        var x509Certificate2 = new X509Certificate2(certificate.GetEncoded());
        this.mockDilithiumAsymmetricCertificateProvider
            .Setup(p => p.FromCertificateAsync(It.IsAny<X509Certificate2>()))
            .ReturnsAsync(Mock.Of<IX509Certificate3>());

        // Act
        var result = await this.provider.FromCertificateAsync(certificate);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<IX509Certificate3>(result);
        this.mockDilithiumAsymmetricCertificateProvider.Verify(p => p.FromCertificateAsync(It.Is<X509Certificate2>(c => c.RawData.SequenceEqual(x509Certificate2.RawData))), Times.Once);
    }

    [Test]
    public async Task FromCertificateAsync_WhenCertificateIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await this.provider.FromCertificateAsync(null));
    }

    private X509Certificate CreateSampleBouncyCastleCertificate()
    {
        // You can use a mock or a real X509Certificate for this test case
        return new X509Certificate(new byte[]{}); // Assuming it returns a valid X509Certificate for testing purposes
    }
}
