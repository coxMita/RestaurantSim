using System.Text.Json.Serialization;

public class RecipeStep
{
    [JsonPropertyName("step")]
    public string Step { get; set; }

    [JsonPropertyName("duration")]
    public int Duration { get; set; }
}