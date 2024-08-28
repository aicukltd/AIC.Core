namespace AIC.Core.Security.Cryptography.Hashing.Extensions;

using System.Text;
using AIC.Core.Security.Cryptography.Hashing.Contracts;

public static class HashingExtensions
{
    public static async Task<string> Hash(this IHashingService hashingService, string data)
    {
        var bytes = Encoding.UTF8.GetBytes(data);
        var hashedData = await hashingService.HashAsync(bytes);

        return Convert.ToBase64String(hashedData);
    }
}