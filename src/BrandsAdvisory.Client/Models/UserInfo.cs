namespace BrandsAdvisory.Client.Models;

/// <summary>
/// Serialized representation of the authenticated user returned by <c>/api/user</c>.
/// Mirrors <c>BrandsAdvisory.Models.UserInfo</c> on the server.
/// </summary>
public class UserInfo
{
    public bool IsAuthenticated { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
}
