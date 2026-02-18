using System.Drawing;
using System.Windows.Forms;

namespace C1FlexGridCalendarSheet
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
      this.panelBottom = new Panel();
      this.buttonShowSelection = new Button();
      this.panelDates = new Panel();
      this.dateTimePickerTo = new DateTimePicker();
      this.dateTimePickerFrom = new DateTimePicker();
      this.labelTo = new Label();
      this.labelFrom = new Label();
      ((System.ComponentModel.ISupportInitialize)this.c1FlexGrid1).BeginInit();
      this.panelBottom.SuspendLayout();
      this.panelDates.SuspendLayout();
      this.SuspendLayout();
      // 
      // c1FlexGrid1
      // 
      this.c1FlexGrid1.AllowEditing = false;
      this.c1FlexGrid1.AllowMergingFixed = C1.Win.FlexGrid.AllowMergingEnum.FixedOnly;
      this.c1FlexGrid1.AllowSorting = C1.Win.FlexGrid.AllowSortingEnum.None;
      this.c1FlexGrid1.ColumnInfo = "2,2,0,0,0,-1,Columns:0{Width:40;Name:\"ColumnYear\";Caption:\"Year\";AllowMerging:True;}\t1{Name:\"ColumMonth\";Caption:\"Month\";Style:\"Font:Microsoft Sans Serif, 8.25pt;\";}\t";
      this.c1FlexGrid1.Dock = DockStyle.Fill;
      this.c1FlexGrid1.Location = new Point(0, 62);
      this.c1FlexGrid1.Name = "c1FlexGrid1";
      this.c1FlexGrid1.SelectionMode = C1.Win.FlexGrid.SelectionModeEnum.CellRange;
      this.c1FlexGrid1.Size = new Size(1014, 326);
      this.c1FlexGrid1.StyleInfo = resources.GetString("c1FlexGrid1.StyleInfo");
      this.c1FlexGrid1.TabIndex = 1;
      // 
      // panelBottom
      // 
      this.panelBottom.Controls.Add(this.buttonShowSelection);
      this.panelBottom.Dock = DockStyle.Bottom;
      this.panelBottom.Location = new Point(0, 388);
      this.panelBottom.Name = "panelBottom";
      this.panelBottom.Size = new Size(1014, 62);
      this.panelBottom.TabIndex = 2;
      // 
      // buttonShowSelection
      // 
      this.buttonShowSelection.Location = new Point(762, 15);
      this.buttonShowSelection.Name = "buttonShowSelection";
      this.buttonShowSelection.Size = new Size(110, 23);
      this.buttonShowSelection.TabIndex = 0;
      this.buttonShowSelection.Text = "Show selection";
      this.buttonShowSelection.UseVisualStyleBackColor = true;
      this.buttonShowSelection.Click += this.buttonShowSelection_Click;
      // 
      // panelDates
      // 
      this.panelDates.Controls.Add(this.dateTimePickerTo);
      this.panelDates.Controls.Add(this.dateTimePickerFrom);
      this.panelDates.Controls.Add(this.labelTo);
      this.panelDates.Controls.Add(this.labelFrom);
      this.panelDates.Dock = DockStyle.Top;
      this.panelDates.Location = new Point(0, 0);
      this.panelDates.Name = "panelDates";
      this.panelDates.Size = new Size(1014, 62);
      this.panelDates.TabIndex = 0;
      // 
      // dateTimePickerTo
      // 
      this.dateTimePickerTo.CustomFormat = "MMM yyyy";
      this.dateTimePickerTo.Format = DateTimePickerFormat.Custom;
      this.dateTimePickerTo.Location = new Point(267, 17);
      this.dateTimePickerTo.Name = "dateTimePickerTo";
      this.dateTimePickerTo.Size = new Size(105, 23);
      this.dateTimePickerTo.TabIndex = 3;
      this.dateTimePickerTo.ValueChanged += this.dateTimePickerTo_ValueChanged;
      // 
      // dateTimePickerFrom
      // 
      this.dateTimePickerFrom.CustomFormat = "MMM yyyy";
      this.dateTimePickerFrom.Format = DateTimePickerFormat.Custom;
      this.dateTimePickerFrom.Location = new Point(79, 17);
      this.dateTimePickerFrom.Name = "dateTimePickerFrom";
      this.dateTimePickerFrom.Size = new Size(105, 23);
      this.dateTimePickerFrom.TabIndex = 1;
      this.dateTimePickerFrom.ValueChanged += this.dateTimePickerFrom_ValueChanged;
      // 
      // labelTo
      // 
      this.labelTo.AutoSize = true;
      this.labelTo.Location = new Point(213, 23);
      this.labelTo.Name = "labelTo";
      this.labelTo.Size = new Size(23, 15);
      this.labelTo.TabIndex = 2;
      this.labelTo.Text = "To:";
      // 
      // labelFrom
      // 
      this.labelFrom.AutoSize = true;
      this.labelFrom.Location = new Point(25, 23);
      this.labelFrom.Name = "labelFrom";
      this.labelFrom.Size = new Size(38, 15);
      this.labelFrom.TabIndex = 0;
      this.labelFrom.Text = "From:";
      // 
      // Form
      // 
      this.AutoScaleDimensions = new SizeF(7F, 15F);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(1014, 450);
      this.Controls.Add(this.c1FlexGrid1);
      this.Controls.Add(this.panelDates);
      this.Controls.Add(this.panelBottom);
      this.Name = "Form";
      this.Text = "Form1";
      ((System.ComponentModel.ISupportInitialize)this.c1FlexGrid1).EndInit();
      this.panelBottom.ResumeLayout(false);
      this.panelDates.ResumeLayout(false);
      this.panelDates.PerformLayout();
      this.ResumeLayout(false);
    }

    #endregion

    private C1FlexGridCalendar c1FlexGrid1;
    private Panel panelBottom;
    private Button buttonShowSelection;
    private Panel panelDates;
    private DateTimePicker dateTimePickerTo;
    private DateTimePicker dateTimePickerFrom;
    private Label labelTo;
    private Label labelFrom;
  }
}