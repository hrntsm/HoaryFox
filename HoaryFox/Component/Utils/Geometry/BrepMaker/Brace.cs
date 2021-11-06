using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component.Utils.Geometry.BrepMaker
{
    public class Brace
    {
        private readonly IReadOnlyList<double> _tolerance;
        private readonly StbSections _sections;

        public Brace(StbSections sections, IReadOnlyList<double> tolerance)
        {
            _tolerance = tolerance;
            _sections = sections;
        }

        public Brep CreateBraceBrep(string idSection, double rotate, StbBraceKind_structure kind, IReadOnlyList<Point3d> sectionPoints, Vector3d memberAxis)
        {
            List<Curve> curveList = CreateFromEachBraceKind(idSection, kind, sectionPoints);

            Utils.RotateCurveList(memberAxis, curveList, rotate, sectionPoints);
            Brep brep = Brep.CreateFromLoft(curveList, Point3d.Unset, Point3d.Unset, LoftType.Straight, false)[0]
                .CapPlanarHoles(_tolerance[0]);

            if (brep.GetVolume() < 0)
            {
                brep.Flip();
            }
            return brep;
        }

        private List<Curve> CreateFromEachBraceKind(string idSection, StbBraceKind_structure kind, IReadOnlyList<Point3d> sectionPoints)
        {
            List<Curve> curveList;
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

            return curveList;
        }

        private List<Curve> SecSteelBraceToCurves(IReadOnlyList<object> figures, IReadOnlyList<Point3d> sectionPoints)
        {
            var curveList = new List<Curve>();
            string start, center, end;
            Vector3d[] localAxis = Utils.CreateLocalAxis(sectionPoints);

            switch (figures.Count)
            {
                case 1:
                    var same = figures[0] as StbSecSteelBrace_S_Same;
                    center = same.shape;
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[0], Utils.SectionType.Brace, localAxis));
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[3], Utils.SectionType.Brace, localAxis));
                    break;
                case 2:
                    var notSames = new[] { figures[0] as StbSecSteelBrace_S_NotSame, figures[1] as StbSecSteelBrace_S_NotSame };
                    start = notSames.First(sec => sec.pos == StbSecSteelBrace_S_NotSamePos.BOTTOM).shape;
                    end = notSames.First(sec => sec.pos == StbSecSteelBrace_S_NotSamePos.TOP).shape;
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, start, sectionPoints[0], Utils.SectionType.Brace, localAxis));
                    curveList.Add(sectionPoints[0] == sectionPoints[1]
                        ? SteelSections.GetCurve(_sections.StbSecSteel, start, sectionPoints[2], Utils.SectionType.Brace, localAxis)
                        : SteelSections.GetCurve(_sections.StbSecSteel, start, sectionPoints[1], Utils.SectionType.Brace, localAxis));
                    curveList.Add(sectionPoints[0] == sectionPoints[1]
                        ? SteelSections.GetCurve(_sections.StbSecSteel, end, sectionPoints[2], Utils.SectionType.Brace, localAxis)
                        : SteelSections.GetCurve(_sections.StbSecSteel, end, sectionPoints[1], Utils.SectionType.Brace, localAxis));
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, end, sectionPoints[3], Utils.SectionType.Brace, localAxis));
                    break;
                case 3:
                    var three = new[] { figures[0] as StbSecSteelBrace_S_ThreeTypes, figures[1] as StbSecSteelBrace_S_ThreeTypes, figures[2] as StbSecSteelBrace_S_ThreeTypes };
                    start = three.First(sec => sec.pos == StbSecSteelBrace_S_ThreeTypesPos.BOTTOM).shape;
                    center = three.First(sec => sec.pos == StbSecSteelBrace_S_ThreeTypesPos.CENTER).shape;
                    end = three.First(sec => sec.pos == StbSecSteelBrace_S_ThreeTypesPos.TOP).shape;
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, start, sectionPoints[0], Utils.SectionType.Brace, localAxis));
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[1], Utils.SectionType.Brace, localAxis));
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[2], Utils.SectionType.Brace, localAxis));
                    curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, end, sectionPoints[3], Utils.SectionType.Brace, localAxis));
                    break;
                default:
                    throw new ArgumentException("Unmatched StbSecSteelBrace_S");
            }

            return curveList;
        }
    }
}
