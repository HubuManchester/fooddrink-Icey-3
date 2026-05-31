using FoodieTracker.Models;
using FoodieTracker.Services;

namespace FoodieTracker.Views;

[QueryProperty(nameof(ItemId), "id")]
public partial class EditEntryPage : ContentPage
{
    private FoodEntry? _entry;
    public string ItemId { set => LoadEntry(value); }

    public EditEntryPage() => InitializeComponent();

    private async void LoadEntry(string id)
    {
        _entry = await FoodDataService.GetByIdAsync(id);
        if (_entry == null) return;

        NameEntry.Text = _entry.Name;
        CategoryPicker.SelectedItem = _entry.Category;
        DescEditor.Text = _entry.Description;
        CalEntry.Text = _entry.Calories.ToString();
        ProteinEntry.Text = _entry.Protein.ToString();
        CarbsEntry.Text = _entry.Carbs.ToString();
        FatEntry.Text = _entry.Fat.ToString();
        AllergyEntry.Text = _entry.AllergyInfo;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (_entry == null) return;

        if (!Validate(out int cal, out int pro, out int carb, out int fat))
            return;

        _entry.Name = NameEntry.Text?.Trim() ?? "";
        _entry.Category = CategoryPicker.SelectedItem?.ToString() ?? "Snack";
        _entry.Description = DescEditor.Text?.Trim() ?? "";
        _entry.Calories = cal;
        _entry.Protein = pro;
        _entry.Carbs = carb;
        _entry.Fat = fat;
        _entry.AllergyInfo = AllergyEntry.Text?.Trim() ?? "None";

        try
        {
            await FoodDataService.UpdateAsync(_entry);
            await DisplayAlert("Success", "Entry updated!", "OK");
            await Shell.Current.GoToAsync("..");
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

    private void ShowError(string msg) => ErrorLabel.Text = msg;
}