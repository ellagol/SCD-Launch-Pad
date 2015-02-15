namespace ReferenceTableManager
{
	partial class SetColumns
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
			this.buttonNext = new System.Windows.Forms.Button();
			this.comboBoxColumnSelect = new System.Windows.Forms.ComboBox();
			this.labelSelectColumn = new System.Windows.Forms.Label();
			this.buttonBack = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.checkBoxUnique = new System.Windows.Forms.CheckBox();
			this.checkBoxErrorValueExists = new System.Windows.Forms.CheckBox();
			this.textBoxIllegalValue = new System.Windows.Forms.TextBox();
			this.labelIllegalValue = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonNext
			// 
			this.buttonNext.AutoSize = true;
			this.buttonNext.BackColor = System.Drawing.Color.SlateGray;
			this.buttonNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.buttonNext.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.buttonNext.Location = new System.Drawing.Point(424, 391);
			this.buttonNext.Name = "buttonNext";
			this.buttonNext.Size = new System.Drawing.Size(90, 30);
			this.buttonNext.TabIndex = 0;
			this.buttonNext.Text = "&Next >>";
			this.buttonNext.UseVisualStyleBackColor = false;
			this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
			// 
			// comboBoxColumnSelect
			// 
			this.comboBoxColumnSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.comboBoxColumnSelect.FormattingEnabled = true;
			this.comboBoxColumnSelect.Location = new System.Drawing.Point(357, 57);
			this.comboBoxColumnSelect.Name = "comboBoxColumnSelect";
			this.comboBoxColumnSelect.Size = new System.Drawing.Size(318, 28);
			this.comboBoxColumnSelect.TabIndex = 1;
			// 
			// labelSelectColumn
			// 
			this.labelSelectColumn.AutoSize = true;
			this.labelSelectColumn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.labelSelectColumn.Location = new System.Drawing.Point(90, 60);
			this.labelSelectColumn.Name = "labelSelectColumn";
			this.labelSelectColumn.Size = new System.Drawing.Size(113, 20);
			this.labelSelectColumn.TabIndex = 2;
			this.labelSelectColumn.Text = "Select column:";
			// 
			// buttonBack
			// 
			this.buttonBack.AutoSize = true;
			this.buttonBack.BackColor = System.Drawing.Color.SlateGray;
			this.buttonBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.buttonBack.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.buttonBack.Location = new System.Drawing.Point(278, 391);
			this.buttonBack.Name = "buttonBack";
			this.buttonBack.Size = new System.Drawing.Size(90, 30);
			this.buttonBack.TabIndex = 3;
			this.buttonBack.Text = "<< &Back";
			this.buttonBack.UseVisualStyleBackColor = false;
			this.buttonBack.Click += new System.EventHandler(this.buttonPrevious_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.AutoSize = true;
			this.buttonCancel.BackColor = System.Drawing.Color.SlateGray;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.buttonCancel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.buttonCancel.Location = new System.Drawing.Point(640, 391);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(90, 30);
			this.buttonCancel.TabIndex = 4;
			this.buttonCancel.Text = "&Cancel";
			this.buttonCancel.UseVisualStyleBackColor = false;
			// 
			// checkBoxUnique
			// 
			this.checkBoxUnique.AutoSize = true;
			this.checkBoxUnique.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.checkBoxUnique.Location = new System.Drawing.Point(357, 128);
			this.checkBoxUnique.Name = "checkBoxUnique";
			this.checkBoxUnique.Size = new System.Drawing.Size(242, 24);
			this.checkBoxUnique.TabIndex = 5;
			this.checkBoxUnique.Text = "Check this if values are unique";
			this.checkBoxUnique.UseVisualStyleBackColor = true;
			this.checkBoxUnique.Visible = false;
			// 
			// checkBoxErrorValueExists
			// 
			this.checkBoxErrorValueExists.AutoSize = true;
			this.checkBoxErrorValueExists.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.checkBoxErrorValueExists.Location = new System.Drawing.Point(357, 195);
			this.checkBoxErrorValueExists.Name = "checkBoxErrorValueExists";
			this.checkBoxErrorValueExists.Size = new System.Drawing.Size(276, 24);
			this.checkBoxErrorValueExists.TabIndex = 6;
			this.checkBoxErrorValueExists.Text = "Check this if there is an illegal value";
			this.checkBoxErrorValueExists.UseVisualStyleBackColor = true;
			this.checkBoxErrorValueExists.Visible = false;
			this.checkBoxErrorValueExists.CheckedChanged += new System.EventHandler(this.checkBoxErrorValueExists_CheckedChanged);
			// 
			// textBoxIllegalValue
			// 
			this.textBoxIllegalValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.textBoxIllegalValue.Location = new System.Drawing.Point(357, 262);
			this.textBoxIllegalValue.Name = "textBoxIllegalValue";
			this.textBoxIllegalValue.Size = new System.Drawing.Size(238, 26);
			this.textBoxIllegalValue.TabIndex = 13;
			this.textBoxIllegalValue.Visible = false;
			// 
			// labelIllegalValue
			// 
			this.labelIllegalValue.AutoSize = true;
			this.labelIllegalValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.labelIllegalValue.Location = new System.Drawing.Point(90, 265);
			this.labelIllegalValue.Name = "labelIllegalValue";
			this.labelIllegalValue.Size = new System.Drawing.Size(95, 20);
			this.labelIllegalValue.TabIndex = 14;
			this.labelIllegalValue.Text = "Illegal value:";
			this.labelIllegalValue.Visible = false;
			// 
			// SetColumns
			// 
			this.AcceptButton = this.buttonNext;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(200)))), ((int)(((byte)(210)))));
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(792, 473);
			this.Controls.Add(this.labelIllegalValue);
			this.Controls.Add(this.textBoxIllegalValue);
			this.Controls.Add(this.checkBoxErrorValueExists);
			this.Controls.Add(this.checkBoxUnique);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonBack);
			this.Controls.Add(this.labelSelectColumn);
			this.Controls.Add(this.comboBoxColumnSelect);
			this.Controls.Add(this.buttonNext);
			this.Name = "SetColumns";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Set Columns";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonNext;
		private System.Windows.Forms.ComboBox comboBoxColumnSelect;
		private System.Windows.Forms.Label labelSelectColumn;
		private System.Windows.Forms.Button buttonBack;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.CheckBox checkBoxUnique;
		private System.Windows.Forms.CheckBox checkBoxErrorValueExists;
		private System.Windows.Forms.TextBox textBoxIllegalValue;
		private System.Windows.Forms.Label labelIllegalValue;
	}
}