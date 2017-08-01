using LetterApp.model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LetterApp.view
{
    public class MainWindowPresenter
    {
        private MainWindow mainWindow;
        private Configuration configuration;
        private Dictionary<CheckBox, Tuple<TextBox, string>> filterPair;

        public MainWindowPresenter(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.filterPair = new Dictionary<CheckBox, Tuple<TextBox, string>>();

            UpdateFilterTab();

            LoadConfiguration();
            RefreshGUI();

            CreateEvents();
        }

        private void UpdateFilterTab()
        {
            var conf = JToken.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "GUIConfiguration.json")));
            var filters = conf["filters"] ?? new JArray();


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

            ManageCheckGroupBox(mainWindow.cbExtendedOptions, mainWindow.gbExtendedOptions);
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
            }
            else
            {
                var alt = tuple.Item1.Text.Trim().Length == 0 ? ", comience agregando algún valor al filtro." : ".";
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
            mainWindow.gbExtendedOptions.Enabled = mainWindow.cbExtendedOptions.Checked;
        }

        private void RefreshGUI()
        {
        }

        private void LoadConfiguration()
        {
            var conf = JToken.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "configuration.json")));
            configuration = new Configuration();
            var files = conf["files"] ?? new JArray();
            files.ToList().ForEach(f =>
            {
                var url = f["url"] ?? null;
                if (url == null) throw new FileLoadException("Error: No es posible cargar un archivo con ua URL vacía.");
                var fileChecked = f["checked"] ?? false;
                var filters = f["filters"] ?? new JArray();
                var notifications = f["notifications"] ?? new JArray();

                configuration.Add(new Format(url.Value<string>(), Convert.ToBoolean(fileChecked.Value<string>()),
                    filters.Values<string>().ToList(), notifications.Values<string>().ToList()));
            });
        }

        private void CreateEvents()
        {
            mainWindow.btGenerateWords.Click += new EventHandler(GenerateLetterButtonEvent);
            mainWindow.btAddFormat.Click += new EventHandler(AddFormat);
            mainWindow.btRemoveFormat.Click += new EventHandler(RemoveFormat);
            mainWindow.btSaveEditorChanges.Click += new EventHandler(SaveEditorChanges);
            mainWindow.btRemoveFilter.Click += new EventHandler(RemoveFilter);
            mainWindow.cbLineWrap.Click += new EventHandler(LineWrap);
            mainWindow.btAddOption.Click += new EventHandler(AddOption);
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
            }
        }

        private void LineWrap(object sender, EventArgs e)
        {
            mainWindow.rtEditor.WordWrap = !mainWindow.cbLineWrap.Checked;
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

                        RefreshGUI();
                    }
                }
            }

        }

        private void SaveEditorChanges(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("¿Está seguro que desea eliminar el formato?",
                                 "Confirmar eliminación", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                //todo guardar los cambios.
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
                if (mainWindow.ckLbFormats.SelectedIndex == -1)
                {
                    MessageBox.Show(mainWindow, "Debe seleccionar un formato a eliminar", "Eliminar formato");
                }
                else
                {
                    var confirmResult = MessageBox.Show("¿Está seguro que desea eliminar el formato?",
                                         "Confirmar eliminación", MessageBoxButtons.YesNo);
                    if (confirmResult == DialogResult.Yes)
                    {
                        //todo eliminar de la conf y actualizar.
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
                try
                {
                    //todo ver si ya está en la conf actual.
                    //todo crear resaltado para json.
                    var currentEncoding = Encoding.UTF8;
                    using (var reader = new System.IO.StreamReader(openFileDialog.FileName, true))
                    {
                        currentEncoding = reader.CurrentEncoding;
                    }
                    mainWindow.rtEditor.Text = File.ReadAllText(openFileDialog.FileName, currentEncoding);
                    configuration.Add(openFileDialog.FileName);
                    RefreshGUI();
                    CleanFilters();
                    CleanNotifications();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: No fue posible leer el formato del disco.");
                }
            }
        }

        private void CleanNotifications()
        {
        }

        private void CleanFilters()
        {
        }

        private void GenerateLetterButtonEvent(object sender, EventArgs e)
        {
            //todo crear cartas.
        }
    }
}
