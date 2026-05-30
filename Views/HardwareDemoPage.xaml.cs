using FoodieTracker.Services;
using Microsoft.Maui.Accessibility;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Media;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FoodieTracker.Views;

public partial class HardwareDemoPage : ContentPage
{
    public HardwareDemoPage()
    {
        InitializeComponent();
        ShakeDetector.ShakeDetected += OnShakeDetected;
    }

    protected override void OnAppearing() => ShakeDetector.Start();
    protected override void OnDisappearing() => ShakeDetector.Stop();

    private async void OnPhotoClicked(object sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo != null)
                PhotoPreview.Source = ImageSource.FromStream(() => photo.OpenReadAsync().Result);
        }
        catch (PermissionException) { SetStatus("Camera permission denied"); }
    }

    private async void OnLocationClicked(object sender, EventArgs e)
    {
        try
        {
            var location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));
            if (location != null)
                LocationText.Text = $"Lat: {location.Latitude:F4}, Lon: {location.Longitude:F4}";
            else
                LocationText.Text = "Location not available";
        }
        catch (PermissionException) { SetStatus("Location permission denied"); }
    }

    private async void OnReadHelpClicked(object sender, EventArgs e) =>
        await SpeechService.SpeakAsync("FoodieTracker helps you log meals and use device hardware like camera, location, and shake gesture.");

    private void OnHapticClicked(object sender, EventArgs e)
    {
        Vibration.Default.Vibrate(300);
        HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
        SetStatus("Vibration + haptic triggered");
    }

    private async void OnShakeDemoClicked(object sender, EventArgs e) =>
        await ShowRandomRecommendation();

    private async void OnShakeDetected(object? sender, EventArgs e) =>
        await ShowRandomRecommendation();

    private async Task ShowRandomRecommendation()
    {
        var all = await FoodDataService.GetAllAsync();
        if (all.Any())
        {
            var random = all.OrderBy(x => Guid.NewGuid()).First();
            ShakeResultLabel.Text = $"🥗 Shake recommendation: {random.Name} – {random.CaloriesLabel}";
            await SpeechService.SpeakAsync($"Try {random.Name}, it has {random.CaloriesLabel}");
        }
        else
            ShakeResultLabel.Text = "No entries yet. Add some foods first!";
        SetStatus("Shake detected!");
    }

    private void SetStatus(string msg) { StatusLabel.Text = msg; SemanticScreenReader.Announce(msg); }
}