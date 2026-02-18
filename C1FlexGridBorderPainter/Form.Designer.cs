namespace C1FlexGridBorderPainter
{
  partial class Form
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;


    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.c1FlexGrid = new C1.Win.FlexGrid.C1FlexGrid();
      this.checkBoxExtendLastCol = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)this.c1FlexGrid).BeginInit();
      this.SuspendLayout();
      // 
      // c1FlexGrid
      // 
      this.c1FlexGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
      this.c1FlexGrid.ColumnInfo = "10,1,0,0,0,-1,Columns:";
      this.c1FlexGrid.Location = new System.Drawing.Point(26, 10);
      this.c1FlexGrid.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.c1FlexGrid.Name = "c1FlexGrid";
      this.c1FlexGrid.Size = new System.Drawing.Size(894, 467);
      this.c1FlexGrid.TabIndex = 0;
      // 
      // checkBoxExtendLastCol
      // 
      this.checkBoxExtendLastCol.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
      this.checkBoxExtendLastCol.AutoSize = true;
      this.checkBoxExtendLastCol.Location = new System.Drawing.Point(30, 488);
      this.checkBoxExtendLastCol.Name = "checkBoxExtendLastCol";
      this.checkBoxExtendLastCol.Size = new System.Drawing.Size(100, 19);
      this.checkBoxExtendLastCol.TabIndex = 1;
      this.checkBoxExtendLastCol.Text = "ExtendLastCol";
      this.checkBoxExtendLastCol.UseVisualStyleBackColor = true;
      this.checkBoxExtendLastCol.CheckedChanged += this.checkBoxExtendLastCol_CheckedChanged;
      // 
      // Form
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(933, 519);
      this.Controls.Add(this.checkBoxExtendLastCol);
      this.Controls.Add(this.c1FlexGrid);
      this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.Name = "Form";
      this.Text = "Form1";
      ((System.ComponentModel.ISupportInitialize)this.c1FlexGrid).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private C1.Win.FlexGrid.C1FlexGrid c1FlexGrid;
    private System.Windows.Forms.CheckBox checkBoxExtendLastCol;
  }
}

