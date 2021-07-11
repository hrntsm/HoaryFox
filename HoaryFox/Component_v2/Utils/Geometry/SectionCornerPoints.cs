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

        public static List<Point3d> Girder(Point3d pt, double width, double height, double angle)
        {
            var points = new List<Point3d>
            {
                new Point3d(pt.X + width / 2 * Math.Sin(angle), pt.Y + width / 2 * Math.Cos(angle), pt.Z - height),
                new Point3d(pt.X - width / 2 * Math.Sin(angle), pt.Y - width / 2 * Math.Cos(angle), pt.Z - height),
                new Point3d(pt.X + width / 2 * Math.Sin(angle), pt.Y + width / 2 * Math.Cos(angle), pt.Z),
                new Point3d(pt.X - width / 2 * Math.Sin(angle), pt.Y - width / 2 * Math.Cos(angle), pt.Z)
            };

            return points;
        }

        public static List<Point3d> Brace(Point3d pt, double width, double angle)
        {
            var points = new List<Point3d>
            {
                new Point3d(pt.X + width / 2 * Math.Sin(angle), pt.Y + width / 2 * Math.Cos(angle), pt.Z - width / 2),
                new Point3d(pt.X - width / 2 * Math.Sin(angle), pt.Y - width / 2 * Math.Cos(angle), pt.Z - width / 2),
                new Point3d(pt.X + width / 2 * Math.Sin(angle), pt.Y + width / 2 * Math.Cos(angle), pt.Z + width / 2),
                new Point3d(pt.X - width / 2 * Math.Sin(angle), pt.Y - width / 2 * Math.Cos(angle), pt.Z + width / 2)
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
    }
}
