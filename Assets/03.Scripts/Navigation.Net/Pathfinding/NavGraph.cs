using System;
using System.Collections.Generic;
using TriangleNet.Geometry;

namespace Navigation.Net.Pathfinding
{
    public class NavGraph
    {
        private List<INavNode> _nodes = new();
    
        public List<INavNode> Nodes => _nodes;

        public ConvexNode AddConvexNode(Polygon polygon)
        {
            var node = new ConvexNode(polygon);
            _nodes.Add(node);
            return node;
        }

        public LinkNode AddLinkNode(Vertex point, double cost)
        {
            var node = new LinkNode(point, cost);
            _nodes.Add(node);
            return node;
        }

        public bool AddConnection(INavNode node1, INavNode node2)
        {
            if (node1 == null || node2 == null)
                throw new NullReferenceException("[NavGraph] One or All of nodes to connect is null");
        
            if (!_nodes.Contains(node1) || !_nodes.Contains(node2))
                return false;
        
            node1.AddConnection(node2);
            node2.AddConnection(node1);
            return true;
        }

        public ConvexNode? GetNodeOfArea(Point point)
        {
            if(_nodes == null || _nodes.Count == 0)
                throw new Exception("[NavGraph] Cannot find nearest node in empty NavGarph");

            foreach (var node in _nodes)
            {
                if(node is ConvexNode convexNode && convexNode.IsBoundaryContains(point))
                    return convexNode;
            }
        
            return null;
        }
    }
}