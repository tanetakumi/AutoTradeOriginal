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
        public HighLow() : base()
        {
        }

        public async Task OpenDemo()
        {
            browser.Enabled = false;
            await LoadPage("https://app.highlow.com/quick-demo");
            await Task.Delay(10000);
            //広告の削除
            await browser.EvaluateScriptAsync(
                "document.evaluate('//*[@id=\"root\"]/div/div[16]', document, null, 6, null).snapshotItem(0).style.display = 'none';" +
                "document.getElementById('chart-container').style.display = 'none';"
                );
            browser.Enabled = true;
        }
        public async Task OpenReal()
        {
            browser.Enabled = false;
            await LoadPage("https://app.highlow.com/login");
            await Task.Delay(10000);
            //広告の削除
            await browser.EvaluateScriptAsync(
                "document.evaluate('//*[@id=\"root\"]/div/div[16]', document, null, 6, null).snapshotItem(0).style.display = 'none';" +
                "document.getElementById('chart-container').style.display = 'none';"
                );
            browser.Enabled = true;
        }

        public async Task ReloadPage()
        {
            browser.Reload();
            await Task.Delay(10000);
            //広告の削除
            await browser.EvaluateScriptAsync(
                "document.evaluate('//*[@id=\"root\"]/div/div[16]', document, null, 6, null).snapshotItem(0).style.display = 'none';" +
                "document.getElementById('chart-container').style.display = 'none';"
                );
        }

        public async Task ResetTab()
        {
            await browser.EvaluateScriptAsync(
                "var num = document.getElementById('content_1').children.length;" + 
                "for (let i = num - 1; i > 0; i--){" +
                    "document.getElementsByClassName('RecentlyOpenOptions_tabClose__2EbqE')[i].click();" + 
                "}");
            await browser.EvaluateScriptAsync("document.getElementById('chart-container').style.display = 'none';");
        }
        
        public async Task<(int, string)> InvestmentReturn()
        {
            string text = await getResultFromScript(
                "function con(){var tabs = document.getElementById('content_1').children;" +
                "for(var i = 0;i<tabs.length;i++){if(tabs[i].getAttribute('class').match('active')){" +
                "return tabs[i].children[1].children[1].innerText +'#'+ document.evaluate('//*[@id=\"root\"]/div/div[8]'," +
                " document, null, 6, null).snapshotItem(0).innerText;}}" +
                "}con();"
            );
            Console.WriteLine("text: "+text);
            if (text != null)
            {
                string[] words = text.Split('#');
                if(words.Length == 2)
                {
                    return (int.Parse(words[0]), words[1]); 
                }
            }
            return (-1, null);
        }

        private async Task<bool> CheckInvestment()
        {
            (int first_i, string first_s) = await InvestmentReturn();
            if(first_i == -1)
            {
                throw new Exception("投資エラー");
            }

            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(500);
                (int tmp_i, string tmp_s) = await InvestmentReturn();
                if (tmp_i == first_i + 1)
                {
                    return true;
                }
                if(tmp_s != "")
                {
                    return false;
                }
            }
            return false;
        }

        public async Task<string> Invest(string message, int retry = 3)
        {
            (string cur, string high_low, int price, int pnum) = MessageToTuple(message);
            await SelectPeriod(pnum, cur);
            await InputPrice(price);
            await Task.Delay(500);
            for (int i = 0; i<retry ; i++)
            {
                if(high_low == "up")
                {
                    await browser.EvaluateScriptAsync("document.getElementById('HIGH_TRADE_BUTTON').click();");
                }
                else
                {
                    await browser.EvaluateScriptAsync("document.getElementById('LOW_TRADE_BUTTON').click();");
                }
                if(await CheckInvestment())
                {
                    return (i+1).ToString()+"回目の投資にて成功";
                }
            }
            return "失敗しました。";
        }
        public async Task Oneclick()
        {
            await browser.EvaluateScriptAsync(
                "var data = document.evaluate('//*[@id=\"scroll_panel_1_content\"]/div[2]/div/div[2]/div/div[2]/div[1]/div'," +
                " document, null, 6, null).snapshotItem(0).getAttribute('data-test');if(!data.match('Enable')){document.evaluate(" +
                "'//*[@id=\"scroll_panel_1_content\"]/div[2]/div/div[2]/div/div[2]/div[2]/div[1]', document, null, 6, null).snapshotItem(0).click();}"
                );
        }

        public async Task InputPrice(int price)
        {   
            
            await browser.EvaluateScriptAsync(
                "var ele = document.evaluate('//*[@id=\"scroll_panel_1_content\"]/div[2]/div/div[2]/div/div[1]/div[1]/div[2]/div/input'," +
                " document, null , 6, null).snapshotItem(0);" +
                "ele.value = 0; ele.focus();"
            );
            await Task.Delay(50);
            inputNumber(price);
        }
        public async Task SelectPeriod(int num, string currency)
        {
            //<non sp>  0 Turbo30s  1  Turbo60s  2  Turbo3m  3  Turbo5m  HighLow15m( 4  sho  5  mid  6  lon )  7  HighLow1h  8  HighLow1d 
            //<sp>      9 Turbo30s  10 Turbo60s  11 Turbo3m  12 Turbo5m  HighLow15m( 14 sho  15 mid  16 lon )  17 HighLow1h  18 HighLow1d
            string category = null;
            string period = null;
            string confirm = null;
            string sub = null;
            switch (num)
            {
                case 0:
                    category = "ChangingStrikeOOD0";
                    confirm = "Turbo30";
                    period = "30000";
                    break;
                case 1:
                    category = "ChangingStrikeOOD0";
                    period = "60000";
                    confirm = "Turbo1";
                    break;
                case 2:
                    category = "ChangingStrikeOOD0";
                    period = "180000";
                    confirm = "Turbo3";
                    break;
                case 3:
                    category = "ChangingStrikeOOD0";
                    period = "300000";
                    confirm = "Turbo5";
                    break;
                case 4:
                    category = "ChangingStrike0";
                    period = "900000";
                    confirm = "HighLow15";
                    sub = "0";
                    break;
                case 5:
                    category = "ChangingStrike0";
                    period = "900000";
                    confirm = "HighLow15";
                    sub = "1";
                    break;
                case 6:
                    category = "ChangingStrike0";
                    period = "900000";
                    confirm = "HighLow15";
                    sub = "2";
                    break;
                case 7:
                    category = "ChangingStrike0";
                    period = "3600000";
                    confirm = "HighLow1";
                    break;
                case 8:
                    category = "ChangingStrike0";
                    period = "86400000";
                    confirm = "HighLow1";
                    break;
                case 9:
                    category = "FixedPayoutHLOOD0";
                    period = "30000";
                    confirm = "Turboスプレッド30";
                    break;
                case 10:
                    category = "FixedPayoutHLOOD0";
                    period = "60000";
                    confirm = "Turboスプレッド1";
                    break;
                case 11:
                    category = "FixedPayoutHLOOD0";
                    period = "180000";
                    confirm = "Turboスプレッド3";
                    break;
                case 12:
                    category = "FixedPayoutHLOOD0";
                    period = "300000";
                    confirm = "Turboスプレッド5";
                    break;
                case 13:
                    category = "FixedPayoutHL0";
                    period = "900000";
                    confirm = "HighLowスプレッド15";
                    sub = "0";
                    break;
                case 14:
                    category = "FixedPayoutHL0";
                    period = "900000";
                    confirm = "HighLowスプレッド15";
                    sub = "1";
                    break;
                case 15:
                    category = "FixedPayoutHL0";
                    period = "900000";
                    confirm = "HighLowスプレッド15";
                    sub = "2";
                    break;
                case 16:
                    category = "FixedPayoutHL0";
                    period = "3600000";
                    confirm = "HighLowスプレッド1";
                    break;
                case 17:
                    category = "FixedPayoutHL0";
                    period = "86400000";
                    confirm = "HighLowスプレッド1";
                    break;
                default:
                    throw new Exception("選択できる期間ではありません。");
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
                    "var select=" + sub + ";var periods = document.getElementById('content_0').children;if(select==0){" +
                    "for(var i=0;i<periods.length;i++){if(periods[i].children[2].children[0].getElementsByTagName('svg')[0]" +
                    ".getAttribute('class')!=null){var num = Number(periods[i].children[2].children[0].innerText.replace(':',''));" +
                    "if(num>5){periods[i].click();break;}}}} else if(select==1){periods[1].click()} else {for(var i=periods.length-1;i>=0;i--)" +
                    "{if(periods[i].children[2].children[0].getElementsByTagName('svg')[0].getAttribute('class')!=null){periods[i].click();break;} }}"
                );
            }
            
            await Task.Delay(50);

            //通貨選択できたか確認
            
            if(!await waitUntilTrue(5, 200,
                "var text = document.evaluate('//*[@id=\"scroll_panel_1_content\"]/div[2]/div/div[1]', document, null, 6, null).snapshotItem(0).innerText;" +
                "var mes = text.split(/\\n/);mes[0]+mes[3].replace(/[^0-9a-zA-Z\\u30a0-\\u30ff]/g,'')=='"+ currency + confirm + "';"
                )){
                await browser.EvaluateScriptAsync("document.getElementById('chart-container').style.display = 'none';");
                throw new Exception("通貨が選択できませんでした。");
            }

            //DisplayOFF
            await browser.EvaluateScriptAsync("document.getElementById('chart-container').style.display = 'none';");
        }

        //受け取ったメッセージからtupleを作成
        private (string, string, int, int) MessageToTuple(string message)
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
                    throw new ArgumentOutOfRangeException("Namedpipeのメッセージが範囲外のものです。"+message);
                }

                //取引価格の選択
                int price = int.Parse(mes[2]);

                //取引時間軸の選択
                int pnum = int.Parse(mes[3]);

                return (cur, high_low, price, pnum);
            }
            else
            {
                throw new IndexOutOfRangeException("Namedpipeのメッセージが範囲外のものです。" + message);
            }
        }

        public async Task test()
        {
            await browser.EvaluateScriptAsync("document.getElementById('login-username').focus();");
            await Task.Delay(1000);
            sendKeyEventChar(74);
        }
    }
}
