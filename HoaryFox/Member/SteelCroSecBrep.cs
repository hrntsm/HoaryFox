using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using Rhino.Geometry;
using STBReader;

namespace HoaryFox.Member
{
    public class SteelCroSecBrep
    {
        private readonly StbData _stbData;
        private readonly double _tol;
        private readonly List<Point3d> _pointStart;
        private readonly List<Point3d> _pointEnd;

        public SteelCroSecBrep(StbData stbData, IReadOnlyList<List<Point3d>> origins)
        {
            _stbData = stbData;
            _tol = _stbData.ToleLength;
            _pointStart = origins[0];
            _pointEnd = origins[1];
        }

        public List<Brep> CShape()
        {
            var breps = new List<Brep>
            {
                Brep.CreateFromCornerPoints(_pointStart[2], _pointStart[1], _pointEnd[1], _pointEnd[2], _tol),
                Brep.CreateFromCornerPoints(_pointStart[1], _pointStart[4], _pointEnd[4], _pointEnd[1], _tol),
                Brep.CreateFromCornerPoints(_pointStart[4], _pointStart[5], _pointEnd[5], _pointEnd[4], _tol)
            };
            List<Brep> joinedBrep = Brep.JoinBreps(breps, _tol).ToList();

            return joinedBrep;
        }

        public List<Brep> TShape()
        {
            Point3d[] pointIs = MakeHInnerPoint(_pointStart);
            Point3d[] pointIe = MakeHInnerPoint(_pointEnd);

            var breps = new List<Brep>
            {
                Brep.CreateFromCornerPoints(_pointStart[3], _pointStart[5], _pointEnd[5], _pointEnd[3], _tol),
                Brep.CreateFromCornerPoints(_pointStart[5], pointIs[5], pointIe[5], _pointEnd[5], _tol),
                Brep.CreateFromCornerPoints(pointIs[5], pointIs[4], pointIe[4], pointIe[5], _tol),
                Brep.CreateFromCornerPoints(pointIs[4], pointIs[1], pointIe[1], pointIe[4], _tol),
                Brep.CreateFromCornerPoints(pointIs[1], pointIs[0], pointIe[0], pointIe[1], _tol),
                Brep.CreateFromCornerPoints(pointIs[0], pointIs[3], pointIe[3], pointIe[0], _tol),
                Brep.CreateFromCornerPoints(pointIs[3], pointIs[2], pointIe[2], pointIe[3], _tol),
                Brep.CreateFromCornerPoints(pointIs[2], _pointStart[3], _pointEnd[3], pointIe[2], _tol),
            };
            List<Brep> joinedBrep = Brep.JoinBreps(breps, _tol).ToList();

            return joinedBrep.Select(b => b.CapPlanarHoles(_tol)).ToList();
        }

        //           o3 -    - o4 -    - o5
        //           |                    |
        //  Y        i2 - i3        i4 - i5
        //  ^              |        |
        //  o >  X   o0 - i0 - o1 - i1 - o2
        private static Point3d[] MakeTInnerPoint(IReadOnlyList<Point3d> outPoints)
        {
            var points = new Point3d[6];

            points[0] = 0.55 * outPoints[0] + 0.45 * outPoints[2];
            points[1] = 0.55 * outPoints[2] + 0.45 * outPoints[0];
            points[2] = 0.95 * outPoints[3] + 0.05 * outPoints[0];
            points[5] = 0.95 * outPoints[5] + 0.05 * outPoints[2];

            points[3] = 0.55 * points[5] + 0.45 * points[2];
            points[4] = 0.55 * points[2] + 0.45 * points[5];

            return points;
        }

