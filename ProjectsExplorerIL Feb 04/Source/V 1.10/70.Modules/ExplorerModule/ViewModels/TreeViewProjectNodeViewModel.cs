using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ATSBusinessLogic;
using ATSBusinessObjects;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using Infra.MVVM;

namespace ExplorerModule
{
    public class TreeViewProjectNodeViewModel : TreeViewNodeViewModelBase
    {
        Guid WSId;
        protected MessengerService MessageMediator = new MessengerService();
        private IMessageBoxService MsgBoxService = null;

        public TreeViewProjectNodeViewModel(Guid workspaceId, HierarchyModel Hierarchy)
            : this(workspaceId, Hierarchy, null)
        {
            WSId = workspaceId;
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
        }

        public TreeViewProjectNodeViewModel(Guid workspaceId, HierarchyModel Hierarchy, TreeViewNodeViewModelBase ParentNode)
            : base(workspaceId, Hierarchy, ParentNode)
        {
            //The messageMediator is registered in the ViewModelBase - Generally you have 1 mediator; Hence, the restricted access to the constructor
            MessageMediator = GetService<MessengerService>();
            WSId = workspaceId;
            MessageMediator.Register(this.WorkSpaceId + "OnSplitDetailsReceived", new Action<HierarchyModel>(OnSplitDetailsReceived)); //Register to recieve a menu refresh
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
        }

        public override string NodeData
        {
            get
            {
                StringBuilder SB = new StringBuilder(this.Name);
                if (this.Hierarchy.Code.Trim().Length > 0 || this.Hierarchy.SelectedStep.Trim().Length > 0)
                {
                    SB.Append(" - " + this.Hierarchy.Code + " " + this.Hierarchy.SelectedStep);
                }
                return SB.ToString();
            }
        }

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
            MessageMediator.NotifyColleagues(WorkSpaceId + "ShowProjectDetails", this); //Will be returned to the Explorer Main signed for this message
        }

        //Sends a message to the MainWindow with the required information to save a details view of the currently selected node
       


        #region  Context Menu Commands (Specific to this Node Type; others appear in the Base Class)

        #endregion

        #region  Disable Project

        public string EnableDisable
        {

            get
            {
                //if (!(string.IsNullOrEmpty(Hierarchy.ProjectStatus.Trim())) || (Hierarchy.ProjectStatus == "Open"))
                if (Hierarchy.ProjectStatus.Trim() == "Disabled")
                {
                    return "Collapsed";
                }
                else
                {
                    return "Visible";
                }

            }

        }

        private RelayCommand _DisableProjectCommand;
        public ICommand DisableProjectCommand
        {
            get
            {
                if (_DisableProjectCommand == null)
                {
                    _DisableProjectCommand = new RelayCommand(ExecuteDisableProjectCommand, CanExecuteDisableProjectCommand);
                }
                return _DisableProjectCommand;
            }
        }

        private bool CanExecuteDisableProjectCommand()
        {
            return Domain.IsPermitted("113");
        }

