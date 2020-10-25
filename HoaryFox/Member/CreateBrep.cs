﻿using System;
using System.Collections.Generic;
using STBReader;
using STBReader.Member;
using STBReader.Model;
using STBReader.Section;
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

        private Brep GetPlaneBrep(Point3d[] pts, StbOpen open)
        {
            Brep brep;
            double tol = _stbData.ToleLength;
            
            try
            {
                var centerPt = new Point3d();

                switch (pts.Length)
                {
                    case 3:
                        brep = Brep.CreateFromCornerPoints(pts[0], pts[1], pts[2], tol);
                        break;
                    case 4:
                        brep = Brep.CreateFromCornerPoints(pts[0], pts[1], pts[2], pts[3], tol);
                        break;
                    default:
                        foreach (Point3d pt in pts)
                        {
                            centerPt.X += pt.X / pts.Length;
                            centerPt.Y += pt.Y / pts.Length;
                            centerPt.Z += pt.Z / pts.Length;
                        }
                        brep = Brep.CreateFromCornerPoints(pts[0], pts[1], centerPt, tol);
                        for (var i = 0; i < pts.Length - 2; i++)
                        {
                            brep.Join(Brep.CreateFromCornerPoints(pts[i + 1], pts[i + 2], centerPt, tol), tol, false);
                        }
                        brep.Join(Brep.CreateFromCornerPoints(pts[pts.Length - 1], pts[0], centerPt, tol), tol, true);
                        break;
                }
            }
            catch(NullReferenceException)
            {
                brep = null;
            }

            Brep planeBrep = brep;
            if (open  != null && pts.Length == 4)
            {
                if (open.Id.Count != 0  && brep != null)
                {
                    Surface surface = brep.Surfaces[0];
                    var trimSurf = new List<Brep>();
                    
                    try
                    {
                        for (var i = 0; i < open.Id.Count; i++)
                        {
                            // TODO:いつトリムが失敗するか調べる
                            var intervalX = new Interval(open.PositionX[i], open.PositionX[i] + open.LengthX[i]);
                            var intervalY = new Interval(open.PositionY[i], open.PositionY[i] + open.LengthY[i]);
                            trimSurf.Add(surface.Trim(intervalX, intervalY).ToBrep());
                        }
                        planeBrep = Brep.CreateBooleanDifference(new[]{ brep }, trimSurf, tol)[0];
                    }
                    catch (NullReferenceException)
                    {
                        planeBrep = brep;
                    }
                }
            }

            return planeBrep;
        }

        public List<Brep> Slab(StbSlabs slabs)
        {
            var brep = new List<Brep>();
            var count = 0;

            foreach (List<int> nodeIds in slabs.NodeIdList)
            {
                var index = new int[nodeIds.Count];
                var pts = new Point3d[nodeIds.Count];
                double offset = slabs.Level[count];

                for (var i = 0; i < nodeIds.Count; i++)
                {
                    index[i] = _stbData.Nodes.Id.IndexOf(nodeIds[i]);
                    pts[i] = new Point3d(_stbData.Nodes.X[index[i]], _stbData.Nodes.Y[index[i]], _stbData.Nodes.Z[index[i]] + offset);
                }
                
                brep.Add(GetPlaneBrep(pts, null));
                count++;
            }

            return brep;
        }

        public List<Brep> Wall(StbWalls walls)
        {
            var brep = new List<Brep>();
            var count = 0;

            foreach (List<int> nodeIds in walls.NodeIdList)
            {
                var index = new int[nodeIds.Count];
                var pts = new Point3d[nodeIds.Count];

                for (var i = 0; i < nodeIds.Count; i++)
                {
                    index[i] = _stbData.Nodes.Id.IndexOf(nodeIds[i]);
                    pts[i] = new Point3d(_stbData.Nodes.X[index[i]], _stbData.Nodes.Y[index[i]], _stbData.Nodes.Z[index[i]]);
                }

                brep.Add(GetPlaneBrep(pts, walls.Opens[count]));
                count++;
            }

            return brep;
        }

        public List<Brep> Frame(StbFrame frame)
        {
            var brep = new List<Brep>();

            double height = -1;
            double width = -1;
            var shape = string.Empty;
            var shapeType = ShapeTypes.H;

            for (var eNum = 0; eNum < frame.Id.Count; eNum++)
            {
                int idSection = frame.IdSection[eNum];
                KindsStructure kind = frame.KindStructure[eNum];
                double rotate = frame.Rotate[eNum];

                // 始点と終点の座標取得
                int nodeIndexStart = _stbData.Nodes.Id.IndexOf(frame.IdNodeStart[eNum]);
                int nodeIndexEnd = _stbData.Nodes.Id.IndexOf(frame.IdNodeEnd[eNum]);
                var nodeStart = new Point3d(_stbData.Nodes.X[nodeIndexStart], _stbData.Nodes.Y[nodeIndexStart], _stbData.Nodes.Z[nodeIndexStart]);
                var nodeEnd = new Point3d(_stbData.Nodes.X[nodeIndexEnd], _stbData.Nodes.Y[nodeIndexEnd], _stbData.Nodes.Z[nodeIndexEnd]);

                int secIndex;
                switch (kind)
                {
                    case KindsStructure.Rc:
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
                            case FrameType.Brace:
                            case FrameType.Slab:
                            case FrameType.Wall:
                            case FrameType.Any:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        shapeType = height <= 0 ? ShapeTypes.Pipe : ShapeTypes.BOX;
                        break;
                    case KindsStructure.S:
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
                            case FrameType.Slab:
                            case FrameType.Wall:
                            case FrameType.Any:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        secIndex = _stbData.SecSteel.Name.IndexOf(shape);
                        height = _stbData.SecSteel.P1[secIndex];
                        width = _stbData.SecSteel.P2[secIndex];
                        shapeType = _stbData.SecSteel.ShapeType[secIndex];
                        break;
                    }
                    case KindsStructure.Src:
                        break;
                    case KindsStructure.Cft:
                        break;
                    case KindsStructure.Deck:
                        break;
                    case KindsStructure.Precast:
                        break;
                    case KindsStructure.Other:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                brep.AddRange(Point2Brep(nodeStart, nodeEnd, height, width, rotate, shapeType,  frame.FrameType));
            }

            return brep;
        }


        private IEnumerable<Brep> Point2Brep(Point3d nodeStart, Point3d nodeEnd, double height, double width, double rotate, ShapeTypes shapeType, FrameType frameType)
        {
            var pointStart = new Point3d[6];
            var pointEnd = new Point3d[6];
            List<Brep> brep;

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
                case FrameType.Slab:
                case FrameType.Wall:
                case FrameType.Any:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(frameType), frameType, null);
            }

            switch (shapeType)
            {
                case ShapeTypes.H:
                    brep = HShapeBrep(pointStart, pointEnd);
                    break;
                case ShapeTypes.BOX:
                case ShapeTypes.BuildBOX:
                case ShapeTypes.RollBOX:
                case ShapeTypes.FB:
                    brep = BoxShapeBrep(pointStart, pointEnd);
                    break;
                case ShapeTypes.Bar:
                case ShapeTypes.Pipe:
                    brep = PipeShapeBrep(nodeStart, nodeEnd, width);
                    break;
                case ShapeTypes.L:
                    brep = LShapeBrep(pointStart, pointEnd);
                    break;
                case ShapeTypes.T:
                    brep = TShapeBrep(pointStart, pointEnd);
                    break;
                case ShapeTypes.C:
                    brep = CShapeBrep(pointStart, pointEnd);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shapeType), shapeType, null);
            }

            double rotateAngle = rotate * Math.PI / 180d;
            var rotationCenter = new Point3d[2];
            if (frameType == FrameType.Girder || frameType == FrameType.Beam)
            {
                rotationCenter[0] = new Point3d(pointStart[4]);
                rotationCenter[1] = new Point3d(pointStart[4]);
            }
            else
            {
                rotationCenter[0] = new Point3d(
                    (pointStart[1].X + pointStart[4].X) / 2,
                    (pointStart[1].Y + pointStart[4].Y) / 2,
                    (pointStart[1].Z + pointStart[4].Z) / 2
                );
                rotationCenter[1] = new Point3d(
                    (pointEnd[1].X + pointEnd[4].X) / 2,
                    (pointEnd[1].Y + pointEnd[4].Y) / 2,
                    (pointEnd[1].Z + pointEnd[4].Z) / 2
                );
            }
            var rotationAxis = new Vector3d(rotationCenter[1] - rotationCenter[0]);
            foreach (Brep b in brep)
            {
                b.Rotate(rotateAngle, rotationAxis, rotationCenter[0]);
            }
            return brep;
        }

        private List<Brep> CShapeBrep(Point3d[] pointStart, Point3d[] pointEnd)
        {
            var brep = new List<Brep>
            {
                Brep.CreateFromCornerPoints(pointStart[3], pointStart[4], pointEnd[5], pointEnd[4], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(pointStart[0], pointStart[1], pointEnd[1], pointEnd[0], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(pointStart[4], pointStart[1], pointEnd[1], pointEnd[4], _stbData.ToleLength)
            };
            return brep;
        }

        private List<Brep> TShapeBrep(Point3d[] pointStart, Point3d[] pointEnd)
        {
            var brep = new List<Brep>
            {
                Brep.CreateFromCornerPoints(pointStart[3], pointStart[5], pointEnd[5], pointEnd[3], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(pointStart[4], pointStart[1], pointEnd[1], pointEnd[4], _stbData.ToleLength)
            };
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
            return (points);
        }

        private static Point3d[] GetGirderPoints(Point3d node, double width, double height, double angle)
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
            return (points);
        }

        private static Point3d[] GetBracePoints(Point3d node, double width, double angle)
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
            return (points);
        }
    }
}
