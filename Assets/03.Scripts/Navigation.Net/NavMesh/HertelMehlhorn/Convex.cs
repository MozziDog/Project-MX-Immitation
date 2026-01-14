using Navigation.Net.Math;
using TriangleNet.Geometry;
using TriangleNet.Topology;

namespace Navigation.Net
{
    public static class Convex
    {
        public static bool isConvex(this Polygon polygon)
        {
            int count = polygon.Count;
            var points = polygon.Points;
            if(count < 3) return false;

            var v12 = new Vector2Double(points[count - 1].x - points[count - 2].x, points[count - 1].y - points[count - 2].y);
            var v23 = new Vector2Double(points[0].x - points[count - 1].x, points[0].y - points[count - 1].y);
            // isCounterClockwise
            bool isCcw = Vector2Double.Cross(v12, v23) < 0;

            v12 = v23;
            v23 = new Vector2Double(points[1].x - points[0].x, points[1].y - points[0].y);
            if(isCcw ^ (Vector2Double.Cross(v12, v23) < 0))
                return false;

            for(int i = 0; i < count - 2; i++)
            {
                v12 = v23;
                v23 = new Vector2Double(points[i + 2].x - points[i + 1].x, points[i + 2].y - points[i + 1].y);
                if (isCcw ^ (Vector2Double.Cross(v12, v23) < 0))
                    return false;
            }
            return true;
        }

        public static Point GetCenterOfMass(this Polygon polygon)
        {
            double A = 0;
            double Cx = 0, Cy = 0;
            var p = polygon.Points;
            int n = p.Count;

            for (int i = 0; i < n; i++) {
                int j = (i + 1) % n;
                double cross = p[i].x * p[j].y - p[j].x * p[i].y;
                A += cross;
                Cx += (p[i].x + p[j].x) * cross;
                Cy += (p[i].y + p[j].y) * cross;
            }

            A *= 0.5;
            Cx /= (6 * A);
            Cy /= (6 * A);

            return new Point(Cx, Cy);
        }
    }
}
