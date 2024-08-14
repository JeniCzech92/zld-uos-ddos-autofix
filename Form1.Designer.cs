namespace DDoS_Autofix
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            label1 = new Label();
            label2 = new Label();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            label3 = new Label();
            label4 = new Label();
            numericUpDown1 = new NumericUpDown();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            textBox4 = new TextBox();
            progressBar1 = new ProgressBar();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(122, 15);
            label1.TabIndex = 999;
            label1.Text = "Address (IP or FQDN):";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(79, 70);
            label2.Name = "label2";
            label2.Size = new Size(55, 15);
            label2.TabIndex = 999;
            label2.Text = "Account:";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(140, 12);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(134, 23);
            textBox1.TabIndex = 1;
            textBox1.Enter += SelectAllText;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(140, 70);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(134, 23);
            textBox2.TabIndex = 3;
            textBox2.Enter += SelectAllText;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(140, 99);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(134, 23);
            textBox3.TabIndex = 4;
            textBox3.UseSystemPasswordChar = true;
            textBox3.Enter += SelectAllText;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(74, 99);
            label3.Name = "label3";
            label3.Size = new Size(60, 15);
            label3.TabIndex = 999;
            label3.Text = "Password:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(102, 44);
            label4.Name = "label4";
            label4.Size = new Size(32, 15);
            label4.TabIndex = 999;
            label4.Text = "Port:";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(140, 41);
            numericUpDown1.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(65, 23);
            numericUpDown1.TabIndex = 2;
            numericUpDown1.Value = new decimal(new int[] { 22, 0, 0, 0 });
            numericUpDown1.Enter += SelectAllText;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button1.Location = new Point(12, 146);
            button1.Name = "button1";
            button1.Size = new Size(128, 23);
            button1.TabIndex = 1000;
            button1.Text = "Connect";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button2.Enabled = false;
            button2.Location = new Point(12, 175);
            button2.Name = "button2";
            button2.Size = new Size(128, 23);
            button2.TabIndex = 1001;
            button2.Text = "Apply/renew policies";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button3.Enabled = false;
            button3.Location = new Point(146, 175);
            button3.Name = "button3";
            button3.Size = new Size(128, 23);
            button3.TabIndex = 1002;
            button3.Text = "Remove policies";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button4.Enabled = false;
            button4.Location = new Point(146, 146);
            button4.Name = "button4";
            button4.Size = new Size(128, 23);
            button4.TabIndex = 1003;
            button4.Text = "Disconnect";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // textBox4
            // 
            textBox4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            textBox4.BackColor = SystemColors.ControlLightLight;
            textBox4.Location = new Point(280, 12);
            textBox4.Multiline = true;
            textBox4.Name = "textBox4";
            textBox4.ReadOnly = true;
            textBox4.ScrollBars = ScrollBars.Vertical;
            textBox4.Size = new Size(508, 187);
            textBox4.TabIndex = 1004;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            progressBar1.Location = new Point(12, 130);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(262, 10);
            progressBar1.TabIndex = 1005;
            progressBar1.Visible = false;
            // 
            // Form1
            // 
            AcceptButton = button1;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 215);
            Controls.Add(progressBar1);
            Controls.Add(textBox4);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(numericUpDown1);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DDoS attackers autoblocker for ZLD & uOS appliances";
            FormClosing += Form1_FormClosing;
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;
        private Label label3;
        private Label label4;
        private NumericUpDown numericUpDown1;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private TextBox textBox4;
        private ProgressBar progressBar1;
    }
}