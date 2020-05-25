using System;
using System.Collections.Generic;
using HoaryFox.Component;
using HoaryFox.STB;
using Rhino.Geometry;

namespace HoaryFox.Util
{
    public class CreateBar
    {
        private readonly StbNodes _nodes;

        public CreateBar(StbNodes nodes)
        {
            this._nodes = nodes;
        }

        public List<Brep> Slab(StbSlabs slabs)
        {
            List<Brep> brep = new List<Brep>();
            return brep;
        }

        public List<Brep> Wall(StbWalls walls)
        {
            List<Brep> brep = new List<Brep>();
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
                Point3d nodeStart = new Point3d(_nodes.X[nodeIndexStart], _nodes.Y[nodeIndexStart], _nodes.Z[nodeIndexStart]);
                Point3d nodeEnd = new Point3d(_nodes.X[nodeIndexEnd], _nodes.Y[nodeIndexEnd], _nodes.Z[nodeIndexEnd]);

                int secIndex = -1;
                if (kind == KindsStructure.RC)
                {
                    switch (frame.FrameType)
                    {
                        case FrameType.Column:
                        case FrameType.Post:
                            secIndex = Stb2Bar.SecColumnRc.Id.IndexOf(idSection);
                            height = Stb2Bar.SecColumnRc.Height[secIndex];
                            width = Stb2Bar.SecColumnRc.Width[secIndex];
                            break;
                        case FrameType.Girder:
                        case FrameType.Beam:
                            secIndex = Stb2Bar.SecBeamRc.Id.IndexOf(idSection);
                            height = Stb2Bar.SecBeamRc.Depth[secIndex];
                            width = Stb2Bar.SecBeamRc.Width[secIndex];
                            break;
                    }

                    shapeType = height <= 0 ? ShapeTypes.Pipe : ShapeTypes.BOX;
                }

                // 配筋の作成
                if (kind == KindsStructure.RC) {
                    if (shapeType == ShapeTypes.BOX) {
                        switch (frame.FrameType) {
                            case FrameType.Column:
                            case FrameType.Post:
                                brep.AddRange(Column(secIndex, nodeStart, nodeEnd, width, height, eNum));
                                break;
                            case FrameType.Girder:
                            case FrameType.Beam:
                                brep.AddRange(Beam(secIndex, nodeStart, nodeEnd, width, height, eNum));
                                break;
                        }
                    }
                }
            }
            return brep;
        }

        private List<Brep> Beam(int index, Point3d nodeStart, Point3d nodeEnd, double width, double height, int eNum)
        {
            var brep = new List<Brep>();
            return brep;
        }

        private List<Brep> Column(int index, Point3d nodeStart, Point3d nodeEnd, double width, double height, int eNum)
        {
            var brep = new List<Brep>();
            
            // かぶり、鉄筋径はとりあえずで設定
            const double kaburi = 50;
            const double bandD = 10;
            const double mainD = 25;
            const double bandSpace = 2 * kaburi + bandD;
            const double main1Space = bandSpace + bandD + mainD;
            double barSpace = Math.Max(1.5 * mainD, 25); // 鉄筋のあき
            double main2Space = main1Space + 2 * (mainD + barSpace);

            Point3d[,] hoopPos = GetColumnCorner(nodeStart, nodeEnd, width - bandSpace, height - bandSpace);
            Point3d[,] main1Pos = GetColumnCorner(nodeStart, nodeEnd, width - main1Space, height - main1Space);
            Point3d[,] mainX2Pos = GetColumnCorner(nodeStart, nodeEnd, width - main1Space, height - main2Space);
            Point3d[,] mainY2Pos = GetColumnCorner(nodeStart, nodeEnd, width - main2Space, height - main1Space);

            brep.AddRange(Hoop(hoopPos, bandD, index));
            brep.AddRange(ColumnMainBar(main1Pos, mainX2Pos, mainY2Pos, barSpace, mainD));

            return brep;
        }
        
        /// <summary>
        /// かぶりを考慮した部材の角のポジションを返す。
        /// [0, n]はスタート側、[1, n]はエンド側
        /// </summary>
        /// <param name="nodeStart"></param>
        /// <param name="nodeEnd"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static Point3d[,] GetColumnCorner(Point3d nodeStart, Point3d nodeEnd, double width, double height)
        {
            //  Z        4 - 3
            //  ^        | 0 |
            //  o >  X   1 - 2
            Point3d[,] cornerPoint = new Point3d[2, 5];
            Point3d node = nodeStart;
            double dx = nodeEnd.X - nodeStart.X;
            double dy = nodeEnd.Y - nodeStart.Y;
            double dz = nodeEnd.Z - nodeStart.Z;
            double angleX = -1.0 * Math.Atan2(dx, dy);
            double angleZ = -1.0 * Math.Atan2(dz, dy);

            for (int i = 0; i < 2; i++) {
                cornerPoint[i, 0] = node;
                cornerPoint[i, 1] = new Point3d(node.X - width / 2.0 * Math.Cos(angleX),
                                                node.Y - height / 2.0 * Math.Cos(angleZ),
                                                node.Z - width / 2.0 * Math.Sin(angleX) - height / 2.0 * Math.Sin(angleZ)
                                                );
                cornerPoint[i, 2] = new Point3d(node.X + width / 2.0 * Math.Cos(angleX),
                                                node.Y - height / 2.0 * Math.Cos(angleZ),
                                                node.Z + width / 2.0 * Math.Sin(angleX) + height / 2.0 * Math.Sin(angleZ)
                                                );
                cornerPoint[i, 3] = new Point3d(node.X + width / 2.0 * Math.Cos(angleX),
                                                node.Y + height / 2.0 * Math.Cos(angleZ),
                                                node.Z + width / 2.0 * Math.Sin(angleX) + height / 2.0 * Math.Sin(angleZ)
                                                );
                cornerPoint[i, 4] = new Point3d(node.X - width / 2.0 * Math.Cos(angleX),
                                                node.Y + height / 2.0 * Math.Cos(angleZ),
                                                node.Z - width / 2.0 * Math.Sin(angleX) - height / 2.0 * Math.Sin(angleZ)
                                                );
                node = nodeEnd;
            }
            return (cornerPoint);
        }

