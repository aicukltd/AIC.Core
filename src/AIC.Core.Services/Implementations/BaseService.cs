namespace AIC.Core.Services.Implementations;

using AIC.Core.Services.Contracts;
using Microsoft.Extensions.Logging;

public abstract class BaseService<TArgument> : BaseService, IService<TArgument>
{
    protected BaseService(ILogger logger)
        : base(logger)
    {
    }

    public async Task InitialiseAsync(TArgument argument)
    {
        try
        {
            await this.InitialiseInternalAsync(argument);
        }
        catch (Exception exception)
        {
            this.Logger.LogError(exception, exception.Message);
        }
    }

    public async Task TerminateAsync(TArgument argument)
    {
        try
        {
            await this.TerminateInternalAsync(argument);
        }
        catch (Exception exception)
        {
            this.Logger.LogError(exception, exception.Message);
        }
    }

    protected abstract Task InitialiseInternalAsync(TArgument argument);
    protected abstract Task TerminateInternalAsync(TArgument argument);
}

public abstract class BaseService : IService
{
    protected readonly ILogger Logger;

    protected BaseService(ILogger logger)
    {
        this.Logger = logger;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await this.InitialiseInternalAsync();
        }
        catch (Exception exception)
        {
            this.Logger.LogError(exception, exception.Message);
        }
    }

    public async Task TerminateAsync()
    {
        try
        {
            await this.TerminateInternalAsync();
        }
        catch (Exception exception)
        {
            this.Logger.LogError(exception, exception.Message);
        }
    }

    public virtual async ValueTask DisposeAsync()
    {
        await this.TerminateAsync();
    }

    protected abstract Task InitialiseInternalAsync();
    protected abstract Task TerminateInternalAsync();
}