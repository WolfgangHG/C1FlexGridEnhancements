using System.Drawing;
using System.Windows.Forms;

namespace C1FlexGrid6CalendarSheet
{
  partial class Form
  {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form));
      this.c1FlexGrid1 = new C1FlexGridCalendar();
      panelBottom = new Panel();
      buttonShowSelection = new Button();
      ((System.ComponentModel.ISupportInitialize)this.c1FlexGrid1).BeginInit();
      panelBottom.SuspendLayout();
      this.SuspendLayout();
      // 
      // c1FlexGrid1
      // 
      this.c1FlexGrid1.ColumnInfo = "10,1,0,0,0,-1,Columns:";
      this.c1FlexGrid1.Dock = DockStyle.Fill;
      this.c1FlexGrid1.Location = new Point(0, 0);
      this.c1FlexGrid1.Name = "c1FlexGrid1";
      this.c1FlexGrid1.Size = new Size(884, 388);
      this.c1FlexGrid1.StyleInfo = resources.GetString("c1FlexGrid1.StyleInfo");
      this.c1FlexGrid1.TabIndex = 0;
      // 
      // panelBottom
      // 
      panelBottom.Controls.Add(buttonShowSelection);
      panelBottom.Dock = DockStyle.Bottom;
      panelBottom.Location = new Point(0, 388);
      panelBottom.Name = "panelBottom";
      panelBottom.Size = new Size(884, 62);
      panelBottom.TabIndex = 1;
      // 
      // buttonShowSelection
      // 
      buttonShowSelection.Location = new Point(762, 15);
      buttonShowSelection.Name = "buttonShowSelection";
      buttonShowSelection.Size = new Size(110, 23);
      buttonShowSelection.TabIndex = 0;
      buttonShowSelection.Text = "Show selection";
      buttonShowSelection.UseVisualStyleBackColor = true;
      buttonShowSelection.Click += this.buttonShowSelection_Click;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new SizeF(7F, 15F);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(884, 450);
      this.Controls.Add(this.c1FlexGrid1);
      this.Controls.Add(panelBottom);
      this.Name = "Form1";
      this.Text = "Form1";
      ((System.ComponentModel.ISupportInitialize)this.c1FlexGrid1).EndInit();
      panelBottom.ResumeLayout(false);
      this.ResumeLayout(false);
    }

    #endregion

    private C1FlexGridCalendar c1FlexGrid1;
    private Panel panelBottom;
    private Button buttonShowSelection;
  }
}