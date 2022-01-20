using CefSharp;
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

        protected OperateBrowser(ChromiumWebBrowser _browser)
        {
            browser = _browser;
        }

        //Javascript が実行できるかどうかの確認
        public bool CheckExcuteJavascript()
        {
            return browser.CanExecuteJavascriptInMainFrame;
        }

        protected async Task<(bool result, string text)> getResultFromScript(string script)
        {
            JavascriptResponse res = await browser.EvaluateScriptAsync(script);
            if (res.Success && res.Result != null) return (true, res.Result.ToString());
            else return (false, null);
        }

        protected async Task<bool> LoadPage(string url)
        {
            browser.Load(url);
            for (int i = 0; i < 5; i++)
            {
                do
                {
                    await Task.Delay(1000);
                    Console.WriteLine(i.ToString() + " : Browser Loading");
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
                (bool res, string text) = await getResultFromScript(script);
                if (res && text == "True")
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
    }
}
