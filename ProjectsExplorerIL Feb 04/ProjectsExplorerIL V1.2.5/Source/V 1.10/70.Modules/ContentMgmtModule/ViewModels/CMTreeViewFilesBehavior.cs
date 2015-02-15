using System.Windows;
using System.Windows.Controls;


namespace ContentMgmtModule
{
    public class CMTreeViewFilesBehavior
    {
        #region IsTreeViewFilesBehaviorProperty

        public static bool GetIsTreeViewFilesBehavior(TreeView treeView)
        {
            return (bool)treeView.GetValue(IsTreeViewFilesBehaviorProperty);
        }

        public static void SetIsTreeViewFilesBehavior(TreeView treeView, bool value)
        {
            treeView.SetValue(IsTreeViewFilesBehaviorProperty, value);
        }

        public static readonly DependencyProperty IsTreeViewFilesBehaviorProperty =
            DependencyProperty.RegisterAttached(
                "IsTreeViewFilesBehavior",
                typeof(bool),
                typeof(CMTreeViewFilesBehavior),
                new UIPropertyMetadata(false, OnIsTreeViewFilesBehaviorOccur));


        private static void OnIsTreeViewFilesBehaviorOccur(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeView item = d as TreeView;
            if (item == null)
                return;

            if (e.NewValue is bool == false)
                return;

            //if ((bool) e.NewValue)
            //{
            //    item.Drop += OnTreeViewDrop;
            //    item.DragOver += OnTreeViewDropDragOver;
            //}
            //else
            //{
            //    item.Drop -= OnTreeViewDrop;
            //    item.DragOver -= OnTreeViewDropDragOver;
            //}
        }

        //private static void OnTreeViewDrop(object sender, DragEventArgs e)
        //{
        //    if (!Locator.VersionDataProvider.UpdateModeData)
        //    {
        //        e.Effects = DragDropEffects.None;
        //        e.Handled = true;
        //        return;
        //    }

        //    if (e.Data.GetDataPresent(DataFormats.FileDrop))
        //    {
        //        if (!ItemFileNode.CanAddItemFilesFs(ItemFileNodeType.Folder, Locator.VersionDataProvider.VersionProperty.SubItemNode, (string[])e.Data.GetData(DataFormats.FileDrop)))
        //        {
        //            e.Effects = DragDropEffects.None;
        //            e.Handled = true;
        //            return;                    
        //        }

        //        ItemFileNode.AddSubItems(null, Locator.VersionDataProvider.VersionProperty.SubItemNode, (string[])e.Data.GetData(DataFormats.FileDrop), ((e.KeyStates & DragDropKeyStates.ControlKey) != DragDropKeyStates.ControlKey));
        //        e.Handled = true;
        //    }

        //    if (e.Data.GetDataPresent(typeof(TreeViewItem).ToString()))
        //    {
        //        object sourceNode = ((TreeViewItem)e.Data.GetData(typeof(TreeViewItem).ToString())).DataContext;

        //        if(sourceNode.GetType() == typeof(ItemFileNode))
        //        {
        //            ItemFileNode sourceItemFileNode = (ItemFileNode)sourceNode;

        //            if (ItemFileNode.CanAddItemFileNode(null, sourceItemFileNode))
        //            {
        //                ItemFileNode.UpdateParent(sourceItemFileNode, null);
        //                e.Handled = true;
        //            }
        //        }
        //    }
        //}

        //private static void OnTreeViewDropDragOver(object sender, DragEventArgs e)
        //{
        //    if (!Locator.VersionDataProvider.UpdateModeData)
        //    {
        //        e.Effects = DragDropEffects.None;
        //        e.Handled = true;
        //        return;
        //    }

        //    if (e.Data.GetDataPresent(DataFormats.FileDrop))
        //    {
        //        if (!ItemFileNode.CanAddItemFilesFs(ItemFileNodeType.Folder, Locator.VersionDataProvider.VersionProperty.SubItemNode, (string[])e.Data.GetData(DataFormats.FileDrop)))
        //        {
        //            e.Effects = DragDropEffects.None;
        //            e.Handled = true;
        //            return;
        //        }

        //        e.Effects = ((e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey) ? DragDropEffects.Copy : DragDropEffects.Move;
        //        e.Handled = true;
        //        return;
        //    }

        //    if (e.Data.GetDataPresent(typeof(TreeViewItem).ToString()))
        //    {
        //        object sourceNode = ((TreeViewItem)e.Data.GetData(typeof(TreeViewItem).ToString())).DataContext;

        //        if (sourceNode.GetType() == typeof(ItemFileNode))
        //        {
        //            ItemFileNode sourceItemFileNode = (ItemFileNode)sourceNode;

        //            if (ItemFileNode.CanAddItemFileNode(null, sourceItemFileNode))
        //            {
        //                e.Effects = DragDropEffects.Move;
        //                e.Handled = true;
        //                return;
        //            }
        //        }
                 
        //        e.Effects = DragDropEffects.None;
        //        e.Handled = true;
        //    }
        //}
        #endregion
    }
}
