using System;
using System.Collections.ObjectModel;
using Infra.MVVM;

namespace ExplorerModule
{
	public class EnvironmentDetailsViewModel : ViewModelBase
	{

        protected MessengerService MessageMediator = new MessengerService();
        private IMessageBoxService MsgBoxService = null;

        private Guid WorkspaceId;

		private string _Name = "";
		public string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
				RaisePropertyChanged("Name");
			}
		}

        private TreeViewNodeViewModelBase Node;
        public ObservableCollection<TreeViewNodeViewModelBase> SubFolders
        {
            get
            {
                return Node.SubFolders;
            }
        }

        public EnvironmentDetailsViewModel(Guid workspaceId, TreeViewNodeViewModelBase TV)
		{
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();
            // Double-Click
            _DoubleClickCommand = new RelayCommand<object>(ExecuteDoubleClickCommand);
            // Initialize Object
            this.WorkspaceId = workspaceId;
            Node = TV;
            _Name = TV.Name;
		}

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
            int I = SubFolders.IndexOf((TreeViewNodeViewModelBase)Param);
            TreeViewNodeViewModelBase TV = SubFolders[I];
            MessageMediator.NotifyColleagues(WorkspaceId + "RequestSubFolderDrillDown", TV); //Will be returned to the ProjectsExplorer signed for this message
        }

        #endregion
	}

} //end of root namespace