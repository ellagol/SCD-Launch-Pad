using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ContentManager.Converters
{
    class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool input;

            if (value == null)
                input = false;
            else
                input = (bool) value;

            if (parameter != null && ((string) parameter) == "[Not]")
                input = !input;

            return (input) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
