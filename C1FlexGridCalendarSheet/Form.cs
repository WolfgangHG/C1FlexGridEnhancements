using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace C1FlexGridCalendarSheet
{
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

      InitializeComponent();

      //Write target framework to window title:
      object[] targetFrameworkAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(TargetFrameworkAttribute), false);
      //There should be exactly one attribute.
      TargetFrameworkAttribute ta = (TargetFrameworkAttribute)targetFrameworkAttributes.FirstOrDefault();
      //Don't know whether a NULL check is required.
      string targetFramework = ta?.FrameworkDisplayName ?? "--unknown--";
      this.Text += $" ({targetFramework})";


      this.dateTimePickerFrom.Value = new DateTime(DateTime.Now.Year, 1, 1);
      //set it to first day of month. Otherwise, spinning the month might return broken values when the month has fewer than 31 days.
      this.dateTimePickerTo.Value = new DateTime(DateTime.Now.Year, 12, 1);

    }

    private void buttonShowSelection_Click(object sender, EventArgs e)
    {

      DateRange selectedDateRange = this.c1FlexGrid1.GetSelectedDateRange();

      if (selectedDateRange != null)
      {
        MessageBox.Show(this, $"Current Selection: {selectedDateRange}");
      }
      else
      {
        MessageBox.Show(this, $"Current Selection: nothing!");
      }
    }

    private void dateTimePickerFrom_ValueChanged(object sender, EventArgs e)
    {
      this.RenderCalendar();
    }

    private void dateTimePickerTo_ValueChanged(object sender, EventArgs e)
    {
      this.RenderCalendar();
    }

    /// <summary>
    /// Refills the calendar sheet based on the currently selected date range.
    /// </summary>
    private void RenderCalendar()
    {
      //The method ignores the day part, so don't care here.
      this.c1FlexGrid1.RenderCalendar(this.dateTimePickerFrom.Value, this.dateTimePickerTo.Value);

    }
  }
}