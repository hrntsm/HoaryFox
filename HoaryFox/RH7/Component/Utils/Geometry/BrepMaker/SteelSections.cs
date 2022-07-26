using System;
using System.Collections.Generic;
using System.Linq;

using Rhino.Geometry;

using STBDotNet.v202;

namespace HoaryFox.Component.Utils.Geometry.BrepMaker
{
    public static class SteelSections
    {
        public static SectionCurve GetCurve(StbSecSteel secSteel, string shape, Point3d point, Utils.SectionType type, Vector3d[] localAxis)
        {
            // TODO: foreach なのに最初にマッチしたもので return しているのでが変なので直す。
            if (secSteel.StbSecBuildBOX != null)
            {
                foreach (StbSecBuildBOX box in secSteel.StbSecBuildBOX.Where(box => box.name == shape))
                {
                    return CurveFromStbSecBox(localAxis, point, type, box.A, box.B, box.t1, box.t2);
                }
            }

            if (secSteel.StbSecRollBOX != null)
            {
                foreach (StbSecRollBOX box in secSteel.StbSecRollBOX.Where(box => box.name == shape))
                {
                    return CurveFromStbSecBox(localAxis, point, type, box.A, box.B, box.t, box.t);
                }
            }

            if (secSteel.StbSecFlatBar != null)
            {
                foreach (StbSecFlatBar flatBar in secSteel.StbSecFlatBar.Where(bar => bar.name == shape))
                {
                    return CurveFromStbSecBox(localAxis, point, type, flatBar.B, flatBar.t, -1, -1);
                }
            }

            if (secSteel.StbSecBuildH != null)
            {
                foreach (StbSecBuildH buildH in secSteel.StbSecBuildH.Where(buildH => buildH.name == shape))
                {
                    return CurveFromStbSecH(localAxis, point, type, buildH.A, buildH.B, buildH.t1, buildH.t2);
                }
            }

            if (secSteel.StbSecRollH != null)
            {
                foreach (StbSecRollH rollH in secSteel.StbSecRollH.Where(rollH => rollH.name == shape))
                {
                    return CurveFromStbSecH(localAxis, point, type, rollH.A, rollH.B, rollH.t1, rollH.t2);
                }
            }

            if (secSteel.StbSecRollL != null)
            {
                foreach (StbSecRollL rollL in secSteel.StbSecRollL.Where(rollL => rollL.name == shape))
                {
                    return CurveFromStbSecL(localAxis, point, type, rollL);
                }
            }

            if (secSteel.StbSecPipe != null)
            {
                foreach (StbSecPipe pipe in secSteel.StbSecPipe.Where(pipe => pipe.name == shape))
                {
                    return CurveFromStbSecPipe(localAxis, point, type, pipe.D, pipe.t);
                }
            }

            if (secSteel.StbSecRoundBar != null)
            {
                foreach (StbSecRoundBar bar in secSteel.StbSecRoundBar.Where(pipe => pipe.name == shape))
                {
                    return CurveFromStbSecPipe(localAxis, point, type, bar.R, -1);
                }
            }

            // TODO: C 断面を実装する
            if (secSteel.StbSecRollC != null || secSteel.StbSecLipC != null)
            {
                throw new ArgumentException("StbSecRollC & StbSecLipC is not supported");
            }

            throw new ArgumentException("There are no matching steel section");
        }

