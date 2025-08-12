using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace C1FlexGridToolTipRequester
{
  /// <summary>
  /// Whenever the mouse is moved, the <see cref="FlexGridToolTipRequester"/> sends an event
  /// to request the tooltip text for the current cursor position.
  /// These event args contain the current cursor position.
  /// The event handler sets the tooltip text for the cell and also a "hint" object.
  /// This object is a info about the current cell (or cell range) that defines a tooltip.
  /// Whenever this hint changes, the tooltip is reshown.
  /// 
  /// Most common usage: if you want to show tooltips per cell, just set e.g. a <see cref="C1.Win.FlexGrid.CellRange"/>
  /// object (or a <see cref="System.Drawing.Point"/>) which identifies a single cell.
  /// </summary>
  public class ToolTipRequestEventArgs : System.EventArgs
  {

    private string strToolTip = string.Empty;


    private int intX = -1;

    private int intY = -1;

    private object objHint = null;
    
    public ToolTipRequestEventArgs(int int_X, int int_Y, object obj_Hint)
    {
      this.intX = int_X;
      this.intY = int_Y;
      this.objHint = obj_Hint;
    }
    
    /// <summary>
    /// The handler of this events sets the tooltip for the grid cell
    /// </summary>
    public string ToolTipText
    {
      get
      {
        return this.strToolTip;
      }
      set
      {
        this.strToolTip = value;
      }
    }

    /// <summary>
    /// X position of mouse cursor.
    /// </summary>
    public int X
    {
      get
      {
        return this.intX;
      }
    }

    /// <summary>
    /// Y position of mouse cursor
    /// </summary>
    public int Y
    {
      get
      {
        return this.intY;
      }
    }

    /// <summary>
    /// Any object which is a unique marker for the tooltip. As long as this hint does not change, the tooltip is not reshown.
    /// </summary>
    public object Hint
    {
      get
      {
        return this.objHint;
      }
      set
      {
        this.objHint = value;
      }
    }
  }
}
