namespace AIC.Core.Data.Models.References.Contracts;

public interface IHasRootUserId
{
    Guid RootUserId { get; set; }
}