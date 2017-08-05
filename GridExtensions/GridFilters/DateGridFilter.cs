namespace GridExtensions.GridFilters
{
    using System;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    /// <summary>
    ///     A <see cref="IGridFilter" /> implementation for filtering date columns
    ///     with a <see cref="DateGridFilterControl" /> to control the filter.
    /// </summary>
    public class DateGridFilter : GridFilterBase
    {
        internal const string InBetween = "<x<";

        private const string FilterFormat = @"{0} {1} #{2:MM\/dd\/yyyy}#";

        private const string FilterFormatBetween = @"{0} >= #{1:MM\/dd\/yyyy}# AND {0} <= #{2:MM\/dd\/yyyy}#";

        private const string FilterRegex =
            @"\[[a-zA-Z].*\] (?<Operator>(<|>|<=|>=|=|<>|)) #(?<Month>[0-9]{2})/(?<Day>[0-9]{2})/(?<Year>[0-9]{4})#";

        private const string FilterRegexBetween =
                @"\[[a-zA-Z].*\] (?<Operator1>(>=)) #(?<Month1>[0-9]{2})/(?<Day1>[0-9]{2})/(?<Year1>[0-9]{4})# AND \[[a-zA-Z].*\] (?<Operator2>(<=)) #(?<Month2>[0-9]{2})/(?<Day2>[0-9]{2})/(?<Year2>[0-9]{4})#"
            ;

        private readonly DateGridFilterControl dateGridFilterControl;

        /// <summary>
        ///     Creates a new instance with <see cref="GridFilterBase.UseCustomFilterPlacement" />
        ///     and <see cref="ShowInBetweenOperator" /> set to false.
        /// </summary>
        public DateGridFilter()
            : this(new DateGridFilterControl(), false, false)
        {
        }

        /// <summary>
        ///     Creates a new instance with <see cref="GridFilterBase.UseCustomFilterPlacement" />
        ///     set to false.
        /// </summary>
        /// <param name="showInBetweenOperator">Determines whether the 'in between' operator is available.</param>
        public DateGridFilter(bool showInBetweenOperator)
            : this(new DateGridFilterControl(), false, showInBetweenOperator)
        {
        }

        /// <summary>
        ///     Creates a new instance with <see cref="GridFilterBase.UseCustomFilterPlacement" />
        ///     set to true and not having the 'in between' operator.
        /// </summary>
        /// <param name="dateGridFilterControl">
        ///     A <see cref="DateGridFilterControl" />
        ///     instance which should be used by the filter.
        /// </param>
        public DateGridFilter(DateGridFilterControl dateGridFilterControl)
            : this(dateGridFilterControl, true, false)
        {
        }

        /// <summary>
        ///     Creates a new instance with <see cref="GridFilterBase.UseCustomFilterPlacement" />
        ///     set to true.
        /// </summary>
        /// <param name="dateGridFilterControl">
        ///     A <see cref="DateGridFilterControl" />
        ///     instance which should be used by the filter.
        /// </param>
        /// <param name="showInBetweenOperator">Determines whether the 'in between' operator is available.</param>
        public DateGridFilter(DateGridFilterControl dateGridFilterControl, bool showInBetweenOperator)
            : this(dateGridFilterControl, true, showInBetweenOperator)
        {
        }

        private DateGridFilter(
            DateGridFilterControl dateGridFilterControl,
            bool useCustomFilterPlacement,
            bool showInBetweenOperator)
            : base(useCustomFilterPlacement)
        {
            this.dateGridFilterControl = dateGridFilterControl;
            this.dateGridFilterControl.Changed += this.OnDateGridFilterControlChanged;
            this.ShowInBetweenOperator = showInBetweenOperator;
        }

        /// <summary>
        ///     Gets or sets the current date of the first contained <see cref="DateTimePicker" />.
        /// </summary>
        public DateTime Date1
        {
            get => this.dateGridFilterControl.DateTimePicker1.Value;
            set => this.dateGridFilterControl.DateTimePicker1.Value = value;
        }

        /// <summary>
        ///     Gets or sets the current date of the second contained <see cref="DateTimePicker" />.
        /// </summary>
        public DateTime Date2
        {
            get => this.dateGridFilterControl.DateTimePicker2.Value;
            set => this.dateGridFilterControl.DateTimePicker2.Value = value;
        }

        /// <summary>
        ///     Returns the instance itsself, which contains a <see cref="DateTimePicker" />
        ///     and a <see cref="ComboBox" /> to adjust the filter.
        /// </summary>
        public override Control FilterControl => this.dateGridFilterControl;

        /// <summary>
        ///     Gets whether a filter is set.
        ///     True, if the <see cref="ComboBox" /> is not empty.
        /// </summary>
        public override bool HasFilter => this.dateGridFilterControl.ComboBox.SelectedItem.ToString().Length > 0;

        /// <summary>
        ///     Gets or sets the current operator of the contained <see cref="ComboBox" />.
        /// </summary>
        public string Operator
        {
            get => (string)this.dateGridFilterControl.ComboBox.SelectedItem;
            set => this.dateGridFilterControl.ComboBox.SelectedItem = value;
        }

        /// <summary>
        ///     Sets or gets whether the 'in between' operator should be available.
        /// </summary>
        public bool ShowInBetweenOperator
        {
            get => this.dateGridFilterControl.ComboBox.Items.Contains(InBetween);
            set
            {
                if (value == this.ShowInBetweenOperator) return;

                if (value)
                {
                    this.dateGridFilterControl.ComboBox.Items.Add(InBetween);
                }
                else
                {
                    this.dateGridFilterControl.ComboBox.Items.Remove(InBetween);
                    if (this.Operator == InBetween) this.dateGridFilterControl.ComboBox.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        ///     Clears the filter to its initial state.
        /// </summary>
        public override void Clear()
        {
            this.dateGridFilterControl.ComboBox.SelectedIndex = 0;
            this.dateGridFilterControl.DateTimePicker1.Value = DateTime.Now;
            this.dateGridFilterControl.DateTimePicker2.Value = DateTime.Now;
        }

        /// <summary>
        ///     Gets a filter with the current criteria in string representation.
        /// </summary>
        /// <param name="columnName">
        ///     The name of the column for which the criteria should be generated.
        /// </param>
        /// <returns>A string representing the current filter criteria</returns>
        public override string GetFilter(string columnName)
        {
            if (this.dateGridFilterControl.DateTimePicker1.Text.Length == 0) return string.Empty;
            if (this.dateGridFilterControl.DateTimePicker2.Text.Length == 0) return string.Empty;

            if (this.dateGridFilterControl.ComboBox.Text.Length == 0) return string.Empty;

            try
            {
                if (this.Operator == InBetween)
                    return string.Format(
                        FilterFormatBetween,
                        columnName,
                        this.dateGridFilterControl.DateTimePicker1.Value,
                        this.dateGridFilterControl.DateTimePicker2.Value);
                return string.Format(
                    FilterFormat,
                    columnName,
                    this.dateGridFilterControl.ComboBox.SelectedItem,
                    this.dateGridFilterControl.DateTimePicker1.Value);
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
                this.dateGridFilterControl.ComboBox.SelectedItem = InBetween;
                this.dateGridFilterControl.DateTimePicker1.Value = new DateTime(
                    Convert.ToInt32(match.Groups["Year1"].Value),
                    Convert.ToInt32(match.Groups["Month1"].Value),
                    Convert.ToInt32(match.Groups["Day1"].Value));
                this.dateGridFilterControl.DateTimePicker2.Value = new DateTime(
                    Convert.ToInt32(match.Groups["Year2"].Value),
                    Convert.ToInt32(match.Groups["Month2"].Value),
                    Convert.ToInt32(match.Groups["Day2"].Value));
            }
            else
            {
                regex = new Regex(FilterRegex, RegexOptions.ExplicitCapture);
                if (regex.IsMatch(filter))
                {
                    var match = regex.Match(filter);
                    this.dateGridFilterControl.ComboBox.SelectedItem = match.Groups["Operator"].Value;
                    this.dateGridFilterControl.DateTimePicker1.Value = new DateTime(
                        Convert.ToInt32(match.Groups["Year"].Value),
                        Convert.ToInt32(match.Groups["Month"].Value),
                        Convert.ToInt32(match.Groups["Day"].Value));
                }
            }
        }

        private void OnDateGridFilterControlChanged(object sender, EventArgs e)
        {
            this.OnChanged();
        }
    }
}