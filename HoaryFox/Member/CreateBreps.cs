using System;
using System.Collections.Generic;
using Rhino.Geometry;
using STBReader;
using STBReader.Member;
using STBReader.Section;

namespace HoaryFox.Member
{
    public static class CreateBreps
    {
        public static IEnumerable<Brep> FromEndPoint(StbData stbData, Point3d nodeStart, Point3d nodeEnd, double height, double width, double rotate, ShapeTypes shapeType, FrameType frameType)
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
                    pointStart = FramePoints.Column(nodeStart, width, height, angleZ);
                    pointEnd = FramePoints.Column(nodeEnd, width, height, angleZ);
                    break;
                case FrameType.Girder:
                case FrameType.Beam:
                    pointStart = FramePoints.Girder(nodeStart, width, height, angleY);
                    pointEnd = FramePoints.Girder(nodeEnd, width, height, angleY);
                    break;
                case FrameType.Brace:
                    pointStart = FramePoints.Brace(nodeStart, width, angleY);
                    pointEnd = FramePoints.Brace(nodeEnd, width, angleY);
                    break;
                case FrameType.Slab:
                case FrameType.Wall:
                case FrameType.Any:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(frameType), frameType, null);
            }

            var secBrep = new SteelCroSecBrep(stbData);
            switch (shapeType)
            {
                case ShapeTypes.H:
                    brep = secBrep.HShape(pointStart, pointEnd);
                    break;
                case ShapeTypes.BOX:
                case ShapeTypes.BuildBOX:
                case ShapeTypes.RollBOX:
                case ShapeTypes.FB:
                    brep = secBrep.BoxShape(pointStart, pointEnd);
                    break;
                case ShapeTypes.Bar:
                case ShapeTypes.Pipe:
                    brep = secBrep.PipeShape(nodeStart, nodeEnd, width);
                    break;
                case ShapeTypes.L:
                    brep = secBrep.LShape(pointStart, pointEnd);
                    break;
                case ShapeTypes.T:
                    brep = secBrep.TShape(pointStart, pointEnd);
                    break;
                case ShapeTypes.C:
                    brep = secBrep.CShape(pointStart, pointEnd);
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
                    (pointStart[1].X + pointStart[4].X) / 2, (pointStart[1].Y + pointStart[4].Y) / 2, (pointStart[1].Z + pointStart[4].Z) / 2
                );
                rotationCenter[1] = new Point3d(
                    (pointEnd[1].X + pointEnd[4].X) / 2, (pointEnd[1].Y + pointEnd[4].Y) / 2, (pointEnd[1].Z + pointEnd[4].Z) / 2
                );
            }
            var rotationAxis = new Vector3d(rotationCenter[1] - rotationCenter[0]);
            foreach (Brep b in brep)
            {
                b.Rotate(rotateAngle, rotationAxis, rotationCenter[0]);
            }
            return brep;
        }
        

        public static Brep PlaneWithOpens(StbData stbData, Point3d[] pts, StbOpen open)
        {
            Brep brep;
            double tol = stbData.ToleLength;
            
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
            if (open == null || pts.Length != 4 || open.Id.Count == 0 || brep == null)
            {
                return planeBrep;
            }
            
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

            return planeBrep;
        }

    }
}