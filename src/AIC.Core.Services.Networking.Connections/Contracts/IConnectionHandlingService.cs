namespace AIC.Core.Services.Networking.Connections.Contracts;

using AIC.Core.Models.Networking.Connections.Contracts;
using AIC.Core.Models.Networking.Contracts;

public interface IConnectionHandlingService : ICanConnectAndDisconnect,
    ICanSendCommands, ICanReceiveData, ICanHandleConnectionInformation, IAsyncDisposable
{
}