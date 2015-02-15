using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ATSBusinessLogic.ContentMgmtBLL;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using Infra.DragDrop;
using Infra.MVVM;
using Microsoft.Win32;
using TraceExceptionWrapper;


namespace ContentMgmtModule
{
    public class CMVersionDetailsViewModel : ViewModelBase, IDropTarget
    {
      
        #region  Data

        protected MessengerService MessageMediator = new MessengerService();

        private IMessageBoxService MsgBoxService = null;

        private Guid WorkspaceId;

        public static int globalTabIndex = 0;

        private int _tabIndex;
        public int tabIndex 
        {
            get
            {
                return _tabIndex;
            }
            set
            {
                globalTabIndex = value;
                _tabIndex = value;
            }

        }

        public CMVersionModel initCmVersion = new CMVersionModel();

        public CMContentsReaderBLL cr { set; get; }

        public static Dictionary<int, CMFolderModel> folders = new Dictionary<int, CMFolderModel>();

        public static Dictionary<int, CMContentModel> contents = new Dictionary<int, CMContentModel>();

        public static Dictionary<int, CMVersionModel> versions = new Dictionary<int, CMVersionModel>();

        public static CMImpersonationBLL imp = new CMImpersonationBLL();

        public bool updateDragDropMode { get; set; }

        public bool linkedVersionErr { get; set; }

        public bool ConfigurationManagementLinkErr { get; set; }

        public static bool editMode = false; //in order to know in the tree behavior if the version in edit mode

        public static CMProgressBarViewModel ProgressBarDataProvider { get; set; }

        private CMVersionModel _CMVersion = null;
        public CMVersionModel CMVersion
        {
            get
            {
                return _CMVersion;
            }
            set
            {
                _CMVersion = value;
                RaisePropertyChanged("EditVersionStatusType");
            }
        }

        private CMTreeViewNodeViewModelBase _CMParentVersionModel;
        public CMTreeViewNodeViewModelBase CMParentVersionModel
        {
            get
            {
                return _CMParentVersionModel;
            }
            set
            {
                _CMParentVersionModel = value;
            }
        }

        private List<KeyValuePair<string, string>> _VersionStatusTypesList;
        public List<KeyValuePair<string, string>> VersionStatusTypesList
        {
            get
            {
                if (_VersionStatusTypesList == null)
                {
                    _VersionStatusTypesList = new List<KeyValuePair<string, string>>();
                    List<KeyValuePair<string, string>> tempVersionStatusTypesList = new List<KeyValuePair<string, string>>();
                    tempVersionStatusTypesList = CMVersionBLL.GetVersiontStatusList();
                    foreach (KeyValuePair<string, string> TypeKeyValue in tempVersionStatusTypesList)
                    {
                        KeyValuePair<string, string> tempTypeKeyValue = new KeyValuePair<string, string>(TypeKeyValue.Value, TypeKeyValue.Key);

                        if (CMVersion.id_ContentVersionStatus.Trim() == "Ac" || CMVersion.id_ContentVersionStatus.Trim() == "Ret")
                        {
                            if (tempTypeKeyValue.Value == "Ac" || tempTypeKeyValue.Value == "Ret")
                            {
                                _VersionStatusTypesList.Add(tempTypeKeyValue);
                            }
                        }
                        else
                        {
                            _VersionStatusTypesList.Add(tempTypeKeyValue);
                        }
                    }
                }

                return _VersionStatusTypesList;
            }
        }

        private ObservableCollection<CMItemVersionLink> _SubItemVersionLinkNode = new ObservableCollection<CMItemVersionLink>();
        public ObservableCollection<CMItemVersionLink> SubItemVersionLinkNode
        {
            get
            {
                return _SubItemVersionLinkNode;
            }
            set
            {
                _SubItemVersionLinkNode = value;
                RaisePropertyChanged("SubItemVersionLinkNode");
            }
        }

        private ObservableCollection<CMItemFileNode> _SubItemNode = new ObservableCollection<CMItemFileNode>();
        public ObservableCollection<CMItemFileNode> SubItemNode
        {
            get
            {
                return _SubItemNode;
            }
            set
            {
                _SubItemNode = value;
                RaisePropertyChanged("SubItemNode");
            }
        }

        #endregion

        #region Presentation Properties

        private CMTreeViewNodeViewModelBase Node;

