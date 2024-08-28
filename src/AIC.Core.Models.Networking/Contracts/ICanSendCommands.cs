namespace AIC.Core.Models.Networking.Contracts;

public interface ICanSendCommands
{
    Task SendCommandAsync(INetworkCommand networkCommand);
}