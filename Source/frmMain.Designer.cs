namespace DatabaseCompare
{
    partial class frmDatabaseCompare
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtSourceServer = new System.Windows.Forms.TextBox();
            this.txtSourceDatabase = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTargetDatabase = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTargetServer = new System.Windows.Forms.TextBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.cboJobType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 17);
            this.label1.TabIndex = 11;
            this.label1.Text = "Source Server";
            // 
            // txtSourceServer
            // 
            this.txtSourceServer.Location = new System.Drawing.Point(148, 12);
            this.txtSourceServer.Name = "txtSourceServer";
            this.txtSourceServer.Size = new System.Drawing.Size(276, 22);
            this.txtSourceServer.TabIndex = 1;
            // 
            // txtSourceDatabase
            // 
            this.txtSourceDatabase.Location = new System.Drawing.Point(148, 40);
            this.txtSourceDatabase.Name = "txtSourceDatabase";
            this.txtSourceDatabase.Size = new System.Drawing.Size(276, 22);
            this.txtSourceDatabase.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 17);
            this.label2.TabIndex = 14;
            this.label2.Text = "Source Database";
            // 
            // txtTargetDatabase
            // 
            this.txtTargetDatabase.Location = new System.Drawing.Point(148, 96);
            this.txtTargetDatabase.Name = "txtTargetDatabase";
            this.txtTargetDatabase.Size = new System.Drawing.Size(276, 22);
            this.txtTargetDatabase.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 17);
            this.label3.TabIndex = 18;
            this.label3.Text = "Target Database";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 17);
            this.label4.TabIndex = 15;
            this.label4.Text = "Target Server";
            // 
            // txtTargetServer
            // 
            this.txtTargetServer.Location = new System.Drawing.Point(148, 68);
            this.txtTargetServer.Name = "txtTargetServer";
            this.txtTargetServer.Size = new System.Drawing.Size(276, 22);
            this.txtTargetServer.TabIndex = 3;
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(148, 154);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(75, 23);
            this.btnGo.TabIndex = 6;
            this.btnGo.Text = "&Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtResults
            // 
            this.txtResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResults.Location = new System.Drawing.Point(148, 186);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResults.Size = new System.Drawing.Size(493, 261);
            this.txtResults.TabIndex = 0;
            this.txtResults.TabStop = false;
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopy.Location = new System.Drawing.Point(229, 468);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 23);
            this.btnCopy.TabIndex = 8;
            this.btnCopy.Text = "&Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Location = new System.Drawing.Point(148, 468);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(26, 124);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 17);
            this.label5.TabIndex = 20;
            this.label5.Text = "Job Type";
            // 
            // cboJobType
            // 
            this.cboJobType.FormattingEnabled = true;
            this.cboJobType.Location = new System.Drawing.Point(148, 124);
            this.cboJobType.Name = "cboJobType";
            this.cboJobType.Size = new System.Drawing.Size(276, 24);
            this.cboJobType.TabIndex = 5;
            // 
            // frmDatabaseCompare
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 503);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cboJobType);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtResults);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.txtTargetDatabase);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtTargetServer);
            this.Controls.Add(this.txtSourceDatabase);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSourceServer);
            this.Name = "frmDatabaseCompare";
            this.ShowIcon = false;
            this.Text = "Database Compare";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSourceServer;
        private System.Windows.Forms.TextBox txtSourceDatabase;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTargetDatabase;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtTargetServer;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboJobType;
    }
}

