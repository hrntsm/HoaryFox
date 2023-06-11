using System;
using System.Collections.Generic;
using System.Linq;

using Rhino.Geometry;

using STBDotNet.v202;

namespace HoaryFox.Component.Utils.Geometry.BrepMaker
{
    public class Column
    {
        private readonly IReadOnlyList<double> _tolerance;
        private readonly StbSections _sections;
        private readonly string _guid;

        public Column(StbSections sections, IReadOnlyList<double> tolerance, string guid)
        {
            _tolerance = tolerance;
            _sections = sections;
            _guid = guid;
        }

        public Brep CreateColumnBrep(string idSection, double rotate, StbColumnKind_structure kind, IReadOnlyList<Point3d> sectionPoints, Vector3d memberAxis)
        {
            SectionCurve[] curveList = CreateFromEachColumnKind(idSection, kind, sectionPoints);
            Utils.RotateCurveList(memberAxis, curveList, rotate, sectionPoints);
            return Utils.CreateCapedBrepFromLoft(curveList, _tolerance[0]);
        }

        private SectionCurve[] CreateFromEachColumnKind(string idSection, StbColumnKind_structure kind, IReadOnlyList<Point3d> sectionPoints)
        {
            SectionCurve[] curveList;
            try
            {
                curveList = CreateCurveList(idSection, kind, sectionPoints);
            }
            catch (Exception)
            {
                throw new ArgumentException($"Error converting guid: {_guid}\nThe cross-sectional shape of the column or post seems to be wrong. Please check.");
            }

            return curveList;
        }

