using System;
using System.Drawing;
using System.Windows.Forms;

namespace C1FlexGrid6CalendarSheet
{
  public partial class Form : System.Windows.Forms.Form
  {
    public Form()
    {
      InitializeComponent();

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