using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Infra.MVVM;

namespace ExplorerModule
{
	public class SearchViewModel : ViewModelBase
	{

        protected MessengerService MessageMediator = new MessengerService();
        private IMessageBoxService MsgBoxService = null;

        public string SearchTerm { get; set; }
        public Boolean SearchOnName { get; set; }
        public Boolean SearchOnDescription { get; set; }
        public Boolean SearchOnUser { get; set; }
        public Boolean SearchOnProject { get; set; }
        public Boolean SearchOnNotes { get; set; }
        public Boolean SearchOnContent { get; set; }
        public Boolean SearchOnStep { get; set; }

        private Guid WorkspaceId { get; set; }

        public SearchViewModel(Guid workspaceId)
		{
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();
            //Initialize
            SearchTerm = string.Empty;
            SearchOnName = true;
            SearchOnStep = true;
            SearchOnDescription = true;
            SearchOnUser = true;
            SearchOnProject = true;
            SearchOnNotes = true;
            SearchOnContent = true;
            WorkspaceId = workspaceId;
        }

        #region  Search - Display Search parameters Flyout

        private RelayCommand _SearchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_SearchCommand == null)
                {
                    _SearchCommand = new RelayCommand(ExecuteSearchCommand, CanExecuteSearchCommand);
                }
                return _SearchCommand;
            }
        }

        private bool CanExecuteSearchCommand()
        {
            return true;
        }

        private void ExecuteSearchCommand()
        {
            Collection<object> SearchParameters = new Collection<object>();
            SearchParameters.Add(SearchTerm);
            SearchParameters.Add(SearchOnName);
            SearchParameters.Add(SearchOnDescription);
            SearchParameters.Add(SearchOnUser);
            SearchParameters.Add(SearchOnProject);
            SearchParameters.Add(SearchOnNotes);
            SearchParameters.Add(SearchOnContent);
            SearchParameters.Add(SearchOnStep);
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
            SearchOnName = true;
            SearchOnDescription = true;
            SearchOnUser = true;
            SearchOnProject = true;
            SearchOnNotes = true;
            SearchOnContent = true;
            SearchOnStep = true;

            RaisePropertyChanged("SearchOnName");
            RaisePropertyChanged("SearchOnDescription");
            RaisePropertyChanged("SearchOnUser");
            RaisePropertyChanged("SearchOnProject");
            RaisePropertyChanged("SearchOnNotes");
            RaisePropertyChanged("SearchOnContent");
            RaisePropertyChanged("SearchOnStep");
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
            SearchOnName = false;
            SearchOnDescription = false;
            SearchOnUser = false;
            SearchOnProject = false;
            SearchOnNotes = false;
            SearchOnContent = false;
            SearchOnStep = false;

            RaisePropertyChanged("SearchOnName");
            RaisePropertyChanged("SearchOnDescription");
            RaisePropertyChanged("SearchOnUser");
            RaisePropertyChanged("SearchOnProject");
            RaisePropertyChanged("SearchOnNotes");
            RaisePropertyChanged("SearchOnContent");
            RaisePropertyChanged("SearchOnStep");
        }

        #endregion

	}

} //end of root namespace