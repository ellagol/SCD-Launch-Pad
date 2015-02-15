using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ATSBusinessLogic;
using ATSBusinessObjects;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using Infra.DragDrop;
using Infra.MVVM;
using NotesModule;
using ResourcesProvider;
 
namespace ExplorerModule
{
    public class ProjectDetailsViewModel : ViewModelBase, IDropTarget
    {

        #region  Data

        protected MessengerService MessageMediator = new MessengerService();
        private IMessageBoxService MsgBoxService = null;

        public int tabIndex { get; set; }

        private bool cannotUpdateActiveContents = true;



        private string _stationCertifiedListResults;
        public string StationCertifiedListResults
        {
            get
            {
                return _stationCertifiedListResults;
            }
            set
            {
                _stationCertifiedListResults = value;
                RaisePropertyChanged("StationCertifiedListResults");
            }
        }
        //public HierarchyModel initHM = new HierarchyModel();

        //public VersionModel initVM = new VersionModel();

        //fix 1869
        public static string DateTimeFormat
        {
            get 
            {
                return Domain.DateTimeFormat;
            }
        }

        public static bool isContentOrCertificateDroped = false;

        private IEnumerable<CertificateModel> _Certificates;

        private IEnumerable<ContentModel> _activeContents;

        //private ObservableCollection<CertificateModel> certificateDisplayList;

        private Guid WorkspaceId;
        //private Boolean _Synchronization;
        //private String _Code;
        private HierarchyModel _Hierarchy;
        //private String _SelectedStep;
        private StringCollection _ProjectSteps;
        //private String _GroupName;

        public static string InitialVersionName = string.Empty;

        Dictionary<string, string> InitialCertificates = new Dictionary<string,string>();
        ObservableCollection<UserCertificateApiModel> InitialUserCertificates = new ObservableCollection<UserCertificateApiModel>();

        public static string LastVersionName = "";
        public static int projectId = -1;
        string defaultVersionName = string.Empty;


        private bool _ShowOverlayContent = false;
        public bool ShowOverlayContent
        {
            get
            {
                return _ShowOverlayContent;
            }
            set
            {
                _ShowOverlayContent = value;
                RaisePropertyChanged("ShowOverlayContent");
            }
        }

        private ViewModelBase _OverlayContentViewModel;
        public ViewModelBase OverlayContentViewModel
        {
            get
            {
                return _OverlayContentViewModel;
            }
            set
            {
                _OverlayContentViewModel = value;
                RaisePropertyChanged("OverlayContentViewModel");
            }
        }

        private bool _CertificatesChanged = true;
        public bool CertificatesChanged
        {
            get
            {
                return _CertificatesChanged;
            }
            set
            {
                _CertificatesChanged = value;
            }
        }

        private Boolean certificateRemove = false; 

        public HierarchyModel Hierarchy
        {
            get
            {
                return _Hierarchy;
            }
            set
            {
                _Hierarchy = value;
            }
        }

        private bool _ShowNotes = true;
        public bool ShowNotes
        {
            get
            {
                return _ShowNotes;
            }
            set
            {
                _ShowNotes = value;
                RaisePropertyChanged("ShowNotes");
            }
        }

        public NotesControlViewModel Notes { get; set; }

        #endregion

        #region Presentation Properties

        public long ID
        {
            get
            {
                if (_Hierarchy != null)
                {
                    return _Hierarchy.Id;
                }
                else
                {
                    return -1;
                }
            }

        }


        public class ProjectInvalidNameAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                if (value == null)
                {
                    return true;
                }
                if (value.ToString().Contains("/") || value.ToString().Contains("/") || value.ToString().Contains("\\")
                                                     || value.ToString().Contains("*") || value.ToString().Contains("?")
                                                     || value.ToString().Contains("\"") || value.ToString().Contains("<")
                                                     || value.ToString().Contains(">") || value.ToString().Contains("|")
                                                      || value.ToString().Contains("\t") || value.ToString().Contains(Environment.NewLine))
                {
                    return false;
                }

