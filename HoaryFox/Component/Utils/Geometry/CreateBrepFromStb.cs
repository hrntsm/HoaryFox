using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component.Utils.Geometry
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
            if (columns == null)
            {
                return brepList;
            }

            foreach (StbColumn column in columns)
            {
                StbColumnKind_structure kind = column.kind_structure;

                StbNode[] endNodes =
                {
                    _nodes.First(node => node.id == column.id_node_bottom),
                    _nodes.First(node => node.id == column.id_node_top)
                };
                Point3d[] offset =
                {
                    new Point3d(column.offset_bottom_X, column.offset_bottom_Y, column.offset_bottom_Z),
                    new Point3d(column.offset_top_X, column.offset_top_Y, column.offset_top_Z)
                };
                Point3d[] sectionPoints =
                {
                    new Point3d(endNodes[0].X, endNodes[0].Y, endNodes[0].Z) + offset[0],
                    new Point3d(),
                    new Point3d(),
                    new Point3d(endNodes[1].X, endNodes[1].Y, endNodes[1].Z) + offset[1]
                };
                Vector3d memberAxis = sectionPoints[3] - sectionPoints[0];
                sectionPoints[1] = sectionPoints[0] + memberAxis / memberAxis.Length * column.joint_bottom;
                sectionPoints[2] = sectionPoints[3] - memberAxis / memberAxis.Length * column.joint_top;

                brepList.Add(CreateColumnBrep(column.id_section, column.rotate, kind, sectionPoints, memberAxis));
            }

            return brepList;
        }

        public List<Brep> Post(IEnumerable<StbPost> posts)
        {
            var brepList = new List<Brep>();
            if (posts == null)
            {
                return brepList;
            }

            foreach (StbPost post in posts)
            {
                StbColumnKind_structure kind = post.kind_structure;

                StbNode[] endNodes =
                {
                    _nodes.First(node => node.id == post.id_node_bottom),
                    _nodes.First(node => node.id == post.id_node_top)
                };
                Point3d[] offset =
                {
                    new Point3d(post.offset_bottom_X, post.offset_bottom_Y, post.offset_bottom_Z),
                    new Point3d(post.offset_top_X, post.offset_top_Y, post.offset_top_Z)
                };
                Point3d[] sectionPoints =
                {
                    new Point3d(endNodes[0].X, endNodes[0].Y, endNodes[0].Z) + offset[0],
                    new Point3d(),
                    new Point3d(),
                    new Point3d(endNodes[1].X, endNodes[1].Y, endNodes[1].Z) + offset[1]
                };
                Vector3d memberAxis = sectionPoints[3] - sectionPoints[0];
                sectionPoints[1] = sectionPoints[0] + memberAxis / memberAxis.Length * post.joint_bottom;
                sectionPoints[2] = sectionPoints[3] - memberAxis / memberAxis.Length * post.joint_top;

                brepList.Add(CreateColumnBrep(post.id_section, post.rotate, kind, sectionPoints, memberAxis));
            }

            return brepList;
        }

        private Brep CreateColumnBrep(string idSection, double rotate, StbColumnKind_structure kind, IReadOnlyList<Point3d> sectionPoints, Vector3d memberAxis)
        {
            var curveList = new List<Curve>();

            switch (kind)
            {
                case StbColumnKind_structure.RC:
                    StbSecColumn_RC rcSec = _sections.StbSecColumn_RC.First(sec => sec.id == idSection);
                    object rcFigure = rcSec.StbSecFigureColumn_RC.Item;
                    curveList = SecRcColumnToCurves(rcFigure, sectionPoints);
                    break;
                case StbColumnKind_structure.S:
                    StbSecColumn_S sSec = _sections.StbSecColumn_S.First(sec => sec.id == idSection);
                    object[] sFigures = sSec.StbSecSteelFigureColumn_S.Items;
                    curveList = SecSteelColumnToCurves(sFigures, sectionPoints);
                    break;
                case StbColumnKind_structure.SRC:
                    StbSecColumn_SRC srcSec = _sections.StbSecColumn_SRC.First(sec => sec.id == idSection);
                    object srcFigure = srcSec.StbSecFigureColumn_SRC.Item;
                    curveList = SecRcColumnToCurves(srcFigure, sectionPoints);
                    break;
                case StbColumnKind_structure.CFT:
                case StbColumnKind_structure.UNDEFINED:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            RotateCurveList(memberAxis, curveList, rotate, sectionPoints, Vector3d.ZAxis);
            Brep brep = Brep.CreateFromLoft(curveList, Point3d.Unset, Point3d.Unset, LoftType.Straight, false)[0]
                .CapPlanarHoles(_tolerance[0]);
            return brep;
        }

        private static List<Curve> SecRcColumnToCurves(object figure, IReadOnlyList<Point3d> sectionPoints)
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
                case StbSecColumn_SRC_Rect rect:
                    curveList.Add(new PolylineCurve(
                        SectionCornerPoints.ColumnRect(sectionPoints[0], rect.width_X, rect.width_Y)));
                    curveList.Add(new PolylineCurve(
                        SectionCornerPoints.ColumnRect(sectionPoints[3], rect.width_X, rect.width_Y)));
                    break;
                case StbSecColumn_RC_Circle circle:
                    curveList.Add(new ArcCurve(new Circle(sectionPoints[0], circle.D / 2d)));
                    curveList.Add(new ArcCurve(new Circle(sectionPoints[3], circle.D / 2d)));
                    break;
                case StbSecColumn_SRC_Circle circle:
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
            Vector3d[] localAxis = CreateLocalAxis(sectionPoints);

            string bottom, center, top;
            switch (figures.Count)
            {
                case 1:
                    var same = figures[0] as StbSecSteelColumn_S_Same;
                    center = same.shape;
                    curveList.Add(GetSteelSec(center, sectionPoints[0], SectionType.Column, localAxis));
                    curveList.Add(GetSteelSec(center, sectionPoints[3], SectionType.Column, localAxis));
                    break;
                case 2:
                    var notSames = new[] { figures[0] as StbSecSteelColumn_S_NotSame, figures[1] as StbSecSteelColumn_S_NotSame };
                    bottom = notSames.First(item => item.pos == StbSecSteelColumn_S_NotSamePos.BOTTOM).shape;
                    top = notSames.First(item => item.pos == StbSecSteelColumn_S_NotSamePos.TOP).shape;
                    curveList.Add(GetSteelSec(bottom, sectionPoints[0], SectionType.Column, localAxis));
                    if (sectionPoints[1].Z > sectionPoints[0].Z)
                    {
                        curveList.Add(GetSteelSec(bottom, sectionPoints[1], SectionType.Column, localAxis));
                        curveList.Add(GetSteelSec(top, sectionPoints[1], SectionType.Column, localAxis));
                    }
                    else
                    {
                        curveList.Add(GetSteelSec(bottom, sectionPoints[2], SectionType.Column, localAxis));
                        curveList.Add(GetSteelSec(top, sectionPoints[2], SectionType.Column, localAxis));
                    }
                    curveList.Add(GetSteelSec(top, sectionPoints[3], SectionType.Column, localAxis));
                    break;
                case 3:
                    var three = new[] { figures[0] as StbSecSteelColumn_S_ThreeTypes, figures[1] as StbSecSteelColumn_S_ThreeTypes, figures[2] as StbSecSteelColumn_S_ThreeTypes };
                    bottom = three.First(item => item.pos == StbSecSteelColumn_S_ThreeTypesPos.BOTTOM).shape;
                    center = three.First(item => item.pos == StbSecSteelColumn_S_ThreeTypesPos.CENTER).shape;
                    top = three.First(item => item.pos == StbSecSteelColumn_S_ThreeTypesPos.TOP).shape;
                    curveList.Add(GetSteelSec(bottom, sectionPoints[0], SectionType.Column, localAxis));
                    curveList.Add(GetSteelSec(center, sectionPoints[1], SectionType.Column, localAxis));
                    curveList.Add(GetSteelSec(center, sectionPoints[2], SectionType.Column, localAxis));
                    curveList.Add(GetSteelSec(top, sectionPoints[3], SectionType.Column, localAxis));
                    break;
                default:
                    throw new ArgumentException("Unmatched StbSecSteelColumn_S");
            }

            return curveList;
        }

        public List<Brep> Girder(IEnumerable<StbGirder> girders)
        {
            var brepList = new List<Brep>();
            if (girders == null)
            {
                return brepList;
            }

            foreach (StbGirder girder in girders)
            {
                StbGirderKind_structure kind = girder.kind_structure;

                StbNode[] endNodes =
                {
                    _nodes.First(node => node.id == girder.id_node_start),
                    _nodes.First(node => node.id == girder.id_node_end)
                };
                Point3d[] offset =
                {
                    new Point3d(girder.offset_start_X, girder.offset_start_Y, girder.offset_start_Z),
                    new Point3d(girder.offset_end_X, girder.offset_end_Y, girder.offset_end_Z)
                };
                Point3d[] sectionPoints =
                {
                    new Point3d(endNodes[0].X, endNodes[0].Y, endNodes[0].Z) + offset[0],
                    Point3d.Origin,
                    Point3d.Origin,
                    new Point3d(endNodes[1].X, endNodes[1].Y, endNodes[1].Z) + offset[1]
                };
                Vector3d memberAxis = sectionPoints[3] - sectionPoints[0];
                sectionPoints[1] = sectionPoints[0] + memberAxis / memberAxis.Length * girder.joint_start;
                sectionPoints[2] = sectionPoints[3] - memberAxis / memberAxis.Length * girder.joint_end;

                brepList.Add(CreateGirderBrep(girder.id_section, girder.rotate, kind, sectionPoints, memberAxis));
            }

            return brepList;
        }

        public List<Brep> Beam(IEnumerable<StbBeam> beams)
        {
            var brepList = new List<Brep>();
            if (beams == null)
            {
                return brepList;
            }

            foreach (StbBeam beam in beams)
            {
                StbGirderKind_structure kind = beam.kind_structure;

                StbNode[] endNodes =
                {
                    _nodes.First(node => node.id == beam.id_node_start),
                    _nodes.First(node => node.id == beam.id_node_end)
                };
                Point3d[] offset =
                {
                    new Point3d(beam.offset_start_X, beam.offset_start_Y, beam.offset_start_Z),
                    new Point3d(beam.offset_end_X, beam.offset_end_Y, beam.offset_end_Z)
                };
                Point3d[] sectionPoints =
                {
                    new Point3d(endNodes[0].X, endNodes[0].Y, endNodes[0].Z) + offset[0],
                    Point3d.Origin,
                    Point3d.Origin,
                    new Point3d(endNodes[1].X, endNodes[1].Y, endNodes[1].Z) + offset[1]
                };
                Vector3d memberAxis = sectionPoints[3] - sectionPoints[0];
                sectionPoints[1] = sectionPoints[0] + memberAxis / memberAxis.Length * beam.joint_start;
                sectionPoints[2] = sectionPoints[3] - memberAxis / memberAxis.Length * beam.joint_end;

                brepList.Add(CreateGirderBrep(beam.id_section, beam.rotate, kind, sectionPoints, memberAxis));
            }

            return brepList;
        }

        private Brep CreateGirderBrep(string idSection, double rotate, StbGirderKind_structure kind, IReadOnlyList<Point3d> sectionPoints, Vector3d memberAxis)
        {
            var curveList = new List<Curve>();
            switch (kind)
            {
                case StbGirderKind_structure.RC:
                    StbSecBeam_RC rcSec = _sections.StbSecBeam_RC.First(sec => sec.id == idSection);
                    object[] rcFigure = rcSec.StbSecFigureBeam_RC.Items;
                    curveList = SecRcBeamCurves(rcFigure, sectionPoints);
                    break;
                case StbGirderKind_structure.S:
                    StbSecBeam_S sSec = _sections.StbSecBeam_S.First(sec => sec.id == idSection);
                    object[] sFigure = sSec.StbSecSteelFigureBeam_S.Items;
                    curveList = SecSteelBeamToCurves(sFigure, sectionPoints);
                    break;
                case StbGirderKind_structure.SRC:
                    StbSecBeam_SRC srcSec = _sections.StbSecBeam_SRC.First(sec => sec.id == idSection);
                    object[] srcFigure = srcSec.StbSecFigureBeam_SRC.Items;
                    curveList = SecSrcBeamCurves(srcFigure, sectionPoints);
                    break;
                case StbGirderKind_structure.UNDEFINED:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //TODO: ローカル軸での回転を実装する
            // RotateCurveList(memberAxis, curveList, rotate, sectionPoints, Vector3d.XAxis);
            Brep brep = Brep.CreateFromLoft(curveList, Point3d.Unset, Point3d.Unset, LoftType.Straight, false)[0]
                .CapPlanarHoles(_tolerance[0]);
            return brep;
        }


        private static List<Curve> SecRcBeamCurves(IReadOnlyList<object> figures, IReadOnlyList<Point3d> sectionPoints)
        {
            var curveList = new List<Curve>();
            Vector3d[] localAxis = CreateLocalAxis(sectionPoints);

            switch (figures.Count)
            {
                case 1:
                    var straight = figures[0] as StbSecBeam_RC_Straight;
                    curveList.Add(new PolylineCurve(
                        SectionCornerPoints.BeamRect(sectionPoints[0], straight.depth, straight.width, localAxis[1], localAxis[2])));
                    curveList.Add(new PolylineCurve(
                        SectionCornerPoints.BeamRect(sectionPoints[3], straight.depth, straight.width, localAxis[1], localAxis[2])));
                    break;
                case 2:
                    var taper = new[] { figures[0] as StbSecBeam_RC_Taper, figures[1] as StbSecBeam_RC_Taper };
                    curveList.Add(new PolylineCurve(
                        SectionCornerPoints.BeamRect(sectionPoints[0], taper[0].depth, taper[0].width, localAxis[1], localAxis[2])));
                    curveList.Add(new PolylineCurve(
                        SectionCornerPoints.BeamRect(sectionPoints[3], taper[1].depth, taper[1].width, localAxis[1], localAxis[2])));
                    break;
                case 3:
                    var haunch = new[] { figures[0] as StbSecBeam_RC_Haunch, figures[1] as StbSecBeam_RC_Haunch, figures[2] as StbSecBeam_RC_Haunch };
                    StbSecBeam_RC_Haunch start = haunch.First(fig => fig.pos == StbSecBeam_RC_HaunchPos.START);
                    StbSecBeam_RC_Haunch center = haunch.First(fig => fig.pos == StbSecBeam_RC_HaunchPos.CENTER);
                    StbSecBeam_RC_Haunch end = haunch.First(fig => fig.pos == StbSecBeam_RC_HaunchPos.END);
                    curveList.Add(new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoints[0], start.depth, start.width, localAxis[1], localAxis[2])));
                    curveList.Add(new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoints[1], center.depth, center.width, localAxis[1], localAxis[2])));
                    curveList.Add(new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoints[2], center.depth, center.width, localAxis[1], localAxis[2])));
                    curveList.Add(new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoints[3], end.depth, end.width, localAxis[1], localAxis[2])));
                    break;
                default:
                    throw new Exception();
            }

            return curveList;
        }

        private static Vector3d[] CreateLocalAxis(IReadOnlyList<Point3d> sectionPoints)
        {
            var xAxis = new Vector3d(sectionPoints[3] - sectionPoints[0]);
            var yAxis = Vector3d.CrossProduct(Vector3d.ZAxis, xAxis);
            var zAxis = Vector3d.CrossProduct(xAxis, yAxis);
            xAxis.Unitize();
            yAxis.Unitize();
            zAxis.Unitize();
            return new Vector3d[] { xAxis, yAxis, zAxis };
        }

        private static List<Curve> SecSrcBeamCurves(IReadOnlyList<object> figures, IReadOnlyList<Point3d> sectionPoints)
        {
            var curveList = new List<Curve>();
            Vector3d[] localAxis = CreateLocalAxis(sectionPoints);

            switch (figures.Count)
            {
                case 1:
                    var straight = figures[0] as StbSecBeam_SRC_Straight;
                    curveList.Add(new PolylineCurve(
                        SectionCornerPoints.BeamRect(sectionPoints[0], straight.depth, straight.width, localAxis[1], localAxis[2])));
                    curveList.Add(new PolylineCurve(
                        SectionCornerPoints.BeamRect(sectionPoints[3], straight.depth, straight.width, localAxis[1], localAxis[2])));
                    break;
                case 2:
                    var taper = new[] { figures[0] as StbSecBeam_SRC_Taper, figures[1] as StbSecBeam_SRC_Taper };
                    curveList.Add(new PolylineCurve(
                        SectionCornerPoints.BeamRect(sectionPoints[0], taper[0].depth, taper[0].width, localAxis[1], localAxis[2])));
                    curveList.Add(new PolylineCurve(
                        SectionCornerPoints.BeamRect(sectionPoints[3], taper[1].depth, taper[1].width, localAxis[1], localAxis[2])));
                    break;
                case 3:
                    var haunch = new[] { figures[0] as StbSecBeam_SRC_Haunch, figures[1] as StbSecBeam_SRC_Haunch, figures[2] as StbSecBeam_SRC_Haunch };
                    StbSecBeam_SRC_Haunch start = haunch.First(fig => fig.pos == StbSecBeam_RC_HaunchPos.START);
                    StbSecBeam_SRC_Haunch center = haunch.First(fig => fig.pos == StbSecBeam_RC_HaunchPos.CENTER);
                    StbSecBeam_SRC_Haunch end = haunch.First(fig => fig.pos == StbSecBeam_RC_HaunchPos.END);
                    curveList.Add(new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoints[0], start.depth, start.width, localAxis[1], localAxis[2])));
                    curveList.Add(new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoints[1], center.depth, center.width, localAxis[1], localAxis[2])));
                    curveList.Add(new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoints[2], center.depth, center.width, localAxis[1], localAxis[2])));
                    curveList.Add(new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoints[3], end.depth, end.width, localAxis[1], localAxis[2])));
                    break;
                default:
                    throw new Exception();
            }

            return curveList;
        }

        private List<Curve> SecSteelBeamToCurves(IReadOnlyList<object> figures, IReadOnlyList<Point3d> sectionPoints)
        {
            var curveList = new List<Curve>();
            string start, center, end;
            Vector3d[] localAxis = CreateLocalAxis(sectionPoints);

            switch (figures.Count)
            {
                case 1:
                    var straight = figures[0] as StbSecSteelBeam_S_Straight;
                    center = straight.shape;
                    curveList.Add(GetSteelSec(center, sectionPoints[0], SectionType.Beam, localAxis));
                    curveList.Add(GetSteelSec(center, sectionPoints[3], SectionType.Beam, localAxis));
                    break;
                case 2:
                    var tapers = new[] { figures[0] as StbSecSteelBeam_S_Taper, figures[1] as StbSecSteelBeam_S_Taper };
                    start = tapers.First(sec => sec.pos == StbSecSteelBeam_S_TaperPos.START).shape;
                    end = tapers.First(sec => sec.pos == StbSecSteelBeam_S_TaperPos.END).shape;
                    curveList.Add(GetSteelSec(start, sectionPoints[0], SectionType.Beam, localAxis));
                    curveList.Add(GetSteelSec(end, sectionPoints[3], SectionType.Beam, localAxis));
                    break;
                case 3:
                    if (figures[0] is StbSecSteelBeam_S_Haunch)
                    {
                        var haunch = new[] { figures[0] as StbSecSteelBeam_S_Haunch, figures[1] as StbSecSteelBeam_S_Haunch, figures[2] as StbSecSteelBeam_S_Haunch };
                        start = haunch.First(sec => sec.pos == StbSecSteelBeam_S_HaunchPos.START).shape;
                        center = haunch.First(sec => sec.pos == StbSecSteelBeam_S_HaunchPos.CENTER).shape;
                        end = haunch.First(sec => sec.pos == StbSecSteelBeam_S_HaunchPos.END).shape;
                    }
                    else
                    {
                        var joint = new[] { figures[0] as StbSecSteelBeam_S_Joint, figures[1] as StbSecSteelBeam_S_Joint, figures[2] as StbSecSteelBeam_S_Joint };
                        start = joint.First(sec => sec.pos == StbSecSteelBeam_S_JointPos.START).shape;
                        center = joint.First(sec => sec.pos == StbSecSteelBeam_S_JointPos.CENTER).shape;
                        end = joint.First(sec => sec.pos == StbSecSteelBeam_S_JointPos.END).shape;
                    }
                    curveList.Add(GetSteelSec(start, sectionPoints[0], SectionType.Beam, localAxis));
                    curveList.Add(GetSteelSec(center, sectionPoints[1], SectionType.Beam, localAxis));
                    curveList.Add(GetSteelSec(center, sectionPoints[2], SectionType.Beam, localAxis));
                    curveList.Add(GetSteelSec(end, sectionPoints[3], SectionType.Beam, localAxis));
                    break;
                case 5:
                    throw new ArgumentException("5 section steel is not supported");
                default:
                    throw new ArgumentException("Unmatched StbSecSteelBeam_S");
            }

            return curveList;
        }

        public List<Brep> Brace(IEnumerable<StbBrace> braces)
        {
            var brepList = new List<Brep>();
            if (braces == null)
            {
                return brepList;
            }

            foreach (StbBrace brace in braces)
            {
                StbBraceKind_structure kind = brace.kind_structure;

                StbNode[] endNodes =
                {
                    _nodes.First(node => node.id == brace.id_node_start),
                    _nodes.First(node => node.id == brace.id_node_end)
                };
                Point3d[] offset =
                {
                    new Point3d(brace.offset_start_X, brace.offset_start_Y, brace.offset_start_Z),
                    new Point3d(brace.offset_end_X, brace.offset_end_Y, brace.offset_end_Z)
                };
                Point3d[] sectionPoints =
                {
                    new Point3d(endNodes[0].X, endNodes[0].Y, endNodes[0].Z) + offset[0],
                    Point3d.Origin,
                    Point3d.Origin,
                    new Point3d(endNodes[1].X, endNodes[1].Y, endNodes[1].Z) + offset[1]
                };
                Vector3d memberAxis = sectionPoints[3] - sectionPoints[0];
                sectionPoints[1] = sectionPoints[0] + memberAxis / memberAxis.Length * brace.joint_start;
                sectionPoints[2] = sectionPoints[3] - memberAxis / memberAxis.Length * brace.joint_end;

                brepList.Add(CreateBraceBrep(brace.id_section, brace.rotate, kind, sectionPoints, memberAxis));
            }

            return brepList;
        }

        private Brep CreateBraceBrep(string idSection, double rotate, StbBraceKind_structure kind, IReadOnlyList<Point3d> sectionPoints, Vector3d memberAxis)
        {
            var curveList = new List<Curve>();

            switch (kind)
            {
                case StbBraceKind_structure.S:
                    StbSecBrace_S sSec = _sections.StbSecBrace_S.First(sec => sec.id == idSection);
                    object[] figures = sSec.StbSecSteelFigureBrace_S.Items;
                    curveList = SecSteelBraceToCurves(figures, sectionPoints);
                    break;
                case StbBraceKind_structure.RC:
                case StbBraceKind_structure.SRC:
                    throw new ArgumentException("Unsupported brace structure type");
                default:
                    throw new ArgumentOutOfRangeException();
            }

            RotateCurveList(memberAxis, curveList, rotate, sectionPoints, Vector3d.ZAxis);
            Brep brep = Brep.CreateFromLoft(curveList, Point3d.Unset, Point3d.Unset, LoftType.Straight, false)[0]
                .CapPlanarHoles(_tolerance[0]);
            return brep;
        }

        private List<Curve> SecSteelBraceToCurves(IReadOnlyList<object> figures, IReadOnlyList<Point3d> sectionPoints)
        {
            var curveList = new List<Curve>();
            string start, center, end;
            Vector3d[] localAxis = CreateLocalAxis(sectionPoints);

            switch (figures.Count)
            {
                case 1:
                    var same = figures[0] as StbSecSteelBrace_S_Same;
                    center = same.shape;
                    curveList.Add(GetSteelSec(center, sectionPoints[0], SectionType.Brace, localAxis));
                    curveList.Add(GetSteelSec(center, sectionPoints[3], SectionType.Brace, localAxis));
                    break;
                case 2:
                    var notSames = new[] { figures[0] as StbSecSteelBrace_S_NotSame, figures[1] as StbSecSteelBrace_S_NotSame };
                    start = notSames.First(sec => sec.pos == StbSecSteelBrace_S_NotSamePos.BOTTOM).shape;
                    end = notSames.First(sec => sec.pos == StbSecSteelBrace_S_NotSamePos.TOP).shape;
                    curveList.Add(GetSteelSec(start, sectionPoints[0], SectionType.Brace, localAxis));
                    curveList.Add(sectionPoints[0] == sectionPoints[1]
                        ? GetSteelSec(start, sectionPoints[2], SectionType.Brace, localAxis)
                        : GetSteelSec(start, sectionPoints[1], SectionType.Brace, localAxis));
                    curveList.Add(sectionPoints[0] == sectionPoints[1]
                        ? GetSteelSec(end, sectionPoints[2], SectionType.Brace, localAxis)
                        : GetSteelSec(end, sectionPoints[1], SectionType.Brace, localAxis));
                    curveList.Add(GetSteelSec(end, sectionPoints[3], SectionType.Brace, localAxis));
                    break;
                case 3:
                    var three = new[] { figures[0] as StbSecSteelBrace_S_ThreeTypes, figures[1] as StbSecSteelBrace_S_ThreeTypes, figures[2] as StbSecSteelBrace_S_ThreeTypes };
                    start = three.First(sec => sec.pos == StbSecSteelBrace_S_ThreeTypesPos.BOTTOM).shape;
                    center = three.First(sec => sec.pos == StbSecSteelBrace_S_ThreeTypesPos.CENTER).shape;
                    end = three.First(sec => sec.pos == StbSecSteelBrace_S_ThreeTypesPos.TOP).shape;
                    curveList.Add(GetSteelSec(start, sectionPoints[0], SectionType.Brace, localAxis));
                    curveList.Add(GetSteelSec(center, sectionPoints[1], SectionType.Brace, localAxis));
                    curveList.Add(GetSteelSec(center, sectionPoints[2], SectionType.Brace, localAxis));
                    curveList.Add(GetSteelSec(end, sectionPoints[3], SectionType.Brace, localAxis));
                    break;
                default:
                    throw new ArgumentException("Unmatched StbSecSteelBrace_S");
            }

            return curveList;
        }

        private static void RotateCurveList(Vector3d memberAxis, IReadOnlyList<Curve> curveList, double stbRotateValue, IReadOnlyList<Point3d> sectionPoints, Vector3d secLocalAxis)
        {
            Vector3d rotateAxis = Vector3d.CrossProduct(secLocalAxis, memberAxis);
            double outPlaneAngle = Vector3d.VectorAngle(secLocalAxis, memberAxis);
            double inPlaneAngle = stbRotateValue * Math.PI / 180;
            int len = curveList.Count;
            switch (len)
            {
                case 2:
                    curveList[0].Rotate(inPlaneAngle, secLocalAxis, sectionPoints[0]); // 断面内の回転
                    curveList[0].Rotate(outPlaneAngle, rotateAxis, sectionPoints[0]); // 断面外の回転 
                    curveList[1].Rotate(inPlaneAngle, secLocalAxis, sectionPoints[3]);
                    curveList[1].Rotate(outPlaneAngle, rotateAxis, sectionPoints[3]);
                    break;
                case 3:
                    curveList[0].Rotate(inPlaneAngle, secLocalAxis, sectionPoints[0]);
                    curveList[0].Rotate(outPlaneAngle, rotateAxis, sectionPoints[0]);
                    curveList[2].Rotate(inPlaneAngle, secLocalAxis, sectionPoints[3]);
                    curveList[2].Rotate(outPlaneAngle, rotateAxis, sectionPoints[3]);
                    if (sectionPoints[2] == sectionPoints[3])
                    {
                        curveList[1].Rotate(inPlaneAngle, secLocalAxis, sectionPoints[1]);
                        curveList[1].Rotate(outPlaneAngle, rotateAxis, sectionPoints[1]);
                    }
                    else
                    {
                        curveList[1].Rotate(inPlaneAngle, secLocalAxis, sectionPoints[2]);
                        curveList[1].Rotate(outPlaneAngle, rotateAxis, sectionPoints[2]);
                    }
                    break;
                case 4:
                    for (var i = 0; i < 4; i++)
                    {
                        curveList[i].Rotate(inPlaneAngle, secLocalAxis, sectionPoints[i]);
                        curveList[i].Rotate(outPlaneAngle, rotateAxis, sectionPoints[i]);
                    }
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        private Curve GetSteelSec(string shape, Point3d point, SectionType type, Vector3d[] localAxis)
        {
            StbSecSteel secSteel = _sections.StbSecSteel;

            // TODO: foreach なのに最初にマッチしたもので return しているのでが変なので直す。
            if (secSteel.StbSecBuildBOX != null)
            {
                foreach (var box in secSteel.StbSecBuildBOX.Where(box => box.name == shape))
                {
                    return CurveFromStbSecBox(localAxis, point, type, box.A, box.B);
                }
            }

            if (secSteel.StbSecRollBOX != null)
            {
                foreach (StbSecRollBOX box in secSteel.StbSecRollBOX.Where(box => box.name == shape))
                {
                    return CurveFromStbSecBox(localAxis, point, type, box.A, box.B);
                }
            }

            if (secSteel.StbSecFlatBar != null)
            {
                foreach (StbSecFlatBar flatBar in secSteel.StbSecFlatBar.Where(bar => bar.name == shape))
                {
                    return CurveFromStbSecBox(localAxis, point, type, flatBar.B, flatBar.t);
                }
            }

            if (secSteel.StbSecBuildH != null)
            {
                foreach (StbSecBuildH buildH in secSteel.StbSecBuildH.Where(buildH => buildH.name == shape))
                {
                    return CurveFromStbSecH(point, type, buildH.A, buildH.B, buildH.t1, buildH.t2);
                }
            }

            if (secSteel.StbSecRollH != null)
            {
                foreach (StbSecRollH rollH in secSteel.StbSecRollH.Where(rollH => rollH.name == shape))
                {
                    return CurveFromStbSecH(point, type, rollH.A, rollH.B, rollH.t1, rollH.t2);
                }
            }

            if (secSteel.StbSecRollL != null)
            {
                foreach (StbSecRollL rollL in secSteel.StbSecRollL.Where(rollL => rollL.name == shape))
                {
                    return CurveFromStbSecL(point, type, rollL);
                }
            }

            if (secSteel.StbSecPipe != null)
            {
                foreach (StbSecPipe pipe in secSteel.StbSecPipe.Where(pipe => pipe.name == shape))
                {
                    return CurveFromStbSecPipe(point, type, pipe.D);
                }
            }

            if (secSteel.StbSecRoundBar != null)
            {
                foreach (StbSecRoundBar bar in secSteel.StbSecRoundBar.Where(pipe => pipe.name == shape))
                {
                    return CurveFromStbSecPipe(point, type, bar.R);
                }
            }

            // TODO: C 断面を実装する
            if (secSteel.StbSecRollC != null || secSteel.StbSecLipC != null)
            {
                throw new ArgumentException("StbSecRollC & StbSecLipC is not supported");
            }

            throw new ArgumentException("There are no matching steel section");
        }

        private static Curve CurveFromStbSecPipe(Point3d point, SectionType type, double diameter)
        {
            switch (type)
            {
                case SectionType.Column:
                case SectionType.Brace:
                    return SectionCornerPoints.ColumnPipe(point, diameter);
                case SectionType.Beam:
                    return SectionCornerPoints.BeamPipe(point, diameter);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static Curve CurveFromStbSecL(Point3d point, SectionType type, StbSecRollL rollL)
        {
            switch (type)
            {
                case SectionType.Column:
                case SectionType.Brace:
                    return new PolylineCurve(
                        SectionCornerPoints.ColumnL(point, rollL.A, rollL.B, rollL.t1, rollL.t2, rollL.type));
                case SectionType.Beam:
                    return new PolylineCurve(
                        SectionCornerPoints.BeamL(point, rollL.A, rollL.B, rollL.t1, rollL.t2, rollL.type));
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static Curve CurveFromStbSecBox(Vector3d[] localAxis, Point3d point, SectionType type, double A, double B)
        {
            switch (type)
            {
                case SectionType.Column:
                case SectionType.Brace:
                    return new PolylineCurve(
                        SectionCornerPoints.ColumnRect(point, B, A));
                case SectionType.Beam:
                    return new PolylineCurve(
                        SectionCornerPoints.BeamRect(point, B, A, localAxis[1], localAxis[2]));
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static Curve CurveFromStbSecH(Point3d point, SectionType type, double A, double B, double t1, double t2)
        {
            switch (type)
            {
                case SectionType.Column:
                case SectionType.Brace:
                    return new PolylineCurve(
                        SectionCornerPoints.ColumnH(point, A, B, t1, t2));
                case SectionType.Beam:
                    return new PolylineCurve(
                        SectionCornerPoints.BeamH(point, A, B, t1, t2));
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public List<Brep> Slab(IEnumerable<StbSlab> slabs)
        {
            var brepList = new List<Brep>();
            if (slabs == null)
            {
                return brepList;
            }

            foreach (StbSlab slab in slabs)
            {
                StbSlabOffset[] offsets = slab.StbSlabOffsetList;
                var curveList = new PolylineCurve[2];
                double depth = GetSlabDepth(slab);
                string[] nodeIds = slab.StbNodeIdOrder.Split(' ');
                var topPts = new List<Point3d>();
                foreach (string nodeId in nodeIds)
                {
                    var offsetVec = new Vector3d();
                    if (offsets != null)
                    {
                        foreach (StbSlabOffset offset in offsets)
                        {
                            if (nodeId == offset.id_node)
                            {
                                offsetVec = new Vector3d(offset.offset_X, offset.offset_Y, offset.offset_Z);
                            }
                        }
                    }

                    StbNode node = _nodes.First(n => n.id == nodeId);
                    topPts.Add(new Point3d(node.X, node.Y, node.Z) + offsetVec);
                }

                topPts.Add(topPts[0]);
                curveList[0] = new PolylineCurve(topPts);
                Vector3d normal = Vector3d.CrossProduct(curveList[0].TangentAtEnd, curveList[0].TangentAtStart);
                curveList[1] = new PolylineCurve(topPts.Select(pt => pt - normal * depth));
                brepList.Add(Brep.CreateFromLoft(curveList, Point3d.Unset, Point3d.Unset, LoftType.Straight, false)[0]
                    .CapPlanarHoles(_tolerance[0]));
            }

            return brepList;
        }

        private double GetSlabDepth(StbSlab slab)
        {
            double depth = 0;

            switch (slab.kind_structure)
            {
                case StbSlabKind_structure.RC:
                    object[] slabRc = _sections.StbSecSlab_RC.First(sec => sec.id == slab.id_section).StbSecFigureSlab_RC.Items;
                    switch (slabRc.Length)
                    {
                        case 1:
                            var straight = slabRc[0] as StbSecSlab_RC_Straight;
                            depth = straight.depth;
                            break;
                        case 2:
                            var tapers = new[] { slabRc[0] as StbSecSlab_RC_Taper, slabRc[1] as StbSecSlab_RC_Taper };
                            depth = tapers.First(sec => sec.pos == StbSecSlab_RC_TaperPos.TIP).depth;
                            break;
                        case 3:
                            var haunches = new[]
                            {
                                slabRc[0] as StbSecSlab_RC_Haunch, slabRc[1] as StbSecSlab_RC_Haunch,
                                slabRc[2] as StbSecSlab_RC_Haunch
                            };
                            depth = haunches.First(sec => sec.pos == StbSecSlab_RC_HaunchPos.CENTER).depth;
                            break;
                    }

                    break;
                case StbSlabKind_structure.DECK:
                // StbSecSlabDeck slabDeck = _sections.StbSecSlabDeck.FirstOrDefault(sec => sec.id == slab.id_section);
                // break;
                case StbSlabKind_structure.PRECAST:
                // StbSecSlabPrecast slabPrecast = _sections.StbSecSlabPrecast.FirstOrDefault(sec => sec.id == slab.id_section);
                // break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return depth;
        }

        public List<Brep> Wall(IEnumerable<StbWall> walls)
        {
            var brepList = new List<Brep>();
            if (walls == null)
            {
                return brepList;
            }

            foreach (StbWall wall in walls)
            {
                StbWallOffset[] offsets = wall.StbWallOffsetList;
                var curveList = new PolylineCurve[2];
                double thickness = GetWallThickness(wall);
                string[] nodeIds = wall.StbNodeIdOrder.Split(' ');
                var topPts = new List<Point3d>();
                foreach (string nodeId in nodeIds)
                {
                    var offsetVec = new Vector3d();
                    if (offsets != null)
                    {
                        foreach (StbWallOffset offset in offsets)
                        {
                            if (nodeId == offset.id_node)
                            {
                                offsetVec = new Vector3d(offset.offset_X, offset.offset_Y, offset.offset_Z);
                                break;
                            }
                        }
                    }

                    StbNode node = _nodes.First(n => n.id == nodeId);
                    topPts.Add(new Point3d(node.X, node.Y, node.Z) + offsetVec);
                }

                topPts.Add(topPts[0]);
                var centerCurve = new PolylineCurve(topPts);
                Vector3d normal = Vector3d.CrossProduct(centerCurve.TangentAtEnd, centerCurve.TangentAtStart);
                curveList[0] = new PolylineCurve(topPts.Select(pt => pt + normal * thickness / 2));
                curveList[1] = new PolylineCurve(topPts.Select(pt => pt - normal * thickness / 2));
                brepList.Add(Brep.CreateFromLoft(curveList, Point3d.Unset, Point3d.Unset, LoftType.Straight, false)[0]
                    .CapPlanarHoles(_tolerance[0]));
            }

            return brepList;
        }

        private double GetWallThickness(StbWall wall)
        {
            return _sections.StbSecWall_RC.First(sec => sec.id == wall.id_section)
                .StbSecFigureWall_RC.StbSecWall_RC_Straight.t;
        }

        private enum SectionType
        {
            Column,
            Beam,
            Brace
        }
    }
}
