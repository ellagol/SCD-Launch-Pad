﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Documents;
using System.Windows.Input;
using ATSBusinessLogic.ContentMgmtBLL;
using ATSBusinessObjects.ContentMgmtModels;
using ATSDomain;
using Infra.MVVM;
using TraceExceptionWrapper;


namespace ContentMgmtModule
{
    public class CMTreeViewFolderNodeViewModel : CMTreeViewNodeViewModelBase
    {
        #region  Data

        private IMessageBoxService MsgBoxService = null;
        private Guid guid;
        private ATSBusinessObjects.ContentMgmtModels.CMFolderModel FM;
        private CMTreeViewNodeViewModelBase ParentNode;

        #endregion

        #region Constructor

        public CMTreeViewFolderNodeViewModel(Guid workspaceId, CMTreeNode TN)
            : this(workspaceId, TN, null)
        {
            this.ID = ((ATSBusinessObjects.ContentMgmtModels.CMFolderModel)(TN)).Id;
            this.TreeNode.TreeNodeType = ATSBusinessObjects.ContentMgmtModels.TreeNodeObjectType.Folder;
        }

        public CMTreeViewFolderNodeViewModel(Guid workspaceId, CMTreeNode TN, CMTreeViewNodeViewModelBase ParentNode)
            : base(workspaceId, TN, ParentNode)
        {
            //Message Box Service
            MsgBoxService = GetService<IMessageBoxService>();
            //The messageMediator is registered in the ViewModelBase - Generally you have 1 mediator; Hence, the restricted access to the constructor
            MessageMediator = GetService<MessengerService>();
            this.TreeNode.TreeNodeType = ATSBusinessObjects.ContentMgmtModels.TreeNodeObjectType.Folder;
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

        private bool _isAddFolder;
        public bool IsAddFolder
        {
            get
            {
                return _isAddFolder;
            }
            set
            {
                _isAddFolder = value;
                RaisePropertyChanged("IsAddFolder");
            }
        }

        private bool _isAddContent;
        public bool IsAddContent
        {
            get
            {
                return _isAddContent;
            }
            set
            {
                _isAddContent = value;
                RaisePropertyChanged("IsAddContent");
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
            MessageMediator.NotifyColleagues(WorkSpaceId + "ShowAndUpdateFolderDetails", this); //Will be returned to the CM Main signed for this message
        }

        #endregion

        #region  Context Menu Commands (Specific to this Node Type; others appear in the Base Class)

            #region  Delete Folder Command

            private RelayCommand _DeleteFolderCommand;
            public ICommand DeleteFolderCommand
            {
                get
                {
                    if (_DeleteFolderCommand == null)
                    {
                        _DeleteFolderCommand = new RelayCommand(ExecuteDeleteFolderCommand, CanExecuteDeleteFolderCommand);
                    }
                    return _DeleteFolderCommand;
                }
            }

            private bool CanExecuteDeleteFolderCommand()
            {
                if (this.Children.Count > 0)
                    return false;

                return true;
            }

            private void ExecuteDeleteFolderCommand()
            {
                /* to check alon *///Dirty

                if(this.IsSelectedDirtyNode != null)
                {
                    if (MsgBoxService.ShowOkCancel(ATSBusinessLogic.HierarchyBLL.GetMessage(), DialogIcons.Question) == DialogResults.Cancel)
                    {
                        return;
                    }
                    //if ok clicked , and "yes" to delete , Dirty set to false
                }

                List<String> parameters = new List<string>();
                var SB = new StringBuilder(string.Empty);
                String errorId = "Delete";
                string displayMessage;
        
                SB.Append("SELECT ED_Description FROM ErrorDescription where ED_ID='" + errorId + "';");
               
                MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                displayMessage = (Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString() ;
                parameters.Add("folder");
                parameters.Add(this.TreeNode.Name);
                displayMessage = CMContentManagementViewModel.UpdateMessageStringParameters(displayMessage, parameters);

                if (MsgBoxService.ShowYesNo(displayMessage, DialogIcons.Question) == DialogResults.Yes)
                {
                    try
                    {
                        object[] ArgMessageParam = { parameters[0], parameters[1] };
                        CMFolderBLL.CompareUpdateTime(((CMFolderModel)(this.TreeNode)).LastUpdateTime, this.TreeNode.ID);
                        CMFolderBLL.GetFolderChildCount(this.TreeNode.ID);
                        CMFolderBLL.DeleteFolderContentTreeUserGroupType(this.TreeNode.ID);
                        CMFolderBLL.DeleteFolder(this.TreeNode.ID);
                        MessageMediator.NotifyColleagues(WorkSpaceId + "DeleteNode", this); //Will be returned to the MainWindow signed for this message, to remove the node from the TreeView                                          
                        CMContentManagementViewModel.ShowErrorAndInfoMessage(errorId, ArgMessageParam);
                        // Update permission
                        CMContentManagementViewModel.UpdatePermissionDeleteNode(this.Parent);
                        MessageMediator.NotifyColleagues(this.WorkSpaceId + "OnIsDirtyNodeReceived", null);
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
                        parameters.Add(this.TreeNode.Name);
                        displayMessage = CMContentManagementViewModel.UpdateMessageStringParameters(displayMessage, parameters);

                        object[] ArgMessageParam = null;
                        if (te.ApplicationErrorID == "Folder changed")
                        {
                            parameters.Add(((CMFolderModel)(this.TreeNode)).LastUpdateUser);
                            object[] tempArgMessageParam = { parameters[0], parameters[1] };
                            ArgMessageParam = tempArgMessageParam;
                        }
                        else if (te.ApplicationErrorID == "Folder deleted" || te.ApplicationErrorID == "Folder delete with subitems")
                        {
                            object[] tempArgMessageParam = { parameters[0] };
                            ArgMessageParam = tempArgMessageParam;
                        }

                        CMContentManagementViewModel.ShowErrorAndInfoMessage(te.ApplicationErrorID, ArgMessageParam);
                        MessageMediator.NotifyColleagues(WorkSpaceId + "RefreshTree", this); //Will be returned to the MainWindow signed for this message
                        return;   
                    }
                }
                MessageMediator.NotifyColleagues(this.WorkSpaceId + "ShowAndUpdateFolderDetails", this.Parent); //Will be returned to the CM Main signed for this message
            }

            #endregion

        #endregion

    }
} 