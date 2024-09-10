namespace AIC.Core.Security.Cryptography.Asymmetric.Certificates.Quantum.BouncyCastle.Tests.Implementations;
using NUnit.Framework;
using Moq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Contracts;
using AIC.Core.Security.Cryptography.Asymmetric.Certificates.Quantum.BouncyCastle.Implementations;
using AIC.Core.Security.Cryptography.Asymmetric.Quantum.BouncyCastle.Contracts;

[TestFixture]
public class DilithiumAsymmetricCertificateProviderTests
{
    private DilithiumAsymmetricCertificateProvider provider;

    [SetUp]
    public void SetUp()
    {
        this.provider = new DilithiumAsymmetricCertificateProvider();
    }

    [Test]
    public void Constructor_WhenCalled_ShouldInitializeDilithiumAsymmetricCryptographyProvider()
    {
        // Arrange & Act
        var result = new DilithiumAsymmetricCertificateProvider();

        // Assert
        Assert.IsNotNull(result);
    }

    [Test]
    public void Constructor_WhenDilithiumAsymmetricCryptographyProviderIsNull_ShouldNotThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.DoesNotThrow(() =>
            new DilithiumAsymmetricCertificateProvider());
    }

    [Test]
    public async Task FromCertificateAsync_WhenCertificateIsProvided_ShouldReturnIX509Certificate3()
    {
        // Arrange
        var certificate = this.CreateSampleCertificate();

        // Act
        var result = await this.provider.FromCertificateAsync(certificate);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<IX509Certificate3>(result);
    }

    [Test]
    public async Task FromCertificateAsync_WhenCertificateIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await this.provider.FromCertificateAsync(null));
    }

    [Test]
    public async Task FromCertificateAsync_WhenCalled_ShouldMapCertificateFieldsCorrectly()
    {
        // Arrange
        var certificate = this.CreateSampleCertificate();

        // Act
        var result = await this.provider.FromCertificateAsync(certificate);

        // Assert
        Assert.AreEqual(certificate.Archived, result.Archived);
        Assert.AreEqual(certificate.FriendlyName, result.FriendlyName);
        Assert.AreEqual(certificate.Handle, result.Handle);
        Assert.AreEqual(certificate.HasPrivateKey, result.HasPrivateKey);
        Assert.AreEqual(certificate.Issuer, result.Issuer);
        Assert.AreEqual(certificate.NotAfter, result.NotAfter);
        Assert.AreEqual(certificate.NotBefore, result.NotBefore);
        Assert.AreEqual(certificate.SerialNumber, result.SerialNumber);
        Assert.AreEqual(certificate.Thumbprint, result.Thumbprint);
    }

    private X509Certificate2 CreateSampleCertificate()
    {
        var certificate = new X509Certificate2();
        // Here you can set any required properties for the certificate
        return certificate;
    }
}
