using System;
using System.Collections.Generic;
using System.Drawing;
using C1.Win.FlexGrid;

namespace C1FlexGrid6BorderPainter
{
  /// <summary>
  /// This class can draw free borders around cells.
  /// 
  /// All borders are drawn with the same pen, so this class cannot be used to draw borders 
  /// in different colors or styles !
  /// 
  /// The borders are drawn exactly on the grid lines, so that top or left borders are actually
  /// drawn in the cell above/to the left. If the direct neighbour is invisible the next visible one
  /// is searched.
  /// 
  /// Usage:
  /// -Create the border painter and specifiy the pen used for all cells.
  /// -When the grid is initialized call "ResetGrid (rows, cols)" to specifiy the row / col count.
  /// -Add borders: "SetBorders (cell range)".
  /// 
  /// -In the grids "OwnerDrawCell" event call "DrawBorders" and specify the range.
  /// </summary>
  public class C1FlexGridBorderPainter
  {
    #region Variables
    /// <summary>
    /// The pen used for drawing the borders.
    /// </summary>
    private Pen pen = null;

    /// <summary>
    /// 2-dimensional Array of borders. The number of cols and rows must match the col/row numbers
    /// of the grid.
    /// For each cell it contains information wether the cell should have top/bottom/right/left borders.
    /// 
    /// First coordinate: row
    /// Second coordinate: col (just like in the flexgrid)
    /// 
    /// </summary>
    private BorderType[,] arrBorders = null;

    /// <summary>
    /// This ist the flexgrid for painting.
    /// </summary>
    private C1FlexGrid flexGrid = null;

    /// <summary>
    /// TRUE: Borders auch in MergedRange zeichnen, wenn definiert. FALSE: Borders NICHT in MergedRange machen.
    /// </summary>
    private bool bolBorderInMergedRange = false;



    /// <summary>
    /// For handling of merged ranges in "DrawBorders": We need the row start position and height very often - so cache it.
    /// </summary>
    private List<KeyValuePair<int, int>> listRowStartHeight;


    /// <summary>
    /// For handling of merged ranges in "DrawBorders": We need the col start position and width very often - so cache it.
    /// </summary>
    private List<KeyValuePair<int, int>> listColStartWidth;
    #endregion

    #region Constructor
    /// <summary>
    /// Create the border painter and specify the pen.
    /// </summary>
    /// <param name="_flexGrid">The grid on which the drawing is performed.</param>
    /// <param name="_pen">Pen used for rendering the borders.</param>
    public C1FlexGridBorderPainter(C1FlexGrid _flexGrid, Pen _pen)
    {
      if (_flexGrid == null)
      {
        throw new ArgumentNullException("_flexGrid");
      }
      if (_pen == null)
      {
        throw new ArgumentNullException("_pen");
      }

      this.flexGrid = _flexGrid;
      this.pen = _pen;

      //Register a GridChanged handler:
      //Not applyable for programmatic drag/move, so don't do it!
      this.flexGrid.GridChanged += new GridChangedEventHandler(this.flexGrid_GridChanged);
    }

    /// <summary>
    /// Copy the border settings of an existing border painter to a new one, 
    /// using a different target grid (with same dimensions as the original grid!)
    /// 
    /// </summary>
    /// <param name="_flexGrid">The grid on which the drawing is performed (not NULL).
    /// Can be different from the </param>
    /// <param name="_borderPainterSource">Source border painter, whose border info is copied
    /// to this instance. The Pen is NOT copied!</param>
    /// <param name="_pen">This is the pen for the new border painter instance (not cloned or copied!)</param>
    public C1FlexGridBorderPainter(C1FlexGrid _flexGrid, C1FlexGridBorderPainter _borderPainterSource, Pen _pen)
    {
      if (_flexGrid == null)
      {
        throw new ArgumentNullException("_flexGrid");
      }
      if (_borderPainterSource == null)
      {
        throw new ArgumentNullException("_borderPainterSource");
      }
      if (_pen == null)
      {
        throw new ArgumentNullException("_pen");
      }

      //Check Flexgrid dimensions!
      if (_flexGrid.Rows.Count != _borderPainterSource.arrBorders.GetLength(0))
      {
        throw new ArgumentException("_flexGrid.Rows.Count (" + _flexGrid.Rows.Count + ") does not match Borderpainter dimension 0 (" + _borderPainterSource.arrBorders.GetLength(0) + ")");
      }
      if (_flexGrid.Cols.Count != _borderPainterSource.arrBorders.GetLength(1))
      {
        throw new ArgumentException("_flexGrid.Cols.Count (" + _flexGrid.Cols.Count + ") does not match Borderpainter dimension 1 (" + _borderPainterSource.arrBorders.GetLength(1) + ")");
      }

      this.flexGrid = _flexGrid;
      this.pen = _pen;

      //copy the contents of the border array:
      this.arrBorders = new BorderType[_flexGrid.Rows.Count, _flexGrid.Cols.Count];
      Array.Copy(_borderPainterSource.arrBorders, this.arrBorders, _borderPainterSource.arrBorders.Length);

    }
    #endregion

