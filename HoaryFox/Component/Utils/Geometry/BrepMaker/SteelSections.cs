using System;
using System.Linq;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component.Utils.Geometry.BrepMaker
{
    public static class SteelSections
    {
        public static Curve GetCurve(StbSecSteel secSteel, string shape, Point3d point, Utils.SectionType type, Vector3d[] localAxis)
        {
            // TODO: foreach なのに最初にマッチしたもので return しているのでが変なので直す。
            if (secSteel.StbSecBuildBOX != null)
            {
                foreach (var box in secSteel.StbSecBuildBOX.Where(box => box.name == shape))
                {
                    return CurveFromStbSecBox(localAxis, point, type, box.A, box.B);
                }
            }

            if (secSteel.StbSecRollBOX != null)
            {
                foreach (StbSecRollBOX box in secSteel.StbSecRollBOX.Where(box => box.name == shape))
                {
                    return CurveFromStbSecBox(localAxis, point, type, box.A, box.B);
                }
            }

            if (secSteel.StbSecFlatBar != null)
            {
                foreach (StbSecFlatBar flatBar in secSteel.StbSecFlatBar.Where(bar => bar.name == shape))
                {
                    return CurveFromStbSecBox(localAxis, point, type, flatBar.B, flatBar.t);
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
                    return CurveFromStbSecPipe(localAxis, point, type, pipe.D);
                }
            }

            if (secSteel.StbSecRoundBar != null)
            {
                foreach (StbSecRoundBar bar in secSteel.StbSecRoundBar.Where(pipe => pipe.name == shape))
                {
                    return CurveFromStbSecPipe(localAxis, point, type, bar.R);
                }
            }

            // TODO: C 断面を実装する
            if (secSteel.StbSecRollC != null || secSteel.StbSecLipC != null)
            {
                throw new ArgumentException("StbSecRollC & StbSecLipC is not supported");
            }

            throw new ArgumentException("There are no matching steel section");
        }

        private static Curve CurveFromStbSecPipe(Vector3d[] localAxis, Point3d point, Utils.SectionType type, double diameter)
        {
            switch (type)
            {
                case Utils.SectionType.Column:
                case Utils.SectionType.Brace:
                    return SectionCornerPoints.ColumnPipe(point, diameter, localAxis[0]);
                case Utils.SectionType.Beam:
                    return SectionCornerPoints.BeamPipe(point, diameter, localAxis[0]);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static Curve CurveFromStbSecL(Vector3d[] localAxis, Point3d point, Utils.SectionType type, StbSecRollL rollL)
        {
            switch (type)
            {
                case Utils.SectionType.Column:
                case Utils.SectionType.Brace:
                    return new PolylineCurve(
                        SectionCornerPoints.ColumnL(point, rollL.A, rollL.B, rollL.t1, rollL.t2, rollL.type, localAxis[1], localAxis[2]));
                case Utils.SectionType.Beam:
                    return new PolylineCurve(
                        SectionCornerPoints.BeamL(point, rollL.A, rollL.B, rollL.t1, rollL.t2, rollL.type, localAxis[1], localAxis[2]));
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static Curve CurveFromStbSecBox(Vector3d[] localAxis, Point3d point, Utils.SectionType type, double A, double B)
        {
            switch (type)
            {
                case Utils.SectionType.Column:
                case Utils.SectionType.Brace:
                    return new PolylineCurve(
                        SectionCornerPoints.ColumnRect(point, B, A, localAxis[1], localAxis[2]));
                case Utils.SectionType.Beam:
                    return new PolylineCurve(
                        SectionCornerPoints.BeamRect(point, B, A, localAxis[1], localAxis[2]));
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static Curve CurveFromStbSecH(Vector3d[] localAxis, Point3d point, Utils.SectionType type, double A, double B, double t1, double t2)
        {
            switch (type)
            {
                case Utils.SectionType.Column:
                case Utils.SectionType.Brace:
                    return new PolylineCurve(
                        SectionCornerPoints.ColumnH(point, A, B, t1, t2, localAxis[1], localAxis[2]));
                case Utils.SectionType.Beam:
                    return new PolylineCurve(
                        SectionCornerPoints.BeamH(point, A, B, t1, t2, localAxis[1], localAxis[2]));
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
