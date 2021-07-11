using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace HoaryFox.Component_v2.Utils.Geometry
{
    public static class SectionCornerPoints
    {
        //  Z        3 - 2
        //  ^        |   |
        //  o > Y    0 - 1
        public static Point3d[] Column(Point3d node, double width, double height)
        {
            var points = new[]
            {
                new Point3d(node.X - width / 2, node.Y - height / 2, node.Z),
                new Point3d(node.X + width / 2, node.Y - height / 2, node.Z),
                new Point3d(node.X + width / 2, node.Y + height / 2, node.Z),
                new Point3d(node.X - width / 2, node.Y + height / 2, node.Z),
                new Point3d(node.X - width / 2, node.Y - height / 2, node.Z),
            };

            return points;
        }

        public static List<Point3d> Girder(Point3d node, double width, double height, double angle)
        {
            var points = new List<Point3d>
            {
                new Point3d(node.X + width / 2 * Math.Sin(angle), node.Y + width / 2 * Math.Cos(angle), node.Z - height),
                new Point3d(node.X - width / 2 * Math.Sin(angle), node.Y - width / 2 * Math.Cos(angle), node.Z - height),
                new Point3d(node.X + width / 2 * Math.Sin(angle), node.Y + width / 2 * Math.Cos(angle), node.Z),
                new Point3d(node.X - width / 2 * Math.Sin(angle), node.Y - width / 2 * Math.Cos(angle), node.Z)
            };

            return points;
        }

        public static List<Point3d> Brace(Point3d node, double width, double angle)
        {
            var points = new List<Point3d>
            {
                new Point3d(node.X + width / 2 * Math.Sin(angle), node.Y + width / 2 * Math.Cos(angle), node.Z - width / 2),
                new Point3d(node.X - width / 2 * Math.Sin(angle), node.Y - width / 2 * Math.Cos(angle), node.Z - width / 2),
                new Point3d(node.X + width / 2 * Math.Sin(angle), node.Y + width / 2 * Math.Cos(angle), node.Z + width / 2),
                new Point3d(node.X - width / 2 * Math.Sin(angle), node.Y - width / 2 * Math.Cos(angle), node.Z + width / 2)
            };

            return points;
        }
    }
}
