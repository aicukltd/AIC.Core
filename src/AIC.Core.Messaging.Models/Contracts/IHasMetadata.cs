namespace AIC.Core.Messaging.Models.Contracts;

public interface IHasMetadata<TMetadata>
{
    TMetadata Metadata { get; set; }
}