# C1FlexGrid enhancements: optimized AutoSizeRows (.NET 4.8 and .NET 8)

This sample demonstrates an alternate "AutoSizeRows" mode for ComponentOne C1FlexGrid (https://www.grapecity.com/componentone/winforms-ui-controls)

It supports C1FlexGrid for .NET framework 4.8 and for .NET 8.


This sample applies to the situation that a C1FlexGrid contains long (and wrapped) texts in merged ranges, which merge over more than one row.

When calling "AutoSizeRows" for the rows of such a merged range, all rows receive an proportionate part of the merged range height. 
E.g. if the range is 150 pixel high and it spans three rows, all three rows will have a height of 50 pixel each.

This screenshots shows the result of `AutoSizeRows` for my sample grid, which contains a lot of merged ranges with wrapped text:
![AutoSizeRows](images/autosizerows.png)

But now assume that the other cells of the first and second row of this range contain only small, single line texts. So, it would be better if both rows had the 
default row height of e.g. 26 pixel (default row height at .NET6 default font) instead of the autosized height of 50 pixel. The remaining height of the merged range could be added to the last
row, leading to a height of 150 - (26+26) pixel = 98 pixel for the last row.

This would result in a more compressed grid, where the empty space is at the end of merged ranges.

This sample demonstrates an  improved "AutoSizeRows" mode, I call it "height to last". Here is the result:
![Height to last](images/height_to_last.png)
Row 4 and 5 are rather high in the original screenshot, while they have a much better height in the optimized sample.

## Hint
A note on merged ranges: at least `AutoSizeRows` seems to be a little picky: best add the cell data to all cells of the merged
range (relevant when using custom merging). Maybe it is even necessary to activate "WordWrap" for all cells.

## Implementation details
The code is based on calls to the method `C1FlexGrid.MeasureCellSize(row, col)`, which returns the measured size of a specific cells.
Note that this method does not return the height of the full merged range if the cell is part of a merged range,
but only a partial height of the row as part of the merged range (as `AutoSizeRows` would do).
So, to the get height of the merged range, call `MeasureCellSize` for all rows and sum the heights.