    #region Public methods
    /// <summary>
    /// Reset the grid and invalidate all border information created up to now.
    /// 
    /// This should be called when the grid is refilled and all borders should be discarded.
    /// </summary>
    /// <param name="_intRowCount">New row count of grid</param>
    /// <param name="_intColCount">New col count of grid</param>
    public void ResetGrid(int _intRowCount, int _intColCount)
    {
      //create the array:
      this.arrBorders = new BorderType[_intRowCount, _intColCount];
      //Set default values (no border)
      for (int intRow = 0; intRow < _intRowCount; intRow++)
      {
        for (int intCol = 0; intCol < _intColCount; intCol++)
        {
          this.arrBorders[intRow, intCol] = BorderType.None;
        }
      }

      this.listRowStartHeight = null;
      this.listColStartWidth = null;
    }

    /// <summary>
    /// Reset all data about column/row start position and with. This is needed after a programmatisch fill and
    /// also after a column resizing.
    /// </summary>
    public void ResetRowColPositionData()
    {
      this.listRowStartHeight = null;
      this.listColStartWidth = null;
    }

    /// <summary>
    /// Remove n rows at position x from the end grid, and keep the rest (is moved down)
    /// </summary>
    /// <param name="_intPosition">The is the position of the first removed row</param>
    /// <param name="_intRemoveCount">Number of rows to be removed</param>
    public void RemoveRows(int _intPosition, int _intRemoveCount)
    {
      int intRowCountOriginal = this.arrBorders.GetLength(0);
      int intColCount = this.arrBorders.GetLength(1);

      //FIRST: Move all data by n rows up in current array, starting from the end:
      for (int intIndex = _intPosition; intIndex < (intRowCountOriginal - _intRemoveCount); intIndex++)
      {
        for (int intCol = 0; intCol < intColCount; intCol++)
        {
          this.arrBorders[intIndex, intCol] = this.arrBorders[intIndex + _intRemoveCount, intCol];
        }
      }


      //copy the contents of the old array to the new one
      BorderType[,] tempArray = new BorderType[intRowCountOriginal - _intRemoveCount, intColCount];
      //Use size of new array:
      Array.Copy(this.arrBorders, tempArray, tempArray.Length);

      //set the original to the new array
      this.arrBorders = tempArray;

      //WKnauf 09.06.2017: Reset der Listen, die für MergedRanges die Höhen/Breiten enthalten. 
      //Die werden hier nämlich nicht angepasst.
      this.ResetRowColPositionData();
    }

    /// <summary>
    /// Insert n rows at position x and moves the content of the previous cells.
    /// </summary>
    /// <param name="_intPosition">This is the position where the rows should be inserted</param>
    /// <param name="_intInsertCount">Number of rows to be inserted</param>
    public void InsertRows(int _intPosition, int _intInsertCount)
    {
      int intRowCountOriginal = this.arrBorders.GetLength(0);
      int intColCount = this.arrBorders.GetLength(1);


      BorderType[,] tempArray = new BorderType[intRowCountOriginal + _intInsertCount, intColCount];
      //copy the contents of the old array to the new one
      Array.Copy(this.arrBorders, tempArray, this.arrBorders.Length);

      //Move all data by n rows down, starting from the end:
      //for (int intIndex = intRowCountOriginal - 1; intIndex > (_intPosition + _intInsertCount); intIndex--)
      for (int intIndex = intRowCountOriginal - 1; intIndex >= _intPosition; intIndex--)
      {
        for (int intCol = 0; intCol < intColCount; intCol++)
        {
          tempArray[intIndex + _intInsertCount, intCol] = tempArray[intIndex, intCol];
        }
      }

      //Reset the borders in the newly added range:
      for (int intRow = _intPosition; intRow < (_intPosition + _intInsertCount); intRow++)
      {
        for (int intCol = 0; intCol < intColCount; intCol++)
        {
          tempArray[intRow, intCol] = BorderType.None;
        }
      }

      //set the original to the new array
      this.arrBorders = tempArray;

      //WKnauf 09.06.2017: Reset der Listen, die für MergedRanges die Höhen/Breiten enthalten. 
      //Die werden hier nämlich nicht angepasst.
      this.ResetRowColPositionData();

    }

    /// <summary>
    /// Reset ALL border information for the specified cell range.
    /// </summary>
    /// <param name="_intTopRow">Top row of cell range, must be valid</param>
    /// <param name="_intLeftCol">Left col of cell range, must be valid</param>
    /// <param name="_intBottomRow">Bottom row of cell range, must be valid</param>
    /// <param name="_intRightCol">Right col of cell range, must be valid</param>
    /// <exception cref="System.ArgumentException">If top row is larger than bottom row, or left col is larger than right col</exception>
    public void ClearBorders(int _intTopRow, int _intLeftCol, int _intBottomRow, int _intRightCol)
    {
      //Check values:
      this.CheckRange(_intTopRow, _intLeftCol, _intBottomRow, _intRightCol);

      //Reset the borders in the newly added range:
      for (int intRow = _intTopRow; intRow <= _intBottomRow; intRow++)
      {
        for (int intCol = _intLeftCol; intCol <= _intRightCol; intCol++)
        {
          this.arrBorders[intRow, intCol] = BorderType.None;
        }
      }
    }

