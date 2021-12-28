using System.Collections.Generic;
using Rhino.Geometry;

namespace KarambaConnect
{
    public static class Convert
    {
        public static STBDotNet.Geometry.Point3 ToStb(this Karamba.Geometry.Point3 kpt)
        {
            return new STBDotNet.Geometry.Point3(kpt.X, kpt.Y, kpt.Z) / 1000;
        }

        public static Karamba.Geometry.Point3 ToKaramba(this STBDotNet.Geometry.Point3 spt)
        {
            return new Karamba.Geometry.Point3(spt.X, spt.Y, spt.Z) * 1000;
        }

        public static Karamba.Geometry.Point3 ToKaramba(this STBDotNet.v202.StbNode sNode)
        {
            return new Karamba.Geometry.Point3(sNode.X, sNode.Y, sNode.Z) * 1000;
        }

        public static Point3d ToRhino(this STBDotNet.v202.StbNode node)
        {
            return new Point3d(node.X, node.Y, node.Z) / 1000;
        }

        public static STBDotNet.v202.StbNode[] ToStb(this List<Karamba.Nodes.Node> kNodes)
        {
            var sNodes = new STBDotNet.v202.StbNode[kNodes.Count];

            for (int i = 0; i < kNodes.Count; i++)
            {
                Karamba.Nodes.Node kNode = kNodes[i];
                var sNode = new STBDotNet.v202.StbNode
                {
                    id = (kNode.ind + 1).ToString(),
                    X = kNode.pos.X * 1000,
                    Y = kNode.pos.Y * 1000,
                    Z = kNode.pos.Z * 1000
                };
                sNodes[i] = sNode;
            }

            return sNodes;
        }

        public static List<Karamba.Nodes.Node> ToKaramba(this List<STBDotNet.v202.StbNode> sNodes)
        {
            var kNodes = new List<Karamba.Nodes.Node>();

            foreach (STBDotNet.v202.StbNode sNode in sNodes)
            {
                var kNode = new Karamba.Nodes.Node
                {
                    ind = int.Parse(sNode.id),
                    is_visible = true,
                    pos = sNode.ToKaramba()
                };

                kNodes.Add(kNode);
            }

            return kNodes;
        }
    }
}
