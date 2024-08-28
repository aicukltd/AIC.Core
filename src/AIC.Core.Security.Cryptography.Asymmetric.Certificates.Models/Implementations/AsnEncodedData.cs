namespace AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Implementations;

using AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Contracts;

public class AsnEncodedData : IAsnEncodedData
{
    public IX509Certificate3Oid Oid { get; set; }
    public byte[] RawData { get; set; }
}