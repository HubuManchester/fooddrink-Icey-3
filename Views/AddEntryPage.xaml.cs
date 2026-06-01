using FoodieTracker.Models;
using FoodieTracker.Services;

namespace FoodieTracker.Views;

public partial class AddEntryPage : ContentPage
{
    public AddEntryPage()
    {
        InitializeComponent();
        AccessibilityService.LargeTextChanged += OnLargeTextChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
    }

    private void OnLargeTextChanged(object? sender, EventArgs e) =>
        AccessibilityService.ApplyFontScale(this);

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (!Validate(out int calories, out int protein, out int carbs, out int fat))
            return;

        var entry = new FoodEntry
        {
            Name = NameEntry.Text!.Trim(),
            Category = CategoryPicker.SelectedItem?.ToString() ?? "Snack",
            Description = DescEditor.Text?.Trim() ?? "",
            Calories = calories,
            Protein = protein,
            Carbs = carbs,
            Fat = fat,
            AllergyInfo = string.IsNullOrWhiteSpace(AllergyEntry.Text) ? "None" : AllergyEntry.Text.Trim()
        };

        try
        {
            var created = await FoodDataService.AddAsync(entry);
            if (created != null && !string.IsNullOrEmpty(created.Id))
            {
                await DisplayAlert("Success", "Added successfully!", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
                await DisplayAlert("Error", "Server did not return valid ID.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private bool Validate(out int cal, out int pro, out int carb, out int fat)
    {
        cal = pro = carb = fat = 0;
        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        { ShowError("Name required"); return false; }
        if (CategoryPicker.SelectedIndex < 0)
        { ShowError("Category required"); return false; }
        if (!int.TryParse(CalEntry.Text, out cal) || cal < 0)
        { ShowError("Valid calories required"); return false; }
        int.TryParse(ProteinEntry.Text, out pro);
        int.TryParse(CarbsEntry.Text, out carb);
        int.TryParse(FatEntry.Text, out fat);
        return true;
    }

    private void ShowError(string msg)
    {
        ErrorLabel.Text = msg;
        ErrorLabel.IsVisible = true;
    }
}