    /// <summary>
    /// Set the borders for the specified cell range (borders are drawn all around the range.
    /// 
    /// Ranges might overlap !
    /// </summary>
    /// <param name="_intTopRow">Top row of cell range, must be valid</param>
    /// <param name="_intLeftCol">Left col of cell range, must be valid</param>
    /// <param name="_intBottomRow">Bottom row of cell range, must be valid</param>
    /// <param name="_intRightCol">Right col of cell range, must be valid</param>
    /// <exception cref="System.ArgumentException">If top row is larger than bottom row, or left col is larger than right col</exception>
    public void SetBorders(int _intTopRow, int _intLeftCol, int _intBottomRow, int _intRightCol)
    {
      //Call the method with partial borders.
      this.SetBorders(_intTopRow, _intLeftCol, _intBottomRow, _intRightCol,
        BorderType.Bottom | BorderType.Left | BorderType.Right | BorderType.Top);
    }

    /// <summary>
    /// Set the borders for the specified cell range (borders are drawn all around the range.
    /// 
    /// Ranges might overlap !
    /// </summary>
    /// <param name="_intTopRow">Top row of cell range, must be valid</param>
    /// <param name="_intLeftCol">Left col of cell range, must be valid</param>
    /// <param name="_intBottomRow">Bottom row of cell range, must be valid</param>
    /// <param name="_intRightCol">Right col of cell range, must be valid</param>
    /// <param name="_borderType">Type of the border: top, left, right, bottom or any combination.</param>
    /// <exception cref="System.ArgumentException">If top row is larger than bottom row, or left col is larger than right col</exception>
    public void SetBorders(int _intTopRow, int _intLeftCol, int _intBottomRow, int _intRightCol, BorderType _borderType)
    {
      //Check values:
      this.CheckRange(_intTopRow, _intLeftCol, _intBottomRow, _intRightCol);

      //Set the left border for all cells of the first col of the range:
      if ((_borderType & BorderType.Left) == BorderType.Left)
      {
        for (int intRow = _intTopRow; intRow <= _intBottomRow; intRow++)
        {
          //Merge a left border:
          this.arrBorders[intRow, _intLeftCol] = this.arrBorders[intRow, _intLeftCol] | BorderType.Left;
        }
      }

      //Same for the right border:
      if ((_borderType & BorderType.Right) == BorderType.Right)
      {
        for (int intRow = _intTopRow; intRow <= _intBottomRow; intRow++)
        {
          //Merge a right border:
          this.arrBorders[intRow, _intRightCol] = this.arrBorders[intRow, _intRightCol] | BorderType.Right;
        }
      }

      //Top row has a top border:
      if ((_borderType & BorderType.Top) == BorderType.Top)
      {
        for (int intCol = _intLeftCol; intCol <= _intRightCol; intCol++)
        {
          //Merge a top border:
          this.arrBorders[_intTopRow, intCol] = this.arrBorders[_intTopRow, intCol] | BorderType.Top;
        }
      }

      //Bottom row has a bottom border:
      if ((_borderType & BorderType.Bottom) == BorderType.Bottom)
      {
        for (int intCol = _intLeftCol; intCol <= _intRightCol; intCol++)
        {
          //Merge a top border:
          this.arrBorders[_intBottomRow, intCol] = this.arrBorders[_intBottomRow, intCol] | BorderType.Bottom;
        }
      }
    }

