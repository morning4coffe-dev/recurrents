﻿using Microsoft.UI.Xaml.Data;

namespace ProjectSBS.Business.Converters;

public class StringToVisibilityConverter : IValueConverter
{

    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string stringValue)
        {
            return string.IsNullOrEmpty(stringValue) ? Visibility.Collapsed : Visibility.Visible;
        }

        return Visibility.Visible;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException("Two-way StringToVisibilityConverter is not supported.");
    }
}