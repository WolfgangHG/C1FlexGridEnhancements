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

namespace C1FlexGridBorderPainter
{
  public partial class Form : System.Windows.Forms.Form
  {
    /// <summary>
    /// This border painter draws red borders and width "1".
    /// </summary>
    private C1FlexGridBorderPainter borderPainterRed;


    /// <summary>
    /// This border painter draws cells with green border and with "2"
    /// </summary>
    private C1FlexGridBorderPainter borderPainterGreen;

    /// <summary>
    /// As we use a custom Pen here, we have to dispose it.
    /// </summary>
    private Pen penGreen;

    public Form()
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

      //Fill grid with some data:
      for (int row = this.c1FlexGridBorderPainter.Rows.Fixed; row < this.c1FlexGridBorderPainter.Rows.Count; row++)
      {
        for (int col = this.c1FlexGridBorderPainter.Cols.Fixed; col < this.c1FlexGridBorderPainter.Cols.Count; col++)
        {
          this.c1FlexGridBorderPainter[row, col] = row + "/" + col;

          this.c1FlexGridCellStyleBorders[row, col] = row + "/" + col;
        }
      }

      //To use the border painter: set DrawMode to "OwnerDraw" and register the event "OwnerDrawCell":
      this.c1FlexGridBorderPainter.DrawMode = C1.Win.FlexGrid.DrawModeEnum.OwnerDraw;
      this.c1FlexGridBorderPainter.OwnerDrawCell += c1FlexGrid_OwnerDrawCell;

      //After having initialized the grid, create the border painter.
      this.borderPainterRed = new C1FlexGridBorderPainter(this.c1FlexGridBorderPainter, Pens.Red);
      this.borderPainterRed.ResetGrid(this.c1FlexGridBorderPainter.Rows.Count, this.c1FlexGridBorderPainter.Cols.Count);

      //Draw some cells:
      this.borderPainterRed.SetBorders(3, 3, 3, 5);

      this.borderPainterRed.SetBorders(7, 3, 8, 4);

      //The second border painter uses green borders with a custom width, so we have to create the pen ourself:
      //Store pen in variable so that it can be disposed.
      this.penGreen = new Pen(Color.Green, 2);
      this.borderPainterGreen = new C1FlexGridBorderPainter(this.c1FlexGridBorderPainter, this.penGreen);
      this.borderPainterGreen.ResetGrid(this.c1FlexGridBorderPainter.Rows.Count, this.c1FlexGridBorderPainter.Cols.Count);

      //This is just a right border (which means: a vertical line)
      this.borderPainterGreen.SetBorders(0, 1, this.c1FlexGridBorderPainter.Rows.Count - 1, 1, BorderType.Right);

      //And another partial border:
      this.borderPainterGreen.SetBorders(10, 3, 11, 4, BorderType.Right | BorderType.Left | BorderType.Bottom);

      //Border in last column - for testing "ExtendLastCol":
      this.borderPainterGreen.SetBorders(3, 9, 4, 9);

      //Create the second grid:
      //Colum 1: a right green border:
      CellStyle styleRightGreen = this.c1FlexGridCellStyleBorders.Styles.Add("RightGreen", this.c1FlexGridCellStyleBorders.Styles.Normal);
      styleRightGreen.Border.Direction = BorderDirEnum.BothDifferent;
      styleRightGreen.Border.VerticalColor = Color.Green;
      styleRightGreen.Border.VerticalWidth = 2;
      //Reset horizontal border to default color, otherwise it is black:
      styleRightGreen.Border.HorizontalColor = styleRightGreen.Border.Color;
      //Apply it to full column:
      this.c1FlexGridCellStyleBorders.Cols[1].Style = styleRightGreen;

      //Fixed row needs a different style:
      CellStyle styleRightGreenFixed = this.c1FlexGridCellStyleBorders.Styles.Add("RightGreenFixed", this.c1FlexGridCellStyleBorders.Styles.Fixed);
      styleRightGreenFixed.Border.Direction = BorderDirEnum.BothDifferent;
      styleRightGreenFixed.Border.VerticalColor = Color.Green;
      styleRightGreenFixed.Border.VerticalWidth = 2;
      //Reset horizontal border to default color, otherwise it is black:
      styleRightGreenFixed.Border.HorizontalColor = styleRightGreenFixed.Border.Color;
      //Apply it to all fixed rows of the column (which is just a single row)
      this.c1FlexGridCellStyleBorders.Cols[1].StyleFixed = styleRightGreenFixed;

      //The red rectangle from 3/3 to 3/5
      CellStyle styleRightRed = this.c1FlexGridCellStyleBorders.Styles.Add("RightRed", this.c1FlexGridCellStyleBorders.Styles.Normal);
      styleRightRed.Border.Direction = BorderDirEnum.BothDifferent;
      styleRightRed.Border.VerticalColor = Color.Red;
      styleRightRed.Border.VerticalWidth = 1;
      //Reset horizontal border to default color, otherwise it is black:
      styleRightRed.Border.HorizontalColor = styleRightRed.Border.Color;
      this.c1FlexGridCellStyleBorders.SetCellStyle(3, 2, styleRightRed);
      
      //Top line of the rectangle:
      CellStyle styleBottomRed = this.c1FlexGridCellStyleBorders.Styles.Add("BottomRed", this.c1FlexGridCellStyleBorders.Styles.Normal);
      styleBottomRed.Border.Direction = BorderDirEnum.BothDifferent;
      styleBottomRed.Border.HorizontalColor = Color.Red;
      styleBottomRed.Border.HorizontalWidth = 1;
      //Reset horizontal border to default color, otherwise it is black:
      styleBottomRed.Border.VerticalColor = styleBottomRed.Border.Color;
      //Bottom border of the row above the rectangle:
      this.c1FlexGridCellStyleBorders.SetCellStyle(2, 3, styleBottomRed);
      this.c1FlexGridCellStyleBorders.SetCellStyle(2, 4, styleBottomRed);
      this.c1FlexGridCellStyleBorders.SetCellStyle(2, 5, styleBottomRed);

