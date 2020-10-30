using System;
using System.Drawing;
using HoaryFox.Component.Base;
using STBReader.Member;

namespace HoaryFox.Component.Tag.Name
{
    public class GirderNameTag:NameTagBase
    {
        public GirderNameTag()
          : base(name: "Girder Name Tag", nickname: "GirderTag", description: "Display girder Name Tag ", frameType: FrameType.Girder)
        {
        }
        protected override Bitmap Icon => Properties.Resource.GirderName;
        public override Guid ComponentGuid => new Guid("35D72484-2675-487E-A970-5DE885582312");
    }
}