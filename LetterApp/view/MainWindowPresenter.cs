﻿namespace LetterApp.view
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using LetterApp.model;

    using LetterCore.Letters;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using CheckBox = System.Windows.Forms.CheckBox;
    using Format = model.Format;
    using Point = System.Drawing.Point;
    using Utils = model.Utils;
    using LetterCore.latex;

    public class MainWindowPresenter
    {
        private readonly MainWindow mainWindow;
        private readonly LettersGenerationDialog progressDialog;
        private readonly LoadingDataDialog loadingDataDialog;
        private readonly JToken guiConfiguration;
        private readonly Dictionary<CheckBox, Tuple<TextBox, string, string, string>> filterPair;

        private Configuration configuration;
        private bool notSavedChanges;
        private int maxProgress;
        private int totalProgress;

        public MainWindowPresenter(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            maxProgress = 0;
            totalProgress = 0;
            progressDialog = new LettersGenerationDialog();
            loadingDataDialog = new LoadingDataDialog();
            notSavedChanges = false;
            filterPair = new Dictionary<CheckBox, Tuple<TextBox, string, string, string>>();

            guiConfiguration = JToken.Parse(File.ReadAllText(
                Path.Combine(Directory.GetCurrentDirectory(), "GUIConfiguration.json"),
                Encoding.UTF8));

            mainWindow.cbPaperSize.DropDownStyle = ComboBoxStyle.DropDownList;
            mainWindow.cbCharge.DropDownStyle = ComboBoxStyle.DropDownList;

            (guiConfiguration["papersizes"] ?? new JArray())
            .Select(GetPaperSize)
            .ToList()
            .ForEach(o => this.mainWindow.cbPaperSize.Items.Add(o));

            UpdateFilterTab();
            LoadConfiguration();
            CreateEvents();
        }

        private static PaperSize GetPaperSize(JToken p)
        {
            return new PaperSize(
                p["displayname"].Value<string>(),
                (p["charges"] ?? new JArray()).Select(GetCharge).ToList());
        }

        private static model.Charge GetCharge(JToken t)
        {
            return new model.Charge(t["ChargeClazz"].Value<string>(), t["DisplayName"].Value<string>());
        }

        private static void AboutHelp(object sender, EventArgs e)
        {
            var year = DateTime.Now.ToString("yyyy");
            MessageBox.Show(
                "Aplicación para la generación de cartas.\n"
                            + "Versión 0.1.\n\nAutor: Rigoberto L. Salgado Reyes.\n"
                            + $"Correo: rigoberto.salgado@rjabogados.com.\n\n© {year} RJAbogados.\n"
                            + "Todos los derechos reservados.",
                "Acerca de",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private static void MainWindowOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            var option = MessageBox.Show(
                "¿Está seguro que desea salir?",
                "Cerrar aplicación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (option == DialogResult.No) cancelEventArgs.Cancel = true;
        }

        private static bool HasBindingSource(Format f)
        {
            return f.BindingSource != null;
        }

        private static CheckBox GetCheckBox(Tuple<JToken, int> tuple, Action<object, EventArgs> filterAction)
        {
            var ckb = new CheckBox()
            {
                Text = tuple.Item1["kind"].Value<string>(),
                Location = new Point(5, (tuple.Item2 + 1) * 23),
                Width = 210
            };
            ckb.Click += new EventHandler(filterAction);

            return ckb;
        }

        private static TextBox GetTextBox(Tuple<JToken, int> t)
        {
            var txb = new TextBox()
            {
                Location = new Point(215, (t.Item2 + 1) * 23),
                Width = 150,
                Height = 23
            };

            return txb;
        }

        private static Configuration GetConfiguration()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "configuration.json");
            return !File.Exists(path)
                       ? new Configuration()
                       : JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(path, Encoding.Default));
        }

        private static Client GetClientFromGroup(List<DataRowView> group)
        {
            var client = ViewUtils.GetClient(group[0]);
            client.DisaggregatedDebts = group.Select(ViewUtils.GetDebt).ToList();

            return client;
        }

        private static List<Client> GetClientsFromBindingSource(Format f)
        {
            return ViewUtils
                .GroupBy(f.BindingSource.List.Cast<DataRowView>(), "Dirección_Base_Carta")
                .Select(GetClientFromGroup)
                .ToList();
        }

        private static LetterCore.Letters.Format GetFormat(model.Format f)
        {
            return new LetterCore.Letters.Format(
                f.Url,
                GetClientsFromBindingSource(f),
                f.Charge.ChargeClazz);
        }

        private static void ExtendedOptionHelp(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Mediante esta opción es posible\n" + "agregar filtros especializados o no\n"
                + "contemplados en las restantes opciones.\n\n" +
                "Este filtro se usa directamente en la consulta a la base de datos.",
                "Información");
        }

        private static void MailHelp(object sender, EventArgs e)
        {
            MessageBox.Show("Las credenciales son las mismas que se \n" +
                            "utilizan para abrir el correo de la empresa.\n\n" +
                            "Ejemplo:\nUsuario: titu.cusi.huallpa@rjabogados.com\n" +
                            "Contraseña: tu-contraseña-personal", "Información");
        }

        private void CreateEvents()
        {
            mainWindow.btGenerateWords.Click += GenerateLetterEvent;
            mainWindow.btAddFormat.Click += AddFormat;
            mainWindow.btRemoveFormat.Click += RemoveFormat;
            mainWindow.btSaveEditorChanges.Click += SaveEditorChanges;
            mainWindow.ckbLineWrap.Click += LineWrap;
            mainWindow.rtEditor.KeyPress += TextOnEditorChange;
            mainWindow.btRemoveMail.Click += RemoveMail;
            mainWindow.btAddMail.Click += AddMail;
            mainWindow.ckbEditEditor.Click += EditEditor;
            mainWindow.ckLbFormats.SelectedIndexChanged += (s, e) => RefreshGui();
            mainWindow.cbPaperSize.SelectedIndexChanged += SelectedPaperSizeChange;
            mainWindow.ckLbFormats.ItemCheck += FormatCheck;
            mainWindow.btMailHelp.Click += MailHelp;
            mainWindow.cbCharge.SelectedIndexChanged += SelectedChargeChange;
            mainWindow.acercaDeToolStripMenuItem.Click += AboutHelp;
            mainWindow.btLoadData.Click += LoadData;
            mainWindow.dgClients.FilterStringChanged += FilterStringChanged;
            mainWindow.dgClients.SortStringChanged += SortStringChanged;
            mainWindow.bwCreateLetters.DoWork += BwCreateLettersOnDoWork;
            mainWindow.bwCreateLetters.RunWorkerCompleted += BwCreateLettersOnRunWorkerCompleted;
            mainWindow.bwCreateLetters.ProgressChanged += BwCreateLettersOnProgressChanged;
            mainWindow.bwGetData.DoWork += BwGetDataOnDoWork;
            mainWindow.bwGetData.RunWorkerCompleted += BwGetDataOnRunWorkerCompleted;
            mainWindow.bwGetData.ProgressChanged += BwGetDataOnProgressChanged;
            progressDialog.Closing += ProgressDialogOnClosing;
            loadingDataDialog.Closing += LoadingDataDialogOnClosing;
            mainWindow.Closing += MainWindowOnClosing;
            mainWindow.cerrarToolStripMenuItem.Click += CerrarToolStripMenuItemOnClick;
            mainWindow.btRemoveFilter.Click += RemoveFilter;
            mainWindow.btAddOption.Click += AddOption;
            mainWindow.btExtendedOptionHelp.Click += ExtendedOptionHelp;
            mainWindow.rbOnlyMail.Click += CheckCredentials;
            mainWindow.rbMailWithAtt.Click += CheckCredentials;
        }

        private void LoadingDataDialogOnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            loadingDataDialog.Visible = false;
            mainWindow.bwGetData.CancelAsync();
        }

        private void BwGetDataOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var format = (Format)e.UserState;

            mainWindow.bsMain = format.BindingSource;
            mainWindow.dgClients.DataSource = format.BindingSource;
            mainWindow.dgClients.CleanFilterAndSort();
            mainWindow.lClientCount.Text = ViewUtils
                .GroupBy(format.BindingSource.List.Cast<DataRowView>(), "Dirección_Base_Carta")
                .Count()
                .ToString();
        }

        private void BwGetDataOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                var msg = $"Ocurrió un error en la obtención de datos.\n\nDescripción:\n{e.Error.Message}";
                MessageBox.Show(msg, "Error");
            }
            else if (e.Cancelled)
            {
                MessageBox.Show("La obtención de datos fue cancelada.", "Advertencia");
            }
            else
            {
                MessageBox.Show("Datos obtenidos correctamente.", "Información");
            }

            mainWindow.btLoadData.Enabled = true;
            loadingDataDialog.Visible = false;
        }

        private void BwGetDataOnDoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            var tuple = (Tuple<object, string>)e.Argument;
            var format = (Format)tuple.Item1;
            var filters = tuple.Item2;
            var conn = guiConfiguration["connectionstring"].Value<string>();
            var token = guiConfiguration["queries"]
                .Where(t => t["format"].Value<string>().Equals(Path.GetFileNameWithoutExtension(format.Url)))
                .Single();
            var countQuery = token["countquery"].Value<string>();
            var dataQuery = token["dataquery"].Value<string>();
            var count = DataHelper.GetCount(conn, countQuery, filters);

            if (count == 0)
            {
                MessageBox.Show(
                    "La consulta no arroja ningún resultado,\n" +
                    "si está seguro que existen datos,\nconsulte al equipo de sistemas.",
                    "Advertencia",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                e.Cancel = true;
                worker.CancelAsync();
                return;
            }

            if (count > Convert.ToInt32(mainWindow.nudLetterCount.Value) + 10)
            {
                var msg = "Por favor, revise los filtros,\n"
                          + "la cantidad de elementos arrojados\n" +
                          "en la consulta excede la cantidad esperada.\n\n" +
                          "¿Desea cargar de todas formas los datos?";

                var option = MessageBox.Show(msg, "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (option == DialogResult.No)
                {
                    e.Cancel = true;
                    worker.CancelAsync();
                    return;
                }
            }

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                worker.CancelAsync();
                return;
            }

            format.BindingSource = new BindingSource
            {
                DataSource = DataHelper.GetData(conn, dataQuery, filters),
                DataMember = "Clientes"
            };
            format.BindingSource.ListChanged += DataSourceChange;

            worker.ReportProgress(0, format);
        }

        private void CheckCredentials(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb.Checked && (string.IsNullOrEmpty(mainWindow.txtbUser.Text.Trim())
                               || string.IsNullOrEmpty(mainWindow.txtbPass.Text.Trim())
                               || mainWindow.lbMails.Items.Count == 0))
            {
                MessageBox.Show("Por favor, compruebe que tenga un correo al cual notificar\ny que completó las credenciales correctamente.", "Información");
                rb.Checked = false;
                mainWindow.rbNoNotification.Checked = true;
            }
        }

        private void UpdateFilterTab()
        {
            var filters = guiConfiguration["filters"] ?? new JArray();

            filters
                .Select((f, i) => new Tuple<JToken, int>(f, i))
                .ToList()
                .ForEach(SetFilterPairValue);

            ManageCheckGroupBox(mainWindow.ckbExtendedOptions, mainWindow.gbExtendedOptions);
        }

        private void SetFilterPairValue(Tuple<JToken, int> t)
        {
            var ckb = GetCheckBox(t, FilterAction);

            mainWindow.gbFilters.Controls.Add(ckb);

            TextBox txb = null;

            if (Convert.ToBoolean(t.Item1["box"].Value<string>()))
            {
                txb = GetTextBox(t);
                mainWindow.gbFilters.Controls.Add(txb);
            }

            filterPair[ckb] = Tuple.Create(
                txb,
                t.Item1["internalname"].Value<string>(),
                t.Item1["validate"].Value<string>(),
                t.Item1["hint"].Value<string>());
        }

        private void FilterAction(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;

            if (filterPair.TryGetValue(chk, out Tuple<TextBox, string, string, string> tuple) && Utils.Validate(tuple))
            {
                var functions = new Dictionary<bool, Action<object>>
                {
                    [true] = f => mainWindow.lbFilters.Items.Add(f),
                    [false] = mainWindow.lbFilters.Items.Remove
                };

                functions[chk.Checked].Invoke(new Filter(chk.Text, tuple.Item2, (tuple.Item1 ?? new TextBox()).Text));

                if (tuple.Item1 != null)
                {
                    tuple.Item1.Enabled = !chk.Checked;
                    tuple.Item1.Text = chk.Checked ? tuple.Item1.Text : string.Empty;
                }

                FiltersChange();
            }
            else
            {
                var alt = string.IsNullOrEmpty(tuple.Item1.Text.Trim()) ?
                    "Comience agregando algún valor al mismo." : tuple.Item4;
                MessageBox.Show($"Hay problemas con la validación del valor del filtro.\n{alt}", "Validación");
                chk.Checked = false;
                tuple.Item1.Text = string.Empty;
            }
        }

        private void ManageCheckGroupBox(CheckBox chk, GroupBox grp)
        {
            if (chk.Parent == grp)
            {
                grp.Parent.Controls.Add(chk);

                chk.Location = new Point(
                    chk.Left + grp.Left,
                    chk.Top + grp.Top);

                chk.BringToFront();
            }

            chk.Click += ExtendedOptions;
        }

        private void ExtendedOptions(object sender, EventArgs e)
        {
            mainWindow.gbExtendedOptions.Enabled = mainWindow.ckbExtendedOptions.Checked;
        }

        private void CerrarToolStripMenuItemOnClick(object sender, EventArgs eventArgs)
        {
            var option = MessageBox.Show(
                "¿Está seguro que desea salir?",
                "Cerrar aplicación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (option == DialogResult.Yes) mainWindow.Dispose();
        }

        private void ProgressDialogOnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            progressDialog.Visible = false;
            mainWindow.bwCreateLetters.CancelAsync();
        }

        private void BwCreateLettersOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var pi = (ProgressIncrement)e.UserState;

            progressDialog.lProgressInfo.Text = pi.Information;

            progressDialog.pbPart.Maximum = pi.Count;
            progressDialog.pbPart.Minimum = 0;
            progressDialog.pbPart.Value = pi.Progress;

            progressDialog.pbTotal.Maximum = maxProgress;
            progressDialog.pbTotal.Minimum = 0;
            progressDialog.pbTotal.Value = ++totalProgress;
        }

        private void BwCreateLettersOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                var msg = $"Ocurrió un error en la creación de cartas.\n\nDescripción:\n{e.Error.Message}";
                MessageBox.Show(msg, "Error");
            }
            else if (e.Cancelled)
            {
                MessageBox.Show("La creación de cartas fue cancelada.", "Advertencia");
            }
            else
            {
                MessageBox.Show("Cartas generadas correctamente.", "Información");
            }

            mainWindow.btGenerateWords.Enabled = true;
            ClearProgressDialog();
            progressDialog.Visible = false;
        }

        private void ClearProgressDialog()
        {
            progressDialog.lProgressInfo.Text = string.Empty;
            progressDialog.pbPart.Value = 0;
            progressDialog.pbPart.Maximum = 0;
            progressDialog.pbPart.Minimum = 0;
            progressDialog.pbTotal.Value = 0;
            progressDialog.pbTotal.Maximum = 0;
            progressDialog.pbTotal.Minimum = 0;
            maxProgress = 0;
            totalProgress = 0;
        }

        private void BwCreateLettersOnDoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            var tuple = (Tuple<object, object>)e.Argument;
            var paperSize = (model.PaperSize)tuple.Item1;
            var charge = (model.Charge)tuple.Item2;
            GenerateLetters(charge.ChargeClazz, worker, e);
        }

        private void GenerateLetters(string chargeClazz, BackgroundWorker worker, DoWorkEventArgs e)
        {
            var formats = mainWindow.ckLbFormats.CheckedItems.Cast<Format>()
                .Where(HasBindingSource)
                .Select(GetFormat)
                .ToList();

            maxProgress = formats.Select(l => l.Clients.Count).Sum();

            var docName = LatexController.LatexGenerator(
                formats,
                "a4paper",
                chargeClazz,
                worker,
                e,
                guiConfiguration["limit"].Value<int>());

            MailNotification(docName);
        }

        private void MailNotification(string docName)
        {
            if (!mainWindow.rbNoNotification.Checked)
            {
                ViewUtils.SendNotification(
                    mainWindow.lbMails.Items.Cast<string>().ToList(),
                    mainWindow.txtbUser.Text,
                    mainWindow.txtbPass.Text,
                    mainWindow.rbMailWithAtt.Checked ? docName : string.Empty);
            }
        }

        private void SortStringChanged(object sender, EventArgs e)
        {
            mainWindow.bsMain.Sort = mainWindow.dgClients.SortString;
        }

        private void FilterStringChanged(object sender, EventArgs e)
        {
            mainWindow.bsMain.Filter = mainWindow.dgClients.FilterString;
        }

        private void DataSourceChange(object sender, ListChangedEventArgs e)
        {
            var index = mainWindow.ckLbFormats.SelectedIndex;
            if (index == -1) return;
            var format = (Format)mainWindow.ckLbFormats.Items[index];
            mainWindow.lClientCount.Text = ViewUtils
                .GroupBy(format.BindingSource.List.Cast<DataRowView>(), "Dirección_Base_Carta")
                .Count()
                .ToString();
        }

        private void LoadData(object sender, EventArgs e)
        {
            try
            {
                var index = mainWindow.ckLbFormats.SelectedIndex;
                if (index == -1) return;

                var filters = string.Join(
                    " ",
                    Enumerable.Range(0, mainWindow.lbFilters.Items.Count)
                        .Select(i => ((Filter)mainWindow.lbFilters.Items[i]).InternalToString()));

                mainWindow.btLoadData.Enabled = false;
                mainWindow.bwGetData.RunWorkerAsync(Tuple.Create(mainWindow.ckLbFormats.Items[index], filters));
                loadingDataDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                var msg = $"Ocurrió un problema al cargar los datos\n\nDescripción:\n{ex.Message}";
                MessageBox.Show(msg, "Error");
            }
        }

        private void SelectedChargeChange(object sender, EventArgs e)
        {
            var charge = mainWindow.cbCharge.SelectedItem as model.Charge ?? model.Charge.DefaultCharge;

            mainWindow.ckLbFormats
                .Items
                .Cast<Format>()
                .ToList()
                .ForEach(f => f.Charge = charge);

            configuration.SetFormats(mainWindow.ckLbFormats.Items.Cast<Format>().ToList());
        }

        private void RefreshGui()
        {
            mainWindow.ckbEditEditor.Enabled = mainWindow.ckLbFormats.Items.Count != 0;

            try
            {
                mainWindow.lbFilters.Items.Clear();

                filterPair
                    .Keys
                    .ToList()
                    .ForEach(k => UpdateFilter(k, false, string.Empty));

                mainWindow.ckbExtendedOptions.Checked = false;
                mainWindow.gbExtendedOptions.Enabled = false;
                mainWindow.txtbExtendedOption.Text = string.Empty;
                mainWindow.rtEditor.Text = string.Empty;
                mainWindow.ckbEditEditor.Checked = false;

                EditEditor(null, null);

                var index = mainWindow.ckLbFormats.SelectedIndex;
                if (index == -1) return;

                var format = (Format)mainWindow.ckLbFormats.Items[index];

                Encoding currentEncoding;
                if (format == null) return;

                //refractorizar esto
                format.Filters.ForEach(UpdateFilter);

                using (var reader = new StreamReader(format.Url, true))
                {
                    currentEncoding = reader.CurrentEncoding;
                }

                mainWindow.rtEditor.Text = File.ReadAllText(format.Url, currentEncoding);
                mainWindow.cbPaperSize.SelectedItem = format.PaperSize;
                mainWindow.cbCharge.Items.Clear();
                format.PaperSize.Charges.ForEach(c => mainWindow.cbCharge.Items.Add(c));
                mainWindow.cbCharge.SelectedItem = format.Charge;

                mainWindow.bsMain = format.BindingSource;
                mainWindow.dgClients.DataSource = format.BindingSource;

                if (!string.IsNullOrEmpty(format.BindingSource?.Filter) ||
                    !string.IsNullOrEmpty(format.BindingSource?.Sort))
                {
                    mainWindow.dgClients.LoadFilterAndSort(format.BindingSource.Filter, format.BindingSource.Sort);
                }
                else
                {
                    if (format.BindingSource != null)
                        mainWindow.dgClients.CleanFilterAndSort();
                }

                mainWindow.lClientCount.Text = ViewUtils.GroupBy(
                    format.BindingSource?.List.Cast<DataRowView>() ?? new List<DataRowView>(),
                    "Dirección_Base_Carta").Count().ToString();
            }
            catch (Exception)
            {
                MessageBox.Show("Ocurrió un error al actualizar la aplicación.", "Error");
            }
        }

        private void UpdateFilter(Filter f)
        {
            mainWindow.lbFilters.Items.Add(f);
            var keys = filterPair.Keys.Where(k => k.Text == f.DisplayName).ToList();
            if (keys.Count == 0) return;
            var key = keys[0];

            UpdateFilter(key, true, f.Value.ToString());
        }

        private void UpdateFilter(CheckBox k, bool active, string value)
        {
            k.Checked = active;
            if (filterPair[k].Item1 == null) return;
            filterPair[k].Item1.Text = value;
            filterPair[k].Item1.Enabled = !active;
        }

        private void FiltersChange()
        {
            var index = mainWindow.ckLbFormats.SelectedIndex;
            if (index == -1) return;
            var format = (Format)mainWindow.ckLbFormats.Items[index];
            format.Filters = mainWindow.lbFilters.Items.Cast<Filter>().ToList();
            configuration.SetFormats(mainWindow.ckLbFormats.Items.Cast<Format>().ToList());
        }

        private void AddOption(object sender, EventArgs e)
        {
            if (mainWindow.txtbExtendedOption.Text.Trim().Length == 0)
            {
                MessageBox.Show("Debe introducir algún valor como opción.", "Validación");
            }
            else
            {
                var option = mainWindow.txtbExtendedOption.Text.Trim();
                mainWindow.lbFilters.Items.Add(new Filter(option, option, string.Empty));
                mainWindow.txtbExtendedOption.Text = string.Empty;
                FiltersChange();
            }
        }

        private void RemoveFilter(object sender, EventArgs e)
        {
            if (mainWindow.lbFilters.Items.Count == 0)
            {
                MessageBox.Show("No existen filtros.", "Información");
            }
            else
            {
                var index = mainWindow.lbFilters.SelectedIndex;
                if (index == -1)
                {
                    MessageBox.Show("Debe seleccionar el filtro a eliminar.", "Información");
                }
                else
                {
                    var confirmResult = MessageBox.Show(
                        "¿Está seguro que desea eliminar el filtro?",
                        "Confirmar eliminación",
                        MessageBoxButtons.YesNo);

                    if (confirmResult != DialogResult.Yes) return;
                    var filter = mainWindow.lbFilters.Items[index] as Filter;

                    var chkl = filterPair.Keys.Select(k => k).Where(k => k.Text == filter.DisplayName).ToList();

                    if (chkl.Count != 0)
                    {
                        chkl[0].Checked = false;
                        FilterAction(chkl[0], null);
                    }
                    else
                    {
                        mainWindow.lbFilters.Items.RemoveAt(index);
                    }

                    FiltersChange();
                }
            }
        }

        private void FormatCheck(object sender, ItemCheckEventArgs e)
        {
            var index = e.Index;
            if (index == -1)
            {
                return;
            }

            if (mainWindow.ckLbFormats.Items[index] is Format format)
            {
                format.Checked = e.NewValue == CheckState.Checked;
            }

            configuration.SetFormats(mainWindow.ckLbFormats.Items.Cast<Format>().ToList());
        }

        private void SetFormatOnListBox(Tuple<Format, int> f)
        {
            mainWindow.ckLbFormats.Items.Add(f.Item1);
            mainWindow.ckLbFormats.SetItemCheckState(f.Item2, f.Item1.Checked ? CheckState.Checked : CheckState.Unchecked);
        }

        private void LoadConfiguration()
        {
            try
            {
                configuration = GetConfiguration();

                configuration.Formats
                    .Select((f, i) => Tuple.Create(f, i))
                    .ToList()
                    .ForEach(SetFormatOnListBox);

                configuration
                    .Notifications
                    .ForEach(n => mainWindow.lbMails.Items.Add(n));

                LoadDefaultFormats();

                if (mainWindow.ckLbFormats.Items.Count != 0)
                {
                    mainWindow.ckLbFormats.SetSelected(0, true);
                }

                RefreshGui();
            }
            catch (Exception)
            {
                MessageBox.Show("Ocurrió un problema al cargar la configuración.", "Error");
            }
        }

        private void LoadDefaultFormats()
        {
            if (mainWindow.ckLbFormats.Items.Count != 0) return;

            LoadFormatByPaperSize(PaperSize.DefaultSize);
        }

        private void SelectedPaperSizeChange(object sender, EventArgs e)
        {
            var paperSize = (mainWindow.cbPaperSize.SelectedItem as PaperSize) ?? PaperSize.DefaultSize;

            mainWindow.cbCharge.Items.Clear();
            paperSize.Charges.ForEach(c => mainWindow.cbCharge.Items.Add(c));

            if (mainWindow.cbCharge.Items.Count != 0)
            {
                mainWindow.cbCharge.SelectedItem = mainWindow.cbCharge.Items[0];
            }

            var charge = (mainWindow.cbCharge.SelectedItem as model.Charge) ?? model.Charge.DefaultCharge;

            LoadFormatByPaperSize(paperSize);

            mainWindow.ckLbFormats
                .Items
                .Cast<Format>()
                .ToList()
                .ForEach(f =>
                {
                    f.PaperSize = paperSize;
                    f.Charge = charge;
                });

            configuration.SetFormats(mainWindow.ckLbFormats.Items.Cast<Format>().ToList());
        }

        private void LoadFormatByPaperSize(PaperSize paperSize)
        {
            mainWindow.ckLbFormats.Items.Clear();

            var dir = Path.Combine(Directory.GetCurrentDirectory(), "latex");

            var stream = Directory
                .EnumerateFiles(dir, "*.tex")
                .Where(f => !Path.GetFileNameWithoutExtension(f).Equals("letters"))
                .Select((url, i) => new { format = new Format(url), index = i })
                .ToList();

            configuration.SetFormats(stream.Select(o => o.format).ToList());

            stream.ForEach(o =>
            {
                mainWindow.ckLbFormats.Items.Add(o.format);
                mainWindow.ckLbFormats.SetItemCheckState(
                    o.index,
                    o.format.Checked ? CheckState.Checked : CheckState.Unchecked);
            });
        }

        private void EditEditor(object sender, EventArgs e)
        {
            mainWindow.rtEditor.ReadOnly = !mainWindow.ckbEditEditor.Checked;
            mainWindow.btSaveEditorChanges.Enabled = mainWindow.ckbEditEditor.Checked;
            mainWindow.ckbLineWrap.Enabled = mainWindow.ckbEditEditor.Checked;
        }

        private void AddMail(object sender, EventArgs e)
        {
            const string Pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            var r = new Regex(Pattern);

            if (r.Match(mainWindow.txtbEmail.Text.Trim()).Success)
            {
                mainWindow.lbMails.Items.Add(mainWindow.txtbEmail.Text.Trim());
                mainWindow.txtbEmail.Text = string.Empty;
                configuration.SetNotifications(mainWindow.lbMails.Items.Cast<string>().ToList());
            }
            else
            {
                MessageBox.Show(
                    "Direccion de correo incorrecta.\n\n" + "Ejemplo de direcciones correctas:\n"
                    + "titu.cusi.huallpa@rjabogados.com\n" +
                    "joseholguin@hotmail.com", "Información");
                mainWindow.txtbEmail.Text = string.Empty;
            }
        }

        private void RemoveMail(object sender, EventArgs e)
        {
            if (mainWindow.lbMails.Items.Count == 0)
            {
                MessageBox.Show("No existen correos.", "Información");
            }
            else
            {
                var index = mainWindow.lbMails.SelectedIndex;
                if (index == -1)
                {
                    MessageBox.Show("Debe seleccionar el correo a eliminar.", "Información");
                }
                else
                {
                    var option = MessageBox.Show("¿Está seguro que desea eliminar el correo?", "Confirmación", MessageBoxButtons.YesNo);
                    if (option != DialogResult.Yes)
                    {
                        return;
                    }

                    mainWindow.lbMails.Items.RemoveAt(index);
                    configuration.SetNotifications(mainWindow.lbMails.Items.Cast<string>().ToList());
                }
            }
        }

        private void TextOnEditorChange(object sender, EventArgs e)
        {
            notSavedChanges = true;
        }

        private void LineWrap(object sender, EventArgs e)
        {
            mainWindow.rtEditor.WordWrap = !mainWindow.ckbLineWrap.Checked;
        }

        private void SaveEditorChanges(object sender, EventArgs e)
        {
            if (!notSavedChanges)
            {
                MessageBox.Show("No existen cambios por guardar.", "Información");
            }
            else
            {
                if (mainWindow.rtEditor.Text.Trim().Length != 0)
                {
                    var confirmResult = MessageBox.Show(
                        "¿Está seguro que desea guardar el formato?",
                        "Confirmación",
                        MessageBoxButtons.YesNo);

                    if (confirmResult == DialogResult.Yes)
                    {
                        var index = mainWindow.ckLbFormats.SelectedIndex;
                        if (index != -1)
                        {
                            try
                            {
                                if (mainWindow.ckLbFormats.Items[index] is Format format)
                                {
                                    Encoding currentEncoding;
                                    using (var reader = new StreamReader(format.Url, true))
                                    {
                                        currentEncoding = reader.CurrentEncoding;
                                    }

                                    File.WriteAllText(format.Url, mainWindow.rtEditor.Text, currentEncoding);
                                }

                                RefreshGui();

                                MessageBox.Show("Cambios guardados correctamente.", "Información");
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Ocurrió un error al guardar el formato.", "Error");
                            }
                        }
                    }
                }

                notSavedChanges = false;
            }
        }

        private void RemoveFormat(object sender, EventArgs e)
        {
            if (mainWindow.ckLbFormats.Items.Count == 0)
            {
                MessageBox.Show(mainWindow, "No existen formatos.", "Eliminar formato");
            }
            else
            {
                var index = mainWindow.ckLbFormats.SelectedIndex;
                if (index == -1)
                {
                    MessageBox.Show(mainWindow, "Debe seleccionar un formato a eliminar", "Eliminar formato");
                }
                else
                {
                    var confirmResult = MessageBox.Show(
                        "¿Está seguro que desea eliminar el formato?",
                        "Confirmar eliminación",
                        MessageBoxButtons.YesNo);

                    if (confirmResult != DialogResult.Yes)
                    {
                        return;
                    }

                    mainWindow.ckLbFormats.Items.RemoveAt(index);
                    configuration.SetFormats(mainWindow.ckLbFormats.Items.Cast<Format>().ToList());

                    if (mainWindow.ckLbFormats.Items.Count != 0)
                    {
                        mainWindow.ckLbFormats.SetSelected(0, true);
                    }

                    RefreshGui();
                }
            }
        }

        private void AddFormat(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),
                Filter = "Formatos RJ (*.rjf)|*.rjf|Todos los archivos (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var format = new Format(openFileDialog.FileName);
                configuration.AddFormat(format);
                mainWindow.ckLbFormats.Items.Add(format);
                mainWindow.ckLbFormats.SetSelected(mainWindow.ckLbFormats.Items.Count - 1, true);
                mainWindow.ckLbFormats.SetItemChecked(mainWindow.ckLbFormats.Items.Count - 1, true);
            }

            RefreshGui();
        }

        private void GenerateLetterEvent(object sender, EventArgs e)
        {
            try
            {
                if (!Enumerable.Range(0, mainWindow.ckLbFormats.Items.Count)
                        .Select(i => ((Format)mainWindow.ckLbFormats.Items[i]).BindingSource)
                        .Any(bs => bs != null))
                {
                    MessageBox.Show("Debe cargar datos en algún formato para generar cartas.", "Información");
                    return;
                }

                mainWindow.btGenerateWords.Enabled = false;
                mainWindow.bwCreateLetters.RunWorkerAsync(Tuple.Create(mainWindow.cbPaperSize.SelectedItem, mainWindow.cbCharge.SelectedItem));

                progressDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                var msg = $"Ocurrió un problema en la generación de cartas\n\nDescripción:\n{ex.Message}";
                MessageBox.Show(msg, "Error");
            }
        }
    }
}
