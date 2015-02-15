using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO; //To support ImageSource property that holds the Thumb of the Workspace
using System.Windows.Media.Imaging; //To support ImageSource property that holds the Thumb of the Workspace
using ATSDomain;
using ContentMgmtModule;
using ExplorerModule;
using Infra.MVVM;
using UserCertModule;


namespace ATSVM
{
    public class MainWindowViewModel : ViewModelBase
    {

        #region  Fields

        private IMessageBoxService MsgBoxService = null;
        private ITaskDialogService TaskDialogService = null;
        private IDialogService DialogService = null;
        private MessengerService MessageMediator = null;

        #endregion

        #region  Properties

        private bool _LeftFlyoutOpen = false;
        public bool LeftFlyoutOpen
        {
            get
            {
                return _LeftFlyoutOpen;
            }
            set
            {
                _LeftFlyoutOpen = value;
            }
        }

        private bool _TopFlyoutOpen = false;
        public bool TopFlyoutOpen
        {
            get
            {
                return _TopFlyoutOpen;
            }
            set
            {
                _TopFlyoutOpen = value;
            }
        }

        private bool _RightFlyoutOpen = false;
        public bool RightFlyoutOpen
        {
            get
            {
                return _RightFlyoutOpen;
            }
            set
            {
                _RightFlyoutOpen = value;
            }
        }

        private bool _BottomFlyoutOpen = false;
        public bool BottomFlyoutOpen
        {
            get
            {
                return _BottomFlyoutOpen;
            }
            set
            {
                _BottomFlyoutOpen = value;
            }
        }

        private string _RibbonAndStatusBackgroundBrush = "#FFDFE9F5";
        public string RibbonAndStatusBackgroundBrush
        {
            get
            {
                return _RibbonAndStatusBackgroundBrush;
            }
            set
            {
                _RibbonAndStatusBackgroundBrush = value;
            }
        }

        private string _RibbonAndStatusForegroundBrush = "Black";
        public string RibbonAndStatusForegroundBrush
        {
            get
            {
                return _RibbonAndStatusForegroundBrush;
            }
            set
            {
                _RibbonAndStatusForegroundBrush = value;
            }
        }

        private int _WorkspacesMargin = 0;
        public int WorkspacesMargin
        {
            get
            {
                return _WorkspacesMargin;
            }
            set
            {
                _WorkspacesMargin = value;
                RaisePropertyChanged("WorkspacesMargin");
            }
        }

        private int _WorkspacesTabItemWidth = 140;
        public int WorkspacesTabItemWidth
        {
            get
            {
                return _WorkspacesTabItemWidth;
            }
            set
            {
                _WorkspacesTabItemWidth = value;
                RaisePropertyChanged("WorkspacesTabItemWidth");
            }
        }

        private string _StatusForegroundBrush;
        public string StatusForegroundBrush
        {
            get
            {
                return _StatusForegroundBrush;
            }
            set
            {
                _StatusForegroundBrush = value;
                RaisePropertyChanged("StatusForegroundBrush");
            }
        }

        private string _StatusBackgroundBrush;
        public string StatusBackgroundBrush
        {
            get
            {
                return _StatusBackgroundBrush;
            }
            set
            {
                _StatusBackgroundBrush = value;
                RaisePropertyChanged("StatusBackgroundBrush");
            }
        }

        private string _StatusMessage = string.Empty;
        public string StatusMessage
        {
            get
            {
                return _StatusMessage;
            }
            set
            {
                _StatusMessage = value;
                RaisePropertyChanged("StatusMessage");
            }
        }

        public bool IsWorkspacesTabVisible
        {
            get
            {
                return Workspaces.Count > 0;
            }
        }

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

        private string _MainWindowTitle = "SCD Launch Pad";
        public string MainWindowTitle
        {
            get
            {
                return _MainWindowTitle;
            }
            set
            {
                _MainWindowTitle = value;
                RaisePropertyChanged("MainWindowTitle");
            }
        }

        private string _RightFlyoutTitle = "Right Flyout Title";
        public string RightFlyoutTitle
        {
            get
            {
                return _RightFlyoutTitle;
            }
            set
            {
                _RightFlyoutTitle = value;
                RaisePropertyChanged("RightFlyoutTitle");
            }
        }

        private ViewModelBase _RightFlyoutViewModel = null;
        public ViewModelBase RightFlyoutViewModel
        {
            get
            {
                return _RightFlyoutViewModel;
            }
            set
            {
                _RightFlyoutViewModel = value;
                RaisePropertyChanged("RightFlyoutViewModel");
            }
        }

