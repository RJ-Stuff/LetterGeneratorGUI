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
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    [DesignerCategory("")]
    public partial class AdvancedDataGridViewSearchToolBar : ToolStrip
    {

        #region public events

        public event AdvancedDataGridViewSearchToolBarSearchEventHandler Search;

        #endregion


        #region class properties

        private DataGridViewColumnCollection _columnsList;

        private const bool ButtonCloseEnabled = false;
        private readonly Hashtable _textStrings = new Hashtable();

        #endregion


        #region constructor

        /// <summary>
        /// AdvancedDataGridViewSearchToolBar constructor
        /// </summary>
        public AdvancedDataGridViewSearchToolBar()
        {
            // set localization strings
            _textStrings.Add("LABELSEARCH", "Search:");
            _textStrings.Add("BUTTONFROMBEGINTOOLTIP", "From Begin");
            _textStrings.Add("BUTTONCASESENSITIVETOOLTIP", "Case Sensitivity");
            _textStrings.Add("BUTTONSEARCHTOOLTIP", "Find Next");
            _textStrings.Add("BUTTONCLOSETOOLTIP", "Hide");
            _textStrings.Add("BUTTONWHOLEWORDTOOLTIP", "Search only Whole Word");
            _textStrings.Add("COMBOBOXCOLUMNSALL", "(All Columns)");
            _textStrings.Add("TEXTBOXSEARCHTOOLTIP", "Value for Search");

            // initialize components
            InitializeComponent();

            comboBox_columns.Items.AddRange(new object[] { _textStrings["COMBOBOXCOLUMNSALL"].ToString() });
            button_close.ToolTipText = _textStrings["BUTTONCLOSETOOLTIP"].ToString();
            label_search.Text = _textStrings["LABELSEARCH"].ToString();
            textBox_search.ToolTipText = _textStrings["TEXTBOXSEARCHTOOLTIP"].ToString();
            button_frombegin.ToolTipText = _textStrings["BUTTONFROMBEGINTOOLTIP"].ToString();
            button_casesensitive.ToolTipText = _textStrings["BUTTONCASESENSITIVETOOLTIP"].ToString();
            button_search.ToolTipText = _textStrings["BUTTONSEARCHTOOLTIP"].ToString();
            button_wholeword.ToolTipText = _textStrings["BUTTONWHOLEWORDTOOLTIP"].ToString();

            // set default values
            if (!ButtonCloseEnabled)
                Items.RemoveAt(0);
            textBox_search.Text = textBox_search.ToolTipText;
            comboBox_columns.SelectedIndex = 0;
        }

        #endregion


        #region button events

        /// <summary>
        /// button Search Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void button_search_Click(object sender, EventArgs e)
        {
            if (textBox_search.TextLength > 0 && textBox_search.Text != textBox_search.ToolTipText && Search != null)
            {
                DataGridViewColumn c = null;
                if (comboBox_columns.SelectedIndex > 0 && _columnsList != null && _columnsList.GetColumnCount(DataGridViewElementStates.Visible) > 0)
                {
                    var cols = _columnsList.Cast<DataGridViewColumn>().Where(col => col.Visible).ToArray();

                    if (cols.Length == comboBox_columns.Items.Count - 1)
                    {
                        if (cols[comboBox_columns.SelectedIndex - 1].HeaderText == comboBox_columns.SelectedItem.ToString())
                            c = cols[comboBox_columns.SelectedIndex - 1];
                    }
                }

                var args = new AdvancedDataGridViewSearchToolBarSearchEventArgs(
                    textBox_search.Text,
                    c,
                    button_casesensitive.Checked,
                    button_wholeword.Checked,
                    button_frombegin.Checked
                );
                Search(this, args);
            }
        }

        /// <summary>
        /// button Close Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void button_close_Click(object sender, EventArgs e)
        {
            Hide();
        }

        #endregion


        #region textbox search events

        /// <summary>
        /// textBox Search TextChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textBox_search_TextChanged(object sender, EventArgs e)
        {
            button_search.Enabled = textBox_search.TextLength > 0 && textBox_search.Text != textBox_search.ToolTipText;
        }


        /// <summary>
        /// textBox Search Enter event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textBox_search_Enter(object sender, EventArgs e)
        {
            if (textBox_search.Text == textBox_search.ToolTipText && textBox_search.ForeColor == Color.LightGray)
                textBox_search.Text = string.Empty;
            else
                textBox_search.SelectAll();

            textBox_search.ForeColor = SystemColors.WindowText;
        }

        /// <summary>
        /// textBox Search Leave event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textBox_search_Leave(object sender, EventArgs e)
        {
            if (textBox_search.Text.Trim() == string.Empty)
            {
                textBox_search.Text = textBox_search.ToolTipText;
                textBox_search.ForeColor = Color.LightGray;
            }
        }


        /// <summary>
        /// textBox Search KeyDown event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textBox_search_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBox_search.TextLength > 0 && textBox_search.Text != textBox_search.ToolTipText && e.KeyData == Keys.Enter)
            {
                button_search_Click(button_search, new EventArgs());
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        #endregion


        #region public methods

        /// <summary>
        /// Set Columns to search in
        /// </summary>
        /// <param name="columns"></param>
        public void SetColumns(DataGridViewColumnCollection columns)
        {
            _columnsList = columns;
            comboBox_columns.BeginUpdate();
            comboBox_columns.Items.Clear();
            new ComponentResourceManager(typeof(AdvancedDataGridViewSearchToolBar));
            comboBox_columns.Items.AddRange(new object[] { "(All columns)" });
            if (_columnsList != null)
                foreach (DataGridViewColumn c in _columnsList)
                    if (c.Visible)
                        comboBox_columns.Items.Add(c.HeaderText);
            comboBox_columns.SelectedIndex = 0;
            comboBox_columns.EndUpdate();
        }

        #endregion


        #region resize events

        /// <summary>
        /// Resize event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeMe(object sender, EventArgs e)
        {
            SuspendLayout();
            var w1 = 150;
            var w2 = 150;
            var oldW = comboBox_columns.Width + textBox_search.Width;
            foreach (ToolStripItem c in Items)
            {
                c.Overflow = ToolStripItemOverflow.Never;
                c.Visible = true;
            }

            var width = PreferredSize.Width - oldW + w1 + w2;
            if (Width < width)
            {
                label_search.Visible = false;
                GetResizeBoxSize(PreferredSize.Width - oldW + w1 + w2, ref w1, ref w2);
                width = PreferredSize.Width - oldW + w1 + w2;

                if (Width < width)
                {
                    button_casesensitive.Overflow = ToolStripItemOverflow.Always;
                    GetResizeBoxSize(PreferredSize.Width - oldW + w1 + w2, ref w1, ref w2);
                    width = PreferredSize.Width - oldW + w1 + w2;
                }

                if (Width < width)
                {
                    button_wholeword.Overflow = ToolStripItemOverflow.Always;
                    GetResizeBoxSize(PreferredSize.Width - oldW + w1 + w2, ref w1, ref w2);
                    width = PreferredSize.Width - oldW + w1 + w2;
                }

                if (Width < width)
                {
                    button_frombegin.Overflow = ToolStripItemOverflow.Always;
                    separator_search.Visible = false;
                    GetResizeBoxSize(PreferredSize.Width - oldW + w1 + w2, ref w1, ref w2);
                    width = PreferredSize.Width - oldW + w1 + w2;
                }

                if (Width < width)
                {
                    comboBox_columns.Overflow = ToolStripItemOverflow.Always;
                    textBox_search.Overflow = ToolStripItemOverflow.Always;
                    w1 = 150;
                    w2 = Math.Max(Width - PreferredSize.Width - textBox_search.Margin.Left - textBox_search.Margin.Right, 75);
                    textBox_search.Overflow = ToolStripItemOverflow.Never;
                    width = PreferredSize.Width - textBox_search.Width + w2;
                }

                if (Width < width)
                {
                    button_search.Overflow = ToolStripItemOverflow.Always;
                    w2 = Math.Max(Width - PreferredSize.Width + textBox_search.Width, 75);
                    width = PreferredSize.Width - textBox_search.Width + w2;
                }

                if (Width < width)
                {
                    button_close.Overflow = ToolStripItemOverflow.Always;
                    textBox_search.Margin = new Padding(8, 2, 8, 2);
                    w2 = Math.Max(Width - PreferredSize.Width + textBox_search.Width, 75);
                    width = PreferredSize.Width - textBox_search.Width + w2;
                }

                if (Width < width)
                {
                    w2 = Math.Max(Width - PreferredSize.Width + textBox_search.Width, 20);
                    width = PreferredSize.Width - textBox_search.Width + w2;
                }

                if (width > Width)
                {
                    textBox_search.Overflow = ToolStripItemOverflow.Always;
                    textBox_search.Margin = new Padding(0, 2, 8, 2);
                    w2 = 150;
                }
            }
            else
            {
                GetResizeBoxSize(width, ref w1, ref w2);
            }

            if (comboBox_columns.Width != w1)
                comboBox_columns.Width = w1;
            if (textBox_search.Width != w2)
                textBox_search.Width = w2;

            ResumeLayout();
        }



        /// <summary>
        /// Get a Resize Size for a box
        /// </summary>
        /// <param name="width"></param>
        /// <param name="w1"></param>
        /// <param name="w2"></param>
        private void GetResizeBoxSize(int width, ref int w1, ref int w2)
        {
            var dif = (int)Math.Round((width - Width) / 2.0, 0, MidpointRounding.AwayFromZero);

            var oldW1 = w1;
            if (Width < width)
            {
                w1 = Math.Max(w1 - dif, 75);
                w2 = Math.Max(w2 - dif, 75);
            }
            else
            {
                w1 = Math.Min(w1 - dif, 150);
                w2 += Width - width + oldW1 - w1;
            }
        }

        #endregion

    }

    public delegate void AdvancedDataGridViewSearchToolBarSearchEventHandler(object sender, AdvancedDataGridViewSearchToolBarSearchEventArgs e);
    public class AdvancedDataGridViewSearchToolBarSearchEventArgs : EventArgs
    {
        public string ValueToSearch { get; }
        public DataGridViewColumn ColumnToSearch { get; }
        public bool CaseSensitive { get; }
        public bool WholeWord { get; }
        public bool FromBegin { get; }

        public AdvancedDataGridViewSearchToolBarSearchEventArgs(string Value, DataGridViewColumn Column, bool Case, bool Whole, bool fromBegin)
        {
            ValueToSearch = Value;
            ColumnToSearch = Column;
            CaseSensitive = Case;
            WholeWord = Whole;
            FromBegin = fromBegin;
        }
    }
}
