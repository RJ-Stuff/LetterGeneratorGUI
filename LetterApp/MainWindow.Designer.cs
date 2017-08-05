namespace LetterApp
{
    using System.ComponentModel;
    using System.Windows.Forms;

    using GridExtensions;
    using GridExtensions.GridFilterFactories;

    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.components = new System.ComponentModel.Container();
            GridExtensions.GridFilterFactories.DefaultGridFilterFactory defaultGridFilterFactory1 = new GridExtensions.GridFilterFactories.DefaultGridFilterFactory();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.btGenerateWords = new System.Windows.Forms.Button();
            this.ckLbFormats = new System.Windows.Forms.CheckedListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabEditor = new System.Windows.Forms.TabPage();
            this.ckbEditEditor = new System.Windows.Forms.CheckBox();
            this.ckbLineWrap = new System.Windows.Forms.CheckBox();
            this.btSaveEditorChanges = new System.Windows.Forms.Button();
            this.rtEditor = new System.Windows.Forms.RichTextBox();
            this.tabData = new System.Windows.Forms.TabPage();
            this.btLoadData = new System.Windows.Forms.Button();
            this.tabNotifications = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbOnlyMail = new System.Windows.Forms.RadioButton();
            this.rbMailWithAtt = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtbEmail = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btAddMail = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btRemoveMail = new System.Windows.Forms.Button();
            this.lbMails = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btMailHelp = new System.Windows.Forms.Button();
            this.txtbUser = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtbPass = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbPaperSize = new System.Windows.Forms.ComboBox();
            this.btAddFormat = new System.Windows.Forms.Button();
            this.btRemoveFormat = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.archivosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cerrarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.acercaDeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btChargesHelp = new System.Windows.Forms.Button();
            this.cbCharge = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this._extender = new GridExtensions.DataGridFilterExtender(this.components);
            this.dgClients = new GridExtensions.ExtendedDataGrid();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabEditor.SuspendLayout();
            this.tabData.SuspendLayout();
            this.tabNotifications.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._extender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgClients)).BeginInit();
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
            this.tabControl1.Controls.Add(this.tabData);
            this.tabControl1.Controls.Add(this.tabNotifications);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(219, 117);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1100, 528);
            this.tabControl1.TabIndex = 3;
            // 
            // tabEditor
            // 
            this.tabEditor.Controls.Add(this.ckbEditEditor);
            this.tabEditor.Controls.Add(this.ckbLineWrap);
            this.tabEditor.Controls.Add(this.btSaveEditorChanges);
            this.tabEditor.Controls.Add(this.rtEditor);
            this.tabEditor.Location = new System.Drawing.Point(4, 25);
            this.tabEditor.Name = "tabEditor";
            this.tabEditor.Padding = new System.Windows.Forms.Padding(3);
            this.tabEditor.Size = new System.Drawing.Size(1092, 499);
            this.tabEditor.TabIndex = 0;
            this.tabEditor.Text = "Editor";
            this.tabEditor.UseVisualStyleBackColor = true;
            // 
            // ckbEditEditor
            // 
            this.ckbEditEditor.AutoSize = true;
            this.ckbEditEditor.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckbEditEditor.Location = new System.Drawing.Point(6, 473);
            this.ckbEditEditor.Name = "ckbEditEditor";
            this.ckbEditEditor.Size = new System.Drawing.Size(78, 21);
            this.ckbEditEditor.TabIndex = 3;
            this.ckbEditEditor.Text = "Editable";
            this.ckbEditEditor.UseVisualStyleBackColor = true;
            // 
            // ckbLineWrap
            // 
            this.ckbLineWrap.AutoSize = true;
            this.ckbLineWrap.Enabled = false;
            this.ckbLineWrap.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckbLineWrap.Location = new System.Drawing.Point(828, 473);
            this.ckbLineWrap.Name = "ckbLineWrap";
            this.ckbLineWrap.Size = new System.Drawing.Size(120, 21);
            this.ckbLineWrap.TabIndex = 2;
            this.ckbLineWrap.Text = "Ajuste de línea";
            this.ckbLineWrap.UseVisualStyleBackColor = true;
            // 
            // btSaveEditorChanges
            // 
            this.btSaveEditorChanges.Enabled = false;
            this.btSaveEditorChanges.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btSaveEditorChanges.Location = new System.Drawing.Point(954, 468);
            this.btSaveEditorChanges.Name = "btSaveEditorChanges";
            this.btSaveEditorChanges.Size = new System.Drawing.Size(132, 28);
            this.btSaveEditorChanges.TabIndex = 1;
            this.btSaveEditorChanges.Text = "Guardar cambios";
            this.btSaveEditorChanges.UseVisualStyleBackColor = true;
            // 
            // rtEditor
            // 
            this.rtEditor.Enabled = false;
            this.rtEditor.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtEditor.Location = new System.Drawing.Point(6, 6);
            this.rtEditor.Name = "rtEditor";
            this.rtEditor.Size = new System.Drawing.Size(1080, 456);
            this.rtEditor.TabIndex = 0;
            this.rtEditor.Text = "";
            // 
            // tabData
            // 
            this.tabData.Controls.Add(this.btLoadData);
            this.tabData.Controls.Add(this.dgClients);
            this.tabData.Location = new System.Drawing.Point(4, 25);
            this.tabData.Name = "tabData";
            this.tabData.Size = new System.Drawing.Size(1092, 499);
            this.tabData.TabIndex = 3;
            this.tabData.Text = "Datos";
            this.tabData.UseVisualStyleBackColor = true;
            // 
            // btLoadData
            // 
            this.btLoadData.Location = new System.Drawing.Point(985, 471);
            this.btLoadData.Name = "btLoadData";
            this.btLoadData.Size = new System.Drawing.Size(104, 25);
            this.btLoadData.TabIndex = 1;
            this.btLoadData.Text = "Cargar datos";
            this.btLoadData.UseVisualStyleBackColor = true;
            // 
            // tabNotifications
            // 
            this.tabNotifications.Controls.Add(this.groupBox4);
            this.tabNotifications.Controls.Add(this.groupBox3);
            this.tabNotifications.Controls.Add(this.groupBox2);
            this.tabNotifications.Controls.Add(this.groupBox1);
            this.tabNotifications.Location = new System.Drawing.Point(4, 25);
            this.tabNotifications.Name = "tabNotifications";
            this.tabNotifications.Padding = new System.Windows.Forms.Padding(3);
            this.tabNotifications.Size = new System.Drawing.Size(1092, 499);
            this.tabNotifications.TabIndex = 2;
            this.tabNotifications.Text = "Notificaciones";
            this.tabNotifications.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rbOnlyMail);
            this.groupBox4.Controls.Add(this.rbMailWithAtt);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(315, 207);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(444, 289);
            this.groupBox4.TabIndex = 14;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Opciones";
            // 
            // rbOnlyMail
            // 
            this.rbOnlyMail.AutoSize = true;
            this.rbOnlyMail.Checked = true;
            this.rbOnlyMail.Location = new System.Drawing.Point(6, 33);
            this.rbOnlyMail.Name = "rbOnlyMail";
            this.rbOnlyMail.Size = new System.Drawing.Size(133, 21);
            this.rbOnlyMail.TabIndex = 12;
            this.rbOnlyMail.TabStop = true;
            this.rbOnlyMail.Text = "Solo notificación.";
            this.rbOnlyMail.UseVisualStyleBackColor = true;
            // 
            // rbMailWithAtt
            // 
            this.rbMailWithAtt.AutoSize = true;
            this.rbMailWithAtt.Location = new System.Drawing.Point(6, 60);
            this.rbMailWithAtt.Name = "rbMailWithAtt";
            this.rbMailWithAtt.Size = new System.Drawing.Size(192, 21);
            this.rbMailWithAtt.TabIndex = 13;
            this.rbMailWithAtt.Text = "Notificar y adjuntar cartas.";
            this.rbMailWithAtt.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtbEmail);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.btAddMail);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(315, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(444, 101);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Agregar notificación";
            // 
            // txtbEmail
            // 
            this.txtbEmail.Location = new System.Drawing.Point(133, 22);
            this.txtbEmail.Name = "txtbEmail";
            this.txtbEmail.Size = new System.Drawing.Size(305, 23);
            this.txtbEmail.TabIndex = 11;
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
            // btAddMail
            // 
            this.btAddMail.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAddMail.Location = new System.Drawing.Point(356, 62);
            this.btAddMail.Name = "btAddMail";
            this.btAddMail.Size = new System.Drawing.Size(82, 28);
            this.btAddMail.TabIndex = 10;
            this.btAddMail.Text = "Agregar";
            this.btAddMail.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btRemoveMail);
            this.groupBox2.Controls.Add(this.lbMails);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(303, 490);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Lista de personas a notificar";
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
            // lbMails
            // 
            this.lbMails.FormattingEnabled = true;
            this.lbMails.ItemHeight = 16;
            this.lbMails.Location = new System.Drawing.Point(6, 22);
            this.lbMails.Name = "lbMails";
            this.lbMails.Size = new System.Drawing.Size(291, 420);
            this.lbMails.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btMailHelp);
            this.groupBox1.Controls.Add(this.txtbUser);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtbPass);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(315, 113);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(444, 88);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Credenciales para el envío";
            // 
            // btMailHelp
            // 
            this.btMailHelp.Location = new System.Drawing.Point(277, 22);
            this.btMailHelp.Name = "btMailHelp";
            this.btMailHelp.Size = new System.Drawing.Size(23, 23);
            this.btMailHelp.TabIndex = 4;
            this.btMailHelp.Text = "?";
            this.btMailHelp.UseVisualStyleBackColor = true;
            // 
            // txtbUser
            // 
            this.txtbUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtbUser.Location = new System.Drawing.Point(101, 22);
            this.txtbUser.Name = "txtbUser";
            this.txtbUser.Size = new System.Drawing.Size(170, 23);
            this.txtbUser.TabIndex = 2;
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
            // txtbPass
            // 
            this.txtbPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtbPass.Location = new System.Drawing.Point(101, 51);
            this.txtbPass.Name = "txtbPass";
            this.txtbPass.PasswordChar = '*';
            this.txtbPass.Size = new System.Drawing.Size(170, 23);
            this.txtbPass.TabIndex = 3;
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(7, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(126, 17);
            this.label4.TabIndex = 1;
            this.label4.Text = "Tamaño del papel:";
            // 
            // cbPaperSize
            // 
            this.cbPaperSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbPaperSize.FormattingEnabled = true;
            this.cbPaperSize.Location = new System.Drawing.Point(139, 26);
            this.cbPaperSize.Name = "cbPaperSize";
            this.cbPaperSize.Size = new System.Drawing.Size(159, 24);
            this.cbPaperSize.TabIndex = 0;
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
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
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
            this.cerrarToolStripMenuItem});
            this.archivosToolStripMenuItem.Name = "archivosToolStripMenuItem";
            this.archivosToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.archivosToolStripMenuItem.Text = "Archivos";
            // 
            // cerrarToolStripMenuItem
            // 
            this.cerrarToolStripMenuItem.Name = "cerrarToolStripMenuItem";
            this.cerrarToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
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
            this.acercaDeToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.acercaDeToolStripMenuItem.Text = "Acerca de";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btChargesHelp);
            this.groupBox5.Controls.Add(this.cbCharge);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.cbPaperSize);
            this.groupBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.Location = new System.Drawing.Point(219, 651);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(1100, 100);
            this.groupBox5.TabIndex = 8;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Opciones del formato";
            // 
            // btChargesHelp
            // 
            this.btChargesHelp.Location = new System.Drawing.Point(304, 65);
            this.btChargesHelp.Name = "btChargesHelp";
            this.btChargesHelp.Size = new System.Drawing.Size(25, 24);
            this.btChargesHelp.TabIndex = 4;
            this.btChargesHelp.Text = "?";
            this.btChargesHelp.UseVisualStyleBackColor = true;
            // 
            // cbCharge
            // 
            this.cbCharge.FormattingEnabled = true;
            this.cbCharge.Location = new System.Drawing.Point(139, 65);
            this.cbCharge.Name = "cbCharge";
            this.cbCharge.Size = new System.Drawing.Size(159, 24);
            this.cbCharge.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 72);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 17);
            this.label5.TabIndex = 2;
            this.label5.Text = "Cargo:";
            // 
            // _extender
            // 
            this._extender.DataGrid = this.dgClients;
            defaultGridFilterFactory1.CreateDistinctGridFilters = false;
            defaultGridFilterFactory1.DefaultGridFilterType = typeof(GridExtensions.GridFilters.TextGridFilter);
            defaultGridFilterFactory1.DefaultShowDateInBetweenOperator = false;
            defaultGridFilterFactory1.DefaultShowNumericInBetweenOperator = false;
            defaultGridFilterFactory1.HandleEnumerationTypes = true;
            defaultGridFilterFactory1.MaximumDistinctValues = 20;
            this._extender.FilterFactory = defaultGridFilterFactory1;
            this._extender.GridMode = GridExtensions.GridMode.Filter;
            // 
            // dgClients
            // 
            this.dgClients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgClients.AutoCreateTableStyles = true;
            this.dgClients.DataMember = "";
            this.dgClients.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgClients.Location = new System.Drawing.Point(3, 3);
            this.dgClients.Name = "dgClients";
            this.dgClients.Size = new System.Drawing.Size(1086, 462);
            this.dgClients.TabIndex = 0;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1331, 761);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.btRemoveFormat);
            this.Controls.Add(this.btAddFormat);
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
            this.tabData.ResumeLayout(false);
            this.tabNotifications.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._extender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgClients)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBox picLogo;
        private TabPage tabEditor;
        private TabPage tabNotifications;
        private GroupBox groupBox3;
        private Label label3;
        private GroupBox groupBox2;
        private GroupBox groupBox1;
        private Label label1;
        private Label label2;
        public Button btGenerateWords;
        public CheckedListBox ckLbFormats;
        public TabControl tabControl1;
        public RichTextBox rtEditor;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem archivosToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        public Button btAddFormat;
        public Button btRemoveFormat;
        public Button btSaveEditorChanges;
        public TextBox txtbEmail;
        public CheckBox ckbLineWrap;
        public Button btRemoveMail;
        public ListBox lbMails;
        public Button btAddMail;
        public TextBox txtbUser;
        private GroupBox groupBox4;
        public RadioButton rbOnlyMail;
        public RadioButton rbMailWithAtt;
        public TextBox txtbPass;
        public ToolStripMenuItem acercaDeToolStripMenuItem;
        public CheckBox ckbEditEditor;
        private Label label4;
        public ComboBox cbPaperSize;
        private GroupBox groupBox5;
        public Button btMailHelp;
        public ToolStripMenuItem cerrarToolStripMenuItem;
        private Label label5;
        public ComboBox cbCharge;
        public Button btChargesHelp;
        private TabPage tabData;
        private DataGridFilterExtender _extender;
        public Button btLoadData;
        public ExtendedDataGrid dgClients;
    }
}

