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
        public static IEnumerable<Brep> FromEndPoint(StbData stbData, ShapeInfo shapeInfo, ShapeTypes shapeType, FrameType frameType)
        {
            Vector3d dVector = shapeInfo.NodeEnd - shapeInfo.NodeStart;
            var angles = new List<double>
            {
                -1 * Math.Atan2(dVector.Y, dVector.X),
                -1 * Math.Atan2(dVector.Z, dVector.X)
            };

            List<List<Point3d>> origins = GetOriginPoints(frameType, shapeInfo, angles);
            List<Brep> breps = GetSecBrep(stbData, shapeType, origins, shapeInfo);
            IEnumerable<Brep> brepRot = ApplyRotation(breps, shapeInfo.Rotate, frameType, origins);
            return brepRot;
        }

        private static IEnumerable<Brep> ApplyRotation(IReadOnlyCollection<Brep> breps, double rotate, FrameType frameType, IReadOnlyList<List<Point3d>> origins)
        {
            double rotateAngle = rotate * Math.PI / 180d;
            var rotationCenter = new Point3d[2];
            if (frameType == FrameType.Girder || frameType == FrameType.Beam)
            {
                rotationCenter[0] = new Point3d(origins[0][4]);
                rotationCenter[1] = new Point3d(origins[0][4]);
            }
            else
            {
                rotationCenter[0] = new Point3d(
                    (origins[0][1].X + origins[0][4].X) / 2, (origins[0][1].Y + origins[0][4].Y) / 2, (origins[0][1].Z + origins[0][4].Z) / 2
                );
                rotationCenter[1] = new Point3d(
                    (origins[1][1].X + origins[1][4].X) / 2, (origins[1][1].Y + origins[1][4].Y) / 2, (origins[1][1].Z + origins[1][4].Z) / 2
                );
            }
            var rotationAxis = new Vector3d(rotationCenter[1] - rotationCenter[0]);
            foreach (Brep b in breps)
            {
                b.Rotate(rotateAngle, rotationAxis, rotationCenter[0]);
            }
            return breps;
        }

        private static List<Brep> GetSecBrep(StbData stbData, ShapeTypes shapeType, IReadOnlyList<List<Point3d>> origins, ShapeInfo shapeInfo)
        {
            var secBrep = new SteelCroSecBrep(stbData, origins);
            switch (shapeType)
            {
                case ShapeTypes.H:
                    return secBrep.HShape();
                case ShapeTypes.BOX:
                case ShapeTypes.BuildBOX:
                case ShapeTypes.RollBOX:
                case ShapeTypes.FB:
                    return secBrep.BoxShape();
                case ShapeTypes.Bar:
                case ShapeTypes.Pipe:
                    return secBrep.PipeShape(shapeInfo);
                case ShapeTypes.L:
                    return secBrep.LShape();
                case ShapeTypes.T:
                    return secBrep.TShape();
                case ShapeTypes.C:
                    return secBrep.CShape();
                default:
                    throw new ArgumentOutOfRangeException(nameof(shapeType), shapeType, null);
            }
        }

        // 梁は部材天端の中心が起点に対して、柱・ブレースは部材芯が起点なので場合分け
        private static List<List<Point3d>> GetOriginPoints(FrameType frameType, ShapeInfo shapeInfo, IReadOnlyList<double> angles)
        {
            var origin = new List<List<Point3d>>();
            switch (frameType)
            {
                case FrameType.Column:
                case FrameType.Post:
                    origin.Add(FramePoints.Column(shapeInfo.NodeStart, shapeInfo.Width, shapeInfo.Height, angles[1]));
                    origin.Add(FramePoints.Column(shapeInfo.NodeEnd, shapeInfo.Width, shapeInfo.Height, angles[1]));
                    break;
                case FrameType.Girder:
                case FrameType.Beam:
                    origin.Add(FramePoints.Girder(shapeInfo.NodeStart, shapeInfo.Width, shapeInfo.Height, angles[0]));
                    origin.Add(FramePoints.Girder(shapeInfo.NodeEnd, shapeInfo.Width, shapeInfo.Height, angles[0]));
                    break;
                case FrameType.Brace:
                    origin.Add(FramePoints.Brace(shapeInfo.NodeStart, shapeInfo.Width, angles[0]));
                    origin.Add(FramePoints.Brace(shapeInfo.NodeEnd, shapeInfo.Width, angles[0]));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(frameType), frameType, null);
            }

            return origin;
        }


        public static Brep PlaneWithOpens(StbData stbData, Point3d[] pts, StbOpen open)
        {
            Brep brep;
            double tol = stbData.ToleLength;

            try
            {
                brep = CreatePlaneBreps(pts, tol);
            }
            catch (NullReferenceException)
            {
                brep = null;
            }

            Brep planeWithOpens = ApplyOpens(brep, pts, open, tol);

            return planeWithOpens;
        }

        private static Brep ApplyOpens(Brep brep, IReadOnlyCollection<Point3d> pts, StbOpen open, double tol)
        {
            Brep planeWithOpens = brep;
            if (open == null || pts.Count != 4 || open.Id.Count == 0 || brep == null)
            {
                return planeWithOpens;
            }

            Surface surface = brep.Surfaces[0];
            var trimSurf = new List<Brep>();

            try
            {
                for (var i = 0; i < open.Id.Count; i++)
                {
                    var intervalX = new Interval(open.PositionX[i], open.PositionX[i] + open.LengthX[i]);
                    var intervalY = new Interval(open.PositionY[i], open.PositionY[i] + open.LengthY[i]);
                    trimSurf.Add(surface.Trim(intervalX, intervalY).ToBrep());
                }
                planeWithOpens = Brep.CreateBooleanDifference(new[] { brep }, trimSurf, tol)[0];
            }
            catch (NullReferenceException)
            {
                planeWithOpens = brep;
            }

            return planeWithOpens;
        }

        private static Brep CreatePlaneBreps(IReadOnlyList<Point3d> pts, double tol)
        {
            switch (pts.Count)
            {
                case 3:
                    return Brep.CreateFromCornerPoints(pts[0], pts[1], pts[2], tol);
                case 4:
                    return Brep.CreateFromCornerPoints(pts[0], pts[1], pts[2], pts[3], tol);
                default:
                    var centerPt = new Point3d();

                    foreach (Point3d pt in pts)
                    {
                        centerPt.X += pt.X / pts.Count;
                        centerPt.Y += pt.Y / pts.Count;
                        centerPt.Z += pt.Z / pts.Count;
                    }
                    var brep = Brep.CreateFromCornerPoints(pts[0], pts[1], centerPt, tol);
                    for (var i = 0; i < pts.Count - 2; i++)
                    {
                        brep.Join(Brep.CreateFromCornerPoints(pts[i + 1], pts[i + 2], centerPt, tol), tol, false);
                    }
                    brep.Join(Brep.CreateFromCornerPoints(pts[pts.Count - 1], pts[0], centerPt, tol), tol, true);
                    return brep;
            }
        }
    }
}
