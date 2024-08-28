namespace AIC.Core.Security.Cryptography.Asymmetric.Contracts;

public interface IAsymmetricPublicKeyParameters
{
    byte[] Modulus { get; set; }
    byte[] Exponent { get; set; }
}