using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ATSBusinessLogic;
using ATSBusinessLogic.ContentMgmtBLL;
using ATSBusinessObjects;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using Infra.DragDrop;
using Infra.MVVM;
using TraceExceptionWrapper;


namespace ContentMgmtModule
{
    #region  Node Operations Enum

    public enum ItemNodeActionType
    {
        Copy,
        Move,
        Delete
    }

    #endregion

    public class CMContentManagementViewModel : WorkspaceViewModelBase, IDropTarget
    {
        #region  Data

        private IMessageBoxService MsgBoxService = null;

        private List<CMTreeNode> contentTree { get; set; }

        private List<String> permissions { get; set; }

        public static Dictionary<int, CMFolderModel> folders = new Dictionary<int, CMFolderModel>();

        public static Dictionary<int, CMContentModel> contents = new Dictionary<int, CMContentModel>();

        public static Dictionary<int, CMVersionModel> versions = new Dictionary<int, CMVersionModel>();

        public CMContentsReaderBLL contentsReader { set; get; }

        protected MessengerService MessageMediator = new MessengerService();

        private CMTreeViewNodeViewModelBase WorkNode;

        public Collection<object> SearchParameters { get; set; }

        public bool updateDragDropMode { get; set; }

        public static bool WritePermission { set; get; }

        public static CMImpersonationBLL cmImp = new CMImpersonationBLL();
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

        private bool _applicationAddRootFolderPermission;
        public bool ApplicationAddRootFolderPermission
        {
            get
            {
                return _applicationAddRootFolderPermission;
            }
            set
            {
                _applicationAddRootFolderPermission = value;
                RaisePropertyChanged("ApplicationAddRootFolderPermission");
            }
        }

        public bool ApplicationUpdateUserGroupPermission;

        private bool _applicationWritePermission;
        public bool ApplicationWritePermission
        {
            get
            {
                return _applicationWritePermission;
            }
            set
            {
                _applicationWritePermission = value;
                RaisePropertyChanged("ApplicationWritePermission");
            }
        }



        #endregion
    
        #region  Ctor

        public CMContentManagementViewModel()
           : base("CmView", "")
        {
            //Initialize this VM
            DisplayName = "Cm";
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();
            //Register as Subscriber to messages from other VMs
            updateDragDropMode = false;
            
            MessageMediator.Register(this.WSId + "ShowAndUpdateFolderDetails", new Action<CMTreeViewNodeViewModelBase>(OnShowAndUpdateCMFolderDetailsReceived)); //Register to recieve a message of a Folder to be displayed and to udated
            MessageMediator.Register(this.WSId + "ShowAndUpdateContentDetails", new Action<CMTreeViewNodeViewModelBase>(OnShowAndUpdateCMContentDetailsReceived)); //Register to recieve a message of a Folder to be displayed and to udated
            MessageMediator.Register(this.WSId + "ShowAndUpdateVersionDetails", new Action<CMTreeViewNodeViewModelBase>(OnShowAndUpdateCMVersionDetailsReceived)); //Register to recieve a message of a Folder to be displayed and to udated
            MessageMediator.Register(this.WSId + "ShowWhereUsedDetails", new Action<CMTreeViewNodeViewModelBase>(OnShowCMWhereUsedDetailsReceived)); //Register to recieve a message of a where used to be displayed           
            MessageMediator.Register(this.WSId + "CloseDetailsView", new Action<object>(OnCloseCMDetailsViewReceived)); //Register to recieve a message to display Cm right-hand side
            MessageMediator.Register(this.WSId + "AddChildNode", new Action<Collection<object>>(OnAddChildNodeReceived)); //Register to recieve a message containing Add Child Node Request Parameters
            MessageMediator.Register(this.WSId + "DeleteNode", new Action<CMTreeViewNodeViewModelBase>(OnDeleteNodeReceived)); //Register to recieve a message containing NodeId of Node to Delete
            MessageMediator.Register(this.WSId + "UpdateNode", new Action<Collection<object>>(OnUpdateNodeReceived)); //Register to recieve a message containing Node to update
            MessageMediator.Register(this.WSId + "SearchParameters", new Action<Collection<object>>(OnSearchParametersReceived)); //Register to recieve a message containing search Parameters
            MessageMediator.Register(this.WSId + "RefreshTree", new Action<object>(OnRefreshTreeReceived)); //Register to recieve a message asking for node refresh

            MessageMediator.Register(this.WSId + "UpdateVersionsPermissions", new Action<Collection<object>>(OnUpdateVersionsPermissionsReceived)); //Register to recieve a message containing Node to update

            //Initialize
            contentsReader = new CMContentsReaderBLL();
            SearchParameters = new Collection<object>();
            setPermissions();
            try
            {
                contentTree = CMTreeNodeBLL.GetTreeObjectsData(true, out folders, out contents, out versions);
            }
            catch (TraceException te)
            {
                String logMessage = te.Message + "\n" + "Source: " + te.Source + "\n" + te.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Internal error ocurred. Please see Data Access log file for more details: Shell->View Log.");
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Internal error ocurred. Please see Data Access log file for more details: Shell->View Log.");
            }

            DateTime t = DateTime.Now;
            int MS = t.Millisecond;
            string sMS = MS.ToString();
            Console.WriteLine(t + "." + sMS + " start PopulateTreeView(0);");

            PopulateTreeView(0);

            t = DateTime.Now;
            MS = t.Millisecond;
            sMS = MS.ToString();
            Console.WriteLine(t + "." + sMS + " end PopulateTreeView(0)");
        }

        public CMContentManagementViewModel(object cmObject)
            : base("CmView", "")
        {
            int cmObjectId = -1;
            switch (cmObject.GetType().ToString())
            {
   		        case "ContentModel":
                    ContentModel cv = (ContentModel)cmObject; //the content version we want to focus on
                    cmObjectId = cv.id;
                    break;
                case "ATSBusinessObjects.ContentModel":
                    cv = (ContentModel)cmObject; //the content version we want to focus on
                    cmObjectId = cv.id;
                    break;
                case "ATSBusinessObjects.ContentMgmtModels.CMWhereUsedContentLinkItemModel":
                    CMWhereUsedContentLinkItemModel c = (CMWhereUsedContentLinkItemModel)cmObject; //the content we want to focus on
                    //cmObjectId = CMContentBLL.GetContentId(c.ContentName);
                    cmObjectId = CMVersionBLL.GetVersionIdByNames(c.ContentName, c.VersionName);
                    break;
                case "ContentMgmtModule.CMItemVersionLink":
                    CMItemVersionLink v = (CMItemVersionLink)cmObject; //the content we want to focus on
                    cmObjectId = v.ContentVersionID;
                    break;

            }
          
            //Initialize this VM
            DisplayName = "Cm";
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();
            //Register as Subscriber to messages from other VMs
            updateDragDropMode = false;

            MessageMediator.Register(this.WSId + "ShowAndUpdateFolderDetails", new Action<CMTreeViewNodeViewModelBase>(OnShowAndUpdateCMFolderDetailsReceived)); //Register to recieve a message of a Folder to be displayed and to udated
            MessageMediator.Register(this.WSId + "ShowAndUpdateContentDetails", new Action<CMTreeViewNodeViewModelBase>(OnShowAndUpdateCMContentDetailsReceived)); //Register to recieve a message of a Folder to be displayed and to udated
            MessageMediator.Register(this.WSId + "ShowAndUpdateVersionDetails", new Action<CMTreeViewNodeViewModelBase>(OnShowAndUpdateCMVersionDetailsReceived)); //Register to recieve a message of a Folder to be displayed and to udated
            MessageMediator.Register(this.WSId + "ShowWhereUsedDetails", new Action<CMTreeViewNodeViewModelBase>(OnShowCMWhereUsedDetailsReceived)); //Register to recieve a message of a where used to be displayed
            MessageMediator.Register(this.WSId + "CloseDetailsView", new Action<object>(OnCloseCMDetailsViewReceived)); //Register to recieve a message to display Cm right-hand side
            MessageMediator.Register(this.WSId + "AddChildNode", new Action<Collection<object>>(OnAddChildNodeReceived)); //Register to recieve a message containing Add Child Node Request Parameters
            MessageMediator.Register(this.WSId + "DeleteNode", new Action<CMTreeViewNodeViewModelBase>(OnDeleteNodeReceived)); //Register to recieve a message containing NodeId of Node to Delete
            MessageMediator.Register(this.WSId + "UpdateNode", new Action<Collection<object>>(OnUpdateNodeReceived)); //Register to recieve a message containing Node to update
            MessageMediator.Register(this.WSId + "RefreshTree", new Action<object>(OnRefreshTreeReceived)); //Register to recieve a message asking for node refresh

            MessageMediator.Register(this.WSId + "UpdateVersionsPermissions", new Action<Collection<object>>(OnUpdateVersionsPermissionsReceived)); //Register to recieve a message containing Node to update

            //Initialize
            contentsReader = new CMContentsReaderBLL();
            SearchParameters = new Collection<object>();
            setPermissions();
            try
            {
                contentTree = CMTreeNodeBLL.GetTreeObjectsData(true, out folders, out contents, out versions);
            }
            catch (TraceException te)
            {
                String logMessage = te.Message + "\n" + "Source: " + te.Source + "\n" + te.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Error occured. Please check log file");
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("Error occured. Please check log file");
            }
            PopulateTreeView(0);

            //expand tree on this content version
            ExecuteExpandOnTreeViewNodeCommand(cmObjectId);
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
                TN.Name = "Content Management";
                TN.TreeNodeType = ATSBusinessObjects.ContentMgmtModels.TreeNodeObjectType.Root;
               
                TreeNodes.Add(new CMTreeViewRootNodeViewModel(this.WSId, TN));
                RootNode.IsAddFolderToRoot = ApplicationAddRootFolderPermission;   //set root add folder security for context menu             
                RootNode.UserGroupUpdatePermission = ApplicationUpdateUserGroupPermission;
                RootNode.IsUpdate = WritePermission;
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
                            TreeNode.UserGroupUpdatePermission = ApplicationUpdateUserGroupPermission;
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

                    UpdateItemNodePermission(TreeNode, RootNode, ApplicationWritePermission);

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
                        TreeNode.UserGroupUpdatePermission = ApplicationUpdateUserGroupPermission;
                        break;
                    case "Content":
                        TreeNode = new CMTreeViewContentNodeViewModel(this.WSId, tn, Node);
                        break;
                    case "ContentVersion":
                        TreeNode = new CMTreeViewVersionNodeViewModel(this.WSId, tn, Node);
                        break;
                }

