#region License
// Advanced DataGridView
//
// Original work Copyright (c), 2013 Zuby <zuby@me.com> 
// Modified work Copyright (c), 2014 Davide Gironi <davide.gironi@gmail.com>
//
// Please refer to LICENSE file for licensing information.
#endregion

namespace Zuby.ADGV
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    using Zuby.Properties;

    [DesignerCategory("")]
    internal class ColumnHeaderCell : DataGridViewColumnHeaderCell
    {

        #region public events

        public event ColumnHeaderCellEventHandler FilterPopup;
        public event ColumnHeaderCellEventHandler SortChanged;
        public event ColumnHeaderCellEventHandler FilterChanged;

        #endregion


        #region class properties

        private Image _filterImage = Resources.ColumnHeader_UnFiltered;
        private Size _filterButtonImageSize = new Size(16, 16);
        private bool _filterButtonPressed;
        private bool _filterButtonOver;
        private Rectangle _filterButtonOffsetBounds = Rectangle.Empty;
        private Rectangle _filterButtonImageBounds = Rectangle.Empty;
        private Padding _filterButtonMargin = new Padding(3, 4, 3, 4);
        private bool _filterEnabled;

        private const bool FilterDateAndTimeDefaultEnabled = false;

        #endregion


        #region constructors

        /// <summary>
        /// ColumnHeaderCell constructor
        /// </summary>
        /// <param name="oldCell"></param>
        /// <param name="filterEnabled"></param>
        public ColumnHeaderCell(DataGridViewColumnHeaderCell oldCell, bool filterEnabled)
        {
            Tag = oldCell.Tag;
            ErrorText = oldCell.ErrorText;
            ToolTipText = oldCell.ToolTipText;
            Value = oldCell.Value;
            ValueType = oldCell.ValueType;
            ContextMenuStrip = oldCell.ContextMenuStrip;
            Style = oldCell.Style;
            _filterEnabled = filterEnabled;

            var oldCellt = oldCell as ColumnHeaderCell;

            if (oldCellt?.MenuStrip != null)
            {
                MenuStrip = oldCellt.MenuStrip;
                _filterImage = oldCellt._filterImage;
                _filterButtonPressed = oldCellt._filterButtonPressed;
                _filterButtonOver = oldCellt._filterButtonOver;
                _filterButtonOffsetBounds = oldCellt._filterButtonOffsetBounds;
                _filterButtonImageBounds = oldCellt._filterButtonImageBounds;
                MenuStrip.FilterChanged += menuStrip_FilterChanged;
                MenuStrip.SortChanged += menuStrip_SortChanged;
            }
            else
            {
                MenuStrip = new MenuStrip(oldCell.OwningColumn.ValueType);
                MenuStrip.FilterChanged += menuStrip_FilterChanged;
                MenuStrip.SortChanged += menuStrip_SortChanged;
            }

            IsFilterDateAndTimeEnabled = FilterDateAndTimeDefaultEnabled;
            IsSortEnabled = true;
            IsFilterEnabled = true;
        }

        ~ColumnHeaderCell()
        {
            if (MenuStrip != null)
            {
                MenuStrip.FilterChanged -= menuStrip_FilterChanged;
                MenuStrip.SortChanged -= menuStrip_SortChanged;
            }
        }

        #endregion


        #region public clone

        /// <summary>
        /// Clone the ColumnHeaderCell
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new ColumnHeaderCell(this, FilterAndSortEnabled);
        }

        #endregion


        #region public methods

        /// <summary>
        /// Get or Set the Filter and Sort enabled status
        /// </summary>
        public bool FilterAndSortEnabled
        {
            get => _filterEnabled;
            set
            {
                if (!value)
                {
                    _filterButtonPressed = false;
                    _filterButtonOver = false;
                }

                if (value != _filterEnabled)
                {
                    _filterEnabled = value;
                    var refreshed = false;
                    if (MenuStrip.FilterString.Length > 0)
                    {
                        menuStrip_FilterChanged(this, new EventArgs());
                        refreshed = true;
                    }

                    if (MenuStrip.SortString.Length > 0)
                    {
                        menuStrip_SortChanged(this, new EventArgs());
                        refreshed = true;
                    }

                    if (!refreshed)
                        RepaintCell();
                }
            }
        }

        /// <summary>
        /// Set or Unset the Filter and Sort to Loaded mode
        /// </summary>
        /// <param name="enabled"></param>
        public void SetLoadedMode(bool enabled)
        {
            MenuStrip.SetLoadedMode(enabled);
            RefreshImage();
            RepaintCell();
        }

        /// <summary>
        /// Clean Sort
        /// </summary>
        public void CleanSort()
        {
            if (MenuStrip != null && FilterAndSortEnabled)
            {
                MenuStrip.CleanSort();
                RefreshImage();
                RepaintCell();
            }
        }

        /// <summary>
        /// Clean Filter
        /// </summary>
        public void CleanFilter()
        {
            if (MenuStrip != null && FilterAndSortEnabled)
            {
                MenuStrip.CleanFilter();
                RefreshImage();
                RepaintCell();
            }
        }

        /// <summary>
        /// Sort ASC
        /// </summary>
        public void SortASC()
        {
            if (MenuStrip != null && FilterAndSortEnabled)
            {
                MenuStrip.SortASC();
            }
        }

        /// <summary>
        /// Sort DESC
        /// </summary>
        public void SortDESC()
        {
            if (MenuStrip != null && FilterAndSortEnabled)
            {
                MenuStrip.SortDESC();
            }
        }

        /// <summary>
        /// Get the MenuStrip for this ColumnHeaderCell
        /// </summary>
        public MenuStrip MenuStrip { get; }

        /// <summary>
        /// Get the MenuStrip SortType
        /// </summary>
        public MenuStrip.SortType ActiveSortType
        {
            get
            {
                if (MenuStrip != null && FilterAndSortEnabled)
                    return MenuStrip.ActiveSortType;
                return MenuStrip.SortType.None;
            }
        }

        /// <summary>
        /// Get the MenuStrip FilterType
        /// </summary>
        public MenuStrip.FilterType ActiveFilterType
        {
            get
            {
                if (MenuStrip != null && FilterAndSortEnabled)
                    return MenuStrip.ActiveFilterType;
                return MenuStrip.FilterType.None;
            }
        }

        /// <summary>
        /// Get the Sort string
        /// </summary>
        public string SortString
        {
            get
            {
                if (MenuStrip != null && FilterAndSortEnabled)
                    return MenuStrip.SortString;
                return string.Empty;
            }
        }

        /// <summary>
        /// Get the Filter string
        /// </summary>
        public string FilterString
        {
            get
            {
                if (MenuStrip != null && FilterAndSortEnabled)
                    return MenuStrip.FilterString;
                return string.Empty;
            }
        }

        /// <summary>
        /// Get the Minimum size
        /// </summary>
        public Size MinimumSize => new Size(_filterButtonImageSize.Width + _filterButtonMargin.Left + _filterButtonMargin.Right,
            _filterButtonImageSize.Height + _filterButtonMargin.Bottom + _filterButtonMargin.Top);

        /// <summary>
        /// Get or Set the Sort enabled status
        /// </summary>
        public bool IsSortEnabled
        {
            get => MenuStrip.IsSortEnabled;
            set => MenuStrip.IsSortEnabled = value;
        }

        /// <summary>
        /// Get or Set the Filter enabled status
        /// </summary>
        public bool IsFilterEnabled
        {
            get => MenuStrip.IsFilterEnabled;
            set => MenuStrip.IsFilterEnabled = value;
        }

        /// <summary>
        /// Get or Set the FilterDateAndTime enabled status
        /// </summary>
        public bool IsFilterDateAndTimeEnabled
        {
            get => MenuStrip.IsFilterDateAndTimeEnabled;
            set => MenuStrip.IsFilterDateAndTimeEnabled = value;
        }

        /// <summary>
        /// Get or Set the NOT IN logic for Filter
        /// </summary>
        public bool IsMenuStripFilterNOTINLogicEnabled
        {
            get => MenuStrip.IsFilterNOTINLogicEnabled;
            set => MenuStrip.IsFilterNOTINLogicEnabled = value;
        }

        /// <summary>
        /// Enabled or disable Sort capabilities
        /// </summary>
        /// <param name="enabled"></param>
        public void SetSortEnabled(bool enabled)
        {
            if (MenuStrip != null)
            {
                MenuStrip.IsSortEnabled = enabled;
                MenuStrip.SetSortEnabled(enabled);
            }
        }

        /// <summary>
        /// Enable or disable Filter capabilities
        /// </summary>
        /// <param name="enabled"></param>
        public void SetFilterEnabled(bool enabled)
        {
            if (MenuStrip != null)
            {
                MenuStrip.IsFilterEnabled = enabled;
                MenuStrip.SetFilterEnabled(enabled);
            }
        }

        #endregion


        #region menustrip events

        /// <summary>
        /// OnFilterChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuStrip_FilterChanged(object sender, EventArgs e)
        {
            RefreshImage();
            RepaintCell();
            if (FilterAndSortEnabled)
                FilterChanged?.Invoke(this, new ColumnHeaderCellEventArgs(MenuStrip, OwningColumn));
        }

        /// <summary>
        /// OnSortChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuStrip_SortChanged(object sender, EventArgs e)
        {
            RefreshImage();
            RepaintCell();
            if (FilterAndSortEnabled)
                SortChanged?.Invoke(this, new ColumnHeaderCellEventArgs(MenuStrip, OwningColumn));
        }

        #endregion


        #region paint methods

        /// <summary>
        /// Repaint the Cell
        /// </summary>
        private void RepaintCell()
        {
            if (Displayed)
                DataGridView?.InvalidateCell(this);
        }

        /// <summary>
        /// Refrash the Cell image
        /// </summary>
        private void RefreshImage()
        {
            if (ActiveFilterType == MenuStrip.FilterType.Loaded)
            {
                _filterImage = Resources.ColumnHeader_SavedFilters;
            }
            else
            {
                if (ActiveFilterType == MenuStrip.FilterType.None)
                {
                    if (ActiveSortType == MenuStrip.SortType.None)
                        _filterImage = Resources.ColumnHeader_UnFiltered;
                    else if (ActiveSortType == MenuStrip.SortType.ASC)
                        _filterImage = Resources.ColumnHeader_OrderedASC;
                    else
                        _filterImage = Resources.ColumnHeader_OrderedDESC;
                }
                else
                {
                    if (ActiveSortType == MenuStrip.SortType.None)
                        _filterImage = Resources.ColumnHeader_Filtered;
                    else if (ActiveSortType == MenuStrip.SortType.ASC)
                        _filterImage = Resources.ColumnHeader_FilteredAndOrderedASC;
                    else
                        _filterImage = Resources.ColumnHeader_FilteredAndOrderedDESC;
                }
            }
        }

        /// <summary>
        /// Pain method
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="clipBounds"></param>
        /// <param name="cellBounds"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellState"></param>
        /// <param name="value"></param>
        /// <param name="formattedValue"></param>
        /// <param name="errorText"></param>
        /// <param name="cellStyle"></param>
        /// <param name="advancedBorderStyle"></param>
        /// <param name="paintParts"></param>
        protected override void Paint(
            Graphics graphics,
            Rectangle clipBounds,
            Rectangle cellBounds,
            int rowIndex,
            DataGridViewElementStates cellState,
            object value,
            object formattedValue,
            string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            if (SortGlyphDirection != SortOrder.None)
                SortGlyphDirection = SortOrder.None;

            base.Paint(graphics, clipBounds, cellBounds, rowIndex,
                cellState, value, formattedValue,
                errorText, cellStyle, advancedBorderStyle, paintParts);

            // Don't display a dropdown for Image columns
            if (OwningColumn.ValueType == typeof(Bitmap))
                return;

            if (FilterAndSortEnabled && paintParts.HasFlag(DataGridViewPaintParts.ContentBackground))
            {
                _filterButtonOffsetBounds = GetFilterBounds();
                _filterButtonImageBounds = GetFilterBounds(false);
                var buttonBounds = _filterButtonOffsetBounds;
                if (buttonBounds != null && clipBounds.IntersectsWith(buttonBounds))
                {
                    ControlPaint.DrawBorder(graphics, buttonBounds, Color.Gray, ButtonBorderStyle.Solid);
                    buttonBounds.Inflate(-1, -1);
                    using (Brush b = new SolidBrush(_filterButtonOver ? Color.WhiteSmoke : Color.White))
                        graphics.FillRectangle(b, buttonBounds);
                    graphics.DrawImage(_filterImage, buttonBounds);
                }
            }
        }

        /// <summary>
        /// Get the ColumnHeaderCell Bounds
        /// </summary>
        /// <param name="withOffset"></param>
        /// <returns></returns>
        private Rectangle GetFilterBounds(bool withOffset = true)
        {
            var cell = DataGridView.GetCellDisplayRectangle(ColumnIndex, -1, false);

            var p = new Point(
                (withOffset ? cell.Right : cell.Width) - _filterButtonImageSize.Width - _filterButtonMargin.Right,
                (withOffset ? cell.Bottom : cell.Height) - _filterButtonImageSize.Height - _filterButtonMargin.Bottom);

            return new Rectangle(p, _filterButtonImageSize);
        }

        #endregion


        #region mouse events

        /// <summary>
        /// OnMouseMove event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(DataGridViewCellMouseEventArgs e)
        {
            if (FilterAndSortEnabled)
            {
                if (_filterButtonImageBounds.Contains(e.X, e.Y) && !_filterButtonOver)
                {
                    _filterButtonOver = true;
                    RepaintCell();
                }
                else if (!_filterButtonImageBounds.Contains(e.X, e.Y) && _filterButtonOver)
                {
                    _filterButtonOver = false;
                    RepaintCell();
                }
            }

            base.OnMouseMove(e);
        }

        /// <summary>
        /// OnMouseDown event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (FilterAndSortEnabled && _filterButtonImageBounds.Contains(e.X, e.Y))
            {
                if (e.Button == MouseButtons.Left && !_filterButtonPressed)
                {
                    _filterButtonPressed = true;
                    _filterButtonOver = true;
                    RepaintCell();
                }
            }
            else
                base.OnMouseDown(e);
        }

        /// <summary>
        /// OnMouseUp event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            if (FilterAndSortEnabled && e.Button == MouseButtons.Left && _filterButtonPressed)
            {
                _filterButtonPressed = false;
                _filterButtonOver = false;
                RepaintCell();
                if (_filterButtonImageBounds.Contains(e.X, e.Y))
                {
                    FilterPopup?.Invoke(this, new ColumnHeaderCellEventArgs(MenuStrip, OwningColumn));
                }
            }

            base.OnMouseUp(e);
        }

        /// <summary>
        /// OnMouseLeave event
        /// </summary>
        /// <param name="rowIndex"></param>
        protected override void OnMouseLeave(int rowIndex)
        {
            if (FilterAndSortEnabled && _filterButtonOver)
            {
                _filterButtonOver = false;
                RepaintCell();
            }

            base.OnMouseLeave(rowIndex);
        }

        #endregion

    }

    internal delegate void ColumnHeaderCellEventHandler(object sender, ColumnHeaderCellEventArgs e);
    internal class ColumnHeaderCellEventArgs : EventArgs
    {
        public MenuStrip FilterMenu { get; }

        public DataGridViewColumn Column { get; }

        public ColumnHeaderCellEventArgs(MenuStrip filterMenu, DataGridViewColumn column)
        {
            FilterMenu = filterMenu;
            Column = column;
        }
    }

}