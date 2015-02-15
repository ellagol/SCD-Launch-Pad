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
    class UserControlTypeVisibleToEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool enable = false;

            switch ((UserControlTypeVisible)value)
            {
                case UserControlTypeVisible.Message:
                    enable = ((string)parameter) == "Message";
                    break;
                case UserControlTypeVisible.ProgressBar:
                    enable = ((string)parameter) == "ProgressBar";
                    break;
                case UserControlTypeVisible.WhereUsed:
                    enable = ((string)parameter) == "WhereUsed";
                    break;
                default:
                    enable = ((string)parameter) == "Na";
                    break;
            }

            return enable;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
