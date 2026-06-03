using FoodieTracker.Services;

namespace FoodieTracker.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        ThemePicker.SelectedIndex = 0;
        LargeTextSwitch.IsToggled = AccessibilityService.IsLargeTextEnabled;
        AccessibilityService.LargeTextChanged += OnLargeTextChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
        UpdateStatusLabel();
    }

    private void OnLargeTextChanged(object? sender, EventArgs e)
    {
        AccessibilityService.ApplyFontScale(this);
        UpdateStatusLabel();
    }

    private void OnThemeChanged(object sender, EventArgs e)
    {
        Application.Current!.UserAppTheme = ThemePicker.SelectedIndex switch
        {
            1 => AppTheme.Light,
            2 => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
        SettingsStatusLabel.Text = $"✅ Theme set to {ThemePicker.SelectedItem}";
        SemanticScreenReader.Announce(SettingsStatusLabel.Text);
    }

    private void OnLargeTextToggled(object sender, ToggledEventArgs e)
    {
        AccessibilityService.IsLargeTextEnabled = e.Value;
        UpdateStatusLabel();
    }

    private void UpdateStatusLabel()
    {
        if (SettingsStatusLabel != null)
        {
            SettingsStatusLabel.Text = AccessibilityService.IsLargeTextEnabled
                ? "🔠 Large text ON"
                : "🔠 Large text OFF";
            SemanticScreenReader.Announce(SettingsStatusLabel.Text);
        }
    }
}