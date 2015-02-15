using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ATSBusinessObjects.ContentMgmtModels;
using Infra.MVVM;

namespace ContentMgmtModule
{
	public abstract class CMTreeViewNodeViewModelBase : ViewModelBase
	{

	    #region  Data 

		private bool _IsExpanded;
		private bool _IsSelected;
		private bool _IsDirty;

		protected Infra.MVVM.MessengerService MessageMediator = new MessengerService();
		private IMessageBoxService MsgBoxService = null;

	    #endregion // Data

	    #region  Constructor 

        protected CMTreeViewNodeViewModelBase(Guid workSpaceId, CMTreeNode TN) : this(workSpaceId, TN, null)
		{
		}

        protected CMTreeViewNodeViewModelBase(Guid workSpaceId, CMTreeNode TN, CMTreeViewNodeViewModelBase ParentNode)
		{            
			//Message Box Service
			MsgBoxService = GetService<IMessageBoxService>();
			//Messenger Service (to exchange messages between VMs)
			MessageMediator = GetService<MessengerService>();
			//Assign Model properties to VM properties
            WorkSpaceId = workSpaceId;

            TreeNode = TN;

            ID = TN.ID;
			
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
			Name = TreeNode.Name;
            
            //if (TreeNode.ID == 0)
            //{
            //    TreeNode.TreeNodeType = ATSBusinessObjects.ContentMgmtModels.TreeNodeObjectType.Root;
            //}

            switch (TreeNode.TreeNodeType.ToString())
			{
				case "Root": //Root Node
                    Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png", "Root"), UriKind.RelativeOrAbsolute));
					break;
                case "Folder": //Folder
                    Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png", "Folder"), UriKind.RelativeOrAbsolute));
                    LargeIcon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ContentMgmtModule;component/Resources/Icons/256x256/{0}.png", "Folder"), UriKind.RelativeOrAbsolute));
					break;
                case "Content": //Content
                    if (((CMContentModel)(TreeNode)).IconPath != "")
                    {
                        try
                        {
                            Icon = new BitmapImage(new Uri((((CMContentModel)(TreeNode)).IconPath), UriKind.RelativeOrAbsolute));
                        }
                        catch (Exception e) { Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png", "Content"), UriKind.RelativeOrAbsolute)); }
                    }
                    else
                    {
                        Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png", "Content"), UriKind.RelativeOrAbsolute));
                    }
					break;
                case "ContentVersion": //Version
                    {
                    //if (((CMVersionModel)(TreeNode)).Status.Icon != "")
                    //{
                        try
                        {
                            switch (((CMVersionModel)(TreeNode)).id_ContentVersionStatus)
                            {
                                case "Ac":
                                    Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png", "ActiveContentVersion"), UriKind.RelativeOrAbsolute));
                                    break;

                                case "Edit":
                                    Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png", "EditContentVersion"), UriKind.RelativeOrAbsolute));
                                    break;

                                case "Ret":
                                    Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png", "RetContentVersion"), UriKind.RelativeOrAbsolute));
                                    break;

                            }                         
                        }
                        catch (Exception e) { Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png", "ActiveContentVersion"), UriKind.RelativeOrAbsolute)); }
                    }
                    break;
            
                    //else
                    //{
                    //    Icon = new BitmapImage(new Uri(string.Format("pack://application:,,,/ContentMgmtModule;component/Resources/Icons/32x32/{0}.png", "ActiveContentVersion"), UriKind.RelativeOrAbsolute));
                    //}
                    //break;
			}
           
       
			_Children = new ObservableCollection<CMTreeViewNodeViewModelBase>();
            MessageMediator.Register(this.WorkSpaceId + "OnIsDirtyNodeReceived", new Action<CMTreeViewNodeViewModelBase>(OnIsDirtyNodeReceived));
		}

        #endregion // Constructor

	    #region  Node Properties 
    
        private CMTreeNode _TreeNode = null;
        public CMTreeNode TreeNode
		{
			get
			{
                return _TreeNode;
			}
			set
			{
                _TreeNode = value;
                RaisePropertyChanged("TreeNode");
			}
		}

		private CMTreeViewNodeViewModelBase _Parent;
		public CMTreeViewNodeViewModelBase Parent
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

        private long _ID = -1;
        public long ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
                RaisePropertyChanged("ID");
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
                RaisePropertyChanged("NodeData");
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

        private bool _IsView = false;
        public bool IsView
        {
            get
            {
                return _IsView;
            }
            set
            {
                _IsView = value;
                RaisePropertyChanged("IsView");
            }
        }


        private bool _IsUpdate = false;
        public bool IsUpdate
        {
            get
            {
                return _IsUpdate;
            }
            set
            {
                _IsUpdate = value;
                RaisePropertyChanged("IsUpdate");
            }
        }

        private bool _IsDelete = true;
        public bool IsDelete
        {
            get
            {
                return _IsDelete;
            }
            set
            {
                _IsDelete = value;
                RaisePropertyChanged("IsDelete");
            }
        }

        private bool _updateModeEditor;
        public bool UpdateModeEditor
        {
            get
            {
                return _updateModeEditor;
            }
            set
            {
                _updateModeEditor = value;
                RaisePropertyChanged("UpdateModeEditor");
            }
        }

        private bool _userGroupUpdatePermission;
        public bool UserGroupUpdatePermission
        {
            get
            {
                return _userGroupUpdatePermission;
            }
            set
            {
                _userGroupUpdatePermission = value;
                RaisePropertyChanged("UserGroupUpdatePermission");
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

		private ObservableCollection<CMTreeViewNodeViewModelBase> _Children;
		public ObservableCollection<CMTreeViewNodeViewModelBase> Children
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

        public Guid WorkSpaceId { get; set; }

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
              
        private static bool IsCanceled = true;
        private static bool ctrlClicked = false;
        private static CMTreeViewNodeViewModelBase DirtyNode = null;
        private static CMTreeViewNodeViewModelBase ctrlClickedNode = null;
        //Show \ Hide associated Details view.
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    if (value != _IsSelected)
                    {
                        if (IsCanceled)
                        {
                            DialogResults Dialog;
                            if(this.IsSelectedDirtyNode != null) //Dirty 
                            {
                                Dialog = MsgBoxService.ShowOkCancel(ATSBusinessLogic.HierarchyBLL.GetMessage(), DialogIcons.Question);
                            }
                            else
                            {// Not Dirty - no need to ask
                                Dialog = DialogResults.OK;
                            }
                            switch (Dialog)
                            {
                                #region  OK
                                case DialogResults.OK:
                                    {
                                        MessageMediator.NotifyColleagues(this.WorkSpaceId + "OnIsDirtyNodeReceived", null);;
                                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                                        {
                                            if (ctrlClicked)
                                            {
                                                ctrlClicked = false;
                                                unSelectCtrlClickedNode();
                                            }
                                            _IsSelected = value;
                                            unSelectOldNoned();
                                            MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                                            MessageMediator.NotifyColleagues("HideFlyouts"); //Send message to the MainViewModel -- Fold all Flyouts
                                            RaisePropertyChanged("IsSelected");
                                            if (value)
                                            {
                                                DisplayDetailsView();
                                            }
                                            else
                                            {
                                                CloseDetailsView();
                                            }
                                        }
                                        break;
                                    }
                                #endregion
                                #region Cancel
                                case DialogResults.Cancel:
                                    {
                                        //Stay on this node and remember it.(to remove selection)
                                        IsCanceled = false;
                                        if (DirtyNode == null)
                                        {
                                            DirtyNode = this;
                                        }
                                        break;
                                    }
                                #endregion
                            }
                        }
                        else//isCanceled
                        {
                            IsCanceled = true;

                        }
                    }//isSelected
                }
                else
                { //control clicked
                    if (!ctrlClicked)
                    {
                        ctrlClickedNode = this;
                    }
                    ctrlClicked = true;
                }
            }
        }

        //to handle dirty unSelect
        private void unSelectOldNoned()
        {
            if (DirtyNode != null)
            {
                DirtyNode.IsSelected = false;
                DirtyNode = null;
            }
        }

        //To handle double selections on ctrl click
        private void unSelectCtrlClickedNode()
        {
            if (ctrlClickedNode != null)
            {
                ctrlClickedNode.IsSelected = false;
                ctrlClickedNode = null;
            }
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

            //Sends a message to the MainWindow with the required information to close a details view of the currently selected node
            protected virtual void CloseDetailsView()
            {
              //MessageMediator.NotifyColleagues(WorkSpaceId + "CloseDetailsView", this); //Will be returned to the MainWindow signed for this message
            }

        #endregion // Display\Close Details

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
            MessageMediator.NotifyColleagues(WorkSpaceId + "RefreshNode", this); //Will be returned to the MainWindow signed for this message
        }

        #endregion

	    #endregion    

        #region  Selected Linked Tree

        private bool _IsSelectedLinkedTree;
        public bool IsSelectedLinkedTree
        {
            get
            {
                return _IsSelectedLinkedTree;
            }
            set
            {
                _IsSelectedLinkedTree = value;
                MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
                RaisePropertyChanged("IsSelectedLinkedTree");
                //
                //Display details view.
                if (value)
                {
                    RaisePropertyChanged("IsSelectedLinkedTree");
                    MessageMediator.NotifyColleagues(WorkSpaceId + "OnShowContentTabDetailsReceived", this);
                }
                else
                {
                    //CloseDetailsView();
                }
            }
            //}
        }

        #endregion

        #region  New Node Command

        private RelayCommand<TreeNodeObjectType> _NewNodeCommand;
        public ICommand NewNodeCommand
        {
            get
            {
                if (_NewNodeCommand == null)
                {
                    _NewNodeCommand = new RelayCommand<TreeNodeObjectType>((NT) => ExecuteNewNodeCommand(NT), P => CanExecuteNewNodeCommand());
                }
                return _NewNodeCommand;
            }
        }

        private bool CanExecuteNewNodeCommand()
        {
            return true;
        }

        private void ExecuteNewNodeCommand(TreeNodeObjectType NodeType)
        {
            //Disable making new Nodes if Dirty.
            if(this.IsSelectedDirtyNode != null)
            {
                DialogResults Dialog = MsgBoxService.ShowOkCancel(ATSBusinessLogic.HierarchyBLL.GetMessage(), DialogIcons.Question);
                switch (Dialog)
                {
                    case DialogResults.OK:
                        {
                            MessageMediator.NotifyColleagues(this.WorkSpaceId + "OnIsDirtyNodeReceived", null);;
                            break;
                        }
                    case DialogResults.Cancel:
                        return;
                        break;
                }

            }
            //Not Dirty
            MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
            Collection<object> MessageParameters = new Collection<object>();
            MessageParameters.Add(NodeType);
            MessageParameters.Add(this);
            MessageMediator.NotifyColleagues(WorkSpaceId + "AddChildNode", MessageParameters); //Will be returned to the MainWindow signed for this message
        }

        #endregion     

        #region DirtyNode
        private CMTreeViewNodeViewModelBase _IsSelectedDirtyNode = null;
        public CMTreeViewNodeViewModelBase IsSelectedDirtyNode
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
        private void OnIsDirtyNodeReceived(CMTreeViewNodeViewModelBase TV)
        {
            this._IsSelectedDirtyNode = TV;
        }

        #endregion
    }

} //end of root namespace