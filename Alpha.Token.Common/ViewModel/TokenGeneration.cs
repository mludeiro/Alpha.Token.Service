using System.Text.Json.Serialization;

namespace Alpha.Token.Common;

public class TokenGeneration
{
    [JsonPropertyName("token")]
    public string? Token {get; set;}
}