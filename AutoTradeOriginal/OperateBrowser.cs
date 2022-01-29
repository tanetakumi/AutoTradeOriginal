﻿using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTradeOriginal
{
    class OperateBrowser
    {
        protected ChromiumWebBrowser browser;

        protected OperateBrowser()
        {
            if (Cef.IsInitialized == false)
            {
                CefSettings settings = new CefSettings();
                settings.Locale = "ja";
                settings.AcceptLanguageList = "ja-JP";
                settings.LogSeverity = LogSeverity.Disable;
                settings.SetOffScreenRenderingBestPerformanceArgs();
                settings.CefCommandLineArgs.Add("no-proxy-server", "1");
                //settings.EnableInternalPdfViewerOffScreen();
                //settings.CefCommandLineArgs.Add("enable-begin-frame-scheduling", "1");


                Cef.Initialize(settings);
            }
            browser = new ChromiumWebBrowser("https://www.google.com/");
        }

        public ChromiumWebBrowser getBrowser()
        {
            return browser;
        }

        public void Dispose()
        {
            if (browser != null)
            {
                browser.Dispose();
            }
            Cef.Shutdown();
        }

        protected async Task<string> getResultFromScript(string script, bool trace = false)
        {
            JavascriptResponse res = await browser.EvaluateScriptAsync(script);
            
            if (res.Success)
            {
                if(res.Result != null)
                {
                    if(res.Result.ToString() != "")
                    {
                        return res.Result.ToString();
                    } 
                    else
                    {
                        if (trace)
                        {
                            Console.WriteLine("Success = true, Result != null, Result.ToString() = \"\"");
                        }
                        return null;
                    }
                } 
                else
                {
                    if (trace)
                    {
                        Console.WriteLine("Success = true, Result = null");
                    }
                    return null;
                }
            } 
            else
            {
                if (trace)
                {
                    Console.WriteLine("Success = true");
                }
                return null;
            }
        }

        protected async Task<bool> LoadPage(string url)
        {
            browser.Load(url);
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(i.ToString() + " 回目のサイト読み込み");
                do
                {
                    await Task.Delay(1000);
                    
                }
                while (browser.IsLoading);

                if (browser.Address == url)
                {
                    Console.WriteLine("URL確認完了");
                    if (browser.CanExecuteJavascriptInMainFrame)
                    {
                        Console.WriteLine("Javascript実行確認完了");
                        string text = await browser.GetTextAsync();//textの取得
                        if (text != "" && text != null)
                        {
                            Console.WriteLine("テキスト確認");
                            return true;
                        }
                    }
                }
                else
                {
                    browser.Load(url);
                }
                await Task.Delay(1000);
            }
            Console.WriteLine("失敗しました。");
            return false;
        }

        protected async Task<bool> waitUntilTrue(int num, int interval, string script)
        {
            bool result = false;
            for (int i = 0; i < num; i++)
            {
                string text = await getResultFromScript(script);
                if (text == "True")
                {
                    result = true;
                    break;
                }
                await Task.Delay(interval);
            }
            return result;
        }

        //スクロール
        public async Task Scroll(int up_down)
        {
            if (browser.CanExecuteJavascriptInMainFrame)
            {
                if (up_down == 0)
                {
                    await browser.EvaluateScriptAsync("scrollBy(0, 100);");
                }
                else
                {
                    await browser.EvaluateScriptAsync("scrollBy(0, -100);");
                }
            }
        }

        public void sendKeyEventChar(int keyCode)
        {
            KeyEvent k = new KeyEvent();
            k.WindowsKeyCode = keyCode;
            k.FocusOnEditableField = true;
            k.IsSystemKey = false;
            k.Type = KeyEventType.Char;
            browser.GetBrowser().GetHost().SendKeyEvent(k);
        }

        protected void inputNumber(int num)
        {
            string numStr = num.ToString();
            foreach(char c in numStr)
            {
                int cnum = int.Parse(c.ToString());
                sendKeyEventChar(cnum + 48);
            }
        }
    }
}