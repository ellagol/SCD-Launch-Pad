using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using ATSBusinessLogic;
using ATSBusinessObjects;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using Infra.MVVM;

namespace ExplorerModule
{
    public class TreeViewTemplateVersionNodeViewModel : TreeViewNodeViewModelBase
    {
        public TreeViewTemplateVersionNodeViewModel(Guid workspaceId, HierarchyModel Hierarchy)
            : this(workspaceId, Hierarchy, null)
        {
            this.Description = Hierarchy.VM.Description;
            this.Name = Hierarchy.VM.VersionName;
            this.Id = Hierarchy.VM.VersionId;
            this.WorkSpaceId = workspaceId;
            
            //The messageMediator is registered in the ViewModelBase - Generally you have 1 mediator; Hence, the restricted access to the constructor
            MessageMediator = GetService<MessengerService>();
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();

        }

        public TreeViewTemplateVersionNodeViewModel(Guid workspaceId, HierarchyModel Hierarchy, TreeViewNodeViewModelBase ParentNode)
            : base(workspaceId, Hierarchy, ParentNode)
        {
            this.Name = Hierarchy.VM.VersionName;
            this.WorkSpaceId = workspaceId;
            //The messageMediator is registered in the ViewModelBase - Generally you have 1 mediator; Hence, the restricted access to the constructor
            MessageMediator = GetService<MessengerService>();
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            if (this.Hierarchy.GroupId == -1)
            {
                activeVersion = ATSBusinessLogic.VersionBLL.GetVersionRow(this.Hierarchy.Id);
            }
            else
                activeVersion = ATSBusinessLogic.VersionBLL.GetVersionRow(this.Hierarchy.GroupId);

        }

