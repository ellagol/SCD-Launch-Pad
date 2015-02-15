using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ATSBusinessLogic;
using ATSBusinessObjects;
using ATSDomain;
using Infra.MVVM;

namespace ExplorerModule
{
	public abstract class TreeViewNodeViewModelBase : ViewModelBase
	{

	#region  Data 

		private bool _IsExpanded;
		private bool _IsSelected;
		private bool _IsDirty;

		protected Infra.MVVM.MessengerService MessageMediator = new MessengerService();

        protected Infra.MVVM.MessengerService MessageMediatorFirst = new MessengerService();
		private IMessageBoxService MsgBoxService = null;

	#endregion // Data

	#region  Constructor 


        protected TreeViewNodeViewModelBase(Guid workSpaceId, HierarchyModel HM) : this(workSpaceId, HM, null)
		{
		}

        protected TreeViewNodeViewModelBase(Guid workSpaceId, HierarchyModel HM, TreeViewNodeViewModelBase ParentNode)
		{
			//Message Box Service
			MsgBoxService = GetService<IMessageBoxService>();
			//Messenger Service (to exchange messages between VMs)
			MessageMediator = GetService<MessengerService>();
			//Assign Model properties to VM properties
            WorkSpaceId = workSpaceId;
			Hierarchy = HM;
			NodeType = Hierarchy.NodeType;
			if (ParentNode == null)
			{
				_NodeLevel = 0;
				Parent = null;
			}
			else
			{
				_NodeLevel = ParentNode.NodeLevel + 1;
				Parent = ParentNode;
			}
			Name = Hierarchy.Name;
			Description = Hierarchy.Description;
			switch (Hierarchy.NodeType)
			{
				case NodeTypes.R: //Root Node
					Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "Root"), UriKind.RelativeOrAbsolute));
					break;
				case NodeTypes.F: //Folder
                    //if (NoteBLL.CheckForSpecialNotes(Hierarchy.Id)) //give sign on icon if there are special notes
                    if (HierarchyBLL.listOfHierarchyIdsWithSpecialNotes.Contains(Convert.ToInt32(Hierarchy.Id)))
                    {
                        Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "FolderSpecial"), UriKind.RelativeOrAbsolute));
                        LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "FolderSpecial"), UriKind.RelativeOrAbsolute));
                    }
                    else
                    {
                        Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "Folder"), UriKind.RelativeOrAbsolute));
                        LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "Folder"), UriKind.RelativeOrAbsolute));
                    }

					break;
				case NodeTypes.P: //Project
                    if (Hierarchy.GroupId == -1)
                    {
                        if (Hierarchy.ProjectStatus == "Disabled")
                        {
                            if (NoteBLL.CheckForSpecialNotes(Hierarchy.Id)) //give sign on icon if there are special notes
                            {
                                Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "ProjectDisSpecial"), UriKind.RelativeOrAbsolute));
                                LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "ProjectDisableSpeciel"), UriKind.RelativeOrAbsolute));
                            }
                            else
                            {
                                Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "ProjectDis"), UriKind.RelativeOrAbsolute));
                                LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "ProjectDisable"), UriKind.RelativeOrAbsolute));
                            }
                        }
                        else
                        {
                            //if (NoteBLL.CheckForSpecialNotes(Hierarchy.Id)) //give sign on icon if there are special notes
                            if (HierarchyBLL.listOfHierarchyIdsWithSpecialNotes.Contains(Convert.ToInt32(Hierarchy.Id)))
                            {
                                Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "ProjectSpecial"), UriKind.RelativeOrAbsolute));
                                LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "ProjectSpeciel"), UriKind.RelativeOrAbsolute));
                            }
                            else
                            {
                                Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "Project"), UriKind.RelativeOrAbsolute));
                                LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "Project"), UriKind.RelativeOrAbsolute));

                            }
                        }
                    }
                    else
                    {
                        if (Hierarchy.ProjectStatus == "Disabled")
                        {
                            Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "groupClosed"), UriKind.RelativeOrAbsolute));
                            LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "groupClosed"), UriKind.RelativeOrAbsolute));
                        }
                        else
                        {
                            Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "groupActive"), UriKind.RelativeOrAbsolute));
                            LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "groupActive"), UriKind.RelativeOrAbsolute));
                        }
                    }
					break;
				case NodeTypes.V: //Version
                    Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "Version"), UriKind.RelativeOrAbsolute));
					break;
                case NodeTypes.T: //Version
                    if (Hierarchy.ProjectStatus == "Disabled")
                    {
                        if (NoteBLL.CheckForSpecialNotes(Hierarchy.Id)) //give sign on icon if there are special notes
                        {
                            Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "TemplateDisableSpesialsmall"), UriKind.RelativeOrAbsolute));
                            LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "TemplateDisableSpesialBig"), UriKind.RelativeOrAbsolute));
                        }
                        else
                        {
                            Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "TemplateDisable"), UriKind.RelativeOrAbsolute));
                            LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "TemplateDisable"), UriKind.RelativeOrAbsolute));
                        }
                            break;
                    }
                    else
                    {
                        if (NoteBLL.CheckForSpecialNotes(Hierarchy.Id)) //give sign on icon if there are special notes
                        {
                            Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "TemplateSpesialSmall"), UriKind.RelativeOrAbsolute));
                            LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "TemplateSpesialBig"), UriKind.RelativeOrAbsolute));
                        }
                        else
                        {
                            Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/32x32/{0}.png", "Template"), UriKind.RelativeOrAbsolute));
                            LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ExplorerModule;component/Resources/Icons/256x256/{0}.png", "Template"), UriKind.RelativeOrAbsolute));
                        }
                            break;
                    }

			}
			_Children = new ObservableCollection<TreeViewNodeViewModelBase>();
            MessageMediator.Register(this.WorkSpaceId + "OnIsDirtyNodeReceived", new Action<TreeViewNodeViewModelBase>(OnIsDirtyNodeReceived));
            MessageMediator.Register(this.WorkSpaceId + "OnIsCanceledNodeReceived", new Action<TreeViewNodeViewModelBase>(OnIsCanceledNodeReceived));
            MessageMediator.Register(this.WorkSpaceId + "OnIsSelectedNodeReceived", new Action<TreeViewNodeViewModelBase>(OnIsSelectedNodeReceived));
            MessageMediator.Register(this.WorkSpaceId + "OnRefreshReceived", new Action<TreeViewNodeViewModelBase>(OnRefreshNodeReceived));
		}

	#endregion // Constructor

	#region  Node Properties 

		public HierarchyModel _Hierarchy = null;
		public HierarchyModel Hierarchy
		{
			get
			{
				return _Hierarchy;
			}
			set
			{
				_Hierarchy = value;
				RaisePropertyChanged("Hierarchy");
			}
		}

		private TreeViewNodeViewModelBase _Parent;
		public TreeViewNodeViewModelBase Parent
		{
			get
			{
				return _Parent;
			}
			set
			{
				_Parent = value;
				RaisePropertyChanged("Parent");
			}
		}


        private long _Id;
        public long Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
                RaisePropertyChanged("Id");
            }
        }

		private int _Sequence = 0;
		public int Sequence
		{
			get
			{
				return _Sequence;
			}
			set
			{
				_Sequence = value;
				RaisePropertyChanged("Sequence");
			}
		}

		private NodeTypes _NodeType = NodeTypes.F;
		public NodeTypes NodeType
		{
			get
			{
				return _NodeType;
			}
			set
			{
				_NodeType = value;
				RaisePropertyChanged("NodeType");
			}
		}

		private string _Name = "";
		public string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
				RaisePropertyChanged("Name");
                RaisePropertyChanged("NodeData");
			}
		}

		private string _Description = "";
		public string Description
		{
			get
			{
				return _Description;
			}
			set
			{
				_Description = value;
				RaisePropertyChanged("Description");
			}
		}

		private int _NodeLevel = 0;
		public int NodeLevel
		{
			get
			{
				return _NodeLevel;
			}
		}

		private ImageSource _Icon;
		public ImageSource Icon
		{
			get
			{
				return _Icon;
			}
			set
			{
				_Icon = value;
				RaisePropertyChanged("Icon");
                RaisePropertyChanged("NodeIcon");
			}
		}

        private ImageSource _LargeIcon;
        public ImageSource LargeIcon
        {
            get
            {
                return _LargeIcon;
            }
            set
            {
                _LargeIcon = value;
                RaisePropertyChanged("LargeIcon");
            }
        }

		public virtual string NodeData
		{
			get
			{
				return this.Name; //Here you can place (in the actual node implementation) any content you want to see in the Tree for this node...
			}
		}

		public virtual ImageSource NodeIcon
		{
			get
			{
				return this.Icon; //Here you can place (in the actual node implementation) any Icon you want to see in the Tree for this node...
			}
		}

		private ObservableCollection<TreeViewNodeViewModelBase> _Children;
		public ObservableCollection<TreeViewNodeViewModelBase> Children
		{
			get
			{
				return _Children;
			}
			set
			{
				_Children = value;
				RaisePropertyChanged("Children");
			}
		}

        // Filtered ReadOnly collection of the SubFolders within Children property
        public ObservableCollection<TreeViewNodeViewModelBase> SubFolders
        {
            get
            {
                ObservableCollection<TreeViewNodeViewModelBase> _SubFolders = new ObservableCollection<TreeViewNodeViewModelBase>();
                foreach (TreeViewNodeViewModelBase Element in Children)
                {
                    if (Element.NodeType == NodeTypes.F)
                    {
                        _SubFolders.Add(Element);
                    }
                }
                return _SubFolders;
            }
        }

        public Guid WorkSpaceId { get; set; }

        private bool _CanPaste = ProjectsExplorerViewModel.CanPasteAfterCut;
        public bool CanPaste
        {
            get
            {
                return _CanPaste;
            }
            set
            {
                _CanPaste = value;
                RaisePropertyChanged("CanPaste");
            }
        }

	#endregion

	#region  Presentation Members 

	#region  HasLoadedChildren 

		public bool HasLoadedChildren
		{
			get
			{
				return this.Children.Count > 0;
			}
		}

	#endregion // HasLoadedChildren

	#region  IsExpanded 

		public bool IsExpanded
		{
			get
			{
				return _IsExpanded;
			}
			set
			{
				if (value != _IsExpanded)
				{
					_IsExpanded = value;
					RaisePropertyChanged("IsExpanded");
				}
				//Expand all the way up to the root.
				if (_IsExpanded && _Parent != null)
				{
					_Parent.IsExpanded = true;
				}
				//Lazy load the child items, if necessary.
				if (! HasLoadedChildren)
				{
					this.LoadChildren();
				}
			}
		}

	#endregion // IsExpanded

    #region  IsSelected

        private bool IsStatusBarParametersAllowed;
        private static bool IsCanceled = true;
        private static bool IsRelated = false;
        private static bool AllowSave = true;
        private static bool isNewTemplateNowCreated = true;
        
        //Show \ Hide associated Details view.
        public bool IsSelected
        {
            get
            {

                return _IsSelected;
            }
            set
            {
                string nodename = this.Hierarchy.Name;
                bool s = this.IsSelected;

                if (value != _IsSelected)
                {
                    if (IsRelated == false)
                    {
                        if (this._IsSelectedDirtyNode == null)
                        {
                            TreeViewNodeViewModelBase TV3 = this;

                            _IsSelected = value;
                            if (IsStatusBarParametersAllowed)
                            {
                                MessageMediator.NotifyColleagues("StatusBarParameters", null);
                            }
                            IsStatusBarParametersAllowed = true;
                            //Send message to the MainViewModel to clear Statusbar from any previous operation
                            //ContentBLL.isCMFlyoutOpen = false;
                            MessageMediator.NotifyColleagues("HideFlyouts"); //Send message to the MainViewModel -- Fold all Flyouts

                            RaisePropertyChanged("IsSelected");

                            //Display details view.
                            if (value)
                            {
                                //Not Dirty
                                if (this._IsSelectedDirtyNode == null)
                                {
                                    DisplayDetailsView();
                                }

                            }

                            else
                            {
                                CloseDetailsView();
                            }
                        }
                        else  //Dirty
                        {
                            
                            if (IsCanceled)
                            {
                                if (IsSelectedDirtyNode != null && _IsSelectedDirtyNode.NodeType == NodeTypes.T && _IsSelectedDirtyNode.Hierarchy.IsNew && isNewTemplateNowCreated)
                                {
                                    isNewTemplateNowCreated = false;
                                    this._IsSelected = false;
                                    return;
                                }
                                OkCancelMessageWhenDirty();
                            }
                            else
                                IsCanceled = true;

                        }


                    }
                    else
                        IsRelated = false;
                }
                //else
                //{
                //    string msg = HierarchyBLL.GetMessage();
                //    DialogIcons q = DialogIcons.Question;

                //    //MsgBoxService = GetService<IMessageBoxService>();

                //    DialogResults Dialog = MsgBoxService.ShowOkCancel(msg, q);
                //}
                
            }
        }

        private void OkCancelMessageWhenDirty() {
            DialogResults Dialog = MsgBoxService.ShowOkCancel(HierarchyBLL.GetMessage(), DialogIcons.Question);
            switch (Dialog)
            {
                #region OK
                case DialogResults.OK:
                    {
                        dirtyNodeOkClicked();
                        break;
                    }
                #endregion
                #region Canceled
                case DialogResults.Cancel:
                    {
                        dirtyNodeCancelClicked();
                        break;
                    }
                #endregion
            }
        }

        private void dirtyNodeOkClicked() {

            TreeViewNodeViewModelBase TempNode = _IsSelectedDirtyNode;
            TreeViewNodeViewModelBase Th = this;

            //RaisePropertyChanged("IsSelected");


            //Prefrorm refresh to the DirtyNode.
            //MessageMediator.NotifyColleagues(this.WorkSpaceId + "RefreshNode", this);
            this.Hierarchy = HierarchyBLL.GetHierarchyRow(Hierarchy.Id);

            MessageMediator.NotifyColleagues(this.WorkSpaceId + "OnIsDirtyNodeReceived", null);
            if (TempNode != this)
            {
                TempNode.IsSelected = false;
            }
            //CloseDetailsView();
            // MessageMediator.NotifyColleagues(this.WorkSpaceId + "OnIsRefreshDirtyNodeReceived", TempNode);
            //_IsSelected = value;
            this.IsSelected = false;
            isNewTemplateNowCreated = true;
            MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
            MessageMediator.NotifyColleagues("HideFlyouts"); //Send message to the MainViewModel -- Fold all Flyouts
            //ContentBLL.isCMFlyoutOpen = false;
            RaisePropertyChanged("IsSelected");
            CloseDetailsView();

            MessageMediator.NotifyColleagues(this.WorkSpaceId + "OnDirtyDrag", this);
            ProjectDetailsViewModel.LastVersionName = string.Empty;
            NewTemplateViewModel.LastVersionName = string.Empty;
            CloneTemplateViewModel.LastVersionName = string.Empty;
            ProjectDetailsViewModel.projectId = -1;
            if (TempNode.NodeType == NodeTypes.T && TempNode.Hierarchy.IsNew)
            {
                IsRefreshed = false;
                OnRefreshNodeReceived(TempNode.Parent);
            }
        
        }

        private void dirtyNodeCancelClicked() {
            //MessageMediator.NotifyColleagues(this.WorkSpaceId + "OnIsDirtyNodeReceived", null);
            //_IsSelected = value;
            // MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
            // MessageMediator.NotifyColleagues("HideFlyouts"); //Send message to the MainViewModel -- Fold all Flyouts
            //RaisePropertyChanged("IsSelected");
            IsCanceled = false;
            MessageMediator.NotifyColleagues(this.WorkSpaceId + "OnDirtyDrag", this);
        }

        #endregion // IsSelected

	#region  Methods 

	#region  Refresh 

		//Invoked when the node needs to be refreshed. This can include activities for refreshing the node's content, as well as loading the children.
		public abstract void Refresh();

	#endregion // LoadChildren

	#region  LoadChildren 

		//Invoked when the child items need to be loaded on demand. Subclasses can override this to populate the Children collection.
		public abstract void LoadChildren();

	#endregion // LoadChildren

	#region  Display\Close Details View 

		//Invoked when a node has been selected and the detailed view needs to be displayed.
		protected abstract void DisplayDetailsView();

        //Invoked when a node has been dirty and save is selected.
    

		//Sends a message to the MainWindow with the required information to close a details view of the currently selected node
		protected virtual void CloseDetailsView()
		{
			MessageMediator.NotifyColleagues(WorkSpaceId + "CloseDetailsView", this); //Will be returned to the MainWindow signed for this message
		}

	#endregion // Display\Close Details

	#region  Search String 

        public bool NodeDataContainsText(Collection<object> SearchParameters)
		{
            string SearchText = (string)SearchParameters[0];
            Boolean SearchOnName = (Boolean)SearchParameters[1];
            Boolean SearchOnDescription = (Boolean)SearchParameters[2];
            Boolean SearchOnUser = (Boolean)SearchParameters[3];
            Boolean SearchOnProject = (Boolean)SearchParameters[4];
            Boolean SearchOnNotes = (Boolean)SearchParameters[5];
            Boolean SearchOnContent = (Boolean)SearchParameters[6];
            Boolean SearchOnStep = (Boolean)SearchParameters[7];

            if (string.IsNullOrEmpty(SearchText) || string.IsNullOrEmpty(this.NodeData))
			{
				return false;
			}
            if (SearchOnName && this.Name.IndexOf(SearchText, StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                return true;
            }
            if (SearchOnDescription && this.Hierarchy.Description.IndexOf(SearchText, StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                return true;
            }
            if (SearchOnUser && this.Hierarchy.LastUpdateUser.IndexOf(SearchText, StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                return true;
            }
            if (SearchOnProject && this.Hierarchy.Code.IndexOf(SearchText, StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                return true;
            }
            if (SearchOnStep && this.Hierarchy.SelectedStep.IndexOf(SearchText, StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                return true;
            }
            if (SearchOnNotes && (NoteBLL.ListOfHierarchyIdWithNotesSearchPattern.Contains(Convert.ToString(this.Hierarchy.Id))
                    || NoteBLL.ListOfHierarchyIdWithNotesSearchPattern.Contains(Convert.ToString(this.Hierarchy.GroupId))))
            {
                return true;
            }

            if (SearchOnContent && (HierarchyBLL.ListOfHierarchyIdBycontentName.Contains(Convert.ToString(this.Hierarchy.Id))||
                HierarchyBLL.ListOfHierarchyIdBycontentName.Contains(Convert.ToString(this.Hierarchy.GroupId))))
            {
                return true;
            }
            
            return false;
        }

	#endregion

	#endregion

	#endregion // Presentation Members

	#region  Context Menu Commands 
        
    #region  Refresh Command

        private RelayCommand _RefreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (_RefreshCommand == null)
                {
                    _RefreshCommand = new RelayCommand(ExecuteRefreshCommand, CanExecuteRefreshCommand) ;
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
            this.Hierarchy.IsDirty = false;
            this.Hierarchy.VM.IsDirty = false;
            MessageMediator.NotifyColleagues(this.WorkSpaceId + "OnIsDirtyNodeReceived", null);
            MessageMediator.NotifyColleagues(WorkSpaceId + "RefreshNode", this); //Will be returned to the MainWindow signed for this message
        }

        #endregion

	#region  New Node Command 

		private RelayCommand<NodeTypes> _NewNodeCommand;
		public ICommand NewNodeCommand
		{
			get
			{
				if (_NewNodeCommand == null)
				{
                    _NewNodeCommand = new RelayCommand<NodeTypes>((NT) => ExecuteNewNodeCommand(NT), P => CanExecuteNewNodeCommand());
				}
				return _NewNodeCommand;
			}
		}

		private bool CanExecuteNewNodeCommand()
		{
            return Domain.IsPermitted("101");
		}

        private void ExecuteNewNodeCommand(NodeTypes NodeType)
        {
            try
            {
                if (IsSelectedDirtyNode != null) { 
                    //Write message
                    OkCancelMessageWhenDirty();

                }

                if (NodeType == NodeTypes.T)
                {
                    if (!Domain.IsPermitted("150"))
                    {
                        ProjectsExplorerViewModel.ShowErrorAndInfoMessage(106, new object[] { 0 });
                        return;
                    }
                }

                if (NodeType != NodeTypes.T)
                {
                    Domain.PersistenceLayer.BeginTransWithIsolation(IsolationLevel.Serializable);
                }
                MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                if (this.NodeType != NodeTypes.R)
                {
                   String result = HierarchyBLL.LastUpadateCheck(ref _Hierarchy);
                   if (!String.IsNullOrEmpty(result))
                   {
                       Domain.PersistenceLayer.AbortTrans();
                       Object[] ArgsList = new Object[] { 0 };
                       ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(104, ArgsList);
                       return;
                   }
                   
                }
                Collection<object> MessageParameters = new Collection<object>();
                MessageParameters.Add(NodeType);
                MessageParameters.Add(this);
                MessageMediator.NotifyColleagues(WorkSpaceId + "AddChildNode", MessageParameters); //Will be returned to the MainWindow signed for this message

                if (Domain.PersistenceLayer.IsInTransaction())
                {
                    Domain.PersistenceLayer.CommitTrans();
                }
                if (this!= null && this.Hierarchy.Id >0 && NodeType == NodeTypes.F)
                {
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(130, new object[] { 0 });
                }

                if (this != null && this.Hierarchy.Id > 0 && NodeType == NodeTypes.P)
                {
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(115, new object[] { 0 });
                }
            }
            catch (Exception E)
            {
                if (Domain.PersistenceLayer.IsInTransaction())
                {
                    Domain.PersistenceLayer.AbortTrans();
                }
                if (E.Message == "DB Error")
                {
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
                }
            }
        }

	#endregion

	#region  Cut\Copy\Paste Node Command 

		private RelayCommand<NodeOperations> _CutCopyPasteNodeCommand;
		public ICommand CutCopyPasteNodeCommand
		{
			get
			{
				if (_CutCopyPasteNodeCommand == null)
				{
                    _CutCopyPasteNodeCommand = new RelayCommand<NodeOperations>((NO) => ExecuteCutCopyPasteNodeCommand(NO), P => CanExecuteCutCopyPasteNodeCommand());
				}
				return _CutCopyPasteNodeCommand;
			}
		}

		private bool CanExecuteCutCopyPasteNodeCommand()
		{
            CanPaste = ProjectsExplorerViewModel.CanPasteAfterCut; //make paste visible\ not visible if cut occurred earlier
            return Domain.IsPermitted("102");
		}

        private void ExecuteCutCopyPasteNodeCommand(NodeOperations NodeOper)
        {
            if (NodeOper.Equals(NodeOperations.Cut))
                ProjectsExplorerViewModel.CanPasteAfterCut = true; //make paste visible after cut
            else
                ProjectsExplorerViewModel.CanPasteAfterCut = false; //make paste not visible

            try
            {
                Domain.PersistenceLayer.BeginTransWithIsolation(IsolationLevel.Serializable);
                MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                String result = HierarchyBLL.LastUpadateCheck(ref _Hierarchy);
                if (!String.IsNullOrEmpty(result))
                {
                    Domain.PersistenceLayer.AbortTrans();
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(104, ArgsList);
                    return;
                }
                Collection<object> MessageParameters = new Collection<object>();
                MessageParameters.Add(NodeOper);
                MessageParameters.Add(this);
                MessageMediator.NotifyColleagues(WorkSpaceId + "CutCopyPasteNode", MessageParameters); //Will be returned to the MainWindow signed for this message
                Domain.PersistenceLayer.CommitTrans();
            }
            catch (Exception E)
            {
                Domain.PersistenceLayer.AbortTrans();
                if (E.Message == "DB Error")
                {
                    Object[] ArgsList = new Object[] { 0 };
                    ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
                }
            }
        }

	#endregion

	#endregion

     private bool _IsSelectedTree;
     public bool IsSelectedTree
        {
            get
            {
                return _IsSelectedTree;
            }
            set
            {


                _IsSelectedTree = value;
                MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                RaisePropertyChanged("IsSelectedTree");
                //
                //Display details view.
                if (value)
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
                        RaisePropertyChanged("IsSelectedTree");
                        MessageMediator.NotifyColleagues(WorkSpaceId + "OnShowTemplateCloneDetailsReceived", this);
                        MessageMediator.NotifyColleagues(WorkSpaceId + "OnShowCloneTemplateHeaderReceived", this);
                        MessageMediator.NotifyColleagues(WorkSpaceId + "OnShowCloneDetailsReceived", this);
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
                        else
                        {
                            Object[] ArgsList = new Object[] { 0 };
                            ExplorerModule.ProjectsExplorerViewModel.ShowErrorAndInfoMessage(105, ArgsList);
                        }
                    }
                  
                }
                else
                {
                    //CloseDetailsView();
                }
            }
            //}
        }

     private bool _IsExpandedTree;
     public bool IsExpandedTree
     {
         get
         {
             return _IsExpandedTree;
         }
         set
         {
             if (value != _IsExpandedTree)
             {
                 _IsExpandedTree = value;
                 RaisePropertyChanged("IsExpandedTree");
             }
             //Expand all the way up to the root.
             if (_IsExpandedTree && _Parent != null)
             {
                 _Parent.IsExpandedTree = true;
             }
             //Lazy load the child items, if necessary.
             if (!HasLoadedChildren)
             {
                 this.LoadChildren();
             }
         }
     }

     #region IsDirtyNode

     private TreeViewNodeViewModelBase _IsSelectedDirtyNode = null;
     public TreeViewNodeViewModelBase IsSelectedDirtyNode
     {
         get
         {
             return _IsSelectedDirtyNode;
         }
         set
         {
             _IsSelectedDirtyNode = value;
         }

     }


     private void OnIsDirtyNodeReceived(TreeViewNodeViewModelBase TV)
     {
         this._IsSelectedDirtyNode = TV;
     }
     private void OnIsCanceledNodeReceived(TreeViewNodeViewModelBase TV)
     {
         if (TV != null)
         {
             AllowSave = false;

             if(TV.Hierarchy.IsClonedRelated && TV.Hierarchy.GroupId == -1)
                 IsRelated = true;
         }
         else
             AllowSave = true;

     }

     private void OnIsSelectedNodeReceived(TreeViewNodeViewModelBase TV)
     {
         TV.IsSelected = true;
         RaisePropertyChanged("IsSelected");
     }
        #endregion IsDirtyNode

    public static bool IsRefreshed = false;
     private void OnRefreshNodeReceived(TreeViewNodeViewModelBase TV)
     {
         if (!IsRefreshed)
         {
             TV.Hierarchy.IsDirty = false;
             TV.Hierarchy.VM.IsDirty = false;
             MessageMediator.NotifyColleagues(this.WorkSpaceId + "OnIsDirtyNodeReceived", null);
             MessageMediator.NotifyColleagues(WorkSpaceId + "RefreshNode", TV); //Will be returned to the MainWindow signed for this message
             IsRefreshed = true;
         }
     }



    }

} //end of root namespace