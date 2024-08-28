namespace AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Implementations;

using AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Contracts;

public class X500DistinguishedName3 : IX500DistinguishedName3
{
    public IX509Certificate3Oid Oid { get; set; }
    public byte[] RawData { get; set; }
    public string Name { get; }
}