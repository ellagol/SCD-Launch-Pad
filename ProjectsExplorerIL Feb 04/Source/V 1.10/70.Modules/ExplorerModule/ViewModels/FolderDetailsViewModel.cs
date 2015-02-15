using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ATSBusinessLogic;
using ATSBusinessObjects;
using ATSDomain;
using Infra.DragDrop;
using Infra.MVVM;
using NotesModule;
using ResourcesProvider;

namespace ExplorerModule
{
    public class FolderDetailsViewModel : ViewModelBase, IDropTarget
    {

        #region  Data

        protected MessengerService MessageMediator = new MessengerService();
        private IMessageBoxService MsgBoxService = null;

        private IEnumerable<CertificateModel> _Certificates;

        Dictionary<string, string> InitialCertificates = new Dictionary<string, string>();

        ObservableCollection<UserCertificateApiModel> InitialUserCertificates = new ObservableCollection<UserCertificateApiModel>();

      //  private ObservableCollection<CertificateModel> certificateDisplayList;

        private Guid WorkspaceId;

       // public HierarchyModel initHM = new HierarchyModel();

        private HierarchyModel _Hierarchy;
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

        private bool _CertificatesChanged = false;
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
        public int tabIndex { get; set; }

        //fix 1869
        public static string DateTimeFormat
        {
            get
            {
                return Domain.DateTimeFormat;
            }
        }

        public NotesControlViewModel Notes { get; set; }

        #endregion

        #region Presentation Properties

        public class FolderInvalidNameAttribute : ValidationAttribute
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


        [FolderInvalidName(ErrorMessage = "Folder name can't contain any of the following characters: \\ / * ? \" < > |, tab, new line")] 
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
                //CertificatesChanged = true;
                if (_Hierarchy != null)
                {
                    _Hierarchy.Name = value;
                    RaisePropertyChanged("Name");
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
                //CertificatesChanged = true;
                if (_Hierarchy != null)
                {
                    _Hierarchy.Description = value;
                    RaisePropertyChanged("Description");
                    Hierarchy.IsDirty = true;
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
                }
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

        private TreeViewNodeViewModelBase Node;

        public ObservableCollection<TreeViewNodeViewModelBase> SubProjects
        {
            get
            {
                return Node.Children;
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

        #endregion

        #region Constructor

        public FolderDetailsViewModel(Guid workspaceId, TreeViewNodeViewModelBase TV)
        {
            tabIndex = 0;
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();
            // Double-Click
            _DoubleClickCommand = new RelayCommand<object>(ExecuteDoubleClickCommand);
            // Initialize Object
            this.WorkspaceId = workspaceId;
            Node = TV;
            Hierarchy = TV.Hierarchy;

            //initialize folder unsaved properties


            //initialize project certificates
            Hierarchy.Certificates.Clear();

            //initialize folder certificate display list
            //certificateDisplayList = new ObservableCollection<CertificateModel>();
            //CertificateBLL bll = new CertificateBLL(Hierarchy);
            //certificateDisplayList = bll.getAllCertificates();

            //initialize Notes side bar
            bool showNotesSideBar = true;

            bool isCloneRelated = false;

            long HierarchyId = -1;

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

            Notes = new NotesControlViewModel(HierarchyId, TV.Name, ref isCloneRelated, ref showNotesSideBar, this.WorkspaceId);
            ShowNotes = showNotesSideBar;
            certificateDataFiller();

            InitialCertificates.Clear();
            CertificateBLL.CertificateBLLReturnResult getCertstatus = CertificateBLL.GetNodeCertificatesDB(Hierarchy, out InitialCertificates);
            if (getCertstatus != CertificateBLL.CertificateBLLReturnResult.Success)
            {
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, new object[] { 0 });
            }

            getSequence();
            certificateToRemove = new List<CertificateModel>();
            UserCertificateFiller();
            InitialUserCertificates.Clear();
            foreach (UserCertificateApiModel uc in Hierarchy.UserCertificates)
            {
                InitialUserCertificates.Add(uc);
            }
            MessageMediator.Register(this.WorkspaceId + "OnSaveFolderReceived", new Action<TreeViewFolderNodeViewModel>(OnSaveFolderReceived)); //Register to recieve a message asking for node refresh
        }

        #endregion

        #region  Commands

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
            //Verify user is permitted to update node properties
            if (!Domain.IsPermitted("123")) 
            {
                return false;
            }

            FolderInvalidNameAttribute fNameInvAttr = new FolderInvalidNameAttribute();
            if (!fNameInvAttr.IsValid(Name))
                return false;

            //check if fields are not the same 
            if (Hierarchy.IsDirty)
            {
                return true;
            }

            if ((String.IsNullOrEmpty(Name) || Name.Length > 30) || (Description.Length > 500))
                return false;
            else if (CertificatesChanged)
                return true;
            else
                return false;
                //return (IsValid);
        }

        private void ExecuteSaveCommand()
        {
            try
            {
                Domain.PersistenceLayer.BeginTransWithIsolation(IsolationLevel.Serializable);

                // Prior to updating, check if object has changed since it was loaded and alert the user if it has
                if (!Hierarchy.IsNew)
                {
                    string LastUpdateCheck = HierarchyBLL.LastUpadateCheck(ref _Hierarchy);
                    if (!(String.IsNullOrEmpty(LastUpdateCheck)))
                    {

                        Domain.PersistenceLayer.AbortTrans();
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(LastUpdateCheck), ArgsList);
                        return;
                    }
                }
               
                if (certificateRemove)
                {
                    //string CertificateLastUpdateCheck = CertificateBLL.CheckLastUpdateCertificateDelete(Certificates, Hierarchy.Id, certificateToRemove);
                    
                    string CertificateLastUpdateCheck = CertificateBLL.CheckLastUpdateCertificate(InitialCertificates, Hierarchy.Id);
                    if (!(String.IsNullOrEmpty(CertificateLastUpdateCheck)))
                    {

                        Domain.PersistenceLayer.AbortTrans();
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(CertificateLastUpdateCheck), ArgsList);
                        return;
                    }
                    //Delete certificate function
                    if (!deleteCertificate())
                    {

                        Domain.PersistenceLayer.AbortTrans();
                        return;
                    }
                    else
                    {
                        certificateToRemove.Clear();
                        certificateRemove = false;
                        //Refresh initial certificates list after delete
                        InitialCertificates.Clear();
                        CertificateBLL.CertificateBLLReturnResult getCertstatus = CertificateBLL.GetNodeCertificatesDB(Hierarchy, out InitialCertificates);
                        if (getCertstatus != CertificateBLL.CertificateBLLReturnResult.Success)
                        {
                            ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, new object[] { 0 });
                        }
                        MessageMediator.NotifyColleagues(this.WorkspaceId + "UpdateNode", this._Hierarchy);
                    }

                }

