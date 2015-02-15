using System;
using Infra.Configuration;
using Infra.MVVM;

namespace ATSVM
{
    public class LoginViewModel : ViewModelBase
    {
        #region  Data

        private IMessageBoxService MsgBoxService = null;
        private ITaskDialogService TaskDialogService = null;
        private MessengerService MessageMediator = null;
        public static XmlConfigSource UserConfigParams; //Reference to the User level Configuration Parameters

        #endregion

        #region  Properties

        private string _LoginName = string.Empty;
        public string LoginName
        {
            get
            {
                return _LoginName;
            }
            set
            {
                _LoginName = value;
                RaisePropertyChanged("LoginName");
            }
        }

        private string _LoginPassword = string.Empty;
        public string LoginPassword
        {
            get
            {
                return _LoginPassword;
            }
            set
            {
                _LoginPassword = value;
                RaisePropertyChanged("LoginPassword");
            }
        }

        private bool _LoginRememberMe = false;
        public bool LoginRememberMe
        {
            get
            {
                return _LoginRememberMe;
            }
            set
            {
                _LoginRememberMe = value;
                RaisePropertyChanged("LoginRememberMe");
            }
        }

        private bool _LoginRememberMyPassword = false;
        public bool LoginRememberMyPassword
        {
            get
            {
                return _LoginRememberMyPassword;
            }
            set
            {
                _LoginRememberMyPassword = value;
                RaisePropertyChanged("LoginRememberMyPassword");
            }
        }

        #endregion

        #region  Constructor

        public LoginViewModel()
        {
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //Task Dialog Service
            TaskDialogService = GetService<ITaskDialogService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();
            //Initialize Application Configuration Parameters Reader/Writer
            UserConfigParams = new XmlConfigSource(AppDomain.CurrentDomain.BaseDirectory + "AppSettings\\UserSettings.xml");
            //Initialize VM properties; Retrieve last sessions' attributes (if saved)
            LoginName = (string)UserConfigParams.GetConfigParam("Login", "User", "String", "", true);
            LoginPassword = (string)UserConfigParams.GetConfigParam("Login", "Password", "String", "", true);
            LoginRememberMe = (LoginName != "") ? true : false;
            LoginRememberMyPassword = (LoginPassword != "") ? true : false;
        }

        #endregion

        #region  Exit Application Command

        private RelayCommand _LoginCommand;
        public RelayCommand LoginCommand
        {
            get
            {
                if (_LoginCommand == null)
                {
                    _LoginCommand = new RelayCommand(ExecuteLoginCommand, CanExecuteLoginCommand);
                }
                return _LoginCommand;
            }
        }

        private bool CanExecuteLoginCommand()
        {
            // TODO: validate if Login fields are filled properly (NOT REQUIRED for SCD)
            return true;
        }

        private void ExecuteLoginCommand()
        {
            //Remember Me \ Remember my Password
            if (LoginRememberMe)
            {
                UserConfigParams.SetConfigParam("Login", "User", LoginName, true);
            }
            else
            {
                UserConfigParams.SetConfigParam("Login", "User", "");
            }
            if (LoginRememberMyPassword)
            {
                UserConfigParams.SetConfigParam("Login", "Password", LoginPassword, true);
            }
            else
            {
                UserConfigParams.SetConfigParam("Login", "Password", "");
            }
            UserConfigParams.SaveDocument();
            // TODO: Update the global application (and maybe the Domain) with the logged user (LoginName); Might be done by MainWindow?; NOT REQUIRED for SCD
            // Save data to the ApplicationData xml (if so requested)
            MessageMediator.NotifyColleagues("RequestLogin", LoginName); //Will be returned to the MainWindow signed for this message
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
            MessageMediator.NotifyColleagues("RequestLogin", "ExitApplication"); //Will be returned to the MainWindow signed for this message
        }

        #endregion

    }
}
