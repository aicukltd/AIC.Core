namespace AIC.Core.Identity.Extensions;

using AIC.Core.Identity.Models.Contracts;

public static class OrganisationExtensions
{
    public static IOrganisation UpdateLogoUrl(this IOrganisation organisation)
    {
        organisation.LogoUrl =
            $"https://ui-avatars.com/api/?name={(organisation?.Name ?? "Not Defined").Replace(" ", "+")}";

        return organisation;
    }
}