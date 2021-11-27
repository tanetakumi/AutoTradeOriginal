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
        private (string currency, string high_low, int price) MessageToTuple(string message)
        {
            string[] mes = message.Split('#');
            if (mes.Length == 3)
            {
                
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

                int price = int.Parse(mes[2]);

                return (mes[0], high_low, price);
            } 
            else
            {
                throw new IndexOutOfRangeException();
            }
            

        }
    }
}
