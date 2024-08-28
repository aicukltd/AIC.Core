namespace AIC.Core.Identity.Models.Contracts;

using AIC.Core.Data.Contracts;
using AIC.Core.Data.Models.References.Contracts;
using AIC.Core.Identity.Subscriptions.Models.References.Contracts;

public interface IOrganisation : IBaseEntity, IHasRootUserId, IHasSubscriptionId, IHasLogoUrl, IHasName
{
    string Address { get; set; }
    string Phone { get; set; }
    string Email { get; set; }
    string RegistrationNumber { get; set; }
    string Website { get; set; }
    string Description { get; set; }
    string ExternalOrganisationId { get; set; }
    Guid ParentOrganisationId { get; set; }
    bool HasBeenSetup { get; set; }
}