using System;
using System.Collections.Generic;
using System.Linq;

using Rhino.Geometry;

using STBDotNet.v202;

namespace HoaryFox.Component.Utils.Geometry.BrepMaker
{
    public class Footing
    {
        private readonly IReadOnlyList<double> _tolerance;
        private readonly StbSections _sections;
        private readonly string _guid;

        public Footing(StbSections sections, IReadOnlyList<double> tolerance, string guid)
        {
            _tolerance = tolerance;
            _sections = sections;
            _guid = guid;
        }

        public Brep CreateFootingBrep(string idSection, double rotate, IReadOnlyList<Point3d> sectionPoints, Vector3d axis)
        {
            SectionCurve[] curveList = CreateCurveList(idSection, sectionPoints, axis);
            Utils.RotateCurveList(axis, curveList, rotate, sectionPoints);
            return Utils.CreateCapedBrepFromLoft(curveList, _tolerance[0]);
        }

        private SectionCurve[] CreateCurveList(string idSection, IReadOnlyList<Point3d> sectionPoints, Vector3d axis)
        {
            SectionCurve[] curveList;
            try
            {
                StbSecFoundation_RC rcSec = _sections.StbSecFoundation_RC.First(sec => sec.id == idSection);
                curveList = SecRcFootingToCurves(rcSec.StbSecFigureFoundation_RC.Item, sectionPoints, axis);
            }
            catch (Exception)
            {
                throw new ArgumentException($"Error converting guid: {_guid}\nThe cross-sectional shape of the footing seems to be wrong. Please check.");
            }

            return curveList;
        }

        private static SectionCurve[] SecRcFootingToCurves(object figure, IReadOnlyList<Point3d> sectionPoints, Vector3d axis)
        {
            var curveList = new List<SectionCurve>();
            Vector3d[] localAxis = Utils.CreateLocalAxis(sectionPoints);

            switch (figure)
            {
                case StbSecFoundation_RC_Rect rect:
                    var topPt = sectionPoints[3] - axis * rect.depth;
                    curveList.Add(SectionCurve.CreateSolidColumnRect(topPt, rect.width_X, rect.width_Y, localAxis));
                    curveList.Add(SectionCurve.CreateSolidColumnRect(sectionPoints[3], rect.width_X, rect.width_Y, localAxis));
                    break;
                default:
                    throw new ArgumentException("Unsupported StbSecFoundation_RC type.");
            }

            return curveList.ToArray();
        }
    }
}
