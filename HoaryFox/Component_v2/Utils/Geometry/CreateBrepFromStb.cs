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
        private readonly StbSections _sections;

        public CreateBrepFromStb(StbSections sections, IEnumerable<StbNode> nodes, IReadOnlyList<double> tolerance)
        {
            _nodes = nodes;
            _tolerance = tolerance;
            _sections = sections;
        }

        public List<Brep> Column(IEnumerable<StbColumn> columns)
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

                Point3d[] sectionPoints =
                {
                    new Point3d(endNodes[0].X, endNodes[0].Y, endNodes[0].Z),
                    new Point3d(endNodes[0].X, endNodes[0].Y, endNodes[0].Z + column.joint_bottom),
                    new Point3d(endNodes[1].X, endNodes[1].Y, endNodes[1].Z - column.joint_top),
                    new Point3d(endNodes[1].X, endNodes[1].Y, endNodes[1].Z)
                };

                var brep = new Brep();
                var curveList = new List<Curve>();

                switch (kind)
                {
                    case StbColumnKind_structure.RC:
                        StbSecColumn_RC rcSec = _sections.StbSecColumn_RC.First(sec => sec.id == column.id_section);

                        switch (rcSec.StbSecFigureColumn_RC.Item)
                        {
                            case StbSecColumn_RC_Rect figure:
                                curveList.Add(new PolylineCurve(SectionCornerPoints.Column(sectionPoints[0],
                                    figure.width_X, figure.width_Y)));
                                curveList.Add(new PolylineCurve(SectionCornerPoints.Column(sectionPoints[3],
                                    figure.width_X, figure.width_Y)));
                                break;
                            case StbSecColumn_RC_Circle figure:
                                curveList.Add(new ArcCurve(new Circle(sectionPoints[0], figure.D / 2d)));
                                curveList.Add(new ArcCurve(new Circle(sectionPoints[3], figure.D / 2d)));
                                break;
                            default:
                                throw new Exception();
                        }

                        break;
                    case StbColumnKind_structure.S:
                        StbSecColumn_S sSec = _sections.StbSecColumn_S.First(sec => sec.id == column.id_section);
                        object[] figures = sSec.StbSecSteelFigureColumn_S.Items;

                        switch (figures[0])
                        {
                            case StbSecSteelColumn_S_Same _:
                                curveList = SteelSecToCurves(figures, sectionPoints);
                                break;
                        }
                        break;
                    case StbColumnKind_structure.SRC:
                    case StbColumnKind_structure.CFT:
                    case StbColumnKind_structure.UNDEFINED:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                brep = Brep.CreateFromLoft(curveList, Point3d.Unset, Point3d.Unset, LoftType.Straight, false)[0]
                    .CapPlanarHoles(_tolerance[0]);

                Vector3d memberAxis = sectionPoints[1] - sectionPoints[0];
                Vector3d rotateAxis = Vector3d.CrossProduct(Vector3d.ZAxis, memberAxis);
                double angle = Vector3d.VectorAngle(Vector3d.ZAxis, memberAxis);
                brep.Rotate(column.rotate, Vector3d.ZAxis, sectionPoints[0]); // 要素軸での回転
                brep.Rotate(angle, rotateAxis, sectionPoints[0]); // デフォルトではZ方向を向いているので、正確な要素方向への回転
                brepList.Add(brep);
            }

            return brepList;
        }

        private List<Curve> SteelSecToCurves(object[] figures, IReadOnlyList<Point3d> sectionPoints)
        {
            var curveList = new List<Curve>();

            switch (figures)
            {
                case StbSecSteelColumn_S_Same[] same: // 1断面
                    curveList.Add(GetSteelSec(same[0].shape, sectionPoints[0], SectionType.Column));
                    curveList.Add(GetSteelSec(same[0].shape, sectionPoints[3], SectionType.Column));
                    break;
                case StbSecSteelColumn_S_NotSame[] notSames: // 2断面
                    curveList.Add(GetSteelSec(notSames[0].shape, sectionPoints[0], SectionType.Column));
                    curveList.Add(GetSteelSec(notSames[0].shape, sectionPoints[3], SectionType.Column));
                    break;
                case StbSecSteelColumn_S_ThreeTypes[] three: // ３断面
                    curveList.Add(GetSteelSec(three[0].shape, sectionPoints[0], SectionType.Column));
                    curveList.Add(GetSteelSec(three[0].shape, sectionPoints[3], SectionType.Column));
                    break;
                default:
                    throw new ArgumentException("Unmatched StbSecSteelColumn_S");
            }

            return curveList;
        }

        private Curve GetSteelSec(string shape, Point3d point, SectionType type)
        {
            StbSecSteel secSteel = _sections.StbSecSteel;

            if (secSteel.StbSecBuildBOX != null)
            {
                foreach (StbSecBuildBOX box in _sections.StbSecSteel.StbSecBuildBOX)
                {
                    if (box.name == shape)
                    {
                        switch (type)
                        {
                            case SectionType.Column:
                                return new PolylineCurve(SectionCornerPoints.Column(point, box.A, box.B));
                            case SectionType.Beam:
                                break;
                            case SectionType.Brace:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(type), type, null);
                        }
                    }
                }
            }

            if (secSteel.StbSecRollBOX != null)
            {
                foreach (StbSecRollBOX box in _sections.StbSecSteel.StbSecRollBOX)
                {
                    if (box.name == shape)
                    {
                        switch (type)
                        {
                            case SectionType.Column:
                                return new PolylineCurve(SectionCornerPoints.Column(point, box.A, box.B));
                            case SectionType.Beam:
                                break;
                            case SectionType.Brace:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(type), type, null);
                        }
                    }
                }
            }

            // foreach (StbSecBuildH buildH in _sections.StbSecSteel.StbSecBuildH)
            // {
            //     if (buildH.name == shape)
            //     {
            //         
            //     }
            // }

            throw new ArgumentException("There are no matching steel section");
        }

        private enum SectionType
        {
            Column,
            Beam,
            Brace
        }
    }
}
