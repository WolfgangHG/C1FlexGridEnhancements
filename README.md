# C1FlexGrid enhancements

This repository contains some enhancements to ComponentOne C1FlexGrid (https://www.grapecity.com/componentone/winforms-ui-controls)
Each sample exists in two versions, one using the NuGet package "C1.Win.C1FlexGrid" for .NET 4.8, the other the package "C1.Win.FlexGrid" for .NET 8.

## C1FlexGridBorderPainter
The samples [C1FlexGridBorderPainter (.NET 4.8)](/C1FlexGrid48BorderPainter) and [C1FlexGridBorderPainter (.NET 8)](/C1FlexGrid6BorderPainter) contain a helper class that simplies the drawing of borders in C1FlexGrid.

## C1FlexGridStyleHandler
The samples [C1FlexGridStyleHandler (.NET 4.8)](/C1FlexGrid48StyleHandler) and [C1FlexGridStyleHandler (.NET 8)](/C1FlexGrid6StyleHandler) contain a helper class that simplies the formatting of cells in a C1FlexGrid 
by using styles

## Copy to Office
The samples [Copy to Office (.NET 4.8)](/C1FlexGrid48CopyOffice) and [Copy to Office (.NET 8)](/C1FlexGrid6CopyOffice) contain a helper class that shows how to write the C1FlexGrid 
content to the clipboard so that it can be pasted to Word/Excel/Outlook and the formatting is kept.  
The same code can also be used to export a C1FlexGrid to Html.

## Calendar sheet
The sample [Calendar sheet (.NET 8)](/C1FlexGrid6CalendarSheet) shows how to render a calendar table with month rows and day columns
and select date ranges, which requires a bar selection instead of the rectangle selection of C1FlexGrid.

## AutoSizeRows mode "Height to last"
The sample [AutoSizeRows: Height to last (.NET 8)](/C1FlexGrid6AutoSizeRowHeightToLast) shows an optimized `AutoSizeRows` mode for multi row merged
ranges, so that the first rows of the range are not higher than necessary.


## FlexGridToolTipRequester
The sample [FlexGridToolTipRequester (.NET 8)](/C1FlexGridToolTipRequester) shows to to display a custom tooltip for each cell of the grid.
