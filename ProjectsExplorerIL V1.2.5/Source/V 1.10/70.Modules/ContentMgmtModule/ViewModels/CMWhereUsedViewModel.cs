using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ATSBusinessLogic;
using ATSBusinessLogic.ContentMgmtBLL;
using ATSBusinessObjects;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using Infra.MVVM;

namespace ContentMgmtModule
{
    public class CMWhereUsedViewModel : ViewModelBase
    {

        #region  Data

        protected MessengerService MessageMediator = new MessengerService();

        private IMessageBoxService MsgBoxService = null;

        private Guid WorkspaceId;

        public string ConnectionString;

        public static Dictionary<int, CMFolderModel> folders = new Dictionary<int, CMFolderModel>();

        public static Dictionary<int, CMContentModel> contents = new Dictionary<int, CMContentModel>();

        public static Dictionary<int, CMVersionModel> versions = new Dictionary<int, CMVersionModel>();

        #endregion

        #region Presentation Properties

        private CMWhereUsedProjectItemModel _SelectedProject = null;
        public CMWhereUsedProjectItemModel SelectedProject
        {
            get
            {
                return _SelectedProject;
            }
            set
            {
                _SelectedProject = value;
                RaisePropertyChanged("SelectedProject");
            }
        }

        private ObservableCollection<CMWhereUsedProjectItemModel> _contentProjects;
        public ObservableCollection<CMWhereUsedProjectItemModel> ContentProjects
        {
            get
            {
                return _contentProjects;
            }
            set
            {
                _contentProjects = value;
                RaisePropertyChanged("ContentProjects");
            }
        }

        private CMWhereUsedContentLinkItemModel _SelectedContent = null;
        public CMWhereUsedContentLinkItemModel SelectedContent
        {
            get
            {
                return _SelectedContent;
            }
            set
            {
                _SelectedContent = value;
                RaisePropertyChanged("SelectedContent");
            }
        }

        private ObservableCollection<CMWhereUsedContentLinkItemModel> _contentLinks;
        public ObservableCollection<CMWhereUsedContentLinkItemModel> ContentLinks
        {
            get
            {
                return _contentLinks;
            }
            set
            {
                _contentLinks = value;
                RaisePropertyChanged("ContentLinks");
            }
        }

        public ObservableCollection<CMWhereUsedProjectItemModel> allContentProjects = new ObservableCollection<CMWhereUsedProjectItemModel>();
        public ObservableCollection<CMWhereUsedProjectItemModel> onlyActiveVersionContentProjects = new ObservableCollection<CMWhereUsedProjectItemModel>();

        private Boolean _UsedInClosedVersions = false;
        public Boolean UsedInClosedVersions
        {

            get
            {
                return _UsedInClosedVersions;
            }
            set
            {
                _UsedInClosedVersions = value;
                RaisePropertyChanged("UsedInClosedVersions");
                if (UsedInClosedVersions)
                {
                    ContentProjects = allContentProjects;
                }
                else
                {
                    ContentProjects = onlyActiveVersionContentProjects;
                }
                RaisePropertyChanged("ContentProjects");
            }
        }

        #endregion

        #region Constructor

        public CMWhereUsedViewModel(Guid workspaceId, CMTreeViewNodeViewModelBase TV)
        {
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();

            //init workscpace
            this.WorkspaceId = workspaceId;
            ConnectionString = Domain.DbConnString;

            SelectedProject = new CMWhereUsedProjectItemModel();
            ContentProjects = new ObservableCollection<CMWhereUsedProjectItemModel>();

            SelectedContent = new CMWhereUsedContentLinkItemModel();
            ContentLinks = new ObservableCollection<CMWhereUsedContentLinkItemModel>();
            UpdateContentVersion((int)TV.ID);
        }

        #endregion

        #region Methods

