using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component.Utils.Geometry.BrepMaker
{
    public class Girder
    {
        private readonly IReadOnlyList<double> _tolerance;
        private readonly StbSections _sections;
        public Girder(StbSections sections, IReadOnlyList<double> tolerance)
        {
            _tolerance = tolerance;
            _sections = sections;
        }
        public Brep CreateGirderBrep(string idSection, double rotate, StbGirderKind_structure kind, IReadOnlyList<Point3d> sectionPoints, Vector3d memberAxis)
        {
            List<Curve> curveList = CreateFromEachGirderKind(idSection, kind, sectionPoints);

            Utils.RotateCurveList(memberAxis, curveList, rotate, sectionPoints);
            Brep brep = Brep.CreateFromLoft(curveList, Point3d.Unset, Point3d.Unset, LoftType.Straight, false)[0]
                .CapPlanarHoles(_tolerance[0]);

            if (brep.GetVolume() < 0)
            {
                brep.Flip();
            }
            return brep;
        }

        private List<Curve> CreateFromEachGirderKind(string idSection, StbGirderKind_structure kind, IReadOnlyList<Point3d> sectionPoints)
        {
            List<Curve> curveList;
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

            return curveList;
        }


        private static List<Curve> SecRcBeamCurves(IReadOnlyList<object> figures, IReadOnlyList<Point3d> sectionPoints)
        {
            var curveList = new List<Curve>();
            Vector3d[] localAxis = Utils.CreateLocalAxis(sectionPoints);

            switch (figures.Count)
            {
                case 1:
                    var straight = figures[0] as StbSecBeam_RC_Straight;
                    curveList.Add(new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoints[0], straight.depth, straight.width, localAxis[1], localAxis[2])));
                    curveList.Add(new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoints[3], straight.depth, straight.width, localAxis[1], localAxis[2])));
                    break;
                case 2:
                    var taper = new[] { figures[0] as StbSecBeam_RC_Taper, figures[1] as StbSecBeam_RC_Taper };
                    curveList.Add(new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoints[0], taper[0].depth, taper[0].width, localAxis[1], localAxis[2])));
                    curveList.Add(new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoints[1], taper[0].depth, taper[0].width, localAxis[1], localAxis[2])));
                    curveList.Add(new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoints[2], taper[1].depth, taper[1].width, localAxis[1], localAxis[2])));
                    curveList.Add(new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoints[3], taper[1].depth, taper[1].width, localAxis[1], localAxis[2])));
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

        private static List<Curve> SecSrcBeamCurves(IReadOnlyList<object> figures, IReadOnlyList<Point3d> sectionPoints)
        {
            var curveList = new List<Curve>();
            Vector3d[] localAxis = Utils.CreateLocalAxis(sectionPoints);

            switch (figures.Count)
            {
                case 1:
                    var straight = figures[0] as StbSecBeam_SRC_Straight;
                    curveList.Add(new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoints[0], straight.depth, straight.width, localAxis[1], localAxis[2])));
                    curveList.Add(new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoints[3], straight.depth, straight.width, localAxis[1], localAxis[2])));
                    break;
                case 2:
                    var taper = new[] { figures[0] as StbSecBeam_SRC_Taper, figures[1] as StbSecBeam_SRC_Taper };
                    curveList.Add(new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoints[0], taper[0].depth, taper[0].width, localAxis[1], localAxis[2])));
                    curveList.Add(new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoints[1], taper[0].depth, taper[0].width, localAxis[1], localAxis[2])));
                    curveList.Add(new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoints[2], taper[1].depth, taper[1].width, localAxis[1], localAxis[2])));
                    curveList.Add(new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoints[3], taper[1].depth, taper[1].width, localAxis[1], localAxis[2])));
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
            Vector3d[] localAxis = Utils.CreateLocalAxis(sectionPoints);
            var toCurveList = new GirderFigureToCurveList(_sections);

            switch (figures.Count)
            {
                case 1:
                    toCurveList.SingleFigure(figures, sectionPoints, curveList, localAxis);
                    break;
                case 2:
                    toCurveList.TwoFigure(figures, sectionPoints, curveList, localAxis);
                    break;
                case 3:
                    toCurveList.ThreeFigure(figures, sectionPoints, curveList, localAxis);
                    break;
                case 5:
                    throw new ArgumentException("5 section steel is not supported");
                default:
                    throw new ArgumentException("Unmatched StbSecSteelBeam_S");
            }

            return curveList;
        }
    }
}
