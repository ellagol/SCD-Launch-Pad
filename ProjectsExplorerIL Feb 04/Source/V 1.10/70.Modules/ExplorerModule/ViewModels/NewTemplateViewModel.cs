using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class NewTemplateViewModel : ViewModelBase, IDropTarget
    {

        #region  Data

        protected MessengerService MessageMediator = new MessengerService();
        private IMessageBoxService MsgBoxService = null;

        public int tabIndex { get; set; }

        private bool cannotUpdateActiveContents = true;

        //CR3483
        string defaultVersionName = string.Empty;

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

        private string InitialVersionName = string.Empty;

        Dictionary<string, string> InitialCertificates = new Dictionary<string,string>();
        ObservableCollection<UserCertificateApiModel> InitialUserCertificates = new ObservableCollection<UserCertificateApiModel>();

        public static string LastVersionName = "";

      
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

        private string _IsNotesVisible = "Visible";
            //"Hidden";
        public string IsNotesVisible
        {
            get
            {
                return _IsNotesVisible;
            }
            set
            {
                _IsNotesVisible = value;
                RaisePropertyChanged("IsNotesVisible");
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

        [StringLength(2, ErrorMessage = "Maximum length (500 characters) exceeded.")]
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
                    _ComboText = " ";
                    RaisePropertyChanged("ComboText");
                    _Hierarchy.SelectedStep = value;
                    RaisePropertyChanged("SelectedStep");
                    Hierarchy.IsDirty = true;
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
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

        public ObservableCollection<String> _ProjectSteps = new ObservableCollection<string>();

        public ObservableCollection<String> ProjectSteps
        {
            get
            {
                if (_ProjectSteps != null)
                    return _ProjectSteps;
                else
                    return null;
            }
            set
            {
                _ProjectSteps = value;
                RaisePropertyChanged("ProjectSteps");
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

        public class SelectComboTextAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                if (value == "Please Select..")
                {
                    return false;
                }

                return true;
            }
        }

        //Display the text of the step combo box.
        private string _ComboText = "Please Select..";
        public string ComboText
        {
            get
            {
                return _ComboText;
            }
            set
            {
                _ComboText = value;
                RaisePropertyChanged("ComboText");
            }
        }

        //public int seq
        //{
        //    get
        //    {
        //        if (_Hierarchy != null)
        //        {
        //            return _Hierarchy;
        //        }
        //        else
        //        {
        //            return -1;
        //        }
        //    }
        //    set
        //    {
        //        CertificatesChanged = true;
        //        if (_Hierarchy != null)
        //        {
        //            _Hierarchy.Sequence = value;
        //            RaisePropertyChanged("seq");
        //        }
        //        Hierarchy.IsDirty = true;
        //        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
        //    }
        //}



        #endregion

        #region Constructor


        public NewTemplateViewModel(Guid workspaceId, TreeViewNodeViewModelBase TV)
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

            //Get All Steps.

                ObservableCollection<HierarchyModel> ProjectChildren = GetALLProjectsFamily(Node);
                _ProjectSteps = TemplateBLL.GetAllSteps(ProjectChildren);
                RaisePropertyChanged("ProjectSteps");

                
           

            //initialize Notes side bar
            if (!Hierarchy.IsNew)
            {
                bool showNotesSideBar = true;
                bool isNotCloneRelated = true;
                long HierarchyId = Hierarchy.Id;
                Notes = new NotesControlViewModel(HierarchyId, TV.Name, ref isNotCloneRelated, ref showNotesSideBar, this.WorkspaceId);
                ShowNotes = showNotesSideBar;
                RaisePropertyChanged("ShowNotes");
            }
            else //Not display notes side bar. 
            {
                IsNotesVisible = "Hidden";
                RaisePropertyChanged("IsNotesVisible");
            }
            Hierarchy.IsCloned = false;
          
            //Add version in case that this is not a new template.
            if(!Hierarchy.IsNew)
                Hierarchy.VM = VersionBLL.GetActiveVersion(Hierarchy.Id);

            CertificateBLL.CertificateBLLReturnResult getCertstatus = CertificateBLL.GetNodeCertificatesDB(Hierarchy, out InitialCertificates);
            certificateDataFiller();

            UserCertificateFiller();
            foreach (UserCertificateApiModel uc in Hierarchy.UserCertificates)
            {
                InitialUserCertificates.Add(uc);
            }

            ServiceProvider.RegisterService<FolderBrowserService>(new FolderBrowserService());
            FolderBrowserDialogService = GetService<FolderBrowserService>();
          
            
            if (activeContents != null && activeContents.ToList().Count > 0)
            {
                 getTreelist();
               RaisePropertyChanged("TreeNodesLinks");
            }
            LastVersionName = string.Empty;
            InitialVersionName = Hierarchy.VM.VersionName;
            if (Hierarchy.IsNew)
            {
                tabIndex = 1;
                Hierarchy.IsDirty = true;
                Hierarchy.VM.IsDirty = true;
                //HERE IS THE REASON FOR BUG OF Crushing on "NewTemplate" - For now fixed
                MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);

                //CR3483
                defaultVersionName = VersionBLL.GenerateDefaultVersionName((int)Hierarchy.Id);

                if (defaultVersionName != string.Empty)
                {
                    this.VM.VersionName = defaultVersionName;
                }
                else //if failed to generate - current functionality
                {
                    this.VM.VersionName = Hierarchy.VM.VersionName;
                }
                RaisePropertyChanged("VersionName");
                //this.VM.Description = Hierarchy.VM.Description;
                this.VM.Description = string.Empty;
                RaisePropertyChanged("VersionDescription");
                this.VM.DefaultTargetPathInd = true;
                RaisePropertyChanged("DefaultTargetPathInd");
                this.VM.TargetPath = getTargetPath();
                RaisePropertyChanged("TargetPath");
                this.VM.EcrId = string.Empty;
                RaisePropertyChanged("EcrId");
            }
            else
            {
                tabIndex = 0;
                _ComboText = "";
                RaisePropertyChanged("ComboText");
            }

            MessageMediator.Register(this.WorkspaceId + "OnShowContentTabDetailsReceived", new Action<CMTreeViewNodeViewModelBase>(OnShowContentTabDetailsReceived)); //Register to recieve a message asking for node refresh
            //MessageMediator.Register(this.WorkspaceId + "OnRapidExecutionReceived", new Action<TreeViewProjectNodeViewModel>(OnRapidExecutionReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnTemplateRapidExecutionReceived", new Action<TreeViewTemplateNodeViewModel>(OnTemplateRapidExecutionReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnTemplateRapidExecutionStartReceived", new Action<TreeViewTemplateNodeViewModel>(OnTemplateRapidExecutionStartReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnTemplateClearViewModelReceived", new Action<TreeViewTemplateNodeViewModel>(OnTemplateClearViewModelReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnClearTemplateViewModelReceived", new Action<TreeViewTemplateNodeViewModel>(OnClearTemplateViewModelReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnDisableTemplateReceived", new Action<TreeViewTemplateNodeViewModel>(OnDisableTemplateReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnIsRefreshDirtyNodeReceived", new Action<TreeViewNodeViewModelBase>(OnIsRefreshDirtyNodeReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnExecuteLinkedVersionReceived", new Action<CMTreeViewVersionNodeViewModel>(OnExecuteLinkedVersionReceived)); //Switched to the first tab when executing linked version
            MessageMediator.Register(this.WorkspaceId + "OnLinkedVersionExecutionStartReceived", new Action<CMTreeViewVersionNodeViewModel>(OnLinkedVersionExecutionStartReceived)); //Progress bar when executing linked version
            MessageMediator.Register(this.WorkspaceId + "OnLinkedVersionExecutionReceived", new Action<CMTreeViewVersionNodeViewModel>(OnLinkedVersionExecutionReceived)); //Progress bar when executing linked version            
            //MessageMediator.Register(this.WorkspaceId + "OnProgressTextReceived", new Action<TreeViewProjectNodeViewModel>(OnProgressTextReceived)); //Progress bar text     
            MessageMediator.Register(this.WorkspaceId + "OnTemplateProgressTextReceived", new Action<TreeViewTemplateNodeViewModel>(OnTemplateProgressTextReceived));
            MessageMediator.Register(this.WorkspaceId + "OnContentsTabProgressTextReceived", new Action<CMTreeViewVersionNodeViewModel>(OnContentsTabProgressTextReceived)); //Progress bar text     
            
            MessageMediator.Register(this.WorkspaceId + "OnRequestPriorityPopupCloseReceived", new Action<string>(OnRequestPriorityPopupCloseReceived)); // To close Priority Popup
            MessageMediator.Register(this.WorkspaceId + "OnRequestUpdateSeqReceived", new Action<int>(OnRequestUpdateSeqReceived)); // To Update Priority Seqence recieved
        }





        #endregion


        #region PopUp Command

        #region PopupContent

        private bool _ShowPopupContent = false;
        public bool ShowPopupContent
        {
            get
            {
                return _ShowPopupContent;
            }
            set
            {
                _ShowPopupContent = value;
                RaisePropertyChanged("ShowPopupContent");
            }
        }

        #endregion

        #region OverlayContentViewModel

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

        #endregion

        #region Open Popup

        private RelayCommand _PopupCommand;
        public RelayCommand PopupCommand
        {
            get
            {
                if (_PopupCommand == null)
                {
                    _PopupCommand = new RelayCommand(ExecutePopupCommand, CanExecutePopupCommand);
                }
                return _PopupCommand;
            }
        }

        private bool CanExecutePopupCommand()
        {
            return true;
        }

        private void ExecutePopupCommand()
        {
            ShowPopupContent = true;
            OverlayContentViewModel = new PriorityPopupViewModel(this.WorkspaceId.ToString() , this.GetType() ,activeContents);
        }
        #endregion


        private void OnRequestUpdateSeqReceived(int seq)
        {
            if(contentToAction == null) { 
                return;
            }
            OverlayContentViewModel = null;
            ShowPopupContent = false;

            cannotUpdateActiveContents = false;
            activeContents = TemplateBLL.updateActiveContentSequence(contentToAction, activeContents, seq);

            #region Set name Red , set Dirty , close Version
            if (Hierarchy.VM.VersionName == InitialVersionName)
            {
                LastVersionName = Hierarchy.VM.VersionName;
                //CR3483
                defaultVersionName = VersionBLL.GenerateDefaultVersionName((int)Hierarchy.Id);

                if (defaultVersionName != string.Empty)
                {
                    this.VM.VersionName = defaultVersionName;
                }
                else //if failed to generate - current functionality
                {
                    this.VM.VersionName = Hierarchy.VM.VersionName;
                }

                RaisePropertyChanged("VersionName");
                //this.VM.Description = Hierarchy.VM.Description;
                this.VM.Description = string.Empty;
                RaisePropertyChanged("VersionDescription");
                this.VM.DefaultTargetPathInd = true;
                RaisePropertyChanged("DefaultTargetPathInd");
                this.VM.TargetPath = getTargetPath();
                RaisePropertyChanged("TargetPath");
                //LastVersionName = string.Empty;
                this.VM.EcrId = string.Empty;
                RaisePropertyChanged("EcrId");
            }

            //Should be ContentUpdated - For Save
            contentRemoved = true; 

            RaisePropertyChanged("activeContents");
            Hierarchy.IsDirty = true;
            Hierarchy.VM.IsDirty = true;
            MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
            #endregion
        }
        private void OnRequestPriorityPopupCloseReceived(string Param)
        {
            OverlayContentViewModel = null;
            ShowPopupContent = false;
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
            {
                return false;
            }

            if (String.IsNullOrEmpty(VersionName.Trim()) || VersionName.Length > 50 || (String.IsNullOrEmpty(Name.Trim()) || Name.Length > 200)
                || (Description.Length > 500) || (String.IsNullOrEmpty(VersionDescription.Trim())
                || VersionDescription.Length > 50) || (String.IsNullOrEmpty(TargetPath.Trim())) ||
                (!string.IsNullOrEmpty(Hierarchy.VM.EcrId) && Hierarchy.VM.EcrId.Length > 100))
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

            SelectComboTextAttribute Sct = new SelectComboTextAttribute();
            if (!Sct.IsValid(ComboText))
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
                if (!Domain.IsPermitted("151"))
                {
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(106, new object[] { 0 });
                    return;
                }
                Domain.PersistenceLayer.BeginTransWithIsolation(IsolationLevel.Serializable);

                // Work variables
                Collection<string> StatusBarParameters = new Collection<string>();

                //ContentBLL.isCMFlyoutOpen = false; //CR 3600

                #region // (1) Check last Update
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
 
                #region Content Removed
                if (contentRemoved)
                {
                    Hierarchy.VM.IsClosed = true;
                    Hierarchy.VM.Contents.Clear();
                    foreach (var i in _activeContents.OrderBy(x => x.seq))
                    {
                        Hierarchy.VM.Contents.Add(i); //Add new contents for insert query.
                    }
                }
                seqContentOverWriten = -1;
                #endregion

                #region //Remove certificate from DB after remove action.
                if (certificateRemove)
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


                #region //(6) Remove certificate last update.
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
                #endregion

                #region//Closed Version.
                if (Hierarchy.VM.IsClosed)
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

                #region clone Template

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
                #endregion


                foreach (UserCertificateApiModel uc in InitialUserCertificates)
                {
                    UserCertificateApiModel ucItemToRemove = (UserCertificateApiModel)Hierarchy.UserCertificates.Where(x => x.UserCertificateId.Equals(uc.UserCertificateId)).FirstOrDefault();

                    if (ucItemToRemove != null && ucItemToRemove.IsNew == true)
                        Hierarchy.UserCertificates.Remove(ucItemToRemove);
                }


                foreach (string uc in InitialCertificates.Keys)
                {
                    String itemToRemove = Convert.ToString(Hierarchy.Certificates.Where(x => x.Equals(uc)).FirstOrDefault());
                    Hierarchy.Certificates.Remove(itemToRemove);
                }

                //IF step not selected
                if (_Hierarchy.IsNew && _Hierarchy.SelectedStep.Equals(" ")) {
                   //show Error
                    return;
                }

                int messageId = 131; //Update
                if (_Hierarchy.IsNew)
                {
                    messageId = 115; //Create
                }
                string Persisted = TemplateBLL.PersistProject(ref _Hierarchy);

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
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "AddClonedProject", this._Hierarchy);
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "OnShowTemplateDetailsReceived", this.Node);
                        Hierarchy.IsCloned = false;
                        this.Node.Hierarchy = hm;
                    }
                    else
                    {
                        //if (Hierarchy.IsClonedRelatedUpdate)
                            // UpdateGroupNode(_TreeNodes); //Update last update to the entire group.

                        //MessageMediator.NotifyColleagues(this.WorkspaceId + "ShowProjectDetails", this.Node);
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "ShowNewTemplate", this.Node);
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
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(messageId, ArgsList);
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "UpdateNode", this._Hierarchy);
                }
                else
                {
                    Domain.PersistenceLayer.AbortTrans();
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Persisted), ArgsList);
                }

                contentRemoved = false;
                certificateRemove = false;
                Hierarchy.Certificates.Clear();
                certificateToRemove.Clear();
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
            return (Domain.IsPermitted("106"));
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
                        _activeContents.ToList().Clear();
                    }
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
                //if (SourceItemType.Contains("ContentModel") && DropCollectionType.Contains("ContentModel"))
                //{
                //    ContentModel SourceItem = DropInfo.Data as ContentModel;
                //    ContentModel DesItem = (ContentModel)DropInfo.TargetItem;
                //    if (DesItem == null)
                //        return;

                //    if (SourceItem != null)
                //    {
                //        DropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                //        DropInfo.Effects = DragDropEffects.Move;
                //    }
                //}
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
                #region CertificateModel
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

                    CMTreeViewVersionNodeViewModel SourceItem = DropInfo.Data as CMTreeViewVersionNodeViewModel;
                    //MsgBoxService.ShowInformation("Perform logic of adding content " + SourceItem.Name);
                    Boolean Status = this.CheckContentValidations(ref SourceItem);
                    if (Status == false)
                    {
                        ContentModel DragCM = new ContentModel(contentNamedarg, SourceItem.Name, SourceItem.TreeNode.ID, DateTime.Now.ToString(), "", contentCategoryDesc);
                        ContentBLL bll = new ContentBLL(Hierarchy.VM.VersionId);
                        DragCM.status = ContentManagementViewModel.versions[SourceItem.TreeNode.ID].Status.Name;
                        // _activeContents = bll.UpdateContents(ref DragCM);
                        Hierarchy.VM.Contents.Clear();
                        if (seqContentOverWriten > -1) {
                            DragCM.seq = seqContentOverWriten;
                        }
                        _activeContents = _activeContents.Concat(new[] { DragCM });
                        

                        foreach (var i in _activeContents)
                        {
                            Hierarchy.VM.Contents.Add(i);
                        }

                        int numOfContents = Hierarchy.VM.Contents.Count;

                        //Only for new , Not for replace
                        if (numOfContents >= 2 && seqContentOverWriten == -1)
                        {
                            ContentModel c = Hierarchy.VM.Contents.ElementAt(numOfContents - 1);
                            c.seq = Hierarchy.VM.Contents.ElementAt(numOfContents - 2).seq + 1;
                            Hierarchy.VM.Contents.RemoveAt(numOfContents - 1);
                            Hierarchy.VM.Contents.Add(c);
                        }
                        seqContentOverWriten = -1;
                        _activeContents = _activeContents.OrderBy(ac => ac.seq);

                        if (Hierarchy.VM.VersionName == InitialVersionName)
                        {
                            LastVersionName = Hierarchy.VM.VersionName;
                            //CR3483
                            defaultVersionName = VersionBLL.GenerateDefaultVersionName((int)Hierarchy.Id);
                            
                            if (defaultVersionName != string.Empty)
                            {
                                this.VM.VersionName = defaultVersionName;
                            }
                            else //if failed to generate - current functionality
                            {
                                this.VM.VersionName = Hierarchy.VM.VersionName;
                            }
                            //this.VM.VersionName = Hierarchy.VM.VersionName;
                            RaisePropertyChanged("VersionName");
                            //this.VM.Description = Hierarchy.VM.Description;
                            this.VM.Description = string.Empty;
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
                if (SourceItemType.Contains("ContentModel"))
                {
                    ContentModel SourceItem = DropInfo.Data as ContentModel;
                    ContentModel DesItem = (ContentModel)DropInfo.TargetItem;

                    if (SourceItem != null && DesItem != null) //replace sequence between content versions
                    {
                        int sourceSec = SourceItem.seq;
                        int desSec = DesItem.seq;

                        cannotUpdateActiveContents = false;

                        int t = activeContents.Where(x => x.id == contentToAction.id).First().seq;
                        t++;
                        foreach (ContentModel cm in activeContents)
                        {
                            if (cm.id == SourceItem.id)
                            {
                                activeContents.Where(x => x.id == cm.id).First().seq = desSec;
                            }
                            if (cm.id == DesItem.id)
                            {
                                activeContents.Where(x => x.id == cm.id).First().seq = sourceSec;
                            }
                        }
                        contentRemoved = true;
                        Hierarchy.VM.IsNew = true;

                        activeContents = activeContents.OrderBy(x => x.seq).ToList();
                        if (Hierarchy.VM.VersionName == InitialVersionName)
                        {
                            LastVersionName = Hierarchy.VM.VersionName;
                            //CR3483
                            defaultVersionName = VersionBLL.GenerateDefaultVersionName((int)Hierarchy.Id);

                            if (defaultVersionName != string.Empty)
                            {
                                this.VM.VersionName = defaultVersionName;
                            }
                            else //if failed to generate - current functionality
                            {
                                this.VM.VersionName = Hierarchy.VM.VersionName;
                            }
                            //this.Hierarchy.VM.VersionName = Hierarchy.VM.VersionName;
                            RaisePropertyChanged("VersionName");
                            //this.VM.Description = Hierarchy.VM.Description;
                            this.VM.Description = string.Empty;
                            RaisePropertyChanged("VersionDescription");
                            this.VM.EcrId = string.Empty;
                            RaisePropertyChanged("EcrId");
                        }
                        //LastVersionName = string.Empty;
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
                if (value.ToString() == LastVersionName)
                {
                    return false;
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
                if (Hierarchy != null)
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
         [Required(ErrorMessage = "'Target Path' field is required.")]
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


        public string getTargetPath()
        {
            var Target = new StringBuilder(string.Empty);
            try
            {

                var SysPathQry = new StringBuilder(string.Empty);

                SysPathQry.Append("select Value from PE_SystemParameters where Variable='ProjectLocalPath'");

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


            Target.Append("/" + Hierarchy.Name.ToString().Trim() + "/" + VM.VersionName.ToString().Trim());


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

        #endregion

        #endregion

        private void OnTemplateRapidExecutionStartReceived(TreeViewTemplateNodeViewModel TP)
        {
            tabIndex = 0;
            RaisePropertyChanged("tabIndex");
            ProgressText = "Performing required validations prior to copying files...";
        }

        //private void OnRapidExecutionReceived(TreeViewProjectNodeViewModel TP)
        //{

        //    ThreadStart threadStart = new ThreadStart(IncrementProgressCounter);
        //    Thread t = new Thread(threadStart);
        //    t.Start();
        //}

        private void OnTemplateRapidExecutionReceived(TreeViewTemplateNodeViewModel TP)
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

        private void OnTemplateProgressTextReceived(TreeViewTemplateNodeViewModel TP)
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

        private void OnTemplateClearViewModelReceived(TreeViewTemplateNodeViewModel TP)
        {
            ClearViewModel();
        }

        private void OnClearTemplateViewModelReceived(TreeViewTemplateNodeViewModel TV)
        {
            ClearViewModel();
        }

        private void OnDisableTemplateReceived(TreeViewTemplateNodeViewModel TP)
        {
            bool showNotesSideBar = true;

            bool isCloneRelated = false;

            Notes = new NotesControlViewModel(Hierarchy.Id, Hierarchy.Name, ref isCloneRelated, ref showNotesSideBar, this.WorkspaceId);
        }
        #endregion Content Execution

        #region Content Remove

        #region Data
        private Boolean contentRemoved { get; set; }
        private int seqContentOverWriten = -1;
        private Boolean seqChanged { get; set; }
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
            ValidateAndRemoveContent();
        }
        #endregion

        #region Verify that user is authorized to remove contents
        private void ValidateAndRemoveContent()
        {
            try
            {
                if (Domain.IsPermitted("106") || Domain.IsPermitted("999")) //Check for premittion
                {
                    RemoveContent(); //Check for related project
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
        private void RemoveContent()
        {
            //Get all the project active contents
            List<ContentModel> cm = _activeContents.ToList();
            cm.Remove(contentToAction); //Remove selected content
            int ContentID = 0;
            if (ContentsKeys != null && ContentsKeys.Count > 0)
            {
                ContentID = ContentManagementViewModel.versions[contentToAction.id].ParentID;
                if (ContentsKeys.ContainsKey(ContentID))
                {
                    ContentsKeys.Remove(ContentID);
                    versionsExistsKeys.Remove(contentToAction.id);
                    deleteFromContentLinksVersions(contentToAction.id, ContentID);                       
                }
            }

            //Refresh GUI after content removed
            _activeContents = cm;
            cannotUpdateActiveContents = false;
            CertificatesChanged = true;
            RaisePropertyChanged("activeContents");
            contentRemoved = true;
            if (Hierarchy.VM.VersionName == InitialVersionName)
            {
                LastVersionName = Hierarchy.VM.VersionName;
                //CR3483
                defaultVersionName = VersionBLL.GenerateDefaultVersionName((int)Hierarchy.Id);

                if (defaultVersionName != string.Empty)
                {
                    this.VM.VersionName = defaultVersionName;
                }
                else //if failed to generate - current functionality
                {
                    this.VM.VersionName = Hierarchy.VM.VersionName;
                }
                VMOld = Hierarchy.VM;
                //this.VM.VersionName = Hierarchy.VM.VersionName;
                RaisePropertyChanged("VersionName");
                //this.VM.Description = Hierarchy.VM.Description;
                this.VM.Description = string.Empty;
                RaisePropertyChanged("VersionDescription");
                this.VM.DefaultTargetPathInd = true;
                RaisePropertyChanged("DefaultTargetPathInd");
                this.VM.TargetPath = getTargetPath();
                RaisePropertyChanged("TargetPath");
                this.VM.EcrId = string.Empty;
                RaisePropertyChanged("EcrId");
            }
            //LastVersionName = string.Empty;
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
                                contentRemoved = true;
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
                //CR3600
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
            MessageMediator.NotifyColleagues(this.WorkspaceId + "ShowNewTemplate", this.Node);
           RaisePropertyChanged("Name");
        }//Register to recieve a message asking for node refresh
        
        private void OnExecuteLinkedVersionReceived(CMTreeViewVersionNodeViewModel CMVersion)
        {
            tabIndex = 0;
            RaisePropertyChanged("tabIndex");
        }//Register to recieve a message asking for node refresh

        #region GetAllProjectsFamily

       
        public ObservableCollection<HierarchyModel> GetALLProjectsFamily( TreeViewNodeViewModelBase Node)
        {
             ObservableCollection<HierarchyModel> ProjectChildren = new ObservableCollection<HierarchyModel>();

            foreach (var i in Node.Parent.Children)
            {
                //Verify that it is a template and not folder.
                if (i.NodeType == NodeTypes.T && i.Hierarchy.Id != Hierarchy.Id)
                {
                    ProjectChildren.Add(i.Hierarchy);

                }
            }
            return ProjectChildren;
        }


        #endregion GetAllProjectsFamily

        #region Update Execution Priority

        private RelayCommand _UpdateExecutionPriorityCommand;
        public ICommand UpdateExecutionPriorityCommand
        {
            get
            {
                if (_UpdateExecutionPriorityCommand == null)
                {
                    _UpdateExecutionPriorityCommand = new RelayCommand(ExecuteUpdateExecutionPriorityCommand, CanExecuteUpdateExecutionPriorityCommand);
                }
                return _UpdateExecutionPriorityCommand;
            }
        }

        private bool CanExecuteUpdateExecutionPriorityCommand()
        {
            return true;
            ////TODO : ELLA
            //if (_activeContents != null && _activeContents.Count() > 0)
            //    return true;
            //else
            //    return false;

        }

        private void ExecuteUpdateExecutionPriorityCommand()
        {
            try
            {
                IsReadOnlyMode = false;
                RaisePropertyChanged("IsReadOnly");
                _activeContents.Count();
            }
            catch (Exception ex) //Please see DAL Log file for more details: Shell --> View Log
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Object[] ArgsList = new Object[] { 0 };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(161, ArgsList);
            }


        }
        #endregion Update Execution Priority

        #region Update Execution Priority

        private  Boolean _IsReadOnlyMode = true;
        public Boolean IsReadOnlyMode
        {
            get
            {
                return _IsReadOnlyMode;
            }
            set
            {
                _IsReadOnlyMode = value;
                RaisePropertyChanged("IsReadOnly");
                RaisePropertyChanged("PriorityLabel");
            }
        }

        private string _PriorityLabel = "Set";
        public string PriorityLabel
        {
            get
            {
                return _PriorityLabel;
            }
            set
            {
                if (IsReadOnlyMode)
                {
                    _PriorityLabel = "Set";
                    RaisePropertyChanged("PriorityLabel");
                }
                else
                {
                    _PriorityLabel = "OK";
                    RaisePropertyChanged("PriorityLabel");
                }

            }
        }
        #endregion Update Execution Priority










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


        private static string _ExpenderRow = "23";
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
        #region CheckHierarchy
        public void CheckHierarchy()
        {
            //Hierarchy still 'dirty'. clone bot been saved
            if (Hierarchy.IsCloned == true && Hierarchy.IsNew == true && HideField == "Visible" && ShowField == "Collapsed")
            {
                Hierarchy.IsCloned = false;
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
                if (Hierarchy.IsClonedRelated == true && Hierarchy.IsDirty)
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


        public static void ExpandTreeOnFolderTarget(TreeViewNodeViewModelBase ParentNode)
        {
            ParentNode.IsExpandedTree = true;
            ParentNode.IsSelectedTree = true;
        }

    }

} //end of root namespace