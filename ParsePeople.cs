using System;
using System.IO;
using System.Net;
using System.Threading;
using RestSharp.Extensions;

namespace ParseDemon
{
    internal class ParsePeople : ParseBase
    {
        public override void Parse(int id)
        {
            try
            {
                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"parsedpeople/people{id}.dat")))
                {
                    return;
                }

                Thread.Sleep(10000);
                Console.WriteLine($"parse people {id}");

                if (ParsedItems[id])
                {
                    return;
                }

                var legacyRequest = (HttpWebRequest)WebRequest.Create("http://www.kinopoisk.ru/name/" + id);
                CurentIp = ProxyList[new Random().Next(0, ProxyList.Count)];

                //legacyRequest.Proxy = new WebProxy(CurentIp);
                Console.WriteLine($"proxy {CurentIp},people:{id}");

                SetRequestProperty(ref legacyRequest);

                var legacyResponse = (HttpWebResponse)legacyRequest.GetResponse();

                if (legacyResponse.ResponseUri.AbsoluteUri == "http://www.kinopoisk.ru/error/?ht=4" || legacyResponse.ResponseUri.AbsoluteUri == "http://www.kinopoisk.ru/error/?ht=403")
                {
                    legacyResponse.Close();
                    Console.WriteLine($"kinopoisk block proxy/ url:{legacyResponse.ResponseUri.AbsoluteUri}");
                    Parse(id);
                }
                if (legacyResponse.ResponseUri.AbsoluteUri != "http://www.kinopoisk.ru/404/" && legacyResponse.ResponseUri.AbsoluteUri == "http://www.kinopoisk.ru/name/" + id)
                {
                    try
                    {
                        var bytes = legacyResponse.GetResponseStream().ReadAsBytes();

                        var path = $"parsedpeople/people{id}.dat";

                        var msg = Save(bytes, path) ?
                            $"people{id}:successfully  parsed" :
                            $"people{id}:not successfully  parsed";

                        Console.WriteLine(msg);
                        ParsedItems[id] = true;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine($"people {id}");
                        Console.WriteLine(exception.Message);
                        Parse(id);
                    }
                }
                legacyResponse.Close();
            }
            catch (WebException exception)
            {
                var status = exception.Response as HttpWebResponse;
                if (status != null && status.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"people:{id} not found");
                    Console.WriteLine(exception.Message);
                    ParsedItems[id] = true;
                }

                Console.WriteLine($"people {id}");
                Console.WriteLine(exception.Message);
                Parse(id);
            }
        }
    }
}
