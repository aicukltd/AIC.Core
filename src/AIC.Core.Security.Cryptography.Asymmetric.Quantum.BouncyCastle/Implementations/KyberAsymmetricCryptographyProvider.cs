namespace AIC.Core.Security.Cryptography.Asymmetric.Quantum.BouncyCastle.Implementations;

using AIC.Core.Extensions;
using AIC.Core.Security.Cryptography.Asymmetric.Quantum.BouncyCastle.Contracts;
using AIC.Core.Security.Cryptography.Symmetric.Contracts;
using Org.BouncyCastle.Pqc.Crypto.Crystals.Kyber;
using Org.BouncyCastle.Pqc.Crypto.Utilities;
using Org.BouncyCastle.Security;

public class KyberAsymmetricCryptographyProvider : BaseAsymmetricCryptographyProvider<KyberKeyPairGenerator>,
    IKyberAsymmetricCryptographyProvider
{
    private readonly ISymmetricCryptographyProvider symmetricCryptographyProvider;

    public KyberAsymmetricCryptographyProvider(ISymmetricCryptographyProvider symmetricCryptographyProvider)
    {
        this.symmetricCryptographyProvider = symmetricCryptographyProvider;
    }

    public override (byte[] privateKeyParameters, byte[] publicKeyParameters) GenerateKeyPair()
    {
        var kyberKeys = this.GenerateKyberKeyPair();
        var kyberPrivateKeyInfo = PqcPrivateKeyInfoFactory.CreatePrivateKeyInfo(kyberKeys.privateKeyParameters);
        var kyberPublicKeyInfo =
            PqcSubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(kyberKeys.publicKeyParameters);


        return
        (
            kyberPrivateKeyInfo.GetEncoded(),
            kyberPublicKeyInfo.GetEncoded()
        );
    }

    public (KyberPrivateKeyParameters privateKeyParameters, KyberPublicKeyParameters publicKeyParameters)
        GenerateKyberKeyPair()
    {
        var kyberKeyPairGenerator = this.InitialiseAlgorithm();

        var keyPair = kyberKeyPairGenerator.GenerateKeyPair();

        var privateKey = (KyberPrivateKeyParameters)keyPair.Private;
        var publicKey = (KyberPublicKeyParameters)keyPair.Public;

        return (privateKey, publicKey);
    }

    public byte[] Encrypt(byte[] data, byte[] publicKey, out byte[] encapsulatedSharedKey)
    {
        var kyberPublicKey = (KyberPublicKeyParameters)PqcPublicKeyFactory.CreateKey(publicKey);

        return this.Encrypt(data, kyberPublicKey, out encapsulatedSharedKey);
    }

    public byte[] Encrypt(byte[] data, KyberPublicKeyParameters publicKey, out byte[] encapsulatedSharedKey)
    {
        var kemGenerator = new KyberKemGenerator(new SecureRandom());

        var encapsulatedSecret = kemGenerator.GenerateEncapsulated(publicKey);

        var key = encapsulatedSecret.GetSecret();
        var encapsulatedKey = encapsulatedSecret.GetEncapsulation();

        encapsulatedSharedKey = encapsulatedKey;

        return this.symmetricCryptographyProvider.Encrypt(data, key, key.GetSubSetOfBytes(16, 0));
    }

    public byte[] Decrypt(byte[] data, byte[] privateKey, byte[] encapsulatedSharedKey)
    {
        var kyberPrivateKey = (KyberPrivateKeyParameters)PqcPrivateKeyFactory.CreateKey(privateKey);

        return this.Decrypt(data, kyberPrivateKey, encapsulatedSharedKey);
    }

    public byte[] Decrypt(byte[] data, KyberPrivateKeyParameters privateKey, byte[] encapsulatedSharedKey)
    {
        var kemExtractor = new KyberKemExtractor(privateKey);

        var key = kemExtractor.ExtractSecret(encapsulatedSharedKey);

        return this.symmetricCryptographyProvider.Decrypt(data, key, key.GetSubSetOfBytes(16, 0));
    }

    protected override KyberKeyPairGenerator InitialiseAlgorithm()
    {
        var kyberKeyPairGenerator = new KyberKeyPairGenerator();

        var kyberKeyGenerationParameters = new KyberKeyGenerationParameters(
            new SecureRandom(),
            KyberParameters.kyber1024_aes
        );

        kyberKeyPairGenerator.Init(kyberKeyGenerationParameters);

        return kyberKeyPairGenerator;
    }
}