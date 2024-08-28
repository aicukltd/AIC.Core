namespace AIC.Core.Identity.Extensions;

using System.Security.Claims;
using AIC.Core.Identity.Models.Contracts;
using AIC.Core.Identity.Models.Implementations;

public static class IdentityExtensions
{
    public static string GetNickname(this ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal?.Claims == null) return "Not Defined";

        return claimsPrincipal.Claims.FirstOrDefault(x => x.Type.Equals("nickname"))?.Value ?? "Not Defined";
    }

    public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal?.Claims == null) return "not@defined.com";

        return claimsPrincipal.Claims.FirstOrDefault(x => x.Type.Equals("name"))?.Value ?? "not@defined.com";
    }

    public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal?.Claims == null) return "not|set";

        return claimsPrincipal.Claims.FirstOrDefault
        (x =>
            x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
        )?.Value ?? "not|set";
    }

    public static string GetPicture(this ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal?.Claims == null) return "https://www.computerhope.com/jargon/g/guest-user.png";

        return claimsPrincipal.Claims.FirstOrDefault(x => x.Type.Equals("picture"))?.Value ??
               "https://www.computerhope.com/jargon/g/guest-user.png";
    }


    public static IEnumerable<IdentityRole> GetIdentityRoles(this ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal?.Claims == null) return new[] { IdentityRole.User };
        var claimsFromProvider = claimsPrincipal.Claims.Where(x =>
            x.Type.Contains("http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));

        return !claimsFromProvider.Any()
            ? new[] { IdentityRole.User }
            : claimsFromProvider.Select(claim => Enum.Parse<IdentityRole>(claim.Value.Replace(" ", ""))).ToArray();
    }

    public static IdentityRole GetIdentityRole(this ClaimsPrincipal claimsPrincipal)
    {
        var allRoles = claimsPrincipal.GetIdentityRoles();

        return allRoles.MaxBy(x => x);
    }

    public static IEnumerable<IdentityRole> GetIdentityRoles(this IUser user)
    {
        return user?.Roles ?? new List<IdentityRole> { IdentityRole.User };
    }

    public static IdentityRole GetIdentityRole(this IUser user)
    {
        var allRoles = user.GetIdentityRoles();

        if (!allRoles.Any()) return IdentityRole.User;

        return allRoles.MaxBy(x => x);
    }
}