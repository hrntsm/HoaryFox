using System;
using System.Collections.Generic;
using Rhino.Geometry;
using StbHopper.Component;
using StbHopper.STB;

namespace StbHopper.Util
{
    public class CreateBrep
    {
        private readonly StbNodes _nodes;

        public CreateBrep(StbNodes nodes)
        {
            this._nodes = nodes;
        }

        public List<Brep> Slab(StbSlabs slabs)
        {
            List<Brep> brep = new List<Brep>();
            int slabNum = 0;

            foreach (List<int> nodeIds in slabs.NodeIdList)
            {
                int[] index = new int[4];
                Point3d[] pt = new Point3d[4];
                double offset = slabs.Level[slabNum];

                for (int i = 0; i < nodeIds.Count; i++)
                {
                    index[i] = _nodes.Id.IndexOf(nodeIds[i]);
                    pt[i] = new Point3d(_nodes.X[index[i]], _nodes.Y[index[i]], _nodes.Z[index[i]] + offset);
                }

                brep.Add(nodeIds.Count == 4
                    ? Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], pt[3], 1)
                    : Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], 1));
                slabNum++;
            }

            return brep;
        }

        public List<Brep> Wall(StbWalls walls)
        {
            List<Brep> brep = new List<Brep>();

            foreach (List<int> nodeIds in walls.NodeIdList)
            {
                int[] index = new int[4];
                Point3d[] pt = new Point3d[4];

                for (int i = 0; i < nodeIds.Count; i++)
                {
                    index[i] = _nodes.Id.IndexOf(nodeIds[i]);
                    pt[i] = new Point3d(_nodes.X[index[i]], _nodes.Y[index[i]], _nodes.Z[index[i]]);
                }

                brep.Add(nodeIds.Count == 4
                    ? Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], pt[3], 1)
                    : Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], 1));
            }

            return brep;
        }

        public List<Brep> Frame(StbFrame frame)
        {
            List<Brep> brep = new List<Brep>();

            double height = -1;
            double width = -1;
            string shape = string.Empty;
            ShapeTypes shapeType = ShapeTypes.H;

            for (int eNum = 0; eNum < frame.Id.Count; eNum++)
            {
                int idSection = frame.IdSection[eNum];
                KindsStructure kind = frame.KindStructure[eNum];

                // 始点と終点の座標取得
                int nodeIndexStart = _nodes.Id.IndexOf(frame.IdNodeStart[eNum]);
                int nodeIndexEnd = _nodes.Id.IndexOf(frame.IdNodeEnd[eNum]);
                Point3d nodeStart = new Point3d(_nodes.X[nodeIndexStart], _nodes.Y[nodeIndexStart],
                    _nodes.Z[nodeIndexStart]);
                Point3d nodeEnd = new Point3d(_nodes.X[nodeIndexEnd], _nodes.Y[nodeIndexEnd], _nodes.Z[nodeIndexEnd]);

                int secIndex;
                if (kind == KindsStructure.RC)
                {
                    switch (frame.FrameType)
                    {
                        case FrameType.Column:
                        case FrameType.Post:
                            secIndex = Stb2Brep.SecColumnRc.Id.IndexOf(idSection);
                            height = Stb2Brep.SecColumnRc.Height[secIndex];
                            width = Stb2Brep.SecColumnRc.Width[secIndex];
                            break;
                        case FrameType.Girder:
                        case FrameType.Beam:
                            secIndex = Stb2Brep.SecBeamRc.Id.IndexOf(idSection);
                            height = Stb2Brep.SecBeamRc.Depth[secIndex];
                            width = Stb2Brep.SecBeamRc.Width[secIndex];
                            break;
                    }

                    shapeType = height <= 0 ? ShapeTypes.Pipe : ShapeTypes.BOX;
                }
                else if (kind == KindsStructure.S)
                {
                    int idShape;
                    switch (frame.FrameType)
                    {
                        case FrameType.Column:
                        case FrameType.Post:
                            idShape = Stb2Brep.SecColumnS.Id.IndexOf(idSection);
                            shape = Stb2Brep.SecColumnS.Shape[idShape];
                            break;
                        case FrameType.Girder:
                        case FrameType.Beam:
                            idShape = Stb2Brep.SecBeamS.Id.IndexOf(idSection);
                            shape = Stb2Brep.SecBeamS.Shape[idShape];
                            break;
                        case FrameType.Brace:
                            idShape = Stb2Brep.SecBraceS.Id.IndexOf(idSection);
                            shape = Stb2Brep.SecBraceS.Shape[idShape];
                            break;
                    }

                    secIndex = Stb2Brep.StbSecSteel.Name.IndexOf(shape);
                    height = Stb2Brep.StbSecSteel.P1[secIndex];
                    width = Stb2Brep.StbSecSteel.P2[secIndex];
                    shapeType = Stb2Brep.StbSecSteel.ShapeType[secIndex];
                }

                brep.AddRange(Point2Brep(nodeStart, nodeEnd, height, width, shapeType, frame.FrameType));
            }

            return brep;
        }


        private List<Brep> Point2Brep(Point3d nodeStart, Point3d nodeEnd, double height, double width, ShapeTypes shapeType, FrameType frameType)
        {
            Point3d[] pointStart = new Point3d[6];
            Point3d[] pointEnd = new Point3d[6];
            List<Brep> brep = new List<Brep>();

            double dx = nodeEnd.X - nodeStart.X;
            double dy = nodeEnd.Y - nodeStart.Y;
            double dz = nodeEnd.Z - nodeStart.Z;
            double angleY = -1 * Math.Atan2(dy, dx);
            double angleZ = -1 * Math.Atan2(dz, dx);

            // 梁は部材天端の中心が起点に対して、柱・ブレースは部材芯が起点なので場合分け
            switch (frameType)
            {
                case FrameType.Column:
                case FrameType.Post:
                    pointStart = GetColumnPoints(nodeStart, width, height, angleZ);
                    pointEnd = GetColumnPoints(nodeEnd, width, height, angleZ);
                    break;
                case FrameType.Girder:
                case FrameType.Beam:
                    pointStart = GetGirderPoints(nodeStart, width, height, angleY);
                    pointEnd = GetGirderPoints(nodeEnd, width, height, angleY);
                    break;
                case FrameType.Brace:
                    pointStart = GetBracePoints(nodeStart, width, angleY);
                    pointEnd = GetBracePoints(nodeEnd, width, angleY);
                    break;
            }

            switch (shapeType)
            {
                case ShapeTypes.H:
                    brep = HShapeBrep(pointStart, pointEnd);
                    break;
                case ShapeTypes.BOX:
                    brep = BoxShapeBrep(pointStart, pointEnd);
                    break;
                case ShapeTypes.Pipe:
                    brep = PipeShapeBrep(nodeStart, nodeEnd, width);
                    break;
                case ShapeTypes.L:
                    brep = LShapeBrep(pointStart, pointEnd);
                    break;
            }

            return brep;
        }

        private static List<Brep> HShapeBrep(Point3d[] pointStart, Point3d[] pointEnd)
        {
            var brep = new List<Brep>
            {
                Brep.CreateFromCornerPoints(pointStart[3], pointStart[5], pointEnd[5], pointEnd[3], Stb2Brep.LengthTolerance),
                Brep.CreateFromCornerPoints(pointStart[0], pointStart[2], pointEnd[2], pointEnd[0], Stb2Brep.LengthTolerance),
                Brep.CreateFromCornerPoints(pointStart[4], pointStart[1], pointEnd[1], pointEnd[4], Stb2Brep.LengthTolerance)
            };
            return brep;
        }

        private static List<Brep> BoxShapeBrep(Point3d[] pointStart, Point3d[] pointEnd)
        {
            var brep = new List<Brep>
            {
                Brep.CreateFromCornerPoints(pointStart[3], pointStart[5], pointEnd[5], pointEnd[3], Stb2Brep.LengthTolerance),
                Brep.CreateFromCornerPoints(pointStart[0], pointStart[2], pointEnd[2], pointEnd[0], Stb2Brep.LengthTolerance),
                Brep.CreateFromCornerPoints(pointStart[3], pointStart[0], pointEnd[0], pointEnd[3], Stb2Brep.LengthTolerance),
                Brep.CreateFromCornerPoints(pointStart[5], pointStart[2], pointEnd[2], pointEnd[5], Stb2Brep.LengthTolerance)
            };
            return brep;
        }

        private static List<Brep> LShapeBrep(Point3d[] pointStart, Point3d[] pointEnd)
        {
            var brep = new List<Brep>
            {
                Brep.CreateFromCornerPoints(pointStart[0], pointStart[2], pointEnd[2], pointEnd[0], Stb2Brep.LengthTolerance),
                Brep.CreateFromCornerPoints(pointStart[5], pointStart[2], pointEnd[2], pointEnd[5], Stb2Brep.LengthTolerance)
            };
            return brep;
        }

        private static List<Brep> PipeShapeBrep(Point3d nodeStart, Point3d nodeEnd, double width)
        {
            var brep = new List<Brep>();            
            brep.AddRange(Brep.CreatePipe(new LineCurve(nodeStart, nodeEnd), width / 2, true, PipeCapMode.Flat, true, Stb2Brep.LengthTolerance, Stb2Brep.AngleTolerance));
            return brep;
        }
        
        private static Point3d[] GetColumnPoints(Point3d node, double width, double height, double angle)
        {
            //  Y        3 - 4 - 5 
            //  ^        |   |   |  
            //  o >  X   0 - 1 - 2
            Point3d[] points = new Point3d[6];

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
            return (points);
        }

        private static Point3d[] GetGirderPoints(Point3d node, double width, double height, double angle)
        {
            //  Y        3 - 4 - 5 
            //  ^        |   |   |  
            //  o >  X   0 - 1 - 2
            Point3d[] points = new Point3d[6];

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
            return (points);
        }

        private static Point3d[] GetBracePoints(Point3d node, double width, double angle)
        {
            //  Y        3 - 4 - 5 
            //  ^        |   |   |  
            //  o >  X   0 - 1 - 2
            Point3d[] points = new Point3d[6];

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
            return (points);
        }
    }
}
