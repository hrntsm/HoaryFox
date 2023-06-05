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
        private readonly string _guid;

        public Pile(StbSections sections, IReadOnlyList<double> tolerance, string guid)
        {
            _tolerance = tolerance;
            _sections = sections;
            _guid = guid;
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
                throw new ArgumentException($"Error converting guid: {_guid}\nThe cross-sectional shape of the pile seems to be wrong. Please check.");
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
                case StbPileKind_structure.PC:
                    StbSecPileProduct productSec = _sections.StbSecPileProduct.First(sec => sec.id == idSection);
                    curveList = SecProductPileToCurves(productSec, sectionPoints, axis);
                    break;
                case StbPileKind_structure.S:
                default:
                    throw new ArgumentException("Unsupported StbPileKind");
            }

            return curveList;
        }

        private SectionCurve[] SecProductPileToCurves(StbSecPileProduct stbSecPileProduct, IReadOnlyList<Point3d> sectionPoints, Vector3d axis)
        {
            var curveList = new List<SectionCurve>();
            Vector3d[] localAxis = Utils.CreateLocalAxis(sectionPoints);
            var figures = PCPileFigure.GetFigureList(stbSecPileProduct);
            foreach ((PCPileFigure fig, int index) in figures.Select((fig, index) => (fig, index)))
            {
                curveList.Add(SectionCurve.CreateSolidColumnPipe(
                    sectionPoints[index], fig.Diameter, fig.Diameter - 2 * fig.Thickness, localAxis[0])
                );
                curveList.Add(SectionCurve.CreateSolidColumnPipe(
                    sectionPoints[index + 1] + 10 * Vector3d.ZAxis, fig.Diameter, fig.Diameter - 2 * fig.Thickness, localAxis[0])
                );
            }
            curveList.Add(SectionCurve.CreateSolidColumnPipe(
                sectionPoints.Last(), figures.Last().Diameter, figures.Last().Diameter - 2 * figures.Last().Thickness, localAxis[0])
            );
            return curveList.ToArray();
        }

        private static SectionCurve[] SecRcPileToCurves(object figure, IReadOnlyList<Point3d> sectionPoints, Vector3d axis)
        {
            var curveList = new List<SectionCurve>();
            Vector3d[] localAxis = Utils.CreateLocalAxis(sectionPoints);
            switch (figure)
            {
                case StbSecPile_RC_Straight straight:
                    PileStraight(sectionPoints, curveList, localAxis, straight.D);
                    break;
                case StbSecPile_RC_ExtendedFoot foot:
                    if (foot.D_axial == foot.D_extended_foot)
                    {
                        PileStraight(sectionPoints, curveList, localAxis, foot.D_axial);
                    }
                    else
                    {
                        PileExtendedFoot(sectionPoints, axis, curveList, localAxis, foot.D_axial, foot.D_extended_foot, foot.length_extended_foot, foot.angle_extended_foot_taper);
                    }
                    break;
                case StbSecPile_RC_ExtendedTop top:
                    if (top.D_axial == top.D_extended_top)
                    {
                        PileStraight(sectionPoints, curveList, localAxis, top.D_axial);
                    }
                    else
                    {
                        PileExtendedTop(sectionPoints, axis, curveList, localAxis, top.D_extended_top, top.D_axial, top.angle_extended_top_taper);
                    }
                    break;
                case StbSecPile_RC_ExtendedTopFoot both:
                    if (both.D_axial == both.D_extended_foot && both.D_axial == both.D_extended_top)
                    {
                        PileStraight(sectionPoints, curveList, localAxis, both.D_axial);
                    }
                    else if (both.D_axial == both.D_extended_foot)
                    {
                        PileExtendedTop(sectionPoints, axis, curveList, localAxis, both.D_extended_top, both.D_axial, both.angle_extended_top_taper);
                    }
                    else if (both.D_axial == both.D_extended_top)
                    {
                        PileExtendedFoot(sectionPoints, axis, curveList, localAxis, both.D_axial, both.D_extended_foot, both.length_extended_foot, both.angle_extended_foot_taper);
                    }
                    else
                    {
                        PileExtendedTopFoot(sectionPoints, axis, curveList, localAxis, both);
                    }
                    break;
                default:
                    throw new ArgumentException();
            }

            return curveList.ToArray();
        }

        private static void PileExtendedTopFoot(IReadOnlyList<Point3d> sectionPoints, Vector3d axis, List<SectionCurve> curveList, Vector3d[] localAxis, StbSecPile_RC_ExtendedTopFoot both)
        {
            var bothPos1 = sectionPoints[1] + axis * (both.D_extended_top - both.D_axial) / 2d / Math.Tan(Math.PI / 180 * both.angle_extended_top_taper);
            var bothPos2 = sectionPoints[3] - axis * both.length_extended_foot;
            var bothPos3 = bothPos2 - axis * (both.D_extended_foot - both.D_axial) / 2d / Math.Tan(Math.PI / 180 * both.angle_extended_foot_taper);
            curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[0], both.D_extended_top, localAxis[0]));
            curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[1], both.D_extended_top, localAxis[0]));
            curveList.Add(SectionCurve.CreateSolidColumnCircle(bothPos1, both.D_axial, localAxis[0]));
            curveList.Add(SectionCurve.CreateSolidColumnCircle(bothPos3, both.D_axial, localAxis[0]));
            curveList.Add(SectionCurve.CreateSolidColumnCircle(bothPos2, both.D_extended_foot, localAxis[0]));
            curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[3], both.D_extended_foot, localAxis[0]));
        }

        private static void PileExtendedTop(IReadOnlyList<Point3d> sectionPoints, Vector3d axis, List<SectionCurve> curveList, Vector3d[] localAxis, double dExtend, double d, double angle)
        {
            var topPos1 = sectionPoints[1] + axis * (dExtend - d) / 2d / Math.Tan(Math.PI / 180 * angle);
            curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[0], dExtend, localAxis[0]));
            curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[1], dExtend, localAxis[0]));
            curveList.Add(SectionCurve.CreateSolidColumnCircle(topPos1, d, localAxis[0]));
            curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[3], d, localAxis[0]));
        }

        private static void PileExtendedFoot(IReadOnlyList<Point3d> sectionPoints, Vector3d axis, List<SectionCurve> curveList, Vector3d[] localAxis, double d, double dExtend, double length, double angle)
        {
            var footPos1 = sectionPoints[3] - axis * length;
            var footPos2 = footPos1 - axis * (dExtend - d) / 2d / Math.Tan(Math.PI / 180 * angle);
            curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[0], d, localAxis[0]));
            curveList.Add(SectionCurve.CreateSolidColumnCircle(footPos2, d, localAxis[0]));
            curveList.Add(SectionCurve.CreateSolidColumnCircle(footPos1, dExtend, localAxis[0]));
            curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[3], dExtend, localAxis[0]));
        }

        private static void PileStraight(IReadOnlyList<Point3d> sectionPoints, List<SectionCurve> curveList, Vector3d[] localAxis, double d)
        {
            curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[0], d, localAxis[0]));
            curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[3], d, localAxis[0]));
        }
    }
}
