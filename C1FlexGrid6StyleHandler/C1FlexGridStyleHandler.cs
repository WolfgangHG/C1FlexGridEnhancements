using System;
using System.Collections;
using System.Drawing;
using System.Text;
using C1.Win.FlexGrid;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace C1FlexGrid6StyleHandler
{
  /// <summary>
  /// This class is used to simplify style life.
  /// -Style parts (BackColor, ForeColor, Font, Borders) can be set by making a call to a "Set"-Method.
  /// -Internally the class checks whether this combination of values was created already. If yes
  ///  this style is re-used.
  /// -It is also possible to append style parts to an existing style. In this case the current
  ///  style of the cell is taken and parts are modified. Internally this may create a new style.
  /// 
  /// The internal style names are created according to this rule:
  /// -First comes the style type (CellStyleEnum value)
  /// -Originally "cellStyle.BuildString" was used (contains only the changed values).
  ///  Currently a custom method is used to build the style strings, so it is not required to
  ///  build a temporary style. This saves a lot of time !
  /// 
  /// A call to SetXYZ and an additional call to MergeXYZ creates two styles, and the first one might
  /// be unused.
  /// 
  /// Usage sample:
  /// Control with a flexgrid: Declare member variable
  ///   private C1FlexGridStyleHandler flexGridStyleHandler = null;
  ///   
  /// In the constructor do the following:
  ///    this.flexGridStyleHandler = new C1FlexGridStyleHandler (this.myFlexGrid);
  ///    
  /// Then set the styles to cells:
  ///    //Normal cell with back Color:
  ///    this.flexGridStyleHandler.SetBackColorNormal (row, col, Color.Blue);
  ///    
  ///    //Another normal cell with back color:
  ///    this.flexGridStyleHandler.SetBackColorNormal (rowOther, colOther, Color.Yellow);
  ///    //This cell should also have a different fore color:
  ///    this.flexGridStyleHandler.MergeForeColorNormal (rowOther, colOther, Color.Green);
  /// </summary>
  public class C1FlexGridStyleHandler
  {
    #region Constants
    /// <summary>
    /// Separator for Style names
    /// </summary>
    private const string STYLENAME_SEPARATOR = "/";
    #endregion

    #region Member variables
    /// <summary>
    /// This is the grid whose styles are handled
    /// </summary>
    private C1FlexGrid flexGrid;

    /// <summary>
    /// Mapping of custom Style-Name and index in grids cell style collection.
    /// </summary>
    /// <remarks>
    /// "StringComparer.Ordinal" is faster than the default Stringcomparer.
    /// </remarks>
    private Dictionary<string, int> htStylesIndexInGrid = new Dictionary<string, int>(StringComparer.Ordinal);

    #endregion

    #region "Dummy" values
    /// <summary>If no color value needs to be specified by a set operation use this value </summary>
    private static Color COLOR_UNSPECIFIED = Color.Empty;
    /// <summary>If no "Border direction" value needs to be specified by a set operation use this value </summary>
    private static BorderDirEnum BORDERDIRECTION_UNSPECIFIED = BorderDirEnum.Both;
    /// <summary>If no "Border style" value needs to be specified by a set operation use this value </summary>
    private static BorderStyleEnum BORDERSTYLE_UNSPECIFIED = BorderStyleEnum.None;
    /// <summary>If no "Border width" value needs to be specified by a set operation use this value </summary>
    private static int BORDERWIDTH_UNSPECIFIED = 0;
    /// <summary>If no "Font" value needs to be specified by a set operation use this value </summary>
    private static Font FONT_UNSPECIFIED = null;
    /// <summary>If no "Text Align" value needs to be specified by a set operation use this value </summary>
    private static TextAlignEnum TEXT_ALIGN_UNSPECIFIED = TextAlignEnum.GeneralCenter;
    /// <summary>If no "WordWrap" value needs to be specified by a set operation use this value </summary>
    private static bool WORD_WRAP_UNSPECIFIED = false;
    /// <summary>If no "Image Align" value needs to be specified by a set operation use this value </summary>
    private static ImageAlignEnum IMAGE_ALIGN_UNSPECIFIED = ImageAlignEnum.CenterCenter;
    /// <summary>If no "TextDirection" value needs to be specified by a set operation use this value </summary>
    private static TextDirectionEnum TEXT_DIRECTION_UNSPECIFIED = TextDirectionEnum.Normal;
    /// <summary>
    /// If no "Format" value needs to be specified by a set operation, then use this value
    /// </summary>
    private static string FORMAT_UNSPECIFIED = null;
    #endregion

    #region Constructors
    /// <summary>
    /// Konstruktor.
    /// </summary>
    /// <param name="_flexGrid">Auf diesem Grid wird gearbeitet.</param>
    public C1FlexGridStyleHandler(C1FlexGrid _flexGrid)
    {
      this.flexGrid = _flexGrid;
    }
    #endregion

    #region Public methods
    /// <summary>
    /// Reset this class: the internal hashtable of cached styles is rebuilt, and 
    /// all custom styles are removed from the underlying hashtable.
    /// 
    /// Call this if you e.g. change the Font of the flexgrid, because otherwise the previously created
    /// custom styles with the old fonts would be reused.
    /// </summary>
    public void Reset()
    {
      //Now remove all custom styles from the grid:
      int intFirstCustom = (int)CellStyleEnum.FirstCustomStyle;
      //WKnauf 29.10.2012: Mantis 5879 - remove only Styles AFTER the FirstCustomStyle, but not the FirstCustomStyle.
      //for (int intIndexStyle = this.flexGrid.Styles.Count - 1; intIndexStyle >= intFirstCustom; intIndexStyle--)
      for (int intIndexStyle = this.flexGrid.Styles.Count - 1; intIndexStyle > intFirstCustom; intIndexStyle--)
      {
        this.flexGrid.Styles.Remove(intIndexStyle);
      }

      //Clear Hashtable of style=>index-mapping.
      this.htStylesIndexInGrid.Clear();
    }

    #region Set/Merge Backcolor
    /// <summary>
    /// Set the Back Color for "Normal" style to a cell.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_intCol"></param>
    /// <param name="_backColor"></param>
    public void SetBackColor(int _intRow, int _intCol, Color _backColor)
    {
      //Use the cell range method:
      this.SetBackColor(_intRow, _intCol, _intRow, _intCol, _backColor);
    }
    /// <summary>
    /// Takes the style information from the cell at (col/row) and merges it with the specified back color.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_intCol"></param>
    /// <param name="_backColor"></param>
    public void MergeBackColor(int _intRow, int _intCol, Color _backColor)
    {
      //Let the cell range method do the work:
      this.MergeBackColor(_intRow, _intCol, _intRow, _intCol, _backColor);
    }

    /// <summary>
    /// Takes the style information from the cell at (col/row) and merges it with the specified back color.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_strCol"></param>
    /// <param name="_backColor"></param>
    public void MergeBackColor(int _intRow, string _strCol, Color _backColor)
    {
      int intCol = this.flexGrid.Cols[_strCol].Index;
      //Let the cell range method do the work:
      this.MergeBackColor(_intRow, intCol, _intRow, intCol, _backColor);
    }

    /// <summary>
    /// Set the Back Color to a cell range.
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_backColor"></param>
    public void SetBackColor(int _intRow1, int _intCol1, int _intRow2, int _intCol2, Color _backColor)
    {
      this.SetStyle(_intRow1, _intCol1, _intRow2, _intCol2, StyleElementFlags.BackColor,
        _backColor,
        COLOR_UNSPECIFIED, BORDERDIRECTION_UNSPECIFIED, BORDERSTYLE_UNSPECIFIED, BORDERWIDTH_UNSPECIFIED,
        FONT_UNSPECIFIED, COLOR_UNSPECIFIED, TEXT_ALIGN_UNSPECIFIED, WORD_WRAP_UNSPECIFIED, IMAGE_ALIGN_UNSPECIFIED, TEXT_DIRECTION_UNSPECIFIED, FORMAT_UNSPECIFIED);
    }

    /// <summary>
    /// Takes the style information from the cell at (col 1/row 1) and merges it with the specified back color.
    /// Then sets the style to the whole cell range.
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_backColor"></param>
    public void MergeBackColor(int _intRow1, int _intCol1, int _intRow2, int _intCol2, Color _backColor)
    {
      this.MergeStyle(_intRow1, _intCol1, _intRow2, _intCol2, StyleElementFlags.BackColor,
        _backColor, COLOR_UNSPECIFIED, BORDERDIRECTION_UNSPECIFIED, BORDERSTYLE_UNSPECIFIED, BORDERWIDTH_UNSPECIFIED,
        FONT_UNSPECIFIED, COLOR_UNSPECIFIED, TEXT_ALIGN_UNSPECIFIED, WORD_WRAP_UNSPECIFIED, IMAGE_ALIGN_UNSPECIFIED, TEXT_DIRECTION_UNSPECIFIED, FORMAT_UNSPECIFIED);
    }

    /// <summary>
    /// Takes the style information from the cells in the range and merge them with the specified back and fore color.
    /// Then sets the style to the whole cell range.
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_backColor"></param>
    /// <param name="_foreColor"></param>
    public void MergeBackForeColor(int _intRow1, int _intCol1, Color _backColor, Color _foreColor)
    {
      //Use common "MergeStyle" method.
      this.MergeBackForeColor(_intRow1, _intCol1, _intRow1, _intCol1, _backColor, _foreColor);
    }

    /// <summary>
    /// Takes the style information from the cells in the range and merge them with the specified back and fore color.
    /// Then sets the style to the whole cell range.
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_strCol1"></param>
    /// <param name="_backColor"></param>
    /// <param name="_foreColor"></param>
    public void MergeBackForeColor(int _intRow1, string _strCol1, Color _backColor, Color _foreColor)
    {
      int intCol = this.flexGrid.Cols[_strCol1].Index;
      //Use common "MergeStyle" method.
      this.MergeBackForeColor(_intRow1, intCol, _intRow1, intCol, _backColor, _foreColor);
    }

    /// <summary>
    /// Takes the style information from the cell cells in the range and merge them with the specified back and fore color.
    /// Then sets the style to the whole cell range.
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_backColor"></param>
    /// <param name="_foreColor"></param>
    public void MergeBackForeColor(int _intRow1, int _intCol1, int _intRow2, int _intCol2, Color _backColor, Color _foreColor)
    {
      //Use common "MergeStyle" method.
      this.MergeStyle(_intRow1, _intCol1, _intRow2, _intCol2, StyleElementFlags.BackColor | StyleElementFlags.ForeColor,
        _backColor, COLOR_UNSPECIFIED, BORDERDIRECTION_UNSPECIFIED, BORDERSTYLE_UNSPECIFIED, BORDERWIDTH_UNSPECIFIED,
        FONT_UNSPECIFIED, _foreColor, TEXT_ALIGN_UNSPECIFIED, WORD_WRAP_UNSPECIFIED, IMAGE_ALIGN_UNSPECIFIED, TEXT_DIRECTION_UNSPECIFIED, FORMAT_UNSPECIFIED);
    }

    /// <summary>
    /// Set the Back and Fore Color to a cell range.
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_backColor"></param>
    /// <param name="_foreColor"></param>
    public void SetBackForeColor(int _intRow1, int _intCol1, int _intRow2, int _intCol2, Color _backColor, Color _foreColor)
    {
      //Use the "allover" method, and set only the back and fore color (all other fields can have their defaults).
      this.SetStyle(_intRow1, _intCol1, _intRow2, _intCol2, StyleElementFlags.BackColor | StyleElementFlags.ForeColor,
        _backColor, COLOR_UNSPECIFIED, BORDERDIRECTION_UNSPECIFIED, BORDERSTYLE_UNSPECIFIED, BORDERWIDTH_UNSPECIFIED,
        FONT_UNSPECIFIED, _foreColor, TEXT_ALIGN_UNSPECIFIED, WORD_WRAP_UNSPECIFIED, IMAGE_ALIGN_UNSPECIFIED, TEXT_DIRECTION_UNSPECIFIED, FORMAT_UNSPECIFIED);
    }

    /// <summary>
    /// If the cell style of the cell at row/col specifies a BackColor, reset it.
    /// 
    /// This might be used if fixed rows/cols have a custom style, but should be rendered using the "SelectedRow/ColumnHeader" style
    /// (which happens automatically if a VisualStyle is set). In this case reset the custom BackColor if the row/col header is 
    /// inside a selection.
    /// But you have to set the BackColor once again if it the selection changes to another cell.
    /// </summary>
    /// <remarks>
    /// Same method needed for a cell range and for all other style elements!
    /// </remarks>
    /// <param name="_intRow"></param>
    /// <param name="_intCol"></param>
    public void ResetBackColor(int _intRow, int _intCol)
    {
      CellStyle cellStyle = this.flexGrid.GetCellStyle(_intRow, _intCol);

      //If the cell has no cellStyle or no BackColor is specified, then don't do anything.
      if (cellStyle != null && (cellStyle.DefinedElements & StyleElementFlags.BackColor) == StyleElementFlags.BackColor)
      {
        StyleElementFlags styleElemFlagsNew = cellStyle.DefinedElements ^ StyleElementFlags.BackColor; //"XOR" entfernt hier die BackColor.
        this.SetStyle(cellStyle, _intRow, _intCol, styleElemFlagsNew, COLOR_UNSPECIFIED,
          cellStyle.Border.Color, cellStyle.Border.Direction, cellStyle.Border.Style, cellStyle.Border.Width,
          cellStyle.Font, cellStyle.ForeColor, cellStyle.TextAlign, cellStyle.WordWrap, cellStyle.ImageAlign, cellStyle.TextDirection, cellStyle.Format);
      }
    }
    #endregion

    #region Set/Merge Border
    /// <summary>
    /// Set the Border to a cell.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_intCol"></param>
    /// <param name="_borderColor"></param>
    /// <param name="_borderDirection"></param>
    /// <param name="_borderStyle"></param>
    /// <param name="_intBorderWidth"></param>
    public void SetBorder(int _intRow, int _intCol, Color _borderColor, BorderDirEnum _borderDirection, BorderStyleEnum _borderStyle, int _intBorderWidth)
    {
      //Use the cell range method:
      this.SetBorder(_intRow, _intCol, _intRow, _intCol, _borderColor, _borderDirection, _borderStyle, _intBorderWidth);
    }

    /// <summary>
    /// Takes the style information from the cell at (col/row) and merges it with the specified border information.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_intCol"></param>
    /// <param name="_borderColor"></param>
    /// <param name="_borderDirection"></param>
    /// <param name="_borderStyle"></param>
    /// <param name="_intBorderWidth"></param>
    public void MergeBorder(int _intRow, int _intCol, Color _borderColor, BorderDirEnum _borderDirection, BorderStyleEnum _borderStyle, int _intBorderWidth)
    {
      //Use the cell range method:
      this.MergeBorder(_intRow, _intCol, _intRow, _intCol, _borderColor, _borderDirection, _borderStyle, _intBorderWidth);
    }

    /// <summary>
    /// Takes the style information from the cell at (col/row) and merges it with the specified border information.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_strCol"></param>
    /// <param name="_borderColor"></param>
    /// <param name="_borderDirection"></param>
    /// <param name="_borderStyle"></param>
    /// <param name="_intBorderWidth"></param>
    public void MergeBorder(int _intRow, string _strCol, Color _borderColor, BorderDirEnum _borderDirection, BorderStyleEnum _borderStyle, int _intBorderWidth)
    {
      int intCol = this.flexGrid.Cols[_strCol].Index;
      //Use the cell range method:
      this.MergeBorder(_intRow, intCol, _intRow, intCol, _borderColor, _borderDirection, _borderStyle, _intBorderWidth);
    }

    /// <summary>
    /// Set the Border to a cell RANGE, all other values are based on the specified base style.
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_borderColor"></param>
    /// <param name="_borderDirection"></param>
    /// <param name="_borderStyle"></param>
    /// <param name="_intBorderWidth"></param>
    public void SetBorder(int _intRow1, int _intCol1, int _intRow2, int _intCol2, Color _borderColor, BorderDirEnum _borderDirection, BorderStyleEnum _borderStyle, int _intBorderWidth)
    {
      //Use the "allover" method, and set only the border values (all other fields can have their defaults).
      this.SetStyle(_intRow1, _intCol1, _intRow2, _intCol2, StyleElementFlags.Border,
        COLOR_UNSPECIFIED,
        _borderColor, _borderDirection, _borderStyle, _intBorderWidth,
        FONT_UNSPECIFIED, COLOR_UNSPECIFIED, TEXT_ALIGN_UNSPECIFIED, WORD_WRAP_UNSPECIFIED, IMAGE_ALIGN_UNSPECIFIED, TEXT_DIRECTION_UNSPECIFIED, FORMAT_UNSPECIFIED);
    }

    /// <summary>
    /// Takes the style information from the cell at (col/row) and merges it with the specified border information.
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_borderColor"></param>
    /// <param name="_borderDirection"></param>
    /// <param name="_borderStyle"></param>
    /// <param name="_intBorderWidth"></param>
    public void MergeBorder(int _intRow1, int _intCol1, int _intRow2, int _intCol2, Color _borderColor, BorderDirEnum _borderDirection, BorderStyleEnum _borderStyle, int _intBorderWidth)
    {
      //Use common "MergeStyle" method.
      this.MergeStyle(_intRow1, _intCol1, _intRow2, _intCol2, StyleElementFlags.Border,
        COLOR_UNSPECIFIED, _borderColor, _borderDirection, _borderStyle, _intBorderWidth,
        FONT_UNSPECIFIED, COLOR_UNSPECIFIED, TEXT_ALIGN_UNSPECIFIED, WORD_WRAP_UNSPECIFIED, IMAGE_ALIGN_UNSPECIFIED, TEXT_DIRECTION_UNSPECIFIED, FORMAT_UNSPECIFIED);
    }
    #endregion

    #region Set/Merge Font
    /// <summary>
    /// Set the Font to a cell.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_intCol"></param>
    /// <param name="_font"></param>
    public void SetFont(int _intRow, int _intCol, Font _font)
    {
      //Use the cell range method.
      this.SetFont(_intRow, _intCol, _intRow, _intCol, _font);
    }
    /// <summary>
    /// Takes the style information from the cell at (col/row) and merges it with the specified font.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_intCol"></param>
    /// <param name="_font"></param>
    public void MergeFont(int _intRow, int _intCol, Font _font)
    {
      //Use the cell range method.
      this.MergeFont(_intRow, _intCol, _intRow, _intCol, _font);
    }

    /// <summary>
    /// Takes the style information from the cell at (col/row) and merges it with the specified font.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_strCol"></param>
    /// <param name="_font"></param>
    public void MergeFont(int _intRow, string _strCol, Font _font)
    {
      int intCol = this.flexGrid.Cols[_strCol].Index;
      //Use the cell range method.
      this.MergeFont(_intRow, intCol, _intRow, intCol, _font);
    }

    /// <summary>
    /// Set the Back Color to a cell range
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_font"></param>
    public void SetFont(int _intRow1, int _intCol1, int _intRow2, int _intCol2, Font _font)
    {
      //Use the "allover" method, and set only the font (all other fields can have their defaults).
      this.SetStyle(_intRow1, _intCol1, _intRow2, _intCol2, StyleElementFlags.Font,
        COLOR_UNSPECIFIED,
        COLOR_UNSPECIFIED, BORDERDIRECTION_UNSPECIFIED, BORDERSTYLE_UNSPECIFIED, BORDERWIDTH_UNSPECIFIED,
        _font, COLOR_UNSPECIFIED, TEXT_ALIGN_UNSPECIFIED, WORD_WRAP_UNSPECIFIED, IMAGE_ALIGN_UNSPECIFIED, TEXT_DIRECTION_UNSPECIFIED, FORMAT_UNSPECIFIED);
    }
    /// <summary>
    /// Takes the style information from the cell at (col 1/row 1) and merges it with the specified font.
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_font"></param>
    public void MergeFont(int _intRow1, int _intCol1, int _intRow2, int _intCol2, Font _font)
    {
      //Use common "MergeStyle" method.
      this.MergeStyle(_intRow1, _intCol1, _intRow2, _intCol2, StyleElementFlags.Font,
        COLOR_UNSPECIFIED, COLOR_UNSPECIFIED, BORDERDIRECTION_UNSPECIFIED, BORDERSTYLE_UNSPECIFIED, BORDERWIDTH_UNSPECIFIED,
        _font, COLOR_UNSPECIFIED, TEXT_ALIGN_UNSPECIFIED, WORD_WRAP_UNSPECIFIED, IMAGE_ALIGN_UNSPECIFIED, TEXT_DIRECTION_UNSPECIFIED, FORMAT_UNSPECIFIED);
    }


    /// <summary>
    /// If the cell style of the cell at row/col specifies a Font, reset it.
    /// </summary>
    /// <remarks>
    /// Same method needed for a cell range and for all other style elements!
    /// </remarks>
    /// <param name="_intRow">Reset font in this row (should be valid)</param>
    /// <param name="_intCol">Reset font in this col (should be valid)</param>
    public void ResetFont(int _intRow, int _intCol)
    {
      CellStyle cellStyle = this.flexGrid.GetCellStyle(_intRow, _intCol);

      //If the cell has no cellStyle or no Font is specified, then don't do anything.
      if (cellStyle != null && (cellStyle.DefinedElements & StyleElementFlags.Font) == StyleElementFlags.Font)
      {
        StyleElementFlags styleElemFlagsNew = cellStyle.DefinedElements ^ StyleElementFlags.Font; //"XOR" entfernt hier die Font.
        this.SetStyle(cellStyle, _intRow, _intCol, styleElemFlagsNew, cellStyle.BackColor,
          cellStyle.Border.Color, cellStyle.Border.Direction, cellStyle.Border.Style, cellStyle.Border.Width,
          FONT_UNSPECIFIED, cellStyle.ForeColor, cellStyle.TextAlign, cellStyle.WordWrap, cellStyle.ImageAlign, cellStyle.TextDirection, cellStyle.Format);
      }
    }
    #endregion

    #region Set/Merge Forecolor
    /// <summary>
    /// Set the Fore Color to a cell.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_intCol"></param>
    /// <param name="_foreColor"></param>
    public void SetForeColor(int _intRow, int _intCol, Color _foreColor)
    {
      //Use the cell range method:
      this.SetForeColor(_intRow, _intCol, _intRow, _intCol, _foreColor);
    }
    /// <summary>
    /// Takes the style information from the cell at (col/row) and merges it with the specified fore color.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_intCol"></param>
    /// <param name="_foreColor"></param>
    public void MergeForeColor(int _intRow, int _intCol, Color _foreColor)
    {
      //Use the cell range method:
      this.MergeForeColor(_intRow, _intCol, _intRow, _intCol, _foreColor);
    }

    /// <summary>
    /// Takes the style information from the cell at (col/row) and merges it with the specified fore color.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_strCol"></param>
    /// <param name="_foreColor"></param>
    public void MergeForeColor(int _intRow, string _strCol, Color _foreColor)
    {
      int intCol = this.flexGrid.Cols[_strCol].Index;
      //Use the cell range method:
      this.MergeForeColor(_intRow, intCol, _intRow, intCol, _foreColor);
    }

    /// <summary>
    /// Set the Fore Color to a cell range
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_foreColor"></param>
    public void SetForeColor(int _intRow1, int _intCol1, int _intRow2, int _intCol2, Color _foreColor)
    {
      //Use the "allover" method, and set only the fore color (all other fields can have their defaults).
      this.SetStyle(_intRow1, _intCol1, _intRow2, _intCol2, StyleElementFlags.ForeColor,
        COLOR_UNSPECIFIED,
        COLOR_UNSPECIFIED, BORDERDIRECTION_UNSPECIFIED, BORDERSTYLE_UNSPECIFIED, BORDERWIDTH_UNSPECIFIED,
        FONT_UNSPECIFIED, _foreColor, TEXT_ALIGN_UNSPECIFIED, WORD_WRAP_UNSPECIFIED, IMAGE_ALIGN_UNSPECIFIED, TEXT_DIRECTION_UNSPECIFIED, FORMAT_UNSPECIFIED);
    }
    /// <summary>
    /// Takes the style information from the cell at (col 1/row 1) and merges it with the specified fore color.
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_foreColor"></param>
    public void MergeForeColor(int _intRow1, int _intCol1, int _intRow2, int _intCol2, Color _foreColor)
    {
      //Use common "MergeStyle" method.
      this.MergeStyle(_intRow1, _intCol1, _intRow2, _intCol2, StyleElementFlags.ForeColor,
        COLOR_UNSPECIFIED, COLOR_UNSPECIFIED, BORDERDIRECTION_UNSPECIFIED, BORDERSTYLE_UNSPECIFIED, BORDERWIDTH_UNSPECIFIED,
        FONT_UNSPECIFIED, _foreColor, TEXT_ALIGN_UNSPECIFIED, WORD_WRAP_UNSPECIFIED, IMAGE_ALIGN_UNSPECIFIED, TEXT_DIRECTION_UNSPECIFIED, FORMAT_UNSPECIFIED);
    }

    /// <summary>
    /// If the cell style of the cell at row/col specifies a ForeColor, reset it.
    /// </summary>
    /// <remarks>
    /// Same method needed for a cell range and for all other style elements!
    /// </remarks>
    /// <param name="_intRow">Reset fore color in this row (should be valid)</param>
    /// <param name="_intCol">Reset fore color in this col (should be valid)</param>
    public void ResetForeColor(int _intRow, int _intCol)
    {
      CellStyle cellStyle = this.flexGrid.GetCellStyle(_intRow, _intCol);

      //If the cell has no cellStyle or no ForeColor is specified, then don't do anything.
      if (cellStyle != null && (cellStyle.DefinedElements & StyleElementFlags.ForeColor) == StyleElementFlags.ForeColor)
      {
        StyleElementFlags styleElemFlagsNew = cellStyle.DefinedElements ^ StyleElementFlags.ForeColor; //"XOR" entfernt hier die ForeColor.
        this.SetStyle(cellStyle, _intRow, _intCol, styleElemFlagsNew, cellStyle.BackColor,
          cellStyle.Border.Color, cellStyle.Border.Direction, cellStyle.Border.Style, cellStyle.Border.Width,
          cellStyle.Font, COLOR_UNSPECIFIED, cellStyle.TextAlign, cellStyle.WordWrap, cellStyle.ImageAlign, cellStyle.TextDirection, cellStyle.Format);
      }
    }
    #endregion

    #region Set/Merge TextAlign
    /// <summary>
    /// Set the TextAlign to a cell.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_intCol"></param>
    /// <param name="_textAlign"></param>
    public void SetTextAlign(int _intRow, int _intCol, TextAlignEnum _textAlign)
    {
      //Use the cell range method:
      this.SetTextAlign(_intRow, _intCol, _intRow, _intCol, _textAlign);
    }
    /// <summary>
    /// Takes the style information from the cell at (col/row) and merges it with the specified TextAlign.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_intCol"></param>
    /// <param name="_textAlign"></param>
    public void MergeTextAlign(int _intRow, int _intCol, TextAlignEnum _textAlign)
    {
      //Use the cell range method:
      this.MergeTextAlign(_intRow, _intCol, _intRow, _intCol, _textAlign);
    }

    /// <summary>
    /// Takes the style information from the cell at (col/row) and merges it with the specified TextAlign.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_strCol"></param>
    /// <param name="_textAlign"></param>
    public void MergeTextAlign(int _intRow, string _strCol, TextAlignEnum _textAlign)
    {
      int intCol = this.flexGrid.Cols[_strCol].Index;
      //Use the cell range method:
      this.MergeTextAlign(_intRow, intCol, _intRow, intCol, _textAlign);
    }

    /// <summary>
    /// Set the TextAlign to a cell range
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_textAlign">Text Align</param>
    public void SetTextAlign(int _intRow1, int _intCol1, int _intRow2, int _intCol2, TextAlignEnum _textAlign)
    {
      //Use the "allover" method, and set only the TextAlign (all other fields can have their defaults).
      this.SetStyle(_intRow1, _intCol1, _intRow2, _intCol2, StyleElementFlags.TextAlign,
        COLOR_UNSPECIFIED,
        COLOR_UNSPECIFIED, BORDERDIRECTION_UNSPECIFIED, BORDERSTYLE_UNSPECIFIED, BORDERWIDTH_UNSPECIFIED,
        FONT_UNSPECIFIED, COLOR_UNSPECIFIED, _textAlign, WORD_WRAP_UNSPECIFIED, IMAGE_ALIGN_UNSPECIFIED, TEXT_DIRECTION_UNSPECIFIED, FORMAT_UNSPECIFIED);
    }
    /// <summary>
    /// Takes the style information from the cell at (col 1/row 1) and merges it with the specified TextAlign.
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_textAlign"></param>
    public void MergeTextAlign(int _intRow1, int _intCol1, int _intRow2, int _intCol2, TextAlignEnum _textAlign)
    {
      //Use common "MergeStyle" method.
      this.MergeStyle(_intRow1, _intCol1, _intRow2, _intCol2, StyleElementFlags.TextAlign,
        COLOR_UNSPECIFIED, COLOR_UNSPECIFIED, BORDERDIRECTION_UNSPECIFIED, BORDERSTYLE_UNSPECIFIED, BORDERWIDTH_UNSPECIFIED,
        FONT_UNSPECIFIED, COLOR_UNSPECIFIED, _textAlign, WORD_WRAP_UNSPECIFIED, IMAGE_ALIGN_UNSPECIFIED, TEXT_DIRECTION_UNSPECIFIED, FORMAT_UNSPECIFIED);
    }
    #endregion

    #region Set/Merge WordWrap
    /// <summary>
    /// Set the WordWrap to a cell.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_intCol"></param>
    /// <param name="_bolWordWrap">TRUE if text can be wrapped.</param>
    public void SetWordWrap(int _intRow, int _intCol, bool _bolWordWrap)
    {
      //Use the cell range method:
      this.SetWordWrap(_intRow, _intCol, _intRow, _intCol, _bolWordWrap);
    }
    /// <summary>
    /// Takes the style information from the cell at (col/row) and merges it with the specified WordWrap.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_intCol"></param>
    /// <param name="_bolWordWrap">TRUE if text can be wrapped.</param>
    public void MergeWordWrap(int _intRow, int _intCol, bool _bolWordWrap)
    {
      //Use the cell range method:
      this.MergeWordWrap(_intRow, _intCol, _intRow, _intCol, _bolWordWrap);
    }

    /// <summary>
    /// Takes the style information from the cell at (col/row) and merges it with the specified WordWrap.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_strCol"></param>
    /// <param name="_bolWordWrap">TRUE if text can be wrapped.</param>
    public void MergeWordWrap(int _intRow, string _strCol, bool _bolWordWrap)
    {
      int intCol = this.flexGrid.Cols[_strCol].Index;
      //Use the cell range method:
      this.MergeWordWrap(_intRow, intCol, _intRow, intCol, _bolWordWrap);
    }

    /// <summary>
    /// Set the WordWrap to a cell range
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_bolWordWrap">TRUE if text can be wrapped.</param>
    public void SetWordWrap(int _intRow1, int _intCol1, int _intRow2, int _intCol2, bool _bolWordWrap)
    {
      //Use the "allover" method, and set only the WordWrap (all other fields can have their defaults).
      this.SetStyle(_intRow1, _intCol1, _intRow2, _intCol2, StyleElementFlags.WordWrap,
        COLOR_UNSPECIFIED,
        COLOR_UNSPECIFIED, BORDERDIRECTION_UNSPECIFIED, BORDERSTYLE_UNSPECIFIED, BORDERWIDTH_UNSPECIFIED,
        FONT_UNSPECIFIED, COLOR_UNSPECIFIED, TEXT_ALIGN_UNSPECIFIED, _bolWordWrap, IMAGE_ALIGN_UNSPECIFIED, TEXT_DIRECTION_UNSPECIFIED, FORMAT_UNSPECIFIED);
    }

    /// <summary>
    /// Takes the style information from the cell at (col 1/row 1) and merges it with the specified WordWrap.
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_bolWordWrap">TRUE if text can be wrapped.</param>
    public void MergeWordWrap(int _intRow1, int _intCol1, int _intRow2, int _intCol2, bool _bolWordWrap)
    {
      //Use common "MergeStyle" method.
      this.MergeStyle(_intRow1, _intCol1, _intRow2, _intCol2, StyleElementFlags.WordWrap,
        COLOR_UNSPECIFIED, COLOR_UNSPECIFIED, BORDERDIRECTION_UNSPECIFIED, BORDERSTYLE_UNSPECIFIED, BORDERWIDTH_UNSPECIFIED,
        FONT_UNSPECIFIED, COLOR_UNSPECIFIED, TEXT_ALIGN_UNSPECIFIED, _bolWordWrap, IMAGE_ALIGN_UNSPECIFIED, TEXT_DIRECTION_UNSPECIFIED, FORMAT_UNSPECIFIED);
    }
    #endregion

    #region Set/Merge ImageAlign
    /// <summary>
    /// Set the ImageAlign to a cell.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_intCol"></param>
    /// <param name="_imageAlign"></param>
    public void SetImageAlign(int _intRow, int _intCol, ImageAlignEnum _imageAlign)
    {
      //Use the cell range method:
      this.SetImageAlign(_intRow, _intCol, _intRow, _intCol, _imageAlign);
    }
    /// <summary>
    /// Takes the style information from the cell at (col/row) and merges it with the specified ImageAlign.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_intCol"></param>
    /// <param name="_imageAlign"></param>
    public void MergeImageAlign(int _intRow, int _intCol, ImageAlignEnum _imageAlign)
    {
      //Use the cell range method:
      this.MergeImageAlign(_intRow, _intCol, _intRow, _intCol, _imageAlign);
    }

    /// <summary>
    /// Takes the style information from the cell at (col/row) and merges it with the specified ImageAlign.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_strCol"></param>
    /// <param name="_imageAlign"></param>
    public void MergeImageAlign(int _intRow, string _strCol, ImageAlignEnum _imageAlign)
    {
      int intCol = this.flexGrid.Cols[_strCol].Index;
      //Use the cell range method:
      this.MergeImageAlign(_intRow, intCol, _intRow, intCol, _imageAlign);
    }

    /// <summary>
    /// Set the ImageAlign to a cell range
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_imageAlign">Text Align</param>
    public void SetImageAlign(int _intRow1, int _intCol1, int _intRow2, int _intCol2, ImageAlignEnum _imageAlign)
    {
      //Use the "allover" method, and set only the ImageAlign (all other fields can have their defaults).
      this.SetStyle(_intRow1, _intCol1, _intRow2, _intCol2, StyleElementFlags.ImageAlign,
        COLOR_UNSPECIFIED,
        COLOR_UNSPECIFIED, BORDERDIRECTION_UNSPECIFIED, BORDERSTYLE_UNSPECIFIED, BORDERWIDTH_UNSPECIFIED,
        FONT_UNSPECIFIED, COLOR_UNSPECIFIED, TEXT_ALIGN_UNSPECIFIED, WORD_WRAP_UNSPECIFIED, _imageAlign, TEXT_DIRECTION_UNSPECIFIED, FORMAT_UNSPECIFIED);
    }

    /// <summary>
    /// Takes the style information from the cell at (col 1/row 1) and merges it with the specified ImageAlign.
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_imageAlign"></param>
    public void MergeImageAlign(int _intRow1, int _intCol1, int _intRow2, int _intCol2, ImageAlignEnum _imageAlign)
    {
      //Use common "MergeStyle" method.
      this.MergeStyle(_intRow1, _intCol1, _intRow2, _intCol2, StyleElementFlags.ImageAlign,
        COLOR_UNSPECIFIED, COLOR_UNSPECIFIED, BORDERDIRECTION_UNSPECIFIED, BORDERSTYLE_UNSPECIFIED, BORDERWIDTH_UNSPECIFIED,
        FONT_UNSPECIFIED, COLOR_UNSPECIFIED, TEXT_ALIGN_UNSPECIFIED, WORD_WRAP_UNSPECIFIED, _imageAlign, TEXT_DIRECTION_UNSPECIFIED, FORMAT_UNSPECIFIED);
    }
    #endregion

    #region Set/Merge TextDirection
    /// <summary>
    /// Set the TextAlign to a cell.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_intCol"></param>
    /// <param name="_textDirection">Text Direction</param>
    public void SetTextDirection(int _intRow, int _intCol, TextDirectionEnum _textDirection)
    {
      //Use the cell range method:
      this.SetTextDirection(_intRow, _intCol, _intRow, _intCol, _textDirection);
    }
    /// <summary>
    /// Takes the style information from the cell at (col/row) and merges it with the specified TextAlign.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_intCol"></param>
    /// <param name="_textDirection">Text Direction</param>
    public void MergeTextDirection(int _intRow, int _intCol, TextDirectionEnum _textDirection)
    {
      //Use the cell range method:
      this.MergeTextDirection(_intRow, _intCol, _intRow, _intCol, _textDirection);
    }

    /// <summary>
    /// Takes the style information from the cell at (col/row) and merges it with the specified TextAlign.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_strCol"></param>
    /// <param name="_textDirection">Text Direction</param>
    public void MergeTextDirection(int _intRow, string _strCol, TextDirectionEnum _textDirection)
    {
      int intCol = this.flexGrid.Cols[_strCol].Index;
      //Use the cell range method:
      this.MergeTextDirection(_intRow, intCol, _intRow, intCol, _textDirection);
    }

    /// <summary>
    /// Set the TextDirection to a cell range
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_textDirection">Text Direction</param>
    public void SetTextDirection(int _intRow1, int _intCol1, int _intRow2, int _intCol2, TextDirectionEnum _textDirection)
    {
      //Use the "allover" method, and set only the TextDirection (all other fields can have their defaults).
      this.SetStyle(_intRow1, _intCol1, _intRow2, _intCol2, StyleElementFlags.TextDirection,
        COLOR_UNSPECIFIED,
        COLOR_UNSPECIFIED, BORDERDIRECTION_UNSPECIFIED, BORDERSTYLE_UNSPECIFIED, BORDERWIDTH_UNSPECIFIED,
        FONT_UNSPECIFIED, COLOR_UNSPECIFIED, TEXT_ALIGN_UNSPECIFIED, WORD_WRAP_UNSPECIFIED, IMAGE_ALIGN_UNSPECIFIED, _textDirection, FORMAT_UNSPECIFIED);
    }

    /// <summary>
    /// Takes the style information from the cell at (col 1/row 1) and merges it with the specified TextDirection.
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_textDirection"></param>
    public void MergeTextDirection(int _intRow1, int _intCol1, int _intRow2, int _intCol2, TextDirectionEnum _textDirection)
    {
      //Use common "MergeStyle" method.
      this.MergeStyle(_intRow1, _intCol1, _intRow2, _intCol2, StyleElementFlags.TextDirection,
        COLOR_UNSPECIFIED, COLOR_UNSPECIFIED, BORDERDIRECTION_UNSPECIFIED, BORDERSTYLE_UNSPECIFIED, BORDERWIDTH_UNSPECIFIED,
        FONT_UNSPECIFIED, COLOR_UNSPECIFIED, TEXT_ALIGN_UNSPECIFIED, WORD_WRAP_UNSPECIFIED, IMAGE_ALIGN_UNSPECIFIED, _textDirection, FORMAT_UNSPECIFIED);
    }
    #endregion

    #region Set/Merge Format
    /// <summary>
    /// Set the WordWrap to a cell.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_intCol"></param>
    /// <param name="_strFormat">Format of the cell data.</param>
    public void SetFormat(int _intRow, int _intCol, string _strFormat)
    {
      //Use the cell range method:
      this.SetFormat(_intRow, _intCol, _intRow, _intCol, _strFormat);
    }

    /// <summary>
    /// Takes the style information from the cell at (col/row) and merges it with the specified WordWrap.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_intCol"></param>
    /// <param name="_strFormat">Format of the cell data.</param>
    public void MergeFormat(int _intRow, int _intCol, string _strFormat)
    {
      //Use the cell range method:
      this.MergeFormat(_intRow, _intCol, _intRow, _intCol, _strFormat);
    }

    /// <summary>
    /// Takes the style information from the cell at (col/row) and merges it with the specified WordWrap.
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_strCol"></param>
    /// <param name="_strFormat">Format of the cell data.</param>
    public void MergeFormat(int _intRow, string _strCol, string _strFormat)
    {
      int intCol = this.flexGrid.Cols[_strCol].Index;
      //Use the cell range method:
      this.MergeFormat(_intRow, intCol, _intRow, intCol, _strFormat);
    }

    /// <summary>
    /// Set the WordWrap to a cell range
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_strFormat">Format of the cell data.</param>
    public void SetFormat(int _intRow1, int _intCol1, int _intRow2, int _intCol2, string _strFormat)
    {
      //Use the "allover" method, and set only the Format (all other fields can have their defaults).
      this.SetStyle(_intRow1, _intCol1, _intRow2, _intCol2, StyleElementFlags.Format,
        COLOR_UNSPECIFIED,
        COLOR_UNSPECIFIED, BORDERDIRECTION_UNSPECIFIED, BORDERSTYLE_UNSPECIFIED, BORDERWIDTH_UNSPECIFIED,
        FONT_UNSPECIFIED, COLOR_UNSPECIFIED, TEXT_ALIGN_UNSPECIFIED, WORD_WRAP_UNSPECIFIED, IMAGE_ALIGN_UNSPECIFIED, TEXT_DIRECTION_UNSPECIFIED, _strFormat);
    }

    /// <summary>
    /// Takes the style information from the cell at (col 1/row 1) and merges it with the specified WordWrap.
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_strFormat">Format of the cell data.</param>
    public void MergeFormat(int _intRow1, int _intCol1, int _intRow2, int _intCol2, string _strFormat)
    {
      //Use common "MergeStyle" method.
      this.MergeStyle(_intRow1, _intCol1, _intRow2, _intCol2, StyleElementFlags.Format,
        COLOR_UNSPECIFIED, COLOR_UNSPECIFIED, BORDERDIRECTION_UNSPECIFIED, BORDERSTYLE_UNSPECIFIED, BORDERWIDTH_UNSPECIFIED,
        FONT_UNSPECIFIED, COLOR_UNSPECIFIED, TEXT_ALIGN_UNSPECIFIED, WORD_WRAP_UNSPECIFIED, IMAGE_ALIGN_UNSPECIFIED, TEXT_DIRECTION_UNSPECIFIED, _strFormat);
    }
    #endregion

    #region Set/Merge all style values
    /// <summary>
    /// Set the style to a CELL RANGE based on a grid style with all or some of the specified parameters.
    /// Which values are set is specified in the "_styleElementFlags" parameter.
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_styleElementFlags">Which elements are set ?
    /// All elements which are not set can have any value.</param>
    /// <param name="_backColor"></param>
    /// <param name="_borderDirection"></param>
    /// <param name="_borderColor"></param>
    /// <param name="_borderStyle"></param>
    /// <param name="_intBorderWidth"></param>
    /// <param name="_font"></param>
    /// <param name="_foreColor"></param>
    /// <param name="_textAlign">Text Align</param>
    /// <param name="_bolWordWrap">TRUE if text can be wrapped.</param>
    /// <param name="_imageAlign">Image Align</param>
    /// <param name="_textDirection">TextDirection</param>
    /// <param name="_format">Format of cell data</param>
    public void SetStyle(int _intRow1, int _intCol1, int _intRow2, int _intCol2, StyleElementFlags _styleElementFlags,
      Color _backColor,
      Color _borderColor, BorderDirEnum _borderDirection, BorderStyleEnum _borderStyle, int _intBorderWidth,
      Font _font, Color _foreColor, TextAlignEnum _textAlign, bool _bolWordWrap, ImageAlignEnum _imageAlign,
      TextDirectionEnum _textDirection, string _format)
    {
      //check params and modify order (loops might fail otherwise)
      if (_intRow1 > _intRow2)
      {
        int iSwap = _intRow1;
        _intRow1 = _intRow2;
        _intRow2 = iSwap;
      }
      if (_intCol1 > _intCol2)
      {
        int iSwap = _intCol1;
        _intCol1 = _intCol2;
        _intCol2 = iSwap;
      }

      //Loop over each cell. Don't take the style of the first cell in range and merge ALL cells,
      //because their styles might differ !
      for (int iIndexRow = _intRow1; iIndexRow <= _intRow2; iIndexRow++)
      {
        for (int iIndexCol = _intCol1; iIndexCol <= _intCol2; iIndexCol++)
        {
          //Hier nicht auf den aktuellen Style der Zelle prüfen, sondern nur auf allgemeine Styles.
          CellStyle cellStyle = this.GetCurrentStyle(iIndexRow, iIndexCol, false);

          this.SetStyle(cellStyle, iIndexRow, iIndexCol, _styleElementFlags, _backColor,
            _borderColor, _borderDirection, _borderStyle, _intBorderWidth,
            _font, _foreColor, _textAlign, _bolWordWrap, _imageAlign, _textDirection, _format);
        }
      }
    }

    /// <summary>
    /// Merges the current style of a CELL RANGE with all or some of the specified parameters.
    /// Which values are set is specified in the "_styleElementFlags" parameter.
    /// </summary>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_intRow2"></param>
    /// <param name="_intCol2"></param>
    /// <param name="_styleElementFlags">Which elements are set ?
    /// All elements which are not set can have any value.</param>
    /// <param name="_backColor"></param>
    /// <param name="_borderDirection"></param>
    /// <param name="_borderColor"></param>
    /// <param name="_borderStyle"></param>
    /// <param name="_intBorderWidth"></param>
    /// <param name="_font"></param>
    /// <param name="_foreColor"></param>
    /// <param name="_textAlign">Text Align </param>
    /// <param name="_bolWordWrap">TRUE if text can be wrapped.</param>
    /// <param name="_imageAlign">Image Align</param>
    /// <param name="_textDirection">Text Direction</param>
    /// <param name="_format">Format of cell data</param>
    public void MergeStyle(int _intRow1, int _intCol1, int _intRow2, int _intCol2, StyleElementFlags _styleElementFlags,
      Color _backColor,
      Color _borderColor, BorderDirEnum _borderDirection, BorderStyleEnum _borderStyle, int _intBorderWidth,
      Font _font, Color _foreColor, TextAlignEnum _textAlign, bool _bolWordWrap, ImageAlignEnum _imageAlign,
      TextDirectionEnum _textDirection, string _format)
    {
      //check params and modify order (loops might fail otherwise)
      if (_intRow1 > _intRow2)
      {
        int iSwap = _intRow1;
        _intRow1 = _intRow2;
        _intRow2 = iSwap;
      }
      if (_intCol1 > _intCol2)
      {
        int iSwap = _intCol1;
        _intCol1 = _intCol2;
        _intCol2 = iSwap;
      }

      //Loop over each cell. Don't take the style of the first cell in range and merge ALL cells,
      //because their styles might differ !
      for (int iIndexRow = _intRow1; iIndexRow <= _intRow2; iIndexRow++)
      {
        for (int iIndexCol = _intCol1; iIndexCol <= _intCol2; iIndexCol++)
        {
          //CellStyle from cell "iIndexRow/iIndexCol":
          //Check for current cellstyle (if one is present)
          CellStyle cellStyleCurrent = this.GetCurrentStyle(iIndexRow, iIndexCol, true);

          //Now set all the relevant values for this new specified base style.
          //Merge it with the already defined elements in the cells current style.
          //But DON'T modify the current style, use "SetXYZ" for each StyleElementFlag.
          #region Modify the values which are set in "_styleElementFlags".

          //CAUTION: If the style already contains changed elements (e.g. Font of FlexGrid is changed),
          //than the defined elements are already modified.
          //So get the current values and change them !


          //WKnauf 29.04.2011:
          //Performance improvement: querying all fields of the style might cost quite a bit of performance. So query only the values
          //of the defined elements! The rest of the values is actually not required later, but we have to assing some value to the variables,
          //so uses the "unspecified" values...

          //The "DefinedElements" property must also be "cached", because some hastable access is done internally.
          StyleElementFlags styleElementFlagsCurrentStyle = cellStyleCurrent.DefinedElements;
          //WKnauf 08.03.2019: Mantis 16294: if this is the fixed Style, reset the "BackColor" Flag!
          //Otherwise, the VisualStyle backcolor will not be applied to this cell.
          if (cellStyleCurrent.Name == this.flexGrid.Styles.Fixed.Name)
          {
            if (styleElementFlagsCurrentStyle.HasFlag(StyleElementFlags.BackColor) == true)
            {
              styleElementFlagsCurrentStyle = styleElementFlagsCurrentStyle & ~StyleElementFlags.BackColor;
            }
            //Same for the "ForeColor"
            if (styleElementFlagsCurrentStyle.HasFlag(StyleElementFlags.ForeColor) == true)
            {
              styleElementFlagsCurrentStyle = styleElementFlagsCurrentStyle & ~StyleElementFlags.ForeColor;
            }
          }
          Color backColorCurrentStyle = (((styleElementFlagsCurrentStyle & StyleElementFlags.BackColor) == StyleElementFlags.BackColor) ? cellStyleCurrent.BackColor : COLOR_UNSPECIFIED);
          Color foreColorCurrentStyle = (((styleElementFlagsCurrentStyle & StyleElementFlags.ForeColor) == StyleElementFlags.ForeColor) ? cellStyleCurrent.ForeColor : COLOR_UNSPECIFIED);
          Font fontCurrentStyle = (((styleElementFlagsCurrentStyle & StyleElementFlags.Font) == StyleElementFlags.Font) ? cellStyleCurrent.Font : FONT_UNSPECIFIED);
          //Für die Border viel Tipparbeit...
          Color borderColorCurrentStyle;
          BorderDirEnum borderDirectionCurrentStyle;
          BorderStyleEnum borderStyleCurrentStyle;
          int intBorderWidthCurrentStyle;
          if ((styleElementFlagsCurrentStyle & StyleElementFlags.Border) == StyleElementFlags.Border)
          {
            CellBorder border = cellStyleCurrent.Border;
            borderColorCurrentStyle = border.Color;
            borderDirectionCurrentStyle = border.Direction;
            borderStyleCurrentStyle = border.Style;
            intBorderWidthCurrentStyle = border.Width;
          }
          else
          {
            borderColorCurrentStyle = COLOR_UNSPECIFIED;
            borderDirectionCurrentStyle = BORDERDIRECTION_UNSPECIFIED;
            borderStyleCurrentStyle = BORDERSTYLE_UNSPECIFIED;
            intBorderWidthCurrentStyle = BORDERWIDTH_UNSPECIFIED;
          }
          TextAlignEnum textAlignCurrentStyle = (((styleElementFlagsCurrentStyle & StyleElementFlags.TextAlign) == StyleElementFlags.TextAlign) ? cellStyleCurrent.TextAlign : TEXT_ALIGN_UNSPECIFIED);
          bool bolWordWrapCurrentStyle = (((styleElementFlagsCurrentStyle & StyleElementFlags.WordWrap) == StyleElementFlags.WordWrap) ? cellStyleCurrent.WordWrap : WORD_WRAP_UNSPECIFIED);
          ImageAlignEnum imageAlignCurrentStyle = (((styleElementFlagsCurrentStyle & StyleElementFlags.ImageAlign) == StyleElementFlags.ImageAlign) ? cellStyleCurrent.ImageAlign : IMAGE_ALIGN_UNSPECIFIED);
          TextDirectionEnum textDirectionCurrentStyle = (styleElementFlagsCurrentStyle.HasFlag(StyleElementFlags.TextDirection) ? cellStyleCurrent.TextDirection : TEXT_DIRECTION_UNSPECIFIED);
          string formatCurrentStyle = (styleElementFlagsCurrentStyle.HasFlag(StyleElementFlags.Format) ? cellStyleCurrent.Format : FORMAT_UNSPECIFIED);

          if ((_styleElementFlags & StyleElementFlags.BackColor) == StyleElementFlags.BackColor && (_styleElementFlags & StyleElementFlags.ForeColor) == StyleElementFlags.ForeColor)
          {
            //Set Back + ForeColor:
            this.SetStyle(cellStyleCurrent, iIndexRow, iIndexCol, StyleElementFlags.BackColor | StyleElementFlags.ForeColor | styleElementFlagsCurrentStyle,
              _backColor,
              borderColorCurrentStyle, borderDirectionCurrentStyle, borderStyleCurrentStyle, intBorderWidthCurrentStyle,
              fontCurrentStyle, _foreColor, textAlignCurrentStyle, bolWordWrapCurrentStyle, imageAlignCurrentStyle, textDirectionCurrentStyle, formatCurrentStyle);
          }
          else
          {

            if ((_styleElementFlags & StyleElementFlags.BackColor) == StyleElementFlags.BackColor)
            {
              //Set BackColor:
              this.SetStyle(cellStyleCurrent, iIndexRow, iIndexCol, StyleElementFlags.BackColor | styleElementFlagsCurrentStyle,
                _backColor,
                borderColorCurrentStyle, borderDirectionCurrentStyle, borderStyleCurrentStyle, intBorderWidthCurrentStyle,
                cellStyleCurrent.Font, cellStyleCurrent.ForeColor, textAlignCurrentStyle, bolWordWrapCurrentStyle, imageAlignCurrentStyle, textDirectionCurrentStyle, formatCurrentStyle);
            }
            if ((_styleElementFlags & StyleElementFlags.ForeColor) == StyleElementFlags.ForeColor)
            {
              //Set ForeColor:
              this.SetStyle(cellStyleCurrent, iIndexRow, iIndexCol, StyleElementFlags.ForeColor | styleElementFlagsCurrentStyle,
                backColorCurrentStyle,
                borderColorCurrentStyle, borderDirectionCurrentStyle, borderStyleCurrentStyle, intBorderWidthCurrentStyle,
                fontCurrentStyle, _foreColor, textAlignCurrentStyle, bolWordWrapCurrentStyle, imageAlignCurrentStyle, textDirectionCurrentStyle, formatCurrentStyle);
            }
          }
          if ((_styleElementFlags & StyleElementFlags.Border) == StyleElementFlags.Border)
          {
            //Border: may be 4 values:
            this.SetStyle(cellStyleCurrent, iIndexRow, iIndexCol, StyleElementFlags.Border | styleElementFlagsCurrentStyle,
              backColorCurrentStyle,
              _borderColor, _borderDirection, _borderStyle, _intBorderWidth,
              fontCurrentStyle, foreColorCurrentStyle, textAlignCurrentStyle, bolWordWrapCurrentStyle, imageAlignCurrentStyle, textDirectionCurrentStyle, formatCurrentStyle);
          }
          if ((_styleElementFlags & StyleElementFlags.Font) == StyleElementFlags.Font)
          {
            //Set font:
            this.SetStyle(cellStyleCurrent, iIndexRow, iIndexCol, StyleElementFlags.Font | styleElementFlagsCurrentStyle,
              backColorCurrentStyle,
              borderColorCurrentStyle, borderDirectionCurrentStyle, borderStyleCurrentStyle, intBorderWidthCurrentStyle,
              _font, foreColorCurrentStyle, textAlignCurrentStyle, bolWordWrapCurrentStyle, imageAlignCurrentStyle, textDirectionCurrentStyle, formatCurrentStyle);
          }

          if ((_styleElementFlags & StyleElementFlags.TextAlign) == StyleElementFlags.TextAlign)
          {
            //Set TextAlign:
            this.SetStyle(cellStyleCurrent, iIndexRow, iIndexCol, StyleElementFlags.TextAlign | styleElementFlagsCurrentStyle,
              backColorCurrentStyle,
              borderColorCurrentStyle, borderDirectionCurrentStyle, borderStyleCurrentStyle, intBorderWidthCurrentStyle,
              fontCurrentStyle, foreColorCurrentStyle, _textAlign, bolWordWrapCurrentStyle, imageAlignCurrentStyle, textDirectionCurrentStyle, formatCurrentStyle);
          }
          if ((_styleElementFlags & StyleElementFlags.WordWrap) == StyleElementFlags.WordWrap)
          {
            //Set WordWrap:
            this.SetStyle(cellStyleCurrent, iIndexRow, iIndexCol, StyleElementFlags.WordWrap | styleElementFlagsCurrentStyle,
              backColorCurrentStyle,
              borderColorCurrentStyle, borderDirectionCurrentStyle, borderStyleCurrentStyle, intBorderWidthCurrentStyle,
              fontCurrentStyle, foreColorCurrentStyle, textAlignCurrentStyle, _bolWordWrap, imageAlignCurrentStyle, textDirectionCurrentStyle, formatCurrentStyle);
          }
          if ((_styleElementFlags & StyleElementFlags.ImageAlign) == StyleElementFlags.ImageAlign)
          {
            //Set ImageAlign:
            this.SetStyle(cellStyleCurrent, iIndexRow, iIndexCol, StyleElementFlags.ImageAlign | styleElementFlagsCurrentStyle,
              backColorCurrentStyle,
              borderColorCurrentStyle, borderDirectionCurrentStyle, borderStyleCurrentStyle, intBorderWidthCurrentStyle,
              fontCurrentStyle, foreColorCurrentStyle, textAlignCurrentStyle, bolWordWrapCurrentStyle, _imageAlign, textDirectionCurrentStyle, formatCurrentStyle);
          }
          if (_styleElementFlags.HasFlag(StyleElementFlags.TextDirection) == true)
          {
            //Set TextDirection:
            this.SetStyle(cellStyleCurrent, iIndexRow, iIndexCol, StyleElementFlags.TextDirection | styleElementFlagsCurrentStyle,
              backColorCurrentStyle,
              borderColorCurrentStyle, borderDirectionCurrentStyle, borderStyleCurrentStyle, intBorderWidthCurrentStyle,
              fontCurrentStyle, foreColorCurrentStyle, textAlignCurrentStyle, bolWordWrapCurrentStyle, imageAlignCurrentStyle, _textDirection, formatCurrentStyle);
          }
          if (_styleElementFlags.HasFlag(StyleElementFlags.Format) == true)
          {
            //Set Format:
            this.SetStyle(cellStyleCurrent, iIndexRow, iIndexCol, StyleElementFlags.Format | styleElementFlagsCurrentStyle,
              backColorCurrentStyle,
              borderColorCurrentStyle, borderDirectionCurrentStyle, borderStyleCurrentStyle, intBorderWidthCurrentStyle,
              fontCurrentStyle, foreColorCurrentStyle, textAlignCurrentStyle, bolWordWrapCurrentStyle, imageAlignCurrentStyle, textDirectionCurrentStyle, _format);
          }
          #endregion
        } //End FOR over cols
      } //End FOR over rows
    }
    #endregion
    #endregion

    #region Private Methods
    /// <summary>
    /// Set the style to a single CELL based on a grid style with all or some of the specified parameters.
    /// Which values are set is specified in the "_styleElementFlags" parameter.
    /// </summary>
    /// <param name="_cellStyleBasedOn">The base cell style which is modified (new style is maybe created).</param>
    /// <param name="_intRow1"></param>
    /// <param name="_intCol1"></param>
    /// <param name="_styleElementFlags">Which elements are set ?
    /// All elements which are not set can have any value.</param>
    /// <param name="_backColor"></param>
    /// <param name="_borderDirection"></param>
    /// <param name="_borderColor"></param>
    /// <param name="_borderStyle"></param>
    /// <param name="_intBorderWidth"></param>
    /// <param name="_font"></param>
    /// <param name="_foreColor"></param>
    /// <param name="_textAlign">Text Align</param>
    /// <param name="_bolWordWrap">TRUE if text can be wrapped.</param>
    /// <param name="_imageAlign">Image Align</param>
    /// <param name="_textDirection">Text Direction</param>
    /// <param name="_format">Format of cell data</param>
    private void SetStyle(CellStyle _cellStyleBasedOn, int _intRow1, int _intCol1, StyleElementFlags _styleElementFlags,
      Color _backColor,
      Color _borderColor, BorderDirEnum _borderDirection, BorderStyleEnum _borderStyle, int _intBorderWidth,
      Font _font, Color _foreColor, TextAlignEnum _textAlign, bool _bolWordWrap, ImageAlignEnum _imageAlign,
      TextDirectionEnum _textDirection, string _format)
    {
      //Create the style name:
      string strStyleName = this.GetStyleString(_styleElementFlags, _backColor,
        _borderColor, _borderDirection, _borderStyle, _intBorderWidth,
        _font, _foreColor, _textAlign, _bolWordWrap, _imageAlign, _textDirection, _format);


      //Is style contained in grid ?
      //Use the mapping hashtable, because the grids cellstyle collection is not a hashtable !
      CellStyle cellStyleNew;
      //WKnauf 29.04.2011: Use "TryGetValue" to reduce the number of string comparisons.
      int intIndexStyle;

      //if (this.htStylesIndexInGrid.ContainsKey(strStyleName) == false)
      //if (this.flexGrid.Styles.Contains (strStyleName) == false)
      if (this.htStylesIndexInGrid.TryGetValue(strStyleName, out intIndexStyle) == false)
      {
        //Create new style. Use the style string as "name".
        cellStyleNew = this.flexGrid.Styles.Add(strStyleName, _cellStyleBasedOn);
        //Add to Hashtable:
        this.htStylesIndexInGrid.Add(strStyleName, this.flexGrid.Styles.Count - 1); //cellStyleNew);

        //Apply the "StyleElementFlags" (required if elements are reset - see "ResetBackColor"
        cellStyleNew.DefinedElements = _styleElementFlags;

        #region Modify the values which are set in "_styleElementFlags":
        if ((_styleElementFlags & StyleElementFlags.BackColor) == StyleElementFlags.BackColor)
          cellStyleNew.BackColor = _backColor;
        if ((_styleElementFlags & StyleElementFlags.Border) == StyleElementFlags.Border)
        {
          //Border: may be 4 values:
          cellStyleNew.Border.Color = _borderColor;
          cellStyleNew.Border.Direction = _borderDirection;
          cellStyleNew.Border.Style = _borderStyle;
          cellStyleNew.Border.Width = _intBorderWidth;
        }
        if ((_styleElementFlags & StyleElementFlags.Font) == StyleElementFlags.Font)
          cellStyleNew.Font = _font;
        if ((_styleElementFlags & StyleElementFlags.ForeColor) == StyleElementFlags.ForeColor)
          cellStyleNew.ForeColor = _foreColor;
        if ((_styleElementFlags & StyleElementFlags.TextAlign) == StyleElementFlags.TextAlign)
          cellStyleNew.TextAlign = _textAlign;
        if ((_styleElementFlags & StyleElementFlags.WordWrap) == StyleElementFlags.WordWrap)
          cellStyleNew.WordWrap = _bolWordWrap;
        if ((_styleElementFlags & StyleElementFlags.ImageAlign) == StyleElementFlags.ImageAlign)
          cellStyleNew.ImageAlign = _imageAlign;
        if (_styleElementFlags.HasFlag(StyleElementFlags.TextDirection) == true)
          cellStyleNew.TextDirection = _textDirection;
        if (_styleElementFlags.HasFlag(StyleElementFlags.Format) == true)
          cellStyleNew.Format = _format;
        #endregion
      }
      else
      {
        //Get Style from grid. Once again don't use the indexer with a string name.
        //The hashtable contains the index in the cellstyle collection for this style.

        //Is this index valid ?
        if (intIndexStyle < 0 || intIndexStyle >= this.flexGrid.Styles.Count)
          throw new InvalidOperationException("CellStyle-Index for Style with name '" + strStyleName + "' is " + intIndexStyle + ", Grid-Stylecollection contains " + this.flexGrid.Styles.Count);
        cellStyleNew = this.flexGrid.Styles[intIndexStyle];
        //Check that the cell style name matches the name we are searching !
        if (cellStyleNew.Name != strStyleName)
          throw new InvalidOperationException("CellStyle-Collection has changed: Style with name '" + strStyleName + "' should be at index " + intIndexStyle + ", we found " + cellStyleNew.Name);
      }

      //set to cell.
      this.flexGrid.SetCellStyle(_intRow1, _intCol1, cellStyleNew);
    }


    /// <summary>
    /// Create a style name for a style who has the specified values set.
    /// </summary>
    /// <param name="_styleElementFlags">Defined elements. All others are not honoured!</param>
    /// <param name="_backColor"></param>
    /// <param name="_borderDirection"></param>
    /// <param name="_borderColor"></param>
    /// <param name="_borderStyle"></param>
    /// <param name="_intBorderWidth"></param>
    /// <param name="_font"></param>
    /// <param name="_foreColor"></param>
    /// <param name="_textAlign">Text Align</param>
    /// <param name="_bolWordWrap">TRUE if text can be wrapped.</param>
    /// <param name="_imageAlign">Image Align</param>
    /// <param name="_textDirection">Text Direction</param>
    /// <param name="_format">Data format</param>
    /// <returns></returns>
    protected string GetStyleString(StyleElementFlags _styleElementFlags,
      Color _backColor,
      Color _borderColor, BorderDirEnum _borderDirection, BorderStyleEnum _borderStyle, int _intBorderWidth,
      Font _font, Color _foreColor, TextAlignEnum _textAlign, bool _bolWordWrap, ImageAlignEnum _imageAlign,
      TextDirectionEnum _textDirection, string _format)
    {
      StringBuilder sb = new StringBuilder(100);
      //First use integer value of the defined fields:
      sb.Append(((int)_styleElementFlags).ToString());

      //Now add all the defined fields. 
      //A style with defined BackColor has the same string as a style with defined ForeColor, but the "Defined elements enum value" is different.
      //Pick all values from the default style which are not to be set !
      if ((_styleElementFlags & StyleElementFlags.BackColor) == StyleElementFlags.BackColor)
      {
        sb.Append(STYLENAME_SEPARATOR);
        C1FlexGridStyleHandler.ColorToStringBuilder(_backColor, sb);
      }
      if ((_styleElementFlags & StyleElementFlags.Border) == StyleElementFlags.Border)
      {
        //Border: may be 4 values:
        sb.Append(STYLENAME_SEPARATOR);
        C1FlexGridStyleHandler.ColorToStringBuilder(_borderColor, sb);
        sb.Append(STYLENAME_SEPARATOR);
        //sb.Append (_borderDirection.ToString() );
        sb.Append(((int)_borderDirection).ToString());
        sb.Append(STYLENAME_SEPARATOR);
        //sb.Append (_borderStyle.ToString() );
        sb.Append(((int)_borderStyle).ToString());
        sb.Append(STYLENAME_SEPARATOR);
        sb.Append(_intBorderWidth.ToString());
      }
      if ((_styleElementFlags & StyleElementFlags.Font) == StyleElementFlags.Font)
      {
        //WKnauf 02.10.2006: Don't use "ToString" because "Bold" flag is ignored ! Use custom method !
        // //"font.ToString()" is half a second in a 200*200 grid...
        //sb.Append (_font.ToString() );
        sb.Append(STYLENAME_SEPARATOR);
        C1FlexGridStyleHandler.FontToStringBuilder(_font, sb);
      }
      if ((_styleElementFlags & StyleElementFlags.ForeColor) == StyleElementFlags.ForeColor)
      {
        sb.Append(STYLENAME_SEPARATOR);
        C1FlexGridStyleHandler.ColorToStringBuilder(_foreColor, sb);
      }
      if ((_styleElementFlags & StyleElementFlags.TextAlign) == StyleElementFlags.TextAlign)
      {
        sb.Append(STYLENAME_SEPARATOR);
        sb.Append(((int)_textAlign).ToString());
      }
      if ((_styleElementFlags & StyleElementFlags.WordWrap) == StyleElementFlags.WordWrap)
      {
        sb.Append(STYLENAME_SEPARATOR);
        sb.Append(_bolWordWrap.ToString());
      }
      if ((_styleElementFlags & StyleElementFlags.ImageAlign) == StyleElementFlags.ImageAlign)
      {
        sb.Append(STYLENAME_SEPARATOR);
        sb.Append(((int)_imageAlign).ToString());
      }
      if (_styleElementFlags.HasFlag(StyleElementFlags.TextDirection) == true)
      {
        sb.Append(STYLENAME_SEPARATOR);
        sb.Append(((int)_textDirection).ToString());
      }
      if (_styleElementFlags.HasFlag(StyleElementFlags.Format) == true)
      {
        sb.Append(STYLENAME_SEPARATOR);
        sb.Append(_format);
      }

      return sb.ToString();
    }

    /// <summary>
    /// Get current style for a cell.
    /// This is:
    /// -current "GetCellStyle" if available
    /// -or fixed style (if fixed cell)
    /// -or frozen style (if frozen cell)
    /// -or normal style
    /// </summary>
    /// <param name="_intRow"></param>
    /// <param name="_intCol"></param>
    /// <param name="_bolCheckForCellStyle">TRUE: zuerst prüfen, ob in der Zelle schon ein Style steckt (beim Merge nötig).
    /// FALSE: nur auf grundlegende Styles prüfen (Fixed, Frozen, Normal), aber nicht auf in Zelle vorhandenen Style (beim Set nötig)</param>
    /// <returns>CellStyle (never NULL)</returns>
    private CellStyle GetCurrentStyle(int _intRow, int _intCol, bool _bolCheckForCellStyle)
    {
      //CellStyle from cell "iIndexRow/iIndexCol":
      CellStyle cellStyleCurrent = (_bolCheckForCellStyle == true ? this.flexGrid.GetCellStyle(_intRow, _intCol) : null);

      //If style is null thake the specified base style.
      if (cellStyleCurrent == null)
      {
        //WKnauf 29.01.2009: Choose the style depending on row/col!
        //if (_intRow < this.flexGrid.Rows.Fixed || _intCol < this.flexGrid.Cols.Fixed)
        if (this.IsFixed(_intRow, _intCol) == true)
        {
          //Row OR Col is in fixed area:
          cellStyleCurrent = this.flexGrid.Styles.Fixed;
        }
        //else if ((_intRow >= this.flexGrid.Rows.Fixed && _intRow < (this.flexGrid.Rows.Fixed + this.flexGrid.Rows.Frozen)) ||
        //  (_intCol >= this.flexGrid.Cols.Fixed && _intCol < (this.flexGrid.Cols.Fixed + this.flexGrid.Cols.Frozen)))
        else if (this.IsFrozen(_intRow, _intCol) == true)
        {
          //Row OR Col is not fixed, but in frozen area!
          //(colum range starts with "FIXED" and ends at "FIXED + FROZEN - 1"
          cellStyleCurrent = this.flexGrid.Styles.Frozen;
        }
        else
        {
          //Any other row/col: use normal style.
          //WKnauf 13.09.2013: check for alternate style: is this enabled?
          if (this.flexGrid.Styles.Alternate.BackColor != this.flexGrid.Styles.Normal.BackColor)
          {
            // //First non-fixed row has ALWAYS the alternate style!
            //int intRelativeRow = _intRow - (this.flexGrid.Rows.Fixed + this.flexGrid.Rows.Frozen);
            // //All even rows have alternate color.
            //if ((intRelativeRow % 2) == 0)
            if (this.IsAlternate(_intRow) == true)
            {
              cellStyleCurrent = this.flexGrid.Styles.Alternate;
            }
            else
            {
              cellStyleCurrent = this.flexGrid.Styles.Normal;
            }
          }
          else
          {
            cellStyleCurrent = this.flexGrid.Styles.Normal;
          }
        } //End if/else fixed/frozen/normal
      }

      return cellStyleCurrent;
    }


    /// <summary>
    /// Is the current cell a fixed row/col?
    /// </summary>
    /// <param name="_row">Grid row</param>
    /// <param name="_col">Grid col</param>
    /// <returns></returns>
    private bool IsFixed(int _row, int _col)
    {
      return _row < this.flexGrid.Rows.Fixed || _col < this.flexGrid.Cols.Fixed;
    }

    /// <summary>
    /// Is the current cell a fixed row/col?
    /// </summary>
    /// <param name="_intRow">Grid row</param>
    /// <param name="_intCol">Grid col</param>
    /// <returns></returns>
    private bool IsFrozen(int _intRow, int _intCol)
    {
      return (_intRow >= this.flexGrid.Rows.Fixed && _intRow < (this.flexGrid.Rows.Fixed + this.flexGrid.Rows.Frozen)) ||
          (_intCol >= this.flexGrid.Cols.Fixed && _intCol < (this.flexGrid.Cols.Fixed + this.flexGrid.Cols.Frozen));
    }

    /// <summary>
    /// Is the current cell a alternative row?
    /// </summary>
    /// <param name="_intRow">Grid row</param>
    /// <returns></returns>
    private bool IsAlternate(int _intRow)
    {
      //First non-fixed row has ALWAYS the alternate style!
      int intRelativeRow = _intRow - (this.flexGrid.Rows.Fixed + this.flexGrid.Rows.Frozen);
      //All even rows have alternate color.
      return ((intRelativeRow % 2) == 0);
    }
    #endregion

    #region Private static Helper
    /// <summary>
    /// Serialize a color value to a StringBuilder.
    /// </summary>
    /// <param name="_color"></param>
    /// <param name="_stringBuilder"></param>
    private static void ColorToStringBuilder(Color _color, StringBuilder _stringBuilder)
    {
      //In a grid with 200*200 rows (each row a different color) "color.ToString()" lasts about half
      //a second longer than adding the RGB values. Reason seems to be that "ToString" searches for known colors.
      //_stringBuilder.Append (_color.ToString() );

      //Add the alpha value only if not default. This saves 0,2 seconds in the 200*200 grid ;-).
      if (_color.A != 255)
      {
        _stringBuilder.Append(_color.A);
        _stringBuilder.Append(",");
      }
      _stringBuilder.Append(_color.R);
      _stringBuilder.Append(",");
      _stringBuilder.Append(_color.G);
      _stringBuilder.Append(",");
      _stringBuilder.Append(_color.B);
    }

    /// <summary>
    /// Serialize a font value to a StringBuilder.
    /// </summary>
    /// <param name="_font"></param>
    /// <param name="_stringBuilder"></param>
    private static void FontToStringBuilder(Font _font, StringBuilder _stringBuilder)
    {
      //Font.ToString() is not good because the "Bold" flag is ignored...

      _stringBuilder.Append(_font.Name);
      
      //Font-Size in eckige Klammern kleben da dies eine Kommazahl sein könnte (mit Komma gemäß deutschen Format).
      _stringBuilder.Append(",[");
      _stringBuilder.Append(_font.Size.ToString());
      _stringBuilder.Append("],");
      //Den Font-Style auf einen Int casten, denn wir brauchen hier nicht die Enum-Werte (für die würde ein Lookup wohl sowieso wehtun)
      _stringBuilder.Append(((int)_font.Style).ToString());
    }

    #endregion


  }
}
