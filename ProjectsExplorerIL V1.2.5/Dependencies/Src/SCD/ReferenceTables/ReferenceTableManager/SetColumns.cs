using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DatabaseProvider;
using ReferenceTableManager.Properties;
using ReferenceTableReader;
using ReferenceTableWriter;

namespace ReferenceTableManager
{
	public partial class SetColumns : Form
	{
		private string Applic;
		private string ConnectionString;
		private string TableId;
		private ReferenceTable.TableProperties TableProperties;
		private ReferenceTable.TableProperties NewTableProperties = new ReferenceTable.TableProperties();
		private int CurrentColumn;
		private List<string> RemovedColNames;
		private int TotalNumColumns;
		private string TableName;
		private bool IsCreate = false;

		public SetColumns(string application, string connectionString, string tableId, string tableName, bool isCreate)
		{
			InitializeComponent();
			Applic = application;
			ConnectionString = connectionString;
			IsCreate = isCreate;
			TableId = tableId;
			RemovedColNames = new List<string>();
			var referenceTable = new ReferenceTable(connectionString, "");
			if (!IsCreate)
			{
				referenceTable.PrepareTableProperties(out TableProperties, tableId);
				TableName = TableProperties.TableName;
			}
			else
				TableName = tableName;
			var colNames = referenceTable.GetTableColumnNames(TableName);
			TotalNumColumns = colNames.Count;
			if (TotalNumColumns <= ReferenceTable.NumCommonColumns)
			{
				MessageHandler.MsgReport.DisplayInformation("No Data Columns", new List<string> { tableName }, "");
				Close();
				return;
			}
			NewTableProperties.TableName = tableName;
			NewTableProperties.NumDataColumns = TotalNumColumns - ReferenceTable.NumCommonColumns;
			NewTableProperties.ColumnNames = new List<string>();
			NewTableProperties.ColumnInfo = new Dictionary<string, ReferenceTable.OneColumnInfo>();
			
			for (int i = 0; i < NewTableProperties.NumDataColumns; i++)
			{
				NewTableProperties.ColumnNames.Add("");
			}
			
			comboBoxColumnSelect.Items.Clear();
			foreach (var c in colNames)
				comboBoxColumnSelect.Items.Add(c);
			CurrentColumn = -1;
			buttonNext_Click(null, new EventArgs());
		}

		private void UpdateDisplay()
		{
			string labelTxt;
			var comboSelectedText = comboBoxColumnSelect.Items[0].ToString();
			switch (CurrentColumn)
			{
				case 0:
					labelTxt = "Select Key column:";
					if (!IsCreate)
						comboSelectedText = TableProperties.TitleKey;
					break;
				case 1:
					labelTxt = "Select DateTime column:";
					if (!IsCreate)
						comboSelectedText = TableProperties.TitleTime;
					break;
				case 2:
					labelTxt = "Select User column:";
					if (!IsCreate)
						comboSelectedText = TableProperties.TitleUser;
					break;
				case 3:
					labelTxt = "Select Computer column:";
					if (!IsCreate)
						comboSelectedText = TableProperties.TitleComputer;
					break;
				case 4:
					labelTxt = "Select Application column:";
					if (!IsCreate)
						comboSelectedText = TableProperties.TitleApplication;
					break;
				default:
					labelTxt = string.Format("Select Data {0} column:", CurrentColumn - 4);
					if (!IsCreate)
						comboSelectedText = TableProperties.ColumnNames[CurrentColumn - 5];
					break;
			}
			labelSelectColumn.Text = labelTxt;
			if (comboBoxColumnSelect.Items.Count > 0)
			{
				if (comboBoxColumnSelect.Items.Contains(comboSelectedText))
				{
					comboBoxColumnSelect.Text = comboSelectedText;
					if (CurrentColumn > (ReferenceTable.NumCommonColumns - 1))
					{
						checkBoxUnique.Checked = (!IsCreate) && TableProperties.ColumnInfo[comboSelectedText].Unique;
						checkBoxErrorValueExists.Checked = (!IsCreate) && TableProperties.ColumnInfo[comboSelectedText].DefaultNotLegal;
						textBoxIllegalValue.Text = (IsCreate) ? "" : TableProperties.ColumnInfo[comboSelectedText].DefaultValue;
						textBoxIllegalValue.Enabled = checkBoxErrorValueExists.Checked;
					}
				}
				else
					comboBoxColumnSelect.SelectedIndex = 0;
			}
		}

