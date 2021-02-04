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
        //private ChromiumWebBrowser cefbrowser;
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

        private struct time_limit
        {
            public int start;
            public int stop;
        }
        private List<time_limit> tl = new List<time_limit>();
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
            textBox_license.Text = Settings.Default.license;
            textBox_username.Text = Settings.Default.username;
            textBox_password.Text = Settings.Default.password;
            textBox_linetoken.Text = Settings.Default.linetoken;
            UpLimit.Value = Settings.Default.uplimit;
            DownLimit.Value = Settings.Default.downlimit;
            string ver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            label15.Text = "バージョン:" + ver;

            listBox1.Items.Add("6: 00～8: 05");
            time_limit tmp = new time_limit();
            tmp.start = 360;
            tmp.stop = 485;
            tl.Add(tmp);

            try
            {
                if (File.Exists("./neo.ico"))
                {
                    Icon = new System.Drawing.Icon("./neo.ico");
                    Text = "NEO SAVIOR AT";
                    auth = new Auth("argVBbMixxBZr9V6d6wdtJBcoh1zyjgOHIydW7iK", "https://neosaviorat.firebaseio.com/");
                }
                else if (File.Exists("./highlowauto.ico"))
                {
                    Icon = new System.Drawing.Icon("./highlowauto.ico");
                    Text = "HighLowAuto";
                    auth = new Auth("cfkSLAxLbLtVklMPkZsv1ulT8cstE1dmUm1uEAO4", "https://highlowauto-c6f2e.firebaseio.com/");
                }
                else if (File.Exists("./hlauto.ico"))
                {
                    Icon = new System.Drawing.Icon("./hlauto.ico");
                    Text = "HLAuto";
                    auth = new Auth("Kc4EOfU6d0GGbG8dZ0cYYqP227wpEEp6aopOJHAx", "https://hlauto-96678.firebaseio.com/");
                }
                else throw new Exception("起動エラー");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            Settings.Default.license = textBox_license.Text;
            Settings.Default.username = textBox_username.Text;
            Settings.Default.password = textBox_password.Text;
            Settings.Default.linetoken = textBox_linetoken.Text;
            Settings.Default.uplimit = (int)UpLimit.Value;
            Settings.Default.downlimit = (int)DownLimit.Value;
            Settings.Default.Save();
            BO.BrowserShutdown();
        }

        private void button_timeAdd_Click(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value < dateTimePicker2.Value)
            {
                listBox1.Items.Add(dateTimePicker1.Value.ToString("HH: mm") + "～" + dateTimePicker2.Value.ToString("HH: mm"));
                time_limit tmp = new time_limit();
                tmp.start = dateTimePicker1.Value.Hour * 60 + dateTimePicker1.Value.Minute;
                tmp.stop = dateTimePicker2.Value.Hour * 60 + dateTimePicker2.Value.Minute;
                tl.Add(tmp);
            }

        }
        private void button_timeDelete_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                int item_index = listBox1.SelectedIndex;
                listBox1.Items.RemoveAt(item_index);
                tl.RemoveAt(item_index);
            }
        }

        void Enable_interface()
        {
            button_stop.Enabled = false;
            button_start.Enabled = true;
            splitContainer1.Panel1.Enabled = true;
            button_import.Enabled = true;
            button_export.Enabled = true;
        }
        private async void button_start_Click(object sender, EventArgs e)
        {
            button_start.Enabled = false;
            button_stop.Enabled = false;
            splitContainer1.Panel1.Enabled = false;
            button_import.Enabled = false;
            button_export.Enabled = false;


            //ユーザー認証とインターネット接続確認
            if (auth.AuthUser(textBox_username.Text, textBox_license.Text))
            {
                add_text("ユーザー認証完了");

                string notify = auth.UpdatingConf(Assembly.GetExecutingAssembly().GetName().Version);
                if (notify != "")
                {
                    DialogResult result = MessageBox.Show("アップデートがあります。\r\nブラウザからダウンロードしますか？", "", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(notify);
                        Enable_interface();
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("ユーザー認証失敗");
                Enable_interface();
                return;
            }
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
                if (!check_time()) return;
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
                //②時間確認
                if (!check_time())
                {
                    add_text("取引時間外");
                    continue;//以下の処理をスキップ
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
                //⑤LINE送信
                if (checkBox_line.Checked)
                {
                    if (result.IndexOf("成功") > -1)
                    {
                        DateTime dt = DateTime.Now;
                        await sendMessage("\r\n取引通貨:" + mes[0] + "\r\n取引時間:" + dt.ToString("MM/dd HH:mm:ss") + "\r\nエントリー方向:" + mes[1], textBox_linetoken.Text);
                    }
                    else
                    {
                        await sendMessage("\r\n投資失敗\r\n取引通貨:" + mes[0], textBox_linetoken.Text);
                    }
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

        private void checkBox_line_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_line.Checked)
            {
                textBox_linetoken.Enabled = true;
            }
            else
            {
                textBox_linetoken.Enabled = false;
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

        private bool check_time()
        {
            DateTime dt = DateTime.Now;
            int sum_minute = dt.Hour * 60 + dt.Minute;
            for (int i = 0; i < tl.Count; i++)
            {
                if (tl[i].start < sum_minute && sum_minute < tl[i].stop) return false;
            }
            return true;
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
        private void button_import_Click(object sender, EventArgs e)
        {
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();
            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            ofd.FileName = "default.html";
            //はじめに表示されるフォルダを指定する
            ofd.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しないとすべてのファイルが表示される
            ofd.Filter = "エクセルファイル(*.xls;*.xlsx)|*.xls;*.xlsx|すべてのファイル(*.*)|*.*";
            //[ファイルの種類]ではじめに選択されるものを指定する
            //2番目の「すべてのファイル」が選択されているようにする
            ofd.FilterIndex = 2;
            //タイトルを設定する
            ofd.Title = "開くファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;
            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされたとき、選択されたファイル名を表示する
                Console.WriteLine(ofd.FileName);
                using (XLWorkbook wb = new XLWorkbook(ofd.FileName))
                {
                    try
                    {
                        IXLWorksheet ws = wb.Worksheet("TradingHistory");
                        int row = ws.RowsUsed().Count();

                        if (row > 200)
                        {
                            throw new Exception("200行を超えています。\r\n速度低下が見込まれるため読み込めません。");
                        }
                        for (int i = 0; i < row - 1; i++)
                        {
                            string[] list = new string[11];
                            for (int j = 0; j < 11; j++)
                            {
                                list[j] = ws.Cell(i + 2, j + 1).Value.ToString();
                            }
                            listView1.Items.Add(new ListViewItem(list));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
            }
        }
        private void button_export_Click(object sender, EventArgs e)
        {
            using (XLWorkbook wb = new XLWorkbook(XLEventTracking.Disabled))
            {
                // ワークシートを作成
                IXLWorksheet sh = wb.AddWorksheet("TradingHistory");
                string[] header = { "シグナル受信時間", "口座", "エントリー方向", "取引源資産", "取引内容", "取引時間", "判定時刻", "ステータス", "判定レート", "購入", "判定時ペイアウト" };
                for (int i = 0; i < header.Length; i++)
                {
                    sh.Cell(1, 1 + i).Value = header[i];
                }

                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    for (int j = 0; j < listView1.Columns.Count; j++)
                    {
                        sh.Cell(2 + i, 1 + j).Value = listView1.Items[i].SubItems[j].Text;
                    }

                }
                SaveFileDialog sfd = new SaveFileDialog();
                // はじめに「ファイル名」で表示される文字列を指定する
                sfd.FileName = "TradingHistory.xlsx";

                // はじめに表示されるフォルダを指定する(ドキュメントフォルダ)
                sfd.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                // [ファイルの種類]に表示される選択肢を指定する
                sfd.Filter = "エクセルファイル(*.xls;*.xlsx)|*.xls;*.xlsx|すべてのファイル(*.*)|*.*";

                // タイトルを設定する
                sfd.Title = "保存先のファイルを選択してください";

                // ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
                sfd.RestoreDirectory = true;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        wb.SaveAs(sfd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    // エクセルファイル保存して終了

                }
            }
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
