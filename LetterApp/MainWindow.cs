namespace LetterApp
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    using LetterApp.model;

    public partial class MainWindow : Form
    {
      //  private DataTable _dataTable = null;
      //  private DataSet _dataSet = null;

        public MainWindow()
        {
            InitializeComponent();

            //initialize dataset
          //  _dataTable = new DataTable();
          //  _dataSet = new DataSet();

         //   var ds = DataHelper.SampleData;

            //initialize bindingsource
           // this.bsMain.DataSource = ds;

           // //initialize datagridview
           // this.dgClients.DataSource = this.bsMain;

           // //set bindingsource
           //// SetTestData();
           // this.bsMain.DataMember = ds.Tables[0].TableName;

            //  AddTestData();
        }

        //private void SetTestData()
        //{
        //    _dataTable = _dataSet.Tables.Add("Clientes");

        //    _dataTable = DataHelper.SampleData.Tables[0];

        //    //_dataTable = _dataSet.Tables.Add("TableTest");
        //    //_dataTable.Columns.Add("int", typeof(int));
        //    //_dataTable.Columns.Add("decimal", typeof(decimal));
        //    //_dataTable.Columns.Add("double", typeof(double));
        //    //_dataTable.Columns.Add("date", typeof(DateTime));
        //    //_dataTable.Columns.Add("datetime", typeof(DateTime));
        //    //_dataTable.Columns.Add("string", typeof(string));
        //    //_dataTable.Columns.Add("boolean", typeof(bool));
        //    //_dataTable.Columns.Add("guid", typeof(Guid));
        //    //_dataTable.Columns.Add("image", typeof(Bitmap));
        //    //_dataTable.Columns.Add("timespan", typeof(TimeSpan));

        //    this.bsMain.DataMember = _dataTable.TableName;
        //}


        private void FormMain_Load(object sender, EventArgs e)
        {

            //setup datagridview
            this.dgClients.DisableFilterAndSort(this.dgClients.Columns["int"]);
            dgClients.SetFilterDateAndTimeEnabled(dgClients.Columns["datetime"], true);
            dgClients.SetSortEnabled(dgClients.Columns["guid"], false);
            dgClients.SortDESC(dgClients.Columns["double"]);
        }

        private void advancedDataGridView_main_FilterStringChanged(object sender, EventArgs e)
        {
            this.bsMain.Filter = this.dgClients.FilterString;
        }

        private void advancedDataGridView_main_SortStringChanged(object sender, EventArgs e)
        {
            this.bsMain.Sort = this.dgClients.SortString;
        }



        private void button_savefilters_Click(object sender, EventArgs e)
        {
            //_filtersaved.Add((comboBox_filtersaved.Items.Count - 1) + 1, advancedDataGridView_main.FilterString);
            //comboBox_filtersaved.DataSource = new BindingSource(_filtersaved, null);
            //comboBox_filtersaved.SelectedIndex = comboBox_filtersaved.Items.Count - 1;
            //_sortsaved.Add((comboBox_sortsaved.Items.Count - 1) + 1, advancedDataGridView_main.SortString);
            //comboBox_sortsaved.DataSource = new BindingSource(_sortsaved, null);
            //comboBox_sortsaved.SelectedIndex = comboBox_sortsaved.Items.Count - 1;
        }

        private void button_setsavedfilter_Click(object sender, EventArgs e)
        {
            //if (comboBox_filtersaved.SelectedIndex != -1 && comboBox_sortsaved.SelectedIndex != -1)
            //    advancedDataGridView_main.LoadFilterAndSort(comboBox_filtersaved.SelectedValue.ToString(), comboBox_sortsaved.SelectedValue.ToString());
        }

        private void button_unloadfilters_Click(object sender, EventArgs e)
        {
            //advancedDataGridView_main.CleanFilterAndSort();
            //comboBox_filtersaved.SelectedIndex = -1;
            //comboBox_sortsaved.SelectedIndex = -1;
        }

        private void advancedDataGridViewSearchToolBar_main_Search(object sender, Zuby.ADGV.AdvancedDataGridViewSearchToolBarSearchEventArgs e)
        {
            //bool restartsearch = true;
            //int startColumn = 0;
            //int startRow = 0;
            //if (!e.FromBegin)
            //{
            //    bool endcol = advancedDataGridView_main.CurrentCell.ColumnIndex + 1 >= advancedDataGridView_main.ColumnCount;
            //    bool endrow = advancedDataGridView_main.CurrentCell.RowIndex + 1 >= advancedDataGridView_main.RowCount;

            //    if (endcol && endrow)
            //    {
            //        startColumn = advancedDataGridView_main.CurrentCell.ColumnIndex;
            //        startRow = advancedDataGridView_main.CurrentCell.RowIndex;
            //    }
            //    else
            //    {
            //        startColumn = endcol ? 0 : advancedDataGridView_main.CurrentCell.ColumnIndex + 1;
            //        startRow = advancedDataGridView_main.CurrentCell.RowIndex + (endcol ? 1 : 0);
            //    }
            //}
            //DataGridViewCell c = advancedDataGridView_main.FindCell(
            //    e.ValueToSearch,
            //    e.ColumnToSearch != null ? e.ColumnToSearch.Name : null,
            //    startRow,
            //    startColumn,
            //    e.WholeWord,
            //    e.CaseSensitive);
            //if (c == null && restartsearch)
            //    c = advancedDataGridView_main.FindCell(
            //        e.ValueToSearch,
            //        e.ColumnToSearch != null ? e.ColumnToSearch.Name : null,
            //        0,
            //        0,
            //        e.WholeWord,
            //        e.CaseSensitive);
            //if (c != null)
            //    advancedDataGridView_main.CurrentCell = c;
        }

    }
}
