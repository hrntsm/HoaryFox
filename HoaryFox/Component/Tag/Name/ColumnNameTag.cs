using System;
using System.Drawing;
using HoaryFox.Component.Base;
using STBReader.Member;

namespace HoaryFox.Component.Tag.Name
{
    public class ColumnNameTag:NameTagBase
    {
        public ColumnNameTag()
          : base(name: "Column Name Tag", nickname: "ColumnTag", description: "Display Column Name Tag", frameType: FrameType.Column)
        {
        }
        protected override Bitmap Icon => Properties.Resource.ColumnName;
        public override Guid ComponentGuid => new Guid("806B9DBE-0207-4E79-A1BE-DD0B37BA9B31");
    }
}