using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using C1.Win.FlexGrid;
using System.Text;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Versioning;
using System.Linq;

namespace C1FlexGridCopyOffice
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public partial class Form : System.Windows.Forms.Form
	{
    
		public Form()
    {
#if NET48
      //Set the default font to "SegoeUI 9", so that in .NET 48, the AutoScaleDimension of the .NET8 designer generated code
      //matches the actual of the form.
      //In .NET8, the font is automatically "SegoeUI 9".
      this.Font = SystemFonts.MessageBoxFont;
#endif

      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      //Write target framework to window title:
      object[] targetFrameworkAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(TargetFrameworkAttribute), false);
      //There should be exactly one attribute.
      TargetFrameworkAttribute ta = (TargetFrameworkAttribute)targetFrameworkAttributes.FirstOrDefault();
      //Don't know whether a NULL check is required.
      string targetFramework = ta?.FrameworkDisplayName ?? "--unknown--";
      this.Text += $" ({targetFramework})";

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
