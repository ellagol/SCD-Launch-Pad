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
using System.Collections.ObjectModel;

namespace ATSUI.CustomControls
{
    public class EnhancedWindowBase : Window
    {
        public static readonly DependencyProperty FlyoutsProperty = DependencyProperty.Register("Flyouts", typeof(FreezableCollection<Flyout>), typeof(EnhancedWindowBase), new PropertyMetadata(null));

        public FreezableCollection<Flyout> Flyouts
        {
            get
            {
                return (FreezableCollection<Flyout>)GetValue(FlyoutsProperty);
            }
            set
            {
                SetValue(FlyoutsProperty, value);
            }
        }

        public EnhancedWindowBase()
        {
            Flyouts = new FreezableCollection<Flyout>();
            Loaded += EnhancedWindowBase_Loaded;
        }

        private void EnhancedWindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "AfterLoaded", true);
        }

    }
}
