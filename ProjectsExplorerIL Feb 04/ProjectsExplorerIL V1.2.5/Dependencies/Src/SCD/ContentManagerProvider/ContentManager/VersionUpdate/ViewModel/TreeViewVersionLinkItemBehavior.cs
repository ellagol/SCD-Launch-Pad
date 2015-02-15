using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ContentManager.General;

namespace ContentManager.VersionUpdate.ViewModel
{
    class TreeViewVersionLinkItemBehavior
    {
        #region IsTreeViewFilesItemBehavior

        public static bool GetIsTreeViewVersionLinkItemBehavior(TreeViewItem treeViewItem)
        {
            return (bool)treeViewItem.GetValue(IsTreeViewVersionLinkItemBehaviorProperty);
        }

        public static void SetIsTreeViewVersionLinkItemBehavior(TreeViewItem treeViewItem, bool value)
        {
            treeViewItem.SetValue(IsTreeViewVersionLinkItemBehaviorProperty, value);
        }

        public static readonly DependencyProperty IsTreeViewVersionLinkItemBehaviorProperty =
            DependencyProperty.RegisterAttached(
                "IsTreeViewVersionLinkItemBehavior",
                typeof(bool),
                typeof(TreeViewVersionLinkItemBehavior),
                new UIPropertyMetadata(false, OnTreeViewVersionLinkItemBehaviorOccur));

        private static void OnTreeViewVersionLinkItemBehaviorOccur(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItem item = d as TreeViewItem;
            if (item == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
            {
                item.KeyDown += OnTreeViewItemKeyDown;
                item.DragOver += OnTreeViewItemDragOver;
                item.MouseMove += OnTreeViewItemMouseMove;
                item.Selected += OnTreeViewItemSelected;
            }
            else
            {
                item.KeyDown -= OnTreeViewItemKeyDown;
                item.DragOver -= OnTreeViewItemDragOver;
                item.MouseMove -= OnTreeViewItemMouseMove;
                item.Selected -= OnTreeViewItemSelected;
            }
        }

        private static void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            if (!Locator.VersionDataProvider.UpdateModeData)
            {
                e.Handled = true;
                return;
            }

            if (Keyboard.IsKeyDown((Key.LeftCtrl)) || Keyboard.IsKeyDown((Key.RightCtrl)))
            {
                ItemVersionLink linkNode = (ItemVersionLink)((TreeViewItem)sender).DataContext;
                linkNode.IsSelected = !linkNode.IsSelected;
                e.Handled = true;
            }
        }

        private static void OnTreeViewItemKeyDown(object sender, KeyEventArgs e)
        {
            if (!Locator.VersionDataProvider.UpdateModeData)
            {
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Delete)
            {
                object sourceNode = ((TreeViewItem)sender).DataContext;

                if (sourceNode.GetType() == typeof(ItemVersionLink))
                {
                    ItemVersionLink sourceItemVersionLinkNode = (ItemVersionLink)sourceNode;
                    Locator.VersionDataProvider.VersionProperty.SubItemVersionLinkNodeRemove(sourceItemVersionLinkNode);
                    e.Handled = true;
                }
            }
        }

        private static void OnTreeViewItemDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None; 
            e.Handled = true;
        }

        private static void OnTreeViewItemMouseMove(object sender, MouseEventArgs e)
        {

            if (!Locator.VersionDataProvider.UpdateModeData)
            {
                e.Handled = true;
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition((TreeViewItem)sender);

                if ((Math.Abs(currentPosition.X) > 10.0) ||
                    (Math.Abs(currentPosition.Y) > 10.0))
                {
                    object draggedItem = ((TreeViewItem)sender).DataContext;
                    if (draggedItem != null && draggedItem.GetType() == typeof(ItemVersionLink))
                    {
                        ItemsControl itemsControlTv = (ItemsControl)sender;

                        while (itemsControlTv.GetType() != typeof(TreeView))
                            itemsControlTv = ItemsControl.ItemsControlFromItemContainer(itemsControlTv);

                        DragDrop.DoDragDrop(itemsControlTv, sender, DragDropEffects.Move);
                    }
                }
            }
        }

        #endregion // IsBroughtIntoViewWhenDrop
    }
}
