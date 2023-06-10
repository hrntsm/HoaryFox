using System;

using Rhino.Geometry;

using STBDotNet.v202;

namespace HoaryFox.Component.Utils.Geometry
{
    public static class SectionCornerPoints
    {
        //  Y        3 - 2
        //  ^        | o |
        //  o > X    0 - 1
        public static Point3d[] ColumnRect(Point3d pt, double width, double height, Vector3d xAxis, Vector3d yAxis)
        {
            return new[]
            {
                pt - xAxis * width / 2 - yAxis * height / 2,
                pt + xAxis * width / 2 - yAxis * height / 2,
                pt + xAxis * width / 2 + yAxis * height / 2,
                pt - xAxis * width / 2 + yAxis * height / 2,
                pt - xAxis * width / 2 - yAxis * height / 2,
            };
        }

        //           7 - - - - 6
        //           8 - 9 4 - 5
        //  Y            |o|    
        //  ^        11-10 3 - 2
        //  o > X    0 - - - - 1
        public static Point3d[] ColumnH(Point3d pt, double height, double width, double tw, double tf, Vector3d xAxis, Vector3d yAxis)
        {
            return new[]
            {
                pt - xAxis * width / 2 - yAxis *  height / 2,
                pt + xAxis * width / 2 - yAxis *  height / 2,
                pt + xAxis * width / 2 - yAxis * (height / 2 - tf),
                pt + xAxis * tw / 2    - yAxis * (height / 2 - tf),
                pt + xAxis * tw / 2    + yAxis * (height / 2 - tf),
                pt + xAxis * width / 2 + yAxis * (height / 2 - tf),
                pt + xAxis * width / 2 + yAxis *  height / 2,
                pt - xAxis * width / 2 + yAxis *  height / 2,
                pt - xAxis * width / 2 + yAxis * (height / 2 - tf),
                pt - xAxis * tw / 2    + yAxis * (height / 2 - tf),
                pt - xAxis * tw / 2    - yAxis * (height / 2 - tf),
                pt - xAxis * width / 2 - yAxis * (height / 2 - tf),
                pt - xAxis * width / 2 - yAxis *  height / 2,
            };
        }

        //        5 - - - - - 4
        //  Y     6 - 7   2 - 3
        //  ^         | o |
        //  o > X     0 - 1
        public static Point3d[] ColumnT(Point3d pt, double height, double width, double tw, double tf, Vector3d xAxis, Vector3d yAxis)
        {
            return new[]
            {
                pt - xAxis * tw              - yAxis *  height / 2,
                pt + xAxis * tw              - yAxis *  height / 2,
                pt + xAxis * tw              + yAxis * (height / 2 - tf),
                pt + xAxis * width           + yAxis * (height / 2 - tf),
                pt + xAxis * width           + yAxis *  height / 2,
                pt - xAxis * width           + yAxis *  height / 2,
                pt - xAxis * width           + yAxis * (height / 2 - tf),
                pt - xAxis * tw              + yAxis * (height / 2 - tf),
            };
        }