                return true;
            }
        }


        [ProjectInvalidName(ErrorMessage = "Project name can't contain any of the following characters: \\ / * ? \" < > |, tab, new line")] 
        [Required(ErrorMessage = "'Name' field is required."), StringLength(200, ErrorMessage = "Maximum length (200 characters) exceeded.")]
        public string Name
        {
            get
            {
                if (_Hierarchy != null)
                {
                    return _Hierarchy.Name;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
            
                if (_Hierarchy != null)
                {
                    CertificatesChanged = true;
                    _Hierarchy.Name = value;
                    RaisePropertyChanged("Name");
                    RaisePropertyChanged("TargetPath");
                    if (VM.DefaultTargetPathInd == true)
                    {
                        Hierarchy.VM.TargetPath = getTargetPath();
                        RaisePropertyChanged("TargetPath");
                    }
                    Hierarchy.VM.IsDirty = true;
                    Hierarchy.IsDirty = true;
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }
            }
        }

        [StringLength(500, ErrorMessage = "Maximum length (500 characters) exceeded.")]
        public string Description
        {
            get
            {
                if (_Hierarchy != null)
                {
                    return _Hierarchy.Description;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                CertificatesChanged = true;
                if (_Hierarchy != null)
                {
                    _Hierarchy.Description = value;
                    RaisePropertyChanged("Description");
                }
                Hierarchy.IsDirty = true;
                MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
            }
        }

        public DateTime? CreationDate
        {
            get
            {
                if (_Hierarchy != null)
                {
                    return _Hierarchy.CreationDate;
                }
                else
                {
                    return null;
                }
            }
            set
            {
            }
        }

        public DateTime? LastUpdateTime
        {
            get
            {
                if (_Hierarchy != null)
                {
                    return _Hierarchy.LastUpdateTime;
                }
                else
                {
                    return null;
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
                if (_Hierarchy != null)
                {
                    return _Hierarchy.LastUpdateUser;
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

        [RegularExpression("[A-Za-z0-9]+", ErrorMessage = "Invalid characters entered."), StringLength(20, ErrorMessage = "Maximum length (20 characters) exceeded.")]
        public string Code
        {
            get
            {
                if (_Hierarchy != null)
                {
                    return _Hierarchy.Code;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                CertificatesChanged = true;
                if (_Hierarchy != null)
                {
                    _Hierarchy.Code = value;
                    RaisePropertyChanged("Code");
                    RaisePropertyChanged("enableStep");
                    if (Hierarchy.IsClonedRelated == false && Hierarchy.IsClonedRelatedUpdate == false)
                    {
                        if (Hierarchy.GroupId != -1)
                        {
                            if (Hierarchy.IsClonedRelatedSplit == true)
                                RaisePropertyChanged("ProjectSteps");
                        }
                        else
                            RaisePropertyChanged("ProjectSteps");

                    }
                    if (Hierarchy.IsClonedRelated == false)
                    {
                        if (String.IsNullOrEmpty(_Hierarchy.Code.Trim()))
                        {
                            Hierarchy.SelectedStep = " ";
                            RaisePropertyChanged("SelectedStep");


                        }
                    }

                    Hierarchy.IsDirty = true;
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }
            }
        }

        public Boolean Synchronization
        {

            get
            {
                if (_Hierarchy != null)
                {
                    return _Hierarchy.Synchronization;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                CertificatesChanged = true;
                if (_Hierarchy != null)
                {
                    _Hierarchy.Synchronization = value;
                    RaisePropertyChanged("Synchronization");
                    Hierarchy.IsDirty = true;
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }

            }

        }

        public Boolean IRISTechInd
        {

            get
            {
                if (_Hierarchy != null)
                {
                    return _Hierarchy.IRISTechInd;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                CertificatesChanged = true;
                if (_Hierarchy != null)
                {
                    _Hierarchy.IRISTechInd = value;
                    RaisePropertyChanged("IRISTechInd");
                    Hierarchy.IsDirty = true;
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }

            }

        }
        bool stepWasNull = false;
        public String SelectedStep
        {
            get
            {
                if (_Hierarchy != null)
                {
                    return _Hierarchy.SelectedStep;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                CertificatesChanged = true;
                if (_Hierarchy != null)
                {
                    //The selected step is null.
                    if (stepWasNull || String.IsNullOrEmpty(_Hierarchy.SelectedStep.Trim()))
                    {
                        stepWasNull = true;
                        string NewStep = value;
                        PopulateContents(NewStep);
                        _Hierarchy.SelectedStep = value;
                        RaisePropertyChanged("SelectedStep");
                        Hierarchy.IsDirty = true;
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                        //tabIndex = 0;
                        //RaisePropertyChanged("tabIndex");
                    }
                    else
                    {
                        _Hierarchy.SelectedStep = value;
                        RaisePropertyChanged("SelectedStep");
                        Hierarchy.IsDirty = true;
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                    }


                }
            }

        }
        public class GroupInvalidNameAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                if (value == null)
                {
                    return true;
                }
                if (value.ToString().Contains("/") || value.ToString().Contains("/") || value.ToString().Contains("\\")
                                                     || value.ToString().Contains("*") || value.ToString().Contains("?")
                                                     || value.ToString().Contains("\"") || value.ToString().Contains("<")
                                                     || value.ToString().Contains(">") || value.ToString().Contains("|")
                                                      || value.ToString().Contains("\t") || value.ToString().Contains(Environment.NewLine))
                {
                    return false;
                }

                return true;
            }
        }

        public class RequiredGroupNameAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                if (value == null || value == string.Empty)
                {
                    return false;
                }
                return true;
            }
        }


        [GroupInvalidName(ErrorMessage = "Group name can't contain any of the following characters: \\ / * ? \" < > |, tab, new line")]
        [RequiredGroupName(ErrorMessage = "'Group Name' field is required."), StringLength(50, ErrorMessage = "Maximum length (50 characters) exceeded.")]
        public String GroupName
        {
            get
            {
                if (_Hierarchy != null)
                {
                   // return _Hierarchy.GroupName;
                    if(Hierarchy.GroupId != -1 || _Hierarchy.IsClonedRelated)
                    //if (_Hierarchy.IsClonedRelated)
                    {
                        return _Hierarchy.GroupName.Trim();
                    }
                    else
                    {
                        return " ";
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                CertificatesChanged = true;
                if (_Hierarchy != null)
                {
                    _Hierarchy.GroupName = value;
                    RaisePropertyChanged("GroupName");
                    Hierarchy.VM.IsDirty = true;
                    Hierarchy.IsDirty = true;
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }
            }

        }

        public String GroupDescription
        {
            get
            {
                if (_Hierarchy != null)
                {
                    return _Hierarchy.GroupDescription;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                CertificatesChanged = true;
                if (_Hierarchy != null)
                {
                    _Hierarchy.GroupDescription = value;
                    RaisePropertyChanged("GroupDescription");
                    Hierarchy.IsDirty = true;
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }
            }

        }

        public String ActiveVersion
        {
            get
            {
                if (_Hierarchy != null)
                {
                    getActiveVersionName();
                    return _Hierarchy.ActiveVersion;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                CertificatesChanged = true;
                if (_Hierarchy != null)
                {
                    _Hierarchy.ActiveVersion = value;
                    RaisePropertyChanged("ActiveVersion");
                }
            }

        }

        public String ProjectStatus
        {
            get
            {
                if (_Hierarchy != null)
                {
                    return _Hierarchy.ProjectStatus;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                CertificatesChanged = true;
                if (_Hierarchy != null)
                {
                    _Hierarchy.ProjectStatus = value;
                    RaisePropertyChanged("ProjectStatus");
                }
            }

        }

        private static Boolean _enableStep = true;
        public Boolean enableStep
        {

            get
            {
                if (Hierarchy.IsClonedRelated == false && Hierarchy.IsClonedRelatedUpdate == false)
                {
                    if (Hierarchy.GroupId != -1)
                    {
                        return false; 
                    }
                    else
                        return (!(string.IsNullOrEmpty(_Hierarchy.Code.Trim())));
                }
                else if (Hierarchy.IsClonedRelated == true)
                {
                    if (Hierarchy.IsClonedRelatedUpdate == true)
                         return true;
                    else
                        return false;
                }
          

                return (!(string.IsNullOrEmpty(_Hierarchy.Code.Trim())));
            }
            set
            {
                _enableStep = value;
                RaisePropertyChanged("enableStep");
            }
          

        }

        public StringCollection ProjectSteps
        {

            get
            {
                return HierarchyBLL.GetAllSteps(Hierarchy.Code, Hierarchy.Id);
            }
            set
            {
            }
                    
        }

        public string TreeHeader
        {
            get
            {
                if (_Hierarchy != null)
                {
                    //_Hierarchy.TreeHeader = ("Production" + VersionBLL.getParentName(TreeId.ToString()))
                    return _Hierarchy.TreeHeader;
                }
                else
                {
                    return " ";
                }
            }
            set
            {
                CertificatesChanged = true;
               
                _Hierarchy.TreeHeader = value;
                RaisePropertyChanged("TreeHeader");
            }
        }
       
        private TreeViewNodeViewModelBase Node;


        private ObservableCollection<TreeViewNodeViewModelBase> _TreeNodes;
        public ObservableCollection<TreeViewNodeViewModelBase> TreeNodes
        {
            get
            {
                return _TreeNodes;
            }
            set
            {
                _TreeNodes = value;
                RaisePropertyChanged("TreeNodes");
            }
        }

        
        public ObservableCollection<UserCertificateApiModel> UserCertificates
        {
            get
            {
                return Hierarchy.UserCertificates;
            }
            set
            {

                Hierarchy.UserCertificates = value;
                RaisePropertyChanged("UserCertificates");
            }
        }


        
        [StringLength(100, ErrorMessage = "Maximum length (100 characters) exceeded.")]
        public string EcrId
        {
            get
            {
                if (_Hierarchy != null)
                {
                    return _Hierarchy.VM.EcrId;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {

                if (_Hierarchy != null)
                {
                    _Hierarchy.VM.EcrId = value;
                    RaisePropertyChanged("EcrId");
                    Hierarchy.IsDirty = true;
                    Hierarchy.VM.IsDirty = true;
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }
            }
        }


        #endregion

        #region Constructor

        public ProjectDetailsViewModel(Guid workspaceId, TreeViewNodeViewModelBase TV, ObservableCollection<TreeViewNodeViewModelBase> TreeNodes)
        {
            certificateToRemove = new List<CertificateModel>();
            allowExecute = true;               
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();

            // Initialize Object
            this.WorkspaceId = workspaceId;
            Node = TV;
            Hierarchy = TV.Hierarchy;
            isContentOrCertificateDroped = false;
            CheckHierarchy();

            if (Hierarchy.IsCloned && Hierarchy.IsNew)
            {
                Hierarchy.Name = TV.Name + "_Clone";
                ProjectSteps = HierarchyBLL.GetAllSteps(Hierarchy.Code, Hierarchy.Id);
                RaisePropertyChanged("ProjectSteps");
                Hierarchy.IsDirty = true;
            }

            //initialize Notes side bar
            bool showNotesSideBar = true;

            bool isNotCloneRelated = false;

            long HierarchyId = -1;

            if (Hierarchy.Id == -1)
            {
                isNotCloneRelated = false;
            }
            else
            {
                if (Hierarchy.GroupId != -1)
                {
                    if (Hierarchy.IsClonedRelatedSplit == true || Hierarchy.IsClonedRelatedUpdate == true)
                        isNotCloneRelated = true;
                    else
                        isNotCloneRelated = false;
                }
                else if (Hierarchy.IsClonedRelated == true)
                {
                    isNotCloneRelated = false;
                }
                else
                {
                    isNotCloneRelated = true;
                }

                if (Hierarchy.GroupId == -1)
                {
                    HierarchyId = TV.Hierarchy.Id;
                }
                else
                {
                    HierarchyId = Hierarchy.GroupId;
                }

            }
            Notes = new NotesControlViewModel(HierarchyId, TV.Name, ref isNotCloneRelated, ref showNotesSideBar, WorkspaceId);
            ShowNotes = showNotesSideBar;
            RaisePropertyChanged("ShowNotes");

            if (_Hierarchy.IsNew == true && Hierarchy.IsCloned == false && Hierarchy.IsClonedRelated == false && Hierarchy.VM.VersionId == -1)
            {
                string defaultVersionName = VersionBLL.GenerateDefaultVersionName((int)Hierarchy.Id);
                if (Hierarchy.VM.Contents != null && Hierarchy.VM.Contents.Count > 0)
                {
                    if (defaultVersionName != string.Empty)
                    {
                        Hierarchy.VM.VersionName = defaultVersionName;
                    }
                    else //if failed to generate - current functionality
                    {
                        Hierarchy.VM.VersionName = "New Version";
                    }
                }
                else
                {
                    Hierarchy.VM.VersionName = "New Version";
                }
                Hierarchy.VM.Description = Hierarchy.VM.VersionName;
                VM.Description = VM.VersionName;
                Hierarchy.VM.EcrId = string.Empty;
                RaisePropertyChanged("EcrId");
            }
            if (!Hierarchy.IsCloned && !Hierarchy.IsClonedRelatedSplit)
            {
                if (_Hierarchy.IsNew != true || Hierarchy.VM.VersionId != -1)
                {
                    if (Hierarchy.GroupId != -1)
                    {
                        Hierarchy.VM = VersionBLL.GetActiveVersion(Hierarchy.GroupId);
                    }
                    else
                        Hierarchy.VM = VersionBLL.GetActiveVersion(Hierarchy.Id);


                }
            }
            if (Hierarchy.IsClonedRelated == true && Hierarchy.IsClonedRelatedSplit == false)
            {
                enableStep = false;
                RaisePropertyChanged("enableStep");
                LockCheck = false;
                LockVersion = true;
                Hierarchy.VM.TargetPath = getTargetPath();
                RaisePropertyChanged("TargetPath");
                //Hierarchy.Name = "New Project";
                Hierarchy.Name = TV.Name + "_Clone";
                RaisePropertyChanged("Name");
                if (Hierarchy.GroupId == -1)
                {
                    Hierarchy.GroupName = "New Group";
                    RaisePropertyChanged("GroupName");
                    Hierarchy.GroupDescription = "New Group Description";
                    RaisePropertyChanged("GroupDescription");
                }
       
            }

            if (Hierarchy.IsClonedRelatedSplit == true)
            {
                Hierarchy.VM.TargetPath = getTargetPath();
                RaisePropertyChanged("TargetPath");
            }


            //Lock fields of related project
            if (Hierarchy.IsNew == false && Hierarchy.IsClonedRelated == false && Hierarchy.GroupId != -1 && Hierarchy.IsClonedRelatedUpdate == false)
            {
                enableStep = false;
                RaisePropertyChanged("enableStep");
                ProjectDetailsViewModel.LockVersion = true;
                ProjectDetailsViewModel.LockCheck = false;
                ProjectDetailsViewModel.ReadGroup = true;

            }
            if (Hierarchy.IsClonedRelated == false && Hierarchy.GroupId == -1)
            {
                enableStep = true;
                RaisePropertyChanged("enableStep");
                ProjectDetailsViewModel.LockVersion = false;
                ProjectDetailsViewModel.LockCheck = true;
                ProjectDetailsViewModel.ReadGroup = true;
            }


            certificateDataFiller();
            InitialCertificates.Clear();
            CertificateBLL.CertificateBLLReturnResult getCertstatus = CertificateBLL.GetNodeCertificatesDB(Hierarchy, out InitialCertificates);
            if (getCertstatus != CertificateBLL.CertificateBLLReturnResult.Success)
            {
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, new object[] {0});
            }

            InitialUserCertificates.Clear();
            UserCertificateFiller();
            foreach (UserCertificateApiModel uc in Hierarchy.UserCertificates)
            {
                InitialUserCertificates.Add(uc);
            }

            _TreeNodes = TreeNodes;
            ServiceProvider.RegisterService<FolderBrowserService>(new FolderBrowserService());
            FolderBrowserDialogService = GetService<FolderBrowserService>();
            getSequence();
            if ((Hierarchy.IsClonedRelatedUpdate == true || Hierarchy.IsClonedRelatedSplit == true) && LockName == false && LockSync == true)
            {
                Hierarchy.IsClonedRelatedUpdate = false;
                Hierarchy.IsClonedRelatedSplit = false;

            }
            if (activeContents != null && activeContents.ToList().Count > 0)
            {
              getTreelist();
              RaisePropertyChanged("TreeNodesLinks");
            }
            LastVersionName = string.Empty;
            InitialVersionName = Hierarchy.VM.VersionName;
            if (Hierarchy.Id == -1 || Hierarchy.IsCloned || Hierarchy.IsClonedRelated || String.IsNullOrEmpty(Hierarchy.VM.TargetPath.Trim())
                || Hierarchy.Name == "New Project")
                tabIndex = 1;
            else
                tabIndex = 0;

            MessageMediator.Register(this.WorkspaceId + "OnShowContentTabDetailsReceived", new Action<CMTreeViewNodeViewModelBase>(OnShowContentTabDetailsReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnShowCloneDetailsReceived", new Action<TreeViewNodeViewModelBase>(OnShowCloneDetailsReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnRapidExecutionReceived", new Action<TreeViewProjectNodeViewModel>(OnRapidExecutionReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnRapidExecutionStartReceived", new Action<TreeViewProjectNodeViewModel>(OnRapidExecutionStartReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnClearViewModelReceived", new Action<TreeViewProjectNodeViewModel>(OnClearViewModelReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnClearVersionViewModelReceived", new Action<TreeViewVersionNodeViewModel>(OnClearVersionViewModelReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnDisableProjectReceived", new Action<TreeViewProjectNodeViewModel>(OnDisableProjectReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnIsRefreshDirtyNodeReceived", new Action<TreeViewNodeViewModelBase>(OnIsRefreshDirtyNodeReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnExecuteLinkedVersionReceived", new Action<CMTreeViewVersionNodeViewModel>(OnExecuteLinkedVersionReceived)); //Switched to the first tab when executing linked version
            MessageMediator.Register(this.WorkspaceId + "OnLinkedVersionExecutionStartReceived", new Action<CMTreeViewVersionNodeViewModel>(OnLinkedVersionExecutionStartReceived)); //Progress bar when executing linked version
            MessageMediator.Register(this.WorkspaceId + "OnLinkedVersionExecutionReceived", new Action<CMTreeViewVersionNodeViewModel>(OnLinkedVersionExecutionReceived)); //Progress bar when executing linked version            
            MessageMediator.Register(this.WorkspaceId + "OnProgressTextReceived", new Action<TreeViewProjectNodeViewModel>(OnProgressTextReceived)); //Progress bar text   
            MessageMediator.Register(this.WorkspaceId + "OnContentsTabProgressTextReceived", new Action<CMTreeViewVersionNodeViewModel>(OnContentsTabProgressTextReceived)); //Progress bar text     
            MessageMediator.Register(this.WorkspaceId + "OnRefreshNotesReceived", new Action<long>(OnRefreshNotesReceived)); //Refresh notes
        }


        public ProjectDetailsViewModel(Guid workspaceId, TreeViewNodeViewModelBase TV)
        {
            certificateToRemove = new List<CertificateModel>();
            allowExecute = true;
 
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();
            // Initialize Object
            this.WorkspaceId = workspaceId;
            Node = TV;
            Hierarchy = TV.Hierarchy;
            CheckHierarchy();
            isContentOrCertificateDroped = false;

            

            //initialize project certificates
          /**  if (Hierarchy.IsCloned == false && Hierarchy.IsClonedRelated == false && Hierarchy.IsClonedRelatedUpdate == false && Hierarchy.IsClonedRelatedSplit == false)
            {
                Hierarchy.Certificates.Clear();
            }
            //initialize project contents
            if (Hierarchy.IsCloned == false && Hierarchy.IsClonedRelated == false && Hierarchy.IsClonedRelatedUpdate == false && Hierarchy.IsClonedRelatedSplit == false)
            {
                Hierarchy.VM.Contents.Clear();
            }
           * **/

            if (Hierarchy.IsCloned && Hierarchy.IsNew)
            {
                ProjectSteps = HierarchyBLL.GetAllSteps(Hierarchy.Code, Hierarchy.Id);
                RaisePropertyChanged("ProjectSteps");
            }

            //initialize Notes side bar
            bool showNotesSideBar = true;

            bool isCloneRelated = false;

            long HierarchyId = -1;
            if (Hierarchy.Id == -1)
            {
                isCloneRelated = false;
            }
            else
            {
                if (Hierarchy.GroupId != -1)
                {
                    if (Hierarchy.IsClonedRelatedSplit == true || Hierarchy.IsClonedRelatedUpdate == true)
                        isCloneRelated = true;
                    else
                        isCloneRelated = false;
                }
                else if (Hierarchy.IsClonedRelated == true)
                {
                    isCloneRelated = false;
                }
                else
                {
                    isCloneRelated = true;
                }

                if (Hierarchy.GroupId == -1)
                {
                    HierarchyId = TV.Hierarchy.Id;
                }
                else
                {
                    HierarchyId = Hierarchy.GroupId;
                }

            }
            Notes = new NotesControlViewModel(HierarchyId, TV.Name, ref isCloneRelated, ref showNotesSideBar, this.WorkspaceId);
            ShowNotes = showNotesSideBar;
            RaisePropertyChanged("ShowNotes");
            //VersionDetailsViewModel vm = new VersionDetailsViewModel(workspaceId, TV);
            //_Hierarchy.GetAllVersions = ATSBusinessLogic.VersionBLL.GetVersion(_Hierarchy.Id);
            //_GetAllVersions = ATSBusinessLogic.VersionBLL.GetVersion(_Hierarchy.Id);
            if (_Hierarchy.IsNew == true && Hierarchy.IsCloned == false && Hierarchy.IsClonedRelated == false && Hierarchy.VM.VersionId == -1)
            {
                Hierarchy.VM.VersionName = "New Version";
                Hierarchy.VM.Description = Hierarchy.VM.VersionName;
            }
            if (!Hierarchy.IsCloned && !Hierarchy.IsClonedRelatedSplit)
            {
                if (_Hierarchy.IsNew != true || Hierarchy.VM.VersionId != -1)
                {
                    //_Hierarchy.GetAllVersions = ATSBusinessLogic.VersionBLL.GetVersion(_Hierarchy.Id);
                    if (Hierarchy.GroupId != -1)
                    {
                        Hierarchy.VM = VersionBLL.GetActiveVersion(Hierarchy.GroupId);
                    }
                    else
                        Hierarchy.VM = VersionBLL.GetActiveVersion(Hierarchy.Id);

                }
            }
            if (Hierarchy.IsClonedRelated == true && Hierarchy.IsClonedRelatedSplit == false)
            {
                enableStep = false;
                LockVersion = true;
                LockCheck = false;
                RaisePropertyChanged("enableStep");
                Hierarchy.VM.TargetPath = getTargetPath();
                RaisePropertyChanged("TargetPath");
                Hierarchy.Name = "New Project";
                RaisePropertyChanged("Name");
                if (Hierarchy.GroupId == -1)
                {
                    Hierarchy.GroupName = "New Group";
                    RaisePropertyChanged("GroupName");
                    Hierarchy.GroupDescription = "New Group Description";
                    RaisePropertyChanged("GroupDescription");
                }
            }

            if (Hierarchy.IsNew == false && Hierarchy.IsClonedRelated == false && Hierarchy.GroupId != -1)
            {
                enableStep = false;
                RaisePropertyChanged("enableStep");
                ProjectDetailsViewModel.LockVersion = true;
                ProjectDetailsViewModel.LockCheck = false;
                ProjectDetailsViewModel.ReadGroup = true;

            }
            
            certificateDataFiller();
            ServiceProvider.RegisterService<FolderBrowserService>(new FolderBrowserService());
            FolderBrowserDialogService = GetService<FolderBrowserService>();
            getSequence();
            if ((Hierarchy.IsClonedRelatedUpdate == true || Hierarchy.IsClonedRelatedSplit == true) && LockName == false && LockSync == true)
            {
                Hierarchy.IsClonedRelatedUpdate = false;
                Hierarchy.IsClonedRelatedSplit = false;

            }
            if (activeContents != null && activeContents.ToList().Count > 0)
            {
                 getTreelist();
               RaisePropertyChanged("TreeNodesLinks");
            }

            if (Hierarchy.IsClonedRelatedSplit == true)
            {
                Hierarchy.VM.TargetPath = getTargetPath();
                RaisePropertyChanged("TargetPath");
            }
            LastVersionName = string.Empty;
            InitialVersionName = Hierarchy.VM.VersionName;
            if (Hierarchy.Id == -1 || Hierarchy.IsCloned || Hierarchy.IsClonedRelated || String.IsNullOrEmpty(Hierarchy.VM.TargetPath.Trim())
                || Hierarchy.Name == "New Project")
                tabIndex = 1;
            else
                tabIndex = 0;


            MessageMediator.Register(this.WorkspaceId + "OnShowContentTabDetailsReceived", new Action<CMTreeViewNodeViewModelBase>(OnShowContentTabDetailsReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnShowCloneDetailsReceived", new Action<TreeViewNodeViewModelBase>(OnShowCloneDetailsReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnRapidExecutionReceived", new Action<TreeViewProjectNodeViewModel>(OnRapidExecutionReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnClearViewModelReceived", new Action<TreeViewProjectNodeViewModel>(OnClearViewModelReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnClearVersionViewModelReceived", new Action<TreeViewVersionNodeViewModel>(OnClearVersionViewModelReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnDisableProjectReceived", new Action<TreeViewProjectNodeViewModel>(OnDisableProjectReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnIsRefreshDirtyNodeReceived", new Action<TreeViewNodeViewModelBase>(OnIsRefreshDirtyNodeReceived)); //Register to recieve a message asking for node refresh
            UserCertificateFiller();
            MessageMediator.Register(this.WorkspaceId + "OnExecuteLinkedVersionReceived", new Action<CMTreeViewVersionNodeViewModel>(OnExecuteLinkedVersionReceived)); //Switched to the first tab when executing linked version
            MessageMediator.Register(this.WorkspaceId + "OnLinkedVersionExecutionStartReceived", new Action<CMTreeViewVersionNodeViewModel>(OnLinkedVersionExecutionStartReceived)); //Progress bar when executing linked version
            MessageMediator.Register(this.WorkspaceId + "OnLinkedVersionExecutionReceived", new Action<CMTreeViewVersionNodeViewModel>(OnLinkedVersionExecutionReceived)); //Progress bar when executing linked version            
            MessageMediator.Register(this.WorkspaceId + "OnProgressTextReceived", new Action<TreeViewProjectNodeViewModel>(OnProgressTextReceived)); //Progress bar text     
            MessageMediator.Register(this.WorkspaceId + "OnContentsTabProgressTextReceived", new Action<CMTreeViewVersionNodeViewModel>(OnContentsTabProgressTextReceived)); //Progress bar text     
             
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
            if (!Hierarchy.IsDirty)
                return false;

            if (!Domain.IsPermitted("123"))
                return false;

            if (String.IsNullOrEmpty(VersionName.Trim()) || VersionName.Length > 50 || (String.IsNullOrEmpty(Name.Trim()) || Name.Length > 200)
                || (Code.Length > 20) || (Description.Length > 500) || (String.IsNullOrEmpty(VersionDescription.Trim())
                || VersionDescription.Length > 50) || (String.IsNullOrEmpty(TargetPath.Trim())) || 
                (!string.IsNullOrEmpty(Hierarchy.VM.EcrId) && Hierarchy.VM.EcrId.Length>100))
            {
                allowExecute = false;
                return false;
            }
            VersionInvalidNameAttribute vInvAttr = new VersionInvalidNameAttribute();
            if (!vInvAttr.IsValid(VersionName))
            {
                allowExecute = false;
                return false;
            }

            ProjectInvalidNameAttribute pNameInvAttr = new ProjectInvalidNameAttribute();
            if (!pNameInvAttr.IsValid(Hierarchy.Name))
            {
                allowExecute = false;
                return false;
            }

            VersionNameAttribute Vma = new VersionNameAttribute();
            if (!Vma.IsValid(Hierarchy.VM.VersionName))
            {
                allowExecute = false;
                return false;
            }

            RequiredGroupNameAttribute gnr = new RequiredGroupNameAttribute();
            GroupInvalidNameAttribute gna = new GroupInvalidNameAttribute();
            if (Hierarchy.IsClonedRelated && (!gna.IsValid(Hierarchy.GroupName) ||
                                                !gnr.IsValid(Hierarchy.GroupName) ||
                                                Hierarchy.GroupName.Length > 50 ||
                                                string.IsNullOrWhiteSpace(Hierarchy.GroupName) ||
                                                string.IsNullOrEmpty(Hierarchy.GroupName))
                )
            {
                allowExecute = false;
                return false;
            }
            allowExecute = false;
            return true;
        }

         
        private void ExecuteSaveCommand()
        {
            try
            {
                if (Hierarchy.ParentId != 0)
                {
                    HierarchyModel parentNode = HierarchyBLL.GetHierarchyRow(Hierarchy.ParentId);
                    if (parentNode.NodeType != NodeTypes.F)
                    {
                        Object[] UserArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(152, UserArgsList);
                        return;
                    }

                }
                //ContentBLL.isCMFlyoutOpen = false; //CR 3600
                Domain.PersistenceLayer.BeginTransWithIsolation(IsolationLevel.Serializable);

                // Work variables
                Collection<string> StatusBarParameters = new Collection<string>();

                #region// (1) Check last Update
                if (!Hierarchy.IsCloned)
                {
                    string updateCheck = HierarchyBLL.LastUpadateCheck(ref _Hierarchy);
                    if (!(String.IsNullOrEmpty(updateCheck)))
                    {
                        Domain.PersistenceLayer.AbortTrans();
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);   
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(updateCheck), ArgsList);
                        return;
                    }

                    //(2) Check last version.
                    if (Hierarchy.VM.IsDirty)
                    {
                        string updateVersionCheck = VersionBLL.LastUpadateVersionCheck(ref _Hierarchy);
                        if (!(String.IsNullOrEmpty(updateVersionCheck)))
                        {
                            Domain.PersistenceLayer.AbortTrans();
                            MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                            Object[] ArgsList = new Object[] { 0 };
                            ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(updateVersionCheck), ArgsList);
                            return;
                        }
                    }
                }
                #endregion

                #region //(3) Check Group last update.
                if (Hierarchy.GroupId != -1)
                {
                    string updateGroupCheck = HierarchyBLL.LastUpadateGroupCheck(ref _Hierarchy);
                    if (!(String.IsNullOrEmpty(updateGroupCheck)))
                    {
                        Domain.PersistenceLayer.AbortTrans();
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(updateGroupCheck), ArgsList);
                        return;
                    }
                }
                #endregion

                #region //ContentRemoved
                if (contentsUpdated)
                {
                    Hierarchy.VM.IsClosed = true;
                    Hierarchy.VM.Contents.Clear();
                    foreach (var i in _activeContents.OrderBy(x => x.seq))
                    {
                        Hierarchy.VM.Contents.Add(i); //Add new contents for insert query.
                    }
                }
                #endregion

                #region//Remove certificate from DB after remove action.
                if (certificateRemove && !Hierarchy.IsCloned && (!(Hierarchy.IsClonedRelated && Hierarchy.GroupId == -1)))
                {
                    //(4) Remove certificate last update.
                    //string CertificateLastUpdateCheck = CertificateBLL.CheckLastUpdateCertificateDelete(Certificates, Hierarchy.Id, certificateToRemove);

                    string CertificateLastUpdateCheck = CertificateBLL.CheckLastUpdateCertificate(InitialCertificates, Hierarchy.Id);
                    if (!(String.IsNullOrEmpty(CertificateLastUpdateCheck)))
                    {

                        Domain.PersistenceLayer.AbortTrans();
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(CertificateLastUpdateCheck), ArgsList);
                        return;
                    }

                    if (!deleteCertificate())
                    {
                        //Failed delete certificate
                        Domain.PersistenceLayer.AbortTrans();
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                        return;
                    }
                    else
                    {
                        certificateRemove = false;
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "UpdateNode", this._Hierarchy);

                        //Need to populate initial certificates again because delete Certificate functionality is out of ExecuteSaveCommand
                        InitialCertificates.Clear();
                        CertificateBLL.CertificateBLLReturnResult getCertstatus = CertificateBLL.GetNodeCertificatesDB(Hierarchy, out InitialCertificates);
                        if (getCertstatus != CertificateBLL.CertificateBLLReturnResult.Success)
                        {
                            ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, new object[] { 0 });
                        }
                    }

                }
                #endregion
                
                if (!Hierarchy.IsCloned && !Hierarchy.IsClonedRelatedUpdate && !Hierarchy.IsClonedRelated)
                {
                    //string LastUpdateCertificate = CertificateBLL.CheckLastUpdateCertificateAdd(Certificates, Hierarchy.Id, Hierarchy.Certificates);

                    //string LastUpdateCertificate = CertificateBLL.CheckLastUpdateCertificate(InitialCertificates, Hierarchy.Id);
                    //if (!(String.IsNullOrEmpty(LastUpdateCertificate)))
                    //{
                    //    Domain.PersistenceLayer.AbortTrans();

                    //    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                    //    Object[] ArgsList = new Object[] { 0 };
                    //    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(LastUpdateCertificate), ArgsList);
                    //    return;
                    //}

                    //(6) Remove certificate last update.
                    string UserCertificateLastUpdateCheck = UserCertificateBLL.CheckLastUpdateUserCertificate(InitialUserCertificates, Hierarchy.Id);
                                                
                        if (!(String.IsNullOrEmpty(UserCertificateLastUpdateCheck)))
                        {

                            Domain.PersistenceLayer.AbortTrans();
                            MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                            Object[] ArgsList = new Object[] { 0 };
                            ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(UserCertificateLastUpdateCheck), ArgsList);
                            return;
                        }
                    if (UserCertificatesRemoved.Count > 0)
                    {
                        //string UserCertificateLastUpdateCheck = UserCertificateBLL.CheckLastUpdateUserCertificateDelete(Hierarchy.UserCertificates, Hierarchy.Id, UserCertificatesRemoved);

                        //Remove user certificate from DB
                        string UserPersisted = string.Empty;
                        UserPersisted = UserCertificateBLL.DeleteUserCertificate(UserCertificatesRemoved, Hierarchy.Id);
                        if (!string.IsNullOrEmpty(UserPersisted))
                        {
                            //failed to remove user certificate.
                            Domain.PersistenceLayer.AbortTrans();
                            MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                            Object[] UserArgsList = new Object[] { 0 };
                            ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(UserPersisted), UserArgsList);
                        }
                    }

                    //(7) add user certificate last update.
                    //string AddUserCertificateLastUpdateCheck = UserCertificateBLL.CheckLastUpdateUserCertificateAdd(Hierarchy.UserCertificates, Hierarchy.Id);
                                     
                    //Temporary solution for add user certificate - no last update check.

                    //if (!(String.IsNullOrEmpty(AddUserCertificateLastUpdateCheck)))
                    //{

                    //    Domain.PersistenceLayer.AbortTrans();
                    //    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                    //    Object[] ArgsList = new Object[] { 0 };
                    //    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(AddUserCertificateLastUpdateCheck), ArgsList);
                    //    return;
                    //}
                }
                #region//Closed Version.
                if (Hierarchy.VM.IsClosed == true && Hierarchy.IsClonedRelatedSplit == false && Hierarchy.IsCloned == false)
                {
                    var TB = new StringBuilder(string.Empty);
                    TB.Append("SELECT Description FROM PE_Messages where id=118");
                    DialogResults ClosedAnswer = MsgBoxService.ShowOkCancel((Domain.PersistenceLayer.FetchDataValue(TB.ToString(), CommandType.Text, null)).ToString(), DialogIcons.Question);
                    if (ClosedAnswer == DialogResults.Cancel)
                    {
                        Domain.PersistenceLayer.CommitTrans();
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", null);
                        //Comment :Not refresh anymore.
                        //Refresh Hierarchy .Not saving and opening new version.
                        //this.Hierarchy = HierarchyBLL.GetHierarchyRow(Hierarchy.Id);
                        //this.Hierarchy.VM = VersionBLL.GetVersionRow(Hierarchy.Id);
                        //MessageMediator.NotifyColleagues(this.WorkspaceId + "UpdateNode", this._Hierarchy); //Send message to the Explorer
                        //Hierarchy.IsDirty = false;
                        //MessageMediator.NotifyColleagues(this.WorkspaceId + "ShowProjectDetails", this.Node);
                        return;
                    }

                    //Closed Version function
                    string ClosedVersionResult = HierarchyBLL.PersistClosedVersion(ref _Hierarchy);
                    if (!(ClosedVersionResult.Equals(string.Empty)))
                    {
                        if (ClosedVersionResult == "124" || ClosedVersionResult == "105")
                        {
                            Domain.PersistenceLayer.AbortTrans();
                            MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                            Object[] UserArgsList = new Object[] { 0 };
                            ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(ClosedVersionResult), UserArgsList);
                            return;
                        }

                    }
                }//end of close version.
                #endregion

                #region//Add New Related Group and Project.
                if (Hierarchy.IsClonedRelated == true && Hierarchy.GroupId == -1)
                {
                    if (String.IsNullOrEmpty(GroupName.Trim()))
                    {

                        Domain.PersistenceLayer.AbortTrans();
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                        var SB = new StringBuilder(string.Empty);
                        SB.Append("Group Name Is Required");
                        MsgBoxService.ShowError(SB.ToString());
                        return;
                    }
                    if (String.IsNullOrEmpty(GroupDescription.Trim()))
                    {
                        Domain.PersistenceLayer.AbortTrans();
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                        var SB = new StringBuilder(string.Empty);
                        SB.Append("Group Description Is Required");
                        MsgBoxService.ShowError(SB.ToString());
                        return;
                    }

                    if (!(String.IsNullOrEmpty(SelectedStep.Trim())))
                    {
                        if (String.IsNullOrEmpty(Code.Trim()))
                        {
                            Domain.PersistenceLayer.AbortTrans();
                            MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                            var SB = new StringBuilder(string.Empty);
                            SB.Append("Code Is Required When Step Is Not Empty");
                            MsgBoxService.ShowError(SB.ToString());
                            return;
                        }
                    }
                  
                    Boolean GroupN = HierarchyBLL.getGroupName(Hierarchy.GroupName);
                    if (GroupN)
                    {
                        Domain.PersistenceLayer.AbortTrans();
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(123, ArgsList);
                        return;
                    }
                    if (Hierarchy.ParentId != 0)
                    {
                        HierarchyModel ParentHierachy = new HierarchyModel();
                        ParentHierachy = HierarchyBLL.GetHierarchyRow(Hierarchy.ParentId);
                        if (ParentHierachy.NodeType == NodeTypes.P)
                        {
                            Domain.PersistenceLayer.AbortTrans();
                            Object[] ArgsList = new Object[] { 0 };
                            ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(152, ArgsList);
                            return;
                        }
                    }
                    //Add new Related Project.
                    HierarchyModel hm = new HierarchyModel();
                    long HiD = Hierarchy.Id;
                    string Persisted = HierarchyBLL.PresistRelatedProject(ref _Hierarchy);

                    if (String.IsNullOrEmpty(Persisted))
                    {
                        Domain.PersistenceLayer.CommitTrans();
                        InitialVersionName = string.Empty;

                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", null);
                    
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", null);
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "AddClonedProject", this._Hierarchy);
     
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "ShowProjectDetails", this.Node);
                        Object[] ArgsList = new Object[] { Hierarchy.Name };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(115, ArgsList);
                        hm = HierarchyBLL.GetHierarchyRow(HiD);

                        this.Node.Hierarchy = hm;
                        RaisePropertyChanged("GroupName");
                        //Hierarchy = HierarchyBLL.GetHierarchyRow(Hierarchy.Id);
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "UpdateNode", hm);
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnSplitDetailsReceived", this._Hierarchy);
                       
                    }
                    else
                    {
                        Domain.PersistenceLayer.AbortTrans();
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Persisted), ArgsList);
                        return;
                    }
                }
                #endregion

                #region//Add New Related Project. Add clone Related project without adding new group (existing group.)
                else if (Hierarchy.IsClonedRelated == true && Hierarchy.GroupId != -1)
                {
                    if (!(String.IsNullOrEmpty(SelectedStep.Trim())))
                    {
                        if (String.IsNullOrEmpty(Code.Trim()))
                        {
                            Domain.PersistenceLayer.AbortTrans();
                            MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                            MessageMediator.NotifyColleagues(this.WorkspaceId + "AddClonedProject", this._Hierarchy);
                            var SB = new StringBuilder(string.Empty);
                            SB.Append("Code Is Required When Step Is Not Empty");
                            MsgBoxService.ShowError(SB.ToString());
                            return;
                        }
                    }
                   
                    HierarchyModel hm = new HierarchyModel();
                    long HiD = Hierarchy.Id;
                    hm = HierarchyBLL.GetHierarchyRow(Hierarchy.Id);

                    //Add clone Related project without adding new group (existing group.)
                    string Persisted = HierarchyBLL.UpdateRelatedProject(ref _Hierarchy);
                    if (String.IsNullOrEmpty(Persisted))
                    {
                        Domain.PersistenceLayer.CommitTrans();
                        InitialVersionName = string.Empty;
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", null);
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", null);
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "AddClonedProject", this._Hierarchy);
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "ShowProjectDetails", this.Node);
                      
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(115, ArgsList);
                        this.Node.Hierarchy = hm;
                    }
                    else
                    {
                        Domain.PersistenceLayer.AbortTrans();
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Persisted), ArgsList);
                        return;
                    }
                }
                #endregion

                #region//Split Related Project.
                else if (Hierarchy.IsClonedRelatedSplit == true)
                {
                    string Persisted = HierarchyBLL.SplitRelatedProject(ref _Hierarchy);
                    // Update Statusbar
                    if (Persisted.Equals(string.Empty))
                    {

                        Domain.PersistenceLayer.CommitTrans();
                        InitialVersionName = string.Empty;
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", null);
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", null);
                        allowExecute = true;
                        Object[] ArgsList = new Object[] { Hierarchy.Name };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(131, ArgsList);
                        this.Hierarchy = HierarchyBLL.GetHierarchyRow(Hierarchy.Id);
                        this.Hierarchy.VM = VersionBLL.GetActiveVersion(Hierarchy.Id);
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "UpdateNode", this._Hierarchy); //Send message to the Explorer
                        Node.IsDirty = false;
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "ShowProjectDetails", this.Node);
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnSplitDetailsReceived", this._Hierarchy);
                        return;
                    }
                    else
                    {
                        Domain.PersistenceLayer.AbortTrans();
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                        Object[] ArgsList = new Object[] { Persisted };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Persisted), ArgsList);
                    }

                }
                #endregion

                #region//Add new Project/ Clone Project / Update project related.
                else
                {
                    if ((Hierarchy.GroupId != -1) && (!(String.IsNullOrEmpty(SelectedStep.Trim()))))
                    {
                        if (String.IsNullOrEmpty(Code.Trim()))
                        {
                            Domain.PersistenceLayer.AbortTrans();
                            MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                            var SB = new StringBuilder(string.Empty);
                            SB.Append("Code Is Required When Step Is Not Empty");
                            MsgBoxService.ShowError(SB.ToString());
                            return;
                        }
                    }

                    //Add new Project/ Clone Project / Update project related./All Related.

                    HierarchyModel hm = new HierarchyModel();
                    long HiD = -1;

                    if (this.Hierarchy.IsCloned == true)
                    {
                        if (Hierarchy.ParentId != 0)
                        {
                            HierarchyModel ParentHierachy = new HierarchyModel();
                            ParentHierachy = HierarchyBLL.GetHierarchyRow(Hierarchy.ParentId);
                            if (ParentHierachy.NodeType == NodeTypes.P)
                            {
                                Domain.PersistenceLayer.AbortTrans();
                                Object[] ArgsList = new Object[] { 0 };
                                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(152, ArgsList);
                                return;
                            }
                        }
                        HiD = Hierarchy.Id;
                        hm = HierarchyBLL.GetHierarchyRow(Hierarchy.Id);

                    }

                    foreach (UserCertificateApiModel uc in InitialUserCertificates)
                    {
                        UserCertificateApiModel ucItemToRemove = (UserCertificateApiModel)Hierarchy.UserCertificates.Where(x => x.UserCertificateId.Equals(uc.UserCertificateId)).FirstOrDefault();

                        if (ucItemToRemove != null && ucItemToRemove.IsNew == true)
                            Hierarchy.UserCertificates.Remove(ucItemToRemove);
                    }

                    //For clone project - don't remove the certificate. they will be coppied.
                    if (!Hierarchy.IsCloned)
                    {
                        foreach (string uc in InitialCertificates.Keys)
                        {
                            String itemToRemove = Convert.ToString(Hierarchy.Certificates.Where(x => x.Equals(uc)).FirstOrDefault());
                            Hierarchy.Certificates.Remove(itemToRemove);
                        }
                    }

                    string Persisted = HierarchyBLL.PersistProject(ref _Hierarchy);

                    if (Persisted.Equals(string.Empty))
                    {
                        Domain.PersistenceLayer.CommitTrans();
                        InitialVersionName = string.Empty;
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", null);
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", null);
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "UpdateGroupLastUpdate", Hierarchy);
                        if (this.Hierarchy.IsCloned == true)
                        {
                            //Update node parent after clone project.
                            Hierarchy.IsCloned = false;    
                            MessageMediator.NotifyColleagues(this.WorkspaceId + "AddClonedProject", this._Hierarchy);
                            MessageMediator.NotifyColleagues(this.WorkspaceId + "ShowProjectDetails", this.Node);
                            this.Node.Hierarchy = hm;
                        }
                        else
                        {
                            if (Hierarchy.IsClonedRelatedUpdate)
                                UpdateGroupNode(_TreeNodes); //Update last update to the entire group.
                            
                            MessageMediator.NotifyColleagues(this.WorkspaceId + "ShowProjectDetails", this.Node);
                        }
                        Hierarchy.IsClonedRelatedUpdate = false;
                        //Refresh list of initial certificates after save.
                        InitialCertificates.Clear();
                        CertificateBLL.CertificateBLLReturnResult getCertstatus = CertificateBLL.GetNodeCertificatesDB(Hierarchy, out InitialCertificates);
                        if (getCertstatus != CertificateBLL.CertificateBLLReturnResult.Success)
                        {
                            ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, new object[] { 0 });
                        }

                        allowExecute = true;
                        Object[] ArgsList = new Object[] { Hierarchy.Name };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(131, ArgsList);
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "UpdateNode", this._Hierarchy);
                    }
                    else
                    {
                        Domain.PersistenceLayer.AbortTrans();
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Persisted), ArgsList);
                    }

                    contentsUpdated = false;

                    certificateRemove = false;
                    Hierarchy.Certificates.Clear();
                    certificateToRemove.Clear();
                }
                #endregion
            }
            catch (Exception E)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", E); // TODO: Log error
                String logMessage = E.Message + "\n" + "Source: " + E.Source + "\n" + E.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Domain.PersistenceLayer.AbortTrans();
                MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                if (E.Message == "DB Error")
                {
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(141, ArgsList);
                }
                else
                {
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
                }
            }
        }

        #endregion

        #region  Add Content Command

        private RelayCommand _AddContentCommand;
        public RelayCommand AddContentCommand
        {
            get
            {
                if (_AddContentCommand == null)
                {
                    _AddContentCommand = new RelayCommand(ExecuteAddContentCommand, CanExecuteAddContentCommand);
                }
                return _AddContentCommand;
            }
        }

        private bool CanExecuteAddContentCommand()
        {
            if (Domain.IsPermitted("106"))
            {
                if (Hierarchy.GroupId != -1)
                {
                    if (Hierarchy.IsClonedRelatedSplit == true || Hierarchy.IsClonedRelatedUpdate == true)
                        return true;
                    else
                        return false;
                }

                else if (Hierarchy.IsClonedRelated == true)
                    return false;
                else
                    return true;

            }
            else
                return false;
        }

        private void ExecuteAddContentCommand()
        {
            tabIndex = 0;
            //ContentBLL.isCMFlyoutOpen = true;
            RaisePropertyChanged("tabIndex");
            MessageMediator.NotifyColleagues("ShowAddContent", WorkspaceId); //Send message to the MainViewModel
        }

        #endregion

        #region  Add Certificate Command

        private RelayCommand _AddCertCommand;
        public RelayCommand AddCertCommand
        {

            get
            {

                if (_AddCertCommand == null)
                {
                    _AddCertCommand = new RelayCommand(ExecuteAddCertCommand, CanExecuteAddCertCommand);
                }
                return _AddCertCommand;
            }
        }

        private bool CanExecuteAddCertCommand()
        {
            if (Domain.IsPermitted("108"))
            {
                if (Hierarchy.IsClonedRelatedSplit == true || Hierarchy.IsClonedRelatedUpdate == true || Hierarchy.IsClonedRelated == true)
                        return false;
                    else
                        return true;
            }
            else
                return false;

        }

        private void ExecuteAddCertCommand()
        {
            tabIndex = 3;
            RaisePropertyChanged("tabIndex");
            MessageMediator.NotifyColleagues("ShowAddCertificate", WorkspaceId); //Send message to the MainViewModel
        }

        #endregion

        #region  Add User Certificate Command

        private RelayCommand _AddUserCertificateCommand;
        public RelayCommand AddUserCertificateCommand
        {

            get
            {

                if (_AddUserCertificateCommand == null)
                {
                    _AddUserCertificateCommand = new RelayCommand(ExecuteAddUserCertificateCommand, CanExecuteAddUserCertificateCommand);
                }
                return _AddUserCertificateCommand;
            }
        }

        private bool CanExecuteAddUserCertificateCommand()
        {
            if (Domain.IsPermitted("140"))
            {
                if (Hierarchy.IsClonedRelatedSplit == true || Hierarchy.IsClonedRelatedUpdate == true || Hierarchy.IsClonedRelated == true)
                    return false;
                else
                    return true;
            }
            else
                return false;

        }

        private void ExecuteAddUserCertificateCommand()
        {
            tabIndex = 3;
            RaisePropertyChanged("tabIndex");
            MessageMediator.NotifyColleagues("ShowAddUserCertificate", WorkspaceId); //Send message to the MainViewModel
        }

        #endregion

        #region Active Contents

        public int activeContentsCounter = 0;
        public void activeContentDataFiller()
        {
            ContentBLL bll = new ContentBLL(VM.VersionId);
            try
            {
                _activeContents = bll.getActiveContents();
                //_activeContents = _activeContents.OrderBy(x => x.seq).ToList();
                try
                {
                    activeContentsCounter = _activeContents.Count();
                }
                catch (Exception) { }
            }
            catch (Exception ex)
            {
                if (ex.Message == "CMDataError")
                {
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowHardCodedErrorMessage("CM API Data error. Please check Contents tree.");
                }
                else if (ex.Message == "144")
                {
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(144, new Object[] { 0 });
                }
                else
                {
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, new Object[] { 0 });
                }
            }
            
            
        }

        public IEnumerable<ContentModel> activeContents
        {
            get
            {
                if (cannotUpdateActiveContents && ContentFiler)
                {
                    if (_activeContents != null)
                    {
                        //Removed for template.
                        //_activeContents.ToList().Clear();
                        return _activeContents;
                    }
                    else
                        activeContentDataFiller();
                }
                return _activeContents;

            }

            set
            {
                _activeContents = value;
                RaisePropertyChanged("activeContents");
            }
        }

        #endregion

        #region Certificate Tab

        #region Data Filler

        public void certificateDataFiller()
        {
            try
            {
                CertificateBLL bll = new CertificateBLL(Hierarchy);
                _Certificates = bll.getAllCertificates();
            }
            catch (Exception e)
            {
                Object[] ArgsList = new Object[] { 0 };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(136, ArgsList);
            }
        }

        public IEnumerable<CertificateModel> 
            Certificates
        {
            get
            {

                return _Certificates;
            }

            set
            {
                _Certificates = value;
                RaisePropertyChanged("Certificates");
            }


        }

        #endregion DataFiller

        #region  Delete

        public CertificateModel selectedCertificate { get; set; }

        private RelayCommand _DeleteCertificateCommand;
        public ICommand DeleteCertificateCommand
        {
            get
            {
                if (_DeleteCertificateCommand == null)
                {
                    _DeleteCertificateCommand = new RelayCommand(ExecuteDeleteCertificateCommand, CanExecuteDeleteCertificateCommand);
                }
                return _DeleteCertificateCommand;
            }
        }
        private List<CertificateModel> certificateToRemove;
        private bool CanExecuteDeleteCertificateCommand()
        {
            if (Domain.IsPermitted("109"))
            {
                if (_Certificates.Count() > 0)
                {
                    if (selectedCertificate == null || Hierarchy.IsClonedRelatedSplit == true || Hierarchy.IsClonedRelatedUpdate == true || Hierarchy.IsClonedRelated == true)
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
            else
                return false; 
        }

        private void ExecuteDeleteCertificateCommand()
        {
            List<CertificateModel> cm = new List<CertificateModel>();
            certificateDataFiller();

            try
            {
                certificateRemove = true;
                Hierarchy.Certificates.RemoveAll(x => x == selectedCertificate.key);
                cm = _Certificates.Where(x => !x.key.Equals(selectedCertificate.key)).ToList();
                //List<CertificateModel> cm = _Certificates.ToList();

                    certificateToRemove.Add(selectedCertificate); //Add selected certificate to remove list  
                    foreach (CertificateModel c in certificateToRemove)
                        cm.RemoveAll(x => x.key == c.key); //Remove selected certificate from gui  

                Certificates = cm;
                RaisePropertyChanged("Certificates");
                MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                Hierarchy.IsDirty = true;
            }
            catch (Exception ex) 
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, new Object[] { 0 });
            }
        }


        private Boolean deleteCertificate()
        {
            if (certificateToRemove != (null))
            {
                //Check for security permission
                if (Domain.IsPermitted("109") || Domain.IsPermitted("999"))
                {
                    foreach (CertificateModel cm in certificateToRemove)
                    {
                        CertificateBLL bll = new CertificateBLL(Hierarchy, cm);
                        String qryResults = bll.ExecuteDeleteCertificateCommand();
                        if (qryResults.Equals("0"))
                        {
                            certificateDataFiller();

                        }
                        else if (qryResults.Equals("104"))
                        {
                            //showMessage(104);
                            Object[] ArgsList = new Object[] { 0 };
                            ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(104, ArgsList);
                            return false;
                        }
                        else if (qryResults.Equals("105"))
                        {
                            //showMessage(105);
                            Object[] ArgsList = new Object[] { 0 };
                            ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
                            return false;
                        }
                    }
                    RaisePropertyChanged("Certificates");
                    return true;
                }
                else
                { //Case user is not permited to remove cerificate
                    certificateToRemove = null;
                    return false;
                }
            }
            else
            {
                //Case certificate is null
                showMessage(105);
                return false;
            }
        }

        #endregion Delete

        #region Station Certificate Stations

        private RelayCommand _StationCertificateStationCommand;
        public ICommand StationCertificateStationCommand
        {
            get
            {
                if (_StationCertificateStationCommand == null)
                {
                    _StationCertificateStationCommand = new RelayCommand(ExecuteStationCertificateStationCommand, CanExecuteStationCertificateStationCommand);
                }
                return _StationCertificateStationCommand;
            }
        }

        private bool CanExecuteStationCertificateStationCommand()
        {
            if (_Certificates.Count() > 0 && selectedCertificate!=null)
            {
                if (Hierarchy.IsClonedRelatedSplit == true || Hierarchy.IsClonedRelatedUpdate == true || Hierarchy.IsClonedRelated == true)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        private void ExecuteStationCertificateStationCommand()
        {
            try
            {
                List<string> cerList = new List<string>();
                CertificateBLL.CertificateBLLReturnResult status = CertificateBLL.getStationCertificateStation(this.Certificates, out cerList);
                StationCertifiedListResults = "";

                //Internal error occurred. Please see Data Access log file for more details: Shell->View Log.
                if (status != CertificateBLL.CertificateBLLReturnResult.Success)
                {
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
                    return;
                }
                
                //None of workstations is associated to selected certificate.
                if (cerList.Count == 0)
                {
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(160, ArgsList);
                    return;
                }


                
                foreach (string s in cerList)
                {
                    StationCertifiedListResults += (s + "\n");
                }

                //Popup messege
                //MsgBoxService.ShowInformation(StationCertifiedListResults);
                MessageMediator.NotifyColleagues("ShowStationCertificateList", StationCertifiedListResults); 
            }
            catch (Exception ex) //Failed to get list of certified stations. Please see DAL Log file for more details: Shell --> View Log
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Object[] ArgsList = new Object[] { 0 };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(161, ArgsList);
            }
            
            
            }
        #endregion Station Certificate Stations

        #endregion Certificate Tab

        #region  IDropTarget Members & Other Drop Activities

        public Boolean ContentFiler = true;
        public void DragOver(Infra.DragDrop.IDropInfo DropInfo)
        {
            try
            {
                string SourceItemType = DropInfo.Data.GetType().ToString();
                string DropCollectionType = DropInfo.TargetCollection.GetType().ToString();

                //if source and target are differnt types do not alow drop 
                if (SourceItemType.Contains("CertificateModel") && DropCollectionType.Contains("UserCertificateApiModel"))
                    return;

                //if source and target are differnt types do not alow drop 
                if (DropCollectionType.Contains("CertificateModel") && SourceItemType.Contains("UserCertificateApiModel"))
                    return;

                if (SourceItemType.Contains("CertificateModel"))
                {
                    CertificateModel SourceItem = DropInfo.Data as CertificateModel;
                    if (SourceItem != null)
                    {
                        DropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                        DropInfo.Effects = DragDropEffects.Move;
                    }
                }
                //if (SourceItemType.Contains("CMTreeViewContentNodeViewModel"))
                if (SourceItemType.Contains("CMTreeViewVersionNodeViewModel"))
                {
                    //CMTreeViewContentNodeViewModel SourceItem = DropInfo.Data as CMTreeViewContentNodeViewModel;
                    CMTreeViewVersionNodeViewModel SourceItem = DropInfo.Data as CMTreeViewVersionNodeViewModel;
                    if (SourceItem != null)
                    {
                        DropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                        DropInfo.Effects = DragDropEffects.Move;
                    }
                }
                if (SourceItemType.Contains("UserCertificateApiModel"))
                {
                    //CMTreeViewContentNodeViewModel SourceItem = DropInfo.Data as CMTreeViewContentNodeViewModel;
                    UserCertificateApiModel SourceItem = DropInfo.Data as UserCertificateApiModel;
                    if (SourceItem != null)
                    {
                        DropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                        DropInfo.Effects = DragDropEffects.Move;
                    }
                }
                if (!Hierarchy.IsClonedRelated && (Hierarchy.GroupId <= 0 || Hierarchy.IsClonedRelatedUpdate)
                    && SourceItemType.Contains("ContentModel") && DropCollectionType.Contains("ContentModel"))
                {
                    ContentModel SourceItem = DropInfo.Data as ContentModel;
                    ContentModel DesItem = (ContentModel)DropInfo.TargetItem;
                    if (DesItem == null)
                        return;

                    if (SourceItem != null)
                    {
                        DropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                        DropInfo.Effects = DragDropEffects.Move;
                    }
                }
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                MsgBoxService.ShowError("Error:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public int VersionClosedId = 0;
        public void Drop(Infra.DragDrop.IDropInfo DropInfo)
        {
            try
            {
                //Identify dropped Entity Type
                string SourceItemType = DropInfo.Data.GetType().ToString();
                string DropCollectionType = DropInfo.TargetCollection.GetType().ToString();
                //If dropping Certificate, verify we drop on the right container and add to certificates list
                #region Certificate
                if (SourceItemType.Contains("CertificateModel") && DropCollectionType.Contains("CertificateModel"))
                {
                    CertificateModel SourceItem = DropInfo.Data as CertificateModel;
                    //verify that user is authorized to drop certificate
                    if (VerifyDragDropAuthorization() == false)
                        return;
                    //verify that dragged certificate is not already associated this folder
                    foreach (var c in Certificates)
                    {
                        if (c.key == SourceItem.key)
                        {
                            //showMessage(109);
                            Object[] ArgsList = new Object[] { 0 };
                            ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(109, ArgsList);

                            return;
                        }
                    }

                    var newInfo = new Certificate(SourceItem.CerName, SourceItem.Description);



                    _Certificates = _Certificates.Concat(new[] { new CertificateModel(newInfo, SourceItem.key, DateTime.Now.ToString()) });
              //    _Certificates.ToList().Add(new CertificateModel(newInfo, SourceItem.key));
                   // _Certificates = certificateDisplayList;
                    Hierarchy.Certificates.Add(SourceItem.key);

                    CertificateModel ItemToRemove = new CertificateModel();
                    ItemToRemove = certificateToRemove.FirstOrDefault(x => x.key == SourceItem.key);

                    certificateToRemove.Remove(ItemToRemove);
                    RaisePropertyChanged("Certificates");
                    isContentOrCertificateDroped = true;
                    Hierarchy.IsDirty = true;
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }
                #endregion
                //If dropping Content, verify we drop on the right container and add to content list
                if (SourceItemType.Contains("CMTreeViewVersionNodeViewModel") && DropCollectionType.Contains("ContentModel"))
                {
                    
                    allowExecute = false;
                    whoIsLinkedExistingContentId.Clear();
                    whoIsLinkedContentId.Clear();

                    CMTreeViewVersionNodeViewModel SourceItem = DropInfo.Data as CMTreeViewVersionNodeViewModel;
                    //MsgBoxService.ShowInformation("Perform logic of adding content " + SourceItem.Name);
                    Boolean Status = this.CheckContentValidations(ref SourceItem);
                    if (Status == false)
                    {
                        ContentModel DragCM = new ContentModel(contentNamedarg, SourceItem.Name, SourceItem.TreeNode.ID, DateTime.Now.ToString(), "", contentCategoryDesc);
                        ContentBLL bll = new ContentBLL(Hierarchy.VM.VersionId);
                        DragCM.status = ContentManagementViewModel.versions[SourceItem.TreeNode.ID].Status.Name;
                        // _activeContents = bll.UpdateContents(ref DragCM);
                        if (seqContentOverWriten > -1)
                        {
                            DragCM.seq = seqContentOverWriten;
                        }
                        Hierarchy.VM.Contents.Clear();
                        _activeContents = _activeContents.Concat(new[] { DragCM });
                        //_activeContents = _activeContents.OrderBy(x => x.seq);
                        foreach (var i in _activeContents)
                        {
                            Hierarchy.VM.Contents.Add(i);
                        }
                        int numOfContents = Hierarchy.VM.Contents.Count;
                        
                        //Only for new , Not for replace
                        if (numOfContents >= 2 && seqContentOverWriten == -1)
                        {
                            ContentModel c = Hierarchy.VM.Contents[numOfContents-1];
                            c.seq = Hierarchy.VM.Contents[numOfContents - 2].seq + 1;
                            Hierarchy.VM.Contents.RemoveAt(numOfContents - 1);
                            Hierarchy.VM.Contents.Add(c);
                        }
                        seqContentOverWriten = -1;
                        _activeContents = _activeContents.OrderBy(x => x.seq);
                        if (Hierarchy.VM.VersionName == InitialVersionName && !Hierarchy.IsCloned && !Hierarchy.IsClonedRelatedSplit)
                        {
                            //set name Red
                            LastVersionName = Hierarchy.VM.VersionName;
                            projectId = (int)Hierarchy.Id;
                            if (Hierarchy.GroupId > 0)
                            {
                                defaultVersionName = VersionBLL.GenerateDefaultVersionName((int)Hierarchy.GroupId);
                            }
                            else
                            {
                                defaultVersionName = VersionBLL.GenerateDefaultVersionName(projectId);
                            }

                            if (defaultVersionName != string.Empty)
                            {
                                this.VM.VersionName = defaultVersionName;
                            }
                            else //if failed to generate - current functionality
                            {
                                this.VM.VersionName = Hierarchy.VM.VersionName;
                            }
                            VM.Description = string.Empty;
                            //this.VM.VersionName = Hierarchy.VM.VersionName;
                            RaisePropertyChanged("VersionName");
                            this.VM.Description = Hierarchy.VM.Description;
                            RaisePropertyChanged("VersionDescription");
                            this.VM.DefaultTargetPathInd = true;
                            RaisePropertyChanged("DefaultTargetPathInd");
                            this.VM.TargetPath = getTargetPath();
                            RaisePropertyChanged("TargetPath");
                            this.VM.EcrId = string.Empty;
                            RaisePropertyChanged("EcrId");
                        }
                        ContentFiler = false;
                        RaisePropertyChanged("activeContents");

                        //Hierarchy.VM.Contents.Add(SourceItem.TreeNode.ID);
                        //First contents, later versions
                        ContentsKeys.Add(CNID, SourceItem.TreeNode.ID);
                        versionsExistsKeys.Add(SourceItem.TreeNode.ID);
                        Hierarchy.VM.IsClosed = true;
                        isContentOrCertificateDroped = true;
                        Hierarchy.VM.IsNew = true;
                        Hierarchy.VM.IsDirty = true;
                        Hierarchy.IsDirty = true;
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                    }
                }
                if (SourceItemType.Contains("UserCertificateApiModel") && DropCollectionType.Contains("UserCertificateApiModel"))
                {
                     UserCertificateApiModel SourceItem = DropInfo.Data as UserCertificateApiModel;
                    if (Domain.IsPermitted("141") || Domain.IsPermitted("999"))
                    {
                        foreach (var i in Hierarchy.UserCertificates)
                        {
                            if (i.UserCertificateId == SourceItem.UserCertificateId)
                            {
                                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(109, new Object[] { 0 });
                                return;
                            }

                        }
                        SourceItem.IsNew = true;
                        isContentOrCertificateDroped = true;
                        Hierarchy.UserCertificates.Add(SourceItem);

                        UserCertificateApiModel UCItemToRemove = new UserCertificateApiModel();
                        UCItemToRemove = UserCertificatesRemoved.FirstOrDefault(x => x.UserCertificateId == SourceItem.UserCertificateId);

                        UserCertificatesRemoved.Remove(UCItemToRemove);

                        RaisePropertyChanged("UserCertificates");
                        Hierarchy.IsDirty = true;
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                    }
                    else
                    {
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(106, ArgsList);
                    }
                }
                if (!Hierarchy.IsClonedRelated && SourceItemType.Contains("ContentModel"))
                {
                    ContentModel SourceItem = DropInfo.Data as ContentModel;
                    ContentModel DesItem = (ContentModel)DropInfo.TargetItem;

                    if (SourceItem != null && DesItem != null) //replace sequence between content versions
                    {
                        Dictionary<int, int> sortedContentSequences = new Dictionary<int, int>();
                        int idx = 0;
                        foreach (ContentModel con in activeContents)
                        {
                            sortedContentSequences.Add(idx,con.seq);
                            idx++;
                        }
                        cannotUpdateActiveContents = false;

                        List<ContentModel> activeContentsList = activeContents.ToList();
                        int destIndex = activeContentsList.FindIndex(x => x == DesItem);
                        activeContentsList.Remove(SourceItem);
                        activeContentsList.Insert(destIndex, SourceItem);

                        foreach (ContentModel con in activeContentsList)
                        {
                            con.seq = sortedContentSequences[activeContentsList.FindIndex(x => x == con)]; 
                        }                       

                        activeContents = activeContentsList;

                        //int sourceSec = SourceItem.seq;
                        //int desSec = DesItem.seq;

                        //cannotUpdateActiveContents = false;

                        //int t = activeContents.Where(x => x.id == contentToAction.id).First().seq;
                        //t++;
                        //foreach (ContentModel cm in activeContents)
                        //{
                        //    if (cm.id == SourceItem.id)
                        //    {
                        //        activeContents.Where(x => x.id == cm.id).First().seq = desSec;
                        //    }
                        //    if (cm.id == DesItem.id)
                        //    {
                        //        activeContents.Where(x => x.id == cm.id).First().seq = sourceSec;
                        //    }
                        //}
                        contentsUpdated = true;
                        Hierarchy.VM.IsNew = true;

                        activeContents = activeContents.OrderBy(x => x.seq).ToList();
                        if (Hierarchy.VM.VersionName == InitialVersionName && !Hierarchy.IsCloned && !Hierarchy.IsClonedRelatedSplit)
                        {
                            LastVersionName = Hierarchy.VM.VersionName;
                            projectId = (int)Hierarchy.Id;
                            if (Hierarchy.GroupId > 0)
                            {
                                defaultVersionName = VersionBLL.GenerateDefaultVersionName((int)Hierarchy.GroupId);
                            }
                            else
                            {
                                defaultVersionName = VersionBLL.GenerateDefaultVersionName(projectId);
                            }
                            if (defaultVersionName != string.Empty)
                            {
                                this.VM.VersionName = defaultVersionName;
                            }
                            else //if failed to generate - current functionality
                            {
                                this.VM.VersionName = Hierarchy.VM.VersionName;
                            }
                            VM.Description = string.Empty;
                            //this.Hierarchy.VM.VersionName = Hierarchy.VM.VersionName;
                            RaisePropertyChanged("VersionName");
                            this.Hierarchy.VM.Description = string.Empty;
                            RaisePropertyChanged("VersionDescription");
                            this.VM.EcrId = string.Empty;
                            RaisePropertyChanged("EcrId");
                        }
                        RaisePropertyChanged("activeContents");
                        Hierarchy.IsDirty = true;
                        Hierarchy.VM.IsDirty = true;
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                MsgBoxService.ShowError("Error:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        #endregion

        #region Verify that user is authorized to drop certificate

        
        private bool VerifyDragDropAuthorization()
        {
            try
            {
                if (Domain.IsPermitted("108") || Domain.IsPermitted("999"))
                {
                    return true;
                }
                else
                {
                    //showMessage(106);
                    Object[] ArgsList = new Object[] {0};
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(106, ArgsList);            

                    return false;
                }
            }
            catch (Exception ex)
            {
                //showMessage(105);
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Object[] ArgsList = new Object[] { ex.Message };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);   
            }

            return false;
        }

        #endregion

        #region Version Managment
    
        public VersionModel VM
        {
            get
            {
                return Hierarchy.VM;
            }
            set
            {
                CertificatesChanged = true;
                Hierarchy.VM = value;
            }
        }
        public class VersionNameAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                if (value == null)
                {
                    return true;
                }
                else
                {
                    if (value.ToString() == LastVersionName )
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public class VersionInvalidNameAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                if (value == null)
                {
                    return true;
                }
                if (value.ToString().Contains("/") || value.ToString().Contains("/") || value.ToString().Contains("\\")
                                                     || value.ToString().Contains("*") || value.ToString().Contains("?")
                                                     || value.ToString().Contains("\"") || value.ToString().Contains("<")
                                                     || value.ToString().Contains(">") || value.ToString().Contains("|")
                                                      || value.ToString().Contains("\t") || value.ToString().Contains(Environment.NewLine))
                {
                    return false;
                }

                return true;
            }
        }


        [VersionName(ErrorMessage = "Please update Version Name")]
        [Required(ErrorMessage = "'Version Name' field is required."), StringLength(50, ErrorMessage = "Maximum length (50 characters) exceeded.")]
        [VersionInvalidName(ErrorMessage = "Version name can't contain any of the following characters: \\ / * ? \" < > |, tab, new line")]
        public string VersionName
        {
            get
            {
                if (Hierarchy != null )
                {
                    return Hierarchy.VM.VersionName;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                CertificatesChanged = true;
                if (Hierarchy != null)
                {
                    Hierarchy.VM.VersionName = value;
                    RaisePropertyChanged("VersionName");
                    RaisePropertyChanged("TargetPath");
                    if (VM.DefaultTargetPathInd == true)
                    {
                        Hierarchy.VM.TargetPath = getTargetPath();
                        RaisePropertyChanged("TargetPath");
                    }
                    Hierarchy.IsDirty = true;
                    Hierarchy.VM.IsDirty = true;
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }

            }
        }


        [Required(ErrorMessage = "'Version Description' field is required."), StringLength(50, ErrorMessage = "Maximum length (50 characters) exceeded.")]
        public string VersionDescription
        {
            get
            {
                if (Hierarchy != null)
                {
                    return Hierarchy.VM.Description;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                CertificatesChanged = true;
                
                if (Hierarchy != null)
                {
                    
                    Hierarchy.VM.Description = value;
                    RaisePropertyChanged("VersionDescription");
                    Hierarchy.VM.IsDirty = true;
                    Hierarchy.IsDirty = true;
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }

            }
        }
        public long VersionId
        {
            get
            {
                if (Hierarchy != null && Hierarchy.VM.VersionId != -1)
                {
                    return Hierarchy.VM.VersionId;
                }
                else
                {
                    return 0;
                }
            }
        }

        private ObservableCollection<VersionModel> _GetAllVersions;
        public ObservableCollection<VersionModel> GetAllVersions
        {
            get
            {
                if (_Hierarchy != null)
                {
                    return _Hierarchy.GetAllVersions;
                }
                else
                {
                    return null;
                }
                
            }
       }
         [Required(ErrorMessage = "'Target Path' field is required."), StringLength(1000, ErrorMessage = "Maximum length (1000 characters) exceeded.")]
        public string TargetPath
        {
            get
            {
                if (Hierarchy != null)
                {
                    
                    if (_Hierarchy.IsNew == true)
                    {
                        Hierarchy.VM.TargetPath = getTargetPath();
                    }
                    return Hierarchy.VM.TargetPath;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                CertificatesChanged = true;
                if (Hierarchy != null)
                {
                    Hierarchy.VM.TargetPath = value;
                    RaisePropertyChanged("TargetPath");
                    Hierarchy.VM.IsDirty = true;
                    Hierarchy.IsDirty = true;
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }

            }
        }

        public Boolean DefaultTargetPathInd
        {

            get
            {
                return VM.DefaultTargetPathInd;
            }
            set
            {
                if (Hierarchy != null)
                {
                    CertificatesChanged = true;
                    VM.DefaultTargetPathInd = value;
                    RaisePropertyChanged("DefaultTargetPathInd");
                    RaisePropertyChanged("EnableSync");
                    RaisePropertyChanged("EnableSyncNot");
                    RaisePropertyChanged("TargetPath");
                    if (VM.DefaultTargetPathInd == true)
                    {
                        Hierarchy.VM.TargetPath = getTargetPath();
                        RaisePropertyChanged("TargetPath");
                    }
                    Hierarchy.VM.IsDirty = true;
                    Hierarchy.IsDirty = true;
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }
            }
        }

        public Boolean EnableSync
        {

            get
            {
                return !VM.DefaultTargetPathInd;
            }
 
        }

        public Boolean EnableSyncNot
        {

            get
            {
                return !EnableSync;
            }

        }


        public  string getTargetPath()
        {
            var Target = new StringBuilder(string.Empty);
            try
            {

                var SysPathQry = new StringBuilder(string.Empty);
                if (Hierarchy.IsClonedRelated == false)
                {
                    SysPathQry.Append("select Value from PE_SystemParameters where Variable='ProjectLocalPath'");
                }
                if (Hierarchy.IsClonedRelated == true)
                {
                    SysPathQry.Append("select Value from PE_SystemParameters where Variable='RelatedProjectLocalPath'");
                }
                string SysParm = (string)Domain.PersistenceLayer.FetchDataValue(SysPathQry.ToString(), System.Data.CommandType.Text, null);

                 Target = new StringBuilder(string.Empty);
                Target.Append(SysParm.ToString().Trim());
                if (_Hierarchy.ParentId != -1 || _Hierarchy.ParentId != null)
                {
                    
                    
                    Target.Append(VersionBLL.getParentName(_Hierarchy.ParentId.ToString().Trim()));
                }

            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", ex); // TODO: Log error
                return ex.Message;
            }

            if ((Hierarchy.GroupId == -1 && Hierarchy.IsClonedRelated == false) || Hierarchy.IsClonedRelatedSplit == true)
            {
                Target.Append("/" + Hierarchy.Name.ToString().Trim() + "/" + VM.VersionName.ToString().Trim());
            }
            else
            {
                Target.Append("/" + Hierarchy.GroupName.ToString().Trim() + "/" + VM.VersionName.ToString().Trim());
            }
            return Target.ToString();

        }

        private IFolderBrowserService FolderBrowserDialogService = null;

        private RelayCommand _SelectDirectoryCommand;
        public ICommand SelectDirectoryCommand
        {
            get
            {
                if (_SelectDirectoryCommand == null)
                {
                    _SelectDirectoryCommand = new RelayCommand(ExecuteSelectDirectoryCommand, CanExecuteSelectDirectoryCommand);
                }
                return _SelectDirectoryCommand;
            }
        }

        private bool CanExecuteSelectDirectoryCommand()
        {
            return Domain.IsPermitted("127");
        }

        private void ExecuteSelectDirectoryCommand()
        {
            string TempTargret = FolderBrowserDialogService.ShowDialog();
            if (!String.IsNullOrEmpty(TempTargret))
            {
                Hierarchy.VM.TargetPath = TempTargret;
               // TempTargret = FolderBrowserDialogService.ShowDialog();
                RaisePropertyChanged("TargetPath");
            }
        }

        #endregion Version Managment

        #region Open Content Version In Cm

        private RelayCommand _OpenContentVersionInCm;
        public ICommand OpenContentVersionInCm
        {
            get
            {
                if (_OpenContentVersionInCm == null)
                {
                    _OpenContentVersionInCm = new RelayCommand(ExecuteOpenContentVersionInCm, CanExecuteOpenContentVersionInCm);
                }
                return _OpenContentVersionInCm;
            }
        }

        private bool CanExecuteOpenContentVersionInCm()
        {
            return true;
        }

        private void ExecuteOpenContentVersionInCm()
        {
            MessageMediator.NotifyColleagues("RequestGotoCm", this.contentToAction); //Send message to the MainViewModel to clear Statusbar from any previous operation
        }

        #endregion

        #region Content Execution

        #region Relay Command
        Dictionary<String, String> certificates = new Dictionary<string, string>();
        private RelayCommand _ContentCopyCommand;
        private bool allowExecute = true;

        public ICommand ContentCopyCommand
        {
            get
            {
                if (_ContentCopyCommand == null)
                {
                    _ContentCopyCommand = new RelayCommand(ExecuteContentCopyCommand, CanExecuteContentCopyCommand);
                }
                return _ContentCopyCommand;
            }
        }
        public ContentModel contentToAction { set; get; }

        private bool CanExecuteContentCopyCommand()
        {
            //Check if version is saved before enabling execute
            if (allowExecute && !Hierarchy.IsCloned && !Hierarchy.IsClonedRelated && !Hierarchy.IsClonedRelatedSplit && !Hierarchy.IsClonedRelatedUpdate && contentToAction is ContentModel)
                return true;
            else
                return false;
        }

        private void ExecuteContentCopyCommand()
        {
            MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
            ContentExecutionBLL.ErrorHandling luStatus = new ContentExecutionBLL.ErrorHandling();
            //Last Update and permission check
            luStatus = ContentExecutionBLL.PriorValidations(_Hierarchy, "112");
            if (!(String.IsNullOrEmpty(luStatus.errorId)))
            {
                MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(luStatus.errorId), luStatus.errorParams);
                return;
            }
            Thread ContentExecutionThread = new Thread(new ThreadStart(ContentExecution));
            ContentExecutionThread.Start();
            certificates = new Dictionary<string, string>();
        }

        public bool IsExecuteEnabled
        {
            get
            {
                if (allowExecute && !Hierarchy.IsCloned &&
                    !Hierarchy.IsClonedRelated &&
                    !Hierarchy.IsClonedRelatedSplit &&
                    !Hierarchy.IsClonedRelatedUpdate
                    && contentToAction is ContentModel)
                {
                    return true;
                }
                else
                {
                    return false;
                }
 
            }
        }

        #endregion

        #region Main content excecute command

        private void ContentExecution()
        {
            ProgressText = "Performing required validations prior to copying files...";
            totalProgress = 0;
            int errorId = -1;
            ContentExecutionBLL.ErrorHandling Status = new ContentExecutionBLL.ErrorHandling();

            try
            {
                #region Get CM Sub tree
                //Get contents, folder and version tree from API
                Dictionary<int, CMFolderModel> outFolders = new Dictionary<int, CMFolderModel>();
                Dictionary<int, CMContentModel> outContents = new Dictionary<int, CMContentModel>();
                Dictionary<int, CMVersionModel> outVersions = new Dictionary<int, CMVersionModel>();

                //Performance
                //Get CM sub tree
                Status = ContentExecutionBLL.GetCMSubTree((ObservableCollection<ContentModel>)_activeContents, out outFolders, out outContents, out outVersions);
                #region Handle Get CM sub tree errors if any
                if (Status.errorId != string.Empty)
                {
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Status.errorId), Status.errorParams);
                    return;
                }
                #endregion

                #endregion

                #region Perform all required validations prior to execution
                Status = ContentExecutionBLL.validations(outContents, outVersions, contentToAction, Hierarchy);
                #region Handle validations errors if any
                if (Status.errorId != string.Empty)
                {
                    if (Status.errorId == "107")
                    {
                        if (MsgBoxService.ShowOkCancel(Status.errorParams[0].ToString(), DialogIcons.Question) != DialogResults.OK)
                        {
                            return;
                        }
                        else
                        {
                            bool temp = ATSDomain.Domain.IsPermitted("126");
                            if (Status.errorId == "107" && !Domain.IsPermitted("126"))
                            {
                                ProjectsExplorerViewModel.ShowErrorAndInfoMessage(106, new object[] { 0 });
                                return;
                            }
                        }
                    }
                    else
                    {
                        ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Status.errorId), new object[] { 0 });
                        return;
                    }
                }
                #endregion

                Status = ContentExecutionBLL.userCertificatesValidations(Hierarchy);
                #region Handle validations errors if any
                if (Status.errorId != string.Empty)
                {
                    if (Status.errorId == "170")
                    {
                        if (MsgBoxService.ShowOkCancel(Status.errorParams[0].ToString(), DialogIcons.Question) != DialogResults.OK)
                        {
                            return;
                        }
                        else
                        {
                            bool temp = ATSDomain.Domain.IsPermitted("126");
                            if (Status.errorId == "170" && !Domain.IsPermitted("133"))
                            {
                                ProjectsExplorerViewModel.ShowErrorAndInfoMessage(106, new object[] { 0 });
                                return;
                            }
                        }
                    }
                    else
                    {
                        ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Status.errorId), new object[] { 0 });
                        return;
                    }
                }
                #endregion

                #endregion

                #region Start new thread for copying files and copy files to local directory
            ThreadStart threadStart = new ThreadStart(IncrementProgressCounter);
            Thread t = new Thread(threadStart);
            t.Start();
                Status = ContentExecutionBLL.prepareLocalDirectoryToExecution(outVersions, outContents, threadStart,
                                                                                 (ObservableCollection<ContentModel>)_activeContents,
                                                                                 contentToAction,
                                                                                 Hierarchy);
                #region Handle files copy errors if any
                if (Status.errorId != string.Empty)
                {
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Status.errorId), Status.errorParams);
                    return;
        }

        #endregion

                #endregion

                #region Execute content version command line
                IncrementProgressCounter(10);

                HierarchyModel updatedHierarchy = Hierarchy;
                updatedHierarchy.VM.Contents = (ObservableCollection<ContentModel>)_activeContents;
                Status = ContentExecutionBLL.ExecuteContentVersionAndSaveInfo(updatedHierarchy, outVersions, outContents, contentToAction.id);
                #region Handle execution errors if any
                if (Status.errorId != string.Empty)
                {
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Status.errorId), Status.errorParams);
                    ProgressText = "0 %";
                    RaisePropertyChanged("ProgressText");
                    return;
                }
                #endregion

                #endregion

                #region     Save Execution History
                Status = ContentExecutionBLL.recordExecutionHistory(Hierarchy.VM.VersionId);
                #region Handle save history errors if any

                if (Status.errorId != string.Empty)
                {
                    String logMessage = "Failed to save record in ExecutionHistory table";
                    Domain.SaveGeneralErrorLog(logMessage);
                }
                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, new object[] { 0 });
            }
        }

        #endregion Main content excecute command

        #region Progress Bar

        #region Fields

        // Property variables
        private float p_Progress = 0;
        private int p_ProgressMax = 10;
        private String p_ProgressText = "";
        private float totalProgress = 0f;
        #endregion

        #region Data Properties

        // The progress of an image processing job.
        // The setter for this property also sets the ProgressMessage property.
        public float Progress
        {
            get { return p_Progress; }

            set
            {
                RaisePropertyChanged("Progress");
                p_Progress = value;
                RaisePropertyChanged("Progress");
            }
        }

        public String ProgressText
        {
            get { return p_ProgressText; }

            set
            {
                RaisePropertyChanged("ProgressText");
                p_ProgressText = value;
                RaisePropertyChanged("ProgressText");
            }
        }

        //The maximum progress value.
        public int ProgressMax
        {
            get { return p_ProgressMax; }

            set
            {
                RaisePropertyChanged("ProgressMax");
                p_ProgressMax = value;
                RaisePropertyChanged("ProgressMax");
            }
        }


        #endregion

        #region Internal Methods


        //Clears the view model.
        internal void ClearViewModel()
        {
            p_Progress = 0;
            p_ProgressMax = 100;
            IncrementProgressCounter(10);
            FileSystemBLL.filesCompleted = 0;
        }

        // Advances the progress counter for the Progress dialog.
        public void IncrementProgressCounter(float incrementClicks)
        {
            incrementClicks = (int)(incrementClicks * 10);

            totalProgress = incrementClicks;
            if (totalProgress > 99)
            {
                totalProgress = 100;
                Progress = p_ProgressMax;
                ProgressText = "100 %";
                FileSystemBLL.filesCompleted = 0;
            }
            else if (totalProgress < 0)
            {
                totalProgress = 0;

                ProgressText = 0 + "%";
            }
            else
            {
                // Increment counter
                this.Progress = incrementClicks / 10;
                ProgressText = totalProgress + "%";
            }

            // Update progress message
            float progress = (p_Progress);
            float progressMax = (p_ProgressMax);
            float f = (progress / progressMax);
            float percentComplete = Single.IsNaN(f) ? 0 : (f);
        }

        public void IncrementProgressCounter()
        {
            if (FileSystemBLL.filesToCopyNumber != 0)
            {
                float upPrecent = (((float)FileSystemBLL.filesCompleted / ((float)FileSystemBLL.filesToCopyNumber))) * 10;
                IncrementProgressCounter(upPrecent);
            }
            else
            {
                IncrementProgressCounter(10);
            }
        }

        #endregion Intenal progress bar methods

        #endregion Progress bar

        #region Other View Models

        private void OnProgressTextReceived(TreeViewProjectNodeViewModel TP)
        {
            this.ProgressText = TP.ProgressText;
            RaisePropertyChanged("ProgressText");
            this.Progress = TP.Progress;
            RaisePropertyChanged("Progress");
        }

        private void OnContentsTabProgressTextReceived(CMTreeViewVersionNodeViewModel CMV)
        {
            this.ProgressText = CMV.ProgressText;
            RaisePropertyChanged("ProgressText");
            this.Progress = CMV.Progress;
            RaisePropertyChanged("Progress");
        }

        private void OnRefreshNotesReceived(long hierarchyId)
        {
            Notes = new NotesControlViewModel(hierarchyId, this.WorkspaceId);
            RaisePropertyChanged("Notes");
        }

        private void OnRapidExecutionStartReceived(TreeViewProjectNodeViewModel TP)
        {
            ProgressText = "Performing required validations prior to copying files...";
        }

        private void OnRapidExecutionReceived(TreeViewProjectNodeViewModel TP)
        {

            ThreadStart threadStart = new ThreadStart(IncrementProgressCounter);
            Thread t = new Thread(threadStart);
            t.Start();
        }

        private void OnLinkedVersionExecutionStartReceived(CMTreeViewVersionNodeViewModel CM)
        {
            ProgressText = "Performing required validations prior to copying files...";
            RaisePropertyChanged("ProgressText");
        }

        private void OnLinkedVersionExecutionReceived(CMTreeViewVersionNodeViewModel CM)
        {

            ThreadStart threadStart = new ThreadStart(IncrementProgressCounter);
            Thread t = new Thread(threadStart);
            t.Start();
        }

        #endregion other VMs

        #endregion Content Execution

        private void OnClearViewModelReceived(TreeViewProjectNodeViewModel TP)
        {
            ClearViewModel();
        }

        private void OnClearVersionViewModelReceived(TreeViewVersionNodeViewModel TV)
        {
            ClearViewModel();
        }

        private void OnDisableProjectReceived(TreeViewProjectNodeViewModel TP)
        {
            bool showNotesSideBar = true;

            bool isCloneRelated = false;

            Notes = new NotesControlViewModel(Hierarchy.Id, Hierarchy.Name, ref isCloneRelated, ref showNotesSideBar, this.WorkspaceId);
        }

        #region Content Remove

        #region Data
        private Boolean contentsUpdated { get; set; }
        private int seqContentOverWriten = -1;
        
        private VersionModel VMOld;
        private VersionModel VMnew;
        #endregion Data

        #region Relay Command
        private RelayCommand _ContentRemoveCommand;
        public ICommand ContentRemoveCommand
        {
            get
            {
                if (_ContentRemoveCommand == null)
                {
                    _ContentRemoveCommand = new RelayCommand(ExecuteContentRemoveCommand, CanExecuteContentRemoveCommand);
                }
                return _ContentRemoveCommand;
            }
        }

        private bool CanExecuteContentRemoveCommand()
        {

            if (Domain.IsPermitted("106") || Domain.IsPermitted("999")) //Check for premission
            {
                if (contentToAction is ContentModel)
                {
                    if (Hierarchy.GroupId != -1) //Check for related project
                    {
                        if (Hierarchy.IsClonedRelatedSplit == true || Hierarchy.IsClonedRelatedUpdate == true)
                            return true;
                        else
                            return false;
                    }

                    else if (Hierarchy.IsClonedRelated == true)
                        return false;
                    else
                        return true;
                }
                else
                    return false;


            }
            else
                return false;

        }

        private void ExecuteContentRemoveCommand()
        {
            MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
            VerifyAuthorization();
        }
        #endregion

        #region Verify that user is authorized to remove contents
        private void VerifyAuthorization()
        {
            try
            {
                if (Domain.IsPermitted("106") || Domain.IsPermitted("999")) //Check for premittion
                {
                    ValidateAndRemove(); //Check for related project
                }
                else
                {
                    //showMessage(106);
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(106, ArgsList);
                }
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                showMessage(105);
            }

        }
        #endregion

        #region Check whether selected project is a Regular project or Related (Multi MAKAT):
        private void ValidateAndRemove()
        {
            #region Varibales
            var SB = new StringBuilder(string.Empty);
            #endregion

            SB.Append("Select GroupId from dbo.PE_Hierarchy Where id='" + Hierarchy.Id.ToString() + "';");
            string QrySteps = SB.ToString();

            //      if ((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)) is System.DBNull)
            //     {

            //Get all the project active contents
            List<ContentModel> cm = _activeContents.ToList();
            cm.Remove(contentToAction); //Remove selected content
            int ContentID = 0;
            if (ContentsKeys != null && ContentsKeys.Count > 0)
            {

                ContentID = ContentManagementViewModel.versions[contentToAction.id].ParentID;

                //foreach (var i in ContentsKeys)
                //{
                    //if (ContentID == i.Key)
                     if (ContentsKeys.ContainsKey(ContentID))
                    {
                        ContentsKeys.Remove(ContentID);
                        versionsExistsKeys.Remove(contentToAction.id);
                        deleteFromContentLinksVersions(contentToAction.id, ContentID);
                       
                    }
                //}
            }

            //Refresh GUI after content removed
            _activeContents = cm;
            cannotUpdateActiveContents = false;
            CertificatesChanged = true;
            RaisePropertyChanged("activeContents");
            contentsUpdated = true;
            if (Hierarchy.VM.VersionName == InitialVersionName && !Hierarchy.IsCloned && !Hierarchy.IsClonedRelatedSplit)
            {
                LastVersionName = Hierarchy.VM.VersionName;
                projectId = (int)Hierarchy.Id;
                VMOld = Hierarchy.VM;
                if (Hierarchy.GroupId > 0)
                {
                    defaultVersionName = VersionBLL.GenerateDefaultVersionName((int)Hierarchy.GroupId);
                }
                else
                {
                    defaultVersionName = VersionBLL.GenerateDefaultVersionName(projectId);
                }
                if (defaultVersionName != string.Empty)
                {
                    this.VM.VersionName = defaultVersionName;
                }
                else //if failed to generate - current functionality
                {
                    this.VM.VersionName = Hierarchy.VM.VersionName;
                }
                VM.Description = string.Empty;
                //this.VM.VersionName = Hierarchy.VM.VersionName;
                RaisePropertyChanged("VersionName");
                this.VM.Description = string.Empty;
                RaisePropertyChanged("VersionDescription");
                this.VM.DefaultTargetPathInd = true;
                RaisePropertyChanged("DefaultTargetPathInd");
                this.VM.TargetPath = getTargetPath();
                RaisePropertyChanged("TargetPath");
                this.VM.EcrId = string.Empty;
                RaisePropertyChanged("EcrId");
            }

            Hierarchy.VM.IsDirty = true;
            Hierarchy.VM.IsNew = true;
            Hierarchy.IsDirty = true;
            MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
            //The rest of this feature is in the SAVE Button command


        }

        public void deleteFromContentLinksVersions(int versionId, int ContentID)
        {
            if (ContentLinksKeys.Count > 0)
            {
                ContentLinksKeys.Remove(ContentID);
                foreach (var i in ContentManagementViewModel.versions[versionId].ContentVersions)
                {

                    deleteFromContentLinksVersions(i.Key, ContentManagementViewModel.versions[i.Key].ParentID);
                }
            }
        }
        #endregion

        //Next methods related to action after pressing save button:

        

        #endregion

        #region Show message
        private void showMessage(int error)
        {
            Collection<string> StatusBarParameters = new Collection<string>();
            var SB = new StringBuilder(string.Empty);
            SB.Append("SELECT Description FROM PE_Messages where id=" + error + ";");
            string QrySteps = SB.ToString();
            // Fetch the DataTable from the database
            StatusBarParameters.Add((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString()); //Message
            StatusBarParameters.Add("White"); //Foreground
            StatusBarParameters.Add("Red"); //Background
            MessageMediator.NotifyColleagues("StatusBarParameters", StatusBarParameters);
        }

        #endregion

        #region getSeqence

        private void getSequence()
        {
            if (Hierarchy.IsNew == true)
            {
                var SB = new StringBuilder(string.Empty);
                SB.Append("select count(Id) + 1 from PE_Hierarchy WHERE ParentId = '" + Hierarchy.ParentId + "'");
                Object CountOBj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null);
                if (CountOBj != null)
                {
                    Int16 Count = Convert.ToInt16(CountOBj);
                    Hierarchy.Sequence = Count;
                }
            }
        }

        #endregion getSeqence

        #region getActiveVersionName
        public void getActiveVersionName(){
          
            if (Hierarchy.VM != null)
            {
                Hierarchy.ActiveVersion = Hierarchy.VM.VersionName;
            }

        }
        #endregion getActiveVersionName

        #region Content D&D Validations

        private List<CMTreeNode> allContents;
        private Dictionary<int, int> ContentsKeys = new Dictionary<int, int>();
        private List<int> ContentsDragKeys = new List<int>();
        private List<int> versionsExistsKeys = new List<int>();
        private List<int> ContentLinksKeys = new List<int>();
        //private List<CMTreeViewVersionNodeViewModel> DragTree = new List<CMTreeViewVersionNodeViewModel>();

        //EG for error message 122
        private Dictionary<int, int> whoIsLinkedContentId = new Dictionary<int, int>();
        private Dictionary<int, int> whoIsLinkedExistingContentId = new Dictionary<int, int>();


        public Boolean retStatus = false;
        public Boolean CheckContentValidations(ref CMTreeViewVersionNodeViewModel contentTree)
        {
            try
            {
                if ((Hierarchy.GroupId == -1) || (Hierarchy.GroupId != -1 && (Hierarchy.IsClonedRelatedUpdate == true || Hierarchy.IsClonedRelatedSplit == true)))
                {
                    if (Domain.IsPermitted("106") || Domain.IsPermitted("999"))
                    {
                       // ContentManagementViewModel.contents
                       // contentManagerApi = new ContentManagerApiProvider("Test", "Test", "Content manager provider", Domain.DbConnString);
                      //  allContents = contentManagerApi.GetTreeObjects(out folders, out contents, out versions);
                        
                        //foreach (var CV in ContentManagementViewModel.versions)
                       // {
                            //if (CV.Key == contentTree.TreeNode.ID)
                           // {

                                //Check if content is retired.
                                if (ContentManagementViewModel.versions[contentTree.TreeNode.ID].Status.ID.Trim() == "Ret")
                                {
                                    if ((Domain.IsPermitted("107") || Domain.IsPermitted("999")))
                                    {
                                        var SB = new StringBuilder(string.Empty);
                                        SB.Append("SELECT Description FROM PE_Messages where id=120");
                                        object SuccessObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null);
                                        if (SuccessObj != null)
                                        {
                                            string Success = (SuccessObj).ToString();

                                            object[] ArgList = { contentTree.Name };
                                            string SuccessName = String.Format(Success, ArgList);
                                            if (MsgBoxService.ShowOkCancel(SuccessName.ToString(), DialogIcons.Question) == DialogResults.Cancel)
                                            {
                                                return true;
                                            }
                                        }

                                    }
                                    else
                                    {
                                        //showMessage(126);
                                        Object[] ArgsList = new Object[] { 0 };
                                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(126, ArgsList);
                                        return true;
                                    }

                                }
                              
                           // }

                        //}//end foreach
                        Boolean GetContentsIND = GetContentsID(ref contentTree);
                        if (GetContentsIND == true)
                        {
                            return true;
                        }

                    }//not permitted
                    else
                        return true;

                }// not allowed d&d
                else
                    return true; 
                return false;
            }
            catch (Exception ex)
            {

                //showMessage(105);
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return false;
            }
        }

        string contentCategoryDesc = " ";

        string contentNamedarg = " ";
        int CNID = 0;

        public Boolean GetContentsID(ref CMTreeViewVersionNodeViewModel contentTree)
        {
            //get the content Id of the drag content version
             int contentIDdarg = 0;


            //get the content Id of the drag content version

            //foreach (var CN in contents)
            //{
            //    foreach (var V in CN.Value.Versions.Keys)
            //    {
            //        if (V == contentTree.TreeNode.ID)
            //        {
            //            CNID = CN.Key;
            //            contentNamedarg = CN.Value.Name;
            //            break;
            //        }
            //    }
            //}

             CNID = ContentManagementViewModel.versions[contentTree.TreeNode.ID].ParentID;
            contentNamedarg = ContentManagementViewModel.contents[CNID].Name;
            contentCategoryDesc = ContentManagementViewModel.contents[CNID].ContentType.Name;
            //get the contents ID of this versions
            var SBCon = new StringBuilder(string.Empty);
            SBCon.Append("Select ContentVersionId from dbo.PE_VersionContent avc Join dbo.PE_Version av " +
            " on av.versionId = avc.VersionId  where av.HierarchyId = ");
            if(Hierarchy.GroupId == -1 )
            {
                SBCon.Append(" '" + Hierarchy.Id + "' ");
            }
            else
                SBCon.Append(" '" + Hierarchy.GroupId + "' ");
            SBCon.Append(" and av.VersionStatus='A' and av.versionId ='" + Hierarchy.VM.VersionId + "'");
            DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(SBCon.ToString(), CommandType.Text, null);

            if (ResTable != null)
            {
                foreach (DataRow DataRow in ResTable.Rows)
                {
                    int CVersionID = (int)DataRow["ContentVersionId"];
                    int DBverParentId = ContentManagementViewModel.versions[CVersionID].ParentID;                   
                    foreach (var A in activeContents)
                    {
                        if (A.id == CVersionID)
                        {
                            if (!ContentsKeys.ContainsKey(DBverParentId))
                            {
                                //First content, later versions.
                                ContentsKeys.Add(DBverParentId, CVersionID);
                                versionsExistsKeys.Add(CVersionID);
                                //checkLinked(V.Value.ContentVersions, versionsExistsKeys);
                            }
                        }
                    }
                }

                if (versionsExistsKeys != null)
                {
                    foreach (var i in versionsExistsKeys)
                    {
                        if (i == contentTree.TreeNode.ID)
                        {
                            Object[] ArgsList = new Object[] { 0 };
                            ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(108, ArgsList);
                            return true;
                        }
                    }
                }




                checkLinkedVersions(ref contentTree);
                getAllContentsExistsVersions();
                Boolean compareLinksInd = compareLinks();
                if (compareLinksInd == true)
                {
                    return true;
                }

                if (ContentsKeys != null)
                {
                    //check if there is same content.
                    //foreach (var i in ContentsKeys)
                    //{
                    if (ContentsKeys.ContainsKey(CNID))
                    {
                        var TB = new StringBuilder(string.Empty);
                        TB.Append("SELECT Description FROM PE_Messages where id=156");
                        object SuccessObj = Domain.PersistenceLayer.FetchDataValue(TB.ToString(), CommandType.Text, null);
                        if (SuccessObj != null)
                        {
                            string Success = (SuccessObj).ToString();

                            object[] ArgList = { ContentManagementViewModel.versions[contentTree.TreeNode.ID].Name, ContentManagementViewModel.versions[ContentsKeys[CNID]].Name };
                            string SuccessName = String.Format(Success, ArgList);


                            DialogResults ReplaceAnswer = MsgBoxService.ShowOkCancel(SuccessName, DialogIcons.Question);
                            if (ReplaceAnswer == DialogResults.Cancel)
                            {
                                return true;
                            }
                            else
                            {
                                ContentModel RemovedContent = null;
                                List<ContentModel> cm = _activeContents.ToList();
                                foreach (var content in cm)
                                {
                                    if (ContentsKeys[CNID] == content.id)
                                    {
                                        RemovedContent = content;
                                        cm.Remove(content); //Remove selected content
                                        break;
                                    }
                                }
                                int ContentID = 0;
                                if (ContentsKeys != null && ContentsKeys.Count > 0)
                                {
                                    ContentID = ContentManagementViewModel.versions[RemovedContent.id].ParentID;

                                    //foreach (var j in ContentsKeys)
                                    //{
                                    //if (ContentID == j.Key)
                                    if (ContentsKeys.ContainsKey(ContentID))
                                    {
                                        ContentsKeys.Remove(ContentID);
                                        if (versionsExistsKeys != null && versionsExistsKeys.Count > 0)
                                            versionsExistsKeys.Remove(RemovedContent.id);
                                        deleteFromContentLinksVersions(RemovedContent.id, ContentID);

                                    }
                                    //}
                                }

                                //Refresh GUI after content removed
                                _activeContents = cm;
                                cannotUpdateActiveContents = false;
                                CertificatesChanged = true;
                                RaisePropertyChanged("activeContents");
                                contentsUpdated = true;
                                seqContentOverWriten = RemovedContent.seq;
                            }
                        }
                        else
                            return true;
                    }
                    //}
                }
            }
            return false;
        }

        //get all contents versions of the drag version
        //public void checkLinkedVersions(ref CMTreeViewVersionNodeViewModel contentTree)
        //{
        //    ContentsDragKeys.Clear();
        //    foreach (var v in versions)
        //    {
        //        if (v.Key == contentTree.TreeNode.ID)
        //        {
        //            ContentsDragKeys.Add(v.Value.ParentID);
        //            checkLinked(v.Value.ContentVersions, ContentsDragKeys);
        //            break;

        //        }
        //    }
        //}

        public void checkLinkedVersions(ref CMTreeViewVersionNodeViewModel contentTree)
        {
            ContentsDragKeys.Clear();
            //foreach (var v in versions)
            //{
            //    if (v.Key == contentTree.TreeNode.ID)
            //    {
                    int conParentId = ContentManagementViewModel.versions[contentTree.TreeNode.ID].ParentID;

                    ContentsDragKeys.Add(conParentId);
                    whoIsLinkedContentId[conParentId] = conParentId;
                    checkLinked(ContentManagementViewModel.versions[contentTree.TreeNode.ID].ContentVersions, ContentsDragKeys, whoIsLinkedContentId);
                   
            //    }
            //}
        }


        // get all the versions links.
        public void checkLinked(Dictionary<int, CMContentVersionSubVersionModel> ContentVersions, List<int> selectedlist, Dictionary<int, int> whoIsLinked)
        {
            foreach (var V in ContentVersions)
            {
                if (!selectedlist.Contains(V.Value.ContentSubVersion.ParentID))
                {
                    selectedlist.Add(V.Value.ContentSubVersion.ParentID);
                }
                whoIsLinked[V.Value.ContentSubVersion.ParentID] = whoIsLinked.Values.Last();
                foreach (var a in V.Value.ContentSubVersion.ContentVersions)
                {
                    checkLinked(V.Value.ContentSubVersion.ContentVersions, selectedlist, whoIsLinked);
                }

            }

        }

        //public void getAllContentsExistsVersions()
        //{
        //    foreach (var v in versions)
        //    {
        //        foreach (int i in versionsExistsKeys)
        //        {
        //            if (v.Key == i)
        //            {
        //                ContentLinksKeys.Add(v.Value.ParentID);
        //                checkLinked(v.Value.ContentVersions, ContentLinksKeys);

        //            }
        //        }
        //    }
        //}

        public void getAllContentsExistsVersions()
        {
            //foreach (var v in versions)
            //{
                foreach (int i in versionsExistsKeys)
                {
                    //if (v.Key == i)
                    //{
                    int conId = ContentManagementViewModel.versions[i].ParentID;
                    if (!ContentLinksKeys.Contains(conId))
                    {
                        ContentLinksKeys.Add(conId);
                    }
                    whoIsLinkedExistingContentId[conId] = conId;
                    checkLinked(ContentManagementViewModel.versions[i].ContentVersions, ContentLinksKeys, whoIsLinkedExistingContentId);
                    //}
                }
            //}
        }

        //public Boolean compareLinks()
        //{
        //    foreach (int i in ContentsDragKeys)
        //    {
        //        foreach (int j in ContentLinksKeys)
        //        {
        //            if (i == j)
        //            {
        //                showMessage(122);
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}


        public Boolean compareLinks()
        {
            foreach (int i in ContentsDragKeys)
            {
                foreach (int j in ContentLinksKeys)
                {
                    if (i == j)
                    {
                        int existingContentId = whoIsLinkedExistingContentId[i];
                        int draggedContentId = whoIsLinkedContentId[i];
                        
                        string iName = GetContentNameById(i);
                        string draggedName = GetContentNameById(draggedContentId);
                        string existingName = GetContentNameById(existingContentId);
                        Object[] ArgsList = new Object[] {0};
                        if (existingContentId == i && draggedContentId == i ) 
                        {
                            continue;
                        }
                       else if (existingContentId != i && draggedContentId != i)
                        {
                            ArgsList = new Object[] { draggedName, existingName, iName };
                            ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(122, ArgsList);
                        }
                        else 
                        {
                            if (existingContentId == i)
                            {
                                ArgsList = new Object[] { existingName, draggedName };
                            }
                            else
                            {
                                ArgsList = new Object[] { draggedName, existingName };
                            }
                            ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(142, ArgsList);
                        } 
                            //showMessage(122);
                        return true;
                    }
                }
            }
            return false;
        }

        //Ella
        private string GetContentNameById(int CId)
        {
            string cName = string.Empty;
            //foreach (KeyValuePair<int, Content> c in contents)
            //{
            //    if (c.Key.Equals(CId))
            //    {
                    cName = ContentManagementViewModel.contents[CId].Name;
            //    }
            //}
            return cName;
        }


            
        #endregion Content D&D Validations

        #region display Expender
        private static string _HideField = "Visible";
        public static string HideField
        {
            get
            {
                return _HideField;
            }
            set
            {
                _HideField = value;


            }
        }


        private static string _ShowField = "Collapsed";
        public static string ShowField
        {
            get
            {
                return _ShowField;
            }
            set
            {
                _ShowField = value;



            }
        }

        private static string _SyncRow = "23";
        public static string SyncRow
        {
            get
            {
                return _SyncRow;
            }
            set
            {
                _SyncRow = value;


            }
        }


        private static string _ExpenderRow = "11";
        public static string ExpenderRow
        {
            get
            {
                return _ExpenderRow;
            }
            set
            {
                _ExpenderRow = value;


            }
        }


        


        private static Boolean _ReadGroup = true;
        public static Boolean ReadGroup
        {
            get
            {
                return _ReadGroup;
            }
            set
            {
                _ReadGroup = value;

            }
        }


        private static Boolean _LockVersion = false;
        public static Boolean LockVersion
        {
            get
            {
                return _LockVersion;
            }
            set
            {
                _LockVersion = value;

            }
        }

        private static Boolean _LockCheck = true;
        public static Boolean LockCheck
        {
            get
            {
                return _LockCheck;
            }
            set
            {
                _LockCheck = value;

            }
        }

        private static Boolean _LockName = false;
        public static Boolean LockName
        {
            get
            {
                return _LockName;
            }
            set
            {
                _LockName = value;

            }
        }

        private static Boolean _LockSync = true;
        public static Boolean LockSync
        {
            get
            {
                return _LockSync;
            }
            set
            {
                _LockSync = value;

            }
        }
        

        #endregion display TreeNode

        #region contentsTab

        private Dictionary<int, CMFolderModel> Folders;
        private Dictionary<int, CMContentModel> Contents;
        private Dictionary<int, CMVersionModel> Versions;
        private List<CMTreeNode> ContentsTree;
        private List<CMTreeNode> LinkedVersions = new List<CMTreeNode>();
        private CMTreeViewNodeViewModelBase WorkNode;

        private ObservableCollection<CMTreeViewNodeViewModelBase> _TreeNodesLinks = new ObservableCollection<CMTreeViewNodeViewModelBase>();
        public ObservableCollection<CMTreeViewNodeViewModelBase> TreeNodesLinks
        {
            get
            {
                return _TreeNodesLinks;
            }
            set
            {
                _TreeNodesLinks = value;
                RaisePropertyChanged("TreeNodesLinks");
            }
        }

        public void getTreelist()
        {
            try
            {
                //Get Tree Node List of all the existing contents.
                foreach (var c in activeContents)
                {
                    if (!ContentBLL.versions.Keys.Contains(c.id) || !ContentBLL.contents.Keys.Contains(ContentBLL.versions[c.id].ParentID))
                    {
                        //Refresh Contents tree from CM - API
                        String logMessage = "Missing information for versionId " + c.id + ".";
                        logMessage = logMessage + "\n                  Refreshing Contents tree data.";
                        Domain.SaveGeneralWarningLog(logMessage);

                        ContentBLL.allContents = ContentBLL.LoadContentTreeToMemory(out ContentBLL.folders, out ContentBLL.contents, out ContentBLL.versions);

                        if (ContentBLL.allContents == null)
                        {
                            ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, new Object[] { 0 });
                        }
                    }
                    int versionParentId = ContentBLL.versions[c.id].ParentID;

                    LinkedVersions.Add(ContentBLL.contents[versionParentId]);

                    PopulateTreeView(0);
                }
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, new Object[] { 0 });
            }

        }// end of getTreeList


        private CMTreeViewRootNodeViewModel RootNode
        {
            get
            {
                return TreeNodesLinks[0] as CMTreeViewRootNodeViewModel;
            }
        }

        private void PopulateTreeView(long ParentId)
        {
            if (LinkedVersions.Count > 0)
            {
                //If the Tree is empty, create the Environment node
                if (TreeNodesLinks.Count == 0)
                {
                    CMTreeNode TN = new CMTreeNode();
                    TN.ID = 0;
                    TN.ParentID = int.MinValue;
                    TN.Name = "Contents";
                    TreeNodesLinks.Add(new CMTreeViewRootNodeViewModel(this.WorkspaceId, TN));
                    WorkNode = RootNode;
                }
                //Local variables initialization
                CMTreeViewNodeViewModelBase OperationRoot = WorkNode;
                OperationRoot.Children.Clear();

                //Scan the contentTree for parent nodes (nodes who are the direct children of the requested parent)
                /**var nodesList =
                    from T in LinkedVersions
                    where T.ParentID == ParentId
                    select T;**/

                CMTreeViewNodeViewModelBase TreeNode = null;
                foreach (CMTreeNode tn in LinkedVersions)
                {
                    if (tn.ID != 0)
                    {
                        switch (tn.TreeNodeType.ToString())
                        {
                            case "Folder":
                                TreeNode = new CMTreeViewFolderNodeViewModel(this.WorkspaceId, tn, RootNode);
                                break;
                            case "Content":
                                TreeNode = new CMTreeViewContentNodeViewModel(this.WorkspaceId, tn, RootNode);
                                break;
                            case "ContentVersion":
                                TreeNode = new CMTreeViewVersionNodeViewModel(this.WorkspaceId, tn, RootNode, Hierarchy, _activeContents);
                                break;
                            default:
                                TreeNode = new CMTreeViewFolderNodeViewModel(this.WorkspaceId, tn, RootNode);
                                break;

                        }
                        PopulateChildren(TreeNode); //Read the children of the newly created node (recursively)
                        
                        OperationRoot.Children.Add(TreeNode);
                    }
                }
            }

        }

        private void PopulateChildren(CMTreeViewNodeViewModelBase Node)
        {
            
            List<CMTreeNode> nodesList = new List<CMTreeNode>();
            List<int> VersionsID = new List<int>();

            if (Node.TreeNode.TreeNodeType.ToString() == "ContentVersion")
            {
                foreach (var t in ContentBLL.versions[Node.TreeNode.ID].ContentVersions)
                {
                    VersionsID.Add(t.Key);
                    //nodesList.Add(t.Value);
                }


                foreach (int j in VersionsID)
                {
                    nodesList.Add(ContentBLL.versions[j]);
                }
            }

            if (Node.TreeNode.TreeNodeType.ToString() == "Content")
            {

                foreach (var i in activeContents)
                {
                    if (ContentBLL.versions[i.id].ParentID == Node.TreeNode.ID)
                    //nodesList.Add(item);
                    nodesList.Add(ContentBLL.versions[i.id]);
                }

            }
            CMTreeViewNodeViewModelBase TreeNode = null;
            foreach (CMTreeNode tn in nodesList)
            {
                switch (tn.TreeNodeType.ToString())
                {
                    case "Folder":
                        TreeNode = new CMTreeViewFolderNodeViewModel(this.WorkspaceId, tn, Node);
                        break;
                    case "Content":
                        TreeNode = new CMTreeViewContentNodeViewModel(this.WorkspaceId, tn, Node);
                        break;
                    case "ContentVersion":
                        TreeNode = new CMTreeViewVersionNodeViewModel(this.WorkspaceId, tn, Node, Hierarchy, _activeContents);
                        break;
                }
                PopulateChildren(TreeNode);
                //For CR3600
                TreeNode.Name = ContentBLL.contents[ContentBLL.versions[tn.ID].ParentID].Name + " " + ContentBLL.versions[tn.ID].Name;
                Node.Children.Add(TreeNode);
            }
        }

        CMTreeViewNodeViewModelBase SelectedNode;
        private void OnShowContentTabDetailsReceived(CMTreeViewNodeViewModelBase Node)
        {
            SelectedNode = Node;
            RaisePropertyChanged("NodeName");
            RaisePropertyChanged("ContentName");
            RaisePropertyChanged("NodeDescription");
            RaisePropertyChanged("CommandLine");
            RaisePropertyChanged("NodeStatus");
            RaisePropertyChanged("ContentPath");
            RaisePropertyChanged("Files");


        }

        private string _NodeName;
        public string NodeName
        {
            get
            {
                if (SelectedNode != null)
                {
                    if (SelectedNode.GetType() == typeof(CMTreeViewVersionNodeViewModel))
                    {

                        return SelectedNode.Name;

                    }
                    else
                        return string.Empty;
                }
                else
                    return string.Empty;
            }
        }

        private string _ContentName;
        public string ContentName
        {
            get
            {
                if (SelectedNode != null)
                {
                    if (SelectedNode.GetType() == typeof(CMTreeViewContentNodeViewModel))
                    {
                        if (SelectedNode != null)
                        {
                            return SelectedNode.Name;
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                    else if (SelectedNode.GetType() == typeof(CMTreeViewVersionNodeViewModel))
                    {
                        return ContentBLL.contents[ContentBLL.versions[SelectedNode.TreeNode.ID].ParentID].Name;
                    }
                    else
                        return string.Empty;
                }
                else
                    return string.Empty;
            }
        }

        private string _NodeDescription;
        public string NodeDescription
        {
            get
            {
                if (SelectedNode != null)
                {
                    if (SelectedNode.GetType() == typeof(CMTreeViewVersionNodeViewModel))
                        return ContentBLL.versions[SelectedNode.TreeNode.ID].Description;
                    else if (SelectedNode.GetType() == typeof(CMTreeViewContentNodeViewModel))
                        return ContentBLL.contents[SelectedNode.TreeNode.ID].Description;
                    else
                        return string.Empty;

                }
                else
                {
                    return string.Empty;
                }
            }
        }



        private string _CommandLine;
        public string CommandLine
        {
            get
            {
                if (SelectedNode != null)
                {
                    if (SelectedNode.GetType() == typeof(CMTreeViewVersionNodeViewModel))
                        return ContentBLL.versions[SelectedNode.TreeNode.ID].RunningString;
                    else
                        return string.Empty;

                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private string _NodeStatus;
        public string NodeStatus
        {
            get
            {
                if (SelectedNode != null)
                {
                    if (SelectedNode.GetType() == typeof(CMTreeViewVersionNodeViewModel))
                        return ContentBLL.versions[SelectedNode.TreeNode.ID].Status.Name;
                    else
                        return string.Empty;

                }
                else
                {
                    return string.Empty;
                }
            }
        }



        private string _ContentPath;
        public string ContentPath
        {
            get
            {
                if (SelectedNode != null)
                {

                    if (SelectedNode.GetType() == typeof(CMTreeViewVersionNodeViewModel))
                    {
                        if (ContentBLL.versions[SelectedNode.TreeNode.ID].Files.Count > 0)
                        {
                            string FilePathf = "";
                            string FilePathN = "";
                            string FilePathR = "";
                            foreach (var i in ContentBLL.versions[SelectedNode.TreeNode.ID].Files)
                            {
                                if (!(String.IsNullOrEmpty(i.Value.FileRelativePath)))
                                {
                                    FilePathf = i.Value.FileFullPath.Replace(i.Value.FileName, "");
                                    //FilePathR = Versions[SelectedNode.TreeNode.ID].Files[0].FileRelativePath;
                                    FilePathN = FilePathf.Replace(i.Value.FileRelativePath, "");
                                    return FilePathN.Trim();

                                }
                                else
                                {
                                    FilePathf = i.Value.FileFullPath.Replace(i.Value.FileName, "");
                                    return FilePathf.Trim();

                                }
                                break;
                            }

                            return string.Empty;

                        }
                        else
                            return string.Empty;
                    }
                    else
                        return string.Empty;
                }
                else
                    return string.Empty;
            }
        }

        /**   private StringCollection _Files = new StringCollection();
           public StringCollection Files
           {

               get
               {
                   if (SelectedNode != null)
                   {

                       if (SelectedNode.GetType() == typeof(CMTreeViewVersionNodeViewModel))
                       {
                           if (Versions[SelectedNode.TreeNode.ID].Files.Count > 0)
                           {
                               foreach (var i in Versions[SelectedNode.TreeNode.ID].Files)
                                   _Files.Add(i.Value.FileFullPath);
                               return _Files;
                           }
                           return _Files;
                       }
                       return _Files;
                   }
                   else
                       return _Files;

               }
           }**/

        private ObservableCollection<string> _Files = new ObservableCollection<string>();
        public ObservableCollection<string> Files
        {

            get
            {
                if (SelectedNode != null)
                {
                    if (_Files != null)
                        _Files.Clear();

                    if (SelectedNode.GetType() == typeof(CMTreeViewVersionNodeViewModel))
                    {
                        if (ContentBLL.versions[SelectedNode.TreeNode.ID].Files.Count > 0)
                        {
                            foreach (var i in ContentBLL.versions[SelectedNode.TreeNode.ID].Files)
                                _Files.Add(i.Value.FileFullPath);
                            return _Files;
                        }
                        return _Files;
                    }
                    return _Files;
                }
                else
                    return _Files;

            }
        }


        #endregion contentsTab

        #region Clone Project
        private void OnShowCloneDetailsReceived(TreeViewNodeViewModelBase ParentNode)
        {
            if (Hierarchy.IsCloned == true)
            {
                ProjectDetailsViewModel.HideField = "Collapsed";
                ProjectDetailsViewModel.ShowField = "Visible";
                ProjectDetailsViewModel.SyncRow = "9";
            }
            else if (Hierarchy.IsClonedRelated)
            {
                ProjectDetailsViewModel.HideField = "Visible";
                ProjectDetailsViewModel.ShowField = "Visible";
                ProjectDetailsViewModel.ExpenderRow = "25";
                ProjectDetailsViewModel.LockVersion = true;
                ProjectDetailsViewModel.LockCheck = false;
                if (Hierarchy.GroupId == -1)
                {
                    ProjectDetailsViewModel.ReadGroup = false;
                }

            }


            Hierarchy.ParentId = ParentNode.Hierarchy.Id;
            //TODO
            Hierarchy.TreeHeader = (Domain.Environment + VersionBLL.getParentName(ParentNode.Hierarchy.Id.ToString()));
            RaisePropertyChanged("TreeHeader");
            Hierarchy.ParentId = ParentNode.Hierarchy.Id;
            // MessageMediator.NotifyColleagues(WorkspaceId + "ShowProjectDetails", this);
            
            ProjectDetailsViewModel.HideField = "Visible";
            ProjectDetailsViewModel.ShowField = "Collapsed";
            ProjectDetailsViewModel.SyncRow = "23";
            ProjectDetailsViewModel.LockVersion = false;
            ProjectDetailsViewModel.LockCheck = true;
            ProjectDetailsViewModel.ReadGroup = true;
        }

        public static void ExpandTreeOnFolderTarget(TreeViewNodeViewModelBase ParentNode)
        {           
            ParentNode.IsExpandedTree = true;
            ParentNode.IsSelectedTree = true;
        }


        #endregion Clone Project

        #region CheckHierarchy
        public void CheckHierarchy()
        {
            //Hierarchy still 'dirty'. clone bot been saved
            if (Hierarchy.IsCloned == true && Hierarchy.IsNew == true && HideField == "Visible" && ShowField == "Collapsed")
            {
                Hierarchy.IsCloned = false ;
                Hierarchy.IsNew = false;
                Hierarchy.IsDirty = false;

            }
            if (Hierarchy.IsClonedRelated == true && LockVersion == false && LockCheck == true)
            {
                Hierarchy.IsClonedRelated = false;
                Hierarchy.IsNew = false;
                Hierarchy.IsDirty = false;

            }
            if (TreeViewProjectNodeViewModel.HierarchyPR != null)
            {
                if (Hierarchy.IsNew == false && Hierarchy.IsCloned == false)
                {
                    this.Hierarchy.IsCloned = false;
                    _HideField = "Visible";
                    _ShowField = "Collapsed";
                    _SyncRow = "23";
                    Hierarchy.IsDirty = false;
                }

                if (Hierarchy.IsNew == false && Hierarchy.IsClonedRelated == false)
                {
                    this.Hierarchy.IsClonedRelated = false;
                    _ShowField = "Collapsed";
                    enableStep = true;
                    RaisePropertyChanged("enableStep");
                    _ReadGroup = true;
                    _LockVersion = false;
                    _LockCheck = true;
                    Hierarchy.IsDirty = false;
                }
                if (Hierarchy.IsCloned == true && Hierarchy.IsClonedRelated == true)
                {

                    this.Hierarchy.IsCloned = false;
                    _HideField = "Visible";
                    _SyncRow = "23";
                    Hierarchy.IsDirty = false;

                }
                if (Hierarchy.IsClonedRelated == true && (Hierarchy.IsClonedRelatedUpdate == true || Hierarchy.IsClonedRelatedSplit == true))
                {

                    this.Hierarchy.IsClonedRelated = false;
                    _ShowField = "Collapsed";
                    _LockVersion = false;
                    _LockCheck = true;
                    _ReadGroup = true;
                    Hierarchy.IsDirty = false;
                }
                if (Hierarchy.IsClonedRelated == true && Hierarchy.IsDirty )
                {
                    Hierarchy.IsClonedRelated = false;
                    Hierarchy.IsDirty = false;
                }

            }
            if (Hierarchy.IsClonedRelatedUpdate == true && Hierarchy.IsDirty)
            {
                Hierarchy.IsClonedRelatedUpdate = false;
                Hierarchy.IsDirty = false;

            }
        }
        #endregion CheckHierarchy

        #region update All Related Step
        public void UpdateGroupNode(ObservableCollection<TreeViewNodeViewModelBase> Tree)
        {
            if (Hierarchy != null && Tree != null)
            {
                if (!(String.IsNullOrEmpty(Hierarchy.SelectedStep)))
                {
                    foreach (var i in Tree)
                    {
                        if (i.Hierarchy.GroupId == Hierarchy.GroupId)
                        {
                            i.Hierarchy.SelectedStep = Hierarchy.SelectedStep; 
                            MessageMediator.NotifyColleagues(this.WorkspaceId + "UpdateNode", i.Hierarchy);
                        }
                        if (i.Children.Count > 0)
                        {
                            UpdateGroupNode(i.Children);
                        }
                    }
                }
            }
        }
        #endregion update All Related Step

        #region Change Content Order

        #region Up

        #region Relay Command

        private RelayCommand _UpContentCommand;
        public ICommand UpContentCommand
        {
            get
            {
                if (_UpContentCommand == null)
                {
                    _UpContentCommand = new RelayCommand(ExecuteUpContentCommand, CanExecuteUpContentCommand);
                }
                return _UpContentCommand;
            }
        }

        private bool CanExecuteUpContentCommand()
        {
            if (contentToAction is ContentModel && activeContentsCounter > 1)
            {
                if (Hierarchy.GroupId != -1)
                {
                    if (Hierarchy.IsClonedRelatedSplit == true || Hierarchy.IsClonedRelatedUpdate == true)
                        return true;
                    else
                        return false;
                }

                else if (Hierarchy.IsClonedRelated == true)
                    return false;
                else
                    return true;
            }
            else
            {
                return false;
            }
        }

        private void ExecuteUpContentCommand()
        {
            cannotUpdateActiveContents = false;
            if (activeContents.Where(x => x.id == contentToAction.id).First().seq > activeContents.Min(x => x.seq))
            {
                int contentToMoveUP_seq = activeContents.Where(x => x.id == contentToAction.id).First().seq;
                int contentToMoveDown_seq = activeContents.Where(x => x.seq < contentToAction.seq).Last().seq;

                foreach (ContentModel cm in activeContents)
                {
                    if (cm.seq == contentToMoveDown_seq)
                    {
                        activeContents.Where(x => x.id == cm.id).First().seq = contentToMoveUP_seq;
                        activeContents.Where(x => x.id == contentToAction.id).First().seq = contentToMoveDown_seq;
                    }
                }
            }
            contentsUpdated = true;

            Hierarchy.VM.IsNew = true;
            if (Hierarchy.VM.VersionName == InitialVersionName && !Hierarchy.IsCloned && !Hierarchy.IsClonedRelatedSplit)
            {
                LastVersionName = Hierarchy.VM.VersionName;
                projectId = (int)Hierarchy.Id;
                if (Hierarchy.GroupId > 0)
                {
                    defaultVersionName = VersionBLL.GenerateDefaultVersionName((int)Hierarchy.GroupId);
                }
                else
                {
                    defaultVersionName = VersionBLL.GenerateDefaultVersionName(projectId);
                }
                if (defaultVersionName != string.Empty)
                {
                    this.VM.VersionName = defaultVersionName;
                }
                else //if failed to generate - current functionality
                {
                    this.VM.VersionName = Hierarchy.VM.VersionName;
                }
                VM.Description = string.Empty;
                //this.Hierarchy.VM.VersionName = Hierarchy.VM.VersionName;
                RaisePropertyChanged("VersionName");
                this.Hierarchy.VM.Description = string.Empty;
                RaisePropertyChanged("VersionDescription");
                this.VM.EcrId = string.Empty;
                RaisePropertyChanged("EcrId");
            }
            activeContents = activeContents.OrderBy(x => x.seq).ToList();
            RaisePropertyChanged("activeContents");
            Hierarchy.VM.IsDirty = true;
            Hierarchy.IsDirty = true;
            MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
        }
        #endregion

        #endregion UP

        #region Down

        #region Relay Command
        private RelayCommand _DownContentCommand;
        public ICommand DownContentCommand
        {
            get
            {
                if (_DownContentCommand == null)
                {
                    _DownContentCommand = new RelayCommand(ExecuteDownContentCommand, CanExecuteDownContentCommand);
                }
                return _DownContentCommand;
            }
        }

        private bool CanExecuteDownContentCommand()
        {
            if (contentToAction is ContentModel && activeContentsCounter > 1)
            {
                if (Hierarchy.GroupId != -1)
                {
                    if (Hierarchy.IsClonedRelatedSplit == true || Hierarchy.IsClonedRelatedUpdate == true)
                        return true;
                    else
                        return false;
                }

                else if (Hierarchy.IsClonedRelated == true)
                    return false;
                else
                    return true;
            }
            else
            {
                return false;
            }
        }

        private void ExecuteDownContentCommand()
        {
            cannotUpdateActiveContents = false;
            if (activeContents.Where(x => x.id == contentToAction.id).First().seq < activeContents.Max(x => x.seq))
            {
                int contentToMoveDown_seq = activeContents.Where(x => x.id == contentToAction.id).First().seq;
                int contentToMoveUp_seq = activeContents.Where(x => x.seq > contentToAction.seq).First().seq;

                foreach (ContentModel cm in activeContents)
                {
                    if (cm.seq == contentToMoveUp_seq)
                    {
                        activeContents.Where(x => x.id == cm.id).First().seq = contentToMoveDown_seq;
                        activeContents.Where(x => x.id == contentToAction.id).First().seq = contentToMoveUp_seq;

                    }
                }
                contentsUpdated = true;
                Hierarchy.VM.IsNew = true;

                activeContents = activeContents.OrderBy(x => x.seq).ToList();
                if (Hierarchy.VM.VersionName == InitialVersionName && !Hierarchy.IsCloned && !Hierarchy.IsClonedRelatedSplit)
                {
                    LastVersionName = Hierarchy.VM.VersionName;
                    projectId = (int)Hierarchy.Id;
                    if (Hierarchy.GroupId > 0)
                    {
                        defaultVersionName = VersionBLL.GenerateDefaultVersionName((int)Hierarchy.GroupId);
                    }
                    else
                    {
                        defaultVersionName = VersionBLL.GenerateDefaultVersionName(projectId);
                    }
                    if (defaultVersionName != string.Empty)
                    {
                        this.VM.VersionName = defaultVersionName;
                    }
                    else //if failed to generate - current functionality
                    {
                        this.VM.VersionName = Hierarchy.VM.VersionName;
                    }
                    VM.Description = string.Empty;
                    //this.Hierarchy.VM.VersionName = Hierarchy.VM.VersionName;
                    RaisePropertyChanged("VersionName");
                    this.Hierarchy.VM.Description = string.Empty;
                    RaisePropertyChanged("VersionDescription");
                    this.VM.EcrId = string.Empty;
                    RaisePropertyChanged("EcrId");
                }
                RaisePropertyChanged("activeContents");
                Hierarchy.IsDirty = true;
                Hierarchy.VM.IsDirty = true;
                MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
            }
        }
        #endregion

        #endregion Down

        #endregion

        #region UserCertificate Tab

        private UserCertificateApiModel _SelectedUserCertificate;
        public UserCertificateApiModel SelectedUserCertificate
        {
            get
            {
                return _SelectedUserCertificate;
            }
            set
            {
                _SelectedUserCertificate = value;
                RaisePropertyChanged("SelectedUserCertificate");
            }
        }

        private void UserCertificateFiller()
        {
            UserCertificatesRemoved.Clear();
            Hierarchy.UserCertificates.Clear();
             UserCertificateBLL.GetUserCertificateForHierachy(ref _Hierarchy);
        }

        private RelayCommand _DeleteUserCertificateCommand;
        public ICommand DeleteUserCertificateCommand
        {
            get
            {
                if (_DeleteUserCertificateCommand == null)
                {
                    _DeleteUserCertificateCommand = new RelayCommand(ExecuteDeleteUserCertificateCommand, CanExecuteDeleteUserCertificateCommand);
                }
                return _DeleteUserCertificateCommand;
            }
        }

       // private List<CertificateModel> certificateToRemove;
        private bool CanExecuteDeleteUserCertificateCommand()
        {
            if (Domain.IsPermitted("141"))
            {
                if (Hierarchy.UserCertificates.Count() > 0 && SelectedUserCertificate != null)
                {
                    if (Hierarchy.IsClonedRelatedSplit == true || Hierarchy.IsClonedRelatedUpdate == true || Hierarchy.IsClonedRelated == true)
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        private ObservableCollection<UserCertificateApiModel> UserCertificatesRemoved = new ObservableCollection<UserCertificateApiModel>();


        private void ExecuteDeleteUserCertificateCommand()
        {
            try
            {

                foreach (var i in Hierarchy.UserCertificates)
                {
                    if (i.UserCertificateId == SelectedUserCertificate.UserCertificateId)
                    {
                        if (i.IsNew == false)
                        {
                            UserCertificatesRemoved.Add(SelectedUserCertificate);
                        }
                        Hierarchy.UserCertificates.Remove(SelectedUserCertificate);
                        break;
                    }
                }
                Hierarchy.IsDirty = true;
                MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                RaisePropertyChanged("UserCertificates");
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, new Object[] { 0 });
            }
        }

        #endregion UserCertificate Tab

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

            return !String.IsNullOrEmpty(Hierarchy.VM.TargetPath);
            
        }

        private void ExecuteOpenPathExecute()
        {
            if (!String.IsNullOrEmpty(Hierarchy.VM.TargetPath))
            {

                if (Directory.Exists(Hierarchy.VM.TargetPath))
                    Process.Start(@Hierarchy.VM.TargetPath);
                else
                {
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(157, ArgsList);

                }
            }
            
            //Process.Start(new ProcessStartInfo
            //{
            //    FileName = "explorer.exe",
            //    Arguments = Hierarchy.VM.TargetPath
            //});
            
        }

        #endregion

        private void OnIsRefreshDirtyNodeReceived(TreeViewNodeViewModelBase TV)
        {
            this._Hierarchy = HierarchyBLL.GetHierarchyRow(Hierarchy.Id);
            MessageMediator.NotifyColleagues(this.WorkspaceId + "ShowProjectDetails", this.Node);
           RaisePropertyChanged("Name");
        }//Register to recieve a message asking for node refresh
        
        private void OnExecuteLinkedVersionReceived(CMTreeViewVersionNodeViewModel CMVersion)
        {
            tabIndex = 0;
            RaisePropertyChanged("tabIndex");
        }//Register to recieve a message asking for node refresh


        #region Populate contets for new project (from Template)

        public void PopulateContents(string Step)
        {
            //Collection to add all contents for new project.
            Dictionary<int, int> Contents;
            string BranchIds = "";
            HierarchyBLL.HierarchyBLLReturnCode getIdsStatus = HierarchyBLL.HierarchyBLLReturnCode.CommonException;
            try
            {
                Dictionary<int, CMFolderModel> outFolders = ProjectsExplorerViewModel.folders;
                Dictionary<int, CMContentModel> outContents = ProjectsExplorerViewModel.contents;
                Dictionary<int, CMVersionModel> outVersions = ProjectsExplorerViewModel.versions;

                //Final Contents add to active contents.
                ContentBLL bll = new ContentBLL(VM.VersionId);
                ObservableCollection<ContentModel> initialProjectContents = new ObservableCollection<ContentModel>();
                if (_Hierarchy.VM.IsClosed)
                {
                    initialProjectContents = bll.getActiveContents();
                }
                else
                {
                    initialProjectContents = (ObservableCollection<ContentModel>)_activeContents;
                }
                Dictionary<int, InheritedContentModel> projectContentsInherited  =new Dictionary<int, InheritedContentModel>();
                 Domain.ErrorHandling Status = HierarchyBLL.PopulateProjectContents(_Hierarchy, 
                                                        //(ObservableCollection<ContentModel>)_activeContents,
                                                        initialProjectContents,
                                                         Step, outContents, outVersions,
                                                         out projectContentsInherited);

                 if (projectContentsInherited != null && projectContentsInherited.Count != 0)
                 {
                     var sortedDictVar = from entry in projectContentsInherited orderby entry.Value.cvPriority ascending select entry;

                     Dictionary<int, InheritedContentModel> sortedDict = sortedDictVar.ToDictionary(x => x.Key, x => x.Value);

                     ObservableCollection<ContentModel> ActiveContents = new ObservableCollection<ContentModel>();
                     foreach (var i in sortedDict)
                     {
                         if (!outVersions.ContainsKey(i.Key)) //Content tree has been updated from parallel instance
                         {
                             //get content tree
                             Domain.CallingAppName = Domain.AppName;

                             ContentBLL.allContents = ContentBLL.LoadContentTreeToMemory(out ContentBLL.folders, out ContentBLL.contents, out ContentBLL.versions);
                             allContents = ContentBLL.allContents;
                             outFolders = ContentBLL.folders;
                             outContents = ContentBLL.contents;
                             outVersions = ContentBLL.versions;
                             Domain.CallingAppName = "";
                         }
                         ContentModel TemplateContent = new ContentModel(outContents[outVersions[i.Key].ParentID].Name,
                                                                                     outVersions[i.Key].Name, i.Key,
                                                                                     DateTime.Now.ToString(), "",
                                                                                     outContents[outVersions[i.Key].ParentID].ContentType.Name);
                         TemplateContent.status = outVersions[i.Key].Status.Name;
                         TemplateContent.seq = sortedDict[i.Key].cvPriority;
                         ActiveContents.Add(TemplateContent);
                     }

                     bool isDifferent = false;
                     if (_activeContents != null && ActiveContents != null)
                     {
                         if (_activeContents.Count() == ActiveContents.Count)
                         {
                             //Compare ContentVersionIds and sequences. If two sets are the same the project is not affected.
                             Dictionary<int, int> currentContents = new Dictionary<int, int>();
                             Dictionary<int, int> inheritedContents = new Dictionary<int, int>();
                             foreach (ContentModel c in _activeContents)
                             {
                                 currentContents.Add(c.id, c.seq);
                             }

                             foreach (ContentModel c in ActiveContents)
                             {
                                 inheritedContents.Add(c.id, c.seq);
                             }

                             IEnumerable<KeyValuePair<int, int>> intersection = inheritedContents.Intersect(currentContents);
                             if (intersection == null || intersection.Count() != inheritedContents.Count)
                             {
                                 isDifferent = true;
                             }
                         }
                         else
                         {
                             isDifferent = true;
                         }
                     }
                     if (isDifferent)
                     {
                         _activeContents = ActiveContents;
                         tabIndex = 0;
                         RaisePropertyChanged("tabIndex");
                         RaisePropertyChanged("activeContents");
                         Hierarchy.VM.IsClosed = true;
                         Hierarchy.VM.Contents = ActiveContents;
                         string defaultVersionName = VersionBLL.GenerateDefaultVersionName((int)Hierarchy.Id);
                         if (defaultVersionName != string.Empty)
                         {
                             Hierarchy.VM.VersionName = defaultVersionName;
                         }
                         //else - inherited, no change
                         Hierarchy.VM.Description = string.Empty;
                         Hierarchy.VM.IsDirty = true;
                         string hierarchyPath = string.Empty;
                         HierarchyBLL.GetProjectFullPathByProjectId(Hierarchy.ParentId, out hierarchyPath);
                         Hierarchy.VM.TargetPath = Domain.PE_SystemParameters["ProjectLocalPath"] + "/" + 
                             hierarchyPath + "/" + Hierarchy.Name + "/" + Hierarchy.VM.VersionName;
                         RaisePropertyChanged("VersionName");
                         RaisePropertyChanged("VersionDescription");
                         RaisePropertyChanged("TargetPath");
                         this.VM.EcrId = string.Empty;
                         RaisePropertyChanged("EcrId");
                     }
                     else
                     {
                         Hierarchy.VM.Contents = (ObservableCollection<ContentModel>)_activeContents;
                     }
                 }
            }
            catch (Exception e)
            {
                ApiBLL.TraceExceptionParameterValue.Add(e.Message);
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                //return "105";
            }

        }

        #endregion Populate contets for new project (Template)

    }

} //end of root namespace