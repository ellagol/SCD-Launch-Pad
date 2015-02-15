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
using System.Text;

namespace ATSUI.Converters
{
        [ValueConversion(typeof(Boolean), typeof(Visibility))]
        public class BoolToVisibilityConverter : IValueConverter
        {

            #region  IValueConverter implementation

            public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
            {
                if (Value == null)
                {
                    return Binding.DoNothing;
                }

                if (Parameter == null)
                {
                    return Binding.DoNothing;
                }

                bool Input = false;
                Boolean.TryParse(Value.ToString(), out Input);

                bool InvertActive = false;
                Boolean.TryParse(Parameter.ToString(), out InvertActive);

                if (Input)
                {
                    return (InvertActive ? Visibility.Visible : Visibility.Collapsed);
                }
                else
                {
                    return (InvertActive ? Visibility.Collapsed : Visibility.Visible);
                }
            }

            public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
            {
                throw new NotImplementedException("Not implemented");
            }

            #endregion

        }
}
