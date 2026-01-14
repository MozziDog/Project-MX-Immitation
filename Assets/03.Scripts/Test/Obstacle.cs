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
        
        public List<Point> Vertices => _vertices;
        
        public Obstacle(Vector2 position, Vector2 scale, float rotation)
        {
            _position = position;
            _scale = scale;
            _rotation = rotation;
            _vertices = CalculateVertices(position, scale, rotation);
        }
        
        public static List<Point> CalculateVertices(Vector2 position, Vector2 scale, float rotationDeg)
        {
            float halfWidth = scale.x / 2;
            float halfHeight = scale.y / 2;

            Point[] cornersLocal = new Point[]
            {
                new Point(halfWidth, halfHeight),   // Top-Right
                new Point(halfWidth, -halfHeight),  // Bottom-Right
                new Point(-halfWidth, -halfHeight), // Bottom-Left
                new Point(-halfWidth, halfHeight)   // Top-Left
            };

            float rad = rotationDeg * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            List<Point> cornersWorld = new (4);

            foreach (var corner in cornersLocal)
            {
                // 회전 변환
                var rotatedX = (corner.X * cos) - (corner.Y * sin);
                var rotatedY = (corner.X * sin) + (corner.Y * cos);

                cornersWorld.Add(new Point(rotatedX + position.x, rotatedY + position.y));
            }

            return cornersWorld;
        }
    }
}