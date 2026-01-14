using System;
using System.Collections.Generic;
using TriangleNet.Geometry;

namespace Navigation.Net.Pathfinding
{
    public class NavGraph
    {
        private List<NavNode> _nodes = new();
        private List<Polygon> _polygons;
    
        public List<NavNode> Nodes => _nodes;

        public NavNode AddNode(Polygon polygon)
        {
            var node = new NavNode(polygon);
            _nodes.Add(node);
            return node;
        }

        public bool AddConnection(Point point1, Point point2)
        {
            var node1 = _nodes.Find((x) => x.Point == point1);
            var node2 = _nodes.Find((x) => x.Point == point2);
            return AddConnection(node1, node2);
        }

        public bool AddConnection(NavNode node1, NavNode node2)
        {
            if (node1 == null || node2 == null)
                throw new NullReferenceException("[NavGraph] One or All of nodes to connect is null");
        
            if (!_nodes.Contains(node1) || !_nodes.Contains(node2))
                return false;
        
            node1.AddConnection(node2);
            node2.AddConnection(node1);
            return true;
        }

        public NavNode? GetNodeOfArea(Point point)
        {
            if(_nodes == null || _nodes.Count == 0)
                throw new Exception("[NavGraph] Cannot find nearest node in empty NavGarph");

            foreach (var node in _nodes)
            {
                if (node.IsBoundaryContains(point))
                    return node;
            }
        
            return null;
        }
    }
}