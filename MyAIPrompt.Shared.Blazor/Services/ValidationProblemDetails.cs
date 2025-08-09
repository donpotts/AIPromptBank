using System.Text.Json.Serialization;

namespace MyAIPrompt.Shared.Blazor.Services;

internal class ValidationProblemDetails : ProblemDetails
{
    [JsonPropertyName("errors")]
    public Dictionary<string, List<string>>? Errors { get; set; }
}
