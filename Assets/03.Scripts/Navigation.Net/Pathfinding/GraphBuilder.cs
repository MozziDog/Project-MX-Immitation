using System;
using System.Collections.Generic;
using TriangleNet.Geometry;
using System.Linq;
using System.Xml.Serialization;

namespace Navigation.Net.Pathfinding
{
    public static class GraphBuilder
    {
        public static NavGraph BuildNavGraph(List<Polygon> polygons, List<(Vertex, Vertex, double, double)>? links = null)
        {
            var navGraph = new NavGraph();
            
            // Add Nodes for merged polygons
            List<ConvexNode> convexNodes = new();
            foreach (var poly in polygons)
            {
                var node = navGraph.AddConvexNode(poly);
                convexNodes.Add(node);
            }
        
            for(int i=0; i<polygons.Count; i++)
            {
                for (int j = 0; j < polygons.Count; j++)
                {
                    if (i == j) continue;

                    if (HertelMehlhorn.GetSharedEdge(polygons[i], polygons[j]) != null)
                    {
                        navGraph.AddConnection(convexNodes[i], convexNodes[j]);
                    }
                }
            }

            // Add Links to NavMesh
            if (links != null)
            {
                foreach (var link in links)
                {
                    (var src, var dst, var costFromSrc, var costFromDst) = link;
                    var srcNode = navGraph.AddLinkNode(src, costFromSrc);
                    var dstNode = navGraph.AddLinkNode(dst, costFromDst);
                    LinkNode.Tie(srcNode, dstNode); 
                    
                    var srcNeighbor = navGraph.GetNodeOfArea(src);
                    var dstNeighbor = navGraph.GetNodeOfArea(dst);
                    if (srcNeighbor == null || dstNeighbor == null)
                        throw new Exception("Both src and dst point of the link must be in NavMesh");
                    
                    navGraph.AddConnection(srcNode, srcNeighbor);
                    navGraph.AddConnection(dstNode, dstNeighbor);
                    navGraph.AddConnection(srcNode, dstNode);
                }
            }

            return navGraph;
        }
    }
}