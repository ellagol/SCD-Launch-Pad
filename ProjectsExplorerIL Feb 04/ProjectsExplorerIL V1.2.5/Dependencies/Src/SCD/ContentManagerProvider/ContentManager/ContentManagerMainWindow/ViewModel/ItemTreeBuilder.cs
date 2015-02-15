using System;
using System.Collections.Generic;
using ContentManager.General;
using ContentManagerProvider;
using System.IO;
using System.Collections.ObjectModel;

namespace ContentManager.ContentManagerMainWindow.ViewModel
{
    public class ItemTreeBuilder
    {
        private const string DefaultFolderImage = "../../Images/Folder.bmp";
        private const string DefaultContentImage = "../../Images/Content.bmp";
        private const string DefaultContentVersionImage = "../../Images/ContentVersion.bmp";

        public ObservableCollection<ItemNode> BuildItemTree(bool writePermission)
        {
            IEnumerable<TreeNode> treeNodeList = GetTreeNodes();
            ObservableCollection<ItemNode> rootItemNodes = new ObservableCollection<ItemNode>();

            foreach(var itemNode in treeNodeList)
                rootItemNodes.Add(BuildItemNode(itemNode, null, writePermission));

            return rootItemNodes;
        }

        public static void UpdateItemNodePermission(TreeNode treeNode, ItemNode itemNode, bool writePermission)
        {
            bool addPermission;
            bool updatePermission;
            bool deletePermission;

            if (!writePermission)
            {
                addPermission = false;
                updatePermission = false;
                deletePermission = false;
            }
            else
            {
                addPermission = treeNode.ExistPermission("Add");
                updatePermission = treeNode.ExistPermission("Update");
                deletePermission = treeNode.ExistPermission("Delete");
            }

            itemNode.IsDelete = deletePermission;
            itemNode.IsUpdate = updatePermission;

            switch (treeNode.TreeNodeType)
            {
                case TreeNodeObjectType.Folder:
                    itemNode.IsAddFolder = addPermission;
                    itemNode.IsAddContent = addPermission;
                    itemNode.IsAddVersion = false;
                    itemNode.IsWhereUsed = false;
                    break;
                case TreeNodeObjectType.Content:
                    itemNode.IsAddFolder = false;
                    itemNode.IsAddContent = false;
                    itemNode.IsWhereUsed = false;
                    itemNode.IsAddVersion = addPermission;
                    break;
                case TreeNodeObjectType.ContentVersion:
                    itemNode.IsAddFolder = false;
                    itemNode.IsAddContent = false;
                    itemNode.IsAddVersion = false;
                    itemNode.IsWhereUsed = true;
                    break;
            }
        }

        public ItemNode BuildItemNode(TreeNode node, ItemNode parent, bool writePermission)
        {
            ItemNode itemNode = new ItemNode
                {
                    ID = node.ID,
                    Name = node.Name,
                    ChildID = node.ChildID,
                    Type = node.TreeNodeType,
                    Parent = parent,
                    TreeNode = node,
                    SubItemNode = new ObservableCollection<ItemNode>()
                };

            UpdateItemNodePermission(node, itemNode, writePermission);

            switch (node.TreeNodeType)
	        {
		        case TreeNodeObjectType.Folder:
                    BuildFolder((Folder)node, itemNode, writePermission);
                    break;

                case TreeNodeObjectType.Content:
                    BuildContent((Content) node, itemNode, writePermission);
                    break;

                case TreeNodeObjectType.ContentVersion:
                    BuildContentVersion((ContentVersion) node, itemNode);
                    break;
	        }

            return itemNode;
        }

        public static void UpdateContentIcon(Content contentNode, ItemNode itemNode)
        {
            if (string.IsNullOrEmpty(contentNode.IconFileFullPath) || !File.Exists(contentNode.IconFileFullPath))
                itemNode.Icon = DefaultContentImage;
            else
                itemNode.Icon = contentNode.IconFileFullPath; 
        }

        private void BuildFolder(Folder node, ItemNode itemNode, bool writePermission)
        {
            ItemNode itemNodeTemp;
            itemNode.Icon = DefaultFolderImage;

            foreach (var item in node.Nodes)
            {
                itemNodeTemp = BuildItemNode(item, itemNode, writePermission);
                itemNode.SubItemNode.Add(itemNodeTemp);
            }
        }

        private void BuildContent(Content node, ItemNode itemNode, bool writePermission)
        {
            ItemNode itemNodeTemp;
            UpdateContentIcon(node, itemNode);

            foreach (var item in node.Versions)
            {
                itemNodeTemp = BuildItemNode(item.Value, itemNode, writePermission);
                itemNode.SubItemNode.Add(itemNodeTemp);
            }
        }

        private void BuildContentVersion(ContentVersion node, ItemNode itemNode)
        {
            if (File.Exists(node.Status.Icon))
            {
                    itemNode.Icon = node.Status.Icon;
                    return; 
            }

            itemNode.Icon = DefaultContentVersionImage;
        }

        public static string GetContentVersionIcon(ContentVersion node)
        {
            if (File.Exists(node.Status.Icon))
                return node.Status.Icon;

            return DefaultContentVersionImage;
        }

        private IEnumerable<TreeNode> GetTreeNodes()
        {
            Dictionary<int, Folder> folders;
            Dictionary<int, Content> contents;
            Dictionary<int, ContentVersion> versions;

            List<TreeNode> treeNodeList = Locator.ContentManagerApiProvider.GetTreeObjectsCm(out folders, out contents, out versions);
            
            Locator.Folders = folders;
            Locator.Contents = contents;
            Locator.ContentVersions = versions;
            
            return treeNodeList;
        }
    }
}
