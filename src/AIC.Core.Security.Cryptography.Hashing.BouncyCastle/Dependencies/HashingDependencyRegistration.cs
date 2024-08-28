namespace AIC.Core.Security.Cryptography.Hashing.BouncyCastle.Dependencies;

using AIC.Core.Security.Cryptography.Hashing.BouncyCastle.Implementations;
using AIC.Core.Security.Cryptography.Hashing.Contracts;
using Microsoft.Extensions.DependencyInjection;

public static class HashingDependencyRegistration
{
    public static void RegisterBouncyCastleHashingDependencies(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IHashingService, Sha3HashingService>();
    }
}