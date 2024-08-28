namespace AIC.Core.Services.Networking.Contracts;

using AIC.Core.Models.Networking.Connections.Contracts;
using AIC.Core.Models.Networking.Contracts;
using AIC.Core.Services.Contracts;
using AIC.Core.Services.Networking.Connections.Contracts;

public interface INetworkService : IService, ICanSendCommands, ICanReceiveData
{
    Task ConnectAsync(ConnectionInformationType connectionInformationType);
    Task DisconnectAsync(Guid connectionId);

    Task<IEnumerable<IConnectionHandlingService>> GetConnectionsAsync(
        ConnectionInformationType? connectionInformationType);

    Task<IConnectionHandlingService> GetConnectionAsync(Guid connectionId);
}

public interface INetworkService<in TArgument> : INetworkService, IService<TArgument>
{
}