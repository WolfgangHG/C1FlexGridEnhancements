using System;
using System.Windows.Forms;

namespace C1FlexGridStyleHandler
{
  internal static class Program
  {
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
#if NET8_0_OR_GREATER
      Application.SetHighDpiMode(HighDpiMode.SystemAware);
#endif
      Application.Run(new Form());
    }
  }
}