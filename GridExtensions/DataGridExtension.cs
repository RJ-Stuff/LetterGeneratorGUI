namespace GridExtensions
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Reflection;
    using System.Windows.Forms;

    /// <summary>
    ///     Implementation of <see cref="IGridExtension" /> to extend a foreign
    ///     instance of <see cref="DataGrid" />. This is done by calling
    ///     protected properties of the grid by using reflection.
    /// </summary>
    internal class DataGridExtension : IGridExtension
    {
        private readonly Color lastCaptionBackColor = Color.Empty;

        private readonly Color lastCaptionForeColor = Color.Empty;

        /// <summary>
        ///     Creates a new instance
        /// </summary>
        /// <param name="grid">The instance to be enhanced</param>
        internal DataGridExtension(DataGrid grid)
        {
            this.Grid = grid;
            this.Grid.Invalidated += this.OnGridInvalidated;
        }

        /// <summary>
        ///     Gets raised when either <see cref="DataGrid.CaptionBackColor" /> or
        ///     <see cref="DataGrid.CaptionForeColor" /> has changed
        /// </summary>
        public event EventHandler CaptionColorsChanged;

        /// <summary>
        ///     Gets the currently visible <see cref="DataView" />.
        ///     Returns null when no <see cref="DataView" /> is set.
        /// </summary>
        public DataView CurrentView
        {
            get
            {
                var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                            | BindingFlags.IgnoreCase;
                var info = typeof(DataGrid).GetProperty("ListManager", flags);
                var manager = info.GetValue(this.Grid, null) as CurrencyManager;

                return manager?.List as DataView;
            }
        }

        /// <summary>
        ///     Gets the extended <see cref="DataGrid" />.
        /// </summary>
        public DataGrid Grid { get; }

        /// <summary>
        ///     Publishes the <see cref="DataGrid" /> class's property <see cref="DataGrid.HorizScrollBar" />
        /// </summary>
        public ScrollBar HorizontalScrollbar
        {
            get
            {
                var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                            | BindingFlags.IgnoreCase;
                var info = typeof(DataGrid).GetProperty("HorizScrollBar", flags);
                var result = info.GetValue(this.Grid, null);
                return result as ScrollBar;
            }
        }

        /// <summary>
        ///     Publishes the <see cref="DataGrid" /> class's property <see cref="DataGrid.VertScrollBar" />
        /// </summary>
        public ScrollBar VerticalScrollbar
        {
            get
            {
                var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                            | BindingFlags.IgnoreCase;
                var info = typeof(DataGrid).GetProperty("VertScrollBar", flags);
                var result = info.GetValue(this.Grid, null);
                return result as ScrollBar;
            }
        }

        private void OnGridInvalidated(object sender, InvalidateEventArgs e)
        {
            if (this.lastCaptionBackColor != this.Grid.CaptionBackColor
                || this.lastCaptionForeColor != this.Grid.CaptionForeColor)
                this.CaptionColorsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}