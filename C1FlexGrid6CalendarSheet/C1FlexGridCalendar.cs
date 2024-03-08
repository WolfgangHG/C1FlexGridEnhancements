using C1.Win.FlexGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace C1FlexGrid6CalendarSheet
{
  public partial class C1FlexGridCalendar : C1FlexGrid
  {
    private DateTime dateFrom;
    private DateTime dateTo;

    /// <summary>
    /// Index of first day column.
    /// </summary>
    private const int COL_FIRST_DAY = 2;

    public C1FlexGridCalendar()
    {
      InitializeComponent();


      this.AllowSorting = C1.Win.FlexGrid.AllowSortingEnum.None;
      this.AllowEditing = false;

      //Allow merging in fixed column (year):
      this.AllowMergingFixed = AllowMergingEnum.FixedOnly;

      //Setting Fixed BackColor does not work here...
      this.Styles.Fixed.BackColor = SystemColors.Control;

      this.Styles.SelectedRowHeader.BackColor = Color.Green;
      this.Styles.SelectedColumnHeader.BackColor = Color.Green;
    }

    /// <summary>
    /// Fills the grid with the months according to the date range.
    /// </summary>
    /// <param name="from">From date. Only the month/year part is used.</param>
    /// <param name="to">To date. Only the month/year part is used.</param>
    public void RenderCalendar (DateTime from, DateTime to)
    {
      //If "to" is before "from", switch to 
      //Set to first day/last day of month if different:
      this.dateFrom = new DateTime(from.Year, from.Month, 1);
      this.dateTo = new DateTime(to.Year, to.Month, DateTime.DaysInMonth(to.Year, to.Month));

      int monthCount = GetMonthDifference(from, to);
      this.Rows.Count = 1 + monthCount;
      this.Rows.Fixed = 1;
      
      //2 fixed columns, 31 day columns
      this.Cols.Count = 33;
      this.Cols.Fixed = 2;

      this.Cols[0].Width = 100;
      for (int day = 1; day <= 31; day++)
      {
        this.Cols[GetColFromDay(day)].Width = 25;
      }

      //Header row: day numbers:
      for (int day = 1; day <= 31; day++)
      {
        this[0, GetColFromDay(day)] = day;
      }

      for (int month = 1; month <= monthCount; month++)
      {
        //Month is 1 based. So subtract 1 to add it to the start date:
        DateTime firstDayOfCurrentMonth = this.dateFrom.AddMonths(month - 1);
        this[month, 0] = firstDayOfCurrentMonth.Year;
        this[month, 1] = firstDayOfCurrentMonth.ToString("MMMM");

        //Days in Month:
        for (int day = 1; day <= DateTime.DaysInMonth(firstDayOfCurrentMonth.Year, firstDayOfCurrentMonth.Month); day++)
        {
          this[month, GetColFromDay(day)] = new DateTime(firstDayOfCurrentMonth.Year, firstDayOfCurrentMonth.Month, day).ToString("ddd");

        }
      }
    }

    /// <summary>
    /// Is the current cell highlighted? The date selection does not match the C1FlexGrid "Selection"!
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public override bool IsCellHighlighted(int row, int col)
    {
      //Get current selection:
      CellRange rangeSel = this.Selection;
      
      if (rangeSel.IsValid == false)
      {
        return base.IsCellHighlighted(row, col);
      }

      if (row == this.Row && col == this.Col)
      {
        //Focused cell: render as "Selected", so use default code.
        return base.IsCellHighlighted(row, col);
      }

      DateRange selectedDateRange = this.GetSelectedDateRange();
      
      if (selectedDateRange == null)
      {
        //Nothing selected?!
        return base.IsCellHighlighted(row, col);
      }

      //Get Date from cell!
      DateTime? datCell = this.GetDate(row, col);
      //Also get the last day of the month:
      DateTime? lastDayOfMonth = this.GetLastDayOfMonth(row);
      if (datCell != null)
      {
        //Cell has date: is it inside the selection?
        return (selectedDateRange.Start <= datCell && selectedDateRange.End >= datCell);
      }
      else if (lastDayOfMonth != null)
      {
        //Is the current selection starting before the end of the month and ends after? Then select cell:
        return (selectedDateRange.Start <= lastDayOfMonth && selectedDateRange.End >= lastDayOfMonth);
      }
      else
      {
        //Fixed or "out of range" day.
        return false;
      }
    }

    /// <summary>
    /// If style "Styles.SelectedColumnHeader" is set, then all columns whose day is selected shall be rendered as "selected".
    /// 
    /// C1FlexGrid renders only the columns for the "Selection" range.
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public override bool IsCellSelectedColumnHeader(int row, int col)
    {
      //Only for fixed row:
      if (row >= this.Rows.Fixed)
      {
        return base.IsCellSelectedColumnHeader(row, col);
      }
      //Fixed columns: do nothing here:
      if (col < this.Cols.Fixed)
      {
        return base.IsCellSelectedColumnHeader(row, col);
      }

      DateRange selectedDateRange = this.GetSelectedDateRange();
      if (selectedDateRange == null)
      {
        //This might happen, if the col is not in a valid date range.
        //We could also grid default, but this looks strange, as no date is selected.
        //return base.IsCellSelectedColumnHeader(row, col);
        return false;
      }

      int day = GetDayFromCol(col);

      //"col" is the day of month. Check whether the selection contains this day.
      if ((selectedDateRange.End.Month - selectedDateRange.Start.Month) > 1)
      {
        //Multi month range:
        return true;
      }
      else if ((selectedDateRange.End.Month - selectedDateRange.Start.Month) == 1)
      {
        //Single month break: 
        return (day >= selectedDateRange.Start.Day || day <= selectedDateRange.End.Day);

      }
      else
      {
        //Inside single month:
        return (day >= selectedDateRange.Start.Day && day <= selectedDateRange.End.Day);
      }
    }

    /// <summary>
    /// After selection change: re-render all affected full rows.
    /// This is necessary, because the selected date ranges do not match the grid rendering range.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnAfterSelChange(RangeEventArgs e)
    {
      //Invalidate cells to the right/left of range:

      CellRange rangeSel = e.NewRange;

      //Invalidate old range:
      if (e.OldRange.IsValid)
      {
        this.Invalidate(e.OldRange.TopRow, 1, e.OldRange.BottomRow, this.Cols.Count - 1);
      }

      //Invalidate new range:
      if (rangeSel.IsValid == true)
      {
        //Invalidate all rows:
        this.Invalidate(rangeSel.TopRow, 1, rangeSel.BottomRow, this.Cols.Count - 1);
      }

      //Invalidate all column headers:
      this.Invalidate(0, this.Cols.Fixed, 0, this.Cols.Count - 1);

      base.OnAfterSelChange(e);
    }

    /// <summary>
    /// Get range of selected dates (start/end range).
    /// </summary>
    /// <remarks>
    /// This is the method that does most of the work: it builds the date range based on the grids "Row/Col" 
    /// (this is the cell where the user started selection) and "RowSel/ColSel" (this is the cell that is currently focused)
    /// 
    /// </remarks>
    /// <returns></returns>
    public DateRange GetSelectedDateRange()
    {
      //We cannot use "CellRange" here, because it seems to be normalized always. Just use it to check whether there
      //is a valid selection.
      CellRange rangeSel = this.Selection;
      if (rangeSel.IsValid == false)
      {
        return null;
      }

      int rowMonthStart;
      int dayStart;
      int rowMonthEnd;
      int dayEnd;

      //"Row" = start row of selection, "RowSel" = current cursor row.

      //Different ways to create a selection by mouse: 
      if (this.RowSel >= this.Row && this.ColSel >= this.Col)
      {
        //single cell or selection in same row to the right or selection down and to the right (default selection):
        rowMonthStart = this.Row;
        dayStart = GetDayFromCol(this.Col);

        rowMonthEnd = this.RowSel;
        dayEnd = GetDayFromCol(this.ColSel);
      }
      else if (this.RowSel > this.Row && this.ColSel < this.Col)
      {
        //Selection down and to the left: 
        rowMonthStart = this.Row;
        dayStart = GetDayFromCol(this.Col);

        rowMonthEnd = this.RowSel;
        dayEnd = GetDayFromCol(this.ColSel);
      }
      else if (this.RowSel <= this.Row && this.ColSel  < this.Col)
      {
        //Move mouse cursor up (or same row) and to the left:
        rowMonthStart = this.RowSel;
        dayStart = GetDayFromCol(this.ColSel);

        rowMonthEnd = this.Row;
        dayEnd = GetDayFromCol(this.Col);
      }
      else if (this.RowSel < this.Row && this.ColSel >= this.Col)
      {
        //Move cursor up and to the right (or same col):
        rowMonthStart = this.RowSel;
        dayStart = GetDayFromCol(this.ColSel);

        rowMonthEnd = this.Row;
        dayEnd = GetDayFromCol(this.Col);
      }
      else
      {
        //Should probably never happen, all combinations are covered by previous conditions....
        rowMonthStart = this.Row;
        dayStart = GetDayFromCol(this.Col);

        rowMonthEnd = this.RowSel;
        dayEnd = GetDayFromCol(this.ColSel);
      }

      int yearStart, monthStart;
      GetYearMonthFromRow(rowMonthStart, out yearStart, out monthStart);

      int yearEnd, monthEnd;
      GetYearMonthFromRow(rowMonthEnd, out yearEnd, out monthEnd);

      //If "dayStart" is higher than last day of month, then go to first day of following month.
      int daysInMonthStart = DateTime.DaysInMonth(yearStart, monthStart);
      //Work around crashes for invalid "dayStart" (selection is in fixed col) => return.
      if (dayStart <= 0)
      {
        return null;
      }
      if (dayStart > daysInMonthStart)
      {
        //Special case: if selection is a single cell beyond the end of the month, then do nothing.
        if (yearStart == yearEnd && monthStart == monthEnd && dayEnd > daysInMonthStart)
        {
          return null;
        }

        ++monthStart;
        dayStart = 1;
        //Wrap year if month goes beyound december. This should not happen here, as december has 31 days.
        if (monthStart > 12)
        {
          ++yearStart;
          monthStart = 1;
        }

      }
      DateTime startDate = new DateTime(yearStart, monthStart, dayStart);


      //If "dayEnd" is higher than last day of month, then go to last day of this month.
      int daysInMonthEnd = DateTime.DaysInMonth(yearEnd, monthEnd);
      if (dayEnd > daysInMonthEnd)
      {
        dayEnd = daysInMonthEnd;
      }

      DateTime endDate = new DateTime(yearEnd, monthEnd, dayEnd);

      return new DateRange(startDate, endDate);
    }

    /// <summary>
    /// Converts a grid column index to a day index: subtract the fixed column count and add "1", as the day is 1 based.
    /// </summary>
    /// <param name="col"></param>
    /// <returns></returns>
    private int GetDayFromCol(int col)
    {
      return col - COL_FIRST_DAY + 1;
    }

    /// <summary>
    /// Converts a a day index to a grid column index: add the fixed column count and subtract "1", as the day is 1 based.
    /// </summary>
    /// <param name="col"></param>
    /// <returns></returns>
    private int GetColFromDay(int day)
    {
      return day + COL_FIRST_DAY - 1;
    }

    /// <summary>
    /// Get date for cell. Returns null of cell is no valid date (fixed or end of a month with less than 31 days)
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    private DateTime? GetDate(int row, int col)
    {
      if (row >= 1 && col >= COL_FIRST_DAY)
      {
        //"row" = month.
        int year, month;
        GetYearMonthFromRow(row, out year, out month);

        int daysInMonth = DateTime.DaysInMonth(year, month);
        int day = GetDayFromCol(col);
        if (day > daysInMonth)
        {
          //Month has fewer days: return end of month???? 
          return null;
        }
        else
        {
          return new DateTime(year, month, day);
        }
      }
      else
      {
        //Fixed?
        return null;
      }
    }

    /// <summary>
    /// Get the last day of the month. Returns null if row is invalid (fixed)
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    private DateTime? GetLastDayOfMonth (int row)
    {
      if (row >= 1)
      {
        //"row" = month.
        int year, month;
        GetYearMonthFromRow(row, out year, out month);

        int daysInMonth = DateTime.DaysInMonth(year, month);

        return new DateTime(year, month, daysInMonth);
      }
      else
      {
        //Fixed?
        return null;
      }
    }

    /// <summary>
    /// Get year and month value for a cell index.
    /// </summary>
    /// <param name="row">Grid row - that is a month number</param>
    /// <param name="_year">Output: year of date</param>
    /// <param name="_month">Output: month of date (1 based)</param>
    private void GetYearMonthFromRow(int row, out int _year, out int _month)
    {
      //Add the row index to the start month to get a "month from start date" number. Remove one month.
      int month = (row + this.dateFrom.Month) - 1;
      //Calculate year: add the number of full years.
      //Subtract 1 from the month to make it zero based.
      int year = this.dateFrom.Year + ((month - 1) / 12);
      //Calculate month in year: Subtract  1 from the month to make it base 0, and add 1 afterwards.
      month = ((month - 1) % 12) + 1;

      _year = year;
      _month = month;
    }

    private static int GetMonthDifference(DateTime dat1, DateTime dat2)
    {
      //Same year?
      if (dat1.Year == dat2.Year)
      {
        //Just subtract the month:
        //Add one month, because also partial months are relevant.
        return( dat2.Month - dat1.Month ) + 1;
      }
      else
      {
        //Different years: count months in loop:
        DateTime datTemp = dat1;
        int months = 0;
        //Set end date to first day of month. Otherwise the remaining month would count, too.
        DateTime datEnde = new DateTime(dat2.Year, dat2.Month, 1);
        while (datTemp.CompareTo(datEnde) < 0)
        {
          datTemp = datTemp.AddMonths(1);
          months += 1;
        }
        //And add one month:
        ++months;


        return months;
      }
    }
  }
}
