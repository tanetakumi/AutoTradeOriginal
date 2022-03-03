using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTradeOriginal
{
    internal static class Auth
    {
        public static async Task<bool> Authentication()
        {
            string num = DiskNumber.GetDiskNumber();
            string res = await FirebaseWapper.Get("https://first-project-6d608-default-rtdb.firebaseio.com/Users/" + num + "/.json");
            if (res == "null")
            {
                Console.WriteLine("null==null");
                if (await FirebaseWapper.Send(num, "https://first-project-6d608-default-rtdb.firebaseio.com/Users/" + num + "/.json"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if(res == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<bool> Login(string num)
        {
            string res = await FirebaseWapper.Get("https://first-project-6d608-default-rtdb.firebaseio.com/Users/" + num + "/.json");
            if (res == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
