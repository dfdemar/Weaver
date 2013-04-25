using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Weaver
{
    class Program
    {
        static void Main(string[] args)
        {
            Spider spider = new Spider();
            spider.Go();

            Console.WriteLine("Spider finished.");
            Console.ReadLine();
        }
    }
}
