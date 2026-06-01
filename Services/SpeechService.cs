using Microsoft.Maui.Media;

namespace FoodieTracker.Services;

public static class SpeechService
{
    private static CancellationTokenSource? _currentSpeech;
    public static bool IsAvailable { get; private set; } = true;

    public static async Task SpeakAsync(string text)
    {
        if (!IsAvailable)
        {
            await ShowTtsUnavailableAlert();
            return;
        }

        Stop();
        _currentSpeech = new CancellationTokenSource();
        try
        {
            await TextToSpeech.Default.SpeakAsync(text, cancelToken: _currentSpeech.Token);
        }
        catch (FeatureNotSupportedException)
        {
            IsAvailable = false;
            await ShowTtsUnavailableAlert();
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            await ShowErrorAlert($"Speech failed: {ex.Message}");
        }
    }

    private static async Task ShowTtsUnavailableAlert()
    {
        var page = GetCurrentPage();
        if (page != null)
        {
            await page.DisplayAlert(
                "Text-to-Speech Unavailable",
                "Your device does not have a text-to-speech engine installed or enabled.",
                "OK");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Cannot show TTS alert: no active page.");
        }
    }

    private static async Task ShowErrorAlert(string message)
    {
        var page = GetCurrentPage();
        if (page != null)
        {
            await page.DisplayAlert("Error", message, "OK");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"Error alert: {message}");
        }
    }

    private static Page? GetCurrentPage()
    {
        return Application.Current?.MainPage ?? Shell.Current?.CurrentPage;
    }

    public static void Stop()
    {
        if (_currentSpeech != null)
        {
            _currentSpeech.Cancel();
            _currentSpeech.Dispose();
            _currentSpeech = null;
        }
    }
}