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
                        object figure = rcSec.StbSecFigureColumn_RC.Item;
                        curveList = SecRcColumnToCurves(figure, sectionPoints);
                        break;
                    case StbColumnKind_structure.S:
                        StbSecColumn_S sSec = _sections.StbSecColumn_S.First(sec => sec.id == column.id_section);
                        object[] figures = sSec.StbSecSteelFigureColumn_S.Items;
                        curveList = SecSteelColumnToCurves(figures, sectionPoints);
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

                Vector3d memberAxis = sectionPoints[3] - sectionPoints[0];
                Vector3d rotateAxis = Vector3d.CrossProduct(Vector3d.ZAxis, memberAxis);
                double angle = Vector3d.VectorAngle(Vector3d.ZAxis, memberAxis);
                brep.Rotate(column.rotate, Vector3d.ZAxis, sectionPoints[0]); // 要素軸での回転
                brep.Rotate(angle, rotateAxis, sectionPoints[0]); // デフォルトではZ方向を向いているので、正確な要素方向への回転
                brepList.Add(brep);
            }

            return brepList;
        }

        private List<Curve> SecRcColumnToCurves(object figure, IReadOnlyList<Point3d> sectionPoints)
        {
            var curveList = new List<Curve>();

            switch (figure)
            {
                case StbSecColumn_RC_Rect rect:
                    curveList.Add(new PolylineCurve(
                        SectionCornerPoints.ColumnRect(sectionPoints[0], rect.width_X, rect.width_Y)));
                    curveList.Add(new PolylineCurve(
                        SectionCornerPoints.ColumnRect(sectionPoints[3], rect.width_X, rect.width_Y)));
                    break;
                case StbSecColumn_RC_Circle circle:
                    curveList.Add(new ArcCurve(new Circle(sectionPoints[0], circle.D / 2d)));
                    curveList.Add(new ArcCurve(new Circle(sectionPoints[3], circle.D / 2d)));
                    break;
                default:
                    throw new Exception();
            }

            return curveList;
        }

        private List<Curve> SecSteelColumnToCurves(IReadOnlyList<object> figures, IReadOnlyList<Point3d> sectionPoints)
        {
            var curveList = new List<Curve>();

            switch (figures.Count)
            {
                case 1:
                    var same = figures[0] as StbSecSteelColumn_S_Same;
                    curveList.Add(GetSteelSec(same.shape, sectionPoints[0], SectionType.Column));
                    curveList.Add(GetSteelSec(same.shape, sectionPoints[3], SectionType.Column));
                    break;
                case 2:
                    var notSames = new[] { figures[0] as StbSecSteelColumn_S_NotSame, figures[1] as StbSecSteelColumn_S_NotSame };
                    StbSecSteelColumn_S_NotSame nsBottom = notSames.First(item => item.pos == StbSecSteelColumn_S_NotSamePos.BOTTOM);
                    StbSecSteelColumn_S_NotSame nsTop = notSames.First(item => item.pos == StbSecSteelColumn_S_NotSamePos.TOP);
                    curveList.Add(GetSteelSec(nsBottom.shape, sectionPoints[0], SectionType.Column));
                    if (sectionPoints[1].Z > sectionPoints[0].Z)
                    {
                        curveList.Add(GetSteelSec(nsBottom.shape, sectionPoints[1], SectionType.Column));
                        curveList.Add(GetSteelSec(nsTop.shape, sectionPoints[1], SectionType.Column));
                    }
                    else
                    {
                        curveList.Add(GetSteelSec(nsBottom.shape, sectionPoints[2], SectionType.Column));
                        curveList.Add(GetSteelSec(nsTop.shape, sectionPoints[2], SectionType.Column));
                    }
                    curveList.Add(GetSteelSec(nsTop.shape, sectionPoints[3], SectionType.Column));
                    break;
                case 3:
                    var three = new[] { figures[0] as StbSecSteelColumn_S_ThreeTypes, figures[1] as StbSecSteelColumn_S_ThreeTypes, figures[2] as StbSecSteelColumn_S_ThreeTypes };
                    StbSecSteelColumn_S_ThreeTypes tBottom = three.First(item => item.pos == StbSecSteelColumn_S_ThreeTypesPos.BOTTOM);
                    StbSecSteelColumn_S_ThreeTypes tCenter = three.First(item => item.pos == StbSecSteelColumn_S_ThreeTypesPos.CENTER);
                    StbSecSteelColumn_S_ThreeTypes tTop = three.First(item => item.pos == StbSecSteelColumn_S_ThreeTypesPos.TOP);
                    curveList.Add(GetSteelSec(tBottom.shape, sectionPoints[0], SectionType.Column));
                    curveList.Add(GetSteelSec(tCenter.shape, sectionPoints[1], SectionType.Column));
                    curveList.Add(GetSteelSec(tCenter.shape, sectionPoints[2], SectionType.Column));
                    curveList.Add(GetSteelSec(tTop.shape, sectionPoints[3], SectionType.Column));
                    break;
                default:
                    throw new ArgumentException("Unmatched StbSecSteelColumn_S");
            }

            return curveList;
        }

        private List<Curve> SecSteelBeamToCurves(object[] figures, IReadOnlyList<Point3d> sectionPoints)
        {
            var curveList = new List<Curve>();

            switch (figures)
            {
                case StbSecSteelBeam_S_Straight[] straights:
                    break;
                case StbSecSteelBeam_S_Taper[] tapers:
                    break;
                case StbSecSteelBeam_S_Joint[] joints:
                    break;
                case StbSecSteelBeam_S_Haunch[] haunches:
                    break;
                case StbSecSteelBeam_S_FiveTypes[] five:
                    break;
                default:
                    throw new ArgumentException("Unmatched StbSecSteelBeam_S");
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
                                return new PolylineCurve(SectionCornerPoints.ColumnRect(point, box.B, box.A));
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
                                return new PolylineCurve(SectionCornerPoints.ColumnRect(point, box.B, box.A));
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

            if (secSteel.StbSecBuildH != null)
            {
                foreach (StbSecBuildH buildH in _sections.StbSecSteel.StbSecBuildH)
                {
                    if (buildH.name == shape)
                    {
                        switch (type)
                        {
                            case SectionType.Column:
                                return new PolylineCurve(SectionCornerPoints.ColumnH(point, buildH.B, buildH.A,
                                    buildH.t1, buildH.t2));
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

            if (secSteel.StbSecRollH != null)
            {
                foreach (StbSecRollH rollH in _sections.StbSecSteel.StbSecRollH)
                {
                    if (rollH.name == shape)
                    {
                        switch (type)
                        {
                            case SectionType.Column:
                                return new PolylineCurve(SectionCornerPoints.ColumnH(point, rollH.B, rollH.A, rollH.t1, rollH.t2));
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

            // TODO: Box と H 以外の断面を実装する

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
