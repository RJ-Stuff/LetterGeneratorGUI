#region License
// Advanced DataGridView
//
// Copyright (c), 2014 Davide Gironi <davide.gironi@gmail.com>
// Original work Copyright (c), 2013 Zuby <zuby@me.com>
//
// Please refer to LICENSE file for licensing information.
#endregion

namespace Zuby.ADGV
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    [DesignerCategory("")]
    public class AdvancedDataGridView : DataGridView
    {

        #region public events

        public event EventHandler SortStringChanged;

        public event EventHandler FilterStringChanged;

        #endregion


        #region class properties

        private readonly List<string> _sortOrderList = new List<string>();
        private readonly List<string> _filterOrderList = new List<string>();
        private readonly List<string> _filteredColumns = new List<string>();

        private bool _loadedFilter;
        private string _sortString;
        private string _filterString;

        #endregion

        #region public Filter and Sort methods

        /// <summary>
        /// Disable a Filter and Sort on a DataGridViewColumn
        /// </summary>
        /// <param name="column"></param>
        public void DisableFilterAndSort(DataGridViewColumn column)
        {
            if (Columns.Contains(column))
            {
                var cell = column.HeaderCell as ColumnHeaderCell;
                if (cell != null)
                {
                    if (cell.FilterAndSortEnabled && (cell.SortString.Length > 0 || cell.FilterString.Length > 0))
                    {
                        CleanFilter(true);
                        cell.FilterAndSortEnabled = false;
                    }
                    else
                        cell.FilterAndSortEnabled = false;
                    _filterOrderList.Remove(column.Name);
                    _sortOrderList.Remove(column.Name);
                    _filteredColumns.Remove(column.Name);
                }
            }
        }

        /// <summary>
        /// Enable a Filter and Sort on a DataGridViewColumn
        /// </summary>
        /// <param name="column"></param>
        public void EnableFilterAndSort(DataGridViewColumn column)
        {
            // this.DataSource = SortString;
            if (Columns.Contains(column))
            {
                var cell = column.HeaderCell as ColumnHeaderCell;
                if (cell != null)
                {
                    if (!cell.FilterAndSortEnabled && (cell.FilterString.Length > 0 || cell.SortString.Length > 0))
                        CleanFilter(true);

                    cell.FilterAndSortEnabled = true;
                    _filteredColumns.Remove(column.Name);

                    SetFilterDateAndTimeEnabled(column, cell.IsFilterDateAndTimeEnabled);
                    SetSortEnabled(column, cell.IsSortEnabled);
                    SetFilterEnabled(column, cell.IsFilterEnabled);
                }
                else
                {
                    column.SortMode = DataGridViewColumnSortMode.Programmatic;
                    cell = new ColumnHeaderCell(column.HeaderCell, true);
                    cell.SortChanged += cell_SortChanged;
                    cell.FilterChanged += cell_FilterChanged;
                    cell.FilterPopup += cell_FilterPopup;
                    column.MinimumWidth = cell.MinimumSize.Width;
                    if (ColumnHeadersHeight < cell.MinimumSize.Height)
                        ColumnHeadersHeight = cell.MinimumSize.Height;
                    column.HeaderCell = cell;
                }
            }
        }

        /// <summary>
        /// Enabled or disable Filter and Sort capabilities on a DataGridViewColumn
        /// </summary>
        /// <param name="column"></param>
        /// <param name="enabled"></param>
        public void SetFilterAndSortEnabled(DataGridViewColumn column, bool enabled)
        {
            if (enabled)
                EnableFilterAndSort(column);
            else
                DisableFilterAndSort(column);
        }

        /// <summary>
        /// Load a Filter and Sort preset
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sorting"></param>
        public void LoadFilterAndSort(string filter, string sorting)
        {
            foreach (var c in FilterableCells)
                c.SetLoadedMode(true);

            _filteredColumns.Clear();

            _filterOrderList.Clear();
            _sortOrderList.Clear();

            if (filter != null)
                FilterString = filter;
            if (sorting != null)
                SortString = sorting;

            _loadedFilter = true;
        }

        /// <summary>
        /// Clean Filter and Sort
        /// </summary>
        public void CleanFilterAndSort()
        {
            foreach (var c in FilterableCells)
                c.SetLoadedMode(false);

            _filteredColumns.Clear();
            _filterOrderList.Clear();
            _sortOrderList.Clear();

            _loadedFilter = false;

            CleanFilter();
            CleanSort();
        }

        /// <summary>
        /// Set the NOTIN Logic for checkbox filter
        /// </summary>
        /// <param name="enabled"></param>
        public void SetMenuStripFilterNOTINLogic(bool enabled)
        {
            foreach (var c in FilterableCells)
                c.IsMenuStripFilterNOTINLogicEnabled = enabled;
        }

        /// <summary>
        /// Get or Set Filter and Sort status
        /// </summary>
        public bool FilterAndSortEnabled
        {
            get => _filterAndSortEnabled;
            set => _filterAndSortEnabled = value;
        }
        private bool _filterAndSortEnabled = true;

        #endregion


        #region public Sort methods

        /// <summary>
        /// Get the Sort string
        /// </summary>
        public string SortString
        {
            get => _sortString ?? string.Empty;
            private set
            {
                var old = value;
                if (old == _sortString) return;
                _sortString = value;

                SortStringChanged?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Enabled or disable Sort capabilities for a DataGridViewColumn
        /// </summary>
        /// <param name="column"></param>
        /// <param name="enabled"></param>
        public void SetSortEnabled(DataGridViewColumn column, bool enabled)
        {
            if (Columns.Contains(column))
            {
                var cell = column.HeaderCell as ColumnHeaderCell;
                cell?.SetSortEnabled(enabled);
            }
        }

        /// <summary>
        /// Sort ASC
        /// </summary>
        public void SortASC(DataGridViewColumn column)
        {
            if (Columns.Contains(column))
            {
                var cell = column.HeaderCell as ColumnHeaderCell;
                cell?.SortASC();
            }
        }

        /// <summary>
        /// Sort ASC
        /// </summary>
        public void SortDESC(DataGridViewColumn column)
        {
            if (Columns.Contains(column))
            {
                var cell = column.HeaderCell as ColumnHeaderCell;
                cell?.SortDESC();
            }
        }

        /// <summary>
        /// Clean all Sort on all columns
        /// </summary>
        /// <param name="fireEvent"></param>
        public void CleanSort(bool fireEvent)
        {
            foreach (var c in FilterableCells)
                c.CleanSort();
            _sortOrderList.Clear();

            if (fireEvent)
                SortString = null;
            else
                _sortString = null;
        }

        public void CleanSort()
        {
            CleanSort(true);
        }

        #endregion


        #region public Filter methods

        /// <summary>
        /// Get the Filter string
        /// </summary>
        public string FilterString
        {
            get => _filterString ?? string.Empty;
            private set
            {
                var old = value;
                if (old == _filterString) return;
                _filterString = value;
                FilterStringChanged?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Set FilterDateAndTime status for a DataGridViewColumn
        /// </summary>
        /// <param name="column"></param>
        /// <param name="enabled"></param>
        public void SetFilterDateAndTimeEnabled(DataGridViewColumn column, bool enabled)
        {
            if (Columns.Contains(column))
            {
                var cell = column.HeaderCell as ColumnHeaderCell;
                if (cell != null)
                {
                    cell.IsFilterDateAndTimeEnabled = enabled;
                }
            }
        }

        /// <summary>
        /// Enable or disable Filter capabilities for a DataGridViewColumn
        /// </summary>
        /// <param name="column"></param>
        /// <param name="enabled"></param>
        public void SetFilterEnabled(DataGridViewColumn column, bool enabled)
        {
            if (Columns.Contains(column))
            {
                var cell = column.HeaderCell as ColumnHeaderCell;
                cell?.SetFilterEnabled(enabled);
            }
        }

        /// <summary>
        /// Clean Filter on all columns
        /// </summary>
        /// <param name="fireEvent"></param>
        public void CleanFilter(bool fireEvent)
        {
            foreach (var c in FilterableCells)
            {
                c.CleanFilter();
            }

            _filterOrderList.Clear();

            if (fireEvent)
                FilterString = null;
            else
                _filterString = null;
        }

        public void CleanFilter()
        {
            CleanFilter(true);
        }

        #endregion


        #region public Find methods

        /// <summary>
        /// Find the Cell with the given value
        /// </summary>
        /// <param name="valueToFind"></param>
        /// <param name="columnName"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <param name="isWholeWordSearch"></param>
        /// <param name="isCaseSensitive"></param>
        /// <returns></returns>
        public DataGridViewCell FindCell(string valueToFind, string columnName, int rowIndex, int columnIndex, bool isWholeWordSearch, bool isCaseSensitive)
        {
            if (valueToFind == null || RowCount <= 0 || ColumnCount <= 0
                || (columnName != null && (!Columns.Contains(columnName)
                                           || !Columns[columnName].Visible))) return null;
            rowIndex = Math.Max(0, rowIndex);

            if (!isCaseSensitive)
                valueToFind = valueToFind.ToLower();

            if (columnName != null)
            {
                var c = Columns[columnName].Index;
                if (columnIndex > c)
                    rowIndex++;
                for (var r = rowIndex; r < RowCount; r++)
                {
                    var value = Rows[r].Cells[c].FormattedValue.ToString();
                    if (!isCaseSensitive)
                        value = value.ToLower();

                    if ((!isWholeWordSearch && value.Contains(valueToFind)) || value.Equals(valueToFind))
                        return Rows[r].Cells[c];
                }
            }
            else
            {
                columnIndex = Math.Max(0, columnIndex);

                for (var r = rowIndex; r < RowCount; r++)
                {
                    for (var c = columnIndex; c < ColumnCount; c++)
                    {
                        if (!Rows[r].Cells[c].Visible)
                            continue;

                        var value = Rows[r].Cells[c].FormattedValue.ToString();
                        if (!isCaseSensitive)
                            value = value.ToLower();

                        if ((!isWholeWordSearch && value.Contains(valueToFind)) || value.Equals(valueToFind))
                            return Rows[r].Cells[c];
                    }

                    columnIndex = 0;
                }
            }

            return null;
        }

        #endregion


        #region cells methods

        /// <summary>
        /// Get all columns
        /// </summary>
        private IEnumerable<ColumnHeaderCell> FilterableCells => from DataGridViewColumn c in Columns
                                                                 where c.HeaderCell is ColumnHeaderCell
                                                                 select c.HeaderCell as ColumnHeaderCell;

        #endregion


        #region column events

        /// <summary>
        /// Overriden  OnColumnAdded event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        {
            e.Column.SortMode = DataGridViewColumnSortMode.Programmatic;
            var cell = new ColumnHeaderCell(e.Column.HeaderCell, FilterAndSortEnabled);
            cell.SortChanged += cell_SortChanged;
            cell.FilterChanged += cell_FilterChanged;
            cell.FilterPopup += cell_FilterPopup;
            e.Column.MinimumWidth = cell.MinimumSize.Width;
            if (ColumnHeadersHeight < cell.MinimumSize.Height)
                ColumnHeadersHeight = cell.MinimumSize.Height;
            e.Column.HeaderCell = cell;

            base.OnColumnAdded(e);
        }

        /// <summary>
        /// Overridden OnColumnRemoved event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnColumnRemoved(DataGridViewColumnEventArgs e)
        {
            _filteredColumns.Remove(e.Column.Name);
            _filterOrderList.Remove(e.Column.Name);
            _sortOrderList.Remove(e.Column.Name);

            var cell = e.Column.HeaderCell as ColumnHeaderCell;
            if (cell != null)
            {
                cell.SortChanged -= cell_SortChanged;
                cell.FilterChanged -= cell_FilterChanged;
                cell.FilterPopup -= cell_FilterPopup;
            }

            base.OnColumnRemoved(e);
        }

        #endregion


        #region rows events

        /// <summary>
        /// Overridden OnRowsAdded event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs e)
        {
            _filteredColumns.Clear();
            base.OnRowsAdded(e);
        }

        /// <summary>
        /// Overridden OnRowsRemoved event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowsRemoved(DataGridViewRowsRemovedEventArgs e)
        {
            _filteredColumns.Clear();
            base.OnRowsRemoved(e);
        }

        #endregion


        #region cell events

        /// <summary>
        /// Overridden OnCellValueChanged event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellValueChanged(DataGridViewCellEventArgs e)
        {
            _filteredColumns.Remove(Columns[e.ColumnIndex].Name);
            base.OnCellValueChanged(e);
        }

        #endregion


        #region filter events

        /// <summary>
        /// Build the complete Filter string
        /// </summary>
        /// <returns></returns>
        private string BuildFilterString()
        {
            var sb = new StringBuilder(string.Empty);
            var appx = string.Empty;

            foreach (var filterOrder in _filterOrderList)
            {
                var Column = Columns[filterOrder];

                var cell = Column?.HeaderCell as ColumnHeaderCell;
                if (cell != null)
                {
                    if (cell.FilterAndSortEnabled && cell.ActiveFilterType != MenuStrip.FilterType.None)
                    {
                        sb.AppendFormat(appx + "(" + cell.FilterString + ")", Column.DataPropertyName);
                        appx = " AND ";
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// FilterPopup event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cell_FilterPopup(object sender, ColumnHeaderCellEventArgs e)
        {
            if (Columns.Contains(e.Column))
            {
                var filterMenu = e.FilterMenu;
                var column = e.Column;

                var rect = GetCellDisplayRectangle(column.Index, -1, true);

                if (_filteredColumns.Contains(column.Name))
                    filterMenu.Show(this, rect.Left, rect.Bottom, false);
                else
                {
                    _filteredColumns.Add(column.Name);
                    if (_filterOrderList.Count > 0 && _filterOrderList.Last() == column.Name)
                        filterMenu.Show(this, rect.Left, rect.Bottom, true);
                    else
                        filterMenu.Show(this, rect.Left, rect.Bottom, MenuStrip.GetValuesForFilter(this, column.Name));
                }
            }
        }

        /// <summary>
        /// FilterChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cell_FilterChanged(object sender, ColumnHeaderCellEventArgs e)
        {
            if (Columns.Contains(e.Column))
            {
                var filterMenu = e.FilterMenu;
                var column = e.Column;

                _filterOrderList.Remove(column.Name);
                if (filterMenu.ActiveFilterType != MenuStrip.FilterType.None)
                    _filterOrderList.Add(column.Name);

                FilterString = BuildFilterString();

                if (_loadedFilter)
                {
                    _loadedFilter = false;
                    foreach (var c in FilterableCells.Where(f => f.MenuStrip != filterMenu))
                        c.SetLoadedMode(false);
                }
            }
        }

        #endregion


        #region sort events

        /// <summary>
        /// Build the complete Sort string
        /// </summary>
        /// <returns></returns>
        private string BuildSortString()
        {
            var sb = new StringBuilder(string.Empty);
            var appx = string.Empty;

            foreach (var sortOrder in _sortOrderList)
            {
                var column = Columns[sortOrder];

                var cell = column?.HeaderCell as ColumnHeaderCell;
                if (cell != null)
                {
                    if (cell.FilterAndSortEnabled && cell.ActiveSortType != MenuStrip.SortType.None)
                    {
                        sb.AppendFormat(appx + cell.SortString, column.DataPropertyName);
                        appx = ", ";
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// SortChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cell_SortChanged(object sender, ColumnHeaderCellEventArgs e)
        {
            if (Columns.Contains(e.Column))
            {
                var filterMenu = e.FilterMenu;
                var column = e.Column;

                _sortOrderList.Remove(column.Name);
                if (filterMenu.ActiveSortType != MenuStrip.SortType.None)
                    _sortOrderList.Add(column.Name);
                SortString = BuildSortString();
            }
        }

        #endregion

    }
}