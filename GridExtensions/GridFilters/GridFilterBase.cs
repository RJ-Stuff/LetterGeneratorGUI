namespace GridExtensions.GridFilters
{
    using System;
    using System.Data;
    using System.Windows.Forms;

    /// <summary>
    ///     Base class for easier <see cref="IGridFilter" /> implementation.
    /// </summary>
    public abstract class GridFilterBase : IGridFilter
    {
        /// <summary>
        ///     Base constructor.
        /// </summary>
        /// <param name="useCustomFilterPlacement">
        ///     False, if the filter control should be
        ///     placed within the grid, otherwise true.
        /// </param>
        protected GridFilterBase(bool useCustomFilterPlacement)
        {
            this.UseCustomFilterPlacement = useCustomFilterPlacement;
        }

        /// <summary>
        ///     Event for notification that the filter criteria for this
        ///     instance has changed.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        ///     The control which contains the GUI elements for the filter
        /// </summary>
        public abstract Control FilterControl { get; }

        /// <summary>
        ///     Gets whether a filter is currently set
        /// </summary>
        public abstract bool HasFilter { get; }

        /// <summary>
        ///     Specifies whether the control is placed automatically or not.
        /// </summary>
        public bool UseCustomFilterPlacement { get; set; }

        /// <summary>
        ///     Clears the filter to its initial state.
        /// </summary>
        public abstract void Clear();

        /// <summary>
        ///     Frees the resources of this instance.
        ///     Not needed in the base implementation but probably a good thing
        ///     in deriving classes.
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        ///     Gets a string representing the current filter.
        ///     This must be a valid expression understandable by the
        ///     <see cref="DataView" /> class's property <see cref="DataView.RowFilter" />.
        /// </summary>
        /// <param name="columnName">
        ///     The name of the column for which the criteria should be generated.
        /// </param>
        /// <returns>a string representing the current filter criteria</returns>
        public abstract string GetFilter(string columnName);

        /// <summary>
        ///     Sets a string which a a previous result of <see cref="GetFilter" />
        ///     in order to configure the <see cref="FilterControl" /> to match the
        ///     given filter criteria.
        /// </summary>
        /// <param name="filter">filter criteria</param>
        /// <returns></returns>
        public abstract void SetFilter(string filter);

        /// <summary>
        ///     Fires the <see cref="Changed" /> event.
        /// </summary>
        protected void OnChanged()
        {
            this.Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}