using FoodieTracker.Services;

namespace FoodieTracker.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        ThemePicker.SelectedIndex = 0;
        LargeTextSwitch.IsToggled = AccessibilityService.IsLargeTextEnabled;
        // 订阅全局字体改变事件，以便当其他页面（如主页）改变字体时本页也能刷新
        AccessibilityService.LargeTextChanged += OnLargeTextChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // 确保每次显示时应用当前的字体缩放
        AccessibilityService.ApplyFontScale(this);
    }

    private void OnLargeTextChanged(object? sender, EventArgs e)
    {
        // 当全局字体设置改变时，重新应用缩放
        AccessibilityService.ApplyFontScale(this);
        UpdatePreviewText();
    }

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
        // 设置全局静态属性，这会触发 LargeTextChanged 事件
        AccessibilityService.IsLargeTextEnabled = e.Value;
        UpdatePreviewText();
    }

    private void UpdatePreviewText()
    {
        if (LargeTextPreviewTitle != null && LargeTextPreviewBody != null && SettingsStatusLabel != null)
        {
            LargeTextPreviewTitle.Text = AccessibilityService.IsLargeTextEnabled
                ? "Large text preview: enlarged"
                : "Large text preview";
            LargeTextPreviewBody.Text = AccessibilityService.IsLargeTextEnabled
                ? "Text is now noticeably larger. The food and hardware pages will use the same setting."
                : "Turn on the switch to enlarge this preview and other page text.";
            SettingsStatusLabel.Text = AccessibilityService.IsLargeTextEnabled
                ? "Large text ON"
                : "Large text OFF";
            SemanticScreenReader.Announce(SettingsStatusLabel.Text);
        }
    }
}