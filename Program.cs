using System;
using System.Threading;

namespace ParseDemon
{
    public class Program
    {

        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(90, 90);
            ThreadPool.SetMaxThreads(100, 100);

            Console.WriteLine("start parsing films");
            new ParseFilm().Parsing();
            Console.WriteLine("parsing films complete");

            Console.WriteLine("start parsing people");
            new ParsePeople().Parsing();
            Console.WriteLine("parsing people complete");
        }
    }
}
