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
        public Column(StbSections sections, IReadOnlyList<double> tolerance)
        {
            _tolerance = tolerance;
            _sections = sections;
        }

        public Brep CreateColumnBrep(string idSection, double rotate, StbColumnKind_structure kind, IReadOnlyList<Point3d> sectionPoints, Vector3d memberAxis)
        {
            List<Curve> curveList = CreateFromEachColumnKind(idSection, kind, sectionPoints);
            Utils.RotateCurveList(memberAxis, curveList, rotate, sectionPoints);
            return Utils.CreateCapedBrepFromLoft(curveList, _tolerance[0]);
        }

        private List<Curve> CreateFromEachColumnKind(string idSection, StbColumnKind_structure kind, IReadOnlyList<Point3d> sectionPoints)
        {
            List<Curve> curveList;
            try
            {
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
                    case StbColumnKind_structure.UNDEFINED:
                        throw new ArgumentException("Unsupported StbColumnKind");
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception)
            {
                throw new ArgumentException("The cross-sectional shape of the column or post seems to be wrong. Please check.");
            }

            return curveList;
        }

        private static List<Curve> SecRcColumnToCurves(object figure, IReadOnlyList<Point3d> sectionPoints)
        {
            var curveList = new List<Curve>();
            Vector3d[] localAxis = Utils.CreateLocalAxis(sectionPoints);

            switch (figure)
            {
                case StbSecColumn_RC_Rect rect:
                    curveList.Add(new PolylineCurve(
                        SectionCornerPoints.ColumnRect(sectionPoints[0], rect.width_X, rect.width_Y, localAxis[1], localAxis[2])));
                    curveList.Add(new PolylineCurve(
                        SectionCornerPoints.ColumnRect(sectionPoints[3], rect.width_X, rect.width_Y, localAxis[1], localAxis[2])));
                    break;
                case StbSecColumn_SRC_Rect rect:
                    curveList.Add(new PolylineCurve(
                        SectionCornerPoints.ColumnRect(sectionPoints[0], rect.width_X, rect.width_Y, localAxis[1], localAxis[2])));
                    curveList.Add(new PolylineCurve(
                        SectionCornerPoints.ColumnRect(sectionPoints[3], rect.width_X, rect.width_Y, localAxis[1], localAxis[2])));
                    break;
                case StbSecColumn_RC_Circle circle:
                    curveList.Add(SectionCornerPoints.ColumnPipe(sectionPoints[0], circle.D, localAxis[0]));
                    curveList.Add(SectionCornerPoints.ColumnPipe(sectionPoints[3], circle.D, localAxis[0]));
                    break;
                case StbSecColumn_SRC_Circle circle:
                    curveList.Add(SectionCornerPoints.ColumnPipe(sectionPoints[0], circle.D, localAxis[0]));
                    curveList.Add(SectionCornerPoints.ColumnPipe(sectionPoints[3], circle.D, localAxis[0]));
                    break;
                default:
                    throw new Exception();
            }

            return curveList;
        }

        private List<Curve> SecSteelColumnToCurves(IReadOnlyList<object> figures, IReadOnlyList<Point3d> sectionPoints)
        {
            var curveList = new List<Curve>();
            Vector3d[] localAxis = Utils.CreateLocalAxis(sectionPoints);

            string bottom, center, top;
            switch (figures.Count)
            {
                case 1:
                    var same = figures[0] as StbSecSteelColumn_S_Same;
                    center = same.shape;
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[0], Utils.SectionType.Column, localAxis));
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[3], Utils.SectionType.Column, localAxis));
                    break;
                case 2:
                    var notSames = new[] { figures[0] as StbSecSteelColumn_S_NotSame, figures[1] as StbSecSteelColumn_S_NotSame };
                    bottom = notSames.First(item => item.pos == StbSecSteelColumn_S_NotSamePos.BOTTOM).shape;
                    top = notSames.First(item => item.pos == StbSecSteelColumn_S_NotSamePos.TOP).shape;
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, bottom, sectionPoints[0], Utils.SectionType.Column, localAxis));
                    if (sectionPoints[1].Z > sectionPoints[0].Z)
                    {
                        curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, bottom, sectionPoints[1], Utils.SectionType.Column, localAxis));
                        curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, top, sectionPoints[1], Utils.SectionType.Column, localAxis));
                    }
                    else
                    {
                        curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, bottom, sectionPoints[2], Utils.SectionType.Column, localAxis));
                        curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, top, sectionPoints[2], Utils.SectionType.Column, localAxis));
                    }
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, top, sectionPoints[3], Utils.SectionType.Column, localAxis));
                    break;
                case 3:
                    var three = new[] { figures[0] as StbSecSteelColumn_S_ThreeTypes, figures[1] as StbSecSteelColumn_S_ThreeTypes, figures[2] as StbSecSteelColumn_S_ThreeTypes };
                    bottom = three.First(item => item.pos == StbSecSteelColumn_S_ThreeTypesPos.BOTTOM).shape;
                    center = three.First(item => item.pos == StbSecSteelColumn_S_ThreeTypesPos.CENTER).shape;
                    top = three.First(item => item.pos == StbSecSteelColumn_S_ThreeTypesPos.TOP).shape;
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, bottom, sectionPoints[0], Utils.SectionType.Column, localAxis));
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[1], Utils.SectionType.Column, localAxis));
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[2], Utils.SectionType.Column, localAxis));
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, top, sectionPoints[3], Utils.SectionType.Column, localAxis));
                    break;
                default:
                    throw new ArgumentException("Unmatched StbSecSteelColumn_S");
            }

            return curveList;
        }
    }
}
