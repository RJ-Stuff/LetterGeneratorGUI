namespace LetterApp.view
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Reactive.Subjects;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using LetterApp.model;

    using LetterCore.Letters;

    using Microsoft.Office.Interop.Word;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using static System.String;

    using Format = LetterApp.model.Format;

    public class MainWindowPresenter
    {
        private readonly MainWindow mainWindow;

        private Configuration configuration;
        private bool notSavedChanges;

        private readonly LettersGenerationDialog progressDialog;

        private int maxProgress;

        private int totalProgress;

        public MainWindowPresenter(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            maxProgress = 0;
            totalProgress = 0;

            progressDialog = new LettersGenerationDialog();

            notSavedChanges = false;

            var guiConfiguration = JToken.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "GUIConfiguration.json"), Encoding.Default));

            mainWindow.cbPaperSize.DropDownStyle = ComboBoxStyle.DropDownList;
            mainWindow.cbCharge.DropDownStyle = ComboBoxStyle.DropDownList;

            (guiConfiguration["papersizes"] ?? new JArray())
            .Select(p =>
            {
                var chargesTokens = p["charges"] ?? new JArray();
                var charges = chargesTokens
                .Select(t => new model.Charge(t["ChargeClazz"].Value<string>(), t["DisplayName"].Value<string>()))
                .ToList();

                return new PaperSize(p["name"].Value<string>(), p["displayname"].Value<string>(), charges);
            })
            .ToList()
            .ForEach(o => this.mainWindow.cbPaperSize.Items.Add(o));

            LoadConfiguration();

            CreateEvents();
        }

        private static void AboutHelp(object sender, EventArgs e)
        {
            var year = DateTime.Now.ToString("yyyy");
            MessageBox.Show($"Aplicación para la generación de cartas.\nVersión 0.1.\n\nAutor: Rigoberto L. Salgado Reyes.\nCorreo: rigoberto.salgado@rjabogados.com.\n\n© {year} RJAbogados.\nTodos los derechos reservados.", "Acerca de", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void ChargesHelp(object sender, EventArgs e)
        {
            MessageBox.Show("El cargo está en relación con el tamaño de la hoja.", "Información");
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
            mainWindow.btChargesHelp.Click += ChargesHelp;
            mainWindow.acercaDeToolStripMenuItem.Click += AboutHelp;
            mainWindow.btLoadData.Click += LoadData;
            mainWindow.dgClients.FilterStringChanged += FilterStringChanged;
            mainWindow.dgClients.SortStringChanged += SortStringChanged;
            mainWindow.bwCreateLetters.DoWork += BwCreateLettersOnDoWork;
            mainWindow.bwCreateLetters.RunWorkerCompleted += BwCreateLettersOnRunWorkerCompleted;
            mainWindow.bwCreateLetters.ProgressChanged += BwCreateLettersOnProgressChanged;
            progressDialog.Closing += ProgressDialogOnClosing;
            mainWindow.Closing += MainWindowOnClosing;
            mainWindow.cerrarToolStripMenuItem.Click += CerrarToolStripMenuItemOnClick;
        }

        private void CerrarToolStripMenuItemOnClick(object sender, EventArgs eventArgs)
        {
            var option = MessageBox.Show(
                "¿Está seguro de que desea salir?",
                "Cerrar aplicación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (option == DialogResult.Yes) mainWindow.Dispose();
        }

        private void MainWindowOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            var option = MessageBox.Show(
                "¿Está seguro de que desea salir?", 
                "Cerrar aplicación", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);

            if (option == DialogResult.No) cancelEventArgs.Cancel = true;
        }
        
        private void ProgressDialogOnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            progressDialog.Visible = false;
            mainWindow.bwCreateLetters.CancelAsync();

            mainWindow.btGenerateWords.Enabled = true;
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
            progressDialog.lProgressInfo.Text = Empty;
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
            GenerateLetters(((PaperSize)e.Argument).Papersize, worker, e);
        }

        private void GenerateLetters(WdPaperSize paperSize, BackgroundWorker worker, DoWorkEventArgs e)
        {
            var formats = mainWindow.ckLbFormats.CheckedItems.Cast<Format>()
                .Where(f => f.BindingSource != null)
                .Select(f =>
                    {
                        var groups = ViewUtils.GroupBy(f.BindingSource.List.Cast<DataRowView>(), "codluna");
                        var clients = groups.Select(g =>
                            {
                                var client = ViewUtils.GetClient(g[0]);
                                var debts = g.Select(ViewUtils.GetDebt).ToList();
                                client.DisaggregatedDebts = debts;

                                return client;
                            }).ToList();

                        return new LetterCore.Letters.Format(f.Url, clients, f.Charge.ChargeClazz);
                    }).ToList();

            maxProgress = formats.Select(l => l.Clients.Count).Sum();

            var docName = AllInOneGenerator.CreateDocs(
                formats,
                paperSize,
                worker,
                e);

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
                .GroupBy(format.BindingSource.List.Cast<DataRowView>(), "codluna")
                .Count()
                .ToString();
        }

        private void LoadData(object sender, EventArgs e)
        {
            var index = mainWindow.ckLbFormats.SelectedIndex;
            if (index == -1) return;

            // todo
            // var query = (mainWindow.ckLbFormats.Items[index] as Format).
            // cargar el query....
            var format = (Format)mainWindow.ckLbFormats.Items[index];

            format.BindingSource = new BindingSource
            {
                DataSource = DataHelper.SampleData,
                DataMember = "Clientes"
            };
            format.BindingSource.ListChanged += DataSourceChange;

            mainWindow.bsMain = format.BindingSource;
            mainWindow.dgClients.DataSource = format.BindingSource;
            mainWindow.dgClients.CleanFilterAndSort();
            mainWindow.lClientCount.Text = ViewUtils
                .GroupBy(format.BindingSource.List.Cast<DataRowView>(), "codluna")
                .Count()
                .ToString();
        }

        private void SelectedChargeChange(object sender, EventArgs e)
        {
            var charge = (mainWindow.cbCharge.SelectedItem as model.Charge) ?? model.Charge.DefaultCharge;

            mainWindow.ckLbFormats
                .Items
                .Cast<Format>()
                .ToList()
                .ForEach(f => f.Charge = charge);

            configuration.SetFormats(mainWindow.ckLbFormats.Items.Cast<Format>().ToList());
        }

        private void MailHelp(object sender, EventArgs e)
        {
            MessageBox.Show("Las credenciales son las mismas que se \n" +
                "utilizan para abrir el correo de la empresa.\n\n" +
                "Ejemplo:\nUsuario: titu.cusi.huallpa@rjabogados.com\n" +
                "Contraseña: tu-contraseña-personal", "Información");
        }

        private void RefreshGui()
        {
            mainWindow.ckbEditEditor.Enabled = mainWindow.ckLbFormats.Items.Count != 0;

            try
            {
                mainWindow.rtEditor.Text = Empty;

                mainWindow.ckbEditEditor.Checked = false;
                EditEditor(null, null);

                var index = mainWindow.ckLbFormats.SelectedIndex;
                if (index == -1)
                {
                    return;
                }

                var format = mainWindow.ckLbFormats.Items[index] as Format;

                Encoding currentEncoding;
                if (format == null)
                {
                    return;
                }

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

                if (!IsNullOrEmpty(format.BindingSource?.Filter) || !IsNullOrEmpty(format.BindingSource?.Sort))
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
                    "codluna").Count().ToString();
            }
            catch (Exception)
            {
                MessageBox.Show("Ocurrió un error al actualizar la aplicación.", "Error");
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

        private void LoadConfiguration()
        {
            try
            {
                configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "configuration.json"), Encoding.Default));

                configuration.Formats
                    .Select((f, i) => new { format = f, index = i })
                    .ToList()
                    .ForEach(p =>
                    {
                        mainWindow.ckLbFormats.Items.Add(p.format);
                        mainWindow.ckLbFormats.SetItemCheckState(p.index, p.format.Checked ? CheckState.Checked : CheckState.Unchecked);
                    });

                configuration.Notifications.ForEach(n => mainWindow.lbMails.Items.Add(n));

                if (mainWindow.ckLbFormats.Items.Count == 0)
                {
                    var dir = Path.Combine(Directory.GetCurrentDirectory(), "formats");
                    if (Directory.Exists(dir))
                    {
                        var stream = Directory
                            .EnumerateFiles(dir, "*.rjf")
                            .Select((url, i) => new { format = new Format(url), index = i })
                            .ToList();

                        configuration.SetFormats(stream.Select(o => o.format).ToList());
                        stream.ForEach(o =>
                        {
                            mainWindow.ckLbFormats.Items.Add(o.format);
                            mainWindow.ckLbFormats.SetItemCheckState(o.index, o.format.Checked ? CheckState.Checked : CheckState.Unchecked);
                        });
                    }
                }

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

        private void EditEditor(object sender, EventArgs e)
        {
            mainWindow.rtEditor.Enabled = mainWindow.ckbEditEditor.Checked;
            mainWindow.btSaveEditorChanges.Enabled = mainWindow.ckbEditEditor.Checked;
            mainWindow.ckbLineWrap.Enabled = mainWindow.ckbEditEditor.Checked;
        }

        private void AddMail(object sender, EventArgs e)
        {
            var pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            var r = new Regex(pattern);

            if (r.Match(mainWindow.txtbEmail.Text.Trim()).Success)
            {
                mainWindow.lbMails.Items.Add(mainWindow.txtbEmail.Text.Trim());
                mainWindow.txtbEmail.Text = Empty;
                configuration.SetNotifications(mainWindow.lbMails.Items.Cast<string>().ToList());
            }
            else
            {
                MessageBox.Show(
                    "Direccion de correo incorrecta.\n\n" + "Ejemplo de direcciones correctas:\n"
                    + "titu.cusi.huallpa@rjabogados.com\n" +
                    "joseholguin@hotmail.com", "Información");
                mainWindow.txtbEmail.Text = Empty;
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
                mainWindow.btGenerateWords.Enabled = false;
                mainWindow.bwCreateLetters.RunWorkerAsync(mainWindow.cbPaperSize.SelectedItem);

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
