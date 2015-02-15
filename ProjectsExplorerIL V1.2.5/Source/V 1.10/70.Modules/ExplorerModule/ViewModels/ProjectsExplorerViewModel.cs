﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ATSBusinessLogic;
using ATSBusinessObjects;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using Infra.DragDrop;
using Infra.MVVM;
using NotesModule;
using System.Threading;

namespace ExplorerModule
{

    #region  Node Operations Enum

    public enum NodeOperations : int
    {
        Cut,
        Copy,
        Paste
    }

    #endregion

    public class ProjectsExplorerViewModel : WorkspaceViewModelBase, IDropTarget
    {
        #region  Data

        private Collection<HierarchyModel> HierarchyDb; //Collection of Hierarchy Entities for a specific parent (or the entire environment)
        private StringCollection Permissions; //Collection of permissions this user is allowed to perform

        protected MessengerService MessageMediator = new MessengerService();
        private IMessageBoxService MsgBoxService = null;

        private TreeViewNodeViewModelBase WorkNode; //Used for Cut\Copy\Paste\Locate
        private NodeOperations? WorkOper; //Used for Cut\Copy\Paste

        public static bool CanPasteAfterCut = false;

        #region Progress Bar

        private float totalProgress = 0f;

        private String p_ProgressText = "";
        public String ProgressText
        {
            get { return p_ProgressText; }

            set
            {
                RaisePropertyChanged("ProgressText");
                p_ProgressText = value;
                RaisePropertyChanged("ProgressText");
            }
        }

        private String p_TitleText = "Archving Project data ...";
        public String TitleText
        {
            get { return p_TitleText; }

            set
            {
                RaisePropertyChanged("TitleText");
                p_TitleText = value;
                RaisePropertyChanged("TitleText");
            }
        }

        //The maximum progress value.
        private int p_ProgressMax = 10;
        public int ProgressMax
        {
            get { return p_ProgressMax; }

            set
            {
                RaisePropertyChanged("ProgressMax");
                p_ProgressMax = value;
                RaisePropertyChanged("ProgressMax");
            }
        }

        private float p_Progress = 0;
        public float Progress
        {
            get { return p_Progress; }

            set
            {
                RaisePropertyChanged("Progress");
                p_Progress = value;
                RaisePropertyChanged("Progress");
            }
        }

        #endregion

        #region CM integration

        public static Dictionary<int, CMFolderModel> folders = ContentBLL.folders;
        public static Dictionary<int, CMContentModel> contents = ContentBLL.contents;
        public static Dictionary<int, CMVersionModel> versions = ContentBLL.versions;
        public static List<CMTreeNode> allContents = ContentBLL.allContents;

        #endregion CM integration

        #endregion

        #region  Properties

        private ObservableCollection<TreeViewNodeViewModelBase> _TreeNodes = new ObservableCollection<TreeViewNodeViewModelBase>();
        public ObservableCollection<TreeViewNodeViewModelBase> TreeNodes
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

        private ViewModelBase _DetailsViewModel = null;
        public ViewModelBase DetailsViewModel
        {
            get
            {
                return _DetailsViewModel;
            }
            set
            {
                _DetailsViewModel = value;
                RaisePropertyChanged("DetailsViewModel");
            }
        }

        public Collection<object> SearchParameters { get; set; }

        private string _SearchText = string.Empty;
        public string SearchText
        {
            get
            {
                return _SearchText;
            }
            set
            {
                _SearchText = value;
                RaisePropertyChanged("SearchText");
            }
        }

        private TreeViewRootNodeViewModel RootNode
        {
            get
            {
                return TreeNodes[0] as TreeViewRootNodeViewModel;
            }
        }

        #endregion

        #region  Ctor

