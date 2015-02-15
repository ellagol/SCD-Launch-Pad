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
using ATSBusinessLogic;


namespace ExplorerModule
{
    public class ContentManagementReplaceViewModel : ViewModelBase
    {

        #region  Data

        private IMessageBoxService MsgBoxService = null;

        private Guid WSId { get; set; }

        private List<CMTreeNode> contentTree { get; set; }

        private List<CMTreeNode> contentUsedSubTree = new List<CMTreeNode>();

        private List<CMTreeNode> contentFullTree = new List<CMTreeNode>();

        private List<CMTreeNode> contentTSubTree = new List<CMTreeNode>();

        private List<CMTreeNode> contentUsedTSubTree = new List<CMTreeNode>();

        public static Dictionary<int, CMFolderModel> folders = new Dictionary<int, CMFolderModel>();

        public static Dictionary<int, CMContentModel> contents = new Dictionary<int, CMContentModel>();

        public static Dictionary<int, CMVersionModel> versions = new Dictionary<int, CMVersionModel>();

        public static Dictionary<int, CMFolderModel> usedsubfolders = new Dictionary<int, CMFolderModel>();

        public static Dictionary<int, CMContentModel> usedsubcontents = new Dictionary<int, CMContentModel>();

        public static Dictionary<int, CMVersionModel> usedsubversions = new Dictionary<int, CMVersionModel>();

        public static Dictionary<int, CMFolderModel> tsubfolders = new Dictionary<int, CMFolderModel>();

        public static Dictionary<int, CMContentModel> tsubcontents = new Dictionary<int, CMContentModel>();

        public static Dictionary<int, CMVersionModel> tsubversions = new Dictionary<int, CMVersionModel>();

        public static Dictionary<int, CMFolderModel> usedtsubfolders = new Dictionary<int, CMFolderModel>();

        public static Dictionary<int, CMContentModel> usedtsubcontents = new Dictionary<int, CMContentModel>();

        public static Dictionary<int, CMVersionModel> usedtsubversions = new Dictionary<int, CMVersionModel>();

        public static Dictionary<int, CMFolderModel> fullfolders = new Dictionary<int, CMFolderModel>();

        public static Dictionary<int, CMContentModel> fullcontents = new Dictionary<int, CMContentModel>();

        public static Dictionary<int, CMVersionModel> fullversions = new Dictionary<int, CMVersionModel>();

        protected MessengerService MessageMediator = new MessengerService();

        private CMTreeViewNodeViewModelBase WorkNode;

        private TreeViewNodeViewModelBase PEParentNode = null;

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

