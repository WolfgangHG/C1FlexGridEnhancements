using C1.Win.FlexGrid;

namespace C1FlexGrid6AutoSizeRowHeightToLast
{
  partial class FormCustomMerge
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
      this.buttonAutoSizeRows = new Button();
      this.buttonHeightToLast = new Button();
      this.c1FlexGrid1 = new C1FlexGrid();
      this.buttonReset = new Button();
      this.checkBoxFourthDataRowVisible = new CheckBox();
      this.checkBoxFifthDataRowVisible = new CheckBox();
      ((System.ComponentModel.ISupportInitialize)this.c1FlexGrid1).BeginInit();
      this.SuspendLayout();
      // 
      // buttonAutoSizeRows
      // 
      this.buttonAutoSizeRows.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.buttonAutoSizeRows.Location = new Point(810, 14);
      this.buttonAutoSizeRows.Margin = new Padding(4, 3, 4, 3);
      this.buttonAutoSizeRows.Name = "buttonAutoSizeRows";
      this.buttonAutoSizeRows.Size = new Size(110, 27);
      this.buttonAutoSizeRows.TabIndex = 1;
      this.buttonAutoSizeRows.Text = "AutoSizeRows";
      this.buttonAutoSizeRows.UseVisualStyleBackColor = true;
      this.buttonAutoSizeRows.Click += this.buttonAutoSizeRows_Click;
      // 
      // buttonHeightToLast
      // 
      this.buttonHeightToLast.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.buttonHeightToLast.Location = new Point(810, 47);
      this.buttonHeightToLast.Margin = new Padding(4, 3, 4, 3);
      this.buttonHeightToLast.Name = "buttonHeightToLast";
      this.buttonHeightToLast.Size = new Size(110, 27);
      this.buttonHeightToLast.TabIndex = 2;
      this.buttonHeightToLast.Text = "Height to last";
      this.buttonHeightToLast.UseVisualStyleBackColor = true;
      this.buttonHeightToLast.Click += this.buttonHeightToLast_Click;
      // 
      // c1FlexGrid1
      // 
      this.c1FlexGrid1.AllowSorting = AllowSortingEnum.None;
      this.c1FlexGrid1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.c1FlexGrid1.ColumnInfo = "6,1,0,0,0,-1,Columns:3{Width:295;}\t";
      this.c1FlexGrid1.Location = new Point(13, 12);
      this.c1FlexGrid1.Margin = new Padding(4, 3, 4, 3);
      this.c1FlexGrid1.Name = "c1FlexGrid1";
      this.c1FlexGrid1.Rows.Count = 9;
      this.c1FlexGrid1.Size = new Size(789, 669);
      this.c1FlexGrid1.TabIndex = 0;
      // 
      // buttonReset
      // 
      this.buttonReset.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.buttonReset.Location = new Point(810, 81);
      this.buttonReset.Margin = new Padding(4, 3, 4, 3);
      this.buttonReset.Name = "buttonReset";
      this.buttonReset.Size = new Size(110, 27);
      this.buttonReset.TabIndex = 3;
      this.buttonReset.Text = "Reset";
      this.buttonReset.UseVisualStyleBackColor = true;
      this.buttonReset.Click += this.buttonReset_Click;
      // 
      // checkBoxFourthDataRowVisible
      // 
      this.checkBoxFourthDataRowVisible.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.checkBoxFourthDataRowVisible.AutoSize = true;
      this.checkBoxFourthDataRowVisible.Checked = true;
      this.checkBoxFourthDataRowVisible.CheckState = CheckState.Checked;
      this.checkBoxFourthDataRowVisible.Location = new Point(810, 114);
      this.checkBoxFourthDataRowVisible.Name = "checkBoxFourthDataRowVisible";
      this.checkBoxFourthDataRowVisible.Size = new Size(146, 19);
      this.checkBoxFourthDataRowVisible.TabIndex = 4;
      this.checkBoxFourthDataRowVisible.Text = "Fourth data row visible";
      this.checkBoxFourthDataRowVisible.UseVisualStyleBackColor = true;
      this.checkBoxFourthDataRowVisible.CheckedChanged += this.checkBoxFourthDataRowVisible_CheckedChanged;
      // 
      // checkBoxFifthDataRowVisible
      // 
      this.checkBoxFifthDataRowVisible.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.checkBoxFifthDataRowVisible.AutoSize = true;
      this.checkBoxFifthDataRowVisible.Checked = true;
      this.checkBoxFifthDataRowVisible.CheckState = CheckState.Checked;
      this.checkBoxFifthDataRowVisible.Location = new Point(810, 139);
      this.checkBoxFifthDataRowVisible.Name = "checkBoxFifthDataRowVisible";
      this.checkBoxFifthDataRowVisible.Size = new Size(135, 19);
      this.checkBoxFifthDataRowVisible.TabIndex = 5;
      this.checkBoxFifthDataRowVisible.Text = "Fifth data row visible";
      this.checkBoxFifthDataRowVisible.UseVisualStyleBackColor = true;
      this.checkBoxFifthDataRowVisible.CheckedChanged += this.checkBoxFifthDataRowVisible_CheckedChanged;
      // 
      // FormCustomMerge
      // 
      this.AutoScaleDimensions = new SizeF(7F, 15F);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(992, 693);
      this.Controls.Add(this.checkBoxFifthDataRowVisible);
      this.Controls.Add(this.checkBoxFourthDataRowVisible);
      this.Controls.Add(this.buttonReset);
      this.Controls.Add(this.buttonHeightToLast);
      this.Controls.Add(this.buttonAutoSizeRows);
      this.Controls.Add(this.c1FlexGrid1);
      this.Margin = new Padding(4, 3, 4, 3);
      this.Name = "FormCustomMerge";
      this.Text = "Form1";
      ((System.ComponentModel.ISupportInitialize)this.c1FlexGrid1).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    #endregion

    private C1FlexGrid c1FlexGrid1;
    private System.Windows.Forms.Button buttonAutoSizeRows;
    private System.Windows.Forms.Button buttonHeightToLast;
    private System.Windows.Forms.Button buttonReset;
    private CheckBox checkBoxFourthDataRowVisible;
    private CheckBox checkBoxFifthDataRowVisible;
  }
}

