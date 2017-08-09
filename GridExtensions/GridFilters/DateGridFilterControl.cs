namespace GridExtensions.GridFilters
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    ///     A control with a <see cref="ComboBox" /> and two <see cref="DateTimePicker" />s
    ///     needed in the <see cref="DateGridFilter" />.
    /// </summary>
    public class DateGridFilterControl : UserControl
    {
        private readonly Container components = null;

        private ComboBox comboBox;

        private DateTimePicker picker1;

        private DateTimePicker picker2;

        /// <summary>
        ///     Creates a new instance
        /// </summary>
        public DateGridFilterControl()
        {
            this.InitializeComponent();

            this.picker1.Format = DateTimePickerFormat.Short;
            this.picker2.Format = DateTimePickerFormat.Short;
            this.comboBox.SelectedIndex = 0;
            this.RefreshPickerWidth();
        }

        /// <summary>
        ///     Event firing when either the <see cref="ComboBox" /> or
        ///     the <see cref="DateTimePicker" /> has changed.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        ///     Gets the contained <see cref="ComboBox" /> instance.
        /// </summary>
        public ComboBox ComboBox => this.comboBox;

        /// <summary>
        ///     Gets the first contained <see cref="DateTimePicker" /> instance.
        /// </summary>
        public DateTimePicker DateTimePicker1 => this.picker1;

        /// <summary>
        ///     Gets the second contained <see cref="DateTimePicker" /> instance.
        /// </summary>
        public DateTimePicker DateTimePicker2 => this.picker2;

        /// <summary>
        ///     Cleans up.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing) this.components?.Dispose();

            base.Dispose(disposing);
        }

        /// <summary>
        ///     Resizes the contained <see cref="DateTimePicker" />s so that they
        ///     have the same width.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.RefreshPickerWidth();
        }

        /// <summary>
        ///     Erforderliche Methode für die Designerunterstützung.
        ///     Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.picker1 = new DateTimePicker();
            this.picker2 = new DateTimePicker();
            this.comboBox = new ComboBox();
            this.SuspendLayout();

            // _picker1
            this.picker1.Checked = false;
            this.picker1.Dock = DockStyle.Fill;
            this.picker1.Location = new Point(40, 0);
            this.picker1.Name = "picker1";
            this.picker1.Size = new Size(64, 20);
            this.picker1.TabIndex = 0;
            this.picker1.TextChanged += this.OnChanged;
            this.picker1.KeyPress += this.OnKeyPress;
            this.picker1.KeyUp += this.OnKeyUp;
            this.picker1.KeyDown += this.OnKeyDown;

            // _picker2
            this.picker2.Checked = false;
            this.picker2.Dock = DockStyle.Right;
            this.picker2.Location = new Point(104, 0);
            this.picker2.Name = "picker2";
            this.picker2.Size = new Size(40, 20);
            this.picker2.TabIndex = 2;
            this.picker2.TextChanged += this.OnChanged;
            this.picker2.KeyPress += this.OnKeyPress;
            this.picker2.KeyUp += this.OnKeyUp;
            this.picker2.KeyDown += this.OnKeyDown;

            // _comboBox
            this.comboBox.Dock = DockStyle.Left;
            this.comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox.Items.AddRange(new object[] { string.Empty, "=", "<>", ">", "<", ">=", "<=" });
            this.comboBox.Location = new Point(0, 0);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new Size(40, 21);
            this.comboBox.TabIndex = 1;
            this.comboBox.KeyDown += this.OnKeyDown;
            this.comboBox.KeyPress += this.OnKeyPress;
            this.comboBox.KeyUp += this.OnKeyUp;
            this.comboBox.SelectedIndexChanged += this.OnChanged;

            // DateGridFilterControl
            this.Controls.Add(this.picker1);
            this.Controls.Add(this.picker2);
            this.Controls.Add(this.comboBox);
            this.Name = "DateGridFilterControl";
            this.Size = new Size(144, 21);
            this.ResumeLayout(false);
        }

        private void OnChanged(object sender, EventArgs e)
        {
            this.picker2.Visible = this.comboBox.Text == DateGridFilter.InBetween;

            this.Changed?.Invoke(this, e);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            this.OnKeyPress(e);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            this.OnKeyUp(e);
        }

        private void RefreshPickerWidth()
        {
            this.picker2.Width = (this.Width - this.comboBox.Width) / 2;
        }
    }
}