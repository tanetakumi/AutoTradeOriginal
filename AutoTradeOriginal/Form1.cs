using AutoTradeOriginal.Properties;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTradeOriginal
{

    public partial class Form1 : Form
    {
        
        private CancellationTokenSource cts_loop = null;
        private HighLow BO;
        private int invest_count = 0;
  
        public Form1()
        {
            InitializeComponent();
            InitializeChromium();
            Microsoft.Win32.SystemEvents.PowerModeChanged += new Microsoft.Win32.PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);
            
        }

        //Chromiumの初期化
        private void InitializeChromium()
        {
            BO = new HighLow();
            splitContainer1.Panel1.Controls.Add(BO.getBrowser());
        }

        //電源周りの設定
        private void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case Microsoft.Win32.PowerModes.Suspend:
                    Logbox("PCの休止・スリープを検知しました");
                    break;
                case Microsoft.Win32.PowerModes.Resume:
                    if (button_stop.Enabled)
                    {
                        button_stop.PerformClick();
                    }
                    Logbox("PCが休止・スリープされました。取引が中断されました。");
                    MessageBox.Show("PCが休止・スリープされました。システムを再起動してください。");
                    break;
            }
        }

        //ロードしたタイミング
        private void Form1_Load(object sender, EventArgs e)
        {
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
            Settings.Default.Save();
            BO.Dispose();
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
            button_loginpage.Enabled = true;
            button_openpage.Enabled = true;
        }
        void Disable_interface()
        {
            button_stop.Enabled = true;
            button_start.Enabled = false;
            button_loginpage.Enabled = false;
            button_openpage.Enabled = false;
        }
        void Loading_interface()
        {
            button_stop.Enabled = false;
            button_start.Enabled = false;
            button_loginpage.Enabled = false;
            button_openpage.Enabled = false;
        }
        private async void button_start_Click(object sender, EventArgs e)
        {
            if (DateTime.Now > DateTime.Parse("2022/02/20 12:34:56"))
            {
                MessageBox.Show("Error");
                return;
            }
            Disable_interface();
            await BO.Oneclick();
            cts_loop = new CancellationTokenSource();
            Restart(cts_loop.Token);
            await InfiniteLoopAsync(cts_loop.Token);
        }

        private void Restart(CancellationToken ct)
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
                        if (dt.Hour % 3 == 0 && dt.Minute == 3)
                        {
                            Console.WriteLine("リロードタスク実行");
                            await BO.ReloadPage();
                            Invoke(new Action(() => Logbox("リロードタスク実行")));
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

        private static AutoResetEvent waitEvent = new AutoResetEvent(true);
        private async Task Investment(string message)
        {
            waitEvent.WaitOne();
            string result = await BO.Invest(message);
            Logbox(result);
            waitEvent.Set();
        }

        private async Task InfiniteLoopAsync(CancellationToken ct)
        {
            while (true)
            {
                //②投資
                try
                {
                    Logbox("待機します");

                    string mes = await NamedPipe.WaitForNamedpipe("highlowpipe", ct);

                    Logbox(mes.Split('#')[0]+"のシグナルを受け取りました");

                    //投資
                    var _ = Task.Run(async () =>
                    {
                        DateTime dt = DateTime.Now;
                        Console.WriteLine("待機中");
                        waitEvent.WaitOne();
                        Console.WriteLine("待機解除");
                        if((DateTime.Now - dt).TotalSeconds < 7)
                        {
                            string res;
                            try
                            {
                                res = await BO.Invest(mes);
                            }
                            catch (Exception e)
                            {
                                res = e.ToString();
                            }
                            Invoke(new Action(() => Logbox(res)));
                        }
                        Console.WriteLine("次の待機解除");
                        waitEvent.Set();
                    });

                    invest_count++;
                    if(invest_count % 3 == 0)
                    {
                        await BO.ResetTab();
                    }
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
                    Logbox(e.ToString());
                }
                if (ct.IsCancellationRequested)
                {
                    Console.WriteLine("main loop の削除");
                    Logbox("投資スレッドのキャンセル");
                    return;
                }
            }
        }


        private void Logbox(string text)
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

        private async void history(CancellationToken ct)
        {
            await Task.Delay(60000);

            var _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        await Task.Delay(10000);
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
            }, ct);
        }

        private async void button_openpage_Click(object sender, EventArgs e)
        {
            label1.Text = "ロード中";
            Loading_interface();
            await BO.OpenDemo();
            Enable_interface();
            label1.Text = "";
        }

        private async void button_loginpage_Click(object sender, EventArgs e)
        {
            label1.Text = "ロード中";
            Loading_interface();
            await BO.OpenReal();
            Enable_interface();
            label1.Text = "";
        }
    }
}