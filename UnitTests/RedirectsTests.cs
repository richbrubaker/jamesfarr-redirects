using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redirects;

namespace UnitTests
{
    [TestClass]
    public class RedirectsTests
    {
        [TestMethod]
        public void RouteAnalyzerImplemenation_ShouldThrowExceptionOnCircularReference()
        {
            Redirects.RouteAnalyzer redirects = new Redirects.Redirects();

            Exception exception = null;

            try
            {
                IEnumerable<string> results = redirects.Process(new List<string> { "/home", "/our-ceo.html -> /about-us.html", "/about-us.html -> /about", "/product-1.html -> /seo", "/about -> /about-us.html" });
            }
            catch( Exception e )
            {
                exception = e;
            }

            Assert.IsNotNull( exception, "RouteAnalzyer implementation: Expected circular reference in routes to throw an exception.");
        }



        [TestMethod]
        public void RouteAnalyzerImplemenation_ShouldConsolidateAndRedirectRoutes()
        {
            Redirects.RouteAnalyzer redirects = new Redirects.Redirects();

            List<string> results = redirects.Process( new List<string> { "/home", "/our-ceo.html -> /about-us.html", "/location.html -> /about-us.html", "/contact.html -> /location.html", "/about-us.html -> /about", "/product-1.html -> /seo", "/about-our-company -> /about" } ).ToList<string>();

            Console.Out.WriteLine(  );
            Assert.AreEqual( 3, results.Count(), "RouteAnalzyer implementation: Expected routes to be consolidated." );

            bool home = false;
            if( results.Contains( "/home" ) )
                home = true;
            Assert.IsTrue( home, "RouteAnalzyer implementation: Expected routes to contain /home.");


            List<string> routes = "/our-ceo.html -> /location.html -> /about-us.html -> /contact.html -> /about-our-company -> /about".Split( new string[] { " -> " }, StringSplitOptions.None ).ToList<string>();
            foreach( string route in routes ) {
                string tempRoute = route.Trim();

                List<string> resultRoutes = results[2].Split( new string[] { " -> " }, StringSplitOptions.None ).ToList<string>();

                bool exists = false;

                foreach( string resultRoute in resultRoutes ) {
                    string tempResultRoute = resultRoute.Trim();
                    if( tempRoute.Equals( tempRoute ) ) {
                        exists = true;
                        break;
                    }
                }

                if( !exists )
                    throw new Exception( "RouteAnalzyer implementation: Expected routes to contain /about." );
            }


            bool seo = false;
            if( results.Contains( "/product-1.html -> /seo" ) )
                seo = true;
            Assert.IsTrue( seo, "RouteAnalzyer implementation: Expected routes to contain /seo.");
        }

        

        [TestMethod]
        public void RouteAnalyzerImplemenation_ShouldRemoveDuplicates()
        {
            Redirects.RouteAnalyzer redirects = new Redirects.Redirects();

            IEnumerable<string> results = redirects.Process(new List<string> { "/home", "/our-ceo.html -> /about-us.html", "/about-us.html -> /about", "/about-us.html -> /about", "/product-1.html -> /seo", "/about" } );

            bool duplicates = false;

            HashSet<string> set = new HashSet<string>();

            foreach( string route in results )
            {
                if( !set.Add( route ) )
                {
                    duplicates = true;
                    break;
                }
            }

            Assert.IsFalse( duplicates, "RouteAnalzyer implementation: Expected duplicates to be removed.");
        }
    }
}
