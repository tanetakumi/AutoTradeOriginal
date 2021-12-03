using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using AutoTradeOriginal.Properties;
using System.Reflection;
using CefSharp.WinForms;
using CefSharp;

namespace AutoTradeOriginal
{

    public partial class Form1 : Form
    {
        
        private CancellationTokenSource cts_loop = null;
        private CancellationTokenSource cts_restart = null;
        private CancellationTokenSource cts_history = null;
        private List<List<string>> history_list = new List<List<string>>();
        private string DemoReal = "デモ口座";
        private BrowserOperation BO;

        public ChromiumWebBrowser browser;

        //-----------------------------------------------------------
        public Form1()
        {
            InitializeComponent();
            InitializeChromium();
            Microsoft.Win32.SystemEvents.PowerModeChanged += new Microsoft.Win32.PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
            
        }

        //Chromiumの初期化
        private void InitializeChromium()
        {
            if (Cef.IsInitialized == false)
            {
                CefSettings settings = new CefSettings();
                settings.Locale = "ja";
                settings.AcceptLanguageList = "ja-JP";
                settings.LogSeverity = LogSeverity.Disable;
                settings.CefCommandLineArgs.Add("disable-gpu", "1");
                Cef.Initialize(settings);
            }
            browser = new ChromiumWebBrowser("https://trade.highlow.com/");
            tabPage_browser.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
            browser.Enabled = false;

            BO = new BrowserOperation(browser);
        }

        //電源周りの設定
        private void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            //----------------------
            // 電源状態を画面に表示
            //----------------------
            switch (e.Mode)
            {
                case Microsoft.Win32.PowerModes.Suspend:
                    add_text("PCの休止・スリープを検知しました");
                    break;
                case Microsoft.Win32.PowerModes.Resume:
                    if (button_stop.Enabled)
                    {
                        button_stop.PerformClick();
                    }
                    add_text("PCが休止・スリープされました。取引が中断されました。");
                    MessageBox.Show("PCが休止・スリープされました。システムを再起動してください。");
                    break;
            }
        }

