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
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Dynamic;

namespace AutoTradeOriginal
{

    public partial class Form1 : Form
    {
        
        private CancellationTokenSource cts_loop = null;
        private HighLow BO;
  
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
            //label15.Text = "バージョン:" + Assembly.GetExecutingAssembly().GetName().Version.ToString();

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
            splitContainer1.Panel1.Enabled = true;
        }
        private async void button_start_Click(object sender, EventArgs e)
        {
            
            await BO.Initialize();
            await Task.Delay(10000);
            await BO.Oneclick();
            cts_loop = new CancellationTokenSource();
            history(cts_loop.Token);
            await InfiniteLoopAsync(cts_loop.Token);

            
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
                    logbox("待機します");

                    string mes = await NamedPipe.WaitForNamedpipe("highlowpipe", ct);

                    logbox("シグナルを受け取りました");

                    //投資
                    await BO.Invest(mes);

                    await Task.Delay(10000, ct);

                    await BO.ResetTab();
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
                    logbox(e.ToString());
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
                        GetUsage();
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

        public void GetUsage()
        {
            PerformanceCounter cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";

            // will always start at 0
            float firstValue = cpuCounter.NextValue();
            System.Threading.Thread.Sleep(1000);
            // now matches task manager reading
            int secondValue = (int)cpuCounter.NextValue();
        }
    }
}

//https://autotradeauth-default-rtdb.firebaseio.com/Users/data.json
/*
private async void button_pagedown_Click(object sender, EventArgs e)
{
    await BO.Scroll(0);
}

private async void button_pageup_Click(object sender, EventArgs e)
{
    await BO.Scroll(1);
}*/