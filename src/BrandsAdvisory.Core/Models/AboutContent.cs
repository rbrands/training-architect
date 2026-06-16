namespace BrandsAdvisory.Core.Models;

public class AboutContent : CosmosDocument
{
    public override string Type => "about";
    public string Title { get; set; } = "Site Title";
    public string Subtitle { get; set; } = String.Empty;
    public string ContactHint { get; set; } = string.Empty;
    public string ProfileUrl { get; set; } = String.Empty;
    public string Location { get; set; } = String.Empty;
    public List<ProfileLink> Links { get; set; } = [];
    public List<string> Tags { get; set; } = [];
    public string AvatarUrl { get; set; } = string.Empty;
    public string HeaderTitle { get; set; } = "Header Title";
    public string HeaderSubtitle { get; set; } = "Subtitle";
    public string FooterCopyright { get; set; } = "Brands Advisory";
    public string FooterLinkTitle { get; set; } = string.Empty;
    public string FooterLink { get; set; } = string.Empty;
    public string LegalServiceProviderName { get; set; } = string.Empty;
    public string LegalServiceProviderLocation { get; set; } = string.Empty;
    public string LegalContact { get; set; } = string.Empty;
}
