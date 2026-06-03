using FoodieTracker.Models;
using FoodieTracker.Services;

namespace FoodieTracker.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        AccessibilityService.LargeTextChanged += OnLargeTextChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadFoodItemsAsync(SearchBar.Text);
        AccessibilityService.ApplyFontScale(this);
    }

    private async void OnLargeTextChanged(object? sender, EventArgs e)
    {
        await LoadFoodItemsAsync(SearchBar.Text);
        AccessibilityService.ApplyFontScale(this);
    }

    private async void OnRefreshViewRefreshing(object? sender, EventArgs e)
    {
        try
        {
            await FoodDataService.RefreshCacheAsync();
            await LoadFoodItemsAsync(SearchBar.Text);
            SemanticScreenReader.Announce("List refreshed with latest data");
        }
        finally
        {
            if (sender is RefreshView refreshView)
                refreshView.IsRefreshing = false;
        }
    }

    private async Task LoadFoodItemsAsync(string? query = null) =>
        FoodList.ItemsSource = await FoodDataService.SearchAsync(query);

    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e) =>
        await LoadFoodItemsAsync(e.NewTextValue);

    private async void OnAddClicked(object sender, EventArgs e) =>
        await Shell.Current.GoToAsync(nameof(AddEntryPage));

    private async void OnDetailsClicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.CommandParameter is string id)
            await Shell.Current.GoToAsync($"{nameof(DetailPage)}?id={id}");
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.CommandParameter is string id)
            await Shell.Current.GoToAsync($"{nameof(EditEntryPage)}?id={id}");
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.CommandParameter is string id)
        {
            bool confirm = await DisplayAlert("Delete", "Are you sure you want to delete this item?", "Yes", "No");
            if (confirm)
            {
                try
                {
                    bool success = await FoodDataService.DeleteAsync(id);
                    if (success)
                    {
                        await FoodDataService.RefreshCacheAsync();
                        await LoadFoodItemsAsync(SearchBar.Text);
                        SemanticScreenReader.Announce("Item deleted");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Delete failed. Please try again.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
            }
        }
    }
}