    /// <summary>
    /// Which borders does the cell at x/y have?
    /// Top borders might also be the bottom border of the cell above.
    /// Right borders might also be the left border of the cell right.
    /// Same for bottom or right borders.
    /// </summary>
    /// <param name="_intRow">Row index of cell</param>
    /// <param name="_intCol">Column index of cell</param>
    /// <returns>Borders for this cell</returns>
    public BorderType GetBorders(int _intRow, int _intCol)
    {
      //First check the cell itself.
      BorderType borderType = this.arrBorders[_intRow, _intCol];

      //If we are in a merged range, we have to check the bottom/right borders against the end of this range!
      CellRange cellRangeMerged = this.flexGrid.GetMergedRange(_intRow, _intCol);
      /*bool todo_NoMerged;
      CellRange cellRangeMerged;
      //if (this.bolBorderInMergedRange == true)
      if (false)
      {
        //Border in merged range: SingleCell:
        cellRangeMerged = this.flexGrid.GetCellRange(_intRow, _intCol);
      }
      else
      {
        //Border around the MergedRange: check the full range.
        cellRangeMerged = this.flexGrid.GetMergedRange(_intRow, _intCol);
      }*/
      if (cellRangeMerged.IsValid == true && cellRangeMerged.IsSingleCell == false)
      {
        //WKnauf 17.06.2013: not needed any more - this method is called for EACH cell of the merged range!
        /*
        //Take the borders of the top/left cell.
        //borderType = this.arrBorders[cellRangeMerged.TopRow, cellRangeMerged.LeftCol];
        //Now also check borders of the bottom/right cell and merge !
         
        if ((this.arrBorders[_intRow, cellRangeMerged.RightCol] & HGFlexGridBorderPainterBorderType.Right) == HGFlexGridBorderPainterBorderType.Right)
        {
          //right edge of range has a right border: draw it !
          borderType = borderType | HGFlexGridBorderPainterBorderType.Right;
        }
        if ((this.arrBorders[cellRangeMerged.BottomRow, _intCol] & HGFlexGridBorderPainterBorderType.Bottom) == HGFlexGridBorderPainterBorderType.Bottom)
        {
          //bottom edge of range has a bottom border: draw it !
          borderType = borderType | HGFlexGridBorderPainterBorderType.Bottom;
        }*/

        if (this.bolBorderInMergedRange == false)
        {
          //Remove left/top/bottom/right border if this is part of a merged range and the mode "Draw borders inside merged range" is OFF!
          if (cellRangeMerged.LeftCol < _intCol && (borderType & BorderType.Left) == BorderType.Left)
          {
            borderType = borderType ^ BorderType.Left;
          }
          if (cellRangeMerged.RightCol > _intCol && (borderType & BorderType.Right) == BorderType.Right)
          {
            borderType = borderType ^ BorderType.Right;
          }
          if (cellRangeMerged.TopRow < _intRow && (borderType & BorderType.Top) == BorderType.Top)
          {
            borderType = borderType ^ BorderType.Top;
          }
          if (cellRangeMerged.BottomRow > _intRow && (borderType & BorderType.Bottom) == BorderType.Bottom)
          {
            borderType = borderType ^ BorderType.Bottom;
          }
        }
      }
      //Next, check the neighbour cells.

      #region Top border
      //Check for top border: might also be the bottom border of the cell above !
      //Don't search neighbour column if the current cell is INSIDE a cellrange!
      if (_intRow > 0 && _intRow == cellRangeMerged.TopRow)
      {
        //check next visible row  !
        int intRowNextVisible = _intRow - 1;
        while (intRowNextVisible >= 0 && this.flexGrid.Rows[intRowNextVisible].Visible == false)
        {
          --intRowNextVisible;
        }

        if (intRowNextVisible >= 0)
        {
          if ((this.arrBorders[intRowNextVisible, _intCol] & BorderType.Bottom) == BorderType.Bottom)
          {
            //The current cell has a top border !
            borderType = borderType | BorderType.Top;
          }
        }
      }
      #endregion

      #region Bottom border
      //Check for Bottom border: might also be the top border of the cell below !
      //Don't search neighbour column if the current cell is INSIDE a cellrange!
      if (_intRow < (this.flexGrid.Rows.Count - 1) && _intRow == cellRangeMerged.BottomRow)
      {
        int intRowNextVisible = -1;
        //check next visible row  !
        //In merged range, we start with the bottom row of this range + 1!
        if (cellRangeMerged.IsValid)
        {
          intRowNextVisible = cellRangeMerged.BottomRow + 1;
        }
        else
        {
          intRowNextVisible = _intRow + 1;
        }
        while (intRowNextVisible < this.flexGrid.Rows.Count && this.flexGrid.Rows[intRowNextVisible].Visible == false)
        {
          ++intRowNextVisible;
        }

        if (intRowNextVisible < this.flexGrid.Rows.Count)
        {
          if ((this.arrBorders[intRowNextVisible, _intCol] & BorderType.Top) == BorderType.Top)
          {
            //The current cell has a bottom border !
            borderType = borderType | BorderType.Bottom;
          }
        }
      }
      #endregion

      #region Right border
      //Check for Right border: might also be the left border of the cell to the right !
      //Don't search neighbour column if the current cell is INSIDE a cellrange!
      if (_intCol < (this.flexGrid.Cols.Count - 1) && _intCol == cellRangeMerged.RightCol)
      {
        //check next visible col !
        int intColNextVisible = _intCol + 1;
        while (intColNextVisible < this.flexGrid.Cols.Count && this.flexGrid.Cols[intColNextVisible].Visible == false)
        {
          ++intColNextVisible;
        }

        if (intColNextVisible < this.flexGrid.Cols.Count)
        {
          if ((this.arrBorders[_intRow, intColNextVisible] & BorderType.Left) == BorderType.Left)
          {
            //The current cell has a right border !
            borderType = borderType | BorderType.Right;
          }
        }
      }
      #endregion

      #region Left border
      //Check for Left border: might also be the right border of the cell to the left !
      if (_intCol > 0 && _intCol == cellRangeMerged.LeftCol)
      {
        //check previous visible col !
        int intColNextVisible = _intCol - 1;
        while (intColNextVisible >= 0 && this.flexGrid.Cols[intColNextVisible].Visible == false)
        {
          --intColNextVisible;
        }
        //Not if inside the current merged range!
        //if (intColNextVisible < cellRangeMerged.LeftCol)
        {
          if (intColNextVisible >= 0)
          {
            if ((this.arrBorders[_intRow, intColNextVisible] & BorderType.Right) == BorderType.Right)
            {
              //The current cell has a left border !
              borderType = borderType | BorderType.Left;
            }
          }
        }
      }
      #endregion

      return borderType;
    }

