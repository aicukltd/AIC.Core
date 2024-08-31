namespace AIC.Core.Security.Cryptography.Asymmetric.Certificates.Quantum.BouncyCastle.Implementations;

using System.Security.Cryptography.X509Certificates;
using AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Contracts;
using AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Implementations;
using AIC.Core.Security.Cryptography.Asymmetric.Certificates.Quantum.BouncyCastle.Contracts;
using AIC.Core.Security.Cryptography.Asymmetric.Quantum.BouncyCastle.Contracts;

public class DilithiumAsymmetricCertificateProvider : IDilithiumAsymmetricCertificateProvider
{
    public async Task<IX509Certificate3> FromCertificateAsync(X509Certificate2 certificate)
    {
        var certificateFacade = new X509Certificate3Facade
        {
            Archived = certificate.Archived,
            Extensions = certificate.Extensions.Select(
                extension => new X509Certificate3Extension
                {
                    Oid = new X509Certificate3Oid
                    {
                        FriendlyName = extension.Oid?.FriendlyName ?? "Not Defined",
                        Value = extension.Oid?.Value ?? "Not Defined"
                    },
                    Critical = extension.Critical,
                    RawData = extension.RawData
                }
            ),
            FriendlyName = certificate.FriendlyName,
            Handle = certificate.Handle,
            HasPrivateKey = certificate.HasPrivateKey,
            Issuer = certificate.Issuer,
            IssuerName =
            {
                Oid = new X509Certificate3Oid
                {
                    FriendlyName = certificate.IssuerName?.Oid?.FriendlyName ??
                                   certificate.IssuerName?.Name ?? "Not Defined",
                    Value = certificate.IssuerName?.Oid?.Value ?? certificate.IssuerName?.Name ?? "Not Defined"
                },
                RawData = certificate.IssuerName?.RawData ?? new byte[] { 0x00 }
            },
            NotAfter = certificate.NotAfter,
            NotBefore = certificate.NotBefore,
            PublicKey = new X509Certificate3PublicKey
            {
                Oid = new X509Certificate3Oid
                {
                    FriendlyName = certificate.PublicKey?.Oid?.FriendlyName ?? "Not Defined",
                    Value = certificate.PublicKey?.Oid?.Value ?? "Not Defined"
                },
                EncodedKeyValue =
                {
                    Oid = new X509Certificate3Oid
                    {
                        FriendlyName = certificate.PublicKey?.EncodedKeyValue?.Oid?.FriendlyName ?? "Not Defined",
                        Value = certificate.PublicKey?.EncodedKeyValue?.Oid?.Value ?? "Not Defined"
                    },
                    RawData = certificate.PublicKey?.EncodedKeyValue?.RawData ?? new byte[] { 0x00 }
                },
                EncodedParameters =
                {
                    Oid = new X509Certificate3Oid
                    {
                        FriendlyName = certificate.PublicKey?.EncodedParameters?.Oid?.FriendlyName ?? "Not Defined",
                        Value = certificate.PublicKey?.EncodedParameters?.Oid?.Value ?? "Not Defined"
                    },
                    RawData = certificate.PublicKey?.EncodedParameters?.RawData ?? new byte[] { 0x00 }
                }
            },
            RawData = certificate.RawData,
            RawDataMemory = certificate.RawDataMemory,
            SerialNumber = certificate.SerialNumber,
            SerialNumberBytes = certificate.SerialNumberBytes,
            SignatureAlgorithm = new X509Certificate3Oid
            {
                FriendlyName = certificate.SignatureAlgorithm?.FriendlyName ?? "Not Defined",
                Value = certificate.SignatureAlgorithm?.Value ?? "Not Defined"
            },
            Subject = certificate.Subject,
            SubjectName = new X500DistinguishedName3
            {
                Oid = new X509Certificate3Oid
                {
                    FriendlyName = certificate.SubjectName?.Oid?.FriendlyName ?? "Not Defined",
                    Value = certificate.SubjectName?.Oid?.Value ?? "Not Defined"
                },
                RawData = certificate.SubjectName?.RawData ?? new byte[] { 0x00 }
            },
            Thumbprint = certificate.Thumbprint
        };

        return X509Certificate3.Initialise(certificateFacade);
    }
}