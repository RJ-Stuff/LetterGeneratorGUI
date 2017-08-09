namespace GridExtensions.GridFilters
{
    using System;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    /// <summary>
    ///     A <see cref="IGridFilter" /> implementation for filtering any columns
    ///     with a <see cref="TextBox" /> to control the filter.
    ///     All rows not beginning with the specified text are filtered out.
    /// </summary>
    public class TextGridFilter : GridFilterBase
    {
        private const string FilterFormat = "Convert({0}, 'System.String') LIKE '{1}*'";

        private const string FilterRegex = @"Convert\(\[[a-zA-Z].*\],\s'System.String'\)\sLIKE\s'(?<Value>.*)\*'";

        private readonly TextBox textBox;

        /// <summary>
        ///     Creates a new instance
        /// </summary>
        public TextGridFilter(TextBox textBox)
            : this(textBox, true)
        {
        }

        /// <summary>
        ///     Creates a new instance
        /// </summary>
        public TextGridFilter()
            : this(new TextBox(), false)
        {
        }

        private TextGridFilter(TextBox textBox, bool useCustomFilterPlacement)
            : base(useCustomFilterPlacement)
        {
            this.textBox = textBox;
            this.textBox.TextChanged += this.OnTextBoxTextChanged;
        }

        /// <summary>
        ///     The <see cref="TextBox" /> for the GUI.
        /// </summary>
        public override Control FilterControl => this.textBox;

        /// <summary>
        ///     Gets whether a filter is set.
        ///     True, if the text of the <see cref="CheckBox" /> is not empty.
        /// </summary>
        public override bool HasFilter => this.textBox.Text.Length > 0;

        /// <summary>
        ///     Gets or sets the current text of the contained <see cref="TextBox" />.
        /// </summary>
        public string Text
        {
            get => this.textBox.Text;
            set => this.textBox.Text = value;
        }

        /// <summary>
        ///     Clears the filter to its initial state.
        /// </summary>
        public override void Clear()
        {
            this.textBox.Text = string.Empty;
        }

        /// <summary>
        ///     Cleans up
        /// </summary>
        public override void Dispose()
        {
            this.textBox.TextChanged -= this.OnTextBoxTextChanged;
            this.textBox.Dispose();
        }

        /// <summary>
        ///     Gets a filter with a like criteria in string representation
        /// </summary>
        /// <param name="columnName">
        ///     The name of the column for which the criteria should be generated.
        /// </param>
        /// <returns>a string representing the current filter criteria</returns>
        public override string GetFilter(string columnName)
        {
            return string.Format(FilterFormat, columnName, this.textBox.Text);
        }

        /// <summary>
        ///     Sets a string which a a previous result of <see cref="GetFilter" />
        ///     in order to configure the <see cref="FilterControl" /> to match the
        ///     given filter criteria.
        /// </summary>
        /// <param name="filter">filter criteria</param>
        /// <returns></returns>
        public override void SetFilter(string filter)
        {
            var regex = new Regex(FilterRegex);
            if (regex.IsMatch(filter))
            {
                var match = regex.Match(filter);
                this.textBox.Text = match.Groups["Value"].Value;
            }
        }

        private void OnTextBoxTextChanged(object sender, EventArgs e)
        {
            this.OnChanged();
        }
    }
}