    /// <summary>
    /// Reset the borders for the cell range.
    /// In Abhängigekeit ob auch die Rahmen innerhalb des Bereiches resettet werden sollen bleiben andere vorhandene Rahmen erhalten oder nicht.
    /// </summary>
    /// <param name="_intTopRow">Top row of cell range, must be valid</param>
    /// <param name="_intLeftCol">Left col of cell range, must be valid</param>
    /// <param name="_intBottomRow">Bottom row of cell range, must be valid</param>
    /// <param name="_intRightCol">Right col of cell range, must be valid</param>
    /// <param name="_bolRemoveInterierBorders">Sollen auch alle Rahmen innerhalb des Bereiches gesäubert werden? Default: false</param>
    /// <exception cref="System.ArgumentException">If top row is larger than bottom row, or left col is larger than right col</exception>
    public void ResetBorders(
      int _intTopRow,
      int _intLeftCol,
      int _intBottomRow,
      int _intRightCol,
      bool _bolRemoveInterierBorders = false)
    {
      //Check values:
      this.CheckRange(_intTopRow, _intLeftCol, _intBottomRow, _intRightCol);

      if (_bolRemoveInterierBorders)
      {
        for (int intRow = _intTopRow; intRow <= _intBottomRow; intRow++)
        {
          for (int intCol = _intLeftCol; intCol <= _intRightCol; intCol++)
          {
            this.arrBorders[intRow, intCol] = BorderType.None;
          }
        }
      }
      else
      {
        //Set the left border for all cells of the first col of the range:
        for (int intRow = _intTopRow; intRow <= _intBottomRow; intRow++)
        {
          //Remove a left border:
          this.arrBorders[intRow, _intLeftCol] = this.arrBorders[intRow, _intLeftCol] & ~BorderType.Left;
          //Remove a right border:
          this.arrBorders[intRow, _intRightCol] = this.arrBorders[intRow, _intRightCol] & ~BorderType.Right;
        }

        /* Aufruf in die Zeile darüber verschoben - AProhl 03.03.2020
        //Same for the right border:
        for (int intRow = _intTopRow; intRow <= _intBottomRow; intRow++)
        {
          //Remove a right border:
          this.arrBorders [intRow, _intRightCol] = this.arrBorders [intRow, _intRightCol] & ~HGFlexGridBorderPainterBorderType.Right;
        }
        */

        //Top row has a top border:
        for (int intCol = _intLeftCol; intCol <= _intRightCol; intCol++)
        {
          //Remove a top border:
          this.arrBorders[_intTopRow, intCol] = this.arrBorders[_intTopRow, intCol] & ~BorderType.Top;
          //Remove a bottom border:
          this.arrBorders[_intBottomRow, intCol] = this.arrBorders[_intBottomRow, intCol] & ~BorderType.Bottom;
        }

        /* Aufruf in die Zeile darüber verschoben - AProhl 03.03.2020
        //Bottom row has a bottom border:
        for (int intCol = _intLeftCol; intCol <= _intRightCol; intCol++)
        {
          //Remove a top border:
          this.arrBorders [_intBottomRow, intCol] = this.arrBorders [_intBottomRow, intCol] & ~HGFlexGridBorderPainterBorderType.Bottom;
        }
        */
      }
    }

