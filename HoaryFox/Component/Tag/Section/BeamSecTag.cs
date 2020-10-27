using System;
using System.Drawing;
using HoaryFox.Component.Base;
using STBReader.Member;

namespace HoaryFox.Component.Tag.Section
{
    public class BeamSecTag:SecTagBase
    {
        public BeamSecTag()
          : base(name: "Beam Section Tag", nickname: "BeamSec", description: "Display Beam Section Tag", frameType: FrameType.Beam)
        {
        }
        protected override Bitmap Icon => Properties.Resource.BeamSection;
        public override Guid ComponentGuid => new Guid("6310E95D-38AF-47A6-B792-E4680FE37F49");
    }
}
