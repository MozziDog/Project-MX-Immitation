using System;
using System.Collections.Generic;
using System.Linq;
using Navigation.Net.Math;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Topology;

namespace Navigation.Net
{
    public class HertelMehlhorn
    {
        public static List<Polygon> Run(IMesh input)
        {
            // Make Polygon List from Triangle list
            var polygons = new List<Polygon>();
            var adj = new Dictionary<Polygon, List<Polygon>>();
            foreach (var tri in input.Triangles)
            {
                var p = new Polygon(0);
                p.Add(new Contour(tri.vertices));
                polygons.Add(p);
                adj.Add(p, new List<Polygon>());
            }

            // Get Adjacency
            for (int i=0; i<polygons.Count; i++)
            {
                for (int j=i+1; j<polygons.Count; j++)
                {
                    var t1 = polygons[i];
                    var t2 = polygons[j];
                    if (t1 == t2) continue;
                    var sharedEdge = GetSharedEdge(t1, t2);
                
                    if (sharedEdge == null)
                        continue;
                
                    adj[t1].Add(t2);
                    adj[t2].Add(t1);
                }
            }

            var merged = false;
            do
            {
                merged = false;
            
                // Get a pair of polygon to merge
                Polygon? polygon1 = null;
                Polygon? polygon2 = null;
                Polygon? mergedConvex = null;
                foreach (var p1 in polygons)
                {
                    foreach (var p2 in adj[p1])
                    {
                        // Merge to polygon and check if It's convex or not
                        mergedConvex = MergePolygons(p1, p2);
                        if (!mergedConvex.isConvex()) continue;

                        merged = true;
                        polygon1 = p1;
                        polygon2 = p2;
                        break;
                    }

                    if (merged) break;
                }

                if (!merged) break;
                if (polygon1 == null || polygon2 == null)
                    throw new Exception("Cannot get polygons to merge");
                if(mergedConvex == null)
                    throw new Exception("Cannot get convex merged after merged proceed");
                
                // Calculate neighbors of merged polygon
                var mergedNeighbors = adj[polygon1]
                    .Union(adj[polygon2])
                    .Where(p => p != polygon1 && p != polygon2)
                    .ToList();

                // Update neighbor's adj list
                foreach (var neighbor in mergedNeighbors)
                {
                    var neighborList = adj[neighbor];

                    neighborList.Remove(polygon1);
                    neighborList.Remove(polygon2);
                    neighborList.Add(mergedConvex);
                }
                
                // Update Adj Dictionary
                adj[mergedConvex] = mergedNeighbors;
                adj.Remove(polygon1);
                adj.Remove(polygon2);
                
                // Remove merged polygons and add result to list
                polygons.Add(mergedConvex);
                polygons.Remove(polygon1);
                polygons.Remove(polygon2);
            } while (merged);

            return polygons;
        }
        
        internal static Polygon MergePolygons(Polygon poly1, Polygon poly2)
        {
            var sharedEdge = GetSharedEdge(poly1, poly2);
            if(sharedEdge == null)
                throw new System.Exception("Polygons do not share an edge.");
            
            var vA = sharedEdge.GetVertex(0);
            var vB = sharedEdge.GetVertex(1);

            // Re-align points of contours
            var contour1 = ExtractPathCCW(poly1, vB, vA);
            var contour2 = ExtractPathCCW(poly2, vA, vB);

            Polygon result = new Polygon(0);

            // Add points but no last points of two contour (prevent duplication)
            for (int i = 0; i < contour1.Count - 1; i++)
                result.Add(contour1[i]);
            for (int i = 0; i < contour2.Count - 1; i++)
                result.Add(contour2[i]);
            
            // Reconnect segments
            for (int i = 0; i < result.Points.Count; i++)
            {
                var v0 = result.Points[i];
                var v1   = result.Points[(i + 1) % result.Points.Count];
                result.Add(new Segment(v0, v1));
            }
            return result;
        }
        
        private static List<Vertex> ExtractPathCCW(Polygon poly, Vertex start, Vertex end)
        {
            var result = new List<Vertex>();
            var pts = poly.Points;
            int n = pts.Count;

            // Swap if start is before end
            var startIndex = pts.FindIndex((v) => v == start);
            var endIndex = pts.FindIndex((v) => v == end);
            if ((startIndex + 1) % pts.Count == endIndex)
            {
                (startIndex, endIndex) = (endIndex, startIndex);
            }

            int i = startIndex;
            if (i < 0)
                throw new Exception("Start vertex not found.");

            do
            {
                result.Add(pts[i]);
                i = (i + 1) % n;
            } while (i != endIndex);
            result.Add(pts[endIndex]);

            return result;
        }

        #region Util

        public static ISegment? GetSharedEdge(Triangle t1, Triangle t2)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var s1 = t1.GetSegment(i);
                    var s2 = t2.GetSegment(j);
                    if (isEdgeSame(s1, s2))
                        return s1;
                }
            }
            
            return null;
        }
        
        public static ISegment? GetSharedEdge(Contour c1, Contour c2)
        {
            var segments1 = c1.GetSegments();
            var segments2 = c2.GetSegments();

            foreach (var s1 in segments1)
            {
                foreach (var s2 in segments2)
                {
                    if (isEdgeSame(s1, s2)) return s1;
                }
            }

            return null;
        }
        
        public static ISegment? GetSharedEdge(Polygon p1, Polygon p2)
        {
            foreach (var s1 in p1.Segments)
            {
                foreach (var s2 in p2.Segments)
                {
                    if (isEdgeSame(s1, s2)) return s1;
                }
            }

            return null;
        }

        private static bool isEdgeSame(ISegment? a, ISegment? b)
        {
            if(a == null || b == null) return false;
            return (a.P0 == b.P0 && a.P1 == b.P1)
                   || (a.P0 == b.P1 && a.P1 == b.P0);
        }

        #endregion
    }
}