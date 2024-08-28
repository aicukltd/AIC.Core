namespace AIC.Core.Data.Models.References.Contracts;

public interface IHasTransactionId
{
    Guid TransactionId { get; set; }
}