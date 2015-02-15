﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using ATSBusinessLogic;
using ATSBusinessObjects;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using Infra.MVVM;
using NotesModule;



namespace ExplorerModule
{
	public class VersionDetailsViewModel : ViewModelBase
	{

        #region  Data

        protected MessengerService MessageMediator = new MessengerService();
        private IMessageBoxService MsgBoxService = null;

        private IEnumerable<ContentModel> _activeContents;

        private bool cannotUpdateActiveContents = true;

        private Guid WorkspaceId;

        private HierarchyModel _Hierarchy;
        public HierarchyModel Hierarchy
        {
            get
            {
                return _Hierarchy;
            }
            set
            {
                _Hierarchy = value;
            }
        }

        private VersionModel _VM;
        public VersionModel VM
        {
            get
            {
                return _VM;
            }
            set
            {
                _VM = value;
            }
        }

        private bool _ShowNotes = true;
        public bool ShowNotes
        {
            get
            {
                return _ShowNotes;
            }
            set
            {
                _ShowNotes = value;
                RaisePropertyChanged("ShowNotes");
            }
        }

        //fix 1869
        public static string DateTimeFormat
        {
            get
            {
                return Domain.DateTimeFormat;
            }
        }

        public NotesControlViewModel Notes { get; set; }

        #endregion

        #region Presentation Properties

        public long VersionId
        {
            get
            {
                return _VM.VersionId;
            }
            set
            {
                if (_VM != null)
                {
                    _VM.VersionId = value;
                    RaisePropertyChanged("VersionId");
                }
            }
        }

        public string ECRId
        {
            get
            {
                return _VM.EcrId;
            }
            set
            {
                if (_VM != null)
                {
                    _VM.EcrId = value;
                    RaisePropertyChanged("ECRId");
                }
            }
        }

        public DateTime? CreationDate
        {
            get
            {
                if (_VM != null)
                {
                    return _VM.CreationDate;
                }
                else
                {
                    return null;
                }
            }
            set
            {
            }
        }

        public DateTime? LastUpdateTime
        {
            get
            {
                if (_VM != null)
                {
                    return _VM.LastUpdateTime;
                }
                else
                {
                    return null;
                }
            }
            set
            {
            }
        }

