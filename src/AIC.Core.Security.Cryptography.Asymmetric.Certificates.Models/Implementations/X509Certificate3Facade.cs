namespace AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Implementations;

using AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Contracts;

internal sealed class X509Certificate3Facade : IX509Certificate3
{
    public X509Certificate3Facade()
    {
        this.Version = 3;
    }

    public bool Archived { get; set; }
    public IEnumerable<IX509Certificate3Extension> Extensions { get; init; }
    public string FriendlyName { get; set; }
    public IntPtr Handle { get; init; }
    public bool HasPrivateKey { get; init; }
    public string Issuer { get; init; }
    public IX500DistinguishedName3 IssuerName { get; init; }
    public DateTime NotAfter { get; init; }
    public DateTime NotBefore { get; init; }
    public IX509Certificate3PublicKey PublicKey { get; init; }
    public byte[] RawData { get; init; }
    public ReadOnlyMemory<byte> RawDataMemory { get; init; }
    public string SerialNumber { get; init; }
    public ReadOnlyMemory<byte> SerialNumberBytes { get; init; }
    public IX509Certificate3Oid SignatureAlgorithm { get; init; }
    public string Subject { get; init; }
    public IX500DistinguishedName3 SubjectName { get; init; }
    public string Thumbprint { get; init; }
    public int Version { get; }
}