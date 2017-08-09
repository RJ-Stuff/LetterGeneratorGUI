namespace GridExtensions.GridFilters.EnumerationSources
{
    using System;
    using System.Collections;

    /// <summary>
    ///     <see cref="IEnumerationSource" /> implementation which supports userdefined
    ///     matching between <see cref="int" /> values in the datasource and <see cref="string" />
    ///     values which should be displayed in the filter.
    /// </summary>
    public class IntStringMapEnumerationSource : IEnumerationSource
    {
        private readonly Hashtable hash;

        private object[] allValues;

        /// <summary>
        ///     Creates a new instance with no mapping.
        /// </summary>
        public IntStringMapEnumerationSource()
        {
            this.hash = new Hashtable();
        }

        /// <summary>
        ///     Creates a new instance mapping the given <see cref="int" /> values to
        ///     the given <see cref="string" /> values.
        /// </summary>
        /// <param name="integerValues"></param>
        /// <param name="stringValues"></param>
        public IntStringMapEnumerationSource(int[] integerValues, string[] stringValues)
            : this()
        {
            if (integerValues.Length != stringValues.Length)
                throw new ArgumentException("Number of integers and strings must match.");

            for (var i = 0; i < integerValues.Length; i++) this.hash.Add(stringValues[i], integerValues[i]);
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
                    var keys = this.hash.Keys;
                    this.allValues = new object[keys.Count];
                    keys.CopyTo(this.allValues, 0);
                }

                return this.allValues;
            }
        }

        /// <summary>
        ///     Adds a mapping
        /// </summary>
        /// <param name="integerValue"></param>
        /// <param name="stringValue"></param>
        public void AddMapping(int integerValue, string stringValue)
        {
            this.hash.Add(stringValue, integerValue);
            this.allValues = null;
        }

        /// <summary>
        ///     Build the filter criteria from the given input.
        /// </summary>
        /// <param name="value">The selected value for which the criteria is created.</param>
        /// <returns>A <see cref="string" /> representing the criteria.</returns>
        public string GetFilterFromValue(object value)
        {
            return this.hash[value].ToString();
        }

        /// <summary>
        ///     Gets the object value for a specified filter.
        /// </summary>
        /// <param name="filter">The filter value to be searched</param>
        /// <returns>object value for the specified filter</returns>
        public object GetValueFromFilter(string filter)
        {
            var integerValue = Convert.ToInt32(filter);

            foreach (string stringValue in this.AllValues)
                if ((int)this.hash[stringValue] == integerValue) return stringValue;

            throw new ArgumentException("Unexpected filter.", nameof(filter));
        }

        /// <summary>
        ///     Removes a mapping
        /// </summary>
        /// <param name="stringValue"></param>
        public void RemoveMapping(string stringValue)
        {
            this.hash.Remove(stringValue);
            this.allValues = null;
        }
    }
}