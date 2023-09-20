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
      panelDates = new Panel();
      dateTimePickerTo = new DateTimePicker();
      dateTimePickerFrom = new DateTimePicker();
      labelTo = new Label();
      labelFrom = new Label();
      ((System.ComponentModel.ISupportInitialize)this.c1FlexGrid1).BeginInit();
      panelBottom.SuspendLayout();
      panelDates.SuspendLayout();
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
      this.c1FlexGrid1.Size = new Size(1014, 326);
      this.c1FlexGrid1.StyleInfo = resources.GetString("c1FlexGrid1.StyleInfo");
      this.c1FlexGrid1.TabIndex = 1;
      // 
      // panelBottom
      // 
      panelBottom.Controls.Add(buttonShowSelection);
      panelBottom.Dock = DockStyle.Bottom;
      panelBottom.Location = new Point(0, 388);
      panelBottom.Name = "panelBottom";
      panelBottom.Size = new Size(1014, 62);
      panelBottom.TabIndex = 2;
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
      // panelDates
      // 
      panelDates.Controls.Add(dateTimePickerTo);
      panelDates.Controls.Add(dateTimePickerFrom);
      panelDates.Controls.Add(labelTo);
      panelDates.Controls.Add(labelFrom);
      panelDates.Dock = DockStyle.Top;
      panelDates.Location = new Point(0, 0);
      panelDates.Name = "panelDates";
      panelDates.Size = new Size(1014, 62);
      panelDates.TabIndex = 0;
      // 
      // dateTimePickerTo
      // 
      dateTimePickerTo.CustomFormat = "MMM yyyy";
      dateTimePickerTo.Format = DateTimePickerFormat.Custom;
      dateTimePickerTo.Location = new Point(267, 17);
      dateTimePickerTo.Name = "dateTimePickerTo";
      dateTimePickerTo.Size = new Size(105, 23);
      dateTimePickerTo.TabIndex = 3;
      dateTimePickerTo.ValueChanged += this.dateTimePickerTo_ValueChanged;
      // 
      // dateTimePickerFrom
      // 
      dateTimePickerFrom.CustomFormat = "MMM yyyy";
      dateTimePickerFrom.Format = DateTimePickerFormat.Custom;
      dateTimePickerFrom.Location = new Point(79, 17);
      dateTimePickerFrom.Name = "dateTimePickerFrom";
      dateTimePickerFrom.Size = new Size(105, 23);
      dateTimePickerFrom.TabIndex = 1;
      dateTimePickerFrom.ValueChanged += this.dateTimePickerFrom_ValueChanged;
      // 
      // labelTo
      // 
      labelTo.AutoSize = true;
      labelTo.Location = new Point(213, 23);
      labelTo.Name = "labelTo";
      labelTo.Size = new Size(22, 15);
      labelTo.TabIndex = 2;
      labelTo.Text = "To:";
      // 
      // labelFrom
      // 
      labelFrom.AutoSize = true;
      labelFrom.Location = new Point(25, 23);
      labelFrom.Name = "labelFrom";
      labelFrom.Size = new Size(38, 15);
      labelFrom.TabIndex = 0;
      labelFrom.Text = "From:";
      // 
      // Form
      // 
      this.AutoScaleDimensions = new SizeF(7F, 15F);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(1014, 450);
      this.Controls.Add(this.c1FlexGrid1);
      this.Controls.Add(panelDates);
      this.Controls.Add(panelBottom);
      this.Name = "Form";
      this.Text = "Form1";
      ((System.ComponentModel.ISupportInitialize)this.c1FlexGrid1).EndInit();
      panelBottom.ResumeLayout(false);
      panelDates.ResumeLayout(false);
      panelDates.PerformLayout();
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