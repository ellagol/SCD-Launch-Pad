using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ATSDomain;



namespace ContentMgmtModule
{
    public class CMTreeViewFilesItemBehavior
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
                typeof(CMTreeViewFilesItemBehavior),
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
                item.MouseDoubleClick += OnMouseDoubleClick;
                //item.Drop += OnTreeViewItemDrop;
                item.KeyDown += OnTreeViewItemKeyDown;
                //item.DragOver += OnTreeViewItemDragOver;
                //item.MouseMove += OnTreeViewItemMouseMove;
                item.Selected += OnTreeViewItemSelected;
            }
            else
            {
                item.MouseDoubleClick -= OnMouseDoubleClick;
                //item.Drop -= OnTreeViewItemDrop;
                item.KeyDown -= OnTreeViewItemKeyDown;
                //item.DragOver -= OnTreeViewItemDragOver;
                //item.MouseMove -= OnTreeViewItemMouseMove;
                item.Selected -= OnTreeViewItemSelected;
            }
        }

        private static void OnMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string fileExtension;
                String errorId;
                CMItemFileNode fileNode = (CMItemFileNode)((TreeViewItem)sender).DataContext;
                fileExtension = System.IO.Path.GetExtension(fileNode.ExecutePath);
                if (fileNode.Type.ToString() == "File")
                {
                    switch (fileExtension)
                    {
                        //don't execute the file
                        case ".bin":
                        case ".exe":
                        case ".dll":
                        case ".pdb":
                        case null:

                            errorId = "UnableToViewFile";
                            CMContentManagementViewModel.ShowErrorAndInfoMessage(errorId, null);
                            break;

                        //execute in edit mode
                        case ".bat":
                            ProcessStartInfo pStartInfo = new ProcessStartInfo("notepad.exe", fileNode.ExecutePath);
                            Process execute = new Process();
                            execute.StartInfo = pStartInfo;
                            execute.Start();
                            break;

                        //execute the file
                        default:
                            System.Diagnostics.Process.Start(fileNode.ExecutePath);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
            }

        }

        private static void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                object myObject = sender.GetType().GetProperty("ParentTreeView", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(sender, null);

                if (myObject.GetType().ToString() == "System.Windows.Controls.TreeView")
                {
                    ObservableCollection<CMItemFileNode> files = (ObservableCollection<CMItemFileNode>)((System.Windows.Controls.TreeView)(myObject)).ItemsSource;
                    CMItemFileNode fileNode = (CMItemFileNode)((TreeViewItem)sender).DataContext;

                    GetSelectedFiles(files, fileNode);
                }
            }
            catch (Exception) { }

        }

        private static void GetSelectedFiles(ObservableCollection<CMItemFileNode> files, CMItemFileNode fileNode)
        {

            if (Keyboard.IsKeyDown((Key.LeftCtrl)) || Keyboard.IsKeyDown((Key.RightCtrl)))
            {
                if (files.Contains(fileNode) && fileNode.Type.ToString() == "File")
                {
                    int indexToInsert = files.IndexOf(fileNode);
                    files.Remove(fileNode);
                    // fileNode.IsSelected = !fileNode.IsSelected;
                    fileNode.Status = ItemFileStatus.Selected;
                    files.Insert(indexToInsert, fileNode);
                    return;
                }
                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        GetSelectedFiles(files[i].SubItemNode, fileNode);
                    }
                }
            }
            else  //if ctrl isnt pressed unselect all the files that where selected before
            {
                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        if(files[i].Status == ItemFileStatus.Selected)
                        {
                            CMItemFileNode tempFile = files[i];
                            int indexToInsert = files.IndexOf(files[i]);
                            files.Remove(files[i]);
                            tempFile.Status = ItemFileStatus.Exist;
                            tempFile.IsSelected = false;
                            files.Insert(indexToInsert, tempFile);
                        }

                        GetSelectedFiles(files[i].SubItemNode, fileNode);
                    }
                }
            }
        }

        private static void OnTreeViewItemKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && CMVersionDetailsViewModel.editMode == true)
            {
                object sourceNode = ((TreeViewItem)sender).DataContext;

                if (sourceNode.GetType() == typeof(CMItemFileNode))
                {
                    CMItemFileNode sourceItemFileNode = (CMItemFileNode)sourceNode;
                    if (sourceItemFileNode.Parent == null)
                    {
                        object myObject = sender.GetType().GetProperty("ParentTreeView", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(sender, null);
                        if (myObject.GetType().ToString() == "System.Windows.Controls.TreeView")
                        {
                            ObservableCollection<CMItemFileNode> files = (ObservableCollection<CMItemFileNode>)((System.Windows.Controls.TreeView)(myObject)).ItemsSource;
                            files.Remove(sourceItemFileNode);
                        }                       
                    }
                    else
                    {
                        sourceItemFileNode.Parent.SubItemNode.Remove(sourceItemFileNode);
                    }
                    e.Handled = true;
                }
            }
        }

        //private static void OnTreeViewItemDragOver(object sender, DragEventArgs e)
        //{
        //    if (!Locator.VersionDataProvider.UpdateModeData)
        //    {
        //        e.Effects = DragDropEffects.None;
        //        e.Handled = true;
        //        return;
        //    }

        //    if (e.Data.GetDataPresent(DataFormats.FileDrop))
        //    {
        //        object destinationNode = ((TreeViewItem)sender).DataContext;
        //        if (destinationNode.GetType() == typeof (ItemFileNode))
        //        {
        //            ItemFileNode destinationItemFileNode = (ItemFileNode)destinationNode;
        //            if (ItemFileNode.CanAddItemFilesFs(destinationItemFileNode.Type, destinationItemFileNode.SubItemNode, (string[])e.Data.GetData(DataFormats.FileDrop)))
        //            {
        //                e.Effects = ((e.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey) ? DragDropEffects.Copy : DragDropEffects.Move;
        //                e.Handled = true;
        //                return;
        //            }

        //            e.Effects = DragDropEffects.None;
        //            e.Handled = true; 
        //            return;
        //        }
        //    }

        //    if (e.Data.GetDataPresent(typeof(TreeViewItem).ToString()))
        //    {
        //        object destinationNode = ((TreeViewItem)sender).DataContext;
        //        object sourceNode = ((TreeViewItem)e.Data.GetData(typeof(TreeViewItem).ToString())).DataContext;

        //        if (sourceNode.GetType() == typeof (ItemFileNode) && destinationNode.GetType() == typeof (ItemFileNode))
        //        {
        //            ItemFileNode sourceItemFileNode = (ItemFileNode) sourceNode;
        //            ItemFileNode destinationItemFileNode = (ItemFileNode) destinationNode;

        //            if (ItemFileNode.CanAddItemFileNode(destinationItemFileNode, sourceItemFileNode))
        //            {
        //                e.Effects = DragDropEffects.Move;
        //                e.Handled = true;
        //                return;
        //            }
        //        }

        //        e.Effects = DragDropEffects.None;
        //        e.Handled = true; 
        //        return;
        //    }
        //}

        //private static void OnTreeViewItemDrop(object sender, DragEventArgs e)
        //{
        //    if (!Locator.VersionDataProvider.UpdateModeData)
        //    {
        //        e.Effects = DragDropEffects.None;
        //        e.Handled = true;
        //        return;
        //    }

        //    if (e.Data.GetDataPresent(DataFormats.FileDrop))
        //    {
        //        ItemFileNode aItemNode = (ItemFileNode)((TreeViewItem)sender).DataContext;

        //        if (aItemNode == null || !ItemFileNode.CanAddItemFilesFs(aItemNode.Type, aItemNode.SubItemNode, (string[])e.Data.GetData(DataFormats.FileDrop)))
        //        {
        //            e.Effects = DragDropEffects.None;
        //            e.Handled = true;
        //            return;                    
        //        }

        //        ItemFileNode.AddSubItems(aItemNode, aItemNode.SubItemNode, (string[])e.Data.GetData(DataFormats.FileDrop), ((e.KeyStates & DragDropKeyStates.ControlKey) != DragDropKeyStates.ControlKey));
        //        e.Handled = true;
        //    }

        //    if (e.Data.GetDataPresent(typeof(TreeViewItem).ToString()))
        //    {
        //        object destinationNode = ((TreeViewItem)sender).DataContext;
        //        object sourceNode = ((TreeViewItem)e.Data.GetData(typeof(TreeViewItem).ToString())).DataContext;

        //        if (sourceNode.GetType() == typeof (ItemFileNode) && destinationNode.GetType() == typeof(ItemFileNode))
        //        {
        //            ItemFileNode sourceItemFileNode = (ItemFileNode) sourceNode;
        //            ItemFileNode destinationItemFileNode = (ItemFileNode) destinationNode;

        //            if (ItemFileNode.CanAddItemFileNode(destinationItemFileNode, sourceItemFileNode))
        //            {
        //                ItemFileNode.UpdateParent(sourceItemFileNode, destinationItemFileNode);
        //                e.Handled = true;
        //            }
        //        }
        //    }
        //}

        //private static void OnTreeViewItemMouseMove(object sender, MouseEventArgs e)
        //{
        //    //if (!Locator.VersionDataProvider.UpdateModeData)
        //    //{
        //    //    e.Handled = true;
        //    //    return;
        //    //}

        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        Point currentPosition = e.GetPosition((TreeViewItem)sender);

        //        if ((Math.Abs(currentPosition.X) > 10.0) ||
        //            (Math.Abs(currentPosition.Y) > 10.0))
        //        {
        //            object draggedItem = ((TreeViewItem)sender).DataContext;
        //            if (draggedItem != null && draggedItem.GetType() == typeof(CMItemFileNode))
        //            {
        //                ItemsControl itemsControlTv = (ItemsControl)sender;

        //                while (itemsControlTv.GetType() != typeof(TreeView))
        //                    itemsControlTv = ItemsControl.ItemsControlFromItemContainer(itemsControlTv);

        //                DragDrop.DoDragDrop(itemsControlTv, sender, DragDropEffects.Move);
        //            }
        //        }
        //    }
        //}

        #endregion // IsBroughtIntoViewWhenDrop
    }
}
