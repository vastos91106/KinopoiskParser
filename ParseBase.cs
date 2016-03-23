using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ParseDemon
{
    public abstract class ParseBase
    {
        public static List<string> ProxyList { get; set; }

        public static string CurentIp { get; set; }

        private static int MaxIteration { get; set; }

        public static Dictionary<int, bool> ParsedItems { get; set; }

        protected ParseBase()
        {
            MaxIteration = 888000;

            var destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "proxy.txt");

            if (File.Exists(destPath))
            {
                ProxyList = new List<string>();

                ProxyList.AddRange(File.ReadLines(destPath));

                ParsedItems = new Dictionary<int, bool>();

                for (var i = 0; i < MaxIteration; i++)
                {
                    ParsedItems.Add(i, false);
                }
            }
            else
            {
                Console.WriteLine("proxy.txt not exist");
            }
        }

        public virtual void Parsing()
        {
            Parallel.For(0, MaxIteration, Parse);
        }

        public virtual void Parse(int id)
        {

        }

        public void SetRequestProperty(ref HttpWebRequest request)
        {
            request.KeepAlive = true;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/38.0.2125.104 Safari/537.36";
            request.Referer = "http://www.kinopoisk.ru";
            request.CookieContainer = new CookieContainer(10, 10, 10);
        }

        public virtual bool Save(byte[] bytes, string path)
        {
            try
            {
                var destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                File.WriteAllBytes(destPath, bytes);
            }
            catch (Exception exception)
            {
                return false;
            }
            return true;
        }
    }
}
