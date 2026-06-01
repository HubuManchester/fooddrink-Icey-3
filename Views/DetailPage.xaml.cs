using FoodieTracker.Models;
using FoodieTracker.Services;
using Microsoft.Maui.Devices;

namespace FoodieTracker.Views;

[QueryProperty(nameof(ItemId), "id")]
public partial class DetailPage : ContentPage
{
    private FoodEntry? _entry;
    public string ItemId { set => LoadEntry(value); }

    public DetailPage()
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

    private async void LoadEntry(string id)
    {
        _entry = await FoodDataService.GetByIdAsync(id);
        if (_entry == null) return;
        NameLabel.Text = _entry.Name;
        CatLabel.Text = _entry.Category;
        CalLabel.Text = _entry.CaloriesLabel;
        MacroLabel.Text = _entry.MacroSummary;
        DescLabel.Text = _entry.Description;
        AllergyLabel.Text = $"⚠️ {_entry.AllergyInfo}";
    }

    private async void OnSpeakClicked(object sender, EventArgs e)
    {
        if (_entry != null)
            await SpeechService.SpeakAsync(_entry.AccessibleSummary);
    }

    private void OnStopClicked(object sender, EventArgs e) => SpeechService.Stop();

    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (_entry != null)
            await Shell.Current.GoToAsync($"{nameof(EditEntryPage)}?id={_entry.Id}");
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_entry == null) return;
        bool confirm = await DisplayAlert("Delete", $"Delete '{_entry.Name}'?", "Yes", "No");
        if (confirm)
        {
            bool success = await FoodDataService.DeleteAsync(_entry.Id);
            if (success)
            {
                await DisplayAlert("Deleted", "Item removed.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
                await DisplayAlert("Error", "Delete failed.", "OK");
        }
    }

    private void OnVibrateClicked(object sender, EventArgs e) => Vibration.Default.Vibrate(500);
}