using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ContentManager.ContentManagerMainWindow.ViewModel;
using ContentManager.General;
using ContentManager.Messanger.ViewModel;
using ContentManagerProvider;
using GalaSoft.MvvmLight.Messaging;
using TraceExceptionWrapper;

namespace ContentManager.VersionUpdate.ViewModel
{
    public class VercionUpdater
    {
        #region Add version

        public static void AddVersionExecuteRunApiProvider(object o)
        {
            ContentVersion contentVersionNew = (ContentVersion) ((object[]) o)[0];
            ItemNode parentContentVersionItem = (ItemNode) ((object[]) o)[1];
            bool updateNodeName = (bool)((object[])o)[2];

            try
            {
                //Add to DB
                Locator.ContentManagerApiProvider.AddTreeObject(contentVersionNew, updateNodeName, Locator.ProgressBarDataProvider);
            }
            catch (TraceException te)
            {
                ((Messenger)Messenger.Default).Send<MessageWrapperData>(new MessageWrapperData(te));

                Locator.VersionDataProvider.UpdateModeActive = true;

                //Close UI Folder control 
                if (te.ReloadData)
                    Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);

                return;
            }

            AddVersionExecuteRunDataUpdater(contentVersionNew, parentContentVersionItem);
        }

        private static void AddVersionExecuteRunDataUpdater(ContentVersion contentVersionNew,
                                                           ItemNode parentContentVersionItem)
        {

            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(
                    new Action(() => AddVersionExecuteRunDataUpdater(contentVersionNew, parentContentVersionItem)));
            else
            {
                List<TreeNode> treeNodesList = GetChangedLinkedVersion(contentVersionNew, null);

                //Add to dictionary
                TreeNodeDictionaryUpdater.AddVersion(contentVersionNew, parentContentVersionItem);

                //Add to UI
                ItemNode newItemNode = Locator.ItemTreeBuilder.BuildItemNode(contentVersionNew, parentContentVersionItem,
                                                                             Locator.ContentManagerDataProvider
                                                                                    .ApplicationWritePermission);

                parentContentVersionItem.SubItemNode.Add(newItemNode);

                // Update permission
                ApplicationPermission.UpdatePermissionAddNode(parentContentVersionItem, newItemNode);
                ApplicationPermission.UpdatePermissionTreeNodeList(treeNodesList);

                //Close UI Folder control 
                Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);
            }
        }

        #endregion

        #region Move Version

        public static void MoveVersionExecuteRunApiProvider(object o)
        {
            ContentVersion newContentVersion = (ContentVersion) ((object[]) o)[0];
            ContentVersion originalContentVersion = (ContentVersion) ((object[]) o)[1];
            ItemNode sourceContentVersion = (ItemNode) ((object[]) o)[2];
            ItemNode destinationContent = (ItemNode) ((object[]) o)[3];

            try
            {
                //Update DB
                Locator.ContentManagerApiProvider.UpdateTreeObject(newContentVersion, originalContentVersion, null);
            }
            catch (TraceException te)
            {
                ((Messenger)Messenger.Default).Send<MessageWrapperData>(new MessageWrapperData(te));

                //Close UI Folder control 
                if (te.ReloadData)
                    Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);

                return;
            }

            MoveVersionExecuteRunDataUpdater(newContentVersion, originalContentVersion, sourceContentVersion,
                                             destinationContent);
        }

        private static void MoveVersionExecuteRunDataUpdater(ContentVersion newContentVersion,
                                                            ContentVersion originalContentVersion,
                                                            ItemNode sourceContentVersion, ItemNode destinationContent)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(
                    new Action(
                        () =>
                        MoveVersionExecuteRunDataUpdater(newContentVersion, originalContentVersion, sourceContentVersion,
                                                         destinationContent)));
            else
            {
                ItemNode oldParent = sourceContentVersion.Parent;

                //Update in dictionary
                TreeNodeDictionaryUpdater.MoveVersion(originalContentVersion, newContentVersion);

                //Update UI  
                ItemNodeCopyMove.UpdateParentItemNodeInUi(sourceContentVersion, destinationContent);

                // Update permission
                ApplicationPermission.UpdatePermissionMoveNode(oldParent, destinationContent, sourceContentVersion);
            }
        }

        #endregion

        #region  Update version

        public static void UpdateVersionExecuteRunApiProvider(object o)
        {
            ContentVersion contentVersionUpdated = (ContentVersion)((object[])o)[0];
            ContentVersion contentVersionOriginal = (ContentVersion)((object[])o)[1];
            ItemNode versionItem = (ItemNode)((object[])o)[2];

            try
            {
                //Update in DB
                Locator.ContentManagerApiProvider.UpdateTreeObject(contentVersionUpdated, contentVersionOriginal, Locator.ProgressBarDataProvider);
            }
            catch (TraceException te)
            {
                ((Messenger)Messenger.Default).Send<MessageWrapperData>(new MessageWrapperData(te));

                Locator.VersionDataProvider.UpdateModeActive = true;

                //Close UI Folder control 
                if (te.ReloadData)
                    Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);

                return;
            }

            UpdateVersionExecuteRunDataUpdater(contentVersionUpdated, contentVersionOriginal, versionItem);
        }

        private static void UpdateVersionExecuteRunDataUpdater(ContentVersion contentVersionUpdated,
                                                               ContentVersion contentVersionOriginal,
                                                               ItemNode versionItem)
        {

            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(
                    new Action(
                        () =>
                        UpdateVersionExecuteRunDataUpdater(contentVersionUpdated, contentVersionOriginal, versionItem)));
            else
            {
                List<TreeNode> treeNodesList = GetChangedLinkedVersion(contentVersionUpdated, contentVersionOriginal);

                //Update in UI
                versionItem.Name = contentVersionUpdated.Name;
                versionItem.Icon = ItemTreeBuilder.GetContentVersionIcon(contentVersionUpdated);

                //Update in dictionary
                UpdateContentVersionParameter(contentVersionUpdated, versionItem);

                // Update permission
                ApplicationPermission.UpdatePermissionUpdateNode(versionItem);
                ApplicationPermission.UpdatePermissionTreeNodeList(treeNodesList);

                //Close UI Folder control 
                Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);
            }
        }

        private static void UpdateContentVersionParameter(ContentVersion contentVersionUpdated, ItemNode versionItem)
        {
            ContentVersion contentVersionOriginal = Locator.ContentVersions[versionItem.ID];

            contentVersionOriginal.Name = contentVersionUpdated.Name;
            contentVersionOriginal.Description = contentVersionUpdated.Description;
            contentVersionOriginal.ECR = contentVersionUpdated.ECR;
            contentVersionOriginal.DocumentID = contentVersionUpdated.DocumentID;
            contentVersionOriginal.Editor = contentVersionUpdated.Editor;
            contentVersionOriginal.RunningString = contentVersionUpdated.RunningString;
            contentVersionOriginal.Status = Locator.ContentStatus[contentVersionUpdated.Status.ID];
            contentVersionOriginal.Files = contentVersionUpdated.Files;
            contentVersionOriginal.ContentVersions = contentVersionUpdated.ContentVersions;
            contentVersionOriginal.Path = contentVersionUpdated.Path;

            contentVersionOriginal.LastUpdateUser = contentVersionUpdated.LastUpdateUser;
            contentVersionOriginal.LastUpdateTime = contentVersionUpdated.LastUpdateTime;
            contentVersionOriginal.LastUpdateComputer = contentVersionUpdated.LastUpdateComputer;
            contentVersionOriginal.LastUpdateApplication = contentVersionUpdated.LastUpdateApplication;
        }

        #endregion

        #region Delete version

        public static void DeleteVersionExecuteRunApiProvider(object o)
        {
            ItemNode versionItem = (ItemNode) ((object[]) o)[0];
            ContentVersion contentVersionToDelete = Locator.ContentVersions[versionItem.ID];

            try
            {
                //Delete from DB
                Locator.ContentManagerApiProvider.DeleteTreeObject(contentVersionToDelete);
            }
            catch (TraceException te)
            {
                ((Messenger)Messenger.Default).Send<MessageWrapperData>(new MessageWrapperData(te));

                //Close UI Folder control 
                if (te.ReloadData)
                    Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);

                return;
            }

            DeleteVersionExecuteRunDataUpdater(versionItem);
        }

        private static void DeleteVersionExecuteRunDataUpdater(ItemNode versionItem)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(
                    new Action(() => DeleteVersionExecuteRunDataUpdater(versionItem)));
            else
            {
                List<TreeNode> treeNodesList = GetChangedLinkedVersion(null, (ContentVersion)versionItem.TreeNode);

                //Delete from dictionary
                TreeNodeDictionaryUpdater.DeleteVersion((ContentVersion)versionItem.TreeNode);

                //Delete from UI
                Locator.ContentManagerDataProvider.DeleteSubItemNode(versionItem);

                // Update permission
                ApplicationPermission.UpdatePermissionDeleteNode(versionItem.Parent);
                ApplicationPermission.UpdatePermissionTreeNodeList(treeNodesList);
            }
        }

        #endregion

        private static List<TreeNode> GetChangedLinkedVersion(ContentVersion contentVersionNew, ContentVersion contentVersionOld)
        {
            bool existVersion;
            List<TreeNode> contentVersionLinkedVersionChenge = new List<TreeNode>();

            if (contentVersionNew == null && contentVersionOld == null)
                return contentVersionLinkedVersionChenge;

            if (contentVersionNew == null)
            {
                foreach (KeyValuePair<int, ContentVersionSubVersion> contentVersionSubVersion in contentVersionOld.ContentVersions)
                    contentVersionLinkedVersionChenge.Add(contentVersionSubVersion.Value.ContentSubVersion);

                return contentVersionLinkedVersionChenge;
            }

            if (contentVersionOld == null)
            {
                foreach (KeyValuePair<int, ContentVersionSubVersion> contentVersionSubVersion in contentVersionNew.ContentVersions)
                    contentVersionLinkedVersionChenge.Add(contentVersionSubVersion.Value.ContentSubVersion);

                return contentVersionLinkedVersionChenge;
            }

            foreach (KeyValuePair<int, ContentVersionSubVersion> contentVersionSubVersionNew in contentVersionNew.ContentVersions)
            {
                existVersion = false;
                foreach (KeyValuePair<int, ContentVersionSubVersion> contentVersionSubVersionOld in contentVersionOld.ContentVersions)
                {
                    if (contentVersionSubVersionNew.Value.ContentSubVersion.ID == contentVersionSubVersionOld.Value.ContentSubVersion.ID)
                        existVersion = true;
                }

                if (!existVersion)
                    contentVersionLinkedVersionChenge.Add(contentVersionSubVersionNew.Value.ContentSubVersion);
            }

            foreach (KeyValuePair<int, ContentVersionSubVersion> contentVersionSubVersionOld in contentVersionOld.ContentVersions)
            {
                existVersion = false;
                foreach (KeyValuePair<int, ContentVersionSubVersion> contentVersionSubVersionNew in contentVersionNew.ContentVersions)
                {
                    if (contentVersionSubVersionNew.Value.ContentSubVersion.ID == contentVersionSubVersionOld.Value.ContentSubVersion.ID)
                        existVersion = true;
                }

                if (!existVersion)
                    contentVersionLinkedVersionChenge.Add(contentVersionSubVersionOld.Value.ContentSubVersion);
            }

            return contentVersionLinkedVersionChenge;
        }
    }
}
