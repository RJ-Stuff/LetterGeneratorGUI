using LetterApp.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LetterApp.view
{
    public class MainWindowPresenter
    {
        private MainWindow mainWindow;
        private Configuration configuration;
        private Dictionary<CheckBox, Tuple<TextBox, string>> filterPair;
        private bool UnSavedChanges;
        private JToken GUIConfiguration;

        public MainWindowPresenter(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.filterPair = new Dictionary<CheckBox, Tuple<TextBox, string>>();
            UnSavedChanges = false;

            GUIConfiguration = JToken.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "GUIConfiguration.json"), Encoding.Default));

            mainWindow.cbPaperSize.DropDownStyle = ComboBoxStyle.DropDownList;

            (GUIConfiguration["papersizes"] ?? new JArray())
            .Select(p => new PaperSize(p["name"].Value<string>(), p["displayname"].Value<string>()))
            .ToList()
            .ForEach(o => this.mainWindow.cbPaperSize.Items.Add(o));

            UpdateFilterTab();

            LoadConfiguration();

            CreateEvents();
        }

        private void CreateEvents()
        {
            mainWindow.btGenerateWords.Click += new EventHandler(GenerateLetterButtonEvent);
            mainWindow.btAddFormat.Click += new EventHandler(AddFormat);
            mainWindow.btRemoveFormat.Click += new EventHandler(RemoveFormat);
            mainWindow.btSaveEditorChanges.Click += new EventHandler(SaveEditorChanges);
            mainWindow.btRemoveFilter.Click += new EventHandler(RemoveFilter);
            mainWindow.ckbLineWrap.Click += new EventHandler(LineWrap);
            mainWindow.btAddOption.Click += new EventHandler(AddOption);
            mainWindow.btExtendedOptionHelp.Click += new EventHandler(ExtendedOptionHelp);
            mainWindow.rtEditor.KeyPress += new KeyPressEventHandler(TextOnEditorChange);
            mainWindow.btRemoveMail.Click += new EventHandler(RemoveMail);
            mainWindow.btAddMail.Click += new EventHandler(AddMail);
            mainWindow.ckbEditEditor.Click += new EventHandler(EditEditor);
            mainWindow.ckLbFormats.SelectedIndexChanged += new EventHandler((s, e) => RefreshGUI());
            mainWindow.cbPaperSize.SelectedIndexChanged += new EventHandler(SelectedPaperSizeChange);
            mainWindow.ckLbFormats.ItemCheck += new ItemCheckEventHandler(FormatCheck);
            mainWindow.btMailHelp.Click += new EventHandler(MailHelp);
        }

        private void MailHelp(object sender, EventArgs e)
        {
            MessageBox.Show("Las credenciales son las mismas que se \n" +
                "utilizan para abrir el correo de la empresa.\n\n" +
                "Ejemplo:\nUsuario: titu.cusi.huallpa@rjabogados.com\n" +
                "Contraseña: tu-contraseña-personal", "Información");
        }

        private void RefreshGUI()
        {
            mainWindow.ckbEditEditor.Enabled = mainWindow.ckLbFormats.Items.Count != 0;

            try
            {
                //limpiar filtros
                mainWindow.lbFilters.Items.Clear();
                filterPair.Keys.ToList().ForEach(k =>
                {
                    k.Checked = false;
                    var tuple = filterPair[k];
                    if (tuple.Item1 != null)
                    {
                        tuple.Item1.Text = "";
                        tuple.Item1.Enabled = true;
                    }
                });
                mainWindow.ckbExtendedOptions.Checked = false;
                mainWindow.gbExtendedOptions.Enabled = false;
                mainWindow.txtbExtendedOption.Text = "";

                //limpiar editor
                mainWindow.rtEditor.Text = "";

                //actualiza el editable
                mainWindow.ckbEditEditor.Checked = false;
                EditEditor(null, null);

                var index = mainWindow.ckLbFormats.SelectedIndex;
                if (index != -1)
                {
                    var format = mainWindow.ckLbFormats.Items[index] as Format;

                    //actualiza editor.
                    //todo crear resaltado para json.
                    var currentEncoding = Encoding.UTF8;
                    using (var reader = new StreamReader(format.URL, true))
                    {
                        currentEncoding = reader.CurrentEncoding;
                    }
                    mainWindow.rtEditor.Text = File.ReadAllText(format.URL, currentEncoding);

                    //actualiza el tipo de papel
                    mainWindow.cbPaperSize.SelectedItem = format.PaperSize;

                    //actualiza filtros
                    format.Filters.ForEach(f =>
                    {
                        mainWindow.lbFilters.Items.Add(f);
                        var keys = filterPair.Keys.Where(k => k.Text == f.DisplayName).ToList();
                        if (keys.Count != 0)
                        {
                            var key = keys[0];
                            key.Checked = true;
                            if (filterPair.ContainsKey(key))
                            {
                                var tuple = filterPair[key];
                                if (tuple.Item1 != null)
                                {
                                    tuple.Item1.Text = f.Value.ToString();
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible leer el formato del disco.", "Error");
            }
        }

        private void FormatCheck(object sender, ItemCheckEventArgs e)
        {
            var index = e.Index;
            if (index != -1)
            {
                var format = mainWindow.ckLbFormats.Items[index] as Format;
                format.Checked = e.NewValue == CheckState.Checked ? true : false;
                configuration.SetFormats(mainWindow.ckLbFormats.Items.Cast<Format>().ToList());
            }
        }

        private void UpdateFilterTab()
        {
            var filters = GUIConfiguration["filters"] ?? new JArray();

            filters.Select((f, i) => new { filter = f, i = i }).ToList().ForEach(f =>
                {
                    var ckb = new CheckBox()
                    {
                        Text = f.filter["kind"].Value<string>(),
                        Location = new Point(5, (f.i + 1) * 23),
                        Width = 210
                    };
                    ckb.Click += new EventHandler(FilterAction);
                    mainWindow.gbFilters.Controls.Add(ckb);

                    if (Convert.ToBoolean(f.filter["box"].Value<string>()))
                    {
                        var txb = new TextBox()
                        {
                            Location = new Point(215, (f.i + 1) * 23),
                            Width = 150,
                            Height = 23
                        };
                        mainWindow.gbFilters.Controls.Add(txb);

                        filterPair[ckb] = Tuple.Create(txb, f.filter["internalname"].Value<string>());
                    }
                    else
                    {
                        filterPair[ckb] = new Tuple<TextBox, string>(null, f.filter["internalname"].Value<string>());
                    }
                });

            ManageCheckGroupBox(mainWindow.ckbExtendedOptions, mainWindow.gbExtendedOptions);
        }

        private void FilterAction(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;

            if (filterPair.TryGetValue(chk, out Tuple<TextBox, string> tuple) && Utils.Validate(tuple.Item1))
            {
                if (chk.Checked)
                {
                    mainWindow.lbFilters.Items.Add(new Filter(chk.Text, tuple.Item2, (tuple.Item1 ?? new TextBox()).Text));
                    if (tuple.Item1 != null)
                    {
                        tuple.Item1.Enabled = false;
                    }
                }
                else
                {
                    mainWindow.lbFilters.Items.Remove(new Filter(chk.Text, tuple.Item2, (tuple.Item1 ?? new TextBox()).Text));
                    if (tuple.Item1 != null)
                    {
                        tuple.Item1.Enabled = true;
                    }
                }
                FiltersChange();
            }
            else
            {
                var alt = tuple.Item1.Text.Trim().Length == 0 ? ", comience agregando algún valor al mismo." : ".";
                MessageBox.Show($"Hay problemas con la validación del valor del filtro{alt}", "Validación");
                chk.Checked = false;
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

            chk.Click += new EventHandler(ExtendedOptions);
        }

        private void ExtendedOptions(object sender, EventArgs e)
        {
            mainWindow.gbExtendedOptions.Enabled = mainWindow.ckbExtendedOptions.Checked;
        }

        private void LoadConfiguration()
        {
            //todo si no hay formatos en la conf, buscarlos en la carpeta formats.
            try
            {
                configuration = JsonConvert.DeserializeObject<Configuration>(
                                File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "configuration.json"), Encoding.Default));

                configuration.Formats
                    .Select((f, i) => new { format = f, index = i })
                    .ToList()
                    .ForEach(p =>
                    {
                        mainWindow.ckLbFormats.Items.Add(p.format);
                        mainWindow.ckLbFormats.SetItemCheckState(p.index, p.format.Checked ? CheckState.Checked : CheckState.Unchecked);
                    });

                configuration.Notifications.ForEach(n => mainWindow.lbMails.Items.Add(n));

                
                if(mainWindow.ckLbFormats.Items.Count == 0)
                {
                    var dir = Path.Combine(Directory.GetCurrentDirectory(), "formats");
                    if (Directory.Exists(dir))
                    {
                        var stream = Directory
                            .EnumerateFiles(dir, "*.rjf")
                            .Select((url,i) => new { format = new Format(url), index = i });

                        configuration.SetFormats(stream.Select(o => o.format).ToList());
                        stream.ToList().ForEach(o =>
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

                RefreshGUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un problema al cargar la configuración.", "Error");
            }
        }

        private void FiltersChange()
        {
            var index = mainWindow.ckLbFormats.SelectedIndex;
            if (index != -1)
            {
                var format = mainWindow.ckLbFormats.Items[index] as Format;
                format.Filters = mainWindow.lbFilters.Items.Cast<Filter>().ToList();
                configuration.SetFormats(mainWindow.ckLbFormats.Items.Cast<Format>().ToList());
            }
        }

        private void SelectedPaperSizeChange(object sender, EventArgs e)
        {
            var index = mainWindow.ckLbFormats.SelectedIndex;
            if (index != -1)
            {
                var format = mainWindow.ckLbFormats.Items[index] as Format;
                format.PaperSize = (mainWindow.cbPaperSize.SelectedItem as PaperSize) ?? PaperSize.DEFAULT_SIZE;
                configuration.SetFormats(mainWindow.ckLbFormats.Items.Cast<Format>().ToList());
            }
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
                mainWindow.txtbEmail.Text = "";
                configuration.SetNotifications(mainWindow.lbMails.Items.Cast<string>().ToList());
            }
            else
            {
                MessageBox.Show("Direccion de correo incorrecta.\n\n" +
                    "Ejemplo de direcciones correctas:\n" +
                    "titu.cusi.huallpa@rjabogados.com\n" +
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
                    if (option == DialogResult.Yes)
                    {
                        mainWindow.lbMails.Items.RemoveAt(index);
                        configuration.SetNotifications(mainWindow.lbMails.Items.Cast<string>().ToList());
                    }
                }
            }
        }

        private void TextOnEditorChange(object sender, EventArgs e)
        {
            UnSavedChanges = true;
        }

        private void ExtendedOptionHelp(object sender, EventArgs e)
        {
            MessageBox.Show("Mediante esta opción es posible\n" +
                "agregar filtros especializados o no\n" +
                "contemplados en las restantes opciones.\n\n" +
                "Este filtro se usa directamente en la consulta a la base de datos.", "Información");
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
                mainWindow.lbFilters.Items.Add(new Filter(option, option, ""));
                mainWindow.txtbExtendedOption.Text = "";
                FiltersChange();
            }
        }

        private void LineWrap(object sender, EventArgs e)
        {
            mainWindow.rtEditor.WordWrap = !mainWindow.ckbLineWrap.Checked;
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
                    var confirmResult = MessageBox.Show("¿Está seguro que desea eliminar el filtro?",
                         "Confirmar eliminación", MessageBoxButtons.YesNo);
                    if (confirmResult == DialogResult.Yes)
                    {
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

        }

        private void SaveEditorChanges(object sender, EventArgs e)
        {
            if (!UnSavedChanges)
            {
                MessageBox.Show("No existen cambios por guardar.", "Información");
            }
            else
            {
                if (mainWindow.rtEditor.Text.Trim().Length != 0)
                {
                    var confirmResult = MessageBox.Show("¿Está seguro que desea guardar el formato?",
                                         "Confirmación", MessageBoxButtons.YesNo);
                    if (confirmResult == DialogResult.Yes)
                    {
                        var index = mainWindow.ckLbFormats.SelectedIndex;
                        if (index != -1)
                        {
                            try
                            {
                                var format = mainWindow.ckLbFormats.Items[index] as Format;

                                var currentEncoding = Encoding.UTF8;
                                using (var reader = new StreamReader(format.URL, true))
                                {
                                    currentEncoding = reader.CurrentEncoding;
                                }

                                File.WriteAllText(format.URL, mainWindow.rtEditor.Text, currentEncoding);
                                RefreshGUI();

                                MessageBox.Show("Cambios guardados correctamente.", "Información");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Ocurrió un error al guardar el formato.", "Error");
                            }

                        }


                    }
                }

                UnSavedChanges = false;
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
                    var confirmResult = MessageBox.Show("¿Está seguro que desea eliminar el formato?",
                                         "Confirmar eliminación", MessageBoxButtons.YesNo);
                    if (confirmResult == DialogResult.Yes)
                    {
                        mainWindow.ckLbFormats.Items.RemoveAt(index);
                        configuration.SetFormats(mainWindow.ckLbFormats.Items.Cast<Format>().ToList());

                        if (mainWindow.ckLbFormats.Items.Count != 0)
                        {
                            mainWindow.ckLbFormats.SetSelected(0, true);
                        }

                        RefreshGUI();
                    }
                }
            }
        }

        private void AddFormat(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = Directory.GetCurrentDirectory(),
                Filter = "Formatos RJ (*.rjf)|*.rjf|Todos los archivos (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var format = new Format(openFileDialog.FileName);
                configuration.Add(format);
                mainWindow.ckLbFormats.Items.Add(format);
                mainWindow.ckLbFormats.SetSelected(mainWindow.ckLbFormats.Items.Count - 1, true);
                mainWindow.ckLbFormats.SetItemChecked(mainWindow.ckLbFormats.Items.Count - 1, true);
            }

            RefreshGUI();
        }

        private void GenerateLetterButtonEvent(object sender, EventArgs e)
        {
            //todo crear cartas.
        }
    }
}
