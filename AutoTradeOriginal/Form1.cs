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
using System.IO.Pipes;
using System.IO;
using System.Runtime.InteropServices;

namespace AutoTradeOriginal
{

    public partial class Form1 : Form
    {
        
        private CancellationTokenSource cts_loop = null;
        private string DemoReal = "デモ口座";
        private HighLow BO;
        private ChromiumWebBrowser browser = null;

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
            if(browser != null)
            {
                browser.Dispose();
                browser = null;
            }
            browser = new ChromiumWebBrowser("https://www.google.com/");
            splitContainer2.Panel2.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
            browser.Enabled = true;

            BO = new HighLow(browser);
        }

        //電源周りの設定
        private void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case Microsoft.Win32.PowerModes.Suspend:
                    logbox("PCの休止・スリープを検知しました");
                    break;
                case Microsoft.Win32.PowerModes.Resume:
                    if (button_stop.Enabled)
                    {
                        button_stop.PerformClick();
                    }
                    logbox("PCが休止・スリープされました。取引が中断されました。");
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
            if (cts_loop != null)
            {
                cts_loop.Cancel();
            }
            Settings.Default.username = textBox_username.Text;
            Settings.Default.password = textBox_password.Text;
            Settings.Default.Save();
            if(browser != null)
            {
                browser.Dispose();
                browser = null;
            }
            
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
            await BO.Initialize();
            await Task.Delay(10000);
            await BO.inputPrice(42213);


        }

        private async Task<string> WaitForNamedpipe(string pipename, CancellationToken ct)
        {
            string message = "";
            using (NamedPipeServerStream npss = new NamedPipeServerStream(pipename, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
            {
                await npss.WaitForConnectionAsync(ct);
                using (StreamReader reader = new StreamReader(npss))
                {
                    message = reader.ReadLine();
                }
            }
            return message;
        }

        private void restart(CancellationToken ct)
        {

            var _ = Task.Run(async () =>
            {
                Console.WriteLine("再起動スレッド開始");
                while (true)
                {
                    try
                    {
                        await Task.Delay(60000, ct);
                        DateTime dt = DateTime.Now;
                        if (dt.Hour == 0 && dt.Minute == 44)
                        {
                            return 0;
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        Console.WriteLine("task delayのキャンセル");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    if (ct.IsCancellationRequested)
                    {
                        Console.WriteLine("再起動スレッドの削除");
                        return 1;
                    }
                }
            },ct).ContinueWith((o) =>
            {
                
                if(o.Result == 0)
                {
                    Console.WriteLine("再起動タスク実行");
                    Invoke(new Action(async () =>
                    {
                        button_stop.PerformClick();
                        await Task.Delay(1000);
                        button_start.PerformClick();
                    }));
                }
                else
                {
                    Console.WriteLine("再起動キャンセルによる削除");
                }
                
            });
        }

        private async void history(CancellationToken ct)
        {
            await Task.Delay(60000);

            var _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                    }
                    catch (TaskCanceledException)
                    {
                        Console.WriteLine("task delayのキャンセル");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    if (ct.IsCancellationRequested)
                    {
                        Console.WriteLine("履歴取得スレッドの削除");
                        return;
                    }
                }
            },ct);
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            if (cts_loop != null)
            {
                cts_loop.Cancel();
            }
            Enable_interface();
        }

        private async Task InfiniteLoopAsync(CancellationToken ct)
        {
            while (true)
            {
                //②投資
                try
                {
                    //Message:  0-通貨# 1-HighLow# 2-金額倍率
                    //①メッセージの受け取り
                    logbox("待機します");
                    string mes = await WaitForNamedpipe("highlowpipe", ct);
                    logbox("シグナルを受け取りました");
                    (string currency, string high_low, int price, int gametab, int period, int rank) = MessageToTuple(mes);
                    int repeat = Decimal.ToInt32(numericUpDown_retry.Value);
                    int retry_milsec = 500;
                    //string result = await BO.InvestHighLow(currency, high_low, price, repeat, retry_milsec, gametab, period, rank);
                    await Task.Delay(1000, ct);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("task delayのキャンセル");
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("namedpipe wait の処理");
                }
                catch (Exception e)
                {
                    logbox("その他の例外\n"+e.ToString());
                }
                if (ct.IsCancellationRequested)
                {
                    Console.WriteLine("main loop の削除");
                    return;
                }
            }
        }


        private void logbox(string text)
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



        //受け取ったメッセージからtupleを作成
        private (string currency, string high_low, int price, int gametab, int period, int rank) MessageToTuple(string message)
        {
            //メッセージ　<通貨>#<HighLow>#<価格>#<時間軸ナンバー>
            string[] mes = message.Split('#');
            if (mes.Length == 4)
            {
                //HighかLowの選択
                string high_low = "";
                if (mes[1].ToLower() == "high" || mes[1].ToLower() == "up")
                {
                    high_low = "up";
                }
                else if (mes[1].ToLower() == "low" || mes[1].ToLower() == "down")
                {
                    high_low = "down";
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Namedpipeのメッセージが範囲外のものです。");
                }

                //取引価格の選択
                int price = int.Parse(mes[2]);

                //取引時間軸の選択
                int pnum = int.Parse(mes[3]);
                (int gametab, int period, int rank) tup = selectPeriod(pnum);

                return (mes[0], high_low, price, tup.gametab, tup.period, tup.rank);
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        //取引時間軸の選択
        private (int gametab, int period, int rank) selectPeriod(int num)
        {
            (int, int, int) tuple;
            switch (num)
            {
                //HighLow 15分
                case 1://15分短期
                    tuple = (1, 2, 1);
                    break;
                case 2://15分中期
                    tuple = (1, 2, 2);
                    break;
                case 3://15分長期
                    tuple = (1, 2, 3);
                    break;
                case 4://1時間
                    tuple = (1, 3, 0);
                    break;
                case 5://1日
                    tuple = (1, 4, 0);
                    break;
                //HighLowスプ
                case 6://15分短期
                    tuple = (2, 2, 1);
                    break;
                case 7://15分中期
                    tuple = (2, 2, 2);
                    break;
                case 8://15分長期
                    tuple = (2, 2, 3);
                    break;
                case 9://1時間
                    tuple = (2, 3, 0);
                    break;
                case 10://1日
                    tuple = (2, 4, 0);
                    break;
                //Turbo
                case 11://30秒
                    tuple = (3, 2, 0);
                    break;
                case 12://1分
                    tuple = (3, 3, 0);
                    break;
                case 13://3分
                    tuple = (3, 4, 0);
                    break;
                case 14://5分
                    tuple = (3, 5, 0);
                    break;
                //Turboスプ
                case 15://30秒
                    tuple = (4, 2, 0);
                    break;
                case 16://1分
                    tuple = (4, 3, 0);
                    break;
                case 17://3分
                    tuple = (4, 4, 0);
                    break;
                case 18://5分
                    tuple = (4, 5, 0);
                    break;
                default:
                    tuple = (0, 0, 0);
                    break;
            }
            return tuple;
        }
    }
}
