using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using EasySave.Application.Resources;

namespace EasySave.GUI.Views;

public class BackupTypeConverter : AvaloniaObject, IValueConverter
{
    public static readonly StyledProperty<ITextProvider?> TextsProperty =
        AvaloniaProperty.Register<BackupTypeConverter, ITextProvider?>(nameof(Texts));

    public ITextProvider? Texts
    {
        get => GetValue(TextsProperty);
        set => SetValue(TextsProperty, value);
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (Texts == null || value is not string type) return value;
        return type.ToLower() switch
        {
            "full" => Texts.Full,
            "differential" => Texts.Differential,
            _ => value
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}