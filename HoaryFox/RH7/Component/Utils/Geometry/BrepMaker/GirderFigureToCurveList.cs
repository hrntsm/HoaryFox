using System;
using System.Collections.Generic;
using System.Linq;

using Rhino.Geometry;

using STBDotNet.v202;

namespace HoaryFox.Component.Utils.Geometry.BrepMaker
{
    public class GirderFigureToCurveList
    {
        private readonly StbSections _sections;

        public GirderFigureToCurveList(StbSections sections)
        {
            _sections = sections;
        }

        public void SingleFigure(IReadOnlyList<object> figures, IReadOnlyList<Point3d> sectionPoints, ICollection<SectionCurve> curveList, Vector3d[] localAxis)
        {
            var straight = figures[0] as StbSecSteelBeam_S_Straight;
            string center = straight.shape;
            curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[0], Utils.SectionType.Beam, localAxis));
            curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[5], Utils.SectionType.Beam, localAxis));
        }

        public void TwoFigure(IReadOnlyList<object> figures, IReadOnlyList<Point3d> sectionPoints, ICollection<SectionCurve> curveList, Vector3d[] localAxis)
        {
            switch (figures[0])
            {
                case StbSecSteelBeam_S_Taper _:
                    {
                        TwoFigureTaper(figures, sectionPoints, curveList, localAxis);
                        break;
                    }
                case StbSecSteelBeam_S_Joint _:
                    {
                        TwoFigureJoint(figures, sectionPoints, curveList, localAxis);
                        break;
                    }
                case StbSecSteelBeam_S_Haunch _:
                    {
                        TwoFigureHaunch(figures, sectionPoints, curveList, localAxis);
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("Unmatched StbSecSteelBeam_S");
                    }
            }
        }

        public void ThreeFigure(IReadOnlyList<object> figures, IReadOnlyList<Point3d> sectionPoints, ICollection<SectionCurve> curveList, Vector3d[] localAxis)
        {
            switch (figures[0])
            {
                case StbSecSteelBeam_S_Haunch _:
                    {
                        ThreeFigureHaunch(figures, sectionPoints, curveList, localAxis);
                        break;
                    }
                case StbSecSteelBeam_S_Joint _:
                    {
                        ThreeFigureJoint(figures, sectionPoints, curveList, localAxis);
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("Unmatched StbSecSteelBeam_S");
                    }
            }
        }
        private void TwoFigureTaper(IReadOnlyList<object> figures, IReadOnlyList<Point3d> sectionPoints, ICollection<SectionCurve> curveList, Vector3d[] localAxis)
        {
            var tapers = new[] { figures[0] as StbSecSteelBeam_S_Taper, figures[1] as StbSecSteelBeam_S_Taper };
            string start = tapers.First(sec => sec.pos == StbSecSteelBeam_S_TaperPos.START).shape;
            string end = tapers.First(sec => sec.pos == StbSecSteelBeam_S_TaperPos.END).shape;
            curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, start, sectionPoints[0], Utils.SectionType.Beam, localAxis));
            curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, start, sectionPoints[1], Utils.SectionType.Beam, localAxis));
            curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, end, sectionPoints[4], Utils.SectionType.Beam, localAxis));
            curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, end, sectionPoints[5], Utils.SectionType.Beam, localAxis));
        }

        private void TwoFigureJoint(IReadOnlyList<object> figures, IReadOnlyList<Point3d> sectionPoints, ICollection<SectionCurve> curveList, Vector3d[] localAxis)
        {
            var joint = new[] { figures[0] as StbSecSteelBeam_S_Joint, figures[1] as StbSecSteelBeam_S_Joint };
            string center = joint.First(sec => sec.pos == StbSecSteelBeam_S_JointPos.CENTER).shape;

            if (joint.FirstOrDefault(sec => sec.pos == StbSecSteelBeam_S_JointPos.START) != null)
            {
                string start = joint.First(sec => sec.pos == StbSecSteelBeam_S_JointPos.START).shape;
                curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, start, sectionPoints[0], Utils.SectionType.Beam, localAxis));
                curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[2], Utils.SectionType.Beam, localAxis));
                curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[3], Utils.SectionType.Beam, localAxis));
                curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[5], Utils.SectionType.Beam, localAxis));
            }
            else
            {
                string end = joint.First(sec => sec.pos == StbSecSteelBeam_S_JointPos.END).shape;
                curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[0], Utils.SectionType.Beam, localAxis));
                curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[2], Utils.SectionType.Beam, localAxis));
                curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[3], Utils.SectionType.Beam, localAxis));
                curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, end, sectionPoints[5], Utils.SectionType.Beam, localAxis));
            }
        }

        private void TwoFigureHaunch(IReadOnlyList<object> figures, IReadOnlyList<Point3d> sectionPoints, ICollection<SectionCurve> curveList, Vector3d[] localAxis)
        {
            var joint = new[] { figures[0] as StbSecSteelBeam_S_Haunch, figures[1] as StbSecSteelBeam_S_Haunch };
            string center;

            if (joint.FirstOrDefault(sec => sec.pos == StbSecSteelBeam_S_HaunchPos.START) != null)
            {
                string start = joint.First(sec => sec.pos == StbSecSteelBeam_S_HaunchPos.START).shape;
                center = joint.First(sec => sec.pos == StbSecSteelBeam_S_HaunchPos.CENTER).shape;
                curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, start, sectionPoints[0], Utils.SectionType.Beam, localAxis));
                curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[1], Utils.SectionType.Beam, localAxis));
                curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[4], Utils.SectionType.Beam, localAxis));
                curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[5], Utils.SectionType.Beam, localAxis));
            }
            else
            {
                center = joint.First(sec => sec.pos == StbSecSteelBeam_S_HaunchPos.CENTER).shape;
                string end = joint.First(sec => sec.pos == StbSecSteelBeam_S_HaunchPos.END).shape;
                curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[0], Utils.SectionType.Beam, localAxis));
                curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[1], Utils.SectionType.Beam, localAxis));
                curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[4], Utils.SectionType.Beam, localAxis));
                curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, end, sectionPoints[5], Utils.SectionType.Beam, localAxis));
            }
        }

        private void ThreeFigureHaunch(IReadOnlyList<object> figures, IReadOnlyList<Point3d> sectionPoints, ICollection<SectionCurve> curveList, Vector3d[] localAxis)
        {
            var haunch = new[] { figures[0] as StbSecSteelBeam_S_Haunch, figures[1] as StbSecSteelBeam_S_Haunch, figures[2] as StbSecSteelBeam_S_Haunch };
            string start = haunch.First(sec => sec.pos == StbSecSteelBeam_S_HaunchPos.START).shape;
            string center = haunch.First(sec => sec.pos == StbSecSteelBeam_S_HaunchPos.CENTER).shape;
            string end = haunch.First(sec => sec.pos == StbSecSteelBeam_S_HaunchPos.END).shape;
            curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, start, sectionPoints[0], Utils.SectionType.Beam, localAxis));
            curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[1], Utils.SectionType.Beam, localAxis));
            curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[4], Utils.SectionType.Beam, localAxis));
            curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, end, sectionPoints[5], Utils.SectionType.Beam, localAxis));
        }

        private void ThreeFigureJoint(IReadOnlyList<object> figures, IReadOnlyList<Point3d> sectionPoints, ICollection<SectionCurve> curveList, Vector3d[] localAxis)
        {
            var joint = new[] { figures[0] as StbSecSteelBeam_S_Joint, figures[1] as StbSecSteelBeam_S_Joint, figures[2] as StbSecSteelBeam_S_Joint };
            string start = joint.First(sec => sec.pos == StbSecSteelBeam_S_JointPos.START).shape;
            string center = joint.First(sec => sec.pos == StbSecSteelBeam_S_JointPos.CENTER).shape;
            string end = joint.First(sec => sec.pos == StbSecSteelBeam_S_JointPos.END).shape;
            curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, start, sectionPoints[0], Utils.SectionType.Beam, localAxis));
            curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[2], Utils.SectionType.Beam, localAxis));
            curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, center, sectionPoints[3], Utils.SectionType.Beam, localAxis));
            curveList.Add(SteelSections.GetCurve(_sections.StbSecSteel, end, sectionPoints[5], Utils.SectionType.Beam, localAxis));
        }
    }
}
