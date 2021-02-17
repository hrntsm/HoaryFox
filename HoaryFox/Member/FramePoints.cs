using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace HoaryFox.Member
{
    public static class FramePoints
    {
        //  Y        3 - 4 - 5 
        //  ^        |   |   |  
        //  o >  X   0 - 1 - 2
        public static List<Point3d> Column(Point3d node, double width, double height, double angle)
        {
            var points = new List<Point3d>
            {
                new Point3d(node.X - width / 2 * Math.Sin(angle), node.Y - height / 2, node.Z - width / 2 * Math.Cos(angle)),
                new Point3d(node.X, node.Y + height / 2, node.Z),
                new Point3d(node.X + width / 2 * Math.Sin(angle), node.Y - height / 2, node.Z + width / 2 * Math.Cos(angle)),
                new Point3d(node.X - width / 2 * Math.Sin(angle), node.Y + height / 2, node.Z - width / 2 * Math.Cos(angle)),
                new Point3d(node.X, node.Y - height / 2, node.Z),
                new Point3d(node.X + width / 2 * Math.Sin(angle), node.Y + height / 2, node.Z + width / 2 * Math.Cos(angle))
            };

            return points;
        }

        public static List<Point3d> Girder(Point3d node, double width, double height, double angle)
        {
            var points = new List<Point3d>
            {
                new Point3d(node.X + width / 2 * Math.Sin(angle), node.Y + width / 2 * Math.Cos(angle), node.Z - height),
                new Point3d(node.X, node.Y, node.Z - height),
                new Point3d(node.X - width / 2 * Math.Sin(angle), node.Y - width / 2 * Math.Cos(angle), node.Z - height),
                new Point3d(node.X + width / 2 * Math.Sin(angle), node.Y + width / 2 * Math.Cos(angle), node.Z),
                new Point3d(node.X, node.Y, node.Z),
                new Point3d(node.X - width / 2 * Math.Sin(angle), node.Y - width / 2 * Math.Cos(angle), node.Z)
            };

            return points;
        }

        public static List<Point3d> Brace(Point3d node, double width, double angle)
        {
            var points = new List<Point3d>
            {
                new Point3d(node.X + width / 2 * Math.Sin(angle), node.Y + width / 2 * Math.Cos(angle), node.Z - width / 2),
                new Point3d(node.X, node.Y, node.Z - width / 2),
                new Point3d(node.X - width / 2 * Math.Sin(angle), node.Y - width / 2 * Math.Cos(angle), node.Z - width / 2),
                new Point3d(node.X + width / 2 * Math.Sin(angle), node.Y + width / 2 * Math.Cos(angle), node.Z + width / 2),
                new Point3d(node.X, node.Y, node.Z + width / 2),
                new Point3d(node.X - width / 2 * Math.Sin(angle), node.Y - width / 2 * Math.Cos(angle), node.Z + width / 2)
            };

            return points;
        }
    }
}
