using C1.Win.FlexGrid;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace C1FlexGridToolTipRequester
{
  public partial class FormMain : Form
  {
    public FormMain()
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


      for (int col = this.c1FlexGrid.Cols.Fixed; col < this.c1FlexGrid.Cols.Count; col++)
      {
        this.c1FlexGrid[0, col] = $"col {col}";
      }
      for (int row = this.c1FlexGrid.Rows.Fixed; row < this.c1FlexGrid.Rows.Count; row++)
      {
        this.c1FlexGrid[row, 0] = $"row {row}";
      }

      for (int row = this.c1FlexGrid.Rows.Fixed; row < this.c1FlexGrid.Rows.Count; row++)
      {
        for (int col = this.c1FlexGrid.Cols.Fixed; col < this.c1FlexGrid.Cols.Count; col++)
        {
          this.c1FlexGrid[row, col] = $"cell {row}/{col}";
        }
      }
    }

    /// <summary>
    /// Tooltip ist requested.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void toolTipRequester_ToolTipRequest(object sender, ToolTipRequestEventArgs e)
    {
      //Get cell for mouse position:
      Point p = new Point(e.X, e.Y);
      HitTestInfo hti = this.c1FlexGrid.HitTest(p);
      
      //Set the cell range as hint - so the tooltip is shown only once per cell.
      e.Hint = this.c1FlexGrid.GetCellRange(hti.Row, hti.Column);

      //Generate a tooltip:
      e.ToolTipText = "Tooltip of cell " + hti.Row + "/" + hti.Column;
    }
  }
}
