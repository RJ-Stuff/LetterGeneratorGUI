namespace GridExtensions
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    ///     Control which embeds an <see cref="ExtendedDataGrid" /> and a
    ///     <see cref="DataGridFilterExtender" /> for providing automatic
    ///     filtering on all visible columns.
    /// </summary>
    public class FilterableDataGrid : UserControl
    {
        private IContainer components;

        private DataGridFilterExtender extender;

        private ExtendedDataGrid grid;

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        public FilterableDataGrid()
        {
            this.InitializeComponent();

            this.RepositionGrid();
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
        ///     Controls whether TableStyles are automatically generated.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Controls whether TableStyles are automatically generated.")]
        public bool AutoCreateTableStyles
        {
            get => this.grid.AutoCreateTableStyles;
            set => this.grid.AutoCreateTableStyles = value;
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
            get => this.extender.AutoRefreshMode;
            set => this.extender.AutoRefreshMode = value;
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
            get => this.extender.BaseFilterEnabled;
            set => this.extender.BaseFilterEnabled = value;
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
            get => this.extender.BaseFilterOperator;
            set => this.extender.BaseFilterOperator = value;
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
        public StringDictionary BaseFilters => this.extender.BaseFilters;

        /// <summary>
        ///     Gets and sets what information is printed to the console
        ///     if an error in the builded filter criterias occurs.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(FilterErrorModes.Off)]
        [Description(
            "Specifies what information is printed to the console "
            + "if an error in the builded filter criterias occurs.")]
        public FilterErrorModes ConsoleErrorMode
        {
            get => this.extender.ConsoleErrorMode;
            set => this.extender.ConsoleErrorMode = value;
        }

        /// <summary>
        ///     Gets or sets the currently used base filter. Internally it adjusts the
        ///     <see cref="BaseFilters" /> collection with the given value and the current
        ///     <see cref="DataTable.TableName" /> and also initiates a refresh.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string CurrentTableBaseFilter
        {
            get => this.extender.CurrentTableBaseFilter;
            set => this.extender.CurrentTableBaseFilter = value;
        }

        /// <summary>
        ///     Gets and sets the <see cref="DataView" /> which should be displayed in the grid.
        ///     This is needed because only <see cref="DataView" />s provide in built mechanisms
        ///     to filter their content.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(null)]
        [Description("The DataView which should be initially displayed.")]
        public DataView DataSource
        {
            get => this.grid.DataSource as DataView;
            set => this.grid.DataSource = value;
        }

        /// <summary>
        ///     Publishes the embedded <see cref="ExtendedDataGrid" /> to allow
        ///     full control over its settings.
        /// </summary>
        [Browsable(false)]
        public ExtendedDataGrid EmbeddedDataGrid => this.grid;

        /// <summary>
        ///     Gets and sets the poisiton of the filter GUI elements.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(FilterPosition.Top)]
        [Description("Gets and sets the position of the filter GUI elements.")]
        public FilterPosition FilterBoxPosition
        {
            get => this.extender.FilterBoxPosition;
            set
            {
                this.extender.FilterBoxPosition = value;
                this.RepositionGrid();
            }
        }

        /// <summary>
        ///     Gets and sets the <see cref="IGridFilterFactory" /> used to generate the filter GUI.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(null)]
        [Description("Gets and sets factory instance which should be " + "used to create grid filters.")]
        public IGridFilterFactory FilterFactory
        {
            get => this.extender.FilterFactory;
            set => this.extender.FilterFactory = value;
        }

        /// <summary>
        ///     Gets and sets the text for the filter label.
        /// </summary>
        [Browsable(true)]
        [DefaultValue("Filter")]
        [Description("Gets and sets the text for the filter label.")]
        public string FilterText
        {
            get => this.extender.FilterText;
            set => this.extender.FilterText = value;
        }

        /// <summary>
        ///     Gets and sets whether the filter label should be visible.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Gets and sets whether the filter label should be visible.")]
        public bool FilterTextVisible
        {
            get => this.extender.FilterTextVisible;
            set => this.extender.FilterTextVisible = value;
        }

        /// <summary>
        ///     Gets and sets the mode for the grid.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Specifies whether the table gets filtered or matching rows get highlighted.")]
        public GridMode GridMode
        {
            get => this.extender.GridMode;
            set => this.extender.GridMode = value;
        }

        /// <summary>
        ///     Gets and sets whether filters are kept while switching between different tables.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Specifies whether filters are kept while switching between different tables.")]
        public bool KeepFilters
        {
            get => this.extender.KeepFilters;
            set => this.extender.KeepFilters = value;
        }

        /// <summary>
        ///     Gets and sets what information is shown to the user
        ///     if an error in the builded filter criterias occurs.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(FilterErrorModes.General)]
        [Description(
            "Specifies what information is shown to the user " + "if an error in the builded filter criterias occurs.")]
        public FilterErrorModes MessageErrorMode
        {
            get => this.extender.MessageErrorMode;
            set => this.extender.MessageErrorMode = value;
        }

        /// <summary>
        ///     The selected operator to combine the filter criterias.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(LogicalOperators.And)]
        [Description("The selected operator to combine the filter criterias.")]
        public LogicalOperators Operator
        {
            get => this.extender.Operator;
            set => this.extender.Operator = value;
        }

        /// <summary>
        ///     Clears all filters to initial state.
        /// </summary>
        public void ClearFilters()
        {
            this.extender.ClearFilters();
        }

        /// <summary>
        ///     Gets all filters currently set
        /// </summary>
        /// <returns></returns>
        public string[] GetFilters()
        {
            return this.extender.GetFilters();
        }

        /// <summary>
        ///     Gets all currently set <see cref="IGridFilter" />s.
        /// </summary>
        /// <returns>Collection of <see cref="IGridFilter" />s.</returns>
        public GridFilterCollection GetGridFilters()
        {
            return this.extender.GetGridFilters();
        }

        /// <summary>
        ///     Refreshes the filter criteria to match the current contents of the associated
        ///     filter controls.
        /// </summary>
        public void RefreshFilters()
        {
            this.extender.RefreshFilters();
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
            this.extender.SetFilters(filters);
        }

        /// <summary>
        ///     Cleans up.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
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

        /// <summary>
        ///     Repositions the grid to match the new size
        /// </summary>
        /// <param name="e">event arguments</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.RepositionGrid();
        }

        /// <summary>
        ///     Erforderliche Methode für die Designerunterstützung.
        ///     Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new Container();
            this.grid = new ExtendedDataGrid();
            this.extender = new DataGridFilterExtender(this.components);
            ((ISupportInitialize)this.grid).BeginInit();
            ((ISupportInitialize)this.extender).BeginInit();
            this.SuspendLayout();

            // _grid
            this.grid.AutoCreateTableStyles = false;
            this.grid.DataMember = string.Empty;
            this.grid.HeaderForeColor = SystemColors.ControlText;
            this.grid.Location = new Point(0, 24);
            this.grid.Name = "grid";
            this.grid.Size = new Size(496, 352);
            this.grid.TabIndex = 0;
            this.grid.MouseDown += this.OnMouseDown;
            this.grid.KeyDown += this.OnKeyDown;
            this.grid.MouseMove += this.OnMouseMove;
            this.grid.MouseEnter += this.OnMouseEnter;
            this.grid.MouseHover += this.OnMouseHover;
            this.grid.MouseLeave += this.OnMouseLeave;
            this.grid.KeyUp += this.OnKeyUp;
            this.grid.MouseUp += this.OnMouseUp;
            this.grid.KeyPress += this.OnKeyPress;
            this.grid.DoubleClick += this.OnDoubleClick;

            // _extender
            this.extender.AutoAdjustGridPosition = false;
            this.extender.ConsoleErrorMode = FilterErrorModes.Off;
            this.extender.DataGrid = this.grid;
            this.extender.FilterBoxPosition = FilterPosition.Top;
            this.extender.FilterText = "Filter";
            this.extender.FilterTextVisible = true;
            this.extender.MessageErrorMode = FilterErrorModes.General;
            this.extender.Operator = LogicalOperators.And;
            this.extender.GridFilterBound += this.OnGridFilterBound;
            this.extender.GridFilterUnbound += this.OnGridFilterUnbound;
            this.extender.AfterFiltersChanged += this.OnAfterFiltersChanged;
            this.extender.BeforeFiltersChanging += this.OnBeforeFiltersChanging;

            // FilterableDataGrid
            this.Controls.Add(this.grid);
            this.Name = "FilterableDataGrid";
            this.Size = new Size(496, 376);
            ((ISupportInitialize)this.grid).EndInit();
            ((ISupportInitialize)this.extender).EndInit();
            this.ResumeLayout(false);
        }

        private void OnAfterFiltersChanged(object sender, EventArgs e)
        {
            this.OnAfterFiltersChanged(e);
        }

        private void OnBeforeFiltersChanging(object sender, EventArgs e)
        {
            this.OnBeforeFiltersChanging(e);
        }

        private void OnDoubleClick(object sender, EventArgs e)
        {
            this.OnDoubleClick(e);
        }

        private void OnGridFilterBound(object sender, GridFilterEventArgs e)
        {
            this.OnGridFilterBound(e);
        }

        private void OnGridFilterUnbound(object sender, GridFilterEventArgs e)
        {
            this.OnGridFilterUnbound(e);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            this.OnKeyPress(e);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            this.OnKeyUp(e);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            this.OnMouseDown(e);
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            this.OnMouseEnter(e);
        }

        private void OnMouseHover(object sender, EventArgs e)
        {
            this.OnMouseHover(e);
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            this.OnMouseLeave(e);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            this.OnMouseMove(e);
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            this.OnMouseUp(e);
        }

        private void RepositionGrid()
        {
            var newTop = this.grid.Top;
            var newHeight = this.grid.Height;
            var newLeft = 0;
            var newWidth = this.Width;
            switch (this.extender.FilterBoxPosition)
            {
                case FilterPosition.Caption:
                case FilterPosition.Off:
                    newTop = 0;
                    newHeight = this.Height;
                    break;
                case FilterPosition.Top:
                    newTop = this.extender.NeededControlHeight + 1;
                    newHeight = this.Height - newTop - 1;
                    break;
                case FilterPosition.Bottom:
                    newTop = 0;
                    newHeight = this.Height - this.extender.NeededControlHeight - 1;
                    break;
            }

            this.grid.SetBounds(newLeft, newTop, newWidth, newHeight, BoundsSpecified.All);
        }
    }
}