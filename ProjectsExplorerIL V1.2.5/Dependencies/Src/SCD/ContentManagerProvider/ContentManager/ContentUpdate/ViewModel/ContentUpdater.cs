using System;
using System.Windows;
using ContentManager.ContentManagerMainWindow.ViewModel;
using ContentManager.General;
using ContentManager.Messanger.ViewModel;
using ContentManagerProvider;
using GalaSoft.MvvmLight.Messaging;
using TraceExceptionWrapper;

namespace ContentManager.ContentUpdate.ViewModel
{
    public class ContentUpdater
    {
        #region Add content

        public static void AddContentExecuteRunApiProvider(object o)
        {
            Content contentNew = (Content)((object[])o)[0];
            ItemNode parentFolderItem = (ItemNode)((object[])o)[1];
            bool updateNodeName = (bool)((object[])o)[2];

            try
            {
                //Add to DB
                Locator.ContentManagerApiProvider.AddTreeObject(contentNew, updateNodeName, null);
            }
            catch (TraceException te)
            {
                ((Messenger)Messenger.Default).Send<MessageWrapperData>(new MessageWrapperData(te));

                Locator.ContentsDataProvider.UpdateModeActive = true;

                //Close UI Folder control 
                if (te.ReloadData)
                    Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);

                return;
            }

            AddContentExecuteRunDataUpdater(contentNew, parentFolderItem);
        }

        private static void AddContentExecuteRunDataUpdater(Content contentNew,
                                                           ItemNode parentFolderItem)
        {

            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(
                    new Action(() => AddContentExecuteRunDataUpdater(contentNew, parentFolderItem)));
            else
            {
                //Add to dictionary
                TreeNodeDictionaryUpdater.AddContent(contentNew, parentFolderItem);

                //Add to UI
                ItemNode newItemNode = Locator.ItemTreeBuilder.BuildItemNode(contentNew, parentFolderItem,
                                                                             Locator.ContentManagerDataProvider
                                                                                    .ApplicationWritePermission);
                if (parentFolderItem != null)
                    parentFolderItem.SubItemNode.Add(newItemNode);
                else
                    Locator.ContentManagerDataProvider.SubItemNode.Add(newItemNode);

                // Update permission
                ApplicationPermission.UpdatePermissionAddNode(parentFolderItem, newItemNode);

                //Close UI Folder control 
                Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);
            }
        }

        #endregion

        #region Update content

        public static void UpdateContentExecuteRunApiProvider(object o)
        {
            Content contentUpdated = (Content) ((object[]) o)[0];
            Content contentOriginal = (Content) ((object[]) o)[1];
            ItemNode contentItem = (ItemNode) ((object[]) o)[2];

            try
            {
                //Update in DB
                Locator.ContentManagerApiProvider.UpdateTreeObject(contentUpdated, contentOriginal, null);
            }
            catch (TraceException te)
            {
                ((Messenger)Messenger.Default).Send<MessageWrapperData>(new MessageWrapperData(te));

                Locator.ContentsDataProvider.UpdateModeActive = true;

                //Close UI Folder control 
                if (te.ReloadData)
                    Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);

                return;
            }


            UpdateContentExecuteRunDataUpdater(contentUpdated, contentOriginal, contentItem);
        }

        private static void UpdateContentExecuteRunDataUpdater(Content contentUpdated,
                                                               Content contentOriginal,
                                                               ItemNode contentItem)
        {

            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(
                    new Action(
                        () =>
                        UpdateContentExecuteRunDataUpdater(contentUpdated, contentOriginal, contentItem)));
            else
            {

                //Update in UI
                contentItem.Name = contentUpdated.Name;
                ItemTreeBuilder.UpdateContentIcon(contentUpdated, contentItem);

                //Update in dictionary
                UpdateContentParameter(contentUpdated, contentOriginal);

                // Update permission
                ApplicationPermission.UpdatePermissionUpdateNode(contentItem);

                //Close UI Folder control 
                Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);
            }
        }

        private static void UpdateContentParameter(Content contentUpdated, Content contentOriginal)
        {
            contentOriginal.Name = contentUpdated.Name;
            contentOriginal.Description = contentUpdated.Description;
            contentOriginal.IconFileFullPath = contentUpdated.IconFileFullPath;
            contentOriginal.CertificateFree = contentUpdated.CertificateFree;
            contentOriginal.ContentType = contentUpdated.ContentType;

            contentOriginal.LastUpdateUser = contentUpdated.LastUpdateUser;
            contentOriginal.LastUpdateTime = contentUpdated.LastUpdateTime;
            contentOriginal.LastUpdateComputer = contentUpdated.LastUpdateComputer;
            contentOriginal.LastUpdateApplication = contentUpdated.LastUpdateApplication;
        }

        #endregion

        #region Delete content

        public static void DeleteContentExecuteRunApiProvider(object o)
        {

            Content contentToDelete = (Content)((object[])o)[0];
            ItemNode contentItem = (ItemNode)((object[])o)[1];

            try
            {
                //Delete from DB
                Locator.ContentManagerApiProvider.DeleteTreeObject(contentToDelete);
            }
            catch (TraceException te)
            {
                ((Messenger)Messenger.Default).Send<MessageWrapperData>(new MessageWrapperData(te));

                //Close UI Folder control 
                if(te.ReloadData)
                    Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);

                return;
            }
             
            DeleteContentExecuteRunDataUpdater(contentItem);
        }

        private static void DeleteContentExecuteRunDataUpdater(ItemNode contentItem)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(
                    new Action(() => DeleteContentExecuteRunDataUpdater(contentItem)));
            else
            {
                //Delete from dictionary
                TreeNodeDictionaryUpdater.DeleteContent((Content)contentItem.TreeNode);

                //Delete from UI
                Locator.ContentManagerDataProvider.DeleteSubItemNode(contentItem);

                // Update permission
                ApplicationPermission.UpdatePermissionDeleteNode(contentItem.Parent);
            }
        }

        #endregion

        #region Move Content

        public static void MoveContentExecuteRunApiProvider(object o)
        {
            Content updatedContent = (Content) ((object[]) o)[0];
            Content originalContent = (Content) ((object[]) o)[1];
            ItemNode sourceContent = (ItemNode) ((object[]) o)[2];
            ItemNode destinationFolder = (ItemNode) ((object[]) o)[3];

            try
            {
                //Update DB
                Locator.ContentManagerApiProvider.UpdateTreeObject(updatedContent, originalContent, null);
            }
            catch (TraceException te)
            {
                ((Messenger) Messenger.Default).Send<MessageWrapperData>(new MessageWrapperData(te));

                //Close UI Folder control 
                if (te.ReloadData)
                    Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);

                return;
            }

            MoveContentExecuteRunDataUpdater(updatedContent, originalContent, sourceContent, destinationFolder);
        }

        private static void MoveContentExecuteRunDataUpdater(Content updatedContent,
                                                             Content originalContent,
                                                             ItemNode sourceContent, ItemNode destinationFolder)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(
                    new Action(
                        () =>
                        MoveContentExecuteRunDataUpdater(updatedContent, originalContent, sourceContent,
                                                         destinationFolder)));
            else
            {
                ItemNode oldParent = sourceContent.Parent;

                //Update in dictionary
                TreeNodeDictionaryUpdater.MoveContent(originalContent, updatedContent);

                //Update UI  
                ItemNodeCopyMove.UpdateParentItemNodeInUi(sourceContent, destinationFolder);

                // Update permission
                ApplicationPermission.UpdatePermissionMoveNode(oldParent, destinationFolder, sourceContent);
            }
        }

        #endregion

    }
}
