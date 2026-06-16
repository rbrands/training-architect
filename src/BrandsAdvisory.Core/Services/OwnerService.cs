using BrandsAdvisory.Core.Interfaces;
using System.Security.Claims;

namespace BrandsAdvisory.Core.Services;

public class OwnerService : IOwnerService
{
    public bool IsOwner(ClaimsPrincipal user)
    {
        if (!user.Identity?.IsAuthenticated ?? true)
            return false;

        return user.IsInRole("SiteAdmin");
    }
}
