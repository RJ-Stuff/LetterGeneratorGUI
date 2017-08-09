namespace GridExtensions
{
    using System;
    using System.Data;
    using System.Windows.Forms;

    /// <summary>
    ///     Delegate for use with <see cref="GridFilterEventArgs" />.
    /// </summary>
    public delegate void GridFilterEventHandler(object sender, GridFilterEventArgs args);

    /// <summary>
    ///     Argumentsclass for events needing extended informations about <see cref="IGridFilter" />s.
    /// </summary>
    public class GridFilterEventArgs : EventArgs
    {
        /// <summary>
        ///     Creates a new instance
        /// </summary>
        /// <param name="column">Column the <see cref="IGridFilter" /> is created for.</param>
        /// <param name="columnStyle">Column style the <see cref="IGridFilter" /> is created for.</param>
        /// <param name="gridFilter">Default <see cref="IGridFilter" /> instance.</param>
        public GridFilterEventArgs(DataColumn column, DataGridColumnStyle columnStyle, IGridFilter gridFilter)
        {
            this.Column = column;
            this.ColumnStyle = columnStyle;
            this.GridFilter = gridFilter;
        }

        /// <summary>
        ///     The column the <see cref="IGridFilter" /> is created for.
        /// </summary>
        public DataColumn Column { get; }

        /// <summary>
        ///     Name of the column the <see cref="IGridFilter" /> is created for.
        /// </summary>
        public string ColumnName => this.Column.ColumnName;

        /// <summary>
        ///     The column style the <see cref="IGridFilter" /> is created for.
        /// </summary>
        public DataGridColumnStyle ColumnStyle { get; }

        /// <summary>
        ///     Type of the column the <see cref="IGridFilter" /> is created for.
        /// </summary>
        public Type DataType => this.Column.DataType;

        /// <summary>
        ///     Gets/sets the <see cref="IGridFilter" /> which should be used.
        /// </summary>
        public IGridFilter GridFilter { get; set; }

        /// <summary>
        ///     Text of the header of the column the <see cref="IGridFilter" /> is created for.
        /// </summary>
        public string HeaderText => this.ColumnStyle.HeaderText;

        /// <summary>
        ///     The table the <see cref="IGridFilter" /> is created for.
        /// </summary>
        public DataTable Table => this.Column.Table;

        /// <summary>
        ///     Name of the table the <see cref="IGridFilter" /> is created for.
        /// </summary>
        public string TableName => this.Table.TableName;
    }
}