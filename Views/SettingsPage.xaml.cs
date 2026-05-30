using FoodieTrackerNew.Services;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;

namespace FoodieTracker.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        ThemePicker.SelectedIndex = 0;
        LargeTextSwitch.IsToggled = AccessibilityService.IsLargeTextEnabled;
    }
    protected override void OnAppearing() => ApplyLargeText();

    private void OnThemeChanged(object sender, EventArgs e)
    {
        Application.Current!.UserAppTheme = ThemePicker.SelectedIndex switch
        {
            1 => AppTheme.Light,
            2 => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
    }

    private void OnLargeTextToggled(object sender, ToggledEventArgs e)
    {
        AccessibilityService.IsLargeTextEnabled = e.Value;
        ApplyLargeText();
    }

    private void ApplyLargeText()
    {
        AccessibilityService.ApplyFontScale(this);
        StatusLabel.Text = AccessibilityService.IsLargeTextEnabled ? "Large text ON" : "Normal text";
    }
}