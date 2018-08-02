using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redirects
{
    class Program
    {
        static int Main( string[] args )
        {
            IEnumerable<string> routes = args;

            RouteAnalyzer redirects = new Redirects();
            Console.Out.Write( redirects.Process( routes ) );
            return 0;
        }
    }
}
