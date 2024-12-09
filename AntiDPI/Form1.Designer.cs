namespace AntiDPI
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
			components = new System.ComponentModel.Container();
			button1 = new Button();
			textBox1 = new TextBox();
			numericUpDown1 = new NumericUpDown();
			checkBox1 = new CheckBox();
			button2 = new Button();
			timer1 = new System.Windows.Forms.Timer(components);
			numericUpDown2 = new NumericUpDown();
			textBox2 = new TextBox();
			richTextBox1 = new RichTextBox();
			checkBox2 = new CheckBox();
			checkBox3 = new CheckBox();
			richTextBox2 = new RichTextBox();
			button3 = new Button();
			((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
			((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
			SuspendLayout();
			// 
			// button1
			// 
			button1.Font = new Font("Segoe UI", 20F);
			button1.Location = new Point(12, 544);
			button1.Name = "button1";
			button1.Size = new Size(611, 59);
			button1.TabIndex = 0;
			button1.Text = "Start Web Proxy";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// textBox1
			// 
			textBox1.Font = new Font("Segoe UI", 20F);
			textBox1.Location = new Point(12, 477);
			textBox1.Name = "textBox1";
			textBox1.Size = new Size(354, 61);
			textBox1.TabIndex = 1;
			textBox1.TextChanged += textBox1_TextChanged;
			// 
			// numericUpDown1
			// 
			numericUpDown1.Font = new Font("Segoe UI", 20F);
			numericUpDown1.Location = new Point(372, 477);
			numericUpDown1.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
			numericUpDown1.Name = "numericUpDown1";
			numericUpDown1.Size = new Size(251, 61);
			numericUpDown1.TabIndex = 2;
			numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
			// 
			// checkBox1
			// 
			checkBox1.AutoSize = true;
			checkBox1.Location = new Point(428, 12);
			checkBox1.Name = "checkBox1";
			checkBox1.Size = new Size(195, 29);
			checkBox1.TabIndex = 3;
			checkBox1.Text = "Auto Proxy Settings";
			checkBox1.UseVisualStyleBackColor = true;
			checkBox1.CheckedChanged += checkBox1_CheckedChanged;
			// 
			// button2
			// 
			button2.Font = new Font("Segoe UI", 20F);
			button2.Location = new Point(12, 335);
			button2.Name = "button2";
			button2.Size = new Size(611, 59);
			button2.TabIndex = 4;
			button2.Text = "Start Dns Proxy";
			button2.UseVisualStyleBackColor = true;
			button2.Click += button2_Click;
			// 
			// timer1
			// 
			timer1.Enabled = true;
			timer1.Tick += timer1_Tick;
			// 
			// numericUpDown2
			// 
			numericUpDown2.Font = new Font("Segoe UI", 20F);
			numericUpDown2.Location = new Point(372, 268);
			numericUpDown2.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
			numericUpDown2.Name = "numericUpDown2";
			numericUpDown2.Size = new Size(251, 61);
			numericUpDown2.TabIndex = 6;
			numericUpDown2.ValueChanged += numericUpDown2_ValueChanged;
			// 
			// textBox2
			// 
			textBox2.Font = new Font("Segoe UI", 20F);
			textBox2.Location = new Point(12, 268);
			textBox2.Name = "textBox2";
			textBox2.Size = new Size(354, 61);
			textBox2.TabIndex = 5;
			textBox2.TextChanged += textBox2_TextChanged;
			// 
			// richTextBox1
			// 
			richTextBox1.Location = new Point(12, 152);
			richTextBox1.Name = "richTextBox1";
			richTextBox1.Size = new Size(354, 110);
			richTextBox1.TabIndex = 7;
			richTextBox1.Text = "";
			richTextBox1.TextChanged += richTextBox1_TextChanged;
			// 
			// checkBox2
			// 
			checkBox2.AutoSize = true;
			checkBox2.Location = new Point(372, 233);
			checkBox2.Name = "checkBox2";
			checkBox2.Size = new Size(217, 29);
			checkBox2.TabIndex = 8;
			checkBox2.Text = "Randomize HostName";
			checkBox2.UseVisualStyleBackColor = true;
			// 
			// checkBox3
			// 
			checkBox3.AutoSize = true;
			checkBox3.Location = new Point(372, 442);
			checkBox3.Name = "checkBox3";
			checkBox3.Size = new Size(224, 29);
			checkBox3.TabIndex = 9;
			checkBox3.Text = "Use Dns Proxy as a Dns";
			checkBox3.UseVisualStyleBackColor = true;
			checkBox3.CheckedChanged += checkBox3_CheckedChanged;
			// 
			// richTextBox2
			// 
			richTextBox2.Location = new Point(12, 400);
			richTextBox2.Name = "richTextBox2";
			richTextBox2.Size = new Size(354, 71);
			richTextBox2.TabIndex = 10;
			richTextBox2.Text = "";
			// 
			// button3
			// 
			button3.Font = new Font("Segoe UI", 20F);
			button3.Location = new Point(12, 12);
			button3.Name = "button3";
			button3.Size = new Size(395, 59);
			button3.TabIndex = 11;
			button3.Text = "Clear Logs";
			button3.UseVisualStyleBackColor = true;
			button3.Click += button3_Click;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(10F, 25F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(635, 615);
			Controls.Add(button3);
			Controls.Add(richTextBox2);
			Controls.Add(checkBox3);
			Controls.Add(checkBox2);
			Controls.Add(richTextBox1);
			Controls.Add(numericUpDown2);
			Controls.Add(textBox2);
			Controls.Add(button2);
			Controls.Add(checkBox1);
			Controls.Add(numericUpDown1);
			Controls.Add(textBox1);
			Controls.Add(button1);
			Name = "Form1";
			Text = "Form1";
			Load += Form1_Load;
			((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
			((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Button button1;
		private TextBox textBox1;
		private NumericUpDown numericUpDown1;
		private CheckBox checkBox1;
		private Button button2;
		private System.Windows.Forms.Timer timer1;
		private NumericUpDown numericUpDown2;
		private TextBox textBox2;
		private RichTextBox richTextBox1;
		private CheckBox checkBox2;
		private CheckBox checkBox3;
		private RichTextBox richTextBox2;
		private Button button3;
	}
}
