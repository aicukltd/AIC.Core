namespace AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Implementations;

using AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Contracts;

public class X509Certificate3PublicKey : IX509Certificate3PublicKey
{
    public IX509Certificate3Oid Oid { get; set; }
    public byte[] RawData { get; set; }
    public IAsnEncodedData EncodedKeyValue { get; }
    public IAsnEncodedData EncodedParameters { get; }
}