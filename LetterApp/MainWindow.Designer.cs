namespace LetterApp
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.btGenerateWords = new System.Windows.Forms.Button();
            this.ckLbFormats = new System.Windows.Forms.CheckedListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabEditor = new System.Windows.Forms.TabPage();
            this.rtEditor = new System.Windows.Forms.RichTextBox();
            this.tabFilters = new System.Windows.Forms.TabPage();
            this.gbExtendedOptions = new System.Windows.Forms.GroupBox();
            this.btExtendedOptionHelp = new System.Windows.Forms.Button();
            this.cbExtendedOptions = new System.Windows.Forms.CheckBox();
            this.txtbExtendedOption = new System.Windows.Forms.TextBox();
            this.lbFilters = new System.Windows.Forms.ListBox();
            this.tabNotifications = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btAddFormat = new System.Windows.Forms.Button();
            this.btRemoveFormat = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.archivosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cargarConfiguraciónToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guardarConfiguraciónToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cerrarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.acercaDeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btSaveEditorChanges = new System.Windows.Forms.Button();
            this.btAddOption = new System.Windows.Forms.Button();
            this.gbFilters = new System.Windows.Forms.GroupBox();
            this.btRemoveFilter = new System.Windows.Forms.Button();
            this.btRemoveMail = new System.Windows.Forms.Button();
            this.txtbEmail = new System.Windows.Forms.TextBox();
            this.cbLineWrap = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabEditor.SuspendLayout();
            this.tabFilters.SuspendLayout();
            this.gbExtendedOptions.SuspendLayout();
            this.tabNotifications.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // picLogo
            // 
            this.picLogo.Image = global::LetterApp.Properties.Resources.logo;
            this.picLogo.Location = new System.Drawing.Point(12, 23);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(295, 88);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLogo.TabIndex = 0;
            this.picLogo.TabStop = false;
            // 
            // btGenerateWords
            // 
            this.btGenerateWords.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btGenerateWords.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btGenerateWords.Location = new System.Drawing.Point(1147, 23);
            this.btGenerateWords.Name = "btGenerateWords";
            this.btGenerateWords.Size = new System.Drawing.Size(172, 41);
            this.btGenerateWords.TabIndex = 1;
            this.btGenerateWords.Text = "Generar Cartas";
            this.btGenerateWords.UseVisualStyleBackColor = true;
            // 
            // ckLbFormats
            // 
            this.ckLbFormats.FormattingEnabled = true;
            this.ckLbFormats.Location = new System.Drawing.Point(12, 117);
            this.ckLbFormats.Name = "ckLbFormats";
            this.ckLbFormats.Size = new System.Drawing.Size(201, 589);
            this.ckLbFormats.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabEditor);
            this.tabControl1.Controls.Add(this.tabFilters);
            this.tabControl1.Controls.Add(this.tabNotifications);
            this.tabControl1.Location = new System.Drawing.Point(219, 117);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1100, 528);
            this.tabControl1.TabIndex = 3;
            // 
            // tabEditor
            // 
            this.tabEditor.Controls.Add(this.cbLineWrap);
            this.tabEditor.Controls.Add(this.btSaveEditorChanges);
            this.tabEditor.Controls.Add(this.rtEditor);
            this.tabEditor.Location = new System.Drawing.Point(4, 22);
            this.tabEditor.Name = "tabEditor";
            this.tabEditor.Padding = new System.Windows.Forms.Padding(3);
            this.tabEditor.Size = new System.Drawing.Size(1092, 502);
            this.tabEditor.TabIndex = 0;
            this.tabEditor.Text = "Editor";
            this.tabEditor.UseVisualStyleBackColor = true;
            // 
            // rtEditor
            // 
            this.rtEditor.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtEditor.Location = new System.Drawing.Point(6, 6);
            this.rtEditor.Name = "rtEditor";
            this.rtEditor.Size = new System.Drawing.Size(1080, 456);
            this.rtEditor.TabIndex = 0;
            this.rtEditor.Text = "";
            // 
            // tabFilters
            // 
            this.tabFilters.Controls.Add(this.btRemoveFilter);
            this.tabFilters.Controls.Add(this.gbFilters);
            this.tabFilters.Controls.Add(this.gbExtendedOptions);
            this.tabFilters.Controls.Add(this.lbFilters);
            this.tabFilters.Location = new System.Drawing.Point(4, 22);
            this.tabFilters.Name = "tabFilters";
            this.tabFilters.Padding = new System.Windows.Forms.Padding(3);
            this.tabFilters.Size = new System.Drawing.Size(1092, 502);
            this.tabFilters.TabIndex = 1;
            this.tabFilters.Text = "Filtros";
            this.tabFilters.UseVisualStyleBackColor = true;
            // 
            // gbExtendedOptions
            // 
            this.gbExtendedOptions.Controls.Add(this.btAddOption);
            this.gbExtendedOptions.Controls.Add(this.cbExtendedOptions);
            this.gbExtendedOptions.Controls.Add(this.btExtendedOptionHelp);
            this.gbExtendedOptions.Controls.Add(this.txtbExtendedOption);
            this.gbExtendedOptions.Enabled = false;
            this.gbExtendedOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbExtendedOptions.Location = new System.Drawing.Point(315, 384);
            this.gbExtendedOptions.Name = "gbExtendedOptions";
            this.gbExtendedOptions.Size = new System.Drawing.Size(353, 107);
            this.gbExtendedOptions.TabIndex = 11;
            this.gbExtendedOptions.TabStop = false;
            // 
            // btExtendedOptionHelp
            // 
            this.btExtendedOptionHelp.Location = new System.Drawing.Point(319, 34);
            this.btExtendedOptionHelp.Name = "btExtendedOptionHelp";
            this.btExtendedOptionHelp.Size = new System.Drawing.Size(23, 25);
            this.btExtendedOptionHelp.TabIndex = 1;
            this.btExtendedOptionHelp.Text = "?";
            this.btExtendedOptionHelp.UseVisualStyleBackColor = true;
            // 
            // cbExtendedOptions
            // 
            this.cbExtendedOptions.AutoSize = true;
            this.cbExtendedOptions.Location = new System.Drawing.Point(12, 0);
            this.cbExtendedOptions.Name = "cbExtendedOptions";
            this.cbExtendedOptions.Size = new System.Drawing.Size(204, 21);
            this.cbExtendedOptions.TabIndex = 0;
            this.cbExtendedOptions.Text = "Activar opciones avanzadas";
            this.cbExtendedOptions.UseVisualStyleBackColor = true;
            // 
            // txtbExtendedOption
            // 
            this.txtbExtendedOption.Location = new System.Drawing.Point(12, 36);
            this.txtbExtendedOption.Name = "txtbExtendedOption";
            this.txtbExtendedOption.Size = new System.Drawing.Size(301, 23);
            this.txtbExtendedOption.TabIndex = 9;
            // 
            // lbFilters
            // 
            this.lbFilters.FormattingEnabled = true;
            this.lbFilters.Location = new System.Drawing.Point(6, 6);
            this.lbFilters.Name = "lbFilters";
            this.lbFilters.Size = new System.Drawing.Size(303, 446);
            this.lbFilters.TabIndex = 0;
            // 
            // tabNotifications
            // 
            this.tabNotifications.Controls.Add(this.groupBox3);
            this.tabNotifications.Controls.Add(this.groupBox2);
            this.tabNotifications.Controls.Add(this.groupBox1);
            this.tabNotifications.Location = new System.Drawing.Point(4, 22);
            this.tabNotifications.Name = "tabNotifications";
            this.tabNotifications.Padding = new System.Windows.Forms.Padding(3);
            this.tabNotifications.Size = new System.Drawing.Size(1092, 502);
            this.tabNotifications.TabIndex = 2;
            this.tabNotifications.Text = "Notificaciones";
            this.tabNotifications.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtbEmail);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.button3);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(315, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(444, 101);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Agregar notificación";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 17);
            this.label3.TabIndex = 9;
            this.label3.Text = "Correo eletrónico:";
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(356, 62);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(82, 28);
            this.button3.TabIndex = 10;
            this.button3.Text = "Agregar";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btRemoveMail);
            this.groupBox2.Controls.Add(this.listBox2);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(303, 490);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Lista de personas a notificar";
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 16;
            this.listBox2.Location = new System.Drawing.Point(6, 22);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(291, 420);
            this.listBox2.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(315, 113);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(444, 88);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Credenciales para el envío";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(101, 22);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(170, 23);
            this.textBox1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Usuario:";
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(101, 51);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(170, 23);
            this.textBox2.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(10, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Contraseña:";
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(219, 651);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1100, 100);
            this.panel1.TabIndex = 4;
            // 
            // btAddFormat
            // 
            this.btAddFormat.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAddFormat.Location = new System.Drawing.Point(12, 723);
            this.btAddFormat.Name = "btAddFormat";
            this.btAddFormat.Size = new System.Drawing.Size(75, 28);
            this.btAddFormat.TabIndex = 5;
            this.btAddFormat.Text = "Agregar";
            this.btAddFormat.UseVisualStyleBackColor = true;
            // 
            // btRemoveFormat
            // 
            this.btRemoveFormat.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btRemoveFormat.Location = new System.Drawing.Point(138, 723);
            this.btRemoveFormat.Name = "btRemoveFormat";
            this.btRemoveFormat.Size = new System.Drawing.Size(75, 28);
            this.btRemoveFormat.TabIndex = 6;
            this.btRemoveFormat.Text = "Eliminar";
            this.btRemoveFormat.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.archivosToolStripMenuItem,
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1331, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // archivosToolStripMenuItem
            // 
            this.archivosToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cargarConfiguraciónToolStripMenuItem,
            this.guardarConfiguraciónToolStripMenuItem,
            this.toolStripSeparator1,
            this.cerrarToolStripMenuItem});
            this.archivosToolStripMenuItem.Name = "archivosToolStripMenuItem";
            this.archivosToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.archivosToolStripMenuItem.Text = "Archivos";
            // 
            // cargarConfiguraciónToolStripMenuItem
            // 
            this.cargarConfiguraciónToolStripMenuItem.Name = "cargarConfiguraciónToolStripMenuItem";
            this.cargarConfiguraciónToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.cargarConfiguraciónToolStripMenuItem.Text = "Cargar configuración";
            // 
            // guardarConfiguraciónToolStripMenuItem
            // 
            this.guardarConfiguraciónToolStripMenuItem.Name = "guardarConfiguraciónToolStripMenuItem";
            this.guardarConfiguraciónToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.guardarConfiguraciónToolStripMenuItem.Text = "Guardar configuración";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(190, 6);
            // 
            // cerrarToolStripMenuItem
            // 
            this.cerrarToolStripMenuItem.Name = "cerrarToolStripMenuItem";
            this.cerrarToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.cerrarToolStripMenuItem.Text = "Cerrar";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.acercaDeToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(24, 20);
            this.toolStripMenuItem1.Text = "?";
            // 
            // acercaDeToolStripMenuItem
            // 
            this.acercaDeToolStripMenuItem.Name = "acercaDeToolStripMenuItem";
            this.acercaDeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.acercaDeToolStripMenuItem.Text = "Acerca de";
            // 
            // btSaveEditorChanges
            // 
            this.btSaveEditorChanges.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btSaveEditorChanges.Location = new System.Drawing.Point(954, 468);
            this.btSaveEditorChanges.Name = "btSaveEditorChanges";
            this.btSaveEditorChanges.Size = new System.Drawing.Size(132, 28);
            this.btSaveEditorChanges.TabIndex = 1;
            this.btSaveEditorChanges.Text = "Guardar cambios";
            this.btSaveEditorChanges.UseVisualStyleBackColor = true;
            // 
            // btAddOption
            // 
            this.btAddOption.Location = new System.Drawing.Point(217, 65);
            this.btAddOption.Name = "btAddOption";
            this.btAddOption.Size = new System.Drawing.Size(125, 28);
            this.btAddOption.TabIndex = 10;
            this.btAddOption.Text = "Agregar opción";
            this.btAddOption.UseVisualStyleBackColor = true;
            // 
            // gbFilters
            // 
            this.gbFilters.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbFilters.Location = new System.Drawing.Point(315, 6);
            this.gbFilters.Name = "gbFilters";
            this.gbFilters.Size = new System.Drawing.Size(401, 372);
            this.gbFilters.TabIndex = 12;
            this.gbFilters.TabStop = false;
            this.gbFilters.Text = "Filtros";
            // 
            // btRemoveFilter
            // 
            this.btRemoveFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btRemoveFilter.Location = new System.Drawing.Point(234, 463);
            this.btRemoveFilter.Name = "btRemoveFilter";
            this.btRemoveFilter.Size = new System.Drawing.Size(75, 28);
            this.btRemoveFilter.TabIndex = 13;
            this.btRemoveFilter.Text = "Eliminar";
            this.btRemoveFilter.UseVisualStyleBackColor = true;
            // 
            // btRemoveMail
            // 
            this.btRemoveMail.Location = new System.Drawing.Point(181, 456);
            this.btRemoveMail.Name = "btRemoveMail";
            this.btRemoveMail.Size = new System.Drawing.Size(116, 28);
            this.btRemoveMail.TabIndex = 5;
            this.btRemoveMail.Text = "Eliminar correo";
            this.btRemoveMail.UseVisualStyleBackColor = true;
            // 
            // txtbEmail
            // 
            this.txtbEmail.Location = new System.Drawing.Point(133, 22);
            this.txtbEmail.Name = "txtbEmail";
            this.txtbEmail.Size = new System.Drawing.Size(305, 23);
            this.txtbEmail.TabIndex = 11;
            // 
            // cbLineWrap
            // 
            this.cbLineWrap.AutoSize = true;
            this.cbLineWrap.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbLineWrap.Location = new System.Drawing.Point(828, 473);
            this.cbLineWrap.Name = "cbLineWrap";
            this.cbLineWrap.Size = new System.Drawing.Size(120, 21);
            this.cbLineWrap.TabIndex = 2;
            this.cbLineWrap.Text = "Ajuste de línea";
            this.cbLineWrap.UseVisualStyleBackColor = true;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1331, 761);
            this.Controls.Add(this.btRemoveFormat);
            this.Controls.Add(this.btAddFormat);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.ckLbFormats);
            this.Controls.Add(this.btGenerateWords);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "Generador de cartas";
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabEditor.ResumeLayout(false);
            this.tabEditor.PerformLayout();
            this.tabFilters.ResumeLayout(false);
            this.gbExtendedOptions.ResumeLayout(false);
            this.gbExtendedOptions.PerformLayout();
            this.tabNotifications.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.TabPage tabEditor;
        private System.Windows.Forms.TabPage tabFilters;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabPage tabNotifications;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Button btGenerateWords;
        public System.Windows.Forms.CheckedListBox ckLbFormats;
        public System.Windows.Forms.TabControl tabControl1;
        public System.Windows.Forms.RichTextBox rtEditor;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem archivosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cargarConfiguraciónToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem guardarConfiguraciónToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem cerrarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem acercaDeToolStripMenuItem;
        public System.Windows.Forms.Button btAddFormat;
        public System.Windows.Forms.Button btRemoveFormat;
        public System.Windows.Forms.Button btSaveEditorChanges;
        public System.Windows.Forms.CheckBox cbExtendedOptions;
        public System.Windows.Forms.GroupBox gbExtendedOptions;
        public System.Windows.Forms.GroupBox gbFilters;
        public System.Windows.Forms.Button btRemoveFilter;
        public System.Windows.Forms.ListBox lbFilters;
        public System.Windows.Forms.Button btAddOption;
        private System.Windows.Forms.Button btRemoveMail;
        public System.Windows.Forms.Button btExtendedOptionHelp;
        public System.Windows.Forms.TextBox txtbExtendedOption;
        public System.Windows.Forms.TextBox txtbEmail;
        public System.Windows.Forms.CheckBox cbLineWrap;
    }
}