        public ProjectsExplorerViewModel()
            : base("ExplorerView", "")
        {
            //Initialize this VM
            DisplayName = "Explorer";
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();
            //Register as Subscriber to messages from other VMs
            MessageMediator.Register(this.WSId + "AddChildNode", new Action<Collection<object>>(OnAddChildNodeReceived)); //Register to recieve a message containing Add Child Node Request Parameters
            MessageMediator.Register(this.WSId + "CutCopyPasteNode", new Action<Collection<object>>(OnCutCopyPasteNodeReceived)); //Register to recieve a message containing Cut\Copy\Paste Node Request Parameters
            MessageMediator.Register(this.WSId + "DeleteNode", new Action<TreeViewNodeViewModelBase>(OnDeleteNodeReceived)); //Register to recieve a message containing HierarchyId of Node to Delete
            MessageMediator.Register(this.WSId + "UpdateNode", new Action<HierarchyModel>(OnUpdateNodeReceived)); //Register to recieve a message containing Hierarchy of Node to update
            MessageMediator.Register(this.WSId + "ShowEnvironmentDetails", new Action<object>(OnShowEnvironmentDetailsReceived)); //Register to recieve a message to display Environment Details on the Explorer right-hand side
            MessageMediator.Register(this.WSId + "ShowFolderDetails", new Action<TreeViewNodeViewModelBase>(OnShowFolderDetailsReceived)); //Register to recieve a message containing HierarchyId of a Folder to be displayed
            MessageMediator.Register(this.WSId + "ShowProjectDetails", new Action<TreeViewNodeViewModelBase>(OnShowProjectDetailsReceived)); //Register to recieve a message containing HierarchyId of a Project to be displayed
            MessageMediator.Register(this.WSId + "ShowTemplateDetails", new Action<TreeViewNodeViewModelBase>(OnShowTemplateDetailsReceived)); //Register to recieve a message containing HierarchyId of a Project to be displayed
            MessageMediator.Register(this.WSId + "CloseDetailsView", new Action<object>(OnCloseDetailsViewReceived)); //Register to recieve a message to display Environment Details on the Explorer right-hand side
            MessageMediator.Register(this.WSId + "RequestSubFolderDrillDown", new Action<TreeViewNodeViewModelBase>(OnShowFolderDetailsReceived)); //Register to recieve a message containing Sub-Folder Double-Click
            MessageMediator.Register(this.WSId + "RefreshNode", new Action<TreeViewNodeViewModelBase>(OnRefreshNodeReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WSId + "SearchParameters", new Action<Collection<object>>(OnSearchParametersReceived)); //Register to recieve a message containing search Parameters
            MessageMediator.Register(this.WSId + "AddClonedProject", new Action<HierarchyModel>(OnAddClonedProjectReceived)); //Register to recieve a message containing Hierarchy of Node to update
            MessageMediator.Register(this.WSId + "ShowVersionDetails", new Action<TreeViewNodeViewModelBase>(OnShowVersionDetailsReceived)); //Register to recieve a message containing search Parameters
            MessageMediator.Register(this.WSId + "ShowTemplateVersionDetails", new Action<TreeViewNodeViewModelBase>(OnShowTemplateVersionDetailsReceived)); //Register to recieve Template versions
            MessageMediator.Register(this.WSId + "OnUpdateVersionReceived", new Action<TreeViewVersionNodeViewModel>(OnUpdateVersionReceived)); //Register to recieve a message containing search Parameters
            MessageMediator.Register(this.WSId + "OnDirtyDrag", new Action<TreeViewNodeViewModelBase>(OnDirtyDrag));
            MessageMediator.Register(this.WSId + "ShowBulkUpdate", new Action<TreeViewNodeViewModelBase>(OnShowBulkUpdateReceived)); //Register to recieve a message containing HierarchyId of a bulk update to be displayed
            MessageMediator.Register(this.WSId + "ShowNewTemplate", new Action<TreeViewNodeViewModelBase>(OnShowNewTemplateReceived)); //Register message to open New Template 
            MessageMediator.Register(this.WSId + "UpdateGroupLastUpdate", new Action<HierarchyModel>(UpdateGroupLastUpdate));//Update All group last update date.
            MessageMediator.Register(this.WSId + "ShowCloneTemplate", new Action<TreeViewNodeViewModelBase>(OnShowCloneTemplateReceived)); //Register message to open New Template 
            MessageMediator.Register(this.WSId + "OnUpdateTemplateVersionReceived", new Action<TreeViewTemplateVersionNodeViewModel>(OnUpdateTemplateVersionReceived)); //Register to recieve a message containing search Parameters
            MessageMediator.Register(this.WSId + "AddImportedProject", new Action<HierarchyModel>(OnAddImportedProjectReceived)); //Register to recieve a message containing Hierarchy of Node to update
            MessageMediator.Register(this.WSId + "AddImportedFolder", new Action<List<int>>(OnAddImportedFoldersReceived)); //Register to recieve a message containing Hierarchy of Node to update
            MessageMediator.Register(this.WSId + "AddAndRefreshImportedProject", new Action<Collection<object>>(OnAddAndRefreshImportedProject));
            MessageMediator.Register(this.WSId + "FolderViewRapidExecution", new Action<TreeViewNodeViewModelBase>(OnFolderViewRapidExecutionReceived));
            MessageMediator.Register(this.WSId + "ImportProjectToEnvironment", new Action<object[]>(OnImportProjectToEnvReceived));
            MessageMediator.Register(this.WSId + "CloseSelectEnvironmentDialog", new Action<string>(OnRequestCloseSelectEnvironmentDialogReceived)); // To close Priority Popup
            MessageMediator.Register(this.WSId + "ReceiveEnvDetailsAndImport", new Action<object[]>(OnReceiveEnvDetailsAndImportReceived));
            MessageMediator.Register(this.WSId + "CloseProgressBar", new Action(CloseProgressBar));
            //Initialize
            SearchParameters = new Collection<object>();

            //Get PE_ProjectStep table
            HierarchyBLL.GetProjectStepDataTable();

            NoteBLL.GetListOfSpecialNotesHierarchyIds();

            //Read the Hierarchy for the Environment from the Db
            HierarchyDb = ReadHierarchy();

            //get content tree
            Domain.CallingAppName = Domain.AppName;
           
            ContentBLL.allContents = ContentBLL.LoadContentTreeToMemory(out ContentBLL.folders, out ContentBLL.contents, out ContentBLL.versions);
            allContents = ContentBLL.allContents;
            folders = ContentBLL.folders;
            contents = ContentBLL.contents;
            versions = ContentBLL.versions;
            Domain.CallingAppName = "";
     
            //Populate TreeView
            PopulateTreeView(0);
        }

        public ProjectsExplorerViewModel(object project)
            : base("ExplorerView", "")
        {
            CMWhereUsedProjectItemModel currProject = (CMWhereUsedProjectItemModel)project; //the project we want to focus on
            //Initialize this VM
            DisplayName = "Explorer";
            //Message Box ServiceF
            MsgBoxService = GetService<IMessageBoxService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();
            //Register as Subscriber to messages from other VMs
            MessageMediator.Register(this.WSId + "AddChildNode", new Action<Collection<object>>(OnAddChildNodeReceived)); //Register to recieve a message containing Add Child Node Request Parameters
            MessageMediator.Register(this.WSId + "CutCopyPasteNode", new Action<Collection<object>>(OnCutCopyPasteNodeReceived)); //Register to recieve a message containing Cut\Copy\Paste Node Request Parameters
            MessageMediator.Register(this.WSId + "DeleteNode", new Action<TreeViewNodeViewModelBase>(OnDeleteNodeReceived)); //Register to recieve a message containing HierarchyId of Node to Delete
            MessageMediator.Register(this.WSId + "UpdateNode", new Action<HierarchyModel>(OnUpdateNodeReceived)); //Register to recieve a message containing Hierarchy of Node to update
            MessageMediator.Register(this.WSId + "ShowEnvironmentDetails", new Action<object>(OnShowEnvironmentDetailsReceived)); //Register to recieve a message to display Environment Details on the Explorer right-hand side
            MessageMediator.Register(this.WSId + "ShowFolderDetails", new Action<TreeViewNodeViewModelBase>(OnShowFolderDetailsReceived)); //Register to recieve a message containing HierarchyId of a Folder to be displayed
            MessageMediator.Register(this.WSId + "ShowProjectDetails", new Action<TreeViewNodeViewModelBase>(OnShowProjectDetailsReceived)); //Register to recieve a message containing HierarchyId of a Project to be displayed
            MessageMediator.Register(this.WSId + "ShowTemplateDetails", new Action<TreeViewNodeViewModelBase>(OnShowTemplateDetailsReceived)); //Register to recieve a message containing HierarchyId of a Project to be displayed
            MessageMediator.Register(this.WSId + "CloseDetailsView", new Action<object>(OnCloseDetailsViewReceived)); //Register to recieve a message to display Environment Details on the Explorer right-hand side
            MessageMediator.Register(this.WSId + "RequestSubFolderDrillDown", new Action<TreeViewNodeViewModelBase>(OnShowFolderDetailsReceived)); //Register to recieve a message containing Sub-Folder Double-Click
            MessageMediator.Register(this.WSId + "RefreshNode", new Action<TreeViewNodeViewModelBase>(OnRefreshNodeReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WSId + "SearchParameters", new Action<Collection<object>>(OnSearchParametersReceived)); //Register to recieve a message containing search Parameters
            MessageMediator.Register(this.WSId + "AddClonedProject", new Action<HierarchyModel>(OnAddClonedProjectReceived)); //Register to recieve a message containing Hierarchy of Node to update
            MessageMediator.Register(this.WSId + "ShowVersionDetails", new Action<TreeViewNodeViewModelBase>(OnShowVersionDetailsReceived)); //Register to recieve a message containing search Parameters
            MessageMediator.Register(this.WSId + "ShowTemplateVersionDetails", new Action<TreeViewNodeViewModelBase>(OnShowTemplateVersionDetailsReceived)); //Register to recieve Template versions
            MessageMediator.Register(this.WSId + "OnUpdateVersionReceived", new Action<TreeViewVersionNodeViewModel>(OnUpdateVersionReceived)); //Register to recieve a message containing search Parameters
            MessageMediator.Register(this.WSId + "OnDirtyDrag", new Action<TreeViewNodeViewModelBase>(OnDirtyDrag));
            MessageMediator.Register(this.WSId + "UpdateGroupLastUpdate", new Action<HierarchyModel>(UpdateGroupLastUpdate));//Update All group last update date.
            MessageMediator.Register(this.WSId + "ShowNewTemplate", new Action<TreeViewNodeViewModelBase>(OnShowNewTemplateReceived)); //Register message to open New Template 
            MessageMediator.Register(this.WSId + "AddImportedProject", new Action<HierarchyModel>(OnAddImportedProjectReceived)); //Register to recieve a message containing Hierarchy of Node to update
            MessageMediator.Register(this.WSId + "AddImportedFolder", new Action<List<int>>(OnAddImportedFoldersReceived)); //Register to recieve a message containing Hierarchy of Node to update
            MessageMediator.Register(this.WSId + "AddAndRefreshImportedProject", new Action<Collection<object>>(OnAddAndRefreshImportedProject));
            MessageMediator.Register(this.WSId + "FolderViewRapidExecution", new Action<TreeViewNodeViewModelBase>(OnFolderViewRapidExecutionReceived));
            MessageMediator.Register(this.WSId + "ImportProjectToEnvironment", new Action<object[]>(OnImportProjectToEnvReceived));
            MessageMediator.Register(this.WSId + "CloseSelectEnvironmentDialog", new Action<string>(OnRequestCloseSelectEnvironmentDialogReceived));
            MessageMediator.Register(this.WSId + "ReceiveEnvDetailsAndImport", new Action<object[]>(OnReceiveEnvDetailsAndImportReceived));
            MessageMediator.Register(this.WSId + "CloseProgressBar", new Action(CloseProgressBar));
            MessageMediator.Register(this.WSId + "ShowBulkUpdate", new Action<TreeViewNodeViewModelBase>(OnShowBulkUpdateReceived)); //Register to recieve a message containing HierarchyId of a bulk update to be displayed
            MessageMediator.Register(this.WSId + "ShowCloneTemplate", new Action<TreeViewNodeViewModelBase>(OnShowCloneTemplateReceived)); //Register message to open New Template 
            MessageMediator.Register(this.WSId + "OnUpdateTemplateVersionReceived", new Action<TreeViewTemplateVersionNodeViewModel>(OnUpdateTemplateVersionReceived)); //Register to recieve a message containing search Parameters
            
            // MessageMediator.Register(this.WSId + "SearchParameters", new Action<Collection<object>>(OnSearchParametersReceived)); //Register to recieve a message containing search Parameters
            //Initialize
            SearchParameters = new Collection<object>();
            //GetProjectStep table
            HierarchyBLL.GetProjectStepDataTable();
            //Read the Hierarchy for the Environment from the Db
            HierarchyDb = ReadHierarchy();

            //get content tree
            Domain.CallingAppName = Domain.AppName;
           
            ContentBLL.allContents = ContentBLL.LoadContentTreeToMemory(out ContentBLL.folders, out ContentBLL.contents, out ContentBLL.versions);
            allContents = ContentBLL.allContents;
            folders = ContentBLL.folders;
            contents = ContentBLL.contents;
            versions = ContentBLL.versions;
            Domain.CallingAppName = "";
     
            //Populate TreeView
            PopulateTreeView(0);

            //find project id by full path
            long currProjectId;
            string tempProjectPath = currProject.HierarchyPath.TrimStart('/');
            HierarchyBLL.HierarchyBLLReturnCode status = HierarchyBLL.GetNodeIdByFullPath(tempProjectPath, out currProjectId);

            if (status != HierarchyBLL.HierarchyBLLReturnCode.Success)
            {
                showMessage(148); //can't find project
            }
            else //expand tree on this project
            {
                ExecuteExpandOnTreeViewNodeCommand(currProjectId);
            }
                      
        }

        #endregion

        #region  Populate TreeView from Hierarchy

        private void PopulateTreeView(long ParentId)
        {
            //If the Tree is empty, create the Environment node
            if (TreeNodes.Count == 0)
            {
                HierarchyModel HM = new HierarchyModel();
               
                HM.Id = 0;
                HM.ParentId = long.MinValue;
                HM.NodeType = NodeTypes.R;
                HM.Name = Domain.Environment;
                HM.Description = HM.Name;
                TreeNodes.Add(new TreeViewRootNodeViewModel(this.WSId, HM));
                WorkNode = RootNode;
            }
            else
            {
                WorkNode = LocateNode(ParentId);
            }
            //Local variables initialization
            TreeViewNodeViewModelBase OperationRoot = WorkNode;
            OperationRoot.Children.Clear();
            //Scan the Hierarchy for parent nodes (nodes who are the direct children of the requested parent)
            var HierarchyList =
                from H in HierarchyDb
                where H.ParentId == ParentId
                select H;

            TreeViewNodeViewModelBase TreeNode = null;
            foreach (HierarchyModel HMWithinLoop in HierarchyList)
            {
                switch (HMWithinLoop.NodeType)
                {
                    case NodeTypes.F:
                        TreeNode = new TreeViewFolderNodeViewModel(this.WSId, HMWithinLoop, RootNode);
                        break;
                    case NodeTypes.P:
                        TreeNode = new TreeViewProjectNodeViewModel(this.WSId, HMWithinLoop, RootNode);
                        break;
                    case NodeTypes.V:
                        TreeNode = new TreeViewVersionNodeViewModel(this.WSId, HMWithinLoop, RootNode);
                        break;
                    case NodeTypes.T:
                        TreeNode = new TreeViewTemplateNodeViewModel(this.WSId, HMWithinLoop, RootNode);
                        break;
                    default:
                        TreeNode = new TreeViewFolderNodeViewModel(this.WSId, HMWithinLoop, RootNode);
                        break;
                }
                PopulateChildren(TreeNode); //Read the children of the newly created node (recursively)
                OperationRoot.Children.Add(TreeNode);
                //sort the children after additing (Improve performance should make AddSorted function)
                ObservableCollection<TreeViewNodeViewModelBase> SortedChilderen = new ObservableCollection<TreeViewNodeViewModelBase>(OperationRoot.Children.OrderBy(Children => Children.Name));
                OperationRoot.Children = SortedChilderen;
            }
        }

        private void PopulateChildren(TreeViewNodeViewModelBase Node)
        {
            var HierarchyList =
                from H in HierarchyDb
                where H.ParentId == Node.Hierarchy.Id
                select H;

            TreeViewNodeViewModelBase TreeNode = null;
            foreach (var HM in HierarchyList)
            {
                switch (HM.NodeType)
                {
                    case NodeTypes.F:
                        TreeNode = new TreeViewFolderNodeViewModel(this.WSId, HM, Node);
                        break;
                    case NodeTypes.P:
                        TreeNode = new TreeViewProjectNodeViewModel(this.WSId, HM, Node);
                        break;
                    case NodeTypes.V:
                        TreeNode = new TreeViewVersionNodeViewModel(this.WSId, HM, Node);
                        break;
                    case NodeTypes.T:
                        TreeNode = new TreeViewTemplateNodeViewModel(this.WSId, HM, Node);
                        break;
                    default:
                        TreeNode = new TreeViewFolderNodeViewModel(this.WSId, HM, Node);
                        break;
                }
                PopulateChildren(TreeNode);
                Node.Children.Add(TreeNode);
                //sort the children after additing (Improve performance should make AddSorted function)
                ObservableCollection<TreeViewNodeViewModelBase> SortedChilderen = new ObservableCollection<TreeViewNodeViewModelBase>(Node.Children.OrderBy(Children => Children.Name));
                Node.Children = SortedChilderen;

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
            foreach (TreeViewNodeViewModelBase TVN in TreeNodes)
            {
                MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                TVN.IsExpanded = true;
                if (TVN.Children.Count > 0)
                {
                    ExecuteExpandAllChildrenCommand(TVN);
                }
            }
        }

        private void ExecuteExpandAllChildrenCommand(TreeViewNodeViewModelBase TVN)
        {
            foreach (TreeViewNodeViewModelBase TV in TVN.Children)
            {
                TV.IsExpanded = true;
                if (TV.Children.Count > 0)
                {
                    ExecuteExpandAllChildrenCommand(TV);
                }
            }
        }

        #endregion

        #region  Expand On TreeView Node

        private RelayCommand<object> _ExpandOnTreeViewNodeCommand;
        public RelayCommand<object> ExpandOnTreeViewNodeCommand
        {
            get
            {
                if (_ExpandOnTreeViewNodeCommand == null)
                {
                    _ExpandOnTreeViewNodeCommand = new RelayCommand<object>(ExecuteExpandOnTreeViewNodeCommand, CanExecuteExpandOnTreeViewNodeCommand);
                }
                return _ExpandOnTreeViewNodeCommand;
            }
        }

        private bool CanExecuteExpandOnTreeViewNodeCommand(object Param)
        {
            return true;
        }

        private void ExecuteExpandOnTreeViewNodeCommand(object Param)
        {
            foreach (TreeViewNodeViewModelBase TVN in TreeNodes)
            {
                if (TVN.Children.Count > 0)
                {
                    ExecuteExpandAllChildrenAndFindNodeCommand(TVN, Param);
                }
            }
        }

        private void ExecuteExpandAllChildrenAndFindNodeCommand(TreeViewNodeViewModelBase TVN, object Param)
        {
            foreach (TreeViewNodeViewModelBase TV in TVN.Children)
            {
                //TV.IsExpanded = true;
                if (TV.Hierarchy.Id == (long)Param)
                {
                    TV.IsExpanded = true;
                    TV.IsSelected = true;
                    if (TV.NodeType == NodeTypes.P)
                    {
                        OnShowProjectDetailsReceived(TV);
                        return;
                    }
                    if (TV.NodeType == NodeTypes.T)
                    {
                        OnShowNewTemplateReceived(TV);
                        return;
                    }
                }
                if (TV.Children.Count > 0)
                {
                    ExecuteExpandAllChildrenAndFindNodeCommand(TV, Param);
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
            foreach (TreeViewNodeViewModelBase TVN in TreeNodes)
            {
                MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                TVN.IsExpanded = false;
                if (TVN.Children.Count > 0)
                {
                    ExecuteCollapseAllChildrenCommand(TVN);
                }
            }
        }

        private void ExecuteCollapseAllChildrenCommand(TreeViewNodeViewModelBase TVN)
        {
            foreach (TreeViewNodeViewModelBase TV in TVN.Children)
            {
                TV.IsExpanded = false;
                if (TV.Children.Count > 0)
                {
                    ExecuteCollapseAllChildrenCommand(TV);
                }
            }
        }

        #endregion

        #region  Find

        private IEnumerator<TreeViewNodeViewModelBase> _MatchingNodesEnumerator;
        private string PrevSearchString = string.Empty;

        private RelayCommand _FindCommand;
        public ICommand FindCommand
        {
            get
            {
                if (_FindCommand == null)
                {
                    _FindCommand = new RelayCommand(ExecuteFindCommand, CanExecuteFindCommand);
                }
                return _FindCommand;
            }
        }

        private bool CanExecuteFindCommand()
        {
            return SearchText.Length > 0;
        }

        private void ExecuteFindCommand()
        {
            if (SearchParameters.Count < 2) //In case the user did not go thru the Search control, rather just filled the search string and hit Find
            {
                SearchParameters.Clear();
                SearchParameters.Add(SearchText);
                SearchParameters.Add(true); //Name
                SearchParameters.Add(true); //Description
                SearchParameters.Add(true); //LastModifiedUser
                SearchParameters.Add(true); //Project Code
                SearchParameters.Add(true); //Notes
                SearchParameters.Add(true); //Content
                SearchParameters.Add(true); //Step
            }
            else
            {
                SearchParameters[0] = SearchText;
            }
            if (SearchText != PrevSearchString)
            {
                _MatchingNodesEnumerator = null;
                PrevSearchString = SearchText;
            }
            if (_MatchingNodesEnumerator == null || (!(_MatchingNodesEnumerator.MoveNext())))
            {
                if (SearchParameters[5].Equals(true)) //Search by notes
                {
                    NoteBLL.GetHierarchyIds(Convert.ToString(SearchParameters[0]));
                }

                if (SearchParameters[6].Equals(true)) //Search By Content Name
                {
                    HierarchyBLL.GetHierarchyIdsByContentName(Convert.ToString(SearchParameters[0]));
                }
                this.VerifyMatchingNodesEnumerator();
            }
            TreeViewNodeViewModelBase Node = _MatchingNodesEnumerator.Current;
            if (Node == null)
            {
                return;
            }
            //Ensure that this Node is in view.
            if (Node.Parent != null)
            {
                Node.Parent.IsExpanded = true;
            }
            Node.IsSelected = true;
            switch (Node.NodeType)
            {
                case NodeTypes.R:
                    {
                        OnShowEnvironmentDetailsReceived(null);
                        break;
                    }
                case NodeTypes.F:
                    {
                        OnShowFolderDetailsReceived(Node);
                        break;
                    }
                case NodeTypes.P:
                    {
                        //if (Node.Id != Node.Hierarchy.Id) //version
                        //{
                        //    OnShowVersionDetailsReceived(Node);
                        //}
                        //else
                        //{
                        //    OnShowProjectDetailsReceived(Node);
                        //}
                        OnShowProjectDetailsReceived(Node);
                        break;
                    }
                case NodeTypes.V:
                    {
                        OnShowVersionDetailsReceived(Node);
                        break;
                    }
                case NodeTypes.T:
                    {
                        //if (Node.Id != Node.Hierarchy.Id) //version
                        //{
                        //    OnShowTemplateVersionDetailsReceived(Node);
                        //}
                        //else
                        //{
                        //    OnShowNewTemplateReceived(Node);
                        //}
                        OnShowNewTemplateReceived(Node);
                        break;
                    }
                default:
                    OnCloseDetailsViewReceived(null);
                    break;
            }

        }

        private void VerifyMatchingNodesEnumerator()
        {
            var Matches = FindMatches(RootNode);
            _MatchingNodesEnumerator = Matches.GetEnumerator();
            if (!(_MatchingNodesEnumerator.MoveNext()))
            {
                MsgBoxService.ShowInformation("No nodes found containing searched data. Please try again.");
            }
        }

        private IEnumerable<TreeViewNodeViewModelBase> FindMatches(TreeViewNodeViewModelBase Node)
        {
            List<TreeViewNodeViewModelBase> L = new List<TreeViewNodeViewModelBase>();
            //
            if (Node.NodeDataContainsText(SearchParameters))
            {
                L.Add(Node);
            }
            //
            foreach (TreeViewNodeViewModelBase Child in Node.Children)
            {
                foreach (TreeViewNodeViewModelBase Match in this.FindMatches(Child))
                {
                    L.Add(Match);
                }
            }
            //
            return L;
        }

        #endregion

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
            MessageMediator.NotifyColleagues("ShowSearchParams", this.WSId); //Send message to the MainViewModel
        }

        #endregion

        #region  IDropTarget Members & Other Drop Activities

        public void DragOver(Infra.DragDrop.IDropInfo DropInfo)
        {
            TreeViewNodeViewModelBase SourceItem = DropInfo.Data as TreeViewNodeViewModelBase;
            TreeViewNodeViewModelBase TargetItem = DropInfo.TargetItem as TreeViewNodeViewModelBase;
            if (SourceItem != null && TargetItem != null)
            {
                if (TargetItem.NodeType != NodeTypes.V) //Do not allow to put things under Version node; only content goes here
                {
                    DropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    DropInfo.Effects = DragDropEffects.Move;
                }
                else
                {
                    DropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    DropInfo.Effects = DragDropEffects.None;
                }
            }
        }

        public void Drop(Infra.DragDrop.IDropInfo DropInfo)
        {
            if (!CanMakeDrag)
            {
                CanMakeDrag = true;
                return;
            }
            if (!Domain.IsPermitted("102"))
            {
                //MsgBoxService.ShowError("You are not permitted to modify Hierarchy structure");
                
                ShowErrorAndInfoMessage(106, new Object[] { 0 });
                return;
            }

            try
            {


                TreeViewNodeViewModelBase SourceItem = DropInfo.Data as TreeViewNodeViewModelBase;
                TreeViewNodeViewModelBase TargetItem = DropInfo.TargetItem as TreeViewNodeViewModelBase;
                if (!SourceItem.Hierarchy.IsDirty)
                {
                    //Temporary fix. If decide to uncomment, don't forget to uncomment also abortTrans and CommitTrans
                    //Domain.PersistenceLayer.BeginTransWithIsolation(IsolationLevel.Serializable);
                    
                    //Source and Target can't be the same
                    string updateCheck = HierarchyBLL.LastUpadateCheck(ref SourceItem._Hierarchy);
                    if (!(String.IsNullOrEmpty(updateCheck)))
                    {
                        //Domain.PersistenceLayer.AbortTrans();
                        MessageMediator.NotifyColleagues(this.WSId + "OnIsCanceledNodeReceived", SourceItem);
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(updateCheck), ArgsList);
                        return;
                    }
                    updateCheck = HierarchyBLL.LastUpadateCheck(ref TargetItem._Hierarchy);
                    if (!(String.IsNullOrEmpty(updateCheck)))
                    {
                        Domain.PersistenceLayer.AbortTrans();
                        MessageMediator.NotifyColleagues(this.WSId + "OnIsCanceledNodeReceived", SourceItem);
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(updateCheck), ArgsList);
                        return;
                    }
                    if (SourceItem.Hierarchy.Id == TargetItem.Hierarchy.Id)
                    {
                        //Domain.PersistenceLayer.AbortTrans();
                        MsgBoxService.ShowError("Invalid Drop operation - Item can't be dropped onto itself");
                        return;
                    }
                    if (TargetItem.Hierarchy.NodeType == NodeTypes.P)
                    {
                        //Domain.PersistenceLayer.AbortTrans();
                        MsgBoxService.ShowError("Invalid Drop operation - Item can't be dropped under Project node.");
                        return;
                    }
                    if (TargetItem.Hierarchy.NodeType == NodeTypes.T)
                    {
                        //Domain.PersistenceLayer.AbortTrans();
                        MsgBoxService.ShowError("Invalid Drop operation - Item can't be dropped under Template node.");
                        return;
                    }
                    //Target can't be child \ siebling of the source
                    HierarchyDb = ReadHierarchy(SourceItem.Hierarchy.Id);
                    foreach (HierarchyModel hm in HierarchyDb)
                    {
                        if (TargetItem.Hierarchy.Id == hm.Id)
                        {
                            //Domain.PersistenceLayer.AbortTrans();
                            MessageMediator.NotifyColleagues(this.WSId + "OnIsCanceledNodeReceived", SourceItem);
                            MsgBoxService.ShowError("Invalid Drop operation - Item can't be dropped onto itself");
                            return;
                        }
                    }
                    //
                    TreeViewNodeViewModelBase SourceNode = LocateNode(SourceItem.Hierarchy.Id);
                    TreeViewNodeViewModelBase ParentNode = null;
                    if (SourceItem.Parent.NodeType == NodeTypes.R)
                    {
                        ParentNode = RootNode;
                    }
                    else
                    {
                        ParentNode = LocateNode(SourceItem.Parent.Hierarchy.Id);
                    }
                    TreeViewNodeViewModelBase TargetNode = null;
                    if (TargetItem.NodeType == NodeTypes.R)
                    {
                        TargetNode = RootNode;
                    }
                    else
                    {
                        TargetNode = LocateNode(TargetItem.Hierarchy.Id);
                    }
                    //Test there is no such name within the target
                    StringBuilder SB = new StringBuilder(string.Empty);
                    SB.Clear();
                    SB.Append("SELECT COUNT(*) FROM PE_Hierarchy WHERE Name = '" + SourceNode.Hierarchy.Name + "' AND Id <> '" + SourceNode.Hierarchy.Id + "' AND ParentId ");
                    if (TargetNode == RootNode)
                    {
                        SB.Append("IS NULL");
                    }
                    else
                    {
                        SB.Append("= " + TargetNode.Hierarchy.Id);
                    }
                    Int16 Count = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));
                    if (Count > 0)
                    {
                        //Domain.PersistenceLayer.AbortTrans();
                        MessageMediator.NotifyColleagues(this.WSId + "OnIsCanceledNodeReceived", SourceItem);
                        MsgBoxService.ShowError(TargetNode.Name + " Already contains child node " + SourceNode.Hierarchy.Name);
                        return;
                    }

                    if (SourceNode.NodeType == NodeTypes.T && SourceNode.Hierarchy.ProjectStatus == "Open" 
                                                        && !isTemplateStepsAvalable(SourceItem, TargetItem)) 
                    {
                        return;
                    }
                    

                    WorkNode = null; //Clean after use
                    //Do the work; This is currently MOVING the SOURCE node to become a CHILD of the TARGET node
                    if (ParentNode != null)
                    {
                        foreach (TreeViewNodeViewModelBase Node in ParentNode.Children)
                        {
                            if (Node.Hierarchy.Id == SourceNode.Hierarchy.Id)
                            {
                                ParentNode.Children.Remove(Node);
                                break;
                            }
                        }
                    }
                    SourceNode.Parent = TargetNode;
                    TargetNode.Children.Add(SourceNode);
                    SourceNode.Hierarchy.ParentId = TargetNode.Hierarchy.Id;
                    HierarchyModel HM = SourceNode.Hierarchy;
                    HierarchyBLL.PersistHierarchyRow(ref HM); // Perform Db update (Update ParentId of the moved HierarchyId)
                    SourceNode.Hierarchy = HM;
                    //sort the children after additing (Improve performance should make AddSorted function)
                    ObservableCollection<TreeViewNodeViewModelBase> SortedChilderen = new ObservableCollection<TreeViewNodeViewModelBase>(TargetNode.Children.OrderBy(Children => Children.Name));
                    TargetNode.Children = SortedChilderen;
                    //Position the view to the newly created node and cleanup

                    TargetNode.IsSelected = true;

                    TargetNode.IsExpanded = true;
                    //Domain.PersistenceLayer.CommitTrans();
                }
                else
                    return;
            }
            catch (Exception E)
            {
                Domain.PersistenceLayer.AbortTrans();
                TreeViewNodeViewModelBase SourceItem = DropInfo.Data as TreeViewNodeViewModelBase;
                MessageMediator.NotifyColleagues(this.WSId + "OnIsCanceledNodeReceived", SourceItem);
                if (E.Message == "DB Error")
                {
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(141, ArgsList);
                }
                else
                {
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
                }
            }
        }
        private TreeViewNodeViewModelBase LocateNode(long HierarchyId)
        {
            WorkNode = null;
            if (HierarchyId == RootNode.Hierarchy.Id)
            {
                return RootNode;
            }
            else
            {
                TreeViewNodeViewModelBase tempNode1 = RootNode;
                LocateTreeChildren(ref tempNode1, HierarchyId);
                return WorkNode;
            }
        }

