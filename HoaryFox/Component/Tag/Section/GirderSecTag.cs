using System;
using System.Drawing;
using HoaryFox.Component.Base;
using STBReader.Member;

namespace HoaryFox.Component.Tag.Section
{
    public class GirderSecTag:SecTagBase
    {
        public GirderSecTag()
          : base(name: "Girder Section Tag", nickname: "GirderSec", description: "Display Girder Section Tag", frameType: FrameType.Girder)
        {
        }
        protected override Bitmap Icon => Properties.Resource.GirderSection;
        public override Guid ComponentGuid => new Guid("D72C9B9D-6233-44EF-B588-D2854BB4FB4F");
    }
}
