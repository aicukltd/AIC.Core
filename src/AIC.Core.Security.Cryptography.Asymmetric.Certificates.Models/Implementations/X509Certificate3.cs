namespace AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Implementations;

using AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Contracts;

public class X509Certificate3 : IX509Certificate3
{
    private X509Certificate3()
    {
    }

    public bool Archived { get; set; }
    public IEnumerable<IX509Certificate3Extension> Extensions { get; }
    public string FriendlyName { get; set; }
    public IntPtr Handle { get; }
    public bool HasPrivateKey { get; }
    public string Issuer { get; }
    public IX500DistinguishedName3 IssuerName { get; }
    public DateTime NotAfter { get; }
    public DateTime NotBefore { get; }
    public IX509Certificate3PublicKey PublicKey { get; }
    public byte[] RawData { get; }
    public ReadOnlyMemory<byte> RawDataMemory { get; }
    public string SerialNumber { get; }
    public ReadOnlyMemory<byte> SerialNumberBytes { get; }
    public IX509Certificate3Oid SignatureAlgorithm { get; }
    public string Subject { get; }
    public IX500DistinguishedName3 SubjectName { get; }
    public string Thumbprint { get; }
    public int Version { get; }

    public static IX509Certificate3 Initialise(IX509Certificate3 certificate)
    {
        return (X509Certificate3)certificate;
    }
}