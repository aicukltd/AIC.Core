namespace AIC.Core.Security.Cryptography.Asymmetric.Quantum.BouncyCastle.Tests.Implementations;

using System.Text;
using AIC.Core.Security.Cryptography.Asymmetric.Quantum.BouncyCastle.Implementations;
using NUnit.Framework;
using Org.BouncyCastle.Crypto;
using System;
using Org.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;

[TestFixture]
public class DilithiumAsymmetricCryptographyProviderTests
{
    private DilithiumAsymmetricCryptographyProvider provider;

    [SetUp]
    public void SetUp()
    {
        this.provider = new DilithiumAsymmetricCryptographyProvider();
    }

    [Test]
    public void GenerateKeyPair_WhenCalled_ShouldReturnValidKeyPair()
    {
        // Arrange & Act
        var result = this.provider.GenerateKeyPair();

        // Assert
        Assert.IsNotNull(result.privateKeyParameters);
        Assert.IsNotNull(result.publicKeyParameters);
        Assert.IsTrue(result.privateKeyParameters.Length > 0);
        Assert.IsTrue(result.publicKeyParameters.Length > 0);
    }

    [Test]
    public void Sign_WhenCalled_ShouldReturnValidSignature()
    {
        // Arrange
        var data = new byte[] { 0x01, 0x02, 0x03 };
        var keyPair = this.provider.GenerateKeyPair();
        var privateKey = keyPair.privateKeyParameters;

        // Act
        var signature = this.provider.Sign(data, privateKey);

        // Assert
        Assert.IsNotNull(signature);
        Assert.IsTrue(signature.Length > 0);
    }

    [Test]
    public void Sign_WhenPrivateKeyIsInvalid_ShouldThrowException()
    {
        // Arrange
        var data = new byte[] { 0x01, 0x02, 0x03 };
        var invalidPrivateKey = new byte[] { 0x00 };

        // Act & Assert
        Assert.Throws<CryptoException>(() => this.provider.Sign(data, invalidPrivateKey));
    }

    [Test]
    public void Verify_WhenCalledWithValidSignature_ShouldReturnTrue()
    {
        // Arrange
        var data = new byte[] { 0x01, 0x02, 0x03 };
        var keyPair = this.provider.GenerateKeyPair();
        var privateKey = keyPair.privateKeyParameters;
        var publicKey = keyPair.publicKeyParameters;

        var signature = this.provider.Sign(data, privateKey);

        // Act
        var isVerified = this.provider.Verify(data, signature, publicKey);

        // Assert
        Assert.IsTrue(isVerified);
    }

    [Test]
    public void Verify_WhenCalledWithInvalidSignature_ShouldReturnFalse()
    {
        // Arrange
        var data = new byte[] { 0x01, 0x02, 0x03 };
        var keyPair = this.provider.GenerateKeyPair();
        var publicKey = keyPair.publicKeyParameters;
        var invalidSignature = new byte[] { 0x00, 0x01 };

        // Act
        var isVerified = this.provider.Verify(data, invalidSignature, publicKey);

        // Assert
        Assert.IsFalse(isVerified);
    }

    [Test]
    public void Verify_WhenPublicKeyIsInvalid_ShouldThrowException()
    {
        // Arrange
        var data = new byte[] { 0x01, 0x02, 0x03 };
        var invalidPublicKey = new byte[] { 0x00 };
        var signature = new byte[] { 0x01, 0x02 };

        // Act & Assert
        Assert.Throws<CryptoException>(() => this.provider.Verify(data, signature, invalidPublicKey));
    }
}
