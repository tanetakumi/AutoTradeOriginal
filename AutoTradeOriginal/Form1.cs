using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO.Pipes;
using System.IO;
using System.Text.RegularExpressions;
//using AutoTradeOriginal;
using AutoTradeOriginal.Properties;
using ClosedXML.Excel;
using System.Reflection;

namespace AutoTradeOriginal
{

    public partial class Form1 : Form
    {
        
        private CancellationTokenSource cts_loop = null;
        private CancellationTokenSource cts_restart = null;
        private CancellationTokenSource cts_history = null;
        private List<List<string>> history_list = new List<List<string>>();
        private string DemoReal = "デモ口座";
        private Browser BO;
        private Auth auth;

        public class Tab
        {
            public int Gametab { get; set; }
            public int Period { get; set; }
            public int Rank { get; set; }
        }
        private Tab tab = new Tab();

        //-----------------------------------------------------------
        public Form1()
        {
            InitializeComponent();
            Microsoft.Win32.SystemEvents.PowerModeChanged += new Microsoft.Win32.PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
            InitializeChromium();
        }

        private void InitializeChromium()
        {
            BO = new Browser();
            tabPage_browser.Controls.Add(BO.browser);
            BO.browser.Dock = DockStyle.Fill;
            BO.browser.Enabled = false;
        }


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


        private void Form1_Load(object sender, EventArgs e)
        {
            textBox_username.Text = Settings.Default.username;
            textBox_password.Text = Settings.Default.password;
            UpLimit.Value = Settings.Default.uplimit;
            DownLimit.Value = Settings.Default.downlimit;
            string ver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            label15.Text = "バージョン:" + ver;

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("エクスポートはしましたか？\n\r終了してもいいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
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
            Settings.Default.uplimit = (int)UpLimit.Value;
            Settings.Default.downlimit = (int)DownLimit.Value;
            Settings.Default.Save();
            BO.BrowserShutdown();
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
            //--------------------------------------------------------------------------------------------------------------

            //選択
            Setperiod();
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
            catch (OperationCanceledException)
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
                Func<Task<string>> pre = BO.get_history;
                string text = await (Task<string>)Invoke(pre);
                if (text == "") return;
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
                string Message = "";
                using (NamedPipeServerStream npss = new NamedPipeServerStream("HighLowPipe", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
                {
                    Invoke(new Action(() => add_text("待機中です")));
                    await npss.WaitForConnectionAsync(ct);
                    using (StreamReader reader = new StreamReader(npss))
                    {
                        Message = reader.ReadLine();
                        Invoke(new Action(() => add_text("シグナルを受け取りました")));
                    }
                }
                //③口座残高確認
                if (checkBox_uplimit.Checked || checkBox_downlimit.Checked)
                {
                    int uplimit = Decimal.ToInt32(UpLimit.Value);
                    int downlimit = Decimal.ToInt32(DownLimit.Value);
                    bool InLimitResult = await BO.IsAmountInLimits(checkBox_uplimit.Checked, checkBox_downlimit.Checked, uplimit, downlimit);
                    if (!InLimitResult)
                    {
                        add_text("口座残高の制限に達しました。");
                        throw new OperationCanceledException();
                    }
                }
                //④投資
                string result = "";
                string[] mes = Message.Split('#');
                try
                {
                    int mag = int.Parse(mes[2]);
                    int amount = mag*Decimal.ToInt32(numericUpDown_amount.Value);
                    int repeat = Decimal.ToInt32(numericUpDown_retry.Value);
                    int retry_milsec = Decimal.ToInt32(numericUpDown_mil.Value);
                    result = await BO.InvestHighLow(mes[0], mes[1], amount, repeat, retry_milsec, tab.Gametab, tab.Period, tab.Rank);
                    add_text(result);
                }
                catch (Exception ex)
                {
                    add_text(ex.Message);
                }

            }
        }


        private void checkBox_downlimit_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_downlimit.Checked)
            {
                DownLimit.Enabled = true;
            }
            else
            {
                DownLimit.Enabled = false;
            }

        }
        private void checkBox_uplimit_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_uplimit.Checked)
            {
                UpLimit.Enabled = true;
            }
            else
            {
                UpLimit.Enabled = false;
            }
        }

        private static async Task sendMessage(string message, string token)
        {
            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "message", message },
                });

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var result = await client.PostAsync("https://notify-api.line.me/api/notify", content);
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

        private void button_delet_Click(object sender, EventArgs e)
        {
            //項目が１つも選択されていない場合
            if (listView1.SelectedItems.Count == 0)
            {
                //処理を抜ける
                return;
            }
            // 選択されているリストを取得しリストビューから削除する
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                listView1.Items.Remove(item);
            }
        }


        private void button_delall_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
        }
        


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Microsoft.Win32.SystemEvents.PowerModeChanged -= new Microsoft.Win32.PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
        }
        private void Setperiod()
        {
            foreach (RadioButton rb in groupBox1.Controls)
            {
                if (rb.Checked)
                {
                    switch (rb.TabIndex)
                    {
                        //HighLow
                        case 1:
                            tab.Gametab = 1;
                            tab.Period = 2;
                            tab.Rank = 1;
                            break;
                        case 2:
                            tab.Gametab = 1;
                            tab.Period = 2;
                            tab.Rank = 2;
                            break;
                        case 3:
                            tab.Gametab = 1;
                            tab.Period = 2;
                            tab.Rank = 3;
                            break;
                        //HighLowスプ
                        case 4:
                            tab.Gametab = 2;
                            tab.Period = 2;
                            tab.Rank = 1;
                            break;
                        case 5:
                            tab.Gametab = 2;
                            tab.Period = 2;
                            tab.Rank = 2;
                            break;
                        case 6:
                            tab.Gametab = 2;
                            tab.Period = 2;
                            tab.Rank = 3;
                            break;
                        //Turbo
                        case 7:
                            tab.Gametab = 3;
                            tab.Period = 3;
                            tab.Rank = 0;
                            break;
                        case 8:
                            tab.Gametab = 3;
                            tab.Period = 4;
                            tab.Rank = 0;
                            break;
                        case 9:
                            tab.Gametab = 3;
                            tab.Period = 5;
                            tab.Rank = 0;
                            break;
                        //Turboスプ
                        case 10:
                            tab.Gametab = 4;
                            tab.Period = 3;
                            tab.Rank = 0;
                            break;
                        case 11:
                            tab.Gametab = 4;
                            tab.Period = 4;
                            tab.Rank = 0;
                            break;
                        case 12:
                            tab.Gametab = 4;
                            tab.Period = 5;
                            tab.Rank = 0;
                            break;
                    }
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

/*
//タスク①　8:05再起動(別スレッド)
cts_restart = new CancellationTokenSource();
var t = Task.Factory.StartNew(async () =>
{
    while (true)
    {
        await Task.Delay(2000, cts_restart.Token);
        DateTime dt = DateTime.Now;
        if (dt.Hour == 8 && dt.Minute == 5 && dt.Second <= 2)
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
TaskScheduler.FromCurrentSynchronizationContext());*/