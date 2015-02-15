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
using System.Windows.Interactivity;

namespace ATSUI.CustomControls
{
    class SetFlyoutOpenAction : TargetedTriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(bool), typeof(SetFlyoutOpenAction), new PropertyMetadata(null));

        public bool Value
        {
            get
            {
                return Convert.ToBoolean(GetValue(ValueProperty));
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        protected override void Invoke(object parameter)
        {
            ((Flyout)TargetObject).IsOpen = Value;
        }

    }
}
