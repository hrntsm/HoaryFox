using System.Collections.Generic;
using Rhino.Geometry;
using STBReader;

namespace HoaryFox.Member
{
    public class SteelCroSecBrep
    {
        private readonly StbData _stbData;

        public SteelCroSecBrep(StbData stbData)
        {
            _stbData = stbData;
        }
        
        public List<Brep> CShape(Point3d[] pointStart, Point3d[] pointEnd)
        {
            var brep = new List<Brep>
            {
                Brep.CreateFromCornerPoints(pointStart[3], pointStart[4], pointEnd[5], pointEnd[4], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(pointStart[0], pointStart[1], pointEnd[1], pointEnd[0], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(pointStart[4], pointStart[1], pointEnd[1], pointEnd[4], _stbData.ToleLength)
            };
            return brep;
        }

        public List<Brep> TShape(Point3d[] pointStart, Point3d[] pointEnd)
        {
            var brep = new List<Brep>
            {
                Brep.CreateFromCornerPoints(pointStart[3], pointStart[5], pointEnd[5], pointEnd[3], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(pointStart[4], pointStart[1], pointEnd[1], pointEnd[4], _stbData.ToleLength)
            };
            return brep;
        }

        public List<Brep> HShape(Point3d[] pointStart, Point3d[] pointEnd)
        {
            var brep = new List<Brep>
            {
                Brep.CreateFromCornerPoints(pointStart[3], pointStart[5], pointEnd[5], pointEnd[3], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(pointStart[0], pointStart[2], pointEnd[2], pointEnd[0], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(pointStart[4], pointStart[1], pointEnd[1], pointEnd[4], _stbData.ToleLength)
            };
            return brep;
        }

        public List<Brep> BoxShape(Point3d[] pointStart, Point3d[] pointEnd)
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

        public List<Brep> LShape(Point3d[] pointStart, Point3d[] pointEnd)
        {
            var brep = new List<Brep>
            {
                Brep.CreateFromCornerPoints(pointStart[0], pointStart[2], pointEnd[2], pointEnd[0], _stbData.ToleLength),
                Brep.CreateFromCornerPoints(pointStart[5], pointStart[2], pointEnd[2], pointEnd[5], _stbData.ToleLength)
            };
            return brep;
        }

        public List<Brep> PipeShape(Point3d nodeStart, Point3d nodeEnd, double width)
        {
            var brep = new List<Brep>();            
            brep.AddRange(Brep.CreatePipe(new LineCurve(nodeStart, nodeEnd), width / 2, true, PipeCapMode.Flat, true, _stbData.ToleLength, _stbData.ToleAngle));
            return brep;
        }
    }
}