using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ReferenceTableWriter
{
	public partial class PropertiesEditing : Form
	{
		public class PropertiesType
		{
			public object PossibleValues { get; set; }
			public bool ReadOnly { get; set; }
			public string InitialValue { get; set; }
			public string FinalValue { get; set; }
			public bool IsError { get; set; }
			public string ErrorValue { get; set; }
			public int ColumnWidth { get; set; }

			public PropertiesType(string initialValue)
			{
				InitialValue = initialValue;
				ErrorValue = initialValue;
				FinalValue = initialValue;
				ReadOnly = false;
				IsError = false;
				ErrorValue = "";
				ColumnWidth = 0;
			}

		}

		private Dictionary<string, PropertiesType> Properties;

		private string Caption;
		private bool Changed = false;


		public PropertiesEditing(string caption, Dictionary<string, PropertiesType> properties)
		{
			InitializeComponent();
			Caption = caption;
			Properties = new Dictionary<string, PropertiesType>();
			foreach(var p in properties)
				Properties.Add(p.Key, p.Value);
			Text = Caption;
			var varStringArray = new string[1];
			var index = 0;
			foreach (var p in Properties)
			{
				if ((p.Value.PossibleValues != null) && (p.Value.PossibleValues.GetType() == varStringArray.GetType()))
					AddDataGridDataRow(p.Key, (string[])p.Value.PossibleValues);
				else
					AddDataGridDataRow(p.Key, p.Value.ReadOnly);
				dataGridViewProperties.Rows[index].Cells[1].Value = p.Value.InitialValue;
				index++;
			}
			buttonOk.Enabled = false;
		}

		public override sealed string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}

		private void AddDataGridDataRow(string title, bool readOnly)
		{
			dataGridViewProperties.Rows.Add(title, "");
			if (readOnly)
			{
				dataGridViewProperties.Rows[dataGridViewProperties.Rows.Count - 1].Cells[1].ReadOnly = true;
				dataGridViewProperties.Rows[dataGridViewProperties.Rows.Count - 1].Cells[0].Style.ForeColor = Color.DarkGray;
				dataGridViewProperties.Rows[dataGridViewProperties.Rows.Count - 1].Cells[1].Style.ForeColor = Color.DarkGray;
			}
		}

		private void AddDataGridDataRow(string title, string[] values)
		{
			dataGridViewProperties.Rows.Add(title, "");
			DataGridViewComboBoxCell cbo = new DataGridViewComboBoxCell { DataSource = values };
			
			dataGridViewProperties.Rows[dataGridViewProperties.Rows.Count - 1].Cells[1] = cbo;
		}

		private void buttonOk_Click(object sender, EventArgs e)
		{
			dataGridViewProperties.CommitEdit(DataGridViewDataErrorContexts.Commit);

			foreach (DataGridViewRow r in dataGridViewProperties.Rows)
			{
				string key = r.Cells[0].Value.ToString();
				Properties[key].FinalValue = r.Cells[1].Value.ToString();
			}

			foreach (var p in Properties)
			{
				if (p.Value.FinalValue != p.Value.InitialValue)
				{
					Changed = true;
					break;
				}
			}
			DialogResult = Changed ? DialogResult.OK : DialogResult.Cancel;
		}

		public string GetValue(string key)
		{
			return Properties[key].FinalValue;
		}

		public Dictionary<string, object> GetChangeableData()
		{
			var properties = new Dictionary<string, object>();
			foreach(var p in Properties)
				if (!p.Value.ReadOnly)
					properties.Add(p.Key, p.Value.FinalValue);
			return properties;
		}

		private void dataGridViewProperties_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			ColourInvalid(e.RowIndex);
		}

		private void ColourInvalid(int rowIndex)
		{
			if (rowIndex > -1)
			{
				var key = dataGridViewProperties.Rows[rowIndex].Cells[0].Value.ToString();
				if (!Properties[key].ReadOnly)
				{
					if ((Properties[key].IsError) &&
					    (dataGridViewProperties.Rows[rowIndex].Cells[1].Value.ToString() == Properties[key].ErrorValue))
					{
						dataGridViewProperties.Rows[rowIndex].Cells[1].Style.ForeColor = Color.Black;
						dataGridViewProperties.Rows[rowIndex].Cells[1].Style.BackColor = Color.MistyRose;
					}
					else
					{
						dataGridViewProperties.Rows[rowIndex].Cells[1].Style.ForeColor = Color.Black;
						dataGridViewProperties.Rows[rowIndex].Cells[1].Style.BackColor = Color.White;
					}
				}
			}
		}

		private void dataGridViewProperties_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
		{
			if (dataGridViewProperties.CurrentRow != null)
			{
				var key = dataGridViewProperties.CurrentRow.Cells[0].Value.ToString();
				if (e.Control is TextBox)
				{
					if (Properties[key].ColumnWidth > 0)
						((TextBox) e.Control).MaxLength = Properties[key].ColumnWidth;
				}
				//if (Properties[key].IsError)
				//{
					ComboBox cmb = e.Control as ComboBox;
					if (cmb != null)
					{
						cmb.SelectedIndexChanged -= cmb_SelectedIndexChanged;
						cmb.SelectedIndexChanged += cmb_SelectedIndexChanged;
					}
					else
					{
						TextBox txt = e.Control as TextBox;
						txt.TextChanged -= txt_TextChanged;
						txt.TextChanged += txt_TextChanged;
					}
				//}
			}
		}

		void cmb_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox cmb = (ComboBox) sender;
			var selectedText = ((DataGridViewComboBoxEditingControl) sender).EditingControlFormattedValue.ToString();
			if (dataGridViewProperties.CurrentRow != null)
			{
				var rowIndex = dataGridViewProperties.CurrentRow.Index;
				var key = dataGridViewProperties.Rows[rowIndex].Cells[0].Value.ToString();
				if ((Properties[key].IsError) && (selectedText == Properties[key].ErrorValue))
				{
					((ComboBox)sender).BackColor = Color.MistyRose;
					buttonOk.Enabled = false;
				}
				else
				{
					((ComboBox) sender).BackColor = Color.White;
					buttonOk.Enabled = true;
				}
			}
		}

		void txt_TextChanged(object sender, EventArgs e)
		{
			TextBox txt = (TextBox) sender;
			var selectedText = ((DataGridViewTextBoxEditingControl)sender).EditingControlFormattedValue.ToString();
			if (dataGridViewProperties.CurrentRow != null)
			{
				var rowIndex = dataGridViewProperties.CurrentRow.Index;
				var key = dataGridViewProperties.Rows[rowIndex].Cells[0].Value.ToString();
				if ((Properties[key].IsError) && (selectedText == Properties[key].ErrorValue))
				{
					((TextBox)sender).BackColor = Color.MistyRose;
					buttonOk.Enabled = false;
				}
				else
				{
					((TextBox) sender).BackColor = Color.White;
					var enable = true;
					foreach (DataGridViewRow row in dataGridViewProperties.Rows)
					{
						key = row.Cells[0].Value.ToString();
						var val = (row.Index == rowIndex ? selectedText : row.Cells[1].Value.ToString());
						if ((Properties[key].IsError) && (val == Properties[key].ErrorValue))
						{
							enable = false;
							break;
						}
					}
					if (enable)
						buttonOk.Enabled = true;
				}
			}
		}

	}

}
