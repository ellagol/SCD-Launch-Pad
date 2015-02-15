using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using MessagesManager;
using ReferenceTableReader;
using ReferenceTableWriter;

namespace ReferenceTableManager
{
	public partial class ReferenceTableManager : Form
	{
		private string Applic;

		public ReferenceTableManager(string user, string application, string connectionString)
		{
			InitializeComponent();
			Applic = application;
			comboBoxSelectOption.SelectedIndex = 0;
			MessageHandler.MsgReport = new MessageReport(user, application, connectionString);
		}

		private void buttonSelectTable_Click(object sender, EventArgs e)
		{
			var operationType = (OperationType)comboBoxSelectOption.SelectedIndex;
			var selectTable = new SelectTable(textBoxUser.Text, Applic, richTextBoxConnectionString.Text, operationType, "");
			Hide();
			selectTable.buttonBack.Visible = true;
			selectTable.buttonNext.Visible = true;
			selectTable.buttonOk.Visible = false;
			selectTable.ShowDialog();
			Show();
		}
	}
}
