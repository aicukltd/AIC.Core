namespace AIC.Core.Security.Cryptography.Asymmetric.Extensions;

using System.Security.Cryptography;
using AIC.Core.Security.Cryptography.Asymmetric.Contracts;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

public static class RsaPublicKeyParametersExtensions
{
    public static RSAParameters ToRsaParameters(this IAsymmetricPublicKeyParameters asymmetricPublicKeyParameters)
    {
        return new RSAParameters
        {
            Modulus = asymmetricPublicKeyParameters.Modulus,
            Exponent = asymmetricPublicKeyParameters.Exponent
        };
    }

    public static RsaKeyParameters ToRsaKeyParameters(this IAsymmetricPublicKeyParameters asymmetricPublicKeyParameters)
    {
        return new RsaKeyParameters(
            false,
            new BigInteger(1, asymmetricPublicKeyParameters.Modulus),
            new BigInteger(1, asymmetricPublicKeyParameters.Exponent));
    }
}