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
using System.Windows.Markup;


namespace ATSUI.Converters
{
        [MarkupExtensionReturnType(typeof(IValueConverter))]
        public abstract class MarkupConverter : MarkupExtension, IValueConverter
        {

            public override object ProvideValue(IServiceProvider serviceProvider)
            {
                return this;
            }

            protected abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
            protected abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

            object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return this.IValueConverter_Convert(value, targetType, parameter, culture);
            }
            private object IValueConverter_Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                try
                {
                    return Convert(value, targetType, parameter, culture);
                }
                catch
                {
                    return DependencyProperty.UnsetValue;
                }
            }

            object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return this.IValueConverter_ConvertBack(value, targetType, parameter, culture);
            }
            private object IValueConverter_ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                try
                {
                    return ConvertBack(value, targetType, parameter, culture);
                }
                catch
                {
                    return DependencyProperty.UnsetValue;
                }
            }
        }
}
