# C1FlexGrid enhancements: Copy to Office (.NET 4.8 and .NET 8)

This sample contains a helper class that shows how to write the content of a ComponentOne C1FlexGrid (https://www.grapecity.com/componentone/winforms-ui-controls) 
to the clipboard so that it can be pasted to Word/Excel/Outlook and the formatting is kept.  
The same code can also be used to export a C1FlexGrid to Html.

It supports C1FlexGrid for .NET framework 4.8 and for .NET 8.

## Features

* Three copy modes are supported:
    * copy selected cells  (row/column headers for the selected cells are also copied)
    * copy selected rows (column headers for the entire grid are also copied)
    * copy entire grid
* The entire grid can also be saved to html
* Supported cell style formatting: forecolor, backcolor, font (font family and font size are only applied if they are not the C1FlexGrid Defaults)
* merged ranges are supported
* borders are supported (including `BorderDirEnum.BothDifferent`) - but no border styles. The border width is also applied, but will behave differently when pasting to Excel or Word.
The sample contains code to also support the [C1FlexGridBorderPainter](../C1FlexGridBorderPainter), but this code is commented.
* column widths - but see below for limitations
* all cell data is copied as string (cell format is set to "@" - relevant for excel paste)


Limitations:
* when pasting to Excel, the column widths are ignored. I did not find a way to apply them... Outlook and Word work.
* a border width greater than "1" looks differently in Word and Excel, no matter what width I choose. Excel has only for fixed width types, and by converting the C1FlexGrid width "1 pixel" to "0.5pt", it results in excel border
type `xlThin` (a bit wider than `xlHairline`), the width "2 pixel" is written as "1pt", which results in `xlMedium`, and everything wider is written "as is" to the style, so it results in `xlThick`.
For Word, "1pt" looks too thin, so another point value might be more appropriate here.

  I found a post at https://learn.microsoft.com/en-us/answers/questions/4840246/printed-thickness-of-cell-borders-in-excel which states:
   
  *I printed out borders in Excel with each of the available widths:  xlHairline, xlThin, xlMedium, xlThick*
  
  *I printed out borders in Word with each of the available widths:  wdLineWidth025Pt, wdLineWidth050Pt, wdLineWidth075Pt, wdLineWidth100Pt, wdLineWidth150Pt, wdLineWidth225Pt, wdLineWidth300Pt, wdLineWidth450Pt, wdLineWidth600Pt
  The prints were done on the same printer.  Then I visually compared the thicknesses.*

  * *xlHairline appears to be the same as wdLineWidth025Pt*
  * *xlThin appears to be the same as wdLineWidth100Pt*
  * *xlMedium appears to be thicker than wdLineWidth150Pt but thinner than wdLineWidth225Pt*
  * *xlThick appears to be the same as wdLineWidth300Pt*

## How it works
Each C1FlexGrid CellStyle is converted to a css style, where the "DefinedElements" enum defines which attributs of the
css style are set. The cells uses this style.

E.g. the style "Fixed" in my sample results in this css style:
~~~~
.stFixed
 {color:#000000;background:#F0F0F0;border-bottom: solid #000000 0.5pt;border-top: solid #000000 0.5pt;border-left: solid #000000 0.5pt;border-right: solid #000000 0.5pt;}
~~~~
The stylenames all have the prefix "st" in order to avoid "invalid" (starting with a number) css style names that could result from my "C1FlexGridStyleHandler" sample).


## Clipboard format
The HTML clipboard format is described here: https://learn.microsoft.com/en-us/windows/win32/dataxchg/html-clipboard-format

Here is an older version of the link with different content: https://web.archive.org/web/20170109133016/https://msdn.microsoft.com/en-us/library/windows/desktop/ms649015(v=vs.85).aspx

Here is a sample:
~~~~html
Version:1.0
StartHTML:0000000107
EndHTML:0000001953
StartFragment:0000001579
EndFragment:0000001903

<html>
<head>
<style>
br
  {mso-data-placement:same-cell;}
td
 {mso-number-format:"\@";}

...all styles of the C1FlexGrid


</style>
</head>
<body>
<!--StartFragment-->
<table border=0 style="border-collapse: collapse;border:none">
<thead>
  ...header rows...
</thead>
...table rows...
<!--EndFragment-->
</table>

</body>
</html>

~~~~


The heading defines positions of the html start and length and of the body content.

In the html head, a list of styles is defined.

The body content is also marked with a comment with the content "StartFragment" / "EndFragment", as required by the specification.

## Hack: text color and Word
In 2026, pasting the fore color of cell content did not work for Word/Outlook, but for Excel.

By copying a table with texts with a fore color from Excel to the clipboard and analyzing the HTML string placed in the clipboard, I found that it works again when setting this 
`meta` tag:

~~~html
<html>
<head>
<meta name="ProgId" content="Excel.Sheet">
</head>
~~~

So my sample always sets this tag.
