using System.Windows;
using System.Windows.Controls;

namespace ContentMgmtModule
{
    public class CMTreeViewBehavior
    {
        #region IsTreeViewBehavior

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
                typeof(CMTreeViewBehavior),
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

                if (sourceNode.GetType() == typeof(CMTreeViewNodeViewModelBase))
                {
                    CMTreeViewNodeViewModelBase sourceItemNode = (CMTreeViewNodeViewModelBase)sourceNode;
                    ItemNodeActionType actionType = ((e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey) ? ItemNodeActionType.Copy : ItemNodeActionType.Move;

                    CMContentManagementViewModel cmvm  = new CMContentManagementViewModel();               
                    if ( (cmvm.CheckIfAllowDrop(sourceItemNode, null, actionType)) )
                    {
                       // ItemNodeCopyMove.ItemNodeAction(sourceItemNode, null, actionType);
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

                if (sourceNode.GetType() == typeof(CMTreeViewNodeViewModelBase))
                {
                    CMTreeViewNodeViewModelBase sourceItemFileNode = (CMTreeViewNodeViewModelBase)sourceNode;
                    ItemNodeActionType actionType = ((e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey) ? ItemNodeActionType.Copy : ItemNodeActionType.Move;

                    CMContentManagementViewModel cmvm = new CMContentManagementViewModel();
                    if (cmvm.CheckIfAllowDrop(sourceItemFileNode, null, actionType))
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

        #endregion
    }     
}
