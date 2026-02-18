using C1.Win.FlexGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C1FlexGridAutoSizeRowHeightToLast
{
  public partial class FormCustomMerge : Form
  {
    public FormCustomMerge()
    {
#if NET48
      //Set the default font to "SegoeUI 9", so that in .NET 48, the AutoScaleDimension of the .NET8 designer generated code
      //matches the actual of the form.
      //In .NET8, the font is automatically "SegoeUI 9".
      this.Font = SystemFonts.MessageBoxFont;
#endif

      InitializeComponent();


      //Write target framework to window title:
      object[] targetFrameworkAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(TargetFrameworkAttribute), false);
      //There should be exactly one attribute.
      TargetFrameworkAttribute ta = (TargetFrameworkAttribute)targetFrameworkAttributes.FirstOrDefault();
      //Don't know whether a NULL check is required.
      string targetFramework = ta?.FrameworkDisplayName ?? "--unknown--";
      this.Text += $" ({targetFramework})";


      //"UseCompatibleTextRendering" = false might cause clipped texts.
      //Console.WriteLine(this.c1FlexGrid1.UseCompatibleTextRendering);
      this.c1FlexGrid1.UseCompatibleTextRendering = true;

      this.c1FlexGrid1.Styles.Fixed.BackColor = SystemColors.Control;

      this.c1FlexGrid1[2, 0] = "row 2, col 0";
      this.c1FlexGrid1[2, 1] = "row 2, col 1";
      this.c1FlexGrid1[2, 2] = "row 2, col 2";
      this.c1FlexGrid1[2, 3] = "row 2, col 3";
      this.c1FlexGrid1[2, 4] = "row 2, col 4";
      this.c1FlexGrid1[2, 5] = "row 2, col 5";

      this.c1FlexGrid1[3, 0] = "row 3, col 0";
      this.c1FlexGrid1[3, 1] = "row 3, col 1";
      this.c1FlexGrid1[3, 2] = "row 3, col 2";


      this.c1FlexGrid1.AllowMerging = AllowMergingEnum.Custom;
      /*Autosizing supports also "Free" or other merge modes. But beware: "Free" does not merge Rows AND colums - 
       *see https://developer.mescius.com/forums/winforms-edition/net6-c1flexgrid-allowmergingfixed-and-rendering-issue
      this.c1FlexGrid1.AllowMerging = AllowMergingEnum.Free;
      foreach (Row row in this.c1FlexGrid1.Rows)
      {
        row.AllowMerging = true;
      }
      foreach (Column col in this.c1FlexGrid1.Cols)
      {
        col.AllowMerging = true;
      }*/

      CellStyle styleWrap = this.c1FlexGrid1.Styles.Add("Wrap", this.c1FlexGrid1.Styles.Normal);
      styleWrap.WordWrap = true;

      //Merge four cells:
      this.c1FlexGrid1[3, 4] = "merged text row 3, col 4 to row 4, col 5";
      //Set text to each cell in order to make them visible also if a row of the range is invisible.
      this.c1FlexGrid1[3, 5] = "merged text row 3, col 4 to row 4, col 5";
      this.c1FlexGrid1[4, 4] = "merged text row 3, col 4 to row 4, col 5";
      this.c1FlexGrid1[4, 5] = "merged text row 3, col 4 to row 4, col 5";
      //Same for the style - otherwise the text might be clipped if row 3 gets invisible.
      this.c1FlexGrid1.SetCellStyle(3, 4, styleWrap);
      this.c1FlexGrid1.SetCellStyle(3, 5, styleWrap);
      this.c1FlexGrid1.SetCellStyle(4, 4, styleWrap);
      this.c1FlexGrid1.SetCellStyle(4, 5, styleWrap);
      this.c1FlexGrid1.MergedRanges.Add(this.c1FlexGrid1.GetCellRange(3, 4, 4, 5));

      //Build a long string:
      string todisplay = "";
      const int MAX = 100;
      for (int index = 0; index < MAX; index++)
      {
        if (index > 0)
        {
          todisplay += " // ";
        }
        //with underscores, the word gets longer and thus causes trouble with splitting. More problems if "UseCompatibleTextRendering" = false.
        //todisplay += "part " + index + " of " + MAX;
        todisplay += "part_" + index + "_of_" + MAX;
      }

      this.c1FlexGrid1[4, 0] = "row 4";
      this.c1FlexGrid1[4, 3] = todisplay;
      this.c1FlexGrid1[5, 3] = todisplay;
      this.c1FlexGrid1[6, 3] = todisplay;

      this.c1FlexGrid1.SetCellStyle(4, 3, styleWrap);
      this.c1FlexGrid1.SetCellStyle(5, 3, styleWrap);
      this.c1FlexGrid1.SetCellStyle(6, 3, styleWrap);

      //First the long text range:
      this.c1FlexGrid1.MergedRanges.Add(this.c1FlexGrid1.GetCellRange(4, 3, 6, 3));

      this.c1FlexGrid1[5, 0] = "row 5";
      this.c1FlexGrid1[5, 1] = "short text row 5";
      //Text has to be set to ALL cells of the merged range.
      this.c1FlexGrid1[5, 3] = todisplay;
      this.c1FlexGrid1.SetCellStyle(5, 3, styleWrap);

      this.c1FlexGrid1[6, 0] = "row 6";
      this.c1FlexGrid1[6, 1] = "short text row 6, col 1";
      this.c1FlexGrid1[6, 2] = "short text row 6, col 2";
      this.c1FlexGrid1[6, 3] = todisplay;
      this.c1FlexGrid1.SetCellStyle(6, 3, styleWrap);

      //some merged text in col 1:
      string shortmerged = "This is a quite short text which should merge and wrap!";
      this.c1FlexGrid1[4, 2] = shortmerged;
      this.c1FlexGrid1.SetCellStyle(4, 2, styleWrap);
      this.c1FlexGrid1[5, 2] = shortmerged;
      this.c1FlexGrid1.SetCellStyle(5, 2, styleWrap);
      this.c1FlexGrid1.MergedRanges.Add(this.c1FlexGrid1.GetCellRange(4, 2, 5, 2));

      //Unmerged text in row:
      string shortunmerged = "This is a quite short text which should wrap!";
      this.c1FlexGrid1[4, 1] = shortunmerged;
      this.c1FlexGrid1.SetCellStyle(4, 1, styleWrap);

      this.c1FlexGrid1[7, 1] = todisplay;
      this.c1FlexGrid1.SetCellStyle(7, 1, styleWrap);

      //Set it to all cells of the range. Otherwise the standard "AutoSizeRows" will not work as expected.
      for (int row = 7; row <= 8; row++)
      {
        for (int col = 1; col <= 5; col++)
        {
          this.c1FlexGrid1[row, col] = todisplay;
          this.c1FlexGrid1.SetCellStyle(row, col, styleWrap);
        }
      }

      this.c1FlexGrid1.MergedRanges.Add(this.c1FlexGrid1.GetCellRange(7, 1, 8, 5));
      this.c1FlexGrid1.AllowResizing = AllowResizingEnum.Both;

      //Default = "true". "false" has slightly different (worse) results.
      //this.c1FlexGrid1.UseCompatibleTextRendering = false;
      this.c1FlexGrid1.UseCompatibleTextRendering = true;
    }


    private void buttonAutoSizeRows_Click(object sender, EventArgs e)
    {
      //"AutoSizeRows" ignored hidden rows, this would result in wrong row height if rows of merged range are invisible:
      //this.c1FlexGrid1.AutoSizeRows();
      this.c1FlexGrid1.AutoSizeRows(0, 0, this.c1FlexGrid1.Rows.Count - 1, this.c1FlexGrid1.Cols.Count - 1, 0, AutoSizeFlags.None);
    }

    private void buttonReset_Click(object sender, EventArgs e)
    {
      for (int row = 0; row < this.c1FlexGrid1.Rows.Count; row++)
      {
        this.c1FlexGrid1.Rows[row].Height = -1;
      }
    }

    private void checkBoxFourthDataRowVisible_CheckedChanged(object sender, EventArgs e)
    {
      this.c1FlexGrid1.Rows[this.c1FlexGrid1.Rows.Fixed + 3].Visible = !this.c1FlexGrid1.Rows[this.c1FlexGrid1.Rows.Fixed + 3].Visible;
    }

    private void checkBoxFifthDataRowVisible_CheckedChanged(object sender, EventArgs e)
    {
      this.c1FlexGrid1.Rows[this.c1FlexGrid1.Rows.Fixed + 4].Visible = !this.c1FlexGrid1.Rows[this.c1FlexGrid1.Rows.Fixed + 4].Visible;
    }

    private void buttonHeightToLast_Click(object sender, EventArgs e)
    {
      this.c1FlexGrid1.AutoSizeRowsHeightToLast();
    }
  }
}