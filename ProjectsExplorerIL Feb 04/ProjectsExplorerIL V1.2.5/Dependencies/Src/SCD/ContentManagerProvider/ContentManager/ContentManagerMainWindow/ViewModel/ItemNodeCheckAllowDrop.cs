using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContentManager.General;
using ContentManagerProvider;

namespace ContentManager.ContentManagerMainWindow.ViewModel
{
    public static class ItemNodeCheckAllowDrop
    {

        public static bool AllowDrop(ItemNode sourceNode, ItemNode destinationNode, ItemNodeActionType actionType)
        {

            if (destinationNode == null)
                return DropToRoot(sourceNode);

            if (!CheckItemNodePermission(sourceNode, destinationNode))
                return false;

            if (!CheckDropParentToChaild(sourceNode, destinationNode))
                return false;

            switch (destinationNode.Type)
            {
                case TreeNodeObjectType.Folder:
                    return CheckDestinationNodeFolder(sourceNode, destinationNode, actionType);

                case TreeNodeObjectType.Content:
                    return CheckDestinationNodeContent(sourceNode, destinationNode, actionType);

                case TreeNodeObjectType.ContentVersion:
                    return CheckDestinationNodeVersion(sourceNode, destinationNode, actionType);
            }

            return false;
        }

        private static bool DropToRoot(ItemNode sourceNode)
        {
            return sourceNode.Type == TreeNodeObjectType.Folder && Locator.ContentManagerDataProvider.ApplicationAddRootFolderPermission;
        }

        private static bool CheckItemNodePermission(ItemNode sourceNode, ItemNode destinationNode)
        {
            return sourceNode.IsUpdate && destinationNode.IsUpdate && Locator.ContentManagerDataProvider.ApplicationWritePermission;
        }

        private static bool CheckDropParentToChaild(ItemNode sourceNode, ItemNode destinationNode)
        {
            ItemNode destinationTemp = destinationNode;
            while (destinationTemp != null)
            {
                if (sourceNode == destinationTemp)
                    return false;

                destinationTemp = destinationTemp.Parent;
            }

            return true;
        }

        private static bool CheckDestinationNodeVersion(ItemNode sourceNode, ItemNode destinationNode, ItemNodeActionType actionType)
        {
            return false;
        }

        private static bool CheckDestinationNodeFolder(ItemNode sourceNode, ItemNode destinationNode, ItemNodeActionType actionType)
        {
            switch (sourceNode.Type)
            {
                case TreeNodeObjectType.Folder:

                    switch (actionType)
                    {
                        case ItemNodeActionType.Copy:
                            return true;
                        case ItemNodeActionType.Move:
                            return !ExistSubNoteWihtSumName(sourceNode, destinationNode);
                         default:
                            return false;
                    }

                case TreeNodeObjectType.Content:
                    return true;
                case TreeNodeObjectType.ContentVersion:
                    return false;
            }
            return false;
        }

        private static bool CheckDestinationNodeContent(ItemNode sourceNode, ItemNode destinationNode, ItemNodeActionType actionType)
        {
            switch (sourceNode.Type)
            {
                case TreeNodeObjectType.Folder:
                    return false;
                case TreeNodeObjectType.Content:
                    return false;
                case TreeNodeObjectType.ContentVersion:
                    switch (actionType)
                    {
                        case ItemNodeActionType.Copy:
                            return true;
                        case ItemNodeActionType.Move:
                            return !ExistSubNoteWihtSumName(sourceNode, destinationNode);
                        default:
                            return false;
                    }
            }
            return false;
        }

        private static bool ExistSubNoteWihtSumName(ItemNode sourceNode, ItemNode destinationNode)
        {
            foreach (ItemNode subItem in destinationNode.SubItemNode)
            {
                if (subItem.Name == sourceNode.Name && subItem != sourceNode)
                    return true;
            }

            return false;
        }
    }
}
