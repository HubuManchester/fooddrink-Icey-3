using FoodieTracker.Models;
using FoodieTracker.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;

namespace FoodieTracker.Views;

[QueryProperty(nameof(ItemId), "id")]
public partial class DetailPage : ContentPage
{
    private FoodEntry? _entry;
    public string ItemId { set => LoadEntry(value); }

    public DetailPage() => InitializeComponent();

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
            try
            {
                await FoodDataService.DeleteAsync(_entry.Id);
                await DisplayAlert("Deleted", "Item removed.", "OK");
                await Shell.Current.GoToAsync(".."); // 返回列表页
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    private void OnVibrateClicked(object sender, EventArgs e) => Vibration.Default.Vibrate(500);
}