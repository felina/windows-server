using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestExecutable
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine("Invalid Arguments");
                //Console.ReadLine();
                return 1;
            }
            else
            {
                Console.WriteLine(new Random().NextDouble());
                //Console.ReadLine(); // Uncomment me to get the window to stay open until you hit enter!
                return 0;
            }
        }
    }
}
