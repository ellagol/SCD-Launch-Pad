using System.ComponentModel.DataAnnotations;
using Infra.MVVM;
using System;
using System.Collections.Generic;
using ATSBusinessObjects;
using System.Linq;
using System.Collections.ObjectModel;

namespace ExplorerModule
{
    public class SelectEnvironmentViewModel : ViewModelBase
    {
        #region  Data

        private IMessageBoxService MsgBoxService = null;
        private ITaskDialogService TaskDialogService = null;
        private MessengerService MessageMediator = null;

        private TreeViewProjectNodeViewModel projectNode = null;
        Guid WorkSpaceId;
        #endregion

        #region  Properties

        private ObservableCollection<string> _UserEnvironmentNames = new ObservableCollection<string>();
        public ObservableCollection<string> UserEnvironmentNames
        {
            get
            {
                if (_UserEnvironmentNames != null)
                    return _UserEnvironmentNames;
                else
                    return null;
            }
            set
            {
                _UserEnvironmentNames = value;
                RaisePropertyChanged("UserEnvironmentNames");
            }
        }

        private ObservableCollection<UserEnvironmentsModel> _UserEnvironments = new ObservableCollection<UserEnvironmentsModel>();
        public ObservableCollection<UserEnvironmentsModel> UserEnvironments
        {
            get
            {
                if (_UserEnvironments != null)
                    return _UserEnvironments;
                else
                    return null;
            }
            set
            {
                _UserEnvironments = value;
            }
        }

        [Required]
        private string _SelectedEnv = string.Empty;
        public string SelectedEnv
        {
            get
            {
                return _SelectedEnv;
            }
            set
            {
                _SelectedEnv = value;
                RaisePropertyChanged("SelectedEnv");
            }
        }

        #endregion

        #region  Constructor

        public SelectEnvironmentViewModel(ObservableCollection<UserEnvironmentsModel> userEnvs, TreeViewProjectNodeViewModel project, Guid WSId)
        {
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //Task Dialog Service
            TaskDialogService = GetService<ITaskDialogService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();

            foreach (UserEnvironmentsModel env in userEnvs)
            {
                UserEnvironmentNames.Add(env.environmentName);
            }
            UserEnvironments = userEnvs;
            projectNode = project;
            WorkSpaceId = WSId;
        }

        #endregion

        #region  CloseOverlay Command

        private RelayCommand _CloseSelectEnvironmentDialogCommand;
        public RelayCommand CloseSelectEnvironmentDialogCommand
        {
            get
            {
                if (_CloseSelectEnvironmentDialogCommand == null)
                {
                    _CloseSelectEnvironmentDialogCommand = new RelayCommand(ExecuteCloseSelectEnvironmentDialogCommand, CanExecuteCloseSelectEnvironmentDialogCommand);
                }
                return _CloseSelectEnvironmentDialogCommand;
            }
        }

        private bool CanExecuteCloseSelectEnvironmentDialogCommand()
        {
            return true;
        }

        private void ExecuteCloseSelectEnvironmentDialogCommand()
        {
            MessageMediator.NotifyColleagues(WorkSpaceId + "CloseSelectEnvironmentDialog", string.Empty); //Will return to tree view
        }

        #endregion

        #region Receive Input Environment Command

        private RelayCommand _ReceiveEnvCommand;
        public RelayCommand ReceiveEnvCommand
        {
            get
            {
                if (_ReceiveEnvCommand == null)
                {
                    _ReceiveEnvCommand = new RelayCommand(ExecuteReceiveEnvCommand, CanExecuteReceiveEnvCommand);
                }
                return _ReceiveEnvCommand;
            }
        }

        private bool CanExecuteReceiveEnvCommand()
        {
            if (SelectedEnv != null && SelectedEnv.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ExecuteReceiveEnvCommand()
        {
            foreach (UserEnvironmentsModel envr in UserEnvironments)
            {
                MessageMediator.NotifyColleagues(WorkSpaceId + "CloseSelectEnvironmentDialog", string.Empty); //Will return to tree view
                if (envr.environmentName == SelectedEnv)
                {
                    object[] parameters = new object[2];
                    parameters[0] = envr;
                    parameters[1] = projectNode;
                    MessageMediator.NotifyColleagues(WorkSpaceId + "ReceiveEnvDetailsAndImport", parameters);
                    break;
                }
            }
        }

        #endregion

    }
}
