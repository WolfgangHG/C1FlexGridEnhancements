using C1.Framework;
using C1.Win.FlexGrid;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C1FlexGrid6AutoSizeRowHeightToLast
{
  /// <summary>
  /// This class contains the magic method that performs auto size of C1FlexGrid rows in optimized mode:
  /// Rows of merged range don't get all the same height, but the first n rows get the size necessary to display the content of non merged cells, 
  /// and the height of the final row of the merged range is set so that it can display the remaining content.
  /// E.g. row1 and row2 of the merged range will be sized to 20 pixel, row 3 will be 50 pixel.
  /// </summary>
  public static class C1FlexGridAutoSizeRowsHeightToLast
  {
    /// <summary>
    /// Autosizes grid rows in a special mode for merged ranges that span multiple rows: the first n rows of the merged range
    /// get the "necessary" height (which is the height of all non merged cells), the last row of the merged range gets the remaining height.
    /// 
    /// Default C1FlexGrid "AutoSizeRows" gives all rows the same height.
    /// </summary>
    /// <param name="flexGrid">Perform autosize on this grid.</param>
    /// <param name="rowFrom">Start row for autosizing. NULL to size grid from the beginning.
    /// If start and end row are specified, they must be valid, and all merged ranges must be completely outside or completely inside the
    /// row span.</param>
    /// <param name="rowTo">End row for autosizing. NULL to size grid to the end.</param>
    public static void AutoSizeRowsHeightToLast(this C1FlexGrid flexGrid, Nullable<int> rowFrom = null, Nullable<int> rowTo = null)
    {
      if (rowFrom.HasValue == true)
      {
        if (rowFrom < 0)
        {
          throw new ArgumentException($"Start row ({rowFrom} must be greater than 0");
        }
        if (rowFrom >= flexGrid.Rows.Count)
        {
          throw new ArgumentException($"Start row ({rowFrom} must be lower than grid row count ({flexGrid.Rows.Count}");
        }
      }
      if (rowTo.HasValue == true)
      {
        if (rowTo < 0)
        {
          throw new ArgumentException($"End row ({rowTo} must be greater than 0");
        }
        if (rowTo >= flexGrid.Rows.Count)
        {
          throw new ArgumentException($"End row ({rowTo} must be lower than grid row count ({flexGrid.Rows.Count}");
        }
      }
      if (rowFrom.HasValue == true && rowTo.HasValue == true)
      {
        if (rowFrom > rowTo)
        {
          throw new ArgumentException($"Start row ({rowFrom} must be less/equal to end row ({rowTo}");
        }
      }
      //Check for "completely inside merged range" is done in next loop.

      int rowFromCurrent = rowFrom ?? 0;
      int rowToCurrent = rowTo ?? flexGrid.Rows.Count - 1;

      flexGrid.BeginUpdate();

      //Use the same graphics object for all measurement calls.
      using (Graphics graphics = flexGrid.CreateGraphics())
      {
        //Step 1: Store height of each merged range: 
        Dictionary<int, List<MergedRangeHeight>> dicRow2MergedRangesAndHeight = new Dictionary<int, List<MergedRangeHeight>>();

        //My real life grid overrides "GetMergedRange" and returns custom ranges.
        //So also call this method.
        //This handles merge mode "custom" - a loop over "C1FlexGrid.MergedRanges" is not necessary.
        for (int row = rowFromCurrent; row <= rowToCurrent; row++)
        {
          for (int col = 0; col < flexGrid.Cols.Count; col++)
          {
            //Possible performance improvement: Increase "col" by col count of range.
            CellRange rangeMerged = flexGrid.GetMergedRange(row, col);
            if (rangeMerged.IsSingleCell == false)
            {
              ////Process only the start cell of the range:
              //if (rangeMerged.TopRow == row && rangeMerged.LeftCol == col)
              //{
              //  MeasureMergedRange(flexGrid, rangeMerged, rowFromCurrent, rowToCurrent, graphics, dicRow2MergedRangesAndHeight);
              //}

              //Ignore invisible ranges - if full column is not visible, it might be empty, and thus the automatic merged range might span the full col.
              bool bolAnyVisible = false;
              for (int colTemp = rangeMerged.LeftCol; colTemp <= rangeMerged.RightCol && bolAnyVisible == false; colTemp++)
              {
                if (flexGrid.Cols[colTemp].Visible == true)
                {
                  //If col is visible, check also all rows of the merged range:
                  for (int rowTemp = rangeMerged.TopRow; rowTemp <= rangeMerged.BottomRow && bolAnyVisible == false; rowTemp++)
                  {
                    if (flexGrid.Rows[rowTemp].Visible == true)
                    {
                      bolAnyVisible = true;
                    }
                  }
                  bolAnyVisible = true;
                }
              }

              //Measure range only if it has any visible cell:
              if (bolAnyVisible == true)
              {
                //Process only the start cell of the range:
                if (rangeMerged.TopRow == row && rangeMerged.LeftCol == col)
                {
                  MeasureMergedRange(flexGrid, rangeMerged, rowFromCurrent, rowToCurrent, graphics, dicRow2MergedRangesAndHeight);
                }
              }
            }
          }
        }


        //Step 2: Calculate the minimum height for each row (height of all non merged cells)
        List<RowHeight> listRowHeights = new List<RowHeight>();

        //Calculate the required row heights.
        //First add dummy entries for all rows that are before "rowFrom" - thus the list index is similar to the grid row index.,
        for (int row = 0; row < rowFromCurrent; row++)
        {
          listRowHeights.Add(new RowHeight(-1));
        }
        for (int row = rowFromCurrent; row <= rowToCurrent; row++)
        {
          int rowHeight = flexGrid.Rows.DefaultSize;

          //Don't increase col index in FOR loop, because it might differ for merged ranges.
          for (int col = 0; col < flexGrid.Cols.Count;)
          {
            //Only visible cols:
            if (flexGrid.Cols[col].Visible == true)
            {
              //Would it cause a performance improvement if I don't measure empty cells (no data, no image?).
              //Could the height of cell depend on its font, even if it is empty?

              //Only cells that span one row:
              CellRange rangeMerged = flexGrid.GetMergedRange(row, col);
              //Check for invisible top and bottom row, too:
              int topRowFirstVisible = GetVisibleTopRowOfRange(flexGrid, rangeMerged);
              int bottomRowLastVisible = GetVisibleBottomRowOfRange(flexGrid, rangeMerged);
              if (bottomRowLastVisible == topRowFirstVisible)
              {
                //If part of merged range, then don't measure again. Only top row is measured.
                Size sizeCell = flexGrid.MeasureCellSize(graphics, row, col);

                rowHeight = Math.Max(rowHeight, sizeCell.Height);

              }
              //Increase col count by merged range col span.
              col += (rangeMerged.RightCol - rangeMerged.LeftCol + 1);
            }
            else
            {
              //Only increase col index:
              ++col;
            }
          }

          RowHeight rowHeightData = new RowHeight(rowHeight);
          //Add all merged ranges that affect this row:
          if (dicRow2MergedRangesAndHeight.ContainsKey(row) == true)
          {
            foreach (MergedRangeHeight mergedRangeHeight in dicRow2MergedRangesAndHeight[row])
            {
              rowHeightData.MergedRangesHeights.Add(mergedRangeHeight);
            }
          }
          listRowHeights.Add(rowHeightData);

          //flexGrid.Rows[row].HeightDisplay = rowHeight;
        }

        //Step 3: Now perform sizing:
        for (int row = rowFromCurrent; row <= rowToCurrent; row++)
        {
          //Touch only visible rows.
          if (flexGrid.Rows[row].Visible == true)
          {
            RowHeight rowHeightData = listRowHeights[row];
            int rowHeightCurrent = rowHeightData.NonMergedHeight == -1 ? flexGrid.Rows[row].HeightDisplay : rowHeightData.NonMergedHeight;

            //Step 3a:
            //All merged ranges of the row: if it is the last row, then set all remaining height to the row height, it the row height is smaller
            //than the merged range height. Else keep the row height and reduce only the merged range height.

            foreach (MergedRangeHeight mergedRangeHeight in rowHeightData.MergedRangesHeights)
            {
              if (mergedRangeHeight.MergedRange.BottomRow == row)
              {
                //Last row of range: if the remaining height of the range is smaller than the current row height, then increase it.
                rowHeightCurrent = Math.Max(rowHeightCurrent, mergedRangeHeight.RemainingHeight);
              }
              else
              {
                //Decrease by row height. But do this only AFTER the row height was calculated.
              }
            }

            //Step 3b: decrease all merged ranges remaining height:
            foreach (MergedRangeHeight mergedRangeHeight in rowHeightData.MergedRangesHeights)
            {
              if (mergedRangeHeight.MergedRange.BottomRow == row)
              {
                //Bottom row was handled before.
              }
              else
              {
                //Decrease by row height. Don't go beyond "0".
                mergedRangeHeight.RemainingHeight = Math.Max(0, mergedRangeHeight.RemainingHeight - rowHeightCurrent);
              }
            }

            //Apply row height:
            flexGrid.Rows[row].HeightDisplay = rowHeightCurrent;
          } //End if (row is visible)
        }

      }
      flexGrid.EndUpdate();
    }

    /// <summary>
    /// If the merged range is part of the row range where autosize is to be performed: Measure the 
    /// height of the merged range and add it to the dictionary
    /// </summary>
    /// <param name="flexGrid">Current flexgrid</param>
    /// <param name="rangeMerged">Current merged range to check. Height will be calculated only if it is part of 
    /// <paramref name="rowFromCurrent"/> and <paramref name="rowToCurrent"/>.
    /// An exception will be throw if it is not completely inside the merged range.</param>
    /// <param name="rowFromCurrent">Start row of autosizing</param>
    /// <param name="rowToCurrent">End row of autosizing</param>
    /// <param name="graphics">Graphics used to measure height</param>
    /// <param name="dicRow2MergedRangesAndHeight">The range height is added to this dictionary: for each row of the range, an entry is added.
    /// All entries reference the same <see cref="MergedRangeHeight"/> instance</param>
    private static void MeasureMergedRange(C1FlexGrid flexGrid, CellRange rangeMerged, int rowFromCurrent,
     int rowToCurrent, Graphics graphics, Dictionary<int, List<MergedRangeHeight>> dicRow2MergedRangesAndHeight)
    {
      //Is it inside the range to autosize?
      //This check can also be done of all rows are to be autosized.
      if (rangeMerged.TopRow >= rowFromCurrent && rangeMerged.BottomRow <= rowToCurrent)
      {
        //Manipulate the range: if it starts/ends with an invisible row, then change it to start/end with visible rows.
        int topRowFirstVisible = GetVisibleTopRowOfRange(flexGrid, rangeMerged);
        int bottomRowLastVisible = GetVisibleBottomRowOfRange(flexGrid, rangeMerged);


        //If full range is invisible, then don't use it.
        //Only if range is larger than one row! The column span is not relevant.
        if (topRowFirstVisible != -1 && bottomRowLastVisible != rangeMerged.TopRow)
        {
          //My initial code:
          //Size sizeCell = flexGrid.MeasureCellSize(graphics, rangeMerged.TopRow, rangeMerged.LeftCol);
          //Workaround suggested by Mescius:
          int height = MeasureCellSizeForRange(flexGrid, rangeMerged);

          Trace.WriteLine("Measuring range " + rangeMerged);
          Trace.WriteLine("\tMeasureCellSize returned " + flexGrid.MeasureCellSize(graphics, rangeMerged.TopRow, rangeMerged.LeftCol));
          Trace.WriteLine("\tWorkaround returned " + height);
          //Here, we can add also entries for invisible rows.

          //Manipulate the range so that it starts/ends with a visible row.
          //The object must be the same instance for all entries, because the remaining height is manipulated:
          CellRange rangeMergedVisible = flexGrid.GetCellRange(topRowFirstVisible, rangeMerged.LeftCol, bottomRowLastVisible, rangeMerged.RightCol);
          //MergedRangeHeight mergedRangeHeight = new MergedRangeHeight(rangeMergedVisible, sizeCell.Height);
          MergedRangeHeight mergedRangeHeight = new MergedRangeHeight(rangeMergedVisible, height);
          for (int row = rangeMerged.TopRow; row <= rangeMerged.BottomRow; row++)
          {
            if (dicRow2MergedRangesAndHeight.ContainsKey(row) == false)
            {
              dicRow2MergedRangesAndHeight.Add(row, new List<MergedRangeHeight>());
            }
            dicRow2MergedRangesAndHeight[row].Add(mergedRangeHeight);
          }
        }
      }
      else
      {
        //Additional checks: merged range should not be completely inside the row range to autosize:
        if (rangeMerged.TopRow < rowFromCurrent && rangeMerged.BottomRow >= rowFromCurrent)
        {
          //Range starts before start row of autosizing, and ends inside the range.
          throw new ArgumentException($"Merged range {rangeMerged} must be completely inside row range to autosize ({rowFromCurrent} - {rowToCurrent})");
        }
        if (rangeMerged.TopRow <= rowToCurrent && rangeMerged.BottomRow > rowToCurrent)
        {
          //Range starts before inside the autosize range and ends after the autosize end row.
          throw new ArgumentException($"Merged range {rangeMerged} must be completely inside row range to autosize ({rowFromCurrent} - {rowToCurrent})");
        }
        if (rangeMerged.TopRow < rowFromCurrent && rangeMerged.BottomRow > rowToCurrent)
        {
          //Range starts before start row of autosizing and ends after the autosize end row.
          throw new ArgumentException($"Merged range {rangeMerged} must be completely inside row range to autosize ({rowFromCurrent} - {rowToCurrent})");
        }
      }

    }

    /// <summary>
    /// Get height of a cell range: sum of the measured ranges of all cells.
    /// </summary>
    /// <param name="c1FlexGrid"></param>
    /// <param name="cellRange"></param>
    /// <returns>Sum of height of all rows in range (includes also invisible rows)</returns>
    private static int MeasureCellSizeForRange(C1FlexGrid c1FlexGrid1, CellRange cellRange)
    {
      //return c1FlexGrid1.MeasureCellSize(cellRange.TopRow, cellRange.LeftCol).Height * (cellRange.BottomRow - cellRange.TopRow + 1);
      var height = 0;
      for (int r = cellRange.r1; r <= cellRange.r2; r++)
      {
        height += c1FlexGrid1.MeasureCellSize(r, cellRange.c1).Height;
      }

      return height;
    }

    /// <summary>
    /// If the bottom row of a range is invisible, then return that last visible row of a range.
    /// 
    /// If the full range is invisible, it returns "-1".
    /// </summary>
    /// <param name="flexGrid"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    private static int GetVisibleBottomRowOfRange(C1FlexGrid flexGrid, CellRange range)
    {
      //Manipulate the end of the range: if the end row is invisible, only the last visible row is relevant for us, becuase
      //it will receive the remaining height.
      int bottomRowLastVisible = range.BottomRow;
      //Fallback: if full range is invisible, then stop at top row.
      while (flexGrid.Rows[bottomRowLastVisible].Visible == false)
      {
        if (bottomRowLastVisible == range.TopRow)
        {
          return -1;
        }
        --bottomRowLastVisible;
      }

      return bottomRowLastVisible;
    }

    /// <summary>
    /// If the topw row of a range is invisible, then return that first visible row of a range.
    /// 
    /// If the full range is invisible, it returns "-1".
    /// </summary>
    /// <param name="flexGrid"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    private static int GetVisibleTopRowOfRange(C1FlexGrid flexGrid, CellRange range)
    {
      //Manipulate the start of the range: if the start row is invisible, only the first visible row is relevant for us, because
      //height calculation will start here
      int topRowFirstVisible = range.TopRow;
      //Fallback: if full range is invisible, then stop at top row.
      while (flexGrid.Rows[topRowFirstVisible].Visible == false)
      {
        if (topRowFirstVisible == range.BottomRow)
        {
          return -1;
        }
        ++topRowFirstVisible;
      }

      return topRowFirstVisible;
    }

    /// <summary>
    /// Helper class: used while calculating the row height: contains the height of all single row cells
    /// and a list of merged ranges heights.
    /// </summary>
    private class RowHeight
    {
      public RowHeight(int nonMergedHeight)
      {
        this.NonMergedHeight = nonMergedHeight;

      }

      /// <summary>
      /// Height of the non merged/single cells of the row.
      /// </summary>
      public int NonMergedHeight
      {
        get;
        set;
      }

      /// <summary>
      /// List of merged ranges and their calculated height
      /// </summary>
      public List<MergedRangeHeight> MergedRangesHeights
      {
        get;
        set;
      } = new List<MergedRangeHeight>();
    }

    /// <summary>
    /// Helper class: used while calculating the height: for each merged range, it contains the "real" cell range (which
    /// might be smaller if the top or bottom row is invisible), the calculated row height and the remaining height (that was
    /// not used for previous rows)
    /// </summary>
    private class MergedRangeHeight
    {
      public MergedRangeHeight(CellRange mergedRange, int height)
      {
        this.MergedRange = mergedRange;
        this.Height = height;

        this.RemainingHeight = height;
      }

      /// <summary>
      /// Merged range. TopRow and BottomRow should be visible.
      /// </summary>
      public CellRange MergedRange
      {
        get;
        set;
      }

      /// <summary>
      /// Height of the merged range
      /// </summary>
      public int Height
      {
        get;
        set;
      }

      public int RemainingHeight
      {
        get;
        set;
      }

    }
  }
}