        public string LastUpdateUser
        {
            get
            {
                if (_VM != null)
                {
                    return _VM.LastUpdateUser;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
            }
        }

        public string VersionName
        {
            get
            {
                if (_VM != null)
                {
                    return _VM.VersionName;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (_VM != null)
                {
                     _VM.VersionName = value;
                    RaisePropertyChanged("VersionName");
                }
            }
        }

        public string VersionDescription
        {
            get
            {
                if (_VM != null)
                {
                    return _VM.Description;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                //CertificatesChanged = true;
                if (_VM != null)
                {
                    _VM.Description = value;
                    RaisePropertyChanged("Description");
                }
            }
        }

        public string TargetPath
        {
            get
            {
                return VM.TargetPath;
            }
            set
            {
                //CertificatesChanged = true;
                if (Hierarchy != null)
                {
                    Hierarchy.VM.TargetPath = value;
                    RaisePropertyChanged("TargetPath");
                }

            }
        }

        public Boolean DefaultTargetPathInd
        {

            get
            {
                return VM.DefaultTargetPathInd;
            }
            set
            {
                //CertificatesChanged = true;
                VM.DefaultTargetPathInd = value;
                RaisePropertyChanged("DefaultTargetPathInd");
                RaisePropertyChanged("TargetPath");
                if (VM.DefaultTargetPathInd == true)
                {
                    Hierarchy.VM.TargetPath = getTargetPath();
                    RaisePropertyChanged("TargetPath");
                }
            }
        }

        public String VersionStatus
        {
            get
            {
                if (_VM != null)
                {
                    return _VM.VersionStatus;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                //CertificatesChanged = true;
                if (_VM != null)
                {
                    _VM.VersionStatus = value;
                    RaisePropertyChanged("VersionStatus");
                }
            }

        }

       private ObservableCollection<VersionModel> _GetAllVersions;
       public ObservableCollection<VersionModel> GetAllVersions
        {
            get
            {
                return _GetAllVersions;
            }

            
        }

       public VersionModel GetVersionModel()
       {
           VersionModel _ActiveVersionModel = new VersionModel();
            if (GetAllVersions != null)
            {
                foreach (VersionModel v in GetAllVersions)
                {
                    if (v.VersionStatus == "A")
                    {
                        _ActiveVersionModel = ATSBusinessLogic.VersionBLL.GetActiveVersion(v.VersionId);
                    }
                }
            }
            return _ActiveVersionModel;
        }

        private TreeViewNodeViewModelBase Node;

        #endregion

        #region Constructor

        public VersionDetailsViewModel(Guid workspaceId, TreeViewNodeViewModelBase TV)
        {
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();
            // Initialize Object
            this.WorkspaceId = workspaceId;
            Node = TV;
            Hierarchy = TV.Hierarchy;

            if (Hierarchy.GroupId == -1)
                VM = VersionBLL.GetVersionByIdAndHierarchyId(TV.Id, TV.Hierarchy.Id);
            else
                VM = VersionBLL.GetVersionByIdAndHierarchyId(TV.Id, TV.Hierarchy.GroupId);



            //VM = TV.Hierarchy.VM;
      
            bool showNotesSideBar = true;

            bool isCloneRelated = false;

            long HierarchyId = -1;

            if (Hierarchy.GroupId == -1)
            {
                HierarchyId = TV.Hierarchy.Id;
            }
            else
            {
                HierarchyId = Hierarchy.GroupId;
            }

            Notes = new NotesControlViewModel(HierarchyId, TV.Name, ref isCloneRelated, ref showNotesSideBar, this.WorkspaceId);
            ShowNotes = showNotesSideBar;
            _GetAllVersions = ATSBusinessLogic.VersionBLL.GetAllVersions(_Hierarchy.Id);
            //VersionModel VM = GetVersionModel();

            if (activeContents != null && activeContents.ToList().Count > 0)
            {
                getTreelist();
                RaisePropertyChanged("TreeNodesLinks");
            }
            MessageMediator.Register(this.WorkspaceId + "OnShowContentTabDetailsReceived", new Action<CMTreeViewNodeViewModelBase>(OnShowContentTabDetailsReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnRapidExecutionVersionReceived", new Action<TreeViewVersionNodeViewModel>(OnRapidExecutionReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnRapidExecutionVersionStartReceived", new Action<TreeViewVersionNodeViewModel>(OnRapidExecutionVersionStartReceived)); //Register to recieve a message asking for node refresh
            MessageMediator.Register(this.WorkspaceId + "OnVersionProgressTextReceived", new Action<TreeViewVersionNodeViewModel>(OnVersionProgressTextReceived)); //Progress bar text     
        
        
        }

        #endregion

        #region  IDropTarget Members & Other Drop Activities

        public Boolean ContentFiler = true;

        #endregion

        #region Active Contents

        public void activeContentDataFiller()
        {
            ContentBLL bll = new ContentBLL(VM.VersionId);
            try
            {
                _activeContents = bll.getActiveContents();
            }
            catch (Exception ex)
            {
                if (ex.Message == "CMDataError")
                {
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowHardCodedErrorMessage("CM API Data error. Please check Contents tree.");
                }
                else if (ex.Message == "144")
                {
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(144, new Object[] { 0 });
                }
                else
                {
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, new Object[] { 0 });
                }
            }
        }

        public IEnumerable<ContentModel> activeContents
        {
            get
            {
                if (cannotUpdateActiveContents && ContentFiler)
                {
                    if (_activeContents != null)
                    {
                        _activeContents.ToList().Clear();
                    }
                    activeContentDataFiller();
                }
                return _activeContents;
            }
        }

        #endregion

        #region  Other Methods

        public string getTargetPath()
        {
            var Target = new StringBuilder(string.Empty);
            try
            {

                var SysPathQry = new StringBuilder(string.Empty);
                if (Hierarchy.IsClonedRelated == false)
                {
                    SysPathQry.Append("select Value from PE_SystemParameters where Variable='ProjectLocalPath'");
                }
                if (Hierarchy.IsClonedRelated == true)
                {
                    SysPathQry.Append("select Value from PE_SystemParameters where Variable='RelatedProjectLocalPath'");
                }
                string SysParm = (string)Domain.PersistenceLayer.FetchDataValue(SysPathQry.ToString(), System.Data.CommandType.Text, null);

                Target = new StringBuilder(string.Empty);
                Target.Append(SysParm.ToString().Trim());
                if (_Hierarchy.ParentId != -1 || _Hierarchy.ParentId != null)
                {


                    Target.Append(VersionBLL.getParentName(_Hierarchy.ParentId.ToString().Trim()));
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                return e.Message;
            }
            if (Hierarchy.GroupId == -1 && Hierarchy.IsClonedRelated == false)
            {
                Target.Append("/" + Hierarchy.Name.ToString().Trim() + "/" + VM.VersionName.ToString().Trim());
            }
            else
            {
                Target.Append("/" + Hierarchy.GroupName.ToString().Trim() + "/" + VM.VersionName.ToString().Trim());
            }
            return Target.ToString();

        }

        #endregion

        #region contentsTab

        private Dictionary<int, CMFolderModel> Folders;
        private Dictionary<int, CMContentModel> Contents;
        private Dictionary<int, CMVersionModel> Versions;
        private List<CMTreeNode> ContentsTree;
        private List<CMTreeNode> LinkedVersions = new List<CMTreeNode>();
        private CMTreeViewNodeViewModelBase WorkNode;

        private ObservableCollection<CMTreeViewNodeViewModelBase> _TreeNodesLinks = new ObservableCollection<CMTreeViewNodeViewModelBase>();
        public ObservableCollection<CMTreeViewNodeViewModelBase> TreeNodesLinks
        {
            get
            {
                return _TreeNodesLinks;
            }
            set
            {
                _TreeNodesLinks = value;
                RaisePropertyChanged("TreeNodesLinks");
            }
        }



        public void getTreelist()
        {
            try
            {

                //Get Tree Node List of all the existing contents.
                foreach (var c in activeContents)
                {
                    if (!ContentBLL.versions.Keys.Contains(c.id) || !ContentBLL.contents.Keys.Contains(ContentBLL.versions[c.id].ParentID))
                    {
                        //Refresh Contents tree from CM - API
                        String logMessage = "Missing information for versionId " + c.id + ".";
                        logMessage = logMessage + "\n                  Refreshing Contents tree data.";
                        Domain.SaveGeneralWarningLog(logMessage);

                        ContentBLL.allContents = ContentBLL.LoadContentTreeToMemory(out ContentBLL.folders, out ContentBLL.contents, out ContentBLL.versions);

                        if (ContentBLL.allContents == null)
                        {
                            ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, new Object[] { 0 });
                        }
                    }
                    int versionParentId = ContentBLL.versions[c.id].ParentID;
                    LinkedVersions.Add(ContentBLL.contents[versionParentId]);
                }
                PopulateTreeView(0);
            }
            catch (Exception e)
            {
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, new object[] { 0 });
            }

        }// end of getTreeList


        private CMTreeViewRootNodeViewModel RootNode
        {
            get
            {
                return TreeNodesLinks[0] as CMTreeViewRootNodeViewModel;
            }
        }

        private void PopulateTreeView(long ParentId)
        {
            if (LinkedVersions.Count > 0)
            {
                //If the Tree is empty, create the Environment node
                if (TreeNodesLinks.Count == 0)
                {
                    CMTreeNode TN = new CMTreeNode();
                    TN.ID = 0;
                    TN.ParentID = int.MinValue;
                    TN.Name = "Contents";
                    TreeNodesLinks.Add(new CMTreeViewRootNodeViewModel(this.WorkspaceId, TN));
                    WorkNode = RootNode;
                }
                //Local variables initialization
                CMTreeViewNodeViewModelBase OperationRoot = WorkNode;
                OperationRoot.Children.Clear();

                //Scan the contentTree for parent nodes (nodes who are the direct children of the requested parent)
                /**var nodesList =
                    from T in LinkedVersions
                    where T.ParentID == ParentId
                    select T;**/

                CMTreeViewNodeViewModelBase TreeNode = null;
                foreach (CMTreeNode tn in LinkedVersions)
                {
                    if (tn.ID != 0)
                    {
                        switch (tn.TreeNodeType.ToString())
                        {
                            case "Folder":
                                TreeNode = new CMTreeViewFolderNodeViewModel(this.WorkspaceId, tn, RootNode);
                                break;
                            case "Content":
                                TreeNode = new CMTreeViewContentNodeViewModel(this.WorkspaceId, tn, RootNode);
                                break;
                            case "ContentVersion":
                                //TreeNode = new CMTreeViewVersionNodeViewModel(this.WorkspaceId, tn, RootNode);
                                TreeNode = new CMTreeViewVersionNodeViewModel(this.WorkspaceId, tn, RootNode, Hierarchy, _activeContents);
                                break;
                            default:
                                TreeNode = new CMTreeViewFolderNodeViewModel(this.WorkspaceId, tn, RootNode);
                                break;

                        }
                        PopulateChildren(TreeNode); //Read the children of the newly created node (recursively)
                        OperationRoot.Children.Add(TreeNode);
                    }
                }
            }

        }

        private void PopulateChildren(CMTreeViewNodeViewModelBase Node)
        {

            List<CMTreeNode> nodesList = new List<CMTreeNode>();
            List<int> VersionsID = new List<int>();

            if (Node.TreeNode.TreeNodeType.ToString() == "ContentVersion")
            {
                VersionsID.AddRange(ContentBLL.versions[Node.TreeNode.ID].ContentVersions.Keys);

                //foreach (var t in ContentBLL.versions[Node.TreeNode.ID].ContentVersions)
                //{
                //    VersionsID.Add(t.Key);
                //    //nodesList.Add(t.Value);
                //}


                foreach (int j in VersionsID)
                {
                    nodesList.Add(ContentBLL.versions[j]);
                }

            }


            if (Node.TreeNode.TreeNodeType.ToString() == "Content")
            {

                foreach (var i in activeContents)
                {

                    if (ContentBLL.versions[i.id].ParentID == Node.TreeNode.ID)
                    //nodesList.Add(item);
                        nodesList.Add(ContentBLL.versions[i.id]);
                }

            }
            CMTreeViewNodeViewModelBase TreeNode = null;
            foreach (CMTreeNode tn in nodesList)
            {
                switch (tn.TreeNodeType.ToString())
                {
                    case "Folder":
                        TreeNode = new CMTreeViewFolderNodeViewModel(this.WorkspaceId, tn, Node);
                        break;
                    case "Content":
                        TreeNode = new CMTreeViewContentNodeViewModel(this.WorkspaceId, tn, Node);
                        break;
                    case "ContentVersion":
                        //TreeNode = new CMTreeViewVersionNodeViewModel(this.WorkspaceId, tn, Node);
                        TreeNode = new CMTreeViewVersionNodeViewModel(this.WorkspaceId, tn, RootNode, Hierarchy, _activeContents);
                        break;
                }
                PopulateChildren(TreeNode);
                //For CR3600
                TreeNode.Name = ContentBLL.contents[ContentBLL.versions[tn.ID].ParentID].Name + " " + ContentBLL.versions[tn.ID].Name;
                Node.Children.Add(TreeNode);
            }


        }

        CMTreeViewNodeViewModelBase SelectedNode;
        private void OnShowContentTabDetailsReceived(CMTreeViewNodeViewModelBase Node)
        {
            SelectedNode = Node;
            RaisePropertyChanged("NodeName");
            RaisePropertyChanged("NodeDescription");
            RaisePropertyChanged("CommandLine");
            RaisePropertyChanged("NodeStatus");
            RaisePropertyChanged("ContentPath");
            RaisePropertyChanged("Files");
            RaisePropertyChanged("ContentName");


        }

        private string _ContentName;
        public string ContentName
        {
            get
            {
                if (SelectedNode != null)
                {
                    if (SelectedNode.GetType() == typeof(CMTreeViewContentNodeViewModel))
                    {
                        if (SelectedNode != null)
                        {
                            return SelectedNode.Name;
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                    else if (SelectedNode.GetType() == typeof(CMTreeViewVersionNodeViewModel))
                    {
                        return ContentBLL.contents[ContentBLL.versions[SelectedNode.TreeNode.ID].ParentID].Name;
                    }
                    else
                        return string.Empty;
                }
                else
                    return string.Empty;
            }
        }

        private string _NodeName;
        public string NodeName
        {
            get
            {
                if (SelectedNode != null)
                {
                    if (SelectedNode.GetType() == typeof(CMTreeViewVersionNodeViewModel))
                    {

                        return SelectedNode.Name;

                    }
                    else
                        return string.Empty;
                }
                else
                    return string.Empty;
            }
        }

        private string _NodeDescription;
        public string NodeDescription
        {
            get
            {
                if (SelectedNode != null)
                {
                    if (SelectedNode.GetType() == typeof(CMTreeViewVersionNodeViewModel))
                        return ContentBLL.versions[SelectedNode.TreeNode.ID].Description;
                    else if (SelectedNode.GetType() == typeof(CMTreeViewContentNodeViewModel))
                        return ContentBLL.contents[SelectedNode.TreeNode.ID].Description;
                    else
                        return string.Empty;

                }
                else
                {
                    return string.Empty;
                }
            }
        }



        private string _CommandLine;
        public string CommandLine
        {
            get
            {
                if (SelectedNode != null)
                {
                    if (SelectedNode.GetType() == typeof(CMTreeViewVersionNodeViewModel))
                        return ContentBLL.versions[SelectedNode.TreeNode.ID].RunningString;
                    else
                        return string.Empty;

                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private string _NodeStatus;
        public string NodeStatus
        {
            get
            {
                if (SelectedNode != null)
                {
                    if (SelectedNode.GetType() == typeof(CMTreeViewVersionNodeViewModel))
                        return ContentBLL.versions[SelectedNode.TreeNode.ID].Status.Name;
                    else
                        return string.Empty;

                }
                else
                {
                    return string.Empty;
                }
            }
        }



        private string _ContentPath;
        public string ContentPath
        {
            get
            {
                if (SelectedNode != null)
                {

                    if (SelectedNode.GetType() == typeof(CMTreeViewVersionNodeViewModel))
                    {
                        if (ContentBLL.versions[SelectedNode.TreeNode.ID].Files.Count > 0)
                        {
                            string FilePathf = "";
                            string FilePathN = "";
                            string FilePathR = "";
                            foreach (var i in ContentBLL.versions[SelectedNode.TreeNode.ID].Files)
                            {
                                if (!(String.IsNullOrEmpty(i.Value.FileRelativePath)))
                                {
                                    FilePathf = i.Value.FileFullPath.Replace(i.Value.FileName, "");
                                    //FilePathR = Versions[SelectedNode.TreeNode.ID].Files[0].FileRelativePath;
                                    FilePathN = FilePathf.Replace(i.Value.FileRelativePath, "");
                                    return FilePathN.Trim();

                                }
                                else
                                {
                                    FilePathf = i.Value.FileFullPath.Replace(i.Value.FileName, "");
                                    return FilePathf.Trim();

                                }
                                break;
                            }

                            return string.Empty;

                        }
                        else
                            return string.Empty;
                    }
                    else
                        return string.Empty;
                }
                else
                    return string.Empty;
            }
        }

      

        private ObservableCollection<string> _Files = new ObservableCollection<string>();
        public ObservableCollection<string> Files
        {

            get
            {
                if (SelectedNode != null)
                {
                    if (_Files != null)
                        _Files.Clear();

                    if (SelectedNode.GetType() == typeof(CMTreeViewVersionNodeViewModel))
                    {
                        if (ContentBLL.versions[SelectedNode.TreeNode.ID].Files.Count > 0)
                        {
                            foreach (var i in ContentBLL.versions[SelectedNode.TreeNode.ID].Files)
                                _Files.Add(i.Value.FileFullPath);
                            return _Files;
                        }
                        return _Files;
                    }
                    return _Files;
                }
                else
                    return _Files;

            }
        }


        #endregion contentsTab

        #region Content Execution

        #region Relay Command
        Dictionary<String, String> certificates = new Dictionary<string, string>();
        private RelayCommand _ContentCopyCommand;
        private bool allowExecute = true;

        public ICommand ContentCopyCommand
        {
            get
            {
                if (_ContentCopyCommand == null)
                {
                    _ContentCopyCommand = new RelayCommand(ExecuteContentCopyCommand, CanExecuteContentCopyCommand);
                }
                return _ContentCopyCommand;
            }
        }
        public ContentModel contentToAction { set; get; }

        private bool CanExecuteContentCopyCommand()
        {
            //Check if version is saved before enabling execute
            if (allowExecute && !Hierarchy.IsCloned && !Hierarchy.IsClonedRelated && !Hierarchy.IsClonedRelatedSplit && !Hierarchy.IsClonedRelatedUpdate && contentToAction is ContentModel)
                return true;
            else
                return false;
        }

        private void ExecuteContentCopyCommand()
        {
            MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
            
            ContentExecutionBLL.ErrorHandling luStatus = new ContentExecutionBLL.ErrorHandling();
            //Last Update and permission check
            luStatus = ContentExecutionBLL.PriorValidations(_Hierarchy, "116");
            if (!(String.IsNullOrEmpty(luStatus.errorId)))
            {
                MessageMediator.NotifyColleagues(this.WorkspaceId + "OnIsCanceledNodeReceived", this.Node);
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(luStatus.errorId), luStatus.errorParams);
                return;
            }
            Thread ContentExecutionThread = new Thread(new ThreadStart(ContentExecution));
            ContentExecutionThread.Start();
            certificates = new Dictionary<string, string>();
        }

        #endregion

        #region Main content excecute command

        private void ContentExecution()
        {
            int errorId = -1;
            ProgressText = "Performing required validations prior to copying files ...";
            ContentExecutionBLL.ErrorHandling Status = new ContentExecutionBLL.ErrorHandling();

            try
            {
                #region Get CM Sub tree
                //Get contents, folder and version tree from API
                Dictionary<int, CMFolderModel> outFolders = new Dictionary<int, CMFolderModel>();
                Dictionary<int, CMContentModel> outContents = new Dictionary<int, CMContentModel>();
                Dictionary<int, CMVersionModel> outVersions = new Dictionary<int, CMVersionModel>();

                HierarchyModel versionHierarchy = Hierarchy;
                versionHierarchy.VM = this.VM;
                versionHierarchy.VM.Contents = (ObservableCollection<ContentModel>)_activeContents;
                //Performance
                //Get CM sub tree
                Status = ContentExecutionBLL.GetCMSubTree((ObservableCollection<ContentModel>)_activeContents, out outFolders, out outContents, out outVersions);
                #region Handle Get CM sub tree errors if any
                if (Status.errorId != string.Empty)
                {
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Status.errorId), Status.errorParams);
                    return;
                }
                #endregion

                #endregion

                #region Perform all required validations prior to execution
                Status = ContentExecutionBLL.validations(outContents, outVersions, contentToAction, versionHierarchy);
                #region Handle validations errors if any
                if (Status.errorId != string.Empty)
                {
                    if (Status.errorId == "107")
                    {
                        if (MsgBoxService.ShowOkCancel(Status.errorParams[0].ToString(), DialogIcons.Question) != DialogResults.OK)
                        {
                            return;
                        }
                        else
                        {
                            bool temp = ATSDomain.Domain.IsPermitted("126");
                            if (Status.errorId == "107" && !Domain.IsPermitted("126"))
                            {
                                ProjectsExplorerViewModel.ShowErrorAndInfoMessage(106, new object[] { 0 });
                                return;
                            }
                        }
                    }
                    else
                    {
                        ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Status.errorId), new object[] { 0 });
                        return;
                    }
                }
                #endregion

                Status = ContentExecutionBLL.userCertificatesValidations(versionHierarchy);
                #region Handle validations errors if any
                if (Status.errorId != string.Empty)
                {
                    if (Status.errorId == "170")
                    {
                        if (MsgBoxService.ShowOkCancel(Status.errorParams[0].ToString(), DialogIcons.Question) != DialogResults.OK)
                        {
                            return;
                        }
                        else
                        {
                            bool temp = ATSDomain.Domain.IsPermitted("126");
                            if (Status.errorId == "170" && !Domain.IsPermitted("133"))
                            {
                                ProjectsExplorerViewModel.ShowErrorAndInfoMessage(106, new object[] { 0 });
                                return;
                            }
                        }
                    }
                    else
                    {
                        ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Status.errorId), new object[] { 0 });
                        return;
                    }
                }
                #endregion

                #endregion

                #region Start new thread for copying files and copy files to local directory
            ThreadStart threadStart = new ThreadStart(IncrementProgressCounter);
            Thread t = new Thread(threadStart);
            t.Start();
                Status = ContentExecutionBLL.prepareLocalDirectoryToExecution(outVersions, outContents, threadStart,
                                                                                 (ObservableCollection<ContentModel>)_activeContents,
                                                                                 contentToAction,
                                                                                 Hierarchy);
                #region Handle files copy errors if any
                if (Status.errorId != string.Empty)
                {
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Status.errorId), Status.errorParams);
                    return;
        }

        #endregion

                #endregion

                #region Execute content version command line
                IncrementProgressCounter(10);
                Status = ContentExecutionBLL.ExecuteContentVersionAndSaveInfo(versionHierarchy, outVersions, outContents, contentToAction.id);
                #region Handle execution errors if any
                if (Status.errorId != string.Empty)
                {
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Status.errorId), Status.errorParams);
                    ProgressText = "0 %";
                    RaisePropertyChanged("ProgressText");
                    return;
                }
                #endregion