                UpdateItemNodePermission(TreeNode, Node, ApplicationWritePermission);

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
            foreach (CMTreeViewNodeViewModelBase TVN in TreeNodes)
            {
                if (TVN.Children.Count > 0)
                {
                    ExecuteExpandAllChildrenAndFindNodeCommand(TVN,Param);
                }
            }
        }

        private void ExecuteExpandAllChildrenAndFindNodeCommand(CMTreeViewNodeViewModelBase TVN, object Param)
        {
            foreach (CMTreeViewNodeViewModelBase TV in TVN.Children)
            {
                //TV.IsExpanded = true;
                //if (TV.ID == (int)Param) fixed bug 3691. Otherwise might jump to content with the same id.
                if (TV.ID == (int)Param && TV.TreeNode.TreeNodeType == TreeNodeObjectType.ContentVersion)
                {
                    TV.IsExpanded = true;
                    TV.IsSelected = true;
                    //switch (TV.TreeNode.TreeNodeType)
                    //{
                    //    case TreeNodeObjectType.Content:
                    //        OnShowAndUpdateCMContentDetailsReceived(TV);
                    //    break;

                    //    case TreeNodeObjectType.ContentVersion:
                    //        OnShowAndUpdateCMVersionDetailsReceived(TV);
                    //    break;
                    //}
                    OnShowAndUpdateCMVersionDetailsReceived(TV);
                    return;
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

        #region  Find

        private IEnumerator<CMTreeViewNodeViewModelBase> _MatchingNodesEnumerator;
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
            if (SearchParameters.Count < 1) //In case the user did not go thru the Search control, rather just filled the search string and hit Find
            {
                SearchParameters.Clear();
                SearchParameters.Add(SearchText);
                SearchParameters.Add(true); //Folder Name
                SearchParameters.Add(true); //Content Name
                SearchParameters.Add(true); //Version Name
                SearchParameters.Add(true); //File Name
                SearchParameters.Add(false); //User
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
                this.VerifyMatchingNodesEnumerator();
            }
            CMTreeViewNodeViewModelBase Node = _MatchingNodesEnumerator.Current;
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
           // Node.bring();  //scroll if necessary
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


        private IEnumerable<CMTreeViewNodeViewModelBase> FindMatches(CMTreeViewNodeViewModelBase Node)
        {
            List<CMTreeViewNodeViewModelBase> L = new List<CMTreeViewNodeViewModelBase>();

            switch (Node.TreeNode.TreeNodeType.ToString())
            {
                case "Folder":
                    if ((Boolean)SearchParameters[1] == true && ((CMFolderModel)(Node.TreeNode)).Name.IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) >= 0) //search by folder name
                    {                               
                        L.Add(Node);
                    }
                    else if ((Boolean)SearchParameters[5] == true && ((CMFolderModel)(Node.TreeNode)).LastUpdateUser.IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) >= 0) //search by user name                    {      
                    {
                        L.Add(Node);
                    }
                    break;

                case "Content":
                    if ((Boolean)SearchParameters[2] == true && ((CMContentModel)(Node.TreeNode)).Name.IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) >= 0) //search by content name
                    {                        
                        L.Add(Node);
                    }
                    else if ((Boolean)SearchParameters[5] == true && ((CMContentModel)(Node.TreeNode)).LastUpdateUser.IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) >= 0) //search by user name
                    {                         
                        L.Add(Node);
                    }
                    break;

                case "ContentVersion":
                    if ((Boolean)SearchParameters[3] == true && ((CMVersionModel)(Node.TreeNode)).Name.IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) >= 0) //search by version name
                    {                     
                       L.Add(Node);
                    }
                    else if ((Boolean)SearchParameters[5] == true && ((CMVersionModel)(Node.TreeNode)).LastUpdateUser.IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) >= 0) //search by user name
                    {
                        L.Add(Node);
                    }
                    else if ((Boolean)SearchParameters[4] == true && ((CMVersionModel)(Node.TreeNode)).Files.Count > 0) //search by file name
                    {
                        foreach (CMContentFileModel file in ((CMVersionModel)(Node.TreeNode)).Files.Values)
                        {
                            if (file.FileName.IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) >= 0)
                            {
                                L.Add(Node);
                                break; //if we found 1 version with this file pattern - break
                            }
                        }
                    }
                    break;
            }

            foreach (CMTreeViewNodeViewModelBase Child in Node.Children)
            {
                foreach (CMTreeViewNodeViewModelBase Match in this.FindMatches(Child))
                {
                    L.Add(Match);
                }
            }
            //
            return L;
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
            MessageMediator.NotifyColleagues("ShowCMSearchParams", this.WSId); //Send message to the MainViewModel
        }

        #endregion

        #region  Receive Messages from TreeView Nodes

            #region  Add Child Node

            private void OnAddChildNodeReceived(Collection<object> Parameters)
            {
                if (Parameters.Count < 2)
                {
                    return;
                }
                //
                TreeNodeObjectType type = (TreeNodeObjectType)(Parameters[0]);
                long ParentId = ((CMTreeViewNodeViewModelBase)(Parameters[1])).ID;
                //Locate Parent
                //CMTreeViewNodeViewModelBase ParentNode = LocateNode(ParentId);
                CMTreeViewNodeViewModelBase ParentNode = ((CMTreeViewNodeViewModelBase)(Parameters[1]));
                CMTreeViewNodeViewModelBase ChildNode = null;

                //Create Model for the new Node
                switch (type.ToString())
                {
                    case "Folder":
                        CMFolderModel FN = new CMFolderModel();
                        FN.ID = -1;
                        FN.Name = "New Folder";      
                        ChildNode = new CMTreeViewFolderNodeViewModel(this.WSId, FN, ParentNode);
                        ChildNode.UserGroupUpdatePermission = ApplicationUpdateUserGroupPermission;
                        DetailsViewModel = new CMFolderDetailsViewModel(this.WSId, ChildNode, ParentNode);                      
                        break;

                    case "Content":
                        CMContentModel CN = new CMContentModel();
                        CN.ID = -1;
                        CN.Name = "New Content";
                        ChildNode = new CMTreeViewContentNodeViewModel(this.WSId, CN, ParentNode);
                        DetailsViewModel = new CMContentDetailsViewModel(this.WSId, ChildNode, ParentNode);                    
                        break;

                    case "ContentVersion":
                        CMVersionModel VN = new CMVersionModel();
                        VN.ID = -1;
                        //CR3483
                        //VN.Name = "New Version";      

                        //Performance#6
                        CMContentsReaderBLL.listOfUsedContentVersionsCM.Clear();
                        CMContentsReaderBLL.listOfUsedContentVersionsPE.Clear();
                        CMVersionBLL.GetListOfUsedContentVersionsCM(ref CMContentsReaderBLL.listOfUsedContentVersionsCM);
                        VersionBLL.GetListOfUsedContentVersionsPE(ref CMContentsReaderBLL.listOfUsedContentVersionsPE);
                        //end #6

                        ChildNode = new CMTreeViewVersionNodeViewModel(this.WSId, VN, ParentNode);
                        DetailsViewModel = new CMVersionDetailsViewModel(this.WSId, ChildNode, ParentNode, false);

                        break;
                } 
       
            }

            #endregion

            #region Update Node's Details

            private void OnUpdateNodeReceived(Collection<object> Parameters)
            {
                CMTreeViewNodeViewModelBase Node = ((CMTreeViewNodeViewModelBase)(Parameters[0]));

                switch (Node.TreeNode.TreeNodeType.ToString())
                {
                    case "Folder":
                        CMTreeViewNodeViewModelBase FolderToUpdate = LocateFolderNode(Node.ID);
                        FolderToUpdate.Name = Node.Name;
                       // FolderToUpdate.Description = ((CMFolderModel)(Node.TreeNode)).Description;
                        break;

                    case "Content":
                        CMTreeViewNodeViewModelBase ParentContentNode = LocateFolderNode(Node.Parent.ID); //find node parent
                        CMContentModel tempContent = new CMContentModel();        //create new content model to replace the old one
                        tempContent.ID = Node.TreeNode.ID;                        //give tree node id in order to get the right type and icon
                        tempContent.Name = Node.Name;
                        tempContent.IconPath = Node.Icon.ToString();             //new icon path
                        tempContent.LastUpdateTime = ((CMContentModel)(Node.TreeNode)).LastUpdateTime;
                        tempContent.TreeNodeType = ATSBusinessObjects.ContentMgmtModels.TreeNodeObjectType.Content; //tree node type
                       
                        var contentToRemove = ParentContentNode.Children.Where(X => X.ID == Node.ID).FirstOrDefault();
                        int indexToInsertContent = ParentContentNode.Children.IndexOf(contentToRemove);

                        CMTreeViewContentNodeViewModel ContentToUpdate = new CMTreeViewContentNodeViewModel(this.WSId, tempContent, ParentContentNode); //create new tree node to insert the tree
                        foreach (CMTreeViewVersionNodeViewModel tv in Node.Children) //if content have versions add them to the updated node before it removed
                        {
                            ContentToUpdate.Children.Add(tv);
                        }
                        ParentContentNode.Children.Remove(contentToRemove);                        //remove the old node from parent
                        ContentToUpdate.ID = Node.ID;
                        ParentContentNode.Children.Insert(indexToInsertContent, ContentToUpdate); //add the new updated node to parent at the same index
                        ContentToUpdate.IsSelected = true;
                        if(Node.IsExpanded)
                            ContentToUpdate.IsExpanded = true;

                        // Update permission
                        CMContentManagementViewModel.UpdatePermissionUpdateNode(ContentToUpdate);
                        break;

                    case "ContentVersion":
                        CMTreeViewNodeViewModelBase ParentVersionNode = LocateContentNode(Node.Parent.ID); //find node parent
                        CMVersionModel tempVersion = new CMVersionModel();        //create new version model to replace the old one
                        tempVersion.ID = Node.TreeNode.ID;                        //give tree node id in order to get the right type and icon
                        tempVersion.Name = Node.Name;
                       // tempVersion.IconPath = Node.Icon.ToString();             //new icon path
                        tempVersion.TreeNodeType = ATSBusinessObjects.ContentMgmtModels.TreeNodeObjectType.ContentVersion; //tree node type
                        var versionToRemove = ParentVersionNode.Children.Where(X => X.ID == Node.ID).FirstOrDefault();
                        int indexToInsertVersion = ParentVersionNode.Children.IndexOf(versionToRemove);
                        CMTreeViewNodeViewModelBase VersionToUpdate = LocateVersionNode(Node.ID);
                        VersionToUpdate.Name = Node.Name;
                        switch (((CMVersionModel)(Node.TreeNode)).id_ContentVersionStatus)
                        {
                            case "Ac":
                                VersionToUpdate.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png", "ActiveContentVersion"), UriKind.RelativeOrAbsolute));
                                break;

                            case "Edit":
                                VersionToUpdate.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png", "EditContentVersion"), UriKind.RelativeOrAbsolute));
                                break;

                            case "Ret":
                                VersionToUpdate.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png", "RetContentVersion"), UriKind.RelativeOrAbsolute));
                                break;
                        }

                        ParentVersionNode.Children.Remove(versionToRemove);
                        VersionToUpdate.ID = Node.ID;            
                        ParentVersionNode.Children.Insert(indexToInsertVersion, VersionToUpdate); //add the new updated node to parent at the same index

                        if (indexToInsertVersion == 0)  //if there are no children enable delete
                        {
                            VersionToUpdate.IsDelete = true;
                        }

                        break;
                }
            }

            #endregion

            #region Update Versions Permissions Received

            private void OnUpdateVersionsPermissionsReceived(Collection<object> Parameters)
            {
                List<CMTreeNode> treeNodesList = (List<CMTreeNode>)Parameters[0];
                List<CMTreeViewNodeViewModelBase> treeNodesListToUpdate = new List<CMTreeViewNodeViewModelBase>();
              
                //Performance#6
                CMContentsReaderBLL.listOfUsedContentVersionsCM.Clear();
                CMContentsReaderBLL.listOfUsedContentVersionsPE.Clear();
                CMVersionBLL.GetListOfUsedContentVersionsCM(ref CMContentsReaderBLL.listOfUsedContentVersionsCM);
                VersionBLL.GetListOfUsedContentVersionsPE(ref CMContentsReaderBLL.listOfUsedContentVersionsPE);
                //end #6
                (new CMUpdatePermissionBLL(folders, contents, versions)).UpdateTreeNodeRecursive(contentTree);

                foreach (CMTreeNode v in treeNodesList)
                {
                    foreach (CMVersionModel tn in CMContentManagementViewModel.versions.Values)
                    {
                        if (tn.ID.Equals(v.ID))
                        {
                            CMTreeViewNodeViewModelBase vm = LocateVersionNode(tn.ID);
                            UpdateItemNodePermission(vm, vm, WritePermission);
                        }
                    }
                }

            }

            #endregion   

            #region Refresh Tree

            private void OnRefreshTreeReceived(object NotUsed)
            {
                try
                {
                    TreeNodes.Clear();//Clear the entire collection
                    contentTree = CMTreeNodeBLL.GetTreeObjectsData(true, out folders, out contents, out versions); //refresh tree
                    PopulateTreeView(0);
                }
                catch (TraceException te)
                {
                    String logMessage = te.Message + "\n" + "Source: " + te.Source + "\n" + te.StackTrace;
                    Domain.SaveGeneralErrorLog(logMessage);
                    throw new Exception("Internal error ocurred. Please see Data Access log file for more details: Shell->View Log.");
                }
                catch (Exception e)
                {
                    String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                    Domain.SaveGeneralErrorLog(logMessage);
                    throw new Exception("Internal error ocurred. Please see Data Access log file for more details: Shell->View Log.");
                }
            }


            #endregion

        #endregion

        #region  IDropTarget Members & Other Drop Activities

            #region  Drag Over

            public void DragOver(Infra.DragDrop.IDropInfo DropInfo)
            {
                CMTreeViewNodeViewModelBase SourceItem = DropInfo.Data as CMTreeViewNodeViewModelBase;
                CMTreeViewNodeViewModelBase TargetItem = DropInfo.TargetItem as CMTreeViewNodeViewModelBase;

                ItemNodeActionType actionType = ((DropInfo.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey) ? ItemNodeActionType.Copy : ItemNodeActionType.Move;

                if (CheckIfAllowDrop(SourceItem, TargetItem, actionType))
                {
                    DropInfo.Effects = actionType == ItemNodeActionType.Copy ? DragDropEffects.Copy : DragDropEffects.Move;
                    DropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    DropInfo.Effects = DragDropEffects.Move;
                    return;
                }

                DropInfo.Effects = DragDropEffects.None;
            }

            #endregion

            #region  Drop

            public void Drop(Infra.DragDrop.IDropInfo DropInfo)
            {
                try
                {
                    //Work variables
                    Collection<string> StatusBarParameters = new Collection<string>();
                    List<String> parameters = new List<string>();
                    var SB = new StringBuilder(string.Empty);
                    String errorId;
                    string displayMessage;

                    CMFolderModel FM = null;
                    CMContentModel CM = null;
                    CMVersionModel VM = null;

                    CMTreeViewNodeViewModelBase sourceNode = DropInfo.Data as CMTreeViewNodeViewModelBase;
                    CMTreeViewNodeViewModelBase operNode = sourceNode;  //the oper node will be moved (copied or moved)
                    CMTreeViewNodeViewModelBase destinationNode = DropInfo.TargetItem as CMTreeViewNodeViewModelBase;

                    ItemNodeActionType actionType = ((DropInfo.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey) ? ItemNodeActionType.Copy : ItemNodeActionType.Move;

                    if (CheckIfAllowDrop(sourceNode, destinationNode, actionType) && UserConfirmation(sourceNode, destinationNode, actionType))
                    {
                        switch (sourceNode.TreeNode.TreeNodeType)
                        {                            
                            case TreeNodeObjectType.Folder:
                                FM = CMFolderBLL.GetFolderRow(sourceNode.ID);  //get moved folder data   
                                FM.UserGroupTypePermission = contentsReader.GetFolderUserGroupType((int)sourceNode.ID);
                                //if folder has been changed by another user  
                                try
                                {
                                    CMFolderBLL.CompareUpdateTime(((CMFolderModel)(sourceNode.TreeNode)).LastUpdateTime, sourceNode.ID);
                                }
                                catch (TraceException te)
                                {
                                    String logMessage = te.Message + "\n" + "Source: " + te.Source + "\n" + te.StackTrace;
                                    Domain.SaveGeneralErrorLog(logMessage);
                                    SB.Clear();
                                    SB.Append("SELECT ED_Description FROM ErrorDescription where ED_ID='" + te.ApplicationErrorID + "';");
                                    MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                                    displayMessage = (Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString();
                                    parameters.Clear();
                                    parameters.Add(sourceNode.Name);
                                    parameters.Add(FM.LastUpdateUser);
                                    displayMessage = CMContentManagementViewModel.UpdateMessageStringParameters(displayMessage, parameters);
                                    object[] ArgMessageParam = { parameters[0], parameters[1] };
                                    CMContentManagementViewModel.ShowErrorAndInfoMessage(te.ApplicationErrorID, ArgMessageParam);
                                    MessageMediator.NotifyColleagues(this.WSId + "RefreshTree", this); //Will be returned to the MainWindow signed for this message
                                    return;
                                }       

                                if (FM.Id < 0)  //if folder has been deleted by another user
                                {
                                    errorId = "Folder deleted";
                                    SB.Clear();
                                    SB.Append("SELECT ED_Description FROM ErrorDescription where ED_ID='" + errorId + "';");
                                    MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                                    displayMessage = (Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString();
                                    parameters.Clear();
                                    parameters.Add(sourceNode.Name);
                                    displayMessage = CMContentManagementViewModel.UpdateMessageStringParameters(displayMessage, parameters);
                                    object[] ArgMessageParam = { parameters[0] };
                                    CMContentManagementViewModel.ShowErrorAndInfoMessage(errorId, ArgMessageParam);
                                    MessageMediator.NotifyColleagues(this.WSId + "RefreshTree", operNode); //Will be returned to the MainWindow signed for this message
                                    return;
                                }
                                break;

                            case TreeNodeObjectType.Content:
                                CM = CMContentBLL.GetContentRow(sourceNode.ID);   //get moved content data  

                                //if content has been changed by another user    
                                try
                                {
                                    CMContentBLL.CompareUpdateTime(((CMContentModel)(sourceNode.TreeNode)).LastUpdateTime, sourceNode.ID);
                                }
                                catch (TraceException te)
                                {
                                    String logMessage = te.Message + "\n" + "Source: " + te.Source + "\n" + te.StackTrace;
                                    Domain.SaveGeneralErrorLog(logMessage);
                                    SB.Clear();
                                    SB.Append("SELECT ED_Description FROM ErrorDescription where ED_ID='" + te.ApplicationErrorID + "';");
                                    MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                                    displayMessage = (Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString();
                                    parameters.Clear();
                                    parameters.Add(sourceNode.Name);
                                    parameters.Add(CM.LastUpdateUser);
                                    displayMessage = CMContentManagementViewModel.UpdateMessageStringParameters(displayMessage, parameters);
                                    object[] ArgMessageParam = { parameters[0], parameters[1] };
                                    CMContentManagementViewModel.ShowErrorAndInfoMessage(te.ApplicationErrorID, ArgMessageParam);
                                    MessageMediator.NotifyColleagues(this.WSId + "RefreshTree", operNode); //Will be returned to the MainWindow signed for this message
                                    return;
                                }

                                if (CM.Id < 0)  //if content has been deleted by another user
                                {
                                    errorId = "Content deleted";
                                    SB.Clear();
                                    SB.Append("SELECT ED_Description FROM ErrorDescription where ED_ID='" + errorId + "';");
                                    MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                                    displayMessage = (Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString();
                                    parameters.Clear();
                                    parameters.Add(sourceNode.Name);
                                    displayMessage = CMContentManagementViewModel.UpdateMessageStringParameters(displayMessage, parameters);
                                    object[] ArgMessageParam = { parameters[0] };
                                    CMContentManagementViewModel.ShowErrorAndInfoMessage(errorId, ArgMessageParam);
                                    MessageMediator.NotifyColleagues(this.WSId + "RefreshTree", operNode); //Will be returned to the MainWindow signed for this message
                                    return;
                                }
                                break;

                            case TreeNodeObjectType.ContentVersion:                              
                                VM = CMVersionBLL.GetVersionRow(sourceNode.ID);  //get moved version data 

                                //if version has been changed by another user            
                                if(!CMVersionBLL.CompareUpdateTime( ((CMVersionModel)(sourceNode.TreeNode)).LastUpdateTime , sourceNode.ID))
                                {
                                    errorId = "Version changed";
                                    SB.Clear();
                                    SB.Append("SELECT ED_Description FROM ErrorDescription where ED_ID='" + errorId + "';");
                                    MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                                    displayMessage = (Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString();
                                    parameters.Clear();
                                    parameters.Add(sourceNode.Name);
                                    parameters.Add(VM.LastUpdateUser);
                                    displayMessage = CMContentManagementViewModel.UpdateMessageStringParameters(displayMessage, parameters);
                                    object[] ArgMessageParam = { parameters[0],parameters[1] };
                                    CMContentManagementViewModel.ShowErrorAndInfoMessage(errorId, ArgMessageParam);
                                    MessageMediator.NotifyColleagues(this.WSId + "RefreshTree", operNode); //Will be returned to the MainWindow signed for this message
                                    return;
                                }

                                //if version has been deleted by another user
                                if (VM.Id < 0)  
                                {
                                    errorId = "Version deleted";
                                    SB.Clear();
                                    SB.Append("SELECT ED_Description FROM ErrorDescription where ED_ID='" + errorId + "';");
                                    MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                                    displayMessage = (Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString();
                                    parameters.Clear();
                                    parameters.Add(sourceNode.Name);
                                    displayMessage = CMContentManagementViewModel.UpdateMessageStringParameters(displayMessage, parameters);
                                    object[] ArgMessageParam = { parameters[0] };
                                    CMContentManagementViewModel.ShowErrorAndInfoMessage(errorId, ArgMessageParam);
                                    MessageMediator.NotifyColleagues(this.WSId + "RefreshTree", operNode); //Will be returned to the MainWindow signed for this message
                                    return;
                                }
                                VM.ID = (int)VM.Id;      
                                contentsReader.UpdateContentVersionSubVersions(VM, versions, contents); //update sub versions
                                break;
                        }

                        CMTreeViewNodeViewModelBase ParentNode = null;

                        if (operNode.Parent.TreeNode.TreeNodeType == TreeNodeObjectType.Root || operNode.Parent.TreeNode.TreeNodeType == TreeNodeObjectType.Folder)
                        {
                            ParentNode = LocateFolderNode(operNode.Parent.ID);
                        }
                        else if (operNode.Parent.TreeNode.TreeNodeType == TreeNodeObjectType.Content)
                        {
                            ParentNode = LocateContentNode(operNode.Parent.ID);
                        }
         
                        if (actionType.ToString() == "Copy")  //if the action type is copy we want that the source node will be a cloned and not the original source
                        {
                            int indexToInsert;

                            switch (operNode.TreeNode.TreeNodeType.ToString())
                            {
                                case "Folder":
                                    CMFolderModel newFolder = CMFolderBLL.CloneFolder(FM); //clone folder in order to move it
                                    FM.Id = newFolder.Id;              //give new id for the cloned folder
                                    operNode = new CMTreeViewFolderNodeViewModel(this.WSId, newFolder, ParentNode);   //create new folder tree node
                                    FM.ParentId = destinationNode.ID;
                                    operNode.Name = CMFolderBLL.GenerateFolderName(ref FM); //update the folder name for tree UI
                                    break;

                                case "Content":
                                    CMContentModel newContent = CMContentBLL.CloneContent(CM); //clone content in order to move it
                                    CM.Id = newContent.Id;              //give new id for the cloned content
                                    operNode = new CMTreeViewContentNodeViewModel(this.WSId, newContent, ParentNode); //create new content tree node
                                    operNode.Name = CMContentBLL.GenerateContentName(ref CM); //update the content name for tree UI
                                    break;

                                case "ContentVersion":  
                               
                                    try
                                    {
                                        VM.ParentID = (int)destinationNode.ID;
                                        VM.ID = (int)VM.Id;
                                        VM.id_ContentVersionStatus = "Edit"; //default
                                        (new CMContentVersionLinkConfirmer()).ConfirmerContentVersion((VM));       //check if version is linked
                                        string defaultName = CMVersionBLL.GenerateDefaultVersionName(destinationNode.ID);
                                        CMVersionModel newVesrion = CMVersionBLL.CloneVersion(VM); //clone version in order to move it
                                        VM.Id = newVesrion.Id;              //give new id for the cloned version                                                          
                                        //Performance#6
                                        CMContentsReaderBLL.listOfUsedContentVersionsCM.Clear();
                                        CMContentsReaderBLL.listOfUsedContentVersionsPE.Clear();
                                        CMVersionBLL.GetListOfUsedContentVersionsCM(ref CMContentsReaderBLL.listOfUsedContentVersionsCM);
                                        VersionBLL.GetListOfUsedContentVersionsPE(ref CMContentsReaderBLL.listOfUsedContentVersionsPE);
                                        //end #6
                                        operNode = new CMTreeViewVersionNodeViewModel(this.WSId, newVesrion, ParentNode); //create new version tree node
                                        
                                        //CR 3483
                                        //operNode.Name = CMVersionBLL.GenerateVersionName(ref VM); //update the version name for tree UI

                                        if (defaultName == string.Empty)
                                        {
                                            operNode.Name = CMVersionBLL.GenerateVersionName(ref VM);
                                        }
                                        else
                                        {
                                            VM.Name = defaultName;
                                            operNode.Name = defaultName;
                                        }
                                        //update files
                                        contentsReader.UpdateContentVersionFiles(VM);
                                        try
                                        {
                                            CMFileSystemUpdaterBLL.AddContentVersionFilesOnFs(VM, null, cmImp);
                                        }
                                        catch (TraceException te)
                                        {                                           
                                            CMContentManagementViewModel.ShowErrorAndInfoMessage(te.Message, null);
                                            CMVersionBLL.DeleteVersion(VM.Id); //delete cloned version if clone files fails
                                            return;
                                        }
                                        CMVersionBLL.AddContentVersionFiles(ref VM);
                                 
                                    }
                                    catch (TraceException te)
                                    {
                                        String logMessage = te.Message + "\n" + "Source: " + te.Source + "\n" + te.StackTrace;
                                        Domain.SaveGeneralErrorLog(logMessage);
                                        //CMVersionBLL.DeleteVersion(newVesrion.Id); //delete the cloned version
                                        SB.Append("SELECT ED_Description FROM ErrorDescription where ED_ID='" + te.ApplicationErrorID + "';");
                                        MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                                        displayMessage = (Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString();
                                        parameters.Add(te.Parameters[0]);  //the linked version name
                                        parameters.Add(destinationNode.Name);
                                        displayMessage = CMContentManagementViewModel.UpdateMessageStringParameters(displayMessage, parameters);
                                        object[] ArgMessageParam = { parameters[0], parameters[1] };
                                        CMContentManagementViewModel.ShowErrorAndInfoMessage(te.ApplicationErrorID, ArgMessageParam);

                                        OnRefreshTreeReceived(null);
                     
                                        return;
                                    }

                                    break; 
                            }           
                     
                        }
  
                        //Do the work; This is currently MOVING the SOURCE node to become a CHILD of the TARGET node
                        if (ParentNode != null)
                        {
                            foreach (CMTreeViewNodeViewModelBase Node in ParentNode.Children)
                            {
                                if (Node.ID == operNode.ID)
                                {
                                    ParentNode.Children.Remove(Node);
                                    break;
                                }
                            }
                        }

                        operNode.Parent = destinationNode; //the destination node is the new parent of the source node
                        destinationNode.Children.Insert(0, operNode); //add source node to detstination children
                        operNode.Parent.ID = destinationNode.ID; //update new parent id of source node

                        switch (operNode.TreeNode.TreeNodeType)
                        {
                            case TreeNodeObjectType.Folder:
                                try
                                {
                                    FM.ParentId = destinationNode.ID;
                                    int tempChildNum1 = 0;
                                    FM.ChildNumber = CMFolderBLL.GetChildIDForAddFolder(destinationNode.ID);
                                    tempChildNum1 = CMContentBLL.GetChildIDForAddContent(destinationNode.ID);

                                    if (tempChildNum1 > FM.ChildNumber)  //take the largest child number
                                        FM.ChildNumber = tempChildNum1;

                                    operNode.ID = CMFolderBLL.UpdateExistingFolder(ref FM); // Perform Db update (Update ParentId and child num of the moved node)
                                    ((CMFolderModel)(operNode.TreeNode)).LastUpdateTime = FM.LastUpdateTime; //update the node lastUpdateTime after move

                                    // Update permission
                                    CMContentManagementViewModel.UpdatePermissionAddNode(operNode.Parent, operNode);
                                    operNode.UserGroupUpdatePermission = operNode.Parent.UserGroupUpdatePermission;
                                    //Bug 3947, 3965
                                    if (!folders.ContainsKey((int)FM.Id))
                                    {
                                        folders.Add((int)FM.Id, FM);
                                    }
                                }
                                catch (TraceException te)
                                {
                                    String logMessage = te.Message + "\n" + "Source: " + te.Source + "\n" + te.StackTrace;
                                    Domain.SaveGeneralErrorLog(logMessage);
                                    SB.Clear();
                                    SB.Append("SELECT ED_Description FROM ErrorDescription where ED_ID='" + te.ApplicationErrorID + "';");
                                    MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                                    displayMessage = (Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString();
                                    parameters.Clear();
                                    parameters.Add(FM.Name);
                                    object[] ArgMessageParam = null;
                                    if (te.ApplicationErrorID == "Folder changed")
                                    {
                                        parameters.Add(FM.LastUpdateUser);
                                        object[] tempArgMessageParam = { parameters[0], parameters[1] };
                                        ArgMessageParam = tempArgMessageParam;
                                    }
                                    else if (te.ApplicationErrorID == "Folder deleted")
                                    {
                                        object[] tempArgMessageParam = { parameters[0] };
                                        ArgMessageParam = tempArgMessageParam;
                                    }

                                    displayMessage = CMContentManagementViewModel.UpdateMessageStringParameters(displayMessage, parameters);

                                    CMContentManagementViewModel.ShowErrorAndInfoMessage(te.ApplicationErrorID, ArgMessageParam);
                                    MessageMediator.NotifyColleagues(WSId + "RefreshTree", this); //Will be returned to the MainWindow signed for this message
                                }

                                break;

                            case TreeNodeObjectType.Content:
                                try
                                {
                                    int tempChildNum2 = 0;
                                    CM.ChildNumber = CMFolderBLL.GetChildIDForAddFolder(destinationNode.ID);
                                    tempChildNum2 = CMContentBLL.GetChildIDForAddContent(destinationNode.ID);

                                    if (tempChildNum2 > CM.ChildNumber)  //take the largest child number
                                        CM.ChildNumber = tempChildNum2;

                                    CM.Id_ContentTree = destinationNode.ID;
                                    CM.ParentID = (int)destinationNode.ID;
                                    operNode.ID = CMContentBLL.UpdateExistingContent(ref CM); // Perform Db update (Update ParentId and child num of the moved node)
                                    ((CMContentModel)(operNode.TreeNode)).LastUpdateTime = CM.LastUpdateTime; //update the node lastUpdateTime after move

                                    // Update permission
                                    CMContentManagementViewModel.UpdatePermissionAddNode(operNode.Parent, operNode);
                                }
                                catch (TraceException te)
                                {
                                    String logMessage = te.Message + "\n" + "Source: " + te.Source + "\n" + te.StackTrace;
                                    Domain.SaveGeneralErrorLog(logMessage);
                                    SB.Clear();
                                    SB.Append("SELECT ED_Description FROM ErrorDescription where ED_ID='" + te.ApplicationErrorID + "';");
                                    MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                                    displayMessage = (Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString();
                                    parameters.Clear();
                                    parameters.Add(CM.Name);
                                    object[] ArgMessageParam = null;
                                    if (te.ApplicationErrorID == "Content changed")
                                    {
                                        parameters.Add(CM.LastUpdateUser);
                                        object[] tempArgMessageParam = { parameters[0], parameters[1] };
                                        ArgMessageParam = tempArgMessageParam;
                                    }
                                    else if (te.ApplicationErrorID == "Content deleted")
                                    {
                                        object[] tempArgMessageParam = { parameters[0] };
                                        ArgMessageParam = tempArgMessageParam;
                                    }

                                    displayMessage = CMContentManagementViewModel.UpdateMessageStringParameters(displayMessage, parameters);

                                    CMContentManagementViewModel.ShowErrorAndInfoMessage(te.ApplicationErrorID, ArgMessageParam);
                                    MessageMediator.NotifyColleagues(WSId + "RefreshTree", this); //Will be returned to the MainWindow signed for this message
                                }
                                break;
                      
                            case TreeNodeObjectType.ContentVersion:
                                try
                                {
                                    VM.id_Content = destinationNode.ID;
                                    VM.ParentID = (int)destinationNode.ID;                             
                                    (new CMContentVersionLinkConfirmer()).ConfirmerContentVersion((VM));       //check if version is linked
                                    VM.ChildNumber = CMVersionBLL.GetChildIDForAddVersion(destinationNode.ID);  //take the largest child number        
                                    operNode.ID = CMVersionBLL.UpdateExistingVersion(ref VM);  // Perform Db update (Update ParentId and child num of the moved node)
                                    ((CMVersionModel)(operNode.TreeNode)).LastUpdateTime = VM.LastUpdateTime;

                                    // Update permission
                                    List<CMTreeNode> treeNodesList = CMVersionBLL.GetChangedLinkedVersion(VM);
                                    CMContentManagementViewModel.UpdatePermissionAddNode(operNode.Parent, operNode);
                                    CMContentManagementViewModel.UpdatePermissionTreeNodeList(treeNodesList);
                                }
                                catch (TraceException te) 
                                {
                                    String logMessage = te.Message + "\n" + "Source: " + te.Source + "\n" + te.StackTrace;
                                    Domain.SaveGeneralErrorLog(logMessage);
                                    SB.Append("SELECT ED_Description FROM ErrorDescription where ED_ID='" + te.ApplicationErrorID + "';");
                                    MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                                    displayMessage = (Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString();
                                    parameters.Add(te.Parameters[0]);  //the linked version name
                                    parameters.Add(destinationNode.Name); 
                                    displayMessage = CMContentManagementViewModel.UpdateMessageStringParameters(displayMessage, parameters);
                                    object[] ArgMessageParam = { parameters[0], parameters[1] };
                                    CMContentManagementViewModel.ShowErrorAndInfoMessage(te.ApplicationErrorID, ArgMessageParam);
                                    OnRefreshTreeReceived(null);                            
                                }
                               
                                break;                         
                        }

                        operNode.TreeNode.ID = (int)operNode.ID;  //update the tree node new id
                        operNode.Parent.IsSelected = true; //focus on parent node after move \ copy

                    }
                }
                catch (Exception e)
                {
                    String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                    Domain.SaveGeneralErrorLog(logMessage);
                    CMContentManagementViewModel.ShowErrorAndInfoMessage("System error", new object[] { 0 });
                }
            }

            #endregion

            #region UserConfirmation

            public bool UserConfirmation(CMTreeViewNodeViewModelBase sourceNode, CMTreeViewNodeViewModelBase destinationNode, ItemNodeActionType actionType)
            {
                List<String> parameters = new List<string>();
                var SB = new StringBuilder(string.Empty);
                String errorId = "";

                switch (actionType)
                {
                    case ItemNodeActionType.Copy:
                         errorId = "Copy";
                         break;
                        
                    case ItemNodeActionType.Move:
                         errorId = "Move";
                         break;                       
                }
              

                SB.Append("SELECT ED_Description FROM ErrorDescription where ED_ID='" + errorId + "';");
                MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                string displayMessage = (Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString();
                parameters.Add(sourceNode.TreeNode.TreeNodeType.ToString());
                parameters.Add(sourceNode.Name);
                parameters.Add(destinationNode.TreeNode.TreeNodeType.ToString());
                parameters.Add(destinationNode.Name);
                displayMessage = CMContentManagementViewModel.UpdateMessageStringParameters(displayMessage, parameters);

                if (MsgBoxService.ShowYesNo(displayMessage, DialogIcons.Question) == DialogResults.Yes)
                {
                    return true;
                }
                return false;
            }

            #endregion

            #region Check If Allow Drop

            public bool CheckIfAllowDrop(CMTreeViewNodeViewModelBase sourceNode, CMTreeViewNodeViewModelBase destinationNode, ItemNodeActionType actionType)
            {
                if (sourceNode == null)
                    return false;
                //if source node is a content version and the user wants to move it - do not allow this (only copy is allowed)
                if ((!Keyboard.IsKeyDown((Key.LeftCtrl)) && !Keyboard.IsKeyDown((Key.RightCtrl))) && sourceNode.GetType().ToString().Equals("ContentMgmtModule.CMTreeViewVersionNodeViewModel"))
                {
                    return false;
                }
                    
                //Destination can't be null
                if (destinationNode == null)
                {
                    return false;
                }
                //No Write permission
                if (!CheckItemNodePermission(sourceNode, destinationNode))
                {
                    return false;
                }
                //Source and Target can't be the same
                if (sourceNode != null && sourceNode.ID == destinationNode.ID)
                {
                    return false;
                }
                //Can't drop parent to child
                if (!CheckDropParentToChild(sourceNode, destinationNode))
                {
                    return false;
                }                
           
                switch (destinationNode.TreeNode.TreeNodeType)
                {
                    case TreeNodeObjectType.Folder:
                    case TreeNodeObjectType.Root:
                        {
                            return CheckDestinationNodeFolder(sourceNode, destinationNode, actionType);
                        }
                    case TreeNodeObjectType.Content:
                        {
                            return CheckDestinationNodeContent(sourceNode, destinationNode, actionType);
                        }
                    case TreeNodeObjectType.ContentVersion:
                        {
                            return CheckDestinationNodeVersion(sourceNode, destinationNode, actionType);
                        }
                }

                return false;
            }

            #endregion

            #region Check Destination Node Folder

            private bool CheckDestinationNodeFolder(CMTreeViewNodeViewModelBase sourceNode, CMTreeViewNodeViewModelBase destinationNode, ItemNodeActionType actionType)
            {
                if (sourceNode == null)
                    return false;

                //bug 3911
                if (destinationNode.TreeNode.TreeNodeType == TreeNodeObjectType.Root
                    && sourceNode.TreeNode.TreeNodeType != TreeNodeObjectType.Folder)
                {
                    return false;
                }

                switch (sourceNode.TreeNode.TreeNodeType)
                {
                    case TreeNodeObjectType.Folder:
                        {
                            switch (actionType)
                            {
                                case ItemNodeActionType.Copy:
                                    return true;
                                case ItemNodeActionType.Move:
                                    return !ExistSubNoteWihtSameName(sourceNode, destinationNode);
                                default:
                                    return false;
                            }
                        }
                    case TreeNodeObjectType.Content:
                        return true;
                    case TreeNodeObjectType.ContentVersion:
                        return false;
                }
                return false;
            }

            #endregion

            #region Check Destination Node Content

            private bool CheckDestinationNodeContent(CMTreeViewNodeViewModelBase sourceNode, CMTreeViewNodeViewModelBase destinationNode, ItemNodeActionType actionType)
            {
                if (sourceNode == null)
                    return false;

                switch (sourceNode.TreeNode.TreeNodeType)
                {
                    case TreeNodeObjectType.Folder:
                        return false;
                    case TreeNodeObjectType.Content:
                        return false;
                    case TreeNodeObjectType.ContentVersion:
                        switch (actionType)
                        {
                            case ItemNodeActionType.Copy:
                                return true;
                            case ItemNodeActionType.Move:
                                return !ExistSubNoteWihtSameName(sourceNode, destinationNode) && ((CMVersionModel)sourceNode.TreeNode).id_ContentVersionStatus == "Edit";
                            default:
                                return false;
                        }
                }
                return false;
            }

            #endregion

            #region Check Destination Node Version

            private static bool CheckDestinationNodeVersion(CMTreeViewNodeViewModelBase sourceNode, CMTreeViewNodeViewModelBase destinationNode, ItemNodeActionType actionType)
            {
                return false;
            }

            #endregion

            #region Exist Sub Note With Same Name

            private bool ExistSubNoteWihtSameName(CMTreeViewNodeViewModelBase sourceNode, CMTreeViewNodeViewModelBase destinationNode)
            {
                foreach (CMTreeViewNodeViewModelBase subItem in destinationNode.Children)
                {
                    if (subItem.Name == sourceNode.Name && subItem != sourceNode)
                        return true;
                }

                return false;
            }

            #endregion

            #region Check Item Node Permission

            private static bool CheckItemNodePermission(CMTreeViewNodeViewModelBase sourceNode, CMTreeViewNodeViewModelBase destinationNode)
            {
                if (sourceNode != null)
                {
                    return sourceNode.IsUpdate && destinationNode.IsUpdate && WritePermission;
                }
                else
                {
                    return destinationNode.IsUpdate && WritePermission;
                }
            }

            #endregion

            #region Check Drop Parent To Child

            private static bool CheckDropParentToChild(CMTreeViewNodeViewModelBase sourceNode, CMTreeViewNodeViewModelBase destinationNode)
            {
                CMTreeViewNodeViewModelBase destinationTemp = destinationNode;
                while (destinationTemp != null)
                {
                    if (sourceNode == destinationTemp)
                        return false;

                    destinationTemp = destinationTemp.Parent;
                }

                return true;
            }

            #endregion

            #region Locate Folder Node

            private CMTreeViewNodeViewModelBase LocateFolderNode(long NodeId)
            {
                WorkNode = null;
                if (NodeId == RootNode.ID)
                {
                    return RootNode;
                }
                else
                {
                    CMTreeViewNodeViewModelBase tempNode1 = RootNode;
                    LocateFolderTreeChildren(ref tempNode1, NodeId);
                    return WorkNode;
                }
            }

            private void LocateFolderTreeChildren(ref CMTreeViewNodeViewModelBase Node, long NodeId)
            {
                foreach (CMTreeViewNodeViewModelBase TreeNode in Node.Children)
                {
                    if (TreeNode.ID == NodeId)
                    {
                        if (TreeNode.TreeNode.TreeNodeType.ToString() == "Folder")
                        {
                            WorkNode = TreeNode;
                            return;
                        }
                    }
                    CMTreeViewNodeViewModelBase tempNode1 = TreeNode;
                    LocateFolderTreeChildren(ref tempNode1, NodeId);
                }
            }

            #endregion

            #region Locate Content Node

            private CMTreeViewNodeViewModelBase LocateContentNode(long NodeId)
            {
                WorkNode = null;
                if (NodeId == RootNode.ID)
                {
                    return RootNode;
                }
                else
                {
                    CMTreeViewNodeViewModelBase tempNode1 = RootNode;
                    LocateContentTreeChildren(ref tempNode1, NodeId);
                    return WorkNode;
                }
            }

            private void LocateContentTreeChildren(ref CMTreeViewNodeViewModelBase Node, long NodeId)
            {
                foreach (CMTreeViewNodeViewModelBase TreeNode in Node.Children)
                {
                    if (TreeNode.ID == NodeId)
                    {
                        if (TreeNode.TreeNode.TreeNodeType.ToString() == "Content")
                        {
                            WorkNode = TreeNode;
                            return;
                        }
                    }
                    CMTreeViewNodeViewModelBase tempNode1 = TreeNode;
                    LocateContentTreeChildren(ref tempNode1, NodeId);
                }
            }

            #endregion

            #region Locate Version Node

            private CMTreeViewNodeViewModelBase LocateVersionNode(long NodeId)
            {
                WorkNode = null;
                if (NodeId == RootNode.ID)
                {
                    return RootNode;
                }
                else
                {
                    CMTreeViewNodeViewModelBase tempNode1 = RootNode;
                    LocateVersionTreeChildren(ref tempNode1, NodeId);
                    return WorkNode;
                }
            }

            private void LocateVersionTreeChildren(ref CMTreeViewNodeViewModelBase Node, long NodeId)
            {
                foreach (CMTreeViewNodeViewModelBase TreeNode in Node.Children)
                {
                    if (TreeNode.ID == NodeId)
                    {
                        if (TreeNode.TreeNode.TreeNodeType.ToString() == "ContentVersion")
                        {
                            WorkNode = TreeNode;
                            return;
                        }
                    }
                    CMTreeViewNodeViewModelBase tempNode1 = TreeNode;
                    LocateVersionTreeChildren(ref tempNode1, NodeId);
                }
            }

            #endregion

        #endregion

        #region  Show / Hide Details on right-hand side

        private void OnShowAndUpdateCMFolderDetailsReceived(CMTreeViewNodeViewModelBase TV)
        {
            if (WritePermission) //if the user have Write Permission enable Update and disable View
            {
                TV.IsView = false;
                TV.IsUpdate = true;
            }
            else  //if the user don't have Write Permission disable Update and enable View
            {
                TV.IsView = true;
                TV.IsUpdate = false;
            }
         
            try
            {
                DetailsViewModel = new CMFolderDetailsViewModel(this.WSId, TV);
            }
            catch (Exception e)
            {
                DetailsViewModel = null;
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
            }
        }

        private void OnShowAndUpdateCMContentDetailsReceived(CMTreeViewNodeViewModelBase TV)
        {
            if (WritePermission) //if the user have Write Permission enable Update and disable View
            {
                TV.IsView = false;
                TV.IsUpdate = true;
            }
            else  //if the user don't have Write Permission disable Update and enable View
            {
                TV.IsView = true;
                TV.IsUpdate = false;
            }
            try
            {
                DetailsViewModel = new CMContentDetailsViewModel(this.WSId, TV);
            }
            catch (Exception e)
            {
                DetailsViewModel = null;
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
            }
        }

        private void OnShowAndUpdateCMVersionDetailsReceived(CMTreeViewNodeViewModelBase TV)
        {
            if (WritePermission) //if the user have Write Permission enable Update and disable View
            {
                TV.IsView = false;
                TV.IsUpdate = true;
            }
            else  //if the user don't have Write Permission disable Update and enable View
            {
                TV.IsView = true;
                TV.IsUpdate = false;
            }
            updateDragDropMode = true;
            try
            {
                DetailsViewModel = new CMVersionDetailsViewModel(this.WSId, TV, updateDragDropMode);
            }
            catch (Exception e)
            {
                DetailsViewModel = null;
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
            }
        }

        private void OnShowCMWhereUsedDetailsReceived(CMTreeViewNodeViewModelBase TV)
        {
            DetailsViewModel = new CMWhereUsedViewModel(this.WSId, TV);
        }

        private void OnCloseCMDetailsViewReceived(object NotUsed)
        {
            DetailsViewModel = null;
        }

        #endregion

        #region  Delete Node

        private void OnDeleteNodeReceived(CMTreeViewNodeViewModelBase Node)
        {
            CMTreeViewNodeViewModelBase SourceNode = null;
            CMTreeViewNodeViewModelBase ParentNode = null;

            if (Node.TreeNode.TreeNodeType.ToString() == "Folder")
            {
                SourceNode = LocateFolderNode(Node.ID);
                ParentNode = LocateFolderNode(Node.Parent.ID);
            }
            else if (Node.TreeNode.TreeNodeType.ToString() == "Content")
            {
               SourceNode = LocateContentNode(Node.ID);
               ParentNode = LocateFolderNode(Node.Parent.ID);
            }
            else if (Node.TreeNode.TreeNodeType.ToString() == "ContentVersion")
            {
                SourceNode = LocateVersionNode(Node.ID);
                ParentNode = LocateContentNode(Node.Parent.ID);
            }

            foreach (CMTreeViewNodeViewModelBase N in ParentNode.Children)
            {
                if (N.ID == SourceNode.ID)
                {
                    ParentNode.Children.Remove(N);
                    break;
                }
            }
            //Clean
            WorkNode = null;

        }

        #endregion

        #region Refresh Button Clicked

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

            //this.Hierarchy.IsDirty = false;
            //this.Hierarchy.VM.IsDirty = false;
            //MessageMediator.NotifyColleagues(this.WorkSpaceId + "OnIsDirtyNodeReceived", null);
            //MessageMediator.NotifyColleagues(this.WSId + "OnRefreshReceived", this.WorkNode); //Will be returned to the MainWindow signed for this message
            OnRefreshTreeReceived(null);
            MessageMediator.NotifyColleagues("StatusBarParameters", null);
        }

        #endregion

        #region Messages

        //Status bar Error and Info messages
        public static MessengerService MessageMediatorErrorAndInfo = ServiceProvider.GetService<MessengerService>();
        public static void ShowErrorAndInfoMessage(string error, object[] Args)
        {
            try
            {
                Collection<string> StatusBarParameters = new Collection<string>();

                var Query = new StringBuilder(string.Empty);
                Query.Append( "SELECT ED_Description, ED_Type FROM ErrorDescription where ED_ID='" + error + "';");
        
                // Fetch the row from the database (retrieving by PK --> 0 or 1 row)
                DataRow MsgRow = (DataRow)Domain.PersistenceLayer.FetchDataTable(Query.ToString(), CommandType.Text, null).Rows[0];

                // Verify the message is found
                if (!string.IsNullOrEmpty((string)MsgRow["ED_Description"]))
                {
                    //Message verbiage with parameters, as retrieved from the table
                    string MsgDescription = (string)MsgRow["ED_Description"];

                    // Arguments substitution, if any
                    string MsgDescriptionWithParam = SetDescriptionParameters(MsgDescription, Args, error);

                    // Verify message description was successfully formatted 

                    // Message Type - consider defining enum for valid values
                    string MsgType = (string)MsgRow["ED_Type"];

                    //Set background color based on message type
                    switch (MsgType.Trim())
                    {
                        case "Acknowledge":
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
                        case "ApiException":
                            if (!MsgDescriptionWithParam.Equals(ErrorString))
                            {
                                StatusBarParameters = SetMessageDescriptionParam(MsgDescriptionWithParam, "White", "Red"); //Background for error messages
                            }
                            else
                            {
                                //MsgDescription = MsgDescription + "(PE_Messages: Invalid number of parameters for Message Id " + error + ")";
                                StatusBarParameters = SetMessageDescriptionParam(MsgDescription, "White", "Red"); //When PE_Messages record contains invalid number of parameters
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
        private static void ShowGenericErrorMessage(string MsgCode)
        {
            Collection<string> StatusBarParametersGenericError = new Collection<string>();

            //String ErrorMessageText = "Unable to retrieve error message " + MsgCode + ". Please see Data Access log file for more details: Shell->View Log."; //Message
            StatusBarParametersGenericError = SetMessageDescriptionParam(MsgCode, "White", "Red");
            MessageMediatorErrorAndInfo.NotifyColleagues("StatusBarParameters", StatusBarParametersGenericError);
        }


        //To catch exception from String.Format function if occurs
        private static string ErrorString = "ParamError";
        private static string SetDescriptionParameters(string MessageDescription, object[] ArgList, string MsgCode)
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
        public static Collection<String> SetMessageDescriptionParam(String MessageText, String FgColor, String BgColor)
        {
            Collection<string> StatusBarParametersAdd = new Collection<string>();

            StatusBarParametersAdd.Add(MessageText); //Message
            StatusBarParametersAdd.Add(FgColor); //Foreground
            StatusBarParametersAdd.Add(BgColor); //Background 

            return StatusBarParametersAdd;
        }   

        public static string UpdateMessageStringParameters(String message, List<String> parameteres)
        {
            if (parameteres != null)
            {
                for (int i = 0; i < parameteres.Count; i++)
                    message = message.Replace("{" + i + "}", parameteres[i]);
            }

            return message;
        }

        #endregion

        #region Security Methods

            #region set Permissions

            public void setPermissions()
            {
                permissions = new List<String> { { "Write" }, { "Update user group" }, { "Add folder to root" } };
                Dictionary<String, bool> applicationPermission =  GetApplicationPermission(permissions);

                ApplicationWritePermission = applicationPermission.ContainsKey("Write") && applicationPermission["Write"];
                WritePermission = ApplicationWritePermission;
                ApplicationUpdateUserGroupPermission = ApplicationWritePermission && applicationPermission.ContainsKey("Update user group") && applicationPermission["Update user group"];
                ApplicationAddRootFolderPermission = ApplicationWritePermission && applicationPermission.ContainsKey("Add folder to root") && applicationPermission["Add folder to root"];
            }

            #endregion

            #region Get Application Permission

            public Dictionary<String, bool> GetApplicationPermission(List<String> permissions)
            {
                try
                {
                    //List<String> cmPermissions = Locator.ProfileManagerProvider.GetApplicationPermission(Locator.UserName);
                    List<String> cmPermissions = CMSecurityBLL.GetApplicationPermission(Domain.User);

                    Dictionary<String, bool> dictionaryPermissions = new Dictionary<string, bool>();
                    foreach (String permission in permissions)
                        dictionaryPermissions.Add(permission, cmPermissions.Contains(permission));

                    return dictionaryPermissions;

                }
                catch (TraceException te)
                {
                    String logMessage = te.Message + "\n" + "Source: " + te.Source + "\n" + te.StackTrace;
                    Domain.SaveGeneralErrorLog(logMessage);
                    te.AddTrace(new StackFrame(1, true));
                    throw te;
                }
                catch (Exception e)
                {
                    String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                    Domain.SaveGeneralErrorLog(logMessage);
                    TraceException te = new TraceException(new StackFrame(1, true), e, "Content manager");
                    throw te;
                }
            }

            #endregion

            #region Update Item Node Permission

            public static void UpdateItemNodePermission(CMTreeViewNodeViewModelBase currNode, CMTreeViewNodeViewModelBase itemNode, bool writePermission)
            {
                if (currNode.TreeNode.TreeNodeType == TreeNodeObjectType.Root) 
                    return;

                bool addPermission;
                bool updatePermission;
                bool deletePermission;

                if (!writePermission)
                {
                    addPermission = false;
                    updatePermission = false;
                    deletePermission = false;
                }
                else
                {
                    addPermission = currNode.TreeNode.ExistPermission("Add");
                    updatePermission = currNode.TreeNode.ExistPermission("Update");
                    deletePermission = currNode.TreeNode.ExistPermission("Delete");
                }

                currNode.IsDelete = deletePermission;
                currNode.IsUpdate = updatePermission;
                           
                switch (currNode.TreeNode.TreeNodeType)
                {
                    case TreeNodeObjectType.Folder:
                        ((CMTreeViewFolderNodeViewModel)(currNode)).IsAddFolder = addPermission;
                        ((CMTreeViewFolderNodeViewModel)(currNode)).IsAddContent = addPermission;
                        break;

                    case TreeNodeObjectType.Content:
                        ((CMTreeViewContentNodeViewModel)(currNode)).IsAddVersion = addPermission;
                        break;
                }
            }

            #endregion

            #region Update Permission Update Node

            public static void UpdatePermissionUpdateNode(CMTreeViewNodeViewModelBase updatedItem)
            {
                List<CMTreeNode> treeNodesList = new List<CMTreeNode> { updatedItem.TreeNode };
                if (updatedItem.Parent != null && updatedItem.Parent.TreeNode.TreeNodeType != TreeNodeObjectType.Root)
                    treeNodesList.Add(updatedItem.Parent.TreeNode);

                (new CMUpdatePermissionBLL(folders, contents, versions)).UpdateTreeNodeRecursive(treeNodesList);

                UpdateItemNodePermission(updatedItem, updatedItem, WritePermission);

                UpdatePermissionItemNode(updatedItem);
                UpdatePermissionItemNode(updatedItem.Parent);

            }

            #endregion

            #region Update Permission ItemNode

            private static void UpdatePermissionItemNode(CMTreeViewNodeViewModelBase itemNode)
            {
                if (itemNode != null)
                    UpdateItemNodePermission(itemNode, itemNode, WritePermission);            
            }

            #endregion

            #region Update Permission Delete Node

            public static void UpdatePermissionDeleteNode(CMTreeViewNodeViewModelBase parentItem)
            {
                if (parentItem != null)
                {
                    (new CMUpdatePermissionBLL(folders, contents, versions)).UpdateTreeNodeRecursive(new List<CMTreeNode> { parentItem.TreeNode });
                    UpdatePermissionItemNode(parentItem);
                }
            }

            #endregion

            #region Update Permission Add Node

            public static void UpdatePermissionAddNode(CMTreeViewNodeViewModelBase parentItem, CMTreeViewNodeViewModelBase newItem)
            {
                List<CMTreeNode> treeNodesList = new List<CMTreeNode>();

                if (parentItem != null)
                    treeNodesList.Add(parentItem.TreeNode);

                if (newItem != null)
                    treeNodesList.Add(newItem.TreeNode);

                if (treeNodesList.Count > 0)
                {
                    (new CMUpdatePermissionBLL(folders, contents, versions)).UpdateTreeNodeRecursive(treeNodesList);

                    if (parentItem != null)
                        UpdatePermissionItemNode(parentItem);

                    if (newItem != null)
                        UpdatePermissionItemNode(newItem);
                }
            }

            #endregion

            #region Update Permission TreeNode List

            public static void UpdatePermissionTreeNodeList(List<CMTreeNode> treeNodesList)
            {
                (new CMUpdatePermissionBLL(folders, contents, versions)).UpdateTreeNodeRecursive(treeNodesList);

               // UpdatePermissionTreeNodeListItemNode(treeNodesList, Locator.ContentManagerDataProvider.SubItemNode);
            }

            #endregion

            //private static void UpdatePermissionTreeNodeListItemNode(List<CMTreeNode> treeNodesList, ObservableCollection<CMTreeViewNodeViewModelBase> itemNodes)
            //{
            //    foreach (CMTreeViewNodeViewModelBase itemNode in itemNodes)
            //    {
            //        if (treeNodesList.Contains(itemNode.TreeNode))
            //            UpdateItemNodePermission(itemNode, itemNode, WritePermission);

            //        //UpdatePermissionTreeNodeListItemNode(treeNodesList, itemNode.SubItemNode);
            //    }
            //}


        #endregion

        #region methods
    
        //Extended Search Parameters received
        private void OnSearchParametersReceived(Collection<object> Parameters)
        {
            //If there are no search parameters OR the search by string is empty return
            if (Parameters.Count < 1 || Parameters[0].Equals(""))
            {
                return;
            }
            //Local variables
            SearchText = (string)(Parameters[0]);
            SearchParameters = Parameters;
            //Execute
            ExecuteFindCommand();
        }

        #endregion
    }
}
