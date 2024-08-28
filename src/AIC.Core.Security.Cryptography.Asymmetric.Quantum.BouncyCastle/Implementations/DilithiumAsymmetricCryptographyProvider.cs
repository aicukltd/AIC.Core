namespace AIC.Core.Security.Cryptography.Asymmetric.Quantum.BouncyCastle.Implementations;

using Org.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;
using Org.BouncyCastle.Pqc.Crypto.Utilities;
using Org.BouncyCastle.Security;

public class DilithiumAsymmetricCryptographyProvider : BaseAsymmetricCryptographyProvider<DilithiumKeyPairGenerator>
{
    public override (byte[] privateKeyParameters, byte[] publicKeyParameters) GenerateKeyPair()
    {
        var dilithiumKeyPairGenerator = this.InitialiseAlgorithm();

        var keyPair = dilithiumKeyPairGenerator.GenerateKeyPair();

        var privateKey = keyPair.Private;
        var publicKey = keyPair.Public;

        var dilithiumPrivateKeyInfo = PqcPrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);
        var dilithiumPublicKeyInfo = PqcSubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);


        return
        (
            dilithiumPrivateKeyInfo.GetEncoded(),
            dilithiumPublicKeyInfo.GetEncoded()
        );
    }


    public override byte[] Sign(byte[] data, byte[] privateKey)
    {
        var dilithiumPrivateKey = PqcPrivateKeyFactory.CreateKey(privateKey);

        var signer = new DilithiumSigner();

        signer.Init(true, dilithiumPrivateKey);

        return signer.GenerateSignature(data);
    }

    public override bool Verify(byte[] data, byte[] signature, byte[] publicKey)
    {
        var dilithiumPublicKey = PqcPublicKeyFactory.CreateKey(publicKey);

        var verifier = new DilithiumSigner();

        verifier.Init(false, dilithiumPublicKey);

        return verifier.VerifySignature(data, signature);
    }

    protected override DilithiumKeyPairGenerator InitialiseAlgorithm()
    {
        var dilithiumKeyPairGenerator = new DilithiumKeyPairGenerator();

        var dilithiumKeyGenerationParameters = new DilithiumKeyGenerationParameters(
            new SecureRandom(),
            DilithiumParameters.Dilithium5Aes
        );

        dilithiumKeyPairGenerator.Init(dilithiumKeyGenerationParameters);

        return dilithiumKeyPairGenerator;
    }
}