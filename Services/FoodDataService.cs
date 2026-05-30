using FoodieTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodieTracker.Services;

public static class FoodDataService
{
    private static readonly List<FoodEntry> _localData = new()
    {
        new() { Name = "Avocado Toast", Category = "Breakfast", Description = "Sourdough with smashed avocado, chili flakes", Calories = 320, Protein = 8, Carbs = 28, Fat = 18, AllergyInfo = "Gluten (optional)" },
        new() { Name = "Quinoa Salad", Category = "Lunch", Description = "Quinoa, cucumber, tomato, feta, lemon vinaigrette", Calories = 410, Protein = 12, Carbs = 45, Fat = 18, AllergyInfo = "Dairy (feta)" },
        new() { Name = "Green Smoothie", Category = "Drink", Description = "Spinach, banana, almond milk, protein powder", Calories = 210, Protein = 15, Carbs = 30, Fat = 5, AllergyInfo = "Nuts (almond)" }
    };

    public static Task<IReadOnlyList<FoodEntry>> GetAllAsync() => Task.FromResult<IReadOnlyList<FoodEntry>>(_localData);

    public static Task<IReadOnlyList<FoodEntry>> SearchAsync(string? query)
    {
        var items = _localData;
        if (string.IsNullOrWhiteSpace(query))
            return Task.FromResult<IReadOnlyList<FoodEntry>>(items);
        var filtered = items.Where(x => x.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
                                     || x.Category.Contains(query, StringComparison.OrdinalIgnoreCase)
                                     || x.Description.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        return Task.FromResult<IReadOnlyList<FoodEntry>>(filtered);
    }

    public static Task<FoodEntry?> GetByIdAsync(string id) =>
        Task.FromResult(_localData.FirstOrDefault(x => x.Id == id));

    public static Task AddAsync(FoodEntry entry)
    {
        _localData.Add(entry);
        return Task.CompletedTask;
    }
}