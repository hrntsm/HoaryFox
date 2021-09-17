using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component.Utils.Geometry
{
    public class CreateLineFromStb
    {
        private readonly StbMembers _members;
        private readonly StbNode[] _nodes;

        public CreateLineFromStb(ST_BRIDGE stBridge)
        {
            _members = stBridge.StbModel.StbMembers;
            _nodes = stBridge.StbModel.StbNodes;
        }

        public List<Point3d> Nodes()
        {
            return _nodes.Select(node => new Point3d(node.X, node.Y, node.Z)).ToList();
        }

        public GH_Structure<GH_Line> Columns()
        {
            var lines = new GH_Structure<GH_Line>();
            if (_members.StbColumns == null)
            {
                return lines;
            }

            foreach ((StbColumn member, int i) in _members.StbColumns.Select((member, index) => (member, index)))
            {
                StbNode nodeBottom = _nodes.First(node => node.id == member.id_node_bottom);
                StbNode nodeTop = _nodes.First(node => node.id == member.id_node_top);

                lines.Append(GH_LineFromStbNode(nodeBottom, nodeTop), new GH_Path(0, i));
            }

            return lines;
        }

        public GH_Structure<GH_Line> Girders()
        {
            var lines = new GH_Structure<GH_Line>();
            if (_members.StbGirders == null)
            {
                return lines;
            }

            foreach ((StbGirder member, int i) in _members.StbGirders.Select((member, index) => (member, index)))
            {
                StbNode nodeStart = _nodes.First(node => node.id == member.id_node_start);
                StbNode nodeEnd = _nodes.First(node => node.id == member.id_node_end);

                lines.Append(GH_LineFromStbNode(nodeStart, nodeEnd), new GH_Path(0, i));
            }

            return lines;
        }

        public GH_Structure<GH_Line> Posts()
        {
            var lines = new GH_Structure<GH_Line>();
            if (_members.StbPosts == null)
            {
                return lines;
            }

            foreach ((StbPost member, int i) in _members.StbPosts.Select((member, index) => (member, index)))
            {
                StbNode nodeBottom = _nodes.First(node => node.id == member.id_node_bottom);
                StbNode nodeTop = _nodes.First(node => node.id == member.id_node_top);

                lines.Append(GH_LineFromStbNode(nodeBottom, nodeTop), new GH_Path(0, i));
            }

            return lines;
        }

        public GH_Structure<GH_Line> Beams()
        {
            var lines = new GH_Structure<GH_Line>();
            if (_members.StbBeams == null)
            {
                return lines;
            }

            foreach ((StbBeam member, int i) in _members.StbBeams.Select((member, index) => (member, index)))
            {
                StbNode nodeStart = _nodes.First(node => node.id == member.id_node_start);
                StbNode nodeEnd = _nodes.First(node => node.id == member.id_node_end);

                lines.Append(GH_LineFromStbNode(nodeStart, nodeEnd), new GH_Path(0, i));
            }

            return lines;
        }

        public GH_Structure<GH_Line> Braces()
        {
            var lines = new GH_Structure<GH_Line>();
            if (_members.StbBraces == null)
            {
                return lines;
            }

            foreach ((StbBrace member, int i) in _members.StbBraces.Select((member, index) => (member, index)))
            {
                StbNode nodeStart = _nodes.First(node => node.id == member.id_node_start);
                StbNode nodeEnd = _nodes.First(node => node.id == member.id_node_end);

                lines.Append(GH_LineFromStbNode(nodeStart, nodeEnd), new GH_Path(0, i));
            }

            return lines;
        }

        private static GH_Line GH_LineFromStbNode(StbNode from, StbNode to)
        {
            var ptFrom = new Point3d(from.X, from.Y, from.Z);
            var ptTo = new Point3d(to.X, to.Y, to.Z);

            return new GH_Line(new Line(ptFrom, ptTo));
        }
    }
}
