using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace macaroni_dev.Converters
{
    public class DecimalConverter : IValueConverter
    {
        // Convert from decimal to string for display
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                return decimalValue.ToString("0.##", culture); // Avoid trailing zeros
            }
            return "0";
        }

        // Convert back from string to decimal
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && decimal.TryParse(stringValue, NumberStyles.Any, culture, out var result))
            {
                return result;
            }
            return 0m;
        }
    }
}