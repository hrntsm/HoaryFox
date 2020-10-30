using System.Collections.Generic;
using Rhino.Geometry;
using STBReader;

namespace HoaryFox.Member
{
    public class SteelCroSecBrep
    {
        private readonly StbData _stbData;
        private readonly List<Point3d> _pointStart;
        private readonly List<Point3d> _pointEnd;

        public SteelCroSecBrep(StbData stbData, List<List<Point3d>> origins)
        {
            _stbData = stbData;
            _pointStart = origins[0];
            _pointEnd = origins[1];
        }
        
        public List<Brep> CShape()
        {
            var brep = new List<Brep>
            {
                Brep.CreateFromCornerPoints(_pointStart[3], _pointStart[4], _pointEnd[5], _pointEnd[4], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(_pointStart[0], _pointStart[1], _pointEnd[1], _pointEnd[0], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(_pointStart[4], _pointStart[1], _pointEnd[1], _pointEnd[4], _stbData.ToleLength)
            };
            return brep;
        }

        public List<Brep> TShape()
        {
            var brep = new List<Brep>
            {
                Brep.CreateFromCornerPoints(_pointStart[3], _pointStart[5], _pointEnd[5], _pointEnd[3], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(_pointStart[4], _pointStart[1], _pointEnd[1], _pointEnd[4], _stbData.ToleLength)
            };
            return brep;
        }

        public List<Brep> HShape()
        {
            var brep = new List<Brep>
            {
                Brep.CreateFromCornerPoints(_pointStart[3], _pointStart[5], _pointEnd[5], _pointEnd[3], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(_pointStart[0], _pointStart[2], _pointEnd[2], _pointEnd[0], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(_pointStart[4], _pointStart[1], _pointEnd[1], _pointEnd[4], _stbData.ToleLength)
            };
            return brep;
        }

        public List<Brep> BoxShape()
        {
            var brep = new List<Brep>
            {
                Brep.CreateFromCornerPoints(_pointStart[3], _pointStart[5], _pointEnd[5], _pointEnd[3], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(_pointStart[0], _pointStart[2], _pointEnd[2], _pointEnd[0], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(_pointStart[3], _pointStart[0], _pointEnd[0], _pointEnd[3], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(_pointStart[5], _pointStart[2], _pointEnd[2], _pointEnd[5], _stbData.ToleLength)
            };
            return brep;
        }

        public List<Brep> LShape()
        {
            var brep = new List<Brep>
            {
                Brep.CreateFromCornerPoints(_pointStart[0], _pointStart[2], _pointEnd[2], _pointEnd[0], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(_pointStart[5], _pointStart[2], _pointEnd[2], _pointEnd[5], _stbData.ToleLength)
            };
            return brep;
        }

        public List<Brep> PipeShape(ShapeInfo shapeInfo)
        {
            var brep = new List<Brep>();            
            brep.AddRange(Brep.CreatePipe(new LineCurve(shapeInfo.NodeStart, shapeInfo.NodeEnd), shapeInfo.Width / 2, true, PipeCapMode.Flat, true, _stbData.ToleLength, _stbData.ToleAngle));
            return brep;
        }
    }
}