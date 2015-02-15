using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Reflection;
using System.Collections.ObjectModel;
using System.IO;
using ATSBusinessObjects;



namespace ExplorerModule
{
    
    public class ActiveVersionsItemBehavior
    {
        #region IsListViewLinkedVersionsItemBehavior

        public static bool GetIsActiveVersionsItemBehavior(ListViewItem listViewItem)
        {
            return (bool)listViewItem.GetValue(ActiveVersionsItemBehaviorProperty);
        }

        public static void SetIsActiveVersionsItemBehavior(ListViewItem listViewItem, bool value)
        {
            listViewItem.SetValue(ActiveVersionsItemBehaviorProperty, value);
        }

        public static readonly DependencyProperty ActiveVersionsItemBehaviorProperty =
            DependencyProperty.RegisterAttached(
                "IsActiveVersionsItemBehavior",
                //typeof(ICommand),
                typeof(bool),
                typeof(ActiveVersionsItemBehavior),
                new UIPropertyMetadata(false, OnIsActiveVersionsItemBehaviorBehaviorOccur));

        private static void OnIsActiveVersionsItemBehaviorBehaviorOccur(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListViewItem item = d as ListViewItem;
            if (item == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
            {
                item.KeyDown += OnListViewItemKeyDown;
                //item.MouseMove += OnListViewItemMouseMove;
                //item.Selected += OnListViewItemSelected;
            }
            else
            {
                item.KeyDown -= OnListViewItemKeyDown;
                //item.MouseMove -= OnListViewItemMouseMove;
                //item.Selected -= OnListViewItemSelected;
            }
        }



        private static void OnListViewItemKeyDown(object sender, KeyEventArgs e)
        {
            ////Pressed V + Ctrl
            // if((e.Key == Key.V) && ( Keyboard.IsKeyUp(Key.LeftCtrl) || Keyboard.IsKeyUp(Key.LeftCtrl)) )
            // {
            // }
            //Pressed Ctrl + V
             if (((e.Key == Key.LeftCtrl) || (e.Key == Key.RightCtrl)) && Keyboard.IsKeyUp(Key.V))
             {
             }
            if (e.Key == Key.D0 || e.Key == Key.D1 || e.Key == Key.D2 || e.Key == Key.D3 || e.Key == Key.D4 || e.Key == Key.D5 || e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9)
            {
              
                object sourceNode = ((ListViewItem)sender).DataContext;

                if (sourceNode.GetType() == typeof(ContentModel))
                {

                }
            }
            e.Handled = true;
        }

        #endregion // IsBroughtIntoViewWhenDrop     
 



    //    public static DependencyProperty CommandProperty =
    //    DependencyProperty.RegisterAttached("Command",
    //    typeof(ICommand),
    //    typeof(MouseDoubleClick),
    //    new UIPropertyMetadata(CommandChanged));

    //public static DependencyProperty CommandParameterProperty =
    //    DependencyProperty.RegisterAttached("CommandParameter",
    //                                        typeof(object),
    //                                        typeof(MouseDoubleClick),
    //                                        new UIPropertyMetadata(null));

    //public static void SetCommand(DependencyObject target, ICommand value)
    //{
    //    target.SetValue(CommandProperty, value);
    //}

    //public static void SetCommandParameter(DependencyObject target, object value)
    //{
    //    target.SetValue(CommandParameterProperty, value);
    //}
    //public static object GetCommandParameter(DependencyObject target)
    //{
    //    return target.GetValue(CommandParameterProperty);
    //}

    //private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    //{
    //    Control control = target as Control;
    //    if (control != null)
    //    {
    //        if ((e.NewValue != null) && (e.OldValue == null))
    //        {
    //            control.MouseDoubleClick += OnMouseDoubleClick;
    //        }
    //        else if ((e.NewValue == null) && (e.OldValue != null))
    //        {
    //            control.MouseDoubleClick -= OnMouseDoubleClick;
    //        }
    //    }
    //}

    //private static void OnMouseDoubleClick(object sender, RoutedEventArgs e)
    //{
    //    Control control = sender as Control;
    //    ICommand command = (ICommand)control.GetValue(CommandProperty);
    //    object commandParameter = control.GetValue(CommandParameterProperty);
    //    command.Execute(commandParameter);
    //}
    }
}
