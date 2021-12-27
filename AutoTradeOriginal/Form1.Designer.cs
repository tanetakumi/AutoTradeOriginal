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
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBox_real = new System.Windows.Forms.CheckBox();
            this.textBox_username = new System.Windows.Forms.TextBox();
            this.textBox_password = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDown_retry = new System.Windows.Forms.NumericUpDown();
            this.button_pagedown = new System.Windows.Forms.Button();
            this.button_pageup = new System.Windows.Forms.Button();
            this.listBox_log = new System.Windows.Forms.ListBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tab_history = new System.Windows.Forms.TabPage();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.textBox_read = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tab_setting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_retry)).BeginInit();
            this.tab_history.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_start
            // 
            this.button_start.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_start.Location = new System.Drawing.Point(51, 604);
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
            this.button_stop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_stop.Enabled = false;
            this.button_stop.Location = new System.Drawing.Point(151, 604);
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
            this.tabControl1.Controls.Add(this.tab_history);
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
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listBox_log);
            this.splitContainer1.Panel2.Controls.Add(this.button_stop);
            this.splitContainer1.Panel2.Controls.Add(this.button_start);
            this.splitContainer1.Panel2.Controls.Add(this.label12);
            this.splitContainer1.Size = new System.Drawing.Size(1154, 640);
            this.splitContainer1.SplitterDistance = 888;
            this.splitContainer1.TabIndex = 19;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox5);
            this.splitContainer2.Panel1.Controls.Add(this.label15);
            this.splitContainer2.Panel1.Controls.Add(this.groupBox4);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.button_pagedown);
            this.splitContainer2.Panel2.Controls.Add(this.button_pageup);
            this.splitContainer2.Size = new System.Drawing.Size(888, 640);
            this.splitContainer2.SplitterDistance = 96;
            this.splitContainer2.TabIndex = 19;
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
            this.groupBox5.Size = new System.Drawing.Size(332, 90);
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
            this.textBox_username.Location = new System.Drawing.Point(109, 36);
            this.textBox_username.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_username.Name = "textBox_username";
            this.textBox_username.Size = new System.Drawing.Size(212, 19);
            this.textBox_username.TabIndex = 4;
            // 
            // textBox_password
            // 
            this.textBox_password.Location = new System.Drawing.Point(109, 57);
            this.textBox_password.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_password.Name = "textBox_password";
            this.textBox_password.Size = new System.Drawing.Size(212, 19);
            this.textBox_password.TabIndex = 4;
            this.textBox_password.UseSystemPasswordChar = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(22, 59);
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
            this.label1.Location = new System.Drawing.Point(22, 38);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "ユーザー名";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(574, 81);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(50, 12);
            this.label15.TabIndex = 18;
            this.label15.Text = "バージョン";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.numericUpDown_retry);
            this.groupBox4.Location = new System.Drawing.Point(341, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(227, 90);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "取引基本設定";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(23, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 12);
            this.label6.TabIndex = 14;
            this.label6.Text = "購入リトライ";
            // 
            // numericUpDown_retry
            // 
            this.numericUpDown_retry.Location = new System.Drawing.Point(90, 34);
            this.numericUpDown_retry.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown_retry.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_retry.Name = "numericUpDown_retry";
            this.numericUpDown_retry.Size = new System.Drawing.Size(61, 19);
            this.numericUpDown_retry.TabIndex = 13;
            this.numericUpDown_retry.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // button_pagedown
            // 
            this.button_pagedown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_pagedown.Font = new System.Drawing.Font("MS UI Gothic", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button_pagedown.Location = new System.Drawing.Point(863, 109);
            this.button_pagedown.Name = "button_pagedown";
            this.button_pagedown.Size = new System.Drawing.Size(22, 65);
            this.button_pagedown.TabIndex = 1;
            this.button_pagedown.Text = "⇩";
            this.button_pagedown.UseVisualStyleBackColor = true;
            this.button_pagedown.Click += new System.EventHandler(this.button_pagedown_Click);
            // 
            // button_pageup
            // 
            this.button_pageup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_pageup.Font = new System.Drawing.Font("MS UI Gothic", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button_pageup.Location = new System.Drawing.Point(863, 38);
            this.button_pageup.Name = "button_pageup";
            this.button_pageup.Size = new System.Drawing.Size(22, 65);
            this.button_pageup.TabIndex = 1;
            this.button_pageup.Text = "⇧";
            this.button_pageup.UseVisualStyleBackColor = true;
            this.button_pageup.Click += new System.EventHandler(this.button_pageup_Click);
            // 
            // listBox_log
            // 
            this.listBox_log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox_log.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.listBox_log.FormattingEnabled = true;
            this.listBox_log.ItemHeight = 12;
            this.listBox_log.Location = new System.Drawing.Point(3, 34);
            this.listBox_log.Name = "listBox_log";
            this.listBox_log.Size = new System.Drawing.Size(256, 544);
            this.listBox_log.TabIndex = 12;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(21, 15);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(47, 12);
            this.label12.TabIndex = 18;
            this.label12.Text = "操作ログ";
            // 
            // tab_history
            // 
            this.tab_history.Controls.Add(this.listView1);
            this.tab_history.Location = new System.Drawing.Point(4, 22);
            this.tab_history.Name = "tab_history";
            this.tab_history.Padding = new System.Windows.Forms.Padding(3);
            this.tab_history.Size = new System.Drawing.Size(1160, 646);
            this.tab_history.TabIndex = 1;
            this.tab_history.Text = "取引履歴";
            this.tab_history.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10,
            this.columnHeader1,
            this.columnHeader11,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(3, 3);
            this.listView1.Margin = new System.Windows.Forms.Padding(2);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1154, 640);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "口座";
            this.columnHeader10.Width = 100;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "取引原資産";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "エントリー方向";
            this.columnHeader11.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "取引内容";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "取引時間";
            this.columnHeader3.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "判定時刻";
            this.columnHeader4.Width = 100;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "ステータス";
            this.columnHeader5.Width = 100;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "判定レート";
            this.columnHeader6.Width = 100;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "購入";
            this.columnHeader7.Width = 100;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "判定時ペイアウト";
            this.columnHeader8.Width = 100;
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
            this.textBox_read.Text = resources.GetString("textBox_read.Text");
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
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_retry)).EndInit();
            this.tab_history.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button_stop;
        private System.Windows.Forms.Button button_start;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tab_setting;
        private System.Windows.Forms.ListBox listBox_log;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDown_retry;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox checkBox_real;
        private System.Windows.Forms.TextBox textBox_username;
        private System.Windows.Forms.TextBox textBox_password;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tab_history;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBox_read;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button button_pageup;
        private System.Windows.Forms.Button button_pagedown;
        private System.Windows.Forms.SplitContainer splitContainer2;
    }
}

