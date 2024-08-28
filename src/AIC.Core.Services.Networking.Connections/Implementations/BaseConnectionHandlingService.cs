namespace AIC.Core.Services.Networking.Connections.Implementations;

using AIC.Core.Delegates.Networking;
using AIC.Core.Models.Networking.Connections.Contracts;
using AIC.Core.Models.Networking.Contracts;
using AIC.Core.Services.Networking.Connections.Contracts;
using Microsoft.Extensions.Logging;

public abstract class BaseConnectionHandlingService : IConnectionHandlingService
{
    protected readonly ILogger Logger;
    protected readonly CancellationTokenSource CancellationTokenSource = new();

    protected BaseConnectionHandlingService(ILogger logger)
    {
        this.Logger = logger;
    }

    public async Task ConnectAsync()
    {
        try
        {
            await this.ConnectInternalAsync();
        }
        catch (Exception exception)
        {
            this.Logger.LogError(exception, exception.Message);
        }
    }

    public async Task DisconnectAsync()
    {
        try
        {
            await this.CancellationTokenSource.CancelAsync();

            await this.DisconnectInternalAsync();
        }
        catch (Exception exception)
        {
            this.Logger.LogError(exception, exception.Message);
        }
    }

    public async Task SendCommandAsync(INetworkCommand networkCommand)
    {
        try
        {
            await this.SendCommandInternalAsync(networkCommand);
        }
        catch (Exception exception)
        {
            this.Logger.LogError(exception, exception.Message);
        }
    }

    public event DataReceived? DataReceived;

    public IHasConnectionInformation ConnectionInformation { get; private set; }

    public void SetConnectionInformation(IHasConnectionInformation connectionInformation)
    {
        this.ConnectionInformation = connectionInformation;
    }

    public async ValueTask DisposeAsync()
    {
        await this.DisconnectAsync();
        await this.DisposeAsyncInternal();
        GC.SuppressFinalize(this);
    }

    protected abstract Task ConnectInternalAsync();

    protected abstract Task DisconnectInternalAsync();

    protected abstract Task SendCommandInternalAsync(INetworkCommand networkCommand);

    protected virtual async Task OnDataReceived(byte[] data)
    {
        await this.DataReceived?.Invoke(data);
    }

    protected virtual async ValueTask DisposeAsyncInternal()
    {
        // TODO release managed resources here
    }
}