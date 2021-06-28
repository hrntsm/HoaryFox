using System;
using System.Collections.Generic;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component_v2.Tag.Name
{
    internal static class Util
    {
        internal static Point3d GetFramePosition(string idStart, string idEnd, IEnumerable<StbNode> nodes)
        {
            var startNode = new StbNode();
            var endNode = new StbNode();

            // TODO: アルゴリズム直す
            foreach (StbNode n in nodes)
            {
                if (n.id == idStart)
                {
                    startNode = n;
                }
                else if (n.id == idEnd)
                {
                    endNode = n;
                }
            }

            return (new Point3d(
                (startNode.X + endNode.X) / 2.0,
                (startNode.Y + endNode.Y) / 2.0,
                (startNode.Z + endNode.Z) / 2.0
            ));
        }
    }
}
