namespace TrainingArchitect.Core.Models;

public class Article : CosmosDocument
{
    public override string Type => "article";
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime? PublishedDate { get; set; }
    public bool IsPublished { get; set; }
    public List<string> Tags { get; set; } = [];
    public string TitleImageUrl { get; set; } = string.Empty;
    /// <summary>
    /// Optional lifetime in days. Null means the article never expires.
    /// </summary>
    public int? ExpiresAfterDays { get; set; }
}
