using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redirects
{
    public class Route
    {
        public string source;
        public string destination;
    }



    public class Redirects: RouteAnalyzer
    {
        public IEnumerable<string> Process( IEnumerable<string> routes )
        {
            List<Route> routeList = new List<Route>();

            foreach( string route in routes )
            {
                List<string> temp = route.Split(new string[] { "->" }, StringSplitOptions.None).ToList<string>();

                string source = temp[0].Trim();

                string destination = null;
                if (temp.Count() > 1)
                    destination = temp[1]?.Trim();

                Route newRoute = new Route();
                newRoute.source = source;
                newRoute.destination = destination;

                routeList.Add( newRoute );
            }



            List<string> newRoutes = new List<string>();

            int skipCount = 0;
            do
            {
                skipCount = 0;

                foreach( Route route in routeList ) {
                    // MARK: Check for circular references and throw an exception.
                    if( route.destination != null ) {
                        List<string> destinations = new List<string>();
                        destinations.Add( route.source );

                        string finalDest = route.destination;
                        do {
                            foreach( string routeString in destinations ) {
                                if( routeString.Equals( finalDest ) ) {
                                    throw new Exception( "RouteAnalyzer: Circular reference." );
                                }
                            }
                            destinations.Add( finalDest );


                            string nextDest = findDestination( finalDest, routeList );
                            if( nextDest == route.source ) {
                                throw new Exception( "RouteAnalyzer: Circular reference." );
                            }

                            finalDest = nextDest;
                        } while( finalDest != null );
                    }


                    bool skipped = true;

                    // Check if source already exists.
                    bool sourceExists = false;

                    foreach( string newRouteString in newRoutes ) {
                        List<string> newRouteStringElements = newRouteString.Split( new string[] { "->" }, StringSplitOptions.None ).ToList<string>();

                        foreach( string element in newRouteStringElements ) {
                            string tempElement = element.Trim();
                            if( tempElement.Equals( route.source ) ) {
                                sourceExists = true;
                                skipped = false;
                                break;
                            }
                        }

                        if( sourceExists )
                            break;
                    }

                    if( sourceExists )
                        continue;


                    // MARK: Look for base routes that don't redirect or for those that already exist in newRoutes array.

                    // MARK: Look for routes without destinations and add to newRoutes.
                    // If the destination doesn't exist and the source doesn't already exist then add source to newRoutes.
                    if( !sourceExists && route.destination == null ) {
                        newRoutes.Add( route.source );
                        sourceExists = true;
                        skipped = false;
                    }


                    // MARK: Check for end routes in destinations if destination is not null; checking first if the destination already exists and skipping if so.
                    if( route.destination != null ) {
                        // check to see if destination already exists
                        bool destinationExists = false;

                        foreach( string newRouteString in newRoutes ) {
                            List<string> newRouteStringElements = newRouteString.Split( new string[] { "->" }, StringSplitOptions.None ).ToList<string>();

                            foreach( string element in newRouteStringElements ) {
                                string tempElement = element.Trim();
                                if( tempElement.Equals( route.destination ) ) {
                                    destinationExists = true;
                                    skipped = false;
                                    break;
                                }
                            }

                            if( destinationExists )
                                break;
                        }


                        // Check for end routes in destinations
                        bool redirectDestination = false;

                        foreach( Route tRoute in routeList ) {
                            if( tRoute.source == route.destination ) {
                                redirectDestination = true;
                                break;
                            }

                        }

                        // If redirectDestination is false then add route.destination
                        if( !redirectDestination && !destinationExists ) {
                            newRoutes.Add( route.destination );
                            skipped = false;
                        }
                    }


                    // MARK: Check for sources that redirect to an existing destination and prepend to the newRoutes string if it doesn't already exist.
                    if( !sourceExists ) {
                        bool addedSource = false;

                        for( int index = 0; index < newRoutes.Count(); index++ ) {
                            string newRouteString = newRoutes[ index ];
                            List<string> newRouteStringElements = newRouteString.Split( new string[] { "->" }, StringSplitOptions.None ).ToList<string>();

                            foreach( string element in newRouteStringElements ) {
                                string tempElement = element.Trim();

                                if( tempElement.Equals( route.destination ) ) {
                                    newRoutes[ index ] = route.source + "->" + newRouteString;
                                    addedSource = true;
                                    skipped = false;
                                    break;
                                }
                            }

                            if( addedSource )
                                break;
                        }
                    }


                    // If skipped then increment skipCount.
                    if( skipped ) {
                        skipCount++;
                    }
                }
            }
            while( skipCount > 0 );

            return newRoutes;
        }



        private string findDestination( string dest, List<Route> routes )
        {
            foreach( Route route in routes ) {
                if( route.source.Equals( dest ) ) {
                    return route.destination;
                }
            }

            return null;
        }
    }
}
