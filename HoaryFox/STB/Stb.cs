using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace HoaryFox.STB
{
    public class StbBase
    {
        public virtual void Load(XDocument stbData, StbData.StbVersion version, string xmlns)
        {
        }
    }

    public class StbData
    {
        public readonly string Path;
        public readonly double ToleLength;
        public readonly double ToleAngle;

        public string Xmlns;
        public StbVersion Version;
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
        public StbSecSteel SecSteel;

        public StbData(string path, double toleLength, double toleAngle)
        {
            Path = path;
            ToleLength = toleLength;
            ToleAngle = toleAngle;
            
            var xDocument = XDocument.Load(Path);

            var root = xDocument.Root;
            if (root != null)
            {
                if (root.Attribute("xmlns") != null)
                {
                    Xmlns = "{" + (string)root.Attribute("xmlns") + "}";
                }
                else
                {
                    Xmlns = string.Empty;
                }

                var tmp = (string) root.Attribute("version");
                switch (tmp.Split('.')[0])
                {
                    case "1":
                        Version = StbVersion.Ver1;
                        break;
                    case "2":
                        Version = StbVersion.Ver2;
                        break;
                }
            }
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
            SecSteel = new StbSecSteel();
        }
        
        private void Load(XDocument xDoc)
        {
            var members = new List<StbBase>()
            {
                Nodes, Slabs, Walls,
                Columns, Posts, Girders, Beams, Braces,
                SecColumnRc, SecColumnS, SecBeamRc, SecBeamS, SecBraceS, SecSteel
            };

            foreach (var member in members)
            {
                member.Load(xDoc, Version, Xmlns);
            }
        }
        
        public enum StbVersion
        {
            Ver1,
            Ver2
        }
    }
}
