using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Grasshopper.Kernel;
using HoaryFox.STB.Member;
using HoaryFox.STB.Model;
using HoaryFox.STB.Section;
using Rhino.Geometry;
using static HoaryFox.STB.StbData;

namespace HoaryFox.STB
{
    interface IStbLoader
    {
        void Load(XDocument stbFile, StbVersion version, string xmlns);
    }
    
    public class StbBase: IStbLoader
    {
        /// <summary>
        /// STBデータ内のタグ
        /// </summary>
        public virtual string Tag { get; } = "StbBase";
        /// <summary>
        /// GUID
        /// </summary>
        public List<string> Guid { get; } = new List<string>();
        /// <summary>
        /// 部材の名前
        /// </summary>
        public List<string> Name { get; } = new List<string>();

        public virtual void Load(XDocument stbFile, StbVersion version, string xmlns)
        {
            if (stbFile.Root == null) 
                return;
            
            var stbElems = stbFile.Root.Descendants(xmlns + Tag);
            foreach (var stbElem in stbElems)
            {
                ElementLoader(stbElem, version, xmlns);
            }
        }

        protected virtual void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            Name.Add((string) stbElem.Attribute("name"));
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
        
        public StbSecColumnRc SecColumnRc;
        public StbSecBeamRc SecBeamRc;
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
            SecColumnRc = new StbSecColumnRc();
            SecBeamRc = new StbSecBeamRc();
            SecColumnS = new StbSecColumnS();
            SecBeamS = new StbSecBeamS();
            SecBraceS = new StbSecBraceS();
            SecSteel = new StbSecSteel();
        }
        
        private void Load(XDocument xDoc)
        {
            var members = new List<IStbLoader>()
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
