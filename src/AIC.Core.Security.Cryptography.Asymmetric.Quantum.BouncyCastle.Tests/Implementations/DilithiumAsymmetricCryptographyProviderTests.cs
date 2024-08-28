namespace AIC.Core.Security.Cryptography.Asymmetric.Quantum.BouncyCastle.Tests.Implementations;

using System.Text;
using AIC.Core.Security.Cryptography.Asymmetric.Quantum.BouncyCastle.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class DilithiumAsymmetricCryptographyProviderTests
{
    private readonly DilithiumAsymmetricCryptographyProvider provider;

    public DilithiumAsymmetricCryptographyProviderTests()
    {
        this.provider = new DilithiumAsymmetricCryptographyProvider();
    }

    [TestMethod]
    public void GenerateKeyPair_ShouldGenerateValidKeyPair()
    {
        var keyPair = this.provider.GenerateKeyPair();

        Assert.IsNotNull(keyPair.privateKeyParameters);
        Assert.IsNotNull(keyPair.publicKeyParameters);
    }

    [TestMethod]
    public void SignAndVerify_ShouldWorkProperly()
    {
        var keyPair = this.provider.GenerateKeyPair();

        var message = "Hello, Dilithium!";
        var data = Encoding.UTF8.GetBytes(message);
        var signature = this.provider.Sign(data, keyPair.privateKeyParameters);

        Assert.IsTrue(this.provider.Verify(data, signature, keyPair.publicKeyParameters));
    }

    [TestMethod]
    public void SignAndVerify_InvalidSignature_ShouldReturnFalse()
    {
        var keyPair1 = this.provider.GenerateKeyPair();
        var privateKey1 = Convert.ToBase64String(keyPair1.privateKeyParameters);

        var keyPair2 = this.provider.GenerateKeyPair();
        var publicKey2 = Convert.ToBase64String(keyPair2.publicKeyParameters);

        var message = "Hello, Dilithium!";
        var data = Encoding.UTF8.GetBytes(message);
        var signature = this.provider.Sign(data, privateKey1);

        Assert.IsFalse(this.provider.Verify(data, signature, publicKey2));
    }
}