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

        public static Point3d[] ColumnL(Point3d pt, double height, double width, double tw, double tf, StbSecRollLType type, Vector3d xAxis, Vector3d yAxis)
        {
            switch (type)
            {
                case StbSecRollLType.SINGLE:
                    return ColumnLSingle(pt, height, width, tw, tf, xAxis, yAxis);
                case StbSecRollLType.BACKTOBACK:
                    return ColumnLBackToBack(pt, height, width, tw, tf, xAxis, yAxis);
                case StbSecRollLType.FACETOFACE:
                    return ColumnLFaceToFace(pt, height, width, tw, tf, xAxis, yAxis);
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
        private static Point3d[] ColumnLSingle(Point3d pt, double height, double width, double tw, double tf, Vector3d xAxis, Vector3d yAxis)
        {
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
        }

        //        5 - - - - - 4
        //  Y     6 - 7   2 - 3
        //  ^         | o |
        //  o > X     0 - 1
        private static Point3d[] ColumnLBackToBack(Point3d pt, double height, double width, double tw, double tf, Vector3d xAxis, Vector3d yAxis)
        {
            throw new NotImplementedException();
        }

        private static Point3d[] ColumnLFaceToFace(Point3d pt, double height, double width, double tw, double tf, Vector3d xAxis, Vector3d yAxis)
        {
            throw new NotImplementedException();
        }

        internal static Curve ColumnPipe(Point3d pt, double d, Vector3d zAxis)
        {
            Plane plane = new Plane(pt, zAxis);
            return new ArcCurve(new Circle(plane, d / 2));
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

        public static Point3d[] BeamL(Point3d pt, double height, double width, double tw, double tf, StbSecRollLType type, Vector3d yAxis, Vector3d zAxis)
        {
            switch (type)
            {
                case StbSecRollLType.SINGLE:
                    return BeamLSingle(pt, height, width, tw, tf, yAxis, zAxis);
                case StbSecRollLType.BACKTOBACK:
                    return BeamLBackToBack(pt, height, width, tw, tf, yAxis, zAxis);
                case StbSecRollLType.FACETOFACE:
                    return BeamLFaceToFace(pt, height, width, tw, tf, yAxis, zAxis);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        // TODO: 断面の原点位置はここでよい？今は4と5の中心
        //           5 - o - 4
        //  Z        | 2 - - 3
        //  ^        | |
        //  o > Y    0 1
        private static Point3d[] BeamLSingle(Point3d pt, double height, double width, double tw, double tf, Vector3d yAxis, Vector3d zAxis)
        {
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
        }

        //        5 - - o - - 4
        //  Z     6 - 7   2 - 3
        //  ^         |   |
        //  o > Y     0 - 1
        private static Point3d[] BeamLBackToBack(Point3d pt, double height, double width, double tw, double tf, Vector3d yAxis, Vector3d zAxis)
        {
            throw new NotImplementedException();
        }

        //            7 - - o - - 6 
        //  Z         |   2 - 3   |
        //  ^         |   |   |   |
        //  o > Y     0 - 1   4 - 5
        private static Point3d[] BeamLFaceToFace(Point3d pt, double height, double width, double tw, double tf, Vector3d yAxis, Vector3d zAxis)
        {
            throw new NotImplementedException();
        }


        internal static Curve BeamPipe(Point3d pt, double d, Vector3d xAxis)
        {
            var centerPt = new Point3d(pt.X, pt.Y, pt.Z - d / 2);
            var plane = new Plane(centerPt, xAxis);
            return new ArcCurve(new Circle(plane, d / 2));
        }
    }
}
