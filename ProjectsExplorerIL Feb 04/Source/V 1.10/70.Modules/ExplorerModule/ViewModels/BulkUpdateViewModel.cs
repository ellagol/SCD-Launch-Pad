﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using ATSBusinessLogic;
using ATSBusinessObjects;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using Infra.DragDrop;
using Infra.MVVM;



namespace ExplorerModule
{
    public class BulkUpdateViewModel : ViewModelBase, IDropTarget
	{

        #region  Data

        protected MessengerService MessageMediator = new MessengerService();
        private IMessageBoxService MsgBoxService = null;


        private Guid WorkspaceId;

        //private HierarchyModel _Hierarchy;
        //public HierarchyModel Hierarchy
        //{
        //    get
        //    {
        //        return _Hierarchy;
        //    }
        //    set
        //    {
        //        _Hierarchy = value;
        //    }
        //}


        #endregion

        #region Presentation Properties

 
        private TreeViewNodeViewModelBase Node;
        
        //All Project Collection
        private ObservableCollection<HierarchyModel> _ProjectsFamily = new ObservableCollection<HierarchyModel>();
        public ObservableCollection<HierarchyModel> ProjectsFamily
        {
            get
            {
                return _ProjectsFamily;
            }
            set
            {
                _ProjectsFamily = value;
                RaisePropertyChanged("ProjectsFamily");
            }

        }
        
        
        private bool _IsProjectListEnabled = true;
        public bool IsProjectListEnabled
        {
            get
            {
                return _IsProjectListEnabled;
            }
            set
            {
                _IsProjectListEnabled = value;
                RaisePropertyChanged("IsProjectListEnabled");
            }
        }
        //From list
        private ObservableCollection<ContentModel> _FindList = new ObservableCollection<ContentModel>();
        public ObservableCollection<ContentModel> FindList
        {
            get
            {
                return _FindList;
            }
        }

        //To list
        private ObservableCollection<ContentModel> _ReplaceList = new ObservableCollection<ContentModel>();
        public ObservableCollection<ContentModel> ReplaceList
        {
            get
            {
                return _ReplaceList;
            }
        }

        private bool _isNotTheSame = true;
        public bool isNotTheSame
        {
            get
            {
                return _isNotTheSame;
            }
            set
            {
                _isNotTheSame = value;
                RaisePropertyChanged("isNotTheSame");
            }
        }
        #endregion

        #region Constructor

        public BulkUpdateViewModel(Guid workspaceId, TreeViewNodeViewModelBase TV)
        {
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //Messenger Service (to exchange messages between VMs)
            MessageMediator = GetService<MessengerService>();
            // Initialize Object
            this.WorkspaceId = workspaceId;
            Node = TV;
            //Hierarchy = TV.Hierarchy;
        }

        #endregion

        #region  IDropTarget Members & Other Drop Activities

