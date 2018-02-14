namespace SimulatoreTLC
{
    partial class Form1
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxRho = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonOutput = new System.Windows.Forms.Button();
            this.textBoxCoda = new System.Windows.Forms.TextBox();
            this.textBoxUtenti = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialogFileOutput = new System.Windows.Forms.FolderBrowserDialog();
            this.buttonSimula = new System.Windows.Forms.Button();
            this.buttonReset = new System.Windows.Forms.Button();
            this.progressBarAvanzamentoSImulazione = new System.Windows.Forms.ProgressBar();
            this.labelTempoSimulazione = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxRho);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.buttonOutput);
            this.groupBox1.Controls.Add(this.textBoxCoda);
            this.groupBox1.Controls.Add(this.textBoxUtenti);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(429, 193);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parametri della simulazione";
            // 
            // textBoxRho
            // 
            this.textBoxRho.Location = new System.Drawing.Point(251, 100);
            this.textBoxRho.Name = "textBoxRho";
            this.textBoxRho.Size = new System.Drawing.Size(172, 22);
            this.textBoxRho.TabIndex = 7;
            this.textBoxRho.TextChanged += new System.EventHandler(this.textBoxRho_TextChanged);
            this.textBoxRho.Leave += new System.EventHandler(this.textBoxRho_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(166, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "Passo incremento di Rho";
            // 
            // buttonOutput
            // 
            this.buttonOutput.Location = new System.Drawing.Point(251, 135);
            this.buttonOutput.Name = "buttonOutput";
            this.buttonOutput.Size = new System.Drawing.Size(172, 30);
            this.buttonOutput.TabIndex = 5;
            this.buttonOutput.Text = "Scegli percorso...";
            this.buttonOutput.UseVisualStyleBackColor = true;
            this.buttonOutput.Click += new System.EventHandler(this.buttonOutput_Click);
            // 
            // textBoxCoda
            // 
            this.textBoxCoda.Location = new System.Drawing.Point(251, 64);
            this.textBoxCoda.Name = "textBoxCoda";
            this.textBoxCoda.Size = new System.Drawing.Size(172, 22);
            this.textBoxCoda.TabIndex = 4;
            this.textBoxCoda.TextChanged += new System.EventHandler(this.textBoxCoda_TextChanged);
            this.textBoxCoda.Leave += new System.EventHandler(this.textBoxCoda_Leave);
            // 
            // textBoxUtenti
            // 
            this.textBoxUtenti.Location = new System.Drawing.Point(251, 32);
            this.textBoxUtenti.Name = "textBoxUtenti";
            this.textBoxUtenti.Size = new System.Drawing.Size(172, 22);
            this.textBoxUtenti.TabIndex = 3;
            this.textBoxUtenti.TextChanged += new System.EventHandler(this.textBoxUtenti_TextChanged);
            this.textBoxUtenti.Leave += new System.EventHandler(this.textBoxUtenti_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 142);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(194, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Destinazione dei file di output";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(174, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Massima dimensione coda";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Numero utenti";
            // 
            // buttonSimula
            // 
            this.buttonSimula.Location = new System.Drawing.Point(12, 283);
            this.buttonSimula.Name = "buttonSimula";
            this.buttonSimula.Size = new System.Drawing.Size(427, 32);
            this.buttonSimula.TabIndex = 1;
            this.buttonSimula.Text = "Simula";
            this.buttonSimula.UseVisualStyleBackColor = true;
            this.buttonSimula.Click += new System.EventHandler(this.buttonSimula_Click);
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(11, 321);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(427, 32);
            this.buttonReset.TabIndex = 4;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // progressBarAvanzamentoSImulazione
            // 
            this.progressBarAvanzamentoSImulazione.Location = new System.Drawing.Point(11, 218);
            this.progressBarAvanzamentoSImulazione.Maximum = 99;
            this.progressBarAvanzamentoSImulazione.Name = "progressBarAvanzamentoSImulazione";
            this.progressBarAvanzamentoSImulazione.Size = new System.Drawing.Size(430, 23);
            this.progressBarAvanzamentoSImulazione.TabIndex = 5;
            // 
            // labelTempoSimulazione
            // 
            this.labelTempoSimulazione.AutoSize = true;
            this.labelTempoSimulazione.Location = new System.Drawing.Point(50, 254);
            this.labelTempoSimulazione.Name = "labelTempoSimulazione";
            this.labelTempoSimulazione.Size = new System.Drawing.Size(0, 17);
            this.labelTempoSimulazione.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 363);
            this.Controls.Add(this.labelTempoSimulazione);
            this.Controls.Add(this.progressBarAvanzamentoSImulazione);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.buttonSimula);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Simulazione sistema a coda MM1 e MM1Y";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonOutput;
        private System.Windows.Forms.TextBox textBoxCoda;
        private System.Windows.Forms.TextBox textBoxUtenti;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogFileOutput;
        private System.Windows.Forms.Button buttonSimula;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.ProgressBar progressBarAvanzamentoSImulazione;
        private System.Windows.Forms.Label labelTempoSimulazione;
        private System.Windows.Forms.TextBox textBoxRho;
        private System.Windows.Forms.Label label4;
    }
}

