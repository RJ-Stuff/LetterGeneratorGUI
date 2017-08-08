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
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;

    using Zuby.Properties;

    [DesignerCategory("")]
    internal partial class MenuStrip : ContextMenuStrip
    {

        #region public enum

        /// <summary>
        /// MenuStrip Filter type
        /// </summary>
        public enum FilterType : byte
        {
            None = 0,
            Custom = 1,
            CheckList = 2,
            Loaded = 3
        }


        /// <summary>
        /// MenuStrip Sort type
        /// </summary>
        public enum SortType : byte
        {
            None = 0,
            ASC = 1,
            DESC = 2
        }

        #endregion


        #region class properties

        private readonly Hashtable _textStrings = new Hashtable();

        private FilterType _activeFilterType = FilterType.None;
        private SortType _activeSortType = SortType.None;
        private TreeNodeItemSelector[] _startingNodes;
        private TreeNodeItemSelector[] _filterNodes;
        private string _sortString;
        private string _filterString;
        private static readonly Point _resizeStartPoint = new Point(1, 1);
        private Point _resizeEndPoint = new Point(-1, -1);

        private static readonly bool _checkTextFilterRemoveNodesOnSearch = true;

        #endregion


        #region costructors

        /// <summary>
        /// MenuStrip constructor
        /// </summary>
        /// <param name="dataType"></param>
        public MenuStrip(Type dataType)
        {
            // set localization strings
            _textStrings.Add("SORTDATETIMEASC", "Sort Oldest to Newest");
            _textStrings.Add("SORTDATETIMEDESC", "Sort Newest to Oldest");
            _textStrings.Add("SORTBOOLASC", "Sort by False/True");
            _textStrings.Add("SORTBOOLDESC", "Sort by True/False");
            _textStrings.Add("SORTNUMASC", "Sort Smallest to Largest");
            _textStrings.Add("SORTNUMDESC", "Sort Largest to Smallest");
            _textStrings.Add("SORTTEXTASC", "Sort А to Z");
            _textStrings.Add("SORTTEXTDESC", "Sort Z to A");
            _textStrings.Add("ADDCUSTOMFILTER", "Add a Custom Filter");
            _textStrings.Add("CUSTOMFILTER", "Custom Filter");
            _textStrings.Add("CLEARFILTER", "Clear Filter");
            _textStrings.Add("CLEARSORT", "Clear Sort");
            _textStrings.Add("BUTTONOK", "Filter");
            _textStrings.Add("BUTTONCANCEL", "Cancel");
            _textStrings.Add("NODESELECTALL", "(Select All)");
            _textStrings.Add("NODESELECTEMPTY", "(Blanks)");

            // initialize components
            InitializeComponent();

            // set type
            DataType = dataType;

            // set components values
            if (DataType == typeof(DateTime) || DataType == typeof(TimeSpan))
            {
                customFilterLastFiltersListMenuItem.Text = _textStrings["CUSTOMFILTER"].ToString();
                sortASCMenuItem.Text = _textStrings["SORTDATETIMEASC"].ToString();
                sortDESCMenuItem.Text = _textStrings["SORTDATETIMEDESC"].ToString();
                sortASCMenuItem.Image = Resources.MenuStrip_OrderASCnum;
                sortDESCMenuItem.Image = Resources.MenuStrip_OrderDESCnum;
            }
            else if (DataType == typeof(bool))
            {
                customFilterLastFiltersListMenuItem.Text = _textStrings["CUSTOMFILTER"].ToString();
                sortASCMenuItem.Text = _textStrings["SORTBOOLASC"].ToString();
                sortDESCMenuItem.Text = _textStrings["SORTBOOLDESC"].ToString();
                sortASCMenuItem.Image = Resources.MenuStrip_OrderASCbool;
                sortDESCMenuItem.Image = Resources.MenuStrip_OrderDESCbool;
            }
            else if (DataType == typeof(Int32) || DataType == typeof(Int64) || DataType == typeof(Int16)
                     || DataType == typeof(UInt32) || DataType == typeof(UInt64) || DataType == typeof(UInt16)
                     || DataType == typeof(Byte) || DataType == typeof(SByte) || DataType == typeof(Decimal)
                     || DataType == typeof(Single) || DataType == typeof(Double))
            {
                customFilterLastFiltersListMenuItem.Text = _textStrings["CUSTOMFILTER"].ToString();
                sortASCMenuItem.Text = _textStrings["SORTNUMASC"].ToString();
                sortDESCMenuItem.Text = _textStrings["SORTNUMDESC"].ToString();
                sortASCMenuItem.Image = Resources.MenuStrip_OrderASCnum;
                sortDESCMenuItem.Image = Resources.MenuStrip_OrderDESCnum;
            }
            else
            {
                customFilterLastFiltersListMenuItem.Text = _textStrings["CUSTOMFILTER"].ToString();
                sortASCMenuItem.Text = _textStrings["SORTTEXTASC"].ToString();
                sortDESCMenuItem.Text = _textStrings["SORTTEXTDESC"].ToString();
                sortASCMenuItem.Image = Resources.MenuStrip_OrderASCtxt;
                sortDESCMenuItem.Image = Resources.MenuStrip_OrderDESCtxt;
            }

            // set check filter textbox
            if (DataType == typeof(DateTime) || DataType == typeof(TimeSpan) || DataType == typeof(bool))
                checkTextFilter.Enabled = false;

            // set default NOT IN logic
            IsFilterNOTINLogicEnabled = false;

            // set default compoents
            customFilterLastFiltersListMenuItem.Enabled = DataType != typeof(bool);
            customFilterLastFiltersListMenuItem.Checked = ActiveFilterType == FilterType.Custom;
            MinimumSize = new Size(PreferredSize.Width, PreferredSize.Height);

            // resize
            ResizeBox(MinimumSize.Width, MinimumSize.Height);
        }

        /// <summary>
        /// Closed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuStrip_Closed(object sender, EventArgs e)
        {
            ResizeClean();

            checkTextFilter.Text = string.Empty;
            _startingNodes = null;
        }

        /// <summary>
        /// LostFocust event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuStrip_LostFocus(object sender, EventArgs e)
        {
            if (!ContainsFocus)
                Close();
        }

        /// <summary>
        /// Get all images for checkList
        /// </summary>
        /// <returns></returns>
        private ImageList GetCheckListStateImages()
        {
            var images = new ImageList();
            Bitmap unCheckImg;// = new Bitmap(16, 16);
            Bitmap checkImg;// = new Bitmap(16, 16);
            Bitmap mixedImg;// = new Bitmap(16, 16);

            using (var img = new Bitmap(16, 16))
            {
                using (var g = Graphics.FromImage(img))
                {
                    CheckBoxRenderer.DrawCheckBox(g, new Point(0, 1), CheckBoxState.UncheckedNormal);
                    unCheckImg = (Bitmap)img.Clone();
                    CheckBoxRenderer.DrawCheckBox(g, new Point(0, 1), CheckBoxState.CheckedNormal);
                    checkImg = (Bitmap)img.Clone();
                    CheckBoxRenderer.DrawCheckBox(g, new Point(0, 1), CheckBoxState.MixedNormal);
                    mixedImg = (Bitmap)img.Clone();
                }
            }

            images.Images.Add("uncheck", unCheckImg);
            images.Images.Add("check", checkImg);
            images.Images.Add("mixed", mixedImg);

            return images;
        }

        #endregion


        #region public events

        /// <summary>
        /// The current Sorting in changed
        /// </summary>
        public event EventHandler SortChanged;

        /// <summary>
        /// The current Filter is changed
        /// </summary>
        public event EventHandler FilterChanged;

        #endregion


        #region public getter and setters

        /// <summary>
        /// Get the current MenuStripSortType type
        /// </summary>
        public SortType ActiveSortType => _activeSortType;

        /// <summary>
        /// Get the current MenuStripFilterType type
        /// </summary>
        public FilterType ActiveFilterType => _activeFilterType;

        /// <summary>
        /// Get the DataType for the MenuStrip Filter
        /// </summary>
        public Type DataType { get; }

        /// <summary>
        /// Get or Set the Filter Sort enabled
        /// </summary>
        public bool IsSortEnabled { get; set; }

        /// <summary>
        /// Get or Set the Filter Sort enabled
        /// </summary>
        public bool IsFilterEnabled { get; set; }

        /// <summary>
        /// Get or Set the Filter DateAndTime enabled
        /// </summary>
        public bool IsFilterDateAndTimeEnabled { get; set; }

        /// <summary>
        /// Get or Set the NOT IN logic for Filter
        /// </summary>
        public bool IsFilterNOTINLogicEnabled { get; set; }

        #endregion


        #region public enablers

        /// <summary>
        /// Enabled or disable Sorting capabilities
        /// </summary>
        /// <param name="enabled"></param>
        public void SetSortEnabled(bool enabled)
        {
            if (!IsSortEnabled)
                enabled = false;

            cancelSortMenuItem.Enabled = enabled;

            sortASCMenuItem.Enabled = enabled;
            sortDESCMenuItem.Enabled = enabled;
        }

        /// <summary>
        /// Enable or disable Filter capabilities
        /// </summary>
        /// <param name="enabled"></param>
        public void SetFilterEnabled(bool enabled)
        {
            if (!IsFilterEnabled)
                enabled = false;

            cancelFilterMenuItem.Enabled = enabled;
            button_ok.Enabled = enabled;
            button_cancel.Enabled = enabled;
            checkList.Enabled = enabled;
            if (enabled)
                customFilterLastFiltersListMenuItem.Enabled = DataType != typeof(bool);
            else
                customFilterLastFiltersListMenuItem.Enabled = false;
        }

        #endregion


        #region preset loader

        public void SetLoadedMode(bool enabled)
        {
            customFilterMenuItem.Enabled = !enabled;
            cancelFilterMenuItem.Enabled = enabled;
            if (enabled)
            {
                _activeFilterType = FilterType.Loaded;
                _sortString = null;
                _filterString = null;
                _filterNodes = null;
                customFilterLastFiltersListMenuItem.Checked = false;
                for (var i = 2; i < customFilterLastFiltersListMenuItem.DropDownItems.Count - 1; i++)
                {
                    (customFilterLastFiltersListMenuItem.DropDownItems[i] as ToolStripMenuItem).Checked = false;
                }

                checkList.Nodes.Clear();
                var allnode = TreeNodeItemSelector.CreateNode("(Select All)" + "            ", null, CheckState.Checked, TreeNodeItemSelector.CustomNodeType.SelectAll);
                allnode.NodeFont = new Font(checkList.Font, FontStyle.Bold);
                allnode.CheckState = CheckState.Indeterminate;
                checkList.Nodes.Add(allnode);

                SetSortEnabled(false);
                SetFilterEnabled(false);
            }
            else
            {
                _activeFilterType = FilterType.None;

                SetSortEnabled(true);
                SetFilterEnabled(true);
            }
        }

        #endregion


        #region public show methods

        /// <summary>
        /// Show the menuStrip
        /// </summary>
        /// <param name="control"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="vals"></param>
        public void Show(Control control, int x, int y, IEnumerable<DataGridViewCell> vals)
        {
            BuildNodes(vals);

            checkTextFilter.Text = string.Empty;
            if (_activeFilterType == FilterType.Custom)
                SetNodesCheckState(checkList.Nodes, false);
            DuplicateNodes();
            Show(control, x, y);
        }

        /// <summary>
        /// Show the menuStrip
        /// </summary>
        /// <param name="control"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="_restoreFilter"></param>
        public void Show(Control control, int x, int y, bool _restoreFilter)
        {
            checkTextFilter.Text = string.Empty;
            if (_restoreFilter)
                RestoreFilterNodes();
            DuplicateNodes();
            Show(control, x, y);
        }

        /// <summary>
        /// Get values used for Show method
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static IEnumerable<DataGridViewCell> GetValuesForFilter(DataGridView grid, string columnName)
        {
            var vals =
                from DataGridViewRow nulls in grid.Rows
                select nulls.Cells[columnName];

            return vals;
        }

        #endregion


        #region public sort methods

        /// <summary>
        /// Sort ASC
        /// </summary>
        public void SortASC()
        {
            sortASCMenuItem_Click(this, null);
        }

        /// <summary>
        /// Sort DESC
        /// </summary>
        public void SortDESC()
        {
            sortDESCMenuItem_Click(this, null);
        }

        /// <summary>
        /// Get the Sorting String
        /// </summary>
        public string SortString
        {
            get => _sortString ?? string.Empty;
            private set
            {
                cancelSortMenuItem.Enabled = !string.IsNullOrEmpty(value);
                _sortString = value;
            }
        }

        /// <summary>
        /// Clean the Sorting
        /// </summary>
        public void CleanSort()
        {
            sortASCMenuItem.Checked = false;
            sortDESCMenuItem.Checked = false;
            _activeSortType = SortType.None;
            SortString = null;
        }

        #endregion


        #region public filter methods

        /// <summary>
        /// Get the Filter string
        /// </summary>
        public string FilterString
        {
            get => _filterString ?? string.Empty;

            private set
            {
                cancelFilterMenuItem.Enabled = !string.IsNullOrEmpty(value);
                _filterString = value;
            }
        }

        /// <summary>
        /// Clean the Filter
        /// </summary>
        public void CleanFilter()
        {
            for (var i = 2; i < customFilterLastFiltersListMenuItem.DropDownItems.Count - 1; i++)
            {
                (customFilterLastFiltersListMenuItem.DropDownItems[i] as ToolStripMenuItem).Checked = false;
            }

            _activeFilterType = FilterType.None;
            SetNodesCheckState(checkList.Nodes, true);
            FilterString = null;
            _filterNodes = null;
            customFilterLastFiltersListMenuItem.Checked = false;
            button_ok.Enabled = true;
        }

        #endregion


        #region checklist filter methods

        /// <summary>
        /// Set the Filter String using checkList selected Nodes
        /// </summary>
        private void SetCheckListFilter()
        {
            UnCheckCustomFilters();

            var selectAllNode = GetSelectAllNode();
            customFilterLastFiltersListMenuItem.Checked = false;

            if (selectAllNode != null && selectAllNode.Checked)
                cancelFilterMenuItem_Click(null, new EventArgs());
            else
            {
                var oldfilter = FilterString;
                FilterString = string.Empty;
                _activeFilterType = FilterType.CheckList;

                if (checkList.Nodes.Count > 1)
                {
                    selectAllNode = GetSelectEmptyNode();
                    if (selectAllNode != null && selectAllNode.Checked)
                        FilterString = "[{0}] IS NULL";

                    if (checkList.Nodes.Count > 2 || selectAllNode == null)
                    {
                        var filter = BuildNodesFilterString(
                            IsFilterNOTINLogicEnabled && (DataType != typeof(DateTime) && DataType != typeof(TimeSpan) && DataType != typeof(bool)) ?
                                checkList.Nodes.AsParallel().Cast<TreeNodeItemSelector>().Where(
                                    n => n.NodeType != TreeNodeItemSelector.CustomNodeType.SelectAll
                                        && n.NodeType != TreeNodeItemSelector.CustomNodeType.SelectEmpty
                                        && n.CheckState == CheckState.Unchecked
                                ) :
                                checkList.Nodes.AsParallel().Cast<TreeNodeItemSelector>().Where(
                                    n => n.NodeType != TreeNodeItemSelector.CustomNodeType.SelectAll
                                        && n.NodeType != TreeNodeItemSelector.CustomNodeType.SelectEmpty
                                        && n.CheckState != CheckState.Unchecked
                                )
                        );

                        if (filter.Length > 0)
                        {
                            if (FilterString.Length > 0)
                                FilterString += " OR ";

                            if (DataType == typeof(DateTime) || DataType == typeof(TimeSpan))
                                FilterString += filter;
                            else if (DataType == typeof(bool))
                                FilterString += "{0}=" + filter;
                            else if (DataType == typeof(Int32) || DataType == typeof(Int64) || DataType == typeof(Int16) ||
                                        DataType == typeof(UInt32) || DataType == typeof(UInt64) || DataType == typeof(UInt16) ||
                                        DataType == typeof(Decimal) ||
                                        DataType == typeof(Byte) || DataType == typeof(SByte) || DataType == typeof(String))
                            {
                                if (IsFilterNOTINLogicEnabled)
                                    FilterString += "[{0}] NOT IN (" + filter + ")";
                                else
                                    FilterString += "[{0}] IN (" + filter + ")";
                            }
                            else if (DataType == typeof(Double))
                            {
                                if (IsFilterNOTINLogicEnabled)
                                    FilterString += "Convert([{0}],System.String) NOT IN (" + filter + ")";
                                else
                                    FilterString += "Convert([{0}],System.String) IN (" + filter + ")";
                            }
                            else if (DataType == typeof(Bitmap))
                            { }
                            else
                            {
                                if (IsFilterNOTINLogicEnabled)
                                    FilterString += "Convert([{0}],System.String) NOT IN (" + filter + ")";
                                else
                                    FilterString += "Convert([{0}],System.String) IN (" + filter + ")";
                            }
                        }
                    }
                }

                DuplicateFilterNodes();

                if (oldfilter != FilterString)
                    FilterChanged?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Build a Filter string based on selectd Nodes
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private string BuildNodesFilterString(IEnumerable<TreeNodeItemSelector> nodes)
        {
            var sb = new StringBuilder(string.Empty);
            var appx = (DataType == typeof(DateTime) || DataType == typeof(TimeSpan)) ? " OR " : ", ";

            if (nodes != null && nodes.Any())
            {
                if (DataType == typeof(DateTime))
                {
                    foreach (var n in nodes)
                    {
                        if (n.Checked && !n.Nodes.AsParallel().Cast<TreeNodeItemSelector>().Any(sn => sn.CheckState != CheckState.Unchecked))
                        {
                            var dt = (DateTime)n.Value;
                            sb.Append("(Convert([{0}], 'System.String') LIKE '%" + Convert.ToString(IsFilterDateAndTimeEnabled ? dt : dt.Date, CultureInfo.CurrentCulture) + "%')" + appx);
                        }
                        else if (n.CheckState != CheckState.Unchecked && n.Nodes.Count > 0)
                        {
                            if (BuildNodesFilterString(n.Nodes.AsParallel().Cast<TreeNodeItemSelector>().Where(sn => sn.CheckState != CheckState.Unchecked)).Length > 0)
                                sb.Append(BuildNodesFilterString(n.Nodes.AsParallel().Cast<TreeNodeItemSelector>().Where(sn => sn.CheckState != CheckState.Unchecked)) + appx);
                        }
                    }
                }
                else if (DataType == typeof(TimeSpan))
                {
                    foreach (var n in nodes)
                    {
                        if (n.Checked && !n.Nodes.AsParallel().Cast<TreeNodeItemSelector>().Any(sn => sn.CheckState != CheckState.Unchecked))
                        {
                            var ts = (TimeSpan)n.Value;
                            sb.Append("(Convert([{0}], 'System.String') LIKE '%P" + (ts.Days > 0 ? ts.Days + "D" : string.Empty) + (ts.TotalHours > 0 ? "T" : string.Empty) + (ts.Hours > 0 ? ts.Hours + "H" : string.Empty) + (ts.Minutes > 0 ? ts.Minutes + "M" : string.Empty) + (ts.Seconds > 0 ? ts.Seconds + "S" : string.Empty) + "%')" + appx);
                        }
                        else if (n.CheckState != CheckState.Unchecked && n.Nodes.Count > 0)
                        {
                            var subnode = BuildNodesFilterString(n.Nodes.AsParallel().Cast<TreeNodeItemSelector>().Where(sn => sn.CheckState != CheckState.Unchecked));
                            if (subnode.Length > 0)
                                sb.Append(subnode + appx);
                        }
                    }
                }
                else if (DataType == typeof(bool))
                {
                    foreach (var n in nodes)
                    {
                        sb.Append(n.Value);
                        break;
                    }
                }
                else if (DataType == typeof(Int32) || DataType == typeof(Int64) || DataType == typeof(Int16) ||
                    DataType == typeof(UInt32) || DataType == typeof(UInt64) || DataType == typeof(UInt16) ||
                    DataType == typeof(Byte) || DataType == typeof(SByte))
                {
                    foreach (var n in nodes)
                        sb.Append(n.Value + appx);
                }
                else if (DataType == typeof(Single) || DataType == typeof(Double) || DataType == typeof(Decimal))
                {
                    foreach (var n in nodes)
                        sb.Append(n.Value.ToString().Replace(",", ".") + appx);
                }
                else if (DataType == typeof(Bitmap))
                { }
                else
                {
                    foreach (var n in nodes)
                        sb.Append("'" + FormatFilterString(n.Value.ToString()) + "'" + appx);
                }
            }

            if (sb.Length > appx.Length && DataType != typeof(bool))
                sb.Remove(sb.Length - appx.Length, appx.Length);

            return sb.ToString();
        }

        /// <summary>
        /// Format a text Filter string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string FormatFilterString(string text)
        {
            return text.Replace("'", "''");
        }

        /// <summary>
        /// Add nodes to checkList
        /// </summary>
        /// <param name="vals"></param>
        private void BuildNodes(IEnumerable<DataGridViewCell> vals)
        {
            checkList.BeginUpdate();
            checkList.Nodes.Clear();

            if (vals != null)
            {
                // add select all node
                var allnode = TreeNodeItemSelector.CreateNode(_textStrings["NODESELECTALL"] + "            ", null, CheckState.Checked, TreeNodeItemSelector.CustomNodeType.SelectAll);
                allnode.NodeFont = new Font(checkList.Font, FontStyle.Bold);
                checkList.Nodes.Add(allnode);

                if (vals.Any())
                {
                    var nonulls = vals.Where(c => c.Value != null && c.Value != DBNull.Value);

                    // add select empty node
                    if (vals.Count() != nonulls.Count())
                    {
                        var nullnode = TreeNodeItemSelector.CreateNode(_textStrings["NODESELECTEMPTY"] + "               ", null, CheckState.Checked, TreeNodeItemSelector.CustomNodeType.SelectEmpty);
                        nullnode.NodeFont = new Font(checkList.Font, FontStyle.Bold);
                        checkList.Nodes.Add(nullnode);
                    }

                    // add datetime nodes
                    if (DataType == typeof(DateTime))
                    {
                        var years =
                            from year in nonulls
                            group year by ((DateTime)year.Value).Year into cy
                            orderby cy.Key
                            select cy;

                        foreach (var year in years)
                        {
                            var yearnode = TreeNodeItemSelector.CreateNode(year.Key.ToString(), year.Key, CheckState.Checked, TreeNodeItemSelector.CustomNodeType.DateTimeNode);
                            checkList.Nodes.Add(yearnode);

                            var months =
                                from month in year
                                group month by ((DateTime)month.Value).Month into cm
                                orderby cm.Key
                                select cm;

                            foreach (var month in months)
                            {
                                var monthnode = yearnode.CreateChildNode(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month.Key), month.Key);

                                var days =
                                    from day in month
                                    group day by ((DateTime)day.Value).Day into cd
                                    orderby cd.Key
                                    select cd;

                                foreach (var day in days)
                                {
                                    TreeNodeItemSelector daysnode;

                                    if (!IsFilterDateAndTimeEnabled)
                                        daysnode = monthnode.CreateChildNode(day.Key.ToString("D2"), day.First().Value);
                                    else
                                    {
                                        daysnode = monthnode.CreateChildNode(day.Key.ToString("D2"), day.Key);

                                        var hours =
                                            from hour in day
                                            group hour by ((DateTime)hour.Value).Hour into ch
                                            orderby ch.Key
                                            select ch;

                                        foreach (var hour in hours)
                                        {
                                            var hoursnode = daysnode.CreateChildNode(hour.Key.ToString("D2") + " " + "h", hour.Key);

                                            var mins =
                                                from min in hour
                                                group min by ((DateTime)min.Value).Minute into cmin
                                                orderby cmin.Key
                                                select cmin;

                                            foreach (var min in mins)
                                            {
                                                var minsnode = hoursnode.CreateChildNode(min.Key.ToString("D2") + " " + "m", min.Key);

                                                var secs =
                                                    from sec in min
                                                    group sec by ((DateTime)sec.Value).Second into cs
                                                    orderby cs.Key
                                                    select cs;

                                                foreach (var sec in secs)
                                                {
                                                    minsnode.CreateChildNode(sec.Key.ToString("D2") + " " + "s", sec.First().Value);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // add timespan nodes
                    else if (DataType == typeof(TimeSpan))
                    {
                        var days =
                            from day in nonulls
                            group day by ((TimeSpan)day.Value).Days into cd
                            orderby cd.Key
                            select cd;

                        foreach (var day in days)
                        {
                            var daysnode = TreeNodeItemSelector.CreateNode(day.Key.ToString("D2"), day.Key, CheckState.Checked, TreeNodeItemSelector.CustomNodeType.DateTimeNode);
                            checkList.Nodes.Add(daysnode);

                            var hours =
                                from hour in day
                                group hour by ((TimeSpan)hour.Value).Hours into ch
                                orderby ch.Key
                                select ch;

                            foreach (var hour in hours)
                            {
                                var hoursnode = daysnode.CreateChildNode(hour.Key.ToString("D2") + " " + "h", hour.Key);

                                var mins =
                                    from min in hour
                                    group min by ((TimeSpan)min.Value).Minutes into cmin
                                    orderby cmin.Key
                                    select cmin;

                                foreach (var min in mins)
                                {
                                    var minsnode = hoursnode.CreateChildNode(min.Key.ToString("D2") + " " + "m", min.Key);

                                    var secs =
                                        from sec in min
                                        group sec by ((TimeSpan)sec.Value).Seconds into cs
                                        orderby cs.Key
                                        select cs;

                                    foreach (var sec in secs)
                                    {
                                        minsnode.CreateChildNode(sec.Key.ToString("D2") + " " + "s", sec.First().Value);
                                    }
                                }
                            }
                        }
                    }

                    // add boolean nodes
                    else if (DataType == typeof(bool))
                    {
                        var values = nonulls.Where(c => (bool)c.Value);

                        if (values.Count() != nonulls.Count())
                        {
                            var node = TreeNodeItemSelector.CreateNode("False", false, CheckState.Checked, TreeNodeItemSelector.CustomNodeType.Default);
                            checkList.Nodes.Add(node);
                        }

                        if (values.Any())
                        {
                            var node = TreeNodeItemSelector.CreateNode("True", true, CheckState.Checked, TreeNodeItemSelector.CustomNodeType.Default);
                            checkList.Nodes.Add(node);
                        }
                    }

                    // ignore image nodes
                    else if (DataType == typeof(Bitmap))
                    { }

                    // add string nodes
                    else
                    {
                        foreach (var v in nonulls.GroupBy(c => c.Value).OrderBy(g => g.Key))
                        {
                            var node = TreeNodeItemSelector.CreateNode(v.First().FormattedValue.ToString(), v.Key, CheckState.Checked, TreeNodeItemSelector.CustomNodeType.Default);
                            checkList.Nodes.Add(node);
                        }
                    }
                }
            }

            checkList.EndUpdate();
        }

        /// <summary>
        /// Check
        /// </summary>
        /// <param name="node"></param>
        private void NodeCheckChange(TreeNodeItemSelector node)
        {
            node.CheckState = node.CheckState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked;

            if (node.NodeType == TreeNodeItemSelector.CustomNodeType.SelectAll)
            {
                SetNodesCheckState(checkList.Nodes, node.Checked);
                button_ok.Enabled = node.Checked;
            }
            else
            {
                if (node.Nodes.Count > 0)
                {
                    SetNodesCheckState(node.Nodes, node.Checked);
                }

                // refresh nodes
                var state = UpdateNodesCheckState(checkList.Nodes);

                GetSelectAllNode().CheckState = state;
                button_ok.Enabled = state != CheckState.Unchecked;
            }
        }

        /// <summary>
        /// Set Nodes CheckState
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="isChecked"></param>
        private void SetNodesCheckState(TreeNodeCollection nodes, bool isChecked)
        {
            foreach (TreeNodeItemSelector node in nodes)
            {
                node.Checked = isChecked;
                if (node.Nodes != null && node.Nodes.Count > 0)
                    SetNodesCheckState(node.Nodes, isChecked);
            }
        }

        /// <summary>
        /// Update Nodes CheckState recursively
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private CheckState UpdateNodesCheckState(TreeNodeCollection nodes)
        {
            var result = CheckState.Unchecked;
            var isFirstNode = true;
            var isAllNodesSomeCheckState = true;

            foreach (TreeNodeItemSelector n in nodes)
            {
                if (n.NodeType == TreeNodeItemSelector.CustomNodeType.SelectAll)
                    continue;

                if (n.Nodes.Count > 0)
                {
                    n.CheckState = UpdateNodesCheckState(n.Nodes);
                }

                if (isFirstNode)
                {
                    result = n.CheckState;
                    isFirstNode = false;
                }
                else
                    if (result != n.CheckState)
                    isAllNodesSomeCheckState = false;
            }

            if (isAllNodesSomeCheckState)
                return result;
            return CheckState.Indeterminate;
        }

        /// <summary>
        /// Get the SelectAll Node
        /// </summary>
        /// <returns></returns>
        private TreeNodeItemSelector GetSelectAllNode()
        {
            TreeNodeItemSelector result = null;
            var i = 0;
            foreach (TreeNodeItemSelector n in checkList.Nodes)
            {
                if (n.NodeType == TreeNodeItemSelector.CustomNodeType.SelectAll)
                {
                    result = n;
                    break;
                }

                if (i > 2)
                    break;
                i++;
            }

            return result;
        }

        /// <summary>
        /// Get the SelectEmpty Node
        /// </summary>
        /// <returns></returns>
        private TreeNodeItemSelector GetSelectEmptyNode()
        {
            TreeNodeItemSelector result = null;
            var i = 0;
            foreach (TreeNodeItemSelector n in checkList.Nodes)
            {
                if (n.NodeType == TreeNodeItemSelector.CustomNodeType.SelectEmpty)
                {
                    result = n;
                    break;
                }

                if (i > 2)
                    break;
                i++;
            }

            return result;
        }

        /// <summary>
        /// Duplicate Nodes
        /// </summary>
        private void DuplicateNodes()
        {
            _startingNodes = new TreeNodeItemSelector[checkList.Nodes.Count];
            var i = 0;
            foreach (TreeNodeItemSelector n in checkList.Nodes)
            {
                _startingNodes[i] = n.Clone();
                i++;
            }
        }

        /// <summary>
        /// Duplicate Filter on Nodes
        /// </summary>
        private void DuplicateFilterNodes()
        {
            _filterNodes = new TreeNodeItemSelector[checkList.Nodes.Count];
            var i = 0;
            foreach (TreeNodeItemSelector n in checkList.Nodes)
            {
                _filterNodes[i] = n.Clone();
                i++;
            }
        }

        /// <summary>
        /// Restore Nodes
        /// </summary>
        private void RestoreNodes()
        {
            checkList.Nodes.Clear();
            if (_startingNodes != null)
                checkList.Nodes.AddRange(_startingNodes);
        }

        /// <summary>
        /// Restore Filter on Nodes
        /// </summary>
        private void RestoreFilterNodes()
        {
            checkList.Nodes.Clear();
            if (_filterNodes != null)
                checkList.Nodes.AddRange(_filterNodes);
        }

        #endregion


        #region checklist filter events

        /// <summary>
        /// CheckList NodeMouseClick event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkList_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var HitTestInfo = checkList.HitTest(e.X, e.Y);
            if (HitTestInfo != null && HitTestInfo.Location == TreeViewHitTestLocations.StateImage)

                // check the node check status
                NodeCheckChange(e.Node as TreeNodeItemSelector);
        }

        /// <summary>
        /// CheckList KeyDown event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)

                // check the node check status
                NodeCheckChange(checkList.SelectedNode as TreeNodeItemSelector);
        }

        /// <summary>
        /// CheckList NodeMouseDoubleClick event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkList_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var n = e.Node as TreeNodeItemSelector;

            // set the new node check status
            SetNodesCheckState(checkList.Nodes, false);
            n.CheckState = CheckState.Unchecked;
            NodeCheckChange(n);

            // do Filter by checkList
            button_ok_Click(this, new EventArgs());
        }

        /// <summary>
        /// CheckList MouseEnter event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkList_MouseEnter(object sender, EventArgs e)
        {
            checkList.Focus();
        }

        /// <summary>
        /// CheckList MouseLeave envet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkList_MouseLeave(object sender, EventArgs e)
        {
            Focus();
        }

        /// <summary>
        /// Set the Filter by checkList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ok_Click(object sender, EventArgs e)
        {
            SetCheckListFilter();
            Close();
        }

        /// <summary>
        /// Undo changed by checkList 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_cancel_Click(object sender, EventArgs e)
        {
            RestoreNodes();
            Close();
        }

        #endregion


        #region filter methods

        /// <summary>
        /// UnCheck all Custom Filter presets
        /// </summary>
        private void UnCheckCustomFilters()
        {
            for (var i = 2; i < customFilterLastFiltersListMenuItem.DropDownItems.Count; i++)
            {
                (customFilterLastFiltersListMenuItem.DropDownItems[i] as ToolStripMenuItem).Checked = false;
            }
        }

        /// <summary>
        /// Set a Custom Filter
        /// </summary>
        /// <param name="filtersMenuItemIndex"></param>
        private void SetCustomFilter(int filtersMenuItemIndex)
        {
            if (_activeFilterType == FilterType.CheckList)
                SetNodesCheckState(checkList.Nodes, false);

            var filterstring = customFilterLastFiltersListMenuItem.DropDownItems[filtersMenuItemIndex].Tag.ToString();
            var viewfilterstring = customFilterLastFiltersListMenuItem.DropDownItems[filtersMenuItemIndex].Text;

            // do preset jobs
            if (filtersMenuItemIndex != 2)
            {
                for (var i = filtersMenuItemIndex; i > 2; i--)
                {
                    customFilterLastFiltersListMenuItem.DropDownItems[i].Text = customFilterLastFiltersListMenuItem.DropDownItems[i - 1].Text;
                    customFilterLastFiltersListMenuItem.DropDownItems[i].Tag = customFilterLastFiltersListMenuItem.DropDownItems[i - 1].Tag;
                }

                customFilterLastFiltersListMenuItem.DropDownItems[2].Text = viewfilterstring;
                customFilterLastFiltersListMenuItem.DropDownItems[2].Tag = filterstring;
            }

            // uncheck other preset
            for (var i = 3; i < customFilterLastFiltersListMenuItem.DropDownItems.Count; i++)
            {
                (customFilterLastFiltersListMenuItem.DropDownItems[i] as ToolStripMenuItem).Checked = false;
            }

            (customFilterLastFiltersListMenuItem.DropDownItems[2] as ToolStripMenuItem).Checked = true;
            _activeFilterType = FilterType.Custom;

            // get Filter string
            var oldfilter = FilterString;
            FilterString = filterstring;

            // set CheckList nodes
            SetNodesCheckState(checkList.Nodes, false);
            DuplicateFilterNodes();

            customFilterLastFiltersListMenuItem.Checked = true;
            button_ok.Enabled = false;

            // fire Filter changed
            if (oldfilter != FilterString)
                FilterChanged?.Invoke(this, new EventArgs());
        }

        #endregion


        #region filter events

        /// <summary>
        /// Cancel Filter Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelFilterMenuItem_Click(object sender, EventArgs e)
        {
            var oldfilter = FilterString;

            // clean Filter
            CleanFilter();

            // fire Filter changed
            if (oldfilter != FilterString)
                FilterChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Cancel Filter MouseEnter event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelFilterMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Enabled)
                (sender as ToolStripMenuItem).Select();
        }

        /// <summary>
        /// Custom Filter Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customFilterMenuItem_Click(object sender, EventArgs e)
        {
            // ignore image nodes
            if (DataType == typeof(Bitmap))
                return;

            // open a new Custom filter window
            var flt = new FormCustomFilter(DataType, IsFilterDateAndTimeEnabled);

            if (flt.ShowDialog() == DialogResult.OK)
            {
                // add the new Filter presets
                var filterString = flt.FilterString;
                var viewFilterString = flt.FilterStringDescription;

                var index = -1;

                for (var i = 2; i < customFilterLastFiltersListMenuItem.DropDownItems.Count; i++)
                {
                    if (customFilterLastFiltersListMenuItem.DropDown.Items[i].Available)
                    {
                        if (customFilterLastFiltersListMenuItem.DropDownItems[i].Text == viewFilterString && customFilterLastFiltersListMenuItem.DropDownItems[i].Tag.ToString() == filterString)
                        {
                            index = i;
                            break;
                        }
                    }
                    else
                        break;
                }

                if (index < 2)
                {
                    for (var i = customFilterLastFiltersListMenuItem.DropDownItems.Count - 2; i > 1; i--)
                    {
                        if (customFilterLastFiltersListMenuItem.DropDownItems[i].Available)
                        {
                            customFilterLastFiltersListMenuItem.DropDownItems[i + 1].Text = customFilterLastFiltersListMenuItem.DropDownItems[i].Text;
                            customFilterLastFiltersListMenuItem.DropDownItems[i + 1].Tag = customFilterLastFiltersListMenuItem.DropDownItems[i].Tag;
                        }
                    }

                    index = 2;

                    customFilterLastFiltersListMenuItem.DropDownItems[2].Text = viewFilterString;
                    customFilterLastFiltersListMenuItem.DropDownItems[2].Tag = filterString;
                }

                // set the Custom Filter
                SetCustomFilter(index);
            }
        }

        /// <summary>
        /// Custom Filter preset MouseEnter event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customFilterLastFiltersListMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Enabled)
                (sender as ToolStripMenuItem).Select();
        }

        /// <summary>
        /// Custom Filter preset MouseEnter event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customFilterLastFiltersListMenuItem_Paint(object sender, PaintEventArgs e)
        {
            var rect = new Rectangle(customFilterLastFiltersListMenuItem.Width - 12, 7, 10, 10);
            ControlPaint.DrawMenuGlyph(e.Graphics, rect, MenuGlyph.Arrow, Color.Black, Color.Transparent);
        }

        /// <summary>
        /// Custom Filter preset 1 Visibility changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customFilterLastFilter1MenuItem_VisibleChanged(object sender, EventArgs e)
        {
            toolStripSeparator2MenuItem.Visible = !customFilterLastFilter1MenuItem.Visible;
            (sender as ToolStripMenuItem).VisibleChanged -= customFilterLastFilter1MenuItem_VisibleChanged;
        }

        /// <summary>
        /// Custom Filter preset Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customFilterLastFilterMenuItem_Click(object sender, EventArgs e)
        {
            var menuitem = sender as ToolStripMenuItem;

            for (var i = 2; i < customFilterLastFiltersListMenuItem.DropDownItems.Count; i++)
            {
                if (customFilterLastFiltersListMenuItem.DropDownItems[i].Text == menuitem.Text && customFilterLastFiltersListMenuItem.DropDownItems[i].Tag.ToString() == menuitem.Tag.ToString())
                {
                    // set current filter preset as active
                    SetCustomFilter(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Custom Filter preset TextChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customFilterLastFilterMenuItem_TextChanged(object sender, EventArgs e)
        {
            (sender as ToolStripMenuItem).Available = true;
            (sender as ToolStripMenuItem).TextChanged -= customFilterLastFilterMenuItem_TextChanged;
        }

        /// <summary>
        /// Check list filter changer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkTextFilter_TextChanged(object sender, EventArgs e)
        {
            if (_checkTextFilterRemoveNodesOnSearch)
            {
                checkList.BeginUpdate();
                RestoreNodes();
            }

            for (var i = checkList.Nodes.Count - 1; i > 0; i--)
            {
                var node = checkList.Nodes[i] as TreeNodeItemSelector;
                node.Checked = !checkList.Nodes[i].Text.ToLower().Contains(checkTextFilter.Text.ToLower());
                NodeCheckChange(node);
            }

            if (_checkTextFilterRemoveNodesOnSearch)
            {
                checkList.EndUpdate();
            }

            for (var i = checkList.Nodes.Count - 1; i > 0; i--)
            {
                if (_checkTextFilterRemoveNodesOnSearch)
                {
                    if (!checkList.Nodes[i].Text.ToLower().Contains(checkTextFilter.Text.ToLower()))
                        checkList.Nodes[i].Remove();
                }
            }
        }

        #endregion


        #region sort events

        /// <summary>
        /// Sort ASC Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sortASCMenuItem_Click(object sender, EventArgs e)
        {
            // ignore image nodes
            if (DataType == typeof(Bitmap))
                return;

            sortASCMenuItem.Checked = true;
            sortDESCMenuItem.Checked = false;
            _activeSortType = SortType.ASC;

            // get Sort String
            var oldsort = SortString;
            SortString = "[{0}] ASC";

            // fire Sort Changed
            if (oldsort != SortString)
                SortChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Sort ASC MouseEnter event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sortASCMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Enabled)
                (sender as ToolStripMenuItem).Select();
        }

        /// <summary>
        /// Sort DESC Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sortDESCMenuItem_Click(object sender, EventArgs e)
        {
            // ignore image nodes
            if (DataType == typeof(Bitmap))
                return;

            sortASCMenuItem.Checked = false;
            sortDESCMenuItem.Checked = true;
            _activeSortType = SortType.DESC;

            // get Sort String
            var oldsort = SortString;
            SortString = "[{0}] DESC";

            // fire Sort Changed
            if (oldsort != SortString)
                SortChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Sort DESC MouseEnter event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sortDESCMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Enabled)
                (sender as ToolStripMenuItem).Select();
        }

        /// <summary>
        /// Cancel Sort Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelSortMenuItem_Click(object sender, EventArgs e)
        {
            var oldsort = SortString;

            // clean Sort
            CleanSort();

            // fire Sort changed
            if (oldsort != SortString) SortChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Cancel Sort MouseEnter event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelSortMenuItem_MouseEnter(object sender, EventArgs e)
        {
            if ((sender as ToolStripMenuItem).Enabled)
                (sender as ToolStripMenuItem).Select();
        }


        #endregion


        #region resize methods

        /// <summary>
        /// Resize the box
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        private void ResizeBox(int w, int h)
        {
            sortASCMenuItem.Width = w - 1;
            sortDESCMenuItem.Width = w - 1;
            cancelSortMenuItem.Width = w - 1;
            cancelFilterMenuItem.Width = w - 1;
            customFilterMenuItem.Width = w - 1;
            customFilterLastFiltersListMenuItem.Width = w - 1;
            checkFilterListControlHost.Size = new Size(w - 35, h - 160 - 25);
            checkFilterListPanel.Size = new Size(w - 35, h - 160 - 25);
            checkTextFilterControlHost.Size = new Size(w - 35, h - 160 - 160 - 25 - 8);
            checkList.Bounds = new Rectangle(4, 4, w - 35 - 8, h - 160 - 25 - 8);
            checkFilterListButtonsControlHost.Size = new Size(w - 35, 24);
            button_ok.Location = new Point(w - 35 - 164, 0);
            button_cancel.Location = new Point(w - 35 - 79, 0);
            resizeBoxControlHost.Margin = new Padding(w - 46, 0, 0, 0);
            Size = new Size(w, h);
        }

        /// <summary>
        /// Clean box for Resize
        /// </summary>
        private void ResizeClean()
        {
            if (_resizeEndPoint.X != -1)
            {
                var startPoint = PointToScreen(_resizeStartPoint);

                var rc =
                    new Rectangle(startPoint.X, startPoint.Y, _resizeEndPoint.X, _resizeEndPoint.Y)
                    {
                        X = Math.Min(startPoint.X, _resizeEndPoint.X),
                        Width = Math.Abs(startPoint.X - _resizeEndPoint.X),
                        Y = Math.Min(startPoint.Y, _resizeEndPoint.Y),
                        Height = Math.Abs(startPoint.Y - _resizeEndPoint.Y)
                    };



                ControlPaint.DrawReversibleFrame(rc, Color.Black, FrameStyle.Dashed);

                _resizeEndPoint.X = -1;
            }
        }

        #endregion


        #region resize events

        /// <summary>
        /// Resize MouseDown event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resizeBoxControlHost_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ResizeClean();
            }
        }

        /// <summary>
        /// Resize MouseMove event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resizeBoxControlHost_MouseMove(object sender, MouseEventArgs e)
        {
            if (Visible)
            {
                if (e.Button == MouseButtons.Left)
                {
                    var x = e.X;
                    var y = e.Y;

                    ResizeClean();

                    x += Width - resizeBoxControlHost.Width;
                    y += Height - resizeBoxControlHost.Height;

                    x = Math.Max(x, MinimumSize.Width - 1);
                    y = Math.Max(y, MinimumSize.Height - 1);

                    var StartPoint = PointToScreen(_resizeStartPoint);
                    var EndPoint = PointToScreen(new Point(x, y));

                    var rc = new Rectangle
                    {
                        X = Math.Min(StartPoint.X, EndPoint.X),
                        Width = Math.Abs(StartPoint.X - EndPoint.X),
                        Y = Math.Min(StartPoint.Y, EndPoint.Y),
                        Height = Math.Abs(StartPoint.Y - EndPoint.Y)
                    };



                    ControlPaint.DrawReversibleFrame(rc, Color.Black, FrameStyle.Dashed);

                    _resizeEndPoint.X = EndPoint.X;
                    _resizeEndPoint.Y = EndPoint.Y;
                }
            }
        }

        /// <summary>
        /// Resize MouseUp event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resizeBoxControlHost_MouseUp(object sender, MouseEventArgs e)
        {
            if (_resizeEndPoint.X == -1) return;
            ResizeClean();

            if (!Visible) return;
            if (e.Button != MouseButtons.Left) return;
            var newWidth = e.X + Width - resizeBoxControlHost.Width;
            var newHeight = e.Y + Height - resizeBoxControlHost.Height;

            newWidth = Math.Max(newWidth, MinimumSize.Width);
            newHeight = Math.Max(newHeight, MinimumSize.Height);

            ResizeBox(newWidth, newHeight);
        }

        /// <summary>
        /// Resize Paint event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resizeBoxControlHost_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Resources.MenuStrip_ResizeGrip, 0, 0);
        }

        #endregion

    }
}