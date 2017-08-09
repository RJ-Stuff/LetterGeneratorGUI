namespace GridExtensions
{
    using System.Data;
    using System.Reflection;

    /// <summary>
    ///     Public Wrapper for the internal DataFilter class in the .Net framework.
    ///     The purpose of this class is to test if single <see cref="DataRow" />s match
    ///     a given filter expression.
    /// </summary>
    public class DataFilter
    {
        private static readonly ConstructorInfo ConstructorInfo;

        private static readonly MethodInfo MethodInvokeInfo;

        private readonly object internalDataFilter;

        static DataFilter()
        {
            var internalDataFilterType = typeof(DataTable).Assembly.GetType("System.Data.DataFilter");
            ConstructorInfo = internalDataFilterType.GetConstructor(
                BindingFlags.Public | BindingFlags.Instance,
                null,
                CallingConventions.Any,
                new[] { typeof(string), typeof(DataTable) },
                null);
            MethodInvokeInfo = internalDataFilterType.GetMethod(
                "Invoke",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(DataRow), typeof(DataRowVersion) },
                null);
        }

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        /// <param name="expression">Filter expression string.</param>
        /// <param name="dataTable"><see cref="DataTable" /> of the rows to be tested.</param>
        public DataFilter(string expression, DataTable dataTable)
        {
            this.internalDataFilter = ConstructorInfo.Invoke(new object[] { expression, dataTable });
        }

        /// <summary>
        ///     Tests whether a single <see cref="DataRow" /> matches the filter expression.
        /// </summary>
        /// <param name="row"><see cref="DataRow" /> to be tested.</param>
        /// <returns>True if the row matches the filter expression, otherwise false.</returns>
        public bool Invoke(DataRow row)
        {
            return this.Invoke(row, DataRowVersion.Default);
        }

        /// <summary>
        ///     Tests whether a single <see cref="DataRow" /> matches the filter expression.
        /// </summary>
        /// <param name="row"><see cref="DataRow" /> to be tested.</param>
        /// <param name="version">The row version to use.</param>
        /// <returns>True if the row matches the filter expression, otherwise false.</returns>
        public bool Invoke(DataRow row, DataRowVersion version)
        {
            return (bool)MethodInvokeInfo.Invoke(this.internalDataFilter, new object[] { row, version });
        }
    }
}