using System;
using ATSBusinessObjects.ContentMgmtModels;
using Infra.MVVM;

namespace ContentMgmtModule
{
	public class CMTreeViewRootNodeViewModel : CMTreeViewNodeViewModelBase
	{

    	#region  Data 

		private IMessageBoxService MsgBoxService = null;

	    #endregion

        #region Constructor

        public CMTreeViewRootNodeViewModel(Guid workspaceId, CMTreeNode TN)
            : this(workspaceId, TN, null)
		{
		}

        public CMTreeViewRootNodeViewModel(Guid workspaceId, CMTreeNode TN, CMTreeViewNodeViewModelBase ParentNode)
            : base(workspaceId, TN, ParentNode)
        {
			//Message Box Service
			MsgBoxService = GetService<IMessageBoxService>();
			//The messageMediator is registered in the ViewModelBase - Generally you have 1 mediator; Hence, the restricted access to the constructor
			MessageMediator = GetService<MessengerService>();
		}

        #endregion

        #region Node Data

        public override string NodeData
		{
			get
			{
				return this.Name; //Here you can place any content you want to see in the Tree for this node...
			}
		}

        private bool _isAddFolderToRoot;
        public bool IsAddFolderToRoot
        {
            get
            {
                return _isAddFolderToRoot;
            }
            set
            {
                _isAddFolderToRoot = value;
                RaisePropertyChanged("IsAddFolderToRoot");
            }
        }

        #endregion

        #region Other Methods

        public override void Refresh()
		{
			this.Children.Clear();
			this.LoadChildren();
			RaisePropertyChanged("NodeData");
		}

		public override void LoadChildren()
		{
		}

        //Sends a message to the MainWindow with the required information to display a details view of the currently selected node
        protected override void DisplayDetailsView()
        {
            MessageMediator.NotifyColleagues(WorkSpaceId + "ShowRootDetails", this); //Will be returned to the Explorer Main signed for this message
        }

        #endregion

    }

} //end of root namespace