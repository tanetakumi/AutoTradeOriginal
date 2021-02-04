
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireSharp.Interfaces;
using FireSharp.Config;
using FireSharp.Response;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Management;

namespace AutoTradeOriginal
{
    class Auth
    {
        public string Username { get; set; }
        public string Datetime { get; set; }

        private IFirebaseClient client;

        public Auth(string auth_secret,string basepath)
        {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = auth_secret,
                BasePath = basepath
            };
            client = new FireSharp.FirebaseClient(ifc);
            if (client == null)
            {
                throw new Exception("Connection Error");
            }
        }
        public bool AuthUser(string username, string license)
        {

            if (!AuthPC(license)) return false;
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()) return false;
            FirebaseResponse res = client.Get(@"Users/" + username);
            Auth ResUser = res.ResultAs<Auth>();// database result
            if (ResUser == null || ResUser.Username == null)
            {
                return false;
            }
            else if (ResUser.Username == username)
            {
                return true;
            }
            else return false;
        }

        class Notify
        {
            public string URL { get; set; }
            public int Major { get; set; }
            public int Minor { get; set; }
        }
        public string UpdatingConf(Version version)
        {
            FirebaseResponse res = client.Get("UpDate");
            Notify update = res.ResultAs<Notify>();
            Version newest = new Version(update.Major, update.Minor, 0, 0);
            if (newest <= version)
            {
                return "";
            }
            else return update.URL;
        }
        private bool AuthPC(string license)
        {
            using (ManagementObject mo = new ManagementObject("Win32_LogicalDisk=\"C:\""))
            {
                string pro = (string)mo.Properties["VolumeSerialNumber"].Value;
                int a = int.Parse(Regex.Replace(pro, @"[^1-9]", ""));
                int pass = (a * 13 + 707) % 10000;
                if (pass.ToString() == license)
                {
                    return true;
                }
                else return false;
            }
        }
    }
    /*
    class Notify
    {
        public string URL { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
    }
    */
}
