using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContentManager.General;
using ContentManagerProvider;

namespace ContentManager.ContentManagerMainWindow.ViewModel
{
    public static class TreeNodeDictionaryUpdater
    {
        #region Move

        public static void MoveFolder(Folder originalFolder, Folder updatedFolder)
        {

            Folder oldParentFolder = originalFolder.ParentID != 0 ? Locator.Folders[originalFolder.ParentID] : null;
            Folder newParentFolder = updatedFolder.ParentID != 0 ? Locator.Folders[updatedFolder.ParentID] : null;

            UpdateMovedTreeNodeInDictionary(originalFolder, updatedFolder);

            if (oldParentFolder != null)
                oldParentFolder.Nodes.Remove(originalFolder);

            if (newParentFolder != null)
                newParentFolder.Nodes.Add(originalFolder);
        }


        public static void MoveContent(Content originalContent, Content updatedContent)
        {

            Folder oldParentFolder = originalContent.ParentID != 0 ? Locator.Folders[originalContent.ParentID] : null;
            Folder newParentFolder = updatedContent.ParentID != 0 ? Locator.Folders[updatedContent.ParentID] : null;

            UpdateMovedTreeNodeInDictionary(originalContent, updatedContent);

            if (oldParentFolder != null)
                oldParentFolder.Nodes.Remove(originalContent);

            if (newParentFolder != null)
                newParentFolder.Nodes.Add(originalContent);
        }

        public static void MoveVersion(ContentVersion originalVersion, ContentVersion updatedVersion)
        {

            Content oldParentContent = originalVersion.ParentID != 0 ? Locator.Contents[originalVersion.ParentID] : null;
            Content newParentContent = updatedVersion.ParentID != 0 ? Locator.Contents[updatedVersion.ParentID] : null;

            UpdateMovedTreeNodeInDictionary(originalVersion, updatedVersion);

            if (oldParentContent != null)
                oldParentContent.Versions.Remove(originalVersion.ID);

            if (newParentContent != null)
                newParentContent.Versions.Add(originalVersion.ID, originalVersion);
        }

        public static void UpdateMovedTreeNodeInDictionary(TreeNode originalTreeNode, TreeNode newTreeNode)
        {
            originalTreeNode.ChildID = newTreeNode.ChildID;
            originalTreeNode.ParentID = newTreeNode.ParentID;
            LastUpdate.UpdateLastUpdate(newTreeNode, originalTreeNode);
        }

        #endregion

        #region Add

        public static void AddFolder(Folder folder, ItemNode parentFolderItem)
        {
            Locator.Folders.Add(folder.ID, folder);

            if (parentFolderItem != null)
                Locator.Folders[parentFolderItem.TreeNode.ID].Nodes.Add(folder);
        }

        public static void AddContent(Content content, ItemNode parentFolderItem)
        {
            Locator.Contents.Add(content.ID, content);

            if (parentFolderItem != null)
                Locator.Folders[parentFolderItem.TreeNode.ID].Nodes.Add(content);
        }

        public static void AddVersion(ContentVersion version, ItemNode parentFolderItem)
        {
            Locator.ContentVersions.Add(version.ID, version);

            if (parentFolderItem != null)
                Locator.Contents[parentFolderItem.TreeNode.ID].Versions.Add(version.ID, version);
        }

        #endregion

        #region Version

        public static void DeleteFolder(Folder folder)
        {
            if (folder.ParentID != 0)
            {
                Folder folderParant = Locator.Folders[folder.ParentID];
                folderParant.Nodes.Remove(folder);
            }

            Locator.Folders.Remove(folder.ID);
        }

        public static void DeleteContent(Content content)
        {
            if (content.ParentID != 0)
            {
                Folder folderParant = Locator.Folders[content.ParentID];
                folderParant.Nodes.Remove(content);
            }

            Locator.Contents.Remove(content.ID);
        }

        public static void DeleteVersion(ContentVersion version)
        {
            if (version.ParentID != 0)
            {
                Content contentParant = Locator.Contents[version.ParentID];
                contentParant.Versions.Remove(version.ID);
            }

            Locator.ContentVersions.Remove(version.ID);
        }

        #endregion

    }
}
