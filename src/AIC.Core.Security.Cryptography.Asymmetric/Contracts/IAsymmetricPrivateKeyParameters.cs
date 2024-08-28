namespace AIC.Core.Security.Cryptography.Asymmetric.Contracts;

public interface IAsymmetricPrivateKeyParameters
{
    byte[] D { get; set; }
    byte[] P { get; set; }
    byte[] Q { get; set; }
    byte[] Dp { get; set; }
    byte[] Dq { get; set; }
    byte[] InverseQ { get; set; }
    byte[] Modulus { get; set; }
    byte[] Exponent { get; set; }
}