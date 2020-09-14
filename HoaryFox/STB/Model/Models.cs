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
    public class StbNodes : StbModel, IStbLoader
    {
        public List<double> X { get; } = new List<double>();
        public List<double> Y { get; } = new List<double>();
        public List<double> Z { get; } = new List<double>();
        public List<Point3d> Pt { get; } = new List<Point3d>();
        public List<KindsNode> Kind { get; } = new List<KindsNode>();
        public List<int> IdMember { get; } = new List<int>();

        public void Load(XDocument stbDoc, StbVersion version, string xmlns)
        {
            var stbNodes = stbDoc.Root.Descendants(xmlns + "StbNode");
            foreach (var stbNode in stbNodes)
            {
                // 必須コード
                Id.Add((int)stbNode.Attribute("id"));
                double posX;
                double posY;
                double posZ;
                switch (version)
                {
                    case StbVersion.Ver1:
                        posX = (double)stbNode.Attribute("x");
                        posY = (double)stbNode.Attribute("y");
                        posZ = (double)stbNode.Attribute("z");
                        break;
                    case StbVersion.Ver2:
                        posX = (double)stbNode.Attribute("X");
                        posY = (double)stbNode.Attribute("Y");
                        posZ = (double)stbNode.Attribute("Z");
                        if (stbNode.Attribute("guid") != null)
                            Guid.Add((string)stbNode.Attribute("guid"));
                        else
                            Guid.Add(string.Empty);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(version), version, null);
                }
                X.Add(posX);
                Y.Add(posY);
                Z.Add(posZ);
                Pt.Add(new Point3d(posX, posY, posZ));
                
                // 必須ではないコード
                if (stbNode.Attribute("id_member") != null)
                    IdMember.Add((int)stbNode.Attribute("id_member"));
                else
                    IdMember.Add(-1);
                
                // ver2 から必須
                switch ((string)stbNode.Attribute("kind"))
                {
                    case "ON_GIRDER":
                        Kind.Add(KindsNode.OnGirder);
                        break;
                    case "ON_BEAM":
                        Kind.Add(KindsNode.OnBeam);
                        break;
                    case "ON_COLUMN":
                        Kind.Add(KindsNode.OnColumn);
                        break;
                    case "ON_POST":
                        Kind.Add(KindsNode.OnPost);
                        break;
                    case "ON_GRID":
                        Kind.Add(KindsNode.OnGrid);
                        break;
                    case "ON_CANTI":
                        Kind.Add(KindsNode.OnCanti);
                        break;
                    case "ON_SLAB":
                        Kind.Add(KindsNode.OnSlab);
                        break;
                    default:
                        Kind.Add(KindsNode.Other);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 節点IDリスト
    /// </summary>
    public class StbNodeIdList
    {
        public List<int> Load(XElement stbElem, StbVersion stbVersion)
        {
            var idList = new List<int>();

            switch (stbVersion)
            {
                case StbVersion.Ver1:
                    IEnumerable<XElement> xNodeIds = stbElem.Element("StbNodeid_List").Elements("StbNodeid");
                    foreach (var xNodeId in xNodeIds)
                        idList.Add((int)xNodeId.Attribute("id"));
                    break;
                case StbVersion.Ver2:
                    string xNodeIdOrders = stbElem.Value;
                    List<string> nodeList = xNodeIdOrders.Split(' ').ToList();
                    foreach (var node in nodeList)
                        idList.Add(Int32.Parse(node));
                    break;
            }

            return idList;
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
    public class StbStories:StbModel, IStbLoader
    {
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

        public void Load(XDocument stbData, StbVersion version, string xmlns)
        {
            var stbStories = stbData.Root.Descendants(xmlns + "StbStory");
            foreach (var stbStory in stbStories)
            {
                // 必須コード
                Id.Add((int)stbStory.Attribute("id"));
                Height.Add((double)stbStory.Attribute("height"));
                switch ((string)stbStory.Attribute("kind"))
                {
                    case "GENERAL":
                        Kind.Add(KindsStory.General);
                        break;
                    case "BASEMENT":
                        Kind.Add(KindsStory.Basement);
                        break;
                    case "ROOF":
                        Kind.Add(KindsStory.Roof);
                        break;
                    case "PENTHOUSE":
                        Kind.Add(KindsStory.Penthouse);
                        break;
                    case "ISOLATION":
                        Kind.Add(KindsStory.Isolation);
                        break;
                    case "DEPENDENCE":
                        Kind.Add(KindsStory.Dependence);
                        break;
                    default:
                        Kind.Add(KindsStory.Any);
                        break;
                }

                // 必須ではないコード
                // リストの長さが合うように、空の場合はstring.Empty
                if (stbStory.Attribute("name") != null)
                {
                    Name.Add((string)stbStory.Attribute("name"));
                }
                else
                {
                    Name.Add(string.Empty);
                }
                if (stbStory.Attribute("concrete_strength") != null)
                {
                    StrengthConcrete.Add((string)stbStory.Attribute("concrete_strength"));
                }
                else
                {
                    StrengthConcrete.Add(string.Empty);
                }

                // TODO
                // 所属節点の読み込み　List<List<int>> NodeIdList　の Set 部分の作成
            }
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
