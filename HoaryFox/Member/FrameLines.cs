using System.Collections.Generic;
using System.Linq;
using STBReader;
using STBReader.Model;
using Rhino.Geometry;
using STBReader.Member;

namespace HoaryFox.Member
{
    public class FrameLines
    {
        private readonly StbData _stbData;
        private readonly StbNodes _nodes;
        
        public FrameLines(StbData stbData)
        {
            _stbData = stbData;
            _nodes = stbData.Nodes;
        }

        public List<Line> Columns()
        {
            return CreateFrameLines(_stbData.Columns);
        }

        public List<Line> Girders()
        {
            return CreateFrameLines(_stbData.Girders);
        }

        public List<Line> Posts()
        {
            return CreateFrameLines(_stbData.Posts);
        }

        public List<Line> Beams()
        {
            return CreateFrameLines(_stbData.Beams);
        }

        public List<Line> Braces()
        {
            return CreateFrameLines(_stbData.Braces);
        }

        public List<Point3d> Nodes()
        {
            return _nodes.Position.Select(point => new Point3d(point.X, point.Y, point.Z)).ToList();
        }

        private List<Line> CreateFrameLines(StbFrame frame)
        {
            var lines = new List<Line>();
            for (var i = 0; i < frame.Id.Count; i++)
            {
                int idNodeStart = _nodes.Id.IndexOf(frame.IdNodeStart[i]);
                int idNodeEnd = _stbData.Nodes.Id.IndexOf(frame.IdNodeEnd[i]);
                var ptStart = new Point3d(_nodes.X[idNodeStart], _nodes.Y[idNodeStart], _nodes.Z[idNodeStart]);
                var ptEnd = new Point3d(_nodes.X[idNodeEnd], _nodes.Y[idNodeEnd], _nodes.Z[idNodeEnd]);
                lines.Add(new Line(ptStart, ptEnd));
            }
            return lines;
        }
    }
}