        private SectionCurve[] CreateCurveList(string idSection, StbColumnKind_structure kind, IReadOnlyList<Point3d> sectionPoints)
        {
            SectionCurve[] curveList;
            switch (kind)
            {
                case StbColumnKind_structure.RC:
                    StbSecColumn_RC rcSec = _sections.StbSecColumn_RC.First(sec => sec.id == idSection);
                    curveList = SecRcColumnToCurves(rcSec.StbSecFigureColumn_RC.Item, sectionPoints);
                    break;
                case StbColumnKind_structure.S:
                    StbSecColumn_S sSec = _sections.StbSecColumn_S.First(sec => sec.id == idSection);
                    curveList = SecSteelColumnToCurves(sSec.StbSecSteelFigureColumn_S.Items, sectionPoints);
                    break;
                case StbColumnKind_structure.SRC:
                    StbSecColumn_SRC srcSec = _sections.StbSecColumn_SRC.First(sec => sec.id == idSection);
                    curveList = SecRcColumnToCurves(srcSec.StbSecFigureColumn_SRC.Item, sectionPoints);
                    break;
                case StbColumnKind_structure.CFT:
                    StbSecColumn_CFT cftSec = _sections.StbSecColumn_CFT.First(sec => sec.id == idSection);
                    curveList = SecCftColumnToCurves(cftSec.StbSecSteelFigureColumn_CFT.Items, sectionPoints);
                    break;
                case StbColumnKind_structure.UNDEFINED:
                    throw new ArgumentException("Unsupported StbColumnKind");
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return curveList;
        }

        private SectionCurve[] SecCftColumnToCurves(IReadOnlyList<object> figures, IReadOnlyList<Point3d> sectionPoints)
        {
            var curveList = new List<SectionCurve>();
            Vector3d[] localAxis = Utils.CreateLocalAxis(sectionPoints);

            string bottom, center, top;
            switch (figures.Count)
            {
                case 1:
                    var same = figures[0] as StbSecSteelColumn_CFT_Same;
                    center = same.shape;
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[0], Utils.SectionPositionType.Column, localAxis));
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[3], Utils.SectionPositionType.Column, localAxis));
                    break;
                case 2:
                    var notSames = new[] { figures[0] as StbSecSteelColumn_CFT_NotSame, figures[1] as StbSecSteelColumn_CFT_NotSame };
                    bottom = notSames.First(item => item.pos == StbSecSteelColumn_CFT_NotSamePos.BOTTOM).shape;
                    top = notSames.First(item => item.pos == StbSecSteelColumn_CFT_NotSamePos.TOP).shape;
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, bottom, sectionPoints[0], Utils.SectionPositionType.Column, localAxis));
                    if (sectionPoints[1].Z > sectionPoints[0].Z)
                    {
                        curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, bottom, sectionPoints[1], Utils.SectionPositionType.Column, localAxis));
                        curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, top, sectionPoints[1], Utils.SectionPositionType.Column, localAxis));
                    }
                    else
                    {
                        curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, bottom, sectionPoints[2], Utils.SectionPositionType.Column, localAxis));
                        curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, top, sectionPoints[2], Utils.SectionPositionType.Column, localAxis));
                    }
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, top, sectionPoints[3], Utils.SectionPositionType.Column, localAxis));
                    break;
                case 3:
                    var three = new[] { figures[0] as StbSecSteelColumn_CFT_ThreeTypes, figures[1] as StbSecSteelColumn_CFT_ThreeTypes, figures[2] as StbSecSteelColumn_CFT_ThreeTypes };
                    bottom = three.First(item => item.pos == StbSecSteelColumn_CFT_ThreeTypesPos.BOTTOM).shape;
                    center = three.First(item => item.pos == StbSecSteelColumn_CFT_ThreeTypesPos.CENTER).shape;
                    top = three.First(item => item.pos == StbSecSteelColumn_CFT_ThreeTypesPos.TOP).shape;
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, bottom, sectionPoints[0], Utils.SectionPositionType.Column, localAxis));
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[1], Utils.SectionPositionType.Column, localAxis));
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[2], Utils.SectionPositionType.Column, localAxis));
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, top, sectionPoints[3], Utils.SectionPositionType.Column, localAxis));
                    break;
                default:
                    throw new ArgumentException("Unmatched StbSecSteelColumn_CFT");
            }
            foreach (var curve in curveList)
            {
                curve.IsCft = true;
            }
            return curveList.ToArray();
        }

        private static SectionCurve[] SecRcColumnToCurves(object figure, IReadOnlyList<Point3d> sectionPoints)
        {
            var curveList = new List<SectionCurve>();
            Vector3d[] localAxis = Utils.CreateLocalAxis(sectionPoints);

            switch (figure)
            {
                case StbSecColumn_RC_Rect rect:
                    curveList.Add(SectionCurve.CreateSolidColumnRect(sectionPoints[0], rect.width_X, rect.width_Y, localAxis));
                    curveList.Add(SectionCurve.CreateSolidColumnRect(sectionPoints[3], rect.width_X, rect.width_Y, localAxis));
                    break;
                case StbSecColumn_SRC_Rect rect:
                    curveList.Add(SectionCurve.CreateSolidColumnRect(sectionPoints[0], rect.width_X, rect.width_Y, localAxis));
                    curveList.Add(SectionCurve.CreateSolidColumnRect(sectionPoints[3], rect.width_X, rect.width_Y, localAxis));
                    break;
                case StbSecColumn_RC_Circle circle:
                    curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[0], circle.D, localAxis[0]));
                    curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[3], circle.D, localAxis[0]));
                    break;
                case StbSecColumn_SRC_Circle circle:
                    curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[0], circle.D, localAxis[0]));
                    curveList.Add(SectionCurve.CreateSolidColumnCircle(sectionPoints[3], circle.D, localAxis[0]));
                    break;
                default:
                    throw new Exception();
            }

            return curveList.ToArray();
        }

        private SectionCurve[] SecSteelColumnToCurves(IReadOnlyList<object> figures, IReadOnlyList<Point3d> sectionPoints)
        {
            var curveList = new List<SectionCurve>();
            Vector3d[] localAxis = Utils.CreateLocalAxis(sectionPoints);

            string bottom, center, top;
            switch (figures.Count)
            {
                case 1:
                    var same = figures[0] as StbSecSteelColumn_S_Same;
                    center = same.shape;
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[0], Utils.SectionPositionType.Column, localAxis));
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[3], Utils.SectionPositionType.Column, localAxis));
                    break;
                case 2:
                    var notSames = new[] { figures[0] as StbSecSteelColumn_S_NotSame, figures[1] as StbSecSteelColumn_S_NotSame };
                    bottom = notSames.First(item => item.pos == StbSecSteelColumn_S_NotSamePos.BOTTOM).shape;
                    top = notSames.First(item => item.pos == StbSecSteelColumn_S_NotSamePos.TOP).shape;
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, bottom, sectionPoints[0], Utils.SectionPositionType.Column, localAxis));
                    if (sectionPoints[1].Z > sectionPoints[0].Z)
                    {
                        curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, bottom, sectionPoints[1], Utils.SectionPositionType.Column, localAxis));
                        curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, top, sectionPoints[1], Utils.SectionPositionType.Column, localAxis));
                    }
                    else
                    {
                        curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, bottom, sectionPoints[2], Utils.SectionPositionType.Column, localAxis));
                        curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, top, sectionPoints[2], Utils.SectionPositionType.Column, localAxis));
                    }
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, top, sectionPoints[3], Utils.SectionPositionType.Column, localAxis));
                    break;
                case 3:
                    var three = new[] { figures[0] as StbSecSteelColumn_S_ThreeTypes, figures[1] as StbSecSteelColumn_S_ThreeTypes, figures[2] as StbSecSteelColumn_S_ThreeTypes };
                    bottom = three.First(item => item.pos == StbSecSteelColumn_S_ThreeTypesPos.BOTTOM).shape;
                    center = three.First(item => item.pos == StbSecSteelColumn_S_ThreeTypesPos.CENTER).shape;
                    top = three.First(item => item.pos == StbSecSteelColumn_S_ThreeTypesPos.TOP).shape;
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, bottom, sectionPoints[0], Utils.SectionPositionType.Column, localAxis));
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[1], Utils.SectionPositionType.Column, localAxis));
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[2], Utils.SectionPositionType.Column, localAxis));
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, top, sectionPoints[3], Utils.SectionPositionType.Column, localAxis));
                    break;
                default:
                    throw new ArgumentException("Unmatched StbSecSteelColumn_S");
            }

            return curveList.ToArray();
        }
    }
}
