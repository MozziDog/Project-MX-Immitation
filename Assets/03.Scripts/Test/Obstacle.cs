using System.Collections.Generic;
using TriangleNet.Geometry;
using UnityEngine;

namespace _03.Scripts.Test
{
    public class Obstacle
    {
        private Vector2 _position;
        private Vector2 _scale;
        private float _rotation;
        private List<Point> _vertices;
        private List<Point> _link;
        
        public List<Point> Vertices => _vertices;
        public List<Point> Link => _link;
        
        public Obstacle(Vector2 position, Vector2 scale, float rotation)
        {
            _position = position;
            _scale = scale;
            _rotation = rotation;
            CalculateVertices(position, scale, rotation);
        }
        
        public void CalculateVertices(Vector2 position, Vector2 scale, float rotationDeg)
        {
            float halfWidth = scale.x / 2;
            float halfHeight = scale.y / 2;

            float rad = rotationDeg * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            
            Point[] cornersLocal = new Point[]
            {
                new Point(halfWidth, halfHeight),   // Top-Right
                new Point(halfWidth, -halfHeight),  // Bottom-Right
                new Point(-halfWidth, -halfHeight), // Bottom-Left
                new Point(-halfWidth, halfHeight)   // Top-Left
            };

            List<Point> cornersWorld = new (4);
            foreach (var p in cornersLocal)
            {
                var rotatedX = (p.X * cos) - (p.Y * sin);
                var rotatedY = (p.X * sin) + (p.Y * cos);

                cornersWorld.Add(new Point(rotatedX + position.x, rotatedY + position.y));
            }
            _vertices = cornersWorld;

            Point[] linkLocal = new Point[]
            {
                new Point(0, -halfHeight - 1.0),
                new Point(0, halfHeight + 1.0)
            };
            
            List<Point> linkWorld = new (2);
            foreach (var p in linkLocal)
            {
                var rotatedX = (p.X * cos) - (p.Y * sin);
                var rotatedY = (p.X * sin) + (p.Y * cos);

                linkWorld.Add(new Point(rotatedX + position.x, rotatedY + position.y));
            }
            _link = linkWorld;
        }
    }
}