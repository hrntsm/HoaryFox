using System;
using System.Drawing;
using HoaryFox.Component.Base;
using STBReader.Member;

namespace HoaryFox.Component.Tag.Section
{
    public class PostSecTag:SecTagBase
    {
        public PostSecTag()
          : base(name: "Post Section Tag", nickname: "PostSec", description: "Display Post Section Tag", frameType: FrameType.Post)
        {
        }
        protected override Bitmap Icon => Properties.Resource.PostSection;
        public override Guid ComponentGuid => new Guid("C5891374-37F7-43E8-9D28-A901D87B497E");
    }
}