		private void SetNew(string previousChoice)
		{
			switch (CurrentColumn - 1)
			{
				case 0:
					NewTableProperties.TitleKey = previousChoice;
					break;
				case 1:
					NewTableProperties.TitleTime = previousChoice;
					break;
				case 2:
					NewTableProperties.TitleUser = previousChoice;
					break;
				case 3:
					NewTableProperties.TitleComputer = previousChoice;
					break;
				case 4:
					NewTableProperties.TitleApplication = previousChoice;
					break;
				default:
					NewTableProperties.ColumnNames[CurrentColumn - ReferenceTable.NumCommonColumns - 1] = previousChoice;
					if (!NewTableProperties.ColumnInfo.ContainsKey(previousChoice))
						NewTableProperties.ColumnInfo.Add(previousChoice, new ReferenceTable.OneColumnInfo());
					NewTableProperties.ColumnInfo[previousChoice].Unique = checkBoxUnique.Checked;
					NewTableProperties.ColumnInfo[previousChoice].DefaultNotLegal = checkBoxErrorValueExists.Checked;
					NewTableProperties.ColumnInfo[previousChoice].DefaultValue = textBoxIllegalValue.Text;
					break;
			}
		}

		private void buttonNext_Click(object sender, EventArgs e)
		{
			if ((CurrentColumn == -1) || ((comboBoxColumnSelect.Text != "") && (CurrentColumn < TotalNumColumns)))
				CurrentColumn++;
			if (CurrentColumn > 0)
			{
				var previousChoice = comboBoxColumnSelect.Text;
				comboBoxColumnSelect.Items.Remove(previousChoice);
				RemovedColNames.Add(previousChoice);
				SetNew(previousChoice);
				if (CurrentColumn == ReferenceTable.NumCommonColumns)
				{
					checkBoxUnique.Visible = true;
					checkBoxErrorValueExists.Visible = true;
					labelIllegalValue.Visible = true;
					textBoxIllegalValue.Visible = true;
				}
			}
			if (CurrentColumn == TotalNumColumns)
			{
				if (MessageHandler.MsgReport.DisplayConfirmation("Confirm Save Changes", new List<string> { }, "") == DialogResult.Yes)
				//if (MessageBox.Show("Save changes?", Applic, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					SaveReferenceTableInfo.UpdateReferenceTable(ConnectionString, Applic, NewTableProperties, TableId, IsCreate);
				else
					MessageHandler.MsgReport.DisplayInformation("Cancelling Operation", new List<string> { }, "");
				DialogResult = DialogResult.OK;
				Close();
				return;
			}
			UpdateDisplay();
		}

		private void buttonPrevious_Click(object sender, EventArgs e)
		{
			CurrentColumn--;
			if (CurrentColumn > -1)
			{
				var previousChoice = RemovedColNames[RemovedColNames.Count - 1];
				comboBoxColumnSelect.Items.Add(previousChoice);
				RemovedColNames.Remove(previousChoice);
				if (CurrentColumn == ReferenceTable.NumCommonColumns - 1)
				{
					checkBoxUnique.Visible = false;
					checkBoxErrorValueExists.Visible = false;
					labelIllegalValue.Visible = false;
					textBoxIllegalValue.Visible = false;
				}
				UpdateDisplay();
			}
			else
			{
				DialogResult = DialogResult.Cancel;
				Close();
			}
		}

		private void checkBoxErrorValueExists_CheckedChanged(object sender, EventArgs e)
		{
			textBoxIllegalValue.Enabled = checkBoxErrorValueExists.Checked;
		}

	}
}