        public void DragOver(Infra.DragDrop.IDropInfo DropInfo)
        {
            try
            {
                string SourceItemType = DropInfo.Data.GetType().ToString();
                string DropCollectionType = DropInfo.TargetCollection.GetType().ToString();
                string Uid = DropInfo.VisualTarget.Uid;

                //From list conatains only 1 item. Not allowed to drag more than one. 
                if (SourceItemType.Contains("CMTreeViewVersionNodeViewModel") && DropCollectionType.Contains("ContentModel") && Uid == "FromList")
                {
                    if (FindList.Count > 0)
                        return;
                }
                //Not allowed drag to 'To List' if replace list is empty. 
                if (SourceItemType.Contains("CMTreeViewVersionNodeViewModel") && DropCollectionType.Contains("ContentModel") && Uid == "ToList")
                {
                    CMTreeViewVersionNodeViewModel SourceItem = DropInfo.Data as CMTreeViewVersionNodeViewModel;
                    bool IsValid = IsValidReplaceList( SourceItem);
                    if (!IsValid)
                        return;
               
                }

                if (SourceItemType.Contains("CMTreeViewVersionNodeViewModel"))
                {
                   
                    CMTreeViewVersionNodeViewModel SourceItem = DropInfo.Data as CMTreeViewVersionNodeViewModel;
                    if (SourceItem != null)
                    {
                        DropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                        DropInfo.Effects = DragDropEffects.Move;
                    }
                }

                if (SourceItemType.Contains("ContentModel"))
                {
                    ContentModel SourceItem = DropInfo.Data as ContentModel;
                    if (SourceItem != null)
                    {
                        foreach (ContentModel cnt in ReplaceList)
                        {
                            if (cnt.id == SourceItem.id)
                            {
                                return;
                            }
                        }
                        DropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                        DropInfo.Effects = DragDropEffects.Move;
                    }
                }
               
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                MsgBoxService.ShowError("Error:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public void Drop(Infra.DragDrop.IDropInfo DropInfo)
        {
            try
            {
                //Identify dropped Entity Type
                string SourceItemType = DropInfo.Data.GetType().ToString();
                string DropCollectionType = DropInfo.TargetCollection.GetType().ToString();
                string Uid = DropInfo.VisualTarget.Uid;

                //If dropping Content, verify we drop on the right container and add to content list
                if (SourceItemType.Contains("CMTreeViewVersionNodeViewModel") && DropCollectionType.Contains("ContentModel") && Uid == "FromList")
                {
                    CMTreeViewVersionNodeViewModel SourceItem = DropInfo.Data as CMTreeViewVersionNodeViewModel;
                    //bool IsPermitted = CheckFromPermissions(ref SourceItem);
                    //if (!IsPermitted)
                    if (Domain.IsPermitted("106") || Domain.IsPermitted("999"))
                    {
                        ContentModel FromCM = new ContentModel(SourceItem.Parent.Name, SourceItem.Name, SourceItem.TreeNode.ID, DateTime.Now.ToString(), "", "");
                        FindList.Add(FromCM);
                        RaisePropertyChanged("FindList");
                        GetAllFamilyProject(Node);
                        ProjectsFamily = BulkUpdateBLL.LinkedContentToProjects(FullProjectList, FindList);
                        if (ProjectsFamily == null || ProjectsFamily.Count == 0)
                        {
                            Object[] ArgsList = new Object[] { 0 };
                            ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(181, ArgsList);
                            return;
                        }
                        RaisePropertyChanged("ProjectsFamily");

                    }
                }
                if ((SourceItemType.Contains("CMTreeViewVersionNodeViewModel")) && DropCollectionType.Contains("ContentModel") && Uid == "ToList")
                {
                    CMTreeViewVersionNodeViewModel SourceItem = DropInfo.Data as CMTreeViewVersionNodeViewModel;
                    ContentModel ToCM = new ContentModel(SourceItem.Parent.Name, SourceItem.Name, SourceItem.TreeNode.ID, DateTime.Now.ToString(), "", "");
                    isNotTheSame = true;
                    foreach (ContentModel cnt in FindList)
                    {
                        if (cnt.id == ToCM.id)
                        {
                            isNotTheSame = false;
                        }
                    }
                    if (isNotTheSame)
                    {
                        bool IsPermitted = CheckFromPermissions(ref SourceItem);
                        if (!IsPermitted)
                        {
                            CheckProjectContents(ProjectsFamily, SourceItem);
                        }
                    }
                    else
                    {
                        CheckProjectContents(ProjectsFamily, SourceItem);
                    }

                    RaisePropertyChanged("ProjectsFamily");                  
                    ReplaceList.Add(ToCM);
                    RaisePropertyChanged("ReplaceList");
                }
                if ((SourceItemType.Contains("ContentModel")) && DropCollectionType.Contains("ContentModel") && Uid == "ToList")
                {
                    ContentModel SourceItem = DropInfo.Data as ContentModel;
                    ContentModel ToCM = SourceItem;
                    isNotTheSame = true;
                    foreach (ContentModel cnt in FindList)
                    {
                        if (cnt.id == ToCM.id)
                        {
                            isNotTheSame = false;
                        }
                    }
                    ReplaceList.Add(ToCM);
                    RaisePropertyChanged("ProjectsFamily");
                    RaisePropertyChanged("ReplaceList");
                }
            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                MsgBoxService.ShowError("Error:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        #endregion

        #region  Add Content Command

        //private RelayCommand _AddContentCommand;
        //public RelayCommand AddContentCommand
        //{
        //    get
        //    {
        //        if (_AddContentCommand == null)
        //        {
        //            _AddContentCommand = new RelayCommand(ExecuteAddContentCommand, CanExecuteAddContentCommand);
        //        }
        //        return _AddContentCommand;
        //    }
        //}

        //private bool CanExecuteAddContentCommand()
        //{
        //    return Domain.IsPermitted("106");
           
        //}


        //private void ExecuteAddContentCommand()
        //{
        //    ////ContentBLL.isCMFlyoutOpen = true;

        //    MessageMediator.NotifyColleagues("ShowAddContent", WorkspaceId); //Send message to the MainViewModel
        //}

        private RelayCommand _FindContentCommand;
        public RelayCommand FindContentCommand
        {
            get
            {
                if (_FindContentCommand == null)
                {
                    _FindContentCommand = new RelayCommand(ExecuteFindContentCommand, CanExecuteFindContentCommand);
                }
                return _FindContentCommand;
            }
        }

        private bool CanExecuteFindContentCommand()
        {
            return true;

        }

        private void ExecuteFindContentCommand()
        {
            object[] parameters = new object[2];
            parameters[0] = WorkspaceId;
            parameters[1] = Node;
            MessageMediator.NotifyColleagues("ShowFindContent", parameters); //Send message to the MainViewModel
        }


        private RelayCommand _ReplaceContentCommand;
        public RelayCommand ReplaceContentCommand
        {
            get
            {
                if (_ReplaceContentCommand == null)
                {
                    _ReplaceContentCommand = new RelayCommand(ExecuteReplaceContentCommand, CanExecuteReplaceContentCommand);
                }
                return _ReplaceContentCommand;
            }
        }

        private bool CanExecuteReplaceContentCommand()
        {
            return true;

        }


        private void ExecuteReplaceContentCommand()
        {
            object[] parameters = new object[2];
            parameters[0] = WorkspaceId;
            parameters[1] = Node;
            MessageMediator.NotifyColleagues("ShowReplaceContent", parameters); //Send message to the MainViewModel
        }

        #endregion

        #region GetAllFamilyProject

        private  ObservableCollection<HierarchyModel> FullProjectList = new ObservableCollection<HierarchyModel>();
      
        public void GetAllFamilyProject(TreeViewNodeViewModelBase Node)
        {
          
            if (Node.Children.Count > 0)
            {
                foreach (var i in Node.Children)
                {
                    //Add only projects to project list
                    if (i.NodeType == NodeTypes.P)
                    {
                        //Intialize version row.
                        if (i.Hierarchy.GroupId == -1)
                            i.Hierarchy.VM = VersionBLL.GetActiveVersion(i.Hierarchy.Id);
                        else
                            i.Hierarchy.VM = VersionBLL.GetActiveVersion(i.Hierarchy.GroupId);

                        i.Hierarchy.IsBulkUpdatedChecked = true;
                        i.Hierarchy.IsBulkUpdatedEnabled = true;
                        i.Hierarchy.IsToolTipEnabled = !i.Hierarchy.IsBulkUpdatedEnabled;
                        FullProjectList.Add(i.Hierarchy);
                    }
                    else
                    {
                        //Check the children of the node.
                        if (i.Children.Count > 0)
                            GetAllFamilyProject(i);
                    }
                }
            }
        }


        #endregion GetAllFamilyProject

        #region From Validation

        public bool CheckFromPermissions(ref CMTreeViewVersionNodeViewModel contentTree)
        {
            try
            {
                if (Domain.IsPermitted("106") || Domain.IsPermitted("999"))
                {
                    //Check If content is retired.
                    if (ContentManagementViewModel.versions[contentTree.TreeNode.ID].Status.ID.Trim() == "Ret")
                    {
                        if ((Domain.IsPermitted("107") || Domain.IsPermitted("999")))
                        {
                            var SB = new StringBuilder(string.Empty);
                            SB.Append("SELECT Description FROM PE_Messages where id=120");
                            object SuccessObj = Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null);
                            if (SuccessObj != null)
                            {
                                string Success = (SuccessObj).ToString();
                                object[] ArgList = { contentTree.Name };
                                string SuccessName = String.Format(Success, ArgList);
                                if (MsgBoxService.ShowOkCancel(SuccessName.ToString(), DialogIcons.Question) == DialogResults.Cancel)
                                {
                                    return true;
                                }
                            }
                            else
                                return true;
                        }
                        else
                        {
                            Object[] ArgsList = new Object[] { 0 };
                            ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(126, ArgsList);
                            return true;
                        }

                    }//Is Content ret.
                }
                //Not allowed drag and drop.
                else
                    return true;
                //All OK
                return false;


            }
            catch (Exception ex)
            {
                String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                return true;
            }

        }

        #endregion From Validation

        #region Replace Validations

        public bool CheckProjectContents(ObservableCollection<HierarchyModel> projects, CMTreeViewVersionNodeViewModel SourceItem)
        {
          
            foreach (var Hierarchy in projects.ToList())
            {
               bool IsValid = IsCheckProject(Hierarchy, SourceItem);
                //All OK and it was allowed (checked) before.
               //if (IsValid && Hierarchy.IsBulkUpdatedChecked && Hierarchy.IsBulkUpdatedEnabled) //OK
               if (IsValid && Hierarchy.IsBulkUpdatedEnabled)
               {
                   //Hierarchy.IsBulkUpdatedChecked = true;
                   Hierarchy.IsBulkUpdatedEnabled = true;                  
               }
               else //Not allowed.
               {
                   Hierarchy.IsBulkUpdatedChecked = false;
                   Hierarchy.IsBulkUpdatedEnabled = false;
               }
               Hierarchy.IsToolTipEnabled = !Hierarchy.IsBulkUpdatedEnabled;
               projects.Remove(Hierarchy);
               projects.Add(Hierarchy);
               RaisePropertyChanged("ProjectsFamily");
            }

            return true;
        }

        public bool CheckProjectContents(ObservableCollection<HierarchyModel> projects, int sourceCmVersionId)
        {

            foreach (var Hierarchy in projects.ToList())
            {
                bool IsValid = IsCheckProject(Hierarchy, sourceCmVersionId);
                //All OK and it was allowed (checked) before.
                if (IsValid && Hierarchy.IsBulkUpdatedChecked && Hierarchy.IsBulkUpdatedEnabled) //OK
                {
                    Hierarchy.IsBulkUpdatedChecked = true;
                    Hierarchy.IsBulkUpdatedEnabled = true;
                }
                else //Not allowed.
                {
                    Hierarchy.IsBulkUpdatedChecked = false;
                    Hierarchy.IsBulkUpdatedEnabled = false;
                }
                Hierarchy.IsToolTipEnabled = !Hierarchy.IsBulkUpdatedEnabled;
                projects.Remove(Hierarchy);
                projects.Add(Hierarchy);
                RaisePropertyChanged("ProjectsFamily");
            }

            return true;
        }
        
        //Detrminte if the checkbox on projects list is enabled or disabled.
        public bool IsCheckProject(HierarchyModel HM, CMTreeViewVersionNodeViewModel SourceItem)
        {
             Dictionary<int, int> ExsitingContents = new Dictionary<int, int>();
                var SB = new StringBuilder(string.Empty);
                SB.Append("SELECT ContentVersionId FROM dbo.PE_VersionContent avc Join dbo.PE_Version av " +
                " on av.versionId = avc.VersionId  WHERE av.HierarchyId = ");
                if (HM.GroupId == -1)
                {
                    SB.Append(" '" + HM.Id + "' ");
                }
                else
                    SB.Append(" '" + HM.GroupId + "' ");
                SB.Append(" AND av.VersionStatus='A' and av.versionId ='" + HM.VM.VersionId + "'");
                SB.Append(" AND ContentVersionId != '" + FindList[0].id + "'");
                DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);

                if (ResTable != null)
                {
                    foreach (DataRow DataRow in ResTable.Rows)
                    {
                        int ContentVersionId = (int)DataRow["ContentVersionId"];

                        //Get content ID.
                        int ContentID = ContentManagementViewModel.versions[ContentVersionId].ParentID;
                        //Adding to dicatonary all the current contents.
                        //key - version id, value - content id.
                        ExsitingContents.Add(ContentVersionId, ContentID);

                    }
                }

                if (ExsitingContents.ContainsKey(SourceItem.TreeNode.ID))
                    return false;
                if (ExsitingContents.ContainsValue(ContentManagementViewModel.versions[SourceItem.TreeNode.ID].ParentID))
                    return false;
            
                Dictionary<int, int> LinkedContents = new Dictionary<int, int>();
                CheckLinkedVersions(ExsitingContents, LinkedContents);
                if (LinkedContents.ContainsKey(SourceItem.TreeNode.ID))
                    return false;
                if (LinkedContents.ContainsValue(ContentManagementViewModel.versions[SourceItem.TreeNode.ID].ParentID))
                    return false;

            return true;
        }

        public bool IsCheckProject(HierarchyModel HM, int cmVersionId)
        {
            Dictionary<int, int> ExsitingContents = new Dictionary<int, int>();
            var SB = new StringBuilder(string.Empty);
            SB.Append("SELECT ContentVersionId FROM dbo.PE_VersionContent avc Join dbo.PE_Version av " +
            " on av.versionId = avc.VersionId  WHERE av.HierarchyId = ");
            if (HM.GroupId == -1)
            {
                SB.Append(" '" + HM.Id + "' ");
            }
            else
                SB.Append(" '" + HM.GroupId + "' ");
            SB.Append(" AND av.VersionStatus='A' and av.versionId ='" + HM.VM.VersionId + "'");
            SB.Append(" AND ContentVersionId != '" + FindList[0].id + "'");
            DataTable ResTable = Domain.PersistenceLayer.FetchDataTable(SB.ToString(), CommandType.Text, null);

            if (ResTable != null)
            {
                foreach (DataRow DataRow in ResTable.Rows)
                {
                    int ContentVersionId = (int)DataRow["ContentVersionId"];

                    //Get content ID.
                    int ContentID = ContentManagementViewModel.versions[ContentVersionId].ParentID;
                    //Adding to dicatonary all the current contents.
                    //key - version id, value - content id.
                    ExsitingContents.Add(ContentVersionId, ContentID);

                }
            }

            if (ExsitingContents.ContainsKey(cmVersionId))
                return false;
            if (ExsitingContents.ContainsValue(ContentManagementViewModel.versions[cmVersionId].ParentID))
                return false;

            Dictionary<int, int> LinkedContents = new Dictionary<int, int>();
            CheckLinkedVersions(ExsitingContents, LinkedContents);
            if (LinkedContents.ContainsKey(cmVersionId))
                return false;
            if (LinkedContents.ContainsValue(ContentManagementViewModel.versions[cmVersionId].ParentID))
                return false;

            return true;
        }


        public void CheckLinkedVersions(Dictionary<int, int> ExsitingContents, Dictionary<int, int> LinkedCollection)
        {
            foreach (var i in ExsitingContents)
            {
                CheckSubLinkedVersions(ContentManagementViewModel.versions[i.Key].ContentVersions, LinkedCollection);
            }
        }


        public void CheckSubLinkedVersions(Dictionary<int, CMContentVersionSubVersionModel> ContentVersions, Dictionary<int, int> LinkedCollection)
        {
            foreach (var i in ContentVersions)
            {
                LinkedCollection.Add(i.Key, i.Value.ContentSubVersion.ParentID);
                CheckSubLinkedVersions(i.Value.ContentSubVersion.ContentVersions, LinkedCollection);
            }
        }



        #endregion CheckToVersion

        #region Drag Replace Validation

        public bool IsValidReplaceList(CMTreeViewVersionNodeViewModel SourceItem)
        {
            if (FindList.Count == 0)
                return false;
            
            //For existing versions of replace list.\
            Dictionary<int, int> ReplaceVersions = new Dictionary<int, int>();
            //Adding all versions from replace list.
            if (ReplaceList.Count > 0)
            {
                foreach (var i in ReplaceList)
                {
                    ReplaceVersions.Add(i.id, ContentManagementViewModel.versions[i.id].ParentID);
                }
            }

            //If in replace list there is the same content or version.
            if (ReplaceList.Count > 0)
            {
                if (ReplaceVersions.ContainsKey(SourceItem.TreeNode.ID))
                    return false;
                if (ReplaceVersions.ContainsValue(SourceItem.TreeNode.ParentID))
                    return false;
            }

            //For Linked Versions of the replace list.
            Dictionary<int, int> ReplaceLinkedVersions = new Dictionary<int, int>();
            CheckLinkedVersions(ReplaceVersions, ReplaceLinkedVersions);

            if (ReplaceLinkedVersions.ContainsKey(SourceItem.TreeNode.ID))
                return false;
            if (ReplaceLinkedVersions.ContainsValue(SourceItem.TreeNode.ParentID))
                return false;

            return true;
        }

        #endregion  Drag Replace Validation

        #region  Save Command

        private RelayCommand _SaveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                {
                    _SaveCommand = new RelayCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
                }
                return _SaveCommand;
            }
        }

        private bool CanExecuteSaveCommand()
        {

            if (!Domain.IsPermitted("123"))
            {
                return false;
            }

            //Find list is empty
            if (FindList.Count == 0)
                return false;
            if (ReplaceList.Count == 1)
            {
                if (ReplaceList[0].id == FindList[0].id) //if the replace and find list are the same. 
                    return false;
            }
            if (ProjectsFamily.Count > 0) //Check that there is projects.
            {
                foreach (var i in ProjectsFamily)
                {
                    if (i.IsBulkUpdatedChecked) //at least one project is checked.
                        return true;
                }
                return false;
            }
            else
                return false;
        }


        private void ExecuteSaveCommand()
        {
            Object[] ArgsList = new Object[] { 0, 0 };
            try
            {
                IsCancelBulkEnabled = false;
                RaisePropertyChanged("IsCancelBulkEnabled");
                Domain.PersistenceLayer.BeginTransWithIsolation(IsolationLevel.Serializable);

                bool IsItFail = false;
                foreach (var Hierarchy in ProjectsFamily){
                    if (Hierarchy.IsBulkUpdatedChecked)
                    {
                        try
                        {
                            string Persisted = BulkUpdateBLL.PresistBulkUpdate(Hierarchy, FindList, ReplaceList);
                            if (!String.IsNullOrEmpty(Persisted))
                            {
                                IsItFail = true;
                                Domain.PersistenceLayer.AbortTrans();
                                ArgsList = new Object[] { Hierarchy.Name, Hierarchy.VM.VersionName };
                                //ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Persisted), ArgsList);
                                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(180, ArgsList);
                                IsCancelBulkEnabled = true;
                                RaisePropertyChanged("IsCancelBulkEnabled");
                                break;
                            }
                        }
                        catch (Exception E)
                        {
                                IsItFail = true;
                                Domain.PersistenceLayer.AbortTrans();
                                String logMessage = E.Message + "\n" + "Source: " + E.Source + "\n" + E.StackTrace;
                                Domain.SaveGeneralErrorLog(logMessage);
                                ArgsList = new Object[] { Hierarchy.Name, Hierarchy.VM.VersionName };
                                //ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(Persisted), ArgsList);
                                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(180, ArgsList);
                                IsCancelBulkEnabled = true;
                                RaisePropertyChanged("IsCancelBulkEnabled");
                                break;
                        }
                    }
                }

                //Not fail.
                if (!IsItFail)
                {
                    Domain.PersistenceLayer.CommitTrans();
                    IsCancelBulkEnabled = true;
                    IsProjectListEnabled = false;

                    ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(203, ArgsList);

                }
               
            }
            catch (Exception E)
            {
                IsCancelBulkEnabled = true;
                System.Diagnostics.Debug.WriteLine("{0} Exception caught.", E); // TODO: Log error
                String logMessage = E.Message + "\n" + "Source: " + E.Source + "\n" + E.StackTrace;
                Domain.SaveGeneralErrorLog(logMessage);
                Domain.PersistenceLayer.AbortTrans();
                if (E.Message == "DB Error")
                {
                    ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(141, ArgsList);
                }
                else
                {
                    ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
                }
            }


        }

        #endregion

        #region  Cancel Command

        private RelayCommand _CancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                if (_CancelCommand == null)
                {
                    _CancelCommand = new RelayCommand(ExecuteCancelCommand, CanExecuteCancelCommand);
                }
                return _CancelCommand;
            }
        }

