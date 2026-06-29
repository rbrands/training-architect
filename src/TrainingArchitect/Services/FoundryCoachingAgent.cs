using Azure.AI.Projects;
using OpenAI.Responses;

namespace TrainingArchitect.Services;

#pragma warning disable OPENAI001

public sealed class FoundryCoachingAgent(AIProjectClient projectClient, IConfiguration configuration) : ICoachingAgent
{
    private readonly AIProjectClient _projectClient = projectClient;
    private readonly string _agentName = configuration["FoundryProjectAgentName"]
        ?? throw new InvalidOperationException("Configuration key 'FoundryProjectAgentName' is required.");

    public async Task<string> PromptAsync(
        string prompt,
        string discipline,
        string language,
        CancellationToken ct = default)
    {
        var responseClient = _projectClient.ProjectOpenAIClient
            .GetProjectResponsesClientForModel(_agentName);

        var options = new CreateResponseOptions
        {
            Instructions = $"Discipline: {discipline}\nRespond in language: {language}"
        };

        options.InputItems.Add(ResponseItem.CreateUserMessageItem(prompt));

        var response = await responseClient.CreateResponseAsync(options, ct);
        return response.Value.GetOutputText();
    }
}

#pragma warning restore OPENAI001