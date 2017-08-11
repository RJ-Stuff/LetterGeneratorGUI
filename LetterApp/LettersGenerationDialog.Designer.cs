namespace LetterApp
{
    partial class LettersGenerationDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LettersGenerationDialog));
            this.pbPart = new System.Windows.Forms.ProgressBar();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pbTotal = new System.Windows.Forms.ProgressBar();
            this.lProgressInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pbPart
            // 
            this.pbPart.Location = new System.Drawing.Point(12, 184);
            this.pbPart.Name = "pbPart";
            this.pbPart.Size = new System.Drawing.Size(473, 19);
            this.pbPart.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbPart.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::LetterApp.Properties.Resources.logo;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(473, 141);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // pbTotal
            // 
            this.pbTotal.Location = new System.Drawing.Point(12, 209);
            this.pbTotal.Name = "pbTotal";
            this.pbTotal.Size = new System.Drawing.Size(473, 19);
            this.pbTotal.TabIndex = 3;
            // 
            // lProgressInfo
            // 
            this.lProgressInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lProgressInfo.Location = new System.Drawing.Point(12, 156);
            this.lProgressInfo.Name = "lProgressInfo";
            this.lProgressInfo.Size = new System.Drawing.Size(473, 20);
            this.lProgressInfo.TabIndex = 4;
            this.lProgressInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lProgressInfo.UseMnemonic = false;
            // 
            // LettersGenerationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 238);
            this.Controls.Add(this.lProgressInfo);
            this.Controls.Add(this.pbTotal);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pbPart);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(513, 277);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(513, 277);
            this.Name = "LettersGenerationDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Creando cartas";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ProgressBar pbPart;
        private System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.ProgressBar pbTotal;
        public System.Windows.Forms.Label lProgressInfo;
    }
}