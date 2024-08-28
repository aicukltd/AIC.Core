namespace AIC.Core.Security.Cryptography.Asymmetric.Implementations;

using AIC.Core.Security.Cryptography.Asymmetric.Contracts;

public class AsymmetricPublicKeyParameters : IAsymmetricPublicKeyParameters
{
    public byte[] Modulus { get; set; }
    public byte[] Exponent { get; set; }
}