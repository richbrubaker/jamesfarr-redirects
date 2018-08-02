using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redirects
{
    public interface RouteAnalyzer
    {
        IEnumerable<string> Process( IEnumerable<string> routes );
    }
}
