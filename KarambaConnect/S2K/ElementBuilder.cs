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
        public static List<BuilderBeam> BuilderBeams(StbModel model, List<string>[] k3dIds)
        {
            var k3dBeams = new List<BuilderBeam>();
            StbNode[] nodes = model.StbNodes;
            StbMembers members = model.StbMembers;

            k3dBeams.AddRange(StbColumnToK3dBeam(k3dIds[0], nodes, members.StbColumns));
            k3dBeams.AddRange(StbGirderToK3dBeam(k3dIds[1], nodes, members.StbGirders));
            k3dBeams.AddRange(StbBraceToK3dBeam(k3dIds[2], nodes, members.StbBraces));

            return k3dBeams;
        }

        private static List<BuilderBeam> StbColumnToK3dBeam(List<string> k3Ids, StbNode[] nodes, IEnumerable<StbColumn> columns)
        {
            if (columns == null)
            {
                return new List<BuilderBeam>();
            }

            var k3dLine3s = new List<Line3>();
            var k3dKit = new KarambaCommon.Toolkit();
            foreach (StbColumn column in columns)
            {
                StbNode nodeStart = nodes.First(node => node.id == column.id_node_bottom);
                StbNode nodeEnd = nodes.First(node => node.id == column.id_node_top);
                var ptFrom = new Point3(nodeStart.X / 1000d, nodeStart.Y / 1000d, nodeStart.Z / 1000d);
                var ptTo = new Point3(nodeEnd.X / 1000d, nodeEnd.Y / 1000d, nodeEnd.Z / 1000d);
                k3dLine3s.Add(new Line3(ptFrom, ptTo));
            }

            return k3dKit.Part.LineToBeam(k3dLine3s, k3Ids, new List<CroSec>(), new MessageLogger(), out _, true);
        }

        private static List<BuilderBeam> StbGirderToK3dBeam(List<string> k3Ids, StbNode[] nodes, IEnumerable<StbGirder> girders)
        {
            if (girders == null)
            {
                return new List<BuilderBeam>();
            }

            var k3dLine3s = new List<Line3>();
            var k3dKit = new KarambaCommon.Toolkit();
            foreach (StbGirder girder in girders)
            {
                StbNode nodeStart = nodes.First(node => node.id == girder.id_node_start);
                StbNode nodeEnd = nodes.First(node => node.id == girder.id_node_end);
                var ptFrom = new Point3(nodeStart.X / 1000d, nodeStart.Y / 1000d, nodeStart.Z / 1000d);
                var ptTo = new Point3(nodeEnd.X / 1000d, nodeEnd.Y / 1000d, nodeEnd.Z / 1000d);
                k3dLine3s.Add(new Line3(ptFrom, ptTo));
            }

            return k3dKit.Part.LineToBeam(k3dLine3s, k3Ids, new List<CroSec>(), new MessageLogger(), out _, true);
        }

        private static List<BuilderBeam> StbBraceToK3dBeam(List<string> k3Ids, StbNode[] nodes, IEnumerable<StbBrace> braces)
        {
            if (braces == null)
            {
                return new List<BuilderBeam>();
            }

            var k3dLine3s = new List<Line3>();
            var k3dKit = new KarambaCommon.Toolkit();
            foreach (StbBrace brace in braces)
            {
                StbNode nodeStart = nodes.First(node => node.id == brace.id_node_start);
                StbNode nodeEnd = nodes.First(node => node.id == brace.id_node_end);
                var ptFrom = new Point3(nodeStart.X / 1000d, nodeStart.Y / 1000d, nodeStart.Z / 1000d);
                var ptTo = new Point3(nodeEnd.X / 1000d, nodeEnd.Y / 1000d, nodeEnd.Z / 1000d);
                k3dLine3s.Add(new Line3(ptFrom, ptTo));
            }

            return k3dKit.Part.LineToBeam(k3dLine3s, k3Ids, new List<CroSec>(), new MessageLogger(), out _, true);
        }
    }
}
