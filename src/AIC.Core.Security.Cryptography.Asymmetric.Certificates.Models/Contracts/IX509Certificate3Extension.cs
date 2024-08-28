namespace AIC.Core.Security.Cryptography.Asymmetric.Certificates.Models.Contracts;

public interface IX509Certificate3Extension : IHasOidData
{
    bool Critical { get; set; }
}