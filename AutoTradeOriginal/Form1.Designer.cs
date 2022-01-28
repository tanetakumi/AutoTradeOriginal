namespace AutoTradeOriginal
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button_start = new System.Windows.Forms.Button();
            this.button_stop = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tab_setting = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBox_real = new System.Windows.Forms.CheckBox();
            this.textBox_username = new System.Windows.Forms.TextBox();
            this.textBox_password = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox_log = new System.Windows.Forms.ListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.textBox_read = new System.Windows.Forms.TextBox();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.tabControl1.SuspendLayout();
            this.tab_setting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_start
            // 
            this.button_start.Location = new System.Drawing.Point(3, 98);
            this.button_start.Margin = new System.Windows.Forms.Padding(2);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(96, 32);
            this.button_start.TabIndex = 0;
            this.button_start.Text = "スタート";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button_start_Click);
            // 
            // button_stop
            // 
            this.button_stop.Enabled = false;
            this.button_stop.Location = new System.Drawing.Point(103, 98);
            this.button_stop.Margin = new System.Windows.Forms.Padding(2);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new System.Drawing.Size(96, 32);
            this.button_stop.TabIndex = 0;
            this.button_stop.Text = "ストップ";
            this.button_stop.UseVisualStyleBackColor = true;
            this.button_stop.Click += new System.EventHandler(this.button_stop_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tab_setting);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1168, 672);
            this.tabControl1.TabIndex = 16;
            // 
            // tab_setting
            // 
            this.tab_setting.Controls.Add(this.splitContainer1);
            this.tab_setting.Location = new System.Drawing.Point(4, 22);
            this.tab_setting.Name = "tab_setting";
            this.tab_setting.Padding = new System.Windows.Forms.Padding(3);
            this.tab_setting.Size = new System.Drawing.Size(1160, 646);
            this.tab_setting.TabIndex = 0;
            this.tab_setting.Text = "設定";
            this.tab_setting.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(1154, 640);
            this.splitContainer1.SplitterDistance = 887;
            this.splitContainer1.TabIndex = 19;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.checkBox_real);
            this.groupBox5.Controls.Add(this.textBox_username);
            this.groupBox5.Controls.Add(this.textBox_password);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Location = new System.Drawing.Point(3, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(203, 90);
            this.groupBox5.TabIndex = 17;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "ユーザー設定";
            // 
            // checkBox_real
            // 
            this.checkBox_real.AutoSize = true;
            this.checkBox_real.Location = new System.Drawing.Point(11, 19);
            this.checkBox_real.Name = "checkBox_real";
            this.checkBox_real.Size = new System.Drawing.Size(108, 16);
            this.checkBox_real.TabIndex = 15;
            this.checkBox_real.Text = "リアル口座の使用";
            this.checkBox_real.UseVisualStyleBackColor = true;
            // 
            // textBox_username
            // 
            this.textBox_username.Location = new System.Drawing.Point(80, 40);
            this.textBox_username.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_username.Name = "textBox_username";
            this.textBox_username.Size = new System.Drawing.Size(113, 19);
            this.textBox_username.TabIndex = 4;
            // 
            // textBox_password
            // 
            this.textBox_password.Location = new System.Drawing.Point(80, 63);
            this.textBox_password.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_password.Name = "textBox_password";
            this.textBox_password.Size = new System.Drawing.Size(113, 19);
            this.textBox_password.TabIndex = 4;
            this.textBox_password.UseSystemPasswordChar = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(5, 63);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "パスワード";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(6, 41);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "ユーザー名";
            // 
            // listBox_log
            // 
            this.listBox_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_log.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.listBox_log.FormattingEnabled = true;
            this.listBox_log.ItemHeight = 12;
            this.listBox_log.Location = new System.Drawing.Point(0, 0);
            this.listBox_log.Name = "listBox_log";
            this.listBox_log.Size = new System.Drawing.Size(263, 500);
            this.listBox_log.TabIndex = 12;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.textBox_read);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1160, 646);
            this.tabPage2.TabIndex = 3;
            this.tabPage2.Text = "注意事項";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // textBox_read
            // 
            this.textBox_read.Location = new System.Drawing.Point(20, 19);
            this.textBox_read.Multiline = true;
            this.textBox_read.Name = "textBox_read";
            this.textBox_read.Size = new System.Drawing.Size(886, 520);
            this.textBox_read.TabIndex = 0;
            this.textBox_read.Text = "あいうえお\r\n";
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.groupBox5);
            this.splitContainer3.Panel1.Controls.Add(this.button_stop);
            this.splitContainer3.Panel1.Controls.Add(this.button_start);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.listBox_log);
            this.splitContainer3.Size = new System.Drawing.Size(263, 640);
            this.splitContainer3.SplitterDistance = 136;
            this.splitContainer3.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1168, 672);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tab_setting.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button_stop;
        private System.Windows.Forms.Button button_start;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tab_setting;
        private System.Windows.Forms.ListBox listBox_log;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox checkBox_real;
        private System.Windows.Forms.TextBox textBox_username;
        private System.Windows.Forms.TextBox textBox_password;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox textBox_read;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer3;
    }
}

