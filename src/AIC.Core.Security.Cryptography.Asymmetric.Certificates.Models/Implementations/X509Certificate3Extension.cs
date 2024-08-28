namespace AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Implementations;

using AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Contracts;

public class X509Certificate3Extension : IX509Certificate3Extension
{
    public IX509Certificate3Oid Oid { get; set; }
    public byte[] RawData { get; set; }
    public bool Critical { get; set; }
}