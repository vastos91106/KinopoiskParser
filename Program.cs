using System;
using System.Linq;
using System.Threading;

namespace ParseDemon
{
    public class Program
    {

        static void Main(string[] options)
        {
            ThreadPool.SetMinThreads(90, 90);
            ThreadPool.SetMaxThreads(100, 100);

            if (options.Length == 0)
            {
                Console.WriteLine("start parsing films");
                new ParseFilm().Parsing();
                Console.WriteLine("parsing films complete");

                Console.WriteLine("start parsing people");
                new ParsePeople().Parsing();
                Console.WriteLine("parsing people complete");
            }
            if (options.Any(param => param == "f"))
            {
                Console.WriteLine("start parsing films");
                new ParseFilm().Parsing();
                Console.WriteLine("parsing films complete");

            }
            if (options.Any(param => param == "p"))
            {
                Console.WriteLine("start parsing people");
                new ParsePeople().Parsing();
                Console.WriteLine("parsing people complete");
            }

        }
    }
}