                #endregion

                #region     Save Execution History
                Status = ContentExecutionBLL.recordExecutionHistory(versionHierarchy.VM.VersionId);
                #region Handle save history errors if any

                if (Status.errorId != string.Empty)
                {
                    String logMessage = "Failed to save record in ExecutionHistory table";
                    Domain.SaveGeneralErrorLog(logMessage);
                }
                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, new object[] { 0 });
            }
        }

        #endregion Main content excecute command

        #region Progress Bar
        #region Fields

        // Property variables
        private float p_Progress = 0;
        private int p_ProgressMax = 10;
        private String p_ProgressText = "";
        private float totalProgress = 0f;
        #endregion

        #region Data Properties

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

        //The maximum progress value.
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


        #endregion

        #region Internal Methods


        //Clears the view model.
        internal void ClearViewModel()
        {
            p_Progress = 0;
            p_ProgressMax = 100;
            IncrementProgressCounter(10);
            FileSystemBLL.filesCompleted = 0;
        }

        // Advances the progress counter for the Progress dialog.
        public void IncrementProgressCounter(float incrementClicks)
        {
            incrementClicks = (int)(incrementClicks * 10);

            totalProgress = incrementClicks;
            if (totalProgress > 99)
            {
                totalProgress = 100;
                Progress = p_ProgressMax;
                ProgressText = "100 %";
                FileSystemBLL.filesCompleted = 0;
            }
            else if (totalProgress < 0)
            {
                totalProgress = 0;

                ProgressText = 0 + "%";
            }
            else
            {
                // Increment counter
                this.Progress = incrementClicks / 10;
                ProgressText = totalProgress + "%";
            }

            // Update progress message
            float progress = (p_Progress);
            float progressMax = (p_ProgressMax);
            float f = (progress / progressMax);
            float percentComplete = Single.IsNaN(f) ? 0 : (f);
        }

        public void IncrementProgressCounter()
        {
            float upPrecent = (((float)FileSystemBLL.filesCompleted / ((float)FileSystemBLL.filesToCopyNumber))) * 10;
            IncrementProgressCounter(upPrecent);
        }

        #endregion

        #endregion

        private void OnRapidExecutionReceived(TreeViewVersionNodeViewModel TV)
        {

            ThreadStart threadStart = new ThreadStart(IncrementProgressCounter);
            Thread t = new Thread(threadStart);
            t.Start();


        }

        public int tabIndex { get; set; }
        private void OnRapidExecutionVersionStartReceived(TreeViewVersionNodeViewModel TV)
        {
            tabIndex = 0;
            RaisePropertyChanged("tabIndex");
            ProgressText = "Performing required validations prior to copying files...";
        }

        private void OnVersionProgressTextReceived(TreeViewVersionNodeViewModel TV)
        {
            this.ProgressText = TV.ProgressText;
            RaisePropertyChanged("ProgressText");
            this.Progress = TV.Progress;
            RaisePropertyChanged("Progress");
        }

        private void OnClearViewModelReceived(TreeViewProjectNodeViewModel TP)
        {
            ClearViewModel();
        }
        private void OnDisableProjectReceived(TreeViewProjectNodeViewModel TP)
        {
            bool showNotesSideBar = true;

            bool isCloneRelated = false;

            Notes = new NotesControlViewModel(Hierarchy.Id, Hierarchy.Name, ref isCloneRelated, ref showNotesSideBar, this.WorkspaceId);
        }

        #endregion Content Execution

        #region Open Path Execute

        private RelayCommand _OpenPathExecute;
        public ICommand OpenPathExecute
        {
            get
            {
                if (_OpenPathExecute == null)
                {
                    _OpenPathExecute = new RelayCommand(ExecuteOpenPathExecute, CanExecuteOpenPathExecute);
                }
                return _OpenPathExecute;
            }
        }

        private bool CanExecuteOpenPathExecute()
        {

            return !String.IsNullOrEmpty(Hierarchy.VM.TargetPath);

        }

        private void ExecuteOpenPathExecute()
        {
            /* Original Code ROTEM */
            //if (!String.IsNullOrEmpty(Hierarchy.VM.TargetPath))
            //{
            //    if (Directory.Exists(Hierarchy.VM.TargetPath))
            //        Process.Start(@Hierarchy.VM.TargetPath);
            //    else
            //    {
            //        Object[] ArgsList = new Object[] { 0 };
            //        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(157, ArgsList);
            //    } 
            //}

            try
            {
                string newPath = this.TargetPath;
                if (Directory.Exists(newPath))
                    Process.Start(@newPath);
                else
                {
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(157, ArgsList);
                } 
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, new Object[] { 0 });
            }
            /* - - - End new Code - - - */
        }

        #endregion

        #region Open Content Version In Cm

        private RelayCommand _OpenContentVersionInCm;
        public ICommand OpenContentVersionInCm
        {
            get
            {
                if (_OpenContentVersionInCm == null)
                {
                    _OpenContentVersionInCm = new RelayCommand(ExecuteOpenContentVersionInCm, CanExecuteOpenContentVersionInCm);
                }
                return _OpenContentVersionInCm;
            }
        }

        private bool CanExecuteOpenContentVersionInCm()
        {
            return true;
        }

        private void ExecuteOpenContentVersionInCm()
        {
            MessageMediator.NotifyColleagues("RequestGotoCm", this.contentToAction); //Send message to the MainViewModel to clear Statusbar from any previous operation
        }

        #endregion

    }

} //end of root namespace