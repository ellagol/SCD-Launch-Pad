using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Windows.Input;
using ATSBusinessLogic;
using ATSBusinessObjects;
using ATSDomain;
using Infra.MVVM;


namespace UserCertModule
{
    public class UsersCertificatesViewModel : WorkspaceViewModelBase
    {
        #region  Data

        
        private StringCollection Permissions; //Collection of permissions this user is allowed to perform

        protected MessengerService MessageMediator = new MessengerService();
        ObservableCollection<string> _Status = new ObservableCollection<string>();
        private IMessageBoxService MsgBoxService = null;
        UserCertificateModel CM = new UserCertificateModel();
        UserModel UM = new UserModel();

        #endregion

        #region  Ctor
        public UsersCertificatesViewModel()
            : base("CertView", "")
        {
            //Initialize this VM
            DisplayName = "Certificates";
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();
            //Register as Subscriber to messages from other VMs

            MessageMediator.Register(this.WSId + "OnUpdateCertificateReceived", new Action<UserCertificateModel>(OnUpdateCertificateReceived)); //Register to recieve a message from certificatesDetailsViewModel
            MessageMediator.Register(this.WSId + "OnUpdateUserReceived", new Action<UserModel>(OnUpdateUserReceived)); //Register to recieve a message from usersDetailsViewModel
            MessageMediator.Register(this.WSId + "OnSaveCertificateReceived", new Action<UserCertificateModel>(OnSaveCertificateReceived)); //Register to recieve a message from usersDetailsViewModel
            MessageMediator.Register(this.WSId + "OnRemoveUserReceived", new Action<UserCertificateModel>(OnRemoveUserReceived)); //Register to recieve a message from usersDetailsViewModel
            MessageMediator.Register(this.WSId + "OnRemoveCertificateReceived", new Action<UserModel>(OnRemoveCertificateReceived)); //Register to recieve a message from usersDetailsViewModel
            MessageMediator.Register(this.WSId + "OnIsDirtyCertificateReceived", new Action<UserCertificateModel>(OnIsDirtyCertificateReceived)); //Register to recieve a message from certificatesDetailsViewModel
            MessageMediator.Register(this.WSId + "OnIsDirtyUserReceived", new Action<UserModel>(OnIsDirtyUserReceived)); //Register to recieve a message from certificatesDetailsViewModel
            //Dummy users - will be replaced with real users from the DB.

            _Users = UserBLL.GetUser();
            RaisePropertyChanged("Users");
            _Certificates = CertificateUserBLL.GetUserCertificate();

            RaisePropertyChanged("Certificates");

            // _Status = CertificateUserBLL.GetAllStatus();


        }
    
        #endregion

        #region Presentation Data

        private ObservableCollection<UserModel> _Users = new ObservableCollection<UserModel>();
        public ObservableCollection<UserModel> Users
        {
            get
            {
                return _Users;
            }
            set
            {
                _Users = value;
                RaisePropertyChanged("Users");

            }
           
        }


        private ObservableCollection<UserCertificateModel> _Certificates = new ObservableCollection<UserCertificateModel>();
        public ObservableCollection<UserCertificateModel> Certificates
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


        private UserModel _SelectedUser;
        public UserModel SelectedUser
        {
            get
            {
                return _SelectedUser;
            }
            set
            {
                // Because we added MouseUp handling event, the code must move; only identifying the value of the selected ListView item should remain here
                _SelectedUser = value;
                if (_SelectedUser != null)
                {
                    OnUpdateUserReceived(_SelectedUser);
                }
                RaisePropertyChanged("SelectedUser");

                //if (_SelectedUser != value)
                //{
                //    _SelectedUser = value;
                //    OnUpdateUserReceived(_SelectedUser);
                //    RaisePropertyChanged("SelectedUser");
                //    UM = _SelectedUser;
                //    _SelectedUser = null;// In order to 'set' the selected for the next time.
                //}
            }
        }



        private UserCertificateModel _SelectedCertificate;
        public UserCertificateModel SelectedCertificate
        {
            get
            {
                return _SelectedCertificate;
            }
            set
            {
                // Because we added MouseUp handling event, the code must move; only identifying the value of the selected ListView item should remain here
                _SelectedCertificate = value;
                if (_SelectedCertificate != null)
                {
                    OnUpdateCertificateReceived(_SelectedCertificate);
                }
                RaisePropertyChanged("SelectedCertificate");
            }
        }



