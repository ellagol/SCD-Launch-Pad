using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ContentManager.General;

namespace ContentManager.ContentManagerMainWindow.ViewModel
{
    class TreeViewBehavior
    {
        public static bool GetIsTreeViewBehavior(TreeView treeView)
        {
            return (bool)treeView.GetValue(IsTreeViewBehaviorProperty);
        }

        public static void SetIsTreeViewBehavior(TreeView treeView, bool value)
        {
            treeView.SetValue(IsTreeViewBehaviorProperty, value);
        }

        public static readonly DependencyProperty IsTreeViewBehaviorProperty =
            DependencyProperty.RegisterAttached(
                "IsTreeViewBehavior",
                typeof(bool),
                typeof(TreeViewBehavior),
                new UIPropertyMetadata(false, OnIsTreeViewBehaviorOccur));

        private static void OnIsTreeViewBehaviorOccur(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeView item = d as TreeView;
            if (item == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
            {
                item.Drop += OnTreeViewDrop;
                item.DragOver += OnTreeViewDropDragOver;
            }
            else
            {
                item.Drop -= OnTreeViewDrop;
                item.DragOver -= OnTreeViewDropDragOver;
            }
        }

        private static void OnTreeViewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeViewItem).ToString()))
            {
                object sourceNode = ((TreeViewItem)e.Data.GetData(typeof(TreeViewItem).ToString())).DataContext;

                if (sourceNode.GetType() == typeof(ItemNode))
                {
                    ItemNode sourceItemNode = (ItemNode)sourceNode;
                    ItemNodeActionType actionType = ((e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey) ? ItemNodeActionType.Copy : ItemNodeActionType.Move;

                    if (ItemNodeCheckAllowDrop.AllowDrop(sourceItemNode, null, actionType))
                    {
                        ItemNodeCopyMove.ItemNodeAction(sourceItemNode, null, actionType);
                        e.Handled = true;
                    }
                }
            }
        }

        private static void OnTreeViewDropDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeViewItem).ToString()))
            {
                object sourceNode = ((TreeViewItem)e.Data.GetData(typeof(TreeViewItem).ToString())).DataContext;

                if (sourceNode.GetType() == typeof(ItemNode))
                {
                    ItemNode sourceItemFileNode = (ItemNode)sourceNode;
                    ItemNodeActionType actionType = ((e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey) ? ItemNodeActionType.Copy : ItemNodeActionType.Move;

                    if (ItemNodeCheckAllowDrop.AllowDrop(sourceItemFileNode, null, actionType))
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
    }
}
