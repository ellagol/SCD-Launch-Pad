using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ContentManager.General;

namespace ContentManager.VersionUpdate.ViewModel
{
    class TreeViewFilesItemBehavior 
    {
        #region IsTreeViewFilesItemBehavior

        public static bool GetIsTreeViewFilesItemBehavior(TreeViewItem treeViewItem)
        {
            return (bool)treeViewItem.GetValue(IsTreeViewFilesItemBehaviorProperty);
        }

        public static void SetIsTreeViewFilesItemBehavior(TreeViewItem treeViewItem, bool value)
        {
            treeViewItem.SetValue(IsTreeViewFilesItemBehaviorProperty, value);
        }

        public static readonly DependencyProperty IsTreeViewFilesItemBehaviorProperty =
            DependencyProperty.RegisterAttached(
                "IsTreeViewFilesItemBehavior",
                typeof(bool),
                typeof(TreeViewFilesItemBehavior),
                new UIPropertyMetadata(false, OnIsTreeViewFilesItemBehaviorOccur));

        private static void OnIsTreeViewFilesItemBehaviorOccur(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItem item = d as TreeViewItem;
            if (item == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool) e.NewValue)
            {
                item.Drop += OnTreeViewItemDrop;
                item.KeyDown += OnTreeViewItemKeyDown;
                item.DragOver += OnTreeViewItemDragOver;
                item.MouseMove += OnTreeViewItemMouseMove;
                item.Selected += OnTreeViewItemSelected;
            }
            else
            {
                item.Drop -= OnTreeViewItemDrop;
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
                ItemFileNode fileNode = (ItemFileNode)((TreeViewItem)sender).DataContext;
                fileNode.IsSelected = !fileNode.IsSelected;
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

                if (sourceNode.GetType() == typeof(ItemFileNode))
                {
                    ItemFileNode sourceItemFileNode = (ItemFileNode)sourceNode;
                    if (sourceItemFileNode.Parent == null)
                        Locator.VersionDataProvider.VersionProperty.SubItemNode.Remove(sourceItemFileNode);
                    else
                        sourceItemFileNode.Parent.SubItemNode.Remove(sourceItemFileNode);

                    e.Handled = true;
                }
            }
        }

        private static void OnTreeViewItemDragOver(object sender, DragEventArgs e)
        {
            if (!Locator.VersionDataProvider.UpdateModeData)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                object destinationNode = ((TreeViewItem)sender).DataContext;
                if (destinationNode.GetType() == typeof (ItemFileNode))
                {
                    ItemFileNode destinationItemFileNode = (ItemFileNode)destinationNode;
                    if (ItemFileNode.CanAddItemFilesFs(destinationItemFileNode.Type, destinationItemFileNode.SubItemNode, (string[])e.Data.GetData(DataFormats.FileDrop)))
                    {
                        e.Effects = ((e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey) ? DragDropEffects.Copy : DragDropEffects.Move;
                        e.Handled = true;
                        return;
                    }

                    e.Effects = DragDropEffects.None;
                    e.Handled = true; 
                    return;
                }
            }

            if (e.Data.GetDataPresent(typeof(TreeViewItem).ToString()))
            {
                object destinationNode = ((TreeViewItem)sender).DataContext;
                object sourceNode = ((TreeViewItem)e.Data.GetData(typeof(TreeViewItem).ToString())).DataContext;

                if (sourceNode.GetType() == typeof (ItemFileNode) && destinationNode.GetType() == typeof (ItemFileNode))
                {
                    ItemFileNode sourceItemFileNode = (ItemFileNode) sourceNode;
                    ItemFileNode destinationItemFileNode = (ItemFileNode) destinationNode;

                    if (ItemFileNode.CanAddItemFileNode(destinationItemFileNode, sourceItemFileNode))
                    {
                        e.Effects = DragDropEffects.Move;
                        e.Handled = true;
                        return;
                    }
                }

                e.Effects = DragDropEffects.None;
                e.Handled = true; 
                return;
            }
        }

        private static void OnTreeViewItemDrop(object sender, DragEventArgs e)
        {
            if (!Locator.VersionDataProvider.UpdateModeData)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                ItemFileNode aItemNode = (ItemFileNode)((TreeViewItem)sender).DataContext;

                if (aItemNode == null || !ItemFileNode.CanAddItemFilesFs(aItemNode.Type, aItemNode.SubItemNode, (string[])e.Data.GetData(DataFormats.FileDrop)))
                {
                    e.Effects = DragDropEffects.None;
                    e.Handled = true;
                    return;                    
                }

                ItemFileNode.AddSubItems(aItemNode, aItemNode.SubItemNode, (string[])e.Data.GetData(DataFormats.FileDrop), ((e.KeyStates & DragDropKeyStates.ControlKey) != DragDropKeyStates.ControlKey));
                e.Handled = true;
            }

            if (e.Data.GetDataPresent(typeof(TreeViewItem).ToString()))
            {
                object destinationNode = ((TreeViewItem)sender).DataContext;
                object sourceNode = ((TreeViewItem)e.Data.GetData(typeof(TreeViewItem).ToString())).DataContext;

                if (sourceNode.GetType() == typeof (ItemFileNode) && destinationNode.GetType() == typeof(ItemFileNode))
                {
                    ItemFileNode sourceItemFileNode = (ItemFileNode) sourceNode;
                    ItemFileNode destinationItemFileNode = (ItemFileNode) destinationNode;

                    if (ItemFileNode.CanAddItemFileNode(destinationItemFileNode, sourceItemFileNode))
                    {
                        ItemFileNode.UpdateParent(sourceItemFileNode, destinationItemFileNode);
                        e.Handled = true;
                    }
                }
            }
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
                    if (draggedItem != null && draggedItem.GetType() == typeof(ItemFileNode))
                    {
                        ItemsControl itemsControlTv = (ItemsControl) sender;

                        while (itemsControlTv.GetType() != typeof (TreeView))
                            itemsControlTv = ItemsControl.ItemsControlFromItemContainer(itemsControlTv);

                        DragDrop.DoDragDrop(itemsControlTv, sender, DragDropEffects.Move);
                    }
                }
            }
        }

        #endregion // IsBroughtIntoViewWhenDrop
    }
}
