using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ATSBusinessObjects.ContentMgmtModels;
using Infra.MVVM;

namespace ExplorerModule
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

		//Show \ Hide associated Details view.
		public bool IsSelected
		{
			get
			{
				return _IsSelected;
			}
			set
			{
				if (value != _IsSelected)
				{
					_IsSelected = value;
                    MessageMediator.NotifyColleagues("StatusBarParameters", null); //Send message to the MainViewModel to clear Statusbar from any previous operation
					RaisePropertyChanged("IsSelected");
				}
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

    }

} //end of root namespace