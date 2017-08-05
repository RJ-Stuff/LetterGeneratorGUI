namespace GridExtensions
{
    using System.Data;
    using System.Windows.Forms;

    /// <summary>
    ///     Class with static methods to automatically generate
    ///     table and column styles
    /// </summary>
    public abstract class DataGridStyleCreator
    {
        private DataGridStyleCreator()
        {
        }

        /// <summary>
        ///     Creates a <see cref="DataGridColumnStyle" /> based on its data type.
        /// </summary>
        /// <param name="column">
        ///     The <see cref="DataColumn" /> for which a style should be generated
        /// </param>
        /// <returns>A column style</returns>
        public static DataGridColumnStyle CreateColumnStyle(DataColumn column)
        {
            return CreateColumnStyle(column, null);
        }

        /// <summary>
        ///     Creates a <see cref="DataGridColumnStyle" /> based on its data type.
        ///     If a grid is specified than its settings will be used for initial
        ///     column style settings.
        /// </summary>
        /// <param name="column">
        ///     The <see cref="DataColumn" /> for which a style should be generated
        /// </param>
        /// <param name="grid">The <see cref="DataGrid" /> with default settings</param>
        /// <returns>A column style</returns>
        public static DataGridColumnStyle CreateColumnStyle(DataColumn column, DataGrid grid)
        {
            DataGridColumnStyle columnStyle;
            if (column.DataType == typeof(bool)) columnStyle = new DataGridBoolColumn();
            else columnStyle = new DataGridTextBoxColumn();

            columnStyle.MappingName = column.ColumnName;
            columnStyle.HeaderText = column.ColumnName;
            if (grid != null)
            {
                columnStyle.Width = grid.PreferredColumnWidth;
                columnStyle.ReadOnly = grid.ReadOnly;
            }

            return columnStyle;
        }

        /// <summary>
        ///     Creates a table style for the specified table.
        /// </summary>
        /// <param name="table">
        ///     The <see cref="DataTable" /> for which a style should be generated
        /// </param>
        /// <returns>The newly generated style.</returns>
        public static DataGridTableStyle CreateTableStyle(DataTable table)
        {
            return CreateTableStyle(table, null, false);
        }

        /// <summary>
        ///     Creates a table style for the specified table.
        ///     If a grid is specified than its settings will be used for initial
        ///     column style settings.
        /// </summary>
        /// <param name="table">
        ///     The <see cref="DataTable" /> for which a style should be generated
        /// </param>
        /// <param name="grid">The <see cref="DataGrid" /> with default settings</param>
        /// <returns>The newly generated style.</returns>
        public static DataGridTableStyle CreateTableStyle(DataTable table, DataGrid grid)
        {
            return CreateTableStyle(table, grid, false);
        }

        /// <summary>
        ///     Creates a table style for the specified table.
        ///     If a grid is specified than its settings will be used for initial
        ///     column style settings.
        /// </summary>
        /// <param name="table">
        ///     The <see cref="DataTable" /> for which a style should be generated
        /// </param>
        /// <param name="grid">The <see cref="DataGrid" /> with default settings</param>
        /// <param name="addToGrid">
        ///     Tells whether the generated style should automatically be added to the
        ///     grid.
        /// </param>
        /// <returns>The newly generated style.</returns>
        public static DataGridTableStyle CreateTableStyle(DataTable table, DataGrid grid, bool addToGrid)
        {
            var tableStyle = new DataGridTableStyle { MappingName = table.TableName };
            foreach (DataColumn column in table.Columns)
                tableStyle.GridColumnStyles.Add(CreateColumnStyle(column, grid));

            if (addToGrid) grid.TableStyles.Add(tableStyle);

            return tableStyle;
        }
    }
}