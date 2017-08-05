namespace GridExtensions.GridFilterFactories
{
    using System;
    using System.Data;
    using System.Windows.Forms;

    using GridExtensions.GridFilters;

    /// <summary>
    ///     Implementation of <see cref="IGridFilterFactory" /> allowing
    ///     to search in multiple columns with only one textbox. Without
    ///     further customization this results in a fulltext-search in the
    ///     whole grid.
    /// </summary>
    public class FullTextSearchGridFilterFactoryTextBox : TextBox, IGridFilterFactory
    {
        /// <summary>
        ///     Event for notification that the behaviour of this
        ///     instance has changed.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        ///     Event for notification when a <see cref="IGridFilter" /> has been
        ///     created in order to use it in a specific column and to allow
        ///     custom modifications to it.
        /// </summary>
        public event GridFilterEventHandler GridFilterCreated;

        /// <summary>
        ///     Notifies this instance that the <see cref="IGridFilter" /> creation process
        ///     is being started.
        /// </summary>
        public void BeginGridFilterCreation()
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="TextGridFilter" /> and always
        ///     specifies itself as the filter control. As a result all created filters
        ///     will react upon changes in this instance.
        /// </summary>
        /// <param name="column">The <see cref="DataColumn" /> for which the filter control should be created.</param>
        /// <param name="columnStyle">The <see cref="DataGridColumnStyle" /> for which the filter control should be created.</param>
        /// <returns>A <see cref="TextGridFilter" />.</returns>
        public IGridFilter CreateGridFilter(DataColumn column, DataGridColumnStyle columnStyle)
        {
            IGridFilter result = new TextGridFilter(this);
            this.OnGridFilterCreated(new GridFilterEventArgs(column, columnStyle, result));
            return result;
        }

        /// <summary>
        ///     Notifies this instance that the <see cref="IGridFilter" /> creation process
        ///     has finished. After this call all created <see cref="IGridFilter" />s should
        ///     be in a usable state.
        /// </summary>
        public void EndGridFilterCreation()
        {
        }

        private void OnChanged()
        {
            this.Changed?.Invoke(this, EventArgs.Empty);
        }

        private void OnGridFilterCreated(GridFilterEventArgs gridFilterEventArgs)
        {
            this.GridFilterCreated?.Invoke(this, gridFilterEventArgs);
        }
    }
}