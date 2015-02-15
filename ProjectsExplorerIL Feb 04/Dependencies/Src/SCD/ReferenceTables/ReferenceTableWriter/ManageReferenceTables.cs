using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DatabaseProvider;
using LastUpdateUtilities;
using MessagesManager;
using ReferenceTableReader;
using TraceExceptionWrapper;

namespace ReferenceTableWriter
{
	public partial class ManageReferenceTables : Form
	{
		private bool WritePermission = false;
		Dictionary<string, Dictionary<string, object>> AllItems;
		private ListViewColumnSorter LvwColumnSorter;
		private ReferenceTable.TableProperties TableProperties;

		public ManageReferenceTables(string user, string application, string connectionString, string tableId)
		{
			InitializeComponent();
			var str = LastUpdateWrite.DllName;
			DatabaseConnection.ConnectionString = connectionString;
			DatabaseConnection.DBaseRTable = new DBprovider(connectionString, application);
			ApplicationPermission.SetPermission(user, application, DatabaseConnection.ConnectionString);
			WritePermission = ApplicationPermission.GetWritePermission();
			var computer = Environment.MachineName;
			LastUpdateUtil.Initialize(LastUpdateWrite.Details, user, computer, application);
			MessageHandling.MsgReport = new MessageReport(user, application, connectionString);

			LvwColumnSorter = new ListViewColumnSorter();
			listViewTable.ListViewItemSorter = LvwColumnSorter;

			var referenceTable = new ReferenceTable(connectionString, "");
			referenceTable.PrepareTableProperties(out TableProperties, tableId);
			groupBoxTable.Text = TableProperties.TableName;
			Text = "Manage " + TableProperties.TableName;
			FillDictionary();
			FillListView();
			buttonExit.Left = Left + Width/2;
		}

		public override sealed string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}

		public void FillDictionary()
		{

			var referenceTable = new ReferenceTable(DatabaseConnection.ConnectionString, LastUpdateWrite.Details.Application);
			var allItems = referenceTable.GetReferenceTableTableDictionary(TableProperties.QueryTable);
			AllItems = new Dictionary<string, Dictionary<string, object>>();
			foreach (var i in allItems)
				AllItems.Add(i[TableProperties.TitleKey].ToString(), i);
		}

		public void FillListView()
		{
			listViewTable.BeginUpdate();
			listViewTable.Clear();
			listViewTable.Columns.Add(TableProperties.TitleKey, 200, HorizontalAlignment.Left);
			listViewTable.Columns.Add(TableProperties.TitleTime, 200, HorizontalAlignment.Left);
			listViewTable.Columns.Add(TableProperties.TitleUser, 200, HorizontalAlignment.Left);
			listViewTable.Columns.Add(TableProperties.TitleComputer, 200, HorizontalAlignment.Left);
			listViewTable.Columns.Add(TableProperties.TitleApplication, 200, HorizontalAlignment.Left);
			if (TableProperties.NumDataColumns > 0)
				for (var j = 0; j < TableProperties.NumDataColumns - 1; j++)
					listViewTable.Columns.Add(TableProperties.ColumnNames[j], 200, HorizontalAlignment.Left);
			listViewTable.Columns.Add(TableProperties.ColumnNames[TableProperties.NumDataColumns - 1], 1000, HorizontalAlignment.Left);

			//Add the items to the ListView.
			foreach (var c in AllItems)
			{
				var i = new ListViewItem(c.Value[TableProperties.TitleKey].ToString());
				i.SubItems.Add(new ListViewItem.ListViewSubItem {Name = TableProperties.TitleTime, Text = c.Value[TableProperties.TitleTime].ToString()});
				i.SubItems.Add(new ListViewItem.ListViewSubItem {Name = TableProperties.TitleUser, Text = c.Value[TableProperties.TitleUser].ToString()});
				i.SubItems.Add(new ListViewItem.ListViewSubItem {Name = TableProperties.TitleComputer, Text = c.Value[TableProperties.TitleComputer].ToString()});
				i.SubItems.Add(new ListViewItem.ListViewSubItem {Name = TableProperties.TitleApplication, Text = c.Value[TableProperties.TitleApplication].ToString()});
				if (TableProperties.NumDataColumns > 0)
					for (var j = 0; j < TableProperties.NumDataColumns; j++)
						i.SubItems.Add(new ListViewItem.ListViewSubItem {Name = TableProperties.ColumnNames[j], Text = c.Value[TableProperties.ColumnNames[j]].ToString()});
				listViewTable.Items.Add(i);
			}

			listViewTable.EndUpdate();
		}

