using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C1FlexGridCalendarSheet
{
  /// <summary>
  /// A date span from start date to end date.
  /// </summary>
  public class DateRange
  {
    public DateRange(DateTime start, DateTime end)
    {
      this.Start = start;
      this.End = end;
    }
    public DateTime Start
    {
      get;
      set;
    }

    public DateTime End
    {
      get;
      set;
    }

    public override string ToString()
    {
      return this.Start.ToShortDateString() + " - " + this.End.ToShortDateString();
    }
  }
}
