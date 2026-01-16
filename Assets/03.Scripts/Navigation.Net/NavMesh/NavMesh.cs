using System.Collections.Generic;
using Navigation.Net.Pathfinding;
using TriangleNet.Geometry;
using TriangleNet.Meshing;

namespace Navigation.Net
{
    public class NavMesh
    {
        private Polygon _inputMesh;
        private List<Polygon> _mergedPolygons;
        private List<(Vertex, Vertex, double, double)> _links;
        private NavGraph _graph;
    
        internal Polygon InputMesh => _inputMesh;
        internal List<Polygon> MergedPolygons => _mergedPolygons;
        internal List<(Vertex, Vertex, double, double)> Links => _links;
        internal NavGraph Graph
        {
            get
            {
                if(_graph == null)
                    BuildNavMesh();
                return _graph;
            }
        }

        public NavMesh()
        {
            _inputMesh = new Polygon();
            _mergedPolygons = new List<Polygon>();
            _graph = new NavGraph();
            _links = new List<(Vertex, Vertex, double, double)>();
        }
    
        /// Add walkable area to NavMesh. points must be sorted CCW
        public void AddSurface(List<Point> points)
        {
            _inputMesh.Add(PointsToContour(points));
        }

        /// Add obstacle to NavMesh. points must be sorted CCW
        public void AddObstacle(List<Point> points)
        {
            _inputMesh.Add(PointsToContour(points), hole: true);
        }

        /// Add 2-way link to NavMesh 
        public void AddLink(Point src, Point dst, double cost)
        {
            _links.Add( (new Vertex(src.X, src.Y), new Vertex(dst.X, dst.Y), cost, cost) );

        }
        
        /// Add asymmetry link to NavMesh
        public void AddLink(Point src, Point dst, double costFromSrc, double costFromDst)
        {
            _links.Add( (new Vertex(src.X, src.Y), new Vertex(dst.X, dst.Y), costFromSrc, costFromDst) );
        }
        
        /// Add 1-way link to NavMesh
        public void AddOneWayLink(Point src, Point dst, double cost)
        {
            _links.Add( (new Vertex(src.X, src.Y), new Vertex(dst.X, dst.Y), cost, double.PositiveInfinity) );
        }

        public void SetLinkEnable(Point src, bool enable)
        {
            var node = _graph.Nodes.Find(x => { return x.Point == src; });
            if (node is LinkNode linkNode)
                SetLinkEnable(linkNode, enable);
        }

        public void SetLinkEnable(LinkNode linkNode, bool enable)
        {
            linkNode.Enabled = enable;
            linkNode.Pair.Enabled = enable;
        }
        
        public void BuildNavMesh()
        {
            var quality = new QualityOptions() { MaximumAngle = 90.0 };
            var mesh = _inputMesh.Triangulate(quality);
            _mergedPolygons = HertelMehlhorn.Run(mesh);
        
            _graph = GraphBuilder.BuildNavGraph(_mergedPolygons, _links);
        }
        
        private Contour PointsToContour(List<Point> points)
        {
            var vertices = new List<Vertex>();
            foreach (var point in points)
            {
                vertices.Add(new Vertex(point.X, point.Y));
            }
            return new Contour(vertices, 0);
        }
    }
}