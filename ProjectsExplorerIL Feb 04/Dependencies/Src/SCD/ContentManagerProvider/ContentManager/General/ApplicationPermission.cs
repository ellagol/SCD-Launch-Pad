using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ContentManager.ContentManagerMainWindow.ViewModel;
using ContentManager.Messanger.ViewModel;
using ContentManagerProvider;
using GalaSoft.MvvmLight.Messaging;
using TraceExceptionWrapper;

namespace ContentManager.General
{
    public static class ApplicationPermission
    {
        private static void UpdatePermissionApiProvider(List<TreeNode> treeNodesList)
        {
            try
            {
                Locator.ContentManagerApiProvider.UpdatePermission(treeNodesList, Locator.Folders, Locator.Contents, Locator.ContentVersions);
            }
            catch (TraceException te)
            {
                ((Messenger)Messenger.Default).Send<MessageWrapperData>(new MessageWrapperData(te));
            }
        }

        private static void UpdatePermissionItemNode(ItemNode itemNode)
        {
            if (itemNode != null)
                itemNode.UpdatePermission();
        }

        private static void UpdatePermissionRecursiveItemNode(ItemNode itemNode)
        {
            itemNode.UpdatePermission();

            foreach (ItemNode subItemNode in itemNode.SubItemNode)
                UpdatePermissionRecursiveItemNode(subItemNode);
        }

        private static void UpdatePermissionRecursive(List<TreeNode> treeNodesList, ItemNode item)
        {
            treeNodesList.Add(item.TreeNode);

            foreach (ItemNode itemNode in item.SubItemNode)
                UpdatePermissionRecursive(treeNodesList, itemNode);
        }

        public static void UpdatePermissionAddNode(ItemNode parentItem, ItemNode newItem)
        {
            List<TreeNode> treeNodesList = new List<TreeNode>();

            if (parentItem != null)
                treeNodesList.Add(parentItem.TreeNode);

            if (newItem != null)
                treeNodesList.Add(newItem.TreeNode);

            if (treeNodesList.Count > 0)
            {
                UpdatePermissionApiProvider(treeNodesList);

                if (parentItem != null)
                    UpdatePermissionItemNode(parentItem);

                if (newItem != null)
                    UpdatePermissionItemNode(newItem);
            }
        }

        public static void UpdatePermissionUpdateNode(ItemNode updatedItem)
        {
            List<TreeNode> treeNodesList = new List<TreeNode> { updatedItem.TreeNode };

            if (updatedItem.Parent != null)
                treeNodesList.Add(updatedItem.Parent.TreeNode);

            UpdatePermissionApiProvider(treeNodesList);

            updatedItem.UpdatePermission();

            UpdatePermissionItemNode(updatedItem);
            UpdatePermissionItemNode(updatedItem.Parent);
        }

        public static void UpdatePermissionDeleteNode(ItemNode parentItem)
        {
            if (parentItem != null)
            {
                UpdatePermissionApiProvider(new List<TreeNode> { parentItem.TreeNode });
                UpdatePermissionItemNode(parentItem);
            }
        }

        public static void UpdatePermissionMoveNode(ItemNode fromItem, ItemNode toItem, ItemNode movedItem)
        {
            List<TreeNode> treeNodesList = new List<TreeNode>();

            if (fromItem != null)
                treeNodesList.Add(fromItem.TreeNode);

            if (toItem != null)
                treeNodesList.Add(toItem.TreeNode);

            UpdatePermissionRecursive(treeNodesList, movedItem);
            UpdatePermissionApiProvider(treeNodesList);

            UpdatePermissionItemNode(toItem);
            UpdatePermissionItemNode(fromItem);
            UpdatePermissionRecursiveItemNode(movedItem);
        }

        public static void UpdatePermissionTreeNodeList(List<TreeNode> treeNodesList)
        {
            UpdatePermissionApiProvider(treeNodesList);
            UpdatePermissionTreeNodeListItemNode(treeNodesList, Locator.ContentManagerDataProvider.SubItemNode);
        }

        private static void UpdatePermissionTreeNodeListItemNode(List<TreeNode> treeNodesList, ObservableCollection<ItemNode> itemNodes)
        {
            foreach (ItemNode itemNode in itemNodes)
            {
                if(treeNodesList.Contains(itemNode.TreeNode))
                    itemNode.UpdatePermission();

                UpdatePermissionTreeNodeListItemNode(treeNodesList, itemNode.SubItemNode);
            }
        }
    }
}