        private void LocateTreeChildren(ref TreeViewNodeViewModelBase Node, long HierarchyId)
        {
            foreach (TreeViewNodeViewModelBase TreeNode in Node.Children)
            {
                if (TreeNode.Hierarchy.Id == HierarchyId)
                {
                    WorkNode = TreeNode;
                    return;
                }
                TreeViewNodeViewModelBase tempNode1 = TreeNode;
                LocateTreeChildren(ref tempNode1, HierarchyId);
            }
        }

        #endregion

        #region  Debug

        private void DebugTree()
        {
            Debug.WriteLine("=============================================================================================");
            Debug.WriteLine(RootNode.Name + "\t" + "Orfand");
            DebugTreeChildren(RootNode);
            Debug.WriteLine("=============================================================================================");
        }

        private void DebugTreeChildren(TreeViewNodeViewModelBase Node)
        {
            foreach (TreeViewNodeViewModelBase TreeNode in Node.Children)
            {
                string ParentName = string.Empty;
                string Offset = string.Empty;
                string Err = string.Empty;
                if (Node.Parent != null)
                {
                    ParentName = TreeNode.Parent.Name;
                    Offset = new String(' ', TreeNode.NodeLevel);
                    if (Node.Name != ParentName)
                    {
                        Err = "****";
                    }
                }
                Debug.WriteLine(Offset + "I am: " + TreeNode.Name + "\t" + " Child of: " + Node.Name + "\t" + "Parent's name: " + ParentName + "\t" + Err);
                DebugTreeChildren(TreeNode);
            }
        }

