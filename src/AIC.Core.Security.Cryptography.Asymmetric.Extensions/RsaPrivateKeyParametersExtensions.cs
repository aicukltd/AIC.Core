namespace AIC.Core.Security.Cryptography.Asymmetric.Extensions;

using System.Security.Cryptography;
using AIC.Core.Security.Cryptography.Asymmetric.Contracts;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

public static class RsaPrivateKeyParametersExtensions
{
    public static RSAParameters ToRsaParameters(this IAsymmetricPrivateKeyParameters asymmetricPrivateKeyParameters)
    {
        return new RSAParameters
        {
            D = asymmetricPrivateKeyParameters.D,
            P = asymmetricPrivateKeyParameters.P,
            Q = asymmetricPrivateKeyParameters.Q,
            DP = asymmetricPrivateKeyParameters.Dp,
            DQ = asymmetricPrivateKeyParameters.Dq,
            InverseQ = asymmetricPrivateKeyParameters.InverseQ,
            Modulus = asymmetricPrivateKeyParameters.Modulus,
            Exponent = asymmetricPrivateKeyParameters.Exponent
        };
    }

    public static RsaPrivateCrtKeyParameters ToRsaPrivateCrtKeyParameters(
        this IAsymmetricPrivateKeyParameters asymmetricPrivateKeyParameters)
    {
        // ref: https://src-bin.com/en/q/e7ddf
        return new RsaPrivateCrtKeyParameters(
            new BigInteger(1, asymmetricPrivateKeyParameters.Modulus),
            new BigInteger(1, asymmetricPrivateKeyParameters.Exponent),
            new BigInteger(1, asymmetricPrivateKeyParameters.D),
            new BigInteger(1, asymmetricPrivateKeyParameters.P),
            new BigInteger(1, asymmetricPrivateKeyParameters.Q),
            new BigInteger(1, asymmetricPrivateKeyParameters.Dp),
            new BigInteger(1, asymmetricPrivateKeyParameters.Dq),
            new BigInteger(1, asymmetricPrivateKeyParameters.InverseQ));
    }
}