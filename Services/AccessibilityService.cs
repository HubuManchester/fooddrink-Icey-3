using System.Runtime.CompilerServices;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace FoodieTracker.Services;

public static class AccessibilityService
{
    private const double LargeFontScale = 1.28;
    private class FontSizeHolder { public double Value { get; set; } }
    private static readonly ConditionalWeakTable<VisualElement, FontSizeHolder> OriginalFontSizes = new();

    private static bool _isLargeTextEnabled = false;
    public static bool IsLargeTextEnabled
    {
        get => _isLargeTextEnabled;
        set
        {
            if (_isLargeTextEnabled == value) return;
            _isLargeTextEnabled = value;
            LargeTextChanged?.Invoke(null, EventArgs.Empty);
        }
    }

    public static event EventHandler? LargeTextChanged;

    public static void ApplyFontScale(Element root)
    {
        if (root is not IVisualTreeElement visualRoot) return;
        ApplyToVisualTree(visualRoot);
        foreach (var child in visualRoot.GetVisualChildren())
            ApplyToVisualTree(child);
    }

    private static void ApplyToVisualTree(IVisualTreeElement element)
    {
        if (element is VisualElement visual)
            ApplyToVisualElement(visual);

        foreach (var child in element.GetVisualChildren())
            ApplyToVisualTree(child);
    }

    private static void ApplyToVisualElement(VisualElement visual)
    {
        double scale = IsLargeTextEnabled ? LargeFontScale : 1.0;
        switch (visual)
        {
            case Label label:
                label.FontSize = GetOriginalFontSize(label, label.FontSize) * scale;
                break;
            case Button button:
                button.FontSize = GetOriginalFontSize(button, button.FontSize) * scale;
                break;
            case Entry entry:
                entry.FontSize = GetOriginalFontSize(entry, entry.FontSize) * scale;
                break;
            case Editor editor:
                editor.FontSize = GetOriginalFontSize(editor, editor.FontSize) * scale;
                break;
            case Picker picker:
                picker.FontSize = GetOriginalFontSize(picker, picker.FontSize) * scale;
                break;
        }
    }

    private static double GetOriginalFontSize(VisualElement control, double currentSize)
    {
        if (!OriginalFontSizes.TryGetValue(control, out var holder))
        {
            double original = (currentSize > 0 && currentSize <= 100) ? currentSize : 14.0;
            holder = new FontSizeHolder { Value = original };
            OriginalFontSizes.Add(control, holder);
        }
        return holder.Value;
    }
}