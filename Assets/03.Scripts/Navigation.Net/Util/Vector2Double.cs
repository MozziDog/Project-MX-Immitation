using System;
using System.Collections.Generic;
using System.Text;
using TriangleNet.Geometry;

namespace Navigation.Net.Math
{
    public class Vector2Double
    {
        public double x;
        public double y;

        public double magnitude => System.Math.Sqrt(sqrMagnitude);
        public double sqrMagnitude => x * x + y * y;
        public Vector2Double normalized { 
            get
            {
                Vector2Double ret = new Vector2Double(this);
                return ret.normalize();
            } 
        }

        public Vector2Double(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2Double(Point a)
        {
            x = a.x;
            y = a.y;
        }

        public Vector2Double(Point from, Point to)
        {
            x = to.x - from.x;
            y = to.y - from.y;
        }

        public Vector2Double(Vector2Double from)
        {
            this.x = from.x;
            this.y = from.y;
        }

        public Vector2Double normalize()
        {
            double magnitude = this.magnitude;
            x /= magnitude;
            y /= magnitude;
            return this;
        }

        public static Vector2Double operator +(Vector2Double v1, Vector2Double v2)
        {
            return new Vector2Double(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector2Double operator -(Vector2Double v1, Vector2Double v2)
        {
            return new Vector2Double(v1.x - v2.x, v1.y - v2.y);
        }

        public static double Dot(Vector2Double a, Vector2Double b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static double Cross(Vector2Double a, Vector2Double b)
        {
            return a.x * b.y - b.x * a.y;
        }
    }
}