		private void listViewTable_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			ListViewUtilities.ColumnClick(sender, e, LvwColumnSorter);
		}

		private void listViewTable_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
		{
			ListViewUtilities.DrawColumnHeader(sender, e, Brushes.Lavender);
		}

		private void listViewTable_DrawItem(object sender, DrawListViewItemEventArgs e)
		{
			e.DrawDefault = true;
		}

		private void listViewTable_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
		{
			e.DrawDefault = true;
		}

		private void listViewTable_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			var index = (from ListViewItem item in listViewTable.SelectedItems select item.Index).FirstOrDefault();
			var id = listViewTable.Items[index].Text;
			var repeat = true;

			var properties = new Dictionary<string, PropertiesEditing.PropertiesType>();
			var record = AllItems[id];
			var titles = new string[] { TableProperties.TitleKey, TableProperties.TitleTime, TableProperties.TitleUser, TableProperties.TitleComputer, TableProperties.TitleApplication };
			foreach (var title in titles)
				properties.Add(title, new PropertiesEditing.PropertiesType(record[title].ToString()) { ReadOnly = true, ColumnWidth = TableProperties.CommonColumnInfo[title].ColumnLength });
			if (TableProperties.NumDataColumns > 0)
				for (var i = 0; i < TableProperties.NumDataColumns; i++)
				{
					var title = TableProperties.ColumnNames[i];
					var colInfo = TableProperties.ColumnInfo[title];
					properties.Add(title, new PropertiesEditing.PropertiesType(record[title].ToString()) { ColumnWidth = colInfo.ColumnLength, IsError = colInfo.DefaultNotLegal, ErrorValue = colInfo.DefaultValue});

				}
			var pe = new PropertiesEditing(TableProperties.TableName + " Properties", properties);

			while (repeat)
			{
				repeat = false;
				if (pe.ShowDialog() == DialogResult.OK)
				{
					try
					{
						/*** get old item properties ***/
						var currentItem = AllItems[id];
						var changedItem = pe.GetChangeableData();

						/*** if there were changes, update in database with new properties ***/
						changedItem.Add(TableProperties.TitleTime, DateTime.Now);
						changedItem.Add(TableProperties.TitleUser, LastUpdateWrite.Details.User);
						changedItem.Add(TableProperties.TitleComputer, LastUpdateWrite.Details.Computer);
						changedItem.Add(TableProperties.TitleApplication, LastUpdateWrite.Details.Application);
						TableWriter.UpdateRecord(TableProperties, id, (DateTime)currentItem[TableProperties.TitleTime], changedItem);

						foreach (var c in changedItem)
						{
							AllItems[id][c.Key] = changedItem[c.Key];
							listViewTable.Items[index].SubItems[c.Key].Text = changedItem[c.Key].ToString();
						}
					}
					catch (TraceException ex)
					{
						MessageHandling.MsgReport.DisplayError(ex);
						repeat = true;
					}
				}
			}
		}

		private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var repeat = true;

			var properties = new Dictionary<string, PropertiesEditing.PropertiesType>();
			var title = TableProperties.TitleKey;
			var colInfo = TableProperties.CommonColumnInfo[title];
			properties.Add(title, new PropertiesEditing.PropertiesType(colInfo.DefaultValue) { ColumnWidth = colInfo.ColumnLength, IsError = colInfo.DefaultNotLegal });
			if (TableProperties.NumDataColumns > 0)
				for (var i = 0; i < TableProperties.NumDataColumns; i++)
				{
					title = TableProperties.ColumnNames[i];
					colInfo = TableProperties.ColumnInfo[title];
					properties.Add(title, new PropertiesEditing.PropertiesType(colInfo.DefaultValue) { ColumnWidth = colInfo.ColumnLength, IsError = colInfo.DefaultNotLegal });

				}
			var pe = new PropertiesEditing(TableProperties.TableName + " Properties", properties);

			while (repeat)
			{
				repeat = false;
				if (pe.ShowDialog() == DialogResult.OK)
				{
					try
					{
						/*** if there were changes, update in database with new properties ***/
						var changedItem = pe.GetChangeableData();
						changedItem.Add(TableProperties.TitleTime, DateTime.Now);
						changedItem.Add(TableProperties.TitleUser, LastUpdateWrite.Details.User);
						changedItem.Add(TableProperties.TitleComputer, LastUpdateWrite.Details.Computer);
						changedItem.Add(TableProperties.TitleApplication, LastUpdateWrite.Details.Application);
						TableWriter.AddRecord(TableProperties, changedItem);
						var key = changedItem[TableProperties.TitleKey].ToString();
						AllItems.Add(key, changedItem);

						var i = new ListViewItem(key);
						i.SubItems.Add(new ListViewItem.ListViewSubItem { Name = TableProperties.TitleTime, Text = AllItems[key][TableProperties.TitleTime].ToString() });
						i.SubItems.Add(new ListViewItem.ListViewSubItem { Name = TableProperties.TitleUser, Text = AllItems[key][TableProperties.TitleUser].ToString() });
						i.SubItems.Add(new ListViewItem.ListViewSubItem { Name = TableProperties.TitleComputer, Text = AllItems[key][TableProperties.TitleComputer].ToString() });
						i.SubItems.Add(new ListViewItem.ListViewSubItem { Name = TableProperties.TitleApplication, Text = AllItems[key][TableProperties.TitleApplication].ToString() });
						if (TableProperties.NumDataColumns > 0)
							for (var j = 0; j < TableProperties.NumDataColumns; j++)
								i.SubItems.Add(new ListViewItem.ListViewSubItem { Name = TableProperties.ColumnNames[j], Text = AllItems[key][TableProperties.ColumnNames[j]].ToString() });
						listViewTable.Items.Add(i);
					}
					catch (TraceException ex)
					{
						MessageHandling.MsgReport.DisplayError(ex);
						repeat = true;
					}
				}
			}

		}
	}
}