        private ViewModelBase _DetailsViewModel = null;
        public ViewModelBase DetailsViewModel
        {
            get
            {
                return _DetailsViewModel;
            }
            set
            {
                _DetailsViewModel = value;
                RaisePropertyChanged("DetailsViewModel");
            }
        }


        private UserCertificateModel _IsSelectedDirtyCertificate = null;
        public UserCertificateModel IsSelectedDirtyCertificate
        {
            get
            {
                return _IsSelectedDirtyCertificate;
            }
            set
            {
                // Because we added MouseUp handling event, the code must move; only identifying the value of the selected ListView item should remain here
                _IsSelectedDirtyCertificate = value;
            }
           
        }

        private UserModel _SelectedUserDirtyUser;
        public UserModel SelectedUserDirtyUser
        {
            get
            {
                return _SelectedUserDirtyUser;
            }
            set
            {
                // Because we added MouseUp handling event, the code must move; only identifying the value of the selected ListView item should remain here
                _SelectedUserDirtyUser = value;
                //if (_SelectedUser != value)
                //{
                //    _SelectedUser = value;
                //    OnUpdateUserReceived(_SelectedUser);
                //    RaisePropertyChanged("SelectedUser");
                //    UM = _SelectedUser;
                //    _SelectedUser = null;// In order to 'set' the selected for the next time.
                //}
            }
        }


        /**private string _Name;
        public string Name
        {
            get
            {
                if (_SelectedCertificate != null)
                {
                    return _SelectedCertificate.Name;
                }
                else
                    return _Name;
            }
            set
            {
                _Name = value;
                RaisePropertyChanged("Name");


            }

        }**/
        #endregion Presentation Data

        #region Show / Hide Certificates and Users.

        private void OnUpdateCertificateReceived(UserCertificateModel UC)
        {
            DetailsViewModel = new CertificatesDetailsViewModel(UC, this.WSId);

        }

        private void OnUpdateUserReceived(UserModel Um)
        {
            DetailsViewModel = new UsersDetailsViewModel(Um, this.WSId);
        }

        private void OnSaveCertificateReceived(UserCertificateModel UC)
        {
           Certificates.Add(UC);
           RaisePropertyChanged("Certificates");

        }


        private void OnRemoveUserReceived(UserCertificateModel UCM)
        {
            string UserName = "";
            foreach (var i in UCM.Users)
            {
                if (i.IsDirty)
                {
                    UserName = i.UserName;
                    break;
                }
            }
            foreach (var j in Users)
            {
                if (j.UserName == UserName)
                {
                    foreach (var certificate in j.UserCertificates)
                    {
                        if (certificate.Id == UCM.Id)
                        {
                            j.UserCertificates.Remove(certificate);
                            break;
                        }
                        
                    }
                }
            }
        }

        private void OnRemoveCertificateReceived(UserModel UM)
        {
            string CertificateId = "";
            foreach (var i in UM.UserCertificates)
            {
                if (i.IsDirty)
                {
                    CertificateId = i.Id;
                    break;
                }
            }
            foreach (var j in Certificates)
            {
                if (j.Id == CertificateId)
                {
                    foreach (var user in j.Users)
                    {
                        if (user.UserName == UM.UserName)
                        {
                            j.Users.Remove(user);
                            break;
                        }

                    }
                }
            }
        }

        private void OnIsDirtyCertificateReceived(UserCertificateModel UC)
        {
            this._IsSelectedDirtyCertificate = UC;

        }


        private void OnIsDirtyUserReceived(UserModel UM)
        {
            this._SelectedUserDirtyUser = UM;

        }


        #endregion Show / Hide Certificates and Users.

        #region Commands

        #region Delete Certificate Command

        public string CheckCertificateStatus
        {

            get
            {

                if (CM.Status == UserCertificateStatusEnum.D)
                {
                    return "Visible";
                }
                else
                {
                    return "Collapsed";
                }

            }

        }

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
            return Domain.IsPermitted("202", "UCM");
        }

