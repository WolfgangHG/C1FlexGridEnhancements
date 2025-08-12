using C1.Win.FlexGrid;
using System.Drawing;
using System.Windows.Forms;

namespace C1FlexGridToolTipRequester
{
  public partial class FormMain : Form
  {
    public FormMain()
    {
      InitializeComponent();

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
