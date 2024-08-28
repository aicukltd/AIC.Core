namespace AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Contracts;

public interface IX509Certificate3PublicKey : IHasOidData
{
    IAsnEncodedData EncodedKeyValue { get; }
    IAsnEncodedData EncodedParameters { get; }
}