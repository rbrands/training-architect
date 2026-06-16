namespace BrandsAdvisory.Models;

/// <summary>
/// Serializable representation of the authenticated user's identity,
/// sent to the Blazor WebAssembly client via <c>/api/user</c>.
/// </summary>
public class UserInfo
{
    public bool IsAuthenticated { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
}
