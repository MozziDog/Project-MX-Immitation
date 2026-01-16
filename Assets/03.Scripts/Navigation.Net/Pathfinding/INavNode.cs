using System.Collections.Generic;
using TriangleNet.Geometry;

namespace Navigation.Net.Pathfinding
{
    public interface INavNode
    {
        /// <summary>
        /// The center of Node. <br/>
        /// Center of gravity of convex if it's Convex Node, <br/>
        /// or 'the point' if it's LinkNode
        /// </summary>
        Point Point { get; }

        List<INavNode> Adj { get; }
        bool AddConnection(INavNode neighbour);
    }
}