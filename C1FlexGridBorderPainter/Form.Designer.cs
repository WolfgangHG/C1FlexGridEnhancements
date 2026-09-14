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
      this.c1FlexGridBorderPainter = new C1.Win.FlexGrid.C1FlexGrid();
      this.checkBoxExtendLastCol = new System.Windows.Forms.CheckBox();
      this.c1FlexGridCellStyleBorders = new C1.Win.FlexGrid.C1FlexGrid();
      this.labelBorderPainter = new System.Windows.Forms.Label();
      this.labelBorderWithStyles = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)this.c1FlexGridBorderPainter).BeginInit();
      ((System.ComponentModel.ISupportInitialize)this.c1FlexGridCellStyleBorders).BeginInit();
      this.SuspendLayout();
      // 
      // c1FlexGridBorderPainter
      // 
      this.c1FlexGridBorderPainter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
      this.c1FlexGridBorderPainter.ColumnInfo = "10,1,0,0,0,-1,Columns:";
      this.c1FlexGridBorderPainter.Location = new System.Drawing.Point(13, 35);
      this.c1FlexGridBorderPainter.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.c1FlexGridBorderPainter.Name = "c1FlexGridBorderPainter";
      this.c1FlexGridBorderPainter.Size = new System.Drawing.Size(907, 329);
      this.c1FlexGridBorderPainter.TabIndex = 1;
      // 
      // checkBoxExtendLastCol
      // 
      this.checkBoxExtendLastCol.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
      this.checkBoxExtendLastCol.AutoSize = true;
      this.checkBoxExtendLastCol.Location = new System.Drawing.Point(12, 752);
      this.checkBoxExtendLastCol.Name = "checkBoxExtendLastCol";
      this.checkBoxExtendLastCol.Size = new System.Drawing.Size(100, 19);
      this.checkBoxExtendLastCol.TabIndex = 4;
      this.checkBoxExtendLastCol.Text = "ExtendLastCol";
      this.checkBoxExtendLastCol.UseVisualStyleBackColor = true;
      this.checkBoxExtendLastCol.CheckedChanged += this.checkBoxExtendLastCol_CheckedChanged;
      // 
      // c1FlexGridCellStyleBorders
      // 
      this.c1FlexGridCellStyleBorders.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
      this.c1FlexGridCellStyleBorders.ColumnInfo = "10,1,0,0,0,-1,Columns:";
      this.c1FlexGridCellStyleBorders.Location = new System.Drawing.Point(13, 408);
      this.c1FlexGridCellStyleBorders.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.c1FlexGridCellStyleBorders.Name = "c1FlexGridCellStyleBorders";
      this.c1FlexGridCellStyleBorders.Size = new System.Drawing.Size(907, 329);
      this.c1FlexGridCellStyleBorders.TabIndex = 3;
      // 
      // labelBorderPainter
      // 
      this.labelBorderPainter.AutoSize = true;
      this.labelBorderPainter.Location = new System.Drawing.Point(13, 9);
      this.labelBorderPainter.Name = "labelBorderPainter";
      this.labelBorderPainter.Size = new System.Drawing.Size(234, 15);
      this.labelBorderPainter.TabIndex = 0;
      this.labelBorderPainter.Text = "C1FlexGrid with BorderPainter workaround:";
      // 
      // labelBorderWithStyles
      // 
      this.labelBorderWithStyles.AutoSize = true;
      this.labelBorderWithStyles.Location = new System.Drawing.Point(13, 377);
      this.labelBorderWithStyles.Name = "labelBorderWithStyles";
      this.labelBorderWithStyles.Size = new System.Drawing.Size(212, 15);
      this.labelBorderWithStyles.TabIndex = 2;
      this.labelBorderWithStyles.Text = "C1FlexGrid with borders from CellStyle:";
      // 
      // Form
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(933, 783);
      this.Controls.Add(this.labelBorderWithStyles);
      this.Controls.Add(this.labelBorderPainter);
      this.Controls.Add(this.c1FlexGridCellStyleBorders);
      this.Controls.Add(this.checkBoxExtendLastCol);
      this.Controls.Add(this.c1FlexGridBorderPainter);
      this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.Name = "Form";
      this.Text = "Form1";
      ((System.ComponentModel.ISupportInitialize)this.c1FlexGridBorderPainter).EndInit();
      ((System.ComponentModel.ISupportInitialize)this.c1FlexGridCellStyleBorders).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private C1.Win.FlexGrid.C1FlexGrid c1FlexGridBorderPainter;
    private System.Windows.Forms.CheckBox checkBoxExtendLastCol;
    private C1.Win.FlexGrid.C1FlexGrid c1FlexGridCellStyleBorders;
    private System.Windows.Forms.Label labelBorderPainter;
    private System.Windows.Forms.Label labelBorderWithStyles;
  }
}

