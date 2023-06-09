using System;
using System.Collections.Generic;
using System.Linq;

using Rhino.Geometry;

using STBDotNet.v202;

namespace HoaryFox.Component.Utils.Geometry.BrepMaker
{
    public static class SteelSections
    {
        public static SectionCurve GetCurve(StbSecSteel secSteel, string shape, Point3d point, Utils.SectionPositionType type, Vector3d[] localAxis)
        {
            if (secSteel.StbSecBuildBOX != null)
            {
                StbSecBuildBOX box = secSteel.StbSecBuildBOX.FirstOrDefault(b => b.name == shape);
                if (box != null)
                {
                    return CurveFromStbSecBox(localAxis, point, type, box.A, box.B, box.t1, box.t2);
                }
            }

            if (secSteel.StbSecRollBOX != null)
            {
                StbSecRollBOX box = secSteel.StbSecRollBOX.FirstOrDefault(b => b.name == shape);
                if (box != null)
                {
                    return CurveFromStbSecBox(localAxis, point, type, box.A, box.B, box.t, box.t);
                }
            }

            if (secSteel.StbSecFlatBar != null)
            {
                StbSecFlatBar flatBar = secSteel.StbSecFlatBar.FirstOrDefault(bar => bar.name == shape);
                if (flatBar != null)
                {
                    return CurveFromStbSecBox(localAxis, point, type, flatBar.B, flatBar.t, -1, -1);
                }
            }

            if (secSteel.StbSecBuildH != null)
            {
                StbSecBuildH buildH = secSteel.StbSecBuildH.FirstOrDefault(bH => bH.name == shape);
                if (buildH != null)
                {
                    return CurveFromStbSecH(localAxis, point, type, buildH.A, buildH.B, buildH.t1, buildH.t2);
                }
            }

            if (secSteel.StbSecRollH != null)
            {
                StbSecRollH rollH = secSteel.StbSecRollH.FirstOrDefault(rH => rH.name == shape);
                if (rollH != null)
                {
                    return CurveFromStbSecH(localAxis, point, type, rollH.A, rollH.B, rollH.t1, rollH.t2);
                }
            }

            if (secSteel.StbSecRollL != null)
            {
                StbSecRollL rollL = secSteel.StbSecRollL.FirstOrDefault(rL => rL.name == shape);
                if (rollL != null)
                {
                    return CurveFromStbSecL(localAxis, point, type, rollL);
                }
            }

            if (secSteel.StbSecPipe != null)
            {
                StbSecPipe pipe = secSteel.StbSecPipe.FirstOrDefault(p => p.name == shape);
                if (pipe != null)
                {
                    return CurveFromStbSecPipe(localAxis, point, type, pipe.D, pipe.t);
                }
            }

            if (secSteel.StbSecRoundBar != null)
            {
                StbSecRoundBar bar = secSteel.StbSecRoundBar.FirstOrDefault(b => b.name == shape);
                if (bar != null)
                {
                    return CurveFromStbSecPipe(localAxis, point, type, bar.R, -1);
                }
            }

            if (secSteel.StbSecRollC != null)
            {
                StbSecRollC rollC = secSteel.StbSecRollC.FirstOrDefault(rC => rC.name == shape);
                if (rollC != null)
                {
                    return CurveFromStbSecRollC(localAxis, point, type, rollC);
                }
            }

            if (secSteel.StbSecLipC != null)
            {
                StbSecLipC lipC = secSteel.StbSecLipC.FirstOrDefault(lC => lC.name == shape);
                if (lipC != null)
                {
                    return CurveFromStbSecLipC(localAxis, point, type, lipC);
                }
            }

            throw new ArgumentException("There are no matching steel section");
        }

