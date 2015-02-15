using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using ContentManager.ContentUpdate.ViewModel;
using ContentManager.FolderUpdate.ViewModel;
using ContentManager.General;
using ContentManager.Messanger.ViewModel;
using ContentManager.VersionUpdate.ViewModel;
using ContentManagerProvider;
using GalaSoft.MvvmLight.Messaging;

namespace ContentManager.ContentManagerMainWindow.ViewModel
{

    public enum ItemNodeActionType
    {
        Copy,
        Move,
        Delete
    }

    public static class ItemNodeCopyMove
    {
        
        #region Move & Copy ItemNode

        public static void Copy(ItemNode source, ItemNode destination)
        {
            switch (source.Type)
            {
                case TreeNodeObjectType.Folder:
                    CopyFolder(source, destination);
                    break;
                case TreeNodeObjectType.Content:
                    CopyContent(source, destination);
                    break;
                case TreeNodeObjectType.ContentVersion:
                    CopyContentVersion(source, destination);
                    break;
            }
        }

        public static void Move(ItemNode source, ItemNode destination)
        {
            switch (source.Type)
            {
                case TreeNodeObjectType.Folder:
                    MoveFolder(source, destination);
                    break;
                case TreeNodeObjectType.Content:
                    MoveContent(source, destination);
                    break;
                case TreeNodeObjectType.ContentVersion:
                    MoveContentVersion(source, destination);
                    break;
            }
        }

        #endregion
        
        #region Folder

        private static void CopyFolder(ItemNode sourceFolder, ItemNode destinationFolder)
        {
            Folder newFolder;
            Folder originalFolder;

            originalFolder = Locator.Folders[sourceFolder.ID];
            newFolder = originalFolder.Clone();
            UpdateTreeNodeParentID(newFolder, destinationFolder);

            object[] threadParams = new object[3];
            threadParams[0] = newFolder;
            threadParams[1] = destinationFolder;
            threadParams[2] = true;

            Thread workerThread = new Thread(FolderUpdater.AddFolderExecuteRunApiProvider);
            workerThread.Start(threadParams);
        }

        private static void MoveFolder(ItemNode sourceFolder, ItemNode destinationFolder)
        {

            Folder folderOriginal = Locator.Folders[sourceFolder.ID];
            Folder folderNew = folderOriginal.Clone();
            UpdateTreeNodeParentID(folderNew, destinationFolder);

            object[] threadParams = new object[4];
            threadParams[0] = folderNew;
            threadParams[1] = folderOriginal;
            threadParams[2] = sourceFolder;
            threadParams[3] = destinationFolder;

            Thread workerThread = new Thread(FolderUpdater.MoveFloderExecuteRunApiProvider);
            workerThread.Start(threadParams);
        }

        #endregion

        #region Content

        private static void CopyContent(ItemNode sourceContent, ItemNode destinationFolder)
        {
            Content newContent;
            Content originalContent;

            originalContent = Locator.Contents[sourceContent.ID];
            newContent = originalContent.Clone();
            UpdateTreeNodeParentID(newContent, destinationFolder);

            object[] threadParams = new object[3];
            threadParams[0] = newContent;
            threadParams[1] = destinationFolder;
            threadParams[2] = true;

            Thread workerThread = new Thread(ContentUpdater.AddContentExecuteRunApiProvider);
            workerThread.Start(threadParams);
        }

        private static void MoveContent(ItemNode sourceContent, ItemNode destinationFolder)
        {

            Content contentNew;
            Content originalContent;

            originalContent = Locator.Contents[sourceContent.ID];
            contentNew = originalContent.Clone();
            UpdateTreeNodeParentID(contentNew, destinationFolder);

            object[] threadParams = new object[4];
            threadParams[0] = contentNew;
            threadParams[1] = originalContent;
            threadParams[2] = sourceContent;
            threadParams[3] = destinationFolder;

            Thread workerThread = new Thread(ContentUpdater.MoveContentExecuteRunApiProvider);
            workerThread.Start(threadParams);
        }

        #endregion

        #region ContentVersion

