using System.Linq;
using System.Xml.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Globalization;

namespace ATSUI.Converters
{
        public class ToUpperConverter : MarkupConverter
        {

            protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var val = value as string;
                return ((val != null) ? val.ToUpper() : value);
            }

            protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return Binding.DoNothing;
            }

        }

        public class ToLowerConverter : MarkupConverter
        {

            protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var val = value as string;
                return ((val != null) ? val.ToLower() : value);
            }

            protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return Binding.DoNothing;
            }

        }

}
