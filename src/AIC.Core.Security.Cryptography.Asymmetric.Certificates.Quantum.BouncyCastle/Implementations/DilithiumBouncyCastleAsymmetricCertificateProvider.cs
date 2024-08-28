namespace AIC.Core.Security.Cryptography.Asymmetric.Certificates.Quantum.BouncyCastle.Implementations;

using System.Security.Cryptography.X509Certificates;
using AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Contracts;
using AIC.Core.Security.Cryptography.Asymmetric.Certificates.Quantum.BouncyCastle.Contracts;
using AIC.Core.Security.Cryptography.Asymmetric.Quantum.BouncyCastle.Contracts;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

public class
    DilithiumBouncyCastleAsymmetricCertificateProvider : IDilithiumBouncyCastleAsymmetricCertificateProvider
{
    private readonly IDilithiumAsymmetricCertificateProvider dilithiumAsymmetricCertificateProvider;
    private readonly IDilithiumAsymmetricCryptographyProvider dilithiumAsymmetricCryptographyProvider;

    public DilithiumBouncyCastleAsymmetricCertificateProvider(
        IDilithiumAsymmetricCryptographyProvider dilithiumAsymmetricCryptographyProvider,
        IDilithiumAsymmetricCertificateProvider dilithiumAsymmetricCertificateProvider)
    {
        this.dilithiumAsymmetricCryptographyProvider = dilithiumAsymmetricCryptographyProvider;
        this.dilithiumAsymmetricCertificateProvider = dilithiumAsymmetricCertificateProvider;
    }

    public async Task<IX509Certificate3> FromCertificateAsync(X509Certificate certificate)
    {
        var x509Certificate2 = new X509Certificate2(certificate.GetEncoded());

        return await this.dilithiumAsymmetricCertificateProvider.FromCertificateAsync(x509Certificate2);
    }
}