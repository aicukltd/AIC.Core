namespace AIC.Core.Services.Networking.Connections.Provisioning.Contracts;

using AIC.Core.Models.Networking.Connections.Contracts;
using AIC.Core.Services.Networking.Connections.Contracts;

public interface IConnectionProvisioningService
{
    Task<IConnectionInstance> GetConnectionInstanceAsync(ConnectionInformationType connectionInformationType);

    Task<IConnectionHandlingService> GetConnectionHandlerAsync(IConnectionInstance connectionInstance);
}