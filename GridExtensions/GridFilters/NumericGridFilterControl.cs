namespace GridExtensions.GridFilters
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    ///     A control with a <see cref="ComboBox" /> and two <see cref="TextBox" />es
    ///     needed in the <see cref="NumericGridFilter" />.
    /// </summary>
    public class NumericGridFilterControl : UserControl
    {
        private readonly Container components = null;

        private ComboBox comboBox;

        private TextBox textBox1;

        private TextBox textBox2;

        /// <summary>
        ///     Creates a new instance
        /// </summary>
        public NumericGridFilterControl()
        {
            this.InitializeComponent();

            this.comboBox.SelectedIndex = 0;
        }

        /// <summary>
        ///     Event firing when either the <see cref="ComboBox" /> or
        ///     the <see cref="TextBox" /> has changed.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        ///     Gets the contained <see cref="ComboBox" /> instance.
        /// </summary>
        public ComboBox ComboBox => this.comboBox;

        /// <summary>
        ///     Gets the first contained <see cref="TextBox" /> instance.
        /// </summary>
        public TextBox TextBox1 => this.textBox1;

        /// <summary>
        ///     Gets the second contained <see cref="TextBox" /> instance.
        /// </summary>
        public TextBox TextBox2 => this.textBox2;

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
            this.RefreshTextBoxWidth();
        }

        /// <summary>
        ///     Erforderliche Methode für die Designerunterstützung.
        ///     Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new TextBox();
            this.textBox2 = new TextBox();
            this.comboBox = new ComboBox();
            this.SuspendLayout();

            // _textBox1
            this.textBox1.Dock = DockStyle.Fill;
            this.textBox1.Location = new Point(40, 0);
            //this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Size(0, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = string.Empty;
            this.textBox1.KeyDown += this.OnKeyDown;
            this.textBox1.KeyPress += this.OnKeyPress;
            this.textBox1.TextChanged += this.OnChanged;
            this.textBox1.KeyUp += this.OnKeyUp;

            // _textBox2
            this.textBox2.Dock = DockStyle.Right;
            this.textBox2.Location = new Point(40, 0);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Size(104, 20);
            this.textBox2.TabIndex = 2;
            this.textBox2.Text = string.Empty;
            this.textBox2.KeyDown += this.OnKeyDown;
            this.textBox2.KeyPress += this.OnKeyPress;
            this.textBox2.TextChanged += this.OnChanged;
            this.textBox2.KeyUp += this.OnKeyUp;

            // _comboBox
            this.comboBox.Dock = DockStyle.Left;
            this.comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox.Items.AddRange(new object[] { "*", "=", "<>", ">", "<", ">=", "<=" });
            this.comboBox.Location = new Point(0, 0);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new Size(40, 21);
            this.comboBox.TabIndex = 1;
            this.comboBox.KeyDown += this.OnKeyDown;
            this.comboBox.KeyPress += this.OnKeyPress;
            this.comboBox.KeyUp += this.OnKeyUp;
            this.comboBox.SelectedIndexChanged += this.OnChanged;

            // NumericGridFilterControl
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.comboBox);
            this.Name = "NumericGridFilterControl";
            this.Size = new Size(144, 21);
            this.ResumeLayout(false);
        }

        private void OnChanged(object sender, EventArgs e)
        {
            this.textBox2.Visible = this.comboBox.Text == NumericGridFilter.InBetween;

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

        private void RefreshTextBoxWidth()
        {
            this.textBox2.Width = (this.Width - this.comboBox.Width) / 2;
        }
    }
}