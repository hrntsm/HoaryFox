using System.Collections.Generic;
using System.Xml.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace HoaryFox.STB
{
    public class StbBase
    {
        public virtual void Load(XDocument stbData)
        {
        }
    }

    public class StbData
    {
        public readonly string Path;
        public readonly double ToleLength;
        public readonly double ToleAngle;
        
        public StbNodes Nodes;
        public StbColumns Columns;
        public StbPosts Posts;
        public StbGirders Girders;
        public StbBeams Beams;
        public StbBraces Braces;
        public StbSlabs Slabs;
        public StbWalls Walls;
        
        public StbSecColRC SecColumnRc;
        public StbSecBeamRC SecBeamRc;
        public StbSecColumnS SecColumnS;
        public StbSecBeamS SecBeamS;
        public StbSecBraceS SecBraceS;
        public StbSecSteel StbSecSteel;

        public StbData(string path, double toleLength, double toleAngle)
        {
            Path = path;
            ToleLength = toleLength;
            ToleAngle = toleAngle;
            
            var xDocument = XDocument.Load(Path);
            
            Init();
            Load(xDocument);
        }
        
        private void Init()
        {
            Nodes = new StbNodes();
            Columns = new StbColumns();
            Posts = new StbPosts();
            Girders = new StbGirders();
            Beams = new StbBeams();
            Braces = new StbBraces();
            Slabs = new StbSlabs();
            Walls = new StbWalls();
            SecColumnRc = new StbSecColRC();
            SecBeamRc = new StbSecBeamRC();
            SecColumnS = new StbSecColumnS();
            SecBeamS = new StbSecBeamS();
            SecBraceS = new StbSecBraceS();
            StbSecSteel = new StbSecSteel();
        }
        
        private void Load(XDocument xDoc)
        {
            var members = new List<StbBase>()
            {
                Nodes, Slabs, Walls,
                Columns, Posts, Girders, Beams, Braces,
                SecColumnRc, SecColumnS, SecBeamRc, SecBeamS, SecBraceS, StbSecSteel
            };

            foreach (var member in members)
            {
                member.Load(xDoc);
            }
        }
    }
}
