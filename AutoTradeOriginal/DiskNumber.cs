using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace AutoTradeOriginal
{
    internal static class DiskNumber
    {
        //ボリュームシリアルのスタートから6文字でチェック
        public static string GetDiskNumber()
        {
            var volumes = new ManagementClass("Win32_DiskDrive").GetInstances();
            foreach (var volume in volumes)
            {
                string vol = volume["SerialNumber"].ToString();
                return vol.Substring(0,6);
            }
            return null;
        }
    }
}
