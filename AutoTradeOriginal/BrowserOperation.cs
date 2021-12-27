using CefSharp;
using CefSharp.WinForms;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoTradeOriginal
{
    class BrowserOperation
    {
        private ChromiumWebBrowser browser;

        public BrowserOperation(ChromiumWebBrowser _browser)
        {
            browser = _browser;
        }

        //Javascript が実行できるかどうかの確認
        public bool CheckExcuteJavascript()
        {
            return browser.CanExecuteJavascriptInMainFrame;
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
            }
            return true;
        }


        //履歴の取得
        public async Task<string> get_history()
        {
            string scr =
                "var item = document.getElementById('tradeActionsTableBody');" +
                    "var text = '';" +
                    "for (var i = 0; i < item.childElementCount; i++){" +
                        "for (var j = 1; j < 9; j++){" +
                            "if (j == 2) { text += item.children[i].children[j].children[0].children[0].getAttribute('class') + '#'; }" +
                                "text += item.children[i].children[j].innerText + '#';" +
                            "}" +
                        "text += '*';}text;";

            return (await getResultFromScript(scr)).text;
        }

        //投資
        public async Task<string> InvestHighLow(string currency, string up_down, int amount, int repeat, int Retry_milsec, int gametab, int period, int lon = 0)
        {
            //取引時間のタブを選択
            await browser.EvaluateScriptAsync(
                $"document.evaluate('//*[@id=\"assetsGameTypeZoneRegion\"]/ul/li[{gametab}]', document, null, 6, null).snapshotItem(0).click();" +
                $"document.evaluate('//*[@id=\"assetsCategoryFilterZoneRegion\"]/div/div[{period}]', document, null, 6, null).snapshotItem(0).click();"
            );

            //通貨選択&通貨確認
            string scr_select_currency =
                "function currency(){document.getElementsByClassName('asset-filter--opener')[0].click();" +
                "var links = document.getElementById('assetsList').children;"+
                "for (var i = 0; i < links.length; i++){"+
                    $"if (links[i].innerText == '{currency}')"+
                        "{ links[i].click(); return true; }"+
                "}document.getElementsByClassName('asset-filter--opener')[0].click();return false;}currency();";

            if ((await getResultFromScript(scr_select_currency)).text == "False") throw new Exception("通貨選択失敗");


            //トレードゾーンが表示されるまで待つ
            if (!await waitUntilTrue(7, 100, "document.getElementById('tradingZoneRegion').getAttribute('style')=='display: block;'"))
            {
                throw new Exception("ERROR:トレードゾーンが非表示でした");
            }


            //15分取引の時
            if (lon != 0)
            {
                if (!await SelectPeriod(lon)) throw new Exception("ERROR:15分取引、時間選択に失敗しました。");
                //トレードゾーンが表示されるまで待つ
                if (!await waitUntilTrue(7, 100, "document.getElementById('tradingZoneRegion').getAttribute('style')=='display: block;'"))
                {
                    throw new Exception("ERROR:トレードゾーンが非表示でした");
                }
            }

            //投資確定
            await browser.EvaluateScriptAsync(
                $"var p = document.evaluate('//*[@id=\"trading_zone_content\"]/div[1]/div[2]/div[2]/div[1]', document, null, 6, null).snapshotItem(0);" +
                $"p.setAttribute('val', '{amount}'); p.click();"+
                $"document.getElementById('{up_down}_button').click();"+
                "document.getElementById('invest_now_button').click();"
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
                    response = (await getResultFromScript("document.getElementById('notification_text').innerText;")).text;
                    if (response!= null && response != "処理中")
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

        public async Task<(bool result, string text)> getResultFromScript(string script)
        {
            CefSharp.JavascriptResponse res = await browser.EvaluateScriptAsync(script);
            if (res.Success && res.Result != null) return (true, res.Result.ToString());
            else return (false, null);
        }

        public async Task<bool> LoadPage(string url)
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

        public async Task<bool> waitUntilTrue(int num, int interval, string script)
        {
            bool result = false;
            for(int i = 0; i < num; i++)
            {
                (bool res, string text) = await getResultFromScript(script);
                if(res && text == "True")
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

            //時間が **:*4 だった時　短期:10分　中期:10分　長期:15分
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
            //時間が **:*5 だった時　短期:5分　中期:10分　長期:10分
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
            //それ以外　短期:5分　中期:10分　長期:15分
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

            string scr =
                "function min(){var ele = document.getElementById('carousel_container').children;" +
                    "for (var i = 0; i < ele.length; i++){" +
                    $"if (ele[i].innerText.indexOf('{J_time}') > -1)" +
                        "{ele[i].click();return true;}" +
                    "}return false;}" +
                "min();";

            string res = (await getResultFromScript(scr)).text;
            if (res == "False" || res == null) return false;
            else return true;
        }
    }
}