                if (UserCertificatesRemoved.Count > 0)
                {
                    //string UserCertificateLastUpdateCheck = UserCertificateBLL.CheckLastUpdateUserCertificateDelete(Hierarchy.UserCertificates, Hierarchy.Id, UserCertificatesRemoved);

                    string UserCertificateLastUpdateCheck = UserCertificateBLL.CheckLastUpdateUserCertificate(InitialUserCertificates, Hierarchy.Id);
                    if (!(String.IsNullOrEmpty(UserCertificateLastUpdateCheck)))
                    {

                        Domain.PersistenceLayer.AbortTrans();
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(UserCertificateLastUpdateCheck), ArgsList);
                        return;
                    }
                    //Remove user certificate from DB
                    string UserPersisted = string.Empty;
                    UserPersisted = UserCertificateBLL.DeleteUserCertificate(UserCertificatesRemoved, Hierarchy.Id);
                    if (!string.IsNullOrEmpty(UserPersisted))
                    {
                        //failed to remove user certificate.
                        Domain.PersistenceLayer.AbortTrans();
                        Object[] UserArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(UserPersisted), UserArgsList);
                        String logMessage = "Error occured when trying to delete User Certificates.";
                        ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                        return;
                    }
                }

                //Temporary solution for add user certificate - no last update check.

                //(7) add user certificate last update.
                //string AddUserCertificateLastUpdateCheck = UserCertificateBLL.CheckLastUpdateUserCertificateAdd(Hierarchy.UserCertificates, Hierarchy.Id);
                //if (!(String.IsNullOrEmpty(AddUserCertificateLastUpdateCheck)))
                //{

                //    Domain.PersistenceLayer.AbortTrans();
                //    Object[] ArgsList = new Object[] { 0 };
                //    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(AddUserCertificateLastUpdateCheck), ArgsList);
                //    return;
                //}

                // Work variables
                Collection<string> StatusBarParameters = new Collection<string>();

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

                // Execute Save command
                string Persisted = HierarchyBLL.PersistHierarchyRow(ref _Hierarchy);
                // Update Statusbar
                if (Persisted.Equals(string.Empty))
                {
                    Domain.PersistenceLayer.CommitTrans();
                    Object[] ArgsList = new Object[] { Hierarchy.Name };
                    CertificatesChanged = false;

                    certificateDataFiller();

                    certificateRemove = false;
                    Hierarchy.Certificates.Clear();
                    certificateToRemove.Clear();

                    //Populate Initial Certificates again after save
                    InitialCertificates.Clear();
                    CertificateBLL.CertificateBLLReturnResult getCertstatus = CertificateBLL.GetNodeCertificatesDB(Hierarchy, out InitialCertificates);
                    if (getCertstatus != CertificateBLL.CertificateBLLReturnResult.Success)
                    {
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, new object[] { 0 });
                    }

                    MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", null);
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(114, ArgsList);
                    InitialUserCertificates.Clear();
                    UserCertificateFiller();
                    foreach (UserCertificateApiModel uc in Hierarchy.UserCertificates)
                    {
                        InitialUserCertificates.Add(uc);
                    }

                    // Refresh the relevant TreeView node
                    MessageMediator.NotifyColleagues(this.WorkspaceId + "UpdateNode", this._Hierarchy); //Send message to the Explorer
                    // Refresh Fields
                    RaisePropertyChanged("CreationDate");
                    RaisePropertyChanged("LastUpdateTime");
                    RaisePropertyChanged("LastUpdateUser");
                   
                }
                else
                {
                    if (Persisted == "100" || Persisted == "104" || Persisted == "109" || Persisted == "141")
                    {
                        Domain.PersistenceLayer.AbortTrans();
                        //showMessage(Convert.ToInt32(Persisted));
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Persisted), ArgsList);
                    }
                    else
                    {
                        Domain.PersistenceLayer.AbortTrans();
                        StatusBarParameters.Add("Error: " + Persisted + ". Correct and re-submit"); //Message
                        StatusBarParameters.Add("White"); //Foreground
                        StatusBarParameters.Add("Red"); //Background
                        MessageMediator.NotifyColleagues("StatusBarParameters", StatusBarParameters); //Send message to the MainViewModel
                    }
                }
            }
            catch (Exception E)
            {
                Domain.PersistenceLayer.AbortTrans();
                if (E.Message == "DB Error")
                {
                    String logMessage = E.Message + "\n" + "Source: " + E.Source + "\n" + E.StackTrace;
                    ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(141, ArgsList);
                }
                else
                {
                    String logMessage = E.Message + "\n" + "Source: " + E.Source + "\n" + E.StackTrace;
                    ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
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
            return false;
        }

        private void ExecuteAddContentCommand()
        {
            // tabIndex = 2;
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
                //CertificatesChanged = false;
                return true;
            }
            else
                return false;
        }

        private void ExecuteAddCertCommand()
        {
            tabIndex = 2;
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
            tabIndex = 2;
            RaisePropertyChanged("tabIndex");
            MessageMediator.NotifyColleagues("ShowAddUserCertificate", WorkspaceId); //Send message to the MainViewModel
        }

        #endregion

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

        #endregion //Commands

        #region  Mouse DoubleClick EventToCommand

        private RelayCommand<object> _DoubleClickCommand;
        public RelayCommand<object> DoubleClickCommand
        {
            get
            {
                return _DoubleClickCommand;
            }
            set
            {
                _DoubleClickCommand = value;
            }
        }

        private void ExecuteDoubleClickCommand(object Param)
        {

            int I = SubProjects.IndexOf((TreeViewNodeViewModelBase)Param);
            TreeViewNodeViewModelBase TV = SubProjects[I];
            if (TV.GetType() == typeof(TreeViewFolderNodeViewModel))
            {
                MessageMediator.NotifyColleagues(WorkspaceId + "RequestSubFolderDrillDown", TV); //Will be returned to the ProjectsExplorer signed for this message
                MessageMediator.NotifyColleagues(WorkspaceId + "OnIsSelectedNodeReceived", TV);
            
            }
            else if (TV.GetType() == typeof(TreeViewProjectNodeViewModel))
            {
                TV.Parent.IsExpanded = true;
                TV.IsSelected = true;
                MessageMediator.NotifyColleagues(WorkspaceId + "ShowProjectDetails", TV); 
            }
            else if (TV.GetType() == typeof(TreeViewTemplateNodeViewModel))
            {
                TV.Parent.IsExpanded = true;
                TV.IsSelected = true;
                MessageMediator.NotifyColleagues(WorkspaceId + "ShowNewTemplate", TV); 
            }
        }

        #endregion

        #region Certificate Tab

        #region Data Filler

        public void certificateDataFiller()
        {
            try
            {
                IsCertificateDeleteEnabled = true;
                CertificateBLL bll = new CertificateBLL(Hierarchy);
                _Certificates = bll.getAllCertificates();
            }
            catch (Exception e)
            {
                Object[] ArgsList = new Object[] { 0 };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(136, ArgsList);    
            }

        }

        public IEnumerable<CertificateModel> Certificates
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
        public Boolean IsCertificateDeleteEnabled { get; set; }

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

        private bool CanExecuteDeleteCertificateCommand()
        {
            if (selectedCertificate is CertificateModel && selectedCertificate !=null)
                return true;
            else
                return false;
        }

        private Boolean certificateRemove = false;
        private List<CertificateModel> certificateToRemove;

        private void ExecuteDeleteCertificateCommand()
        {
            List<CertificateModel> cm = new List<CertificateModel>();
            
            try
            {
                certificateDataFiller();
                certificateRemove = true;
                Hierarchy.Certificates.RemoveAll(x => x == selectedCertificate.key);
                cm = _Certificates.Where(x => !x.key.Equals(selectedCertificate.key)).ToList();
                //List<CertificateModel> cm = _Certificates.ToList();
                CertificatesChanged = true;

                certificateToRemove.Add(selectedCertificate); //Add selected certificate to remove list  
                foreach (CertificateModel c in certificateToRemove)
                    cm.RemoveAll(x => x.key == c.key); //Remove selected certificate from gui  
                Certificates = cm;
                RaisePropertyChanged("Certificates");
                MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsDirtyNodeReceived", this.Node);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
            }
        }


        private Boolean deleteCertificate()
        {
            try
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
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
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
            if (selectedCertificate != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //private void ExecuteStationCertificateStationCommand()
        //{
        //    MessageMediator.NotifyColleagues("ShowStationCertificateList", "GUY");
        //    //   MessageMediator.NotifyColleagues(this.WorkspaceId + "ShowStationCertificateList", this.Node);  
        //}

        #endregion Station Certificate Stations
        #endregion Certificate Tab

        #region  IDropTarget Members & Other Drop Activities

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
            }
            catch (Exception ex)
            {
                MsgBoxService.ShowError("Error:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public void Drop(Infra.DragDrop.IDropInfo DropInfo)
        {
            try
            {
                string SourceItemType = DropInfo.Data.GetType().ToString();
                string DropCollectionType = DropInfo.TargetCollection.GetType().ToString();
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
                            Object[] ArgsList = new Object[] {0};                            
                            ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(109, ArgsList); 
                            return;
                        }
                    }

                    var newInfo = new Certificate(SourceItem.CerName, SourceItem.Description);
                    //certificateDisplayList.Add(new CertificateModel(newInfo, SourceItem.key));
                    //_Certificates = certificateDisplayList;
                    CertificateModel NewCM = new CertificateModel(newInfo, SourceItem.key, DateTime.Now.ToString());
                    NewCM.IsNew = true;
                   // _Certificates = _Certificates.Concat(new[] { new CertificateModel(newInfo, SourceItem.key, DateTime.Now.ToString()) });
                    _Certificates = _Certificates.Concat(new[] { NewCM });
                    //CertificatesChanged = false;

                    CertificatesChanged = true;
                    
                    this.Certificates.ToList();
                    Hierarchy.Certificates.Add(SourceItem.key);

                    CertificateModel ItemToRemove = new CertificateModel();
                    ItemToRemove = certificateToRemove.FirstOrDefault(x => x.key == SourceItem.key);

                    certificateToRemove.Remove(ItemToRemove);

                    RaisePropertyChanged("Certificates");
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
                        Hierarchy.UserCertificates.Add(SourceItem);

                        UserCertificateApiModel UCItemToRemove = new UserCertificateApiModel();
                        UCItemToRemove = UserCertificatesRemoved.FirstOrDefault(x => x.UserCertificateId == SourceItem.UserCertificateId);

                        UserCertificatesRemoved.Remove(UCItemToRemove);

                        CertificatesChanged = true;
                        RaisePropertyChanged("UserCertificates");

                    }
                    else
                    {
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(106, ArgsList);
                    }
                }
            }
            catch (Exception ex)
            {
                MsgBoxService.ShowError("Error:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

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

        #region getSequence

        private void getSequence()
        {
            if (Hierarchy.IsNew == true)
            {
                var SB = new StringBuilder(string.Empty);
                SB.Append("select count(Id) + 1 from PE_Hierarchy WHERE ParentId = '" + Hierarchy.ParentId + "'");
                Int16 Count = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));
                Hierarchy.Sequence = Count;
            }
        }

        #endregion getSeqence

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
            catch (Exception e)
            {
                showMessage(105);
            }

            return false;
        }

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
            }
        }

        #endregion UserCertificate Tab

        private void OnSaveFolderReceived(TreeViewFolderNodeViewModel TV)
        {
            if (Domain.IsPermitted("123"))
            {
                this.ExecuteSaveCommand();
            }
        }
    }

} //end of root namespace