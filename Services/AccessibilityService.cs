using System.Runtime.CompilerServices;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace FoodieTrackerNew.Services;

public static class AccessibilityService
{
    private const double LargeFontScale = 1.28;

    // 包装类，解决 ConditionalWeakTable 要求 TValue 为引用类型的问题
    private class FontSizeHolder
    {
        public double Value { get; set; }
    }

    private static readonly ConditionalWeakTable<VisualElement, FontSizeHolder> OriginalFontSizes = new();

    public static bool IsLargeTextEnabled { get; set; }

    public static void ApplyFontScale(Element root)
    {
        if (root is not IVisualTreeElement visualRoot) return;
        ApplyToVisualElement(visualRoot);

        foreach (var child in visualRoot.GetVisualChildren())
        {
            if (child is Element element)
                ApplyFontScale(element);
        }
    }

    private static void ApplyToVisualElement(IVisualTreeElement element)
    {
        if (element is not VisualElement visual) return;

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