        public List<Brep> HShape()
        {
            Point3d[] pointIs = MakeHInnerPoint(_pointStart);
            Point3d[] pointIe = MakeHInnerPoint(_pointEnd);

            var breps = new List<Brep>
            {
                Brep.CreateFromCornerPoints(_pointStart[3], _pointStart[5], _pointEnd[5], _pointEnd[3], _tol),
                Brep.CreateFromCornerPoints(_pointStart[5], pointIs[7], pointIe[7], _pointEnd[5], _tol),
                Brep.CreateFromCornerPoints(pointIs[7], pointIs[6], pointIe[6], pointIe[7], _tol),
                Brep.CreateFromCornerPoints(pointIs[6], pointIs[2], pointIe[2], pointIe[6], _tol),
                Brep.CreateFromCornerPoints(pointIs[2], pointIs[3], pointIe[3], pointIe[2], _tol),
                Brep.CreateFromCornerPoints(pointIs[3], _pointStart[2], _pointEnd[2], pointIe[3], _tol),
                Brep.CreateFromCornerPoints(_pointStart[2], _pointStart[0], _pointEnd[0], _pointEnd[2], _tol),
                Brep.CreateFromCornerPoints(_pointStart[0], pointIs[0], pointIe[0], _pointEnd[0], _tol),
                Brep.CreateFromCornerPoints(pointIs[0], pointIs[1], pointIe[1], pointIe[0], _tol),
                Brep.CreateFromCornerPoints(pointIs[1], pointIs[5], pointIe[5], pointIe[1], _tol),
                Brep.CreateFromCornerPoints(pointIs[5], pointIs[4], pointIe[4], pointIe[5], _tol),
                Brep.CreateFromCornerPoints(pointIs[4], _pointStart[3], _pointEnd[3], pointIe[4], _tol),
            };
            List<Brep> joinedBrep = Brep.JoinBreps(breps, _tol).ToList();

            return joinedBrep.Select(b => b.CapPlanarHoles(_tol)).ToList();
        }

        //           o3  -  o4  -  o5
        //           |              |
        //           i4 - i5  i6 - i7
        //                 |  |
        //  Y        i0 - i1  i2 - i3
        //  ^        |              |
        //  o >  X   o0  -  o1  -  o2
        private static Point3d[] MakeHInnerPoint(IReadOnlyList<Point3d> outPoints)
        {
            var points = new Point3d[8];

            points[0] = 0.95 * outPoints[0] + 0.05 * outPoints[3];
            points[3] = 0.95 * outPoints[2] + 0.05 * outPoints[5];
            points[4] = 0.95 * outPoints[3] + 0.05 * outPoints[0];
            points[7] = 0.95 * outPoints[5] + 0.05 * outPoints[2];

            points[1] = 0.55 * points[0] + 0.45 * points[3];
            points[2] = 0.55 * points[3] + 0.45 * points[0];
            points[5] = 0.55 * points[4] + 0.45 * points[7];
            points[6] = 0.55 * points[7] + 0.45 * points[4];

            return points;
        }

        public List<Brep> BoxShape()
        {
            var breps = new List<Brep>
            {
                Brep.CreateFromCornerPoints(_pointStart[3], _pointStart[5], _pointEnd[5], _pointEnd[3], _tol),
                Brep.CreateFromCornerPoints(_pointStart[0], _pointStart[2], _pointEnd[2], _pointEnd[0], _tol),
                Brep.CreateFromCornerPoints(_pointStart[3], _pointStart[0], _pointEnd[0], _pointEnd[3], _tol),
                Brep.CreateFromCornerPoints(_pointStart[5], _pointStart[2], _pointEnd[2], _pointEnd[5], _tol)
            };
            List<Brep> joinedBrep = Brep.JoinBreps(breps, _tol).ToList();

            return joinedBrep.Select(b => b.CapPlanarHoles(_tol)).ToList();
        }

        public List<Brep> LShape()
        {
            var breps = new List<Brep>
            {
                Brep.CreateFromCornerPoints(_pointStart[0], _pointStart[2], _pointEnd[2], _pointEnd[0], _tol),
                Brep.CreateFromCornerPoints(_pointStart[5], _pointStart[2], _pointEnd[2], _pointEnd[5], _tol)
            };
            List<Brep> joinedBrep = Brep.JoinBreps(breps, _tol).ToList();

            return joinedBrep;
        }

        public List<Brep> PipeShape(ShapeInfo shapeInfo)
        {
            var brep = new List<Brep>();
            brep.AddRange(Brep.CreatePipe(new LineCurve(shapeInfo.NodeStart, shapeInfo.NodeEnd), shapeInfo.Width / 2, true, PipeCapMode.Flat, true, _stbData.ToleLength, _stbData.ToleAngle));
            return brep;
        }
    }
}
