namespace BrandsAdvisory.Core.Models;

public class ProfileLink
{
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    // Bootstrap Icon class name, e.g. "bi-envelope", "bi-linkedin"
    // See: https://icons.getbootstrap.com/
    public string Icon { get; set; } = string.Empty;
}
