


using System.Collections.Generic;
using System.Text.Json.Serialization;

public class Recipe
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("difficulty")]
    public string Difficulty { get; set; }

    [JsonPropertyName("equipment")]
    public List<string> Equipment { get; set; }

    [JsonPropertyName("steps")]
    public List<RecipeStep> Steps { get; set; }
}