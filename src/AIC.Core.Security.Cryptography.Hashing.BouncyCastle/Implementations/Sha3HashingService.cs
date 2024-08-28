namespace AIC.Core.Security.Cryptography.Hashing.BouncyCastle.Implementations;

using AIC.Core.Security.Cryptography.Hashing.Contracts;
using Org.BouncyCastle.Crypto.Digests;

public class Sha3HashingService : IHashingService
{
    public async Task<byte[]> HashAsync(byte[] data)
    {
        var hashAlgorithm = new Sha3Digest(512);

        hashAlgorithm.BlockUpdate(data, 0, data.Length);

        var result = new byte[64]; // 512 / 8 = 64
        hashAlgorithm.DoFinal(result, 0);

        return result;
    }
}