        private string _BottomFlyoutTitle = "Bottom Flyout Title";
        public string BottomFlyoutTitle
        {
            get
            {
                return _BottomFlyoutTitle;
            }
            set
            {
                _BottomFlyoutTitle = value;
                RaisePropertyChanged("BottomFlyoutTitle");
            }
        }

        private ViewModelBase _BottomFlyoutViewModel = null;
        public ViewModelBase BottomFlyoutViewModel
        {
            get
            {
                return _BottomFlyoutViewModel;
            }
            set
            {
                _BottomFlyoutViewModel = value;
                RaisePropertyChanged("BottomFlyoutViewModel");
            }
        }

        private bool _isOpenWorkspaceEnabled = true;
        public bool isOpenWorkspaceEnabled
        {
            get
            {
                return _isOpenWorkspaceEnabled;
            }
            set
            {
                _isOpenWorkspaceEnabled = value;
                RaisePropertyChanged("isOpenWorkspaceEnabled");
            }
        }

        #endregion

        #region  Constructor

        public MainWindowViewModel()
            : base()
        //Call base class; will register all known service types (MessengerService; MessageBox; Dialog)
        {
            //DialogService (to display popups); This is not used in this application - I am using Overlays
            DialogService = GetService<IDialogService>();
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //Task Dialog Service
            TaskDialogService = GetService<ITaskDialogService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();
            //Register to recieve a message containing StatusBar Parameters
            MessageMediator.Register("StatusBarParameters", new Action<Collection<string>>(OnStatusBarParametersReceived));
            //Register to recieve a message containing Workspace to go to Parameter
            MessageMediator.Register("RequestGotoWorkspace", new Action<string>(OnRequestGotoWorkspaceReceived));
            //Register to recieve a message containing Login information
            MessageMediator.Register("RequestLogin", new Action<string>(OnRequestLoginReceived));
            //Register to recieve a message requesting display of Content Flyout
            MessageMediator.Register("ShowAddContent", new Action<Guid>(OnRequestShowContentReceived));
            //Register to recieve a message requesting display of Find Content Flyout (Bulk Update
            MessageMediator.Register("ShowFindContent", new Action<object[]>(OnRequestShowFindContentReceived));
            //Register to recieve a message requesting display of Replace Content Flyout (Bulk Update
            MessageMediator.Register("ShowReplaceContent", new Action<object[]>(OnRequestShowReplaceContentReceived));
            //Register to recieve a message requesting display of Certificates Flyout
            MessageMediator.Register("ShowAddCertificate", new Action<Guid>(OnRequestShowCertificatesReceived));
            //Register to recieve a message requesting display of Search Parameters Flyout
            MessageMediator.Register("ShowSearchParams", new Action<Guid>(OnRequestShowSearchParamsReceived));
            //Register to recieve a message requesting hide of Search Parameters Flyout
            MessageMediator.Register("ShowCMSearchParams", new Action<Guid>(OnRequestShowCMSearchParamsReceived));
            //Register to recieve a message requesting hide of Search Parameters Flyout
            MessageMediator.Register("HideSearchParams", new Action(OnRequestHideSearchParamsReceived));
            //Register to recieve a message requesting hide all Flyouts
            MessageMediator.Register("HideFlyouts", new Action(OnRequestHideFlyoutsReceived));
            //Delete last run screenshots (if any)
            MessageMediator.Register("ShowAddUserCertificate", new Action<Guid>(OnRequestShowUserCertificatesReceived));
            //Register to recieve a message requesting display of Search Parameters Flyout
            //Register to recieve a station certificate list popup
            MessageMediator.Register("ShowStationCertificateList", new Action<String>(OnRequestShowStationCertificateListReceived));
            //Register to recieve a message containing Workspace to go to Parameter
            MessageMediator.Register("RequestGotoCm", new Action<object>(OnRequestGotoCmReceived));
            //Register to recieve a message containing Workspace to go to Parameter
            MessageMediator.Register("RequestGotoPeProject", new Action<object>(OnRequestGotoPeProjectReceived));
            //Register to recieve a message requesting display of progress bar
            MessageMediator.Register("ShowCmProgressBar", new Action(OnRequestShowCmProgressBarReceived));
            //Register to recieve a message requesting close progress bar
            MessageMediator.Register("CloseCmProgressBar", new Action(OnRequestCloseCmProgressBarReceived));
            //Lock-unlock ribbon bar when exporting data to another environment
            MessageMediator.Register("LockRibbonBar", new Action(OnLockRibbonBarReceived));
            MessageMediator.Register("UnLockRibbonBar", new Action(OnUnLockRibbonBarReceived));

            foreach (string Screenshot in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.WSID.png"))
            {
                File.Delete(Screenshot);
            }
            //Initializations
            _StatusForegroundBrush = RibbonAndStatusForegroundBrush;
            _StatusBackgroundBrush = RibbonAndStatusBackgroundBrush;
            _WorkspacesListDoubleClickCommand = new RelayCommand<object>(ExecuteWorkspacesListDoubleClickCommand);
            //Show Login Overlay VM - Disabled for SCD
            //ExecuteShowLoginCommand();
            //Compose the Window Title
            MainWindowTitle += "          User: " + Domain.User;
            MainWindowTitle += "          Environment: " + Domain.Environment;
        }

        #endregion

        #region  Ribbon Activated Commands

        #region  Exit Application Command

        private RelayCommand _ExitApplicationCommand;
        public RelayCommand ExitApplicationCommand
        {
            get
            {
                if (_ExitApplicationCommand == null)
                {
                    _ExitApplicationCommand = new RelayCommand(ExecuteExitApplicationCommand, CanExecuteExitApplicationCommand);
                }
                return _ExitApplicationCommand;
            }
        }

        private bool CanExecuteExitApplicationCommand()
        {
            return true;
        }

        private void ExecuteExitApplicationCommand()
        {
            CloseWindow();
        }

        #endregion

        #region  Projects Explorer Command

        private RelayCommand _ProjectsExplorerCommand;
        public RelayCommand ProjectsExplorerCommand
        {
            get
            {
                if (_ProjectsExplorerCommand == null)
                {
                    _ProjectsExplorerCommand = new RelayCommand(ExecuteProjectsExplorerCommand, CanExecuteProjectsExplorerCommand);
                }
                return _ProjectsExplorerCommand;
            }
        }

        private bool CanExecuteProjectsExplorerCommand()
        {
            return true;
        }

        private void ExecuteProjectsExplorerCommand()
        {
            WorkspaceViewModelBase WSVM = new ProjectsExplorerViewModel();
            AddWorkspace(WSVM);
        }

        #endregion

        #region  Users Certificates Command

        private RelayCommand _UsersCertificatesCommand;
        public RelayCommand UsersCertificatesCommand
        {
            get
            {
                if (_UsersCertificatesCommand == null)
                {
                    _UsersCertificatesCommand = new RelayCommand(ExecuteUsersCertificatesCommand, CanExecuteUsersCertificatesCommand);
                }
                return _UsersCertificatesCommand;
            }
        }

        private bool CanExecuteUsersCertificatesCommand()
        {
            return true;
        }

        private void ExecuteUsersCertificatesCommand()
        {

            WorkspaceViewModelBase WSVM = new UsersCertificatesViewModel();
            AddWorkspace(WSVM);

        }

        #endregion

        #region  Content Management Command

        private RelayCommand _ContentManagementCommand;
        public RelayCommand ContentManagementCommand
        {
            get
            {
                if (_ContentManagementCommand == null)
                {
                    _ContentManagementCommand = new RelayCommand(ExecuteContentManagementCommand, CanExecuteContentManagementCommand);
                }
                return _ContentManagementCommand;
            }
        }

        private bool CanExecuteContentManagementCommand()
        {
            return true;
        }

        private void ExecuteContentManagementCommand()
        {
            try
            {
                WorkspaceViewModelBase WSVM = new ContentMgmtModule.CMContentManagementViewModel();
                AddWorkspace(WSVM);
            }
            catch (Exception)
            {
                Collection<string> StatusBarParameters = new Collection<string>();
                StatusBarParameters.Clear();
                StatusBarParameters.Add("Internal error ocurred. Please see Data Access log file for more details: Shell->View Log."); //Message
                StatusBarParameters.Add("White"); //Foreground
                StatusBarParameters.Add("Red"); //Background
                MessageMediator.NotifyColleagues("StatusBarParameters", StatusBarParameters); //Send message to the MainViewModel
 
            }
        }

        #endregion

        #region  Content Management On Content Command

        private void ExecuteContentManagementOnContentCommand(object contentVersion)
        {
            WorkspaceViewModelBase WSVM = new ContentMgmtModule.CMContentManagementViewModel(contentVersion);
            AddWorkspace(WSVM);
        }

        #endregion

        #region  Project Explorer On Project Command

        private void ExecuteProjectExplorerOnProjectCommand(object project)
        {
            WorkspaceViewModelBase WSVM = new ProjectsExplorerViewModel(project);
            AddWorkspace(WSVM);
        }

        #endregion

        #region  Workspaces Command

        private RelayCommand _WorkspacesCommand;
        public RelayCommand WorkspacesCommand
        {
            get
            {
                if (_WorkspacesCommand == null)
                {
                    _WorkspacesCommand = new RelayCommand(ExecuteWorkspacesCommand, CanExecuteWorkspacesCommand);
                }
                return _WorkspacesCommand;
            }
        }

        private bool CanExecuteWorkspacesCommand()
        {
            return true;
        }

        private void ExecuteWorkspacesCommand()
        {
            WorkspaceViewModelBase WSVM = new WorkspacesViewModel(Workspaces);
            AddWorkspace(WSVM);
        }

        #endregion

        #region  Workspaces Flyout Command

        private RelayCommand _WorkspacesFlyoutCommand;
        public RelayCommand WorkspacesFlyoutCommand
        {
            get
            {
                if (_WorkspacesFlyoutCommand == null)
                {
                    _WorkspacesFlyoutCommand = new RelayCommand(ExecuteWorkspacesFlyoutCommand, CanExecuteWorkspacesFlyoutCommand);
                }
                return _WorkspacesFlyoutCommand;
            }
        }

        private bool CanExecuteWorkspacesFlyoutCommand()
        {
            return true;
        }

        private void ExecuteWorkspacesFlyoutCommand()
        {
            foreach (WorkspaceViewModelBase WS in Workspaces)
            {
                string WSImagePath = WS.WSId.ToString() + ".WSID.png";
                WS.Thumb = GetWorkspaceThumb(WSImagePath);
            }
            //
            LeftFlyoutOpen = true;
            RaisePropertyChanged("LeftFlyoutOpen");
        }

        private RelayCommand<object> _WorkspacesListDoubleClickCommand;
        public RelayCommand<object> WorkspacesListDoubleClickCommand
        {
            get
            {
                return _WorkspacesListDoubleClickCommand;
            }
            set
            {
                _WorkspacesListDoubleClickCommand = value;
            }
        }

        private void ExecuteWorkspacesListDoubleClickCommand(object Param)
        {
            CurrentWorkspace = Workspaces.IndexOf((WorkspaceViewModelBase)Param);
        }

        #endregion

        #region  ViewErrorLog Command

        private RelayCommand _ViewErrorLogCommand;
        public RelayCommand ViewErrorLogCommand
        {
            get
            {
                if (_ViewErrorLogCommand == null)
                {
                    _ViewErrorLogCommand = new RelayCommand(ExecuteViewErrorLogCommand, CanExecuteViewErrorLogCommand);
                }
                return _ViewErrorLogCommand;
            }
        }

        private bool CanExecuteViewErrorLogCommand()
        {
            return true;
        }

        private void ExecuteViewErrorLogCommand()
        {
            WorkspaceViewModelBase WSVM = new ErrorLogViewModel();
            AddWorkspace(WSVM);
        }

        #endregion

        #region  Flyouts

        #region  Left Flyout Command

        private RelayCommand _ToggleLeftFlyoutCommand;
        public RelayCommand ToggleLeftFlyoutCommand
        {
            get
            {
                if (_ToggleLeftFlyoutCommand == null)
                {
                    _ToggleLeftFlyoutCommand = new RelayCommand(ExecuteToggleLeftFlyoutCommand, CanExecuteToggleLeftFlyoutCommand);
                }
                return _ToggleLeftFlyoutCommand;
            }
        }

        private bool CanExecuteToggleLeftFlyoutCommand()
        {
            return true;
        }

        private void ExecuteToggleLeftFlyoutCommand()
        {
            LeftFlyoutOpen = !LeftFlyoutOpen;
            RaisePropertyChanged("LeftFlyoutOpen");
        }

        #endregion

        #region  Top Flyout Command

        private RelayCommand _ToggleTopFlyoutCommand;
        public RelayCommand ToggleTopFlyoutCommand
        {
            get
            {
                if (_ToggleTopFlyoutCommand == null)
                {
                    _ToggleTopFlyoutCommand = new RelayCommand(ExecuteToggleTopFlyoutCommand, CanExecuteToggleTopFlyoutCommand);
                }
                return _ToggleTopFlyoutCommand;
            }
        }

        private bool CanExecuteToggleTopFlyoutCommand()
        {
            return true;
        }

        private void ExecuteToggleTopFlyoutCommand()
        {
            TopFlyoutOpen = !TopFlyoutOpen;
            RaisePropertyChanged("TopFlyoutOpen");
        }

        #endregion

        #region  Right Flyout Command

        private RelayCommand _ToggleRightFlyoutCommand;
        public RelayCommand ToggleRightFlyoutCommand
        {
            get
            {
                if (_ToggleRightFlyoutCommand == null)
                {
                    _ToggleRightFlyoutCommand = new RelayCommand(ExecuteToggleRightFlyoutCommand, CanExecuteToggleRightFlyoutCommand);
                }
                return _ToggleRightFlyoutCommand;
            }
        }

        private bool CanExecuteToggleRightFlyoutCommand()
        {
            return true;
        }

        private void ExecuteToggleRightFlyoutCommand()
        {
            RightFlyoutOpen = !RightFlyoutOpen;
            RaisePropertyChanged("RightFlyoutOpen");
        }

        #endregion

        #region  Bottom Flyout Command

        private RelayCommand _ToggleBottomFlyoutCommand;
        public RelayCommand ToggleBottomFlyoutCommand
        {
            get
            {
                if (_ToggleBottomFlyoutCommand == null)
                {
                    _ToggleBottomFlyoutCommand = new RelayCommand(ExecuteToggleBottomFlyoutCommand, CanExecuteToggleBottomFlyoutCommand);
                }
                return _ToggleBottomFlyoutCommand;
            }
        }

        private bool CanExecuteToggleBottomFlyoutCommand()
        {
            return true;
        }

        private void ExecuteToggleBottomFlyoutCommand()
        {
            BottomFlyoutOpen = !BottomFlyoutOpen;
            RaisePropertyChanged("BottomFlyoutOpen");
        }

        #endregion

        #region  Close All Flyouts

        private void CloseAllFlyouts()
        {
            LeftFlyoutOpen = false;
            TopFlyoutOpen = false;
            RightFlyoutOpen = false;
            BottomFlyoutOpen = false;
            RaisePropertyChanged("LeftFlyoutOpen");
            RaisePropertyChanged("TopFlyoutOpen");
            RaisePropertyChanged("RightFlyoutOpen");
            RaisePropertyChanged("BottomFlyoutOpen");
        }

        #endregion

        #endregion

        #region About popup

        private RelayCommand _ShowAboutCommand;
        public RelayCommand ShowAboutCommand
        {
            get
            {
                if (_ShowAboutCommand == null)
                {
                    _ShowAboutCommand = new RelayCommand(ExecuteShowAboutCommand, CanExecuteShowAboutCommand);
                }
                return _ShowAboutCommand;
            }
        }

        private bool CanExecuteShowAboutCommand()
        {
            return true;
        }

        private void ExecuteShowAboutCommand()
        {
          //  ShowOverlayContent = true;
          //  OverlayContentViewModel = new AboutViewModel();
            string title = MainWindowTitle.Replace("         User: " + Domain.User, " ");
            MsgBoxService.ShowInformation(title + "\nVersion: " + ATSDomain.Domain.PE_Version);
         //   MsgBoxService.ShowInformation(this.MainWindowTitle+"\nVersion: " + ATSDomain.Domain.PE_Version);
        }

        #endregion

        #region Lock Ribbon
        private void OnLockRibbonBarReceived()
        {
            isOpenWorkspaceEnabled = false;
        }

        private void OnUnLockRibbonBarReceived()
        {
            isOpenWorkspaceEnabled = true;
        }

        #endregion Lock Ribbon

        #endregion

        #region  Methods

        //Receive a message from a VM requesting StatusBar display
        private void OnStatusBarParametersReceived(Collection<string> StatusBarParameters)
        {
            if (StatusBarParameters != null)
            {
                Int16 I = -1;
                foreach (string P in StatusBarParameters)
                {
                    I += 1;
                    if (P != null)
                    {
                        switch (I)
                        {
                            case 0:
                                StatusMessage = P;
                                break;
                            case 1:
                                StatusForegroundBrush = P;
                                break;
                            case 2:
                                StatusBackgroundBrush = P;
                                break;
                        }
                    }
                    else
                    {
                        switch (I)
                        {
                            case 0:
                                StatusMessage = string.Empty;
                                break;
                            case 1:
                                StatusForegroundBrush = RibbonAndStatusForegroundBrush;
                                break;
                            case 2:
                                StatusBackgroundBrush = RibbonAndStatusBackgroundBrush;
                                break;
                        }
                    }
                }
            }
            else //Reset
            {
                StatusMessage = string.Empty;
                StatusForegroundBrush = RibbonAndStatusForegroundBrush;
                StatusBackgroundBrush = RibbonAndStatusBackgroundBrush;
            }
        }


        //Receive a message from Thums display to go to a specific workspace
        private void OnRequestGotoWorkspaceReceived(string WSID)
        {
            int I = -1;
            foreach (WorkspaceViewModelBase WS in Workspaces)
            {
                I += 1;
                if (WS.WSId.ToString() == WSID)
                {
                    CurrentWorkspace = I;
                    break;
                }
            }
        }

        private void OnRequestGotoCmReceived(object cmObject)
        {
            ExecuteContentManagementOnContentCommand(cmObject);
        }

        private void OnRequestGotoPeProjectReceived(object cmObject)
        {
            ExecuteProjectExplorerOnProjectCommand(cmObject);
        }
     
        //Get Workspace Thumb from disk (as captured by the TabControl)
        public BitmapImage GetWorkspaceThumb(string Path)
        {
            BitmapImage SourceImage = new BitmapImage();
            if (File.Exists(Path))
            {
                if (new FileInfo(Path).Length != 0)
                {
                    SourceImage.BeginInit();
                    SourceImage.UriSource = new Uri(Path, UriKind.Relative);
                    SourceImage.CacheOption = BitmapCacheOption.OnLoad;
                    //SourceImage.DecodePixelHeight = 191
                    //SourceImage.DecodePixelWidth = 455
                    SourceImage.EndInit();
                }
            }
            return SourceImage;
        }

        //Receive a message requesting to show Content Flyout (data from CM to D&D onto Folder\Project)
        private void OnRequestShowContentReceived(Guid WSId)
        {
            RightFlyoutTitle = "Content Management";
            RightFlyoutViewModel = new ExplorerModule.ContentManagementViewModel(WSId);
            RightFlyoutOpen = true;
            RaisePropertyChanged("RightFlyoutOpen");
        }

        private void OnRequestShowFindContentReceived(object[] Parameters)
        {
            Guid WSId = (Guid)Parameters[0];
            TreeViewNodeViewModelBase Node = (TreeViewNodeViewModelBase)Parameters[1];

            RightFlyoutTitle = "Content Management";
            RightFlyoutViewModel = new ExplorerModule.ContentManagementFindViewModel(WSId, Node);
            RightFlyoutOpen = true;
            RaisePropertyChanged("RightFlyoutOpen");
        }

        private void OnRequestShowReplaceContentReceived(object[] Parameters)
        {
            Guid WSId = (Guid)Parameters[0];
            TreeViewNodeViewModelBase Node = (TreeViewNodeViewModelBase)Parameters[1];

            RightFlyoutTitle = "Content Management";
            RightFlyoutViewModel = new ExplorerModule.ContentManagementReplaceViewModel(WSId, Node);
            RightFlyoutOpen = true;
            RaisePropertyChanged("RightFlyoutOpen");
        }

        //Receive a message requesting to show Certificates Flyout (data from external system to D&D onto Folder\Project)
        private void OnRequestShowCertificatesReceived(Guid WSId)
        {
            RightFlyoutTitle = "Certificates";
            RightFlyoutViewModel = new CertificatesViewModel(WSId);
            RightFlyoutOpen = true;
            RaisePropertyChanged("RightFlyoutOpen");
        }

        //Receive a message requesting to show Certificates Flyout (data from external system to D&D onto Folder\Project)
        private void OnRequestShowUserCertificatesReceived(Guid WSId)
        {
            RightFlyoutTitle = "User Certificates";
            RightFlyoutViewModel = new UserCertificatesViewModel(WSId);
            RightFlyoutOpen = true;
            RaisePropertyChanged("RightFlyoutOpen");
        }

        private void OnRequestShowCmProgressBarReceived()
        {
            ShowOverlayContent = true;
            OverlayContentViewModel = new CMProgressBarViewModel();
        }

        private void OnRequestCloseCmProgressBarReceived()
        {
            ShowOverlayContent = false;
        }
        
        //Receive a message requesting to show Search Parameters Flyout
        private void OnRequestShowSearchParamsReceived(Guid WSId)
        {
            BottomFlyoutTitle = "Hierarchy Search Parameters";
            BottomFlyoutViewModel = new SearchViewModel(WSId);
            BottomFlyoutOpen = true;
            RaisePropertyChanged("BottomFlyoutOpen");
        }

        //Receive a message requesting to show Search Parameters Flyout
        private void OnRequestShowCMSearchParamsReceived(Guid WSId)
        {
            BottomFlyoutTitle = "Cm Search Parameters";
            BottomFlyoutViewModel = new CMSearchViewModel(WSId);
            BottomFlyoutOpen = true;
            RaisePropertyChanged("BottomFlyoutOpen");
        }

        //Receive a message requesting to hide Search Parameters Flyout
        private void OnRequestHideSearchParamsReceived()
        {
            BottomFlyoutOpen = false;
            RaisePropertyChanged("BottomFlyoutOpen");
        }

        //Receive a message requesting to hide all Flyouts
        private void OnRequestHideFlyoutsReceived()
        {
            LeftFlyoutOpen = false;
            RaisePropertyChanged("LeftFlyoutOpen");
            TopFlyoutOpen = false;
            RaisePropertyChanged("TopFlyoutOpen");
            RightFlyoutOpen = false;
            RaisePropertyChanged("RightFlyoutOpen");
            BottomFlyoutOpen = false;
            RaisePropertyChanged("BottomFlyoutOpen");
        }

        private void OnRequestShowStationCertificateListReceived(String id)
        {
            ShowOverlayContent = true;
            OverlayContentViewModel = new StationCertificateListViewModel(id);
        }

        #endregion

        #region  Workspaces

        //Workspaces
        private ObservableCollection<WorkspaceViewModelBase> _Workspaces;
        public ObservableCollection<WorkspaceViewModelBase> Workspaces
        {
            get
            {
                if (_Workspaces == null)
                {
                    _Workspaces = new ObservableCollection<WorkspaceViewModelBase>();
                    _Workspaces.CollectionChanged += OnWorkspacesChanged;
                }
                return _Workspaces;
            }
        }

        private int _CurrentWorkspace = 0;
        public int CurrentWorkspace
        {
            get
            {
                return _CurrentWorkspace;
            }
            set
            {
                //Switch to a new Workspace
                _CurrentWorkspace = value;
                RaisePropertyChanged("CurrentWorkspace");
                MessageMediator.NotifyColleagues("RefreshWorkspacesList", Workspaces); //Will be returned to the WorkspacesWindow signed for this message
                //Reset StatusBar colors
                StatusMessage = string.Empty;
                StatusForegroundBrush = RibbonAndStatusForegroundBrush;
                StatusBackgroundBrush = RibbonAndStatusBackgroundBrush;
                //Close all Flyouts
                CloseAllFlyouts();
            }
        }

        private void AddWorkspace(WorkspaceViewModelBase WSVM)
        {
            Workspaces.Add(WSVM);
            CurrentWorkspace = Workspaces.Count - 1;
            RaisePropertyChanged("IsWorkspacesTabVisible");
        }

        private void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
            {
                foreach (WorkspaceViewModelBase workspace in e.NewItems)
                {
                    workspace.CloseWorkSpace += OnWorkspaceRequestClose;
                }
            }
            if (e.OldItems != null && e.OldItems.Count != 0)
            {
                foreach (WorkspaceViewModelBase workspace in e.OldItems)
                {
                    workspace.CloseWorkSpace -= OnWorkspaceRequestClose;
                }
            }
        }

