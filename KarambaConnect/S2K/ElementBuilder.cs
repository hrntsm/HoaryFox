using System.Collections.Generic;
using System.Linq;
using Karamba.CrossSections;
using Karamba.Elements;
using Karamba.Geometry;
using Karamba.Utilities;
using STBDotNet.v202;

namespace KarambaConnect.S2K
{
    public static class ElementBuilder
    {
        public static List<BuilderBeam> BuilderBeams(ST_BRIDGE stBridge, List<string>[] k3Ids)
        {
            var k3d = new KarambaCommon.Toolkit();
            var elems = new List<BuilderBeam>();
            var k3Elems = new[] { new List<Line3>(), new List<Line3>() };
            StbNode[] nodes = stBridge.StbModel.StbNodes;
            StbMembers members = stBridge.StbModel.StbMembers;

            StbColumnToKarambaLine3(members.StbColumns, nodes, k3Elems);
            StbGirderToKarambaLine3(members.StbGirders, nodes, k3Elems);
            StbBraceToKarambaLine3(members.StbBraces, nodes, k3Elems);
            elems.AddRange(k3d.Part.LineToBeam(k3Elems[0], k3Ids[0], new List<CroSec>(), new MessageLogger(), out _, true));
            elems.AddRange(k3d.Part.LineToBeam(k3Elems[1], k3Ids[1], new List<CroSec>(), new MessageLogger(), out _, true));

            return elems;
        }

        private static void StbColumnToKarambaLine3(StbColumn[] columns, StbNode[] nodes, List<Line3>[] k3Elems)
        {
            foreach (StbColumn column in columns)
            {
                StbNode nodeStart = nodes.First(node => node.id == column.id_node_bottom);
                StbNode nodeEnd = nodes.First(node => node.id == column.id_node_top);
                var ptFrom = new Point3(nodeStart.X / 1000d, nodeStart.Y / 1000d, nodeStart.Z / 1000d);
                var ptTo = new Point3(nodeEnd.X / 1000d, nodeEnd.Y / 1000d, nodeEnd.Z / 1000d);
                k3Elems[0].Add(new Line3(ptFrom, ptTo));
            }
        }

        private static void StbGirderToKarambaLine3(StbGirder[] girders, StbNode[] nodes, List<Line3>[] k3Elems)
        {
            foreach (StbGirder girder in girders)
            {
                StbNode nodeStart = nodes.First(node => node.id == girder.id_node_start);
                StbNode nodeEnd = nodes.First(node => node.id == girder.id_node_end);
                var ptFrom = new Point3(nodeStart.X / 1000d, nodeStart.Y / 1000d, nodeStart.Z / 1000d);
                var ptTo = new Point3(nodeEnd.X / 1000d, nodeEnd.Y / 1000d, nodeEnd.Z / 1000d);
                k3Elems[0].Add(new Line3(ptFrom, ptTo));
            }
        }

        private static void StbBraceToKarambaLine3(StbBrace[] braces, StbNode[] nodes, List<Line3>[] k3Elems)
        {
            foreach (StbBrace brace in braces)
            {
                StbNode nodeStart = nodes.First(node => node.id == brace.id_node_start);
                StbNode nodeEnd = nodes.First(node => node.id == brace.id_node_end);
                var ptFrom = new Point3(nodeStart.X / 1000d, nodeStart.Y / 1000d, nodeStart.Z / 1000d);
                var ptTo = new Point3(nodeEnd.X / 1000d, nodeEnd.Y / 1000d, nodeEnd.Z / 1000d);
                k3Elems[1].Add(new Line3(ptFrom, ptTo));
            }
        }
    }
}
