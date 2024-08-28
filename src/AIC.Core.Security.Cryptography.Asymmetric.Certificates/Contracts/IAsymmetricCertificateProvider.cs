namespace AIC.Core.Security.Cryptography.Asymmetric.Certificates.Contracts;

using AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Contracts;

public interface IAsymmetricCertificateProvider<in TFromCertificate>
{
    Task<IX509Certificate3> FromCertificateAsync(TFromCertificate certificate);
}