        private void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            WorkspaceViewModelBase Workspace = sender as WorkspaceViewModelBase;
            CloseWorkspace(Workspace);
        }

        private void CloseWorkspace(WorkspaceViewModelBase Workspace)
        {
            //Determine if workspace being closed is the last one in the collection; we will need this to set the CurrentWorkspace in case we are closing the last one in the collection
            bool ClosingLastWorkspace = false;
            if (Workspace.WSId == Workspaces[Workspaces.Count - 1].WSId)
            {
                ClosingLastWorkspace = true;
            }
            if (Workspace.IsCloseable)
            {
                if (Workspace.IsDirty)
                {
                    if (MsgBoxService.ShowOkCancel("Un-saved data will be lost!" + Environment.NewLine + "Press OK to close, Cancel to resume editing.", DialogIcons.Warning) == DialogResults.Cancel)
                    {
                        return;
                    }
                }
                Workspace.Dispose();
                this.Workspaces.Remove(Workspace);
                try //Delete the Thumb file
                {
                    string FileName = Workspace.WSId.ToString() + ".WSID.png";
                    if (File.Exists(FileName))
                    {
                        File.Delete(FileName);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
                //was the workspace the last one in the collection?
                if (ClosingLastWorkspace && Workspaces.Count > 0)
                {
                    CurrentWorkspace = Workspaces.Count - 1;
                }
            }
            RaisePropertyChanged("IsWorkspacesTabVisible");
        }

        private RelayCommand _CloseAllWorkspacesCommand;
        public RelayCommand CloseAllWorkspacesCommand
        {
            get
            {
                if (_CloseAllWorkspacesCommand == null)
                {
                    _CloseAllWorkspacesCommand = new RelayCommand(ExecuteCloseAllWorkspacesCommand, CanExecuteCloseAllWorkspacesCommand);
                }
                return _CloseAllWorkspacesCommand;
            }
        }

        private bool CanExecuteCloseAllWorkspacesCommand()
        {
            return true;
        }

        public void ExecuteCloseAllWorkspacesCommand()
        {
            for (var Index = this.Workspaces.Count - 1; Index >= 0; Index--)
            {
                WorkspaceViewModelBase WS = this.Workspaces[Index];
                CloseWorkspace(WS);
            }
        }

        private RelayCommand _CloseOtherWorkspacesCommand;
        public RelayCommand CloseOtherWorkspacesCommand
        {
            get
            {
                if (_CloseOtherWorkspacesCommand == null)
                {
                    _CloseOtherWorkspacesCommand = new RelayCommand(ExecuteCloseOtherWorkspacesCommand, CanExecuteCloseOtherWorkspacesCommand);
                }
                return _CloseOtherWorkspacesCommand;
            }
        }

        private bool CanExecuteCloseOtherWorkspacesCommand()
        {
            return true;
        }

        public void ExecuteCloseOtherWorkspacesCommand()
        {
            string CurrWSID = Workspaces[CurrentWorkspace].WSId.ToString();
            for (var Index = this.Workspaces.Count - 1; Index >= 0; Index--)
            {
                if (Workspaces[Index].WSId.ToString() != CurrWSID)
                {
                    CloseWorkspace(Workspaces[Index]);
                }
            }
        }

        #region  Decrease Workspaces Tab Item Width Command

        private RelayCommand _DecreaseWorkspacesTabItemWidthCommand;
        public RelayCommand DecreaseWorkspacesTabItemWidthCommand
        {
            get
            {
                if (_DecreaseWorkspacesTabItemWidthCommand == null)
                {
                    _DecreaseWorkspacesTabItemWidthCommand = new RelayCommand(ExecuteDecreaseWorkspacesTabItemWidthCommand, CanExecuteDecreaseWorkspacesTabItemWidthCommand);
                }
                return _DecreaseWorkspacesTabItemWidthCommand;
            }
        }

        private bool CanExecuteDecreaseWorkspacesTabItemWidthCommand()
        {
            return true;
        }

        private void ExecuteDecreaseWorkspacesTabItemWidthCommand()
        {
            if (WorkspacesTabItemWidth > 100)
            {
                WorkspacesTabItemWidth -= 10;
            }
        }

        #endregion

        #region  Increase Workspaces Tab Item Width Command

        private RelayCommand _IncreaseWorkspacesTabItemWidthCommand;
        public RelayCommand IncreaseWorkspacesTabItemWidthCommand
        {
            get
            {
                if (_IncreaseWorkspacesTabItemWidthCommand == null)
                {
                    _IncreaseWorkspacesTabItemWidthCommand = new RelayCommand(ExecuteIncreaseWorkspacesTabItemWidthCommand, CanExecuteIncreaseWorkspacesTabItemWidthCommand);
                }
                return _IncreaseWorkspacesTabItemWidthCommand;
            }
        }

        private bool CanExecuteIncreaseWorkspacesTabItemWidthCommand()
        {
            return true;
        }

        private void ExecuteIncreaseWorkspacesTabItemWidthCommand()
        {
            if (WorkspacesTabItemWidth < 360)
            {
                WorkspacesTabItemWidth += 10;
            }
        }

        #endregion

        #endregion

        #region  Login

        #region  ShowLogin Command

        private RelayCommand _ShowLoginCommand;
        public RelayCommand ShowLoginCommand
        {
            get
            {
                if (_ShowLoginCommand == null)
                {
                    _ShowLoginCommand = new RelayCommand(ExecuteShowLoginCommand, CanExecuteShowLoginCommand);
                }
                return _ShowLoginCommand;
            }
        }

        private bool CanExecuteShowLoginCommand()
        {
            return true;
        }

        private void ExecuteShowLoginCommand()
        {
            ShowOverlayContent = true;
            OverlayContentViewModel = new LoginViewModel();
        }

        #endregion

        //Receive a message from Login Overlay
        private void OnRequestLoginReceived(string UserName)
        {
            if (UserName.Equals("ExitApplication"))
            {
                ExecuteExitApplicationCommand();
            }
            else
            {
                OverlayContentViewModel = null;
                ShowOverlayContent = false;
            }
        }

        #endregion

    }
}
