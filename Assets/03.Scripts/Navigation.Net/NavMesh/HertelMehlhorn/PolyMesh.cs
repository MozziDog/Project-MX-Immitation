using System.Collections.Generic;
using TriangleNet.Geometry;

namespace Navigation.Net
{
    public class PolyMesh
    {
        private readonly List<Vertex> vertices;
        private readonly List<ISegment> segments;
        private readonly List<List<Vertex>> polygons;

        public List<Vertex> Vertices => vertices;
        public List<ISegment> Segments => segments;
        public List<List<Vertex>> Polygons => polygons;
    
        public int VertexCount => vertices.Count;
        public int SegmentCount => segments.Count;
        public int PolygonCount => polygons.Count;

        public PolyMesh()
        {
            vertices = new List<Vertex>();
            segments = new List<ISegment>();
            polygons = new List<List<Vertex>>();
        }

        public PolyMesh(PolyMesh other)
        {
            vertices = other.Vertices;
            segments = other.Segments;
            polygons = other.Polygons;
        }
    }
}