namespace AIC.Core.Identity.Models.Implementations;

using AIC.Core.Identity.Models.Contracts;

public class PaymentMethod : IPaymentMethod
{
    public string NameOnCard { get; set; }
    public string Number { get; set; }
    public DateTime Start { get; set; }
    public DateTime Expiry { get; set; }
    public int Cvc { get; set; }
    public string ExternalPaymentMethodId { get; set; }
}