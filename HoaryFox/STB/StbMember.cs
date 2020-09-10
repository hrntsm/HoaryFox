using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using HoaryFox.STB;
using Rhino.Geometry;

namespace HoaryFox.STB
{
    public class StbMembers:StbBase
    {
        public List<int> Id { get; } = new List<int>();
        public List<string> Name { get; } = new List<string>();
        public List<int> IdSection { get; } = new List<int>();
        public List<KindsStructure> KindStructure { get; } = new List<KindsStructure>();
    }

    public class StbFrame:StbMembers
    {
        public virtual string Tag { get; } = "StbFrame";
        public virtual FrameType FrameType { get; } = FrameType.Any;
        public List<int> IdNodeStart { get; } = new List<int>();
        public List<int> IdNodeEnd { get; } = new List<int>();
        public List<double> Rotate { get; } = new List<double>();
    }

    public class StbColumns:StbFrame
    {
        public override string Tag { get; } = "StbColumn";
        public override FrameType FrameType { get; } = FrameType.Column;

        public override void Load(XDocument stbData, StbData.StbVersion version, string xmlns)
        {
            if (stbData.Root != null)
            {
                var stbElems = stbData.Root.Descendants(xmlns + Tag);
                foreach (var stbElem in stbElems)
                {
                    Id.Add((int) stbElem.Attribute("id"));
                    Name.Add((string) stbElem.Attribute("name"));
                    IdSection.Add((int) stbElem.Attribute("id_section"));
                    switch (version)
                    {
                        case StbData.StbVersion.Ver1:
                            IdNodeStart.Add((int) stbElem.Attribute("idNode_bottom"));
                            IdNodeEnd.Add((int) stbElem.Attribute("idNode_top"));
                            break;
                        case StbData.StbVersion.Ver2:
                            IdNodeStart.Add((int) stbElem.Attribute("id_node_bottom"));
                            IdNodeEnd.Add((int) stbElem.Attribute("id_node_top"));
                            break;
                    }

                    if (stbElem.Attribute("rotate") != null)
                        Rotate.Add((double) stbElem.Attribute("rotate"));
                    else
                        Rotate.Add(0d);

                    switch ((string) stbElem.Attribute("kind_structure"))
                    {
                        case "RC":
                            KindStructure.Add(KindsStructure.RC);
                            break;
                        case "S":
                            KindStructure.Add(KindsStructure.S);
                            break;
                        case "SRC":
                            KindStructure.Add(KindsStructure.SRC);
                            break;
                        case "CFT":
                            KindStructure.Add(KindsStructure.CFT);
                            break;
                        default:
                            KindStructure.Add(KindsStructure.Other);
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 間柱情報（複数）
    /// </summary>
    public class StbPosts:StbFrame
    {
        public override string Tag { get; } = "StbPost";
        public override FrameType FrameType { get; } = FrameType.Post;

        public override void Load(XDocument stbData, StbData.StbVersion version, string xmlns)
        {
            if (stbData.Root != null)
            {
                var stbElems = stbData.Root.Descendants(xmlns + Tag);
                foreach (var stbElem in stbElems)
                {
                    Id.Add((int)stbElem.Attribute("id"));
                    Name.Add((string)stbElem.Attribute("name"));
                    IdSection.Add((int)stbElem.Attribute("id_section"));
                    switch (version)
                    {
                        case StbData.StbVersion.Ver1:
                            IdNodeStart.Add((int) stbElem.Attribute("idNode_bottom"));
                            IdNodeEnd.Add((int) stbElem.Attribute("idNode_top"));
                            break;
                        case StbData.StbVersion.Ver2:
                            IdNodeStart.Add((int) stbElem.Attribute("id_node_bottom"));
                            IdNodeEnd.Add((int) stbElem.Attribute("id_node_top"));
                            break;
                    }

                    if (stbElem.Attribute("rotate") != null)
                        Rotate.Add((double) stbElem.Attribute("rotate"));
                    else
                        Rotate.Add(0d);

                    switch ((string)stbElem.Attribute("kind_structure"))
                    {
                        case "RC":
                            KindStructure.Add(KindsStructure.RC);
                            break;
                        case "S":
                            KindStructure.Add(KindsStructure.S);
                            break;
                        case "SRC":
                            KindStructure.Add(KindsStructure.SRC);
                            break;
                        case "CFT":
                            KindStructure.Add(KindsStructure.CFT);
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 大梁情報（複数）
    /// </summary>
    public class StbGirders:StbFrame
    {
        public override string Tag { get; } = "StbGirder";
        public override FrameType FrameType { get; } = FrameType.Girder;
        public List<double> Level { get; } = new List<double>();

        public override void Load(XDocument stbData, StbData.StbVersion version, string xmlns)
        {
            if (stbData.Root != null)
            {
                var stbElems = stbData.Root.Descendants(xmlns + Tag);
                foreach (var stbElem in stbElems)
                {
                    Id.Add((int)stbElem.Attribute("id"));
                    Name.Add((string)stbElem.Attribute("name"));
                    IdSection.Add((int)stbElem.Attribute("id_section"));
                    switch (version)
                    {
                        case StbData.StbVersion.Ver1:
                            IdNodeStart.Add((int)stbElem.Attribute("idNode_start"));
                            IdNodeEnd.Add((int)stbElem.Attribute("idNode_end"));
                            break;
                        case StbData.StbVersion.Ver2:
                            IdNodeStart.Add((int)stbElem.Attribute("id_node_start"));
                            IdNodeEnd.Add((int)stbElem.Attribute("id_node_end"));
                            break;
                    }

                    if (stbElem.Attribute("rotate") != null)
                        Rotate.Add((double) stbElem.Attribute("rotate"));
                    else
                        Rotate.Add(0d);

                    switch ((string)stbElem.Attribute("kind_structure"))
                    {
                        case "RC":
                            KindStructure.Add(KindsStructure.RC);
                            break;
                        case "S":
                            KindStructure.Add(KindsStructure.S);
                            break;
                        case "SRC":
                            KindStructure.Add(KindsStructure.SRC);
                            break;
                        case "CFT":
                            KindStructure.Add(KindsStructure.CFT);
                            break;
                        default:
                            break;
                    }

                    if (stbElem.Attribute("level") != null)
                    {
                        Level.Add((double)stbElem.Attribute("level"));
                    }
                    else
                    {
                        Level.Add(0d);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 小梁情報（複数）
    /// </summary>
    public class StbBeams:StbFrame
    {
        public override string Tag { get; } = "StbBeam";
        public override FrameType FrameType { get; } = FrameType.Beam;
        public List<double> Level { get; } = new List<double>();

        public override void Load(XDocument stbData, StbData.StbVersion version, string xmlns)
        {
            if (stbData.Root != null)
            {
                var stbElems = stbData.Root.Descendants(xmlns + Tag);
                foreach (var stbElem in stbElems)
                {
                    Id.Add((int)stbElem.Attribute("id"));
                    Name.Add((string)stbElem.Attribute("name"));
                    IdSection.Add((int)stbElem.Attribute("id_section"));
                    switch (version)
                    {
                        case StbData.StbVersion.Ver1:
                            IdNodeStart.Add((int)stbElem.Attribute("idNode_start"));
                            IdNodeEnd.Add((int)stbElem.Attribute("idNode_end"));
                            break;
                        case StbData.StbVersion.Ver2:
                            IdNodeStart.Add((int)stbElem.Attribute("id_node_start"));
                            IdNodeEnd.Add((int)stbElem.Attribute("id_node_end"));
                            break;
                    }

                    if (stbElem.Attribute("rotate") != null)
                        Rotate.Add((double) stbElem.Attribute("rotate"));
                    else
                        Rotate.Add(0d);

                    switch ((string)stbElem.Attribute("kind_structure"))
                    {
                        case "RC":
                            KindStructure.Add(KindsStructure.RC);
                            break;
                        case "S":
                            KindStructure.Add(KindsStructure.S);
                            break;
                        case "SRC":
                            KindStructure.Add(KindsStructure.SRC);
                            break;
                        case "CFT":
                            KindStructure.Add(KindsStructure.CFT);
                            break;
                        default:
                            break;
                    }

                    if (stbElem.Attribute("level") != null)
                    {
                        Level.Add((double)stbElem.Attribute("level"));
                    }
                    else
                    {
                        Level.Add(0d);
                    }
                }
            }
        }
    }

    /// <summary>
    /// ブレース情報（複数）
    /// </summary>
    public class StbBraces:StbFrame
    {
        public override string Tag { get; } = "StbBrace";
        public override FrameType FrameType { get; } = FrameType.Brace;

        public override void Load(XDocument stbData, StbData.StbVersion version, string xmlns)
        {
            if (stbData.Root != null)
            {
                var stbElems = stbData.Root.Descendants(xmlns + Tag);
                foreach (var stbElem in stbElems)
                {
                    Id.Add((int)stbElem.Attribute("id"));
                    Name.Add((string)stbElem.Attribute("name"));
                    IdSection.Add((int)stbElem.Attribute("id_section"));
                    switch (version)
                    {
                        case StbData.StbVersion.Ver1:
                            IdNodeStart.Add((int)stbElem.Attribute("idNode_start"));
                            IdNodeEnd.Add((int)stbElem.Attribute("idNode_end"));
                            break;
                        case StbData.StbVersion.Ver2:
                            IdNodeStart.Add((int)stbElem.Attribute("id_node_start"));
                            IdNodeEnd.Add((int)stbElem.Attribute("id_node_end"));
                            break;
                    }

                    if (stbElem.Attribute("rotate") != null)
                        Rotate.Add((double) stbElem.Attribute("rotate"));
                    else
                        Rotate.Add(0d);

                    switch ((string)stbElem.Attribute("kind_structure"))
                    {
                        case "RC":
                            KindStructure.Add(KindsStructure.RC);
                            break;
                        case "S":
                            KindStructure.Add(KindsStructure.S);
                            break;
                        case "SRC":
                            KindStructure.Add(KindsStructure.SRC);
                            break;
                        case "CFT":
                            KindStructure.Add(KindsStructure.CFT);
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// スラブ情報（複数）
    /// </summary>
    public class StbSlabs:StbMembers
    {
        public List<KindsSlab> KindSlab { get; } = new List<KindsSlab>();
        public List<double> Level { get; } = new List<double>();
        public List<double> ThicknessExUpper { get; } = new List<double>();
        public List<double> ThicknessExBottom { get; } = new List<double>();
        public List<DirsLoad> DirLoad { get; } = new List<DirsLoad>();
        public List<double> AngleLoad { get; } = new List<double>();
        public List<bool> IsFoundation { get; } = new List<bool>();
        public List<TypesHanch> TypeHaunch { get; } = new List<TypesHanch>();
        public List<List<int>> NodeIdList { get; } = new List<List<int>>();

        public override void Load(XDocument stbDoc, StbData.StbVersion version, string xmlns)
        {
            if (stbDoc.Root != null)
            {
                var stbElems = stbDoc.Root.Descendants(xmlns + "StbSlab");
                foreach (var stbElem in stbElems)
                {
                    // 必須コード
                    Id.Add((int)stbElem.Attribute("id"));
                    Name.Add((string)stbElem.Attribute("name"));
                    IdSection.Add((int)stbElem.Attribute("id_section"));

                    if (stbElem.Attribute("kind_structure") != null)
                    {
                        switch ((string) stbElem.Attribute("kind_structure"))
                        {
                            case "RC":
                                KindStructure.Add(KindsStructure.RC);
                                break;
                            case "DECK":
                                KindStructure.Add(KindsStructure.Deck);
                                break;
                            case "PRECAST":
                                KindStructure.Add(KindsStructure.Precast);
                                break;
                        }
                    }

                    // 必須ではないコード
                    if (stbElem.Attribute("kind_slab") != null)
                    {
                        switch ((string)stbElem.Attribute("kind_slab"))
                        {
                            case "NORMAL":
                                KindSlab.Add(KindsSlab.NORMAL);
                                break;
                            default:
                                KindSlab.Add(KindsSlab.CANTI);
                                break;
                        }
                    }
                    if (stbElem.Attribute("level") != null)
                    {
                        Level.Add((double)stbElem.Attribute("level"));
                    }
                    else
                    {
                        Level.Add(0d);
                    }

                    // 子要素 StbNodeid_List
                    var stbNodeIdList = new StbNodeIdList();
                    NodeIdList.Add(stbNodeIdList.Load(stbElem, version));
                }
            }
        }
    }

    /// <summary>
    /// 壁情報（複数）
    /// </summary>
    public class StbWalls:StbMembers
    {
        public List<List<int>> NodeIdList { get; } = new List<List<int>>();
        public List<KindsLayout> KindLayout { get; } = new List<KindsLayout>();

        public override void Load(XDocument stbDoc, StbData.StbVersion version, string xmlns)
        {
            if (stbDoc.Root != null)
            {
                var stbSlabs = stbDoc.Root.Descendants(xmlns + "StbWall");
                foreach (var stbSlab in stbSlabs)
                {
                    // 必須コード
                    Id.Add((int)stbSlab.Attribute("id"));
                    Name.Add((string)stbSlab.Attribute("name"));
                    IdSection.Add((int)stbSlab.Attribute("id_section"));
                    KindStructure.Add(KindsStructure.RC); // 壁はRCのみ
                    
                    // ver2から必須
                    if (stbSlab.Attribute("kind_layout") != null)
                    {
                        switch ((string)stbSlab.Attribute("kind_layout"))
                        {
                            case "ON_GIRDER":
                                KindLayout.Add(KindsLayout.OnGirder);
                                break;
                            case "ON_BEAM":
                                KindLayout.Add(KindsLayout.OnBeam);
                                break;
                            case "ON_SLAB":
                                KindLayout.Add(KindsLayout.OnSlab);
                                break;
                            default:
                                KindLayout.Add(KindsLayout.Other);
                                break;
                        }
                    }

                    // 子要素 StbNodeid_List
                    var stbNodeIdList = new StbNodeIdList();
                    NodeIdList.Add(stbNodeIdList.Load(stbSlab, version));
                }
            }
        }
    }

    public enum KindsSlab
    {
        NORMAL,
        CANTI
    }

    public enum KindsLayout
    {
        OnGirder,
        OnBeam,
        OnSlab,
        Other
    }

    public enum DirsLoad
    {
        OneWay,
        TwoWay
    }

    public enum TypesHanch
    {
        BOTH,
        TOP,
        BOTTOM
    }

    public enum FrameType
    {
        Column,
        Post,
        Girder,
        Beam,
        Brace,
        Slab,
        Wall,
        Any
    }
}
