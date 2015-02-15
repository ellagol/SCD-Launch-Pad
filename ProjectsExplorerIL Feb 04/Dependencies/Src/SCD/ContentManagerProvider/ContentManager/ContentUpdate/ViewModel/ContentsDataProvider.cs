using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using ContentManager.ContentManagerMainWindow.ViewModel;
using ContentManager.General;
using ContentManager.Messanger.ViewModel;
using ContentManager.VersionUpdate.ViewModel;
using ContentManagerProvider;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;

namespace ContentManager.ContentUpdate.ViewModel
{
    public class ContentsDataProvider : ViewModelBase, IDataErrorInfo
    {
        private ItemNode ContentItem { get; set; }
        private ItemNode ParentFolderOfContentItem { get; set; }
        private ItenNodeAction ContentAction { get; set; }

        public ContentsDataProvider()
        {
            ActionName = "Add";
            UpdateContentType();
            ContentProperty = new ItemNodeContent();
            BrowseIconContent = new RelayCommand(BrowseIconFn);
            CancelExecuteContent = new RelayCommand(CancelContentExecute);
            ActionExecuteContent = new RelayCommand(ActionContentExecute, CanActionContentExecute);
        }

        public UserControlType ExecuteAction(ItemNode itemNode, ItenNodeAction action)
        {
            ContentAction = action;
            UpdateContentType();

            switch (ContentAction)
            {
                case ItenNodeAction.Add:
                    ActionName = "Add";
                    UpdateMode = true;
                    UpdateModeActive = true;
                    ParentFolderOfContentItem = itemNode;
                    InitUpdateParameters(null);
                    return UserControlType.Content;

                case ItenNodeAction.Update:
                    ActionName = "Save";
                    UpdateMode = true;
                    UpdateModeActive = true;
                    InitUpdateParameters(itemNode);
                    return UserControlType.Content;

                case ItenNodeAction.View:
                    UpdateMode = false;
                    UpdateModeActive = false;
                    InitUpdateParameters(itemNode);
                    return UserControlType.Content;

                case ItenNodeAction.Delete:
                    ContentItem = itemNode;
                    DeleteContentExecute();
                    return UserControlType.None;
            }

            return UserControlType.None;
        }

        private void InitUpdateParameters(ItemNode itemNode)
        {
            if (itemNode == null)
            {
                ContentProperty.Name = String.Empty;
                ContentProperty.Description = String.Empty;
                ContentProperty.Icon = String.Empty;
                ContentProperty.Type = ContentTypeList[0];
                ContentProperty.Type = null;
                ContentProperty.Type = ContentTypeList[0];
                ContentProperty.CertificateFree = false;
            }
            else
            {
                ContentItem = itemNode;
                Content content = Locator.Contents[ContentItem.ID];
                ContentProperty.Name = content.Name;
                ContentProperty.Description = content.Description;
                ContentProperty.Icon = content.IconFileFullPath;
                ContentProperty.Type = GetObservableContentTypeByID(content.ContentType.ID);
                ContentProperty.CertificateFree = content.CertificateFree;
            }
        }

        private ObservableContentType GetObservableContentTypeByID(String contentTypeID)
        {
            foreach (ObservableContentType observableContentType in ContentTypeList)
            {
                if (observableContentType.ID == contentTypeID)
                    return observableContentType;
            }

            return ContentTypeList[0];
        }

        private void UpdateContentType()
        {
            if (ContentTypeList != null || Locator.ContentType == null)
                return;

            ObservableContentType ooContentType;
            ObservableCollection<ObservableContentType> tempContentTypeList = new ObservableCollection
                <ObservableContentType>
                {
                    new ObservableContentType {ID = "Sel", Name = "Select"}
                };

            foreach (var contentType in Locator.ContentType)
            {
                ooContentType = new ObservableContentType
                {
                    ID = contentType.Value.ID,
                    Name = contentType.Value.Name
                };
                tempContentTypeList.Add(ooContentType);
            }
            ContentTypeList = tempContentTypeList;
        }

