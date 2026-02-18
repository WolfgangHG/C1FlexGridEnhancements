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
* Supported cell style formatting: forecolor, backcolor, font (font family and font size are only applied if the are not the C1FlexGrid Defaults)
* merged ranges are supported
* borders are supported - but just basic support, e.g. no width and no border styles.  
The sample contains code to also support the [C1FlexGridBorderPainter](../C1FlexGridBorderPainter), but this code is commented.
* column widths - but see below for limitations
* all cell data is copied as string (cell format is set to "@" - relevant for excel paste)


Limitations:
* when pasting to Excel, the column widths are ignored. I did not find a way to apply them... Outlook and Word work.

## How it works
Each C1FlexGrid CellStyle is converted to a css style, where the "DefinedElements" enum defines which attributs of the
css style are set. The cells uses this style.

E.g. the style "Fixed" in my sample results in this css style:
~~~~
.stFixed
 {color:#000000;background:#F0F0F0;border-bottom: solid #000000 0.5pt;border-top: solid #000000 0.5pt;border-left: solid #000000 0.5pt;border-right: solid #000000 0.5pt;}
~~~~
The stylenames all have the prefix "st" in order to avoid "invalid" css style names that could result from my "C1FlexGridStyleHandler" sample).


## Clipboard format
The HTML clipboard format is described here: https://learn.microsoft.com/en-us/windows/win32/dataxchg/html-clipboard-format

And an older version of the link with different content: https://web.archive.org/web/20170109133016/https://msdn.microsoft.com/en-us/library/windows/desktop/ms649015(v=vs.85).aspx

Here is a sample:
~~~~
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