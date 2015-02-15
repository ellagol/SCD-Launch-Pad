using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Infra.MVVM;

namespace ContentMgmtModule
{
    public class CMSearchViewModel : ViewModelBase
    {

        #region Data

        protected MessengerService MessageMediator = new MessengerService();
        private IMessageBoxService MsgBoxService = null;

        private Guid WorkspaceId { get; set; }

        public string SearchTerm { get; set; }
        public Boolean SearchOnFolderName { get; set; }
        public Boolean SearchOnContentName { get; set; }
        public Boolean SearchOnVersionName { get; set; }
        public Boolean SearchOnFileName { get; set; }
        public Boolean SearchOnUser { get; set; }

        #endregion

        #region Constructor

        public CMSearchViewModel(Guid workspaceId)
		{
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();
            //Initialize
            SearchTerm = string.Empty;
            SearchOnFolderName = true;
            SearchOnContentName = true;
            SearchOnVersionName = true;
            SearchOnFileName = true;
            SearchOnUser = false;

            WorkspaceId = workspaceId;
        }

        #endregion

        #region  Search - Display Search parameters Flyout

        private RelayCommand _CMSearchCommand;
        public ICommand CMSearchCommand
        {
            get
            {
                if (_CMSearchCommand == null)
                {
                    _CMSearchCommand = new RelayCommand(ExecuteCMSearchCommand, CanExecuteCMSearchCommand);
                }
                return _CMSearchCommand;
            }
        }

        private bool CanExecuteCMSearchCommand()
        {
            return true;
        }

        private void ExecuteCMSearchCommand()
        {
            Collection<object> SearchParameters = new Collection<object>();
            SearchParameters.Add(SearchTerm);
            SearchParameters.Add(SearchOnFolderName);
            SearchParameters.Add(SearchOnContentName);
            SearchParameters.Add(SearchOnVersionName);
            SearchParameters.Add(SearchOnFileName);
            SearchParameters.Add(SearchOnUser);

            MessageMediator.NotifyColleagues("HideSearchParams"); //Send message to the MainViewModel -- Fold the Bottom Flyout
            MessageMediator.NotifyColleagues(WorkspaceId + "SearchParameters", SearchParameters); //Send message to the ExplorerViewModel
        }

        #endregion

        #region Select All Parameters Command

        private RelayCommand _SelectAllParametersCommand;
        public ICommand SelectAllParametersCommand
        {
            get
            {
                if (_SelectAllParametersCommand == null)
                {
                    _SelectAllParametersCommand = new RelayCommand(ExecuteSelectAllParametersCommand, CanExecuteSelectAllParametersCommand);
                }
                return _SelectAllParametersCommand;
            }
        }

        private bool CanExecuteSelectAllParametersCommand()
        {
            return true;
        }

        private void ExecuteSelectAllParametersCommand()
        {
            SearchOnFolderName = true;
            SearchOnContentName = true;
            SearchOnVersionName = true;
            SearchOnFileName = true;
            SearchOnUser = true;

            RaisePropertyChanged("SearchOnFolderName");
            RaisePropertyChanged("SearchOnContentName");
            RaisePropertyChanged("SearchOnVersionName");
            RaisePropertyChanged("SearchOnFileName");
            RaisePropertyChanged("SearchOnUser");
        }

        #endregion

        #region Clear All Parameters Command

        private RelayCommand _ClearAllParametersCommand;
        public ICommand ClearAllParametersCommand
        {
            get
            {
                if (_ClearAllParametersCommand == null)
                {
                    _ClearAllParametersCommand = new RelayCommand(ExecuteClearAllParametersCommand, CanExecuteClearAllParametersCommand);
                }
                return _ClearAllParametersCommand;
            }
        }

        private bool CanExecuteClearAllParametersCommand()
        {
            return true;
        }

        private void ExecuteClearAllParametersCommand()
        {
            SearchTerm = string.Empty;
            SearchOnFolderName = false;    
            SearchOnContentName = false;
            SearchOnVersionName = false;
            SearchOnFileName = false;
            SearchOnUser = false;

            RaisePropertyChanged("SearchTerm");
            RaisePropertyChanged("SearchOnFolderName");
            RaisePropertyChanged("SearchOnContentName");
            RaisePropertyChanged("SearchOnVersionName");
            RaisePropertyChanged("SearchOnFileName");
            RaisePropertyChanged("SearchOnUser");
        }

        #endregion

    }

}
