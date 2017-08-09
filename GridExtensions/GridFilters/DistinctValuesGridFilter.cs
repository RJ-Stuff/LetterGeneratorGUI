namespace GridExtensions.GridFilters
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    /// <summary>
    ///     A <see cref="IGridFilter" /> implementation for columns with a <see cref="ComboBox" />
    ///     containing all values found within the column.
    /// </summary>
    public class DistinctValuesGridFilter : GridFilterBase
    {
        private const string FilterFormat = "Convert({0}, System.String) = '{1}'";

        private const string FilterRegex = @"Convert\(\[[a-zA-Z].*\],\sSystem.String\)\s=\s'(?<Value>.*)'";

        private readonly ComboBox combo;

        private string[] _values;

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        /// <param name="column">Column where the values list should be generated from.</param>
        public DistinctValuesGridFilter(DataColumn column)
            : this(new ComboBox(), false)
        {
            this.Fill(column);
        }

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        /// <param name="column">Column where the values list should be generated from.</param>
        /// <param name="comboBox">Control which should be used to display the values.</param>
        public DistinctValuesGridFilter(DataColumn column, ComboBox comboBox)
            : this(comboBox, true)
        {
            this.Fill(column);
        }

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        /// <param name="values">The list of values to be displayed.</param>
        /// <param name="containsDbNull">Indicates whether the (null) entry should be displayed or not.</param>
        public DistinctValuesGridFilter(string[] values, bool containsDbNull)
            : this(new ComboBox(), false)
        {
            this.Fill(values, containsDbNull);
        }

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        /// <param name="values">The list of values to be displayed.</param>
        /// <param name="containsDbNull">Indicates whether the (null) entry should be displayed or not.</param>
        /// <param name="comboBox">Control which should be used to display the values.</param>
        public DistinctValuesGridFilter(string[] values, bool containsDbNull, ComboBox comboBox)
            : this(comboBox, true)
        {
            this.Fill(values, containsDbNull);
        }

        private DistinctValuesGridFilter(ComboBox comboBox, bool useCustomFilterPlacement)
            : base(useCustomFilterPlacement)
        {
            this.combo = comboBox;
            this.combo.DropDownStyle = ComboBoxStyle.DropDownList;
            this.combo.SelectedIndexChanged += this.OnComboSelectedIndexChanged;

            this.combo.Items.Clear();
            this.combo.Items.Add(SpecialValue.NoFilter);
            this.combo.SelectedIndex = 0;
        }

        private enum SpecialValueType
        {
            None,

            Null
        }

        /// <summary>
        ///     Gets or sets the current value of the contained <see cref="ComboBox" />.
        /// </summary>
        public object CurrentValue
        {
            get => this.combo.SelectedItem;
            set
            {
                if (!(value is string) || !(value is SpecialValue))
                    throw new ArgumentException("Value must be either a string or of type SpecialValue", "value");
                this.combo.SelectedItem = value;
            }
        }

        /// <summary>
        ///     The <see cref="ComboBox" /> for the GUI.
        /// </summary>
        public override Control FilterControl => this.combo;

        /// <summary>
        ///     Gets whether a filter is set.
        ///     True, if the text of the <see cref="ComboBox" /> is not empty.
        /// </summary>
        public override bool HasFilter => this.combo.SelectedItem != SpecialValue.NoFilter;

        /// <summary>
        ///     Gets all values contained in the <see cref="ComboBox" />.
        /// </summary>
        public object[] Values
        {
            get
            {
                var result = new object[this.combo.Items.Count];
                this.combo.Items.CopyTo(result, 0);
                return result;
            }
        }

        /// <summary>
        ///     Gets all values found in the specified columns as a string array.
        /// </summary>
        /// <param name="column">Column to search for values.</param>
        /// <param name="containsDbNull">Indicates whether the (null) entry is contained in the column or not.</param>
        /// <returns>Array with different values.</returns>
        public static string[] GetDistinctValues(DataColumn column, out bool containsDbNull)
        {
            return GetDistinctValues(column, int.MaxValue, out containsDbNull);
        }

        /// <summary>
        ///     Gets all values found in the specified columns as a string array
        ///     limited in size to the value specified. If this value is exceeded
        ///     than null will be returned instead.
        /// </summary>
        /// <param name="column">Column to search for values.</param>
        /// <param name="maximumValues">Value indicating how many different values should be fetched at maximum.</param>
        /// <param name="containsDbNull">Indicates whether the (null) entry is contained in the column or not.</param>
        /// <returns>Array with different values, or Null.</returns>
        public static string[] GetDistinctValues(DataColumn column, int maximumValues, out bool containsDbNull)
        {
            var hash = new Hashtable();
            containsDbNull = false;

            foreach (DataRow row in column.Table.Rows)
                if (row[column] == DBNull.Value)
                {
                    containsDbNull = true;
                }
                else
                {
                    var value = row[column].ToString();
                    if (!hash.ContainsKey(value))
                    {
                        hash.Add(value, 0);
                        if (hash.Count > maximumValues) return null;
                    }
                }

            var result = new string[hash.Count];
            hash.Keys.CopyTo(result, 0);
            return result;
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

            if (this.combo.SelectedItem == SpecialValue.NullFilter)
                return string.Format(NullGridFilter.FilterFormat, columnName, "=");

            return string.Format(FilterFormat, columnName, (string)this.combo.SelectedItem);
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
            var regex = new Regex(NullGridFilter.FilterRegex);
            if (regex.IsMatch(filter))
            {
                this.combo.SelectedItem = SpecialValue.NullFilter;
            }
            else
            {
                regex = new Regex(FilterRegex);
                if (regex.IsMatch(filter))
                {
                    var match = regex.Match(filter);

                    this.combo.SelectedItem = match.Groups["Value"].Value;
                }
            }
        }

        private void Fill(DataColumn column)
        {
            bool containsDbNull;
            var values = GetDistinctValues(column, out containsDbNull);
            this.Fill(values, containsDbNull);
        }

        private void Fill(string[] values, bool containsDbNull)
        {
            Array.Sort(values);
            this._values = values;
            if (containsDbNull) this.combo.Items.Add(SpecialValue.NullFilter);
            this.combo.Items.AddRange(values);
        }

        private void OnComboSelectedIndexChanged(object sender, EventArgs e)
        {
            this.OnChanged();
        }

        /// <summary>
        ///     Defines special values which can be contained in the <see cref="ComboBox" />
        ///     of a <see cref="DistinctValuesGridFilter" />.
        /// </summary>
        public class SpecialValue
        {
            /// <summary>
            ///     The special value meaning 'No filtering'.
            /// </summary>
            public static readonly SpecialValue NoFilter = new SpecialValue(SpecialValueType.None);

            /// <summary>
            ///     The special value meaning 'Filter null values'.
            /// </summary>
            public static readonly SpecialValue NullFilter = new SpecialValue(SpecialValueType.Null);

            private readonly SpecialValueType type;

            private SpecialValue(SpecialValueType type)
            {
                this.type = type;
            }

            /// <summary>
            ///     Gets a textual representation.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                switch (this.type)
                {
                    case SpecialValueType.None: return "(*)";
                    case SpecialValueType.Null: return "(null)";
                }
                return base.ToString();
            }
        }
    }
}