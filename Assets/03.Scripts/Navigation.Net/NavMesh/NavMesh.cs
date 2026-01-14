using System.Collections.Generic;
using Navigation.Net.Pathfinding;
using TriangleNet.Geometry;
using TriangleNet.Meshing;

namespace Navigation.Net
{
    public class NavMesh
    {
        private Polygon _input;
        private List<Polygon> _mergedPolygons;
        private NavGraph _navGraph;
    
        internal Polygon Input => _input;
        internal List<Polygon> MergedPolygons => _mergedPolygons;
        internal NavGraph NavGraph
        {
            get
            {
                if(_navGraph == null)
                    BuildNavMesh();
                return _navGraph;
            }
        }

        public NavMesh()
        {
            _input = new Polygon();
            _mergedPolygons = new List<Polygon>();
            _navGraph = new NavGraph();
        }
    
        /// Add walkable area to NavMesh. points must be sorted CCW
        public void AddSurface(List<Point> points)
        {
            _input.Add(PointsToContour(points));
        }

        /// Add obstacle to NavMesh. points must be sorted CCW
        public void AddObstacle(List<Point> points)
        {
            _input.Add(PointsToContour(points), hole: true);
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

        public void BuildNavMesh()
        {
            var quality = new QualityOptions() { MaximumAngle = 90.0 };
            var mesh = _input.Triangulate(quality);
        
            _mergedPolygons = HertelMehlhorn.Run(mesh);
        
            _navGraph = GraphBuilder.BuildNavGraph(_mergedPolygons);
        }
    }
}