    /// <summary>
    /// Update BorderPainter after moving columns
    /// </summary>
    /// <remarks>
    /// We cannot register to the <see cref="C1FlexGridBase.GridChanged"/> event for <see cref="GridChangedTypeEnum.ColMoved"/>, because this seems to be fired
    /// only by user drag/drop, but not by programmatic calls to <see cref="ColumnCollection.MoveRange"/> 
    /// </remarks>
    /// <param name="_intStartCol">First column to move</param>
    /// <param name="_intColCount">Number of columns to move</param>
    /// <param name="_intNewIndex">New column index. No change if identical to <paramref name="_intStartCol"/></param>
    public void MoveColumns(int _intStartCol, int _intColCount, int _intNewIndex)
    {
      //Update the borders array: for each row, swap the cols, 

      if (_intStartCol < _intNewIndex)
      {
        //Moved columns to the right:
        for (int intRow = 0; intRow < this.flexGrid.Rows.Count; intRow++)
        {
          //For each column in the moved index:
          //We have to move backwards. Needed if moving 2 or more columns.
          //E.g. move column 2+3 one to the right, so that the group starts at new index "3" or previous col 4 is now 2!
          //So we first move column 4 to position 3, than column 3 (originally 4) to column 2!
          //for (int intIndexColInRange = 0; intIndexColInRange < _intColCount; intIndexColInRange++)
          for (int intIndexColInRange = _intColCount - 1; intIndexColInRange >= 0; intIndexColInRange--)
          {
            //Get data of first col:
            BorderType borderTypeCol = this.arrBorders[intRow, _intStartCol + intIndexColInRange];

            //Move all following borders one to the left:
            for (int intCol = _intStartCol + intIndexColInRange; intCol < _intNewIndex + intIndexColInRange; intCol++)
            {
              this.arrBorders[intRow, intCol] = this.arrBorders[intRow, intCol + 1];
            }

            //And now set original border to the new column:
            this.arrBorders[intRow, _intNewIndex + intIndexColInRange] = borderTypeCol;
          } //Ende for over column range
        }
      }
      else if (_intStartCol > _intNewIndex)
      {
        //Moved columns to the left:
        for (int intRow = 0; intRow < this.flexGrid.Rows.Count; intRow++)
        {
          //For each column in the moved index:
          //Do it forwards here!
          for (int intIndexColInRange = 0; intIndexColInRange < _intColCount; intIndexColInRange++)
          {
            //Get data of first col:
            BorderType borderTypeCol = this.arrBorders[intRow, _intStartCol + intIndexColInRange];

            //Move all following borders one to the right:
            for (int intCol = _intStartCol + intIndexColInRange; intCol > _intNewIndex + intIndexColInRange; intCol--)
            {
              this.arrBorders[intRow, intCol] = this.arrBorders[intRow, intCol - 1];
            }

            //And now set original border to the new column:
            this.arrBorders[intRow, _intNewIndex + intIndexColInRange] = borderTypeCol;
          }
        } //Ende for over column range
      }
    }


    /// <summary>
    /// Update BorderPainter after grid changes: when the user e.g. resizes a column, then clear the information about start positions and widths
    /// of the rows and cols.
    /// </summary>
    /// <param name="sender">Absender des Events</param>
    /// <param name="e">Argumente des Events</param>
    private void flexGrid_GridChanged(object sender, GridChangedEventArgs e)
    {
      //Console.WriteLine("GridChanged: " + e.GridChangedType + " " + e.c1 + "/" + e.r1 + " - " + e.c2 + "/" + e.r2);
      if (e.GridChangedType == GridChangedTypeEnum.GridChanged ||
        e.GridChangedType == GridChangedTypeEnum.ColMoved ||
        e.GridChangedType == GridChangedTypeEnum.RowMoved)
      {
        //Reset the start positions and widths.
        this.listColStartWidth = null;
        this.listRowStartHeight = null;
      }
      /*
      //This code could handle a column drag/drop by the user, but it cannot handle programmatic flexGrid.Cols.MoveRange(...) [if moving multiple cols], so it is not used.
      if (e.GridChangedType == GridChangedTypeEnum.ColMoved)
      {
        Console.WriteLine (this.pen.Color.ToString() + " = " + this.pen.Width + " = " + this.pen.DashStyle + " => Col moved:" + e.c1 + " => " + e.c2);

        //Update the borders array: for each row, swap the cols, 
        if (e.c1 < e.c2)
        {
          //Moved column 1 to the right:

          for (int intRow = 0; intRow < this.flexGrid.Rows.Count; intRow++)
          {
            //Get data of first col:
            HGFlexGridBorderPainterBorderType borderTypeCol = this.arrBorders[intRow, e.c1];

            //Move all following borders one to the left:
            for (int intCol = e.c1; intCol < e.c2; intCol++)
            {
              this.arrBorders[intRow, intCol] = this.arrBorders[intRow, intCol + 1];
            }

            //And now set original border to the new column:
            this.arrBorders[intRow, e.c2] = borderTypeCol;
          }
        }
        else if (e.c1 > e.c2)
        {
          //Moved column 1 to the left:
          //"c1" is the original column index, "c2" the target index.
          for (int intRow = 0; intRow < this.flexGrid.Rows.Count; intRow++)
          {
            //Get data of first col:
            HGFlexGridBorderPainterBorderType borderTypeCol = this.arrBorders[intRow, e.c1];

            //Move all following borders one to the right:
            for (int intCol = e.c1; intCol > e.c2; intCol--)
            {
              this.arrBorders[intRow, intCol] = this.arrBorders[intRow, intCol - 1];
            }

            //And now set original border to the new column:
            this.arrBorders[intRow, e.c2] = borderTypeCol;
          }

        }
      }*/
    }

    /// <summary>
    /// Draws borders for the specified cell.
    /// Call this method in the "OwnerDrawCell" event of your grid.
    /// </summary>
    /// <param name="_e">The OwnerDrawCellEventArgs contain information about the cell to be painted.</param>
    public void DrawBorders(OwnerDrawCellEventArgs _e)
    {
      //Draw all four borders.
      this.DrawBorders(_e, BorderType.Bottom | BorderType.Left | BorderType.Right | BorderType.Top);
    }

