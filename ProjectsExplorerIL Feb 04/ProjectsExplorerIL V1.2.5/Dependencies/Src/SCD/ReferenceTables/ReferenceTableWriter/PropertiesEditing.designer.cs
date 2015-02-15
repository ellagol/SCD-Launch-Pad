namespace ReferenceTableWriter
{
	partial class PropertiesEditing
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle53 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle54 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle52 = new System.Windows.Forms.DataGridViewCellStyle();
			this.dataGridViewProperties = new System.Windows.Forms.DataGridView();
			this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// dataGridViewProperties
			// 
			this.dataGridViewProperties.AllowUserToAddRows = false;
			this.dataGridViewProperties.AllowUserToDeleteRows = false;
			this.dataGridViewProperties.AllowUserToResizeColumns = false;
			this.dataGridViewProperties.AllowUserToResizeRows = false;
			this.dataGridViewProperties.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
			this.dataGridViewProperties.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
			this.dataGridViewProperties.BackgroundColor = System.Drawing.Color.LightSteelBlue;
			this.dataGridViewProperties.ColumnHeadersHeight = 40;
			this.dataGridViewProperties.ColumnHeadersVisible = false;
			this.dataGridViewProperties.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Type,
            this.Value});
			dataGridViewCellStyle53.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle53.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle53.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			dataGridViewCellStyle53.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle53.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle53.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle53.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridViewProperties.DefaultCellStyle = dataGridViewCellStyle53;
			this.dataGridViewProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridViewProperties.Location = new System.Drawing.Point(0, 0);
			this.dataGridViewProperties.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.dataGridViewProperties.Name = "dataGridViewProperties";
			dataGridViewCellStyle54.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle54.BackColor = System.Drawing.Color.CornflowerBlue;
			dataGridViewCellStyle54.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			dataGridViewCellStyle54.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle54.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle54.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle54.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridViewProperties.RowHeadersDefaultCellStyle = dataGridViewCellStyle54;
			this.dataGridViewProperties.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.dataGridViewProperties.Size = new System.Drawing.Size(409, 257);
			this.dataGridViewProperties.TabIndex = 1;
			this.dataGridViewProperties.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewProperties_CellValueChanged);
			this.dataGridViewProperties.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridViewProperties_EditingControlShowing);
			// 
			// Type
			// 
			this.Type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.Type.HeaderText = "Type";
			this.Type.Name = "Type";
			this.Type.ReadOnly = true;
			this.Type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.Type.Width = 5;
			// 
			// Value
			// 
			this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			dataGridViewCellStyle52.NullValue = "\"\"";
			this.Value.DefaultCellStyle = dataGridViewCellStyle52;
			this.Value.HeaderText = "Value";
			this.Value.Name = "Value";
			this.Value.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.Value.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// splitContainer1
			// 
			this.splitContainer1.BackColor = System.Drawing.Color.LightSteelBlue;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.dataGridViewProperties);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.buttonCancel);
			this.splitContainer1.Panel2.Controls.Add(this.buttonOk);
			this.splitContainer1.Size = new System.Drawing.Size(409, 327);
			this.splitContainer1.SplitterDistance = 257;
			this.splitContainer1.TabIndex = 2;
			// 
			// buttonCancel
			// 
			this.buttonCancel.AutoSize = true;
			this.buttonCancel.BackColor = System.Drawing.SystemColors.Info;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatAppearance.BorderColor = System.Drawing.Color.White;
			this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonCancel.Location = new System.Drawing.Point(245, 9);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 30);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "&Cancel";
			this.buttonCancel.UseVisualStyleBackColor = false;
			// 
			// buttonOk
			// 
			this.buttonOk.AutoSize = true;
			this.buttonOk.BackColor = System.Drawing.SystemColors.Info;
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonOk.ForeColor = System.Drawing.Color.Black;
			this.buttonOk.Location = new System.Drawing.Point(88, 9);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(75, 30);
			this.buttonOk.TabIndex = 2;
			this.buttonOk.Text = "&Ok";
			this.buttonOk.UseVisualStyleBackColor = false;
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// PropertiesEditing
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.LightSteelBlue;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(409, 327);
			this.Controls.Add(this.splitContainer1);
			this.Name = "PropertiesEditing";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Properties";
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewProperties)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridViewProperties;
		private System.Windows.Forms.DataGridViewTextBoxColumn Type;
		private System.Windows.Forms.DataGridViewTextBoxColumn Value;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
	}
}