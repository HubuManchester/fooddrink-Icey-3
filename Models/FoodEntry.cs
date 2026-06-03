using System.Text.Json.Serialization;

namespace FoodieTracker.Models;

public class FoodEntry
{
    [JsonPropertyName("ID")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("calories")]
    public int Calories { get; set; }

    [JsonPropertyName("protein")]
    public int Protein { get; set; }

    [JsonPropertyName("carbs")]
    public int Carbs { get; set; }

    [JsonPropertyName("fat")]
    public int Fat { get; set; }

    [JsonPropertyName("allergyInfo")]
    public string AllergyInfo { get; set; } = "None";

    [JsonIgnore]
    public string CaloriesLabel => $"{Calories} kcal";

    [JsonIgnore]
    public string MacroSummary => $"P {Protein}g / C {Carbs}g / F {Fat}g";

    [JsonIgnore]
    public string AccessibleSummary => $"{Name}, {Category}, {Calories} calories, {MacroSummary}, Allergy: {AllergyInfo}";
}