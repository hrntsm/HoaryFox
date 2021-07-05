using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component_v2.Utils.Geometry
{
    public class CreateLineFromStb
    {
        private readonly ST_BRIDGE _stBridge;
        private readonly StbNode[] _nodes;

        public CreateLineFromStb(ST_BRIDGE stBridge)
        {
            _stBridge = stBridge;
            _nodes = stBridge.StbModel.StbNodes;
        }

        public List<Point3d> Nodes()
        {
            return _nodes.Select(node => new Point3d(node.X, node.Y, node.Z)).ToList();
        }

        public List<Line> Columns()
        {
            var lines = new List<Line>();

            foreach (StbColumn column in _stBridge.StbModel.StbMembers.StbColumns)
            {
                StbNode nodeBottom = _nodes.First(node => node.id == column.id_node_bottom);
                StbNode nodeTop = _nodes.First(node => node.id == column.id_node_top);

                lines.Add(LineFromStbNode(nodeBottom, nodeTop));
            }

            return lines;
        }

        public List<Line> Girders()
        {
            var lines = new List<Line>();

            foreach (StbGirder girder in _stBridge.StbModel.StbMembers.StbGirders)
            {
                StbNode nodeStart = _nodes.First(node => node.id == girder.id_node_start);
                StbNode nodeEnd = _nodes.First(node => node.id == girder.id_node_end);

                lines.Add(LineFromStbNode(nodeStart, nodeEnd));
            }

            return lines;
        }

        public List<Line> Posts()
        {
            var lines = new List<Line>();

            foreach (StbPost post in _stBridge.StbModel.StbMembers.StbPosts)
            {
                StbNode nodeBottom = _nodes.First(node => node.id == post.id_node_bottom);
                StbNode nodeTop = _nodes.First(node => node.id == post.id_node_top);

                lines.Add(LineFromStbNode(nodeBottom, nodeTop));
            }

            return lines;
        }

        public List<Line> Beams()
        {
            var lines = new List<Line>();

            foreach (StbBeam beam in _stBridge.StbModel.StbMembers.StbBeams)
            {
                StbNode nodeStart = _nodes.First(node => node.id == beam.id_node_start);
                StbNode nodeEnd = _nodes.First(node => node.id == beam.id_node_end);

                lines.Add(LineFromStbNode(nodeStart, nodeEnd));
            }

            return lines;
        }

        public List<Line> Braces()
        {
            var lines = new List<Line>();

            foreach (StbBrace brace in _stBridge.StbModel.StbMembers.StbBraces)
            {
                StbNode nodeStart = _nodes.First(node => node.id == brace.id_node_start);
                StbNode nodeEnd = _nodes.First(node => node.id == brace.id_node_end);

                lines.Add(LineFromStbNode(nodeStart, nodeEnd));
            }

            return lines;
        }

        private static Line LineFromStbNode(StbNode from, StbNode to)
        {
            var ptFrom = new Point3d(from.X, from.Y, from.Z);
            var ptTo = new Point3d(to.X, to.Y, to.Z);

            return new Line(ptFrom, ptTo);
        }

    }
}
