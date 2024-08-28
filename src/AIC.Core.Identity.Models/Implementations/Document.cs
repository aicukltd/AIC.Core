namespace AIC.Core.Identity.Models.Implementations;

using AIC.Core.Identity.Models.Contracts;

public class Document : IDocument
{
    public DocumentType Type { get; set; }
    public string Number { get; set; }
    public DateTime Expiry { get; set; }
    public string ImageUrl { get; set; }
}