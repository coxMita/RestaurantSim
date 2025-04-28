using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("JSON File Reader Test");
        
        // Try multiple possible locations
        string[] possiblePaths = new[]
        {
            "ExerciseJSON.json",
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExerciseJSON.json"),
            Path.Combine(Directory.GetCurrentDirectory(), "ExerciseJSON.json"),
            Path.Combine("..", "ExerciseJSON.json"),
            Path.Combine("RestaurantSimulator", "ExerciseJSON.json")
        };
        
        foreach (var path in possiblePaths)
        {
            Console.WriteLine($"Checking path: {path}");
            if (File.Exists(path))
            {
                Console.WriteLine($"Found JSON file at: {path}");
                try
                {
                    string jsonContent = await File.ReadAllTextAsync(path);
                    Console.WriteLine($"File content length: {jsonContent.Length} characters");
                    Console.WriteLine("First 100 characters:");
                    Console.WriteLine(jsonContent.Substring(0, Math.Min(100, jsonContent.Length)));
                    
                    // Try to parse it
                    using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                    {
                        Console.WriteLine("Successfully parsed JSON!");
                        
                        // Count recipes
                        if (doc.RootElement.TryGetProperty("recipes", out var recipesElement))
                        {
                            int recipeCount = 0;
                            foreach (var _ in recipesElement.EnumerateArray())
                            {
                                recipeCount++;
                            }
                            Console.WriteLine($"Found {recipeCount} recipes in the JSON file");
                        }
                        else
                        {
                            Console.WriteLine("No 'recipes' property found in JSON");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading/parsing file: {ex.Message}");
                }
                
                break;
            }
        }
        
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
