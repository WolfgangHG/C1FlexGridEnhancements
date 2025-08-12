namespace C1FlexGridToolTipRequester
{
  partial class FormMain
  {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.c1FlexGrid = new C1.Win.FlexGrid.C1FlexGrid();
      this.toolTipRequester = new FlexGridToolTipRequester(this.components);
      ((System.ComponentModel.ISupportInitialize)this.c1FlexGrid).BeginInit();
      this.SuspendLayout();
      // 
      // c1FlexGrid
      // 
      this.c1FlexGrid.ColumnInfo = "10,1,0,0,0,-1,Columns:";
      this.c1FlexGrid.Location = new System.Drawing.Point(48, 20);
      this.c1FlexGrid.Name = "c1FlexGrid";
      this.c1FlexGrid.Size = new System.Drawing.Size(564, 398);
      this.c1FlexGrid.TabIndex = 0;
      // 
      // toolTipRequester
      // 
      this.toolTipRequester.ControlForToolTip = this.c1FlexGrid;
      this.toolTipRequester.ToolTipRequest += this.toolTipRequester_ToolTipRequest;
      // 
      // FormMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.c1FlexGrid);
      this.Name = "FormMain";
      this.Text = "FormMain";
      ((System.ComponentModel.ISupportInitialize)this.c1FlexGrid).EndInit();
      this.ResumeLayout(false);
    }

    #endregion

    private C1.Win.FlexGrid.C1FlexGrid c1FlexGrid;
    private FlexGridToolTipRequester toolTipRequester;
  }
}