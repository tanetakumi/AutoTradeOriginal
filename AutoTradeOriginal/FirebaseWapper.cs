using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AutoTradeOriginal
{
    public static class FirebaseWapper
    {
        private class Person
        {
            public string data { get; set; }

            public Person(string _data)
            {
                data = _data;
            }
        }


        public static async Task<bool> Send(string name, string url)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Put, url)
                {
                    Content = new StringContent("{\"" + name + "\":\"true\"}", Encoding.UTF8, "application/json")
                };
                var result = await client.SendAsync(request);
                Console.WriteLine("--- result ---");
                Console.WriteLine(result);
                Console.WriteLine("--- content ---");
                Console.WriteLine(await result.Content.ReadAsStringAsync());
                return result.IsSuccessStatusCode;
            }
        }

        public static async Task<string> Get(string url)
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync(url);
                var content = await result.Content.ReadAsStringAsync();
                Console.WriteLine("--- UserCheck result ---");
                Console.WriteLine(result);
                Console.WriteLine("--- UserCheck content ---");
                Console.WriteLine(content);
                return content;
            }
        }
    }
}
