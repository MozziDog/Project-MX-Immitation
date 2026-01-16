using System.Collections.Generic;
using Navigation.Net;
using TriangleNet.Geometry;

namespace Logic.Pathfind
{
    public class NavigationSystem
    {
        private NavMesh _navMesh;
        public NavMesh NavMesh => _navMesh;
        
        public NavigationSystem(float fieldWidth, float fieldLength, List<ObstacleLogic> obstacles)
        {
            _navMesh = new NavMesh();
            _navMesh.AddSurface(CalculateField(fieldWidth, fieldLength));
            
            foreach (var obstacle in obstacles)
            {
                var verts = new List<Point>();
                foreach(var p in obstacle.Vertices)
                    verts.Add(ToPoint(p));
                _navMesh.AddObstacle(verts);

                var src = ToPoint(obstacle.CoveringPoint[0]);
                var dst = ToPoint(obstacle.CoveringPoint[1]);
                _navMesh.AddLink(src, dst, 2f);

                obstacle.OnObstacleOccupied += DisableNavLink;
                obstacle.OnObstacleOccupied += EnableNavLink;
            }
            
            _navMesh.BuildNavMesh();
        }

        public (List<Position2>, List<bool>) GetPath(Position2 start, Position2 end)
        {
            var src = ToPoint(start);
            var dst = ToPoint(end); 
            (var pathOfPoint, var isNavLinkStart) = Navigation.Net.Pathfind.Run(src, dst, _navMesh);
            var pathOfPosition = new List<Position2>();
            foreach (var p in pathOfPoint)
            {
                pathOfPosition.Add(ToPosition2(p));
            }

            return (pathOfPosition, isNavLinkStart);
        }
        
        private List<Point> CalculateField(float width, float length)
        {
            float halfWidth = width / 2;

            List<Point> corners = new();
            corners.Add(new Point(halfWidth, length));   // Top-Right
            corners.Add(new Point(halfWidth, -3));  // Bottom-Right
            corners.Add(new Point(-halfWidth, -3)); // Bottom-Left
            corners.Add(new Point(-halfWidth, length)); // Top-Left
            
            return corners;
        }
        
        private void EnableNavLink(Position2 coveringPoint)
        {
            _navMesh.SetLinkEnable(ToPoint(coveringPoint), true);
        }

        private void DisableNavLink(Position2 coveringPoint)
        {
            _navMesh.SetLinkEnable(ToPoint(coveringPoint), true);
        }

        private Point ToPoint(Position2 pos)
        {
            return new Point(pos.x, pos.y);
        }

        private Position2 ToPosition2(Point point)
        {
            return new Position2((float)point.X, (float)point.Y);
        }
    }
}