    /// <summary>
    /// Draws borders for the specified cell, and specifiy which borders !
    /// Call this method in the "OwnerDrawCell" event of your grid.
    /// </summary>
    /// <param name="_e">The OwnerDrawCellEventArgs contain information about the cell to be painted.</param>
    /// <param name="_borderType">Draw only those borders !</param>
    public void DrawBorders(OwnerDrawCellEventArgs _e, BorderType _borderType)
    {
      //Drawing the borders can be switched off:
      if (this.CanDrawBorders == false)
      {
        return;
      }
      //If no borders are specified, do nothing.
      if (this.arrBorders == null)
      {
        return;
      }

      //Do nothing if bounds are "0/0" !
      if (_e.Bounds.X == 0 && _e.Bounds.Y == 0 && _e.Bounds.Width == 0 && _e.Bounds.Height == 0)
      {
        return;
      }

      //WKnauf 17.06.2013: now we draw the borders for EACH cell in the merged range. 
      //There is no chance to find the start position of cells INSIDE a merged range => so count the
      //row/column positions if not done already.
      if (this.listColStartWidth == null)
      {
        //Startzellen ermitteln:
        this.listRowStartHeight = new List<KeyValuePair<int, int>>();
        this.listColStartWidth = new List<KeyValuePair<int, int>>();

        int intColPos = 0;
        for (int intCol = 0; intCol < this.flexGrid.Cols.Count; intCol++)
        {
          int intWidthDisplay = this.flexGrid.Cols[intCol].WidthDisplay;
          this.listColStartWidth.Add(new KeyValuePair<int, int>(intColPos, intWidthDisplay));
          intColPos += intWidthDisplay;
        }

        int intRowPos = 0;
        for (int intRow = 0; intRow < this.flexGrid.Rows.Count; intRow++)
        {
          int intHeightDisplay = this.flexGrid.Rows[intRow].HeightDisplay;
          this.listRowStartHeight.Add(new KeyValuePair<int, int>(intRowPos, intHeightDisplay));
          intRowPos += intHeightDisplay;
        }
      }

      //In a merged range, draw EACH border separately. Handle also unmerged cells als "merged", which is a single cell range.
      CellRange rangeMerged = this.flexGrid.GetMergedRange(_e.Row, _e.Col);
      for (int intRowInRange = rangeMerged.TopRow; intRowInRange <= rangeMerged.BottomRow; intRowInRange++)
      {
        for (int intColInRange = rangeMerged.LeftCol; intColInRange <= rangeMerged.RightCol; intColInRange++)
        {
          //Startzelle errechnen!

          int intColInRangePos = this.listColStartWidth[intColInRange].Key;
          //Scrollposition abziehen:
          //Not if col is in fixed area!
          if (intColInRange >= this.flexGrid.Cols.Fixed)
          {
            intColInRangePos += this.flexGrid.ScrollPosition.X;
          }
          int intRowInRangePos = this.listRowStartHeight[intRowInRange].Key;
          //Scrollposition abziehen (negativ bei Rechtsscrollung):
          //Not if row is in fixed area!
          if (intRowInRange >= this.flexGrid.Rows.Fixed)
          {
            intRowInRangePos += this.flexGrid.ScrollPosition.Y;
          }
          int intColInRangeWidth = this.listColStartWidth[intColInRange].Value;
          int intRowInRangeHeight = this.listRowStartHeight[intRowInRange].Value;

          //Shouldn't we check the scroll position INSIDE the cell?

          //First the easy cases: bottom and right borders. The are drawn in the current cell.

          //Calculate left edge and width of the cell.
          int intX = intColInRangePos;
          int intWidth = intColInRangeWidth;
          //If "ExtendLastCol" is true and the current col is the last visible col, then the display area width might be wider than the column width property.
          if (this.flexGrid.ExtendLastCol == true)
          {
            //Last visible col?
            int intLastVisibleCol = this.flexGrid.Cols.Count - 1;
            while (this.flexGrid.Cols[intLastVisibleCol].Visible == false)
            {
              --intLastVisibleCol;
            }

            if (intColInRange == intLastVisibleCol)
            {
              //Take the client size of grid and subtract the start position of the column:
              int intWidthRemaining = this.flexGrid.ClientSize.Width - intX;
              //If this is larger than the current width, then the column is displayed wider than the actual width.
              if (intWidthRemaining > intWidth)
              {
                intWidth = intWidthRemaining;
              }
            }
          }

          //If this is not the first column of the grid, then draw the border on the grid line.
          //For the first column, reduce the width by 1, because in the first column the border has to be pointed on the left edge of the cell.
          if (_e.Col > 0)
          {
            --intX;
          }
          else
          {
            --intWidth;
          }

          //Y-Wert und Breite berechnen:
          //int intY = _e.Bounds.Y;
          int intY = intRowInRangePos;
          //int intHeight = _e.Bounds.Height;
          int intHeight = intRowInRangeHeight;
          //Wenn es sicht nicht um die erste Zeile handelt, den y-Wert nochmal
          //um 1 verringern.
          if (_e.Row > 0)
          {
            --intY;
          }
          else
            --intHeight;

          //Use "GetBorders" to check for the borders of the current cell (will also care for neighbour cell borders !)
          //HGFlexGridBorderPainterBorderType borderTypeCell = this.GetBorders(_e.Row, _e.Col);
          BorderType borderTypeCell = this.GetBorders(intRowInRange, intColInRange);

          //Now check for the four border types:
          #region Bottom border
          if ((_borderType & BorderType.Bottom) == BorderType.Bottom &&
            (borderTypeCell & BorderType.Bottom) == BorderType.Bottom)
          {
            _e.Graphics.DrawLine(this.pen, intX, intY + intHeight, intX + intWidth, intY + intHeight);
          }
          #endregion

          #region Right border
          if ((_borderType & BorderType.Right) == BorderType.Right &&
            (borderTypeCell & BorderType.Right) == BorderType.Right)
          {
            _e.Graphics.DrawLine(this.pen, intX + intWidth, intY, intX + intWidth, intY + intHeight);
          }
          #endregion

          #region Top border
          if ((_borderType & BorderType.Top) == BorderType.Top &&
            (borderTypeCell & BorderType.Top) == BorderType.Top)
          {
            _e.Graphics.DrawLine(this.pen, intX, intY, intX + intWidth, intY);
          }
          #endregion

          #region Left border
          if ((_borderType & BorderType.Left) == BorderType.Left &&
            (borderTypeCell & BorderType.Left) == BorderType.Left)
          {
            _e.Graphics.DrawLine(this.pen, intX, intY, intX, intY + intHeight);
          }
          #endregion
        }
      }
    }
    #endregion

