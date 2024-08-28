namespace AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Contracts;

public interface IHasOidData
{
    IX509Certificate3Oid Oid { get; set; }
    byte[] RawData { get; set; }
}