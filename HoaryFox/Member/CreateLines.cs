using System.Collections.Generic;
using HoaryFox.STB;
using HoaryFox.STB.Model;
using Rhino.Geometry;

namespace HoaryFox.Member
{
    public class CreateLines
    {
        private readonly StbData _stbData;
        private readonly StbNodes _nodes;
        
        public CreateLines(StbData stbData)
        {
            _stbData = stbData;
            _nodes = stbData.Nodes;
        }

        public List<Line> Columns()
        {
            var columns = new List<Line>();
            for (var i = 0; i < _stbData.Columns.Id.Count; i++)
            {
                var idNodeStart = _nodes.Id.IndexOf(_stbData.Columns.IdNodeStart[i]);
                var idNodeEnd = _stbData.Nodes.Id.IndexOf(_stbData.Columns.IdNodeEnd[i]);
                columns.Add(new Line(_nodes.Pt[idNodeStart], _nodes.Pt[idNodeEnd]));
            }

            return columns;
        }

        public List<Line> Girders()
        {
            var girders = new List<Line>();
            for (var i = 0; i < _stbData.Girders.Id.Count; i++)
            {
                var idNodeStart = _nodes.Id.IndexOf(_stbData.Girders.IdNodeStart[i]);
                var idNodeEnd = _nodes.Id.IndexOf(_stbData.Girders.IdNodeEnd[i]);
                girders.Add(new Line(_nodes.Pt[idNodeStart], _nodes.Pt[idNodeEnd]));
            }

            return girders;
        }

        public List<Line> Posts()
        {
            var posts = new List<Line>();
            
            for (var i = 0; i < _stbData.Posts.Id.Count; i++)
            {
                var idNodeStart = _nodes.Id.IndexOf(_stbData.Posts.IdNodeStart[i]);
                var idNodeEnd = _nodes.Id.IndexOf(_stbData.Posts.IdNodeEnd[i]);
                posts.Add(new Line(_nodes.Pt[idNodeStart], _nodes.Pt[idNodeEnd]));
            }

            return posts;
        }

        public List<Line> Beams()
        {
            var beams = new List<Line>();
            
            for (var i = 0; i < _stbData.Beams.Id.Count; i++)
            {
                var idNodeStart = _nodes.Id.IndexOf(_stbData.Beams.IdNodeStart[i]);
                var idNodeEnd = _nodes.Id.IndexOf(_stbData.Beams.IdNodeEnd[i]);
                beams.Add(new Line(_nodes.Pt[idNodeStart], _nodes.Pt[idNodeEnd]));
            }

            return beams;
        }

        public List<Line> Braces()
        {
            var braces = new List<Line>();

            for (var i = 0; i < _stbData.Braces.Id.Count; i++)
            {
                var idNodeStart = _nodes.Id.IndexOf(_stbData.Braces.IdNodeStart[i]);
                var idNodeEnd = _nodes.Id.IndexOf(_stbData.Braces.IdNodeEnd[i]);
                braces.Add(new Line(_nodes.Pt[idNodeStart], _nodes.Pt[idNodeEnd]));
            }

            return braces;
        }
    }
}