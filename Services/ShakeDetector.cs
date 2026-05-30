using Microsoft.Maui.Devices.Sensors;
using System;

namespace FoodieTracker.Services;

public static class ShakeDetector
{
    private static readonly TimeSpan ShakeCooldown = TimeSpan.FromSeconds(2);
    private static DateTime _lastShakeTime = DateTime.MinValue;

    public static event EventHandler? ShakeDetected;

    public static void Start()
    {
        if (!Accelerometer.Default.IsSupported)
            return;
        Accelerometer.Default.ReadingChanged += OnReadingChanged;
        if (!Accelerometer.Default.IsMonitoring)
            Accelerometer.Default.Start(SensorSpeed.UI);
    }

    public static void Stop()
    {
        Accelerometer.Default.ReadingChanged -= OnReadingChanged;
        if (Accelerometer.Default.IsMonitoring)
            Accelerometer.Default.Stop();
    }

    private static void OnReadingChanged(object? sender, AccelerometerChangedEventArgs e)
    {
        var data = e.Reading.Acceleration;
        var magnitude = Math.Sqrt(data.X * data.X + data.Y * data.Y + data.Z * data.Z);
        if (magnitude > 1.8 && (DateTime.UtcNow - _lastShakeTime) > ShakeCooldown)
        {
            _lastShakeTime = DateTime.UtcNow;
            ShakeDetected?.Invoke(null, EventArgs.Empty);
        }
    }
}