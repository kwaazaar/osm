using OsmSharp;
using OsmSharp.Streams;
using System;
using System.IO;
using System.Linq;

namespace OsmFilter
{
    class Program
    {
        static void Main(string[] args)
        {
            // Download data from: http://download.geofabrik.de/europe/netherlands.html
            using (var fileStream = File.OpenRead(@"c:\temp\netherlands-latest.osm.pbf"))
            {
                // create source stream.
                var source = new PBFOsmStreamSource(fileStream);
                var nodes = source.EnumerateAndIgore(ignoreNodes: false, ignoreRelations: true, ignoreWays: true);

                for (var ni = 0; ni < 1000; ni++)
                {
                    var n = nodes.Skip(ni).First();
                    if (n.Tags.Count > 0)
                        Console.WriteLine($"Node {ni}, tags: { String.Join(',', n.Tags.Select(t => t.Key)) }");
                }

                //Console.WriteLine($"Node count: {nodes.Count()}");

                var first = true;
                var result = nodes.Where((n) =>
                {
                    var match = n.Tags.TryGetValue("amenity", out string tagValue);
                    if (match && first)
                    {
                        var node = n as Node;
                        first = false;
                        Console.WriteLine($"First match: {tagValue}, name: {n.Tags["name"]}, location: {node.Longitude}, {node.Latitude}");
                    }
                    return match;
                });

                Console.WriteLine($"Node matches: {result.Count()}");
                Console.ReadLine();
            }
        }
    }
}
