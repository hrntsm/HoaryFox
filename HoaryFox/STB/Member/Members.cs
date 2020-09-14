using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using HoaryFox.STB;
using Rhino.ApplicationSettings;
using Rhino.Geometry;
using static HoaryFox.STB.StbData;
using HoaryFox.STB.Model;

namespace HoaryFox.STB.Member
{

    public class StbColumns : StbFrame, IStbLoader
    {
        public override string Tag { get; } = "StbColumn";
        public override FrameType FrameType { get; } = FrameType.Column;

        public void Load(XDocument stbData, StbData.StbVersion version, string xmlns)
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
                        case StbVersion.Ver1:
                            IdNodeStart.Add((int) stbElem.Attribute("idNode_bottom"));
                            IdNodeEnd.Add((int) stbElem.Attribute("idNode_top"));
                            break;
                        case StbVersion.Ver2:
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
                            KindStructure.Add(KindsStructure.Rc);
                            break;
                        case "S":
                            KindStructure.Add(KindsStructure.S);
                            break;
                        case "SRC":
                            KindStructure.Add(KindsStructure.Src);
                            break;
                        case "CFT":
                            KindStructure.Add(KindsStructure.Cft);
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
    public class StbPosts : StbFrame, IStbLoader
    {
        public override string Tag { get; } = "StbPost";
        public override FrameType FrameType { get; } = FrameType.Post;

        public void Load(XDocument stbData, StbData.StbVersion version, string xmlns)
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
                        case StbVersion.Ver1:
                            IdNodeStart.Add((int) stbElem.Attribute("idNode_bottom"));
                            IdNodeEnd.Add((int) stbElem.Attribute("idNode_top"));
                            break;
                        case StbVersion.Ver2:
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
                            KindStructure.Add(KindsStructure.Rc);
                            break;
                        case "S":
                            KindStructure.Add(KindsStructure.S);
                            break;
                        case "SRC":
                            KindStructure.Add(KindsStructure.Src);
                            break;
                        case "CFT":
                            KindStructure.Add(KindsStructure.Cft);
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 大梁情報（複数）
    /// </summary>
    public class StbGirders : StbFrame, IStbLoader
    {
        public override string Tag { get; } = "StbGirder";
        public override FrameType FrameType { get; } = FrameType.Girder;
        public List<double> Level { get; } = new List<double>();

        public void Load(XDocument stbData, StbData.StbVersion version, string xmlns)
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
                        case StbVersion.Ver1:
                            IdNodeStart.Add((int)stbElem.Attribute("idNode_start"));
                            IdNodeEnd.Add((int)stbElem.Attribute("idNode_end"));
                            break;
                        case StbVersion.Ver2:
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
                            KindStructure.Add(KindsStructure.Rc);
                            break;
                        case "S":
                            KindStructure.Add(KindsStructure.S);
                            break;
                        case "SRC":
                            KindStructure.Add(KindsStructure.Src);
                            break;
                        case "CFT":
                            KindStructure.Add(KindsStructure.Cft);
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
    public class StbBeams : StbFrame, IStbLoader
    {
        public override string Tag { get; } = "StbBeam";
        public override FrameType FrameType { get; } = FrameType.Beam;
        public List<double> Level { get; } = new List<double>();

        public void Load(XDocument stbData, StbData.StbVersion version, string xmlns)
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
                        case StbVersion.Ver1:
                            IdNodeStart.Add((int)stbElem.Attribute("idNode_start"));
                            IdNodeEnd.Add((int)stbElem.Attribute("idNode_end"));
                            break;
                        case StbVersion.Ver2:
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
                            KindStructure.Add(KindsStructure.Rc);
                            break;
                        case "S":
                            KindStructure.Add(KindsStructure.S);
                            break;
                        case "SRC":
                            KindStructure.Add(KindsStructure.Src);
                            break;
                        case "CFT":
                            KindStructure.Add(KindsStructure.Cft);
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
    public class StbBraces : StbFrame, IStbLoader
    {
        public override string Tag { get; } = "StbBrace";
        public override FrameType FrameType { get; } = FrameType.Brace;

