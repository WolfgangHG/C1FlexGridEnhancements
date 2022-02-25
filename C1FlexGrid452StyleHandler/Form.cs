using C1.Win.C1FlexGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C1FlexGrid452StyleHandler
{
  public partial class Form : System.Windows.Forms.Form
  {
    public Form()
    {
      InitializeComponent();

      for (int row = this.c1FlexGrid1.Rows.Fixed; row < this.c1FlexGrid1.Rows.Count; row++)
      {
        for (int col = this.c1FlexGrid1.Cols.Fixed; col < this.c1FlexGrid1.Cols.Count; col++)
        {
          this.c1FlexGrid1[row, col] = "row " + row + "/col " + col;
        }
      }

      C1FlexGridStyleHandler styleHandler = new C1FlexGridStyleHandler(this.c1FlexGrid1);

      //Three rows with green backcolor
      styleHandler.MergeBackColor(2, 1, 4, this.c1FlexGrid1.Cols.Count - 1, Color.Green);
      //one column with red forecolor:
      styleHandler.MergeForeColor(1, 3, this.c1FlexGrid1.Rows.Count - 1, 3, Color.Red);

      //Some bold cells:
      for (int col = this.c1FlexGrid1.Cols.Fixed; col < this.c1FlexGrid1.Cols.Count; col++)
      {
        styleHandler.MergeFont(col, col, new Font(this.c1FlexGrid1.Font, FontStyle.Bold));
      }

      //align text vertical:
      styleHandler.MergeTextAlign(8, 2, TextAlignEnum.RightCenter);
      styleHandler.MergeTextAlign(8, 3, TextAlignEnum.RightCenter);
      styleHandler.MergeTextAlign(8, 4, TextAlignEnum.RightCenter);

      //Word Wrap:
      //We have to set a longer  text:
      this.c1FlexGrid1[13, 2] = "this is a long text that wraps";
      styleHandler.MergeWordWrap(13, 2,true);

      //Apply a border to this cell.
      //To archive this, we have to set the bottom border of the cell above and the right border of the cell to the left.
      //Unfortunately, the cell to the left looses the bottom border, and the cell above has no right border - better use my "BorderPainter" sample.
      styleHandler.MergeBorder(13, 2, Color.Green, BorderDirEnum.Both, BorderStyleEnum.Flat, 2);
      styleHandler.MergeBorder(12, 2, Color.Green, BorderDirEnum.Horizontal, BorderStyleEnum.Flat, 2);
      styleHandler.MergeBorder(13, 1, Color.Green, BorderDirEnum.Vertical, BorderStyleEnum.Flat, 2);

      //Auto size the row - do this after applying the border, as the border width "2" affects the row height:
      this.c1FlexGrid1.AutoSizeRow(13);
    }
  }
}
