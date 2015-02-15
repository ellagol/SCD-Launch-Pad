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
    [ValueConversion(typeof(int), typeof(Thickness))]
    public class IntToThicknessConverter : IValueConverter
    {

        #region  IValueConverter implementation

        public object Convert(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            if (Value == null)
            {
                return new Thickness(0, 0, 0, 0);
            }

            //if (Parameter == null)
            //{
            //    return Binding.DoNothing;
            //}

            int thicknessValue = (int)Value;
            return new Thickness(thicknessValue, thicknessValue, thicknessValue, thicknessValue);

        }

        public object ConvertBack(object Value, Type TargetType, object Parameter, CultureInfo Culture)
        {
            throw new NotImplementedException("Not implemented");
        }

        #endregion

    }
}
