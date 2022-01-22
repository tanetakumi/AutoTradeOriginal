using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTradeOriginal
{
    class HighLow : OperateBrowser
    {
        public HighLow(ChromiumWebBrowser _browser) : base(_browser)
        {
        }

        //初期化
        public async Task<bool> Initialize(bool real, string username, string password)
        {
            //HighLowが開かれるまで待機
            await LoadPage("https://trade.highlow.com/");

            //リアル口座
            if (real)
            {
                //ログインクリック
                await browser.EvaluateScriptAsync("document.querySelector('#header > div > div > div > div > div > span > span > a:nth-child(5) > i').click()");

                //ログイン画面になるまで10秒待つ
                if (!await waitUntilTrue(10, 1000, "document.getElementById('login-password').getAttribute('placeholder')=='パスワード'"))
                {
                    Console.WriteLine("リアル口座のログインできませんでした");
                    return false;
                }

                //username passwordを入力する
                await browser.EvaluateScriptAsync(
                    $"document.getElementById('login-username').value = '{username}';" +
                    $"document.getElementById('login-password').value = '{password}';");

                await Task.Delay(3000);

                //ログインボタンのクリック
                await browser.EvaluateScriptAsync("document.getElementsByClassName('btn btn-highlight btn-extruded btn-fluid form-loading-indicator')[0].click();");

                //トレードゾーンが表示されるまで待つ
                if (!await waitUntilTrue(15, 1000, "document.getElementById('tradingZoneRegion').getAttribute('style')=='display: block;'"))
                {
                    Console.WriteLine("トレードゾーンの非表示");
                    return false;
                }

            }
            //デモ口座
            else
            {
                //デモ口座URL
                await LoadPage("https://app.highlow.com/quick-demo?source=header-quick-demo-cta");

                await Task.Delay(10000);
                //広告の削除
                await browser.EvaluateScriptAsync("document.getElementsByClassName('SecondaryBanner_closeButton__1iz3l')[1].click()");
                /*
                //クイックデモ クリック
                await browser.EvaluateScriptAsync("document.querySelector('#header > div > div > div > div > div > span > span > a > i').click()");

                //切り替わるまで待機
                if(!await waitUntilTrue(10, 1000, "document.querySelector('body > div:nth-child(8)').className.indexOf('active')>0"))
                {
                    Console.WriteLine("デモ口座のログインできませんでした0x21");
                    return false;
                }

                await Task.Delay(1000);

                //取引を始める 黄色いボタンをクリック
                await browser.EvaluateScriptAsync("document.querySelector('#account-balance > div > div > div > a').click();");

                //下の広告の削除
                await browser.EvaluateScriptAsync("var obj = document.getElementsByClassName('p-ticker--wrapper')[0];if(typeof obj!='undefined'){ obj.remove(); }");

                //デモ口座での取引画面が出たか
                if (!await waitUntilTrue(10, 1000, "document.querySelector('body > div:nth-child(8)').className == 'onboarding-overlay'"))
                {
                    Console.WriteLine("デモ口座のログインできませんでした0x21");
                    return false;
                }
                */
            }
            return true;
        }

        public async Task resetTab()
        {
            await browser.EvaluateScriptAsync(
                "var num = document.getElementById('content_1').children.length;" + 
                "for (let i = num - 1; i > 0; i--){" +
                    "document.getElementsByClassName('RecentlyOpenOptions_tabClose__2EbqE')[i].click();" + 
                "}");
        }

        public async Task selectPeriod()
        {
            await browser.EvaluateScriptAsync("document.getElementById('ChangingStrikeOOD0').click();");
            await browser.EvaluateScriptAsync("document.getElementById('ChangingStrike0').click();");
            await browser.EvaluateScriptAsync("document.getElementById('FixedPayoutHLOOD0').click();");
            await browser.EvaluateScriptAsync("document.getElementById('FixedPayoutHL0').click();");

            await browser.EvaluateScriptAsync("document.getElementById('30000').click();");
            await browser.EvaluateScriptAsync("document.getElementById('60000').click();");
            await browser.EvaluateScriptAsync("document.getElementById('180000').click();");
            await browser.EvaluateScriptAsync("document.getElementById('300000').click();");
            await browser.EvaluateScriptAsync("document.getElementById('900000').click();");
            await browser.EvaluateScriptAsync("document.getElementById('3600000').click();");
            await browser.EvaluateScriptAsync("document.getElementById('86400000').click();");
        }

    }
}
