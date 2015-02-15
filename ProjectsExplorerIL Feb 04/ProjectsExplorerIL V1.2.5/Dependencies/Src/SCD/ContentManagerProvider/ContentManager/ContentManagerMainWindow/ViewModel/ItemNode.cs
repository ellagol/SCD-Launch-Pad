using System.Collections.Generic;
using ContentManager.General;
using ContentManager.Messanger.ViewModel;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using ContentManagerProvider;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ContentManager.ContentManagerMainWindow.ViewModel
{
    public class ItemNode : ObservableObject
    {
        public ItemNode()
        {
            Update = new RelayCommand(UpdateFunc);
            Delete = new RelayCommand(DeleteFunc);
            WhereUsed = new RelayCommand(WhereUsedFunc);
            Add = new RelayCommand<string>(AddFunc);
        }

        public void DeleteSubItemNode(ItemNode itemNode)
        {
            SubItemNode.Remove(itemNode);
        }

        public void UpdatePermission()
        {
            ItemTreeBuilder.UpdateItemNodePermission(TreeNode, this, Locator.ContentManagerDataProvider.ApplicationWritePermission);
        }

        virtual public bool Contains(string text) 
        {
            return TreeNode.Contains(text);
        }

        #region Properties

        public ObservableCollection<ItemNode> SubItemNode { set; get; }

        private int _id;
        public int ID
        {
            get { return _id; }
            set { Set(() => ID, ref _id, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set(() => Name, ref _name, value); }
        }

        private string _icon;
        public string Icon
        {
            get { return _icon; }
            set { Set(() => Icon, ref _icon, value); }
        }

        private int _childID;
        public int ChildID
        {
            get { return _childID; }
            set { Set(() => ChildID, ref _childID, value); }
        }

        private TreeNodeObjectType _type;
        public TreeNodeObjectType Type
        {
            get { return _type; }
            set { Set(() => Type, ref _type, value); }
        }
        
        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { Set(() => IsExpanded, ref _isExpanded, value); }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(() => IsSelected, ref _isSelected, value); }
        }

        public TreeNode TreeNode { set; get; }
        public ItemNode Parent { set; get; }
    
        #endregion

        #region Is ability options

        private bool _isWhereUsed;
        public bool IsWhereUsed
        {
            get { return _isWhereUsed; }
            set { Set(() => IsWhereUsed, ref _isWhereUsed, value); }
        }

        public bool IsAdd
        {
            get { return IsAddFolder || IsAddContent || IsAddVersion; }
        }

        private bool _isDelete;
        public bool IsDelete
        {
            get
            {
                return _isDelete;
            }
            set
            {
                Set(() => IsDelete, ref _isDelete, value);
            }
        }

        private bool _isAddFolder;
        public bool IsAddFolder
        {
            get
            {
                return _isAddFolder;
            }
            set
            {
                Set(() => IsAddFolder, ref _isAddFolder, value);
            }
        }

        private bool _isAddContent;
        public bool IsAddContent
        {
            get
            {
                return _isAddContent;
            }
            set
            {
                Set(() => IsAddContent, ref _isAddContent, value);
            }
        }

        private bool _isAddVersion;
        public bool IsAddVersion
        {
            get
            {
                return _isAddVersion;
            }
            set
            {
                Set(() => IsAddVersion, ref _isAddVersion, value);
            }
        }

        private bool _isUpdate;
        public bool IsUpdate
        {
            get { return _isUpdate; }
            set
            {
                Set(() => IsUpdate, ref _isUpdate, value);
            }
        }

        #endregion
        
        #region Commands

        public RelayCommand Update { get; set; }
        public RelayCommand Delete { get; set; }
        public RelayCommand WhereUsed { get; set; }
        public RelayCommand<string> Add { get; set; }

        public void ViewFunc()
        {
            switch (Type)
            {
                case TreeNodeObjectType.Folder:
                    Locator.ContentManagerDataProvider.ActiveUserControl(UserControlType.Folder, ItenNodeAction.View, this, false);
                    break;

                case TreeNodeObjectType.Content:
                    Locator.ContentManagerDataProvider.ActiveUserControl(UserControlType.Content, ItenNodeAction.View, this, false);
                    break;

                case TreeNodeObjectType.ContentVersion:
                    Locator.ContentManagerDataProvider.ActiveUserControl(UserControlType.Version, ItenNodeAction.View, this, false);
                    break;
            }
        }

        public void AddFunc(string param)
        {
            IsSelected = true;
            switch (param)
            {
                case "Folder":
                    Locator.ContentManagerDataProvider.ActiveUserControl(UserControlType.Folder, ItenNodeAction.Add, this, true);
                    break;

                case "Content":
                    Locator.ContentManagerDataProvider.ActiveUserControl(UserControlType.Content, ItenNodeAction.Add, this, true);
                    break;

                case "Version":
                    Locator.ContentManagerDataProvider.ActiveUserControl(UserControlType.Version, ItenNodeAction.Add, this, true);
                    break;
            }
        }

        public void UpdateFunc()
        {
            IsSelected = true;
            switch (Type)
            {
                case TreeNodeObjectType.Folder:
                    Locator.ContentManagerDataProvider.ActiveUserControl(UserControlType.Folder, ItenNodeAction.Update, this, true);
                    break;

                case TreeNodeObjectType.Content:
                    Locator.ContentManagerDataProvider.ActiveUserControl(UserControlType.Content, ItenNodeAction.Update, this, true);
                    break;

                case TreeNodeObjectType.ContentVersion:
                    Locator.ContentManagerDataProvider.ActiveUserControl(UserControlType.Version, ItenNodeAction.Update, this, true);
                    break;
            }
        }

        public void DeleteFunc()
        {
            IsSelected = true;
            ItemNodeCopyMove.ItemNodeAction(this, null, ItemNodeActionType.Delete);
        }

        public void WhereUsedFunc()
        {
            Locator.WhereUsedViewModel.UpdateContentVersion(ID);
            Locator.ContentManagerDataProvider.UserControlVisible = UserControlTypeVisible.WhereUsed;
        }

        #endregion
    }
}
