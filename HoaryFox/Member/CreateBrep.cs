using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HoaryFox.STB;
using Rhino.Geometry;
using Rhino.Geometry.Collections;

namespace HoaryFox.Member
{
    public class CreateBrep
    {
        private readonly StbData _stbData;

        public CreateBrep(StbData stbData)
        {
            _stbData = stbData;
        }

        private Brep GetPlaneBrep(int count, Point3d[] pt, StbOpen open)
        {
            Brep brep = null;
            Brep planeBrep = null;
            var tol = _stbData.ToleLength;
            
            // TODO: 10節点までとりあえず対応。節点が同一線上にあるとたぶんエラーになるので とりあえずtry-catch で対応
            try
            {
                switch (count)
                {
                    case 3:
                        brep = Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], tol);
                        break;
                    case 4:
                        brep = Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2],  pt[3], tol);
                        break;
                    case 5:
                        brep = Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], pt[3], tol);
                        brep.Join(Brep.CreateFromCornerPoints(pt[3], pt[4], pt[0], tol), tol, true);
                        break;
                    case 6:
                        brep = Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], pt[3], tol);
                        brep.Join(Brep.CreateFromCornerPoints(pt[0], pt[3], pt[4], pt[5], tol), tol, true);
                        break;
                    case 7:
                        brep = Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], pt[3], tol);
                        brep.Join(Brep.CreateFromCornerPoints(pt[3], pt[4], pt[5], pt[6], tol), tol, false);
                        brep.Join(Brep.CreateFromCornerPoints(pt[6], pt[0], pt[3], tol), tol, true);
                        break;
                    case 8:
                        brep = Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], pt[3], tol);
                        brep.Join(Brep.CreateFromCornerPoints(pt[3], pt[4], pt[5], pt[6], tol), tol, false);
                        brep.Join(Brep.CreateFromCornerPoints(pt[6], pt[7], pt[0], pt[3], tol), tol, true);
                        break;
                    case 9:
                        brep = Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], pt[3], tol);
                        brep.Join(Brep.CreateFromCornerPoints(pt[3], pt[4], pt[5], pt[6], tol), tol, false);
                        brep.Join(Brep.CreateFromCornerPoints(pt[6], pt[7], pt[8], pt[0], tol), tol, false);
                        brep.Join(Brep.CreateFromCornerPoints(pt[0], pt[3], pt[6], tol), tol, true);
                        break;
                    case 10:
                        brep = Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], pt[3], tol);
                        brep.Join(Brep.CreateFromCornerPoints(pt[3], pt[4], pt[5], pt[6], tol), tol, false);
                        brep.Join(Brep.CreateFromCornerPoints(pt[6], pt[7], pt[8], pt[9], tol), tol, false);
                        brep.Join(Brep.CreateFromCornerPoints(pt[9], pt[0], pt[3], pt[6], tol), tol, true);
                        break;
                    default:
                        brep = null;
                        break;
                }
            }
            catch(NullReferenceException)
            {
                brep = Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], pt[3], tol);
            }

            if (open  != null)
            {
                if (open.Id.Count != 0  && brep != null)
                {
                    var surface = brep.Surfaces[0];
                    var trimSurf = new List<Brep>();

                    for (var i = 0; i < open.Id.Count; i++)
                    {
                        var intervalX = new Interval(open.PositionX[i], open.PositionX[i] + open.LengthX[i]);
                        var intervalY = new Interval(open.PositionY[i], open.PositionY[i] + open.LengthY[i]);
                        trimSurf.Add(surface.Trim(intervalX, intervalY).ToBrep());
                    }
                    try
                    {
                        var diffBrep = Brep.CreateBooleanDifference(new Brep[]{ brep }, trimSurf.ToArray(), tol)[0];
                        planeBrep = diffBrep;
                    }
                    catch
                    {
                        planeBrep = brep;
                    }
                }
                else
                {
                    planeBrep = brep;
                }
            }
            else
            {
                planeBrep = brep;
            }

            return planeBrep;
        }

        public List<Brep> Slab(StbSlabs slabs)
        {
            var brep = new List<Brep>();
            var count = 0;

            foreach (var nodeIds in slabs.NodeIdList)
            {
                var index = new int[10];
                var pt = new Point3d[10];
                var offset = slabs.Level[count];

                for (var i = 0; i < nodeIds.Count; i++)
                {
                    if (i > 9) continue;
                    index[i] = _stbData.Nodes.Id.IndexOf(nodeIds[i]);
                    pt[i] = new Point3d(_stbData.Nodes.X[index[i]], _stbData.Nodes.Y[index[i]], _stbData.Nodes.Z[index[i]] + offset);
                }
                
                brep.Add(GetPlaneBrep(nodeIds.Count, pt, null));
                count++;
            }

            return brep;
        }

        public List<Brep> Wall(StbWalls walls)
        {
            var brep = new List<Brep>();
            var count = 0;

            foreach (var nodeIds in walls.NodeIdList)
            {
                var index = new int[10];
                var pt = new Point3d[10];

                for (var i = 0; i < nodeIds.Count; i++)
                {
                    if (i > 9) continue;
                    index[i] = _stbData.Nodes.Id.IndexOf(nodeIds[i]);
                    pt[i] = new Point3d(_stbData.Nodes.X[index[i]], _stbData.Nodes.Y[index[i]], _stbData.Nodes.Z[index[i]]);
                }

                brep.Add(GetPlaneBrep(nodeIds.Count, pt, walls.Opens[count]));
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
                var idSection = frame.IdSection[eNum];
                var kind = frame.KindStructure[eNum];
                var rotate = frame.Rotate[eNum];

                // 始点と終点の座標取得
                var nodeIndexStart = _stbData.Nodes.Id.IndexOf(frame.IdNodeStart[eNum]);
                var nodeIndexEnd = _stbData.Nodes.Id.IndexOf(frame.IdNodeEnd[eNum]);
                var nodeStart = new Point3d(_stbData.Nodes.X[nodeIndexStart], _stbData.Nodes.Y[nodeIndexStart], _stbData.Nodes.Z[nodeIndexStart]);
                var nodeEnd = new Point3d(_stbData.Nodes.X[nodeIndexEnd], _stbData.Nodes.Y[nodeIndexEnd], _stbData.Nodes.Z[nodeIndexEnd]);

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
                        case FrameType.Brace:
                        case FrameType.Slab:
                        case FrameType.Wall:
                        case FrameType.Any:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
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
                }

                brep.AddRange(Point2Brep(nodeStart, nodeEnd, height, width, rotate, shapeType,  frame.FrameType));
            }

            return brep;
        }


        private List<Brep> Point2Brep(Point3d nodeStart, Point3d nodeEnd, double height, double width, double rotate, ShapeTypes shapeType, FrameType frameType)
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

            var rotateAngle = rotate * Math.PI / 180;
            Point3d rotationCenter;
            if (frameType == FrameType.Girder || frameType == FrameType.Beam)
            {
                rotationCenter = new Point3d(pointStart[4]);
            }
            else
            {
                rotationCenter = new Point3d(
                    (pointStart[1].X + pointStart[4].X) / 2,
                    (pointStart[1].Y + pointStart[4].Y) / 2,
                    (pointStart[1].Z + pointStart[4].Z) / 2
                );
            }
            var rotationAxis = new Vector3d(pointStart[1] - pointEnd[1]);
            foreach (var b in brep)
            {
                b.Rotate(rotateAngle, rotationAxis, rotationCenter);
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
