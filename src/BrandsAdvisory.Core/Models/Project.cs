namespace BrandsAdvisory.Core.Models;

public class Project : CosmosDocument
{
    public override string Type => "project";
    public string Title { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
    public List<string> Outcomes { get; set; } = [];
    public DateTime? ProjectStart { get; set; }
}
