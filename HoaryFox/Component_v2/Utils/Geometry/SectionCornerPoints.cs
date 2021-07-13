using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace HoaryFox.Component_v2.Utils.Geometry
{
    public static class SectionCornerPoints
    {
        //  Y        3 - 2
        //  ^        |   |
        //  o > X    0 - 1
        public static Point3d[] ColumnRect(Point3d pt, double width, double height)
        {
            var points = new[]
            {
                new Point3d(pt.X - width / 2, pt.Y - height / 2, pt.Z),
                new Point3d(pt.X + width / 2, pt.Y - height / 2, pt.Z),
                new Point3d(pt.X + width / 2, pt.Y + height / 2, pt.Z),
                new Point3d(pt.X - width / 2, pt.Y + height / 2, pt.Z),
                new Point3d(pt.X - width / 2, pt.Y - height / 2, pt.Z),
            };

            return points;
        }

        //           7 - - - - 6
        //           8 - 9 4 - 5
        //  Y            | |    
        //  ^        11-10 3 - 2
        //  o > X    0 - - - - 1
        public static Point3d[] ColumnH(Point3d pt, double height, double width, double tw, double tf)
        {
            var points = new[]
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

            return points;
        }

        //  Z        3 - 2
        //  ^        |   |
        //  o > Y    0 - 1
        public static Point3d[] BeamRect(Point3d pt, double depth, double width)
        {
            var points = new[]
            {
                new Point3d(pt.X, pt.Y - width / 2, pt.Z - depth),
                new Point3d(pt.X, pt.Y + width / 2, pt.Z - depth),
                new Point3d(pt.X, pt.Y + width / 2, pt.Z),
                new Point3d(pt.X, pt.Y - width / 2, pt.Z),
                new Point3d(pt.X, pt.Y - width / 2, pt.Z - depth),
            };

            return points;
        }

        //           7 - - - - 6
        //           8 - 9 4 - 5
        //  Z            | |    
        //  ^        11-10 3 - 2
        //  o > Y    0 - - - - 1
        public static Point3d[] BeamH(Point3d pt, double height, double width, double tw, double tf)
        {
            var points = new[]
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

            return points;
        }

    }
}
