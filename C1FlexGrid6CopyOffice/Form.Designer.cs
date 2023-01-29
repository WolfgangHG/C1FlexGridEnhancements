using System.Drawing.Text;

namespace C1FlexGrid6CopyOffice
{
  partial class Form
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.c1FlexGrid = new C1.Win.FlexGrid.C1FlexGrid();
      this.buttonCopySelectedCells = new System.Windows.Forms.Button();
      this.buttonCopySelectedRows = new System.Windows.Forms.Button();
      this.buttonCopyAll = new System.Windows.Forms.Button();
      this.buttonSaveHtml = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.c1FlexGrid)).BeginInit();
      this.SuspendLayout();
      // 
      // c1FlexGrid
      // 
      this.c1FlexGrid.AllowMerging = C1.Win.FlexGrid.AllowMergingEnum.Custom;
      this.c1FlexGrid.AutoClipboard = true;
      this.c1FlexGrid.ColumnInfo = "10,1,0,0,0,85,Columns:";
      this.c1FlexGrid.Location = new System.Drawing.Point(8, 8);
      this.c1FlexGrid.Name = "c1FlexGrid";
      this.c1FlexGrid.Size = new System.Drawing.Size(688, 459);
      this.c1FlexGrid.TabIndex = 0;
      this.c1FlexGrid.UseCompatibleTextRendering = true;
      // 
      // buttonCopySelectedCells
      // 
      this.buttonCopySelectedCells.Location = new System.Drawing.Point(8, 473);
      this.buttonCopySelectedCells.Name = "buttonCopySelectedCells";
      this.buttonCopySelectedCells.Size = new System.Drawing.Size(139, 23);
      this.buttonCopySelectedCells.TabIndex = 1;
      this.buttonCopySelectedCells.Text = "Copy selected cells";
      this.buttonCopySelectedCells.UseVisualStyleBackColor = true;
      this.buttonCopySelectedCells.Click += new System.EventHandler(this.buttonCopySelectedCells_Click);
      // 
      // buttonCopySelectedRows
      // 
      this.buttonCopySelectedRows.Location = new System.Drawing.Point(153, 473);
      this.buttonCopySelectedRows.Name = "buttonCopySelectedRows";
      this.buttonCopySelectedRows.Size = new System.Drawing.Size(139, 23);
      this.buttonCopySelectedRows.TabIndex = 2;
      this.buttonCopySelectedRows.Text = "Copy selected rows";
      this.buttonCopySelectedRows.UseVisualStyleBackColor = true;
      this.buttonCopySelectedRows.Click += new System.EventHandler(this.buttonCopySelectedRows_Click);
      // 
      // buttonCopyAll
      // 
      this.buttonCopyAll.Location = new System.Drawing.Point(298, 473);
      this.buttonCopyAll.Name = "buttonCopyAll";
      this.buttonCopyAll.Size = new System.Drawing.Size(139, 23);
      this.buttonCopyAll.TabIndex = 3;
      this.buttonCopyAll.Text = "Copy all";
      this.buttonCopyAll.UseVisualStyleBackColor = true;
      this.buttonCopyAll.Click += new System.EventHandler(this.buttonCopyAll_Click);
      // 
      // buttonSaveHtml
      // 
      this.buttonSaveHtml.Location = new System.Drawing.Point(511, 473);
      this.buttonSaveHtml.Name = "buttonSaveHtml";
      this.buttonSaveHtml.Size = new System.Drawing.Size(75, 23);
      this.buttonSaveHtml.TabIndex = 4;
      this.buttonSaveHtml.Text = "Save Html";
      this.buttonSaveHtml.UseVisualStyleBackColor = true;
      this.buttonSaveHtml.Click += new System.EventHandler(this.buttonSaveHtml_Click);
      // 
      // Form
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(704, 508);
      this.Controls.Add(this.buttonSaveHtml);
      this.Controls.Add(this.buttonCopyAll);
      this.Controls.Add(this.buttonCopySelectedRows);
      this.Controls.Add(this.buttonCopySelectedCells);
      this.Controls.Add(this.c1FlexGrid);
      this.Name = "Form";
      this.Text = "Form1";
      ((System.ComponentModel.ISupportInitialize)(this.c1FlexGrid)).EndInit();
      this.ResumeLayout(false);

    }
    #endregion

    private C1.Win.FlexGrid.C1FlexGrid c1FlexGrid;
    private System.Windows.Forms.Button buttonCopySelectedRows;
    private System.Windows.Forms.Button buttonCopyAll;
    private System.Windows.Forms.Button buttonSaveHtml;
    private System.Windows.Forms.Button buttonCopySelectedCells;
  }
}