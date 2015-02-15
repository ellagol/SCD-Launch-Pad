using System;
using System.Data;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ATSBusinessLogic;
using ATSBusinessObjects;
using ATSDomain;
using Infra.MVVM;
using Microsoft.Win32;

namespace ExplorerModule
{
	public class TreeViewFolderNodeViewModel : TreeViewNodeViewModelBase
	{

	    #region  Data 

		private IMessageBoxService MsgBoxService = null;

	#endregion

        public TreeViewFolderNodeViewModel(Guid workspaceId, HierarchyModel Hierarchy)
            : this(workspaceId, Hierarchy, null)
		{
		}

        public TreeViewFolderNodeViewModel(Guid workspaceId, HierarchyModel Hierarchy, TreeViewNodeViewModelBase ParentNode)
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
            MessageMediator.NotifyColleagues(WorkSpaceId + "ShowFolderDetails", this); //Will be returned to the Explorer Main signed for this message
		}
        //Sends a message to the MainWindow with the required information to save a details view of the currently selected node
   

      
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
            return (this.Children.Count < 1) && (Domain.IsPermitted("103"));
        }

        private void ExecuteDeleteFolderCommand()
        {

            var SB = new StringBuilder(string.Empty);
            SB.Append("SELECT Description FROM PE_Messages where id=101;");
            MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
            if (MsgBoxService.ShowYesNo((Domain.PersistenceLayer.FetchDataValue(SB.ToString(), CommandType.Text, null)).ToString(), DialogIcons.Question) == DialogResults.Yes)
            {
                try
                {

                    Domain.PersistenceLayer.BeginTransWithIsolation(IsolationLevel.Serializable);

                    int ChildNum = HierarchyBLL.CountHierarchyChild(Hierarchy.Id);
                    if (ChildNum > 0)
                    {
                        Domain.PersistenceLayer.AbortTrans();
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(104, ArgsList);
                        return;
                    }


                    string LastUpdateCheck = HierarchyBLL.LastUpadateCheck(ref _Hierarchy);
                    if (!(String.IsNullOrEmpty(LastUpdateCheck)))
                    {
                        Domain.PersistenceLayer.AbortTrans();
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(LastUpdateCheck), ArgsList);
                        return;
                    }


                    HierarchyBLL.DeleteFolder(this.Hierarchy);
                    MessageMediator.NotifyColleagues(WorkSpaceId + "DeleteNode", this); //Will be returned to the MainWindow signed for this message, to remove the node from the TreeView
                    Domain.PersistenceLayer.CommitTrans();
                }
                catch (Exception e)
                {
                    Domain.PersistenceLayer.AbortTrans();
                    if (e.Message == "DB Error")
                    {
                        Object[] ArgsList = new Object[] { 0 };
                        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(141, ArgsList);
                    }
                    System.Diagnostics.Debug.WriteLine("{0} Exception caught.", e); // TODO: Log error
                }
            }
        }

        #endregion

    #region  Bulk Update Command

        private RelayCommand _BulkUpdateCommand;
        public ICommand BulkUpdateCommand
        {
            get
            {
                if (_BulkUpdateCommand == null)
                {
                    _BulkUpdateCommand = new RelayCommand(ExecuteBulkUpdateCommand, CanExecuteBulkUpdateCommand);
                }
                return _BulkUpdateCommand;
            }
        }

        private bool CanExecuteBulkUpdateCommand()
        {
            return (this.Children.Count > 0) && (Domain.IsPermitted("170"));
        }

        private void ExecuteBulkUpdateCommand()
        {
            MessageMediator.NotifyColleagues(WorkSpaceId + "ShowBulkUpdate", this);
        }

        #endregion


    #region  New Template Command

        private RelayCommand _NewTemplateCommand;
        public ICommand NewTemplateCommand
        {
            get
            {
                if (_NewTemplateCommand == null)
                {
                    _NewTemplateCommand = new RelayCommand(ExecuteNewTemplateCommand, CanExecuteNewTemplateCommand);
                }
                return _NewTemplateCommand;
            }
        }

        private bool CanExecuteNewTemplateCommand()
        {
            //TODO: Add Is Permitted '150'.
            return Domain.IsPermitted("170");
        }

        private void ExecuteNewTemplateCommand()
        {
            this.Hierarchy.IsNew = true;
            MessageMediator.NotifyColleagues(WorkSpaceId + "ShowNewTemplate", this);
        }

        #endregion

	#endregion

        //Import moved to root
        //#region Import

        //private RelayCommand _ImportProjectArchive;
        //public ICommand ImportProjectArchive
        //{
        //    get
        //    {
        //        if (_ImportProjectArchive == null)
        //        {
        //            _ImportProjectArchive = new RelayCommand(ExecuteImportProjectArchive, CanExecuteImportProjectArchive);
        //        }
        //        return _ImportProjectArchive;
        //    }
        //}

        //private bool CanExecuteImportProjectArchive()
        //{
        //    return true;
        //}

        //string archiveFolderName = string.Empty;

        //private void ExecuteImportProjectArchive()
        //{
        //    MessageMediator.NotifyColleagues("StatusBarParameters", null);

        //    String result = HierarchyBLL.LastUpadateCheck(ref _Hierarchy);
        //    if (!String.IsNullOrEmpty(result))
        //    {
        //        Object[] ArgsList = new Object[] { 0 };
        //        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(104, ArgsList);
        //        return;
        //    }

        //    if (!Domain.IsPermitted("153"))
        //    {
        //        Object[] ArgsList = new Object[] { 0 };
        //        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(106, ArgsList);
        //        return;
        //    }
            
        //    OpenFileDialog dialog = new OpenFileDialog();

        //    dialog.Title = "Select Project Archive";
        //    dialog.Filter = "Project archive files (*.project) | *.project";

        //    if ((bool)dialog.ShowDialog())
        //    {
        //        archiveFolderName = dialog.FileName;
        //        string messageText = "Selected project will be imported to current environment. Parent folder is " + this.Hierarchy.Name;
        //        if (MsgBoxService.ShowOkCancel(messageText, DialogIcons.Information) == DialogResults.OK)
        //        {
        //            archiveFolderName = dialog.FileName;
        //        }
        //        else
        //        {
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        return;
        //    }
        //    ProjectsExplorerViewModel.ShowErrorAndInfoMessage(224, new Object[] { 0 });
        //    Thread ImportProjectThread = new Thread(new ThreadStart(ProceedImportProject));
        //    ImportProjectThread.Start();   
        //}

        //private void ProceedImportProject()
        //{
        //    Domain.ErrorHandling result = new Domain.ErrorHandling();
        //    HierarchyModel Project = new HierarchyModel(); ;

        //    try
        //    {
        //        result = ImportProjectBLL.ImportMain(Hierarchy, archiveFolderName, out Project);
        //        if (result.messsageId != string.Empty || Project == null) //Failed
        //        {
        //            ProjectsExplorerViewModel.ShowErrorAndInfoMessage(Convert.ToInt32(result.messsageId), result.messageParams);
        //            return;
        //        }
        //        else
        //        {
        //            //Success
        //            Application.Current.Dispatcher.Invoke((Action)delegate 
        //            {
        //                MessageMediator.NotifyColleagues(this.WorkSpaceId + "AddImportedProject", Project);
        //            });
        //        }
        //        //ProjectsExplorerViewModel.ShowErrorAndInfoMessage(224, new Object[] { 0 });
        //    }
        //    catch (Exception ex)
        //    {
        //        String logMessage = ex.Message + "\n" + "Source: " + ex.Source + "\n" + ex.StackTrace;
        //        Domain.SaveGeneralErrorLog(logMessage);
        //        Object[] ArgsList = new Object[] { 0 };
        //        ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
        //    }
        //}
        //#endregion

	}

} //end of root namespace