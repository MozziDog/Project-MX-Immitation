using System;
using System.Collections.Generic;
using Navigation.Net.Pathfinding;
using TriangleNet.Geometry;

namespace Navigation.Net
{
    public class LinkNode : INavNode
    {
        private Vertex _vert;
        private List<INavNode> _adj;
        private LinkNode _pair;
        private double _cost;

        public Vertex Vertex => _vert;
        public Point Point => _vert as Point;
        public List<INavNode> Adj => _adj;
        /// Other side of NavLink
        public LinkNode Pair => _pair;
        /// Cost of using the link when enter to this linkNode
        public double Cost => _cost;
        /// Can use NavLink?
        public bool Enabled;

        public LinkNode(Vertex vert, double cost)
        {
            _vert = vert;
            _adj = new List<INavNode>();
            _cost = cost;
            Enabled = true;
        }
    
        public bool AddConnection(INavNode neighbour)
        {
            if (neighbour == null)
                throw new NullReferenceException("[NavNode] neighbour is null");

            if (_adj.Contains(neighbour))
                return false;
        
            _adj.Add(neighbour);
            return true;
        }

        /// Mark 2 LinkNodes as pair NavLink
        public static void Tie(LinkNode node1, LinkNode node2)
        {
            node1._pair = node2;
            node2._pair = node1;
        }

        public static bool isPair(LinkNode node1, LinkNode node2)
        {
            return node1.Pair == node2;
        }
    }
}