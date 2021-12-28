using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace HoaryFox.Component.Utils.Geometry.BrepMaker
{
    public static class Utils
    {
        public static Vector3d[] CreateLocalAxis(IReadOnlyList<Point3d> sectionPoints)
        {
            var xAxis = new Vector3d(sectionPoints[3] - sectionPoints[0]);
            var yAxis = Vector3d.CrossProduct(Vector3d.ZAxis, xAxis);
            if (yAxis == Vector3d.Zero)
            {
                yAxis = -Vector3d.XAxis;
            }
            var zAxis = Vector3d.CrossProduct(xAxis, yAxis);
            xAxis.Unitize();
            yAxis.Unitize();
            zAxis.Unitize();
            return new Vector3d[] { xAxis, yAxis, zAxis };
        }

        public static void RotateCurveList(Vector3d rotateAxis, IReadOnlyList<Curve> curveList, double stbRotateValue, IReadOnlyList<Point3d> sectionPoints)
        {
            double inPlaneAngle = stbRotateValue * Math.PI / 180;
            switch (curveList.Count)
            {
                case 2:
                    curveList[0].Rotate(inPlaneAngle, rotateAxis, sectionPoints[0]);
                    curveList[1].Rotate(inPlaneAngle, rotateAxis, sectionPoints[3]);
                    break;
                case 3:
                    curveList[0].Rotate(inPlaneAngle, rotateAxis, sectionPoints[0]);
                    curveList[2].Rotate(inPlaneAngle, rotateAxis, sectionPoints[3]);
                    if (sectionPoints[2] == sectionPoints[3])
                    {
                        curveList[1].Rotate(inPlaneAngle, rotateAxis, sectionPoints[1]);
                    }
                    else
                    {
                        curveList[1].Rotate(inPlaneAngle, rotateAxis, sectionPoints[2]);
                    }
                    break;
                case 4:
                    for (var i = 0; i < 4; i++)
                    {
                        curveList[i].Rotate(inPlaneAngle, rotateAxis, sectionPoints[i]);
                    }
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public static Brep CreateCapedBrepFromLoft(List<Curve> curveList, double tolerance)
        {
            Brep brep = Brep.CreateFromLoft(curveList, Point3d.Unset, Point3d.Unset, LoftType.Straight, false)[0]
                .CapPlanarHoles(tolerance);
            if (brep.SolidOrientation == BrepSolidOrientation.Inward)
            {
                brep.Flip();
            }
            brep.Faces.SplitKinkyFaces();
            return brep;
        }

        public enum SectionType
        {
            Column,
            Beam,
            Brace
        }
    }
}
