using System;
using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 엄폐물.
    /// 캐릭터가 엄폐하거나 뛰어넘는 중에는 엄폐물을 '점유'한 것으로 취급,
    /// '점유'된 중에는 다른 캐릭터의 길찾기 알고리즘에 경로로 고려되지 않도록 NavLink 비활성화
    /// </summary>
    [Serializable]
    public class ObstacleLogic
    {
        private List<Position2> _vertices;
        private Position2[] _coveringPoint;
        private Position2 _position;
        private Position2 _scale;
        private float _rotation;

        public List<Position2> Vertices => _vertices;
        public Position2[] CoveringPoint => _coveringPoint;
        public Position2 Position => _position;
        public Position2 Scale => _scale;
        public float Rotation => _rotation;
        bool _isOccupied = false;

        // 이벤트
        public Action<Position2> OnObstacleOccupied;
        public Action<Position2> OnObstacleUnoccupied;
        
        public bool isOccupied
        {
            get => _isOccupied;
            set
            {
                _isOccupied = value;
                if(_isOccupied)
                    OnObstacleOccupied?.Invoke(_coveringPoint[0]);
                else
                    OnObstacleUnoccupied?.Invoke(_coveringPoint[0]);
            }
        }

        public ObstacleLogic(ObstacleData data, Position2 position, float rotationDeg)
        {
            _position = position;
            _scale = new Position2(data.Width, data.Length);
            CalculateVertices(position, data.Width, data.Length, rotationDeg);
        }
        
        public void CalculateVertices(Position2 position, float width, float length, float rotationDeg)
        {
            float halfWidth = width * 0.5f;
            float halfHeight = length * 0.5f;

            float rad = rotationDeg * MathF.PI / 180;
            float cos = MathF.Cos(rad);
            float sin = MathF.Sin(rad);
            
            Position2[] cornersLocal = new Position2[]
            {
                new Position2(halfWidth, halfHeight),   // Top-Right
                new Position2(halfWidth, -halfHeight),  // Bottom-Right
                new Position2(-halfWidth, -halfHeight), // Bottom-Left
                new Position2(-halfWidth, halfHeight)   // Top-Left
            };

            List<Position2> cornersWorld = new (4);
            foreach (var p in cornersLocal)
            {
                var rotatedX = (p.x * cos) - (p.y * sin);
                var rotatedY = (p.x * sin) + (p.y * cos);

                cornersWorld.Add(new Position2(rotatedX + position.x, rotatedY + position.y));
            }
            _vertices = cornersWorld;

            var linkLocal = new Position2[]
            {
                new Position2(0, -halfHeight - 1.0f),
                new Position2(0, halfHeight + 1.0f)
            };
            
            var linkWorld = new Position2[2];
            for (int i=0; i<2; i++)
            {
                var p = linkLocal[i];
                
                var rotatedX = (p.x * cos) - (p.y * sin);
                var rotatedY = (p.x * sin) + (p.y * cos);

                linkWorld[i] = new Position2(rotatedX + position.x, rotatedY + position.y);
            }
            _coveringPoint = linkWorld;
        }

        /// 2개의 엄폐위치 중에 입력과 가까운 쪽을 리턴
        public Position2 GetNearCoveringPoint(Position2 curPos)
        {
            var dist0 = (_coveringPoint[0] - curPos).sqrMagnitude;
            var dist1 = (_coveringPoint[1] - curPos).sqrMagnitude;
            
            if(dist0 < dist1)
                return _coveringPoint[0];
            else
                return _coveringPoint[1];
        }

        /// 2개의 엄폐위치 중에 입력과 먼 쪽을 리턴
        public Position2 GetFarCoveringPoint(Position2 curPos)
        {
            var dist0 = (_coveringPoint[0] - curPos).sqrMagnitude;
            var dist1 = (_coveringPoint[1] - curPos).sqrMagnitude;
            
            if(dist0 < dist1)
                return _coveringPoint[1];
            else
                return _coveringPoint[0];
        }
    }
}