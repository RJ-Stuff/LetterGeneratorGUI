namespace GridExtensions.GridFilters.EnumerationSources
{
    using System;

    /// <summary>
    ///     <see cref="IEnumerationSource" /> implementation which gets its values from
    ///     an enumeration type via reflection.
    /// </summary>
    public class TypeEnumerationSource : IEnumerationSource
    {
        private readonly Type enumType;

        private object[] allValues;

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        /// <param name="dataType">Enumeration type</param>
        public TypeEnumerationSource(Type dataType)
        {
            if (!dataType.IsEnum) throw new ArgumentException("Only enumeration types are valid arguments.");

            this.enumType = dataType;
        }

        /// <summary>
        ///     Gets all values which should be displayed.
        /// </summary>
        public object[] AllValues
        {
            get
            {
                if (this.allValues == null)
                {
                    var arr = Enum.GetValues(this.enumType);
                    this.allValues = new object[arr.Length];
                    arr.CopyTo(this.allValues, 0);
                }

                return this.allValues;
            }
        }

        /// <summary>
        ///     Build the filter criteria from the given input.
        /// </summary>
        /// <param name="value">The selected value for which the criteria is created.</param>
        /// <returns>A <see cref="string" /> representing the criteria.</returns>
        public string GetFilterFromValue(object value)
        {
            return Convert.ToInt32(value).ToString();
        }

        /// <summary>
        ///     Gets the object value for a specified filter.
        /// </summary>
        /// <param name="filter">The filter value to be searched</param>
        /// <returns>object value for the specified filter</returns>
        public object GetValueFromFilter(string filter)
        {
            return Enum.ToObject(this.enumType, Convert.ToInt32(filter));
        }
    }
}