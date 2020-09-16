using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Rhino.Geometry;
using static HoaryFox.STB.StbData;

namespace HoaryFox.STB.Model
{

    /// <summary>
    /// 節点（複数） 各節点を管理
    /// </summary>
    public class StbNodes : StbModel
    {
        public override string Tag { get; } = "StbNode";
        public List<double> X { get; } = new List<double>();
        public List<double> Y { get; } = new List<double>();
        public List<double> Z { get; } = new List<double>();
        public List<Point3d> Pt { get; } = new List<Point3d>();
        public List<KindsNode> Kind { get; } = new List<KindsNode>();
        public List<int> IdMember { get; } = new List<int>();

        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
                // 必須コード
                double posX, posY, posZ;
                switch (version)
                {
                    case StbVersion.Ver1:
                        posX = (double)stbElem.Attribute("x");
                        posY = (double)stbElem.Attribute("y");
                        posZ = (double)stbElem.Attribute("z");
                        break;
                    case StbVersion.Ver2:
                        posX = (double)stbElem.Attribute("X");
                        posY = (double)stbElem.Attribute("Y");
                        posZ = (double)stbElem.Attribute("Z");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(version), version, null);
                }
                X.Add(posX);
                Y.Add(posY);
                Z.Add(posZ);
                Pt.Add(new Point3d(posX, posY, posZ));
                
                // 必須ではないコード
                if (stbElem.Attribute("id_member") != null)
                    IdMember.Add((int)stbElem.Attribute("id_member"));
                else
                    IdMember.Add(-1);
                
                // ver2 から必須
                switch ((string)stbElem.Attribute("kind"))
                {
                    case "ON_GIRDER":
                        Kind.Add(KindsNode.OnGirder); break;
                    case "ON_BEAM":
                        Kind.Add(KindsNode.OnBeam); break;
                    case "ON_COLUMN":
                        Kind.Add(KindsNode.OnColumn); break;
                    case "ON_POST":
                        Kind.Add(KindsNode.OnPost); break;
                    case "ON_GRID":
                        Kind.Add(KindsNode.OnGrid); break;
                    case "ON_CANTI":
                        Kind.Add(KindsNode.OnCanti); break;
                    case "ON_SLAB":
                        Kind.Add(KindsNode.OnSlab); break;
                    default:
                        Kind.Add(KindsNode.Other); break;
                }
        }
    }

    /// <summary>
    /// 節点IDリスト
    /// </summary>
    public class StbNodeIdList
    {
        public List<int> IdList { get; }= new List<int>();
        
        public void Load(XElement stbElem, StbVersion stbVersion)
        {
            switch (stbVersion)
            {
                case StbVersion.Ver1:
                    var xNodeIds = stbElem.Element("StbNodeid_List")?.Elements("StbNodeid");
                    if (xNodeIds != null)
                        foreach (var xNodeId in xNodeIds)
                            IdList.Add((int) xNodeId.Attribute("id"));
                    break;
                case StbVersion.Ver2:
                    var xNodeIdOrders = stbElem.Value;
                    var nodeList = xNodeIdOrders.Split(' ').ToList();
                    foreach (var node in nodeList)
                        IdList.Add(int.Parse(node));
                    break;
            }
        }
    }

    /// <summary>
    /// 軸情報
    /// </summary>
    public class StbAxes:StbModel
    {
    }

    /// <summary>
    /// 階情報（複数）
    /// </summary>
    public class StbStories:StbModel
    {
        public override string Tag { get; } = "StbStory";
        /// <summary>
        /// 階高(m)
        /// </summary>
        public List<double> Height { get; } = new List<double>();
        /// <summary>
        /// 階属性
        /// </summary>
        public List<KindsStory> Kind { get; } = new List<KindsStory>();
        /// <summary>
        /// コンクリート強度
        /// </summary>
        public List<string> StrengthConcrete { get; } = new List<string>();
        public List<List<int>> NodeIdList { get; } = new List<List<int>>();

        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
            Height.Add((double)stbElem.Attribute("height"));
            switch ((string)stbElem.Attribute("kind"))
            {
                case "GENERAL":
                    Kind.Add(KindsStory.General); break;
                case "BASEMENT":
                    Kind.Add(KindsStory.Basement); break;
                case "ROOF":
                    Kind.Add(KindsStory.Roof); break;
                case "PENTHOUSE":
                    Kind.Add(KindsStory.Penthouse); break;
                case "ISOLATION":
                    Kind.Add(KindsStory.Isolation); break;
                case "DEPENDENCE":
                    Kind.Add(KindsStory.Dependence); break;
                default:
                    Kind.Add(KindsStory.Any); break;
            }

            // 必須ではないコード
            if (stbElem.Attribute("name") != null)
                Name.Add((string)stbElem.Attribute("name"));
            else
                Name.Add(string.Empty);
            
            if (stbElem.Attribute("concrete_strength") != null)
                StrengthConcrete.Add((string)stbElem.Attribute("concrete_strength"));
            else
                StrengthConcrete.Add(string.Empty);
        }
    }

    /// <summary>
    /// 継手情報
    /// </summary>
    public class StbJoints
    {
    }

    /// <summary>
    /// 床組（複数）
    /// </summary>
    public class StbSlabFrames
    {
    }
}
