namespace AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Implementations;

using AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Contracts;

public class X509Certificate3Oid : IX509Certificate3Oid
{
    public string FriendlyName { get; set; }
    public string Value { get; set; }
}