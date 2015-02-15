using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ATSBusinessLogic;
using ATSBusinessObjects;
using ATSDomain;
using Infra.MVVM;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ExplorerModule
{
	public class TreeViewRootNodeViewModel : TreeViewNodeViewModelBase
	{

    	#region  Data 

		private IMessageBoxService MsgBoxService = null;

	#endregion

        public TreeViewRootNodeViewModel(Guid workspaceId, HierarchyModel Hierarchy) 
            : this(workspaceId, Hierarchy, null)
		{
		}

        public TreeViewRootNodeViewModel(Guid workspaceId, HierarchyModel Hierarchy, TreeViewNodeViewModelBase ParentNode)
            : base(workspaceId, Hierarchy, ParentNode)
		{
			//Message Box Service
			MsgBoxService = GetService<IMessageBoxService>();
			//The messageMediator is registered in the ViewModelBase - Generally you have 1 mediator; Hence, the restricted access to the constructor
			MessageMediator = GetService<MessengerService>();
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
            MessageMediator.NotifyColleagues(WorkSpaceId + "ShowEnvironmentDetails", this); //Will be returned to the Explorer Main signed for this message
		}

        //Sends a message to the MainWindow with the required information to save a details view of the currently selected node
    
	#region  Context Menu Commands (Specific to this Node Type; others appear in the Base Class) 

	#endregion

        #region Import

        private RelayCommand _ImportProjectArchive;
        public ICommand ImportProjectArchive
        {
            get
            {
                if (_ImportProjectArchive == null)
                {
                    _ImportProjectArchive = new RelayCommand(ExecuteImportProjectArchive, CanExecuteImportProjectArchive);
                }
                return _ImportProjectArchive;
            }
        }

        private bool CanExecuteImportProjectArchive()
        {
            return true;
        }

        string archiveFolderName = string.Empty;

        private void ExecuteImportProjectArchive()
        {
            MessageMediator.NotifyColleagues("StatusBarParameters", null);

            String result = HierarchyBLL.LastUpadateCheck(ref _Hierarchy);
            if (!String.IsNullOrEmpty(result))
            {
                Object[] ArgsList = new Object[] { 0 };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(104, ArgsList);
                return;
            }

            if (!Domain.IsPermitted("153"))
            {
                Object[] ArgsList = new Object[] { 0 };
                ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(106, ArgsList);
                return;
            }

            //OpenFileDialog dialog = new OpenFileDialog();

            //dialog.Title = "Select Project Archive";
            //dialog.Filter = "Project archive files (*.project) | *.project";

            //if ((bool)dialog.ShowDialog())
            //{
            //    archiveFolderName = dialog.FileName;
            //    string messageText = "Selected project will be imported to current environment. Parent folder is " + this.Hierarchy.Name;
            //    if (MsgBoxService.ShowOkCancel(messageText, DialogIcons.Information) == DialogResults.OK)
            //    {
            //        archiveFolderName = dialog.FileName;
            //    }
            //    else
            //    {
            //        return;
            //    }
            //}
            //else
            //{
            //    return;
            //}

            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.RootFolder = Environment.SpecialFolder.Desktop;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string messageText = "Selected project will be imported to current environment.";
                    if (MsgBoxService.ShowOkCancel(messageText, DialogIcons.Information) == DialogResults.OK)
                    {
                        archiveFolderName = dlg.SelectedPath;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            ProjectsExplorerViewModel.ShowErrorAndInfoMessage(224, new Object[] { 0 });
            Thread ImportProjectThread = new Thread(new ThreadStart(ProceedImportProject));
            ImportProjectThread.Start();
        }

        private void ProceedImportProject()
        {
            Domain.ErrorHandling result = new Domain.ErrorHandling();
            HierarchyModel Project = new HierarchyModel();
            bool projectExists = false;

            try
            {
                result = ImportProjectBLL.ImportProject(archiveFolderName, out Project);
                if (result.messsageId != string.Empty || Project == null) //Failed
                {
                    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(result.messsageId), result.messageParams);
                    return;
                }
                else
                {
                    MessageMediator.NotifyColleagues("StatusBarParameters", null);
                    this.IsSelected = false;
                    try
                    {
                        string listOfProjectParentFolders = string.Empty;
                        HierarchyBLL.HierarchyBLLReturnCode status = HierarchyBLL.GetHierarchyBranchIdsProjectId(Project.Id, out listOfProjectParentFolders);
                        if (status != HierarchyBLL.HierarchyBLLReturnCode.Success || string.IsNullOrEmpty(listOfProjectParentFolders))
                        {
                            String logMessage = "Import succeeded, but failed to add folder node to hierarchy tree. Please refresh.";
                            Domain.SaveGeneralErrorLog(logMessage);
                            Object[] ArgsList = new Object[] { 0 };
                            ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
                        }
                        string[] hIds = listOfProjectParentFolders.Split(',');
                        List<int> listHIds = new List<int>();

                        foreach (string hId in hIds)
                        {
                            int intHId = Convert.ToInt32(hId);
                            listHIds.Add(intHId);
                        }
                        if (listHIds.Count == 1)
                        {
                            //created under Root
                            System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                MessageMediator.NotifyColleagues(this.WorkSpaceId + "AddImportedProject", Project);
                            });
                        }
                        else
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                MessageMediator.NotifyColleagues(this.WorkSpaceId + "AddImportedFolder", listHIds);
                            });
                        }
                        //ProjectsExplorerViewModel.ShowErrorAndInfoMessage(225, new Object[] { 0 });
                    }
                    catch (Exception ex)
                    {
                        String logMessage = "Import succeeded, but failed to add folder node to hierarchy tree. Please refresh.";
                        logMessage = logMessage + "\n" + ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
                        Domain.SaveGeneralErrorLog(logMessage);
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
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
        #endregion

	}

} //end of root namespace