using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C1FlexGridToolTipRequester
{
  /// <summary>
  /// Whenever the mouse is moved, the <see cref="FlexGridToolTipRequester"/> sends the event <see cref="ToolTipRequest"/>
  /// to request the tooltip text for the current cursor position.
  /// These event args contain the current cursor position.
  /// 
  /// The event handler sets the tooltip text for the cell and also a "hint" object.
  /// This object is a info about the current cell (or cell range) that defines a tooltip.
  /// Whenever this hint changes, the tooltip is reshown.
  /// 
  /// Most common usage: if you want to show tooltips per cell, just set e.g. a <see cref="C1.Win.FlexGrid.CellRange"/>
  /// object (or a <see cref="System.Drawing.Point"/>) which identifies a single cell.
  /// 
  /// 
  /// To use this component: use the WinForms designer to add it to a form (it will show up in the
  /// "component" area at the bottom). Then set the property <see cref="ControlForToolTip"/>
  /// to your C1FlexGrid instance.
  /// Finally add a handler to the event <see cref="ToolTipRequest"/>.
  /// </summary>
  public partial class FlexGridToolTipRequester : Component
  {
    #region Variables
   
    /// <summary>
    /// This is the C1FlexGrid for which the tooltip is shown.
    /// </summary>
    private Control controlToolTip = null;

    /// <summary>
    /// Result of <see cref="ToolTipRequest"/>: a unique "key" to identify the object for which a tooltip is shown.
    /// Might be e.g. a "CellRange" object.
    /// </summary>
    private object objHint = null;
    #endregion

    #region Events

    public delegate void ToolTipRequestEventHandler(object sender, ToolTipRequestEventArgs e);

    /// <summary>
    /// A ToolTip for a cursor position is requested.
    /// Handle this event and set the tooltip.
    /// </summary>
    [Description("A ToolTip for a cursor position is requested. Handle this event and set the tooltip.")]
    public event ToolTipRequestEventHandler ToolTipRequest;
    #endregion

    #region constructor
    /// <summary>
    /// Create a tooltip requester instance.
    /// </summary>
    /// <param name="container"></param>
    public FlexGridToolTipRequester(IContainer container)
    {
      container.Add(this);

      InitializeComponent();
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Request tooltip for cell under mouse (position is contained in parameter) and 
    /// start the timer to show the tooltip if the "hint" has changed.
    /// </summary>
    /// <param name="toolTipRequestEventArgs"></param>
    private void HandleMouseMove(ToolTipRequestEventArgs toolTipRequestEventArgs)
    {
      if (this.ToolTipRequest == null)
        return;

      object objHintOld = this.objHint;
      this.ToolTipRequest(this, toolTipRequestEventArgs);

      //If hint has not changed, the cursor did not move out of e.g. the current cell - don't reshow the tooltip.
      if (Object.Equals(objHintOld, toolTipRequestEventArgs.Hint) == true)
        return;

      this.objHint = toolTipRequestEventArgs.Hint;

      if (this.controlToolTip.Disposing == true)
      {
        return;
      }

      //Hide the tooltip of the previous cell if it was not automatically hidden before.
      this.toolTip.Active = false;
      this.toolTip.SetToolTip(this.controlToolTip, toolTipRequestEventArgs.ToolTipText);
      try
      {
        this.toolTip.Hide(this.controlToolTip);
      }
      catch (ObjectDisposedException dispEx)
      {
        Trace.WriteLine("ObjectDisposedException beim HGToolTipRequester.HandleMouseMove und Ausblenden des Tooltips");
        Trace.WriteLine(dispEx.ToString());
      }

      //Don't do anything if no text was returned from the event handler
      if (toolTipRequestEventArgs.ToolTipText == null ||
        toolTipRequestEventArgs.ToolTipText.Length == 0)
        return;

      //Start the timer to reshow the tooltip.
      this.timerToolTipShow.Interval = this.toolTip.ReshowDelay;
      this.timerToolTipShow.Enabled = true;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Set the control for which the tooltip is to be shown.
    /// </summary>
    [Description("Set the control for which the tooltip is to be shown..")]
    [DefaultValue(null)]
    public Control ControlForToolTip
    {
      get
      {
        return this.controlToolTip;
      }
      set
      {
        if (this.controlToolTip != null)
        {
          this.controlToolTip.MouseMove -= new System.Windows.Forms.MouseEventHandler(this.controlForToolTip_MouseMove);
          this.controlToolTip.MouseLeave -= new System.EventHandler(this.controlForToolTip_MouseLeave);
        }

        this.controlToolTip = value;

        if (this.controlToolTip != null)
        {
          this.controlToolTip.MouseMove += new System.Windows.Forms.MouseEventHandler(this.controlForToolTip_MouseMove);
          this.controlToolTip.MouseLeave += new System.EventHandler(this.controlForToolTip_MouseLeave);
        }
      }
    }

    /// <summary>
    /// Tooltip is shown for this timespan in millis
    /// </summary>
    [Description("Tooltip is shown for this timespan in millis")]
    [DefaultValue(5000)]
    public int AutoPopDelay
    {
      get
      {
        return this.toolTip.AutoPopDelay;
      }
      set
      {
        this.toolTip.AutoPopDelay = value;
      }
    }
    #endregion

    #region EventHandler
    /// <summary>
    /// Mouse is moved: check whether a new tooltip has to be initialized.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void controlForToolTip_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      if (this.controlToolTip.Disposing == false)
      {
        ToolTipRequestEventArgs toolTipRequestEventArgs = new ToolTipRequestEventArgs(e.X, e.Y, this.objHint);

        this.HandleMouseMove(toolTipRequestEventArgs);
      }
    }

    /// <summary>
    /// Timer for reshowing the tooltip ticks: enable the tooltip.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void timerToolTipShow_Tick(object sender, System.EventArgs e)
    {
      this.toolTip.Active = true;
      this.timerToolTipShow.Enabled = false;

      //Set the cursor again - otherwise the tooltip might not be shown
      Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y);
    }

    /// <summary>
    /// Cursor is moved outside of the control: hide the tooltip.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void controlForToolTip_MouseLeave(object sender, System.EventArgs e)
    {
      this.timerToolTipShow.Enabled = false;
    }
    #endregion
  }
}
