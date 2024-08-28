namespace AIC.Core.Security.Cryptography.Asymmetric.Quantum.BouncyCastle.Contracts;

using AIC.Core.Security.Cryptography.Asymmetric.Contracts;
using Org.BouncyCastle.Pqc.Crypto.Crystals.Kyber;

public interface IKyberAsymmetricCryptographyProvider : IAsymmetricCryptographyProvider
{
     (KyberPrivateKeyParameters privateKeyParameters, KyberPublicKeyParameters publicKeyParameters)
        GenerateKyberKeyPair();

    byte[] Encrypt(byte[] data, byte[] publicKey, out byte[] encapsulatedKey);
    byte[] Encrypt(byte[] data, KyberPublicKeyParameters publicKey, out byte[] encapsulatedKey);
    byte[] Decrypt(byte[] data, byte[] privateKey, byte[] encapsulatedSharedKey);
    byte[] Decrypt(byte[] data, KyberPrivateKeyParameters privateKey, byte[] encapsulatedSharedKey);
}