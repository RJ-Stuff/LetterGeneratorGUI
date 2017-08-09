namespace GridExtensions.GridFilterFactories
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    ///     A panel which positions <see cref="Control" />s with their
    ///     corresponding <see cref="Label" />s in a layouted way.
    /// </summary>
    public sealed class LayoutedPanel : Panel
    {
        private Control[] controls;

        private int controlsMinimumWidth = 40;

        private int horizontalSpacing;

        private Label[] labels;

        private bool rightAlignLabels;

        private int verticalSpacing = 4;

        /// <summary>
        ///     Creates a new instance
        /// </summary>
        public LayoutedPanel()
        {
            this.AutoScroll = true;
        }

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
            get => this.controlsMinimumWidth;
            set
            {
                if (value < 1) throw new ArgumentException("Value must not be smaller 0", "ControlsMinimumWidth");
                if (value == this.controlsMinimumWidth) return;

                this.controlsMinimumWidth = value;
                this.RefreshLayout();
            }
        }

        /// <summary>
        ///     Gets and sets the horizontal space between the labels and controls.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(0)]
        [Description("Gets and sets the horizontal space between the labels and controls.")]
        public int HorizontalSpacing
        {
            get => this.horizontalSpacing;
            set
            {
                if (value != this.horizontalSpacing)
                {
                    this.horizontalSpacing = value;
                    this.RefreshLayout();
                }
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
            get => this.rightAlignLabels;
            set
            {
                if (value != this.rightAlignLabels)
                {
                    this.rightAlignLabels = value;
                    this.RefreshLayout();
                }
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
            get => this.verticalSpacing;
            set
            {
                if (value != this.verticalSpacing)
                {
                    this.verticalSpacing = value;
                    this.RefreshLayout();
                }
            }
        }

        /// <summary>
        ///     Clear the contents of this instance.
        /// </summary>
        public void Clear()
        {
            this.labels = null;
            this.controls = null;
            this.Controls.Clear();
        }

        /// <summary>
        ///     Fills the instance with the given controls in the two arrays.
        ///     Both arrays must have the same size. Otherwise an <see cref="ArgumentException" />
        ///     will be thrown.
        /// </summary>
        /// <param name="labels">Array with <see cref="Label" /> objects</param>
        /// <param name="controls">Array with <see cref="Control" /> objects</param>
        public void Fill(Label[] labels, Control[] controls)
        {
            if (labels.Length != controls.Length)
                throw new ArgumentException(
                    "Number of specified labels must match the number of specified controls.",
                    "labels");

            this.Clear();

            this.labels = new Label[labels.Length];
            labels.CopyTo(this.labels, 0);
            this.controls = new Control[controls.Length];
            controls.CopyTo(this.controls, 0);

            foreach (var t in this.labels) this.Controls.Add(t);
            foreach (var t in this.controls) this.Controls.Add(t);

            this.RefreshLayout();
        }

        /// <summary>
        ///     Repositions the contents after the control has been resized.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.RefreshLayout();
        }

        private void RefreshLayout()
        {
            if (this.labels == null || this.controls == null) return;

            var maximumLabelWidth = 0;
            foreach (var t in this.labels)
            {
                t.AutoSize = true;
                maximumLabelWidth = Math.Max(maximumLabelWidth, t.Width);
            }

            var currentVerticalPosition = 0;
            for (var i = 0; i < this.labels.Length; i++)
            {
                var currentHeight = Math.Max(this.controls[i].Height, this.labels[i].Height);

                this.controls[i].Location = new Point(
                    maximumLabelWidth + this.horizontalSpacing,
                    currentVerticalPosition + (currentHeight - this.controls[i].Height) / 2);
                this.controls[i].Width = Math.Max(
                    this.controlsMinimumWidth,
                    this.ClientSize.Width - this.controls[i].Left);
                this.controls[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

                this.labels[i].Location = this.rightAlignLabels
                                              ? new Point(
                                                  maximumLabelWidth - this.labels[i].Width,
                                                  currentVerticalPosition + (currentHeight - this.labels[i].Height) / 2)
                                              : new Point(
                                                  0,
                                                  currentVerticalPosition
                                                  + (currentHeight - this.labels[i].Height) / 2);

                currentVerticalPosition += currentHeight + this.verticalSpacing;
            }

            this.AutoScrollMinSize = new Size(
                maximumLabelWidth + this.horizontalSpacing + this.controlsMinimumWidth,
                20);
        }
    }
}