      //Bottom line of the rectangle:
      this.c1FlexGridCellStyleBorders.SetCellStyle(3, 3, styleBottomRed);
      this.c1FlexGridCellStyleBorders.SetCellStyle(3, 4, styleBottomRed);

      //Right cell has bottom and right border. So it is a "Both" border:
      CellStyle styleBottomRightRed = this.c1FlexGridCellStyleBorders.Styles.Add("BottomRightRed", this.c1FlexGridCellStyleBorders.Styles.Normal);
      styleBottomRightRed.Border.Direction = BorderDirEnum.Both;
      styleBottomRightRed.Border.Color = Color.Red;
      styleBottomRightRed.Border.Width = 1;
      this.c1FlexGridCellStyleBorders.SetCellStyle(3, 5, styleBottomRightRed);

     

      //Red rectangle from 7/3 to 8/4:
      this.c1FlexGridCellStyleBorders.SetCellStyle(7, 2, styleRightRed);  //left border
      this.c1FlexGridCellStyleBorders.SetCellStyle(8, 2, styleRightRed);  //left border
      this.c1FlexGridCellStyleBorders.SetCellStyle(6, 3, styleBottomRed); //top border
      this.c1FlexGridCellStyleBorders.SetCellStyle(6, 4, styleBottomRed); //top border
      this.c1FlexGridCellStyleBorders.SetCellStyle(7, 4, styleRightRed); //right border
      this.c1FlexGridCellStyleBorders.SetCellStyle(8, 3, styleBottomRed); //bottom border
      this.c1FlexGridCellStyleBorders.SetCellStyle(8, 4, styleBottomRightRed); //bottom/right border


      //Green border from 10/3 to 11/4:
      //First create missing styles:
      CellStyle styleBottomGreen = this.c1FlexGridCellStyleBorders.Styles.Add("BottomGreen", this.c1FlexGridCellStyleBorders.Styles.Normal);
      styleBottomGreen.Border.Direction = BorderDirEnum.BothDifferent;
      styleBottomGreen.Border.HorizontalColor = Color.Green;
      styleBottomGreen.Border.HorizontalWidth = 2;
      //Reset horizontal border to default color, otherwise it is black:
      styleBottomGreen.Border.VerticalColor = styleBottomGreen.Border.Color;

      CellStyle styleBottomRightGreen = this.c1FlexGridCellStyleBorders.Styles.Add("BottomRightGreen", this.c1FlexGridCellStyleBorders.Styles.Normal);
      styleBottomRightGreen.Border.Direction = BorderDirEnum.Both;
      styleBottomRightGreen.Border.Color = Color.Green;
      styleBottomRightGreen.Border.Width = 2;

      this.c1FlexGridCellStyleBorders.SetCellStyle(10, 2, styleRightGreen);  //left border
      this.c1FlexGridCellStyleBorders.SetCellStyle(11, 2, styleRightGreen);  //left border
      //No top border.
      //this.c1FlexGridBorders.SetCellStyle(9, 3, styleBottomGreen); //top border
      //this.c1FlexGridBorders.SetCellStyle(9, 4, styleBottomGreen); //top border
      this.c1FlexGridCellStyleBorders.SetCellStyle(10, 4, styleRightGreen); //right border
      this.c1FlexGridCellStyleBorders.SetCellStyle(11, 3, styleBottomGreen); //bottom border
      this.c1FlexGridCellStyleBorders.SetCellStyle(11, 4, styleBottomRightGreen); //bottom/right border

      //Green border from 3/9 to 4/9:
      this.c1FlexGridCellStyleBorders.SetCellStyle(3, 8, styleRightGreen);  //left border
      this.c1FlexGridCellStyleBorders.SetCellStyle(4, 8, styleRightGreen);  //left border
      this.c1FlexGridCellStyleBorders.SetCellStyle(2, 9, styleBottomGreen); //top border
      this.c1FlexGridCellStyleBorders.SetCellStyle(3, 9, styleRightGreen); //right border
      this.c1FlexGridCellStyleBorders.SetCellStyle(4, 9, styleBottomRightGreen); //bottom/right border

    }

    /// <summary>
    /// Handle the "OwnerDrawCell" event: draw custom borders:
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void c1FlexGrid_OwnerDrawCell(object sender, C1.Win.FlexGrid.OwnerDrawCellEventArgs e)
    {
      //First draw the cell:
      e.DrawCell();

      //Then draw our borders:
      //Note: if border from two border painter might overlap, the latest border will win:
      this.borderPainterRed.DrawBorders(e);
      this.borderPainterGreen.DrawBorders(e);
    }

    /// <summary>
    /// Toggle "ExtendLastCol" in FlexGrid.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void checkBoxExtendLastCol_CheckedChanged(object sender, EventArgs e)
    {
      this.c1FlexGridBorderPainter.ExtendLastCol = true;
      this.c1FlexGridCellStyleBorders.ExtendLastCol = true;
    }

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

      //Cleanup all "Pen" objects that are autocreated:
      if (disposing && this.penGreen != null)
      {
        this.penGreen.Dispose();
        this.penGreen = null;
      }
      base.Dispose(disposing);
    }

  }
}