        public override string NodeData
        {
            get
            {
                return this.Name; //Here you can place any content you want to see in the Tree for this node...
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
            MessageMediator.NotifyColleagues(WorkSpaceId + "ShowTemplateVersionDetails", this); //Will be returned to the Explorer Main signed for this message
        }
        //Sends a message to the MainWindow with the required information to save a details view of the currently selected node
      
        //private string VersionStatus = " ";
        private IMessageBoxService MsgBoxService = null;

        #region  Context Menu Commands

        #region  ReOpen Version Command

        #region ReOpenCommand
        private RelayCommand _ReOpenCommand;
        public ICommand ReOpenCommand
        {
            get
            {
                if (_ReOpenCommand == null)
                {
                    _ReOpenCommand = new RelayCommand(ExecuteReOpenCommand, CanExecuteReOpenCommand);
                }
                return _ReOpenCommand;
            }
        }

        VersionModel closedVersion;
       private static VersionModel activeVersion;

        private bool CanExecuteReOpenCommand()
        {
            try
            {
                //To enable or disable reopen, first we will check if version is closed.

                if (this.Id == activeVersion.VersionId)
                {
                    return false;
                    //if (closedVersion.VersionStatus.Equals("A")) //If version closed enable reopen
                    //    return false;
                    //else return true;
                }
                else
                    return true;

            }
            catch (Exception ex)
            {
                //Case exeption disable reopen
                return false;
            }
        }

        private void ExecuteReOpenCommand()
        {
            try
            {
                Domain.PersistenceLayer.BeginTransWithIsolation(IsolationLevel.Serializable);
                if (this.Hierarchy.GroupId == -1)
                {
                    activeVersion = ATSBusinessLogic.VersionBLL.GetVersionRow(this.Hierarchy.Id);
                }
                else
                    activeVersion = ATSBusinessLogic.VersionBLL.GetVersionRow(this.Hierarchy.GroupId);
                string updateCheck = HierarchyBLL.LastUpadateCheck(ref _Hierarchy);
                if (!(String.IsNullOrEmpty(updateCheck)))
                {
                    Domain.PersistenceLayer.AbortTrans();
                    MessageMediator.NotifyColleagues(this.WorkSpaceId + "OnIsCanceledNodeReceived", this);
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(updateCheck), ArgsList);
                    return;
                }

                //(2) Check active version.
                string updateVersionCheck = VersionBLL.LastUpadateReOpenVersionCheck(ref activeVersion);
                if (!(String.IsNullOrEmpty(updateVersionCheck)))
                {
                    Domain.PersistenceLayer.AbortTrans();
                    MessageMediator.NotifyColleagues(this.WorkSpaceId + "OnIsCanceledNodeReceived", this);
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(updateVersionCheck), ArgsList);
                    return;
                }

                //(3) Check close version.
                if (Hierarchy.GroupId == -1)
                {
                    closedVersion = ATSBusinessLogic.VersionBLL.GetVersionByIdAndHierarchyId(this.Id, this.Hierarchy.Id);
                }
                else
                    closedVersion = ATSBusinessLogic.VersionBLL.GetVersionByIdAndHierarchyId(this.Id, this.Hierarchy.GroupId);
                updateVersionCheck = VersionBLL.LastUpadateReOpenVersionCheck(ref closedVersion);
                if (!(String.IsNullOrEmpty(updateVersionCheck)))
                {
                    Domain.PersistenceLayer.AbortTrans();
                    MessageMediator.NotifyColleagues(this.WorkSpaceId + "OnIsCanceledNodeReceived", this);
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(updateVersionCheck), ArgsList);
                    return;
                }

            
                bool ValidateResult = validate();
                if (ValidateResult)
                    Domain.PersistenceLayer.CommitTrans();
                else
                    Domain.PersistenceLayer.AbortTrans();
            }
            catch (Exception E)
            {
                Domain.PersistenceLayer.AbortTrans();
                MessageMediator.NotifyColleagues(this.WorkSpaceId + "OnIsCanceledNodeReceived", this);
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
        #endregion

        #region Validate user selection

        private bool validate()
        {
            try
            {
                if (showMessage() == DialogResults.OK)
                {
                    bool OpenResult = ATSBusinessLogic.VersionBLL.reOpenVersion(activeVersion.VersionId, closedVersion.VersionId);
                    if (OpenResult)
                    {
                        MessageMediator.NotifyColleagues(this.WorkSpaceId + "OnUpdateTemplateVersionReceived", this);
                        if (Hierarchy.GroupId == -1)
                        {
                            activeVersion = ATSBusinessLogic.VersionBLL.GetVersionRow(this.Hierarchy.Id);
                        }
                        else
                            activeVersion = ATSBusinessLogic.VersionBLL.GetVersionRow(this.Hierarchy.GroupId);
                        //closedVersion = ATSBusinessLogic.VersionBLL.GetVersionByIdAndHierarchyId(activeVersion.VersionId, this.Hierarchy.Id);
                        return true;
                    }
                    else
                    {
                        //Show Faile Message
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(141, ArgsList);
                        return false;                   
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                ATSDomain.Domain.SaveGeneralErrorLog(logMessage);
                throw new Exception("DB Error");
                return false;
            }
        }
        #endregion

        #region Show Are you sure message

        private DialogResults showMessage()
        {
            IMessageBoxService MsgBoxService = GetService<IMessageBoxService>();
           // activeVersion = ATSBusinessLogic.VersionBLL.GetVersionRow(this.Hierarchy.Id);
            var SB = new StringBuilder(string.Empty);
            SB.Append("SELECT Description FROM PE_Messages where id=112");
            String message = (Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString();
            Object[] ArgsList = new Object[] { activeVersion.VersionName, closedVersion.VersionName };


            //showMessage(112);
            return MsgBoxService.ShowOkCancel(SetDescriptionParameters(message, ArgsList, 112), DialogIcons.Question);
        }

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
        #endregion

        #endregion

        #region  Rapid Execution

        private RelayCommand _RapidExecutionVersionCommand;
        public ICommand RapidExecutionVersionCommand
        {
            get
            {
                if (_RapidExecutionVersionCommand == null)
                {
                    _RapidExecutionVersionCommand = new RelayCommand(ExecuteRapidExecutionVersionCommand, CanExecuteRapidExecutionVersionCommand);
                }
                return _RapidExecutionVersionCommand;
            }
        }
        private bool CanExecuteRapidExecutionVersionCommand()
        {
            if (Domain.IsPermitted("116"))
            {
                if (!Hierarchy.IsNew && !Hierarchy.VM.IsNew && !Hierarchy.IsCloned && !Hierarchy.IsClonedRelated && !Hierarchy.IsClonedRelatedSplit && !Hierarchy.IsClonedRelatedUpdate)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        VersionModel vm;
        private void ExecuteRapidExecutionVersionCommand()
        {
            try
            {
                MessageMediator.NotifyColleagues(WorkSpaceId + "OnTemplateVersionRapidExecutionVersionStartReceived", this);

                ContentExecutionBLL.ErrorHandling luStatus = new ContentExecutionBLL.ErrorHandling();
                //Last Update and permission check
                luStatus = ContentExecutionBLL.PriorValidations(_Hierarchy, "130");
                if (!(String.IsNullOrEmpty(luStatus.errorId)))
                {
                    MessageMediator.NotifyColleagues(this.WorkSpaceId + "OnIsCanceledNodeReceived", this);
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(luStatus.errorId), luStatus.errorParams);
                    return;
                }
                //Check project contents associated to activated version
                if (Hierarchy.GroupId < 1)
                {
                    ContentBLL contentBLL = new ContentBLL(Hierarchy.Id);
                    vm = ATSBusinessLogic.VersionBLL.GetVersionByIdAndHierarchyId(this.Id, this.Hierarchy.Id);
                    activeVersionCMs = contentBLL.getActiveContents(vm.VersionName);
                }
                else
                {
                    ContentBLL contentBLL = new ContentBLL(Hierarchy.GroupId);
                    vm = ATSBusinessLogic.VersionBLL.GetVersionByIdAndHierarchyId(this.Id, this.Hierarchy.GroupId);
                    activeVersionCMs = contentBLL.getActiveContents(vm.VersionName);
                }

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
                ProjectsExplorerViewModel.ShowErrorAndInfoMessage(151, new object[] { 0 });
                String logMessage = e.Message + "\n" + "Source: " + e.Source + "\n" + e.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return;
            }
        }

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
                #region Get CM Sub tree
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

                #endregion

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

                #region Start new thread for copying files and copy files to local directory
            ThreadStart threadStart = new ThreadStart(IncrementProgressCounter);
            Thread t = new Thread(threadStart);
            t.Start();
                Status = ContentExecutionBLL.prepareLocalDirectoryToExecution(outVersions, outContents, threadStart,
                                                                                 activeVersionCMs,
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
                HierarchyModel updatedHierarchy = Hierarchy;
                updatedHierarchy.VM = this.vm;
                updatedHierarchy.VM.Contents = activeVersionCMs;
                Status = ContentExecutionBLL.ExecuteContentVersionAndSaveInfo(updatedHierarchy, outVersions, outContents, contentToAction.id);
                #region Handle execution errors if any
                if (Status.errorId != string.Empty)
                {
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Status.errorId), Status.errorParams);
                    ProgressText = "0 %";
                    MessageMediator.NotifyColleagues(WorkSpaceId + "OnTemplateVersionProgressTextReceived", this);
                    return;
                }
                #endregion

                #endregion

                #region     Save Execution History
                Status = ContentExecutionBLL.recordExecutionHistory(Hierarchy.VM.VersionId);
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
            MessageMediator.NotifyColleagues(WorkSpaceId + "OnClearTemplateVersionViewModelReceived", this);
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

            MessageMediator.NotifyColleagues(WorkSpaceId + "OnTemplateVersionProgressTextReceived", this);

            // Update progress message
            float progress = (p_Progress);
            float progressMax = (p_ProgressMax);
            float f = (progress / progressMax);
            float percentComplete = Single.IsNaN(f) ? 0 : (f);
        }

        public void IncrementProgressCounter()
        {
            MessageMediator.NotifyColleagues(WorkSpaceId + "OnTemplateVersionRapidExecutionVersionReceived", this);
        }
        #endregion



        #endregion

        #endregion  Rapid Execution
        
        #endregion

    }

} //end of root namespace