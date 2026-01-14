using System.Collections.Generic;
using TriangleNet.Geometry;
using System.Linq;

namespace Navigation.Net.Pathfinding
{
    public static class GraphBuilder
    {
        public static NavGraph BuildNavGraph(List<Polygon> polygons)
        {
            var navGraph = new NavGraph();

            List<Point> points = new();
            foreach (var poly in polygons)
            {
                var node = navGraph.AddNode(poly);
                points.Add(node.Point);
            }
        
            for(int i=0; i<polygons.Count; i++)
            {
                for (int j = 0; j < polygons.Count; j++)
                {
                    if (i == j) continue;

                    if (HertelMehlhorn.GetSharedEdge(polygons[i], polygons[j]) != null)
                    {
                        navGraph.AddConnection(points[i], points[j]);
                    }
                }
            }

            return navGraph;
        }
    }
}