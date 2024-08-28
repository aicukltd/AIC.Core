namespace AIC.Core.Identity.Models.Contracts;

public interface IPaymentMethod
{
    string NameOnCard { get; set; }
    string Number { get; set; }
    DateTime Start { get; set; }
    DateTime Expiry { get; set; }
    int Cvc { get; set; }
    string ExternalPaymentMethodId { get; set; }
}