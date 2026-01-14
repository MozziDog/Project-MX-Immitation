using System;
using System.Collections.Generic;
using Navigation.Net.Math;
using Navigation.Net.Pathfinding;
using TriangleNet.Geometry;

namespace Navigation.Net
{
    public class Pathfind
    {
        public static List<Point> Run(Point src, Point dst, NavMesh navMesh)
        {
            return Run(src, dst, navMesh.NavGraph);
        }
    
        public static List<Point> Run(Point src, Point dst, NavGraph graph)
        {
            var firstNode = graph.GetNodeOfArea(src);
            var lastNode = graph.GetNodeOfArea(dst);

            if (firstNode == null || lastNode == null)
            {
                Console.WriteLine("src or/and dst is out of NavMesh");
                return null;
            }
        
            var path = AStar(firstNode, lastNode, graph);
            var stringPulled = StringPull(src, dst, path);

            return stringPulled;
        }

        private static List<NavNode> AStar(NavNode start, NavNode end, NavGraph graph)
        {
            if(start == end)
                return new List<NavNode>();
            
            PriorityQueue<NavNode, double> pq = new();
            HashSet<NavNode> visited = new();
            Dictionary<NavNode, NavNode> parent = new();
        
            pq.Enqueue(start, 0);

            while (pq.Count > 0)
            {
                pq.TryPeek(out var cur, out var cost);
                pq.Dequeue();
                if (cur == end)
                    break;


                foreach (var next in cur.Adj)
                {
                    if (visited.Contains(next))
                        continue;
                
                    var g = Distance(cur, next);
                    var h = Distance(next, end);
                    pq.Enqueue(next, g + h);
                    visited.Add(next);
                    parent.Add(next, cur);
                }
            }

            // Failed to find path
            if (!visited.Contains(end))
                return null;

            var ret = new List<NavNode>();
            var n = end;
            while (n != start)
            {
                ret.Add(n);
                n = parent[n];
            }
            ret.Add(n);
            ret.Reverse();

            return ret;
        }

        private static List<Point> StringPull(Point start, Point end, List<NavNode> nodes)
        {
            // Get portals
            var portals = new List<ISegment>();
            for (int i = 0; i < nodes.Count-1; i++)
            {
                var p = HertelMehlhorn.GetSharedEdge(nodes[i].Boundary, nodes[i + 1].Boundary);
                if (p == null)
                    throw new Exception("[Pathfind] Cannot find shared edge from neighboring polygons");
            
                portals.Add(p);
            }
        
            var path = new List<Point>();
            path.Add(start);
        
            var portalApex = start;
            var portalLeft = start;
            var portalRight = start;

            int leftIndex = 0;
            int rightIndex = 0;

            for (int i = 0; i < nodes.Count; i++)
            {
                // Get left and right point of portal
                // NOTE: Vertex of segment are already sorted as CCW
                var left = (i == nodes.Count - 1) ? end : portals[i].GetVertex(1);
                var right = (i == nodes.Count - 1) ? end : portals[i].GetVertex(0);
            
                var toPortalLeft = new Vector2Double(portalApex, portalLeft);
                var toPortalRight = new Vector2Double(portalApex, portalRight);
                var toLeft = new Vector2Double(portalApex, left);
                var toRight = new Vector2Double(portalApex, right);
            
                // Update Right side
                if (Vector2Double.Cross(toPortalRight, toRight) >= 0f)
                {
                    if (toPortalLeft.sqrMagnitude < 0.0001f || 
                        Vector2Double.Cross(toPortalLeft, toRight) < 0f)
                    {
                        // Narrows right side
                        portalRight = right;
                        rightIndex = i;
                    }
                    else
                    {
                        // When left and right side of funnel cross over,
                        // Add portalLeft to Path and restart funnel from there.
                        path.Add(portalLeft);
                        portalApex = portalLeft;
                        portalRight = portalApex;
                        portalLeft = portalApex;
                        rightIndex = leftIndex;
                        i = rightIndex;
                        continue;
                    }
                }
            
                // Update Left side
                if (Vector2Double.Cross(toPortalLeft, toLeft) <= 0)
                {
                    if (toPortalRight.sqrMagnitude < 0.0001f || 
                        Vector2Double.Cross(toPortalRight, toLeft) > 0f)
                    {
                        // Narrows right side
                        portalLeft = left;
                        leftIndex = i;
                    }
                    else
                    {
                        // when 'left' and 'right' cross, add right point to the path
                        path.Add(portalRight);
                        portalApex = portalRight;
                        portalLeft = portalApex;
                        portalRight = portalApex;
                        leftIndex = rightIndex;
                        i = leftIndex;
                        continue;
                    }
                }
            }
        
            path.Add(end);
            return path;
        }

        private static double Distance(NavNode a, NavNode b)
        {
            var dx = a.Point.x - b.Point.x;
            var dy = a.Point.y - b.Point.y;
            return System.Math.Sqrt(dx * dx + dy * dy);
        }
    }
}