        private void ExecuteDisableProjectCommand()
        {
            try
            {

                Domain.PersistenceLayer.BeginTransWithIsolation(IsolationLevel.Serializable);
                // (1) Check last Update
                string updateCheck = HierarchyBLL.LastUpadateCheck(ref _Hierarchy);
                if (!(String.IsNullOrEmpty(updateCheck)))
                {
                    Domain.PersistenceLayer.AbortTrans();
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(updateCheck), ArgsList);
                    return;
                }
                if (HierarchyBLL.UpdateProjectStatus(this.Hierarchy.Id, "D") == 1)
                {
                    Hierarchy.ProjectStatus = "Disabled";
                    MessageMediator.NotifyColleagues(WorkSpaceId + "UpdateNode", this.Hierarchy); //Register to recieve a message asking for node refresh

                    RaisePropertyChanged("EnableDisable");
                    RaisePropertyChanged("EnableResume");
                    MessageMediator.NotifyColleagues(WorkSpaceId + "OnDisableProjectReceived", this);
                    Domain.PersistenceLayer.CommitTrans();
                }
            }
            catch (Exception e)
            {
                Domain.PersistenceLayer.AbortTrans();
                if (e.Message == "DB Error")
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

        #endregion  Disable Project

        #region  Clone Project

        private RelayCommand _CloneProjectCommand;
        public ICommand CloneProjectCommand
        {
            get
            {
                if (_CloneProjectCommand == null)
                {
                    _CloneProjectCommand = new RelayCommand(ExecuteCloneProjectCommand, CanExecuteCloneProjectCommand);
                }
                return _CloneProjectCommand;
            }
        }

        private bool CanExecuteCloneProjectCommand()
        {
            return Domain.IsPermitted("119");
        }

        public static HierarchyModel HierarchyPR;
        public static TreeViewProjectNodeViewModel TreePR;

        private void ExecuteCloneProjectCommand()
        {
            try
            {
                ProjectDetailsViewModel prop = new ProjectDetailsViewModel(WSId, this);
                ProjectDetailsViewModel.HideField = "Collapsed";

                ProjectDetailsViewModel.ShowField = "Visible";

                ProjectDetailsViewModel.SyncRow = "9";
                ProjectDetailsViewModel.ExpenderRow = "11";
                Hierarchy.IsNew = true;
                Hierarchy.IsCloned = true;

                Hierarchy.VM.Contents.Clear();
                foreach (var i in prop.activeContents)
                {
                    Hierarchy.VM.Contents.Add(i);
                }
                Hierarchy.Certificates.Clear();
                foreach (var j in prop.Certificates)
                {
                    Hierarchy.Certificates.Add(j.key);
                }
                HierarchyPR = new HierarchyModel();
                TreePR = this;
                HierarchyPR = this.Hierarchy;

                //CR 3483
                string defaultVersionName = VersionBLL.GenerateDefaultVersionName(-1);
                if (defaultVersionName != string.Empty)
                {
                    Hierarchy.VM.VersionName = defaultVersionName;
                }
                //else - inherited, no change

                Hierarchy.VM.Description = Hierarchy.VM.VersionName;
                this.Hierarchy.VM.DefaultTargetPathInd = true;
                this.Hierarchy.VM.EcrId = string.Empty;
                //HierarchyModel HierarchyClone = new HierarchyModel();
                //HierarchyClone.IsNew = true;

                // Hierarchy = HierarchyClone;

                //ProjectDetailsViewModel prop = new ProjectDetailsViewModel(WSId, this);
                this.Hierarchy.VM.TargetPath = prop.getTargetPath();

                //string temp = NodeData;
                MessageMediator.NotifyColleagues(WorkSpaceId + "ShowProjectDetails", this);
                ProjectDetailsViewModel.HideField = "Visible";

                ProjectDetailsViewModel.ShowField = "Collapsed";

                ProjectDetailsViewModel.SyncRow = "23";
                //HierarchyBLL.PersistProject(ref HierarchyClone);

                //expand tree on selected parent and focus on selected node
                ProjectDetailsViewModel.ExpandTreeOnFolderTarget(this.Parent);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
            }


        }



        #endregion  Clone Project

        #region  Clone Template

        private RelayCommand _CloneTemplateCommand;
        public ICommand CloneTemplateCommand
        {
            get
            {
                if (_CloneTemplateCommand == null)
                {
                    _CloneTemplateCommand = new RelayCommand(ExecuteCloneTemplateCommand, CanExecuteCloneTemplateCommand);
                }
                return _CloneTemplateCommand;
            }
        }

        private bool CanExecuteCloneTemplateCommand()
        {
            return Domain.IsPermitted("119");
        }

        private void ExecuteCloneTemplateCommand()
        {
            try
            {

                NewTemplateViewModel prop = new NewTemplateViewModel(WSId, this);
                prop.Hierarchy.IsCloned = true;
                prop.Hierarchy.NodeType = NodeTypes.T;
                //NewTemplateViewModel.HideField = "Collapsed";

                //NewTemplateViewModel.ShowField = "Visible";

                //NewTemplateViewModel.SyncRow = "9";
                //NewTemplateViewModel.ExpenderRow = "11";
                Hierarchy.IsNew = true;
                Hierarchy.IsCloned = true;

                Hierarchy.VM.Contents.Clear();
                foreach (var i in prop.activeContents)
                {
                    Hierarchy.VM.Contents.Add(i);
                }
                Collection<object> MessageParameters = new Collection<object>();
                
                //Hierarchy.Certificates.Clear();
                //foreach (var j in prop.Certificates)
                //{
                //    Hierarchy.Certificates.Add(j.key);
                //}
                HierarchyPR = new HierarchyModel();
                TreePR = this;
                HierarchyPR = this.Hierarchy;
                this.Hierarchy.VM.DefaultTargetPathInd = true;
                //HierarchyModel HierarchyClone = new HierarchyModel();
                //HierarchyClone.IsNew = true;

                // Hierarchy = HierarchyClone;

                //ProjectDetailsViewModel prop = new ProjectDetailsViewModel(WSId, this);
                this.Hierarchy.VM.TargetPath = prop.getTargetPath();

                //string temp = NodeData;
                MessageMediator.NotifyColleagues(WorkSpaceId + "ShowTemplateDetails", this);
                NewTemplateViewModel.HideField = "Visible";

                NewTemplateViewModel.ShowField = "Collapsed";

                NewTemplateViewModel.SyncRow = "23";
                //HierarchyBLL.PersistProject(ref HierarchyClone);

                //expand tree on selected parent and focus on selected node
                NewTemplateViewModel.ExpandTreeOnFolderTarget(this.Parent);
                //MessageMediator.NotifyColleagues(WorkSpaceId + "OnShowTemplateCloneDetailsReceived", this.Parent);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
            }


        }



        #endregion  Clone Template

        #region Collapse Versions

        private RelayCommand _CollapseVersionsCommand;
        public ICommand CollapseVersionsCommand
        {
            get
            {
                if (_CollapseVersionsCommand == null)
                {
                    _CollapseVersionsCommand = new RelayCommand(ExecuteCollapseVersionsCommand, CanExecuteCollapseVersionsCommand);
                }
                return _CollapseVersionsCommand;
            }
        }

        private bool CanExecuteCollapseVersionsCommand()
        {
            //if there is childrens to this node. --> children exists only if show version applied.
            if (this.Children.Count > 0)
            {
                return Domain.IsPermitted("119");
            }
            else
                return false;
        }

        private void ExecuteCollapseVersionsCommand()
        {
            Children = new ObservableCollection<TreeViewNodeViewModelBase>();

        }

        #endregion

        #region  Show Versions

        private RelayCommand _ShowVersionsCommand;
        public ICommand ShowVersionsCommand
        {
            get
            {
                if (_ShowVersionsCommand == null)
                {
                    _ShowVersionsCommand = new RelayCommand(ExecuteShowVersionsCommand, CanExecuteShowVersionsCommand);
                }
                return _ShowVersionsCommand;
            }
        }

        private bool CanExecuteShowVersionsCommand()
        {

            return Domain.IsPermitted("119");
        }

        private void ExecuteShowVersionsCommand()
        {
            try
            {
                this.Children.Clear();
                //Get All Versions for project
                ObservableCollection<VersionModel> allVersions;
                if (Hierarchy.GroupId == -1)
                    allVersions = VersionBLL.GetAllVersions(Hierarchy.Id); //Case Project is regular
                else
                    allVersions = VersionBLL.GetAllVersions(Hierarchy.GroupId); //Case Project is related
                long Hid = Hierarchy.Id;

                HierarchyModel hm = Hierarchy;
                VersionModel vCur = Hierarchy.VM;

                //Add versions to the hierarchy tree
                foreach (VersionModel V in allVersions)
                {
                    hm.VM = V;
                    TreeViewVersionNodeViewModel toAdd = new TreeViewVersionNodeViewModel(WSId, hm);
                    if (toAdd.Hierarchy.VM.VersionStatus == "A")
                    {
                        toAdd.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "Version"), UriKind.RelativeOrAbsolute));
                    }
                    else
                    {
                        toAdd.Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "VersionDis"), UriKind.RelativeOrAbsolute));
                    }

                    if (Children.Count < allVersions.Count) //For disable duplicate add of version
                        Children.Add(toAdd);
                }
                Hierarchy.VM = vCur;
            }
            catch (Exception e)
            {
                Object[] ArgsList = new Object[] { 0 };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
                return;
            }


        }

        #endregion  Show Versions

        #region  Resume Project

        public string EnableResume
        {

            get
            {
                if (Hierarchy.ProjectStatus.Trim() == "Open")
                {
                    return "Collapsed";
                }
                else
                {
                    return "Visible";
                }

            }
        }



        private RelayCommand _ResumeProjectCommand;
        public ICommand ResumeProjectCommand
        {
            get
            {
                if (_ResumeProjectCommand == null)
                {
                    _ResumeProjectCommand = new RelayCommand(ExecuteResumeProjectCommand, CanExecuteResumeProjectCommand);
                }
                return _ResumeProjectCommand;
            }
        }

        private bool CanExecuteResumeProjectCommand()
        {
            return Domain.IsPermitted("115");
        }

        private void ExecuteResumeProjectCommand()
        {
            try
            {
                Domain.PersistenceLayer.BeginTransWithIsolation(IsolationLevel.Serializable);
                // (1) Check last Update
                string updateCheck = HierarchyBLL.LastUpadateCheck(ref _Hierarchy);
                if (!(String.IsNullOrEmpty(updateCheck)))
                {
                    Domain.PersistenceLayer.AbortTrans();
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(updateCheck), ArgsList);
                    return;
                }
                if (HierarchyBLL.UpdateProjectStatus(this.Hierarchy.Id, "O") == 1)
                {
                    Hierarchy.ProjectStatus = "Open";
                    MessageMediator.NotifyColleagues(WorkSpaceId + "UpdateNode", this.Hierarchy); //Register to recieve a message asking for node refresh
                    RaisePropertyChanged("EnableResume");
                    RaisePropertyChanged("EnableDisable");
                    
                    Domain.PersistenceLayer.CommitTrans();
                }
            }
            catch (Exception e)
            {
                Domain.PersistenceLayer.AbortTrans();
                if (e.Message == "DB Error")
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
        #endregion  Resume Project

        #region  Clone Related Project

        public string EnableClone
        {

            get
            {

                if (Hierarchy.GroupId != -1)
                {
                    return "Collapsed";
                }
                else
                {
                    return "Visible";
                }

            }

        }


        private RelayCommand _CloneRelatedProjectCommand;
        public ICommand CloneRelatedProjectCommand
        {
            get
            {
                if (_CloneRelatedProjectCommand == null)
                {
                    _CloneRelatedProjectCommand = new RelayCommand(ExecuteCloneRelatedProjectCommand, CanExecuteCloneRelatedProjectCommand);
                }
                return _CloneRelatedProjectCommand;
            }
        }

        private bool CanExecuteCloneRelatedProjectCommand()
        {
            return Domain.IsPermitted("120");
        }


        private void ExecuteCloneRelatedProjectCommand()
        {
            try
            {
                
                // MessageMediator.NotifyColleagues(WorkSpaceId + "ShowProjectDetails", this);

                ProjectDetailsViewModel prop = new ProjectDetailsViewModel(WSId, this);
                prop.enableStep = true;
                Hierarchy.IsClonedRelated = true;
                Hierarchy.IsNew = true;
                //Hierarchy.IsDirty = true;
                ProjectDetailsViewModel.HideField = "Visible";
                ProjectDetailsViewModel.ShowField = "Visible";
                ProjectDetailsViewModel.ExpenderRow = "25";
                ProjectDetailsViewModel.LockVersion = true;
                ProjectDetailsViewModel.LockCheck = false;
                Hierarchy.VM.Contents.Clear();
                foreach (var i in prop.activeContents)
                {
                    Hierarchy.VM.Contents.Add(i);
                }
                Hierarchy.Certificates.Clear();
                foreach (var j in prop.Certificates)
                {
                    Hierarchy.Certificates.Add(j.key);
                }


                this.Hierarchy = HierarchyBLL.GetGroupIDFromHierarchy(Hierarchy);
                //Hierarchy.IsNew = true;

                HierarchyPR = this.Hierarchy;
                TreePR = this;
                HierarchyPR = this.Hierarchy;
                //If not exists group Id the field is editable.
                if (Hierarchy.GroupId == -1)
                {
                    ProjectDetailsViewModel.ReadGroup = false;
                }

                MessageMediator.NotifyColleagues(WorkSpaceId + "ShowProjectDetails", this);
                Hierarchy.IsDirty = true;
                ProjectDetailsViewModel.ShowField = "Collapsed";
                ProjectDetailsViewModel.LockVersion = false;
                ProjectDetailsViewModel.LockCheck = true;

                ProjectDetailsViewModel.ReadGroup = true;

                //expand tree on selected parent and focus on selected node
                ProjectDetailsViewModel.ExpandTreeOnFolderTarget(this.Parent);

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
            }


        }



        #endregion  Clone Project

        #region All Related Projects


        public string EnableRelated
        {

            get
            {

                if (Hierarchy.GroupId == -1)
                {
                    return "Collapsed";
                }
                else
                {
                    return "Visible";
                }

            }

        }


        private RelayCommand _UpdateRelatedProjectCommand;
        public ICommand UpdateRelatedProjectCommand
        {
            get
            {
                if (_UpdateRelatedProjectCommand == null)
                {
                    _UpdateRelatedProjectCommand = new RelayCommand(ExecuteUpdateRelatedProjectCommand, CanExecuteUpdateRelatedProjectCommand);
                }
                return _UpdateRelatedProjectCommand;
            }
        }

        private bool CanExecuteUpdateRelatedProjectCommand()
        {
            return Domain.IsPermitted("120");
        }


        private void ExecuteUpdateRelatedProjectCommand()
        {
            try
            {
                // MessageMediator.NotifyColleagues(WorkSpaceId + "ShowProjectDetails", this);

                ProjectDetailsViewModel prop = new ProjectDetailsViewModel(WSId, this);
                //Hierarchy.IsDirty = true;
                //Hierarchy.IsClonedRelated = true;
                ProjectDetailsViewModel.LockVersion = false;
                ProjectDetailsViewModel.LockCheck = true;
                ProjectDetailsViewModel.LockName = true;
                ProjectDetailsViewModel.LockSync = false;
                Hierarchy.IsClonedRelatedUpdate = true;
                Hierarchy.VM.Contents.Clear();
                foreach (var i in prop.activeContents)
                {
                    Hierarchy.VM.Contents.Add(i);
                }
                Hierarchy.Certificates.Clear();
                foreach (var j in prop.Certificates)
                {
                    Hierarchy.Certificates.Add(j.key);
                }
                //CR 3483
                string defaultVersionName = VersionBLL.GenerateDefaultVersionName((int)Hierarchy.GroupId);
                if (defaultVersionName != string.Empty)
                {
                    Hierarchy.VM.VersionName = defaultVersionName;
                }
                //else - inherited, no change
                Hierarchy.VM.Description = Hierarchy.VM.VersionName;
                MessageMediator.NotifyColleagues(WorkSpaceId + "ShowProjectDetails", this);
                Hierarchy.IsClonedRelated = false;
                ProjectDetailsViewModel.LockName = false;
                ProjectDetailsViewModel.LockSync = true;
                Hierarchy.IsClonedRelatedUpdate = true;
                Hierarchy.IsDirty = true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
            }


        }


        #endregion  All Related Projects

        #region Split Related Projects

        private RelayCommand _SplitRelatedProjectCommand;
        public ICommand SplitRelatedProjectCommand
        {
            get
            {
                if (_SplitRelatedProjectCommand == null)
                {
                    _SplitRelatedProjectCommand = new RelayCommand(ExecuteSplitRelatedProjectCommand, CanExecuteSplitRelatedProjectCommand);
                }
                return _SplitRelatedProjectCommand;
            }
        }

        private bool CanExecuteSplitRelatedProjectCommand()
        {
            return Domain.IsPermitted("120");
        }


        private void ExecuteSplitRelatedProjectCommand()
        {
            try
            {
                // MessageMediator.NotifyColleagues(WorkSpaceId + "ShowProjectDetails", this);

                ProjectDetailsViewModel prop = new ProjectDetailsViewModel(WSId, this);

                //Hierarchy.IsClonedRelated = true;
                ProjectDetailsViewModel.LockVersion = false;
                ProjectDetailsViewModel.LockCheck = true;
                ProjectDetailsViewModel.LockName = true;
                ProjectDetailsViewModel.LockSync = false;
               Hierarchy.IsClonedRelatedUpdate = true;
                Hierarchy.IsClonedRelatedSplit = true;
                Hierarchy.VM.Contents.Clear();
                foreach (var i in prop.activeContents)
                {
                    Hierarchy.VM.Contents.Add(i);
                }
                Hierarchy.Certificates.Clear();
                //CR 3483
                //string defaultVersionName = VersionBLL.GenerateDefaultVersionName((int)Hierarchy.GroupId);
                //if (defaultVersionName != string.Empty)
                //{
                //    Hierarchy.VM.VersionName = defaultVersionName;
                //}
                ////else - inherited, no change
                //Hierarchy.VM.Description = Hierarchy.VM.VersionName;
                //foreach (var j in prop.Certificates)
                //{
                //    Hierarchy.Certificates.Add(j.key);
                //}

               // Hierarchy.IsDirty = true;
                MessageMediator.NotifyColleagues(WorkSpaceId + "ShowProjectDetails", this);
                Hierarchy.IsDirty = true;
                Hierarchy.IsClonedRelated = false;
                ProjectDetailsViewModel.LockName = false;
                ProjectDetailsViewModel.LockSync = true;
                Hierarchy.IsClonedRelatedUpdate = false;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
            }


        }







        private void OnSplitDetailsReceived(HierarchyModel HM)
        {
            RaisePropertyChanged("EnableRelated");
            RaisePropertyChanged("EnableClone");
        }








        #endregion  All Related Projects

        #region  Rapid Execution

        #region Command

        private RelayCommand _RapidExecutionCommand;
        public ICommand RapidExecutionCommand
        {
            get
            {
                if (_RapidExecutionCommand == null)
                {
                    _RapidExecutionCommand = new RelayCommand(ExecuteRapidExecutionCommand, CanExecuteRapidExecutionCommand);
                }
                return _RapidExecutionCommand;
            }
        }
        private bool CanExecuteRapidExecutionCommand()
        {
            if (Domain.IsPermitted("130"))
            {
                if (!Hierarchy.IsNew && !Hierarchy.VM.IsNew && !Hierarchy.IsCloned && !Hierarchy.IsClonedRelated && !Hierarchy.IsClonedRelatedSplit && !Hierarchy.IsClonedRelatedUpdate)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        private void ExecuteRapidExecutionCommand()
        {
            try
            {
                if (!CanExecuteRapidExecutionCommand())
                    return;
                MessageMediator.NotifyColleagues(WorkSpaceId + "OnRapidExecutionStartReceived", this);

                ContentExecutionBLL.ErrorHandling luStatus = new ContentExecutionBLL.ErrorHandling();
                //Last Update and permission check
                luStatus = ContentExecutionBLL.PriorValidations(_Hierarchy, "130");
                if (!(String.IsNullOrEmpty(luStatus.errorId)))
                {
                    MessageMediator.NotifyColleagues(this.WorkSpaceId + "OnIsCanceledNodeReceived", this);
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(luStatus.errorId), luStatus.errorParams);
                    return;
                }

                ContentBLL contentBLL;
                //Check project contents associated to active version
                //If group related
                if(Hierarchy.GroupId == -1)
                    contentBLL = new ContentBLL(Hierarchy.Id);
                else
                    contentBLL = new ContentBLL(Hierarchy.GroupId);
                activeVersionCMs = contentBLL.getActiveContents(Hierarchy.ActiveVersion);

                //If no contents are associated to active project version - error 150 
                if (activeVersionCMs.Count == 0)
                {
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(150, new object[] { 0 });
                    return;
                }

                //If exist - contentToAction is contentVersion with sequence = 1;
                //contentToAction = activeVersionCMs.Where(x => x.seq == 1).FirstOrDefault();
                int minContentSeqNo = activeVersionCMs.Min(x => x.seq);
                contentToAction = activeVersionCMs.Where(x => x.seq == minContentSeqNo).FirstOrDefault();

                //If there is no content with sequence 1 – issue error 151 
                if (contentToAction.Equals(null))
                {
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(151, new object[] { 0 });
                    return;
                }


                //Execute in the same manner as regular execution
                Thread ContentExecutionThread = new Thread(new ThreadStart(ContentExecution));
                ContentExecutionThread.Start();

            }
            catch (Exception e)
            {
                //If there is no content with sequence 1 – issue error 151 
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                ProjectsExplorerViewModel.ShowErrorAndInfoMessage(151, new object[] { 0 });
                return;
            }
        }

        #endregion Command

        #region Data
        private ContentModel contentToAction;
        ObservableCollection<ContentModel> activeVersionCMs;
        private String targetPath;
        #endregion Data

        #region Main content excecute command

        private void ContentExecution()
        {
            int errorId = -1;
            ContentExecutionBLL.ErrorHandling Status = new ContentExecutionBLL.ErrorHandling();

            try
            {
                #region Get relevant content sub tree
                //Get contents, folder and version tree from API
                Dictionary<int, CMFolderModel> outFolders = new Dictionary<int, CMFolderModel>();
                Dictionary<int, CMContentModel> outContents = new Dictionary<int, CMContentModel>();
                Dictionary<int, CMVersionModel> outVersions = new Dictionary<int, CMVersionModel>();

                //Performance
                //Get CM sub tree
                Status = ContentExecutionBLL.GetCMSubTree(activeVersionCMs, out outFolders, out outContents, out outVersions);
                #region Handle Get CM sub tree errors if any
                if (Status.errorId != string.Empty)
                {
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Status.errorId), Status.errorParams);
                return;
            }
                #endregion

                #endregion Get CM sub tree

                #region Perform all required validations prior to execution
                Status = ContentExecutionBLL.validations(outContents, outVersions, contentToAction, Hierarchy);
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

                Status = ContentExecutionBLL.userCertificatesValidations(Hierarchy);
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

                #region Copy content files

                //Start new thread for copying files and copy files to local directory
                ThreadStart threadStart = new ThreadStart(IncrementProgressCounter);
                Thread t = new Thread(threadStart);
                t.Start();
                HierarchyModel updatedHierarchy = Hierarchy;
                updatedHierarchy.VM.Contents = activeVersionCMs;
                Status = ContentExecutionBLL.prepareLocalDirectoryToExecution(outVersions, outContents, threadStart,
                                                                                 activeVersionCMs,
                                                                                 contentToAction,
                                                                                 updatedHierarchy);
                #region Handle files copy errors if any
                if (Status.errorId != string.Empty)
                {
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Status.errorId), Status.errorParams);
                    return;
        }

        #endregion

                #endregion Copy files

                #region Run command line

                //Execute file
                IncrementProgressCounter(10);
                Status = ContentExecutionBLL.ExecuteContentVersionAndSaveInfo(updatedHierarchy, outVersions, outContents, contentToAction.id);
                #region Handle execution errors if any
                if (Status.errorId != string.Empty)
                {
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Status.errorId), Status.errorParams);
                    ProgressText = "0 %";
                    MessageMediator.NotifyColleagues(WorkSpaceId + "OnProgressTextReceived", this);
                    return;
                }
                #endregion

                #endregion Execute

                #region Save history

                //Record Execute History
                Status = ContentExecutionBLL.recordExecutionHistory(Hierarchy.VM.VersionId);
                #region Handle save history errors if any

                if (Status.errorId != string.Empty)
                {
                    String logMessage = "Failed to save record in ExecutionHistory table";
                    Domain.SaveGeneralErrorLog(logMessage);
                }
                #endregion

                #endregion save history
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, new object[] { 0 });
            }
        }

        #endregion

        #region Progress Bar
        #region Fields

        // Property variables
        private float p_Progress = 0;
        private int p_ProgressMax = 10;
        private String p_ProgressText = "";
        private float totalProgress = 0f;
        #endregion

        #region Data Properties

        // The progress of an image processing job.
        // The setter for this property also sets the ProgressMessage property.
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
            MessageMediator.NotifyColleagues(WorkSpaceId + "OnClearViewModelReceived", this);
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

            MessageMediator.NotifyColleagues(WorkSpaceId + "OnProgressTextReceived", this);

            // Update progress message
            float progress = (p_Progress);
            float progressMax = (p_ProgressMax);
            float f = (progress / progressMax);
            float percentComplete = Single.IsNaN(f) ? 0 : (f);
        }

        public void IncrementProgressCounter()
        {
            MessageMediator.NotifyColleagues(WorkSpaceId + "OnRapidExecutionReceived", this);
        }
        #endregion

        #endregion

        #endregion  Rapid Execution

        #region Export

        #region export to FS
        public string EnableExport
        {
            get
            {
                if (Hierarchy.GroupId != -1 || Hierarchy.NodeType != NodeTypes.P)
                {
                    return "Collapsed";
                }
                else
                {
                    return "Visible";
                }

            }

        }

        private RelayCommand _ExportProjectArchive;
        public ICommand ExportProjectArchive
        {
            get
            {
                if (_ExportProjectArchive == null)
                {
                    _ExportProjectArchive = new RelayCommand(ExecuteExportProjectArchive, CanExecuteExportProjectArchive);
                }
                return _ExportProjectArchive;
            }
        }

        private bool CanExecuteExportProjectArchive()
        {
            return true;
        }

        string rootExportFolderFullPath = string.Empty;
        string exportPackageName = string.Empty;

        private void ExecuteExportProjectArchive()
        {
            try
            {
                if (!Domain.IsPermitted("152"))
                {
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(106, ArgsList);
                    return;
                }
                //Validate last update date
                string updateCheck = HierarchyBLL.LastUpadateCheck(ref _Hierarchy);
                if (!(String.IsNullOrEmpty(updateCheck)))
                {
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(updateCheck), ArgsList);
                    return;
                }

                //Select target folder
                string archiveFileName = string.Empty;
                using (FolderBrowserDialog dlg = new FolderBrowserDialog())
                {
                    dlg.RootFolder = Environment.SpecialFolder.Desktop;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        string messageText = "Project " + this.Hierarchy.Name + " will be saved under " + dlg.SelectedPath +
                            ".\nPackage folder name is " + this.Hierarchy.Name + "_" + this.Hierarchy.VM.VersionName + "_<TimeStamp>";
                        if (MsgBoxService.ShowOkCancel(messageText, DialogIcons.Information) == DialogResults.OK)
                        {
                            rootExportFolderFullPath = dlg.SelectedPath;
                            exportPackageName = this.Hierarchy.Name + "_" + this.Hierarchy.Id;
                            Thread ExportProjectThread = new Thread(new ThreadStart(ProceedExportProject));
                            ExportProjectThread.Start();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Object[] ArgsList = new Object[] { 0 };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
            }
        }

        private void ProceedExportProject()
        {
            Domain.ErrorHandling result = new Domain.ErrorHandling();
 
            try
            {
                //Timestamp to prevent ovewriting 
                DateTime NowTime = System.DateTime.Now;
                string StartTime = string.Format("{0:yyyyMMddhhmmss}", NowTime);
                exportPackageName = exportPackageName + "_" + StartTime;

                ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32("223"), new object[] {0});

                result = ExportProjectBLL.ExportProject(Hierarchy, rootExportFolderFullPath, exportPackageName);
                if (result.messsageId != string.Empty)
                {
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(result.messsageId), result.messageParams);
                }
                else
                {
                    result.messsageId = "222";
                    result.messageParams[0] = exportPackageName + ".project";
                    result.messageParams[1] = rootExportFolderFullPath;
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(result.messsageId), result.messageParams);
                }
            }
            catch (Exception e)
            {
                Object[] ArgsList = new Object[] { 0 };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
            }

        }

        #endregion export to FS

        #region export to Environment

        public string EnableExportToEnv
        {
            get
            {
                if (Hierarchy.GroupId != -1 || Hierarchy.NodeType != NodeTypes.P)
                {
                    return "Collapsed";
                }
                else
                {
                    return "Visible";
                }

            }

        }

        private RelayCommand _ExportProjectToEnv;
        public ICommand ExportProjectToEnv
        {
            get
            {
                if (_ExportProjectToEnv == null)
                {
                    _ExportProjectToEnv = new RelayCommand(ExecuteExportProjectToEnv, CanExecuteExportProjectToEnv);
                }
                return _ExportProjectToEnv;
            }
        }

        private bool CanExecuteExportProjectToEnv()
        {
            return true;
        }

        private void ExecuteExportProjectToEnv()
        {
            Domain.ErrorHandling Status = new Domain.ErrorHandling();
            try
            {
                //Validate last update date
                string updateCheck = HierarchyBLL.LastUpadateCheck(ref _Hierarchy);
                if (!(String.IsNullOrEmpty(updateCheck)))
                {
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(updateCheck), ArgsList);
                    return;
                }

                //CR 4231 - export is not allowed if step/code is not populated
                if (string.IsNullOrEmpty(Hierarchy.SelectedStep) || string.IsNullOrWhiteSpace(Hierarchy.SelectedStep))
                {
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(243, ArgsList);
                    return;
                }

                ObservableCollection<UserEnvironmentsModel> userEnvironments = new ObservableCollection<UserEnvironmentsModel>();
                Status = ExportProjectToEnvBLL.GetAvailableUserEnvironments(out userEnvironments);

                if (userEnvironments != null && userEnvironments.Count > 0)
                {
                    object[] parameters = new object[2];

                    parameters[0] = this;
                    parameters[1] = userEnvironments;
                    MessageMediator.NotifyColleagues(this.WorkSpaceId + "ImportProjectToEnvironment", parameters);
                }
                else
                {
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(237, ArgsList);
                    return;
                }
            }
            catch (Exception e)
            {
                Object[] ArgsList = new Object[] { 0 };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
            }
        }
        #endregion export to Environment
        #endregion

    }

} //end of root namespace