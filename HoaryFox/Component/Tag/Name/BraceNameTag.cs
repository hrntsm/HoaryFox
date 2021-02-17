using System;
using System.Drawing;
using HoaryFox.Component.Base;
using STBReader.Member;

namespace HoaryFox.Component.Tag.Name
{
    public class BraceNameTag : NameTagBase
    {
        public BraceNameTag()
          : base(name: "Brace Name Tag", nickname: "BraceTag", description: "Display Brace Name Tag", frameType: FrameType.Brace)
        {
        }
        protected override Bitmap Icon => Properties.Resource.BraceName;
        public override Guid ComponentGuid => new Guid("E566DDCB-4192-40B2-8E96-2083207CC5A8");
    }
}
