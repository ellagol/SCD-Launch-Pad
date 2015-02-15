using System;
using Infra.Configuration;
using Infra.MVVM;

namespace ATSVM
{
    public class AboutViewModel : ViewModelBase
    {

        #region  Data

        private IMessageBoxService MsgBoxService = null;
        private ITaskDialogService TaskDialogService = null;
        private MessengerService MessageMediator = null;
        public static XmlConfigSource UserConfigParams; //Reference to the User level Configuration Parameters
        #endregion

        #region  Properties

        private string pe_Version;
        public string PE_Version
        {
            get { return pe_Version; }
            set { pe_Version = value; }
        }

        #endregion

        #region  Constructor

        public AboutViewModel()
        {
            //Get Version Details
            PE_Version = "Version: "+ATSDomain.Domain.PE_Version;

            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //Task Dialog Service
            TaskDialogService = GetService<ITaskDialogService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();
            //Initialize Application Configuration Parameters Reader/Writer
            UserConfigParams = new XmlConfigSource(AppDomain.CurrentDomain.BaseDirectory + "AppSettings\\UserSettings.xml");

        }

        #endregion

        #region  OK Command

        private RelayCommand _OkCommand;
        public RelayCommand OkCommand
        {
            get
            {
                if (_OkCommand == null)
                {
                    _OkCommand = new RelayCommand(ExecuteOkCommand, CanExecuteOkCommand);
                }
                return _OkCommand;
            }
        }

        private bool CanExecuteOkCommand()
        {
            // TODO: validate if Login fields are filled properly (NOT REQUIRED for SCD)
            return true;
        }

        private void ExecuteOkCommand()
        {
            MessageMediator.NotifyColleagues("RequestLogin", ATSDomain.Domain.User); //Will be returned to the MainWindow signed for this message
        }

        #endregion

    }
}
