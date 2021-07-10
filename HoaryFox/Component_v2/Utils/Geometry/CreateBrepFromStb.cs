using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component_v2.Utils.Geometry
{
    public class CreateBrepFromStb
    {
        private readonly IEnumerable<StbNode> _nodes;
        private readonly IReadOnlyList<double> _tolerance;

        public CreateBrepFromStb(IEnumerable<StbNode> nodes, IReadOnlyList<double> tolerance)
        {
            _nodes = nodes;
            _tolerance = tolerance;
        }

        public List<Brep> Column(IEnumerable<StbColumn> columns, StbSections sections)
        {
            var brepList = new List<Brep>();

            foreach (StbColumn column in columns)
            {
                StbColumnKind_structure kind = column.kind_structure;

                StbNode[] endNodes =
                {
                    _nodes.First(node => node.id == column.id_node_bottom),
                    _nodes.First(node => node.id == column.id_node_top)
                };
                Point3d[] endPoints =
                {
                    new Point3d(endNodes[0].X, endNodes[0].Y, endNodes[0].Z),
                    new Point3d(endNodes[1].X, endNodes[1].Y, endNodes[1].Z)
                };

                var brep = new Brep();

                switch (kind)
                {
                    case StbColumnKind_structure.RC:
                        StbSecColumn_RC section = sections.StbSecColumn_RC.First(sec => sec.id == column.id_section);

                        switch (section.StbSecFigureColumn_RC.Item)
                        {
                            case StbSecColumn_RC_Rect figure:
                                var startSection =
                                    new PolylineCurve(SectionCornerPoints.Column(endPoints[0], figure.width_X, figure.width_Y));
                                var endSection =
                                    new PolylineCurve(SectionCornerPoints.Column(endPoints[1], figure.width_X, figure.width_Y));
                                brep = Brep.CreateFromLoft(new[] { startSection, endSection }, Point3d.Unset, Point3d.Unset, LoftType.Straight, false)[0]
                                    .CapPlanarHoles(_tolerance[0]);
                                break;
                            case StbSecColumn_RC_Circle figure:
                                var lineCurve = new LineCurve(endPoints[0], endPoints[1]);
                                brep = Brep.CreatePipe(lineCurve, figure.D / 2d, true, PipeCapMode.Flat, true, _tolerance[0], _tolerance[1])[0];
                                break;
                            default:
                                throw new Exception();
                        }
                        break;
                    case StbColumnKind_structure.S:
                    case StbColumnKind_structure.SRC:
                    case StbColumnKind_structure.CFT:
                    case StbColumnKind_structure.UNDEFINED:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                Vector3d memberAxis = endPoints[1] - endPoints[0];
                Vector3d rotateAxis = Vector3d.CrossProduct(Vector3d.ZAxis, memberAxis);
                double angle = Vector3d.VectorAngle(Vector3d.ZAxis, memberAxis);
                brep.Rotate(column.rotate, Vector3d.ZAxis, endPoints[0]); // 要素軸での回転
                brep.Rotate(angle, rotateAxis, endPoints[0]); // デフォルトではZ方向を向いているので、正確な要素方向への回転
                brepList.Add(brep);
            }

            return brepList;
        }
    }
}
