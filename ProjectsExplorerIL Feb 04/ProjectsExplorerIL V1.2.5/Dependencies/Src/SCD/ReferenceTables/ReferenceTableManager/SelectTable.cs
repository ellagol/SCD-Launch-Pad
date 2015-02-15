using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DatabaseProvider;
using ReferenceTableReader;
using ReferenceTableWriter;

namespace ReferenceTableManager
{
	public enum OperationType
	{
		EditData,
		EditMetaData,
		CreateMetaData,
		UpdateTableName
	}

	public partial class SelectTable : Form
	{
		private string User;
		private string Applic;
		private string ConnectionString;
		private OperationType OperationType;
		internal string TableId;
		public SelectTable(string user, string application, string connectionString, OperationType operationType, string tableId)
		{
			InitializeComponent();
			User = user;
			Applic = application;
			ConnectionString = connectionString;
			OperationType = operationType;
			TableId = tableId;
			var referenceTable = new ReferenceTable(connectionString, application);
			if ((operationType == OperationType.CreateMetaData) || ((operationType == OperationType.UpdateTableName) && (!string.IsNullOrEmpty(tableId))))
			{
				var tableNames = referenceTable.GetAllTableNames();
				foreach (var item in tableNames)
					comboBoxSelect.Items.Add(item);
				if (operationType == OperationType.CreateMetaData)
				{
					labelId.Visible = true;
					textBoxId.Visible = true;
					textBoxId.Text = "";
				}
				else
				{
					labelId.Visible = false;
					textBoxId.Visible = false;
				}
			}
			else
			{
				var allItems = referenceTable.GetReferenceTableTableDictionary(TableInfo.TableName, null);
				foreach (var item in allItems)
					comboBoxSelect.Items.Add(item[TableInfo.TableNameColumn].ToString());
				labelId.Visible = false;
				textBoxId.Visible = false;
			}
			comboBoxSelect.SelectedIndex = 0;
		}

		private void buttonNext_Click(object sender, EventArgs e)
		{
			var tableName = comboBoxSelect.Text;
			string tableId;
			bool success;
			GetTableId(tableName, out tableId, out success);
			if (!success)
				return;

			if (OperationType == OperationType.EditData)
			{
				var referenceTableForm = new ManageReferenceTables(User, Applic, ConnectionString, tableId);
				Hide();
				referenceTableForm.ShowDialog();
				Show();
			}
			else if (OperationType == OperationType.EditMetaData)
			{
				var referenceTable = new ReferenceTable(ConnectionString, Applic);
				var tableNames = referenceTable.GetAllTableNames();
				if (!tableNames.Contains(tableName))
				{
					MessageHandler.MsgReport.DisplayInformation("Metadata no Table", new List<string> { }, "");
					//MessageBox.Show("Table does not exist but metadata exists.", Applic, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				var setColumnsForm = new SetColumns(Applic, ConnectionString, tableId, comboBoxSelect.Text, false);
				Hide();
				setColumnsForm.ShowDialog();
				Show();
			}
			else if (OperationType == OperationType.CreateMetaData)
			{
				if (textBoxId.Text == "")
				{
					MessageHandler.MsgReport.DisplayInformation("Missing ID", new List<string> { }, "");
//					MessageBox.Show("Please enter ID.", Applic, MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}
				tableId = textBoxId.Text;
				if (SaveReferenceTableInfo.ValueAlreadyExists(ConnectionString, Applic, TableInfo.TableName, TableInfo.TableIdColumn, tableId) ||
					SaveReferenceTableInfo.ValueAlreadyExists(ConnectionString, Applic, TableColumnsData.TableName, TableColumnsData.TableIdColumn, tableId))
				{
					MessageHandler.MsgReport.DisplayInformation("ID Already Exists", new List<string> { }, "");
					//MessageBox.Show("ID already exists. Choose another.", Applic, MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}


				var setColumnsForm = new SetColumns(Applic, ConnectionString, tableId, comboBoxSelect.Text, true);
				Hide();
				if (setColumnsForm.ShowDialog() != DialogResult.Cancel)
					Close();
				else
					Show();
			}
			else // OperationType.UpdateTableName
			{
				var selectTable = new SelectTable(User, Applic, ConnectionString, OperationType, tableId);
				Hide();
				selectTable.buttonBack.Visible = false;
				selectTable.buttonNext.Visible = false;
				selectTable.buttonOk.Left = (selectTable.Width - selectTable.buttonOk.Width)/2;
				selectTable.buttonOk.Visible = true;
				if (selectTable.ShowDialog() != DialogResult.Cancel)
					Close();
				else
					Show();
			}
		}

		private void buttonOk_Click(object sender, EventArgs e)
		{
			var newTableName = comboBoxSelect.Text;
			SaveReferenceTableInfo.UpdateReferenceTableName(ConnectionString, Applic, newTableName, TableId);
			DialogResult = DialogResult.OK;
			Close();
		}

		private void GetTableId(string tableName, out string tableId, out bool success)
		{
			success = true;
			var tableIdCommand = string.Format("Select {0} From {1} Where {2} = '{3}'", TableInfo.TableIdColumn, TableInfo.TableName, TableInfo.TableNameColumn, tableName);
			var db = new DBprovider(ConnectionString, Applic);
			db.OpenConnection();
			tableId = db.ExecuteStringCommand(tableIdCommand);
			db.CloseConnection();
			if (string.IsNullOrEmpty(tableId))
				tableId = "";
			if (string.IsNullOrEmpty(tableId) && (OperationType != OperationType.CreateMetaData))
			{
				MessageHandler.MsgReport.DisplayInformation("ID Empty", new List<string> { comboBoxSelect.Text }, "");
				success = false;
			}

			else if (!string.IsNullOrEmpty(tableId) && (OperationType == OperationType.CreateMetaData))
			{
				MessageHandler.MsgReport.DisplayInformation("Metadata Already Exists", new List<string> { comboBoxSelect.Text }, "");
				success = false;
			}
		}
	}
}
