namespace AIC.Core.Services.Networking.Implementations;

using AIC.Core.Delegates.Networking;
using AIC.Core.Models.Networking.Connections.Contracts;
using AIC.Core.Models.Networking.Contracts;
using AIC.Core.Services.Implementations;
using AIC.Core.Services.Networking.Connections.Contracts;
using AIC.Core.Services.Networking.Connections.Provisioning.Contracts;
using AIC.Core.Services.Networking.Contracts;
using Microsoft.Extensions.Logging;

public abstract class BaseNetworkService : BaseService, INetworkService
{
    private readonly IConnectionProvisioningService connectionProvisioningService;

    private readonly IDictionary<Guid, IConnectionHandlingService> connections;

    protected BaseNetworkService(ILogger logger, IConnectionProvisioningService connectionProvisioningService) :
        base(logger)
    {
        this.connectionProvisioningService = connectionProvisioningService;

        this.connections = new Dictionary<Guid, IConnectionHandlingService>();
    }

    public async Task ConnectAsync(ConnectionInformationType connectionInformationType)
    {
        try
        {
            await this.InitialiseAsync();

            await this.ConnectInternalAsync(connectionInformationType);
        }
        catch (Exception exception)
        {
            this.Logger.LogError(exception, exception.Message);
        }
    }

    public async Task DisconnectAsync(Guid connectionId)
    {
        try
        {
            await this.DisconnectInternalAsync(connectionId);

            await this.TerminateAsync();
        }
        catch (Exception exception)
        {
            this.Logger.LogError(exception, exception.Message);
        }
    }

    public async Task<IEnumerable<IConnectionHandlingService>> GetConnectionsAsync(
        ConnectionInformationType? connectionInformationType)
    {
        return connectionInformationType == null
            ? this.connections.Values
            : this.connections.Values.Where(connection =>
                connection.ConnectionInformation.Type == connectionInformationType);
    }

    public async Task<IConnectionHandlingService> GetConnectionAsync(Guid connectionId)
    {
        return this.connections[connectionId];
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

    protected virtual async Task ConnectInternalAsync(ConnectionInformationType connectionInformationType)
    {
        ArgumentNullException.ThrowIfNull(connectionInformationType);

        var connectionInstance =
            await this.connectionProvisioningService.GetConnectionInstanceAsync(connectionInformationType);

        var connectionHandler = await this.connectionProvisioningService.GetConnectionHandlerAsync(connectionInstance);

        connectionHandler.DataReceived += this.OnDataReceived;

        await connectionHandler.ConnectAsync();

        this.connections.Add(connectionInstance.Id, connectionHandler);
    }

    protected virtual async Task DisconnectInternalAsync(Guid connectionId)
    {
        if (connectionId == default) throw new ArgumentNullException(nameof(connectionId));

        if (!this.connections.TryGetValue(connectionId, out var connectionInstance))
            throw new InvalidOperationException("A connection doesn't exist with that key.");

        await connectionInstance.DisconnectAsync();
    }

    protected virtual async Task SendCommandInternalAsync(INetworkCommand networkCommand)
    {
        ArgumentNullException.ThrowIfNull(networkCommand);

        if (!this.connections.Any()) throw new InvalidOperationException("No connections exist.");

        foreach (var connectionInstance in this.connections)
            await connectionInstance.Value.SendCommandAsync(networkCommand);
    }

    protected virtual async Task OnDataReceived(byte[] data)
    {
        await this.DataReceived?.Invoke(data);
    }
}