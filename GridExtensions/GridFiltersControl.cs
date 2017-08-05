namespace GridExtensions
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    using GridExtensions.GridFilterFactories;

    #region Enum LogicalOperators

    /// <summary>
    ///     Logical operators which can be used to determine how the filter
    ///     criterias are combined
    /// </summary>
    public enum LogicalOperators
    {
        /// <summary>
        ///     Logical And
        /// </summary>
        And,

        /// <summary>
        ///     Logical Or
        /// </summary>
        Or
    }

    #endregion

    #region Enum FilterErrorModes

    /// <summary>
    ///     Modes which determine the output generated when an error
    ///     in the builded filter criterias occurs.
    /// </summary>
    [Flags]
    public enum FilterErrorModes
    {
        /// <summary>
        ///     No error output at all
        /// </summary>
        Off = 0,

        /// <summary>
        ///     General error message
        /// </summary>
        General = 1,

        /// <summary>
        ///     Message of the exception that occured
        /// </summary>
        ExceptionMessage = 2,

        /// <summary>
        ///     StackTrace of the exception that occured
        /// </summary>
        StackTrace = 4,

        /// <summary>
        ///     All available output
        /// </summary>
        All = 7
    }

    #endregion

    #region Enum GridMode

    /// <summary>
    ///     Defines the modes the grid can have.
    /// </summary>
    public enum GridMode
    {
        /// <summary>
        ///     The shown data is filtered with the filter criterias entered.
        /// </summary>
        Filter,

        /// <summary>
        ///     The rows matching the filter criteria are highlighted.
        /// </summary>
        Highlight
    }

    #endregion

    #region Enum RefreshMode

    /// <summary>
    ///     Modes which determine when the filter criteria get automatically
    ///     applied to the contents of the grid.
    /// </summary>
    public enum RefreshMode
    {
        /// <summary>
        ///     Filters are regenerated on every user input.
        /// </summary>
        OnInput,

        /// <summary>
        ///     Filters are regenerated whenever the user presses Enter while
        ///     the focus is in one of the filter controls.
        /// </summary>
        OnEnter,

        /// <summary>
        ///     Filters are regenerated whenever one of the filter controls
        ///     looses input focus.
        /// </summary>
        OnLeave,

        /// <summary>
        ///     Filters are regenerated whenever one of the filter controls
        ///     looses input focus or the user presses Enter while
        ///     the focus is in one of the filter controls.
        /// </summary>
        OnEnterOrLeave,

        /// <summary>
        ///     No automatic filter generation.
        /// </summary>
        Off
    }

    #endregion

    /// <summary>
    ///     A control where all controls all placed which are necessary for
    ///     extending a grid for filtering.
    /// </summary>
    internal class GridFiltersControl : UserControl
    {
        private readonly Hashtable columnStyleToGridFilterHash;

        private readonly Container components = null;

        private readonly Hashtable keepFiltersHash;

        private LogicalOperators _operator;

        private RefreshMode autoRefreshMode = RefreshMode.OnInput;

        private bool baseFilterEnabled = true;

        private LogicalOperators baseFilterOperator = LogicalOperators.And;

        private GridColumnStylesCollection currentColumnStyleCollection;

        private DataGridTableStyle currentTableStyle;

        private IGridFilterFactory filterFactory;

        private DataGrid grid;

        private IGridExtension gridExtension;

        private GridMode gridMode;

        private bool keepFilters;

        private string lastRowFilter = string.Empty;

        private Label lblFilter;

        private TextBox refBox;

        private bool refreshDisabled;

        /// <summary>
        ///     Creates a new instance
        /// </summary>
        internal GridFiltersControl()
        {
            this.InitializeComponent();

            this.columnStyleToGridFilterHash = new Hashtable();
            this.keepFiltersHash = new Hashtable();
            this.BaseFilters = new StringDictionary();

            this.FilterFactory = new DefaultGridFilterFactory();

            this.RecreateGridFilters();
        }

        /// <summary>
        ///     Event, which gets fired whenever the filter criteria has been changed.
        /// </summary>
        internal event EventHandler AfterFiltersChanged;

        /// <summary>
        ///     Event, which gets fired whenever the filter criteria are going to be changed.
        /// </summary>
        internal event EventHandler BeforeFiltersChanging;

        /// <summary>
        ///     Event, which gets fired whenever an <see cref="IGridFilter" /> has been bound
        ///     and thus added to this instance.
        /// </summary>
        internal event GridFilterEventHandler GridFilterBound;

        /// <summary>
        ///     Event, which gets fired whenever an <see cref="IGridFilter" /> has been unbound
        ///     and thus removed to this instance.
        /// </summary>
        internal event GridFilterEventHandler GridFilterUnbound;

        /// <summary>
        ///     Gets and sets whether the filter criteria is automatically refreshed when
        ///     changes are made to the filter controls. If set to false then a call to
        ///     <see cref="RefreshFilters" /> is needed to manually refresh the criteria.
        /// </summary>
        internal RefreshMode AutoRefreshMode
        {
            get => this.autoRefreshMode;
            set
            {
                this.autoRefreshMode = value;
                this.RecreateRowFilter();
            }
        }

        /// <summary>
        ///     Gets or sets whether base filters should be used when refreshing
        ///     the filter criteria. Setting it to false will disable the functionality
        ///     while still keeping the base filter strings in the <see cref="BaseFilters" />
        ///     collection intact.
        /// </summary>
        internal bool BaseFilterEnabled
        {
            get => this.baseFilterEnabled;
            set
            {
                this.baseFilterEnabled = value;
                this.RecreateRowFilter();
            }
        }

        /// <summary>
        ///     Gets or sets which operator should be used to combine the base filter
        ///     with the automatically created filters.
        /// </summary>
        internal LogicalOperators BaseFilterOperator
        {
            get => this.baseFilterOperator;
            set
            {
                this.baseFilterOperator = value;
                this.RecreateRowFilter();
            }
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
        internal StringDictionary BaseFilters { get; }

        /// <summary>
        ///     Gets and sets what information is showed to the user
        ///     if an error in the builded filter criterias occurs.
        /// </summary>
        internal FilterErrorModes ConsoleErrorMode { get; set; } = FilterErrorModes.Off;

        /// <summary>
        ///     Gets or sets the currently used base filter. Internally it adjusts the
        ///     <see cref="BaseFilters" /> collection with the given value and the current
        ///     <see cref="DataTable.TableName" /> and also initiates a refresh.
        /// </summary>
        internal string CurrentTableBaseFilter
        {
            get
            {
                if (this.gridExtension.CurrentView == null) return null;
                return this.BaseFilters[this.gridExtension.CurrentView.Table.TableName];
            }

            set
            {
                if (this.gridExtension.CurrentView == null) return;

                this.BaseFilters[this.gridExtension.CurrentView.Table.TableName] = value;
                this.RecreateRowFilter();
            }
        }

        /// <summary>
        ///     Gets and sets the <see cref="IGridFilterFactory" /> used to generate the filter GUI.
        /// </summary>
        internal IGridFilterFactory FilterFactory
        {
            get => this.filterFactory;
            set
            {
                if (this.filterFactory != null) this.filterFactory.Changed -= this.OnFilterFactoryChanged;
                this.filterFactory = value ?? new DefaultGridFilterFactory();
                this.filterFactory.Changed += this.OnFilterFactoryChanged;
                this.RecreateGridFilters();
            }
        }

        /// <summary>
        ///     Gets and sets the text for the filter label.
        /// </summary>
        internal string FilterText
        {
            get => this.lblFilter.Text;
            set => this.lblFilter.Text = value;
        }

        /// <summary>
        ///     Gets and sets whether the filter label should be visible.
        /// </summary>
        internal bool FilterTextVisible
        {
            get => this.lblFilter.Visible;
            set => this.lblFilter.Visible = value;
        }

        /// <summary>
        ///     Gets and sets the <see cref="IGridExtension" /> instance to use.
        /// </summary>
        internal IGridExtension GridExtension
        {
            get => this.gridExtension;
            set
            {
                if (this.gridExtension != null)
                {
                    this.grid.DataSourceChanged -= this.OnDataSourceChanged;
                    this.grid.DoubleClick -= this.OnGridDoubleClick;
                    this.grid.TableStyles.CollectionChanged -= this.OnTableStylesCollectionChanged;
                    this.gridExtension.HorizontalScrollbar.ValueChanged -= this.OnGridHorizontalScrollbarValueChanged;
                }

                this.gridExtension = value;
                this.grid = this.gridExtension?.Grid;

                if (this.gridExtension != null)
                {
                    this.grid.DataSourceChanged += this.OnDataSourceChanged;
                    this.grid.DoubleClick += this.OnGridDoubleClick;
                    this.grid.TableStyles.CollectionChanged += this.OnTableStylesCollectionChanged;
                    this.gridExtension.HorizontalScrollbar.ValueChanged += this.OnGridHorizontalScrollbarValueChanged;
                }

                this.RecreateGridFilters();
            }
        }

        /// <summary>
        ///     Gets and sets the mode for the grid.
        /// </summary>
        internal GridMode GridMode
        {
            get => this.gridMode;
            set
            {
                this.gridMode = value;
                this.lastRowFilter = string.Empty;
                if (this.gridExtension.CurrentView != null)
                    if (this.gridMode == GridMode.Highlight) this.gridExtension.CurrentView.RowFilter = string.Empty;
                    else for (var i = 0; i < this.gridExtension.CurrentView.Count; i++) this.grid.UnSelect(i);

                this.RecreateRowFilter();
            }
        }

        /// <summary>
        ///     Gets and sets whether filters are kept while switching between different tables.
        /// </summary>
        internal bool KeepFilters
        {
            get => this.keepFilters;
            set
            {
                this.keepFilters = value;
                if (!this.keepFilters) this.keepFiltersHash.Clear();
                else this.RecreateRowFilter();
            }
        }

        /// <summary>
        ///     Gets and sets what information is showed to the user
        ///     if an error in the builded filter criterias occurs.
        /// </summary>
        internal FilterErrorModes MessageErrorMode { get; set; } = FilterErrorModes.General;

        /// <summary>
        ///     The selected operator to combine the filter criterias.
        /// </summary>
        internal LogicalOperators Operator
        {
            get => this._operator;
            set
            {
                this._operator = value;
                this.RecreateRowFilter();
            }
        }

        private int RowHeaderWidth
        {
            get
            {
                if (this.currentTableStyle == null || !this.currentTableStyle.RowHeadersVisible) return 0;

                return this.currentTableStyle.RowHeaderWidth;
            }
        }

        /// <summary>
        ///     Clears all filters to initial state.
        /// </summary>
        internal void ClearFilters()
        {
            try
            {
                this.refreshDisabled = true;
                foreach (IGridFilter gridFilter in this.columnStyleToGridFilterHash.Values) gridFilter.Clear();
            }
            finally
            {
                this.refreshDisabled = false;
            }

            this.RecreateRowFilter();
        }

        /// <summary>
        ///     Gets all filters currently set
        /// </summary>
        /// <returns></returns>
        internal string[] GetFilters()
        {
            var result = new string[this.columnStyleToGridFilterHash.Count];
            for (var i = 0; i < this.currentColumnStyleCollection.Count; i++)
            {
                var columnStyle = this.currentColumnStyleCollection[i];
                var gridFilter = (IGridFilter)this.columnStyleToGridFilterHash[columnStyle];
                if (gridFilter.HasFilter) result[i] = gridFilter.GetFilter($"[{columnStyle.MappingName}]");
                else result[i] = string.Empty;
            }

            return result;
        }

        /// <summary>
        ///     Gets all currently set <see cref="IGridFilter" />s.
        /// </summary>
        /// <returns>Collection of <see cref="IGridFilter" />s.</returns>
        internal GridFilterCollection GetGridFilters()
        {
            if (this.currentColumnStyleCollection == null || this.columnStyleToGridFilterHash == null) return null;

            return new GridFilterCollection(this.currentColumnStyleCollection, this.columnStyleToGridFilterHash);
        }

        /// <summary>
        ///     Refreshes the filter criteria to match the current contents of the associated
        ///     filter controls.
        /// </summary>
        internal void RefreshFilters()
        {
            this.lastRowFilter = string.Empty;
            this.RecreateRowFilter(true);
        }

        /// <summary>
        ///     Sets all filters to the specified values.
        ///     The values must be in order of the column styles in the current view.
        ///     This function should normally be used with data previously coming
        ///     from the <see cref="GetFilters" /> function.
        /// </summary>
        /// <param name="filters">filters to set</param>
        internal void SetFilters(string[] filters)
        {
            for (var i = 0; i < this.currentColumnStyleCollection.Count && i < filters.Length; i++)
            {
                var gridFilter = (IGridFilter)this.columnStyleToGridFilterHash[this.currentColumnStyleCollection[i]];
                if (filters[i].Length > 0) gridFilter.SetFilter(filters[i]);
                else gridFilter.Clear();
            }
        }

        /// <summary>
        ///     Die verwendeten Ressourcen bereinigen.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (this.gridExtension != null)
            {
                this.grid.DataSourceChanged -= this.OnDataSourceChanged;
                this.grid.DoubleClick -= this.OnGridDoubleClick;
                this.grid.TableStyles.CollectionChanged -= this.OnTableStylesCollectionChanged;
                this.gridExtension.HorizontalScrollbar.ValueChanged -= this.OnGridHorizontalScrollbarValueChanged;
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

        /// <summary>
        ///     Initiates recalculation for the positions of the filter GUI elements.
        /// </summary>
        /// <param name="e">Event data</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.RepositionGridFilters();
        }

        /// <summary>
        ///     Initiates recalculation for the positions of the filter GUI elements.
        /// </summary>
        /// <param name="e">Event data</param>
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            this.RepositionGridFilters();
        }

        private string GetMessageFromMode(FilterErrorModes mode, Exception exc)
        {
            var result = string.Empty;

            if ((mode & FilterErrorModes.General) == FilterErrorModes.General) result += "Invalid filter specified.";
            if ((mode & FilterErrorModes.ExceptionMessage) == FilterErrorModes.ExceptionMessage)
                result += (result.Length > 0 ? "\n" : string.Empty) + exc.Message;
            if ((mode & FilterErrorModes.StackTrace) == FilterErrorModes.StackTrace)
                result += (result.Length > 0 ? "\n" : string.Empty) + exc.StackTrace;

            return result;
        }

        /// <summary>
        ///     Erforderliche Methode für die Designerunterstützung.
        ///     Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.refBox = new TextBox();
            this.lblFilter = new Label();
            this.SuspendLayout();

            // _refBox
            this.refBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.refBox.Location = new Point(344, 0);
            this.refBox.Name = "refBox";
            this.refBox.Size = new Size(40, 20);
            this.refBox.TabIndex = 0;
           // this.refBox.Text = "textBox1";
            this.refBox.Visible = false;

            // _lblFilter
            this.lblFilter.Dock = DockStyle.Left;
            this.lblFilter.Location = new Point(0, 0);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new Size(100, 24);
            this.lblFilter.TabIndex = 1;
           // this.lblFilter.Text = "Filter";
            this.lblFilter.TextAlign = ContentAlignment.MiddleLeft;

            // GridFiltersControl
            this.Controls.Add(this.lblFilter);
            this.Controls.Add(this.refBox);
            this.Name = "GridFiltersControl";
            this.Size = new Size(384, 24);
            this.ResumeLayout(false);
        }

        private void OnColumnStyleWidthChanged(object sender, EventArgs e)
        {
            this.RepositionGridFilters();
        }

        private void OnCurrentTableStyleRowHeadersVisibleChanged(object sender, EventArgs e)
        {
            this.RepositionGridFilters();
        }

        private void OnCurrentTableStyleRowHeaderWidthChanged(object sender, EventArgs e)
        {
            this.RepositionGridFilters();
        }

        private void OnDataSourceChanged(object sender, EventArgs e)
        {
            this.lastRowFilter = string.Empty;

            // this probably looks weird but the DataSourceChanged event of the grid
            // must complete before calling RecreateGridFilters, otherwise the DataGrid
            // has soem real nasty behaviour (e.g. showing only 3 lines although
            // the view has 100 lines)
            if (this.grid.Handle.ToInt32() > 0) this.grid.BeginInvoke(new MethodInvoker(this.RecreateGridFilters));
        }

        private void OnFilterChanged(object sender, EventArgs e)
        {
            if (this.autoRefreshMode == RefreshMode.OnInput) this.RecreateRowFilter();
        }

        private void OnFilterControlKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' && (this.autoRefreshMode == RefreshMode.OnEnter
                                      || this.autoRefreshMode == RefreshMode.OnEnterOrLeave)) this.RefreshFilters();
        }

        private void OnFilterControlLeave(object sender, EventArgs e)
        {
            if (this.autoRefreshMode == RefreshMode.OnLeave || this.autoRefreshMode == RefreshMode.OnEnterOrLeave)
                this.RefreshFilters();
        }

        private void OnFilterFactoryChanged(object sender, EventArgs e)
        {
            this.RecreateGridFilters();
        }

        private void OnGridColumnStylesCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Refresh) return;
            this.RecreateGridFilters();
        }

        private void OnGridDoubleClick(object sender, EventArgs e)
        {
            var hti = this.grid.HitTest(this.grid.PointToClient(Cursor.Position));
            if (hti.Type == DataGrid.HitTestType.ColumnResize || hti.Type == DataGrid.HitTestType.ColumnHeader)
                this.RepositionGridFilters();
        }

        private void OnGridHorizontalScrollbarValueChanged(object sender, EventArgs e)
        {
            this.RepositionGridFilters();
        }

        private void OnTableStylesCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            this.RecreateGridFilters();
        }

        /// <summary>
        ///     Initiates a recalculation of the needed filter GUI elements and their positions.
        /// </summary>
        private void RecreateGridFilters()
        {
            // first clean up what has beed done before
            foreach (DataGridColumnStyle columnStyle in this.columnStyleToGridFilterHash.Keys)
            {
                var gridFilter = this.columnStyleToGridFilterHash[columnStyle] as IGridFilter;
                gridFilter.Changed -= this.OnFilterChanged;
                gridFilter.FilterControl.KeyPress -= this.OnFilterControlKeyPress;
                gridFilter.FilterControl.Leave -= this.OnFilterControlLeave;
                columnStyle.WidthChanged -= this.OnColumnStyleWidthChanged;
                if (this.Controls.Contains(gridFilter.FilterControl))
                {
                    this.Controls.Remove(gridFilter.FilterControl);
                    gridFilter.FilterControl.Dispose();
                }
            }

            this.columnStyleToGridFilterHash.Clear();

            if (this.currentTableStyle != null)
            {
                this.currentTableStyle.RowHeaderWidthChanged -= this.OnCurrentTableStyleRowHeaderWidthChanged;
                this.currentTableStyle.RowHeadersVisibleChanged -= this.OnCurrentTableStyleRowHeadersVisibleChanged;
                this.currentColumnStyleCollection.CollectionChanged -= this.OnGridColumnStylesCollectionChanged;
                this.currentTableStyle = null;
                this.currentColumnStyleCollection = null;
            }

            // adjust the position for the filter GUI
            this.Height = this.refBox.Height;

            if (this.gridExtension == null) return;

            this.lblFilter.Width = this.grid.RowHeaderWidth;

            if (this.gridExtension.CurrentView == null)
            {
                // provide a dummy represantation when nothing is set
                // this allows better desing time support
                this.refBox.Visible = true;
                this.refBox.Left = this.grid.RowHeaderWidth + 1;
                this.refBox.Width = this.Width - this.grid.RowHeaderWidth - 1;
                return;
            }

            this.refBox.Visible = false;

            if (this.grid.TableStyles.Contains(this.gridExtension.CurrentView.Table.TableName))
            {
                // get the appropriate table style
                this.currentTableStyle = this.grid.TableStyles[this.gridExtension.CurrentView.Table.TableName];
                this.currentTableStyle.RowHeaderWidthChanged += this.OnCurrentTableStyleRowHeaderWidthChanged;
                this.currentTableStyle.RowHeadersVisibleChanged += this.OnCurrentTableStyleRowHeadersVisibleChanged;

                this.currentColumnStyleCollection = this.currentTableStyle.GridColumnStyles;
                this.currentColumnStyleCollection.CollectionChanged += this.OnGridColumnStylesCollectionChanged;

                this.filterFactory.BeginGridFilterCreation();
                try
                {
                    foreach (DataGridColumnStyle columnStyle in this.currentColumnStyleCollection)
                    {
                        // we need notification when the width of the column changes
                        columnStyle.WidthChanged += this.OnColumnStyleWidthChanged;

                        // get the datatype

                        // create a filter
                        var gridFilter = this.filterFactory.CreateGridFilter(
                            this.gridExtension.CurrentView.Table.Columns[columnStyle.MappingName],
                            columnStyle);
                        if (!gridFilter.UseCustomFilterPlacement)
                        {
                            // adjust the vertical positions
                            gridFilter.FilterControl.Top = 0;
                            gridFilter.FilterControl.Height = this.Height;
                            gridFilter.FilterControl.Visible = false;

                            // add the GUI element to our controls collection
                            this.Controls.Add(gridFilter.FilterControl);
                            gridFilter.FilterControl.BringToFront();
                        }

                        // notification needed when the filter settings are changed
                        gridFilter.Changed += this.OnFilterChanged;
                        gridFilter.FilterControl.KeyPress += this.OnFilterControlKeyPress;
                        gridFilter.FilterControl.Leave += this.OnFilterControlLeave;

                        // added to hash to provider fast access
                        this.columnStyleToGridFilterHash.Add(columnStyle, gridFilter);
                    }
                }
                finally
                {
                    this.filterFactory.EndGridFilterCreation();
                }

                if (this.keepFilters && this.keepFiltersHash.ContainsKey(this.gridExtension.CurrentView.Table.TableName)
                ) this.SetFilters((string[])this.keepFiltersHash[this.gridExtension.CurrentView.Table.TableName]);
            }

            this.RepositionGridFilters();
        }

        private void RecreateRowFilter(bool ignoreAutoRefresh = false)
        {
            if (this.currentColumnStyleCollection == null || this.refreshDisabled) return;

            if (this.autoRefreshMode == RefreshMode.Off && !ignoreAutoRefresh) return;

            if (this.gridExtension.CurrentView != null && this.currentTableStyle != null
                && this.gridExtension.CurrentView.Table.TableName != this.currentTableStyle.MappingName) return;

            try
            {
                string rowFilter;
                var operatorString = this._operator == LogicalOperators.And ? " AND " : " OR ";

                switch (this._operator)
                {
                    case LogicalOperators.And:
                    case LogicalOperators.Or:
                        rowFilter = (from DataGridColumnStyle columnStyle in this.currentColumnStyleCollection
                                     let gridFilter = this.columnStyleToGridFilterHash[columnStyle] as IGridFilter
                                     where gridFilter.HasFilter
                                     select gridFilter.GetFilter($"[{columnStyle.MappingName}]")).Aggregate(
                            string.Empty,
                            (current, filter) => current + (current.Length > 0 && filter.Length > 0
                                                                ? operatorString
                                                                : string.Empty) + filter);

                        break;
                    default:
                        rowFilter = string.Empty;
                        break;
                }

                var baseFilter = this.CurrentTableBaseFilter;
                var hasBaseFilter = !string.IsNullOrEmpty(baseFilter);
                if (hasBaseFilter && this.baseFilterEnabled)
                {
                    operatorString = this.baseFilterOperator == LogicalOperators.And ? " AND " : " OR ";
                    if (rowFilter.Length > 0)
                        rowFilter = "(" + rowFilter + ")" + operatorString + "(" + this.CurrentTableBaseFilter + ")";
                    else rowFilter += this.CurrentTableBaseFilter;
                }

                if (this.lastRowFilter != rowFilter || this.lastRowFilter.Length == 0
                    && this.gridExtension.CurrentView.RowFilter.Length > 0)
                {
                    this.OnBeforeFiltersChanging(EventArgs.Empty);
                    switch (this.gridMode)
                    {
                        case GridMode.Filter:
                            // set the filter criteria
                            this.gridExtension.CurrentView.RowFilter = rowFilter;
                            break;
                        case GridMode.Highlight:
                            if (rowFilter.Length == 0)
                            {
                                for (var i = 0; i < this.gridExtension.CurrentView.Count; i++) this.grid.UnSelect(i);
                            }
                            else
                            {
                                var dataFilter = new DataFilter(rowFilter, this.gridExtension.CurrentView.Table);
                                for (var i = 0; i < this.gridExtension.CurrentView.Count; i++)
                                    if (dataFilter.Invoke(this.gridExtension.CurrentView[i].Row))
                                    {
                                        if (!this.grid.IsSelected(i)) this.grid.Select(i);
                                    }
                                    else
                                    {
                                        if (this.grid.IsSelected(i)) this.grid.UnSelect(i);
                                    }
                            }

                            break;
                        default: throw new ArgumentOutOfRangeException();
                    }
                    this.lastRowFilter = rowFilter;
                    this.OnAfterFiltersChanged(EventArgs.Empty);
                }
            }
            catch (Exception exc)
            {
                var text = this.GetMessageFromMode(this.ConsoleErrorMode, exc);
                if (text.Length > 0) Console.WriteLine(text);
                text = this.GetMessageFromMode(this.MessageErrorMode, exc);
                if (text.Length > 0) MessageBox.Show(text, "Filter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (this.keepFilters)
                this.keepFiltersHash[this.gridExtension.CurrentView.Table.TableName] = this.GetFilters();
        }

        private void RepositionGridFilters()
        {
            if (this.currentTableStyle == null) return;

            try
            {
                this.SuspendLayout();

                var filterWidth = this.RowHeaderWidth - 1;
                var curPos = this.RowHeaderWidth;
                if (filterWidth > 0)
                {
                    this.lblFilter.Width = filterWidth;
                    this.lblFilter.Visible = true;
                    curPos++;
                    if (this.RightToLeft == RightToLeft.Yes)
                    {
                        if (this.lblFilter.Dock != DockStyle.Right) this.lblFilter.Dock = DockStyle.Right;
                    }
                    else
                    {
                        if (this.lblFilter.Dock != DockStyle.Left) this.lblFilter.Dock = DockStyle.Left;
                    }
                }
                else
                {
                    this.lblFilter.Visible = false;
                }

                // this loop goes through all column styles and iteratively sets 
                // their horizontal positions and widths
                for (var i = 0; i < this.currentColumnStyleCollection.Count; i++)
                {
                    var columnStyle = this.currentColumnStyleCollection[i];

                    var gridFilter = this.columnStyleToGridFilterHash[columnStyle] as IGridFilter;
                    if (gridFilter != null && !gridFilter.UseCustomFilterPlacement)
                    {
                        var from = curPos - this.gridExtension.HorizontalScrollbar.Value;
                        var width = columnStyle.Width + (i == 0 ? 1 : 0);

                        if (from < this.RowHeaderWidth)
                        {
                            width -= this.RowHeaderWidth - from;
                            from = this.RowHeaderWidth;
                        }

                        if (from + width > this.Width) width = this.Width - from;

                        if (width < 4)
                        {
                            gridFilter.FilterControl.Visible = false;
                        }
                        else
                        {
                            if (this.RightToLeft == RightToLeft.Yes) from = this.Width - from - width;

                            gridFilter.FilterControl.SetBounds(
                                from,
                                0,
                                width,
                                0,
                                BoundsSpecified.X | BoundsSpecified.Width);
                            gridFilter.FilterControl.Visible = true;
                        }
                    }

                    curPos += columnStyle.Width + (i == 0 ? 1 : 0);
                }
            }
            finally
            {
                this.ResumeLayout();
            }

            this.RecreateRowFilter();
            this.Invalidate();
        }
    }
}