        private static List<Brep> Hoop(Point3d[,] cornerPos, double bandD, int index)
        {
            var brep = new List<Brep>();
            double pitch = Stb2Bar.SecColumnRc.BarList[index][5];
            int dirXNum = Stb2Bar.SecColumnRc.BarList[index][6];
            int dirYNum = Stb2Bar.SecColumnRc.BarList[index][7];
            int sumBar = dirXNum + dirYNum;
            Curve crv = new LineCurve(cornerPos[0, 0], cornerPos[1, 0]);
            double distance = crv.GetLength();
            List<Point3d> pts = new List<Point3d>();
            
            // 中子も含めたpointを求める。
            Point3d[,] hoopPos = GetBandPos(cornerPos, dirXNum, dirYNum);
        
            for (int i = 0; i < dirXNum + dirYNum; i++)
            {
                var rail = new LineCurve(hoopPos[0, 2*i], hoopPos[0, 2*i+1]);
                brep.AddRange(Brep.CreatePipe(rail, bandD / 2.0, true, PipeCapMode.Flat, true, Stb2Bar.LengthTolerance, Stb2Bar.AngleTolerance));
            }
            for (int i = 0; i < dirXNum + dirYNum; i++)
            {
                var rail = new LineCurve(hoopPos[1, 2*i], hoopPos[1, 2*i+1]);
                brep.AddRange(Brep.CreatePipe(rail, bandD / 2.0, true, PipeCapMode.Flat, true, Stb2Bar.LengthTolerance, Stb2Bar.AngleTolerance));
            }
            // 始点と終点の間のフープを作成
            //for (int i = 0; i < dirXNum + dirYNum; i++)
            //{
            //    Point3d[] pts1;
            //    Point3d[] pts2;
            //    var crv1 = new LineCurve(hoopPos[0, 2 * i], hoopPos[1, 2 * i]);
            //    var crv2 = new LineCurve(hoopPos[0, 2 * i + 1], hoopPos[1, 2 * i + 1]);
            //    crv1.DivideByCount((int)(distance/pitch), true, out pts1);
            //    crv2.DivideByCount((int)(distance/pitch), true, out pts2);

            //    for (int j = 0; j < (int)(distance/pitch); j++)
            //    {
            //        var rail = new LineCurve(pts1[j], pts2[j]);
            //        brep.AddRange(Brep.CreatePipe(rail, bandD / 2.0, true, PipeCapMode.Flat, true, Stb2Bar.LengthTolerance, Stb2Bar.AngleTolerance));
            //    }
            //}
            
            return brep;
        }

        static Point3d[,] GetBandPos(Point3d[,] cornerPos, int dirXNum, int dirYNum) {
            Point3d[,] bandPos = new Point3d[2, 2 * (dirXNum + dirYNum)];
            Point3d[] pts1;
            Point3d[] pts2;
                
            // dir_X
            for (int j = 0; j < 2; j++) {
                var crv1 = new  LineCurve(cornerPos[j, 1], cornerPos[j, 4]);
                var crv2 = new  LineCurve(cornerPos[j, 2], cornerPos[j, 3]);

                crv1.DivideByCount(dirXNum, true, out pts1);
                crv2.DivideByCount(dirXNum, true, out pts2);

                for (int i = 0; i < dirXNum; i++)
                {
                    bandPos[j, 2 * i] = pts1[i];
                    bandPos[j, 2 * i + 1] = pts2[i];
                }
            }
            // dir_Y
            for (int j = 0; j < 2; j++) {
                var crv1 = new  LineCurve(cornerPos[j, 1], cornerPos[j, 2]);
                var crv2 = new  LineCurve(cornerPos[j, 4], cornerPos[j, 3]);

                crv1.DivideByCount(dirXNum, true, out pts1);
                crv2.DivideByCount(dirXNum, true, out pts2);

                for (int i = dirXNum; i < dirXNum + dirYNum; i++)
                {
                    bandPos[j, 2 * i] = pts1[i - dirXNum];
                    bandPos[j, 2 * i + 1] = pts2[i - dirXNum];
                }
            }
            
            return (bandPos);
        }

        private static List<Brep> ColumnMainBar(Point3d[,] main1Pos, Point3d[,] mainX2Pos, Point3d[,] mainY2Pos, double barSpace, double mainD)
        {
            var brep = new List<Brep>();
            
            return brep;
        }
    }
}
