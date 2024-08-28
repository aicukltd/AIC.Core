namespace AIC.Core.Security.Cryptography.Asymmetric.Quantum.BouncyCastle.Tests.Implementations;

using System.Text;
using AIC.Core.Security.Cryptography.Asymmetric.Quantum.BouncyCastle.Implementations;
using AIC.Core.Security.Cryptography.Symmetric.AES.Implementations.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class KyberAsymmetricCryptographyProviderTests
{
    private KyberAsymmetricCryptographyProvider provider;

    [TestInitialize]
    public void SetUp()
    {
        this.provider = new KyberAsymmetricCryptographyProvider(new AesBouncyCastleSymmetricCryptographyProvider());
    }

    [TestMethod]
    public void GenerateKeyPair_ShouldReturnValidKeys()
    {
        var keys = this.provider.GenerateKeyPair();

        Assert.IsNotNull(keys.privateKeyParameters);
        Assert.IsNotNull(keys.publicKeyParameters);
    }

    [TestMethod]
    public void EncryptDecrypt_ShouldReturnOriginalData()
    {
        var data = Encoding.UTF8.GetBytes("Hello, Kyber!");
        var keys = this.provider.GenerateKyberKeyPair();

        var encryptedData = this.provider.Encrypt(data, keys.publicKeyParameters, out var encapsulatedSharedKey);
        var decryptedData = this.provider.Decrypt(encryptedData, keys.privateKeyParameters, encapsulatedSharedKey);

        var decryptedString = Encoding.UTF8.GetString(decryptedData);

        Assert.AreEqual("Hello, Kyber!", decryptedString);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Decrypt_WithInvalidPrivateKey_ShouldThrowException()
    {
        var data = Encoding.UTF8.GetBytes("Hello, Kyber!");
        var keys = this.provider.GenerateKeyPair();

        var encryptedData = this.provider.Encrypt(data, keys.publicKeyParameters);

        var invalidPrivateKey = new byte[] { 1, 2, 3, 4, 5 }; // Some random invalid key
        this.provider.Decrypt(encryptedData, invalidPrivateKey);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Encrypt_WithInvalidPublicKey_ShouldThrowException()
    {
        var data = Encoding.UTF8.GetBytes("Hello, Kyber!");
        var invalidPublicKey = new byte[] { 1, 2, 3, 4, 5 }; // Some random invalid key

        this.provider.Encrypt(data, invalidPublicKey);
    }
}