namespace AIC.Core.Services.Networking.Implementations;

using AIC.Core.Services.Networking.Connections.Provisioning.Contracts;
using AIC.Core.Services.Networking.Contracts;
using Microsoft.Extensions.Logging;

public sealed class DefaultNetworkService : BaseNetworkService, INetworkService
{
    public DefaultNetworkService(ILogger logger, IConnectionProvisioningService connectionProvisioningService) : base(
        logger, connectionProvisioningService)
    {
    }

    protected override Task InitialiseInternalAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task TerminateInternalAsync()
    {
        return Task.CompletedTask;
    }
}