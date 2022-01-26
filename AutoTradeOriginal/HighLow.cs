﻿using CefSharp;
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
        public async Task<bool> Initialize(bool real = false, string username = "", string password = "")
        {
            //HighLowが開かれるまで待機
            //await LoadPage("https://app.highlow.com/quick-demo");

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
                await LoadPage("https://app.highlow.com/quick-demo");
                await Task.Delay(10000);
                //広告の削除
                await browser.EvaluateScriptAsync("document.evaluate('//*[@id=\"root\"]/div/div[16]/div/div[1]', document, null, 6, null).snapshotItem(0).click();");
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
        
        public async Task<(int, string)> investmentReturn()
        {
            string text = await getResultFromScript(
                "function con(){var tabs = document.getElementById('content_1').children;" +
                "for(var i = 0;i<tabs.length;i++){if(tabs[i].getAttribute('class').match('active')){" +
                "return tabs[i].children[1].children[1].innerText +'#'+ document.evaluate('//*[@id=\"root\"]/div/div[8]'," +
                " document, null, 6, null).snapshotItem(0).innerText;}}" +
                "}con();"
            );
            if (text != null)
            {
                string[] words = text.Split('#');
                if(words.Length == 2)
                {
                    return (Int32.Parse(words[0]), words[1]); 
                }
            }
            return (-1, null);
        }

        private async Task<bool> checkInvestment()
        {
            var first = await investmentReturn();
            if(first.Item1 == -1)
            {
                throw new Exception("投資エラー");
            }

            for(int i = 0; i < 10; i++)
            {
                await Task.Delay(300);
                var tmp = await investmentReturn();
                if(tmp.Item1 == first.Item1 + 1)
                {
                    return true;
                }
                else if(tmp.Item2!="")
                {
                    return false;
                }
            }
            return false;
        }

        public async Task inputPrice(int price)
        {   
            
            await browser.EvaluateScriptAsync(
                "var ele = document.evaluate('//*[@id=\"scroll_panel_1_content\"]/div[2]/div/div[2]/div/div[1]/div[1]/div[2]/div/input'," +
                " document, null , 6, null).snapshotItem(0);" +
                "ele.value = 0; ele.focus();"
            );
            inputNumber(price);
        }
        public async Task selectPeriod(int num, string currency)
        {
            //<non sp>  0 Turbo30s  1  Turbo60s  2  Turbo3m  3  Turbo5m  HighLow15m( 4  sho  5  mid  6  lon )  7  HighLow1h  8  HighLow1d 
            //<sp>      9 Turbo30s  10 Turbo60s  11 Turbo3m  12 Turbo5m  HighLow15m( 14 sho  15 mid  16 lon )  17 HighLow1h  18 HighLow1d
            string category = null;
            string period = "600";
            string sub = null;
            switch (num)
            {
                case 0:
                    category = "ChangingStrikeOOD0";
                    period = "30000";
                    break;
                case 1:
                    category = "ChangingStrikeOOD0";
                    period = "60000";
                    break;
                case 2:
                    category = "ChangingStrikeOOD0";
                    period = "180000";
                    break;
                case 3:
                    category = "ChangingStrikeOOD0";
                    period = "300000";
                    break;
                case 4:
                    category = "ChangingStrike0";
                    period = "900000";
                    sub = "sho";
                    break;
                case 5:
                    category = "ChangingStrike0";
                    period = "900000";
                    sub = "mid";
                    break;
                case 6:
                    category = "ChangingStrike0";
                    period = "900000";
                    sub = "lon";
                    break;
                case 7:
                    category = "ChangingStrike0";
                    period = "3600000";
                    break;
                case 8:
                    category = "ChangingStrike0";
                    period = "86400000";
                    break;
                case 9:
                    category = "FixedPayoutHLOOD0";
                    period = "30000";
                    break;
                case 10:
                    category = "FixedPayoutHLOOD0";
                    period = "60000";
                    break;
                case 11:
                    category = "FixedPayoutHLOOD0";
                    period = "180000";
                    break;
                case 12:
                    category = "FixedPayoutHLOOD0";
                    period = "300000";
                    break;
                case 13:
                    category = "FixedPayoutHL0";
                    period = "900000";
                    sub = "sho";
                    break;
                case 14:
                    category = "FixedPayoutHL0";
                    period = "900000";
                    sub = "mid";
                    break;
                case 15:
                    category = "FixedPayoutHL0";
                    period = "900000";
                    sub = "lon";
                    break;
                case 16:
                    category = "FixedPayoutHL0";
                    period = "3600000";
                    break;
                case 17:
                    category = "FixedPayoutHL0";
                    period = "86400000";
                    break;
            }


            await browser.EvaluateScriptAsync(
                "if(!document.getElementById('" + category + "').className.match('active')){document.getElementById('" + category+"').click();}" +
                "if(!document.getElementById('" + period + "').className.match('selected')){document.getElementById('" + period + "').click();}" +
                "if(!document.getElementById('" + currency + "').className.match('selected')){document.getElementById('" + currency + "').click();}"
            );

            await Task.Delay(700);
            //15分を選んだかどうか
            if(sub == null)
            {
                //一つだけ選択
                await browser.EvaluateScriptAsync("document.getElementById('content_0').children[0].click();");
            }
            else
            {
                //長さ選んで選択
                await browser.EvaluateScriptAsync(
                    "var select = 0;var periods = document.getElementById('content_0').children;if(select==0){" +
                    "for(var i=0;i<periods.length;i++){if(periods[i].children[2].children[0].getElementsByTagName('svg')[0]" +
                    ".getAttribute('class')!=null){var num = Number(periods[i].children[2].children[0].innerText.replace(':',''));" +
                    "if(num>5){periods[i].click();break;}}}} else if(select==1){periods[1].click()} else {for(var i=periods.length-1;i>=0;i--)" +
                    "{if(periods[i].children[2].children[0].getElementsByTagName('svg')[0].getAttribute('class')!=null){periods[i].click();break;} }}"
                );
            }
        }

        //受け取ったメッセージからtupleを作成
        private (string currency, string high_low, int price, int pnum) MessageToTuple(string message)
        {
            //メッセージ　<通貨>#<HighLow>#<価格>#<時間軸ナンバー>

            //<non sp>  0 Turbo30s  1  Turbo60s  2  Turbo3m  3  Turbo5m  HighLow15m( 4  sho  5  mid  6  lon )  7  HighLow1h  8  HighLow1d 
            //<sp>      9 Turbo30s  10 Turbo60s  11 Turbo3m  12 Turbo5m  HighLow15m( 14 sho  15 mid  16 lon )  17 HighLow1h  18 HighLow1d

            string[] mes = message.Split('#');
            if (mes.Length == 4)
            {   
                //通貨について
                string cur = mes[0];
                if (cur.Length == 6 && !cur.Contains("/"))
                {
                    cur = cur.Insert(3, "/");
                }


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

                return (cur, high_low, price, pnum);
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        

    }
}