        public void Load(XDocument stbData, StbData.StbVersion version, string xmlns)
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
                        case StbVersion.Ver1:
                            IdNodeStart.Add((int)stbElem.Attribute("idNode_start"));
                            IdNodeEnd.Add((int)stbElem.Attribute("idNode_end"));
                            break;
                        case StbVersion.Ver2:
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
                            KindStructure.Add(KindsStructure.Rc);
                            break;
                        case "S":
                            KindStructure.Add(KindsStructure.S);
                            break;
                        case "SRC":
                            KindStructure.Add(KindsStructure.Src);
                            break;
                        case "CFT":
                            KindStructure.Add(KindsStructure.Cft);
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// スラブ情報（複数）
    /// </summary>
    public class StbSlabs : StbPlate, IStbLoader
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

        public void Load(XDocument stbDoc, StbVersion version, string xmlns)
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
                                KindStructure.Add(KindsStructure.Rc);
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
                                KindSlab.Add(KindsSlab.Normal);
                                break;
                            default:
                                KindSlab.Add(KindsSlab.Canti);
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
    public class StbWalls : StbPlate, IStbLoader
    {
        public List<List<int>> NodeIdList { get; } = new List<List<int>>();
        public List<StbOpen> Opens { get; } = new List<StbOpen>();
        public List<KindsLayout> KindLayout { get; } = new List<KindsLayout>();

        public override void Load(XDocument stbDoc, StbVersion version, string xmlns)
        public void Load(XDocument stbDoc, StbData.StbVersion version, string xmlns)
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
                    KindStructure.Add(KindsStructure.Rc); // 壁はRCのみ
                    
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
                    var stbOpen = new StbOpen();
                    stbOpen.Load(stbSlab, version);
                    Opens.Add(stbOpen);
                }
            }
        }
    }

    public class StbOpen
    {
        public List<int> Id { get; } = new List<int>();
        public List<string> Name { get; } = new List<string>();
        public List<int> IdSection { get; } = new List<int>();
        public List<double> PositionX { get; } = new List<double>();
        public List<double> PositionY { get; } = new List<double>();
        public List<double> LengthX { get; } = new List<double>();
        public List<double> LengthY { get; } = new List<double>();
        public List<double> Rotate { get; } = new List<double>();
        
        public void Load(XElement stbElem, StbVersion version)
        {
            switch (version)
            {
                case StbVersion.Ver1:
                    var xOpens = stbElem.Elements("StbOpen");
                    if (xOpens == null)
                    {
                            Id.Add(-1);
                            Name.Add(string.Empty);
                            IdSection.Add(-1);
                            PositionX.Add(-1);
                            PositionY.Add(-1);
                            LengthX.Add(-1);
                            LengthY.Add(-1);
                            Rotate.Add(-1);
                    }
                    else
                    {
                        foreach (var xOpen in xOpens)
                        {
                            if (xOpen.Attribute("id") != null)
                                Id.Add((int) xOpen.Attribute("id"));
                            else
                                Id.Add(0);

                            if (xOpen.Attribute("name") != null)
                                Name.Add((string)xOpen.Attribute("name"));
                            else
                                Name.Add(string.Empty);
                        
                            IdSection.Add((int)xOpen.Attribute("id_section"));
                            PositionX.Add((double)xOpen.Attribute("position_X"));
                            PositionY.Add((double)xOpen.Attribute("position_Y"));
                            LengthX.Add((double)xOpen.Attribute("length_X"));
                            LengthY.Add((double)xOpen.Attribute("length_Y"));
                            Rotate.Add((double)xOpen.Attribute("rotate"));
                        }
                    }
                    break;
                case StbVersion.Ver2:
                    break;
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
