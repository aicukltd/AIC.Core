namespace AIC.Core.Security.Cryptography.Hashing.Contracts;

public interface IHashingService
{
    Task<byte[]> HashAsync(byte[] data);
}