        #region Binding objects

        private ObservableCollection<ObservableContentType> _contentType;
        public ObservableCollection<ObservableContentType> ContentTypeList
        {
            get { return _contentType; }
            set { Set(() => ContentTypeList, ref _contentType, value); }
        }

        public ItemNodeContent ContentProperty { get; set; }

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

        #endregion

        #region Commands

        public RelayCommand BrowseIconContent { get; set; }
        public RelayCommand ActionExecuteContent { get; set; }
        public RelayCommand CancelExecuteContent { get; set; }

        private bool CanActionContentExecute()
        {
            return ContentProperty.AllPropertiesValid;
        }

        private void BrowseIconFn()
        {
            var dialog = new OpenFileDialog();

            if((bool) dialog.ShowDialog())
                ContentProperty.Icon = dialog.FileName;
        }

        private void CancelContentExecute()
        {
            Locator.ContentManagerDataProvider.UpdateUiControlVisible(UserControlType.None);
        }

        private void ActionContentExecute()
        {
            UpdateModeActive = false;

            switch (ContentAction)
            {
                case ItenNodeAction.Add:
                    AddContentExecute();
                    break;
                case ItenNodeAction.Update:
                    UpdateContentExecute();
                    break;
            }
        }


        private void AddContentExecute()
        {
            Content contentNew = new Content
            {
                Name = ContentProperty.Name,
                Description = ContentProperty.Description,
                IconFileFullPath = ContentProperty.Icon,
                ChildID = GetChildIDForAddFolder(),
                Versions = new Dictionary<int, ContentVersion>(),
                ParentID = ParentFolderOfContentItem != null ? ParentFolderOfContentItem.ID : 0,
                CertificateFree = ContentProperty.CertificateFree,
                ContentType = Locator.ContentType[ContentProperty.Type.ID]
            };

            object[] threadParams = new object[3];
            threadParams[0] = contentNew;
            threadParams[1] = ParentFolderOfContentItem;
            threadParams[2] = false;

            Thread workerThread = new Thread(ContentUpdater.AddContentExecuteRunApiProvider);
            workerThread.Start(threadParams);

        }

        private int GetChildIDForAddFolder()
        {
            int lastChildID = 0;

            if (ParentFolderOfContentItem != null)
            {
                foreach (ItemNode subItem in ParentFolderOfContentItem.SubItemNode)
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

        private void UpdateContentExecute()
        {
            Content contentOriginal = Locator.Contents[ContentItem.ID];
            Content contentUpdated = contentOriginal.Clone();

            contentUpdated.Name = ContentProperty.Name;
            contentUpdated.Description = ContentProperty.Description;
            contentUpdated.IconFileFullPath = ContentProperty.Icon;
            contentUpdated.CertificateFree = ContentProperty.CertificateFree;
            contentUpdated.ContentType = Locator.ContentType[ContentProperty.Type.ID];

            object[] threadParams = new object[3];
            threadParams[0] = contentUpdated;
            threadParams[1] = contentOriginal;
            threadParams[2] = ContentItem;

            Thread workerThread = new Thread(ContentUpdater.UpdateContentExecuteRunApiProvider);
            workerThread.Start(threadParams);
        }

        private void DeleteContentExecute()
        {

            Content contentToDelete = Locator.Contents[ContentItem.ID];

            object[] threadParams = new object[2];
            threadParams[0] = contentToDelete;
            threadParams[1] = ContentItem;

            Thread workerThread = new Thread(ContentUpdater.DeleteContentExecuteRunApiProvider);
            workerThread.Start(threadParams);
        }

        #endregion

        #region IDataErrorInfo members

        public string Error
        {
            get { return (ContentProperty as IDataErrorInfo).Error; }
        }

        public string this[string propertyName]
        {
            get
            {
                string error = (ContentProperty as IDataErrorInfo)[propertyName];
                CommandManager.InvalidateRequerySuggested();
                return error;
            }
        }

        #endregion
    }
}
