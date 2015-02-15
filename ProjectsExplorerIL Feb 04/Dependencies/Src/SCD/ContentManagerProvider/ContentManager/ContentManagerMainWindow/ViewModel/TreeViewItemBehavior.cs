using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ContentManager.General;

namespace ContentManager.ContentManagerMainWindow.ViewModel
{
    public static class TreeViewItemBehavior
    {
        #region IsTreeViewItemBehavior

        public static bool GetIsTreeViewItemBehavior(TreeViewItem treeViewItem)
        {
            return (bool) treeViewItem.GetValue(IsTreeViewItemBehaviorProperty);
        }

        public static void SetIsTreeViewItemBehavior(TreeViewItem treeViewItem, bool value)
        {
            treeViewItem.SetValue(IsTreeViewItemBehaviorProperty, value);
        }
        
        public static readonly DependencyProperty IsTreeViewItemBehaviorProperty =
            DependencyProperty.RegisterAttached(
                "IsTreeViewItemBehavior",
                typeof (bool),
                typeof (TreeViewItemBehavior),
                new UIPropertyMetadata(false, OnIsTreeViewItemBehaviorOccur));

        private static void OnIsTreeViewItemBehaviorOccur(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItem item = d as TreeViewItem;
            if (item == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool) e.NewValue)
            {
                item.Drop += OnTreeViewItemDrop;
                item.DragOver += OnTreeViewItemDragOver;
                item.MouseMove += OnTreeViewItemMouseMove;
                item.Selected += OnTreeViewItemSelected;
            }
            else
            {
                item.Drop -= OnTreeViewItemDrop;
                item.DragOver -= OnTreeViewItemDragOver;
                item.MouseMove -= OnTreeViewItemMouseMove;
                item.Selected -= OnTreeViewItemSelected;
            }
        }

        // OK
        private static void OnTreeViewItemDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeViewItem).ToString()))
            {
                object destinationNode = ((TreeViewItem)sender).DataContext;
                object sourceNode = ((TreeViewItem)e.Data.GetData(typeof(TreeViewItem).ToString())).DataContext;

                if (sourceNode.GetType() == typeof(ItemNode) && destinationNode.GetType() == typeof(ItemNode))
                {
                    ItemNode sourceItemNode = (ItemNode)sourceNode;
                    ItemNode destinationItemNode = (ItemNode)destinationNode;
                    ItemNodeActionType actionType = ((e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey) ? ItemNodeActionType.Copy : ItemNodeActionType.Move;

                    if (ItemNodeCheckAllowDrop.AllowDrop(sourceItemNode, destinationItemNode, actionType))
                    {
                        e.Effects = actionType == ItemNodeActionType.Copy ? DragDropEffects.Copy : DragDropEffects.Move;
                        e.Handled = true;
                        return;
                    }
                }

                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        // OK
        private static void OnTreeViewItemDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeViewItem).ToString()))
            {
                object destinationNode = ((TreeViewItem)sender).DataContext;
                object sourceNode = ((TreeViewItem)e.Data.GetData(typeof(TreeViewItem).ToString())).DataContext;

                if (sourceNode.GetType() == typeof(ItemNode) && destinationNode.GetType() == typeof(ItemNode))
                {
                    ItemNode sourceItemNode = (ItemNode)sourceNode;
                    ItemNode destinationItemNode = (ItemNode)destinationNode;
                    ItemNodeActionType actionType = ((e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey) ? ItemNodeActionType.Copy : ItemNodeActionType.Move;

                    if (ItemNodeCheckAllowDrop.AllowDrop(sourceItemNode, destinationItemNode, actionType))
                    {
                        ItemNodeCopyMove.ItemNodeAction(sourceItemNode, destinationItemNode, actionType);
                        e.Handled = true;
                    }
                }
            }
        }

        private static void OnTreeViewItemMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition((TreeViewItem)sender);

                if ((Math.Abs(currentPosition.X) > 10.0) ||
                    (Math.Abs(currentPosition.Y) > 10.0))
                {
                    object draggedItem = ((TreeViewItem)sender).DataContext;
                    if (draggedItem != null && draggedItem.GetType() == typeof(ItemNode))
                    {
                        ItemsControl itemsControlTv = (ItemsControl)sender;

                        while (itemsControlTv.GetType() != typeof(TreeView))
                            itemsControlTv = ItemsControl.ItemsControlFromItemContainer(itemsControlTv);

                        DragDrop.DoDragDrop(itemsControlTv, sender, DragDropEffects.Move | DragDropEffects.Copy);
                    }
                }
            }
        }

        private static void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            // Only react to the Selected event raised by the TreeViewItem
            // whose IsSelected property was modified.  Ignore all ancestors
            // who are merely reporting that a descendant's Selected fired.
            if (!Object.ReferenceEquals(sender, e.OriginalSource))
                return;

            TreeViewItem item = e.OriginalSource as TreeViewItem;
            if (item != null)
                item.BringIntoView();
        }

        #endregion // IsBroughtIntoViewWhenDrop
    }
}
