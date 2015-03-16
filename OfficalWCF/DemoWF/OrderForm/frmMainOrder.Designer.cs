namespace DemoWF.OrderForm
{
    partial class frmMainOrder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.Search = new System.Windows.Forms.GroupBox();
            this.buttonX3 = new DevComponents.DotNetBar.ButtonX();
            this.btnDelete = new DevComponents.DotNetBar.ButtonX();
            this.btnAdd = new DevComponents.DotNetBar.ButtonX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.txtFreight = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtDateShipped = new DevComponents.Editors.DateTimeAdv.DateTimeInput();
            this.txtDateRequired = new DevComponents.Editors.DateTimeAdv.DateTimeInput();
            this.dtgOrder = new System.Windows.Forms.DataGridView();
            this.orderIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.customerIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.orderDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.requireDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.shippedDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.shipViaDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.freightDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DeleteComlumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.orderBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.Search.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDateShipped)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDateRequired)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtgOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.orderBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // Search
            // 
            this.Search.Controls.Add(this.buttonX3);
            this.Search.Controls.Add(this.btnDelete);
            this.Search.Controls.Add(this.btnAdd);
            this.Search.Controls.Add(this.labelX3);
            this.Search.Controls.Add(this.labelX2);
            this.Search.Controls.Add(this.labelX1);
            this.Search.Controls.Add(this.txtFreight);
            this.Search.Controls.Add(this.txtDateShipped);
            this.Search.Controls.Add(this.txtDateRequired);
            this.Search.Dock = System.Windows.Forms.DockStyle.Top;
            this.Search.Location = new System.Drawing.Point(0, 0);
            this.Search.Name = "Search";
            this.Search.Size = new System.Drawing.Size(760, 127);
            this.Search.TabIndex = 0;
            this.Search.TabStop = false;
            //this.Search.Enter += new System.EventHandler(this.Search_Enter);
            // 
            // buttonX3
            // 
            this.buttonX3.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX3.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonX3.Location = new System.Drawing.Point(313, 73);
            this.buttonX3.Name = "buttonX3";
            this.buttonX3.Size = new System.Drawing.Size(85, 29);
            this.buttonX3.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonX3.TabIndex = 8;
            this.buttonX3.Text = "Search";
            this.buttonX3.Click += new System.EventHandler(this.buttonX3_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnDelete.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnDelete.Location = new System.Drawing.Point(189, 73);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(85, 29);
            this.btnDelete.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnDelete.TabIndex = 7;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnAdd.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnAdd.Location = new System.Drawing.Point(66, 73);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(85, 29);
            this.btnAdd.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // labelX3
            // 
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Location = new System.Drawing.Point(235, 16);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(52, 23);
            this.labelX3.TabIndex = 5;
            this.labelX3.Text = "Shipped";
            // 
            // labelX2
            // 
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(464, 16);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(52, 23);
            this.labelX2.TabIndex = 4;
            this.labelX2.Text = "Freight";
            // 
            // labelX1
            // 
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(8, 16);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(52, 23);
            this.labelX1.TabIndex = 3;
            this.labelX1.Text = "Required";
            // 
            // txtFreight
            // 
            // 
            // 
            // 
            this.txtFreight.Border.Class = "TextBoxBorder";
            this.txtFreight.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtFreight.Location = new System.Drawing.Point(522, 19);
            this.txtFreight.Name = "txtFreight";
            this.txtFreight.Size = new System.Drawing.Size(121, 20);
            this.txtFreight.TabIndex = 2;
            // 
            // txtDateShipped
            // 
            // 
            // 
            // 
            this.txtDateShipped.BackgroundStyle.Class = "DateTimeInputBackground";
            this.txtDateShipped.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtDateShipped.ButtonDropDown.Shortcut = DevComponents.DotNetBar.eShortcut.AltDown;
            this.txtDateShipped.ButtonDropDown.Visible = true;
            this.txtDateShipped.IsPopupCalendarOpen = false;
            this.txtDateShipped.Location = new System.Drawing.Point(293, 19);
            // 
            // 
            // 
            this.txtDateShipped.MonthCalendar.AnnuallyMarkedDates = new System.DateTime[0];
            // 
            // 
            // 
            this.txtDateShipped.MonthCalendar.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtDateShipped.MonthCalendar.CalendarDimensions = new System.Drawing.Size(1, 1);
            this.txtDateShipped.MonthCalendar.ClearButtonVisible = true;
            // 
            // 
            // 
            this.txtDateShipped.MonthCalendar.CommandsBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.txtDateShipped.MonthCalendar.CommandsBackgroundStyle.BackColorGradientAngle = 90;
            this.txtDateShipped.MonthCalendar.CommandsBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.txtDateShipped.MonthCalendar.CommandsBackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.txtDateShipped.MonthCalendar.CommandsBackgroundStyle.BorderTopColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.txtDateShipped.MonthCalendar.CommandsBackgroundStyle.BorderTopWidth = 1;
            this.txtDateShipped.MonthCalendar.CommandsBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtDateShipped.MonthCalendar.DisplayMonth = new System.DateTime(2015, 3, 1, 0, 0, 0, 0);
            this.txtDateShipped.MonthCalendar.FirstDayOfWeek = System.DayOfWeek.Monday;
            this.txtDateShipped.MonthCalendar.MarkedDates = new System.DateTime[0];
            this.txtDateShipped.MonthCalendar.MonthlyMarkedDates = new System.DateTime[0];
            // 
            // 
            // 
            this.txtDateShipped.MonthCalendar.NavigationBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.txtDateShipped.MonthCalendar.NavigationBackgroundStyle.BackColorGradientAngle = 90;
            this.txtDateShipped.MonthCalendar.NavigationBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.txtDateShipped.MonthCalendar.NavigationBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtDateShipped.MonthCalendar.TodayButtonVisible = true;
            this.txtDateShipped.MonthCalendar.WeeklyMarkedDays = new System.DayOfWeek[0];
            this.txtDateShipped.Name = "txtDateShipped";
            this.txtDateShipped.Size = new System.Drawing.Size(154, 20);
            this.txtDateShipped.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.txtDateShipped.TabIndex = 1;
            // 
            // txtDateRequired
            // 
            // 
            // 
            // 
            this.txtDateRequired.BackgroundStyle.Class = "DateTimeInputBackground";
            this.txtDateRequired.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtDateRequired.ButtonDropDown.Shortcut = DevComponents.DotNetBar.eShortcut.AltDown;
            this.txtDateRequired.ButtonDropDown.Visible = true;
            this.txtDateRequired.IsPopupCalendarOpen = false;
            this.txtDateRequired.Location = new System.Drawing.Point(66, 19);
            // 
            // 
            // 
            this.txtDateRequired.MonthCalendar.AnnuallyMarkedDates = new System.DateTime[0];
            // 
            // 
            // 
            this.txtDateRequired.MonthCalendar.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtDateRequired.MonthCalendar.CalendarDimensions = new System.Drawing.Size(1, 1);
            this.txtDateRequired.MonthCalendar.ClearButtonVisible = true;
            // 
            // 
            // 
            this.txtDateRequired.MonthCalendar.CommandsBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.txtDateRequired.MonthCalendar.CommandsBackgroundStyle.BackColorGradientAngle = 90;
            this.txtDateRequired.MonthCalendar.CommandsBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.txtDateRequired.MonthCalendar.CommandsBackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.txtDateRequired.MonthCalendar.CommandsBackgroundStyle.BorderTopColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.txtDateRequired.MonthCalendar.CommandsBackgroundStyle.BorderTopWidth = 1;
            this.txtDateRequired.MonthCalendar.CommandsBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtDateRequired.MonthCalendar.DisplayMonth = new System.DateTime(2015, 3, 1, 0, 0, 0, 0);
            this.txtDateRequired.MonthCalendar.FirstDayOfWeek = System.DayOfWeek.Monday;
            this.txtDateRequired.MonthCalendar.MarkedDates = new System.DateTime[0];
            this.txtDateRequired.MonthCalendar.MonthlyMarkedDates = new System.DateTime[0];
            // 
            // 
            // 
            this.txtDateRequired.MonthCalendar.NavigationBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.txtDateRequired.MonthCalendar.NavigationBackgroundStyle.BackColorGradientAngle = 90;
            this.txtDateRequired.MonthCalendar.NavigationBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.txtDateRequired.MonthCalendar.NavigationBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtDateRequired.MonthCalendar.TodayButtonVisible = true;
            this.txtDateRequired.MonthCalendar.WeeklyMarkedDays = new System.DayOfWeek[0];
            this.txtDateRequired.Name = "txtDateRequired";
            this.txtDateRequired.Size = new System.Drawing.Size(149, 20);
            this.txtDateRequired.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.txtDateRequired.TabIndex = 0;
            // 
            // dtgOrder
            // 
            this.dtgOrder.AllowUserToAddRows = false;
            this.dtgOrder.AllowUserToDeleteRows = false;
            this.dtgOrder.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtgOrder.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dtgOrder.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtgOrder.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.orderIDDataGridViewTextBoxColumn,
            this.customerIDDataGridViewTextBoxColumn,
            this.orderDateDataGridViewTextBoxColumn,
            this.requireDateDataGridViewTextBoxColumn,
            this.shippedDateDataGridViewTextBoxColumn,
            this.shipViaDataGridViewTextBoxColumn,
            this.freightDataGridViewTextBoxColumn,
            this.DeleteComlumn});
            this.dtgOrder.DataSource = this.orderBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(57)))), ((int)(((byte)(91)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dtgOrder.DefaultCellStyle = dataGridViewCellStyle2;
            this.dtgOrder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtgOrder.EnableHeadersVisualStyles = false;
            this.dtgOrder.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.dtgOrder.Location = new System.Drawing.Point(0, 127);
            this.dtgOrder.Name = "dtgOrder";
            this.dtgOrder.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtgOrder.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dtgOrder.Size = new System.Drawing.Size(760, 123);
            this.dtgOrder.TabIndex = 1;
            this.dtgOrder.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtgOrder_CellContentDoubleClick);
            this.dtgOrder.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dtgOrder_Scroll);
            // 
            // orderIDDataGridViewTextBoxColumn
            // 
            this.orderIDDataGridViewTextBoxColumn.DataPropertyName = "OrderID";
            this.orderIDDataGridViewTextBoxColumn.HeaderText = "OrderID";
            this.orderIDDataGridViewTextBoxColumn.Name = "orderIDDataGridViewTextBoxColumn";
            this.orderIDDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // customerIDDataGridViewTextBoxColumn
            // 
            this.customerIDDataGridViewTextBoxColumn.DataPropertyName = "CustomerID";
            this.customerIDDataGridViewTextBoxColumn.HeaderText = "CustomerID";
            this.customerIDDataGridViewTextBoxColumn.Name = "customerIDDataGridViewTextBoxColumn";
            this.customerIDDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // orderDateDataGridViewTextBoxColumn
            // 
            this.orderDateDataGridViewTextBoxColumn.DataPropertyName = "OrderDate";
            this.orderDateDataGridViewTextBoxColumn.HeaderText = "OrderDate";
            this.orderDateDataGridViewTextBoxColumn.Name = "orderDateDataGridViewTextBoxColumn";
            this.orderDateDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // requireDateDataGridViewTextBoxColumn
            // 
            this.requireDateDataGridViewTextBoxColumn.DataPropertyName = "RequireDate";
            this.requireDateDataGridViewTextBoxColumn.HeaderText = "RequireDate";
            this.requireDateDataGridViewTextBoxColumn.Name = "requireDateDataGridViewTextBoxColumn";
            this.requireDateDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // shippedDateDataGridViewTextBoxColumn
            // 
            this.shippedDateDataGridViewTextBoxColumn.DataPropertyName = "ShippedDate";
            this.shippedDateDataGridViewTextBoxColumn.HeaderText = "ShippedDate";
            this.shippedDateDataGridViewTextBoxColumn.Name = "shippedDateDataGridViewTextBoxColumn";
            this.shippedDateDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // shipViaDataGridViewTextBoxColumn
            // 
            this.shipViaDataGridViewTextBoxColumn.DataPropertyName = "ShipVia";
            this.shipViaDataGridViewTextBoxColumn.HeaderText = "ShipVia";
            this.shipViaDataGridViewTextBoxColumn.Name = "shipViaDataGridViewTextBoxColumn";
            this.shipViaDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // freightDataGridViewTextBoxColumn
            // 
            this.freightDataGridViewTextBoxColumn.DataPropertyName = "Freight";
            this.freightDataGridViewTextBoxColumn.HeaderText = "Freight";
            this.freightDataGridViewTextBoxColumn.Name = "freightDataGridViewTextBoxColumn";
            this.freightDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // DeleteComlumn
            // 
            this.DeleteComlumn.HeaderText = "Delete";
            this.DeleteComlumn.Name = "DeleteComlumn";
            this.DeleteComlumn.ReadOnly = true;
            // 
            // orderBindingSource
            // 
            this.orderBindingSource.DataSource = typeof(DemoWF.NorthwindService.Order);
            // 
            // frmMainOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 250);
            this.Controls.Add(this.dtgOrder);
            this.Controls.Add(this.Search);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmMainOrder";
            this.Text = "frmMainOrder";
            this.Load += new System.EventHandler(this.frmMainOrder_Load);
            this.Search.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtDateShipped)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDateRequired)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtgOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.orderBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox Search;
        private DevComponents.DotNetBar.ButtonX buttonX3;
        private DevComponents.DotNetBar.ButtonX btnDelete;
        private DevComponents.DotNetBar.ButtonX btnAdd;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.TextBoxX txtFreight;
        private DevComponents.Editors.DateTimeAdv.DateTimeInput txtDateShipped;
        private DevComponents.Editors.DateTimeAdv.DateTimeInput txtDateRequired;
        private System.Windows.Forms.DataGridView dtgOrder;
        private System.Windows.Forms.BindingSource orderBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn orderIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn customerIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn orderDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn requireDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn shippedDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn shipViaDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn freightDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn DeleteComlumn;
    }
}