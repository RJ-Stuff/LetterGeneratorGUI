namespace GridExtensions.GridFilterFactories
{
    using System.Data;
    using System.Windows.Forms;

    using GridExtensions.GridFilters;

    /// <summary>
    ///     <see cref="IGridFilterFactory" /> implementation which creates a
    ///     <see cref="GridFilters.DistinctValuesGridFilter" /> on every column.
    /// </summary>
    public class DistinctValuesGridFilterFactory : GridFilterFactoryBase
    {
        /// <summary>
        ///     Return always a <see cref="GridFilters.DistinctValuesGridFilter" />.
        /// </summary>
        /// <param name="column">The <see cref="DataColumn" /> for which the filter control should be created.</param>
        /// <param name="columnStyle">The <see cref="DataGridColumnStyle" /> for which the filter control should be created.</param>
        /// <returns>A <see cref="IGridFilter" />.</returns>
        protected override IGridFilter CreateGridFilterInternal(DataColumn column, DataGridColumnStyle columnStyle)
        {
            return new DistinctValuesGridFilter(column);
        }
    }
}