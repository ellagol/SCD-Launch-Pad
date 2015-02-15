using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Windows;
using ContentManager.ContentManagerMainWindow.ViewModel;
using ContentManager.General;
using ContentManager.Messanger.ViewModel;
using ContentManagerProvider;
using GalaSoft.MvvmLight.Messaging;
using TraceExceptionWrapper;

namespace ContentManager.FolderUpdate.ViewModel
{
    public class FolderUpdater
    {
        #region Add folder

        public static void AddFolderExecuteRunApiProvider(object o)
        {
            Folder folderNew = (Folder)((object[])o)[0];
            ItemNode parentFolderItem = (ItemNode)((object[])o)[1];
            bool updateNodeName = (bool)((object[])o)[2];

            try
            {
                //Add to DB
                Locator.ContentManagerApiProvider.AddTreeObject(folderNew, updateNodeName, null);
            }
            catch (TraceException te)
            {
                ((Messenger)Messenger.Default).Send<MessageWrapperData>(new MessageWrapperData(te));

                Locator.FoldersDataProvider.UpdateModeActive = true;

                //Close UI Folder control 
                if(te.ReloadData)
                    Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);

                return;
            }


            AddFolderExecuteRunDataUpdater(folderNew, parentFolderItem);
        }

        private static void AddFolderExecuteRunDataUpdater(Folder folderNew, ItemNode parentFolderItem)
        {

            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(
                    new Action(() => AddFolderExecuteRunDataUpdater(folderNew, parentFolderItem)));
            else
            {
                //Add to dictionary
                TreeNodeDictionaryUpdater.AddFolder(folderNew, parentFolderItem);

                //Add to UI
                ItemNode newItemNode = Locator.ItemTreeBuilder.BuildItemNode(folderNew, parentFolderItem,
                                                                 Locator.ContentManagerDataProvider
                                                                        .ApplicationWritePermission);
                if (parentFolderItem != null)
                    parentFolderItem.SubItemNode.Add(newItemNode);
                else
                    Locator.ContentManagerDataProvider.SubItemNode.Add(newItemNode);

                //Close UI Folder control 
                Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);

                // Update permission
                ApplicationPermission.UpdatePermissionAddNode(parentFolderItem, newItemNode);
            }
        }

        #endregion

        #region Update folder

        public static void UpdateFolderExecuteRunApiProvider(object o)
        {
            Folder folderNew = (Folder) ((object[]) o)[0];
            Folder folderOriginal = (Folder) ((object[]) o)[1];
            ItemNode folderItem = (ItemNode) ((object[]) o)[2];

            try
            {
                //Update in DB
                Locator.ContentManagerApiProvider.UpdateTreeObject(folderNew, folderOriginal, null);
            }
            catch (TraceException te)
            {
                ((Messenger)Messenger.Default).Send<MessageWrapperData>(new MessageWrapperData(te));

                Locator.FoldersDataProvider.UpdateModeActive = true;

                //Close UI Folder control 
                if (te.ReloadData)
                    Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);

                return;
            }


            UpdateFolderExecuteRunDataUpdater(folderNew, folderOriginal, folderItem);
        }

        private static void UpdateFolderExecuteRunDataUpdater(Folder folderUpdated, Folder folderOriginal,
                                                              ItemNode folderItem)
        {

            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(
                    new Action(() => UpdateFolderExecuteRunDataUpdater(folderUpdated, folderOriginal, folderItem)));
            else
            {
                //Update in UI
                folderItem.Name = folderUpdated.Name;

                //Update in dictionary
                UpdateFolderParameter(folderUpdated, folderOriginal);

                //Close UI Folder control 
                Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);

                // Update permission
                ApplicationPermission.UpdatePermissionUpdateNode(folderItem);
            }
        }

        private static void UpdateFolderParameter(Folder folderUpdated, Folder folderOriginal)
        {
            folderOriginal.Name = folderUpdated.Name;
            folderOriginal.Description = folderUpdated.Description;
            folderOriginal.UserGroupTypePermission = folderUpdated.UserGroupTypePermission;

            folderOriginal.LastUpdateUser = folderUpdated.LastUpdateUser;
            folderOriginal.LastUpdateTime = folderUpdated.LastUpdateTime;
            folderOriginal.LastUpdateComputer = folderUpdated.LastUpdateComputer;
            folderOriginal.LastUpdateApplication = folderUpdated.LastUpdateApplication;
        }

        #endregion

        #region Delete folder

        public static void DeleteFolderExecuteRunApiProvider(object o)
        {
            Folder folderToDelete = (Folder) ((object[]) o)[0];
            ItemNode folderItem = (ItemNode) ((object[]) o)[1];

            try
            {
                //Delete from DB
                Locator.ContentManagerApiProvider.DeleteTreeObject(folderToDelete);
            }
            catch (TraceException te)
            {
                ((Messenger)Messenger.Default).Send<MessageWrapperData>(new MessageWrapperData(te));

                //Close UI Folder control 
                if (te.ReloadData)
                    Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);

                return;
            }

            DeleteFolderExecuteRunDataUpdater(folderItem);
        }

        private static void DeleteFolderExecuteRunDataUpdater(ItemNode folderItem)
        {

            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(
                    new Action(() => DeleteFolderExecuteRunDataUpdater(folderItem)));
            else
            {
                //Delete from dictionary
                TreeNodeDictionaryUpdater.DeleteFolder((Folder)folderItem.TreeNode);

                //Delete from UI
                Locator.ContentManagerDataProvider.DeleteSubItemNode(folderItem);

                // Update permission
                ApplicationPermission.UpdatePermissionDeleteNode(folderItem.Parent);
            }
        }

        #endregion

        #region Move folder

        public static void MoveFloderExecuteRunApiProvider(object o)
        {
            Folder updatedFolder = (Folder) ((object[]) o)[0];
            Folder originalFolder = (Folder) ((object[]) o)[1];
            ItemNode sourceFolder = (ItemNode) ((object[]) o)[2];
            ItemNode destinationFolder = (ItemNode) ((object[]) o)[3];

            try
            {
                //Update DB
                Locator.ContentManagerApiProvider.UpdateTreeObject(updatedFolder, originalFolder, null);
            }
            catch (TraceException te)
            {
                ((Messenger)Messenger.Default).Send<MessageWrapperData>(new MessageWrapperData(te));

                //Close UI Folder control 
                if (te.ReloadData)
                    Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);

                return;
            }


            MoveFloderExecuteRunDataUpdater(updatedFolder, originalFolder, sourceFolder, destinationFolder);
        }

        private static void MoveFloderExecuteRunDataUpdater(Folder updatedFolder,
                                                            Folder originalFolder,
                                                            ItemNode sourceFolder, ItemNode destinationFolder)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(
                    new Action(
                        () =>
                        MoveFloderExecuteRunDataUpdater(updatedFolder, originalFolder, sourceFolder, destinationFolder)));
            else
            {
                ItemNode oldParent = sourceFolder.Parent;

                //Update in dictionary
                TreeNodeDictionaryUpdater.MoveFolder(originalFolder, updatedFolder);

                //Update UI  
                ItemNodeCopyMove.UpdateParentItemNodeInUi(sourceFolder, destinationFolder);

                // Update permission
                ApplicationPermission.UpdatePermissionMoveNode(oldParent, destinationFolder, sourceFolder);
            }
        }

        #endregion

    }
}