        private Boolean _IsCancelBulkEnabled = true;
        public Boolean IsCancelBulkEnabled
        {
            get
            {
                return _IsCancelBulkEnabled;
            }
            set
            {
                _IsCancelBulkEnabled = value;
                RaisePropertyChanged("IsCancelBulkEnabled");
            }
        }

        private bool CanExecuteCancelCommand()
        {
            return true;
        }


        private void ExecuteCancelCommand()
        {
            MessageMediator.NotifyColleagues(WorkspaceId + "ShowFolderDetails", Node);
        }

        #endregion

        #region  Clear All Command

        private RelayCommand _ClearAllCommand;
        public RelayCommand ClearAllCommand
        {
            get
            {
                if (_ClearAllCommand == null)
                {
                    _ClearAllCommand = new RelayCommand(ExecuteClearAllCommand, CanExecuteClearAllCommand);
                }
                return _ClearAllCommand;
            }
        }

        private bool CanExecuteClearAllCommand()
        {
            return true;
        }


        private void ExecuteClearAllCommand()
        {
            MessageMediator.NotifyColleagues(WorkspaceId + "ShowBulkUpdate", Node);
        }

        #endregion

        #region  "To" list remove command

        private RelayCommand _RemoveToCommand;
        public RelayCommand RemoveToCommand
        {
            get
            {
                if (_RemoveToCommand == null)
                {
                    _RemoveToCommand = new RelayCommand(ExecuteRemoveToCommand, CanExecuteRemoveToCommand);
                }
                return _RemoveToCommand;
            }
        }

        private bool CanExecuteRemoveToCommand()
        {
            return true;
        }

        public ContentModel contentToAction { set; get; }
        private void ExecuteRemoveToCommand()
        {
            ReplaceList.Remove(contentToAction);
            RaisePropertyChanged("ReplaceList");
            foreach (var pr in ProjectsFamily.ToList())
            {
                pr.IsBulkUpdatedChecked = true;
                pr.IsBulkUpdatedEnabled = true;
                pr.IsToolTipEnabled = !pr.IsBulkUpdatedEnabled;
                ProjectsFamily.Remove(pr);
                ProjectsFamily.Add(pr);
            }
            foreach (var c in ReplaceList)
            {
                CheckProjectContents(ProjectsFamily, c.id);
            }
            RaisePropertyChanged("ProjectsFamily");

            if (ReplaceList.Count == 1 && ReplaceList[0].id == FindList[0].id)
            {
                //if the replace and find list are the same. 
                isNotTheSame = false;
            }
            else
            {
                isNotTheSame = true;
            }
        }

        #endregion
    }

} //end of root namespace