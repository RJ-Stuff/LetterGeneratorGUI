namespace GridExtensions
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Windows.Forms;

    using GridExtensions.GridFilterFactories;

    #region Enum FilterPosition

    /// <summary>
    ///     Enumeration representing the regions where the filter GUI elements
    ///     are shown.
    /// </summary>
    public enum FilterPosition
    {
        /// <summary>
        ///     Filter GUI above the grid.
        /// </summary>
        Top,

        /// <summary>
        ///     Filter GUI beyond the grid.
        /// </summary>
        Bottom,

        /// <summary>
        ///     Filter GUI in the caption of the grid.
        /// </summary>
        Caption,

        /// <summary>
        ///     Turns off the filter
        /// </summary>
        Off
    }

    #endregion

    /// <summary>
    ///     Component which allows <see cref="ExtendedDataGrid" />s to be extended with
    ///     autometed filter functionality.
    /// </summary>
    public class DataGridFilterExtender : Component, ISupportInitialize
    {
        private bool autoAdjustGrid;

        private Container components;

        private Control currentParent;

        private FilterPosition filterPosition = FilterPosition.Caption;

        private GridFiltersControl filters;

        private DataGrid grid;

        private IGridExtension gridExtension;

        private bool initializing;

        /// <summary>
        ///     Creates a new instance
        /// </summary>
        /// <param name="container"></param>
        public DataGridFilterExtender(IContainer container)
        {
            container.Add(this);
            this.InitializeComponent();

            this.filters = new GridFiltersControl();
            this.FilterFactory = new DefaultGridFilterFactory();
        }

        /// <summary>
        ///     Creates a new instance
        /// </summary>
        public DataGridFilterExtender()
        {
            this.InitializeComponent();

            this.filters = new GridFiltersControl();
            this.FilterFactory = new DefaultGridFilterFactory();
        }

        /// <summary>
        ///     Event, which gets fired whenever the filter criteria has been changed.
        /// </summary>
        public event EventHandler AfterFiltersChanged;

        /// <summary>
        ///     Event, which gets fired whenever the filter criteria are going to be changed.
        /// </summary>
        public event EventHandler BeforeFiltersChanging;

        /// <summary>
        ///     Event, which gets fired whenever an <see cref="IGridFilter" /> has been bound
        ///     and thus added to this instance.
        /// </summary>
        public event GridFilterEventHandler GridFilterBound;

        /// <summary>
        ///     Event, which gets fired whenever an <see cref="IGridFilter" /> has been unbound
        ///     and thus removed to this instance.
        /// </summary>
        public event GridFilterEventHandler GridFilterUnbound;

        /// <summary>
        ///     Sets whether the bounds of the extended DataGrid should be
        ///     set automatically depending on where the filters are displayed,
        ///     so that the totally covered area by grid and filters is always
        ///     the same.
        /// </summary>
        /// <remarks>
        ///     This wont function correctly if the grid is docked
        /// </remarks>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description(
            "Sets whether the bounds of the extended DataGrid should be "
            + "set automatically depending on where the filters are displayed, so "
            + "that the totally covered area by grid and filters is always the same.")]
        public bool AutoAdjustGridPosition
        {
            get => this.autoAdjustGrid;
            set
            {
                if (this.autoAdjustGrid == value) return;

                this.autoAdjustGrid = value;

                if (this.autoAdjustGrid) this.AdjustGridPosition(FilterPosition.Off, this.filterPosition);
                else this.AdjustGridPosition(this.filterPosition, FilterPosition.Off);
            }
        }

        /// <summary>
        ///     Gets and sets whether the filter criteria is automatically refreshed when
        ///     changes are made to the filter controls. If set to false then a call to
        ///     <see cref="RefreshFilters" /> is needed to manually refresh the criteria.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(RefreshMode.OnInput)]
        [Description(
            "Specifies if the view automatically refreshes to reflect " + "changes in the grid filter controls.")]
        public RefreshMode AutoRefreshMode
        {
            get => this.filters.AutoRefreshMode;
            set => this.filters.AutoRefreshMode = value;
        }

        /// <summary>
        ///     Gets or sets whether base filters should be used when refreshing
        ///     the filter criteria. Setting it to false will disable the functionality
        ///     while still keeping the base filter strings in the <see cref="BaseFilters" />
        ///     collection intact.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Gets or sets whether base filters should be used when " + "refreshing the filter criteria.")]
        public bool BaseFilterEnabled
        {
            get => this.filters.BaseFilterEnabled;
            set => this.filters.BaseFilterEnabled = value;
        }

        /// <summary>
        ///     Gets or sets which operator should be used to combine the base filter
        ///     with the automatically created filters.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(LogicalOperators.And)]
        [Description(
            "Operator which should be used to combine the base filter " + "with the automatically created filters.")]
        public LogicalOperators BaseFilterOperator
        {
            get => this.filters.BaseFilterOperator;
            set => this.filters.BaseFilterOperator = value;
        }

        /// <summary>
        ///     Gets a modifyable collection which maps <see cref="DataTable.TableName" />s
        ///     to base filter strings which are applied in front of the automatically
        ///     created filter.
        /// </summary>
        /// <remarks>
        ///     The grid contents is not automatically refreshed when modifying this
        ///     collection. A call to <see cref="RefreshFilters" /> is needed for this.
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StringDictionary BaseFilters => this.filters.BaseFilters;

        /// <summary>
        ///     Gets and sets what information is showed to the user
        ///     if an error in the builded filter criterias occurs.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(FilterErrorModes.Off)]
        [Description(
            "Specifies what information is printed to the console "
            + "if an error in the builded filter criterias occurs.")]
        public FilterErrorModes ConsoleErrorMode
        {
            get => this.filters.ConsoleErrorMode;
            set => this.filters.ConsoleErrorMode = value;
        }

        /// <summary>
        ///     The bounds of the control with the GUI for filtering
        /// </summary>
        [Browsable(false)]
        public Rectangle ControlBounds => this.filters?.Bounds ?? Rectangle.Empty;

        /// <summary>
        ///     Gets or sets the currently used base filter. Internally it adjusts the
        ///     <see cref="BaseFilters" /> collection with the given value and the current
        ///     <see cref="DataTable.TableName" /> and also initiates a refresh.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string CurrentTableBaseFilter
        {
            get => this.filters.CurrentTableBaseFilter;
            set => this.filters.CurrentTableBaseFilter = value;
        }

        /// <summary>
        ///     Gets and sets the grid which should be extended.
        /// </summary>
        [Browsable(true)]
        [Description("Gets and sets the grid which should be extended.")]
        public DataGrid DataGrid
        {
            get => this.grid;
            set
            {
                if (this.grid == value)
                {
                    return;
                }

                this.RemoveFilterControl();
                if (this.grid != null)
                {
                    this.grid.LocationChanged -= this.OnGridLocationChanged;
                    this.grid.Resize -= this.OnGridResize;
                    this.grid.ParentChanged -= this.OnGridParentChanged;
                    this.gridExtension.CaptionColorsChanged -= this.OnColorsChanged;
                    this.grid.CaptionVisibleChanged -= this.OnGridCaptionVisibleChanged;
                }

                this.grid = value;
                if (this.grid is IGridExtension) this.gridExtension = (IGridExtension)this.grid;
                else this.gridExtension = new DataGridExtension(this.grid);

                this.filters.GridExtension = this.gridExtension;

                this.AdjustFilterControlToGrid();
                this.AddFilterControl();

                if (this.autoAdjustGrid) this.AdjustGridPosition(FilterPosition.Off, this.filterPosition);

                this.grid.LocationChanged += this.OnGridLocationChanged;
                this.grid.Resize += this.OnGridResize;
                this.grid.ParentChanged += this.OnGridParentChanged;
                this.gridExtension.CaptionColorsChanged += this.OnColorsChanged;
                this.grid.CaptionVisibleChanged += this.OnGridCaptionVisibleChanged;
            }
        }

        /// <summary>
        ///     Gets and sets the position of the filter GUI elements.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(FilterPosition.Caption)]
        [Description("Gets and sets the position of the filter GUI elements.")]
        public FilterPosition FilterBoxPosition
        {
            get => this.filterPosition;
            set
            {
                if (this.filterPosition == value) return;

                if (this.autoAdjustGrid) this.AdjustGridPosition(this.filterPosition, value);

                this.filterPosition = value;
                this.AdjustFilterControlToGrid();
            }
        }

        /// <summary>
        ///     Gets and sets the <see cref="IGridFilterFactory" /> used to generate the filter GUI.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(null)]
        public IGridFilterFactory FilterFactory
        {
            get => this.filters.FilterFactory;
            set => this.filters.FilterFactory = value;
        }

        /// <summary>
        ///     Gets and sets the text for the filter label.
        /// </summary>
        [Browsable(true)]
        [DefaultValue("Filter")]
        [Description("Gets and sets the text for the filter label.")]
        public string FilterText
        {
            get => this.filters.FilterText;
            set => this.filters.FilterText = value;
        }

        /// <summary>
        ///     Gets and sets whether the filter label should be visible.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Gets and sets whether the filter label should be visible.")]
        public bool FilterTextVisible
        {
            get => this.filters.FilterTextVisible;
            set => this.filters.FilterTextVisible = value;
        }

        /// <summary>
        ///     Gets and sets the mode for the grid.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Specifies whether the table gets filtered or matching rows get highlighted.")]
        public GridMode GridMode
        {
            get => this.filters.GridMode;
            set => this.filters.GridMode = value;
        }

        /// <summary>
        ///     Gets and sets whether filters are kept while switching between different tables.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Specifies whether filters are kept while switching between different tables.")]
        public bool KeepFilters
        {
            get => this.filters.KeepFilters;
            set => this.filters.KeepFilters = value;
        }

        /// <summary>
        ///     Gets and sets what information is showed to the user
        ///     if an error in the builded filter criterias occurs.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(FilterErrorModes.General)]
        [Description(
            "Specifies what information is shown to the user " + "if an error in the builded filter criterias occurs.")]
        public FilterErrorModes MessageErrorMode
        {
            get => this.filters.MessageErrorMode;
            set => this.filters.MessageErrorMode = value;
        }

        /// <summary>
        ///     The Height of the control which is positioned for filtering
        /// </summary>
        [Browsable(false)]
        public int NeededControlHeight => this.filters?.Height ?? 0;

        /// <summary>
        ///     The selected operator to combine the filter criterias.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(LogicalOperators.And)]
        public LogicalOperators Operator
        {
            get => this.filters.Operator;
            set => this.filters.Operator = value;
        }

        /// <summary>
        ///     Sets a flag to true representing that the component is now initializing.
        /// </summary>
        /// <remarks>
        ///     This is important as the component must know if the properties are set within
        ///     the designer generated code so that no abnormal moving of the contained grid occurs
        ///     when AutoAdjustGridPosition is set to true
        /// </remarks>
        public void BeginInit()
        {
            this.initializing = true;
        }

        /// <summary>
        ///     Clears all filters to initial state.
        /// </summary>
        public void ClearFilters()
        {
            this.filters.ClearFilters();
        }

        /// <summary>
        ///     Sets a flag to false representing that the initialization of the
        ///     component has completed
        /// </summary>
        public void EndInit()
        {
            this.initializing = false;
        }

        /// <summary>
        ///     Gets all filters currently set
        /// </summary>
        /// <returns></returns>
        public string[] GetFilters()
        {
            return this.filters.GetFilters();
        }

        /// <summary>
        ///     Gets all currently set <see cref="IGridFilter" />s.
        /// </summary>
        /// <returns>Collection of <see cref="IGridFilter" />s.</returns>
        public GridFilterCollection GetGridFilters()
        {
            return this.filters.GetGridFilters();
        }

        /// <summary>
        ///     Refreshes the filter criteria to match the current contents of the associated
        ///     filter controls.
        /// </summary>
        public void RefreshFilters()
        {
            this.filters.RefreshFilters();
        }

        /// <summary>
        ///     Sets all filters to the specified values.
        ///     The values must be in order of the column styles in the current view.
        ///     This function should normally be used with data previously coming
        ///     from the <see cref="GetFilters" /> function.
        /// </summary>
        /// <param name="filters">filters to set</param>
        public void SetFilters(string[] filters)
        {
            if (filters != null) this.filters.SetFilters(filters);
        }

        /// <summary>
        ///     Verwendete Ressourcen bereinigen.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (this.filters != null)
            {
                this.RemoveFilterControl();
                this.filters.Dispose();
                this.filters = null;
            }

            if (disposing) this.components?.Dispose();

            base.Dispose(disposing);
        }

        /// <summary>
        ///     Raises the <see cref="AfterFiltersChanged" /> event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnAfterFiltersChanged(EventArgs e)
        {
            this.AfterFiltersChanged?.Invoke(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="BeforeFiltersChanging" /> event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnBeforeFiltersChanging(EventArgs e)
        {
            this.BeforeFiltersChanging?.Invoke(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="GridFilterBound" /> event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnGridFilterBound(GridFilterEventArgs e)
        {
            this.GridFilterBound?.Invoke(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="GridFilterUnbound" /> event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnGridFilterUnbound(GridFilterEventArgs e)
        {
            this.GridFilterUnbound?.Invoke(this, e);
        }

        private void AddFilterControl()
        {
            this.RemoveFilterControl();

            if (this.grid.Parent != null)
            {
                if (this.currentParent != null)
                {
                    this.currentParent.BackColorChanged -= this.OnColorsChanged;
                    this.currentParent.ForeColorChanged -= this.OnColorsChanged;
                }

                this.currentParent = this.grid.Parent;
                this.currentParent.BackColorChanged += this.OnColorsChanged;
                this.currentParent.ForeColorChanged += this.OnColorsChanged;
                this.grid.Parent.Controls.Add(this.filters);
                this.filters.BringToFront();
                this.filters.AfterFiltersChanged += this.OnAfterFiltersChanged;
                this.filters.BeforeFiltersChanging += this.OnBeforeFiltersChanging;
                this.filters.GridFilterBound += this.OnGridFilterBound;
                this.filters.GridFilterUnbound += this.OnGridFilterUnbound;
            }

            this.AdjustFilterControlToGrid();
        }

        private void AdjustFilterControlToGrid()
        {
            if (this.grid == null || this.filters == null || this.grid.Parent == null) return;

            switch (this.filterPosition)
            {
                case FilterPosition.Top:
                    this.filters.Top = this.grid.Top - this.filters.Height;
                    this.filters.Left = this.grid.Left;
                    this.filters.Width = this.grid.Width;
                    this.filters.BackColor = this.grid.Parent.BackColor;
                    this.filters.ForeColor = this.grid.Parent.ForeColor;
                    this.filters.Visible = true;
                    break;
                case FilterPosition.Bottom:
                    this.filters.Top = this.grid.Bottom + 1;
                    this.filters.Left = this.grid.Left;
                    this.filters.Width = this.grid.Width;
                    this.filters.BackColor = this.grid.Parent.BackColor;
                    this.filters.ForeColor = this.grid.Parent.ForeColor;
                    this.filters.Visible = true;
                    break;
                case FilterPosition.Caption:
                    this.filters.Top = this.grid.Top;
                    this.filters.Left = this.grid.Left;
                    this.filters.Width = this.grid.Width;
                    this.filters.BackColor = this.grid.CaptionBackColor;
                    this.filters.ForeColor = this.grid.CaptionForeColor;
                    this.filters.Visible = true;
                    this.grid.CaptionVisible = true;
                    break;
                default:
                    this.filters.Visible = false;
                    break;
            }
        }

        private void AdjustGridPosition(FilterPosition fromPosition, FilterPosition toPosition)
        {
            if (this.grid == null || this.filters == null || fromPosition == toPosition) return;

            if (this.initializing) return;

            var newTop = this.grid.Top;
            var newHeight = this.grid.Height;

            switch (fromPosition)
            {
                case FilterPosition.Bottom:
                    newHeight += this.filters.Height;
                    break;
                case FilterPosition.Top:
                    newTop -= this.filters.Height;
                    newHeight += this.filters.Height;
                    break;
            }

            switch (toPosition)
            {
                case FilterPosition.Bottom:
                    newHeight -= this.filters.Height;
                    break;
                case FilterPosition.Top:
                    newTop += this.filters.Height;
                    newHeight -= this.filters.Height;
                    break;
            }

            var oldStyle = this.grid.Anchor;
            this.grid.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            this.grid.SetBounds(0, newTop, 0, newHeight, BoundsSpecified.Y | BoundsSpecified.Height);
            this.grid.Anchor = oldStyle;
        }

        /// <summary>
        ///     Erforderliche Methode für die Designerunterstützung.
        ///     Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new Container();
        }

        private void OnAfterFiltersChanged(object sender, EventArgs e)
        {
            this.OnAfterFiltersChanged(e);
        }

        private void OnBeforeFiltersChanging(object sender, EventArgs e)
        {
            this.OnBeforeFiltersChanging(e);
        }

        private void OnColorsChanged(object sender, EventArgs e)
        {
            this.AdjustFilterControlToGrid();
        }

        private void OnGridCaptionVisibleChanged(object sender, EventArgs e)
        {
            if (!this.grid.CaptionVisible && this.filterPosition == FilterPosition.Caption)
                this.FilterBoxPosition = FilterPosition.Off;
        }

        private void OnGridFilterBound(object sender, GridFilterEventArgs e)
        {
            this.OnGridFilterBound(e);
        }

        private void OnGridFilterUnbound(object sender, GridFilterEventArgs e)
        {
            this.OnGridFilterUnbound(e);
        }

        private void OnGridLocationChanged(object sender, EventArgs e)
        {
            this.AdjustFilterControlToGrid();
        }

        private void OnGridParentChanged(object sender, EventArgs e)
        {
            this.AddFilterControl();
        }

        private void OnGridResize(object sender, EventArgs e)
        {
            this.AdjustFilterControlToGrid();
        }

        private void RemoveFilterControl()
        {
            if (this.currentParent != null)
            {
                this.filters.AfterFiltersChanged -= this.OnAfterFiltersChanged;
                this.filters.BeforeFiltersChanging -= this.OnBeforeFiltersChanging;
                this.filters.GridFilterBound -= this.OnGridFilterBound;
                this.filters.GridFilterUnbound -= this.OnGridFilterUnbound;
                this.currentParent.Controls.Remove(this.filters);
                this.currentParent.BackColorChanged -= this.OnColorsChanged;
                this.currentParent.ForeColorChanged -= this.OnColorsChanged;
                this.currentParent = null;
            }
        }
    }
}