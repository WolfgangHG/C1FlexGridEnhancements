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
      for (int row = this.c1FlexGrid.Rows.Fixed; row < this.c1FlexGrid.Rows.Count; row++)
      {
        for (int col = this.c1FlexGrid.Cols.Fixed; col < this.c1FlexGrid.Cols.Count; col++)
        {
          this.c1FlexGrid[row, col] = row + "/" + col;
        }
      }

      //To use the border painter: set DrawMode to "OwnerDraw" and register the event "OwnerDrawCell":
      this.c1FlexGrid.DrawMode = C1.Win.FlexGrid.DrawModeEnum.OwnerDraw;
      this.c1FlexGrid.OwnerDrawCell += c1FlexGrid_OwnerDrawCell;

      //After having initialized the grid, create the border painter.
      this.borderPainterRed = new C1FlexGridBorderPainter(this.c1FlexGrid, Pens.Red);
      this.borderPainterRed.ResetGrid(this.c1FlexGrid.Rows.Count, this.c1FlexGrid.Cols.Count);

      //Draw some cells:
      this.borderPainterRed.SetBorders(3, 3, 3, 5);

      this.borderPainterRed.SetBorders(7, 3, 8, 4);

      //The second border painter uses green borders with a custom width, so we have to create the pen ourself:
      //Store pen in variable so that it can be disposed.
      this.penGreen = new Pen(Color.Green, 2);
      this.borderPainterGreen = new C1FlexGridBorderPainter(this.c1FlexGrid, this.penGreen);
      this.borderPainterGreen.ResetGrid(this.c1FlexGrid.Rows.Count, this.c1FlexGrid.Cols.Count);

      //This is just a right border (which means: a vertical line)
      this.borderPainterGreen.SetBorders(0, 1, this.c1FlexGrid.Rows.Count - 1, 1, BorderType.Right);

      //And another partial border:
      this.borderPainterGreen.SetBorders(10, 3, 11, 4, BorderType.Right | BorderType.Left | BorderType.Bottom);

      //Border in last column - for testing "ExtendLastCol":
      this.borderPainterGreen.SetBorders(3, 9, 4, 9);
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
      this.c1FlexGrid.ExtendLastCol = true;
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