    #region Properties
    /// <summary>
    /// Abrufen/Setzen des aktuellen Pen.
    /// </summary>
    public Pen Pen
    {
      get
      {
        return this.pen;
      }
      set
      {
        if (value == null)
        {
          throw new ArgumentNullException();
        }
        this.pen = value;
      }
    }

    /// <summary>
    /// Does this border painter allow drawing of borders INSIDE a merged range (TRUE)?
    /// If false, then those borders are ignored.
    /// </summary>
    public bool BorderInMergedRange
    {
      get
      {
        return this.bolBorderInMergedRange;
      }
      set
      {
        this.bolBorderInMergedRange = value;
      }
    }

    /// <summary>
    /// TRUE: draw borders in call to <see cref="DrawBorders(OwnerDrawCellEventArgs)"/>. FALSE: switch off drawing, e.g. during refill of the grid
    /// </summary>
    public bool CanDrawBorders
    {
      get;
      set;
    } = true;

    #endregion

    #region Private Methods
    /// <summary>
    /// Checks that the row/col parameters are a valid cell range.
    /// Throws ArgumentException if an error occurs.
    /// </summary>
    /// <param name="_intTopRow">Top row of a cell range, if invalid an exception occurs</param>
    /// <param name="_intLeftCol">Left col of cell range, if invalid an exception occurs</param>
    /// <param name="_intBottomRow">Bottom row of cell range, if invalid an exception occurs</param>
    /// <param name="_intRightCol">Right col of cell range, if invalid an exception occurs</param>
    /// <exception cref="System.ArgumentException">If top row is larger than bottom row, or left col is larger than right col</exception>
    private void CheckRange(int _intTopRow, int _intLeftCol, int _intBottomRow, int _intRightCol)
    {
      //Check values and raise exception ?
      if (_intTopRow > _intBottomRow)
        throw new ArgumentException("TopRow (" + _intTopRow + ") must be smaller than BottomRow (" + _intBottomRow + ")");
      if (_intLeftCol > _intRightCol)
        throw new ArgumentException("LeftCol (" + _intLeftCol + ") must be smaller than RightCol (" + _intRightCol + ")");

      //WKnauf 26.11.2020: auch die Gridborders berücksichtigen.
      if (_intTopRow < 0)
      {
        throw new ArgumentException("TopRow is negative: " + _intTopRow);
      }
      if (_intLeftCol < 0)
      {
        throw new ArgumentException("LeftCol is negative: " + _intTopRow);
      }
      //Rechts/unten: auf Arraylength prüfen. Hier könnte ich auch auf das Grid prüfen.
      if (_intBottomRow >= this.arrBorders.GetLength(0))
      {
        throw new ArgumentException("BottomRow (" + _intBottomRow + ") does not match length of internal borders array: " + this.arrBorders.GetLength(0));
      }
      if (_intRightCol >= this.arrBorders.GetLength(1))
      {
        throw new ArgumentException("RightCol (" + _intRightCol + ") does not match length of internal borders array: " + this.arrBorders.GetLength(1));
      }
    }
    #endregion
  }
}
