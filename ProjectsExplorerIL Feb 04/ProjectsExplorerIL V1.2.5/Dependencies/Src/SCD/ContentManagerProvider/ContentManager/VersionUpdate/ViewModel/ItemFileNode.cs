using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
using ContentManager.General;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ContentManager.VersionUpdate.ViewModel
{

    public enum ItemFileStatus
    {
        New,
        Exist,
        Updated
    }

    public enum ItemFileNodeType
    {
        File,
        Folder
    }
    //DeleteSelectedFiles
    public class ItemFileNode : ObservableObject
    {
        private const string DefaultFolderImage = "../../Images/Folder.bmp";
        private const string DefaultFileImage = "../../Images/ContentVersion.bmp";

        public ItemFileNode Parent { set; get; }
        public ObservableCollection<ItemFileNode> SubItemNode { set; get; }

        public ItemFileNode()
        {
            SubItemNode = new ObservableCollection<ItemFileNode>();
        }

        public int ID { set; get; }

        private ItemFileNodeType _type;
        public ItemFileNodeType Type
        {
            get { return _type; }
            set
            {
                switch (value)
                {
                    case ItemFileNodeType.File:
                        Icon = DefaultFileImage;
                        break;
                    case ItemFileNodeType.Folder:
                        Icon = DefaultFolderImage;
                        break;
                }
                Set(() => Type, ref _type, value);
            }
        }

        private ItemFileStatus _status;
        public ItemFileStatus Status
        {
            get { return _status; }
            set { Set(() => Status, ref _status, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set(() => Name, ref _name, value); }
        }

        private string _path;
        public string Path
        {
            get { return _path; }
            set { Set(() => Path, ref _path, value); }
        }

        private string _icon;
        public string Icon
        {
            get { return _icon; }
            set { Set(() => Icon, ref _icon, value); }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(() => IsSelected, ref _isSelected, value); }
        }

        public static bool CanAddItemFilesFs(ItemFileNodeType itemType, ObservableCollection<ItemFileNode> existingSubItemNodes, string[] files)
        {

            if (itemType != ItemFileNodeType.Folder)
                return false;

            foreach (string file in files)
                if (!CanAddItemFileFs(existingSubItemNodes, file))
                    return false;

            return true;
        }

        public static bool CanAddItemFileFs(ObservableCollection<ItemFileNode> existingSubItemNodes, string file)
        {
            if (existingSubItemNodes == null)
                return true;

            foreach (ItemFileNode fileNode in existingSubItemNodes)
            {
                if (fileNode.Name == System.IO.Path.GetFileName(file))
                    return false;
            }

            return true;
        }

        public static bool CanAddItemFileNode(ItemFileNode updatedItemFileNode, ItemFileNode itemFileNode)
        {
            ObservableCollection<ItemFileNode> parentSubItemNode;

            if (updatedItemFileNode == itemFileNode)
                return false;

            if (updatedItemFileNode == null)
                parentSubItemNode = Locator.VersionDataProvider.VersionProperty.SubItemNode;
            else
            {
                if (updatedItemFileNode.Type != ItemFileNodeType.Folder)
                    return false;

                parentSubItemNode = updatedItemFileNode.SubItemNode;
            }

            if (itemFileNode != null)
            {
                foreach (ItemFileNode fileNode in parentSubItemNode)
                {
                    if (fileNode.Name == itemFileNode.Name && fileNode != itemFileNode)
                        return false;
                }

                ItemFileNode destinationTemp = updatedItemFileNode;

                while (destinationTemp != null)
                {
                    if (itemFileNode == destinationTemp)
                        return false;

                    destinationTemp = destinationTemp.Parent;
                }
            }

            return true;
        }

        public static void UpdateParent(ItemFileNode updatedItemFileNode, ItemFileNode parent)
        {
            UpdateParentNodeStatusRecursive(updatedItemFileNode, ItemFileStatus.Exist, ItemFileStatus.Updated);

            if (updatedItemFileNode.Parent != null)
                updatedItemFileNode.Parent.SubItemNode.Remove(updatedItemFileNode);
            else
                Locator.VersionDataProvider.VersionProperty.SubItemNode.Remove(updatedItemFileNode);

            if (parent == null)
            {
                updatedItemFileNode.Parent = null;
                Locator.VersionDataProvider.VersionProperty.SubItemNode.Add(updatedItemFileNode);
            }
            else
            {
                updatedItemFileNode.Parent = parent;
                parent.SubItemNode.Add(updatedItemFileNode);
            }
        }

        private static void UpdateParentNodeStatusRecursive(ItemFileNode node, ItemFileStatus fromStatus, ItemFileStatus toStatus)
        {
            if(node.Status == fromStatus)
                node.Status = toStatus;

            foreach (ItemFileNode subNode in node.SubItemNode)
                UpdateParentNodeStatusRecursive(subNode, fromStatus, toStatus);
        }

        public static void AddSubItems(ItemFileNode patent, ObservableCollection<ItemFileNode> parentItemsCollection, string[] items, bool addFolderRecursive)
        {

            if (patent != null && patent.Type != ItemFileNodeType.Folder)
                return;

            foreach (string item in items)
            {
                ItemFileNode newItem = new ItemFileNode
                    {
                        ID = 0,
                        Path = item,
                        Status = ItemFileStatus.New,
                        SubItemNode = new ObservableCollection<ItemFileNode>()
                    };

                if (File.Exists(item))
                {
                    newItem.Type = ItemFileNodeType.File;
                    newItem.Name = System.IO.Path.GetFileName(item);
                }
                else
                {
                    newItem.Type = ItemFileNodeType.Folder;
                    newItem.Name = new DirectoryInfo(item).Name;
                }

                if (newItem.Type == ItemFileNodeType.Folder && addFolderRecursive)
                {
                    ItemFileNode.AddSubItems(newItem, newItem.SubItemNode, Directory.GetDirectories(item), true);
                    ItemFileNode.AddSubItems(newItem, newItem.SubItemNode, Directory.GetFiles(item), true);
                }

                newItem.Parent = patent;
                parentItemsCollection.Add(newItem);
            }
        }
    }
}
