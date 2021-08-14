using System;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component_v2.Utils.Geometry
{
    public static class SectionCornerPoints
    {
        //  Y        3 - 2
        //  ^        | o |
        //  o > X    0 - 1
        public static Point3d[] ColumnRect(Point3d pt, double width, double height)
        {
            return new[]
            {
                new Point3d(pt.X - width / 2, pt.Y - height / 2, pt.Z),
                new Point3d(pt.X + width / 2, pt.Y - height / 2, pt.Z),
                new Point3d(pt.X + width / 2, pt.Y + height / 2, pt.Z),
                new Point3d(pt.X - width / 2, pt.Y + height / 2, pt.Z),
                new Point3d(pt.X - width / 2, pt.Y - height / 2, pt.Z),
            };
        }

        //           7 - - - - 6
        //           8 - 9 4 - 5
        //  Y            |o|    
        //  ^        11-10 3 - 2
        //  o > X    0 - - - - 1
        public static Point3d[] ColumnH(Point3d pt, double height, double width, double tw, double tf)
        {
            return new[]
            {
                new Point3d(pt.X - width / 2, pt.Y - height / 2, pt.Z),
                new Point3d(pt.X + width / 2, pt.Y - height / 2, pt.Z),
                new Point3d(pt.X + width / 2, pt.Y - height / 2 + tf, pt.Z),
                new Point3d(pt.X + tw / 2, pt.Y - height / 2 + tf, pt.Z),
                new Point3d(pt.X + tw / 2, pt.Y + height / 2 - tf, pt.Z),
                new Point3d(pt.X + width / 2, pt.Y + height / 2 - tf, pt.Z),
                new Point3d(pt.X + width / 2, pt.Y + height / 2, pt.Z),
                new Point3d(pt.X - width / 2, pt.Y + height / 2, pt.Z),
                new Point3d(pt.X - width / 2, pt.Y + height / 2 - tf, pt.Z),
                new Point3d(pt.X - tw / 2, pt.Y + height / 2 - tf, pt.Z),
                new Point3d(pt.X - tw / 2, pt.Y - height / 2 + tf, pt.Z),
                new Point3d(pt.X - width / 2, pt.Y - height / 2 + tf, pt.Z),
                new Point3d(pt.X - width / 2, pt.Y - height / 2, pt.Z)
            };
        }

        public static Point3d[] ColumnL(Point3d pt, double height, double width, double tw, double tf, StbSecRollLType type)
        {
            switch (type)
            {
                case StbSecRollLType.SINGLE:
                    return ColumnLSingle(pt, height, width, tw, tf);
                case StbSecRollLType.BACKTOBACK:
                    return ColumnLBackToBack(pt, height, width, tw, tf);
                case StbSecRollLType.FACETOFACE:
                    return ColumnLFaceToFace(pt, height, width, tw, tf);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        // TODO: 重心位置をちゃんと計算する
        // 今は 0-5 間長さと 4-5 間長さ から計算していて板厚を考慮していない。
        //           5 - - 4
        //  Y        | 2 - 3
        //  ^        | | o
        //  o > X    0 1
        private static Point3d[] ColumnLSingle(Point3d pt, double height, double width, double tw, double tf)
        {
            var cpt = new Point3d(width / 2 * height / (height + width), height / 2 * width / (height + width), 0);
            return new[]
            {
                new Point3d(pt.X - cpt.X, pt.Y - (height - cpt.Y), pt.Z),
                new Point3d(pt.X - (cpt.X + tw), pt.Y - (height - cpt.Y), pt.Z),
                new Point3d(pt.X - (cpt.X + tw), pt.Y + (cpt.Y - tf), pt.Z),
                new Point3d(pt.X + (width - cpt.X), pt.Y + (cpt.Y - tf), pt.Z),
                new Point3d(pt.X + (width - cpt.X), pt.Y + cpt.Y, pt.Z),
                new Point3d(pt.X - cpt.X, pt.Y + cpt.Y, pt.Z),
                new Point3d(pt.X - cpt.X, pt.Y - (height - cpt.Y), pt.Z),
            };
        }

        //        5 - - - - - 4
        //  Y     6 - 7   2 - 3
        //  ^         | o |
        //  o > X     0 - 1
        private static Point3d[] ColumnLBackToBack(Point3d pt, double height, double width, double tw, double tf)
        {
            throw new NotImplementedException();
        }

        private static Point3d[] ColumnLFaceToFace(Point3d pt, double height, double width, double tw, double tf)
        {
            throw new NotImplementedException();
        }

        internal static Curve ColumnPipe(Point3d pt, double d)
        {
            Plane plane = new Plane(pt, Vector3d.ZAxis);
            return new ArcCurve(new Circle(plane, d / 2));
        }

        //  Z        3 o 2
        //  ^        |   |
        //  o > Y    0 - 1
        public static Point3d[] BeamRect(Point3d pt, double depth, double width)
        {
            return new[]
            {
                new Point3d(pt.X, pt.Y - width / 2, pt.Z - depth),
                new Point3d(pt.X, pt.Y + width / 2, pt.Z - depth),
                new Point3d(pt.X, pt.Y + width / 2, pt.Z),
                new Point3d(pt.X, pt.Y - width / 2, pt.Z),
                new Point3d(pt.X, pt.Y - width / 2, pt.Z - depth),
            };
        }

        //           7 - -o- - 6
        //           8 - 9 4 - 5
        //  Z            | |    
        //  ^        11-10 3 - 2
        //  o > Y    0 - - - - 1
        public static Point3d[] BeamH(Point3d pt, double height, double width, double tw, double tf)
        {
            return new[]
            {
                new Point3d(pt.X, pt.Y - width / 2, pt.Z - height),
                new Point3d(pt.X, pt.Y + width / 2, pt.Z - height),
                new Point3d(pt.X, pt.Y + width / 2, pt.Z - height + tf),
                new Point3d(pt.X, pt.Y + tw / 2, pt.Z - height + tf),
                new Point3d(pt.X, pt.Y + tw / 2, pt.Z - tf),
                new Point3d(pt.X, pt.Y + width / 2, pt.Z - tf),
                new Point3d(pt.X, pt.Y + width / 2, pt.Z),
                new Point3d(pt.X, pt.Y - width / 2, pt.Z),
                new Point3d(pt.X, pt.Y - width / 2, pt.Z - tf),
                new Point3d(pt.X, pt.Y - tw / 2, pt.Z - tf),
                new Point3d(pt.X, pt.Y - tw / 2, pt.Z - height + tf),
                new Point3d(pt.X, pt.Y - width / 2, pt.Z - height + tf),
                new Point3d(pt.X, pt.Y - width / 2, pt.Z - height)
            };
        }

        public static Point3d[] BeamL(Point3d pt, double height, double width, double tw, double tf, StbSecRollLType type)
        {
            switch (type)
            {
                case StbSecRollLType.SINGLE:
                    return BeamLSingle(pt, height, width, tw, tf);
                case StbSecRollLType.BACKTOBACK:
                    return BeamLBackToBack(pt, height, width, tw, tf);
                case StbSecRollLType.FACETOFACE:
                    return BeamLFaceToFace(pt, height, width, tw, tf);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        // TODO: 断面の原点位置はここでよい？今は4と5の中心
        //           5 - o - 4
        //  Z        | 2 - - 3
        //  ^        | |
        //  o > Y    0 1
        private static Point3d[] BeamLSingle(Point3d pt, double height, double width, double tw, double tf)
        {
            return new[]
            {
                new Point3d(pt.X, pt.Y - width/2, pt.Z - height),
                new Point3d(pt.X, pt.Y - width/2 + tw, pt.Z - height),
                new Point3d(pt.X, pt.Y - width/2 + tw, pt.Z - tf),
                new Point3d(pt.X, pt.Y + width/2, pt.Z - tf),
                new Point3d(pt.X, pt.Y + width/2, pt.Z),
                new Point3d(pt.X, pt.Y - width/2, pt.Z),
                new Point3d(pt.X, pt.Y - width/2, pt.Z - height),
            };
        }

        //        5 - - o - - 4
        //  Z     6 - 7   2 - 3
        //  ^         |   |
        //  o > Y     0 - 1
        private static Point3d[] BeamLBackToBack(Point3d pt, double height, double width, double tw, double tf)
        {
            throw new NotImplementedException();
        }

        //            7 - - o - - 6 
        //  Z         |   2 - 3   |
        //  ^         |   |   |   |
        //  o > Y     0 - 1   4 - 5
        private static Point3d[] BeamLFaceToFace(Point3d pt, double height, double width, double tw, double tf)
        {
            throw new NotImplementedException();
        }


        internal static Curve BeamPipe(Point3d pt, double d)
        {
            Point3d centerPt = new Point3d(pt.X, pt.Y, pt.Z - d / 2);
            Plane plane = new Plane(centerPt, Vector3d.XAxis);
            return new ArcCurve(new Circle(plane, d / 2));
        }
    }
}
