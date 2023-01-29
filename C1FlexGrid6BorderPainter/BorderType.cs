using System;

namespace C1FlexGrid6BorderPainter
{
  /// <summary>
  /// This enumeration describes which borders should be painted.
  /// The values might be ORed
  /// 
  /// </summary>
  [Flags()]
  public enum BorderType
  {
    /// <summary>
    /// No border
    /// </summary>
    None = 0,
    /// <summary>
    /// upper border
    /// </summary>
    Top = 1,
    /// <summary>
    /// right border
    /// </summary>
    Right = 2,
    /// <summary>
    /// lower border
    /// </summary>
    Bottom = 4,
    /// <summary>
    /// left border
    /// </summary>
    Left = 8
  }
}
