using System;
using Rhino.Geometry;

namespace HoaryFox.Member
{
    public static class FramePoints
    {
        
        
        public static Point3d[] Column(Point3d node, double width, double height, double angle)
        {
            //  Y        3 - 4 - 5 
            //  ^        |   |   |  
            //  o >  X   0 - 1 - 2
            var points = new Point3d[6];

            points[0] = new Point3d(node.X - width / 2 * Math.Sin(angle),
                                    node.Y - height / 2,
                                    node.Z - width / 2 * Math.Cos(angle)
            );
            points[1] = new Point3d(node.X,
                                    node.Y + height / 2,
                                    node.Z
            );
            points[2] = new Point3d(node.X + width / 2 * Math.Sin(angle),
                                    node.Y - height / 2,
                                    node.Z + width / 2 * Math.Cos(angle)
            );
            points[3] = new Point3d(node.X - width / 2 * Math.Sin(angle),
                                    node.Y + height / 2,
                                    node.Z - width / 2 * Math.Cos(angle)
            );
            points[4] =  new Point3d(node.X,
                                     node.Y - height / 2,
                                     node.Z
            );
            points[5] = new Point3d(node.X + width / 2 * Math.Sin(angle),
                                    node.Y + height / 2,
                                    node.Z + width / 2 * Math.Cos(angle)
            );
            return points;
        }

        public static Point3d[] Girder(Point3d node, double width, double height, double angle)
        {
            //  Y        3 - 4 - 5 
            //  ^        |   |   |  
            //  o >  X   0 - 1 - 2
            var points = new Point3d[6];

            points[0] = new Point3d(node.X + width / 2 * Math.Sin(angle),
                                    node.Y + width / 2 * Math.Cos(angle),
                                    node.Z - height
            );
            points[1] = new Point3d(node.X,
                                    node.Y,
                                    node.Z - height
            );
            points[2] = new Point3d(node.X - width / 2 * Math.Sin(angle),
                                    node.Y - width / 2 * Math.Cos(angle),
                                    node.Z - height
            );
            points[3] = new Point3d(node.X + width / 2 * Math.Sin(angle),
                                    node.Y + width / 2 * Math.Cos(angle),
                                    node.Z
            );
            points[4] = new Point3d(node.X,
                                    node.Y,
                                    node.Z
            );
            points[5] = new Point3d(node.X - width / 2 * Math.Sin(angle),
                                    node.Y - width / 2 * Math.Cos(angle),
                                    node.Z
            );
            return points;
        }

        public static Point3d[] Brace(Point3d node, double width, double angle)
        {
            //  Y        3 - 4 - 5 
            //  ^        |   |   |  
            //  o >  X   0 - 1 - 2
            var points = new Point3d[6];

            points[0] = new Point3d(node.X + width / 2 * Math.Sin(angle),
                                    node.Y + width / 2 * Math.Cos(angle),
                                    node.Z - width / 2
            );
            points[1] = new Point3d(node.X,
                                    node.Y,
                                    node.Z - width / 2
            );
            points[2] = new Point3d(node.X - width / 2 * Math.Sin(angle),
                                    node.Y - width / 2 * Math.Cos(angle),
                                    node.Z - width / 2
            );
            points[3] = new Point3d(node.X + width / 2 * Math.Sin(angle),
                                    node.Y + width / 2 * Math.Cos(angle),
                                    node.Z + width / 2
            );
            points[4] = new Point3d(node.X,
                                    node.Y,
                                    node.Z + width / 2
            );
            points[5] = new Point3d(node.X - width / 2 * Math.Sin(angle),
                                    node.Y - width / 2 * Math.Cos(angle),
                                    node.Z + width / 2
            );
            return points;
        }
    }
}