        public void UpdateContentVersion(int contentVersionID)
        {
            
            Dictionary<int, HierarchyModel> contentVersionProjects =
                ApiBLL.GetProjectsByContentVersionID(contentVersionID);

            ContentProjects = new ObservableCollection<CMWhereUsedProjectItemModel>();

            foreach (KeyValuePair<int, HierarchyModel> versionProject in contentVersionProjects)
            {
                foreach (VersionModel pv in versionProject.Value.GetAllVersions)
                {
                    string vStatus;
                    if (pv.VersionStatus == "A")
                    {
                        vStatus = "Active";
                        onlyActiveVersionContentProjects.Add(new CMWhereUsedProjectItemModel
                        {
                            Name = versionProject.Value.Name,
                            Step = versionProject.Value.SelectedStep,
                            Code = versionProject.Value.Code,
                            HierarchyPath = versionProject.Value.TreeHeader,
                            VersionName = pv.VersionName,
                            VersionStatus = vStatus
                        });
                    }
                    else
                    {
                        vStatus = "Closed";
                    }
                    allContentProjects.Add(new CMWhereUsedProjectItemModel
                    {
                        Name = versionProject.Value.Name,
                        Step = versionProject.Value.SelectedStep,
                        Code = versionProject.Value.Code,
                        HierarchyPath = versionProject.Value.TreeHeader,
                        VersionName = pv.VersionName,
                        VersionStatus = vStatus
                    });
                }
            }
            ContentProjects = onlyActiveVersionContentProjects;

            ContentLinks = new ObservableCollection<CMWhereUsedContentLinkItemModel>();

            //Performance # 3474 - uncomment to reverse
            //CMContentsReaderBLL cr = new CMContentsReaderBLL();
            //cr.UpdateReferenceList();

            //versions = new Dictionary<int, CMVersionModel>();

            //folders = cr.GetFoldersDictionary();
            //contents = cr.GetContentsDictionaryByID(null, versions, true);

            //t = DateTime.Now;
            //MS = t.Millisecond;
            //sMS = MS.ToString();
            //Console.WriteLine(t + "." + sMS + " end cr.GetContentsDictionaryByID(null, versions, true);");
            //Console.WriteLine(t + "." + sMS + " start foreach (KeyValuePair<int, CMVersionModel> contentVersion in versions)");
            //foreach (KeyValuePair<int, CMVersionModel> contentVersion in versions)
            //{
            //    foreach (KeyValuePair<int, CMContentVersionSubVersionModel> contentVersionSubVersion in contentVersion.Value.ContentVersions)
            //    {
            //        if (contentVersionSubVersion.Value.ContentSubVersion.ID == contentVersionID)
            //        {
            //            ContentLinks.Add(new CMWhereUsedContentLinkItemModel
            //            {
            //                ContentName = contents[contentVersion.Value.ParentID].Name,
            //                VersionName = contentVersion.Value.Name
            //            });
            //            break;
            //        }
            //    }
            //}
            //t = DateTime.Now;
            //MS = t.Millisecond;
            //sMS = MS.ToString();
            //Console.WriteLine(t + "." + sMS + " end foreach (KeyValuePair<int, CMVersionModel> contentVersion in versions)");

            ContentLinks = CMVersionBLL.GetListOfWhereUsedByVersionId(contentVersionID);

            //end 3474
        }

        #endregion

        #region  Commands

        #region  Close Command
/*
        private RelayCommand _CloseCommand;
            public RelayCommand CloseCommand
            {
                get
                {
                    if (_CloseCommand == null)
                    {
                        _CloseCommand = new RelayCommand(ExecuteCloseCommand, CanExecuteCloseCommand);
                    }
                    return _CloseCommand;
                }
            }

            private bool CanExecuteCloseCommand()
            {
                return true;
            }

            private void ExecuteCloseCommand()
            {
                MessageMediator.NotifyColleagues(WorkspaceId + "CloseDetailsView", this); //Will be returned to the MainWindow signed for this message, to remove the node from the TreeView
            }
        */
            #endregion 

        #region Open Content In Another Workspace

        private RelayCommand _OpenContentInAnotherWorkspaceCommand;
        public ICommand OpenContentInAnotherWorkspaceCommand
        {
            get
            {
                if (_OpenContentInAnotherWorkspaceCommand == null)
                {
                    _OpenContentInAnotherWorkspaceCommand = new RelayCommand(ExecuteOpenContentInAnotherWorkspaceCommand, CanExecuteOpenContentInAnotherWorkspaceCommand);
                }
                return _OpenContentInAnotherWorkspaceCommand;
            }
        }

        private bool CanExecuteOpenContentInAnotherWorkspaceCommand()
        {
            return true;
        }

        private void ExecuteOpenContentInAnotherWorkspaceCommand()
        {
            MessageMediator.NotifyColleagues("RequestGotoCm", SelectedContent); //Send message to the MainViewModel to clear Statusbar from any previous operation
        }

        #endregion

        #region Open Project In Pe

        private RelayCommand _OpenProjectInPe;
        public ICommand OpenProjectInPe
        {
            get
            {
                if (_OpenProjectInPe == null)
                {
                    _OpenProjectInPe = new RelayCommand(ExecuteOpenProjectInPe, CanExecuteOpenProjectInPe);
                }
                return _OpenProjectInPe;
            }
        }

        private bool CanExecuteOpenProjectInPe()
        {
            return true;
        }

        private void ExecuteOpenProjectInPe()
        {
            MessageMediator.NotifyColleagues("RequestGotoPeProject", SelectedProject); //Send message to the MainViewModel to clear Statusbar from any previous operation
        }

        #endregion

        #endregion

    }
}
