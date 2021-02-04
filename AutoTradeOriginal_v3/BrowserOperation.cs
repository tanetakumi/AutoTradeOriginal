using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp.WinForms;
using CefSharp;
using System.Text.RegularExpressions;

namespace AutoTradeOriginal_v3
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
            browser = new ChromiumWebBrowser();
        }
        public void BrowserShutdown()
        {
            browser.Dispose();
            Cef.Shutdown();
        }
        public bool CheckExcuteJavascript()
        {
            return browser.CanExecuteJavascriptInMainFrame;
        }

        public async Task Initialize(bool real, string username, string password)
        {
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
                await ExB("document.querySelector('#header > div > div > div > div > div > span > span > a:nth-child(5) > i').click()");

                await Wait(15000, 1000
                    , "document.getElementById('login-password').getAttribute('placeholder');"
                    , "パスワード"
                    );

                await ExB($"document.getElementById('login-username').value = '{username}';" +
                    $"document.getElementById('login-password').value = '{password}';");
                await Task.Delay(3000);
                await ExB("document.getElementsByClassName('btn btn-highlight btn-extruded btn-fluid form-loading-indicator')[0].click();");

                if (!await Wait(15000, 1000
                    , "document.getElementById('tradingZoneRegion').getAttribute('style');"
                    , "display: block;"
                    )) throw new Exception("リアル口座のログインできませんでした");

            }
            //デモ口座
            else
            {
                //クイックデモ
                await ExB("document.querySelector('#header > div > div > div > div > div > span > span > a.highlight.hidden-xs.outlineNone > i').click()");

                if (!await Wait(15000, 1000
                    , "document.querySelector('body').getAttribute('class')"
                    , "non-responsive language-ja-jp cashbackCurrency-392 logged-in quick-demo   userCurrency-392 comLabel  complete onboarding-activated"
                    )) throw new Exception("デモ口座のログインできませんでした0x21");
                await Task.Delay(1000);

                //取引を始める
                await ExB("document.querySelector('#account-balance > div.pull-left.staBlock.cashback-balance.onboarding-highlighted.hiddenArea > div > div.onboarding-tooltip-content.success.last-child > a').click();");
                if (!await Wait(5000, 1000
                    , "document.querySelector('body').getAttribute('class')"
                    , "non-responsive language-ja-jp cashbackCurrency-392 logged-in quick-demo   userCurrency-392 comLabel  complete"
                    )) throw new Exception("デモ口座のログインできませんでした0x22");
            }
        }

        public async Task<bool> IsAmountInLimits(bool _highlimit, bool _lowlimit, int HighLimit, int LowLimit)
        {
            string amount_str = await ExR("document.getElementById('balance').innerText;");
            int now_amount = int.Parse(Regex.Replace(amount_str, @"[^0-9]", ""));

            if (_highlimit && now_amount >= HighLimit)
            {
                return false;
            }
            if (_lowlimit && LowLimit >= now_amount)
            {
                return false;
            }
            else return true;
        }
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
        public async Task<string> InvestHighLow(string currency, string up_down, int amount, int repeat, int Retry_milsec, int gametab, int period, int lon = 0)
        {
            await ExB($"document.evaluate('//*[@id=\"assetsGameTypeZoneRegion\"]/ul/li[{gametab}]', document, null, 6, null).snapshotItem(0).click();" +
                      $"document.evaluate('//*[@id=\"assetsCategoryFilterZoneRegion\"]/div/div[{period}]', document, null, 6, null).snapshotItem(0).click();");

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
            await ExB($"var p = document.evaluate('//*[@id=\"trading_zone_content\"]/div[1]/div[2]/div[2]/div[1]', document, null, 6, null).snapshotItem(0);p.setAttribute('val', '{amount}'); p.click();"
                       + $"document.getElementById('{up_down}_button').click();"
                       + "document.getElementById('invest_now_button').click(); "
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
                        await ExB("document.getElementById('invest_now_button').click();");
                        reinvest++;
                        await Task.Delay(Retry_milsec);
                    }
                }
            }
            if (invest) return "再購入" + reinvest.ToString() + "回　投資成功";
            else return "再購入" + reinvest.ToString() + "回　投資失敗";
        }

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
        public async Task<bool> ExB(string script)
        {
            bool res = false;
            //string jsScript = string.Format(script);

            await browser.EvaluateScriptAsync(script).ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success)
                {
                    res = true;
                }
            });
            return res;
        }
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
        public async Task Scroll(int up_down)
        {
            if (browser.CanExecuteJavascriptInMainFrame)
            {
                if (up_down == 0)
                {
                    await ExB("scrollBy(0, 100);");
                }
                else
                {
                    await ExB("scrollBy(0, -100);");
                }
            }
        }
        private async Task<bool> SelectPeriod(int rank)
        {
            DateTime dt = DateTime.Now;

            if (dt.Second > 44)
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
