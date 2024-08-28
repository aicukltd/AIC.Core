namespace AIC.Core.Security.Cryptography.Asymmetric.Extensions;

using System.Security.Cryptography;
using AIC.Core.Security.Cryptography.Asymmetric.Contracts;
using AIC.Core.Security.Cryptography.Asymmetric.Implementations;

public static class RsaParametersExtensions
{
    public static IAsymmetricPrivateKeyParameters ToPrivateKeyParameters(this RSAParameters rsaParameters)
    {
        return new AsymmetricPrivateKeyParameters
        {
            D = rsaParameters.D,
            P = rsaParameters.P,
            Q = rsaParameters.Q,
            Dp = rsaParameters.DP,
            Dq = rsaParameters.DQ,
            InverseQ = rsaParameters.InverseQ,
            Modulus = rsaParameters.Modulus,
            Exponent = rsaParameters.Exponent
        };
    }

    public static IAsymmetricPublicKeyParameters ToPublicKeyParameters(this RSAParameters rsaParameters)
    {
        return new AsymmetricPublicKeyParameters
        {
            Modulus = rsaParameters.Modulus,
            Exponent = rsaParameters.Exponent
        };
    }
}