        private static SectionCurve CurveFromStbSecLipC(IReadOnlyList<Vector3d> localAxis, Point3d point, Utils.SectionPositionType type, StbSecLipC lipC)
        {
            StbSecRollCType shapeType;
            switch (lipC.type)
            {
                case StbSecLipCType.SINGLE:
                    shapeType = StbSecRollCType.SINGLE;
                    break;
                case StbSecLipCType.BACKTOBACK:
                    shapeType = StbSecRollCType.BACKTOBACK;
                    break;
                case StbSecLipCType.FACETOFACE:
                    shapeType = StbSecRollCType.FACETOFACE;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return CurveFromSecC(localAxis, point, type, lipC.A, lipC.H, lipC.t, lipC.t, shapeType);
        }

        private static SectionCurve CurveFromStbSecRollC(IReadOnlyList<Vector3d> localAxis, Point3d point, Utils.SectionPositionType type, StbSecRollC rollC)
        {
            return CurveFromSecC(localAxis, point, type, rollC.B, rollC.A, rollC.t1, rollC.t2, rollC.type);
        }

        private static SectionCurve CurveFromSecC(IReadOnlyList<Vector3d> localAxis, Point3d point, Utils.SectionPositionType type, double width, double height, double tw, double tf, StbSecRollCType shapeType)
        {
            var sectionCurve = new SectionCurve
            {
                Shape = SectionShape.C,
                Type = shapeType == StbSecRollCType.FACETOFACE ? SectionType.Hollow : SectionType.Solid,
                XAxis = localAxis[0],
            };
            switch (type)
            {
                case Utils.SectionPositionType.Column:
                case Utils.SectionPositionType.Brace:
                    if (shapeType == StbSecRollCType.FACETOFACE)
                    {
                        sectionCurve.OuterCurve = new PolylineCurve(SectionCornerPoints.ColumnRect(point, 2 * width, height, localAxis[1], localAxis[2]));
                        sectionCurve.InnerCurve = new PolylineCurve(SectionCornerPoints.ColumnRect(point, 2 * width - tw * 2, height - tf * 2, localAxis[1], localAxis[2]));
                    }
                    else
                    {
                        sectionCurve.OuterCurve = new PolylineCurve(SectionCornerPoints.ColumnC(point, height, width, tw, tf, shapeType, localAxis[1], localAxis[2]));
                    }
                    break;
                case Utils.SectionPositionType.Beam:
                    if (shapeType == StbSecRollCType.FACETOFACE)
                    {
                        sectionCurve.OuterCurve = new PolylineCurve(SectionCornerPoints.BeamRect(point, height, width, localAxis[1], localAxis[2]));
                        sectionCurve.InnerCurve = new PolylineCurve(SectionCornerPoints.BeamRect(point, height - tf * 2, width - tw * 2, localAxis[1], localAxis[2]));
                    }
                    else
                    {
                        sectionCurve.OuterCurve = new PolylineCurve(SectionCornerPoints.BeamC(point, height, width, tw, tf, shapeType, localAxis[1], localAxis[2]));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return sectionCurve;
        }

        private static SectionCurve CurveFromStbSecPipe(IReadOnlyList<Vector3d> localAxis, Point3d point, Utils.SectionPositionType type, double diameter, double thickness)
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
                case Utils.SectionPositionType.Column:
                case Utils.SectionPositionType.Brace:
                    sectionCurve.OuterCurve = SectionCornerPoints.ColumnPipe(point, radius, localAxis[0]);
                    sectionCurve.InnerCurve = thickness > 0 ? SectionCornerPoints.ColumnPipe(point, radius - thickness, localAxis[0]) : null;
                    break;
                case Utils.SectionPositionType.Beam:
                    sectionCurve.OuterCurve = SectionCornerPoints.BeamPipe(point, radius, localAxis[0]);
                    sectionCurve.InnerCurve = thickness > 0 ? SectionCornerPoints.BeamPipe(point, radius - thickness, localAxis[0]) : null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return sectionCurve;
        }

        private static SectionCurve CurveFromStbSecL(IReadOnlyList<Vector3d> localAxis, Point3d point, Utils.SectionPositionType type, StbSecRollL rollL)
        {
            var sectionCurve = new SectionCurve
            {
                Shape = SectionShape.L,
                Type = SectionType.Solid,
                XAxis = localAxis[0],
            };
            switch (type)
            {
                case Utils.SectionPositionType.Column:
                case Utils.SectionPositionType.Brace:
                    sectionCurve.OuterCurve = new PolylineCurve(SectionCornerPoints.ColumnL(point, rollL.A, rollL.B, rollL.t1, rollL.t2, rollL.type, localAxis[1], localAxis[2]));
                    break;
                case Utils.SectionPositionType.Beam:
                    sectionCurve.OuterCurve = new PolylineCurve(SectionCornerPoints.BeamL(point, rollL.A, rollL.B, rollL.t1, rollL.t2, rollL.type, localAxis[1], localAxis[2]));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return sectionCurve;
        }

        private static SectionCurve CurveFromStbSecBox(IReadOnlyList<Vector3d> localAxis, Point3d point, Utils.SectionPositionType type, double A, double B, double t1, double t2)
        {
            var sectionCurve = new SectionCurve
            {
                Shape = t1 > 0 ? SectionShape.Box : SectionShape.Rectangle,
                Type = t1 > 0 ? SectionType.Hollow : SectionType.Solid,
                XAxis = localAxis[0],
            };
            switch (type)
            {
                case Utils.SectionPositionType.Column:
                case Utils.SectionPositionType.Brace:
                    sectionCurve.OuterCurve = new PolylineCurve(SectionCornerPoints.ColumnRect(point, B, A, localAxis[1], localAxis[2]));
                    sectionCurve.InnerCurve = t1 > 0 ? new PolylineCurve(SectionCornerPoints.ColumnRect(point, B - 2 * t1, A - 2 * t2, localAxis[1], localAxis[2])) : null;
                    break;
                case Utils.SectionPositionType.Beam:
                    sectionCurve.OuterCurve = new PolylineCurve(SectionCornerPoints.BeamRect(point, B, A, localAxis[1], localAxis[2]));
                    sectionCurve.InnerCurve = t1 > 0 ? new PolylineCurve(SectionCornerPoints.BeamRect(point, B - 2 * t1, A - 2 * t2, localAxis[1], localAxis[2])) : null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return sectionCurve;
        }

        private static SectionCurve CurveFromStbSecH(IReadOnlyList<Vector3d> localAxis, Point3d point, Utils.SectionPositionType type, double A, double B, double t1, double t2)
        {
            var sectionCurve = new SectionCurve
            {
                Shape = SectionShape.H,
                Type = SectionType.Solid,
                XAxis = localAxis[0],
            };
            switch (type)
            {
                case Utils.SectionPositionType.Column:
                case Utils.SectionPositionType.Brace:
                    sectionCurve.OuterCurve = new PolylineCurve(SectionCornerPoints.ColumnH(point, A, B, t1, t2, localAxis[1], localAxis[2]));
                    break;
                case Utils.SectionPositionType.Beam:
                    sectionCurve.OuterCurve = new PolylineCurve(SectionCornerPoints.BeamH(point, A, B, t1, t2, localAxis[1], localAxis[2]));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return sectionCurve;
        }
    }
}
