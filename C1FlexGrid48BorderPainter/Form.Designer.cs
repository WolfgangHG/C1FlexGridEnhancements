namespace C1FlexGrid48BorderPainter
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
      this.c1FlexGrid = new C1.Win.C1FlexGrid.C1FlexGrid();
      this.checkBoxExtendLastCol = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.c1FlexGrid)).BeginInit();
      this.SuspendLayout();
      // 
      // c1FlexGrid
      // 
      this.c1FlexGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.c1FlexGrid.ColumnInfo = "10,1,0,0,0,-1,Columns:";
      this.c1FlexGrid.Location = new System.Drawing.Point(22, 9);
      this.c1FlexGrid.Name = "c1FlexGrid";
      this.c1FlexGrid.Size = new System.Drawing.Size(766, 405);
      this.c1FlexGrid.TabIndex = 0;
      // 
      // checkBoxExtendLastCol
      // 
      this.checkBoxExtendLastCol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.checkBoxExtendLastCol.AutoSize = true;
      this.checkBoxExtendLastCol.Location = new System.Drawing.Point(22, 421);
      this.checkBoxExtendLastCol.Name = "checkBoxExtendLastCol";
      this.checkBoxExtendLastCol.Size = new System.Drawing.Size(94, 17);
      this.checkBoxExtendLastCol.TabIndex = 1;
      this.checkBoxExtendLastCol.Text = "ExtendLastCol";
      this.checkBoxExtendLastCol.UseVisualStyleBackColor = true;
      this.checkBoxExtendLastCol.CheckedChanged += new System.EventHandler(this.checkBoxExtendLastCol_CheckedChanged);
      // 
      // Form
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.checkBoxExtendLastCol);
      this.Controls.Add(this.c1FlexGrid);
      this.Name = "Form";
      this.Text = "Form1";
      ((System.ComponentModel.ISupportInitialize)(this.c1FlexGrid)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private C1.Win.C1FlexGrid.C1FlexGrid c1FlexGrid;
    private System.Windows.Forms.CheckBox checkBoxExtendLastCol;
  }
}