        private void ExecuteDeleteCertificateCommand()
        {
            try
            {
                if (CertificateUserBLL.DeleteCertificate(CM.Id)== String.Empty)
                {
                    Certificates.Remove(CM);

                    RaisePropertyChanged("Certificates");

                    RaisePropertyChanged("CheckCertificateStatus");
                    SelectedCertificate = Certificates[0];
                    MessageMediator.NotifyColleagues(this.WSId + "OnUpdateCertificateReceived", _SelectedCertificate);
                    RaisePropertyChanged("SelectedCertificate");
             

                }
            }
            catch (Exception e)
            {
                Object[] ArgsList = new Object[] { 0 };
                UsersCertificatesViewModel.ShowErrorAndInfoMessage(105, ArgsList);
            }
        }
        #endregion Delete Certificate Command

        #region Certificates MouseUp Command

        private RelayCommand _CertsMouseUpCommand;
        public ICommand CertsMouseUpCommand
        {
            get
            {
                if (_CertsMouseUpCommand == null)
                {
                    _CertsMouseUpCommand = new RelayCommand(ExecuteCertsMouseUpCommand, CanExecuteCertsMouseUpCommand);
                }
                return _CertsMouseUpCommand;
            }
        }

        private bool CanExecuteCertsMouseUpCommand()
        {
            return true;
        }

