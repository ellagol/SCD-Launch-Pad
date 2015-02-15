using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using ContentManager.ContentManagerMainWindow.ViewModel;

namespace ContentManager.Converters
{
    class UserControlTypeVisibleToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool visible = false;

            switch ((UserControlTypeVisible)value)
            {
                case UserControlTypeVisible.Message:
                    visible = ((string)parameter) == "Message";
                    break;
                case UserControlTypeVisible.ProgressBar:
                    visible = ((string)parameter) == "ProgressBar";
                    break;
                case UserControlTypeVisible.WhereUsed:
                    visible = ((string)parameter) == "WhereUsed";
                    break;
                default:
                    visible = ((string)parameter) == "Na";
                    break;
            }

            return (visible) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
