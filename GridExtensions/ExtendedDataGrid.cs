namespace GridExtensions
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    ///     From <see cref="DataGrid" /> deriven class which provides some extra functionalities.
    /// </summary>
    public class ExtendedDataGrid : DataGrid, IGridExtension
    {
        private readonly Color lastCaptionBackColor = Color.Empty;

        private readonly Color lastCaptionForeColor = Color.Empty;

        /// <summary>
        ///     Gets raised when either <see cref="DataGrid.CaptionBackColor" /> or
        ///     <see cref="DataGrid.CaptionForeColor" /> has changed
        /// </summary>
        [Browsable(true)]
        [Description("Gets fired when the forecolor or the backcolor of the caption have changed.")]
        public event EventHandler CaptionColorsChanged;

        /// <summary>
        ///     Controls whether TableStyles are automatically generated.
        /// </summary>
        [Browsable(true)]
        [Description("Controls whether TableStyles are automatically generated.")]
        public bool AutoCreateTableStyles { get; set; }

        /// <summary>
        ///     Gets the currently visible <see cref="DataView" />.
        ///     Returns null when no <see cref="DataView" /> is set.
        /// </summary>
        [Browsable(false)]
        public DataView CurrentView => this.ListManager?.List as DataView;

        /// <summary>
        ///     Returns the instance itself
        /// </summary>
        [Browsable(false)]
        public DataGrid Grid => this;

        /// <summary>
        ///     Publishes the <see cref="DataGrid" /> class's property <see cref="DataGrid.HorizScrollBar" />
        /// </summary>
        [Browsable(false)]
        public ScrollBar HorizontalScrollbar => this.HorizScrollBar;

        /// <summary>
        ///     Publishes the <see cref="DataGrid" /> class's property <see cref="DataGrid.VertScrollBar" />
        /// </summary>
        [Browsable(false)]
        public ScrollBar VerticalScrollbar => this.VertScrollBar;

        /// <summary>
        ///     If <see cref="AutoCreateTableStyles" /> is set to true and no
        ///     <see cref="DataGridTableStyle" /> is set for the current visible table
        ///     then this method will automatically create a default style.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDataSourceChanged(EventArgs e)
        {
            if (this.CurrentView != null && this.AutoCreateTableStyles)
                if (!this.TableStyles.Contains(this.CurrentView.Table.TableName))
                    this.CreateDefaultTableStyle(this.CurrentView.Table);

            base.OnDataSourceChanged(e);
        }

        /// <summary>
        ///     Raises the <see cref="CaptionColorsChanged" /> event when
        ///     <see cref="DataGrid.CaptionBackColor" /> or
        ///     <see cref="DataGrid.CaptionForeColor" /> have changed
        /// </summary>
        /// <param name="e">event arguments</param>
        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);

            if (this.lastCaptionBackColor != this.CaptionBackColor || this.lastCaptionForeColor != this.CaptionForeColor
            ) this.CaptionColorsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Method for automatically generating a <see cref="DataGridTableStyle" /> for
        ///     a given table and adds it to the list of table styles.
        /// </summary>
        /// <param name="table">
        ///     <see cref="DataTable" /> for which the <see cref="DataGridTableStyle" /> should be generated.
        /// </param>
        private void CreateDefaultTableStyle(DataTable table)
        {
            DataGridStyleCreator.CreateTableStyle(table, this, true);
        }
    }
}