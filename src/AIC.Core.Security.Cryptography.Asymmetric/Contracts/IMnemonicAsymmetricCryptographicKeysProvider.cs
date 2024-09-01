namespace AIC.Core.Security.Cryptography.Asymmetric.Contracts;

public interface IMnemonicAsymmetricCryptographicKeysProvider
{
    (byte[] privateKey, byte[] publicKey) GenerateKeyPair();
    (byte[] privateKey, byte[] publicKey) GenerateKeyPair(string mnemonic);

}