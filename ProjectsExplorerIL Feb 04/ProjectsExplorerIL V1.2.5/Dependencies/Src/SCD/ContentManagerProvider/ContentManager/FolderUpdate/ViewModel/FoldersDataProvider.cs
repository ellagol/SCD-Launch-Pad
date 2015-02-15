using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ContentManager.ContentManagerMainWindow.ViewModel;
using ContentManager.General;
using ContentManager.Messanger.ViewModel;
using ContentManagerProvider;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ContentManager.FolderUpdate.ViewModel
{
    public class FoldersDataProvider : ObservableObject, IDataErrorInfo
    {
        private ItemNode FolderItem { get; set; }
        private ItenNodeAction FolderAction { get; set; }

        public FoldersDataProvider()
        {
            ActionName = "Add";
            FolderProperty = new ItemNodeFolder();
            CancelExecuteFolder = new RelayCommand(CancelFolderExecute);
            ActionExecuteFolder = new RelayCommand(ActionFolderExecute, CanActionFolderExecute);
        }

        #region Execute Action

        public UserControlType ExecuteAction(ItemNode itemNode, ItenNodeAction action)
        {
            FolderItem = itemNode;
            FolderAction = action;
            Folder folder;

            folder = FolderItem == null ? null : Locator.Folders[FolderItem.ID];

            switch (FolderAction)
            {
                case ItenNodeAction.Add:
                    return ExecuteActionAdd(itemNode, action, folder);

                case ItenNodeAction.Update:
                    return ExecuteActionUpdate(itemNode, action, folder);

                case ItenNodeAction.View:
                    return ExecuteActionView(itemNode, action);

                case ItenNodeAction.Delete:
                    DeleteFolderExecute();
                    return UserControlType.None;
            }

            return UserControlType.None;
        }

        public UserControlType ExecuteActionAdd(ItemNode itemNode, ItenNodeAction action, Folder folder)
        {
            FolderItem = itemNode;
            FolderAction = action;
            ActionName = "Add";
            UpdateMode = true;
            UpdateModeActive = true;

            if(folder != null)
                UserGroupUpdatePermission = folder.ExistPermission("UpdateUserGroup") && UpdateMode;
            else
                UserGroupUpdatePermission = Locator.ContentManagerDataProvider.ApplicationUpdateUserGroupPermission && UpdateMode;
            
            FolderProperty.InitItemNodeFolder();
            return UserControlType.Folder;
        }

        public UserControlType ExecuteActionUpdate(ItemNode itemNode, ItenNodeAction action, Folder folder)
        {
            ActionName = "Update";
            UpdateMode = true;
            UpdateModeActive = true;
            UserGroupUpdatePermission = folder.ExistPermission("UpdateUserGroup") && UpdateMode;
            FolderProperty.InitItemNodeFolder(folder);
            return UserControlType.Folder;
        }

        public UserControlType ExecuteActionView(ItemNode itemNode, ItenNodeAction action)
        {
            UpdateMode = false;
            UpdateModeActive = false;
            UserGroupUpdatePermission = false;
            FolderProperty.InitItemNodeFolder(Locator.Folders[FolderItem.ID]);
            return UserControlType.Folder;
        }

        #endregion


        #region Binding objects

        public ItemNodeFolder FolderProperty { get; set; }

        private String _actionName;
        public String ActionName
        {
            get { return _actionName; }
            set { Set(() => ActionName, ref _actionName, value); }
        }

        private bool _updateMode;
        public bool UpdateMode 
        {
            get { return _updateMode; }
            set { Set(() => UpdateMode, ref _updateMode, value); }
        }

        private bool _updateModeActive;
        public bool UpdateModeActive 
        {
            get { return _updateModeActive; }
            set { Set(() => UpdateModeActive, ref _updateModeActive, value); }
        }

        private bool _userGroupUpdatePermission;
        public bool UserGroupUpdatePermission 
        {
            get { return _userGroupUpdatePermission; }
            set { Set(() => UserGroupUpdatePermission, ref _userGroupUpdatePermission, value); }
        }

        #endregion

        #region Commands

        public RelayCommand ActionExecuteFolder { get; set; }
        public RelayCommand CancelExecuteFolder { get; set; }

        private bool CanActionFolderExecute()
        {
            return FolderProperty.AllPropertiesValid;
        }

        private void CancelFolderExecute()
        {
            Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);
        }

        private void ActionFolderExecute()
        {
            UpdateModeActive = false;

            switch (FolderAction)
                {
                    case ItenNodeAction.Add:
                        AddFolderExecute();
                        break;
                    case ItenNodeAction.Update:
                        UpdateFolderExecute();
                        break;
                }
        }

        #endregion

        #region Add folder

        private void AddFolderExecute()
        {
            Folder folderNew = new Folder
                {
                    Name = FolderProperty.Name,
                    Description = FolderProperty.Description,
                    ChildID = GetChildIDForAddFolder(),
                    Nodes = new List<TreeNode>(),
                    Permission= new Dictionary<String, bool>(),
                    UserGroupTypePermission = new Dictionary<string, FolderUserGroupType>(),
                    ParentID = FolderItem != null ? FolderItem.ID : 0
                };

            UpdateUserGroupTypePermission(folderNew);

            object[] threadParams = new object[3];
            threadParams[0] = folderNew;
            threadParams[1] = FolderItem;
            threadParams[2] = false;

            Thread workerThread = new Thread(FolderUpdater.AddFolderExecuteRunApiProvider);
            workerThread.Start(threadParams);
        }

        private void UpdateUserGroupTypePermission(Folder folder)
        {
            Dictionary<String, FolderUserGroupType> userGroupTypePermission =
                new Dictionary<string, FolderUserGroupType>();

            foreach (UserGroupTypeObservable userGroupTypeObservable in FolderProperty.UserGroupTypeList)
            {
                if (userGroupTypeObservable.Checked)
                {
                    if (folder.UserGroupTypePermission.ContainsKey(userGroupTypeObservable.UserGroupType.ID))
                        userGroupTypePermission.Add(userGroupTypeObservable.UserGroupType.ID, folder.UserGroupTypePermission[userGroupTypeObservable.UserGroupType.ID]);
                    else
                        userGroupTypePermission.Add(userGroupTypeObservable.UserGroupType.ID, new FolderUserGroupType() { UserGroupType = userGroupTypeObservable.UserGroupType });
                }
            }

            folder.UserGroupTypePermission = userGroupTypePermission;
        }

        private int GetChildIDForAddFolder()
        {
            int lastChildID = 0;

            if (FolderItem != null)
            {
                foreach (ItemNode subItem in FolderItem.SubItemNode)
                {
                    if (subItem.ChildID > lastChildID)
                        lastChildID = subItem.ChildID;
                }
            }
            else
            {
                foreach (var folder in Locator.Folders)
                {
                    if (folder.Value.ParentID == 0)
                    {
                        if (folder.Value.ChildID > lastChildID)
                            lastChildID = folder.Value.ChildID;
                    }
                }
            }

            lastChildID++;
            return lastChildID;
        }

        #endregion

        #region Update folder

        private void UpdateFolderExecute()
        {
            Folder folderOld = Locator.Folders[FolderItem.ID];
            Folder folderNew = folderOld.Clone();

            folderNew.Name = FolderProperty.Name;
            folderNew.Description = FolderProperty.Description;
            UpdateUserGroupTypePermission(folderNew);

            object[] threadParams = new object[3];
            threadParams[0] = folderNew;
            threadParams[1] = folderOld;
            threadParams[2] = FolderItem;

            Thread workerThread = new Thread(FolderUpdater.UpdateFolderExecuteRunApiProvider);
            workerThread.Start(threadParams);
        }

        #endregion

        #region Delete folder

        private void DeleteFolderExecute()
        {
            Folder folderToDelete = Locator.Folders[FolderItem.ID];

            object[] threadParams = new object[2];
            threadParams[0] = folderToDelete;
            threadParams[1] = FolderItem;

            Thread workerThread = new Thread(FolderUpdater.DeleteFolderExecuteRunApiProvider);
            workerThread.Start(threadParams);
        }

        #endregion

        #region IDataErrorInfo members

        public string Error
        {
            get { return (FolderProperty as IDataErrorInfo).Error; }
        }

        public string this[string propertyName]
        {
            get
            {
                string error = (FolderProperty as IDataErrorInfo)[propertyName];
                CommandManager.InvalidateRequerySuggested();
                return error;
            }
        }

        #endregion

    }
}
