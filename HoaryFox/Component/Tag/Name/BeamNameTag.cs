using System;
using System.Drawing;
using HoaryFox.Component.Base;
using STBReader.Member;

namespace HoaryFox.Component.Tag.Name
{
    public class BeamNameTag:NameTagBase
    {
        public BeamNameTag()
            : base(name: "Beam Name Tag", nickname: "BeamTag", description: "Display Beam Name Tag", frameType: FrameType.Beam)
        {
        }
        protected override Bitmap Icon => Properties.Resource.BeamName;
        public override Guid ComponentGuid => new Guid("758DE991-F652-4EDC-BC63-2A454BA43FB1");
    }
}