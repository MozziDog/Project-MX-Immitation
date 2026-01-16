using System;
using System.Collections.Generic;
using Logic.Pathfind;

namespace Logic
{
    public class NavAgent
    {
        public List<Position2> _path;

        private NavigationSystem _navigation;
        private List<bool> _isNavLinkStart;
        private bool _isOnNavLink;

        public bool IsOnNavLink => _isOnNavLink;
        
        public NavAgent(NavigationSystem navi)
        {
            _navigation = navi;
            _isNavLinkStart = new ();
        }

        public bool CalculatePath(Position2 srcPos, Position2 destPos)
        {
            try
            {
                (_path, _isNavLinkStart) = _navigation.GetPath(srcPos, destPos);
                return true;
            }
            catch (Exception e)
            {
                // 경로 찾기 실패
                return false;
            }
        }

        public Position2 FollowPath(Position2 curPos, float stepLength)
        {
            for (int i = 0; i < _path.Count; i++)
            {
                float distToNextPoint = Position2.Distance(curPos, _path[i]);
                if (distToNextPoint < stepLength)
                {
                    stepLength -= distToNextPoint;
                    curPos = _path[i];
                }
                else
                {
                    curPos = Position2.MoveTowards(curPos, _path[i], stepLength);
                    _isOnNavLink = _isNavLinkStart[i];
                    break;
                }
            }
            return curPos;
        }
    }
}