        #endregion

        #region  Receive Messages from TreeView Nodes

        #region  Add Child Node

        private void OnAddChildNodeReceived(Collection<object> Parameters)
        {
            try
            {
                if (Parameters.Count < 2)
                {
                    return;
                }
                //
                NodeTypes ChildType = (NodeTypes)(Parameters[0]);
                long ParentId = ((TreeViewNodeViewModelBase)(Parameters[1])).Hierarchy.Id;
                //Locate Parent
                TreeViewNodeViewModelBase ParentNode = LocateNode(ParentId);
                //Create HierarchyModel for the new Node
                HierarchyModel HM = new HierarchyModel();
                HM.VM = new VersionModel();
                HM.VM.Contents.Clear();
                HM.Id = -1;
                HM.ParentId = ParentId;
                HM.NodeType = ChildType;
                switch (ChildType)
                {
                    case NodeTypes.F:
                        HM.Name = "New Folder";
                        break;
                    case NodeTypes.P:
                        HM.Name = "New Project";
                        break;
                    case NodeTypes.V:
                        HM.Name = "New Version";
                        break;
                    case NodeTypes.T:
                        HM.Name = "New Template";
                        break;
                }
                HM.Description = HM.Name;

                switch (HM.Status)
                {
                    case ProjectStatusEnum.O:
                        HM.ProjectStatus = "Open";
                        break;
                    case ProjectStatusEnum.D:
                        HM.ProjectStatus = "Disabled";
                        break;
                    default:
                        HM.ProjectStatus = "Open";
                        break;
                }
                ObservableCollection<HierarchyModel> activeChildren = GetALLProjectsFamily(ParentNode);

                if (HM.NodeType == NodeTypes.T)
                {
                    //ObservableCollection<string> _ProjectSteps = TemplateBLL.GetAllSteps(activeChildren);

                    //if (_ProjectSteps.Count == 0)
                    //{
                    //    Object[] ArgsList = new Object[] { 0 };
                    //    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(165, ArgsList);
                    //    return;

                    //}

                    Domain.ErrorHandling Status = TemplateBLL.IsStepAvailable(HM, ParentId);
                    if (Status.messsageId == "165")
                    {
                        Domain.PersistenceLayer.AbortTrans();
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Status.messsageId), Status.messageParams);
                        return;
                    }
                    if (Status.messsageId == "166" && !HM.IsNew)
                    {
                        Domain.PersistenceLayer.AbortTrans();
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Status.messsageId), Status.messageParams);
                        return;
                    }

                }
                
                //Add contents from template for new project.
                if (HM.NodeType == NodeTypes.P)
                    PopulateContents(HM);
                //Not saving template.
                if (HM.NodeType != NodeTypes.T)
                {
                    //CR 3483
                    if (HM.VM.Contents != null && HM.VM.Contents.Count > 0)
                    {
                        string defaultVersionName = VersionBLL.GenerateDefaultVersionName(-1);
                        if (defaultVersionName != string.Empty)
                        {
                            HM.VM.VersionName = defaultVersionName;
                        }
                        //else - inherited, no change
                        HM.VM.Description = HM.VM.VersionName;
                        string hierarchyPath = string.Empty;
                        HierarchyBLL.GetProjectFullPathByProjectId(HM.ParentId, out hierarchyPath);
                        HM.VM.TargetPath = Domain.PE_SystemParameters["ProjectLocalPath"] + "/" + hierarchyPath + "/" + HM.Name + "/" + HM.VM.VersionName;
                        HM.VM.EcrId = string.Empty;
                        //RaisePropertyChanged("EcrId");
                    }
                    HM = PersistHierarchyModel(HM);
                }
                //Create a Node for the newy created HierarchyModel, and add it to the children of the designated parent
                TreeViewNodeViewModelBase ChildNode = null;
                switch (ChildType)
                {
                    case NodeTypes.F:
                        ChildNode = new TreeViewFolderNodeViewModel(this.WSId, HM, ParentNode);
                        break;
                    case NodeTypes.P:
                        ChildNode = new TreeViewProjectNodeViewModel(this.WSId, HM, ParentNode);
                        break;
                    case NodeTypes.V:
                        ChildNode = new TreeViewVersionNodeViewModel(this.WSId, HM, ParentNode);
                        break;
                    case NodeTypes.T:
                        ChildNode = new TreeViewTemplateNodeViewModel(this.WSId, HM, ParentNode);
                        break;
                }
                ParentNode.Children.Add(ChildNode);
                //sort the children after additing (Improve performance should make AddSorted function)
                ObservableCollection<TreeViewNodeViewModelBase> SortedChilderen = new ObservableCollection<TreeViewNodeViewModelBase>(ParentNode.Children.OrderBy(Children => Children.Name));
                ParentNode.Children = SortedChilderen;
                //Position the view to the newly created node and cleanup

                ChildNode.IsSelected = true;

                ParentNode.IsExpanded = true;
                //ParentNode.IsSelected = false;
                WorkNode = null; //Clean after use
            }
            catch (Exception e)
            {
                Object[] ArgsList = new Object[] { 0 };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
            }
        }

        #endregion

        #region  Cut \ Copy \ Paste Node

        private void OnCutCopyPasteNodeReceived(Collection<object> Parameters)
        {
            try
            {
                //Two parameters expected
                if (Parameters.Count < 2)
                {
                    return;
                }
                //Local variables
                NodeOperations Oper = (NodeOperations)(Parameters[0]);
                //If Cut or Copy, only store Work values for the 'Paste' command
                if (Oper == NodeOperations.Cut || Oper == NodeOperations.Copy)
                {
                    WorkNode = LocateNode(((TreeViewNodeViewModelBase)(Parameters[1])).Hierarchy.Id); //Store for the Paste
                    WorkOper = Oper; //Store for the Paste
                    return;
                }
                //Paste Variables
                TreeViewNodeViewModelBase SourceNode = WorkNode; //Located as part of the Cut\Copy operation executed earlier
                TreeViewNodeViewModelBase ParentNode = null;

                if (SourceNode == null || SourceNode.Parent == null)
                {
                    var SBE = new StringBuilder(string.Empty);
                    SBE.Append("SELECT Description FROM PE_Messages where id=148;");
                    MsgBoxService.ShowError(Domain.PersistenceLayer.FetchDataValue(SBE.ToString(), CommandType.Text, null).ToString());
                    return;
                }


                if (SourceNode.Parent.NodeType == NodeTypes.R)
                {
                    ParentNode = RootNode;
                }
                else
                {
                    ParentNode = LocateNode(SourceNode.Parent.Hierarchy.Id);
                }
                TreeViewNodeViewModelBase TargetNode = LocateNode(((TreeViewNodeViewModelBase)(Parameters[1])).Hierarchy.Id);
                //Validate Paste request
                //1. Make sure we have a Cut \ Copy request stored in the stack
                if ((SourceNode == null) | (WorkOper.Equals(null)))
                {
                    return;
                }
                //2. Source can't be target
                if (SourceNode.Hierarchy.Id == TargetNode.Hierarchy.Id)
                {
                    showMessage(127);
                    return;
                }
                //3. If the target is a Project, the source must be a Version
                if (TargetNode.NodeType == NodeTypes.P && SourceNode.NodeType != NodeTypes.V)
                {
                    return;
                }
                //4. If the source is a Version, the target must be a Project
                if (SourceNode.NodeType == NodeTypes.V && TargetNode.NodeType != NodeTypes.P)
                {
                    return;
                }
                //5. Test there is no such name within the target
                StringBuilder SB = new StringBuilder(string.Empty);
                SB.Clear();
                SB.Append("SELECT COUNT(*) FROM PE_Hierarchy WHERE Name = '" + SourceNode.Hierarchy.Name + "' AND Id <> '" + SourceNode.Hierarchy.Id + "' AND ParentId ");
                if (TargetNode == RootNode)
                {
                    SB.Append("IS NULL");
                }
                else
                {
                    SB.Append("= " + TargetNode.Hierarchy.Id);
                }
                Int16 Count = Convert.ToInt16((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), System.Data.CommandType.Text, null)));
                if (Count > 0)
                {
                    MsgBoxService.ShowError(TargetNode.Name + " Already contains child node " + SourceNode.Hierarchy.Name);
                    return;
                }

                if (SourceNode.NodeType ==NodeTypes.T && SourceNode.Hierarchy.ProjectStatus != "Disabled" 
                                                        && !isTemplateStepsAvalable(SourceNode, TargetNode)) 
                {
                    return;
                }

                //6. Check if the target is not a child
                bool flag = false;
                CheckForOffspring(ref SourceNode, ref flag, SourceNode.Hierarchy.Id, TargetNode.Hierarchy.Id);
                if (flag == true)
                {
                    showMessage(127);
                    return;
                }
                //7. If the source is a Template, the target must be a Folder
                //if (SourceNode.NodeType == NodeTypes.T && TargetNode.NodeType != NodeTypes.F)
                //{
                //    return;
                //}
                ///// Finally - Do the Paste work \\\
                //1. If it is a Copy operation, create a new Node (which is a clone of the source node)
                TreeViewNodeViewModelBase NewNode = null;
                if (WorkOper == NodeOperations.Copy)
                {
                    HierarchyModel HM = new HierarchyModel();

                    HM.Id = -1;
                    HM.ParentId = TargetNode.Hierarchy.Id;
                    HM.NodeType = SourceNode.NodeType;
                    HM.Name = "Copy of " + SourceNode.Name;
                    HM.Description = SourceNode.Description;
                    HM.Code = SourceNode.Hierarchy.Code;
                    HM.SelectedStep = SourceNode.Hierarchy.SelectedStep;
                    HM.Synchronization = SourceNode.Hierarchy.Synchronization;
                    HM.ProjectStatus = SourceNode.Hierarchy.ProjectStatus;
                    HM = PersistHierarchyModel(HM);

                    CopyNotes(SourceNode.Hierarchy.Id, HM.Id);
                    CopyCertificates(SourceNode.Hierarchy.Id, HM.Id);
                    CopyContent(SourceNode.Hierarchy.Id, HM.Id);

                    switch (SourceNode.NodeType)
                    {
                        case NodeTypes.F:
                            NewNode = new TreeViewFolderNodeViewModel(this.WSId, HM, TargetNode);
                            break;
                        case NodeTypes.P:
                            NewNode = new TreeViewProjectNodeViewModel(this.WSId, HM, TargetNode);
                            break;
                        case NodeTypes.V:
                            NewNode = new TreeViewVersionNodeViewModel(this.WSId, HM, TargetNode);
                            break;
                        case NodeTypes.T:
                            NewNode = new TreeViewTemplateNodeViewModel(this.WSId, HM, TargetNode);
                            break;
                    }
                    //Scan the children of the cloned source node, and clone them under the cloned node
                    HierarchyDb.Clear();
                    GetSubHierarchy(SourceNode); //Convert sub-tree to a collection of HierarchyModel
                    //Scan the Hierarchy for parent nodes
                    var HierarchyList =
                        from H in HierarchyDb
                        where H.ParentId == SourceNode.Hierarchy.Id
                        select H;

                    TreeViewNodeViewModelBase TreeNode = null;
                    foreach (HierarchyModel HMWithinLoop in HierarchyList)
                    {
                        HM = HMWithinLoop;
                        //Create and persist a new HierarchyModel
                        HierarchyModel NHM = new HierarchyModel();
                        NHM.Id = -1;
                        NHM.ParentId = NewNode.Hierarchy.Id;
                        NHM.NodeType = HMWithinLoop.NodeType;
                        NHM.Name = HMWithinLoop.Name;
                        NHM.Description = HMWithinLoop.Description;
                        NHM = PersistHierarchyModel(NHM);
                        //Create a new Node
                        switch (HMWithinLoop.NodeType)
                        {
                            case NodeTypes.F:
                                TreeNode = new TreeViewFolderNodeViewModel(this.WSId, NHM, NewNode);
                                break;
                            case NodeTypes.P:
                                TreeNode = new TreeViewProjectNodeViewModel(this.WSId, NHM, NewNode);
                                break;
                            case NodeTypes.V:
                                TreeNode = new TreeViewVersionNodeViewModel(this.WSId, NHM, NewNode);
                                break;
                            case NodeTypes.T:
                                TreeNode = new TreeViewTemplateNodeViewModel(this.WSId, NHM, NewNode);
                                break;
                            default:
                                TreeNode = new TreeViewFolderNodeViewModel(this.WSId, NHM, NewNode);
                                break;
                        }
                        PopulateSubHierarchyChildren(TreeNode, HMWithinLoop.Id);
                        NewNode.Children.Add(TreeNode);
                    }
                }
                //3. If it is a Cut operation: (1) Remove the node from the children of the Parent; (2) Update Parent of the affected node
                if (WorkOper == NodeOperations.Cut)
                {
                    foreach (TreeViewNodeViewModelBase N in ParentNode.Children)
                    {
                        if (N.Hierarchy.Id == SourceNode.Hierarchy.Id)
                        {
                            ParentNode.Children.Remove(N); // Update the TreeView
                            N.Hierarchy.ParentId = TargetNode.Hierarchy.Id;
                            HierarchyModel HM = N.Hierarchy;
                            HierarchyBLL.PersistHierarchyRow(ref HM); // Perform Db update (Update ParentId of the moved HierarchyId)
                            break;
                        }
                    }
                }
                //2. Update the target node's children
                switch (WorkOper)
                {
                    case NodeOperations.Cut:
                        TargetNode.Children.Add(SourceNode);
                        ExecuteExpandAllChildrenCommand(TargetNode);
                        break;
                    case NodeOperations.Copy:
                        TargetNode.Children.Add(NewNode);
                        ExecuteExpandAllChildrenCommand(TargetNode);
                        break;
                }
                //Refresh
                OnRefreshNodeReceived(SourceNode);
                //Clean
                WorkNode = null;
                WorkOper = null;
                //Set Display Properties
                TargetNode.IsExpanded = true;
                TargetNode.IsSelected = true;
            }
            catch (Exception e)
            {
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
          
               // throw new Exception("DB Error");
            }
        }

        private void GetSubHierarchy(TreeViewNodeViewModelBase Node)
        {
            foreach (TreeViewNodeViewModelBase TreeNode in Node.Children)
            {
                HierarchyModel HM = new HierarchyModel();
                HM.Id = TreeNode.Hierarchy.Id;
                HM.ParentId = TreeNode.Parent.Hierarchy.Id;
                HM.NodeType = TreeNode.NodeType;
                HM.Name = TreeNode.Name;
                HM.Description = TreeNode.Description;
                HierarchyDb.Add(HM);
                GetSubHierarchy(TreeNode);
            }
        }

        private void PopulateSubHierarchyChildren(TreeViewNodeViewModelBase Node, long QueryId)
        {
            try
            {
                var HierarchyList =
                    from H in HierarchyDb
                    where H.ParentId == QueryId
                    select H;

                TreeViewNodeViewModelBase TreeNode = null;
                foreach (var HM in HierarchyList)
                {
                    //Create and persist a new HierarchyModel
                    HierarchyModel NHM = new HierarchyModel();
                    NHM.Id = -1;
                    NHM.ParentId = Node.Hierarchy.Id;
                    NHM.NodeType = HM.NodeType;
                    NHM.Name = HM.Name;
                    NHM.Description = HM.Description;
                    NHM = PersistHierarchyModel(NHM);
                    //Create a new Node
                    switch (HM.NodeType)
                    {
                        case NodeTypes.F:
                            TreeNode = new TreeViewFolderNodeViewModel(this.WSId, NHM, Node);
                            break;
                        case NodeTypes.P:
                            TreeNode = new TreeViewProjectNodeViewModel(this.WSId, NHM, Node);
                            break;
                        case NodeTypes.V:
                            TreeNode = new TreeViewVersionNodeViewModel(this.WSId, NHM, Node);
                            break;
                        case NodeTypes.T:
                            TreeNode = new TreeViewTemplateNodeViewModel(this.WSId, NHM, Node);
                            break;
                        default:
                            TreeNode = new TreeViewFolderNodeViewModel(this.WSId, NHM, Node);
                            break;
                    }
                    PopulateSubHierarchyChildren(TreeNode, HM.Id);
                    Node.Children.Add(TreeNode);
                    ObservableCollection<TreeViewNodeViewModelBase> SortedChilderen = new ObservableCollection<TreeViewNodeViewModelBase>(Node.Children.OrderBy(Children => Children.Name));
                    Node.Children = SortedChilderen;
                }
            }
            catch (Exception e)
            {
                Object[] ArgsList = new Object[] { 0 };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);

            }
        }

        private void CopyNotes(long FromHierarchyId, long ToHierarchyId)
        {
        }
        private void CopyCertificates(long FromHierarchyId, long ToHierarchyId)
        {
        }
        private void CopyContent(long FromHierarchyId, long ToHierarchyId)
        {
        }

        #endregion

        #region Update Node's Details

        private void OnUpdateNodeReceived(HierarchyModel Hierarchy)
        {
            TreeViewNodeViewModelBase Node = LocateNode(Hierarchy.Id);
            Node.Hierarchy = Hierarchy;
            Node.Name = Hierarchy.Name;

            //update Add Note button
            NotesControlViewModel.UpdateProjectStatusFotes(Hierarchy.ProjectStatus);
            

            if (Hierarchy.NodeType == NodeTypes.F)
            {
                if (NoteBLL.CheckForSpecialNotes(Hierarchy.Id)) //give sign on icon if there are special notes
                {
                    Node.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "FolderSpecial"), UriKind.RelativeOrAbsolute));
                    Node.LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "FolderSpecial"), UriKind.RelativeOrAbsolute));
                }
                else
                {
                    Node.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "Folder"), UriKind.RelativeOrAbsolute));
                    Node.LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "Folder"), UriKind.RelativeOrAbsolute));
                }
            }
            else if (Hierarchy.NodeType == NodeTypes.P)
            {
                if (Hierarchy.GroupId == -1)
                {
                    if (Hierarchy.ProjectStatus == "Disabled")
                    {

                        if (NoteBLL.CheckForSpecialNotes(Hierarchy.Id)) //give sign on icon if there are special notes
                        {
                            Node.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "ProjectDisSpecial"), UriKind.RelativeOrAbsolute));
                            Node.LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "ProjectDisableSpeciel"), UriKind.RelativeOrAbsolute));
                        }
                        else
                        {
                            Node.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "ProjectDis"), UriKind.RelativeOrAbsolute));
                            Node.LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "ProjectDisable"), UriKind.RelativeOrAbsolute));
                        }                    
                    }
                    else
                    {
                        if (NoteBLL.CheckForSpecialNotes(Hierarchy.Id)) //give sign on icon if there are special notes
                        {
                            Node.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "ProjectSpecial"), UriKind.RelativeOrAbsolute));
                            Node.LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "ProjectSpeciel"), UriKind.RelativeOrAbsolute));
                        }
                        else
                        {
                            Node.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "Project"), UriKind.RelativeOrAbsolute));
                            Node.LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "Project"), UriKind.RelativeOrAbsolute));
                        }
                       
                    }
                }

                else
                {
                    if (Hierarchy.ProjectStatus == "Disabled")
                    {
                        Node.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "groupClosed"), UriKind.RelativeOrAbsolute));
                        Node.LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "groupClosedSpeciel"), UriKind.RelativeOrAbsolute));
                    }
                    else
                    {
                        Node.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "groupActive"), UriKind.RelativeOrAbsolute));
                        Node.LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "groupActiveSpeciel"), UriKind.RelativeOrAbsolute));
                    }
                }
                
            }
            else if (Hierarchy.NodeType == NodeTypes.T)
            {
                if (Hierarchy.ProjectStatus == "Disabled") //give sign on icon if there are special notes
                {
                    if (NoteBLL.CheckForSpecialNotes(Hierarchy.Id)) //give sign on icon if there are special notes
                    {
                        Node.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "TemplateDisableSpesialsmall"), UriKind.RelativeOrAbsolute));
                        Node.LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "TemplateDisableSpesialBig"), UriKind.RelativeOrAbsolute));
                    }
                    else
                    {
                        Node.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "TemplateDisable"), UriKind.RelativeOrAbsolute));
                        Node.LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "TemplateDisable"), UriKind.RelativeOrAbsolute));
                    }
                }
                else
                {
                    if (NoteBLL.CheckForSpecialNotes(Hierarchy.Id)) //give sign on icon if there are special notes
                    {
                        Node.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "TemplateSpesialSmall"), UriKind.RelativeOrAbsolute));
                        Node.LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "TemplateSpesialBig"), UriKind.RelativeOrAbsolute));
                    }
                    else
                    {
                        Node.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "Template"), UriKind.RelativeOrAbsolute));
                        Node.LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "Template"), UriKind.RelativeOrAbsolute));
                    }
                                       }
            }
        }

        #endregion

        #region Update Version icon

        private void OnUpdateVersionReceived(TreeViewVersionNodeViewModel tree)
        {

            TreeViewNodeViewModelBase curProject = LocateNode(tree.Hierarchy.Id);
            foreach (TreeViewNodeViewModelBase V in curProject.Children)
            {
                if (!V.IsSelected)
                {
                    V.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "VersionDis"), UriKind.RelativeOrAbsolute));
                }
                else
                {
                    V.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "Version"), UriKind.RelativeOrAbsolute));
                }
            }

           // OnRefreshNodeReceived(tree);
        }

        private void OnUpdateTemplateVersionReceived(TreeViewTemplateVersionNodeViewModel tree)
        {

            TreeViewNodeViewModelBase curProject = LocateNode(tree.Hierarchy.Id);
            foreach (TreeViewNodeViewModelBase V in curProject.Children)
            {
                if (!V.IsSelected)
                {
                    V.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "VersionDis"), UriKind.RelativeOrAbsolute));
                }
                else
                {
                    V.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "Version"), UriKind.RelativeOrAbsolute));
                }
            }
             //OnRefreshNodeReceived(tree);
        }

        #endregion

        #region  Delete Node

        private void OnDeleteNodeReceived(TreeViewNodeViewModelBase Node)
        {
            TreeViewNodeViewModelBase SourceNode = LocateNode(Node.Hierarchy.Id);
            TreeViewNodeViewModelBase ParentNode = null;
            if (SourceNode.Parent.NodeType == NodeTypes.R)
            {
                ParentNode = RootNode;
            }
            else
            {
                ParentNode = LocateNode(SourceNode.Parent.Hierarchy.Id);
            }
            foreach (TreeViewNodeViewModelBase N in ParentNode.Children)
            {
                if (N.Hierarchy.Id == SourceNode.Hierarchy.Id)
                {
                    ParentNode.Children.Remove(N);
                    break;
                }
            }
            //Clean
            WorkNode = null;
            WorkOper = null;
        }

        #endregion

        #region  Refresh Node

            private void OnRefreshNodeReceived(TreeViewNodeViewModelBase Node)
        {
            TreeViewNodeViewModelBase SourceNode = LocateNode(Node.Hierarchy.Id);
            //*SourceNode.IsSelected = false;
            TreeViewNodeViewModelBase ParentNode = LocateNode(Node.Hierarchy.ParentId);

            NoteBLL.GetListOfSpecialNotesHierarchyIds(); //Refresh special notes list
            if (SourceNode.NodeType == NodeTypes.R) // Root Node
            {
                //Clear the entire collection
                TreeNodes.Clear();
                //Read the Hierarchy for the Environment from the Db
                HierarchyDb = ReadHierarchy();
                //Populate TreeView
                PopulateTreeView(0);

                //Get Contents tree from CM - API
                try
                {
                    Domain.CallingAppName = Domain.AppName;
                    ContentBLL.allContents = ContentBLL.LoadContentTreeToMemory(out ContentBLL.folders, out ContentBLL.contents, out ContentBLL.versions);
                    Domain.CallingAppName = "";
                }
                catch (Exception e)
                {
                    String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                    Domain.SaveGeneralErrorLog(logMessage);
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(144, new object[] { 0 });
                }
            }
            else // Other node
            {
                //Bug 4050
                //Get Contents tree from CM - API
                try
                {
                    Domain.CallingAppName = Domain.AppName;
                    ContentBLL.allContents = ContentBLL.LoadContentTreeToMemory(out ContentBLL.folders, out ContentBLL.contents, out ContentBLL.versions);
                    Domain.CallingAppName = "";
                }
                catch (Exception e)
                {
                    String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                    Domain.SaveGeneralErrorLog(logMessage);
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(144, new object[] { 0 });
                }

                // Remove the node from the current parent
                ParentNode.Children.Remove(SourceNode);
                //ParentNode.IsSelected = false;
                //Read the Hierarchy for the node from the Db
                //HierarchyDb = ReadHierarchy(Node.Hierarchy.Id);
                HierarchyDb = ReadHierarchy();
                //Create a new node with the new data; cater for the node being moved by another user
                TreeViewNodeViewModelBase TreeNode = null;
                HierarchyModel HM = HierarchyBLL.GetHierarchyRow(Node.Hierarchy.Id);
                ParentNode = LocateNode(HM.ParentId);
                if (ParentNode == null)
                    return;
                switch (HM.NodeType)
                {
                    case NodeTypes.F:
                        TreeNode = new TreeViewFolderNodeViewModel(this.WSId, HM, ParentNode);
                        break;
                    case NodeTypes.P:
                        TreeNode = new TreeViewProjectNodeViewModel(this.WSId, HM, ParentNode);
                        break;
                    case NodeTypes.V:
                        TreeNode = new TreeViewVersionNodeViewModel(this.WSId, HM, ParentNode);
                        break;
                    case NodeTypes.T:
                        TreeNode = new TreeViewTemplateNodeViewModel(this.WSId, HM, ParentNode);
                        break;
                    default:
                        TreeNode = new TreeViewFolderNodeViewModel(this.WSId, HM, ParentNode);
                        break;
                }
                PopulateChildren(TreeNode); //Read the children of the newly created node (recursively)
                ParentNode.Children.Add(TreeNode);
                ObservableCollection<TreeViewNodeViewModelBase> SortedChilderen = new ObservableCollection<TreeViewNodeViewModelBase>(ParentNode.Children.OrderBy(Children => Children.Name));
                ParentNode.Children = SortedChilderen;
                //if (Node.NodeType == NodeTypes.P)
                //{
                //    ParentNode.IsSelected = false;
                //    Node.IsSelected = true;
                //    SourceNode.IsSelected = true;
                //    MessageMediator.NotifyColleagues(this.WSId + "OnIsSelectedNodeReceived", Node);
                //    MessageMediator.NotifyColleagues(this.WSId + "ShowProjectDetails", Node);
                   
                //}

                TreeViewNodeViewModelBase newSourceNode = LocateNode(Node.Hierarchy.Id);
                if (ParentNode.Children.Contains(newSourceNode) && newSourceNode.NodeType == NodeTypes.F)
                {
                    ParentNode.IsSelected = false;
                    TreeNode.IsExpanded = true;
                    TreeNode.IsSelected = true;
                    MessageMediator.NotifyColleagues(this.WSId + "OnIsSelectedNodeReceived", TreeNode);
                }
            }
        }

        #endregion

        #region  Show / Hide Details on right-hand side

        private void OnShowEnvironmentDetailsReceived(object NotUsed)
        {
            DetailsViewModel = new EnvironmentDetailsViewModel(this.WSId, RootNode);
        }

        private void OnShowFolderDetailsReceived(TreeViewNodeViewModelBase TV)
        {
            DetailsViewModel = new FolderDetailsViewModel(this.WSId, TV);
        }

        private void OnShowProjectDetailsReceived(TreeViewNodeViewModelBase TV)
        {
            DetailsViewModel = new ProjectDetailsViewModel(this.WSId, TV,  _TreeNodes);
        }

        private void OnShowTemplateDetailsReceived(TreeViewNodeViewModelBase TV)
        {
            DetailsViewModel = new CloneTemplateViewModel(this.WSId, TV, _TreeNodes);
        }

        private void OnShowVersionDetailsReceived(TreeViewNodeViewModelBase TV)
        {
            DetailsViewModel = new VersionDetailsViewModel(this.WSId, TV);
        }

        private void OnShowTemplateVersionDetailsReceived(TreeViewNodeViewModelBase TV)
        {
            DetailsViewModel = new TemplateVersionDetailsViewModel(this.WSId, TV);
        }

        private void OnCloseDetailsViewReceived(object NotUsed)
        {
            //DetailsViewModel = null;
        }

        private void OnShowBulkUpdateReceived(TreeViewNodeViewModelBase TV)
        {
            DetailsViewModel = new BulkUpdateViewModel(this.WSId, TV);
        }

        private void OnShowNewTemplateReceived(TreeViewNodeViewModelBase TV)
        {
            DetailsViewModel = new NewTemplateViewModel(this.WSId, TV);
        }

        private void OnShowCloneTemplateReceived(TreeViewNodeViewModelBase TV)
        {
            DetailsViewModel = new CloneTemplateViewModel(this.WSId, TV, TreeNodes);
        }

        #endregion

        #region  Add Clone Node

        private void OnAddClonedProjectReceived(HierarchyModel Hierarchy)
        {

           // NodeTypes ChildType = (NodeTypes)(Hierarchy);
           
            long ParentId = Hierarchy.ParentId;
            //Locate Parent;
            TreeViewNodeViewModelBase ParentNode = LocateNode(ParentId);
            //Create HierarchyModel for the new Node
          
           
            //Create a Node for the newy created HierarchyModel, and add it to the children of the designated parent
            TreeViewNodeViewModelBase ChildNode = null;

            if (Hierarchy.NodeType == NodeTypes.T)
                ChildNode = new TreeViewTemplateNodeViewModel(this.WSId, Hierarchy, ParentNode);
            else
                ChildNode = new TreeViewProjectNodeViewModel(this.WSId, Hierarchy, ParentNode);
          
            ParentNode.Children.Add(ChildNode);
            //sort the children after additing (to Improve performance should make AddSorted function)
            ObservableCollection<TreeViewNodeViewModelBase> SortedChilderen = new ObservableCollection<TreeViewNodeViewModelBase>(ParentNode.Children.OrderBy(Children => Children.Name));
            ParentNode.Children = SortedChilderen;

            //Position the view to the newly created node and cleanup
            ParentNode.IsExpanded = true;

            ChildNode.IsSelected = true;
            ChildNode._Hierarchy = HierarchyBLL.GetHierarchyRow(Hierarchy.Id);
            WorkNode = null; //Clean after use
        }

        #endregion

        #region Import

        private void OnAddImportedProjectReceived(HierarchyModel Hierarchy)
        {
            try
            {
                long ParentId = Hierarchy.ParentId;
                //Locate Parent;
                TreeViewNodeViewModelBase ParentNode = LocateNode(ParentId);
 
                //Create a Node for the newy created HierarchyModel, and add it to the children of the designated parent
                TreeViewNodeViewModelBase ChildNode = null;
                
                ChildNode = new TreeViewProjectNodeViewModel(this.WSId, Hierarchy, ParentNode);

                bool existingNode = false;
                foreach (TreeViewNodeViewModelBase child in ParentNode.Children)
                {
                    if (child.Hierarchy.Name == ChildNode.Hierarchy.Name)
                    {
                        existingNode = true;
                        ChildNode = child;
                        ChildNode._Hierarchy = HierarchyBLL.GetHierarchyRow(Hierarchy.Id);
                        break;
                    }
                }
                if (!existingNode)
                {
                    ParentNode.Children.Add(ChildNode);
                }

                ObservableCollection<TreeViewNodeViewModelBase> SortedChilderen = new ObservableCollection<TreeViewNodeViewModelBase>(ParentNode.Children.OrderBy(Children => Children.Name));
                ParentNode.Children = SortedChilderen;
                ParentNode.IsExpanded = true;

                ChildNode.IsSelected = true;
 
                //Position the view to the newly created node and cleanup

                ChildNode._Hierarchy = HierarchyBLL.GetHierarchyRow(Hierarchy.Id);
                OnShowProjectDetailsReceived(ChildNode);
                WorkNode = null; //Clean after use                
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Object[] ArgsList = new Object[] { 0 };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
            }
        }

        private void OnAddImportedFoldersReceived(List<int> subTreeIdsSorted)
        {
            try
            {
                foreach (int hId in subTreeIdsSorted)
                {
                    HierarchyModel Hierarchy = HierarchyBLL.GetHierarchyRow(hId);
                    if (Hierarchy.NodeType == NodeTypes.F)
                    {
                        long ParentId = Hierarchy.ParentId;
                        //Locate Parent;
                        TreeViewNodeViewModelBase ParentNode = LocateNode(ParentId);
                        //Create a Node for the newy created HierarchyModel, and add it to the children of the designated parent
                        TreeViewNodeViewModelBase ChildNode = null;

                        ChildNode = new TreeViewFolderNodeViewModel(this.WSId, Hierarchy, ParentNode);
                        bool existingNode = false;
                        foreach (TreeViewNodeViewModelBase child in ParentNode.Children)
                        {
                            if (child.Hierarchy.Name == ChildNode.Hierarchy.Name)
                            {
                                existingNode = true;
                                break;
                            }
                        }
                        if (!existingNode)
                        {
                            ParentNode.Children.Add(ChildNode);
                        }

                        //sort the children after additing (to Improve performance should make AddSorted function)
                        ObservableCollection<TreeViewNodeViewModelBase> SortedChilderen = new ObservableCollection<TreeViewNodeViewModelBase>(ParentNode.Children.OrderBy(Children => Children.Name));
                        ParentNode.Children = SortedChilderen;
                        ChildNode.IsExpanded = true;
                        ChildNode._Hierarchy = Hierarchy;
                        WorkNode = null; //Clean after use       
                    }
                    else
                    {
                        OnAddImportedProjectReceived(Hierarchy);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Object[] ArgsList = new Object[] { 0 };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
            }
        }

        private void OnAddAndRefreshImportedProject(Collection<object> Parameters)
        {
            try
            {
                if (Parameters.Count < 1)
                {
                    return;
                }
                //

                HierarchyModel importedProject = (HierarchyModel)Parameters[0];
                //Locate Parent
                TreeViewNodeViewModelBase ParentNode = LocateNode(0);
                //Clear the entire collection
                TreeNodes.Clear();
                //Read the Hierarchy for the Environment from the Db
                HierarchyDb = ReadHierarchy();
                //Populate TreeView
                PopulateTreeView(0);

                //Get Contents tree from CM - API
                try
                {
                    Domain.CallingAppName = Domain.AppName;
                    ContentBLL.allContents = ContentBLL.LoadContentTreeToMemory(out ContentBLL.folders, out ContentBLL.contents, out ContentBLL.versions);
                    Domain.CallingAppName = "";
                }
                catch (Exception e)
                {
                    String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                    Domain.SaveGeneralErrorLog(logMessage);
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(144, new object[] { 0 });
                }

                long projectParentId = importedProject.ParentId;
                TreeViewNodeViewModelBase projectParentNode = LocateNode(projectParentId);

                TreeViewNodeViewModelBase projectNode = new TreeViewProjectNodeViewModel(this.WSId, importedProject, projectParentNode);
                projectNode._Hierarchy = HierarchyBLL.GetHierarchyRow(importedProject.Id);
                projectParentNode.IsExpanded = true;
                projectNode.IsExpanded = true;
                projectNode.IsSelected = true;
                WorkNode = null; //Clean after use
            }
            catch (Exception e)
            {
                Object[] ArgsList = new Object[] { 0 };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
            }
        }

        #endregion Import

        #region Rapid Execution
        private void OnFolderViewRapidExecutionReceived(TreeViewNodeViewModelBase TV)
        {
            OnShowProjectDetailsReceived(TV);
            TreeViewNodeViewModelBase ParentNode = LocateNode(TV.Hierarchy.ParentId);
            foreach (TreeViewNodeViewModelBase child in ParentNode.Children)
            {
                if (child.Hierarchy.Id == TV.Hierarchy.Id)
                {
                    TV = child;
                    break;
                }
            }
            ParentNode.IsExpanded = true;
            TV.IsSelected = true;
        }

        #endregion

        #endregion

        #region Show message

        private void showMessage(int error)
        {
            Collection<string> StatusBarParameters = new Collection<string>();
            var SB = new StringBuilder(string.Empty);
            SB.Append("SELECT Description FROM PE_Messages where id=" + error + ";");
            string QrySteps = SB.ToString();
            // Fetch the DataTable from the database
            StatusBarParameters.Add((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString()); //Message
            StatusBarParameters.Add("White"); //Foreground
            StatusBarParameters.Add("Red"); //Background
            MessageMediator.NotifyColleagues("StatusBarParameters", StatusBarParameters);
        }

        #endregion

        #region  Other Methods

        //Read the Hierarchy table and return a collection of HierarchyModel entities
        //This will be replaced by calling the DAL and reading the hierarchy from the Db.

        private ObservableCollection<HierarchyModel> ReadHierarchy()
        {
            return ReadHierarchy(0);
        }

        public bool isTemplateStepsAvalable(TreeViewNodeViewModelBase SourceNode, TreeViewNodeViewModelBase TargetNode)
        {
            bool isAvalable = true;
            //ObservableCollection<HierarchyModel> activeChildren = GetALLProjectsFamily(TargetNode);
            //if (SourceNode.NodeType == NodeTypes.T)
            //{
            //    ObservableCollection<string> _ProjectSteps = TemplateBLL.GetAllSteps(activeChildren);

            //    if (_ProjectSteps.Count == 0 || !_ProjectSteps.Contains(SourceNode.Hierarchy.SelectedStep))
            //    {
            //        Object[] ArgsList = new Object[] { 0 };
            //        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(165, ArgsList);
            //        isAvalable = false;
            //    }
            //}

            Domain.ErrorHandling Status = TemplateBLL.IsStepAvailable(SourceNode.Hierarchy, TargetNode.Hierarchy.Id);
            if (Status.messsageId != string.Empty)
            {
                Domain.PersistenceLayer.AbortTrans();
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Status.messsageId), Status.messageParams);
                return false;
            }
            return isAvalable;
        }

        private ObservableCollection<HierarchyModel> ReadHierarchy(long ParentId)
        {
            return ATSBusinessLogic.HierarchyBLL.GetHierarchy(ParentId);
        }

        //Persist HierarchyModel entity
        private HierarchyModel PersistHierarchyModel(HierarchyModel HM)
        {
            try
            {
                HierarchyBLL.PersistHierarchyRow(ref HM); // Perform Db update
                return HM;
            }
            catch (Exception e)
            {
                Object[] ArgsList = new Object[] { 0 };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
                throw new Exception("DB Error");
                return HM;
            }
        }

        //Extended Search Parameters received
        private void OnSearchParametersReceived(Collection<object> Parameters)
        {
            //Seven parameters expected
            if (Parameters.Count < 7)
            {
                return;
            }
            //Local variables
            SearchText = (string)(Parameters[0]);
            SearchParameters = Parameters;
            //Execute
            ExecuteFindCommand();
        }

        //Check for offspring
        private void CheckForOffspring(ref TreeViewNodeViewModelBase tm, ref bool flag, long sourceHierarchyId, long targetHierarchyId)
        {
            foreach (TreeViewNodeViewModelBase TreeNode in tm.Children)
            {
                if (TreeNode.Hierarchy.Id == targetHierarchyId)
                {
                    flag = true;
                }
                else
                {
                    TreeViewNodeViewModelBase tempNode1 = TreeNode;
                    CheckForOffspring(ref tempNode1, ref flag, sourceHierarchyId, targetHierarchyId);
                }
            }
        }


        #endregion

        #region Messages

        //Status bar Error and Info messages
        public static MessengerService MessageMediatorErrorAndInfo = ServiceProvider.GetService<MessengerService>();
        public static void ShowErrorAndInfoMessage(int error, object[] Args)
        {
            try
            {
                Collection<string> StatusBarParameters = new Collection<string>();
                string Query = "SELECT Description, Type FROM PE_Messages where id=" + error + ";";

                // Fetch the row from the database (retrieving by PK --> 0 or 1 row)
                DataRow MsgRow = (DataRow)Domain.PersistenceLayer.FetchDataTable(Query, CommandType.Text, null).Rows[0];

                // Verify the message is found
                if (!string.IsNullOrEmpty((string)MsgRow["Description"]))
                {
                    //Message verbiage with parameters, as retrieved from the table
                    string MsgDescription = (string)MsgRow["Description"];

                    // Arguments substitution, if any
                    string MsgDescriptionWithParam = SetDescriptionParameters(MsgDescription, Args, error);

                    // Verify message description was successfully formatted 

                    // Message Type - consider defining enum for valid values
                    string MsgType = (string)MsgRow["Type"];

                    //Set background color based on message type
                    switch (MsgType.Trim())
                    {
                        case "I":
                            if (!MsgDescriptionWithParam.Equals(ErrorString))
                            {
                                StatusBarParameters = SetMessageDescriptionParam(MsgDescriptionWithParam, "White", "Green"); //Background for info messages
                            }
                            else
                            {
                                MsgDescription = MsgDescription + "(PE_Messages: Invalid number of parameters for Message Id " + error + ")";
                                StatusBarParameters = SetMessageDescriptionParam(MsgDescription, "White", "Green"); //When PE_Messages record contains invalid number of parameters
                            }
                            break;
                        case "E":
                            if (!MsgDescriptionWithParam.Equals(ErrorString))
                            {
                                StatusBarParameters = SetMessageDescriptionParam(MsgDescriptionWithParam, "White", "Red"); //Background for error messages
                            }
                            else
                            {
                                MsgDescription = MsgDescription + "(PE_Messages: Invalid number of parameters for Message Id " + error + ")";
                                StatusBarParameters = SetMessageDescriptionParam(MsgDescription, "White", "Red"); //When PE_Messages record contains invalid number of parameters
                            }
                            break;
                        case "W":
                            if (!MsgDescriptionWithParam.Equals(ErrorString))
                            {
                                StatusBarParameters = SetMessageDescriptionParam(MsgDescriptionWithParam, "White", "Blue"); //Background for info messages
                            }
                            else
                            {
                                MsgDescription = MsgDescription + "(PE_Messages: Invalid number of parameters for Message Id " + error + ")";
                                StatusBarParameters = SetMessageDescriptionParam(MsgDescription, "White", "Green"); //When PE_Messages record contains invalid number of parameters
                            }
                            break;
                        default:
                            StatusBarParameters = SetMessageDescriptionParam(MsgDescription, "White", "Red"); //invalid type of message
                            break;
                    }
                    MessageMediatorErrorAndInfo.NotifyColleagues("StatusBarParameters", StatusBarParameters);
                }
            }
            catch (Exception)
            {
                ShowGenericErrorMessage(error); //If DB connection failed or no rows selected
            }
        }

        //If DB connection failed or no rows selected
        private static void ShowGenericErrorMessage(int MsgCode)
        {
            Collection<string> StatusBarParametersGenericError = new Collection<string>();

            String ErrorMessageText = "Unable to retrieve error message " + MsgCode + ". Please see Data Access log file for more details: Shell->View Log."; //Message
            StatusBarParametersGenericError = SetMessageDescriptionParam(ErrorMessageText, "White", "Red");
            MessageMediatorErrorAndInfo.NotifyColleagues("StatusBarParameters", StatusBarParametersGenericError);
        }

        public static void ShowHardCodedErrorMessage(string messageText)
        {
            Collection<string> StatusBarParametersHardCodedError = new Collection<string>();

            String ErrorMessageText = messageText + ". Please see Data Access log file for more details: Shell->View Log."; //Message
            StatusBarParametersHardCodedError = SetMessageDescriptionParam(ErrorMessageText, "White", "Red");
            MessageMediatorErrorAndInfo.NotifyColleagues("StatusBarParameters", StatusBarParametersHardCodedError);
        }


        //To catch exception from String.Format function if occurs
        private static string ErrorString = "ParamError";
        private static string SetDescriptionParameters(string MessageDescription, object[] ArgList, int MsgCode)
        {
            try
            {
                string MsgDescriptionWithParam = String.Format(MessageDescription, ArgList);

                return MsgDescriptionWithParam;
            }
            catch (Exception)
            {
                return ErrorString;
            }
        }

        // Set status bar parameters
        private static Collection<String> SetMessageDescriptionParam(String MessageText, String FgColor, String BgColor)
        {
            Collection<string> StatusBarParametersAdd = new Collection<string>();

            StatusBarParametersAdd.Add(MessageText); //Message
            StatusBarParametersAdd.Add(FgColor); //Foreground
            StatusBarParametersAdd.Add(BgColor); //Background 

            return StatusBarParametersAdd;
        }

        #endregion

        private bool CanMakeDrag = true;
        private void OnDirtyDrag(TreeViewNodeViewModelBase TV)
        {

            CanMakeDrag = false;
        }

        #region  Refresh Command

        private RelayCommand _RefreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (_RefreshCommand == null)
                {
                    _RefreshCommand = new RelayCommand(ExecuteRefreshCommand, CanExecuteRefreshCommand);
                }
                return _RefreshCommand;
            }
        }

        private bool CanExecuteRefreshCommand()
        {
            return true;
        }

        private void ExecuteRefreshCommand()
        {
            bool IsFound = false;
            foreach (var i in this.TreeNodes)
            {
                if (i.Children.Count > 0)
                {
                    IsFound =CheckIsSelectedNode(i);
                    if (IsFound)
                        break;

                }
            }
           if(!IsFound)
                MessageMediator.NotifyColleagues(this.WSId + "OnRefreshReceived",TreeNodes[0]);
           TreeViewNodeViewModelBase.IsRefreshed = false;
           
            //this.Hierarchy.IsDirty = false;
            //this.Hierarchy.VM.IsDirty = false;
            //MessageMediator.NotifyColleagues(this.WorkSpaceId + "OnIsDirtyNodeReceived", null);
            //MessageMediator.NotifyColleagues(this.WSId + "OnRefreshReceived", this.WorkNode); //Will be returned to the MainWindow signed for this message
        }

        private bool  CheckIsSelectedNode(TreeViewNodeViewModelBase Nodes)
        {
            foreach (var j in Nodes.Children)
            {
                if (j.IsSelected)
                {
                    MessageMediator.NotifyColleagues(this.WSId + "OnRefreshReceived", j); //Will be returned to the MainWindow signed for this message
                    return true;
                }
                if (j.Children.Count > 0)
                {
                  bool IsFound = CheckIsSelectedNode(j);
                  if (IsFound)
                  {
                      return true;
                  }
                }
            }
            return false;
        }

        #endregion

        #region UpdateGroupLastUpdate

        private void UpdateGroupLastUpdate(HierarchyModel Hierarchy)
        {
            foreach(var node in TreeNodes)
            {
                if(node.NodeType == NodeTypes.P)
                {
                    if(node.Hierarchy.GroupId == Hierarchy.GroupId)
                        node.Hierarchy.GroupLastUpdateTime = Hierarchy.GroupLastUpdateTime;
                }
                if (node.Children.Count > 0)
                    UpdateGroupLastUpdate(Hierarchy, node.Children);
                    
            }
        }

        private void UpdateGroupLastUpdate(HierarchyModel Hierarchy , ObservableCollection<TreeViewNodeViewModelBase> Tree)
        {
            foreach (var node in Tree)
            {
                if (node.NodeType == NodeTypes.P)
                {
                    if (node.Hierarchy.GroupId == Hierarchy.GroupId)
                        node.Hierarchy.GroupLastUpdateTime = Hierarchy.GroupLastUpdateTime;
                }
                if (node.Children.Count > 0)
                    UpdateGroupLastUpdate(Hierarchy, node.Children);

            }
        }

        #endregion UpdateGroupLastUpdate

        #region Populate contents for new project (Template)

        public void PopulateContents(HierarchyModel Hierarchy)
        {
            //Collection to add all contents for new project.
            Dictionary<int, int> Contents;
            string BranchIds = "";
            HierarchyBLL.HierarchyBLLReturnCode getIdsStatus = HierarchyBLL.HierarchyBLLReturnCode.CommonException;
            try
            {
                Dictionary<int, CMFolderModel> outFolders = ProjectsExplorerViewModel.folders;
                Dictionary<int, CMContentModel> outContents = ProjectsExplorerViewModel.contents;
                Dictionary<int, CMVersionModel> outVersions = ProjectsExplorerViewModel.versions;

                //Final Contents add to active contents.
                ObservableCollection<ContentModel> ActiveContents = new ObservableCollection<ContentModel>();
                Dictionary<int, InheritedContentModel> projectContentsInherited = new Dictionary<int, InheritedContentModel>();
                Domain.ErrorHandling Status = HierarchyBLL.PopulateProjectContents(Hierarchy, ActiveContents,
                                                                        string.Empty, outContents, outVersions,
                                                                   out projectContentsInherited);

                var sortedDictVar = from entry in projectContentsInherited orderby entry.Value.cvPriority ascending select entry;

                Dictionary<int, InheritedContentModel> sortedDict = sortedDictVar.ToDictionary(x => x.Key, x => x.Value);

                foreach (var i in sortedDict)
                {
                    ContentModel TemplateContent = new ContentModel(outContents[outVersions[i.Key].ParentID].Name,
                                                                                outVersions[i.Key].Name, i.Key,
                                                                                DateTime.Now.ToString(), "",
                                                                                outContents[outVersions[i.Key].ParentID].ContentType.Name);
                    TemplateContent.status = outVersions[i.Key].Status.Name;
                    TemplateContent.seq = sortedDict[i.Key].cvPriority;
                    ActiveContents.Add(TemplateContent);
                }

                //RaisePropertyChanged("activeContents");
                Hierarchy.VM.Contents = ActiveContents;
                Hierarchy.VM.IsDirty = true;
            }
            catch (Exception e)
            {
                ApiBLL.TraceExceptionParameterValue.Add(e.Message);
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                //return "105";
            }

        }

        #endregion Populate contets for new project (Template)


        public ObservableCollection<HierarchyModel> GetALLProjectsFamily(TreeViewNodeViewModelBase Node)
        {
            ObservableCollection<HierarchyModel> ProjectChildren = new ObservableCollection<HierarchyModel>();

            foreach (var i in Node.Children)
            {
                //Verify that it is a template and not folder.
                if (i.NodeType == NodeTypes.T && !i.Hierarchy.ProjectStatus.Equals("Disabled"))
                {
                    ProjectChildren.Add(i.Hierarchy);
                }
            }
            return ProjectChildren;
        }

        #region  import to environment

        private string _ShowPopupContent = "Hidden";
        public string ShowPopupContent
        {
            get
            {
                return _ShowPopupContent;
            }
            set
            {
                _ShowPopupContent = value;
                RaisePropertyChanged("ShowPopupContent");
            }
        }

        private ViewModelBase _OverlayContentViewModel;
        public ViewModelBase OverlayContentViewModel
        {
            get
            {
                return _OverlayContentViewModel;
            }
            set
            {
                _OverlayContentViewModel = value;
                RaisePropertyChanged("OverlayContentViewModel");
            }
        }

        private void OnImportProjectToEnvReceived(object[] inputParams)
        {
            ShowSelectEnvironmentDialog(inputParams);
        }

        private void ShowSelectEnvironmentDialog(object[] inputParams)
        {
            ShowPopupContent = "Visible";

            TreeViewProjectNodeViewModel projectNode = (TreeViewProjectNodeViewModel)inputParams[0];
            ObservableCollection<UserEnvironmentsModel> userEnvironments = (ObservableCollection<UserEnvironmentsModel>)inputParams[1];

            OverlayContentViewModel = new SelectEnvironmentViewModel(userEnvironments, projectNode, WSId);
        }

        private void ShowProgressBar()
        {
            ShowPopupContent = "Visible";

            OverlayContentViewModel = new ProgressBarViewModel(WSId);
        }

        private void OnRequestCloseSelectEnvironmentDialogReceived(string Param)
        {
            OverlayContentViewModel = null;
            ShowPopupContent = "Hidden";
        }

        private void CloseProgressBar()
        {
            OverlayContentViewModel = null;
            ShowPopupContent = "Hidden";
        }
        string exportPackageName = string.Empty;

        private void OnReceiveEnvDetailsAndImportReceived(object[] inputParms)
        {
            OnRequestCloseSelectEnvironmentDialogReceived(null);

            TreeViewProjectNodeViewModel importedProjectNode = (TreeViewProjectNodeViewModel)inputParms[1];
            UserEnvironmentsModel envDetails = (UserEnvironmentsModel)inputParms[0];

            //ContentBLL contentBLL = new ContentBLL(importedProjectNode.Hierarchy.Id);
            //ObservableCollection<ContentModel> activeVersionCMs = contentBLL.getActiveContents(importedProjectNode.Hierarchy.ActiveVersion);

            targetConnString = envDetails.connectionString;
            targetEnvName = envDetails.environmentName;
            importedProject = importedProjectNode.Hierarchy;

            try
            {
                string projectPath = string.Empty;
                HierarchyBLL.HierarchyBLLReturnCode rc = HierarchyBLL.GetProjectFullPathByProjectId(importedProject.Id, out projectPath);
                importedProject.TreeHeader = projectPath;
            }
            catch (Exception ex)
            {
                String logMessage = "Failed to retrieve project full path." +
                    "\n" + ex.Message +
                    "\n" + ex.StackTrace;
                Domain.SaveGeneralWarningLog(logMessage);
 
            } //ignore failure, issue warning

            UserEnvironmentsModel targetEnvironment = (UserEnvironmentsModel)inputParms[0];
            exportPackageName = importedProject.Name + "_" + importedProject.Id;
            Thread ExportProjectThread = new Thread(new ThreadStart(ProceedImportToEnv));
            ExportProjectThread.Start();

        }

        private HierarchyModel importedProject = null;
        private string targetConnString = string.Empty;
        private string targetEnvName = string.Empty;

        private void ProceedImportToEnv()
        {
            Domain.ErrorHandling result = new Domain.ErrorHandling();

            try
            {
                //Timestamp to prevent ovewriting 
                DateTime NowTime = System.DateTime.Now;
                string StartTime = string.Format("{0:yyyyMMddhhmmss}", NowTime);
                exportPackageName = exportPackageName + "_" + StartTime;

                string importTempFolderPath = Domain.PE_SystemParameters["ExportToEnvFolder"];

                result = ExportProjectToEnvBLL.ValidateBeforeExport(importedProject, targetConnString, targetEnvName);
                if (result.messsageId != string.Empty)
                {
                    if (result.messsageId == "240")
                    {
                        string msgBoxDescriptionWithParams = Domain.GetMessageDescriptionById(result.messsageId);
                        string msgBoxDescription = SetDescriptionParameters(msgBoxDescriptionWithParams, result.messageParams, Convert.ToInt32(result.messsageId));
                        IMessageBoxService MsgBoxService = new MessageBoxService();

                        if (MsgBoxService.ShowOkCancel(msgBoxDescription, DialogIcons.Warning) == DialogResults.Cancel)
                        {
                            return;
                        }
                    }
                    else
                    {
                        ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(result.messsageId), result.messageParams);
                        return;
                    }
                }

                ShowProgressBar();
                MessageMediator.NotifyColleagues("LockRibbonBar");

                ThreadStart threadStart = new ThreadStart(IncrementProgressCounter);
                Thread t = new Thread(threadStart);
                t.Start();



                result = ExportProjectToEnvBLL.ExportImportProjectToEnv(importedProject, importTempFolderPath, exportPackageName,
                                                                                        targetConnString, targetEnvName, threadStart);

                MessageMediator.NotifyColleagues("UnLockRibbonBar");
                CloseProgressBar();

                FileSystemBLL.importFilesCompleted = 0;
                FileSystemBLL.exportFilesCompleted = 0;
                FileSystemBLL.totalExportImportFiles = 0;
                ExportProjectToEnvBLL.progressBarTitle = String.Empty;
                
                if (result.messsageId != string.Empty)
                {
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(result.messsageId), result.messageParams);
                }
                else
                {
                    result.messsageId = "238";
                    result.messageParams[0] = importedProject.Name;
                    result.messageParams[1] = importedProject.ActiveVersion;
                    result.messageParams[2] = Domain.Environment;
                    result.messageParams[3] = targetEnvName;
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(result.messsageId), result.messageParams);
                    MessageMediator.NotifyColleagues(WSId + "OnRefreshNotesReceived", importedProject.Id);
                }
            }
            catch (Exception e)
            {
                Object[] ArgsList = new Object[] { 0 };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
            }
        }

        public void IncrementProgressCounter()
        {
            MessageMediator.NotifyColleagues(WSId + "OnExportImportReceived", null);
        }

        #endregion
    }

} //end of root namespace