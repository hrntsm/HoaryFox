using System;
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
        private readonly StbSections _sections;
        private readonly StbNode[] _nodes;
        private readonly bool _isOffset;

        public CreateLineFromStb(ST_BRIDGE stBridge, bool isOffset)
        {
            _members = stBridge.StbModel.StbMembers;
            _sections = stBridge.StbModel.StbSections;
            _nodes = stBridge.StbModel.StbNodes;
            _isOffset = isOffset;
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
                StbNode nodeBottom = new StbNode();
                StbNode nodeTop = new StbNode();

                StbNode nodeBottomBase = _nodes.First(node => node.id == member.id_node_bottom);
                StbNode nodeTopBase = _nodes.First(node => node.id == member.id_node_top);

                if (_isOffset)
                {
                    nodeBottom = new StbNode
                    {
                        X = nodeBottomBase.X + member.offset_bottom_X,
                        Y = nodeBottomBase.Y + member.offset_bottom_Y,
                        Z = nodeBottomBase.Z + member.offset_bottom_Z
                    };
                    nodeTop = new StbNode
                    {
                        X = nodeTopBase.X + member.offset_top_X,
                        Y = nodeTopBase.Y + member.offset_top_Y,
                        Z = nodeTopBase.Z + member.offset_top_Z
                    };
                }
                else
                {
                    nodeBottom = nodeBottomBase;
                    nodeTop = nodeTopBase;
                }

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
                var nodeStart = new StbNode();
                var nodeEnd = new StbNode();

                StbNode nodeStartBase = _nodes.First(node => node.id == member.id_node_start);
                StbNode nodeEndBase = _nodes.First(node => node.id == member.id_node_end);

                if (_isOffset)
                {
                    nodeStart = new StbNode
                    {
                        X = nodeStartBase.X + member.offset_start_X,
                        Y = nodeStartBase.Y + member.offset_start_Y,
                        Z = nodeStartBase.Z + member.offset_start_Z
                    };
                    nodeEnd = new StbNode
                    {
                        X = nodeEndBase.X + member.offset_end_X,
                        Y = nodeEndBase.Y + member.offset_end_Y,
                        Z = nodeEndBase.Z + member.offset_end_Z
                    };
                }
                else
                {
                    nodeStart = nodeStartBase;
                    nodeEnd = nodeEndBase;
                }
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
                var nodeBottom = new StbNode();
                var nodeTop = new StbNode();

                StbNode nodeBottomBase = _nodes.First(node => node.id == member.id_node_bottom);
                StbNode nodeTopBase = _nodes.First(node => node.id == member.id_node_top);

                if (_isOffset)
                {
                    nodeBottom = new StbNode
                    {
                        X = nodeBottomBase.X + member.offset_bottom_X,
                        Y = nodeBottomBase.Y + member.offset_bottom_Y,
                        Z = nodeBottomBase.Z + member.offset_bottom_Z
                    };
                    nodeTop = new StbNode
                    {
                        X = nodeTopBase.X + member.offset_top_X,
                        Y = nodeTopBase.Y + member.offset_top_Y,
                        Z = nodeTopBase.Z + member.offset_top_Z
                    };
                }
                else
                {
                    nodeBottom = nodeBottomBase;
                    nodeTop = nodeTopBase;
                }

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
                var nodeStart = new StbNode();
                var nodeEnd = new StbNode();

                StbNode nodeStartBase = _nodes.First(node => node.id == member.id_node_start);
                StbNode nodeEndBase = _nodes.First(node => node.id == member.id_node_end);

                if (_isOffset)
                {
                    nodeStart = new StbNode
                    {
                        X = nodeStartBase.X + member.offset_start_X,
                        Y = nodeStartBase.Y + member.offset_start_Y,
                        Z = nodeStartBase.Z + member.offset_start_Z
                    };
                    nodeEnd = new StbNode
                    {
                        X = nodeEndBase.X + member.offset_end_X,
                        Y = nodeEndBase.Y + member.offset_end_Y,
                        Z = nodeEndBase.Z + member.offset_end_Z
                    };
                }
                else
                {
                    nodeStart = nodeStartBase;
                    nodeEnd = nodeEndBase;
                }

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
                var nodeStart = new StbNode();
                var nodeEnd = new StbNode();

                StbNode nodeStartBase = _nodes.First(node => node.id == member.id_node_start);
                StbNode nodeEndBase = _nodes.First(node => node.id == member.id_node_end);

                if (_isOffset)
                {
                    nodeStart = new StbNode
                    {
                        X = nodeStartBase.X + member.offset_start_X,
                        Y = nodeStartBase.Y + member.offset_start_Y,
                        Z = nodeStartBase.Z + member.offset_start_Z
                    };
                    nodeEnd = new StbNode
                    {
                        X = nodeEndBase.X + member.offset_end_X,
                        Y = nodeEndBase.Y + member.offset_end_Y,
                        Z = nodeEndBase.Z + member.offset_end_Z
                    };
                }
                else
                {
                    nodeStart = nodeStartBase;
                    nodeEnd = nodeEndBase;
                }

                lines.Append(GH_LineFromStbNode(nodeStart, nodeEnd), new GH_Path(0, i));
            }

            return lines;
        }

        internal GH_Structure<GH_Line> Piles()
        {
            var lines = new GH_Structure<GH_Line>();
            if (_members.StbPiles == null)
            {
                return lines;
            }

            foreach ((StbPile member, int i) in _members.StbPiles.Select((member, index) => (member, index)))
            {
                switch (member.kind_structure)
                {
                    case StbPileKind_structure.RC:
                        GetRcPileLines(lines, member, i);
                        break;
                    case StbPileKind_structure.PC:
                        GetPcPileLines(lines, member, i);
                        break;
                    case StbPileKind_structure.S:
                        throw new NotImplementedException();
                }
            }

            return lines;
        }

        private void GetPcPileLines(GH_Structure<GH_Line> lines, StbPile member, int i)
        {
            var figures = PCPileFigure.GetFigureList(_sections, member);

            var nodes = new List<StbNode>();
            StbNode node = _nodes.First(n => n.id == member.id_node);
            if (_isOffset)
            {
                nodes.Add(new StbNode
                {
                    X = node.X + member.offset_X,
                    Y = node.Y + member.offset_Y,
                    Z = node.Z + member.level_top
                });
            }
            else
            {
                nodes.Add(new StbNode
                {
                    X = node.X,
                    Y = node.Y,
                    Z = node.Z
                });
            }

            foreach ((PCPileFigure figure, int index) in figures.Select((figure, index) => (figure, index)))
            {
                nodes.Add(new StbNode
                {
                    X = nodes[index].X,
                    Y = nodes[index].Y,
                    Z = nodes[index].Z - figure.Length
                });
                lines.Append(GH_LineFromStbNode(nodes[index], nodes[index + 1]), new GH_Path(0, i));
            }
        }

        private void GetRcPileLines(GH_Structure<GH_Line> lines, StbPile member, int i)
        {
            var nodeBottom = new StbNode();
            var nodeTop = new StbNode();

            StbNode node = _nodes.First(n => n.id == member.id_node);
            if (_isOffset)
            {
                nodeTop = new StbNode
                {
                    X = node.X + member.offset_X,
                    Y = node.Y + member.offset_Y,
                    Z = node.Z + member.level_top
                };
                nodeBottom = new StbNode
                {
                    X = nodeTop.X,
                    Y = nodeTop.Y,
                    Z = nodeTop.Z - member.length_all
                };

            }
            else
            {
                nodeTop = new StbNode
                {
                    X = node.X,
                    Y = node.Y,
                    Z = node.Z
                };
                nodeBottom = new StbNode
                {
                    X = nodeTop.X,
                    Y = nodeTop.Y,
                    Z = nodeTop.Z - member.length_all + member.level_top
                };
            }
            lines.Append(GH_LineFromStbNode(nodeBottom, nodeTop), new GH_Path(0, i));
        }

        private static GH_Line GH_LineFromStbNode(StbNode from, StbNode to)
        {
            var ptFrom = new Point3d(from.X, from.Y, from.Z);
            var ptTo = new Point3d(to.X, to.Y, to.Z);

            return new GH_Line(new Line(ptFrom, ptTo));
        }
    }
}
