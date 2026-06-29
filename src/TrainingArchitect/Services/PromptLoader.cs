using System.Collections.Concurrent;
using System.Reflection;

namespace TrainingArchitect.Services;
public static class PromptLoader
{
    private static readonly Assembly _assembly = typeof(PromptLoader).Assembly;
    private static readonly ConcurrentDictionary<string, string> _cache = new();

    public static string Load(string promptName) =>
        _cache.GetOrAdd(promptName, name =>
        {
            var resourceName = $"TrainingArchitect.Server.Prompts.{name}.md";
            using var stream = _assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException($"Prompt '{name}' not found.");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        });
}