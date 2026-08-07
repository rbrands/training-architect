using System.Text;
using System.Xml;
using TrainingArchitect.Core.Interfaces;

namespace TrainingArchitect.Endpoints;

/// <summary>
/// Minimal API endpoints for sitemap and robots metadata.
/// </summary>
public static class SitemapEndpoints
{
    private static readonly string[] PublicPagePaths =
    [
        "/",
        "/blog",
        "/legal",
        "/privacy",
        "/projects"
    ];

    public static IEndpointRouteBuilder MapSitemapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/robots.txt", (HttpContext context) =>
            Results.Content(BuildRobotsText(context), "text/plain; charset=utf-8"))
            .AllowAnonymous()
            .ExcludeFromDescription();

        app.MapGet("/sitemap.xml", async (HttpContext context, IArticleRepository articleRepository) =>
            Results.Content(await BuildSitemapXmlAsync(context, articleRepository), "application/xml; charset=utf-8"))
            .AllowAnonymous()
            .ExcludeFromDescription();

        return app;
    }

    private static string BuildRobotsText(HttpContext context)
    {
        var siteUri = ResolveSiteUri(context);
        var sitemapUri = new Uri(siteUri, "/sitemap.xml");

        var builder = new StringBuilder()
            .AppendLine("User-agent: *")
            .AppendLine("Allow: /")
            .Append("Sitemap: ")
            .AppendLine(sitemapUri.AbsoluteUri);

        return builder.ToString();
    }

    private static async Task<string> BuildSitemapXmlAsync(HttpContext context, IArticleRepository articleRepository)
    {
        var siteUri = ResolveSiteUri(context);
        var entries = new List<SitemapEntry>();

        entries.AddRange(PublicPagePaths.Select(path => new SitemapEntry(new Uri(siteUri, path).AbsoluteUri)));

        var publishedArticles = await articleRepository.GetPublishedAsync();
        foreach (var article in publishedArticles)
        {
            var slug = string.IsNullOrWhiteSpace(article.Slug) ? article.Id : article.Slug;
            var articleUri = new Uri(siteUri, $"/blog/{Uri.EscapeDataString(slug)}").AbsoluteUri;
            var lastModified = article.PublishedDate ?? article.UpdatedAt;
            entries.Add(new SitemapEntry(articleUri, lastModified));
        }

        var xmlSettings = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            Indent = true,
            OmitXmlDeclaration = false
        };

        var stringBuilder = new StringBuilder();
        using var xmlWriter = XmlWriter.Create(stringBuilder, xmlSettings);

        xmlWriter.WriteStartDocument();
        xmlWriter.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

        foreach (var entry in entries)
        {
            xmlWriter.WriteStartElement("url");
            xmlWriter.WriteElementString("loc", entry.Loc);

            if (entry.LastModifiedUtc.HasValue)
            {
                xmlWriter.WriteElementString(
                    "lastmod",
                    XmlConvert.ToString(entry.LastModifiedUtc.Value, XmlDateTimeSerializationMode.Utc));
            }

            xmlWriter.WriteEndElement();
        }

        xmlWriter.WriteEndElement();
        xmlWriter.WriteEndDocument();
        xmlWriter.Flush();

        return stringBuilder.ToString();
    }

    private static Uri ResolveSiteUri(HttpContext context)
    {
        var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
        var configuredSiteUrl = configuration["SiteUrl"];

        if (!string.IsNullOrWhiteSpace(configuredSiteUrl)
            && !configuredSiteUrl.StartsWith("__", StringComparison.Ordinal)
            && Uri.TryCreate(configuredSiteUrl, UriKind.Absolute, out var siteUri))
        {
            return siteUri;
        }

        return new Uri($"{context.Request.Scheme}://{context.Request.Host}");
    }

    private sealed record SitemapEntry(string Loc, DateTime? LastModifiedUtc = null);
}