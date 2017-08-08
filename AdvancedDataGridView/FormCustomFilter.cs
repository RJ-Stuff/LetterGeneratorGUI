﻿#region License
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
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    internal partial class FormCustomFilter : Form
    {

        #region class properties

        private enum FilterType
        {
            Unknown,
            DateTime,
            TimeSpan,
            String,
            Float,
            Integer
        }

        private readonly FilterType _filterType;
        private readonly Control _valControl1;
        private readonly Control _valControl2;

        private readonly bool _filterDateAndTimeEnabled;

        private string _filterString;
        private string _filterStringDescription;

        private readonly Hashtable _textStrings = new Hashtable();

        #endregion


        #region constructors

        /// <summary>
        /// Main constructor
        /// </summary>
        private FormCustomFilter()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Form constructor
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="filterDateAndTimeEnabled"></param>
        public FormCustomFilter(Type dataType, bool filterDateAndTimeEnabled)
            : this()
        {
            // set localization strings
            _textStrings.Add("EQUALS", "igual");
            _textStrings.Add("DOES_NOT_EQUAL", "distinto");
            _textStrings.Add("EARLIER_THAN", "antes de");
            _textStrings.Add("LATER_THAN", "después de");
            _textStrings.Add("BETWEEN", "entre");
            _textStrings.Add("GREATER_THAN", "mayor que");
            _textStrings.Add("GREATER_THAN_OR_EQUAL_TO", "mayor o igual que");
            _textStrings.Add("LESS_THAN", "menor que");
            _textStrings.Add("LESS_THAN_OR_EQUAL_TO", "menor o igual que");
            _textStrings.Add("BEGINS_WITH", "comienza con");
            _textStrings.Add("DOES_NOT_BEGIN_WITH", "no comienza con");
            _textStrings.Add("ENDS_WITH", "termina con");
            _textStrings.Add("DOES_NOT_END_WITH", "no termina con");
            _textStrings.Add("CONTAINS", "contiene");
            _textStrings.Add("DOES_NOT_CONTAIN", "no contiene");
            _textStrings.Add("INVALID_VALUE", "Valor incorrecto");
            _textStrings.Add("FILTER_STRING_DESCRIPTION", "Mostrar filas en donde valor {0} \"{1}\"");
            _textStrings.Add("FORM_TITLE", "Filtro personalizado");
            _textStrings.Add("LABEL_COLUMNNAMETEXT", "Mostrar filas donde valor");
            _textStrings.Add("LABEL_AND", "Y");
            _textStrings.Add("BUTTON_OK", "OK");
            _textStrings.Add("BUTTON_CANCEL", "Cancel");

            Text = _textStrings["FORM_TITLE"].ToString();
            label_columnName.Text = _textStrings["LABEL_COLUMNNAMETEXT"].ToString();
            label_and.Text = _textStrings["LABEL_AND"].ToString();
            button_ok.Text = _textStrings["BUTTON_OK"].ToString();
            button_cancel.Text = _textStrings["BUTTON_CANCEL"].ToString();

            if (dataType == typeof(DateTime)) _filterType = FilterType.DateTime;
            else if (dataType == typeof(TimeSpan)) _filterType = FilterType.TimeSpan;
            else if (dataType == typeof(Int32) || dataType == typeof(Int64) || dataType == typeof(Int16)
                     || dataType == typeof(UInt32) || dataType == typeof(UInt64) || dataType == typeof(UInt16)
                     || dataType == typeof(Byte) || dataType == typeof(SByte)) _filterType = FilterType.Integer;
            else if (dataType == typeof(Single) || dataType == typeof(Double) || dataType == typeof(Decimal))
                _filterType = FilterType.Float;
            else if (dataType == typeof(String)) _filterType = FilterType.String;
            else _filterType = FilterType.Unknown;

            _filterDateAndTimeEnabled = filterDateAndTimeEnabled;

            switch (_filterType)
            {
                case FilterType.DateTime:
                    _valControl1 = new DateTimePicker();
                    _valControl2 = new DateTimePicker();
                    if (_filterDateAndTimeEnabled)
                    {
                        var dt = Thread.CurrentThread.CurrentCulture.DateTimeFormat;

                        ((DateTimePicker)_valControl1).CustomFormat = dt.ShortDatePattern + " " + "HH:mm";
                        ((DateTimePicker)_valControl2).CustomFormat = dt.ShortDatePattern + " " + "HH:mm";
                        ((DateTimePicker)_valControl1).Format = DateTimePickerFormat.Custom;
                        ((DateTimePicker)_valControl2).Format = DateTimePickerFormat.Custom;
                    }
                    else
                    {
                        ((DateTimePicker)_valControl1).Format = DateTimePickerFormat.Short;
                        ((DateTimePicker)_valControl2).Format = DateTimePickerFormat.Short;
                    }

                    comboBox_filterType.Items.AddRange(
                        new object[]
                            {
                                _textStrings["EQUALS"].ToString(), _textStrings["DOES_NOT_EQUAL"].ToString(),
                                _textStrings["EARLIER_THAN"].ToString(), _textStrings["LATER_THAN"].ToString(),
                                _textStrings["BETWEEN"].ToString()
                            });
                    break;

                case FilterType.TimeSpan:
                    _valControl1 = new TextBox();
                    _valControl2 = new TextBox();
                    comboBox_filterType.Items.AddRange(
                        new object[] { _textStrings["CONTAINS"].ToString(), _textStrings["DOES_NOT_CONTAIN"].ToString() });
                    break;

                case FilterType.Integer:
                case FilterType.Float:
                    _valControl1 = new TextBox();
                    _valControl2 = new TextBox();
                    _valControl1.TextChanged += valControl_TextChanged;
                    _valControl2.TextChanged += valControl_TextChanged;
                    comboBox_filterType.Items.AddRange(
                        new object[]
                            {
                                _textStrings["EQUALS"].ToString(), _textStrings["DOES_NOT_EQUAL"].ToString(),
                                _textStrings["GREATER_THAN"].ToString(),
                                _textStrings["GREATER_THAN_OR_EQUAL_TO"].ToString(),
                                _textStrings["LESS_THAN"].ToString(),
                                _textStrings["LESS_THAN_OR_EQUAL_TO"].ToString(), _textStrings["BETWEEN"].ToString()
                            });
                    _valControl1.Tag = true;
                    _valControl2.Tag = true;
                    button_ok.Enabled = false;
                    break;

                default:
                    _valControl1 = new TextBox();
                    _valControl2 = new TextBox();
                    comboBox_filterType.Items.AddRange(
                        new object[]
                            {
                                _textStrings["EQUALS"].ToString(), _textStrings["DOES_NOT_EQUAL"].ToString(),
                                _textStrings["BEGINS_WITH"].ToString(),
                                _textStrings["DOES_NOT_BEGIN_WITH"].ToString(), _textStrings["ENDS_WITH"].ToString(),
                                _textStrings["DOES_NOT_END_WITH"].ToString(), _textStrings["CONTAINS"].ToString(),
                                _textStrings["DOES_NOT_CONTAIN"].ToString()
                            });
                    break;
            }
            comboBox_filterType.SelectedIndex = 0;

            _valControl1.Name = "valControl1";
            _valControl1.Location = new Point(30, 66);
            _valControl1.Size = new Size(166, 20);
            _valControl1.TabIndex = 4;
            _valControl1.Visible = true;
            _valControl1.KeyDown += valControl_KeyDown;

            _valControl2.Name = "valControl2";
            _valControl2.Location = new Point(30, 108);
            _valControl2.Size = new Size(166, 20);
            _valControl2.TabIndex = 5;
            _valControl2.Visible = false;
            _valControl2.VisibleChanged += valControl2_VisibleChanged;
            _valControl2.KeyDown += valControl_KeyDown;

            Controls.Add(_valControl1);
            Controls.Add(_valControl2);

            errorProvider.SetIconAlignment(_valControl1, ErrorIconAlignment.MiddleRight);
            errorProvider.SetIconPadding(_valControl1, -18);
            errorProvider.SetIconAlignment(_valControl2, ErrorIconAlignment.MiddleRight);
            errorProvider.SetIconPadding(_valControl2, -18);
        }

        /// <summary>
        /// Form loaders
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormCustomFilter_Load(object sender, EventArgs e)
        { }

        #endregion


        #region public filter methods

        /// <summary>
        /// Get the Filter string
        /// </summary>
        public string FilterString => _filterString;

        /// <summary>
        /// Get the Filter string description
        /// </summary>
        public string FilterStringDescription => _filterStringDescription;

        #endregion


        #region filter builder

        /// <summary>
        /// Build a Filter string
        /// </summary>
        /// <param name="filterType"></param>
        /// <param name="filterDateAndTimeEnabled"></param>
        /// <param name="filterTypeConditionText"></param>
        /// <param name="control1"></param>
        /// <param name="control2"></param>
        /// <returns></returns>
        private string BuildCustomFilter(FilterType filterType, bool filterDateAndTimeEnabled, string filterTypeConditionText, Control control1, Control control2)
        {
            var column = "[{0}] ";

            if (filterType == FilterType.Unknown)
                column = "Convert([{0}], 'System.String') ";

            var filterString = column;

            switch (filterType)
            {
                case FilterType.DateTime:
                    var dt = ((DateTimePicker)control1).Value;
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);

                    if (filterTypeConditionText == _textStrings["EQUALS"].ToString())
                    {
                        filterString = "Convert([{0}], 'System.String') LIKE '%" + Convert.ToString(filterDateAndTimeEnabled ? dt : dt.Date, CultureInfo.CurrentCulture) + "%'";
                    }
                    else if (filterTypeConditionText == _textStrings["EARLIER_THAN"].ToString())
                    {
                        filterString += "< '" + Convert.ToString(filterDateAndTimeEnabled ? dt : dt.Date, CultureInfo.CurrentCulture) + "'";
                    }
                    else if (filterTypeConditionText == _textStrings["LATER_THAN"].ToString())
                    {
                        filterString += "> '" + Convert.ToString(filterDateAndTimeEnabled ? dt : dt.Date, CultureInfo.CurrentCulture) + "'";
                    }
                    else if (filterTypeConditionText == _textStrings["BETWEEN"].ToString())
                    {
                        var dt1 = ((DateTimePicker)control2).Value;
                        dt1 = new DateTime(dt1.Year, dt1.Month, dt1.Day, dt1.Hour, dt1.Minute, 0);
                        filterString += ">= '" + Convert.ToString(filterDateAndTimeEnabled ? dt : dt.Date, CultureInfo.CurrentCulture) + "'";
                        filterString += " AND " + column + "<= '" + Convert.ToString(filterDateAndTimeEnabled ? dt1 : dt1.Date, CultureInfo.CurrentCulture) + "'";
                    }
                    else if (filterTypeConditionText == _textStrings["DOES_NOT_EQUAL"].ToString())
                    {
                        filterString = "Convert([{0}], 'System.String') NOT LIKE '%" + Convert.ToString(filterDateAndTimeEnabled ? dt : dt.Date, CultureInfo.CurrentCulture) + "%'";
                    }

                    break;

                case FilterType.TimeSpan:
                    try
                    {
                        var ts = TimeSpan.Parse(control1.Text);

                        if (filterTypeConditionText == _textStrings["CONTAINS"].ToString())
                        {
                            filterString = "(Convert([{0}], 'System.String') LIKE '%P" + (ts.Days > 0 ? ts.Days + "D" : string.Empty) + (ts.TotalHours > 0 ? "T" : string.Empty) + (ts.Hours > 0 ? ts.Hours + "H" : string.Empty) + (ts.Minutes > 0 ? ts.Minutes + "M" : string.Empty) + (ts.Seconds > 0 ? ts.Seconds + "S" : string.Empty) + "%')";
                        }
                        else if (filterTypeConditionText == _textStrings["DOES_NOT_CONTAIN"].ToString())
                        {
                            filterString = "(Convert([{0}], 'System.String') NOT LIKE '%P" + (ts.Days > 0 ? ts.Days + "D" : string.Empty) + (ts.TotalHours > 0 ? "T" : string.Empty) + (ts.Hours > 0 ? ts.Hours + "H" : string.Empty) + (ts.Minutes > 0 ? ts.Minutes + "M" : string.Empty) + (ts.Seconds > 0 ? ts.Seconds + "S" : string.Empty) + "%')";
                        }
                    }
                    catch (FormatException)
                    {
                        filterString = null;
                    }

                    break;

                case FilterType.Integer:
                case FilterType.Float:

                    var num = control1.Text;

                    if (filterType == FilterType.Float)
                        num = num.Replace(",", ".");

                    if (filterTypeConditionText == _textStrings["EQUALS"].ToString())
                        filterString += "= " + num;
                    else if (filterTypeConditionText == _textStrings["BETWEEN"].ToString())
                        filterString += ">= " + num + " AND " + column + "<= " + (filterType == FilterType.Float ? control2.Text.Replace(",", ".") : control2.Text);
                    else if (filterTypeConditionText == _textStrings["DOES_NOT_EQUAL"].ToString())
                        filterString += "<> " + num;
                    else if (filterTypeConditionText == _textStrings["GREATER_THAN"].ToString())
                        filterString += "> " + num;
                    else if (filterTypeConditionText == _textStrings["GREATER_THAN_OR_EQUAL_TO"].ToString())
                        filterString += ">= " + num;
                    else if (filterTypeConditionText == _textStrings["LESS_THAN"].ToString())
                        filterString += "< " + num;
                    else if (filterTypeConditionText == _textStrings["LESS_THAN_OR_EQUAL_TO"].ToString())
                        filterString += "<= " + num;
                    break;
                case FilterType.Unknown:
                    break;
                case FilterType.String:
                    break;
                default:
                    var txt = FormatFilterString(control1.Text);
                    if (filterTypeConditionText == _textStrings["EQUALS"].ToString())
                        filterString += "LIKE '" + txt + "'";
                    else if (filterTypeConditionText == _textStrings["DOES_NOT_EQUAL"].ToString())
                        filterString += "NOT LIKE '" + txt + "'";
                    else if (filterTypeConditionText == _textStrings["BEGINS_WITH"].ToString())
                        filterString += "LIKE '" + txt + "%'";
                    else if (filterTypeConditionText == _textStrings["ENDS_WITH"].ToString())
                        filterString += "LIKE '%" + txt + "'";
                    else if (filterTypeConditionText == _textStrings["DOES_NOT_BEGIN_WITH"].ToString())
                        filterString += "NOT LIKE '" + txt + "%'";
                    else if (filterTypeConditionText == _textStrings["DOES_NOT_END_WITH"].ToString())
                        filterString += "NOT LIKE '%" + txt + "'";
                    else if (filterTypeConditionText == _textStrings["CONTAINS"].ToString())
                        filterString += "LIKE '%" + txt + "%'";
                    else if (filterTypeConditionText == _textStrings["DOES_NOT_CONTAIN"].ToString())
                        filterString += "NOT LIKE '%" + txt + "%'";
                    break;
            }

            return filterString;
        }

        /// <summary>
        /// Format a text Filter string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string FormatFilterString(string text)
        {
            var result = string.Empty;
            string[] replace = { "%", "[", "]", "*", "\"", "\\" };

            foreach (var t in text)
            {
                var s = t.ToString();
                if (replace.Contains(s))
                    result += "[" + s + "]";
                else
                    result += s;
            }

            return result.Replace("'", "''");
        }


        #endregion


        #region buttons events

        /// <summary>
        /// Button Cancel Clieck
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_cancel_Click(object sender, EventArgs e)
        {
            _filterStringDescription = null;
            _filterString = null;
            Close();
        }

        /// <summary>
        /// Button OK Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ok_Click(object sender, EventArgs e)
        {
            if ((_valControl1.Visible && _valControl1.Tag != null && ((bool)_valControl1.Tag)) ||
                (_valControl2.Visible && _valControl2.Tag != null && ((bool)_valControl2.Tag)))
            {
                button_ok.Enabled = false;
                return;
            }

            var filterString = BuildCustomFilter(_filterType, _filterDateAndTimeEnabled, comboBox_filterType.Text, _valControl1, _valControl2);

            if (!string.IsNullOrEmpty(filterString))
            {
                _filterString = filterString;
                _filterStringDescription = string.Format(_textStrings["FILTER_STRING_DESCRIPTION"].ToString(), comboBox_filterType.Text, _valControl1.Text);
                if (_valControl2.Visible)
                    _filterStringDescription += " " + label_and.Text + " \"" + _valControl2.Text + "\"";
                DialogResult = DialogResult.OK;
            }
            else
            {
                _filterString = null;
                _filterStringDescription = null;
                DialogResult = DialogResult.Cancel;
            }

            Close();
        }

        #endregion


        #region changed status events

        /// <summary>
        /// Changed condition type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_filterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _valControl2.Visible = comboBox_filterType.Text == _textStrings["BETWEEN"].ToString();
            button_ok.Enabled = !(_valControl1.Visible && _valControl1.Tag != null && ((bool)_valControl1.Tag)) ||
                (_valControl2.Visible && _valControl2.Tag != null && ((bool)_valControl2.Tag));
        }

        /// <summary>
        /// Changed control2 visibility
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void valControl2_VisibleChanged(object sender, EventArgs e)
        {
            label_and.Visible = _valControl2.Visible;
        }

        /// <summary>
        /// Changed a control Text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void valControl_TextChanged(object sender, EventArgs e)
        {
            var hasErrors = false;
            switch (_filterType)
            {
                case FilterType.Integer:
                    long val;
                    hasErrors = !long.TryParse((sender as TextBox).Text, out val);
                    break;

                case FilterType.Float:
                    double val1;
                    hasErrors = !double.TryParse((sender as TextBox).Text, out val1);
                    break;
            }

            (sender as Control).Tag = hasErrors || (sender as TextBox).Text.Length == 0;

            if (hasErrors && (sender as TextBox).Text.Length > 0)
                errorProvider.SetError(sender as Control, _textStrings["INVALID_VALUE"].ToString());
            else
                errorProvider.SetError(sender as Control, string.Empty);

            button_ok.Enabled = !(_valControl1.Visible && _valControl1.Tag != null && ((bool)_valControl1.Tag)) ||
                (_valControl2.Visible && _valControl2.Tag != null && ((bool)_valControl2.Tag));
        }

        /// <summary>
        /// KeyDown on a control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void valControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (sender == _valControl1)
                {
                    if (_valControl2.Visible)
                        _valControl2.Focus();
                    else
                        button_ok_Click(button_ok, new EventArgs());
                }
                else
                {
                    button_ok_Click(button_ok, new EventArgs());
                }

                e.SuppressKeyPress = false;
                e.Handled = true;
            }
        }

        #endregion

    }
}