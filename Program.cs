using System;
using Newtonsoft.Json.Linq;
using System.Net;

namespace NitroGenerator
{
    public class Program
    {
        private static CookieContainer cookies = new CookieContainer();
        private static List<Proxy> proxies = new List<Proxy>();
        static void Main(string[] args)
        {
            string[] proxyFile = File.ReadAllLines("proxy.txt");
            foreach (String proxy in proxyFile)
            {
                var data = proxy.Split(":");
                proxies.Add(new Proxy(data[0], data[1]));
            }
            Console.WriteLine(proxies.Count + " Proxy loaded.");
            int count = 0;
            while (true)
            {
                for(int i=0;i<6;i++)
                {
                    if (i == 5)
                    {
                        count++;
                        i = 0;
                    }
                    if (count == proxies.Count) count = 0;
                    var code = createCode(16);
                    var proxy = proxies[count];
                    Console.WriteLine(useCode(code, proxy.Ip, proxy.Port) + " Kod: " + code);
                    Thread.Sleep(500);
                }
            }
        }

        public static string createCode(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string useCode(String giftCode, String ip, String port)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.Proxy = new WebProxy(ip, Int32.Parse(port));
            handler.UseProxy = true;
            HttpClient client = new HttpClient(handler);
            return client.GetAsync("https://discord.com/api/v9/entitlements/gift-codes/"+giftCode+"?country_code=TR").
                Result.Content.ReadAsStringAsync().Result;
        }

        public class Proxy
        {
            private String ip;
            private String port;

            public Proxy(String ip, String port)
            {
                this.ip = ip;
                this.port = port;
            }

            public String Ip { get { return ip; } }
            public String Port { get { return port; } }
        }

    }
}
