using System.Text.Json.Serialization;

namespace archerly.core;

public class ApiError
{
    [JsonPropertyName("code")]
    public string Code { get; init; }
    [JsonPropertyName("message")]
    public string Message { get; init; }
    [JsonPropertyName("details")]
    public Dictionary<string, object?> Details { get; init; }

    public ApiError(string code, string message)
    {
        Code = code;
        Message = message;
        Details = new();
    }

    public void MergeDetails(IDetailProvider provider)
    {
        if (provider == null || provider.Details == null)
        {
            return;
        }

        foreach (var kvp in provider.Details)
        {
            // Only add if the key does not already exist
            if (!Details.ContainsKey(kvp.Key))
            {
                Details[kvp.Key] = kvp.Value;
            }
        }
    }
}