        private static SectionCurve CurveFromStbSecPipe(IReadOnlyList<Vector3d> localAxis, Point3d point, Utils.SectionType type, double diameter, double thickness)
        {
            var radius = diameter / 2;
            var sectionCurve = new SectionCurve
            {
                Shape = thickness > 0 ? SectionShape.Pipe : SectionShape.Circle,
                Type = thickness > 0 ? SectionType.Hollow : SectionType.Solid,
                XAxis = localAxis[0],
            };
            switch (type)
            {
                case Utils.SectionType.Column:
                case Utils.SectionType.Brace:
                    sectionCurve.OuterCurve = SectionCornerPoints.ColumnPipe(point, radius, localAxis[0]);
                    sectionCurve.InnerCurve = thickness > 0 ? SectionCornerPoints.ColumnPipe(point, radius - thickness, localAxis[0]) : null;
                    break;
                case Utils.SectionType.Beam:
                    sectionCurve.OuterCurve = SectionCornerPoints.BeamPipe(point, radius, localAxis[0]);
                    sectionCurve.InnerCurve = thickness > 0 ? SectionCornerPoints.BeamPipe(point, radius - thickness, localAxis[0]) : null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return sectionCurve;
        }

        private static SectionCurve CurveFromStbSecL(IReadOnlyList<Vector3d> localAxis, Point3d point, Utils.SectionType type, StbSecRollL rollL)
        {
            var sectionCurve = new SectionCurve
            {
                Shape = SectionShape.L,
                Type = SectionType.Solid,
                XAxis = localAxis[0],
            };
            switch (type)
            {
                case Utils.SectionType.Column:
                case Utils.SectionType.Brace:
                    sectionCurve.OuterCurve = new PolylineCurve(SectionCornerPoints.ColumnL(point, rollL.A, rollL.B, rollL.t1, rollL.t2, rollL.type, localAxis[1], localAxis[2]));
                    break;
                case Utils.SectionType.Beam:
                    sectionCurve.OuterCurve = new PolylineCurve(SectionCornerPoints.BeamL(point, rollL.A, rollL.B, rollL.t1, rollL.t2, rollL.type, localAxis[1], localAxis[2]));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return sectionCurve;
        }

        private static SectionCurve CurveFromStbSecBox(IReadOnlyList<Vector3d> localAxis, Point3d point, Utils.SectionType type, double A, double B, double t1, double t2)
        {
            var sectionCurve = new SectionCurve
            {
                Shape = t1 > 0 ? SectionShape.Box : SectionShape.Rectangle,
                Type = t1 > 0 ? SectionType.Hollow : SectionType.Solid,
                XAxis = localAxis[0],
            };
            switch (type)
            {
                case Utils.SectionType.Column:
                case Utils.SectionType.Brace:
                    sectionCurve.OuterCurve = new PolylineCurve(SectionCornerPoints.ColumnRect(point, B, A, localAxis[1], localAxis[2]));
                    sectionCurve.InnerCurve = t1 > 0 ? new PolylineCurve(SectionCornerPoints.ColumnRect(point, B - 2 * t1, A - 2 * t2, localAxis[1], localAxis[2])) : null;
                    break;
                case Utils.SectionType.Beam:
                    sectionCurve.OuterCurve = new PolylineCurve(SectionCornerPoints.BeamRect(point, B, A, localAxis[1], localAxis[2]));
                    sectionCurve.InnerCurve = t1 > 0 ? new PolylineCurve(SectionCornerPoints.BeamRect(point, B - 2 * t1, A - 2 * t2, localAxis[1], localAxis[2])) : null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return sectionCurve;
        }

        private static SectionCurve CurveFromStbSecH(IReadOnlyList<Vector3d> localAxis, Point3d point, Utils.SectionType type, double A, double B, double t1, double t2)
        {
            var sectionCurve = new SectionCurve
            {
                Shape = SectionShape.H,
                Type = SectionType.Solid,
                XAxis = localAxis[0],
            };
            switch (type)
            {
                case Utils.SectionType.Column:
                case Utils.SectionType.Brace:
                    sectionCurve.OuterCurve = new PolylineCurve(SectionCornerPoints.ColumnH(point, A, B, t1, t2, localAxis[1], localAxis[2]));
                    break;
                case Utils.SectionType.Beam:
                    sectionCurve.OuterCurve = new PolylineCurve(SectionCornerPoints.BeamH(point, A, B, t1, t2, localAxis[1], localAxis[2]));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return sectionCurve;
        }
    }
}
