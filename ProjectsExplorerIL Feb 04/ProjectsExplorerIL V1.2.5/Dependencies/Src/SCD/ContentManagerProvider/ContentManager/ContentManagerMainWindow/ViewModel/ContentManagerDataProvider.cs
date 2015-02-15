

using System;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ContentManager.General;
using ContentManagerProvider;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using TraceExceptionWrapper;

namespace ContentManager.ContentManagerMainWindow.ViewModel
{

    public enum UserControlTypeVisible
    {
        Na,
        Message,
        ProgressBar,
        WhereUsed
    }

    public class ContentManagerDataProvider : ViewModelBase
    {

        public TraceException LoadedTraceException { get; set; }

        private bool IsViewModeActive { get; set; }
        public RelayCommand AddFolder { get; set; }
        public RelayCommand<ItemNode> ChangeSelectedItem { get; set; }

        public ContentManagerDataProvider()
        {
            IsLoaded = false;
            IsViewModeActive = false;
            IsSerchVisible = false;

            if (Locator.UserName == "" || Locator.ConnectionString == "")
                return;

            try
            {
                Locator.ContentStatus = Locator.ContentManagerApiProvider.GetContentStatus();
                Locator.UserGroupTypes = Locator.ContentManagerApiProvider.GetUserGroupTypes();
                Locator.ContentType = Locator.ContentManagerApiProvider.GetContentType();
                Locator.SystemParameters = Locator.ContentManagerApiProvider.GetSystemParameters();

                List<String> permissions = new List<String> { { "Write" }, { "Update user group" }, { "Add folder to root" } };
                Dictionary<String, bool> applicationPermission = Locator.ContentManagerApiProvider.GetApplicationPermission(permissions);

                ApplicationWritePermission = applicationPermission.ContainsKey("Write") && applicationPermission["Write"];
                ApplicationUpdateUserGroupPermission = ApplicationWritePermission && applicationPermission.ContainsKey("Update user group") && applicationPermission["Update user group"];
                ApplicationAddRootFolderPermission = ApplicationWritePermission && applicationPermission.ContainsKey("Add folder to root") && applicationPermission["Add folder to root"];

                UserControlVisible = UserControlTypeVisible.Na;
                AddFolder = new RelayCommand(AddFolderFunc);
                UiControlVisible = UserControlType.None;
                SearchExecute = new RelayCommand(SearchExecuteFn);
                ViewMode = new RelayCommand(ViewModeFn);

                SubItemNode = Locator.ItemTreeBuilder.BuildItemTree(ApplicationWritePermission);
                ChangeSelectedItem = new RelayCommand<ItemNode>(ChangeSelectedItemExecute);

                IsLoaded = true;
            }
            catch (TraceException te)
            {
                LoadedTraceException = te;
                UserControlVisible = UserControlTypeVisible.Message;
            }
        }

        private void AddFolderFunc()
        {
            ActiveUserControl(UserControlType.Folder, ItenNodeAction.Add, null, true);
        }

        public void DeleteSubItemNode(ItemNode itemNode)
        {
            if (itemNode.Parent == null)
                SubItemNode.Remove(itemNode);
            else
                itemNode.Parent.DeleteSubItemNode(itemNode);
        }

        #region Selected item

        private void ChangeSelectedItemExecute(ItemNode selectedItem)
        {
            SelectedItem = selectedItem;

            if (SelectedItem != null && IsViewModeActive && !UiControlVisibleInEditMode)
                SelectedItem.ViewFunc();
        }

        private ItemNode _selectedItem;

        public ItemNode SelectedItem
        {
            get { return _selectedItem; }
            set { Set(() => SelectedItem, ref _selectedItem, value); }
        }

        #endregion

        #region UI controls manager

        public void ActiveUserControl(UserControlType controlType, ItenNodeAction itenAction, ItemNode item, bool editMode)
        {
            UiControlVisibleInEditMode = editMode;
            switch (controlType)
            {
                case UserControlType.Folder:
                    UiControlVisible = Locator.FoldersDataProvider.ExecuteAction(item, itenAction);
                    break;
                case UserControlType.Content:
                    UiControlVisible = Locator.ContentsDataProvider.ExecuteAction(item, itenAction);
                    break;
                case UserControlType.Version:
                    UiControlVisible = Locator.VersionDataProvider.ExecuteAction(item, itenAction);
                    break;
                case UserControlType.None:
                    UiControlVisible = UserControlType.None;
                    break;
            }
        }