        private static void CopyContentVersion(ItemNode source, ItemNode destination)
        {
            ContentVersion contentVersionNew;
            ContentVersion originalContentVersion;

            originalContentVersion = Locator.ContentVersions[source.ID];
            contentVersionNew = originalContentVersion.Clone();
            UpdateTreeNodeParentID(contentVersionNew, destination);

            object[] threadParams = new object[3];
            threadParams[0] = contentVersionNew;
            threadParams[1] = destination;
            threadParams[2] = true;

            Thread workerThread = new Thread(VercionUpdater.AddVersionExecuteRunApiProvider);
            workerThread.Start(threadParams);
        }

        private static void MoveContentVersion(ItemNode source, ItemNode destination)
        {
            ContentVersion contentVersionNew;
            ContentVersion originalContentVersion;

            originalContentVersion = Locator.ContentVersions[source.ID];
            contentVersionNew = originalContentVersion.Clone();
            UpdateTreeNodeParentID(contentVersionNew, destination);

            object[] threadParams = new object[4];
            threadParams[0] = contentVersionNew;
            threadParams[1] = originalContentVersion;
            threadParams[2] = source;
            threadParams[3] = destination;

            Thread workerThread = new Thread(VercionUpdater.MoveVersionExecuteRunApiProvider);
            workerThread.Start(threadParams);
        }

        #endregion

        #region Acknowledge ItemNode Action

        public static void ItemNodeAction(ItemNode source, ItemNode destination, ItemNodeActionType type)
        {
            String messageID="";
            List<String> parameters = new List<string>();

            switch (type)
            {
                case ItemNodeActionType.Copy:
                    messageID = "Copy";
                    ItemNodeTitle(source, parameters);
                    ItemNodeTitle(destination, parameters);
                    break;
                case ItemNodeActionType.Move:
                    messageID = "Move";
                    ItemNodeTitle(source, parameters);
                    ItemNodeTitle(destination, parameters);
                    break;
                case ItemNodeActionType.Delete:
                    messageID = "Delete";
                    ItemNodeTitle(source, parameters);
                    break;
            }

            ((Messenger) Messenger.Default).Send<MessageWrapperData>(new MessageWrapperData(messageID, parameters, false, MessageSender.MessageSenderItemNode, new List<object>(){type,source,destination}));
        }

        public static void ItemNodeTitle(ItemNode source, List<String> parameters)
        {
            if (source == null)
            {
                parameters.Add("folder");
                parameters.Add("Root");
                return;
            }

            switch (source.Type)
            {
                case TreeNodeObjectType.Folder:
                    parameters.Add("folder");
                    parameters.Add(source.Name);
                    return;

                case TreeNodeObjectType.Content:
                    parameters.Add("content");
                    parameters.Add(source.Name);
                    return;

                case TreeNodeObjectType.ContentVersion:
                    parameters.Add("version");
                    parameters.Add(source.Name);
                    return;
            }
        }

        #endregion

        #region Util functions TreeNode update Parent (UI & Dictionary)

        public static void UpdateParentItemNodeInUi(ItemNode sourceNode, ItemNode newParentNode)
        {
            if (sourceNode.Parent == null)
            {
                if (Locator.ContentManagerDataProvider.SubItemNode.Contains(sourceNode))
                    Locator.ContentManagerDataProvider.SubItemNode.Remove(sourceNode);
            }
            else
            {
                if (sourceNode.Parent.SubItemNode.Contains(sourceNode))
                    sourceNode.Parent.SubItemNode.Remove(sourceNode);                
            }

            sourceNode.Parent = newParentNode;

            if (newParentNode == null)
                Locator.ContentManagerDataProvider.SubItemNode.Add(sourceNode);
            else
                newParentNode.SubItemNode.Add(sourceNode);
        }

        #endregion

        #region Update ParentID

        private static void UpdateTreeNodeParentID(TreeNode treeNode, ItemNode destination)
        {
            treeNode.ChildID = GetChildIDForNewFolder(destination);
            treeNode.ParentID = destination == null ? 0 : destination.ID;
        }

        private static int GetChildIDForNewFolder(ItemNode folder)
        {
            int lastChildID = 0;
            ObservableCollection<ItemNode> subItemNode = folder != null
                                                             ? folder.SubItemNode
                                                             : Locator.ContentManagerDataProvider.SubItemNode;

            foreach (ItemNode subItem in subItemNode)
            {
                if (subItem.ChildID > lastChildID)
                    lastChildID = subItem.ChildID;
            }

            lastChildID++;
            return lastChildID;
        }

        #endregion
    }
}
