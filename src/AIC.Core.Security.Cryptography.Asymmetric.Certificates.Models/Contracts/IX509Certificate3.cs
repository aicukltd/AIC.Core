namespace AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Contracts;

public interface IX509Certificate3
{
    bool Archived { get; set; }
    IEnumerable<IX509Certificate3Extension> Extensions { get; }
    string FriendlyName { get; set; }
    IntPtr Handle { get; }
    bool HasPrivateKey { get; }
    string Issuer { get; }
    IX500DistinguishedName3 IssuerName { get; }
    DateTime NotAfter { get; }
    DateTime NotBefore { get; }
    IX509Certificate3PublicKey PublicKey { get; }
    byte[] RawData { get; }
    ReadOnlyMemory<byte> RawDataMemory { get; }
    string SerialNumber { get; }
    ReadOnlyMemory<byte> SerialNumberBytes { get; }
    IX509Certificate3Oid SignatureAlgorithm { get; }
    string Subject { get; }
    IX500DistinguishedName3 SubjectName { get; }
    string Thumbprint { get; }
    int Version { get; }
}