namespace GridExtensions.GridFilters
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    /// <summary>
    ///     A <see cref="IGridFilter" /> implementation for filtering numeric columns
    ///     with a <see cref="NumericGridFilterControl" /> to control the filter.
    /// </summary>
    public class NumericGridFilter : GridFilterBase
    {
        internal const string InBetween = "<x<";

        private const string FilterFormatBetween = "{0} >= {1} AND {0} <= {2}";

        private const string FilterFormatSingle = "{0} {1} {2}";

        private const string FilterFormatString = "Convert({0}, 'System.String') LIKE '{1}*'";

        private const string FilterRegexBetween =
                @"\[[a-zA-Z].*\] (?<Operator1>(>=)) (?<Value1>(\+|-)?[0-9][0-9]*(\.[0-9]*)?) AND \[[a-zA-Z].*\] (?<Operator2>(<=)) (?<Value2>(\+|-)?[0-9][0-9]*(\.[0-9]*)?)"
            ;

        private const string FilterRegexSingle =
            @"\[[a-zA-Z].*\] (?<Operator>(<|>|<=|>=|=|<>|)) (?<Value>(\+|-)?[0-9][0-9]*(\.[0-9]*)?)";

        private const string FilterRegexString =
            @"Convert\(\[[a-zA-Z].*\],\s'System.String'\)\sLIKE\s'(?<Value>(\+|-)?[0-9][0-9]*(\.[0-9]*)?)\*'";

        private readonly NumericGridFilterControl numericGridFilterControl;

        /// <summary>
        ///     Creates a new instance with <see cref="GridFilterBase.UseCustomFilterPlacement" />
        ///     and <see cref="ShowInBetweenOperator" /> set to false.
        /// </summary>
        public NumericGridFilter()
            : this(new NumericGridFilterControl(), false, false)
        {
        }

        /// <summary>
        ///     Creates a new instance with <see cref="GridFilterBase.UseCustomFilterPlacement" />
        ///     set to false.
        /// </summary>
        /// <param name="showInBetweenOperator">Determines whether the 'in between' operator is available.</param>
        public NumericGridFilter(bool showInBetweenOperator)
            : this(new NumericGridFilterControl(), false, showInBetweenOperator)
        {
        }

        /// <summary>
        ///     Creates a new instance with <see cref="GridFilterBase.UseCustomFilterPlacement" />
        ///     set to true and not having the 'in between' operator.
        /// </summary>
        /// <param name="numericGridFilterControl">
        ///     A <see cref="NumericGridFilterControl" />
        ///     instance which should be used by the filter.
        /// </param>
        public NumericGridFilter(NumericGridFilterControl numericGridFilterControl)
            : this(numericGridFilterControl, true, false)
        {
        }

        /// <summary>
        ///     Creates a new instance with <see cref="GridFilterBase.UseCustomFilterPlacement" />
        ///     set to true and not having the 'in between' operator.
        /// </summary>
        /// <param name="numericGridFilterControl">
        ///     A <see cref="NumericGridFilterControl" />
        ///     instance which should be used by the filter.
        /// </param>
        /// <param name="showInBetweenOperator">Determines whether the 'in between' operator is available.</param>
        public NumericGridFilter(NumericGridFilterControl numericGridFilterControl, bool showInBetweenOperator)
            : this(numericGridFilterControl, true, showInBetweenOperator)
        {
        }

        private NumericGridFilter(
            NumericGridFilterControl numericGridFilterControl,
            bool useCustomFilterPlacement,
            bool showInBetweenOperator)
            : base(useCustomFilterPlacement)
        {
            this.numericGridFilterControl = numericGridFilterControl;
            this.numericGridFilterControl.Changed += this.OnNumericGridFilterControlChanged;
            this.ShowInBetweenOperator = showInBetweenOperator;
        }

        /// <summary>
        ///     Returns the instance itsself, which contains a <see cref="TextBox" />
        ///     and a <see cref="ComboBox" /> to adjust the filter.
        /// </summary>
        public override Control FilterControl => this.numericGridFilterControl;

        /// <summary>
        ///     Gets whether a filter is set.
        ///     True, if the text of the <see cref="TextBox" /> is not empty.
        /// </summary>
        public override bool HasFilter => this.Text1.Length > 0 || this.Operator == InBetween && this.Text2.Length > 0;

        /// <summary>
        ///     Gets or sets the current operator of the contained <see cref="ComboBox" />.
        /// </summary>
        public string Operator
        {
            get => (string)this.numericGridFilterControl.ComboBox.SelectedItem;
            set => this.numericGridFilterControl.ComboBox.SelectedItem = value;
        }

        /// <summary>
        ///     Sets or gets whether the 'in between' operator should be available.
        /// </summary>
        public bool ShowInBetweenOperator
        {
            get => this.numericGridFilterControl.ComboBox.Items.Contains(InBetween);
            set
            {
                if (value == this.ShowInBetweenOperator) return;

                if (value)
                {
                    this.numericGridFilterControl.ComboBox.Items.Add(InBetween);
                }
                else
                {
                    this.numericGridFilterControl.ComboBox.Items.Remove(InBetween);
                    if (this.Operator == InBetween) this.numericGridFilterControl.ComboBox.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the current text of the first contained <see cref="TextBox" />.
        /// </summary>
        public string Text1
        {
            get => this.numericGridFilterControl.TextBox1.Text;
            set => this.numericGridFilterControl.TextBox1.Text = value;
        }

        /// <summary>
        ///     Gets or sets the current text of the second contained <see cref="TextBox" />.
        /// </summary>
        public string Text2
        {
            get => this.numericGridFilterControl.TextBox2.Text;
            set => this.numericGridFilterControl.TextBox2.Text = value;
        }

        /// <summary>
        ///     Clears the filter to its initial state.
        /// </summary>
        public override void Clear()
        {
            this.numericGridFilterControl.ComboBox.SelectedIndex = 0;
            this.Text1 = string.Empty;
            this.Text2 = string.Empty;
        }

        /// <summary>
        ///     Gets a filter with the current criteria in string representation.
        ///     If operator '*' is set in the <see cref="ComboBox" /> a text criteria
        ///     with like will be created.
        ///     All other operators will do numerical comparisons. If no valid number
        ///     is entered then all rows will be filtered out.
        /// </summary>
        /// <param name="columnName">
        ///     The name of the column for which the criteria should be generated.
        /// </param>
        /// <returns>A string representing the current filter criteria</returns>
        public override string GetFilter(string columnName)
        {
            if (!this.HasFilter) return string.Empty;

            if (this.Operator == "*") return string.Format(FilterFormatString, columnName, this.Text1);

            try
            {
                if (this.Operator == InBetween)
                {
                    var decimal1 = this.Text1.Length == 0 ? decimal.MinValue : Convert.ToDecimal(this.Text1);
                    var decimal2 = this.Text2.Length == 0 ? decimal.MaxValue : Convert.ToDecimal(this.Text2);

                    var number1 = decimal1.ToString(CultureInfo.CreateSpecificCulture("en-US"));
                    var number2 = decimal2.ToString(CultureInfo.CreateSpecificCulture("en-US"));

                    return string.Format(FilterFormatBetween, columnName, number1, number2);
                }

                var number = Convert.ToDecimal(this.Text1).ToString(CultureInfo.CreateSpecificCulture("en-US"));
                return string.Format(FilterFormatSingle, columnName, this.Operator, number);
            }
            catch
            {
                return columnName + " = " + false;
            }
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
            var regex = new Regex(FilterRegexBetween, RegexOptions.ExplicitCapture);
            if (this.ShowInBetweenOperator && regex.IsMatch(filter))
            {
                var match = regex.Match(filter);
                this.numericGridFilterControl.ComboBox.SelectedItem = InBetween;

                var decimal1 = Convert.ToDecimal(
                    match.Groups["Value1"].Value,
                    CultureInfo.CreateSpecificCulture("en-US"));
                var decimal2 = Convert.ToDecimal(
                    match.Groups["Value2"].Value,
                    CultureInfo.CreateSpecificCulture("en-US"));

                this.numericGridFilterControl.TextBox1.Text =
                    decimal1 == decimal.MinValue ? string.Empty : decimal1.ToString();
                this.numericGridFilterControl.TextBox2.Text =
                    decimal2 == decimal.MaxValue ? string.Empty : decimal2.ToString();
            }
            else
            {
                regex = new Regex(FilterRegexString);
                if (regex.IsMatch(filter))
                {
                    var match = regex.Match(filter);
                    this.Text1 = match.Groups["Value"].Value;
                    this.Operator = "*";
                }
                else
                {
                    regex = new Regex(FilterRegexSingle);
                    if (regex.IsMatch(filter))
                    {
                        var match = regex.Match(filter);
                        this.Text1 = Convert.ToDecimal(
                            match.Groups["Value"].Value,
                            CultureInfo.CreateSpecificCulture("en-US")).ToString();
                        this.Operator = match.Groups["Operator"].Value;
                    }
                }
            }
        }

        private void OnNumericGridFilterControlChanged(object sender, EventArgs e)
        {
            this.OnChanged();
        }
    }
}