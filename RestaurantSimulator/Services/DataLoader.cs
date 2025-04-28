using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class DataLoader
{
    public List<Ingredient> Ingredients { get; private set; }
    public List<Recipe> Recipes { get; private set; }

    public void LoadData(string filePath)
    {
        var jsonString = File.ReadAllText(filePath);
        var document = JsonDocument.Parse(jsonString);

        Ingredients = document.RootElement.GetProperty("ingredients")
            .Deserialize<List<Ingredient>>();

        Recipes = document.RootElement.GetProperty("recipes")
            .Deserialize<List<Recipe>>();
        Console.WriteLine($"Loaded {Recipes.Count} recipes.");
    }
}
