namespace LetterApp.view
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using LetterApp.model;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class MainWindowPresenter
    {
        private readonly MainWindow mainWindow;

        private Configuration configuration;
        private bool notSavedChanges;

        public MainWindowPresenter(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            notSavedChanges = false;

            var guiConfiguration = JToken.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "GUIConfiguration.json"), Encoding.Default));

            mainWindow.cbPaperSize.DropDownStyle = ComboBoxStyle.DropDownList;
            mainWindow.cbCharge.DropDownStyle = ComboBoxStyle.DropDownList;

            (guiConfiguration["papersizes"] ?? new JArray())
            .Select(p =>
            {
                var chargesTokens = p["charges"] ?? new JArray();
                var charges = chargesTokens
                .Select(t => new Charge(t["ChargeClazz"].Value<string>(), t["DisplayName"].Value<string>()))
                .ToList();

                return new PaperSize(p["name"].Value<string>(), p["displayname"].Value<string>(), charges);
            })
            .ToList()
            .ForEach(o => this.mainWindow.cbPaperSize.Items.Add(o));

            // UpdateFilterTab();
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
            mainWindow.btGenerateWords.Click += GenerateLetterButtonEvent;
            mainWindow.btAddFormat.Click += AddFormat;
            mainWindow.btRemoveFormat.Click += RemoveFormat;
            mainWindow.btSaveEditorChanges.Click += SaveEditorChanges;
            mainWindow.ckbLineWrap.Click += LineWrap;
            mainWindow.rtEditor.KeyPress += TextOnEditorChange;
            mainWindow.btRemoveMail.Click += RemoveMail;
            mainWindow.btAddMail.Click += AddMail;
            mainWindow.ckbEditEditor.Click += EditEditor;
            mainWindow.ckLbFormats.SelectedIndexChanged += (s, e) => this.RefreshGui();
            mainWindow.cbPaperSize.SelectedIndexChanged += SelectedPaperSizeChange;
            mainWindow.ckLbFormats.ItemCheck += FormatCheck;
            mainWindow.btMailHelp.Click += MailHelp;
            mainWindow.cbCharge.SelectedIndexChanged += SelectedChargeChange;
            mainWindow.btChargesHelp.Click += ChargesHelp;
            mainWindow.acercaDeToolStripMenuItem.Click += AboutHelp;
            mainWindow.btLoadData.Click += LoadData;
        }

        private void LoadData(object sender, EventArgs e)
        {
            var index = mainWindow.ckLbFormats.SelectedIndex;
            if (index != -1)
            {
                // var query = (mainWindow.ckLbFormats.Items[index] as Format).
                // cargar el query....
                mainWindow.dgClients.DataSource = DataHelper.SampleData.Tables[0].DefaultView;
            }
        }

        private void SelectedChargeChange(object sender, EventArgs e)
        {
            var charge = (mainWindow.cbCharge.SelectedItem as Charge) ?? Charge.DefaultCharge;

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
                mainWindow.rtEditor.Text = string.Empty;

                mainWindow.ckbEditEditor.Checked = false;
                EditEditor(null, null);

                var index = mainWindow.ckLbFormats.SelectedIndex;
                if (index == -1)
                {
                    return;
                }

                var format = mainWindow.ckLbFormats.Items[index] as Format;

                // todo crear resaltado para json.
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
            }
            catch (Exception)
            {
                MessageBox.Show("No fue posible leer el formato del disco.", "Error");
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

                this.RefreshGui();
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

            var charge = (mainWindow.cbCharge.SelectedItem as Charge) ?? Charge.DefaultCharge;

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
                mainWindow.txtbEmail.Text = string.Empty;
                configuration.SetNotifications(mainWindow.lbMails.Items.Cast<string>().ToList());
            }
            else
            {
                MessageBox.Show(
                    "Direccion de correo incorrecta.\n\n" + "Ejemplo de direcciones correctas:\n"
                    + "titu.cusi.huallpa@rjabogados.com\n" +
                    "joseholguin@hotmail.com", "Información");
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

                                this.RefreshGui();

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

                    this.RefreshGui();
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

            this.RefreshGui();
        }

        private void GenerateLetterButtonEvent(object sender, EventArgs e)
        {
            // todo crear cartas.
        }
    }
}
