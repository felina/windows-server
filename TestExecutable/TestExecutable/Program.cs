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
                return 1;
            }
            else
            {
                Console.WriteLine(0.75);
                return 0;
            }
        }
    }
}
