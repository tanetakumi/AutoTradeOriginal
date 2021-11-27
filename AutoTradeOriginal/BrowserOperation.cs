using CefSharp;
using CefSharp.WinForms;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoTradeOriginal
{
    class Browser
    {
        public ChromiumWebBrowser browser { get; set; }

        public Browser()
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
        }

        //shutdown
        public void BrowserShutdown()
        {
            browser.Dispose();
            Cef.Shutdown();
        }

        //Javascript が実行できるかどうかの確認
        public bool CheckExcuteJavascript()
        {
            return browser.CanExecuteJavascriptInMainFrame;
        }

        //初期化
        public async Task Initialize(bool real, string username, string password)
        {
            //HighLowが開かれるまで待機
            browser.Load("https://trade.highlow.com/");
            do
            {
                await Task.Delay(500);
            }
            while (browser.IsLoading);

            //リアル口座
            if (real)
            {
                //ログインクリック
                await browser.EvaluateScriptAsync("document.querySelector('#header > div > div > div > div > div > span > span > a:nth-child(5) > i').click()");

                await Wait(15000, 1000
                    , "document.getElementById('login-password').getAttribute('placeholder');"
                    , "パスワード"
                    );

                await browser.EvaluateScriptAsync($"document.getElementById('login-username').value = '{username}';" +
                    $"document.getElementById('login-password').value = '{password}';");
                await Task.Delay(3000);
                await browser.EvaluateScriptAsync("document.getElementsByClassName('btn btn-highlight btn-extruded btn-fluid form-loading-indicator')[0].click();");

                if (!await Wait(15000, 1000
                    , "document.getElementById('tradingZoneRegion').getAttribute('style');"
                    , "display: block;"
                    )) throw new Exception("リアル口座のログインできませんでした");

            }
            //デモ口座
            else
            {
                Console.WriteLine((await getResultFromScript("document.querySelector('body').getAttribute('class').indexOf('onboarding-activated')>0")).text);
                /*
                Console.WriteLine("hello"+
                    await ExR(
                    "const sleep = ms => new Promise(res => setTimeout(res, ms))" +
                    "async function wait(){" +
                        "await sleep(2000)" +
                        "return true;" +
                    "}await wait()")
                    );

                //クイックデモ
                await browser.EvaluateScriptAsync("document.querySelector('#header > div > div > div > div > div > span > span > a > i').click()");

                if (!await Wait(15000, 1000
                    , "document.querySelector('body').getAttribute('class')"
                    , "non-responsive language-ja-jp cashbackCurrency-392 logged-in quick-demo   userCurrency-392 comLabel  complete onboarding-activated"
                    )) throw new Exception("デモ口座のログインできませんでした0x21");

                await Task.Delay(1000);

                //取引を始める
                await browser.EvaluateScriptAsync(
                    "document.querySelector('#account-balance > div.pull-left.staBlock.cashback-balance.onboarding-highlighted.hiddenArea > div > div.onboarding-tooltip-content.success.last-child > a').click();"
                );

                //下の広告の削除
                await browser.EvaluateScriptAsync(
                    "var obj = document.getElementsByClassName('p-ticker--wrapper')[0];if (typeof obj != 'undefined') { obj.remove(); }"
                );

                if (!await Wait(5000, 1000
                    , "document.querySelector('body').getAttribute('class')"
                    , "non-responsive language-ja-jp cashbackCurrency-392 logged-in quick-demo   userCurrency-392 comLabel  complete"
                )) throw new Exception("デモ口座のログインできませんでした0x22");*/
            }
        }


        //履歴の取得
        public async Task<string> get_history()
        {
            string text = await ExR("var item = document.getElementById('tradeActionsTableBody');" +
                                "var text = '';" +
                                "for (var i = 0; i < item.childElementCount; i++){" +
                                "for (var j = 1; j < 9; j++){" +
                                "if (j == 2) { text += item.children[i].children[j].children[0].children[0].getAttribute('class') + '#'; }" +
                                "text += item.children[i].children[j].innerText + '#';" +
                                "}text += '*';}text;");
            return text;
        }

        //投資
        public async Task<string> InvestHighLow(string currency, string up_down, int amount, int repeat, int Retry_milsec, int gametab, int period, int lon = 0)
        {
            await browser.EvaluateScriptAsync(
                $"document.evaluate('//*[@id=\"assetsGameTypeZoneRegion\"]/ul/li[{gametab}]', document, null, 6, null).snapshotItem(0).click();" +
                $"document.evaluate('//*[@id=\"assetsCategoryFilterZoneRegion\"]/div/div[{period}]', document, null, 6, null).snapshotItem(0).click();"
            );

            //通貨選択&通貨確認
            if (await ExR(
                "function currency(){document.getElementsByClassName('asset-filter--opener')[0].click();"
                + "var links = document.getElementById('assetsList').children;"
                + "for (var i = 0; i < links.length; i++){"
                    + $"if (links[i].innerText == '{currency}')"
                    + "{ links[i].click(); return true; }"
                    + "}document.getElementsByClassName('asset-filter--opener')[0].click();return false;}currency();"
                ) == "False") throw new Exception("通貨選択失敗");

            //tradeZoneが表示されるまで待つ
            if (!await Wait(7000, 100
                , "document.getElementById('tradingZoneRegion').getAttribute('style');"
                , "display: block;"
                )) throw new Exception("ERROR:トレードゾーンが非表示でした");

            //15分取引の時
            if (lon != 0)
            {
                if (!await SelectPeriod(lon)) throw new Exception("ERROR:15分取引、時間選択に失敗しました。");
                if (!await Wait(7000, 100
                , "document.getElementById('tradingZoneRegion').getAttribute('style');"
                , "display: block;"
                )) throw new Exception("ERROR:トレードゾーンが非表示でした");
            }

            //投資確定
            await browser.EvaluateScriptAsync(
                $"var p = document.evaluate('//*[@id=\"trading_zone_content\"]/div[1]/div[2]/div[2]/div[1]', document, null, 6, null).snapshotItem(0);p.setAttribute('val', '{amount}'); p.click();"
                + $"document.getElementById('{up_down}_button').click();"
                + "document.getElementById('invest_now_button').click();"
            );

            //投資確認と再購入
            bool invest = false;
            int reinvest = 0;
            for (int j = 0; j < repeat + 1; j++)
            {
                string response = "";
                for (int i = 0; i < 30; i++)
                {
                    await Task.Delay(100);
                    response = await ExR("document.getElementById('notification_text').innerText;");
                    if (response != "処理中")
                    {
                        break;
                    }
                }
                if (response == "成功")
                {
                    invest = true;
                    break;
                }
                else
                {
                    //もう一度、投資をクリック
                    if (j < repeat)
                    {
                        await browser.EvaluateScriptAsync("document.getElementById('invest_now_button').click();");
                        reinvest++;
                        await Task.Delay(Retry_milsec);
                    }
                }
            }
            if (invest) return "再購入" + reinvest.ToString() + "回　投資成功";
            else return "再購入" + reinvest.ToString() + "回　投資失敗";
        }


        //Javascript実行　修正したい
        public async Task<string> ExR(string script)
        {
            string res = "";
            //string jsScript = string.Format(script);

            await browser.EvaluateScriptAsync(script).ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    res = response.Result.ToString();
                }
            });
            return res;
        }

        public async Task<(bool result, string text)> getResultFromScript(string script)
        {
            CefSharp.JavascriptResponse res = await browser.EvaluateScriptAsync(script);
            if (res.Success && res.Result != null) return (true, res.Result.ToString());
            else return (false, null);
        }

        public async Task<bool> waitUntilTrue(string script)
        {
            await getResultFromScript(script);
            return true;
        }

        //Javascriptを何度も実行して要素が変化するのを待つ
        public async Task<bool> Wait(int timeout, int interval, string change_element, string after)
        {
            int t1 = timeout / interval;
            for (int i = 0; i < t1; i++)
            {
                string response = await ExR(change_element);
                if (response == after)
                {
                    return true;
                }
                else
                {
                    await Task.Delay(interval);
                }
            }
            return false;
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

        private async Task<bool> SelectPeriod(int rank)
        {
            DateTime dt = DateTime.Now;

            //50秒以上の時は+60秒として次の+1分を考える
            if (dt.Second > 50)
            {
                dt = dt + new TimeSpan(0, 1, 0);
            }
            int m = dt.Minute;
            TimeSpan ts;

            if (m % 5 == 4)
            {
                if (rank == 3)//短期
                {
                    ts = new TimeSpan(0, 10 - m % 5, 0);
                }
                else if (rank == 2)//中期
                {
                    ts = new TimeSpan(0, 10 - m % 5, 0);
                }
                else//長期
                {
                    ts = new TimeSpan(0, 15 - m % 5, 0);
                }
            }
            else if (m % 5 == 0)
            {
                if (rank == 3)//短期
                {
                    ts = new TimeSpan(0, 5 - m % 5, 0);
                }
                else if (rank == 2)//中期
                {
                    ts = new TimeSpan(0, 10 - m % 5, 0);
                }
                else//長期
                {
                    ts = new TimeSpan(0, 10 - m % 5, 0);
                }
            }
            else
            {
                if (rank == 3)
                {
                    ts = new TimeSpan(0, 5 - m % 5, 0);
                }
                else if (rank == 2)
                {
                    ts = new TimeSpan(0, 10 - m % 5, 0);
                }
                else
                {
                    ts = new TimeSpan(0, 15 - m % 5, 0);
                }
            }
            string J_time = (dt + ts).ToString("HH:mm");
            if (await ExR("function min(){var ele = document.getElementById('carousel_container').children;" +
                "for (var i = 0; i < ele.length; i++){" +
                $"if (ele[i].innerText.indexOf('{J_time}') > -1)" +
                "{ele[i].click();return true;}}return false;}" +
                "min();") == "False") return false;
            else return true;
        }
    }
}
