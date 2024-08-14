namespace DDoS_Autofix
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            lbAddress = new Label();
            lbAccount = new Label();
            tbAddress = new TextBox();
            tbAccount = new TextBox();
            tbPassword = new TextBox();
            lbPassword = new Label();
            lbPort = new Label();
            nudPort = new NumericUpDown();
            btnConnect = new Button();
            btnApply = new Button();
            btnRemove = new Button();
            btnDisconnect = new Button();
            tbLog = new TextBox();
            progressBar = new ProgressBar();
            ((System.ComponentModel.ISupportInitialize)nudPort).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            lbAddress.AutoSize = true;
            lbAddress.Location = new Point(12, 15);
            lbAddress.Name = "label1";
            lbAddress.Size = new Size(122, 15);
            lbAddress.TabIndex = 999;
            lbAddress.Text = "Address (IP or FQDN):";
            // 
            // label2
            // 
            lbAccount.AutoSize = true;
            lbAccount.Location = new Point(79, 70);
            lbAccount.Name = "label2";
            lbAccount.Size = new Size(55, 15);
            lbAccount.TabIndex = 999;
            lbAccount.Text = "Account:";
            // 
            // textBox1
            // 
            tbAddress.Location = new Point(140, 12);
            tbAddress.Name = "textBox1";
            tbAddress.Size = new Size(134, 23);
            tbAddress.TabIndex = 1;
            tbAddress.Enter += SelectAllText;
            // 
            // textBox2
            // 
            tbAccount.Location = new Point(140, 70);
            tbAccount.Name = "textBox2";
            tbAccount.Size = new Size(134, 23);
            tbAccount.TabIndex = 3;
            tbAccount.Enter += SelectAllText;
            // 
            // textBox3
            // 
            tbPassword.Location = new Point(140, 99);
            tbPassword.Name = "textBox3";
            tbPassword.Size = new Size(134, 23);
            tbPassword.TabIndex = 4;
            tbPassword.UseSystemPasswordChar = true;
            tbPassword.Enter += SelectAllText;
            // 
            // label3
            // 
            lbPassword.AutoSize = true;
            lbPassword.Location = new Point(74, 99);
            lbPassword.Name = "label3";
            lbPassword.Size = new Size(60, 15);
            lbPassword.TabIndex = 999;
            lbPassword.Text = "Password:";
            // 
            // label4
            // 
            lbPort.AutoSize = true;
            lbPort.Location = new Point(102, 44);
            lbPort.Name = "label4";
            lbPort.Size = new Size(32, 15);
            lbPort.TabIndex = 999;
            lbPort.Text = "Port:";
            // 
            // numericUpDown1
            // 
            nudPort.Location = new Point(140, 41);
            nudPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            nudPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudPort.Name = "numericUpDown1";
            nudPort.Size = new Size(65, 23);
            nudPort.TabIndex = 2;
            nudPort.Value = new decimal(new int[] { 22, 0, 0, 0 });
            nudPort.Enter += SelectAllText;
            // 
            // button1
            // 
            btnConnect.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnConnect.Location = new Point(12, 146);
            btnConnect.Name = "button1";
            btnConnect.Size = new Size(128, 23);
            btnConnect.TabIndex = 1000;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += BtnConnect_Click;
            // 
            // button2
            // 
            btnApply.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnApply.Enabled = false;
            btnApply.Location = new Point(12, 175);
            btnApply.Name = "button2";
            btnApply.Size = new Size(128, 23);
            btnApply.TabIndex = 1001;
            btnApply.Text = "Apply/renew policies";
            btnApply.UseVisualStyleBackColor = true;
            btnApply.Click += BtnApplyPolicies_Click;
            // 
            // button3
            // 
            btnRemove.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnRemove.Enabled = false;
            btnRemove.Location = new Point(146, 175);
            btnRemove.Name = "button3";
            btnRemove.Size = new Size(128, 23);
            btnRemove.TabIndex = 1002;
            btnRemove.Text = "Remove policies";
            btnRemove.UseVisualStyleBackColor = true;
            btnRemove.Click += BtnRemovePolicies_Click;
            // 
            // button4
            // 
            btnDisconnect.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnDisconnect.Enabled = false;
            btnDisconnect.Location = new Point(146, 146);
            btnDisconnect.Name = "button4";
            btnDisconnect.Size = new Size(128, 23);
            btnDisconnect.TabIndex = 1003;
            btnDisconnect.Text = "Disconnect";
            btnDisconnect.UseVisualStyleBackColor = true;
            btnDisconnect.Click += BtnDisconnect_Click;
            // 
            // textBox4
            // 
            tbLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            tbLog.BackColor = SystemColors.ControlLightLight;
            tbLog.Location = new Point(280, 12);
            tbLog.Multiline = true;
            tbLog.Name = "textBox4";
            tbLog.ReadOnly = true;
            tbLog.ScrollBars = ScrollBars.Vertical;
            tbLog.Size = new Size(508, 187);
            tbLog.TabIndex = 1004;
            // 
            // progressBar1
            // 
            progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            progressBar.Location = new Point(12, 130);
            progressBar.Name = "progressBar1";
            progressBar.Size = new Size(262, 10);
            progressBar.TabIndex = 1005;
            progressBar.Visible = false;
            // 
            // Form1
            // 
            AcceptButton = btnConnect;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 215);
            Controls.Add(progressBar);
            Controls.Add(tbLog);
            Controls.Add(btnDisconnect);
            Controls.Add(btnRemove);
            Controls.Add(btnApply);
            Controls.Add(btnConnect);
            Controls.Add(nudPort);
            Controls.Add(lbPort);
            Controls.Add(lbPassword);
            Controls.Add(tbPassword);
            Controls.Add(tbAccount);
            Controls.Add(tbAddress);
            Controls.Add(lbAccount);
            Controls.Add(lbAddress);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DDoS attackers autoblocker for ZLD & uOS appliances";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)nudPort).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lbAddress;
        private Label lbAccount;
        private TextBox tbAddress;
        private TextBox tbAccount;
        private TextBox tbPassword;
        private Label lbPassword;
        private Label lbPort;
        private NumericUpDown nudPort;
        private Button btnConnect;
        private Button btnApply;
        private Button btnRemove;
        private Button btnDisconnect;
        private TextBox tbLog;
        private ProgressBar progressBar;
    }
}