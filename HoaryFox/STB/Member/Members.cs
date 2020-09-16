using System.Collections.Generic;
using System.Xml.Linq;
using HoaryFox.STB.Model;
using static HoaryFox.STB.StbData;

namespace HoaryFox.STB.Member
{
    public class StbColumns:StbFrame
    {
        public override string Tag { get; } = "StbColumn";
        public override FrameType FrameType { get; } = FrameType.Column;

        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
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
        }
    }

    /// <summary>
    /// 間柱情報（複数）
    /// </summary>
    public class StbPosts:StbFrame
    {
        public override string Tag { get; } = "StbPost";
        public override FrameType FrameType { get; } = FrameType.Post;

        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
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

        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
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

            if (stbElem.Attribute("level") != null)
                Level.Add((double)stbElem.Attribute("level"));
            else
                Level.Add(0d);
        }
    }

    /// <summary>
    /// 小梁情報（複数）
    /// </summary>
    public class StbBeams : StbFrame
    {
        public override string Tag { get; } = "StbBeam";
        public override FrameType FrameType { get; } = FrameType.Beam;
        public List<double> Level { get; } = new List<double>();

        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
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

            if (stbElem.Attribute("level") != null)
                Level.Add((double)stbElem.Attribute("level"));
            else
                Level.Add(0d);
        }
    }

    /// <summary>
    /// ブレース情報（複数）
    /// </summary>
    public class StbBraces : StbFrame
    {
        public override string Tag { get; } = "StbBrace";
        public override FrameType FrameType { get; } = FrameType.Brace;

        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
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
        }
    }

    /// <summary>
    /// スラブ情報（複数）
    /// </summary>
    public class StbSlabs:StbPlate
    {
        public override string Tag { get; } = "StbSlab";
        public List<double> Level { get; } = new List<double>();
        public List<KindsSlab> KindSlab { get; } = new List<KindsSlab>();
        public List<double> ThicknessExUpper { get; } = new List<double>();
        public List<double> ThicknessExBottom { get; } = new List<double>();
        public List<DirsLoad> DirLoad { get; } = new List<DirsLoad>();
        public List<double> AngleLoad { get; } = new List<double>();
        public List<bool> IsFoundation { get; } = new List<bool>();
        public List<TypesHanch> TypeHaunch { get; } = new List<TypesHanch>();

        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
            if (stbElem.Attribute("level") != null)
                Level.Add((double)stbElem.Attribute("level"));
            else
                Level.Add(0d);
        }
    }

    /// <summary>
    /// 壁情報（複数）
    /// </summary>
    public class StbWalls : StbPlate
    {
        public override string Tag { get; } = "StbWall";
        public List<StbOpen> Opens { get; } = new List<StbOpen>();
        public List<KindsLayout> KindLayout { get; } = new List<KindsLayout>();

        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
                    
            if (stbElem.Attribute("kind_layout") == null)
            {
                KindLayout.Add(KindsLayout.Other);
            }
            else
            {
                switch ((string)stbElem.Attribute("kind_layout"))
                {
                    case "ON_GIRDER":
                        KindLayout.Add(KindsLayout.OnGirder); break;
                    case "ON_BEAM":
                        KindLayout.Add(KindsLayout.OnBeam); break;
                    case "ON_SLAB":
                        KindLayout.Add(KindsLayout.OnSlab); break;
                    default:
                        KindLayout.Add(KindsLayout.Other); break;
                }
            }
            
            var stbOpen = new StbOpen();
            stbOpen.Load(stbElem, version);
            Opens.Add(stbOpen);
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
                    foreach (var xOpen in xOpens)
                    {
                        if (xOpen.Attribute("id") != null)
                            Id.Add((int) xOpen.Attribute("id"));
                        else
                            Id.Add(0);

                        if (xOpen.Attribute("name") != null)
                            Name.Add((string) xOpen.Attribute("name"));
                        else
                            Name.Add(string.Empty);

                        IdSection.Add((int) xOpen.Attribute("id_section"));
                        PositionX.Add((double) xOpen.Attribute("position_X"));
                        PositionY.Add((double) xOpen.Attribute("position_Y"));
                        LengthX.Add((double) xOpen.Attribute("length_X"));
                        LengthY.Add((double) xOpen.Attribute("length_Y"));
                        Rotate.Add((double) xOpen.Attribute("rotate"));
                    }
                    break;
                case StbVersion.Ver2:
                    break;
            }
        }
    }
}
