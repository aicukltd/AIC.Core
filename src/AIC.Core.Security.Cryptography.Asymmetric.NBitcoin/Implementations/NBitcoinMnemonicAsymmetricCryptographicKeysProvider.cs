using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIC.Core.Security.Cryptography.Asymmetric.NBitcoin.Implementations
{
    using AIC.Core.Security.Cryptography.Asymmetric.Contracts;
    using global::NBitcoin;

    public class NBitcoinMnemonicAsymmetricCryptographicKeysProvider : IMnemonicAsymmetricCryptographicKeysProvider
    {
        public (byte[] privateKey, byte[] publicKey) GenerateKeyPair()
        {
            var mnemonic = new Mnemonic(Wordlist.English, WordCount.TwentyFour);
            
            return this.GenerateKeyPair(mnemonic.ToString());
        }

        public (byte[] privateKey, byte[] publicKey) GenerateKeyPair(string mnemonic)
        {
            if (!this.IsMnemonicSeedValid(mnemonic, WordCount.TwentyFour))
                throw new ArgumentException("Mnemonic Seed is not the correct length: 24");

            var mnemonicInstance = new Mnemonic(mnemonic, Wordlist.English);

            var extKey = mnemonicInstance.DeriveExtKey();

            return (extKey.PrivateKey.ToBytes(), extKey.PrivateKey.PubKey.ToBytes());
        }

        private bool IsMnemonicSeedValid(string mnemonicSeed, WordCount wordCount)
        {
            var words = mnemonicSeed.Split(' ').ToList();

            return words.Count() == (int)wordCount;
        }
    }
}
