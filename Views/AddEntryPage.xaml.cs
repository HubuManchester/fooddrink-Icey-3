using FoodieTracker.Models;
using FoodieTracker.Services;
using FoodieTrackerNew.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using System;
using System.Formats.Tar;

namespace FoodieTracker.Views;

public partial class AddEntryPage : ContentPage
{
    public AddEntryPage() => InitializeComponent();
    protected override void OnAppearing() => AccessibilityService.ApplyFontScale(this);

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

        await FoodDataService.AddAsync(entry);
        await DisplayAlert("Saved", "Food entry added successfully!", "OK");
        await Shell.Current.GoToAsync("..");
    }

    private bool Validate(out int cal, out int pro, out int carb, out int fat)
    {
        cal = pro = carb = fat = 0;
        if (string.IsNullOrWhiteSpace(NameEntry.Text)) { ShowError("Name required"); return false; }
        if (CategoryPicker.SelectedIndex < 0) { ShowError("Category required"); return false; }
        if (!int.TryParse(CalEntry.Text, out cal) || cal < 0) { ShowError("Valid calories required"); return false; }
        int.TryParse(ProteinEntry.Text, out pro);
        int.TryParse(CarbsEntry.Text, out carb);
        int.TryParse(FatEntry.Text, out fat);
        return true;
    }

    private void ShowError(string msg) { ErrorLabel.Text = msg; ErrorLabel.IsVisible = true; Vibration.Default.Vibrate(200); }
}