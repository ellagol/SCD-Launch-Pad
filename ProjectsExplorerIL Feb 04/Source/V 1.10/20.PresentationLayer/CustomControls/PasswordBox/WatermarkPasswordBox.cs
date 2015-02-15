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
using System.Text;

namespace ATSUI.CustomControls
{
    class WatermarkPasswordBox
    {
        private static bool IsInitialized = false;

        public static string GetWatermark(DependencyObject obj)
        {
            return Convert.ToString(obj.GetValue(WatermarkProperty));
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(WatermarkPasswordBox), new UIPropertyMetadata(null, WatermarkChanged));

        public static bool GetShowWatermark(DependencyObject obj)
        {
            return Convert.ToBoolean(obj.GetValue(ShowWatermarkProperty));
        }

        public static void SetShowWatermark(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowWatermarkProperty, value);
        }

        public static readonly DependencyProperty ShowWatermarkProperty = DependencyProperty.RegisterAttached("ShowWatermark", typeof(bool), typeof(WatermarkPasswordBox), new UIPropertyMetadata(false));

        private static void WatermarkChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var pwd = obj as PasswordBox;

            CheckShowWatermark(pwd);

            if (!IsInitialized)
            {
                pwd.PasswordChanged += pwd_PasswordChanged;
                pwd.Unloaded += pwd_Unloaded;
                IsInitialized = true;
            }

        }

        private static void CheckShowWatermark(PasswordBox pwd)
        {
            pwd.SetValue(WatermarkPasswordBox.ShowWatermarkProperty, pwd.Password == string.Empty);
        }

        private static void pwd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var pwd = sender as PasswordBox;
            CheckShowWatermark(pwd);
        }

        private static void pwd_Unloaded(object sender, RoutedEventArgs e)
        {
            var pwd = sender as PasswordBox;
            pwd.PasswordChanged -= pwd_PasswordChanged;
            IsInitialized = false;
        }
    }
}
