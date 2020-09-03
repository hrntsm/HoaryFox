using System.Collections.Generic;
using System.Xml.Linq;
using Rhino.Geometry;

namespace HoaryFox.STB
{
    /// <summary>
    /// 位置・断面情報（節点・部材・階・軸）
    /// </summary>
    public class StbModel
    {
        // TODO 一括でStbModelに属するものを読み込めるようにする
        //public void LoadAll(XDocument stbData) {
        //}
    }

    /// <summary>
    /// 節点（複数） 各節点を管理
    /// </summary>
    public class StbNodes:StbBase
    {
        public List<int> Id { get; } = new List<int>();
        public List<double> X { get; } = new List<double>();
        public List<double> Y { get; } = new List<double>();
        public List<double> Z { get; } = new List<double>();
        public List<Point3d> Pt { get; } = new List<Point3d>();
        public List<KindsNode> Kind { get; } = new List<KindsNode>();
        public List<int> IdMember { get; } = new List<int>();

        public override void Load(XDocument stbDoc)
        {
            var stbNodes = stbDoc.Root.Descendants("StbNode");
            foreach (var stbNode in stbNodes)
            {
                // 必須コード
                Id.Add((int)stbNode.Attribute("id"));
                X.Add((double)stbNode.Attribute("x"));
                Y.Add((double)stbNode.Attribute("y"));
                Z.Add((double)stbNode.Attribute("z"));
                Pt.Add(new Point3d((double)stbNode.Attribute("x"),(double)stbNode.Attribute("y"), (double)stbNode.Attribute("z")));

                // 必須ではないコード
                if (stbNode.Attribute("id_member") != null)
                    IdMember.Add((int)stbNode.Attribute("id_member"));
                else
                    IdMember.Add(-1);
                switch ((string)stbNode.Attribute("kind"))
                {
                    case "ON_BEAM":
                        Kind.Add(KindsNode.OnBeam); break;
                    case "ON_COLUMN":
                        Kind.Add(KindsNode.OnColumn); break;
                    case "ON_GRID":
                        Kind.Add(KindsNode.OnGrid); break;
                    case "ON_CANTI":
                        Kind.Add(KindsNode.OnCanti); break;
                    case "ON_SLAB":
                        Kind.Add(KindsNode.OnSlab); break;
                    case "OTHER":
                        Kind.Add(KindsNode.Other); break;
                    default:
                        break;
                }
            }
        }

        public enum KindsNode
        {
            OnBeam,
            OnColumn,
            OnGrid,
            OnCanti,
            OnSlab,
            Other
        }
    }

    /// <summary>
    /// 節点IDリスト
    /// </summary>
    public class StbNodeIdList
    {
        public List<int> Load(XElement stbElem)
        {
            List<int> idList = new List<int>();

            var xNodeIds = stbElem.Element("StbNodeid_List").Elements("StbNodeid");
            foreach (var xNodeId in xNodeIds)
            {
                idList.Add((int)xNodeId.Attribute("id"));
            }
            return idList;
        }
    }

    /// <summary>
    /// 軸情報
    /// </summary>
    public class StbAxes
    {
    }


    /// <summary>
    /// 階情報（複数）
    /// </summary>
    public class StbStorys:StbBase
    {
        /// <summary>
        /// 階のID
        /// </summary>
        public List<int> Id { get; } = new List<int>();
        /// <summary>
        /// 階名称
        /// </summary>
        public List<string> Name { get; } = new List<string>();
        /// <summary>
        /// 階高(m)
        /// </summary>
        public List<float> Height { get; } = new List<float>();
        /// <summary>
        /// 階属性
        /// </summary>
        public List<KindsStory> Kind { get; } = new List<KindsStory>();
        /// <summary>
        /// コンクリート強度
        /// </summary>
        public List<string> StrengthConcrete { get; } = new List<string>();
        public List<List<int>> NodeIdList { get; } = new List<List<int>>();

        public override void Load(XDocument stbData)
        {
            var stbStorys = stbData.Root.Descendants("StbStory");
            foreach (var stbStory in stbStorys)
            {
                // 必須コード
                Id.Add((int)stbStory.Attribute("id"));
                Height.Add((float)stbStory.Attribute("height") / 1000f);
                switch ((string)stbStory.Attribute("kind"))
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
                    default:
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

        public enum KindsStory
        {
            General,
            Basement,
            Roof,
            Penthouse,
            Isolation
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

    public enum KindsStructure
    {
        RC,
        S,
        SRC,
        CFT
    }
}
