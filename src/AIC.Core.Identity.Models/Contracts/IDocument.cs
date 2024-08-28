namespace AIC.Core.Identity.Models.Contracts;

using AIC.Core.Identity.Models.Implementations;

public interface IDocument
{
    DocumentType Type { get; set; }
    string Number { get; set; }
    DateTime Expiry { get; set; }
    string ImageUrl { get; set; }
}