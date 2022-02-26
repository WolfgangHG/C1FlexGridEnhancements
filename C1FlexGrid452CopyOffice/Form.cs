using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using C1.Win.C1FlexGrid;
using System.Text;
using System.IO;
using System.Globalization;
using System.Diagnostics;

namespace C1FlexGrid452CopyOffice
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form : System.Windows.Forms.Form
	{
    private C1.Win.C1FlexGrid.C1FlexGrid c1FlexGrid;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
    private Button buttonCopySelectedRows;
    private Button buttonCopyAll;
    private Button buttonSaveHtml;
    private Button buttonCopySelectedCells;
    
		public Form()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
      
      //Create texts:
      for (int iRow = 0; iRow < this.c1FlexGrid.Rows.Count; iRow++)
      {
        for (int iCol = 0; iCol < this.c1FlexGrid.Cols.Count; iCol++)
        {
          this.c1FlexGrid.SetData (iRow, iCol, iCol + "/" + iRow);
        }
      }

      CellStyle styleBackColorRed = this.c1FlexGrid.Styles.Add("BackColorRed", this.c1FlexGrid.Styles.Normal);
      styleBackColorRed.BackColor = Color.Red;

      CellStyle styleBackColorRedFontYellow = this.c1FlexGrid.Styles.Add("BackColorRedFontYellow", styleBackColorRed);
      styleBackColorRedFontYellow.ForeColor = Color.Yellow;
      //different font:
      styleBackColorRedFontYellow.Font = new Font("Baskerville Old Face", 12.25f, FontStyle.Bold);

      CellStyle styleBackColorRedFontStrikeout = this.c1FlexGrid.Styles.Add("BackColorRedFontStrikeout", styleBackColorRed);
      //different font:
      styleBackColorRedFontStrikeout.Font = new Font(this.c1FlexGrid.Styles.Normal.Font, FontStyle.Strikeout);


      CellStyle styleWrap = this.c1FlexGrid.Styles.Add("WordWrap", this.c1FlexGrid.Styles.Normal);
      styleWrap.WordWrap = true;
      
      this.c1FlexGrid.SetCellStyle(1, 1, styleBackColorRed);
      this.c1FlexGrid.SetCellStyle(2, 1, styleBackColorRed);
      this.c1FlexGrid.SetCellStyle(3, 1, styleBackColorRed);
      this.c1FlexGrid.SetCellStyle(4, 1, styleBackColorRed);
      this.c1FlexGrid.SetCellStyle(5, 1, styleBackColorRed);

      this.c1FlexGrid.SetCellStyle(1, 2, styleBackColorRed);
      this.c1FlexGrid.SetCellStyle(2, 2, styleBackColorRedFontYellow);
      this.c1FlexGrid.SetCellStyle(3, 2, styleBackColorRedFontYellow);
      this.c1FlexGrid.SetCellStyle(4, 2, styleBackColorRedFontYellow);
      this.c1FlexGrid.SetCellStyle(5, 2, styleBackColorRedFontStrikeout);

      this.c1FlexGrid.SetCellStyle(1, 3, styleBackColorRed);
      this.c1FlexGrid.SetCellStyle(2, 3, styleBackColorRedFontYellow);
      this.c1FlexGrid.SetCellStyle(3, 3, styleBackColorRedFontYellow);
      this.c1FlexGrid.SetCellStyle(4, 3, styleBackColorRedFontYellow);
      this.c1FlexGrid.SetCellStyle(5, 3, styleBackColorRedFontStrikeout);


      this.c1FlexGrid.SetCellStyle(1, 4, styleBackColorRed);
      this.c1FlexGrid.SetCellStyle(2, 4, styleBackColorRedFontYellow);
      this.c1FlexGrid.SetCellStyle(3, 4, styleBackColorRedFontYellow);
      this.c1FlexGrid.SetCellStyle(4, 4, styleBackColorRedFontYellow);
      this.c1FlexGrid.SetCellStyle(5, 4, styleBackColorRedFontStrikeout);

      this.c1FlexGrid.SetCellStyle(1, 5, styleBackColorRed);
      this.c1FlexGrid.SetCellStyle(2, 5, styleBackColorRed);
      this.c1FlexGrid.SetCellStyle(3, 5, styleBackColorRed);
      this.c1FlexGrid.SetCellStyle(4, 5, styleBackColorRed);
      this.c1FlexGrid.SetCellStyle(5, 5, styleBackColorRed);

      //Word wrap causes no problem:
      this.c1FlexGrid.SetCellStyle(7, 3, styleWrap);
      //Wrap cell - the newline causes (solved) trouble later:
      this.c1FlexGrid[7, 3] = "Long text which should wrap" + Environment.NewLine + "It has even a second line";
      this.c1FlexGrid.AutoSizeRow(7);

      //switch off border in the normal style:
      this.c1FlexGrid.Styles.Normal.Border.Style = BorderStyleEnum.None;

      //Merge some cells:
      this.c1FlexGrid.MergedRanges.Add(this.c1FlexGrid.GetCellRange(10, 2, 12, 4));
      //Also merge fixed cells:
      this.c1FlexGrid.MergedRanges.Add(this.c1FlexGrid.GetCellRange(10, 0, 12, 0));
      this.c1FlexGrid.MergedRanges.Add(this.c1FlexGrid.GetCellRange(0, 4, 0, 6));

    }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
      this.c1FlexGrid = new C1.Win.C1FlexGrid.C1FlexGrid();
      this.buttonCopySelectedCells = new System.Windows.Forms.Button();
      this.buttonCopySelectedRows = new System.Windows.Forms.Button();
      this.buttonCopyAll = new System.Windows.Forms.Button();
      this.buttonSaveHtml = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.c1FlexGrid)).BeginInit();
      this.SuspendLayout();
      // 
      // c1FlexGrid
      // 
      this.c1FlexGrid.AllowMerging = C1.Win.C1FlexGrid.AllowMergingEnum.Custom;
      this.c1FlexGrid.AutoClipboard = true;
      this.c1FlexGrid.ColumnInfo = "10,1,0,0,0,85,Columns:";
      this.c1FlexGrid.Location = new System.Drawing.Point(8, 8);
      this.c1FlexGrid.Name = "c1FlexGrid";
      this.c1FlexGrid.Rows.DefaultSize = 19;
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

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form());
		}

    private void buttonCopySelectedCells_Click(object sender, EventArgs e)
    {
      C1FlexGridCopyToOffice.Copy(this.c1FlexGrid, C1FlexGridCopyToOffice.CopyMode.SelectedCells);
    }

    private void buttonCopySelectedRows_Click(object sender, EventArgs e)
    {
      C1FlexGridCopyToOffice.Copy(this.c1FlexGrid, C1FlexGridCopyToOffice.CopyMode.SelectedRows);
    }

    private void buttonCopyAll_Click(object sender, EventArgs e)
    {
      C1FlexGridCopyToOffice.Copy(this.c1FlexGrid, C1FlexGridCopyToOffice.CopyMode.All);
    }

    private void buttonSaveHtml_Click(object sender, EventArgs e)
    {
      string css;
      string htmlBody = C1FlexGridCopyToOffice.ToHTML(this.c1FlexGrid, C1FlexGridCopyToOffice.CopyMode.All, out css);

      StringBuilder sbHTML = new StringBuilder();
      //Create a HTML5 document:
      sbHTML.AppendLine("<!DOCTYPE html>");
      sbHTML.AppendLine("<html>");
      sbHTML.AppendLine("<head>");

      //Append css styles created from CellStyles:
      sbHTML.AppendLine("<style>");
      sbHTML.AppendLine(css);
      sbHTML.AppendLine("</style>");
      
      sbHTML.AppendLine("</head>");

      sbHTML.AppendLine("<body>");
      sbHTML.AppendLine(htmlBody);
      sbHTML.AppendLine("</body>");
      sbHTML.AppendLine("</html>");


      File.WriteAllText("result.html", sbHTML.ToString());

      Process.Start("result.html");
    }
  }
}
