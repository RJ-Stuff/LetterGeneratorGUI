namespace GridExtensions.GridFilterFactories
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Windows.Forms;

    using GridExtensions.GridFilters;

    /// <summary>
    ///     Implementation of <see cref="IGridFilterFactory" /> extending another
    ///     <see cref="IGridFilterFactory" /> instance by overriding the default
    ///     placement of the filter controls and placing them in a layouted way
    ///     outside of the grid.
    /// </summary>
    public class LayoutedGridFilterFactoryControl : UserControl, IGridFilterFactory
    {
        private readonly Container components = null;

        private readonly LayoutedPanel layoutedPanel;

        private ArrayList createdControls;

        private ArrayList createdLabels;

        private IGridFilterFactory innerGridFilterFactory;

        private bool showEmptyGridFilters;

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        public LayoutedGridFilterFactoryControl()
        {
            this.InitializeComponent();
            this.layoutedPanel = new LayoutedPanel { Dock = DockStyle.Fill };
            this.Controls.Add(this.layoutedPanel);
            this.InnerGridFilterFactory = new DefaultGridFilterFactory();
        }

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
        ///     Gets and sets the minimum width for the controls. If the panel isn't
        ///     big enough scrollbars will be created.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(40)]
        [Description(
            "Gets and sets the minimum width for the controls. If the panel isn't "
            + "big enough scrollbars will be created.")]
        public int ControlsMinimumWidth
        {
            get => this.layoutedPanel.ControlsMinimumWidth;
            set => this.layoutedPanel.ControlsMinimumWidth = value;
        }

        /// <summary>
        ///     Gets and sets the horizontal space between the labels and controls.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(0)]
        [Description("Gets and sets the horizontal space between the labels and controls.")]
        public int HorizontalSpacing
        {
            get => this.layoutedPanel.HorizontalSpacing;
            set => this.layoutedPanel.HorizontalSpacing = value;
        }

        /// <summary>
        ///     Gets and sets the <see cref="IGridFilterFactory" /> instance which should
        ///     be used for creating <see cref="IGridFilter" />s.
        /// </summary>
        [Browsable(false)]
        public IGridFilterFactory InnerGridFilterFactory
        {
            get => this.innerGridFilterFactory;
            set
            {
                if (this.innerGridFilterFactory != null)
                    this.innerGridFilterFactory.Changed -= this.OnGridFilterFactoryChanged;

                this.innerGridFilterFactory = value;

                if (this.innerGridFilterFactory != null)
                    this.innerGridFilterFactory.Changed += this.OnGridFilterFactoryChanged;

                this.OnChanged();
            }
        }

        /// <summary>
        ///     Gets and sets whether the labels are aligned to the right or to the left.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Gets and sets whether the labels are aligned to the right or to the left.")]
        public bool RightAlignLabels
        {
            get => this.layoutedPanel.RightAlignLabels;
            set => this.layoutedPanel.RightAlignLabels = value;
        }

        /// <summary>
        ///     Gets and sets whether EmptyGridFilter instances should be shown.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Gets and sets whether EmptyGridFilter instances should be shown.")]
        public bool ShowEmptyGridFilters
        {
            get => this.showEmptyGridFilters;
            set
            {
                this.showEmptyGridFilters = value;
                this.OnChanged();
            }
        }

        /// <summary>
        ///     Gets and sets the vertical space between the rows.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(4)]
        [Description("Gets and sets the vertical space between the rows.")]
        public int VerticalSpacing
        {
            get => this.layoutedPanel.VerticalSpacing;
            set => this.layoutedPanel.VerticalSpacing = value;
        }

        /// <summary>
        ///     Notifies this instance that the <see cref="IGridFilter" /> creation process
        ///     is being started.
        /// </summary>
        public void BeginGridFilterCreation()
        {
            if (this.innerGridFilterFactory == null) return;

            this.innerGridFilterFactory.BeginGridFilterCreation();

            this.createdLabels = new ArrayList();
            this.createdControls = new ArrayList();
        }

        /// <summary>
        ///     Creates a new instance of <see cref="IGridFilter" /> by calling the
        ///     <see cref="InnerGridFilterFactory" /> and then modifying the default
        ///     placement.
        /// </summary>
        /// <param name="column">The <see cref="DataColumn" /> for which the filter control should be created.</param>
        /// <param name="columnStyle">The <see cref="DataGridColumnStyle" /> for which the filter control should be created.</param>
        /// <returns>A <see cref="IGridFilter" />.</returns>
        public IGridFilter CreateGridFilter(DataColumn column, DataGridColumnStyle columnStyle)
        {
            if (this.innerGridFilterFactory == null) return new EmptyGridFilter();

            var result = this.innerGridFilterFactory.CreateGridFilter(column, columnStyle);
            result.UseCustomFilterPlacement = true;

            var eventArgs = new GridFilterEventArgs(column, columnStyle, result);
            this.OnGridFilterFactoryGridFilterCreated(eventArgs);
            result = eventArgs.GridFilter;
            if (!result.UseCustomFilterPlacement) return result;

            if (this.createdLabels == null || this.createdControls == null) return result;

            if (result is EmptyGridFilter && !this.showEmptyGridFilters) return result;

            var label = new Label { Text = columnStyle.HeaderText + ":" };
            this.createdLabels.Add(label);
            this.createdControls.Add(result.FilterControl);

            return result;
        }

        /// <summary>
        ///     Notifies this instance that the <see cref="IGridFilter" /> creation process
        ///     has finished. After this call all created <see cref="IGridFilter" />s should
        ///     be in a usable state.
        /// </summary>
        public void EndGridFilterCreation()
        {
            if (this.innerGridFilterFactory == null) return;

            this.innerGridFilterFactory.EndGridFilterCreation();

            if (this.createdLabels == null || this.createdControls == null) return;

            var labels = new Label[this.createdLabels.Count];
            this.createdLabels.CopyTo(labels);
            var controls = new Control[this.createdControls.Count];
            this.createdControls.CopyTo(controls);

            this.layoutedPanel.Fill(labels, controls);

            this.createdLabels = null;
            this.createdControls = null;
        }

        /// <summary>
        ///     Notification method to this instance that the filter
        ///     customization logic has changed and that the filters
        ///     need to be recreated
        /// </summary>
        public void HasChanged()
        {
            this.OnChanged();
        }

        /// <summary>
        ///     Die verwendeten Ressourcen bereinigen.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            this.InnerGridFilterFactory = null;

            if (disposing) this.components?.Dispose();

            base.Dispose(disposing);
        }

        /// <summary>
        ///     Erforderliche Methode für die Designerunterstützung.
        ///     Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            // LayoutedFilterFactoryControl
            this.Name = "LayoutedFilterFactoryControl";
            this.Size = new Size(456, 296);
        }

        private void OnChanged()
        {
            this.Changed?.Invoke(this, EventArgs.Empty);
        }

        private void OnGridFilterFactoryChanged(object sender, EventArgs e)
        {
            this.OnChanged();
        }

        private void OnGridFilterFactoryGridFilterCreated(GridFilterEventArgs args)
        {
            this.GridFilterCreated?.Invoke(this, args);
        }
    }
}