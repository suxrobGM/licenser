using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Sms.Licensing.Client.Activator.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            var boolValue = (bool)value;

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            var visibilityValue = (Visibility) value;
            return visibilityValue == Visibility.Visible;
        }
    }
}