        //ロードしたタイミング
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox_username.Text = Settings.Default.username;
            textBox_password.Text = Settings.Default.password;
            label15.Text = "バージョン:" + Assembly.GetExecutingAssembly().GetName().Version.ToString();

        }

        //閉じるとき
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("終了してもいいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }
            if (cts_restart != null)
            {
                cts_restart.Cancel();
            }
            if (cts_loop != null)
            {
                cts_loop.Cancel();
            }
            if (cts_history != null)
            {
                cts_history.Cancel();
            }

            Settings.Default.username = textBox_username.Text;
            Settings.Default.password = textBox_password.Text;
            Settings.Default.Save();
            browser.Dispose();
            Cef.Shutdown();
        }

        //閉じ切った後
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Microsoft.Win32.SystemEvents.PowerModeChanged -= new Microsoft.Win32.PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
        }


        void Enable_interface()
        {
            button_stop.Enabled = false;
            button_start.Enabled = true;
            splitContainer1.Panel1.Enabled = true;
        }


        private async void button_start_Click(object sender, EventArgs e)
        {
            button_start.Enabled = false;
            button_stop.Enabled = false;
            splitContainer1.Panel1.Enabled = false;


            //デモ口座の認証確認
            if (checkBox_real.Checked)
            {
                DemoReal = "リアル口座";
            }
            if (DemoReal == "リアル口座" && !checkBox_real.Checked)
            {
                MessageBox.Show("リアル口座のログイン情報が残っています。\r\n再起動してください。");
                Enable_interface();
                return;
            }

            //①初期化
            try
            {
                tabControl1.SelectedTab = tabPage_browser;
                label13.Text = "初期化中しばらくお待ちください";
                await BO.Initialize(checkBox_real.Checked, textBox_username.Text, textBox_password.Text);
                add_text("初期化成功");
            }
            catch (Exception a)
            {
                add_text("初期化失敗 " + a.Message);
                return;
            }
            finally
            {
                tabControl1.SelectedTab = tab_setting;
                label13.Text = "";
                button_stop.Enabled = true;
            }

            //タスク①　8:05再起動(別スレッド)
            cts_restart = new CancellationTokenSource();
            var t = Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await Task.Delay(2000, cts_restart.Token);
                    DateTime dt = DateTime.Now;
                    if (dt.Hour == 22 && dt.Minute == 34 && dt.Second <= 2)
                    {
                        return;
                    }
                }
            },
            cts_restart.Token).Unwrap().ContinueWith(async o =>
            {
                cts_restart.Dispose();
                cts_restart = null;
                if (!o.IsCanceled)
                {
                    add_text("8:05 自動再起動");
                    await BO.Initialize(checkBox_real.Checked, textBox_username.Text, textBox_password.Text);
                }
            },
            TaskScheduler.FromCurrentSynchronizationContext());

            //タスク②　履歴の取得とサイトの確認
            cts_history = new CancellationTokenSource();
            var t2 = Task.Factory.StartNew(async () =>
            {
                bool pr = true;
                await Task.Delay(60000, cts_history.Token);
                while (true)
                {
                    await Task.Delay(1000, cts_history.Token);
                    DateTime dt = DateTime.Now;
                    int sec = dt.Second;
                    if (sec > 30 && !pr)
                    {
                        await insert_historylist();
                        pr = true;
                    }
                    if (sec <= 30) pr = false;
                }
            },
            cts_history.Token
            ).Unwrap().ContinueWith(o =>
            {
                Console.WriteLine("履歴の取得スレッド終了");
                add_text("履歴の取得スレッド終了");
                cts_history.Dispose();
                cts_history = null;

            },
            TaskScheduler.FromCurrentSynchronizationContext());


            //②投資ループ
            try
            {
                cts_loop = new CancellationTokenSource();
                await InfiniteLoopAsync(cts_loop.Token);
            }
            catch
            {
                //キャンセルされたら、つまりCancellationTokenのCancelメソッドが実行されたら、
                //発生するOperationCanceledExceptionをキャッチしてキャンセル処理を行うコードです。
                cts_loop.Dispose();
                cts_loop = null;
                if (cts_history != null)
                {
                    cts_history.Cancel();
                }
                if (cts_restart != null)
                {
                    cts_restart.Cancel();
                }
                Enable_interface();
                add_text("ｷｬﾝｾﾙ");
            }
        }


        private async Task insert_historylist()
        {
            if (BO.CheckExcuteJavascript())
            {
                string text = await BO.get_history();
                if (text == "" || text == null) return;
                string[] data = text.Remove(text.Length - 1, 1).Split('*');
                string[][] history = new string[data.Length][];
                for (int i = data.Length - 1; i >= 0; i--)
                {
                    history[i] = data[i].Remove(data[i].Length - 1, 1).Split('#');
                    if (history[i][5] == "取引終了")
                    {
                        if (history[i][1].IndexOf("PutMarker") > 0) history[i][1] = "Low";
                        else if (history[i][1].IndexOf("CallMarker") > 0) history[i][1] = "High";
                        List<string> hList = new List<string>(history[i]);
                        hList.Insert(0, DemoReal);
                        bool IsExist = false;
                        for (int j = 0; j < history_list.Count; j++)
                        {
                            if (hList.SequenceEqual(history_list[j]))
                            {
                                IsExist = true;
                            }
                        }
                        if (!IsExist)
                        {
                            Invoke(new Action<List<string>>((o) => listView1.Items.Insert(0, new ListViewItem(o.ToArray()))), hList);
                            history_list.Add(hList);
                        }
                    }
                }
            }
            else
            {
                Invoke(new Action(async () =>
                {
                    add_text("通信エラーのため自動再起動");
                    await BO.Initialize(checkBox_real.Checked, textBox_username.Text, textBox_password.Text);
                }));
            }

        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            if (cts_restart != null)
            {
                cts_restart.Cancel();
            }
            if (cts_loop != null)
            {
                cts_loop.Cancel();
            }
            if (cts_history != null)
            {
                cts_history.Cancel();
            }
            Enable_interface();
        }

        private async Task InfiniteLoopAsync(CancellationToken ct)
        {
            while (true)
            {
                
                //Message:  0-通貨# 1-HighLow# 2-金額倍率
                //①メッセージの受け取り
                add_text("待機します");
                string message = await WaitForNamedpipe("highlowpipe", ct);
                add_text("シグナルを受け取りました");

                //②投資
                try
                {
                    (string currency, string high_low, int price, int gametab, int period, int rank) tags = MessageToTuple(message);
                    int repeat = Decimal.ToInt32(numericUpDown_retry.Value);
                    int retry_milsec = Decimal.ToInt32(numericUpDown_mil.Value);
                    string result = await BO.InvestHighLow(tags.currency, tags.high_low, tags.price, repeat, retry_milsec, tags.gametab, tags.period, tags.rank);
                    add_text(result);
                }
                catch (Exception ex)
                {
                    add_text(ex.Message);
                }

            }
        }


        private void add_text(string text)
        {
            DateTime dt = DateTime.Now;
            listBox_log.Items.Add(dt.ToString("yyyy/MM/dd HH:mm:ss") + "   " + text);
            if (listBox_log.Items.Count > 50)
            {
                for (int i = 0; i < listBox_log.Items.Count - 50; i++)
                {
                    listBox_log.Items.RemoveAt(i);
                }
            }
        }

        

        private async void button_pagedown_Click(object sender, EventArgs e)
        {
            await BO.Scroll(0);
        }

        private async void button_pageup_Click(object sender, EventArgs e)
        {
            await BO.Scroll(1);
        }
    }
}
