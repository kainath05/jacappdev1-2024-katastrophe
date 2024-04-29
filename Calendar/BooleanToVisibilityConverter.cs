using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Calendar
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convert the boolean to Visibility.
            if (value is bool && (bool)value)
            {
                return Visibility.Visible;
            }
            else
            {
                // Check if the converter is used with a parameter to invert the logic
                if (parameter is string paramStr && paramStr.Equals("Inverse", StringComparison.OrdinalIgnoreCase))
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convert Visibility back to boolean.
            if (value is Visibility && (Visibility)value == Visibility.Visible)
            {
                return true;
            }
            return false;
        }
    }
}

