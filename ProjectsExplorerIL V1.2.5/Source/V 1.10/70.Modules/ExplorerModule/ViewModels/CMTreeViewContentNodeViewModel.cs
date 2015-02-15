using System;
using ATSBusinessObjects.ContentMgmtModels;
using Infra.MVVM;

namespace ExplorerModule
{
    public class CMTreeViewContentNodeViewModel : CMTreeViewNodeViewModelBase
    {      
        #region  Data

        private IMessageBoxService MsgBoxService = null;

        #endregion

        #region Constructor

        public CMTreeViewContentNodeViewModel(Guid workspaceId, CMTreeNode TN)
            : this(workspaceId, TN, null)
        {
        }

        public CMTreeViewContentNodeViewModel(Guid workspaceId, CMTreeNode TN, CMTreeViewNodeViewModelBase ParentNode)
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

        #endregion
    }
} 