        private bool UiControlVisibleInEditMode { set; get; }


        public void UpdateUiControlVisible(UserControlType ucType)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(
                    new Action(() => UpdateUiControlVisible(ucType)));
            else
                UiControlVisible = ucType;
        }

        private UserControlType UiControlVisible
        {
            set
            {
                switch (value)
                {
                    case UserControlType.Folder:
                        IsFolderUiVisible = true;
                        IsContentUiVisible = false;
                        IsVersionUiVisible = false;
                        break;

                    case UserControlType.Content:
                        IsFolderUiVisible = false;
                        IsContentUiVisible = true;
                        IsVersionUiVisible = false;
                        break;

                    case UserControlType.Version:
                        IsFolderUiVisible = false;
                        IsContentUiVisible = false;
                        IsVersionUiVisible = true;
                        break;

                    case UserControlType.None:
                        IsFolderUiVisible = false;
                        IsContentUiVisible = false;
                        IsVersionUiVisible = false;

                        if(SelectedItem != null && IsViewModeActive)
                            SelectedItem.ViewFunc();
                        break;
                }
            }
        }

        private ObservableCollection<ItemNode> _subItemNode;
        public ObservableCollection<ItemNode> SubItemNode
        {
            get { return _subItemNode; }
            set { Set(() => SubItemNode, ref _subItemNode, value); }
        }


        private bool _applicationAddRootFolderPermission;
        public bool ApplicationAddRootFolderPermission
        {
            get { return _applicationAddRootFolderPermission; }
            set { Set(() => ApplicationAddRootFolderPermission, ref _applicationAddRootFolderPermission, value); }
        }

        public bool ApplicationUpdateUserGroupPermission;

        private bool _applicationWritePermission;
        public bool ApplicationWritePermission
        {
            get { return _applicationWritePermission; }
            set { Set(() => ApplicationWritePermission, ref _applicationWritePermission, value); }
        }

        private bool _isLoaded;
        public bool IsLoaded
        {
            get { return _isLoaded; }
            set { Set(() => IsLoaded, ref _isLoaded, value); }
        }

        private bool _isSerchVisible;
        public bool IsSerchVisible
        {
            get { return _isSerchVisible; }
            set { Set(() => IsSerchVisible, ref _isSerchVisible, value); }
        }

        private bool _isFolderUiVisible;
        public bool IsFolderUiVisible
        {
            get { return _isFolderUiVisible; }
            set { Set(() => IsFolderUiVisible, ref _isFolderUiVisible, value); }
        }

        private bool _isContentUiVisible;
        public bool IsContentUiVisible
        {
            get { return _isContentUiVisible; }
            set { Set(() => IsContentUiVisible, ref _isContentUiVisible, value); }
        }

        private bool _isVersionUiVisible;
        public bool IsVersionUiVisible
        {
            get { return _isVersionUiVisible; }
            set { Set(() => IsVersionUiVisible, ref _isVersionUiVisible, value); }
        }

        private UserControlTypeVisible _userControlVisible;
        public UserControlTypeVisible UserControlVisible
        {
            get { return _userControlVisible; }
            set
            {
                Set(() => UserControlVisible, ref _userControlVisible, value);
            }
        }
   
        #endregion

        public RelayCommand ViewMode { get; set; }
        public RelayCommand SearchExecute { get; set; }

        private void ViewModeFn()
        {
            IsViewModeActive = !IsViewModeActive;

            if (!IsViewModeActive && !UiControlVisibleInEditMode)
                UiControlVisible = UserControlType.None;
            else
            {
                if (SelectedItem != null && IsViewModeActive)
                    SelectedItem.ViewFunc();                
            }
        }

        private void SearchExecuteFn()
        {
            bool isSerchVisible = IsSerchVisible;
            IsSerchVisible = !isSerchVisible;
        }
    }
}
