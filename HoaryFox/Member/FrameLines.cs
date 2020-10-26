using System.Collections.Generic;
using System.Linq;
using STBReader;
using STBReader.Model;
using Rhino.Geometry;

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
            var columns = new List<Line>();
            for (var i = 0; i < _stbData.Columns.Id.Count; i++)
            {
                int idNodeStart = _nodes.Id.IndexOf(_stbData.Columns.IdNodeStart[i]);
                int idNodeEnd = _stbData.Nodes.Id.IndexOf(_stbData.Columns.IdNodeEnd[i]);
                var ptStart = new Point3d(_nodes.X[idNodeStart], _nodes.Y[idNodeStart], _nodes.Z[idNodeStart]);
                var ptEnd = new Point3d(_nodes.X[idNodeEnd], _nodes.Y[idNodeEnd], _nodes.Z[idNodeEnd]);
                columns.Add(new Line(ptStart, ptEnd));
            }

            return columns;
        }

        public List<Line> Girders()
        {
            var girders = new List<Line>();
            for (var i = 0; i < _stbData.Girders.Id.Count; i++)
            {
                int idNodeStart = _nodes.Id.IndexOf(_stbData.Girders.IdNodeStart[i]);
                int idNodeEnd = _nodes.Id.IndexOf(_stbData.Girders.IdNodeEnd[i]);
                var ptStart = new Point3d(_nodes.X[idNodeStart], _nodes.Y[idNodeStart], _nodes.Z[idNodeStart]);
                var ptEnd = new Point3d(_nodes.X[idNodeEnd], _nodes.Y[idNodeEnd], _nodes.Z[idNodeEnd]);
                girders.Add(new Line(ptStart, ptEnd));
            }

            return girders;
        }

        public List<Line> Posts()
        {
            var posts = new List<Line>();
            
            for (var i = 0; i < _stbData.Posts.Id.Count; i++)
            {
                int idNodeStart = _nodes.Id.IndexOf(_stbData.Posts.IdNodeStart[i]);
                int idNodeEnd = _nodes.Id.IndexOf(_stbData.Posts.IdNodeEnd[i]);
                var ptStart = new Point3d(_nodes.X[idNodeStart], _nodes.Y[idNodeStart], _nodes.Z[idNodeStart]);
                var ptEnd = new Point3d(_nodes.X[idNodeEnd], _nodes.Y[idNodeEnd], _nodes.Z[idNodeEnd]);
                posts.Add(new Line(ptStart, ptEnd));
            }

            return posts;
        }

        public List<Line> Beams()
        {
            var beams = new List<Line>();
            
            for (var i = 0; i < _stbData.Beams.Id.Count; i++)
            {
                int idNodeStart = _nodes.Id.IndexOf(_stbData.Beams.IdNodeStart[i]);
                int idNodeEnd = _nodes.Id.IndexOf(_stbData.Beams.IdNodeEnd[i]);
                var ptStart = new Point3d(_nodes.X[idNodeStart], _nodes.Y[idNodeStart], _nodes.Z[idNodeStart]);
                var ptEnd = new Point3d(_nodes.X[idNodeEnd], _nodes.Y[idNodeEnd], _nodes.Z[idNodeEnd]);
                beams.Add(new Line(ptStart, ptEnd));
            }

            return beams;
        }

        public List<Line> Braces()
        {
            var braces = new List<Line>();

            for (var i = 0; i < _stbData.Braces.Id.Count; i++)
            {
                int idNodeStart = _nodes.Id.IndexOf(_stbData.Braces.IdNodeStart[i]);
                int idNodeEnd = _nodes.Id.IndexOf(_stbData.Braces.IdNodeEnd[i]);
                var ptStart = new Point3d(_nodes.X[idNodeStart], _nodes.Y[idNodeStart], _nodes.Z[idNodeStart]);
                var ptEnd = new Point3d(_nodes.X[idNodeEnd], _nodes.Y[idNodeEnd], _nodes.Z[idNodeEnd]);
                braces.Add(new Line(ptStart, ptEnd));
            }

            return braces;
        }

        public List<Point3d> Nodes()
        {
            return _nodes.Pt.Select(point => new Point3d(point.X, point.Y, point.Z)).ToList();
        }
    }
}