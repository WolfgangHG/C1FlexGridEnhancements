using C1.Win.C1FlexGrid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C1FlexGrid452CopyOffice
{
  /// <summary>
  /// This utility class copies C1FlexGrid content to the clipboard in HTML format so that it can be pasted to Office.
  /// </summary>
  public static class C1FlexGridCopyToOffice
  {
    #region Static Variables
    /// <summary>
    /// English Culture - for number output
    /// </summary>
    private static CultureInfo cultureInfoEnglish = new CultureInfo("en-US");
    #endregion

    #region Enums
    /// <summary>
    /// For the "Copy to clipboard for pasting in office" mode: different copy modes
    /// </summary>
    public enum CopyMode
    {
      /// <summary>
      /// Selected cells (plus header)
      /// </summary>
      SelectedCells,
      /// <summary>
      /// Selected rows (plus Header)
      /// </summary>
      SelectedRows,
      /// <summary>
      /// Full grid
      /// </summary>
      All
    }
    #endregion


    #region Publics
    /// <summary>
    /// Converts the selection in the grid (or the full grid) to html.
    /// 
    /// If a selection is copied, the row and column headers for the selected rows/cols are also copied.
    /// 
    /// In mode <see cref="CopyMode.SelectedRows"/> or <see cref="CopyMode.SelectedCells"/>, the method checks that the selection is valid and reports an error message otherwise.
    /// </summary>
    /// <param name="_flexGrid">This grid is copied</param>
    /// <param name="_copyMode">Defines whether the full grid or a selection is to be copied</param>
    /// <param name="_listBorderPainter">Optional: List of Border Painters, see my other sample .
    /// 
    /// The four borders of a cell can be defined in different BorderPainter.
    /// 
    /// If a border is defined in multiple BorderPainter, the BorderPainter with the highest index wins.</param>
    public static void Copy(C1FlexGrid _flexGrid, CopyMode _copyMode) //, List<HGFlexGridBorderPainter> _listBorderPainter = null)
    {
      if (_flexGrid == null)
      {
        throw new ArgumentNullException(nameof(_flexGrid));
      }
      //If copy mode is "selection", there should be a valid selection in the grid.
      if (_copyMode != CopyMode.All)
      {
        CellRange selection = _flexGrid.Selection;
        //It should be sufficient to check "selection.IsValid". But I also check that TopRow/LeftCol are in the non fixed area.
        if (selection.IsValid == false || selection.TopRow < _flexGrid.Rows.Fixed || selection.LeftCol < _flexGrid.Cols.Fixed)
        {
          throw new Exception("Nothing selected");
        }
      }

      string css;
      string htmlFlexGrid = ToHTML(_flexGrid, _copyMode, out css); //, _listBorderPainter);

      StringBuilder sbHTML = new StringBuilder();
      sbHTML.AppendLine("Version:1.0");
      //First add the clipboard tags. The locations are place holders and are replaced at the end
      sbHTML.AppendLine("StartHTML:0000000000");
      sbHTML.AppendLine("EndHTML:0000000000");
      sbHTML.AppendLine("StartFragment:0000000000");
      sbHTML.AppendLine("EndFragment:0000000000");
      sbHTML.AppendLine();

      sbHTML.AppendLine("<html>");
      sbHTML.AppendLine("<head>");

      #region Convert styles to CSS styles.
      sbHTML.AppendLine("<style>");

      //Newlines ("<br>"): keep data in excel in same cell, don't create newline!!!
      sbHTML.AppendLine("br");
      sbHTML.AppendLine("  {mso-data-placement:same-cell;}");

      //Format all cell data as string: redefine the "td" base style.
      sbHTML.AppendLine("td");
      sbHTML.AppendLine(" {mso-number-format:\"\\@\";}");  //This means: mso-number-format:"\@";
      sbHTML.AppendLine(css);
      sbHTML.AppendLine("</style>");
      #endregion

      sbHTML.AppendLine("</head>");

      sbHTML.AppendLine("<body>");
      sbHTML.AppendLine("<!--StartFragment-->");
      sbHTML.AppendLine(htmlFlexGrid);
      sbHTML.AppendLine("</body>");
      sbHTML.AppendLine("</html>");

      //Set the real html position/length to the "StartHTML"/"EndHTML" placeholders:
      string strHTML = sbHTML.ToString();
      int indexHTML = strHTML.IndexOf("<html>");
      strHTML = strHTML.Replace("StartHTML:0000000000", "StartHTML:" + indexHTML.ToString("0000000000"));
      strHTML = strHTML.Replace("EndHTML:0000000000", "EndHTML:" + strHTML.Length.ToString("0000000000"));

      int indexStartFragment = strHTML.IndexOf("<!--StartFragment-->");
      //Also set the real html body position/length to the "StartFragment"/"EndFragment" placeholders:
      indexStartFragment += "<!--StartFragment-->".Length;
      strHTML = strHTML.Replace("StartFragment:0000000000", "StartFragment:" + indexStartFragment.ToString("0000000000"));
      //"EndFragment" contains the position of the end tag:
      int indexEndFragment = strHTML.IndexOf("<!--EndFragment-->");
      strHTML = strHTML.Replace("EndFragment:0000000000", "EndFragment:" + indexEndFragment.ToString("0000000000"));

      Clipboard.SetData(DataFormats.Html, strHTML);
    }

    /// <summary>
    /// Converts the selection in the grid (or the full grid) to html.
    /// 
    /// If a selection is copied, the row and column headers for the selected rows/cols are also copied.
    /// 
    /// </summary>
    /// <param name="_flexGrid">This grid is copied</param>
    /// <param name="_copyMode">The range to be copied (full grid or selection)</param>
    /// <param name="_listBorderPainter">Optional: List of Border Painters, see my other sample .
    /// 
    /// The four borders of a cell can be defined in different BorderPainter.
    /// 
    /// If a border is defined in multiple BorderPainter, the BorderPainter with the highest index wins.</param>
    public static string ToHTML(C1FlexGrid _flexGrid, CopyMode _copyMode, out string _css) //List<HGFlexGridBorderPainter> _listBorderPainter = null)
    {
      if (_flexGrid == null)
      {
        throw new ArgumentNullException(nameof(_flexGrid));
      }

      var sbStyle = new StringBuilder();

      #region Convert styles to CSS styles.

      //export styles:
      for (int indexStyle = 0; indexStyle < _flexGrid.Styles.Count; indexStyle++)
      {
        #region Export style
        CellStyle style = _flexGrid.Styles[indexStyle];

        string strStyleData = string.Empty;
        if ((style.DefinedElements & StyleElementFlags.ForeColor) == StyleElementFlags.ForeColor)
        {
          //Create color object, so that no "KnownColor" value is used by "ColorTranslator"
          strStyleData += "color:" + ColorTranslator.ToHtml(Color.FromArgb(style.ForeColor.ToArgb())) + ";";
        }
        if ((style.DefinedElements & StyleElementFlags.BackColor) == StyleElementFlags.BackColor)
        {
          //Create color object, so that no "KnownColor" value is used by "ColorTranslator"
          strStyleData += "background:" + ColorTranslator.ToHtml(Color.FromArgb(style.BackColor.ToArgb())) + ";";
        }
        if ((style.DefinedElements & StyleElementFlags.Font) == StyleElementFlags.Font)
        {
          //Font: apply only non-standard values für font style/size/fontname.
          //See https://www.html-seminar.de/css-schriften.htm
          Font fontStyle = style.Font;
          if (fontStyle.Name != _flexGrid.Font.Name)
          {
            //Apply font name only if it not the grid default.
            //Place it in quotes (because of spaces in name)
            strStyleData += "font-family:\"" + fontStyle.Name + "\";";
          }
          if (fontStyle.Bold == true)
          {
            strStyleData += "font-weight:bold;";
          }
          if (fontStyle.Italic == true)
          {
            strStyleData += "font-style:italic;";
          }
          if (fontStyle.Strikeout == true)
          {
            strStyleData += "text-decoration:line-through;";
          }
          if (fontStyle.Size != _flexGrid.Font.Size)
          {
            //Size: apply only if it is non-default.
            //ENGLISH number format!
            strStyleData += "font-size:" + fontStyle.SizeInPoints.ToString(cultureInfoEnglish) + "pt;";
          }
        }
        if ((style.DefinedElements & StyleElementFlags.Border) == StyleElementFlags.Border &&
          style.Border.Style != BorderStyleEnum.None)
        {
          //Does that style have a border? This is the case if the DefinedElements are set, AND the style is not "None".

          //Also create top/left border definition - compatible to previous version...
          strStyleData += GetBorderStyleCSS(style, true, true, true, true);
        }
        //TextAlign??? Bisher noch nicht benötigt.

        //Nur exportieren wenn der Style etwas definiert.
        if (strStyleData.Length > 0)
        {
          string styleName = NormalizeStyleName(style.Name);
          sbStyle.AppendLine("." + styleName);
          sbStyle.AppendLine(" {" + strStyleData + "}");
        }
        #endregion
      }

      #endregion

      var sbHTML = new StringBuilder();
      //Ohne Border kopieren. TODO...
      if (_flexGrid.Styles.Normal.Border.Style != BorderStyleEnum.None)
      {
        //sbHTML.AppendLine("<table border style=\"border-top:1.5pt solid windowtext;border-right:1.5pt solid windowtext;border-bottom:1.5pt solid windowtext;border-left:1.5pt solid windowtext\">");
        //"border-collapse: collapse" => führt dazu, dass KEINE doppelten Borders entstehen!
        //https://www.w3schools.com/Css/css_table.asp
        sbHTML.AppendLine("<table border style=\"border-collapse: collapse;border:none\">");
      }
      else
      {
        //Border abschalten:
        //Auch wenn sie aus ist muss "border-collapse: collapse" eingeschaltet werden, da ohne die Zellenborders sonst doppelt werden.
        sbHTML.AppendLine("<table border=0 style=\"border-collapse: collapse;border:none\">");
        //sbHTML.AppendLine("<table border=0 style=mso-border-alt:solid windowtext .5pt;>");
        //Das hier führt eine dicke Doppelborder ein: "style=mso-border-alt:solid windowtext .5pt;"
      }
      int intRowFrom, intColFrom, intRowTo, intColTo;

      //WKnauf 20.08.2021: nach CopyMode unterscheiden:
      switch (_copyMode)
      {
        case CopyMode.All:
          {
            //Gesamtes Grid:
            intRowFrom = _flexGrid.Rows.Fixed;
            intColFrom = _flexGrid.Cols.Fixed;
            intRowTo = _flexGrid.Rows.Count - 1;
            intColTo = _flexGrid.Cols.Count - 1;
            break;
          }
        case CopyMode.SelectedRows:
          {
            CellRange selection = _flexGrid.Selection;
            if (selection.IsValid == false || selection.TopRow < _flexGrid.Rows.Fixed)
            {
              //Fallback bei ungültiger Auswahl: eigentlich nicht mehr nötig - war vorher nur in der Dispoansicht-HTML-Konvertierung
              //benötigt.
              intRowFrom = _flexGrid.Rows.Fixed;
              intColFrom = _flexGrid.Cols.Fixed;
              intRowTo = _flexGrid.Rows.Count - 1;
              intColTo = _flexGrid.Cols.Count - 1;
            }
            else
            {
              //Gültige Selection: die Spalten gehen über die gesamte Breite.
              intRowFrom = selection.TopRow;
              intColFrom = _flexGrid.Cols.Fixed;
              intRowTo = selection.BottomRow;
              intColTo = _flexGrid.Cols.Count - 1;
            }
            break;
          }
        case CopyMode.SelectedCells:
          {
            CellRange selection = _flexGrid.Selection;
            if (selection.IsValid == false || selection.TopRow < _flexGrid.Rows.Fixed || selection.LeftCol < _flexGrid.Cols.Fixed)
            {
              //Fallback bei ungültiger Auswahl: eigentlich nicht mehr nötig - war vorher nur in der Dispoansicht-HTML-Konvertierung
              //benötigt.
              intRowFrom = _flexGrid.Rows.Fixed;
              intColFrom = _flexGrid.Cols.Fixed;
              intRowTo = _flexGrid.Rows.Count - 1;
              intColTo = _flexGrid.Cols.Count - 1;
            }
            else
            {
              intRowFrom = selection.TopRow;
              intColFrom = selection.LeftCol;
              intRowTo = selection.BottomRow;
              intColTo = selection.RightCol;
            }
            break;
          }
        default:
          throw new InvalidOperationException("Unbekannter CopyMode: " + _copyMode);
      }


      //Testen: Excel => wie kriege ich das zur Breitenübernahme?

      /*Bringt nix....
      //Angeblich: https://www.experts-exchange.com/questions/24305882/Setting-Excel-Column-Width-in-HTML.html => funzt net
      //Zuallererst die Cols:
      //Der Einfachheit halber: über alle laufen von vorne bis zum Endspalte.
      for (int intCol = 0; intCol <= intColTo; intCol++)
      {
      //Die zwischen Fixed und gewähltem Bereich werden rausgelassen.
        if (intCol < _flexGrid.Cols.Fixed || intCol >= intColFrom)
        {
          int intColWidth = _flexGrid.Cols[intCol].WidthDisplay;
          sbHTML.AppendLine("<col width=" + intColWidth);
          // //"mso-width-source:userset" aus Excel-Copy heraus geholt.
          //sbHTML.AppendLine("<col width=" + intColWidth + " style='width:" + intColWidth + "pt;mso-width-source:userset;mso-width-alt:8301;'>");
        }
      }*/

      //Row heights are no problem - at least autosized rows are handled by office if no row heigth is specified.

      //Erstmal die Fixed:
      if (_flexGrid.Rows.Fixed > 0)
      {
        sbHTML.AppendLine("<thead>");
        for (int intRow = 0; intRow < _flexGrid.Rows.Fixed; intRow++)
        {
          sbHTML.AppendLine("<tr height=" + _flexGrid.Rows[intRow].HeightDisplay + ">");
          //Copy fixed cols always:
          if (_flexGrid.Cols.Fixed > 0)
          {
            CopyCells(_flexGrid, intRow, 0, 0, _flexGrid.Cols.Fixed - 1, _flexGrid.Rows.Fixed - 1, /*_listBorderPainter,*/ sbHTML, "th");
          }

          //Datenzellen: nur im kopierten Bereich:
          CopyCells(_flexGrid, intRow, intColFrom, 0, intColTo, _flexGrid.Rows.Fixed - 1, /*_listBorderPainter,*/ sbHTML, "th");
          sbHTML.AppendLine("</tr>");
        }
        sbHTML.AppendLine("</thead>");
      }
      for (int intRow = intRowFrom; intRow <= intRowTo; intRow++)
      {
        sbHTML.AppendLine("<tr height=" + _flexGrid.Rows[intRow].HeightDisplay + ">");
        //Copy fixed cols always:

        //Die fixed werden immer mitkopiert:
        if (_flexGrid.Cols.Fixed > 0)
        {
          CopyCells(_flexGrid, intRow, 0, intRowFrom, _flexGrid.Cols.Fixed - 1, intRowTo, /*_listBorderPainter,*/ sbHTML);
        }

        CopyCells(_flexGrid, intRow, intColFrom, intRowFrom, intColTo, intRowTo, /*_listBorderPainter,*/ sbHTML);
        sbHTML.AppendLine("</tr>");
      }

      sbHTML.AppendLine("<!--EndFragment-->");

      sbHTML.AppendLine("</table>");

      //WKnauf 20.08.2021: Mantis 19034: das ist nicht mehr nötig.
      // //Auswahl wieder setzen:
      //_flexGrid.Select(intRowFrom, intColFrom, intRowTo, intColTo);
      //_flexGrid.EndUpdate();

      _css = sbStyle.ToString();
      return sbHTML.ToString();
    }

    #endregion

    #region Helpers
    /// <summary>
    /// Normalize style name: replace all special characters which might cause trouble in CSS style names?
    /// </summary>
    /// <param name="_styleName"></param>
    /// <returns></returns>
    private static string NormalizeStyleName(string _styleName)
    {
      _styleName = _styleName.Replace(" ", "_");
      //Some special chars which might be generated by our own code.
      _styleName = _styleName.Replace("/", "_");
      _styleName = _styleName.Replace(",", "_");
      _styleName = _styleName.Replace("[", "_");
      _styleName = _styleName.Replace("]", "_");
      return "st" + _styleName;
    }

    /// <summary>
    /// Copies a range of cells on the current row.
    /// Merged ranges are applied.
    /// </summary>
    /// <param name="_c1FlexGrid"></param>
    /// <param name="_intRowAktuell">Current row</param>
    /// <param name="_intColFrom">Start col of copied range</param>
    /// <param name="_intRowFrom">Start row of export range (for handling merged ranges: only the part of the merged range which is contained in the row range is copied)</param>
    /// <param name="_intColTo">End col of copied range</param>
    /// <param name="_intRowTo">end row of export range(für Merged Ranges)</param>
    /// <param name="_listBorderPainter">Liste von Border Painter, aus denen Rahmeninformationen gezogen werden können.
    /// Darf NULL sein</param>
    /// <param name="_sbHTML">Generated HTML is appended.</param>
    /// <param name="_tdElement">td ist der default fuer Datenzellen. Fuer die Titelspalten sollte th verwendet werden.</param>
    private static void CopyCells(C1FlexGrid _c1FlexGrid, int _intRowAktuell, int _intColFrom, int _intRowFrom, int _intColTo, int _intRowTo,
      //List<HGFlexGridBorderPainter> _listBorderPainter, 
      StringBuilder _sbHTML, string _tdElement = "td")
    {

      for (int intCol = _intColFrom; intCol <= _intColTo; intCol++)
      {
        //Only visible cols:
        if (_c1FlexGrid.Cols[intCol].Visible == true)
        {
          //check merged range: if the cell is part of a merged range, export it only in special situtations:
          CellRange rangeMerged = _c1FlexGrid.GetMergedRange(_intRowAktuell, intCol);
          bool bolCanExport = false;

          if (rangeMerged.IsSingleCell == true)
          {
            //not merged: export!
            bolCanExport = true;
          }
          else
          {
            //merged cell: check overlapping with selection.
            if (rangeMerged.TopRow == _intRowAktuell && rangeMerged.LeftCol == intCol)
            {
              //merged range starts in current cell: export it.
              bolCanExport = true;
            }
            else if (rangeMerged.TopRow < _intRowFrom && _intRowAktuell == _intRowFrom && rangeMerged.LeftCol == intCol)
            {
              //Or: MergedRange start row is BEFORE the copied area, and the current row is the first row inside the copied area.
              //And we are the start col of the range.
              bolCanExport = true;
            }
            else if (rangeMerged.LeftCol < _intColFrom && intCol == _intColFrom && rangeMerged.TopRow == _intRowAktuell)
            {
              //Or: MergedRange start col is BEFORE the copied area, and the current col is the first col insided the copied area.
              //And we are the the row of the range.
              bolCanExport = true;
            }
            else if (rangeMerged.TopRow < _intRowFrom && _intRowAktuell == _intRowFrom &&
              rangeMerged.LeftCol < _intColFrom && intCol == _intColFrom)
            {
              //Another special case: MergedRange start row is ABOVE the copied area AND the start col is BEFORE the copied area,
              //but this is the first row/col inside the copied area (top left cell of the area).
              bolCanExport = true;
            }

          }

          if (bolCanExport == true)
          {
            #region Copy cell
            //Calculate the sub-area of the merged range which is part of the copied range:
            Rectangle rectMerged = new Rectangle(rangeMerged.LeftCol, rangeMerged.TopRow, (rangeMerged.RightCol - rangeMerged.LeftCol + 1), (rangeMerged.BottomRow - rangeMerged.TopRow + 1));
            Rectangle rectCopy = new Rectangle(_intColFrom, _intRowFrom, (_intColTo - _intColFrom + 1), (_intRowTo - _intRowFrom + 1));
            //get intersection of merged range and copied range:
            Rectangle rectIntersect = Rectangle.Intersect(rectMerged, rectCopy);

            //How large is the MergedRange (but we use only the part that is in the copied range!)
            int intRowSpan = rectIntersect.Height;
            int intColSpan = rectIntersect.Width;


            _sbHTML.Append("  <" + _tdElement);

            //Set col width.
            //Excel ignores it, but with Outlook/Word it works
            //In a MergedRange we need the sum of all widths
            int intColWidth;
            if (rangeMerged.IsSingleCell == true)
            {
              intColWidth = _c1FlexGrid.Cols[intCol].WidthDisplay;
            }
            else
            {
              intColWidth = 0;
              //"<= rectIntersect.Right" liefert einen um 1 zu hohen Wert???
              for (int intColInRange = rectIntersect.Left; intColInRange < (rectIntersect.Left + rectIntersect.Width); intColInRange++)
              {
                intColWidth += _c1FlexGrid.Cols[intColInRange].WidthDisplay;
              }
            }
            bool todo_Testen;
            //Aus Excel heraus kopieren: da kommt noch ein Style-Attribut mit. Das lassen wir hier weg.
            //_sbHTML.Append(" width=" + intColWidth + " style ='width:" + intColWidth + "pt;'");
            _sbHTML.Append(" width=" + intColWidth);

            //Merged range? Export only if > 1:
            if (intRowSpan > 1)
            {
              _sbHTML.Append(" rowspan=" + intRowSpan);
            }
            if (intColSpan > 1)
            {
              _sbHTML.Append(" colspan=" + intColSpan);
            }
            //Is a CellStyle defined? If yes set the style class attribute:
            CellStyle styleCell = _c1FlexGrid.GetCellStyle(_intRowAktuell, intCol);
            if (styleCell != null)
            {
              _sbHTML.Append(" class=\"" + NormalizeStyleName(styleCell.Name) + "\"");
            }
            else
            {
              //If no CellStyle is defined and the current cell is a fixed cell, then set the "Fixed" Style.
              if (_intRowAktuell < _c1FlexGrid.Rows.Fixed || intCol < _c1FlexGrid.Cols.Fixed)
              {
                CellStyle styleFixed = _c1FlexGrid.Styles.Fixed;
                _sbHTML.Append(" class=\"" + NormalizeStyleName(styleFixed.Name) + "\"");
              }
            }


            #region Support for C1FlexGridBorderPainter
            /*Uncomment this if border painters are used.
            //Border Painter:
            if (_listBorderPainter != null)
            {
              #region Handle borders of border painters
              HGFlexGridBorderPainter borderPainterTop = null;
              HGFlexGridBorderPainter borderPainterRight = null;
              HGFlexGridBorderPainter borderPainterBottom = null;
              HGFlexGridBorderPainter borderPainterLeft = null;
              foreach (HGFlexGridBorderPainter borderPainterCheck in _listBorderPainter)
              {
                HGFlexGridBorderPainterBorderType borderType = borderPainterCheck.GetBorders(_intRowAktuell, intCol);
                if ((borderType & HGFlexGridBorderPainterBorderType.Top) == HGFlexGridBorderPainterBorderType.Top)
                {
                  borderPainterTop = borderPainterCheck;
                }
                if ((borderType & HGFlexGridBorderPainterBorderType.Right) == HGFlexGridBorderPainterBorderType.Right)
                {
                  borderPainterRight = borderPainterCheck;
                }
                if ((borderType & HGFlexGridBorderPainterBorderType.Bottom) == HGFlexGridBorderPainterBorderType.Bottom)
                {
                  borderPainterBottom = borderPainterCheck;
                }
                if ((borderType & HGFlexGridBorderPainterBorderType.Left) == HGFlexGridBorderPainterBorderType.Left)
                {
                  borderPainterLeft = borderPainterCheck;
                }
              }

              //Irgendwas gefunden?
              if (borderPainterTop != null || borderPainterRight != null || borderPainterBottom != null || borderPainterLeft != null)
              {
                //Attribut "style" erzeugen.
                //Erstmal alle Border abschalten - so macht Word das beim Copy.
                //WKnauf 17.05.2019: Mantis 16518: in fixed Cells, we have to default the borders: inherit from fixed style!
                if (styleCell == null && intCol < _c1FlexGrid.Cols.Fixed)
                {
                  CellStyle styleFixed = _c1FlexGrid.Styles.Fixed;
                  if (styleFixed.DefinedElements.HasFlag(StyleElementFlags.Border) == true)
                  {
                    //Apply Border from style only if no border from BorderPainter is defined.
                    //Never set a top/left border here - those are taken from the previous cell.
                    string strStyleData = GetBorderStyleCSS(styleFixed, false, false, (borderPainterBottom == null), (borderPainterRight == null));
                    _sbHTML.Append(" style=\"" + strStyleData);
                  }
                  else
                  {
                    //No border defines in fixed: create style element which die
                    _sbHTML.Append(" style=\"border:none;");
                  }
                }
                else if (styleCell != null)
                {
                  //Maybe the cellStyle defines borders:
                  //The Border might be switched off by setting the "Border.Style" to "None":
                  if (styleCell.DefinedElements.HasFlag(StyleElementFlags.Border) == true &&
                    styleCell.Border.Style != BorderStyleEnum.None)
                  {
                    //Apply Border from style only if no border from BorderPainter is defined.
                    //Never set a top/left border here - those are taken from the previous cell.
                    string strStyleData = GetBorderStyleCSS(styleCell, false, false, (borderPainterBottom == null), (borderPainterRight == null));
                    _sbHTML.Append(" style=\"" + strStyleData);
                  }
                  else
                  {
                    //TODO: Fallback!!!!
                    _sbHTML.Append(" style=\"border:none;");
                  }
                }
                else
                {
                  _sbHTML.Append(" style=\"border:none;");
                }
                //Border gefunden?
                if (borderPainterTop != null)
                {
                  ApplyBorderPainter(borderPainterTop.Pen, "border-top", _sbHTML);
                }
                if (borderPainterRight != null)
                {
                  ApplyBorderPainter(borderPainterRight.Pen, "border-right", _sbHTML);
                }
                if (borderPainterBottom != null)
                {
                  ApplyBorderPainter(borderPainterBottom.Pen, "border-bottom", _sbHTML);
                }
                if (borderPainterLeft != null)
                {
                  ApplyBorderPainter(borderPainterLeft.Pen, "border-left", _sbHTML);
                }
                //Schließendes Quote.
                _sbHTML.Append("\"");
              } //Ende if (irgendwas gefunden)
              #endregion
            }*/
            #endregion

            _sbHTML.Append(">");

            //Cell content: treat merged range differently.
            string strCellContent;
            if (rangeMerged.IsSingleCell == true)
            {
              strCellContent = _c1FlexGrid.GetDataDisplay(_intRowAktuell, intCol);
            }
            else
            {
              //Merged range: fetch data of top left cell:
              strCellContent = _c1FlexGrid.GetDataDisplay(rangeMerged.TopRow, rangeMerged.LeftCol);
            }
            //Replace entities and newlines:
            strCellContent = ReplaceHtmlChars(strCellContent);
            _sbHTML.Append(strCellContent);

            _sbHTML.AppendLine("</" + _tdElement + ">");
            #endregion
          }
        } //End if (Visible)
      }
    }

    /// <summary>
    /// Apply of cell border from a border painter to a "style" attribut of a cell: appends the style attributes for the
    /// according to the Pen of the border painter.
    /// Before calling this method, a "style=" attribute must have been started. No closing quote is added so that
    /// the calling code can continue appending style attributes.
    /// </summary>
    /// <param name="_pen">Current borderpainter pen</param>
    /// <param name="_strBorderType">CSS name of the border ("border-top", "border-right", "border-bottom", "border-left")</param>
    /// <param name="_sbHTML">Here the border style is appended.</param>
    private static void ApplyBorderPainter(Pen _pen, string _strBorderType, StringBuilder _sbHTML)
    {
      //See https://www.w3schools.com/css/css_border.asp

      //Word generates individual borders this way:
      /*<td width=184 style='width:138.0pt;border:none;border-right:dashed windowtext 1.0pt;
         mso-border-left-alt:dashed windowtext .25pt;mso-border-left-alt:dashed windowtext .25pt;
         mso-border-right-alt:dashed windowtext .25pt;padding:.75pt .75pt .75pt .75pt;
         height:15.75pt'></td>
       * 
       */
      //So I do it the same way and use the three attribute syntax.
      //According to the link it could be done with "border-top-style: dotted; border-right-style: solid; border-bottom-style: dotted; border-left-style: solid;".
      //(similar for "border-left-width: 12px;" and "border-left-color: ...;").
      //But this did not work in my tests.

      _sbHTML.Append(_strBorderType + ":");
      //Style:
      if (_pen.DashStyle == System.Drawing.Drawing2D.DashStyle.Custom)
      {
        _sbHTML.Append("dashed ");
      }
      else if (_pen.DashStyle == System.Drawing.Drawing2D.DashStyle.Solid)
      {
        _sbHTML.Append("solid ");
      }
      else
      {
        throw new InvalidOperationException("Unsupported DashStyle: " + _pen.DashStyle);
      }
      //Color:
      _sbHTML.Append(ColorTranslator.ToHtml(Color.FromArgb(_pen.Color.ToArgb())) + " ");
      //Width: it seems we have to divide it by 2, otherwise it is much too bold.
      _sbHTML.Append((_pen.Width / 2.0).ToString(cultureInfoEnglish) + "pt;");
    }

    /// <summary>
    /// Returns the CSS attributes for a CellStyle border definition.
    /// </summary>
    /// <param name="_style"></param>
    /// <param name="_bolCreateBorderBottom">TRUE: set Bottom Borders according to the Definition in CellStyle for "bottom".
    /// FALSE: don't set because a BorderPainter defines it differently.</param>
    /// <param name="_bolCreateBorderTop">TRUE: set Top Borders according to the Definition in CellStyle for "bottom" (ONLY relevant for fixed cells!)
    /// FALSE: don't set because a BorderPainter defines it differently, or set the border only below (relevant for "normal" cells)</param>
    /// <param name="_bolCreateBorderRight">TRUE: set Right Borders according to the Definition in CellStyle for "right".
    /// FALSE: don't set because a BorderPainter defines it differently.</param>
    /// <param name="_bolCreateBorderLeft">TRUE: set Left Borders according to the Definition in CellStyle for "ight" (ONLY relevant for fixed cells!)
    /// FALSE: don't set because a BorderPainter defines it differently, or set the border only right (relevant for "normal" cells)</param>
    /// <returns></returns>
    private static string GetBorderStyleCSS(CellStyle _style, bool _bolCreateBorderTop, bool _bolCreateBorderLeft, bool _bolCreateBorderBottom, bool _bolCreateBorderRight)
    {
      //See https://stackoverflow.com/questions/10783491/applying-table-cell-borders#10783731

      string strStyleData = string.Empty;

      //Create color string:
      string strColor = ColorTranslator.ToHtml(Color.FromArgb(_style.ForeColor.ToArgb()));

      //C1Flexgrid can only define borders to the right and below, so apply it similar.
      if (_style.Border.Direction == BorderDirEnum.Horizontal || _style.Border.Direction == BorderDirEnum.Both)
      {
        //Width: the value "0.5pt" seems to match the thin Border of Excel.
        //"0.5px" (0,5 Pixel?) does not work, so Point seems to be better.

        //For fixed cells, I have to also set the top/left border, otherwise fixed borders will not work in Excel.
        //But only create top/left, if the parameter says so (this is relevant if the border results from a BorderPainter).
        if (_bolCreateBorderBottom == true)
        {
          strStyleData += $"border-bottom: solid {strColor} 0.5pt;";
        }
        if (_bolCreateBorderTop == true)
        {
          strStyleData += $"border-top: solid {strColor} 0.5pt;";
        }
      }
      if (_style.Border.Direction == BorderDirEnum.Vertical || _style.Border.Direction == BorderDirEnum.Both)
      {
        if (_bolCreateBorderLeft == true)
        {
          strStyleData += $"border-left: solid {strColor} 0.5pt;";
        }
        if (_bolCreateBorderRight == true)
        {
          strStyleData += $"border-right: solid {strColor} 0.5pt;";
        }
      }

      return strStyleData;
    }

    /// <summary>
    /// Replaces the HTML characters (<![CDATA[<, >, & and "]]>) with the HTML entities.
    /// 
    /// Also replaces Newlines with "br"
    /// </summary>
    private static string ReplaceHtmlChars(string strString)
    {
      StringBuilder html = new StringBuilder(strString);
      // Replace "&" first, sonst schlaegt
      // diese Ersetzung bei den andern Entities zu.
      html.Replace("&", "&amp;");

      html.Replace("\"", "&quot;");
      html.Replace("<", "&lt;");
      html.Replace(">", "&gt;");


      html.Replace(Environment.NewLine, "<br>");

      return html.ToString();
    }
    #endregion
  }
}
