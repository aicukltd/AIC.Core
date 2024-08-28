namespace AIC.Core.Models.Networking.Connections.Contracts;

public interface ICanHandleConnectionInformation
{
    IHasConnectionInformation ConnectionInformation { get; }

    void SetConnectionInformation(IHasConnectionInformation connectionInformation);
}