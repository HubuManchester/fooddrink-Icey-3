using FoodieTracker.Models;
using FoodieTracker.Services;
using FoodieTrackerNew.Services;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace FoodieTracker.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadData();
        AccessibilityService.ApplyFontScale(this);
    }

    private async Task LoadData(string? query = null) =>
        FoodList.ItemsSource = await FoodDataService.SearchAsync(query);

    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e) => await LoadData(e.NewTextValue);
    private async void OnAddClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync(nameof(AddEntryPage));
    private async void OnDetailsClicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.CommandParameter is string id)
            await Shell.Current.GoToAsync($"{nameof(DetailPage)}?id={id}");
    }
}