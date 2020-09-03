using System;
using System.Collections.Generic;
using HoaryFox.STB;
using Rhino.Geometry;

namespace HoaryFox.Member
{
    public class CreateBrep
    {
        private readonly StbData _stbData;

        public CreateBrep(StbData stbData)
        {
            _stbData = stbData;
        }

        private List<Brep> GetPlaneBrep(List<Brep> brep, int count, Point3d[] pt)
        {
            // TODO: 10節点までとりあえず対応。節点が同一線上にあるとたぶんエラーになるので要対応
            switch (count)
            {
                case 3:
                    brep.Add(Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], 0.001));
                    break;
                case 4:
                    brep.Add(Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2],  pt[3], 0.001));
                    break;
                case 5:
                    brep.Add(Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], pt[3], 0.001));
                    brep.Add(Brep.CreateFromCornerPoints(pt[3], pt[4], pt[0], 0.001));
                    break;
                case 6:
                    brep.Add(Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], pt[3], 0.001));
                    brep.Add(Brep.CreateFromCornerPoints(pt[3], pt[4], pt[5], pt[0], 0.001));
                    break;
                case 7:
                    brep.Add(Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], pt[3], 0.001));
                    brep.Add(Brep.CreateFromCornerPoints(pt[3], pt[4], pt[5], pt[6], 0.001));
                    brep.Add(Brep.CreateFromCornerPoints(pt[6], pt[0], pt[3], 0.001));
                    break;
                case 8:
                    brep.Add(Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], pt[3], 0.001));
                    brep.Add(Brep.CreateFromCornerPoints(pt[3], pt[4], pt[5], pt[6], 0.001));
                    brep.Add(Brep.CreateFromCornerPoints(pt[6], pt[7], pt[0], pt[3], 0.001));
                    break;
                case 9:
                    brep.Add(Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], pt[3], 0.001));
                    brep.Add(Brep.CreateFromCornerPoints(pt[3], pt[4], pt[5], pt[6], 0.001));
                    brep.Add(Brep.CreateFromCornerPoints(pt[6], pt[7], pt[8], pt[0], 0.001));
                    brep.Add(Brep.CreateFromCornerPoints(pt[0], pt[3], pt[6], 0.001));
                    break;
                case 10:
                    brep.Add(Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], pt[3], 0.001));
                    brep.Add(Brep.CreateFromCornerPoints(pt[3], pt[4], pt[5], pt[6], 0.001));
                    brep.Add(Brep.CreateFromCornerPoints(pt[6], pt[7], pt[8], pt[9], 0.001));
                    brep.Add(Brep.CreateFromCornerPoints(pt[9], pt[0], pt[3], pt[6], 0.001));
                    break;
            }

            return brep;
        }

        public List<Brep> Slab(StbSlabs slabs)
        {
            List<Brep> brep = new List<Brep>();
            int slabNum = 0;

            foreach (List<int> nodeIds in slabs.NodeIdList)
            {
                int[] index = new int[10];
                Point3d[] pt = new Point3d[10];
                double offset = slabs.Level[slabNum];

                for (int i = 0; i < nodeIds.Count; i++)
                {
                    if (i > 9) continue;
                    index[i] = _stbData.Nodes.Id.IndexOf(nodeIds[i]);
                    pt[i] = new Point3d(_stbData.Nodes.X[index[i]], _stbData.Nodes.Y[index[i]], _stbData.Nodes.Z[index[i]] + offset);
                }
                
                brep = GetPlaneBrep(brep, nodeIds.Count, pt);

                slabNum++;
            }

            return brep;
        }

        public List<Brep> Wall(StbWalls walls)
        {
            List<Brep> brep = new List<Brep>();

            foreach (List<int> nodeIds in walls.NodeIdList)
            {
                int[] index = new int[10];
                Point3d[] pt = new Point3d[10];

                for (int i = 0; i < nodeIds.Count; i++)
                {
                    if (i > 9) continue;
                    index[i] = _stbData.Nodes.Id.IndexOf(nodeIds[i]);
                    pt[i] = new Point3d(_stbData.Nodes.X[index[i]], _stbData.Nodes.Y[index[i]], _stbData.Nodes.Z[index[i]]);
                }
                
                brep = GetPlaneBrep(brep, nodeIds.Count, pt);
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
                int nodeIndexStart = _stbData.Nodes.Id.IndexOf(frame.IdNodeStart[eNum]);
                int nodeIndexEnd = _stbData.Nodes.Id.IndexOf(frame.IdNodeEnd[eNum]);
                Point3d nodeStart = new Point3d(_stbData.Nodes.X[nodeIndexStart], _stbData.Nodes.Y[nodeIndexStart], _stbData.Nodes.Z[nodeIndexStart]);
                Point3d nodeEnd = new Point3d(_stbData.Nodes.X[nodeIndexEnd], _stbData.Nodes.Y[nodeIndexEnd], _stbData.Nodes.Z[nodeIndexEnd]);

                int secIndex;
                if (kind == KindsStructure.RC)
                {
                    switch (frame.FrameType)
                    {
                        case FrameType.Column:
                        case FrameType.Post:
                            secIndex = _stbData.SecColumnRc.Id.IndexOf(idSection);
                            height = _stbData.SecColumnRc.Height[secIndex];
                            width = _stbData.SecColumnRc.Width[secIndex];
                            break;
                        case FrameType.Girder:
                        case FrameType.Beam:
                            secIndex = _stbData.SecBeamRc.Id.IndexOf(idSection);
                            height = _stbData.SecBeamRc.Depth[secIndex];
                            width = _stbData.SecBeamRc.Width[secIndex];
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
                            idShape = _stbData.SecColumnS.Id.IndexOf(idSection);
                            shape = _stbData.SecColumnS.Shape[idShape];
                            break;
                        case FrameType.Girder:
                        case FrameType.Beam:
                            idShape = _stbData.SecBeamS.Id.IndexOf(idSection);
                            shape = _stbData.SecBeamS.Shape[idShape];
                            break;
                        case FrameType.Brace:
                            idShape = _stbData.SecBraceS.Id.IndexOf(idSection);
                            shape = _stbData.SecBraceS.Shape[idShape];
                            break;
                    }

                    secIndex = _stbData.SecSteel.Name.IndexOf(shape);
                    height = _stbData.SecSteel.P1[secIndex];
                    width = _stbData.SecSteel.P2[secIndex];
                    shapeType = _stbData.SecSteel.ShapeType[secIndex];
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

        private List<Brep> HShapeBrep(Point3d[] pointStart, Point3d[] pointEnd)
        {
            var brep = new List<Brep>
            {
                Brep.CreateFromCornerPoints(pointStart[3], pointStart[5], pointEnd[5], pointEnd[3], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(pointStart[0], pointStart[2], pointEnd[2], pointEnd[0], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(pointStart[4], pointStart[1], pointEnd[1], pointEnd[4], _stbData.ToleLength)
            };
            return brep;
        }

        private List<Brep> BoxShapeBrep(Point3d[] pointStart, Point3d[] pointEnd)
        {
            var brep = new List<Brep>
            {
                Brep.CreateFromCornerPoints(pointStart[3], pointStart[5], pointEnd[5], pointEnd[3], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(pointStart[0], pointStart[2], pointEnd[2], pointEnd[0], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(pointStart[3], pointStart[0], pointEnd[0], pointEnd[3], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(pointStart[5], pointStart[2], pointEnd[2], pointEnd[5], _stbData.ToleLength)
            };
            return brep;
        }

        private List<Brep> LShapeBrep(Point3d[] pointStart, Point3d[] pointEnd)
        {
            var brep = new List<Brep>
            {
                Brep.CreateFromCornerPoints(pointStart[0], pointStart[2], pointEnd[2], pointEnd[0], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(pointStart[5], pointStart[2], pointEnd[2], pointEnd[5], _stbData.ToleLength)
            };
            return brep;
        }

        private List<Brep> PipeShapeBrep(Point3d nodeStart, Point3d nodeEnd, double width)
        {
            var brep = new List<Brep>();            
            brep.AddRange(Brep.CreatePipe(new LineCurve(nodeStart, nodeEnd), width / 2, true, PipeCapMode.Flat, true, _stbData.ToleLength, _stbData.ToleAngle));
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
