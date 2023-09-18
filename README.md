# C1FlexGrid enhancements

This repository contains some enhancements to ComponentOne C1FlexGrid (https://www.grapecity.com/componentone/winforms-ui-controls)
Each sample exists in two versions, one using the NuGet package "C1.Win.C1FlexGrid" for .NET 4.5.2, the other the package "C1.Win.FlexGrid" for .NET 6.

## C1FlexGridBorderPainter
The samples [C1FlexGridBorderPainter (.NET 4.5.2)](/C1FlexGrid452BorderPainter) and [C1FlexGridBorderPainter (.NET 6)](/C1FlexGrid6BorderPainter) contain a helper class that simplies the drawing of borders in C1FlexGrid.

## C1FlexGridStyleHandler
The samples [C1FlexGridStyleHandler (.NET 4.5.2)](/C1FlexGrid452StyleHandler) and [C1FlexGridStyleHandler (.NET 6)](/C1FlexGrid6StyleHandler) contain a helper class that simplies the formatting of cells in a C1FlexGrid 
by using styles


## Copy to Office
The samples [Copy to Office (.NET 4.5.2)](/C1FlexGrid452CopyOffice) and [Copy to Office (.NET 6)](/C1FlexGrid6CopyOffice) contain a helper class that shows how to write the C1FlexGrid 
content to the clipboard so that it can be pasted to Word/Excel/Outlook and the formatting is kept.  
The same code can also be used to export a C1FlexGrid to Html.

## Calendar sheet
The sample [Calendar sheet (.NET 6)](/C1FlexGrid6CalendarSheet) shows how to render a calendar table with month rows and day columns
and select date ranges, which requires a bar selection instead of the rectangle selection of C1FlexGrid.
