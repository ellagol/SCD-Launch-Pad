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
using System.Windows.Media.Animation;

namespace ATSUI.CustomControls
{
    public enum Position : int
    {
        Left,
        Right,
        Top,
        Bottom
    }

    [TemplatePart(Name = "PART_BackButton", Type = typeof(Button)), TemplatePart(Name = "PART_Header", Type = typeof(ContentPresenter))]
    public class Flyout : ContentControl
    {

        public static event EventHandler IsOpenChanged;

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(Flyout), new PropertyMetadata(null));
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(Position), typeof(Flyout), new PropertyMetadata(Position.Left, PositionChanged));
        public static readonly DependencyProperty IsPinnableProperty = DependencyProperty.Register("IsPinnable", typeof(bool), typeof(Flyout), new PropertyMetadata(null));
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(Flyout), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsOpenedChanged));
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(Flyout));

        public DataTemplate HeaderTemplate
        {
            get
            {
                return (DataTemplate)GetValue(HeaderTemplateProperty);
            }
            set
            {
                SetValue(HeaderTemplateProperty, value);
            }
        }

        public bool IsOpen
        {
            get
            {
                return Convert.ToBoolean(GetValue(IsOpenProperty));
            }
            set
            {
                SetValue(IsOpenProperty, value);
            }
        }

        public bool IsPinnable
        {
            get
            {
                return Convert.ToBoolean(GetValue(IsPinnableProperty));
            }
            set
            {
                SetValue(IsPinnableProperty, value);
            }
        }

        public Position position
        {
            get
            {
                return (Position)GetValue(PositionProperty);
            }
            set
            {
                SetValue(PositionProperty, value);
            }
        }

        public string Header
        {
            get
            {
                return Convert.ToString(GetValue(HeaderProperty));
            }
            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        internal static void IsOpenedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (Flyout)dependencyObject;
            VisualStateManager.GoToState(flyout, ((Convert.ToBoolean(e.NewValue) == false) ? "Hide" : "Show"), true);
            if (IsOpenChanged != null)
                IsOpenChanged(flyout, EventArgs.Empty);
        }

        private static void PositionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var flyout = (Flyout)dependencyObject;
            flyout.ApplyAnimation((Position)e.NewValue);
        }

        static Flyout()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Flyout), new FrameworkPropertyMetadata(typeof(Flyout)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ApplyAnimation(position);
        }

        internal void ApplyAnimation(Position position)
        {
            var root = (Grid)GetTemplateChild("root");
            if (root == null)
            {
                return;
            }

            var hideFrame = (EasingDoubleKeyFrame)GetTemplateChild("hideFrame");
            var hideFrameY = (EasingDoubleKeyFrame)GetTemplateChild("hideFrameY");
            var showFrame = (EasingDoubleKeyFrame)GetTemplateChild("showFrame");
            var showFrameY = (EasingDoubleKeyFrame)GetTemplateChild("showFrameY");

            if (hideFrame == null || showFrame == null || hideFrameY == null || showFrameY == null)
            {
                return;
            }

            if (this.position == Position.Left || this.position == Position.Right)
            {
                showFrame.Value = 0;
            }
            if (this.position == Position.Top || this.position == Position.Bottom)
            {
                showFrameY.Value = 0;
            }
            root.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            switch (position)
            {
                case Position.Right:
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                    hideFrame.Value = root.DesiredSize.Width;
                    root.RenderTransform = new TranslateTransform(root.DesiredSize.Width, 0);
                    break;
                case Position.Top:
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    hideFrameY.Value = -root.DesiredSize.Height;
                    root.RenderTransform = new TranslateTransform(0, -root.DesiredSize.Height);
                    break;
                case Position.Bottom:
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    hideFrameY.Value = root.DesiredSize.Height;
                    root.RenderTransform = new TranslateTransform(0, root.DesiredSize.Height);
                    break;
                default:
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                    hideFrame.Value = -root.DesiredSize.Width;
                    root.RenderTransform = new TranslateTransform(-root.DesiredSize.Width, 0);
                    break;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if ((!sizeInfo.WidthChanged) && (!sizeInfo.HeightChanged))
            {
                return;
            }

            if (!IsOpen)
            {
                ApplyAnimation(position);
                return;
            }

            var root = (Grid)GetTemplateChild("root");
            if (root == null)
            {
                return;
            }

            var hideFrame = (EasingDoubleKeyFrame)GetTemplateChild("hideFrame");
            var hideFrameY = (EasingDoubleKeyFrame)GetTemplateChild("hideFrameY");
            var showFrame = (EasingDoubleKeyFrame)GetTemplateChild("showFrame");
            var showFrameY = (EasingDoubleKeyFrame)GetTemplateChild("showFrameY");

            if (hideFrame == null || showFrame == null || hideFrameY == null || showFrameY == null)
            {
                return;
            }

            if (position == Position.Left || position == Position.Right)
            {
                showFrame.Value = 0;
            }
            if (position == Position.Top || position == Position.Bottom)
            {
                showFrameY.Value = 0;
            }

            switch (position)
            {
                case Position.Right:
                    hideFrame.Value = root.DesiredSize.Width;
                    break;
                case Position.Top:
                    hideFrameY.Value = -root.DesiredSize.Height;
                    break;
                case Position.Bottom:
                    hideFrameY.Value = root.DesiredSize.Height;
                    break;
                default:
                    hideFrame.Value = -root.DesiredSize.Width;
                    break;
            }
        }
    }
}
