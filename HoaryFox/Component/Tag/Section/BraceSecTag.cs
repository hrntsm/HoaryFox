using System;
using System.Drawing;
using HoaryFox.Component.Base;
using STBReader.Member;

namespace HoaryFox.Component.Tag.Section
{
    public class BraceSecTag:SecTagBase
    {
        public BraceSecTag()
          : base(name: "Brace Section Tag", nickname: "BraceSec", description: "Display Beam Section Tag", frameType: FrameType.Brace)
        {
        }
        protected override Bitmap Icon => Properties.Resource.BraceSection;
        public override Guid ComponentGuid => new Guid("DE9D56D2-C82C-4AB1-96FF-D1253B420A43");
    }
}
