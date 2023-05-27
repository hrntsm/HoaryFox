using System;
using System.Collections.Generic;
using System.Linq;

using Rhino.Geometry;

using STBDotNet.v202;

namespace HoaryFox.Component.Utils.Geometry.BrepMaker
{
    public class Pile
    {
        private readonly IReadOnlyList<double> _tolerance;
        private readonly StbSections _sections;
        public Pile(StbSections sections, IReadOnlyList<double> tolerance)
        {
            _tolerance = tolerance;
            _sections = sections;
        }

        public Brep CreatePileBrep(string idSection, StbPileKind_structure kind, IReadOnlyList<Point3d> sectionPoints, Vector3d axis)
        {
            SectionCurve[] curveList = CreateFromEachColumnKind(idSection, kind, sectionPoints, axis);
            return Utils.CreateCapedBrepFromLoft(curveList, _tolerance[0]);
        }

        private SectionCurve[] CreateFromEachColumnKind(string idSection, StbPileKind_structure kind, IReadOnlyList<Point3d> sectionPoints, Vector3d axis)
        {
            SectionCurve[] curveList;
            try
            {
                curveList = CreateCurveList(idSection, kind, sectionPoints, axis);
            }
            catch (Exception)
            {
                throw new ArgumentException("The cross-sectional shape of the pile seems to be wrong. Please check.");
            }

            return curveList;
        }

        private SectionCurve[] CreateCurveList(string idSection, StbPileKind_structure kind, IReadOnlyList<Point3d> sectionPoints, Vector3d axis)
        {
            SectionCurve[] curveList;
            switch (kind)
            {
                case StbPileKind_structure.RC:
                    StbSecPile_RC rcSec = _sections.StbSecPile_RC.First(sec => sec.id == idSection);
                    curveList = SecRcPileToCurves(rcSec.StbSecFigurePile_RC.Item, sectionPoints, axis);
                    break;
                case StbPileKind_structure.S:
                case StbPileKind_structure.PC:
                default:
                    throw new ArgumentException("Unsupported StbPileKind");
            }

            return curveList;
        }

        private static SectionCurve[] SecRcPileToCurves(object figure, IReadOnlyList<Point3d> sectionPoints, Vector3d axis)
        {
            var curveList = new List<SectionCurve>();
            Vector3d[] localAxis = Utils.CreateLocalAxis(sectionPoints);

            switch (figure)
            {
                case StbSecPile_RC_Straight straight:
                    curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[0], straight.D, localAxis[0]));
                    curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[3], straight.D, localAxis[0]));
                    break;
                case StbSecPile_RC_ExtendedFoot foot:
                    var footPos1 = sectionPoints[3] - axis * foot.length_extended_foot;
                    var footPos2 = footPos1 - axis * (foot.D_extended_foot - foot.D_axial) / 2d / Math.Tan(Math.PI / 180 * foot.angle_extended_foot_taper);
                    curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[0], foot.D_axial, localAxis[0]));
                    curveList.Add(SectionCurve.CreateSolidColumnCircle(footPos2, foot.D_axial, localAxis[0]));
                    curveList.Add(SectionCurve.CreateSolidColumnCircle(footPos1, foot.D_extended_foot, localAxis[0]));
                    curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[3], foot.D_extended_foot, localAxis[0]));
                    break;
                case StbSecPile_RC_ExtendedTop top:
                    var topPos1 = sectionPoints[1] + axis * (top.D_extended_top - top.D_axial) / 2d / Math.Tan(Math.PI / 180 * top.angle_extended_top_taper);
                    curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[0], top.D_extended_top, localAxis[0]));
                    curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[1], top.D_extended_top, localAxis[0]));
                    curveList.Add(SectionCurve.CreateSolidColumnCircle(topPos1, top.D_axial, localAxis[0]));
                    curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[3], top.D_axial, localAxis[0]));
                    break;
                case StbSecPile_RC_ExtendedTopFoot both:
                    var bothPos1 = sectionPoints[1] + axis * (both.D_extended_top - both.D_axial) / 2d / Math.Tan(Math.PI / 180 * both.angle_extended_top_taper);
                    var bothPos2 = sectionPoints[3] - axis * both.length_extended_foot;
                    var bothPos3 = bothPos2 - axis * (both.D_extended_foot - both.D_axial) / 2d / Math.Tan(Math.PI / 180 * both.angle_extended_foot_taper);
                    curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[0], both.D_extended_top, localAxis[0]));
                    curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[1], both.D_extended_top, localAxis[0]));
                    curveList.Add(SectionCurve.CreateSolidColumnCircle(bothPos1, both.D_axial, localAxis[0]));
                    curveList.Add(SectionCurve.CreateSolidColumnCircle(bothPos3, both.D_axial, localAxis[0]));
                    curveList.Add(SectionCurve.CreateSolidColumnCircle(bothPos2, both.D_extended_foot, localAxis[0]));
                    curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[3], both.D_extended_foot, localAxis[0]));
                    break;
                default:
                    throw new ArgumentException();
            }

            return curveList.ToArray();
        }
    }
}
