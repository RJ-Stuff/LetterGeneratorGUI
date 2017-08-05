namespace GridExtensions
{
    using System;
    using System.Data;
    using System.Windows.Forms;

    /// <summary>
    ///     Interface whichs implementors extend the DataGrid by some extra functionality
    /// </summary>
    internal interface IGridExtension
    {
        /// <summary>
        ///     Gets raised when either <see cref="DataGrid.CaptionBackColor" /> or
        ///     <see cref="DataGrid.CaptionForeColor" /> has changed
        /// </summary>
        event EventHandler CaptionColorsChanged;

        /// <summary>
        ///     Gets the currently visible <see cref="DataView" />.
        ///     Returns null when no <see cref="DataView" /> is set.
        /// </summary>
        DataView CurrentView { get; }

        /// <summary>
        ///     Gets the extended <see cref="DataGrid" />.
        /// </summary>
        DataGrid Grid { get; }

        /// <summary>
        ///     Publishes the <see cref="DataGrid" /> class's property <see cref="DataGrid.HorizScrollBar" />
        /// </summary>
        ScrollBar HorizontalScrollbar { get; }

        /// <summary>
        ///     Publishes the <see cref="DataGrid" /> class's property <see cref="DataGrid.VertScrollBar" />
        /// </summary>
        ScrollBar VerticalScrollbar { get; }
    }
}