        private void ExecuteCertsMouseUpCommand()
        {
            try
            {
                if (_SelectedCertificate != null)
                {
                    if (_IsSelectedDirtyCertificate == null && _SelectedUserDirtyUser == null)
                    {
                        MessageMediator.NotifyColleagues("StatusBarParameters", null);
                        MessageMediator.NotifyColleagues(this.WSId + "OnUpdateCertificateReceived", _SelectedCertificate);
                        RaisePropertyChanged("SelectedCertificate");
                        //Intialize status drop down list in loading . One time for the entire session.
                        //MessageMediator.NotifyColleagues(this.WSId + "OnGetStatusReceived", _Status);// get the status drop down list.
                        CM = _SelectedCertificate;
                        RaisePropertyChanged("CheckCertificateStatus");
                    }
                    else
                    {
                        if (_SelectedUserDirtyUser != null)
                        {
                            _SelectedUser = _SelectedUserDirtyUser;
                            RaisePropertyChanged("SelectedUser");
                        }
                        if (_IsSelectedDirtyCertificate != null && _IsSelectedDirtyCertificate.IsNew == false)
                        {
                            _SelectedCertificate = _IsSelectedDirtyCertificate;
                            RaisePropertyChanged("SelectedCertificate");
                        }

                        DialogResults Dialog = MsgBoxService.ShowYesNoCancel(CertificateUserBLL.GetMessage(), DialogIcons.Question);
                        switch (Dialog)
                        {
                            case DialogResults.Yes:
                                {
                                    if (_IsSelectedDirtyCertificate != null)
                                    {
                                        MessageMediator.NotifyColleagues(this.WSId + "OnApplySaveCertificateCommand", _SelectedCertificate);

                                        //Add new certificate after changes to certificate list.
                                        RaisePropertyChanged("Certificates");
                                        UserCertificateModel TempUCM = _SelectedCertificate;
                                        _Certificates.Remove(_SelectedCertificate);
                                        _Certificates.Add(TempUCM);
                                        _SelectedCertificate = TempUCM;

                                        RaisePropertyChanged("SelectedCertificate");

                                        RaisePropertyChanged("Certificates");


                                    }
                                    if (_SelectedUserDirtyUser != null)
                                    {
                                        MessageMediator.NotifyColleagues(this.WSId + "OnApplySaveUserCommand", _SelectedUser);
                                    }

                                    break;
                                }
                            case DialogResults.No:
                                {
                                    if (_IsSelectedDirtyCertificate != null)
                                    {
                                        if (_IsSelectedDirtyCertificate.IsNew == false)
                                        {
                                            for (int i = 0; i < Certificates.Count; i++)
                                            {
                                                if (Certificates[i].Id == _IsSelectedDirtyCertificate.Id)
                                                {
                                                    Certificates[i] = CertificateUserBLL.GetUserCertificateRow(_IsSelectedDirtyCertificate.Id);
                                                    _IsSelectedDirtyCertificate = null;
                                                    _SelectedCertificate = Certificates[i];
                                                    RaisePropertyChanged("SelectedCertificate");
                                                    MessageMediator.NotifyColleagues(this.WSId + "OnUpdateCertificateReceived", _SelectedCertificate);
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {

                                            MessageMediator.NotifyColleagues("StatusBarParameters", null);
                                            MessageMediator.NotifyColleagues(this.WSId + "OnIsDirtyCertificateReceived", null);
                                            MessageMediator.NotifyColleagues(this.WSId + "OnUpdateCertificateReceived", _SelectedCertificate);
                                            RaisePropertyChanged("SelectedCertificate");
                                        }
                                    }
                                    if (_SelectedUserDirtyUser != null)
                                    {
                                        for (int i = 0; i < Users.Count; i++)
                                        {
                                            if (Users[i].UserId == _SelectedUserDirtyUser.UserId)
                                            {
                                                Users[i] = UserBLL.GetUserRow(_SelectedUserDirtyUser.UserName);

                                                _SelectedUserDirtyUser = null;
                                                _SelectedUser = Users[i];
                                                RaisePropertyChanged("SelectedUser");
                                                //Refresh user details viwe model
                                                MessageMediator.NotifyColleagues(this.WSId + "OnUpdateUserReceived", _SelectedUser);
                                                break;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case DialogResults.Cancel:
                                {
                                    break;
                                }

                        }
                        //if (MsgBoxService.ShowOkCancel((Domain.PersistenceLayer.FetchDataValue(TB.ToString(), CommandType.Text, null)).ToString(), DialogIcons.Question) == DialogResults.Cancel)
                    }

                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                Object[] ArgsList = new Object[] { 0 };
                UsersCertificatesViewModel.ShowErrorAndInfoMessage(105, ArgsList);
            }
        }

        #endregion Certificates MouseUp Command

        #region Users MouseUp Command

        private RelayCommand _UsersMouseUpCommand;
        public ICommand UsersMouseUpCommand
        {
            get
            {
                if (_UsersMouseUpCommand == null)
                {
                    _UsersMouseUpCommand = new RelayCommand(ExecuteUsersMouseUpCommand, CanExecuteUsersMouseUpCommand);
                }
                return _UsersMouseUpCommand;
            }
        }

        private bool CanExecuteUsersMouseUpCommand()
        {
            return true;
        }

        private void ExecuteUsersMouseUpCommand()
        {
            try
            {
                if (_SelectedUser != null)
                {
                    if (_SelectedUserDirtyUser == null && _IsSelectedDirtyCertificate == null)
                    {
                        MessageMediator.NotifyColleagues("StatusBarParameters", null);
                        MessageMediator.NotifyColleagues(this.WSId + "OnUpdateUserReceived", _SelectedUser);
                        //OnUpdateUserReceived(_SelectedUser);
                        RaisePropertyChanged("SelectedUser");
                        UM = _SelectedUser;
                    }
                    else
                    {
                        if (_SelectedUserDirtyUser != null)
                        {
                            _SelectedUser = _SelectedUserDirtyUser;
                            RaisePropertyChanged("SelectedUser");
                        }
                        if (_IsSelectedDirtyCertificate != null)
                        {
                            _SelectedCertificate = _IsSelectedDirtyCertificate;
                            RaisePropertyChanged("SelectedCertificate");
                        }
                        DialogResults Dialog = MsgBoxService.ShowYesNoCancel(CertificateUserBLL.GetMessage(), DialogIcons.Question);
                        switch (Dialog)
                        {
                            case DialogResults.Yes:
                                {
                                    if (_IsSelectedDirtyCertificate != null)
                                    {
                                        MessageMediator.NotifyColleagues(this.WSId + "OnApplySaveCertificateCommand", _SelectedCertificate);

                                        //Add new certificate after changes to certificate list.
                                        RaisePropertyChanged("Certificates");
                                        UserCertificateModel TempUCM = _SelectedCertificate;
                                        _Certificates.Remove(_SelectedCertificate);
                                        _Certificates.Add(TempUCM);
                                        _SelectedCertificate = TempUCM;

                                        RaisePropertyChanged("SelectedCertificate");

                                        RaisePropertyChanged("Certificates");
                                    }
                                    if (_SelectedUserDirtyUser != null)
                                    {
                                        MessageMediator.NotifyColleagues(this.WSId + "OnApplySaveUserCommand", _SelectedUser);
                                    }
                                    break;
                                }
                            case DialogResults.No:
                                {

                                    if (_IsSelectedDirtyCertificate != null)
                                    {

                                        for (int i = 0; i < Certificates.Count; i++)
                                        {
                                            if (Certificates[i].Id == _IsSelectedDirtyCertificate.Id)
                                            {
                                                Certificates[i] = CertificateUserBLL.GetUserCertificateRow(_IsSelectedDirtyCertificate.Id);
                                                _IsSelectedDirtyCertificate = null;
                                                _SelectedCertificate = Certificates[i];
                                                RaisePropertyChanged("SelectedCertificate");
                                                MessageMediator.NotifyColleagues(this.WSId + "OnUpdateCertificateReceived", _SelectedCertificate);
                                                break;
                                            }
                                        }
                                    }
                                    if (_SelectedUserDirtyUser != null)
                                    {
                                        for (int i = 0; i < Users.Count; i++)
                                        {
                                            if (Users[i].UserId == _SelectedUserDirtyUser.UserId)
                                            {
                                                Users[i] = UserBLL.GetUserRow(_SelectedUserDirtyUser.UserName);

                                                _SelectedUserDirtyUser = null;
                                                _SelectedUser = Users[i];
                                                RaisePropertyChanged("SelectedUser");
                                                //Refresh user details viwe model
                                                MessageMediator.NotifyColleagues(this.WSId + "OnUpdateUserReceived", _SelectedUser);
                                                break;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case DialogResults.Cancel:
                                {
                                    break;
                                }

                        }
                    }


                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                Object[] ArgsList = new Object[] { 0 };
                UsersCertificatesViewModel.ShowErrorAndInfoMessage(105, ArgsList);
            }
        }

        #endregion Users MouseUp Command

        #region Refresh Certificate Command


        private RelayCommand _RefreshCertificateCommand;
        public ICommand RefreshCertificateCommand
        {
            get
            {
                if (_RefreshCertificateCommand == null)
                {
                    _RefreshCertificateCommand = new RelayCommand(ExecuteRefreshCertificateCommand, CanExecuteRefreshCertificateCommand);
                }
                return _RefreshCertificateCommand;
            }
        }

        private bool CanExecuteRefreshCertificateCommand()
        {
            return Domain.IsPermitted("113");
        }

        private void ExecuteRefreshCertificateCommand()
        {
            try
            {

               UserCertificateModel RefCM = CertificateUserBLL.GetUserCertificateRow(CM.Id);
               if (Certificates.Contains(CM))
               {
                   Certificates.Remove(CM);
                   if (RefCM != null && !string.IsNullOrEmpty(RefCM.Id) && !string.IsNullOrWhiteSpace(RefCM.Id))
                   {
                       Certificates.Add(RefCM);
                       ObservableCollection<UserCertificateModel> sortedCertificates = new ObservableCollection<UserCertificateModel>(Certificates.OrderBy(x => x.Name));
                       CM = RefCM;
                       Certificates = sortedCertificates;
                       SelectedCertificate = CM;
                       RaisePropertyChanged("SelectedCertificate");
                   }
                   RaisePropertyChanged("Certificates");
               }
               MessageMediator.NotifyColleagues(this.WSId + "OnUpdateCertificateReceived", CM);
               // MessageMediator.NotifyColleagues(this.WSId + "OnGetStatusReceived", _Status);// get the status drop down list.
     
                RaisePropertyChanged("CheckCertificateStatus");

            }
            catch (Exception e)
            {
                Object[] ArgsList = new Object[] { 0 };
                UsersCertificatesViewModel.ShowErrorAndInfoMessage(105, ArgsList);
            }
        }
        #endregion Refresh Certificate Command

        #region Refresh All Certificate Command


        private RelayCommand _RefreshAllCertificateCommand;
        public ICommand RefreshAllCertificateCommand
        {
            get
            {
                if (_RefreshAllCertificateCommand == null)
                {
                    _RefreshAllCertificateCommand = new RelayCommand(ExecuteRefreshAllCertificateCommand, CanExecuteRefreshAllCertificateCommand);
                }
                return _RefreshAllCertificateCommand;
            }
        }

        private bool CanExecuteRefreshAllCertificateCommand()
        {
            return Domain.IsPermitted("113");
        }

        private void ExecuteRefreshAllCertificateCommand()
        {
            try
            {
                Certificates.Clear();
                Certificates = CertificateUserBLL.GetUserCertificate();

                RaisePropertyChanged("Certificates");
                RaisePropertyChanged("CheckCertificateStatus");
                //MessageMediator.NotifyColleagues(this.WSId + "OnGetStatusReceived", _Status);
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
            }
        }
        #endregion Refresh All Certificate Command

        #region Add New Certificate Command

        private RelayCommand _AddNewCertificateCommand;
        public ICommand AddNewCertificateCommand
        {
            get
            {
                if (_AddNewCertificateCommand == null)
                {
                    _AddNewCertificateCommand = new RelayCommand(ExecuteAddNewCertificateCommand, CanExecuteAddNewCertificateCommand);
                }
                return _AddNewCertificateCommand;
            }
        }

        private bool CanExecuteAddNewCertificateCommand()
        {
            return Domain.IsPermitted("201", "UCM");
        }

        private void ExecuteAddNewCertificateCommand()
        {
            try
            {
                //Intialize new UserCertificateModel
                UserCertificateModel UCM = new UserCertificateModel();
                UCM.IsNew = true;
                MessageMediator.NotifyColleagues(this.WSId + "OnUpdateCertificateReceived", UCM);

                //MessageMediator.NotifyColleagues(this.WSId + "OnAddNewCertificateReceived", UCM);
                //MessageMediator.NotifyColleagues(this.WSId + "OnGetStatusReceived", _Status);// get the status drop down list.
            }
            catch (Exception e)
            {
                Object[] ArgsList = new Object[] { 0 };
                UsersCertificatesViewModel.ShowErrorAndInfoMessage(105, ArgsList);
            }
        }

        #endregion Add New Command

        #endregion Commands

        #region Messages

        //Status bar Error and Info messages
        public static MessengerService MessageMediatorErrorAndInfo = ServiceProvider.GetService<MessengerService>();
        public static void ShowErrorAndInfoMessage(int error, object[] Args)
        {
            try
            {
                Collection<string> StatusBarParameters = new Collection<string>();
                string Query = "SELECT Description, Type FROM PE_Messages where id=" + error + ";";

                // Fetch the row from the database (retrieving by PK --> 0 or 1 row)
                DataRow MsgRow = (DataRow)Domain.PersistenceLayer.FetchDataTable(Query, CommandType.Text, null).Rows[0];

                // Verify the message is found
                if (!string.IsNullOrEmpty((string)MsgRow["Description"]))
                {
                    //Message verbiage with parameters, as retrieved from the table
                    string MsgDescription = (string)MsgRow["Description"];

                    // Arguments substitution, if any
                    string MsgDescriptionWithParam = SetDescriptionParameters(MsgDescription, Args, error);

                    // Verify message description was successfully formatted 

                    // Message Type - consider defining enum for valid values
                    string MsgType = (string)MsgRow["Type"];

                    //Set background color based on message type
                    switch (MsgType.Trim())
                    {
                        case "I":
                            if (!MsgDescriptionWithParam.Equals(ErrorString))
                            {
                                StatusBarParameters = SetMessageDescriptionParam(MsgDescriptionWithParam, "White", "Green"); //Background for info messages
                            }
                            else
                            {
                                MsgDescription = MsgDescription + "(PE_Messages: Invalid number of parameters for Message Id " + error + ")";
                                StatusBarParameters = SetMessageDescriptionParam(MsgDescription, "White", "Green"); //When PE_Messages record contains invalid number of parameters
                            }
                            break;
                        case "E":
                            if (!MsgDescriptionWithParam.Equals(ErrorString))
                            {
                                StatusBarParameters = SetMessageDescriptionParam(MsgDescriptionWithParam, "White", "Red"); //Background for error messages
                            }
                            else
                            {
                                MsgDescription = MsgDescription + "(PE_Messages: Invalid number of parameters for Message Id " + error + ")";
                                StatusBarParameters = SetMessageDescriptionParam(MsgDescription, "White", "Red"); //When PE_Messages record contains invalid number of parameters
                            }
                            break;
                        default:
                            StatusBarParameters = SetMessageDescriptionParam(MsgDescription, "White", "Red"); //invalid type of message
                            break;
                    }
                    MessageMediatorErrorAndInfo.NotifyColleagues("StatusBarParameters", StatusBarParameters);
                }
            }
            catch (Exception)
            {
                ShowGenericErrorMessage(error); //If DB connection failed or no rows selected
            }
        }

        //If DB connection failed or no rows selected
        private static void ShowGenericErrorMessage(int MsgCode)
        {
            Collection<string> StatusBarParametersGenericError = new Collection<string>();

            String ErrorMessageText = "Unable to retrieve error message " + MsgCode + ". Please see Data Access log file for more details: Shell->View Log."; //Message
            StatusBarParametersGenericError = SetMessageDescriptionParam(ErrorMessageText, "White", "Red");
            MessageMediatorErrorAndInfo.NotifyColleagues("StatusBarParameters", StatusBarParametersGenericError);
        }


        //To catch exception from String.Format function if occurs
        private static string ErrorString = "ParamError";
        private static string SetDescriptionParameters(string MessageDescription, object[] ArgList, int MsgCode)
        {
            try
            {
                string MsgDescriptionWithParam = String.Format(MessageDescription, ArgList);

                return MsgDescriptionWithParam;
            }
            catch (Exception)
            {
                return ErrorString;
            }
        }

        // Set status bar parameters
        private static Collection<String> SetMessageDescriptionParam(String MessageText, String FgColor, String BgColor)
        {
            Collection<string> StatusBarParametersAdd = new Collection<string>();

            StatusBarParametersAdd.Add(MessageText); //Message
            StatusBarParametersAdd.Add(FgColor); //Foreground
            StatusBarParametersAdd.Add(BgColor); //Background 

            return StatusBarParametersAdd;
        }

        #endregion

        #region  Find


        private string _SearchText = string.Empty;
        private UserModel PreviousUser;
        private int count = 0;
        
        public string SearchText
        {
            get
            {
                return _SearchText;
            }
            set
            {
                _SearchText = value;
                RaisePropertyChanged("SearchText");
            }
        }
        private string PrevSearchString = string.Empty;

        private RelayCommand _FindCommand;
        public ICommand FindCommand
        {
            get
            {
                if (_FindCommand == null)
                {
                    _FindCommand = new RelayCommand(ExecuteFindCommand, CanExecuteFindCommand);
                }
                return _FindCommand;
            }
        }

        private bool CanExecuteFindCommand()
        {
            return SearchText.Length > 0;
        }

        private void ExecuteFindCommand()
        {
            try
            {
                bool foundRecord = false;
                if (!String.IsNullOrEmpty(SearchText))
                {
                    if (SearchText != PrevSearchString)
                    {
                        PreviousUser = null;
                        count = 0;
                        PrevSearchString = SearchText;
                    }
                    for (count = count; count < Users.Count; count++)
                    {
                        if (Users[count] != PreviousUser)
                        {
                            if (Users[count].UserName.Contains(SearchText))
                            {
                                SelectedUser = Users[count];
                                PreviousUser = SelectedUser = Users[count];

                                RaisePropertyChanged("SelectedUser");
                                ExecuteUsersMouseUpCommand();
                                count = count;
                                foundRecord = true;
                                break;
                            }
                        }
                    }
                    if (!foundRecord)
                        MsgBoxService.ShowInformation("No users found containing searched data. Please try again.");
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                Object[] ArgsList = new Object[] { 0 };
                UsersCertificatesViewModel.ShowErrorAndInfoMessage(105, ArgsList);

            }
        }

    

        #endregion

        
    }
}
