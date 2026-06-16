using BrandsAdvisory.Core.Models;

namespace BrandsAdvisory.Core.Interfaces;

/// <summary>
/// Provides the authenticated athlete's identity and subscription entitlement.
/// Resolved server-side — the tier must never be read from client-supplied input.
/// </summary>
public interface IAuthContext
{
    /// <summary>
    /// Returns the current athlete's profile, or <c>null</c> when the request
    /// is unauthenticated.
    /// TODO: Resolve AthleteId from OIDC subject claim and look up tier from
    ///       the entitlement store (server-side only).
    /// </summary>
    Task<AthleteProfile?> GetAthleteAsync(CancellationToken cancellationToken = default);
}
