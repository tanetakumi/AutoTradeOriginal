using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoTradeOriginal
{
    public partial class Form1
    {
        private async Task<string> WaitForNamedpipe(string pipename, CancellationToken ct)
        {
            string message = "";
            using (NamedPipeServerStream npss = new NamedPipeServerStream(pipename, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous))
            {
                await npss.WaitForConnectionAsync(ct);
                using (StreamReader reader = new StreamReader(npss))
                {
                    message = reader.ReadLine();
                }
            }
            return message;
        }
    }

    public partial class Form1
    {
        //受け取ったメッセージからtupleを作成
        private (string currency, string high_low, int price, int gametab, int period, int rank) MessageToTuple(string message)
        {
            //メッセージ　<通貨>#<HighLow>#<価格>#<時間軸ナンバー>
            string[] mes = message.Split('#');
            if (mes.Length == 4)
            {
                //HighかLowの選択
                string high_low = "";
                if(mes[1].ToLower() == "high" || mes[1].ToLower() == "up")
                {
                    high_low = "up";
                } 
                else if(mes[1].ToLower() == "low" || mes[1].ToLower() == "down")
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
                (int gametab, int period, int rank) tup = selectPeriod(pnum);

                return (mes[0], high_low, price, tup.gametab, tup.period, tup.rank);
            } 
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        //取引時間軸の選択
        private (int gametab, int period, int rank) selectPeriod(int num)
        {
            (int, int, int) tuple;
            switch (num)
            {
                //HighLow
                case 1:
                    tuple = (1, 2, 1);
                    break;
                case 2:
                    tuple = (1, 2, 2);
                    break;
                case 3:
                    tuple = (1, 2, 3);
                    break;
                //HighLowスプ
                case 4:
                    tuple = (2, 2, 1);
                    break;
                case 5:
                    tuple = (2, 2, 2);
                    break;
                case 6:
                    tuple = (2, 2, 3);
                    break;
                //Turbo
                case 7:
                    tuple = (3, 3, 0);
                    break;
                case 8:
                    tuple = (3, 4, 0);
                    break;
                case 9:
                    tuple = (3, 5, 0);
                    break;
                //Turboスプ
                case 10:
                    tuple = (4, 3, 0);
                    break;
                case 11:
                    tuple = (4, 4, 0);
                    break;
                case 12:
                    tuple = (4, 5, 0);
                    break;
                default:
                    tuple = (0, 0, 0);
                    break;
            }
            return tuple;
        }
    }
}
