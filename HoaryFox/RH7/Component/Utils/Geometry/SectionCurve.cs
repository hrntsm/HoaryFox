using Rhino.Geometry;

namespace HoaryFox.Component.Utils.Geometry
{
    public class SectionCurve
    {
        public Curve OuterCurve { get; set; }
        public Curve InnerCurve { get; set; }
        public SectionShape Shape { get; set; }
        public SectionType Type { get; set; }
        public Vector3d XAxis { get; set; }
        public bool IsCft { get; set; }

        public void RotateSection(double inPlaneAngle, Vector3d rotateAxis, Point3d sectionPoint)
        {
            OuterCurve.Rotate(inPlaneAngle, rotateAxis, sectionPoint);
            InnerCurve?.Rotate(inPlaneAngle, rotateAxis, sectionPoint);
        }

        public static SectionCurve CreateSolidColumnRect(Point3d sectionPoint, double widthX, double widthY, Vector3d[] localAxis)
        {
            return new SectionCurve
            {
                OuterCurve = new PolylineCurve(SectionCornerPoints.ColumnRect(sectionPoint, widthX, widthY, localAxis[1], localAxis[2])),
                InnerCurve = null,
                Shape = SectionShape.Rectangle,
                Type = SectionType.Solid,
                XAxis = localAxis[0],
            };
        }

        public static SectionCurve CreateSolidColumnCircle(Point3d sectionPoint, double diameter, Vector3d xAxis)
        {
            return new SectionCurve
            {
                OuterCurve = SectionCornerPoints.ColumnPipe(sectionPoint, diameter / 2, xAxis),
                InnerCurve = null,
                Shape = SectionShape.Circle,
                Type = SectionType.Solid,
                XAxis = xAxis,
            };
        }

        public static SectionCurve CreateSolidBeamRect(Point3d sectionPoint, double depth, double width, Vector3d[] localAxis)
        {
            return new SectionCurve()
            {
                OuterCurve = new PolylineCurve(SectionCornerPoints.BeamRect(sectionPoint, depth, width, localAxis[1], localAxis[2])),
                InnerCurve = null,
                Shape = SectionShape.Rectangle,
                Type = SectionType.Solid,
            };
        }
    }
}
