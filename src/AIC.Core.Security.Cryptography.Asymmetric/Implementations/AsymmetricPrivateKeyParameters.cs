namespace AIC.Core.Security.Cryptography.Asymmetric.Implementations;

using AIC.Core.Security.Cryptography.Asymmetric.Contracts;

public class AsymmetricPrivateKeyParameters : IAsymmetricPrivateKeyParameters
{
    public byte[] D { get; set; }
    public byte[] P { get; set; }
    public byte[] Q { get; set; }
    public byte[] Dp { get; set; }
    public byte[] Dq { get; set; }
    public byte[] InverseQ { get; set; }
    public byte[] Modulus { get; set; }
    public byte[] Exponent { get; set; }
}