        [Required(ErrorMessage = "'Name' field is required."), StringLength(50, ErrorMessage = "Maximum length (50 characters) exceeded.")]
        public string VersionName
        {
            get
            {
                if (CMVersion != null)
                {

                    return CMVersion.Name;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (CMVersion != null)
                {
                    CMVersion.Name = value;
                    RaisePropertyChanged("VersionName");
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }
            }
        }

        [StringLength(1000, ErrorMessage = "Maximum length (1000 characters) exceeded.")]
        public string VersionDescription
        {
            get
            {
                if (CMVersion != null)
                {
                    return CMVersion.Description;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (CMVersion != null)
                {
                    CMVersion.Description = value;
                    RaisePropertyChanged("VersionDescription");
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }
            }
        }

        [StringLength(50, ErrorMessage = "Maximum length (50 characters) exceeded.")]
        public string ECR
        {
            get
            {
                if (CMVersion != null)
                {
                    return CMVersion.ECR;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (CMVersion != null)
                {
                    CMVersion.ECR = value;
                    RaisePropertyChanged("ECR");
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }
            }
        }

        [StringLength(50, ErrorMessage = "Maximum length (50 characters) exceeded.")]
        public string DocumentID
        {
            get
            {
                if (CMVersion != null)
                {
                    return CMVersion.DocumentID;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (CMVersion != null)
                {
                    CMVersion.DocumentID = value;
                    RaisePropertyChanged("DocumentID");
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }
            }
        }

        [StringLength(50, ErrorMessage = "Maximum length (50 characters) exceeded.")]
        public string PDMDocumentVersion
        {
            get
            {
                if (CMVersion != null)
                {
                    return CMVersion.PDMDocumentVersion;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (CMVersion != null)
                {
                    CMVersion.PDMDocumentVersion = value;
                    RaisePropertyChanged("PDMDocumentVersion");
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }
            }
        }

        [StringLength(1000, ErrorMessage = "Maximum length (1000 characters) exceeded.")]
        public string ConfigurationManagementLink
        {
            get
            {
                if (CMVersion != null)
                {
                    return CMVersion.ConfigurationManagementLink;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (CMVersion != null)
                {
                    CMVersion.ConfigurationManagementLink = value;
                    RaisePropertyChanged("ConfigurationManagementLink");
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }
            }
        }

        public string EditVersionStatusType
        {
            get
            {
                if (CMVersion != null)
                {
                    return CMVersion.id_ContentVersionStatus.Trim();
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (CMVersion != null)
                {
                    CMVersion.id_ContentVersionStatus = value;
                    RaisePropertyChanged("EditVersionStatusType");
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }
            }
        }

        [StringLength(100, ErrorMessage = "Maximum length (100 characters) exceeded.")]
        public string LockNotes
        {
            get
            {
                if (CMVersion != null)
                {
                    return CMVersion.LockWithDescription.Trim();
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (CMVersion != null)
                {
                    CMVersion.LockWithDescription = value;
                    RaisePropertyChanged("LockNotes");
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }
            }
        }

        public string _editorName;
        public string EditorName
        {
            get
            {

                return _editorName;

            }
            set
            {
                _editorName = value;
                RaisePropertyChanged("EditorName");
            }
        }

        public string CreationDate
        {
            get
            {
                if (CMVersion.CreationDate == DateTime.MinValue)
                {
                    return "";
                }
                else
                {
                    return CMVersion.CreationDate.ToString();
                } 
            }
            set
            {
            }
        }

        public string LastUpdateTime
        {
            get
            {
                if (CMVersion.LastUpdateTime == DateTime.MinValue)
                {
                    return "";
                }
                else
                {
                    return CMVersion.LastUpdateTime.ToString();
                }
            }
            set
            {
            }
        }

        public string LastUpdateUser
        {
            get
            {
                if (CMVersion != null)
                {
                    return CMVersion.LastUpdateUser;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
            }
        }

        private bool _isVisibleEditorName;
        public bool IsVisibleEditorName
        {
            get
            {

                return _isVisibleEditorName;

            }
            set
            {
                _isVisibleEditorName = value;
                RaisePropertyChanged("IsVisibleEditorName");
            }
        }

        [StringLength(1000, ErrorMessage = "Maximum length (1000 characters) exceeded.")]
        public string CommandLine
        {
            get
            {
                if (CMVersion != null)
                {
                    return CMVersion.CommandLine.Trim();
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (CMVersion != null)
                {
                    CMVersion.CommandLine = value;
                    RaisePropertyChanged("CommandLine");
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }
            }
        }

        [StringLength(1000, ErrorMessage = "Maximum length (1000 characters) exceeded.")]
        public string Path
        {
            get
            {
                if (CMVersion != null)
                {
                    return CMVersion.Path.Name;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (CMVersion != null)
                {
                    CMVersion.Path.Name = value;
                    RaisePropertyChanged("Path");
                }
            }
        }

        private bool _ActionMode = false;
        public bool ActionMode
        {
            get
            {
                return _ActionMode;
            }
            set
            {
                _ActionMode = value;
                RaisePropertyChanged("ActionMode");
            }
        }

        public bool UpdateMode
        {
            get
            {
                if (CMVersion != null)
                {
                    return CMVersion.UpdateMode;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (CMVersion != null)
                {
                    CMVersion.UpdateMode = value;
                    RaisePropertyChanged("UpdateMode");
                }
            }
        }

        private String _ActionName = "";
        public String ActionName
        {
            get
            {
                return _ActionName;
            }
            set
            {
                _ActionName = value;
                RaisePropertyChanged("ActionName");
            }
        }

        private bool _updateModeProperty;
        public bool UpdateModeProperty
        {
            get
            {
                return _updateModeProperty;
            }
            set
            {
                _updateModeProperty = value;
                RaisePropertyChanged("UpdateModeProperty");
            }
        }
        private bool _IsReadOnly;
        public bool IsReadOnly
        {
            get
            {
                return _IsNameReadOnly;
            }
            set
            {
                _IsNameReadOnly = value;
            }
        }

        private bool _UpdateNameModeProperty;
        public bool UpdateNameModeProperty
        {
            get
            {
                return _UpdateNameModeProperty;
            }
            set
            {
                _UpdateNameModeProperty = value;
                RaisePropertyChanged("UpdateNameModeProperty");
                IsReadOnly = !_UpdateNameModeProperty;
            }
        }
        private bool _IsNameReadOnly;
        public bool IsNameReadOnly
        {
            get
            {
                return _IsNameReadOnly;
            }
            set
            {
                _IsNameReadOnly = value;
                RaisePropertyChanged("IsNameReadOnly");
            }
        }

        private bool _updateModeData;
        public bool UpdateModeData
        {
            get
            {
                return _updateModeData;
            }
            set
            {
                _updateModeData = value;
                RaisePropertyChanged("UpdateModeData");
                IsDataReadOnly = !_updateModeData;
            }
        }
        private bool _IsDataReadOnly;
        public bool IsDataReadOnly
        {
            get
            {
                return _IsDataReadOnly;
            }
            set
            {
                _IsDataReadOnly = value;
                RaisePropertyChanged("IsDataReadOnly");
            }
        }
        private bool _updateModeEditor;
        public bool UpdateModeEditor
        {
            get
            {
                return _updateModeEditor;
            }
            set
            {
                _updateModeEditor = value;
                RaisePropertyChanged("UpdateModeEditor");
            }
        }

        private bool _updateModeStatus;
        public bool UpdateModeStatus
        {
            get
            {
                return _updateModeStatus;
            }
            set
            {
                _updateModeStatus = value;
                RaisePropertyChanged("UpdateModeStatus");
            }
        }

        private CMItemVersionLink _SelectedLinkedVersion = null;
        public CMItemVersionLink SelectedLinkedVersion
        {
            get
            {
                return _SelectedLinkedVersion;
            }
            set
            {
                _SelectedLinkedVersion = value;
                RaisePropertyChanged("SelectedLinkedVersion");
            }
        }

        #endregion

        #region Constructor

        public CMVersionDetailsViewModel(Guid workspaceId, CMTreeViewNodeViewModelBase TV, bool _updateDragDropMode)
        {
            //set tab to same index as last version was
            tabIndex = globalTabIndex;
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();

            //init workscpace
            this.WorkspaceId = workspaceId;
            //init version node
            Node = TV;

            ConfigurationManagementLinkErr = true; //init for status bar

            updateDragDropMode = _updateDragDropMode;

            initCmVersion = CMVersionBLL.GetVersionRow(Node.ID);
            initCmVersion.ID = (int)initCmVersion.Id;

            cr = new CMContentsReaderBLL();
            cr.UpdateReferenceList();
            cr.UpdateContentVersionFiles(initCmVersion);    //get version files

            UpdateVersionLinkedVersions(initCmVersion); // get version linked versions if an update occured              

            if (Node.IsView == true) //view action
            {
                ActionName = "";
            }
            else if (Node.IsUpdate) //update action
            {
                ActionName = "Update";
                ActionMode = true;

                UpdateNameModeProperty = UpdateModeProperty = Node.IsUpdate && ((CMVersionModel)(Node.TreeNode)).ExistPermission("UpdateProperty");
                if (initCmVersion.id_ContentVersionStatus == "Ac") //if stutus is active do not allow to update version name
                    UpdateNameModeProperty = false;

                UpdateModeData = Node.IsUpdate && ((CMVersionModel)(Node.TreeNode)).ExistPermission("UpdateData");
                editMode = UpdateModeData;
                UpdateModeEditor = Node.IsUpdate && ((CMVersionModel)(Node.TreeNode)).ExistPermission("UpdateEditor");
                UpdateModeStatus = Node.IsUpdate && ((CMVersionModel)(Node.TreeNode)).ExistPermission("UpdateStatus");

            }
            else if (Node.ID == -1) //add action
            {
                ActionName = "Add";
                Node.IsUpdate = true;
                ActionMode = true;
            }

            //init version details
            CMVersion = new CMVersionModel();
            CMVersion.Id = initCmVersion.Id;
            CMVersion.Name = initCmVersion.Name;
            CMVersion.Description = initCmVersion.Description;
            CMVersion.ECR = initCmVersion.ECR;
            CMVersion.DocumentID = initCmVersion.DocumentID;
            CMVersion.PDMDocumentVersion = initCmVersion.PDMDocumentVersion;
            CMVersion.ConfigurationManagementLink = initCmVersion.ConfigurationManagementLink;
            CMVersion.id_ContentVersionStatus = initCmVersion.id_ContentVersionStatus;
            CMVersion.LockWithDescription = initCmVersion.LockWithDescription;
            CMVersion.CommandLine = initCmVersion.CommandLine;
            CMVersion.id_Content = initCmVersion.id_Content;
            CMVersion.ContentVersions = initCmVersion.ContentVersions;
            CMVersion.Files = initCmVersion.Files;
            CMVersion.UpdateMode = Node.IsUpdate;

            CMVersion.LastUpdateUser = initCmVersion.LastUpdateUser;
            CMVersion.CreationDate = initCmVersion.CreationDate;
            //CMVersion.LastUpdateApplication = initCmVersion.LastUpdateApplication;
            //CMVersion.LastUpdateComputer = initCmVersion.LastUpdateComputer;
            CMVersion.LastUpdateTime = ((CMVersionModel)(TV.TreeNode)).LastUpdateTime;

            EditorName = CMVersion.LockWithDescription == String.Empty ? String.Empty : CMVersion.LastUpdateUser;
            IsVisibleEditorName = EditorName != String.Empty;

            if (initCmVersion.Path.Type == ATSBusinessObjects.ContentMgmtModels.CMVersionModel.PathType.Full)
            {
                CMVersion.Path.Name = initCmVersion.Path.Name;
            }
            else
            {
                CMVersion.Path.Name = CMTreeNodeBLL.getRootPath() + "\\" + initCmVersion.Path.Name;
            }
            if (CMVersion.Id == -1)
                InitAddParameters(CMVersion.Id, CMVersion.id_Content);
            else
                InitUpdateParameters(CMVersion.Id, CMVersion.id_Content);

            RaisePropertyChanged("SubItemVersionLinkNode");

        }

        public CMVersionDetailsViewModel(Guid workspaceId, CMTreeViewNodeViewModelBase currTV, CMTreeViewNodeViewModelBase parentTV, bool _updateDragDropMode)
        {
            //set tab to same index as last version was
            tabIndex = globalTabIndex;
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();
            //init workscpace
            this.WorkspaceId = workspaceId;
            //init folder details
            Node = currTV;

            ConfigurationManagementLinkErr = true; //init for status bar

            updateDragDropMode = _updateDragDropMode;

            CMParentVersionModel = parentTV;
            initCmVersion = CMVersionBLL.GetVersionRow(Node.ID);

            CMVersion = new CMVersionModel();

            CMVersion.Id = initCmVersion.Id;
            //CR 3483
            CMVersion.Name = CMVersionBLL.GenerateDefaultVersionName(parentTV.ID);

            CMVersion.id_Content = parentTV.ID;

            ActionName = "Add";
            Node.IsUpdate = true;
            Node.IsView = true;
            ActionMode = true;
            CMVersion.UpdateMode = true;

            UpdateModeData = true;
            UpdateModeEditor = true;
            UpdateModeStatus = true;
            UpdateModeProperty = true;
            UpdateNameModeProperty = true;

            //Assign new version permissions
            CMUpdatePermissionBLL upPer = new CMUpdatePermissionBLL(CMContentManagementViewModel.versions);
            upPer.GetContentVersionPermission(((CMVersionModel)(Node.TreeNode)));
        }

        #endregion

        #region Commands

        #region Open Path Execute

        private RelayCommand _OpenPathExecute;
        public ICommand OpenPathExecute
        {
            get
            {
                if (_OpenPathExecute == null)
                {
                    _OpenPathExecute = new RelayCommand(ExecuteOpenPathExecute, CanExecuteOpenPathExecute);
                }
                return _OpenPathExecute;
            }
        }

        private bool CanExecuteOpenPathExecute()
        {
            return Directory.Exists(CMVersion.Path.Name);
        }

        private void ExecuteOpenPathExecute()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = CMVersion.Path.Name
            });

        }

        #endregion

        #region Browse Files Execute

        private RelayCommand _BrowseFilesExecute;
        public ICommand BrowseFilesExecute
        {
            get
            {
                if (_BrowseFilesExecute == null)
                {
                    _BrowseFilesExecute = new RelayCommand(ActionChooseFilesExecute, CanActionOpenPathExecute);
                }
                return _BrowseFilesExecute;
            }
        }

        private bool CanActionOpenPathExecute()
        {
            return UpdateModeData;
        }

        private void ActionChooseFilesExecute()
        {
            List<String> parameters = new List<string>();
            var SB = new StringBuilder(string.Empty);
            string dir1 = Directory.GetCurrentDirectory();
            Microsoft.Win32.OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            Nullable<bool> result = dlg.ShowDialog();
            Directory.SetCurrentDirectory(dir1);
            CMItemFileNode fn = null;
  
            if (result == true)
            {
                string displayMessage;
                string[] itemToReplace = new string[1]; //the item that will replace the removed file each iteration
                ObservableCollection<CMItemFileNode> TempExistingSubItemNodes = new ObservableCollection<CMItemFileNode>();

                foreach (CMItemFileNode it in SubItemNode)
                {
                    TempExistingSubItemNodes.Add(it);
                }
                foreach (string FileName in (string[])dlg.FileNames)
                {
                    itemToReplace[0] = FileName;
                    FileInfo FI = new FileInfo(FileName);
                    parameters.Clear();
                    String errorId = "FileExists";
                    SB.Append("SELECT ED_Description FROM ErrorDescription where ED_ID='" + errorId + "';");
                    displayMessage = (Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString();
                    parameters.Add(FI.Name);
                    displayMessage = CMContentManagementViewModel.UpdateMessageStringParameters(displayMessage, parameters);

                    if (!CMItemFileNode.CanAddItemFilesFs(ItemFileNodeType.Folder, TempExistingSubItemNodes, itemToReplace, ref fn))
                    {
                        if (MsgBoxService.ShowYesNo(displayMessage, DialogIcons.Question) == DialogResults.Yes)
                        {
                            SubItemNode.Remove(fn);
                        }
                        else //else do nothing and return
                        {
                            continue; //check next file
                        }
                    }

                    AddSubItems(null, SubItemNode, itemToReplace, true);
                }

     
            }
            MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
        }

        #endregion

        #region  Cancel Command

        private RelayCommand _CancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                if (_CancelCommand == null)
                {
                    _CancelCommand = new RelayCommand(ExecuteCancelCommand, CanExecuteCancelCommand);
                }
                return _CancelCommand;
            }
        }

        private bool CanExecuteCancelCommand()
        {
            return true;
        }

        private void ExecuteCancelCommand()
        {
           // MessageMediator.NotifyColleagues(WorkspaceId + "CloseDetailsView", this); //Will be returned to the MainWindow signed for this message, to remove the node from the TreeView
        }

        #endregion

        #region  Save Command

        private RelayCommand _SaveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                {
                    _SaveCommand = new RelayCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
                }
                return _SaveCommand;
            }
        }

        private bool CanExecuteSaveCommand()
        {
            //Bug Fix : 2901, 2902. 
            //Application.Current.Dispatcher.BeginInvoke(new ThreadStart(() =>
            //{
            //    CommandManager.InvalidateRequerySuggested();

            //}));
            linkedVersionErr = true;

            if ((String.IsNullOrEmpty(VersionName) || VersionName.Length > 50 || VersionDescription.Length > 1000 || ECR.Length > 50
                || DocumentID.Length > 50 || Path.Length > 1000 || CommandLine.Length > 1000 || LockNotes.Length > 100 ||
                PDMDocumentVersion.Length > 50 || ConfigurationManagementLink.Length > 1000 || !IsConfigurationManagementLinkRequired() 
                || !((CMTreeViewVersionNodeViewModel)(Node)).TreeNode.ExistPermission("Update")))
                return false;

            //Moved validation to execute SaveCommand - CR 3523

            //if this version status is "Edit" and it has linked versions with status "Edit" disble add/update button
            //if (initCmVersion.id_ContentVersionStatus == "Edit" && CMVersion.id_ContentVersionStatus != "Edit")
            //{
            //    foreach (CMItemVersionLink vl in SubItemVersionLinkNode)
            //    {
            //        if (vl.Status == "Edit")
            //        {
            //            return false;
            //        }
            //    }
            //}

            try
            {
                linkedVersionErr = (new CMItemNodeVersionVersionLinkConfirmer().ConfirmerContentVersion(SubItemVersionLinkNode, (int)CMVersion.Id, (int)CMVersion.id_Content, CMContentManagementViewModel.versions));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void ExecuteSaveCommand()
        {
            try
            {
                MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", null);
                //Work variables
                Collection<string> StatusBarParameters = new Collection<string>();
                List<String> parameters = new List<string>();
                CMVersionModel v;
                var SB = new StringBuilder(string.Empty);

                if (initCmVersion.id_ContentVersionStatus == "Edit" && CMVersion.id_ContentVersionStatus != "Edit")
                {
                    foreach (CMItemVersionLink vl in SubItemVersionLinkNode)
                    {
                        if (vl.Status == "Edit")
                        {
                            object[] ArgMessageParam = { vl.Name };
                            CMContentManagementViewModel.ShowErrorAndInfoMessage("Linked version in edit", ArgMessageParam);
                            return;
                        }
                    }
                }

                v = CMVersion;

                //to see later if there was change in the linkedVersions
                v.BeforeUpdateLinkedVersions.Clear();
                foreach (int keyLinkedVersion in v.ContentVersions.Keys)
                {
                    v.BeforeUpdateLinkedVersions.Add(keyLinkedVersion, v.ContentVersions[keyLinkedVersion]);
                }

                // add or update linked versions
                v.ContentVersions.Clear();

                for (int i = 0; i < SubItemVersionLinkNode.Count; i++)
                {
                    CMContentVersionSubVersionModel subVersion = new CMContentVersionSubVersionModel
                    {
                        Order = i,
                        Content = CMContentBLL.GetContentRow(SubItemVersionLinkNode[i].ContentID),
                        ContentSubVersion = CMVersionBLL.GetVersionRow(SubItemVersionLinkNode[i].ContentVersionID)
                    };
                    subVersion.ContentSubVersion.Name = SubItemVersionLinkNode[i].Name;
                    subVersion.ContentSubVersion.LastUpdateTime = SubItemVersionLinkNode[i].LastUpdateTime;
                    subVersion.ContentSubVersion.ParentID = SubItemVersionLinkNode[i].ContentID;
                    subVersion.ContentSubVersion.ID = SubItemVersionLinkNode[i].ContentVersionID;
                    v.ContentVersions.Add((int)subVersion.ContentSubVersion.Id, subVersion);
                }

                //to see later if there was change in the files
                v.BeforeUpdateFiles.Clear();
                foreach (int keyFile in v.Files.Keys)
                {
                    v.BeforeUpdateFiles.Add(keyFile, v.Files[keyFile]);
                }

                //add or update files
                v.Files.Clear();

                foreach (CMItemFileNode fileNode in SubItemNode)
                    AddFilesToContentVersionFiles(v, String.Empty, fileNode, initCmVersion);

                ProgressBarDataProvider = new CMProgressBarViewModel(); //init progress bar

                if (CMVersion.Id == -1 && ActionName == "Add")  //Add new version
                {

                    v.id_Content = CMParentVersionModel.ID;
                    v.ChildNumber = 0;
                    v.Path.Name = CMParentVersionModel.Name + v.ChildNumber;

                    string displayMessage;

                    if (CMVersionBLL.CheckVersionName(ref v).Equals("Adding existing version"))
                    {
                        String errorId = "Adding existing version";
                        SB.Append("SELECT ED_Description FROM ErrorDescription where ED_ID='" + errorId + "';");
                        MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                        displayMessage = (Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString();
                        parameters.Add(v.Name);
                        displayMessage = CMContentManagementViewModel.UpdateMessageStringParameters(displayMessage, parameters);
                        object[] ArgMessageParam = { parameters[0] };
                        CMContentManagementViewModel.ShowErrorAndInfoMessage(errorId, ArgMessageParam);
                    }
                    else
                    {
                        try
                        {
                            (new CMContentVersionLinkConfirmer()).ConfirmerContentVersion((v));
                            CMFileSystemUpdaterBLL.AddContentVersionFilesOnFs(v, ProgressBarDataProvider, imp);
                            CMVersionBLL.AddNewVersion(ref v);
                            CMVersionBLL.AddContentVersionFiles(ref v);
                            ////CR3692
                            //if (v.ContentVersions != null)
                            //{
                            //    foreach (int keyLinkedVersion in v.ContentVersions.Keys)
                            //    {
                            //        if (v.BeforeUpdateLinkedVersions == null || !v.BeforeUpdateLinkedVersions.ContainsKey(keyLinkedVersion))
                            //        {
                            //            int linkedVersionId = v.ContentVersions[keyLinkedVersion].ContentSubVersion.ID;
                            //            CMVersionModel lv = CMContentManagementViewModel.versions[linkedVersionId];
                            //            lv.ID = (int)lv.Id;
                            //            Domain.ErrorHandling Status = CMVersionBLL.CheckLinkedVersionsInPE(v, lv);
                            //            if (Status.messsageId != string.Empty)
                            //            {
                            //                object[] ArgMessageParam = Status.messageParams;
                            //                CMContentManagementViewModel.ShowErrorAndInfoMessage(Status.messsageId, ArgMessageParam);
                            //                return;
                            //            }
                            //        }
                            //    }
                            //}
                            CMVersionBLL.AddContentVersionVersionLink(ref v);
                            AddTreeNode();
                        }
                        catch (TraceException te)
                        {
                            String logMessage = te.Message + "\n" + "Source: " + te.Source + "\n" + te.StackTrace;
                            Domain.SaveGeneralErrorLog(logMessage);
                            imp.Dispose();
                            SB.Clear();
                            SB.Append("SELECT ED_Description FROM ErrorDescription where ED_ID='" + te.ApplicationErrorID + "';");
                            MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                            displayMessage = (Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString();
                            parameters.Clear();
                            if (te.Parameters != null)
                            {
                                parameters.Add(te.Parameters[0]);
                            }
                            object[] ArgMessageParam = null;
                            if (te.ApplicationErrorID == "Vresion Link loop" || te.ApplicationErrorID == "Create directory")
                            {
                                if (te.Parameters != null)
                                {
                                    parameters.Add(te.Parameters[1]);
                                    object[] tempArgMessageParam = { parameters[0], parameters[1] };
                                    ArgMessageParam = tempArgMessageParam;
                                }
                                //parameters.Add(te.Parameters[1]);
                                //object[] tempArgMessageParam = { parameters[0], parameters[1] };
                                //ArgMessageParam = tempArgMessageParam;
                            }
                            else if (te.ApplicationErrorID == "Copy file")
                            {
                                parameters.Clear();
                                if (te.Parameters != null)
                                {
                                    displayMessage = te.Parameters[2];
                                }
                                //displayMessage = te.Parameters[2];
                                StatusBarParameters = CMContentManagementViewModel.SetMessageDescriptionParam(displayMessage, "White", "Red");
                                MessageMediator.NotifyColleagues("StatusBarParameters", StatusBarParameters);//invalid type of message
                                return;
                            }

                            displayMessage = CMContentManagementViewModel.UpdateMessageStringParameters(displayMessage, parameters);
                            CMContentManagementViewModel.ShowErrorAndInfoMessage(te.ApplicationErrorID, ArgMessageParam);
                            MessageMediator.NotifyColleagues(WorkspaceId + "RefreshTree", this); //Will be returned to the MainWindow signed for this message
                        }
                    }
                }
                else //Update version
                {
                    string displayMessage;

                    if (CMVersionBLL.CheckExistingVersionName(ref v).Equals("Update version name to existing"))
                    {
                        String errorId = "Update version name to existing";
                        SB.Append("SELECT ED_Description FROM ErrorDescription where ED_ID='" + errorId + "';");
                        MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                        displayMessage = (Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString();
                        parameters.Add(v.Name);
                        displayMessage = CMContentManagementViewModel.UpdateMessageStringParameters(displayMessage, parameters);
                        object[] ArgMessageParam = { parameters[0] };
                        CMContentManagementViewModel.ShowErrorAndInfoMessage(errorId, ArgMessageParam);
                    }
                    else
                    {
                        try
                        {
                            bool updateFs;
                            (new CMContentVersionLinkConfirmer()).ConfirmerContentVersion((v));
                            if (CMVersionBLL.CompareUpdateTime(v.LastUpdateTime, v.Id) == false)
                            {
                                throw new TraceException("Version changed", true, new List<string>() { v.Name }, "Content manager");
                            }
                            CMVersionBLL.SelectContentVersionLinked(v);
                            CMVersionBLL.UpdateContentVersionFilesOnFs(v, initCmVersion, out updateFs, ProgressBarDataProvider, imp);

                            ////CR3692
                            //if (v.ContentVersions != null)
                            //{
                            //    foreach (int keyLinkedVersion in v.ContentVersions.Keys)
                            //    {
                            //        if (v.BeforeUpdateLinkedVersions == null || !v.BeforeUpdateLinkedVersions.ContainsKey(keyLinkedVersion))
                            //        {
                            //            int linkedVersionId = v.ContentVersions[keyLinkedVersion].ContentSubVersion.ID;
                            //            CMVersionModel lv = CMContentManagementViewModel.versions[linkedVersionId];
                            //            lv.ID = (int)lv.Id;
                            //            Domain.ErrorHandling Status = CMVersionBLL.CheckLinkedVersionsInPE(v, lv);
                            //            if (Status.messsageId != string.Empty)
                            //            {
                            //                object[] ArgMessageParam = Status.messageParams;
                            //                CMContentManagementViewModel.ShowErrorAndInfoMessage(Status.messsageId, ArgMessageParam);
                            //                return;
                            //            }
                            //        }
                            //    }
                            //}
                            CMVersionBLL.UpdateExistingVersion(ref v);

                            if (updateFs) //if we need to update the files
                            {
                                CMVersionBLL.UpdateContentVersionFiles(ref v);
                            }
                            CMVersionBLL.UpdateContentVersionVersionLinks(ref initCmVersion, ref v);
                            CMVersionBLL.PostUpdateContentVersion(ref v, ref initCmVersion, imp);

                            // Refresh the relevant TreeView node
                            StatusBarParameters.Clear();
                            MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                            Collection<object> MessageParameters = new Collection<object>();

                            Node.Name = v.Name;
                            ((CMVersionModel)(Node.TreeNode)).LockWithDescription = v.LockWithDescription;
                            ((CMVersionModel)(Node.TreeNode)).LastUpdateTime = v.LastUpdateTime;
                            ((CMVersionModel)(Node.TreeNode)).id_ContentVersionStatus = v.id_ContentVersionStatus;
                            ((CMVersionModel)(Node.TreeNode)).Files = v.Files;
                            ((CMVersionModel)(Node.TreeNode)).ContentVersions = v.ContentVersions;

                            // Update permission
                            List<CMTreeNode> treeNodesList = CMVersionBLL.GetChangedLinkedVersion(v);
                            MessageParameters.Clear();
                            MessageParameters.Add(treeNodesList);
                            MessageMediator.NotifyColleagues(this.WorkspaceId + "UpdateVersionsPermissions", MessageParameters); //Send message 
                            CMContentManagementViewModel.UpdatePermissionUpdateNode(Node);
                            CMContentManagementViewModel.UpdatePermissionTreeNodeList(treeNodesList);

                            MessageParameters.Clear();
                            MessageParameters.Add(Node);
                            MessageMediator.NotifyColleagues(this.WorkspaceId + "UpdateNode", MessageParameters); //Send message 
                            MessageMediator.NotifyColleagues(this.WorkspaceId + "ShowAndUpdateVersionDetails", Node); //Will be returned to the CM Main signed for this message

                            //StatusBarParameters.Clear();
                            StatusBarParameters.Add("Data Saved Successfully"); //Message
                            StatusBarParameters.Add("White"); //Foreground
                            StatusBarParameters.Add("Green"); //Background
                            MessageMediator.NotifyColleagues("StatusBarParameters", StatusBarParameters); //Send message to the MainViewModel
                        }
                        catch (TraceException te)
                        {
                            String logMessage = te.Message + "\n" + "Source: " + te.Source + "\n" + te.StackTrace;
                            Domain.SaveGeneralErrorLog(logMessage);
                            imp.Dispose();
                            SB.Clear();
                            SB.Append("SELECT ED_Description FROM ErrorDescription where ED_ID='" + te.ApplicationErrorID + "';");
                            MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                            displayMessage = (Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString();
                            parameters.Clear();
                            if (te.Parameters != null)
                            {
                                parameters.Add(te.Parameters[0]);
                            }

                            object[] ArgMessageParam = null;
                            if (te.ApplicationErrorID == "Version changed")
                            {
                                parameters.Add(v.LastUpdateUser);
                                object[] tempArgMessageParam = { parameters[0], parameters[1] };
                                ArgMessageParam = tempArgMessageParam;
                            }
                            else if (te.ApplicationErrorID == "Version deleted")
                            {
                                object[] tempArgMessageParam = { parameters[0] };
                                ArgMessageParam = tempArgMessageParam;
                            }
                            else if (te.ApplicationErrorID == "Vresion Link loop" || te.ApplicationErrorID == "Create directory")
                            {
                                if (te.Parameters != null)
                                {
                                    parameters.Add(te.Parameters[1]);
                                }

                                object[] tempArgMessageParam = { parameters[0], parameters[1] };
                                ArgMessageParam = tempArgMessageParam;
                            }
                            else if (te.ApplicationErrorID == "Copy file")
                            {
                                parameters.Clear();
                                displayMessage = te.Parameters[2];
                                StatusBarParameters = CMContentManagementViewModel.SetMessageDescriptionParam(displayMessage, "White", "Red");
                                MessageMediator.NotifyColleagues("StatusBarParameters", StatusBarParameters);//invalid type of message
                                return;
                            }

                            displayMessage = CMContentManagementViewModel.UpdateMessageStringParameters(displayMessage, parameters);
                            CMContentManagementViewModel.ShowErrorAndInfoMessage(te.ApplicationErrorID, ArgMessageParam);
                            MessageMediator.NotifyColleagues(WorkspaceId + "RefreshTree", this); //Will be returned to the MainWindow signed for this message
                            MessageMediator.NotifyColleagues(WorkspaceId + "CloseDetailsView", this); //Will be returned to the MainWindow signed for this message, to remove the node from the TreeView
                        }
                    }
                }
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                CMContentManagementViewModel.ShowErrorAndInfoMessage("System error", new object[] { 0 });
            }
        }

        #endregion

        #region  Execute File Command

        private RelayCommand<ObservableCollection<CMItemFileNode>> _ExecuteFileCommand;
        public RelayCommand<ObservableCollection<CMItemFileNode>> ExecuteFileCommand
        {
            get
            {
                if (_ExecuteFileCommand == null)
                {
                    _ExecuteFileCommand = new RelayCommand<ObservableCollection<CMItemFileNode>>(ExecuteExecuteFileCommand, CanExecuteFileCommand);
                }
                return _ExecuteFileCommand;
            }
        }

        private bool CanExecuteFileCommand(ObservableCollection<CMItemFileNode> subItemNodes)
        {
            if(initCmVersion.LockWithDescription.Length > 0 && Domain.User != CMVersion.LastUpdateUser) //if there are lock notes and the last update was by a different user
                return false;

            return Node.IsUpdate && ExistSelectedItemFiles(SubItemNode) && (initCmVersion.id_ContentVersionStatus == "Edit"); //if we are in update mode and there are files and version status is Edit
        }

        private void ExecuteExecuteFileCommand(ObservableCollection<CMItemFileNode> subItemNodes)
        {
            string fileExtension;
            string displayMessage;
            var SB = new StringBuilder(string.Empty);
            String errorId;

            if (subItemNodes == null)
                subItemNodes = SubItemNode;

            List<CMItemFileNode> itemForExecute = new List<CMItemFileNode>();

            foreach (CMItemFileNode fileNode in subItemNodes)
            {
                if (fileNode.IsSelected)
                    itemForExecute.Add(fileNode);
            }

            foreach (CMItemFileNode fileNode in itemForExecute)
            {
                try
                {
                    string FileExecutePath;
                    MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation

                    if (fileNode.ExecutePath == null) //if we want to execute file before we saved
                    {
                        fileExtension = System.IO.Path.GetExtension(fileNode.Path);
                        FileExecutePath = fileNode.Path;
                    }
                    else  //file already exists in the system
                    {
                        fileExtension = System.IO.Path.GetExtension(fileNode.ExecutePath);
                        FileExecutePath = fileNode.ExecutePath;
                    }
                    switch (fileExtension)
                    {
                        //don't execute the file
                        case ".bin":
                        case ".exe":
                        case ".dll":
                        case ".pdb":
                        case null:

                            errorId = "UnableToViewFile";
                            CMContentManagementViewModel.ShowErrorAndInfoMessage(errorId, null);
                            break;

                        //execute in edit mode
                        case ".bat":
                            ProcessStartInfo pStartInfo = new ProcessStartInfo("notepad.exe", fileNode.ExecutePath);
                            Process execute = new Process();
                            execute.StartInfo = pStartInfo;
                            execute.Start();
                            break;

                        //execute the file
                        default:
                            System.Diagnostics.Process.Start(FileExecutePath);
                            break;
                    }
                }
                catch (Exception e)
                {
                    String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                    Domain.SaveGeneralErrorLog(logMessage);
                }
                
            }
               
            foreach (CMItemFileNode fileNode in subItemNodes)
            {
                if (fileNode.SubItemNode.Count > 0)
                    ExecuteExecuteFileCommand(fileNode.SubItemNode);
            }
        }

        #endregion

        #region  Delete Selected Versions Command

        private RelayCommand _DeleteSelectedVersionsCommand;
        public RelayCommand DeleteSelectedVersionsCommand
        {
            get
            {
                if (_DeleteSelectedVersionsCommand == null)
                {
                    _DeleteSelectedVersionsCommand = new RelayCommand(ExecuteDeleteSelectedVersionsCommand, CanExecuteDeleteSelectedVersionsCommand);
                }
                return _DeleteSelectedVersionsCommand;
            }
        }

        private bool CanExecuteDeleteSelectedVersionsCommand()
        {
            if (!Node.IsUpdate)
                return false;

            if (initCmVersion.id_ContentVersionStatus == "Ac" || CMVersion.id_ContentVersionStatus == "Ret")
                return false;

            return true;
        }

        private void ExecuteDeleteSelectedVersionsCommand()
        {
            List<CMItemVersionLink> itemForDelete = new List<CMItemVersionLink>();

            //add to list the selected linked version that we want to delete
            foreach (CMItemVersionLink linkNode in SubItemVersionLinkNode) 
            {
                if (linkNode.IsSelected)
                {
                    itemForDelete.Add(linkNode);
                }
            }

            //remove the linked versions from our collection
            foreach (CMItemVersionLink linkNode in itemForDelete)
            {
                SubItemVersionLinkNode.Remove(linkNode);
            }

            MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
        }

        #endregion

        #region  Delete All Versions Command

        private RelayCommand _DeleteAllVersionsCommand;
        public RelayCommand DeleteAllVersionsCommand
        {
            get
            {
                if (_DeleteAllVersionsCommand == null)
                {
                    _DeleteAllVersionsCommand = new RelayCommand(ExecuteDeleteAllVersionsCommand, CanExecuteDeleteAllVersionsCommand);
                }
                return _DeleteAllVersionsCommand;
            }
        }

        private bool CanExecuteDeleteAllVersionsCommand()
        {
            if (initCmVersion.id_ContentVersionStatus == "Ac" || CMVersion.id_ContentVersionStatus == "Ret")
                return false;

            if (ActionName == "Add" || Node.IsUpdate && SubItemVersionLinkNode.Count > 0)  //if we are in update mode or add new version and there are linkes versions 
                return true;
            return false;
        }

        private void ExecuteDeleteAllVersionsCommand()
        {
            SubItemVersionLinkNode.Clear();
            MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
        }

        #endregion

        #region  Delete Selected Files Command

        private RelayCommand<ObservableCollection<CMItemFileNode>>_DeleteSelectedFilesCommand;
        public RelayCommand<ObservableCollection<CMItemFileNode>> DeleteSelectedFilesCommand
        {
            get
            {
                if (_DeleteSelectedFilesCommand == null)
                {
                    _DeleteSelectedFilesCommand = new RelayCommand<ObservableCollection<CMItemFileNode>>(ExecuteDeleteSelectedFilesCommand, CanExecuteDeleteSelectedFilesCommand);
                }
                return _DeleteSelectedFilesCommand;
            }
        }


        private bool ExistSelectedItemFiles(IEnumerable<CMItemFileNode> subItemNodes)
        {
            foreach (CMItemFileNode fileNode in subItemNodes)
                if (fileNode.IsSelected || ExistSelectedItemFiles(fileNode.SubItemNode))
                    return true;

            return false;
        }

        private bool CanExecuteDeleteSelectedFilesCommand(ObservableCollection<CMItemFileNode> subItemNodes)
        {
            if (initCmVersion.LockWithDescription.Length > 0 && Domain.User != CMVersion.LastUpdateUser) //if there are lock notes and the last update was by a different user
                return false;

            return Node.IsUpdate && ExistSelectedItemFiles(SubItemNode) && (initCmVersion.id_ContentVersionStatus == "Edit"); //if we are in update mode and there are files and version status is Edit
        }

        private void ExecuteDeleteSelectedFilesCommand(ObservableCollection<CMItemFileNode> subItemNodes)
        {
            if (subItemNodes == null)
                subItemNodes = SubItemNode;

            List<CMItemFileNode> itemForDelete = new List<CMItemFileNode>();

            foreach (CMItemFileNode fileNode in subItemNodes)
            {
                if (fileNode.IsSelected)
                    itemForDelete.Add(fileNode);
            }

            foreach (CMItemFileNode fileNode in itemForDelete)
                subItemNodes.Remove(fileNode);

            foreach (CMItemFileNode fileNode in subItemNodes)
            {
                if(fileNode.SubItemNode.Count > 0)
                  ExecuteDeleteSelectedFilesCommand(fileNode.SubItemNode);
            }
            MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
        }

        #endregion

        #region  Delete All Files Command

        private RelayCommand _DeleteAllFilesCommand;
        public RelayCommand DeleteAllFilesCommand
        {
            get
            {
                if (_DeleteAllFilesCommand == null)
                {
                    _DeleteAllFilesCommand = new RelayCommand(ExecuteDeleteAllFilesCommand, CanExecuteDeleteAllFilesCommand);
                }
                return _DeleteAllFilesCommand;
            }
        }

        private bool CanExecuteDeleteAllFilesCommand()
        {
            if (initCmVersion.LockWithDescription.Length > 0 && Domain.User != CMVersion.LastUpdateUser) //if there are lock notes and the last update was by a different user
                return false;

            return Node.IsUpdate && SubItemNode.Count > 0 && (initCmVersion.id_ContentVersionStatus == "Edit"); //if we are in update mode and there are files and version status is Edit
        }

        private void ExecuteDeleteAllFilesCommand()
        {
            SubItemNode.Clear();
            MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
        }

        #endregion     

        #region Open Linked Version In Another Workspace

        private RelayCommand _OpenLinkedVersionInAnotherWorkspaceCommand;
        public ICommand OpenLinkedVersionInAnotherWorkspaceCommand
        {
            get
            {
                if (_OpenLinkedVersionInAnotherWorkspaceCommand == null)
                {
                    _OpenLinkedVersionInAnotherWorkspaceCommand = new RelayCommand(ExecuteOpenLinkedVersionInAnotherWorkspaceCommand, CanExecuteOpenLinkedVersionInAnotherWorkspaceCommand);
                }
                return _OpenLinkedVersionInAnotherWorkspaceCommand;
            }
        }

        private bool CanExecuteOpenLinkedVersionInAnotherWorkspaceCommand()
        {
            return true;
        }

        private void ExecuteOpenLinkedVersionInAnotherWorkspaceCommand()
        {
            MessageMediator.NotifyColleagues("RequestGotoCm", SelectedLinkedVersion); //Send message to the MainViewModel to clear Statusbar from any previous operation
        }

        #endregion

        #endregion

        #region Methods

        #region Update Version Linked Versions

        private void UpdateVersionLinkedVersions(CMVersionModel v)
        {
            versions = new Dictionary<int, CMVersionModel>();

            CMVersionModel contentVersionTemp;
            DataTable contentVersionsDataTable = CMTreeNodeBLL.GetContentVersionsDataTableByContentID((int)v.id_Content);

            foreach (DataRow row in contentVersionsDataTable.Rows)
            {
                contentVersionTemp = CMVersionBLL.GetVersionRow((int)row["CV_ID"]);
                contentVersionTemp.ParentID = ((int)row["CV_id_Content"]);

                if (versions != null)
                    versions.Add((int)contentVersionTemp.Id, contentVersionTemp);

            }

            foreach (KeyValuePair<int, CMVersionModel> contentVersion in versions)
            {
                int contentVersionSubVersion;
                CMContentVersionSubVersionModel subVersion;
                v.ContentVersions.Clear();

                DataTable contentVersionsSubVersionsDataTable = CMTreeNodeBLL.GetContentVersionSubVersionsByContentVersionID(v.Id);

                foreach (DataRow row in contentVersionsSubVersionsDataTable.Rows)
                {
                    subVersion = new CMContentVersionSubVersionModel();

                    contentVersionSubVersion = (int)row["CVVL_id_ContentVersion_Link"];
                    //LastUpdateUtil.UpdateObjectByDataReader(subVersion, row);
                    subVersion.Order = (int)row["CVVL_ChildNumber"];

                    subVersion.ContentSubVersion = CMVersionBLL.GetVersionRow(contentVersionSubVersion);
                    subVersion.Content = CMContentBLL.GetContentRow(subVersion.ContentSubVersion.id_Content);
                    subVersion.Content.ID = (int)subVersion.ContentSubVersion.id_Content;
                    subVersion.ContentSubVersion.ID = (int)subVersion.ContentSubVersion.Id;
                    subVersion.ContentSubVersion.ParentID = (int)subVersion.ContentSubVersion.id_Content;
                    subVersion.ContentSubVersion.LastUpdateTime = subVersion.ContentSubVersion.LastUpdateTime;
                    v.ContentVersions.Add(contentVersionSubVersion, subVersion);
                }
            }
        }
        #endregion    
        
        #region  Add Tree Node

        private void AddTreeNode()
        {
            //Work variables
            Collection<object> MessageParameters = new Collection<object>();
            Collection<string> StatusBarParameters = new Collection<string>();
            List<String> parameters = new List<string>();
            CMTreeViewNodeViewModelBase CurrNode = null;
            CMVersionModel VN = new CMVersionModel();

            VN.ID = (int)CMVersion.Id;
            VN.Name = CMVersion.Name;
            VN.Description = CMVersion.Description;
            VN.LastUpdateTime = CMVersion.LastUpdateTime;
            VN.CreationDate = CMVersion.CreationDate;
            VN.ECR = CMVersion.ECR;
            VN.DocumentID = CMVersion.DocumentID;
            VN.PDMDocumentVersion = CMVersion.PDMDocumentVersion;
            VN.ConfigurationManagementLink = CMVersion.ConfigurationManagementLink;
            VN.RunningString = CMVersion.CommandLine;
            VN.Path.Name = CMVersion.Path.Name;
            VN.Status = CMVersion.Status;
            VN.Status.ID = CMVersion.id_ContentVersionStatus;
            VN.id_ContentVersionStatus = CMVersion.id_ContentVersionStatus;
            VN.LockWithDescription = CMVersion.LockWithDescription;
            VN.Files = CMVersion.Files;
            VN.ContentVersions = CMVersion.ContentVersions;

            CurrNode = new CMTreeViewVersionNodeViewModel(this.WorkspaceId, VN, CMParentVersionModel);
            CurrNode.TreeNode.ParentID = (int)CMVersion.id_Content;

            switch (CMVersion.id_ContentVersionStatus)
            {
                case "Ac":
                    CurrNode.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png", "ActiveContentVersion"), UriKind.RelativeOrAbsolute));
                    break;

                case "Edit":
                    CurrNode.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png", "EditContentVersion"), UriKind.RelativeOrAbsolute));
                    break;

                case "Ret":
                    CurrNode.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png", "RetContentVersion"), UriKind.RelativeOrAbsolute));
                    break;

                default:
                    CurrNode.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png", "ContentVersion"), UriKind.RelativeOrAbsolute));
                    break;
            }
            //CR3483 - insert latest version first
            CMParentVersionModel.Children.Insert(0,CurrNode);
            //Position the view to the newly created node and cleanup
            CMParentVersionModel.IsExpanded = true;
            CurrNode.IsSelected = true;

            CMContentManagementViewModel.versions.Add(VN.ID, VN); //add new version to static versions Dictionary

            // Update permission
            List<CMTreeNode> treeNodesList = CMVersionBLL.GetChangedLinkedVersion(VN);
            MessageParameters.Clear();
            MessageParameters.Add(treeNodesList);
            MessageMediator.NotifyColleagues(this.WorkspaceId + "UpdateVersionsPermissions", MessageParameters); //Send message 
            CMContentManagementViewModel.UpdatePermissionAddNode(CMParentVersionModel, CurrNode);
            CMContentManagementViewModel.UpdatePermissionTreeNodeList(treeNodesList);

            MessageMediator.NotifyColleagues(this.WorkspaceId + "ShowAndUpdateVersionDetails", CurrNode); //Will be returned to the CM Main signed for this message
            StatusBarParameters.Clear();
            StatusBarParameters.Add("Data Saved Successfully"); //Message
            StatusBarParameters.Add("White"); //Foreground
            StatusBarParameters.Add("Green"); //Background
            MessageMediator.NotifyColleagues("StatusBarParameters", StatusBarParameters); //Send message to the MainViewModel 

        }

        #endregion

        #region Init Add Parameters

        private void InitAddParameters(long contentVersionID, long contentID)
        {
            //VetsionID = contentVersionID;
            //ContentVetsionID = contentID;
            //ECR = String.Empty;
            //DocumentID = String.Empty;
            //Path = String.Empty;
            //Name = String.Empty;
            //Editor = String.Empty;
            //EditorName = String.Empty;
            //IsVisibleEditorName = false;
            //Description = String.Empty;
            //RunningString = String.Empty;

            //Status = Locator.VersionDataProvider.ContentStatusList[0];

            //if (Locator.VersionDataProvider.ContentStatusList.Count > 1) // Bug of WPF
            //{
            //    Status = Locator.VersionDataProvider.ContentStatusList[1];
            //    Status = Locator.VersionDataProvider.ContentStatusList[0];
            //}

            //SubItemNode.Clear();
            //SubItemVersionLinkNode.Clear();
        }

        #endregion

        #region Init Update Parameters

        private void InitUpdateParameters(long contentVersionID, long contentID)
        {
            SubItemNode.Clear();
            UpdateFile(CMVersion.Files);
            InitUpdateParameterSubVersions(CMVersion.ContentVersions);
        }

        #endregion

        #region Init Update Parameter Sub Versions

        private void InitUpdateParameterSubVersions(Dictionary<int, CMContentVersionSubVersionModel> subVersions)
        {
            SubItemVersionLinkNode.Clear();

            foreach (KeyValuePair<int, CMContentVersionSubVersionModel> subVersion in subVersions)
            {
                CMItemVersionLink versionLink = new CMItemVersionLink
                {
                    ContentVersionID = subVersion.Value.ContentSubVersion.ID,
                    ContentID = subVersion.Value.Content.ID,
                    Icon = GetContentVersionIcon(subVersion.Value.ContentSubVersion),
                    ContentName = subVersion.Value.Content.Name,
                    Name = subVersion.Value.ContentSubVersion.Name,
                    Status = subVersion.Value.ContentSubVersion.id_ContentVersionStatus,
                    LastUpdateTime = subVersion.Value.ContentSubVersion.LastUpdateTime
                };
                SubItemVersionLinkNode.Add(versionLink);
            }
        }

        #endregion

        #region Get Content Version Icon

        private ImageSource GetContentVersionIcon(CMVersionModel node)
        {
            ImageSource returnIcon = null;

            switch (node.id_ContentVersionStatus)
            {
                case "Ac":
                    returnIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png", "ActiveContentVersion"), UriKind.RelativeOrAbsolute));
                    break;

                case "Edit":
                    returnIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png", "EditContentVersion"), UriKind.RelativeOrAbsolute));
                    break;

                case "Ret":
                    returnIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png", "RetContentVersion"), UriKind.RelativeOrAbsolute));
                    break;
            }

            return returnIcon;
        }

        #endregion

        #region Is Configuration Management Link Required

        private bool IsConfigurationManagementLinkRequired()
        {
            try
            {
                if (CMVersion.id_ContentVersionStatus == "Ac" && ConfigurationManagementLink == "")
                {
                    CMContentManagementViewModel.ShowErrorAndInfoMessage("ConfigurationManagementLink field is required", null);
                    ConfigurationManagementLinkErr = false;
                    return false;
                }
                else
                {
                    if (ConfigurationManagementLinkErr == false)
                    {
                        MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                        ConfigurationManagementLinkErr = true;
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                CMContentManagementViewModel.ShowErrorAndInfoMessage("ConfigurationManagementLink field is required", null);
                return false;
            }
        }

        #endregion

        #endregion

        #region  IDropTarget Members & Other Drop Activities

        public void DragOver(Infra.DragDrop.IDropInfo DropInfo)
        {
            try
            {
                MessageMediator.NotifyColleagues("StatusBarParameters", null); //clear status bar from previous messages
                if (!UpdateModeData)
                {
                    DropInfo.Effects = DragDropEffects.None;
                    return;
                }

                string SourceItemType = DropInfo.Data.GetType().ToString();

                if (SourceItemType.Contains("CMTreeViewVersionNodeViewModel"))
                {
                    CMTreeViewVersionNodeViewModel SourceItem = DropInfo.Data as CMTreeViewVersionNodeViewModel;

                    if (CMVersion.id_ContentVersionStatus != "Edit") //if this version is in edit status we can drop in add and update mode
                    {
                        if ((updateDragDropMode == true) || (CMVersion.id_ContentVersionStatus == "Ret") || (updateDragDropMode == false && ((CMVersionModel)(SourceItem.TreeNode)).id_ContentVersionStatus == "Edit"))
                        {
                            DropInfo.Effects = DragDropEffects.None;
                            return;
                        }
                    }

                    if (Domain.User != CMVersion.LastUpdateUser && CMVersion.LastUpdateUser != "" && CMVersion.LockWithDescription != "")  //if version created by another user and lock notes field isnt empty do not alow drop
                    {
                        DropInfo.Effects = DragDropEffects.None;
                        return;
                    }

                    if (SourceItem != null && AllowDropItemVersionLink(SourceItem, CMVersion.id_Content, SubItemVersionLinkNode))
                    {
                        DropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                        DropInfo.Effects = DragDropEffects.Move;
                    }
                }
                else if (SourceItemType.Contains("System.Windows.DataObject"))
                {

                    //check and do not allow drop file on file
                    System.Windows.DataObject SourceItem = (System.Windows.DataObject)DropInfo.Data;
                    string[] FileNames = (string[])SourceItem.GetData(DataFormats.FileDrop, true);

                    if (FileNames == null)
                        return;

                    foreach (string FileName in FileNames)
                    {
                        FileInfo FI = new FileInfo(FileName); //the droped file
                        CMItemFileNode whereToDrop = (CMItemFileNode)DropInfo.TargetItem; //the target file                   
                    }

                    DropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                    DropInfo.Effects = DragDropEffects.Link;
                }
                else if (SourceItemType.Contains("CMItemFileNode"))  //Drag inside the TreeView
                {
                    CMItemFileNode sourceItemFileNode = (CMItemFileNode)DropInfo.Data;

                    if (CanAddItemFileNode(null, sourceItemFileNode))
                    {
                        DropInfo.Effects = DragDropEffects.Move;
                        return;
                    }
                }
                else
                {
                    DropInfo.Effects = DragDropEffects.None;
                    return;
                }
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                MsgBoxService.ShowError("Error:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public void Drop(Infra.DragDrop.IDropInfo DropInfo)
        {
            try
            {                          
                List<String> parameters = new List<string>();
                var SB = new StringBuilder(string.Empty);
                //Identify dropped Entity Type
                string SourceItemType = DropInfo.Data.GetType().ToString();
                string DropCollectionType = DropInfo.TargetCollection.GetType().ToString();
                string displayMessage;

                #region Add Files
                if (SourceItemType.Contains("System.Windows.DataObject"))
                {
                    CMItemFileNode fileToRemove = null;  //the file that needs to be replaced if necessary
                    string[] itemToReplace = new string[1]; //the item that will replace the removed file each iteration
                    CMItemFileNode whereToDrop = (CMItemFileNode)DropInfo.TargetItem; //if it's not null the drop target is a folder (sirectory)
                    ObservableCollection<CMItemFileNode> TempExistingSubItemNodes = new ObservableCollection<CMItemFileNode>();

                    if (whereToDrop != null && whereToDrop.Type.ToString() != "File") //build temp collection not from directory
                    {
                        foreach (CMItemFileNode it in whereToDrop.SubItemNode)
                        {
                            TempExistingSubItemNodes.Add(it);
                        }
                    }
                    else ////build temp collection from existing directory
                    {
                        foreach (CMItemFileNode it in SubItemNode)
                        {
                            TempExistingSubItemNodes.Add(it);
                        }
                    }

                    System.Windows.DataObject SourceItem = (System.Windows.DataObject)DropInfo.Data;
                    string[] FileNames = (string[])SourceItem.GetData(DataFormats.FileDrop, true);
                    //string FileName = FileNames[0];
                    foreach (string FileName in FileNames)
                    {
                        itemToReplace[0] = FileName;
                        FileInfo FI = new FileInfo(FileName);
                        parameters.Clear();
                        String errorId = "FileExists";
                        SB.Append("SELECT ED_Description FROM ErrorDescription where ED_ID='" + errorId + "';");
                        displayMessage = (Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString();
                        parameters.Add(FI.Name);
                        displayMessage = CMContentManagementViewModel.UpdateMessageStringParameters(displayMessage, parameters);

                        if (whereToDrop != null && whereToDrop.Type.ToString() != "File")  //if drop into directory
                        {
                            if (!CMItemFileNode.CanAddItemFilesFs(ItemFileNodeType.Folder, TempExistingSubItemNodes, itemToReplace, ref fileToRemove))
                            {
                                if (MsgBoxService.ShowYesNo(displayMessage, DialogIcons.Question) == DialogResults.Yes)
                                {
                                    whereToDrop.SubItemNode.Remove(fileToRemove); //remove the file we want to replace
                                }
                                else //else do nothing and return
                                {
                                    DropInfo.Effects = DragDropEffects.None;
                                    continue; //check next file
                                }
                            }
                        }
                        else    //if drop not into directory
                        {
                            if (!CMItemFileNode.CanAddItemFilesFs(ItemFileNodeType.Folder, TempExistingSubItemNodes, itemToReplace, ref fileToRemove))
                            {
                                if (MsgBoxService.ShowYesNo(displayMessage, DialogIcons.Question) == DialogResults.Yes)
                                {
                                    SubItemNode.Remove(fileToRemove); 
                                }
                                else //else do nothing and return
                                {
                                    DropInfo.Effects = DragDropEffects.None;
                                    continue; //check next file
                                }
                            }
                        }
                        if (whereToDrop == null || whereToDrop.Type.ToString() == "File")
                        {
                            AddSubItems(null, SubItemNode, itemToReplace, ((DropInfo.KeyStates & DragDropKeyStates.ControlKey) != DragDropKeyStates.ControlKey));
                        }
                        else
                        {
                            AddSubItems(whereToDrop, whereToDrop.SubItemNode, itemToReplace, ((DropInfo.KeyStates & DragDropKeyStates.ControlKey) != DragDropKeyStates.ControlKey));
                        }
                    }
                }
                else if (SourceItemType.Contains("CMItemFileNode"))  //Drop files inside the TreeView
                {
                    CMItemFileNode sourceItemFileNode = (CMItemFileNode)DropInfo.Data;
                    object destinationNode = DropInfo.TargetItem;

                    if (destinationNode != null && sourceItemFileNode.GetType() == typeof(CMItemFileNode) && destinationNode.GetType() == typeof(CMItemFileNode))
                    {
                        CMItemFileNode destinationItemFileNode = (CMItemFileNode)destinationNode;

                        if (CanAddItemFileNode(destinationItemFileNode, sourceItemFileNode))
                        {
                            UpdateParent(sourceItemFileNode, destinationItemFileNode);
                        }
                    }
                    else
                    {
                        if (CanAddItemFileNode(null, sourceItemFileNode))
                        {
                            UpdateParent(sourceItemFileNode, null);
                            return;
                        }
                    }
                }
                #endregion AddFiles

                #region Link Versions
                //If dropping Version, verify we drop on the right container and add to content list
                if (SourceItemType.Contains("CMTreeViewVersionNodeViewModel") && DropCollectionType.Contains("CMItemVersionLink"))
                {
                    CMTreeViewVersionNodeViewModel SourceItem = DropInfo.Data as CMTreeViewVersionNodeViewModel;

                    //CR3682
                    Domain.ErrorHandling Status = CheckVersionLinksInPE(CMVersion, SourceItem);
                    if (Status.messsageId != string.Empty)
                    {
                        object[] ArgMessageParam = Status.messageParams;
                        CMContentManagementViewModel.ShowErrorAndInfoMessage(Status.messsageId, ArgMessageParam);
                        return;
                    }

                    if (AllowDropItemVersionLink(SourceItem, CMVersion.id_Content, SubItemVersionLinkNode))
                    {
                        //contentNamedarg, SourceItem.Name, SourceItem.TreeNode.ID, DateTime.Now.ToString(), "");
                        CMItemVersionLink DragVL = new CMItemVersionLink
                        {
                            ContentVersionID = (int)SourceItem.ID,
                            ContentID = (int)SourceItem.Parent.ID,
                            Icon = SourceItem.Icon,
                            ContentName = SourceItem.Parent.Name,
                            Name = SourceItem.Name,
                            Status = ((CMVersionModel)(SourceItem.TreeNode)).id_ContentVersionStatus,
                            LastUpdateTime = ((CMVersionModel)(SourceItem.TreeNode)).LastUpdateTime
                        };

                        SubItemVersionLinkNode.Add(DragVL);
                    }
                }
                if (SourceItemType.Contains("ContentMgmtModule.CMItemVersionLink"))
                {
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }

                #endregion Link Versions
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                MsgBoxService.ShowError("Error:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        #endregion

        #region Version Files

        #region Init files

        private void UpdateFile(Dictionary<int, CMContentFileModel> files)
        {
            CMItemFileNode fileNode;
            foreach (KeyValuePair<int, CMContentFileModel> file in files)
            {
                fileNode = CreateItemFileNode(file.Value);
                AddFileItemToFolder(file.Value.FileRelativePath, fileNode);
            }
        }

        private CMItemFileNode CreateItemFileNode(CMContentFileModel file)
        {
            CMItemFileNode fileNode = new CMItemFileNode
            {
                ID = file.ID,
                Name = file.FileName,
                Path = file.FileRelativePath,
                ExecutePath = file.FileFullPath,
                FileSize = file.FileSize,//File Size in Bytes   
                ModifiedOn = file.FileLastWriteTime.ToString(),
                Type = ItemFileNodeType.File,
                Status = ItemFileStatus.Exist,
            };

            return fileNode;
        }

        private void AddFileItemToFolder(string filePath, CMItemFileNode fileNode)
        {
            bool existSubFolder;
            filePath = filePath.Trim();

            CMItemFileNode parentNode = null;
            ObservableCollection<CMItemFileNode> parentSubItemNodes = SubItemNode;

            if (filePath == String.Empty)
            {
                fileNode.Parent = null;
                SubItemNode.Add(fileNode);
                return;
            }

            string[] folders = filePath.Split('\\');

            foreach (string folderName in folders)
            {
                existSubFolder = false;
                foreach (CMItemFileNode itemFolder in parentSubItemNodes)
                {
                    if (itemFolder.Name == folderName && itemFolder.Type == ItemFileNodeType.Folder)
                    {
                        parentNode = itemFolder;
                        parentSubItemNodes = itemFolder.SubItemNode;
                        existSubFolder = true;
                    }
                }

                if (!existSubFolder)
                {
                    CMItemFileNode newSubFolder = CreateItemFolderNode(folderName, parentNode);
                    parentNode = newSubFolder;
                    parentSubItemNodes.Add(newSubFolder);
                    parentSubItemNodes = newSubFolder.SubItemNode;
                }
            }
            fileNode.Parent = parentNode;
            parentSubItemNodes.Add(fileNode);
        }

        private CMItemFileNode CreateItemFolderNode(string name, CMItemFileNode parent)
        {
            CMItemFileNode fileNode = new CMItemFileNode
            {
                ID = 0,
                Name = name,
                Parent = parent,
                Type = ItemFileNodeType.Folder,
                Status = ItemFileStatus.Exist,
                SubItemNode = new ObservableCollection<CMItemFileNode>()
            };

            return fileNode;
        }

        #endregion

        #region Add Files To Content Version Files

        private void AddFilesToContentVersionFiles(CMVersionModel contentVersion, String relativePath, CMItemFileNode file, CMVersionModel contentVersionOriginal)
        {
            if (file.Type == ItemFileNodeType.File)
            {
                int newIndex;

                CMContentFileModel newFile = new CMContentFileModel
                {
                    FileName = file.Name,
                    FileRelativePath = relativePath,
                    ID = file.ID
                };

                if (file.ID != 0)
                {
                    newIndex = file.ID;
                    // LastUpdate.UpdateLastUpdate(contentVersionOriginal.Files[file.ID], newFile);
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
                    newFile.FileFullPath = contentVersion.Path.Name + "\\" + file.Name;
                else
                {
                    if (newFile.ID == 0) //New file
                        newFile.FileFullPath = file.Path;
                    else
                        newFile.FileFullPath = contentVersion.Path.Name + "\\" + file.Path + "\\" + file.Name;
                }
                try
                {
                    FileInfo fi = new FileInfo(newFile.FileFullPath);
                    newFile.FileSize = fi.Length.ToString(); //File Size in Bytes
                    newFile.FileLastWriteTime = fi.LastWriteTime;
                }
                catch (Exception e)
                {
                    String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                    Domain.SaveGeneralErrorLog(logMessage);
                }

                contentVersion.Files.Add(newIndex, newFile);
            }
            else
            {
                foreach (CMItemFileNode fileNode in file.SubItemNode)
                    AddFilesToContentVersionFiles(contentVersion, relativePath == String.Empty ? file.Name : relativePath + "\\" + file.Name, fileNode, contentVersionOriginal);
            }
        }

        #endregion

        #region Can Add Item File Node

        public bool CanAddItemFileNode(CMItemFileNode updatedItemFileNode, CMItemFileNode itemFileNode)
        {
            ObservableCollection<CMItemFileNode> parentSubItemNode;

            if (updatedItemFileNode == itemFileNode)
                return false;

            if (updatedItemFileNode == null)
            {
                parentSubItemNode = SubItemNode;
            }
            else
            {
                if (updatedItemFileNode.Type != ItemFileNodeType.Folder)
                    return false;

                parentSubItemNode = updatedItemFileNode.SubItemNode;
            }

            if (itemFileNode != null)
            {
                foreach (CMItemFileNode fileNode in parentSubItemNode)
                {
                    if (fileNode.Name == itemFileNode.Name && fileNode != itemFileNode)
                        return false;
                }

                CMItemFileNode destinationTemp = updatedItemFileNode;

                while (destinationTemp != null)
                {
                    if (itemFileNode == destinationTemp)
                        return false;

                    destinationTemp = destinationTemp.Parent;
                }
            }

            return true;
        }

        #endregion

        #region Update Parent

        public void UpdateParent(CMItemFileNode updatedItemFileNode, CMItemFileNode parent)
        {
            UpdateParentNodeStatusRecursive(updatedItemFileNode, ItemFileStatus.Exist, ItemFileStatus.Updated);

            if (updatedItemFileNode.Parent != null)
            {
                updatedItemFileNode.Parent.SubItemNode.Remove(updatedItemFileNode);
            }
            else
            {
                SubItemNode.Remove(updatedItemFileNode);
            }

            if (parent == null)
            {
                updatedItemFileNode.Parent = null;
                SubItemNode.Add(updatedItemFileNode);
            }
            else
            {
                updatedItemFileNode.Parent = parent;
                parent.SubItemNode.Add(updatedItemFileNode);
            }
        }

        #endregion

        #region Update Parent Node Status Recursive

        private static void UpdateParentNodeStatusRecursive(CMItemFileNode node, ItemFileStatus fromStatus, ItemFileStatus toStatus)
        {
            if (node.Status == fromStatus)
                node.Status = toStatus;

            foreach (CMItemFileNode subNode in node.SubItemNode)
                UpdateParentNodeStatusRecursive(subNode, fromStatus, toStatus);
        }

        #endregion

        #region Add Sub Items

        public void AddSubItems(CMItemFileNode parent, ObservableCollection<CMItemFileNode> parentItemsCollection, string[] items, bool addFolderRecursive)
        {

            if (parent != null && parent.Type != ItemFileNodeType.Folder)
                return;

            foreach (string item in items)
            {
                CMItemFileNode newItem = new CMItemFileNode
                {
                    ID = 0,
                    Path = item,
                    Status = ItemFileStatus.New,
                    SubItemNode = new ObservableCollection<CMItemFileNode>()
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
                    AddSubItems(newItem, newItem.SubItemNode, Directory.GetDirectories(item), true);
                    AddSubItems(newItem, newItem.SubItemNode, Directory.GetFiles(item), true);
                }

                newItem.Parent = parent;
                parentItemsCollection.Add(newItem);
            }
        }

        #endregion

        #endregion

        #region Version Linked Versions

        #region Allow Drop Item Version Link

        private bool AllowDropItemVersionLink(CMTreeViewVersionNodeViewModel sourceItemVersionNode, long parentContentID, ObservableCollection<CMItemVersionLink> existingSubItems)
        {
            List<long> linkedContents = new List<long> { parentContentID };

            // Add existing links to List 
            foreach (CMItemVersionLink itemVersionLink in existingSubItems)
            {
                if (!linkedContents.Contains(itemVersionLink.ContentID))
                    linkedContents.Add(itemVersionLink.ContentID);
                else
                    return false;

                if (!AddSubLinkedContents(linkedContents, CMVersionBLL.GetVersionRow(itemVersionLink.ContentVersionID)))
                    return false;
            }

            //Add new links to List
            if (!linkedContents.Contains(sourceItemVersionNode.Parent.ID))
            {
                linkedContents.Add(sourceItemVersionNode.Parent.ID);
                MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
            }
            else
                return false;

            if (!AddSubLinkedContents(linkedContents, CMVersionBLL.GetVersionRow(sourceItemVersionNode.ID)))
                return false;

            return true;
        }

        private Domain.ErrorHandling CheckVersionLinksInPE(CMVersionModel CMVersion, CMTreeViewVersionNodeViewModel dropVersion)
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            Status = CMVersionBLL.CheckLinkedVersionsInPE(CMVersion, (CMVersionModel)dropVersion.TreeNode);
            return Status;
        }
        #endregion

        #region Add Sub Linked Contents

        private bool AddSubLinkedContents(List<long> linkedContents, CMVersionModel contentVersion)
        {
            //cr = new CMContentsReader();
            //cr.UpdateContentVersionSubVersions(contentVersion, versions, contents);
            UpdateVersionLinkedVersions(contentVersion); // get version linked versions if an update occured  
            foreach (KeyValuePair<int, CMContentVersionSubVersionModel> subVersion in contentVersion.ContentVersions)
            {
                if (!linkedContents.Contains(subVersion.Value.Content.ID))
                {
                    linkedContents.Add(subVersion.Value.Content.ID);
                }
                else
                    return false;

                CMVersionModel cv = CMVersionBLL.GetVersionRow(((CMTreeNode)(subVersion.Value.ContentSubVersion)).ID);
                if (!AddSubLinkedContents(linkedContents, cv))
                    return false;
            }

            return true;
        }

        #endregion

        #endregion

    }

} //end of root namespace