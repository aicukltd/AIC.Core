namespace AIC.Core.Models.Networking.Contracts;

using AIC.Core.Delegates.Networking;

public interface ICanReceiveData
{
    event DataReceived DataReceived;
}