        // TODO: 重心位置をちゃんと計算する
        // 今は バウンディングボックスの図心 = STBの節点位置 になっている。
        //           7 - - 6
        //           | 4 - 5
        //  Y        | | o
        //  ^        | 3 - 2
        //  o > X    0 - - 1
        public static Point3d[] ColumnC(Point3d pt, double height, double width, double tw, double tf, StbSecRollCType type, Vector3d xAxis, Vector3d yAxis)
        {
            switch (type)
            {
                case StbSecRollCType.SINGLE:
                    return new[]
                    {
                        pt - xAxis *  width / 2       - yAxis *  height / 2,
                        pt + xAxis *  width / 2       - yAxis *  height / 2,
                        pt + xAxis *  width / 2       - yAxis * (height / 2 - tf),
                        pt - xAxis * (width / 2 - tw) - yAxis * (height / 2 - tf),
                        pt - xAxis * (width / 2 - tw) + yAxis * (height / 2 - tf),
                        pt + xAxis *  width / 2       + yAxis * (height / 2 - tf),
                        pt + xAxis *  width / 2       + yAxis *  height / 2,
                        pt - xAxis *  width / 2       + yAxis *  height / 2,
                    };
                case StbSecRollCType.BACKTOBACK:
                    return ColumnH(pt, height, width, 2 * tw, tf, xAxis, yAxis);
                case StbSecRollCType.FACETOFACE:
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        // TODO: 重心位置をちゃんと計算する
        // 今は バウンディングボックスの図心 = STBの節点位置 になっている。
        //           5 - - 4
        //  Y        | 2 - 3
        //  ^        | | o
        //  o > X    0 1
        public static Point3d[] ColumnL(Point3d pt, double height, double width, double tw, double tf, StbSecRollLType type, Vector3d xAxis, Vector3d yAxis)
        {
            switch (type)
            {
                case StbSecRollLType.SINGLE:
                    return new[]
                    {
                        pt - xAxis *  width / 2       - yAxis *  height / 2,
                        pt - xAxis * (width / 2 - tw) - yAxis *  height / 2,
                        pt - xAxis * (width / 2 - tw) + yAxis * (height / 2 - tf),
                        pt + xAxis *  width / 2       + yAxis * (height / 2 - tf),
                        pt + xAxis *  width / 2       + yAxis *  height / 2,
                        pt - xAxis *  width / 2       + yAxis *  height / 2,
                        pt - xAxis *  width / 2       - yAxis *  height / 2,
                    };
                case StbSecRollLType.BACKTOBACK:
                    return ColumnT(pt, height, 2 * width, 2 * tw, tf, xAxis, yAxis);
                case StbSecRollLType.FACETOFACE:
                    return ColumnC(pt, 2 * width, height, tf, tw, StbSecRollCType.SINGLE, -yAxis, xAxis);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        internal static Curve ColumnPipe(Point3d pt, double radius, Vector3d zAxis)
        {
            Plane plane = new Plane(pt, zAxis);
            return new ArcCurve(new Circle(plane, radius));
        }

        //  Z        3 o 2
        //  ^        |   |
        //  o > Y    0 - 1
        public static Point3d[] BeamRect(Point3d pt, double depth, double width, Vector3d yAxis, Vector3d zAxis)
        {
            return new[]
            {
                pt - yAxis * width / 2 - zAxis * depth,
                pt + yAxis * width / 2 - zAxis * depth,
                pt + yAxis * width / 2,
                pt - yAxis * width / 2,
                pt - yAxis * width / 2 - zAxis * depth,
            };
        }

        //           7 - -o- - 6
        //           8 - 9 4 - 5
        //  Z            | |    
        //  ^        11-10 3 - 2
        //  o > Y    0 - - - - 1
        public static Point3d[] BeamH(Point3d pt, double height, double width, double tw, double tf, Vector3d yAxis, Vector3d zAxis)
        {
            return new[]
            {
                pt - yAxis * width / 2 - zAxis *  height,
                pt + yAxis * width / 2 - zAxis *  height,
                pt + yAxis * width / 2 - zAxis * (height - tf),
                pt + yAxis * tw / 2    - zAxis * (height - tf),
                pt + yAxis * tw / 2    - zAxis *  tf,
                pt + yAxis * width / 2 - zAxis *  tf,
                pt + yAxis * width / 2,
                pt - yAxis * width / 2,
                pt - yAxis * width / 2 - zAxis *  tf,
                pt - yAxis * tw / 2    - zAxis *  tf,
                pt - yAxis * tw / 2    - zAxis * (height - tf),
                pt - yAxis * width / 2 - zAxis * (height - tf),
                pt - yAxis * width / 2 - zAxis *  height,
            };
        }

        //        5 - - o - - 4
        //  Y     6 - 7   2 - 3
        //  ^         |   |
        //  o > X     0 - 1
        public static Point3d[] BeamT(Point3d pt, double height, double width, double tw, double tf, Vector3d xAxis, Vector3d yAxis)
        {
            return new[]
            {
                pt - xAxis * tw              - yAxis *  height,
                pt + xAxis * tw              - yAxis *  height,
                pt + xAxis * tw              + yAxis * (height - tf),
                pt + xAxis * width           + yAxis * (height - tf),
                pt + xAxis * width           + yAxis *  height,
                pt - xAxis * width           + yAxis *  height,
                pt - xAxis * width           + yAxis * (height - tf),
                pt - xAxis * tw              + yAxis * (height - tf),
            };
        }

        // TODO: 原点はここでよい？
        //           7 - o - 6
        //           | 4 - - 5
        //  Z        | |
        //  ^        | 3 - - 2
        //  o > Y    0 - - - 1
        public static Point3d[] BeamC(Point3d pt, double height, double width, double tw, double tf, StbSecRollCType type, Vector3d yAxis, Vector3d zAxis)
        {
            switch (type)
            {
                case StbSecRollCType.SINGLE:
                    return new[]
                    {
                        pt - yAxis *  width / 2       - zAxis *  height,
                        pt + yAxis *  width / 2       - zAxis *  height,
                        pt + yAxis *  width / 2       - zAxis * (height - tf),
                        pt - yAxis * (width / 2 - tw) - zAxis * (height - tf),
                        pt - yAxis * (width / 2 - tw) + zAxis * (height - tf),
                        pt + yAxis *  width / 2       + zAxis * (height - tf),
                        pt + yAxis *  width / 2,
                        pt - yAxis *  width / 2,
                    };
                case StbSecRollCType.BACKTOBACK:
                    return BeamH(pt, height, width, 2 * tw, tf, yAxis, yAxis);
                case StbSecRollCType.FACETOFACE:
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        // TODO: 断面の原点位置はここでよい？今は4と5の中心
        //           5 - o - 4
        //  Z        | 2 - - 3
        //  ^        | |
        //  o > Y    0 1
        public static Point3d[] BeamL(Point3d pt, double height, double width, double tw, double tf, StbSecRollLType type, Vector3d yAxis, Vector3d zAxis)
        {
            switch (type)
            {
                case StbSecRollLType.SINGLE:
                    return new[]
                    {
                        pt - yAxis *  width / 2       - zAxis * height,
                        pt - yAxis * (width / 2 - tw) - zAxis * height,
                        pt - yAxis * (width / 2 - tw) - zAxis * tf,
                        pt + yAxis *  width / 2       - zAxis * tf,
                        pt + yAxis *  width / 2,
                        pt - yAxis *  width / 2,
                        pt - yAxis *  width / 2       - zAxis * height
                    };
                case StbSecRollLType.BACKTOBACK:
                    return BeamT(pt, height, width, tw, tf, yAxis, zAxis);
                case StbSecRollLType.FACETOFACE:
                    return BeamC(pt, 2 * width, height, tf, tw, StbSecRollCType.SINGLE, -zAxis, yAxis);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        internal static Curve BeamPipe(Point3d pt, double radius, Vector3d xAxis)
        {
            var centerPt = new Point3d(pt.X, pt.Y, pt.Z - radius);
            var plane = new Plane(centerPt, xAxis);
            return new ArcCurve(new Circle(plane, radius));
        }
    }
}
