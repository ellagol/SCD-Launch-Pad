using System.Linq;
using System.Xml.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using System.Collections.ObjectModel;
using System.ComponentModel;
using Infra.MVVM;

namespace ExplorerModule
{
	public class TreeViewNodeViewModel : TreeViewNodeViewModelBase
	{


		public TreeViewNodeViewModel(HierarchyModel Hierarchy): this(Hierarchy, null)
		{
		}

		public TreeViewNodeViewModel(HierarchyModel Hierarchy, TreeViewNodeViewModelBase ParentNode) : base(Hierarchy, ParentNode)
		{
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
		}

	#region  Context Menu Commands (Specific to this Node Type; others appear in the Base Class) 

	#endregion

	}

} //end of root namespace