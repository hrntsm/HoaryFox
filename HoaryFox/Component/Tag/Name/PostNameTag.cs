using System;
using System.Drawing;
using HoaryFox.Component.Base;
using STBReader.Member;

namespace HoaryFox.Component.Tag.Name
{
    public class PostNameTag:NameTagBase
    {
        public PostNameTag()
          : base(name: "Post Name Tag", nickname: "PostTag", description: "Display Post Name Tag", frameType: FrameType.Post)
        {
        }
        protected override Bitmap Icon => Properties.Resource.PostName;
        public override Guid ComponentGuid => new Guid("8FAC9887-B49F-4FC1-8B6B-7847FCE49339");
    }
}