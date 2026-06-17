using TrainingArchitect.Core.Interfaces;
using System.Security.Claims;

namespace TrainingArchitect.Core.Services;

public class OwnerService : IOwnerService
{
    public bool IsOwner(ClaimsPrincipal user)
    {
        if (!user.Identity?.IsAuthenticated ?? true)
            return false;

        return user.IsInRole("SiteAdmin");
    }
}
