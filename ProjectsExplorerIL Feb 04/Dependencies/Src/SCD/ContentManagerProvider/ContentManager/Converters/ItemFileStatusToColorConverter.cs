using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using ContentManager.VersionUpdate.ViewModel;

namespace ContentManager.Converters
{
    class ItemFileStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ItemFileStatus)value)
            {
                case ItemFileStatus.New:
                    return Brushes.DarkGreen;

                case ItemFileStatus.Updated:
                    return Brushes.Blue;

                default:
                    return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
