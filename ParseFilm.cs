using System;
using System.IO;
using System.Net;
using System.Threading;
using RestSharp.Extensions;

namespace ParseDemon
{
    public class ParseFilm : ParseBase
    {
        public override void Parse(int filmId)
        {
            try
            {
                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"parsedfilm/film{filmId}.dat")))
                {
                    return;
                }

                Thread.Sleep(10000);
                Console.WriteLine($"parse film {filmId}");

                if (ParsedItems[filmId])
                {
                    return;
                }

                var legacyRequest = (HttpWebRequest)WebRequest.Create("http://www.kinopoisk.ru/film/" + filmId);
                CurentIp = ProxyList[new Random().Next(0, ProxyList.Count)];

                legacyRequest.Proxy = new WebProxy(CurentIp);
                Console.WriteLine($"proxy {CurentIp},film:{filmId}");

                SetRequestProperty(ref legacyRequest);

                var legacyResponse = (HttpWebResponse)legacyRequest.GetResponse();

                if (legacyResponse.ResponseUri.AbsoluteUri == "http://www.kinopoisk.ru/error/?ht=4" || legacyResponse.ResponseUri.AbsoluteUri == "http://www.kinopoisk.ru/error/?ht=403")
                {
                    legacyResponse.Close();
                    Console.WriteLine($"kinopoisk block proxy/ url:{legacyResponse.ResponseUri.AbsoluteUri}");
                    Parse(filmId);
                }
                if (legacyResponse.ResponseUri.AbsoluteUri == "http://www.kinopoisk.ru/film/" + filmId)
                {
                    try
                    {
                        var bytes = legacyResponse.GetResponseStream().ReadAsBytes();

                        var path = $"parsedfilm/film{filmId}.dat";

                        var msg = Save(bytes, path) ?
                            $"film{filmId}:successfully  parsed" :
                            $"film{filmId}:not successfully  parsed";

                        Console.WriteLine(msg);
                        ParsedItems[filmId] = true;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine($"film {filmId}");
                        Console.WriteLine(exception.Message);
                        Parse(filmId);
                    }
                }
                legacyResponse.Close();
            }
            catch (WebException exception)
            {
                var status = exception.Response as HttpWebResponse;
                if (status != null && status.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"film:{filmId} not found");
                    Console.WriteLine(exception.Message);
                    ParsedItems[filmId] = true;
                }
                Console.WriteLine($"film {filmId}");
                Console.WriteLine(exception.Message);
                Parse(filmId);
            }
        }

    }
}
