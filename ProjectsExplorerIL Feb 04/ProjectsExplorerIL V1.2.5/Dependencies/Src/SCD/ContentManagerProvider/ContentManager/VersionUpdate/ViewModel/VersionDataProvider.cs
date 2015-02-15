using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ContentManager.ContentManagerMainWindow.ViewModel;
using ContentManager.General;
using ContentManagerProvider;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ContentManager.VersionUpdate.ViewModel
{
    public class VersionDataProvider : ViewModelBase, IDataErrorInfo
    {
        public ItemNode VersionItem { get; set; }
        public ItemNode ParentFolderOfContentVersionItem { get; set; }
        public ItenNodeAction VersionAction { get; set; }
        public ItemNodeVersion VersionProperty { get; set; }
        
        public VersionDataProvider()
        {
            ActionName = "Add";
            ContentStatusList = null;
            VersionProperty = new ItemNodeVersion();
            OpenPathExecute = new RelayCommand(ActionOpenPathExecute, CanActionOpenPathExecute);
            CancelExecuteVersion = new RelayCommand(CancelVersionExecute);
            ActionExecuteVersion = new RelayCommand(ActionVersionExecute, CanActionVersionExecute);

            DeleteAllFilesCommand = new RelayCommand(ActionDeleteAllFiles, CanActionDeleteAllFiles);
            DeleteSelectedFilesCommand = new RelayCommand(ActionDeleteSelectedFiles, CanActionDeleteSelectedFiles);

            DeleteAllVersionsCommand = new RelayCommand(ActionDeleteAllVersions, CanActionDeleteAllVersions);
            DeleteSelectedVersionsCommand = new RelayCommand(ActionDeleteSelectedVersions, CanActionDeleteSelectedVersions);
        }

        public UserControlType ExecuteAction(ItemNode itemNode, ItenNodeAction action)
        {
            
            VersionAction = action;
           

            switch (VersionAction)
            {
                case ItenNodeAction.Add:
                    ActionName = "Add";
                    UpdateMode = true;
                    UpdateModeActive = true;
                    UpdateModeData = true;
                    UpdateModeEditor = true;
                    UpdateModeStatus = true;
                    UpdateModeProperty = true;
                    UpdateContentStatus(null);
                    ParentFolderOfContentVersionItem = itemNode;
                    VersionProperty.InitParameters(0);
                    return UserControlType.Version;

                case ItenNodeAction.Update:
                    ActionName = "Save";
                    VersionItem = itemNode;
                    ContentVersion version = Locator.ContentVersions[itemNode.ID];
                    UpdateMode = true;
                    UpdateModeActive = true;
                    UpdateModeData = UpdateMode && version.ExistPermission("UpdateData");
                    UpdateModeEditor = UpdateMode && version.ExistPermission("UpdateEditor");
                    UpdateModeStatus = UpdateMode && version.ExistPermission("UpdateStatus");
                    UpdateModeProperty = UpdateMode && version.ExistPermission("UpdateProperty");
                    UpdateContentStatus(version.Status);
                    VersionProperty.InitParameters(itemNode.ID);
                    return UserControlType.Version;

                case ItenNodeAction.View:
                    VersionItem = itemNode;
                    UpdateMode = false;
                    UpdateModeActive = false;
                    UpdateModeData = false;
                    UpdateModeEditor = false;
                    UpdateModeStatus = false;
                    UpdateModeProperty = false;
                    UpdateContentStatus(null);
                    VersionProperty.InitParameters(itemNode.ID);
                    return UserControlType.Version;

                case ItenNodeAction.Delete:
                    VersionItem = itemNode;
                    DeleteVersionExecute();
                    return UserControlType.None;
            }

            return UserControlType.None;
        }

        #region ContentStatusList

        private void UpdateContentStatus(ContentStatus status)
        {

            bool addStatus;
            ObservableContentStatus ooContentStatus;
            ObservableCollection<ObservableContentStatus> tempContentStatusList =
                new ObservableCollection<ObservableContentStatus>();
                
            if(status == null)
                tempContentStatusList.Add( new ObservableContentStatus {ID = "Sel", Name = "Select", Icon = ""});

            foreach (var content in Locator.ContentStatus)
            {

                if (status == null)
                {
                    addStatus = true;
                }
                else
                {
                    switch (status.ID)
                    {
                        case "Ac":
                            addStatus = content.Value.ID == "Ret" || content.Value.ID == "Ac";
                            break;

                        case "Edit":
                            addStatus = true;
                            break;

                        case "Ret":
                            addStatus = content.Value.ID == "Ret" || content.Value.ID == "Ac";
                            break;

                        default:
                            addStatus = false;
                            break;
                    }                   
                }

                if (addStatus)
                {
                    ooContentStatus = new ObservableContentStatus
                        {
                            ID = content.Value.ID,
                            Name = content.Value.Name,
                            Icon = content.Value.Icon
                        };
                    tempContentStatusList.Add(ooContentStatus);
                }
            }
            ContentStatusList = tempContentStatusList;
        }

        #endregion

        #region Observable objects

        private ObservableCollection<ObservableContentStatus> _contentStatuses;
        public ObservableCollection<ObservableContentStatus> ContentStatusList
        {
            get { return _contentStatuses; }
            set { Set(() => ContentStatusList, ref _contentStatuses, value); }
        }

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

        private bool _updateModeEditor;
        public bool UpdateModeEditor
        {
            get { return _updateModeEditor; }
            set { Set(() => UpdateModeEditor, ref _updateModeEditor, value); }
        }

        private bool _updateModeData;
        public bool UpdateModeData
        {
            get { return _updateModeData; }
            set { Set(() => UpdateModeData, ref _updateModeData, value); }
        }

        private bool _updateModeStatus;
        public bool UpdateModeStatus
        {
            get { return _updateModeStatus; }
            set { Set(() => UpdateModeStatus, ref _updateModeStatus, value); }
        }

        private bool _updateModeProperty;
        public bool UpdateModeProperty
        {
            get { return _updateModeProperty; }
            set { Set(() => UpdateModeProperty, ref _updateModeProperty, value); }
        }

        #endregion

        #region Commands

        public RelayCommand DeleteAllFilesCommand { get; set; }
        public RelayCommand DeleteSelectedFilesCommand { get; set; }

        public RelayCommand DeleteAllVersionsCommand { get; set; }
        public RelayCommand DeleteSelectedVersionsCommand { get; set; }

        public RelayCommand ActionExecuteVersion { get; set; }
        public RelayCommand CancelExecuteVersion { get; set; }
        public RelayCommand OpenPathExecute { get; set; }

        private void ActionDeleteAllFiles()
        {
            VersionProperty.SubItemNode.Clear();
        }

        private bool CanActionDeleteAllFiles()
        {
            return UpdateModeData && VersionProperty.SubItemNode.Count > 0;
        }

        private void ActionDeleteSelectedFiles()
        {
            DeleteSelectedItemFiles(VersionProperty.SubItemNode);
        }

        private bool CanActionDeleteSelectedFiles()
        {
            return UpdateModeData && ExistSelectedItemFiles(VersionProperty.SubItemNode);
        }

        private bool ExistSelectedItemFiles(IEnumerable<ItemFileNode> subItemNodes)
        {
            foreach (ItemFileNode fileNode in subItemNodes)
                if (fileNode.IsSelected || ExistSelectedItemFiles(fileNode.SubItemNode))
                    return true;

            return  false;
        }

        private void DeleteSelectedItemFiles(ObservableCollection<ItemFileNode> subItemNodes)
        {
            List<ItemFileNode> itemForDelete = new List<ItemFileNode>();

            foreach (ItemFileNode fileNode in subItemNodes)
            {
                if(fileNode.IsSelected)
                    itemForDelete.Add(fileNode);
            }

            foreach (ItemFileNode fileNode in itemForDelete)
                subItemNodes.Remove(fileNode);

            foreach (ItemFileNode fileNode in subItemNodes)
                DeleteSelectedItemFiles(fileNode.SubItemNode);
        }

        private void ActionDeleteAllVersions()
        {
            VersionProperty.SubItemVersionLinkNode.Clear();
        }

        private bool CanActionDeleteAllVersions()
        {
            return UpdateModeData && VersionProperty.SubItemVersionLinkNode.Count > 0;
        }

        private void ActionDeleteSelectedVersions()
        {
            List<ItemVersionLink> itemForDelete = new List<ItemVersionLink>();

            foreach (ItemVersionLink linkNode in VersionProperty.SubItemVersionLinkNode)
            {
                if (linkNode.IsSelected)
                    itemForDelete.Add(linkNode);
            }

            foreach (ItemVersionLink linkNode in itemForDelete)
                VersionProperty.SubItemVersionLinkNode.Remove(linkNode);
        }

        private bool CanActionDeleteSelectedVersions()
        {
            if (!UpdateModeData)
                return false;

            foreach (ItemVersionLink linkNode in VersionProperty.SubItemVersionLinkNode)
            {
                if (linkNode.IsSelected)
                    return true;
            }

            return false;
        }

        private void ActionOpenPathExecute()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = VersionProperty.Path
            });
        }

        private bool CanActionOpenPathExecute()
        {
            return Directory.Exists(VersionProperty.Path);
        }

        private bool CanActionVersionExecute()
        {
            return VersionProperty.AllPropertiesValid;
        }

        private void CancelVersionExecute()
        {
            Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);
        }

        private void ActionVersionExecute()
        {
            UpdateModeActive = false;
            switch (VersionAction)
            {
                case ItenNodeAction.Add:
                    AddVersionExecute();
                    break;
                case ItenNodeAction.Update:
                    UpdateVersionExecute();
                    break;
            }
        }

        private void AddVersionExecute()
        {
            ContentVersion contentVersionNew = new ContentVersion
            {
                Name = VersionProperty.Name,
                Description = VersionProperty.Description,
                ECR = VersionProperty.ECR,
                DocumentID = VersionProperty.DocumentID,
                Editor = VersionProperty.Editor,
                RunningString = VersionProperty.RunningString,
                Status = Locator.ContentStatus[VersionProperty.Status.ID],
                ChildID = GetChildIDForAddContentVersion(),
                Files = new Dictionary<int, ContentFile>(),
                ContentVersions = new Dictionary<int, ContentVersionSubVersion>(),
                ParentID = ParentFolderOfContentVersionItem.ID
            };

            for (int i=0; i < VersionProperty.SubItemVersionLinkNode.Count; i++)
            {
                ContentVersionSubVersion subVersion = new ContentVersionSubVersion
                    {
                        Order = i,
                        Content = Locator.Contents[contentVersionNew.ParentID],
                        ContentSubVersion = Locator.ContentVersions[VersionProperty.SubItemVersionLinkNode[i].ContentVersionID]
                    };
                contentVersionNew.ContentVersions.Add(subVersion.ContentSubVersion.ID, subVersion);
            }

            foreach (ItemFileNode fileNode in VersionProperty.SubItemNode)
                AddFilesToContentVersionFiles(contentVersionNew, String.Empty, fileNode, null);

            object[] threadParams = new object[3];
            threadParams[0] = contentVersionNew;
            threadParams[1] = ParentFolderOfContentVersionItem;
            threadParams[2] = false;

            Thread workerThread = new Thread(VercionUpdater.AddVersionExecuteRunApiProvider);
            workerThread.Start(threadParams);
        }

        private void UpdateVersionExecute()
        {
            ContentVersion contentVersionOriginal = Locator.ContentVersions[VersionItem.ID];
            ContentVersion contentVersionUpdated = new ContentVersion
            {
                ID = contentVersionOriginal.ID,
                Name = VersionProperty.Name,
                Description = VersionProperty.Description,
                ECR = VersionProperty.ECR,
                DocumentID = VersionProperty.DocumentID,
                Editor = VersionProperty.Editor,
                RunningString = VersionProperty.RunningString,
                Status = Locator.ContentStatus[VersionProperty.Status.ID],
                ChildID =  contentVersionOriginal.ChildID,
                Files = new Dictionary<int, ContentFile>(),
                ContentVersions = new Dictionary<int, ContentVersionSubVersion>(),
                ParentID = contentVersionOriginal.ParentID,
                Path= new PathFS(),
            };

            for (int i = 0; i < VersionProperty.SubItemVersionLinkNode.Count; i++)
            {
                ContentVersionSubVersion subVersion = new ContentVersionSubVersion
                    {
                        Order = i,
                        Content = Locator.Contents[VersionProperty.SubItemVersionLinkNode[i].ContentID],
                        ContentSubVersion = Locator.ContentVersions[VersionProperty.SubItemVersionLinkNode[i].ContentVersionID]
                    };
                contentVersionUpdated.ContentVersions.Add(subVersion.ContentSubVersion.ID, subVersion);
            }

            foreach (ItemFileNode fileNode in VersionProperty.SubItemNode)
                AddFilesToContentVersionFiles(contentVersionUpdated, String.Empty, fileNode, contentVersionOriginal);

            object[] threadParams = new object[3];
            threadParams[0] = contentVersionUpdated;
            threadParams[1] = contentVersionOriginal;
            threadParams[2] = VersionItem;

            Thread workerThread = new Thread(VercionUpdater.UpdateVersionExecuteRunApiProvider);
            workerThread.Start(threadParams);
        }

        private void AddFilesToContentVersionFiles(ContentVersion contentVersion, String relativePath, ItemFileNode file, ContentVersion contentVersionOriginal)
        {
            if (file.Type == ItemFileNodeType.File)
            {

                int newIndex;

                ContentFile newFile = new ContentFile
                    {
                        FileName = file.Name,
                        FileRelativePath = relativePath,
                        ID = file.ID
                    };



                if (file.ID != 0)
                {
                    newIndex = file.ID;
                    LastUpdate.UpdateLastUpdate(contentVersionOriginal.Files[file.ID], newFile);
                }
                else
                {
                    if (contentVersion.Files.Count == 0)
                        newIndex = -1;
                    else
                    {
                        if (contentVersion.Files.Keys.Min() < 0)
                            newIndex = contentVersion.Files.Keys.Min() - 1;
                        else
                            newIndex = -1;
                    }
                }

                if (String.IsNullOrEmpty(file.Path))
                    newFile.FileFullPath = VersionProperty.Path + "\\" + file.Name;
                else
                {
                    if (newFile.ID == 0) //New file
                        newFile.FileFullPath = file.Path;
                    else
                        newFile.FileFullPath = VersionProperty.Path + "\\" + file.Path + "\\" + file.Name;
                }
                contentVersion.Files.Add(newIndex, newFile);
            }
            else
            {
                foreach (ItemFileNode fileNode in file.SubItemNode)
                    AddFilesToContentVersionFiles(contentVersion, relativePath == String.Empty ? file.Name : relativePath + "\\" + file.Name, fileNode, contentVersionOriginal);
            }
        }

        private int GetChildIDForAddContentVersion()
        {
            int lastChildID = 0;

            foreach (ItemNode subItem in ParentFolderOfContentVersionItem.SubItemNode)
            {
                if (subItem.ChildID > lastChildID)
                    lastChildID = subItem.ChildID;
            }

            lastChildID++;
            return lastChildID;
        }

        private void DeleteVersionExecute()
        {
            object[] threadParams = new object[1];
            threadParams[0] = VersionItem;

            Thread workerThread = new Thread(VercionUpdater.DeleteVersionExecuteRunApiProvider);
            workerThread.Start(threadParams);
        }
        #endregion

        #region IDataErrorInfo members

        public string Error
        {
            get { return (VersionProperty as IDataErrorInfo).Error; }
        }

        public string this[string propertyName]
        {
            get
            {
                string error = (VersionProperty as IDataErrorInfo)[propertyName];
                CommandManager.InvalidateRequerySuggested();
                return error;
            }
        }

        #endregion
    }
}
