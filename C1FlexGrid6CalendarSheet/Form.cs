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


      this.c1FlexGrid1.RenderCalendar(new DateTime(2023, 1, 1), new DateTime(2024, 12, 31));

      //Set fixed style after filling the grid. Does not seem to work befor any column info is set:
      this.c1FlexGrid1.Styles.Fixed.BackColor = SystemColors.Control;
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
  }
}