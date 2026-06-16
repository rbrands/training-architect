namespace BrandsAdvisory.Core.Models;

/// <summary>Subscription tier that gates feature access.</summary>
public enum AthleteTier
{
    Free,
    Pro,
    Elite
}

/// <summary>
/// Identity and entitlement record for the current athlete.
/// Resolved server-side — never read from client input.
/// </summary>
public class AthleteProfile
{
    /// <summary>Unique identifier sourced from the OIDC subject claim.</summary>
    public string AthleteId { get; set; } = string.Empty;

    /// <summary>Display name from the identity provider.</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Subscription tier.
    /// TODO: Resolve from entitlement store (server-side only).
    /// </summary>
    public AthleteTier Tier { get; set; }
}
