using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace ReferenceTableWriter
{
	/// <summary>
	/// This class is an implementation of the 'IComparer' interface.
	/// </summary>
	public class ListViewColumnSorter : IComparer
	{
		/// <summary>
		/// Specifies the column to be sorted
		/// </summary>
		private int ColumnToSort;
		/// <summary>
		/// Specifies the order in which to sort (i.e. 'Ascending').
		/// </summary>
		private SortOrder OrderOfSort;
		/// <summary>
		/// Case insensitive comparer object
		/// </summary>
		private CaseInsensitiveComparer ObjectCompare;

		/// <summary>
		/// Class constructor.  Initializes various elements
		/// </summary>
		public ListViewColumnSorter()
		{
			// Initialize the column to '0'
			ColumnToSort = 0;

			// Initialize the sort order to 'none'
			OrderOfSort = SortOrder.Ascending;	// Miriam

			// Initialize the CaseInsensitiveComparer object
			ObjectCompare = new CaseInsensitiveComparer();
		}

		/// <summary>
		/// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
		/// </summary>
		/// <param name="x">First object to be compared</param>
		/// <param name="y">Second object to be compared</param>
		/// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
		public int Compare(object x, object y)
		{
			int compareResult;
			ListViewItem listviewX, listviewY;

			// Cast the objects to be compared to ListViewItem objects
			listviewX = (ListViewItem)x;
			listviewY = (ListViewItem)y;

			// Compare the two items
			compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);

			// Calculate correct return value based on object comparison
			if (OrderOfSort == SortOrder.Ascending)
			{
				// Ascending sort is selected, return normal result of compare operation
				return compareResult;
			}
			else if (OrderOfSort == SortOrder.Descending)
			{
				// Descending sort is selected, return negative result of compare operation
				return (-compareResult);
			}
			else
			{
				// Return '0' to indicate they are equal
				return 0;
			}
		}

		/// <summary>
		/// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
		/// </summary>
		public int SortColumn
		{
			set
			{
				ColumnToSort = value;
			}
			get
			{
				return ColumnToSort;
			}
		}

		/// <summary>
		/// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
		/// </summary>
		public SortOrder Order
		{
			set
			{
				OrderOfSort = value;
			}
			get
			{
				return OrderOfSort;
			}
		}

	}

	public class ListViewUtilities
	{
		public const string ListViewTypeName = "System.Windows.Forms.ListView";
		public static void ColumnClick(object sender, ColumnClickEventArgs e, ListViewColumnSorter cs)
		{
			var lv = (ListView)sender;

			// Determine if clicked column is already the column that is being sorted.
			if (e.Column == cs.SortColumn)
			{
				// Reverse the current sort direction for this column.
				cs.Order = cs.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
			}
			else
			{
				// Set the column number that is to be sorted; default to ascending.
				cs.SortColumn = e.Column;
				cs.Order = SortOrder.Ascending;
			}

			// Perform the sort with these new sort options.
			lv.Sort();
		}

		public static void DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e, Brush b)
		{
			var lv = (ListView)sender;

			e.Graphics.FillRectangle(b, e.Bounds);
			e.DrawText();
			e.Graphics.DrawRectangle(Pens.Black, e.Bounds);

		}

		public static void KeyCtrlADown(object sender, KeyEventArgs e)
		{
			var lv = (ListView)sender;

			if (e.KeyCode == Keys.A && e.Control)
				foreach (ListViewItem item in lv.Items)
					item.Selected = true;
		}

		public static void ItemDrag(object sender, ItemDragEventArgs e)
		{
			var listView = (ListView)sender;
			if (e.Button == MouseButtons.Right) return;
			listView.DoDragDrop(listView, DragDropEffects.Copy | DragDropEffects.Move);
		}

		public static void ListViewDragEnter(object source, DragEventArgs e)
		{
			e.Effect = ItemsAreFrom(source, e) ? DragDropEffects.Copy : DragDropEffects.None;
		}

		public static bool ItemsAreFrom(object source, DragEventArgs e)
		{
			var valid = false;
			var validName = ((ListView)source).Name;
			if (e.Data.GetDataPresent(ListViewTypeName))
			{
				var control = (ListView)e.Data.GetData(ListViewTypeName);
				var controlName = control.Name;
				valid = ((controlName == validName) && (control.SelectedItems.Count > 0));
			}
			return valid;
		}

	}
}
