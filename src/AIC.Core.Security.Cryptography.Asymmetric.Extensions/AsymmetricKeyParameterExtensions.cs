namespace AIC.Core.Security.Cryptography.Asymmetric.Extensions;

using AIC.Core.Security.Cryptography.Asymmetric.Contracts;
using AIC.Core.Security.Cryptography.Asymmetric.Implementations;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

public static class AsymmetricKeyParameterExtensions
{
    public static IAsymmetricPrivateKeyParameters ToPrivateKeyParameters(this AsymmetricKeyParameter keyParameters)
    {
        var rsaParameters = keyParameters as RsaPrivateCrtKeyParameters;
        return new AsymmetricPrivateKeyParameters
        {
            D = rsaParameters.Exponent.ToByteArrayUnsigned(),
            P = rsaParameters.P.ToByteArrayUnsigned(),
            Q = rsaParameters.Q.ToByteArrayUnsigned(),
            Dp = rsaParameters.DP.ToByteArrayUnsigned(),
            Dq = rsaParameters.DQ.ToByteArrayUnsigned(),
            InverseQ = rsaParameters.QInv.ToByteArrayUnsigned(),
            Modulus = rsaParameters.Modulus.ToByteArrayUnsigned(),
            Exponent = rsaParameters.PublicExponent.ToByteArrayUnsigned()
        };
    }

    public static IAsymmetricPublicKeyParameters ToPublicKeyParameters(this AsymmetricKeyParameter keyParameters)
    {
        var rsaParameters = keyParameters as RsaKeyParameters;
        return new AsymmetricPublicKeyParameters
        {
            Modulus = rsaParameters.Modulus.ToByteArrayUnsigned(),
            Exponent = rsaParameters.Exponent.ToByteArrayUnsigned()
        };
    }

    public static string SerialisePrivateKey(this AsymmetricKeyParameter privateKey)
    {
        var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);
        var serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetDerEncoded();
        return Convert.ToBase64String(serializedPrivateBytes);
    }

    public static string SerialisePublicKey(this AsymmetricKeyParameter publicKey)
    {
        var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
        var serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();
        return Convert.ToBase64String(serializedPublicBytes);
    }

    public static byte[] GetPrivateKeyBytes(this AsymmetricKeyParameter privateKey)
    {
        var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);
        return privateKeyInfo.ToAsn1Object().GetDerEncoded();
    }

    public static byte[] GetPublicKeyBytes(this AsymmetricKeyParameter publicKey)
    {
        var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
        return publicKeyInfo.ToAsn1Object().GetDerEncoded();
    }

    public static AsymmetricKeyParameter DeserialisePrivateKey(this string serialisedPrivateKey)
    {
        return PrivateKeyFactory.CreateKey(Convert.FromBase64String(serialisedPrivateKey));
    }

    public static AsymmetricKeyParameter DeserialisePublicKey(this string serialisedPublicKey)
    {
        return PublicKeyFactory.CreateKey(Convert.FromBase64String(serialisedPublicKey));
    }
}