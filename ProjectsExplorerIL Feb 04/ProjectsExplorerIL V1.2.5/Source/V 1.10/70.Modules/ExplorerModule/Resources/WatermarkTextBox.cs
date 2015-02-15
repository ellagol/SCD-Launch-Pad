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
    class WatermarkTextBox
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

        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(WatermarkTextBox), new UIPropertyMetadata(null, WatermarkChanged));

        public static bool GetShowWatermark(DependencyObject obj)
        {
            return Convert.ToBoolean(obj.GetValue(ShowWatermarkProperty));
        }

        public static void SetShowWatermark(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowWatermarkProperty, value);
        }

        public static readonly DependencyProperty ShowWatermarkProperty = DependencyProperty.RegisterAttached("ShowWatermark", typeof(bool), typeof(WatermarkTextBox), new UIPropertyMetadata(false));

        private static void WatermarkChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var box = obj as TextBox;

            CheckShowWatermark(box);

            if (!IsInitialized)
            {
                box.TextChanged += box_TextChanged;
                box.Unloaded += box_Unloaded;
                IsInitialized = true;
            }
        }

        private static void CheckShowWatermark(TextBox box)
        {
            box.SetValue(WatermarkTextBox.ShowWatermarkProperty, box.Text == string.Empty);
        }

        private static void box_TextChanged(object sender, TextChangedEventArgs e)
        {
            var box = sender as TextBox;
            CheckShowWatermark(box);
        }

        private static void box_Unloaded(object sender, RoutedEventArgs e)
        {
            var box = sender as TextBox;
            box.TextChanged -= box_TextChanged;
            IsInitialized = false;
        }
    }
}
