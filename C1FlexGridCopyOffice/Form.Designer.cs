using System.Drawing.Text;

namespace C1FlexGridCopyOffice
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form));
      this.c1FlexGrid = new C1.Win.FlexGrid.C1FlexGrid();
      this.buttonCopySelectedCells = new System.Windows.Forms.Button();
      this.buttonCopySelectedRows = new System.Windows.Forms.Button();
      this.buttonCopyAll = new System.Windows.Forms.Button();
      this.buttonSaveHtml = new System.Windows.Forms.Button();
      this.labelDiagnostics = new System.Windows.Forms.Label();
      this.buttonSaveClipboardContent = new System.Windows.Forms.Button();
      this.buttonSetClipboardContentFromFile = new System.Windows.Forms.Button();
      this.groupBoxDiagnostics = new System.Windows.Forms.GroupBox();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      ((System.ComponentModel.ISupportInitialize)this.c1FlexGrid).BeginInit();
      this.groupBoxDiagnostics.SuspendLayout();
      this.SuspendLayout();
      // 
      // c1FlexGrid
      // 
      this.c1FlexGrid.AllowMerging = C1.Win.FlexGrid.AllowMergingEnum.Custom;
      this.c1FlexGrid.AutoClipboard = true;
      this.c1FlexGrid.ColumnInfo = "10,1,0,0,0,85,Columns:";
      this.c1FlexGrid.Location = new System.Drawing.Point(8, 8);
      this.c1FlexGrid.Name = "c1FlexGrid";
      this.c1FlexGrid.Size = new System.Drawing.Size(688, 501);
      this.c1FlexGrid.TabIndex = 0;
      this.c1FlexGrid.UseCompatibleTextRendering = true;
      // 
      // buttonCopySelectedCells
      // 
      this.buttonCopySelectedCells.Location = new System.Drawing.Point(8, 515);
      this.buttonCopySelectedCells.Name = "buttonCopySelectedCells";
      this.buttonCopySelectedCells.Size = new System.Drawing.Size(139, 23);
      this.buttonCopySelectedCells.TabIndex = 1;
      this.buttonCopySelectedCells.Text = "Copy selected cells";
      this.toolTip.SetToolTip(this.buttonCopySelectedCells, "Copies the selected cells and corresponsponding row/col headers with formatting to clipboard");
      this.buttonCopySelectedCells.UseVisualStyleBackColor = true;
      this.buttonCopySelectedCells.Click += this.buttonCopySelectedCells_Click;
      // 
      // buttonCopySelectedRows
      // 
      this.buttonCopySelectedRows.Location = new System.Drawing.Point(153, 515);
      this.buttonCopySelectedRows.Name = "buttonCopySelectedRows";
      this.buttonCopySelectedRows.Size = new System.Drawing.Size(139, 23);
      this.buttonCopySelectedRows.TabIndex = 2;
      this.buttonCopySelectedRows.Text = "Copy selected rows";
      this.toolTip.SetToolTip(this.buttonCopySelectedRows, "Copies the selected rows and corresponsponding col headers with formatting to clipboard.");
      this.buttonCopySelectedRows.UseVisualStyleBackColor = true;
      this.buttonCopySelectedRows.Click += this.buttonCopySelectedRows_Click;
      // 
      // buttonCopyAll
      // 
      this.buttonCopyAll.Location = new System.Drawing.Point(298, 515);
      this.buttonCopyAll.Name = "buttonCopyAll";
      this.buttonCopyAll.Size = new System.Drawing.Size(139, 23);
      this.buttonCopyAll.TabIndex = 3;
      this.buttonCopyAll.Text = "Copy all";
      this.toolTip.SetToolTip(this.buttonCopyAll, "Copies the entire grid with formatting to clipboard");
      this.buttonCopyAll.UseVisualStyleBackColor = true;
      this.buttonCopyAll.Click += this.buttonCopyAll_Click;
      // 
      // buttonSaveHtml
      // 
      this.buttonSaveHtml.Location = new System.Drawing.Point(511, 515);
      this.buttonSaveHtml.Name = "buttonSaveHtml";
      this.buttonSaveHtml.Size = new System.Drawing.Size(75, 23);
      this.buttonSaveHtml.TabIndex = 4;
      this.buttonSaveHtml.Text = "Save Html";
      this.toolTip.SetToolTip(this.buttonSaveHtml, "Saves the HTML that is generated during clipboard copy (minus some header) to a file.");
      this.buttonSaveHtml.UseVisualStyleBackColor = true;
      this.buttonSaveHtml.Click += this.buttonSaveHtml_Click;
      // 
      // labelDiagnostics
      // 
      this.labelDiagnostics.AutoSize = true;
      this.labelDiagnostics.Location = new System.Drawing.Point(6, 19);
      this.labelDiagnostics.Name = "labelDiagnostics";
      this.labelDiagnostics.Size = new System.Drawing.Size(312, 15);
      this.labelDiagnostics.TabIndex = 0;
      this.labelDiagnostics.Text = "Those buttons can be used to analyze the Office behavior.";
      // 
      // buttonSaveClipboardContent
      // 
      this.buttonSaveClipboardContent.Location = new System.Drawing.Point(6, 37);
      this.buttonSaveClipboardContent.Name = "buttonSaveClipboardContent";
      this.buttonSaveClipboardContent.Size = new System.Drawing.Size(197, 23);
      this.buttonSaveClipboardContent.TabIndex = 1;
      this.buttonSaveClipboardContent.Text = "Save clipboard content to file";
      this.toolTip.SetToolTip(this.buttonSaveClipboardContent, resources.GetString("buttonSaveClipboardContent.ToolTip"));
      this.buttonSaveClipboardContent.UseVisualStyleBackColor = true;
      this.buttonSaveClipboardContent.Click += this.buttonSaveClipboardContent_Click;
      // 
      // buttonSetClipboardContentFromFile
      // 
      this.buttonSetClipboardContentFromFile.Location = new System.Drawing.Point(209, 37);
      this.buttonSetClipboardContentFromFile.Name = "buttonSetClipboardContentFromFile";
      this.buttonSetClipboardContentFromFile.Size = new System.Drawing.Size(203, 23);
      this.buttonSetClipboardContentFromFile.TabIndex = 2;
      this.buttonSetClipboardContentFromFile.Text = "Load clipboard content from file";
      this.toolTip.SetToolTip(this.buttonSetClipboardContentFromFile, resources.GetString("buttonSetClipboardContentFromFile.ToolTip"));
      this.buttonSetClipboardContentFromFile.UseVisualStyleBackColor = true;
      this.buttonSetClipboardContentFromFile.Click += this.buttonSetClipboardContentFromFile_Click;
      // 
      // groupBoxDiagnostics
      // 
      this.groupBoxDiagnostics.Controls.Add(this.labelDiagnostics);
      this.groupBoxDiagnostics.Controls.Add(this.buttonSetClipboardContentFromFile);
      this.groupBoxDiagnostics.Controls.Add(this.buttonSaveClipboardContent);
      this.groupBoxDiagnostics.Location = new System.Drawing.Point(8, 544);
      this.groupBoxDiagnostics.Name = "groupBoxDiagnostics";
      this.groupBoxDiagnostics.Size = new System.Drawing.Size(688, 71);
      this.groupBoxDiagnostics.TabIndex = 5;
      this.groupBoxDiagnostics.TabStop = false;
      this.groupBoxDiagnostics.Text = "Diagnostics";
      // 
      // Form
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(704, 627);
      this.Controls.Add(this.groupBoxDiagnostics);
      this.Controls.Add(this.buttonSaveHtml);
      this.Controls.Add(this.buttonCopyAll);
      this.Controls.Add(this.buttonCopySelectedRows);
      this.Controls.Add(this.buttonCopySelectedCells);
      this.Controls.Add(this.c1FlexGrid);
      this.Name = "Form";
      this.Text = "Copy to office";
      ((System.ComponentModel.ISupportInitialize)this.c1FlexGrid).EndInit();
      this.groupBoxDiagnostics.ResumeLayout(false);
      this.groupBoxDiagnostics.PerformLayout();
      this.ResumeLayout(false);

    }
    #endregion

    private C1.Win.FlexGrid.C1FlexGrid c1FlexGrid;
    private System.Windows.Forms.Button buttonCopySelectedRows;
    private System.Windows.Forms.Button buttonCopyAll;
    private System.Windows.Forms.Button buttonSaveHtml;
    private System.Windows.Forms.Button buttonCopySelectedCells;
    private System.Windows.Forms.Label labelDiagnostics;
    private System.Windows.Forms.Button buttonSaveClipboardContent;
    private System.Windows.Forms.Button buttonSetClipboardContentFromFile;
    private System.Windows.Forms.GroupBox groupBoxDiagnostics;
    private System.Windows.Forms.ToolTip toolTip;
  }
}