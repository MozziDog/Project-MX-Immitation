using System;
using System.Collections.Generic;
using TriangleNet.Geometry;

namespace Navigation.Net.Pathfinding
{
    public class NavNode
    {
        private Point _point;
        private readonly List<NavNode> _adj;
        private Polygon _boundary;
    
        /// <summary>
        /// The center of gravity of a convex
        /// </summary>
        public Point Point => _point;
        public List<NavNode> Adj => _adj;
        public Polygon Boundary => _boundary;
    
        public NavNode(Polygon poly)
        {
            _boundary = poly;
            _point = poly.GetCenterOfMass();
            _adj = new();
        }

        public bool AddConnection(NavNode neighbour)
        {
            if (neighbour == null)
                throw new NullReferenceException("[NavNode] neighbour is null");

            if (_adj.Contains(neighbour))
                return false;
        
            _adj.Add(neighbour);
            return true;
        }

        // _boundary.Points must be Counter-Clockwise-ordered
        public bool IsBoundaryContains(Point point)
        {
            int n = _boundary.Count;
            if (n < 3) return false;

            var polygon = _boundary.Points;
        
            if (cross_product(polygon[0], polygon[1], point) < 0) return false;
            if (cross_product(polygon[0], polygon[n - 1], point) > 0) return false;
        
            int left = 1, right = n - 1;
            while (left + 1 < right) {
                int mid = left + (right - left) / 2;
                if (cross_product(polygon[0], polygon[mid], point) >= 0) {
                    left = mid;
                } else {
                    right = mid;
                }
            }
        
            return cross_product(polygon[left], polygon[right], point) >= 0;
        }
    
        private double cross_product(Point a, Point b, Point c) {
            return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
        }
    }
}