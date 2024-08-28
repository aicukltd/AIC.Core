namespace AIC.Core.Models.Networking.Contracts;

using AIC.Core.Data.Contracts;

public interface IPacket : IHasId<Guid>, IHasData<byte[]>, IHasCompleteFlag, IHasLength
{
    byte Header => this.Data.First();
}