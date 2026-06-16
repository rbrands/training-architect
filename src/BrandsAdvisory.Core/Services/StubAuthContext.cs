using BrandsAdvisory.Core.Interfaces;
using BrandsAdvisory.Core.Models;

namespace BrandsAdvisory.Core.Services;

/// <summary>
/// Development stub for <see cref="IAuthContext"/>.
/// Returns a hardcoded athlete for local development.
/// TODO: Replace with OIDC-backed implementation that reads the subject claim
///       from <c>IHttpContextAccessor</c> and resolves tier from the entitlement store.
/// </summary>
public sealed class StubAuthContext : IAuthContext
{
    public Task<AthleteProfile?> GetAthleteAsync(CancellationToken cancellationToken = default)
    {
        // TODO: Read subject claim from authenticated HTTP context.
        // TODO: Look up AthleteTier from entitlement store — never trust client input.
        var profile = new AthleteProfile
        {
            AthleteId = "dev-athlete-001",
            DisplayName = "Dev Athlete",
            Tier = AthleteTier.Pro
        };

        return Task.FromResult<AthleteProfile?>(profile);
    }
}
