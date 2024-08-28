namespace AIC.Core.Services.Contracts;

public interface IService : IAsyncDisposable
{
    Task InitialiseAsync();
    Task TerminateAsync();
}

public interface IService<in TArgument> : IService
{
    Task InitialiseAsync(TArgument argument);
    Task TerminateAsync(TArgument argument);
}