using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;
using ATSBusinessLogic.ContentMgmtBLL;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using Infra.MVVM;


namespace ExplorerModule
{
    public class ContentManagementViewModel : ViewModelBase
    {

        #region  Data

        private IMessageBoxService MsgBoxService = null;

        private Guid WSId { get; set; }

        private List<CMTreeNode> contentTree { get; set; }

        public static Dictionary<int, CMFolderModel> folders = new Dictionary<int, CMFolderModel>();

        public static Dictionary<int, CMContentModel> contents = new Dictionary<int, CMContentModel>();

        public static Dictionary<int, CMVersionModel> versions = new Dictionary<int, CMVersionModel>();

        protected MessengerService MessageMediator = new MessengerService();

        private CMTreeViewNodeViewModelBase WorkNode;

        #endregion

        #region  Properties

        private ObservableCollection<CMTreeViewNodeViewModelBase> _TreeNodes = new ObservableCollection<CMTreeViewNodeViewModelBase>();
        public ObservableCollection<CMTreeViewNodeViewModelBase> TreeNodes
        {
            get
            {
                return _TreeNodes;
            }
            set
            {
                _TreeNodes = value;
                RaisePropertyChanged("TreeNodes");
            }
        }

        private CMTreeViewRootNodeViewModel RootNode
        {
            get
            {
                return TreeNodes[0] as CMTreeViewRootNodeViewModel;
            }
        }

        #endregion

        #region  Ctor

        public ContentManagementViewModel(Guid workspaceId)
        {
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            WSId = workspaceId;

            try
            {
                Domain.CallingAppName = Domain.AppName;
                contentTree = CMTreeNodeBLL.GetTreeObjects(out folders, out contents, out versions);
                Domain.CallingAppName = "";

                //Populate TreeView
                PopulateTreeView(0);
            }
            catch (Exception e)
            {
                //var SB = new StringBuilder(string.Empty);
                //SB.Append("SELECT Description FROM PE_Messages where id=105;");
                //MsgBoxService.ShowError(Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null).ToString());  
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Object[] ArgsList = new Object[] { e.Message };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
            }
        }

        #endregion

        #region  Populate TreeView from contentTree

        private void PopulateTreeView(long ParentId)
        {
            //If the Tree is empty, create the Environment node
            if (TreeNodes.Count == 0)
            {
                CMTreeNode TN = new CMTreeNode();
                TN.ID = 0;
                TN.ParentID = int.MinValue;
                TN.Name = Domain.Environment;
                TreeNodes.Add(new CMTreeViewRootNodeViewModel(this.WSId, TN));
                WorkNode = RootNode;
            }
            //Local variables initialization
            CMTreeViewNodeViewModelBase OperationRoot = WorkNode;
            OperationRoot.Children.Clear();

            //Scan the contentTree for parent nodes (nodes who are the direct children of the requested parent)
            var nodesList =
                from T in contentTree
                where T.ParentID == ParentId
                select T;

            CMTreeViewNodeViewModelBase TreeNode = null;
            foreach (CMTreeNode tn in nodesList)
            {
                if (tn.ID != 0)
                {
                    switch (tn.TreeNodeType.ToString())
                    {
                        case "Folder":
                            TreeNode = new CMTreeViewFolderNodeViewModel(this.WSId, tn, RootNode);
                            break;
                        case "Content":
                            TreeNode = new CMTreeViewContentNodeViewModel(this.WSId, tn, RootNode);
                            break;
                        case "ContentVersion":
                            TreeNode = new CMTreeViewVersionNodeViewModel(this.WSId, tn, RootNode);
                            break;
                        default:
                            TreeNode = new CMTreeViewFolderNodeViewModel(this.WSId, tn, RootNode);
                            break;

                    }
                    PopulateChildren(TreeNode); //Read the children of the newly created node (recursively)
                    OperationRoot.Children.Add(TreeNode);
                }
            }
        }

        private void PopulateChildren(CMTreeViewNodeViewModelBase Node)
        {
            List<CMTreeNode> nodesList = new List<CMTreeNode>();
            if (Node.TreeNode.TreeNodeType.ToString() == "Folder")
            {

                int folderId = Node.TreeNode.ID;
                nodesList.AddRange(folders[folderId].Nodes);
            }

            if (Node.TreeNode.TreeNodeType.ToString() == "Content")
            {
                //improved performance
                int conId = Node.TreeNode.ID;
                nodesList.AddRange(contents[conId].Versions.Values);
            }

            CMTreeViewNodeViewModelBase TreeNode = null;
            foreach (CMTreeNode tn in nodesList)
            {
                switch (tn.TreeNodeType.ToString())
                {
                    case "Folder":
                        TreeNode = new CMTreeViewFolderNodeViewModel(this.WSId, tn, Node);
                        break;
                    case "Content":
                        TreeNode = new CMTreeViewContentNodeViewModel(this.WSId, tn, Node);
                        break;
                    case "ContentVersion":
                        TreeNode = new CMTreeViewVersionNodeViewModel(this.WSId, tn, Node);
                        break;
                }
                PopulateChildren(TreeNode);
                Node.Children.Add(TreeNode);
            }
        }

        #endregion

        #region  Expand All TreeView Nodes

        private RelayCommand _ExpandAllCommand;
        public ICommand ExpandAllCommand
        {
            get
            {
                if (_ExpandAllCommand == null)
                {
                    _ExpandAllCommand = new RelayCommand(ExecuteExpandAllCommand, CanExecuteExpandAllCommand);
                }
                return _ExpandAllCommand;
            }
        }

        private bool CanExecuteExpandAllCommand()
        {
            return true;
        }

        private void ExecuteExpandAllCommand()
        {
            foreach (CMTreeViewNodeViewModelBase TVN in TreeNodes)
            {
                MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                TVN.IsExpanded = true;
                if (TVN.Children.Count > 0)
                {
                    ExecuteExpandAllChildrenCommand(TVN);
                }
            }
        }

        private void ExecuteExpandAllChildrenCommand(CMTreeViewNodeViewModelBase TVN)
        {
            foreach (CMTreeViewNodeViewModelBase TV in TVN.Children)
            {
                TV.IsExpanded = true;
                if (TV.Children.Count > 0)
                {
                    ExecuteExpandAllChildrenCommand(TV);
                }
            }
        }

        #endregion

        #region  Collapse All TreeView Nodes

        private RelayCommand _CollapseAllCommand;
        public ICommand CollapseAllCommand
        {
            get
            {
                if (_CollapseAllCommand == null)
                {
                    _CollapseAllCommand = new RelayCommand(ExecuteCollapseAllCommand, CanExecuteCollapseAllCommand);
                }
                return _CollapseAllCommand;
            }
        }

        private bool CanExecuteCollapseAllCommand()
        {
            return true;
        }

        private void ExecuteCollapseAllCommand()
        {
            foreach (CMTreeViewNodeViewModelBase TVN in TreeNodes)
            {
                MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                TVN.IsExpanded = false;
                if (TVN.Children.Count > 0)
                {
                    ExecuteCollapseAllChildrenCommand(TVN);
                }
            }
        }

        private void ExecuteCollapseAllChildrenCommand(CMTreeViewNodeViewModelBase TVN)
        {
            foreach (CMTreeViewNodeViewModelBase TV in TVN.Children)
            {
                TV.IsExpanded = false;
                if (TV.Children.Count > 0)
                {
                    ExecuteCollapseAllChildrenCommand(TV);
                }
            }
        }

        #endregion

    }
}