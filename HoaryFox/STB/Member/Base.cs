using System.Collections.Generic;
using HoaryFox.STB.Model;

namespace HoaryFox.STB.Member
{
    public class StbMembers : StbBase
    {
        public override string Tag { get; } = "StbMember";
        public List<int> Id { get; } = new List<int>();
        public List<int> IdSection { get; } = new List<int>();
        public List<KindsStructure> KindStructure { get; } = new List<KindsStructure>();
    }

    public class StbFrame : StbMembers
    {
        public override string Tag { get; } = "StbFrame";
        public virtual FrameType FrameType { get; } = FrameType.Any;
        public List<int> IdNodeStart { get; } = new List<int>();
        public List<int> IdNodeEnd { get; } = new List<int>();
        public List<double> Rotate { get; } = new List<double>();
    }

    public class StbPlate : StbMembers
    {
        public override string Tag { get; } = "StbPlate";
        public List<List<int>> NodeIdList { get; } = new List<List<int>>();
    }
}