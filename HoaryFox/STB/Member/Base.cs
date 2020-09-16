using System.Collections.Generic;
using System.Xml.Linq;
using HoaryFox.STB.Model;
using static HoaryFox.STB.StbData;

namespace HoaryFox.STB.Member
{
    public class StbMembers : StbBase
    {
        public override string Tag { get; } = "StbMember";
        public List<int> Id { get; } = new List<int>();
        public List<int> IdSection { get; } = new List<int>();
        public List<KindsStructure> KindStructure { get; } = new List<KindsStructure>();

        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
            Id.Add((int) stbElem.Attribute("id"));
            IdSection.Add((int) stbElem.Attribute("id_section"));

            switch ((string) stbElem.Attribute("kind_structure"))
            {
                case "RC":
                    KindStructure.Add(KindsStructure.Rc); break;
                case "S":
                    KindStructure.Add(KindsStructure.S); break;
                case "SRC":
                    KindStructure.Add(KindsStructure.Src); break;
                case "CFT":
                    KindStructure.Add(KindsStructure.Cft); break;
                default:
                    KindStructure.Add(KindsStructure.Other); break;
            }
        }
    }

    public class StbFrame : StbMembers
    {
        public override string Tag { get; } = "StbFrame";
        public virtual FrameType FrameType { get; } = FrameType.Any;
        public List<double> Rotate { get; } = new List<double>();
        public List<int> IdNodeStart { get; } = new List<int>();
        public List<int> IdNodeEnd { get; } = new List<int>();

        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
            
            if (stbElem.Attribute("rotate") != null)
                Rotate.Add((double) stbElem.Attribute("rotate"));
            else
                Rotate.Add(0d);
        }
    }

    public class StbPlate : StbMembers
    {
        public override string Tag { get; } = "StbPlate";
        public List<List<int>> NodeIdList { get; } = new List<List<int>>();

        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);

            // 子要素 StbNodeid_List
            var stbNodeIdList = new StbNodeIdList();
            stbNodeIdList.Load(stbElem, version);
            NodeIdList.Add(stbNodeIdList.IdList);
        }
    }
}