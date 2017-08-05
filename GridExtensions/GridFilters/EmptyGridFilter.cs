namespace GridExtensions.GridFilters
{
    using System.Windows.Forms;

    /// <summary>
    ///     A dummy <see cref="IGridFilter" /> implementation, which does no filtering.
    /// </summary>
    public class EmptyGridFilter : GridFilterBase
    {
        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        public EmptyGridFilter()
            : base(false)
        {
            this.FilterControl = new Control();
        }

        /// <summary>
        ///     Gets an empty control.
        /// </summary>
        public override Control FilterControl { get; }

        /// <summary>
        ///     Always return false.
        /// </summary>
        public override bool HasFilter => false;

        /// <summary>
        ///     Clears the filter to its initial state.
        /// </summary>
        public override void Clear()
        {
        }

        /// <summary>
        ///     Always returns an empty string.
        /// </summary>
        /// <param name="columnName">Not necessary.</param>
        /// <returns>An empty string.</returns>
        public override string GetFilter(string columnName)
        {
            return string.Empty;
        }

        /// <summary>
        ///     Does nothing.
        /// </summary>
        /// <param name="filter">filter criteria</param>
        /// <returns></returns>
        public override void SetFilter(string filter)
        {
        }
    }
}