namespace GridExtensions.GridFilters
{
    using System;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using GridExtensions.GridFilters.EnumerationSources;

    /// <summary>
    ///     A <see cref="IGridFilter" /> implementation for filtering any columns
    ///     with enmuration types. A <see cref="ComboBox" /> will show all
    ///     possible enumeration values from which the user can select one.
    /// </summary>
    public class EnumerationGridFilter : GridFilterBase
    {
        private const string FilterFormat = "{0} = {1}";

        private const string FilterRegex = @"\[[a-zA-Z].*\] = (?<Value>(\+|-)?[0-9][0-9]*)";

        private readonly ComboBox combo;

        private readonly IEnumerationSource enumerationSource;

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        /// <param name="enumerationSource">
        ///     Source defining what values should
        ///     be displayed and how they are filtered.
        /// </param>
        public EnumerationGridFilter(IEnumerationSource enumerationSource)
            : this(enumerationSource, new ComboBox(), false)
        {
        }

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        /// <param name="enumerationSource">
        ///     Source defining what values should
        ///     be displayed and how they are filtered.
        /// </param>
        /// <param name="comboBox">Control which should be used to display the enumeration values.</param>
        public EnumerationGridFilter(IEnumerationSource enumerationSource, ComboBox comboBox)
            : this(enumerationSource, comboBox, true)
        {
        }

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        /// <param name="dataType">
        ///     <see cref="Type" /> of the enumeration which values
        ///     should be displayed
        /// </param>
        public EnumerationGridFilter(Type dataType)
            : this(new TypeEnumerationSource(dataType))
        {
        }

        private EnumerationGridFilter(
            IEnumerationSource enumerationSource,
            ComboBox comboBox,
            bool useCustomFilterPlacement)
            : base(useCustomFilterPlacement)
        {
            this.enumerationSource = enumerationSource;

            this.combo = comboBox;
            this.combo.DropDownStyle = ComboBoxStyle.DropDownList;
            this.combo.SelectedIndexChanged += this.OnComboSelectedIndexChanged;

            this.combo.Items.Clear();
            this.combo.Items.Add(string.Empty);
            this.combo.SelectedIndex = 0;
            this.combo.Items.AddRange(this.enumerationSource.AllValues);

            this.combo.Sorted = true;
        }

        /// <summary>
        ///     The <see cref="ComboBox" /> for the GUI.
        /// </summary>
        public override Control FilterControl => this.combo;

        /// <summary>
        ///     Gets whether a filter is set.
        ///     True, if the text of the <see cref="ComboBox" /> is not empty.
        /// </summary>
        public override bool HasFilter => this.combo.Text.Length > 0;

        /// <summary>
        ///     Gets or sets the current value of the contained <see cref="ComboBox" />.
        /// </summary>
        public object Value
        {
            get => this.combo.SelectedItem;
            set
            {
                if (this.combo.Items.Contains(value)) this.combo.SelectedItem = value;
                else this.combo.SelectedIndex = 0;
            }
        }

        /// <summary>
        ///     Clears the filter to its initial state.
        /// </summary>
        public override void Clear()
        {
            this.combo.SelectedIndex = 0;
        }

        /// <summary>
        ///     Cleans up
        /// </summary>
        public override void Dispose()
        {
            this.combo.SelectedIndexChanged -= this.OnComboSelectedIndexChanged;
            this.combo.Dispose();
        }

        /// <summary>
        ///     Gets a filter with a criteria in string representation.
        /// </summary>
        /// <param name="columnName">
        ///     The name of the column for which the criteria should be generated.
        /// </param>
        /// <returns>a string representing the current filter criteria</returns>
        public override string GetFilter(string columnName)
        {
            if (!this.HasFilter) return string.Empty;

            return string.Format(
                FilterFormat,
                columnName,
                this.enumerationSource.GetFilterFromValue(this.combo.SelectedItem));
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

                this.combo.SelectedItem = this.enumerationSource.GetValueFromFilter(match.Groups["Value"].Value);
            }
        }

        private void OnComboSelectedIndexChanged(object sender, EventArgs e)
        {
            this.OnChanged();
        }
    }
}