        public ContentManagementReplaceViewModel(Guid workspaceId, TreeViewNodeViewModelBase PeNode)
        {
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            WSId = workspaceId;

            try
            {
                PEParentNode = PeNode;
                Domain.CallingAppName = Domain.AppName;
                contentTree = CMTreeNodeBLL.GetTreeObjects(out folders, out contents, out versions);
                Domain.CallingAppName = "";

                contentFullTree = contentTree;
                fullfolders = folders;
                fullcontents = contents;
                fullversions = versions;

                ContentManagementViewModel.folders = folders;
                ContentManagementViewModel.contents = contents;
                ContentManagementViewModel.versions = versions;

                //Populate TreeView
                PopulateTreeView(0);
            }
            catch (Exception e)
            {
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

        #region Filter by existing contents

        private RelayCommand _FilterByExistingCommand;
        public ICommand FilterByExistingCommand
        {
            get
            {
                if (_FilterByExistingCommand == null)
                {
                    _FilterByExistingCommand = new RelayCommand(ExecuteFilterByExistingCommand, CanExecuteFilterByExistingCommand);
                }
                return _FilterByExistingCommand;
            }
        }

        private bool CanExecuteFilterByExistingCommand()
        {
            return true;
        }

        private void ExecuteFilterByExistingCommand()
        {
            List<int> usedContentIds = new List<int>();
            List<int> usedContentVersionIds = new List<int>();
            try
            {
                if (!IsFilteredUsed) //pressed Apply filter button
                {
                    if (!IsTFilteredUsed) //Only projects filter applied
                    {
                        if (contentUsedSubTree == null || contentUsedSubTree.Count == 0) //Applying filter for the first time
                        {
                            HierarchyBLL.GetAllUsedCM((int)PEParentNode.Hierarchy.Id, out usedContentIds, out usedContentVersionIds);

                            if (usedContentIds != null && usedContentVersionIds != null)
                            {
                                contentTree = CMTreeNodeBLL.GetSubTreeObjects(usedContentIds, usedContentVersionIds, out folders, out contents, out versions);
                            }
                            PopulateSubTreeFromTree(ref contentUsedSubTree, ref usedsubfolders, ref usedsubcontents, ref usedsubversions);
                        }
                        PopulateTreeFromSubTree(ref contentUsedSubTree, ref usedsubfolders, ref usedsubcontents, ref usedsubversions);

                        PopulateTreeView(0);
                    }
                    else //both filters applied
                    {
                        if (contentUsedTSubTree == null || contentUsedTSubTree.Count == 0) //Applying both filters for the first time
                        {
                            HierarchyBLL.GetAllTemplatesUsedCM((int)PEParentNode.Hierarchy.Id, out usedContentIds, out usedContentVersionIds);

                            if (usedContentIds != null && usedContentVersionIds != null)
                            {
                                contentTree = CMTreeNodeBLL.GetSubTreeObjects(usedContentIds, usedContentVersionIds, out folders, out contents, out versions);
                            }
                            PopulateSubTreeFromTree(ref contentUsedTSubTree, ref usedtsubfolders, ref usedtsubcontents, ref usedtsubversions);
                        }
                        PopulateTreeFromSubTree(ref contentUsedTSubTree, ref usedtsubfolders, ref usedtsubcontents, ref usedtsubversions);

                        PopulateTreeView(0);
                    }
                    //Switch to Remove filter button
                    ButtonIconPath = "pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/IconFilterUsedCancel.png";
                    ButtonToolTip = "Remove filter";
                    IsFilteredUsed = !IsFilteredUsed; //switch
                }
                else //pressed Remove filter button
                {
                    if (!IsTFilteredUsed) //Only projects filter was active
                    {
                        PopulateTreeFromSubTree(ref contentFullTree, ref fullfolders, ref fullcontents, ref fullversions);

                        PopulateTreeView(0);
                    }
                    else //both filters were active
                    {
                        if (contentTSubTree == null || contentTSubTree.Count == 0) //Applying T filter for the first time
                        {
                            HierarchyBLL.GetAllTemplatesCM((int)PEParentNode.Hierarchy.Id, out usedContentIds, out usedContentVersionIds);

                            if (usedContentIds != null && usedContentVersionIds != null)
                            {
                                contentTree = CMTreeNodeBLL.GetSubTreeObjects(usedContentIds, usedContentVersionIds, out folders, out contents, out versions);
                            }
                            PopulateSubTreeFromTree(ref contentTSubTree, ref tsubfolders, ref tsubcontents, ref tsubversions);
                        }
                        PopulateTreeFromSubTree(ref contentTSubTree, ref tsubfolders, ref tsubcontents, ref tsubversions);

                        PopulateTreeView(0);
                    }
                    //Switch to Apply filter button
                    ButtonIconPath = "pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/IconFilterUsed.png";
                    ButtonToolTip = "Only contents used in projects within selected subtree";
                    IsFilteredUsed = !IsFilteredUsed; //switch
                }
                ExecuteCollapseAllCommand();
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Object[] ArgsList = new Object[] { e.Message };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
            }
        }


        #endregion

        #region Filter by template contents

        private RelayCommand _FilterByTemplatesCommand;
        public ICommand FilterByTemplatesCommand
        {
            get
            {
                if (_FilterByTemplatesCommand == null)
                {
                    _FilterByTemplatesCommand = new RelayCommand(ExecuteFilterByTemplatesCommand, CanExecuteFilterByTemplatesCommand);
                }
                return _FilterByTemplatesCommand;
            }
        }

        private bool CanExecuteFilterByTemplatesCommand()
        {
            return true;
        }

        private void ExecuteFilterByTemplatesCommand()
        {
            List<int> usedContentIds = new List<int>();
            List<int> usedContentVersionIds = new List<int>();
            try
            {
                if (!IsTFilteredUsed) //pressed Apply filter button
                {
                    if (!IsFilteredUsed) //only T filter applied
                    {
                        if (contentTSubTree == null || contentTSubTree.Count == 0) //Applying filter for the first time
                        {
                            HierarchyBLL.GetAllTemplatesCM((int)PEParentNode.Hierarchy.Id, out usedContentIds, out usedContentVersionIds);

                            if (usedContentIds != null && usedContentVersionIds != null)
                            {
                                contentTree = CMTreeNodeBLL.GetSubTreeObjects(usedContentIds, usedContentVersionIds, out folders, out contents, out versions);
                            }
                            PopulateSubTreeFromTree(ref contentTSubTree, ref tsubfolders, ref tsubcontents, ref tsubversions);
                        }
                        PopulateTreeFromSubTree(ref contentTSubTree, ref tsubfolders, ref tsubcontents, ref tsubversions);

                        PopulateTreeView(0);
                    }
                    else //both filters applied
                    {
                        if (contentUsedTSubTree == null || contentUsedTSubTree.Count == 0) //Applying both filters for the first time
                        {
                            HierarchyBLL.GetAllTemplatesUsedCM((int)PEParentNode.Hierarchy.Id, out usedContentIds, out usedContentVersionIds);

                            if (usedContentIds != null && usedContentVersionIds != null)
                            {
                                contentTree = CMTreeNodeBLL.GetSubTreeObjects(usedContentIds, usedContentVersionIds, out folders, out contents, out versions);
                            }
                            PopulateSubTreeFromTree(ref contentUsedTSubTree, ref usedtsubfolders, ref usedtsubcontents, ref usedtsubversions);
                        }
                        PopulateTreeFromSubTree(ref contentUsedTSubTree, ref usedtsubfolders, ref usedtsubcontents, ref usedtsubversions);

                        PopulateTreeView(0);
                    }
                    //Switch to Remove filter button
                    TButtonIconPath = "pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/IconFilterTCancel.png";
                    TButtonToolTip = "Remove filter";
                    IsTFilteredUsed = !IsTFilteredUsed; //switch
                }
                else //pressed Remove filter button
                {
                    if (!IsFilteredUsed) //only T filter was applied
                    {
                        PopulateTreeFromSubTree(ref contentFullTree, ref fullfolders, ref fullcontents, ref fullversions);

                        PopulateTreeView(0);
                    }
                    else //both filters were applied
                    {
                        if (contentUsedSubTree == null || contentUsedSubTree.Count == 0) //Applying filter for the first time
                        {
                            HierarchyBLL.GetAllUsedCM((int)PEParentNode.Hierarchy.Id, out usedContentIds, out usedContentVersionIds);

                            if (usedContentIds != null && usedContentVersionIds != null)
                            {
                                contentTree = CMTreeNodeBLL.GetSubTreeObjects(usedContentIds, usedContentVersionIds, out folders, out contents, out versions);
                            }
                            PopulateSubTreeFromTree(ref contentUsedSubTree, ref usedsubfolders, ref usedsubcontents, ref usedsubversions);
                        }
                        PopulateTreeFromSubTree(ref contentUsedSubTree, ref usedsubfolders, ref usedsubcontents, ref usedsubversions);

                        PopulateTreeView(0);
                    }
                    //Switch to Apply filter button
                    TButtonIconPath = "pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/IconFilterT.png";
                    TButtonToolTip = "Only contents used in templates within selected subtree";
                    IsTFilteredUsed = !IsTFilteredUsed; //switch
                }
                ExecuteCollapseAllCommand();
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Object[] ArgsList = new Object[] { e.Message };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
            }
        }
        #endregion

        private void PopulateSubTreeFromTree(ref List<CMTreeNode> subTree, ref Dictionary<int, CMFolderModel> subFolders,
                                             ref Dictionary<int, CMContentModel> subContents,
                                             ref Dictionary<int, CMVersionModel> subVersions)
        {
            subTree = contentTree;
            subFolders = folders;
            subContents = contents;
            subVersions = versions;
        }

        private void PopulateTreeFromSubTree(ref List<CMTreeNode> subTree, ref Dictionary<int, CMFolderModel> subFolders,
                                                    ref Dictionary<int, CMContentModel> subContents,
                                                    ref Dictionary<int, CMVersionModel> subVersions)
        {
            contentTree = subTree;
            folders = subFolders;
            contents = subContents;
            versions = subVersions;
        }

        #region buttons switch

        private string _ButtonIconPath = "pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/IconFilterUsed.png";
        public string ButtonIconPath
        {
            get
            {
                return _ButtonIconPath;
            }
            set
            {
                _ButtonIconPath = value;
                RaisePropertyChanged("ButtonIconPath");
            }
        }

        private string _ButtonToolTip = "Only contents used in projects within selected subtree";
        public string ButtonToolTip
        {
            get
            {
                return _ButtonToolTip;
            }
            set
            {
                _ButtonToolTip = value;
                RaisePropertyChanged("ButtonToolTip");
            }
        }


        private bool _IsFilteredUsed = false;
        public bool IsFilteredUsed
        {
            get
            {
                return _IsFilteredUsed;
            }
            set
            {
                _IsFilteredUsed = value;
            }
        }

        private string _TButtonIconPath = "pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/IconFilterT.png";
        public string TButtonIconPath
        {
            get
            {
                return _TButtonIconPath;
            }
            set
            {
                _TButtonIconPath = value;
                RaisePropertyChanged("TButtonIconPath");
            }
        }

        private string _TButtonToolTip = "Only contents used in templates within selected subtree";
        public string TButtonToolTip
        {
            get
            {
                return _TButtonToolTip;
            }
            set
            {
                _TButtonToolTip = value;
                RaisePropertyChanged("TButtonToolTip");
            }
        }


        private bool _IsTFilteredUsed = false;
        public bool IsTFilteredUsed
        {
            get
            {
                return _IsTFilteredUsed;
            }
            set
            {
                _IsTFilteredUsed = value;
            }
        }
        #endregion
    }
}