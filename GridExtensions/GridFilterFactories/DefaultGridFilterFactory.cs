namespace GridExtensions.GridFilterFactories
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Windows.Forms;

    using GridExtensions.GridFilters;

    /// <summary>
    ///     Default implementation for <see cref="IGridFilterFactory" /> which
    ///     should be applicable for most standard needs.
    ///     The creation process consists of these steps:
    ///     1. If the column data type is an enumeration and <see cref="HandleEnumerationTypes" />
    ///     is set to true than an <see cref="EnumerationGridFilter" /> is created.
    ///     2. If <see cref="CreateDistinctGridFilters" /> is set to true than it is analyzed
    ///     if the column contains less or equal distinct values than specified by
    ///     <see cref="MaximumDistinctValues" />. If yes than an <see cref="DistinctValuesGridFilter" />
    ///     is created. The <see cref="MaximumDistinctValues" /> property is not only
    ///     important to reduce the maximum number of entries the <see cref="ComboBox" />
    ///     gets filled with but also to improve performance because the analysis of the
    ///     columns data will be stopped immediately when more values are found then
    ///     specified by it and thus the analysis doesn't have to search through the whole
    ///     data source.
    ///     3. If a grid filter type is specified for the data type of the column than this
    ///     one will be created. The data type to grid filter type matching can be altered
    ///     by calls to <see cref="AddGridFilter" /> and <see cref="RemoveGridFilter" />. Note
    ///     that only grid filter types which implement <see cref="IGridFilter" /> and which
    ///     have an empty public constructor are allowed.
    ///     4. If still no filter was created than the filter specified by
    ///     <see cref="DefaultGridFilterType" /> will be created. By default this is the
    ///     <see cref="TextGridFilter" />. Note that again only grid filter types which implement
    ///     <see cref="IGridFilter" /> and which have an empty public constructor are allowed.
    /// </summary>
    public class DefaultGridFilterFactory : GridFilterFactoryBase
    {
        private readonly Hashtable hash;

        private bool createDistinctGridFilters;

        private Type defaultGridFilterType;

        private bool defaultShowDateInBetweenOperator;

        private bool defaultShowNumericInBetweenOperator;

        private bool handleEnumerationTypes = true;

        private int maximumDistinctValues = 20;

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        public DefaultGridFilterFactory()
        {
            this.hash = new Hashtable();

            this.DefaultGridFilterType = typeof(TextGridFilter);
            this.AddGridFilter(typeof(bool), typeof(BoolGridFilter));

            this.AddGridFilter(typeof(byte), typeof(NumericGridFilter));
            this.AddGridFilter(typeof(short), typeof(NumericGridFilter));
            this.AddGridFilter(typeof(int), typeof(NumericGridFilter));
            this.AddGridFilter(typeof(long), typeof(NumericGridFilter));

            this.AddGridFilter(typeof(float), typeof(NumericGridFilter));
            this.AddGridFilter(typeof(double), typeof(NumericGridFilter));
            this.AddGridFilter(typeof(decimal), typeof(NumericGridFilter));

            this.AddGridFilter(typeof(ushort), typeof(NumericGridFilter));
            this.AddGridFilter(typeof(ulong), typeof(NumericGridFilter));

            this.AddGridFilter(typeof(DateTime), typeof(DateGridFilter));
        }

        /// <summary>
        ///     Gets or sets whether grid filters of type <see cref="DistinctValuesGridFilter" />
        ///     should be created automatically. Note that this might reduce performance
        ///     as every column is analyzed to get the different values it contains.
        /// </summary>
        public bool CreateDistinctGridFilters
        {
            get => this.createDistinctGridFilters;
            set => this.ConfigureDistinctGridFilteHandling(value, this.maximumDistinctValues);
        }

        /// <summary>
        ///     Gets and sets the <see cref="Type" /> of the IGridFilter which
        ///     should handle all unspecified datatypes.
        /// </summary>
        public Type DefaultGridFilterType
        {
            get => this.defaultGridFilterType;
            set
            {
                this.CheckIfValidGridFilterType(value);
                this.defaultGridFilterType = value;
                this.OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Sets or gets whether created <see cref="DateGridFilter" />s should by default
        ///     show the 'in between' operator.
        /// </summary>
        public bool DefaultShowDateInBetweenOperator
        {
            get => this.defaultShowDateInBetweenOperator;
            set
            {
                if (value == this.defaultShowDateInBetweenOperator) return;
                this.defaultShowDateInBetweenOperator = value;
                this.OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Sets or gets whether created <see cref="NumericGridFilter" />s should by default
        ///     show the 'in between' operator.
        /// </summary>
        public bool DefaultShowNumericInBetweenOperator
        {
            get => this.defaultShowNumericInBetweenOperator;
            set
            {
                if (value == this.defaultShowNumericInBetweenOperator) return;
                this.defaultShowNumericInBetweenOperator = value;
                this.OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Gets/sets whether enumeration types are automatically handled
        ///     with a special <see cref="IGridFilter" /> implementation.
        ///     Only applies for datatypes not explicitely set.
        /// </summary>
        public bool HandleEnumerationTypes
        {
            get => this.handleEnumerationTypes;
            set
            {
                this.handleEnumerationTypes = value;
                this.OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Gets or sets the maximum number of distinct values column should have
        ///     when a <see cref="DistinctValuesGridFilter" /> is created.
        ///     If this limit is exceeded than a standard filter will be created.
        ///     Value is only considered when <see cref="CreateDistinctGridFilters" /> is
        ///     set to true.
        ///     The value set must be set to 1 or greater. If all values contained within
        ///     a column without any limitation should be generated than set this property
        ///     to <see cref="Int32.MaxValue" />.
        /// </summary>
        public int MaximumDistinctValues
        {
            get => this.maximumDistinctValues;
            set => this.ConfigureDistinctGridFilteHandling(this.createDistinctGridFilters, value);
        }

        /// <summary>
        ///     Adds a type for <see cref="IGridFilter" /> for the
        ///     specified datatype.
        /// </summary>
        /// <param name="dataType">
        ///     <see cref="Type" /> for which a special <see cref="IGridFilter" /> should be generated.
        /// </param>
        /// <param name="gridFilterType">
        ///     <see cref="Type" /> of the <see cref="IGridFilter" /> to be generated.
        /// </param>
        public void AddGridFilter(Type dataType, Type gridFilterType)
        {
            this.CheckIfValidGridFilterType(gridFilterType);
            this.hash.Add(dataType, gridFilterType);
            this.OnChanged(EventArgs.Empty);
        }

        /// <summary>
        ///     Sets <see cref="CreateDistinctGridFilters" /> and <see cref="MaximumDistinctValues" />
        ///     simultaneously to improve performance.
        /// </summary>
        /// <param name="createDistinctGridFilters">
        ///     Indicator whether grid filters of type <see cref="DistinctValuesGridFilter" />
        ///     should be created automatically
        /// </param>
        /// <param name="maximumDistinctValues">
        ///     Maximum number of distinct values column should have
        ///     when a <see cref="DistinctValuesGridFilter" /> is created.
        /// </param>
        public void ConfigureDistinctGridFilteHandling(bool createDistinctGridFilters, int maximumDistinctValues)
        {
            if (maximumDistinctValues <= 0)
                throw new ArgumentException("Value must be 1 or greater.", "maximumDistinctValues");
            this.maximumDistinctValues = maximumDistinctValues;

            this.createDistinctGridFilters = createDistinctGridFilters;

            this.OnChanged(EventArgs.Empty);
        }

        /// <summary>
        ///     Removes a specialized type for <see cref="IGridFilter" /> for a given datatype.
        /// </summary>
        /// <param name="dataType">
        ///     <see cref="Type" /> for which a special <see cref="IGridFilter" /> should be removed.
        /// </param>
        public void RemoveGridFilter(Type dataType)
        {
            this.hash.Remove(dataType);
            this.OnChanged(EventArgs.Empty);
        }

        /// <summary>
        ///     Creates a new instance of <see cref="IGridFilter" />.
        ///     The concrete implementation depends on the given datatype.
        ///     The parameters tablename and columnanem are ignored in this implementation.
        /// </summary>
        /// <param name="column">The <see cref="DataColumn" /> for which the filter control should be created.</param>
        /// <param name="columnStyle">The <see cref="DataGridColumnStyle" /> for which the filter control should be created.</param>
        /// <returns>A <see cref="IGridFilter" />.</returns>
        protected override IGridFilter CreateGridFilterInternal(DataColumn column, DataGridColumnStyle columnStyle)
        {
            IGridFilter result = null;

            var dataType = column.DataType;

            if (dataType.IsEnum && this.handleEnumerationTypes)
            {
                result = new EnumerationGridFilter(dataType);
            }
            else if (this.createDistinctGridFilters)
            {
                bool containsDbNull;
                var values =
                    DistinctValuesGridFilter.GetDistinctValues(column, this.maximumDistinctValues, out containsDbNull);
                if (values != null) result = new DistinctValuesGridFilter(values, containsDbNull);
            }

            if (result == null)
                if (this.hash.ContainsKey(dataType))
                {
                    var gridFilterType = this.hash[dataType] as Type;
                    result = gridFilterType.Assembly.CreateInstance(gridFilterType.FullName) as IGridFilter;
                }
                else if (this.defaultGridFilterType != null)
                {
                    result =
                        this.defaultGridFilterType.Assembly.CreateInstance(this.defaultGridFilterType.FullName) as
                            IGridFilter;
                }

            if (result is DateGridFilter)
                (result as DateGridFilter).ShowInBetweenOperator = this.defaultShowDateInBetweenOperator;
            if (result is NumericGridFilter)
                (result as NumericGridFilter).ShowInBetweenOperator = this.defaultShowNumericInBetweenOperator;

            return result;
        }

        private void CheckIfValidGridFilterType(Type gridFilterType)
        {
            if (!gridFilterType.IsClass && !gridFilterType.IsValueType)
                throw new ArgumentException(
                    "Specified grid filter type must be either a class or a struct.",
                    "gridFilterType");

            if (!typeof(IGridFilter).IsAssignableFrom(gridFilterType))
                throw new ArgumentException(
                    "Specified grid filter type does not implement IGridFilter.",
                    "gridFilterType");

            if (gridFilterType.GetConstructor(new Type[0]) == null)
                throw new ArgumentException(
                    "Specified grid filter type does not have an empty public constructor are allowed.",
                    "gridFilterType");
        }
    }
}