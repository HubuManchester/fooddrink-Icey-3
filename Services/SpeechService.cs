using Microsoft.Maui.Media;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FoodieTracker.Services;

public static class SpeechService
{
    private static CancellationTokenSource? _cts;
    public static async Task SpeakAsync(string text)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        try
        {
            await TextToSpeech.Default.SpeakAsync(text, cancelToken: _cts.Token);
        }
        catch (OperationCanceledException) { }
    }
    public static void Stop() { _cts?.Cancel(); _cts = null; }
}