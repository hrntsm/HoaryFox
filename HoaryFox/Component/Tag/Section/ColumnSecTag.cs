using System;
using System.Drawing;
using HoaryFox.Component.Base;
using STBReader.Member;

namespace HoaryFox.Component.Tag.Section
{
    public class ColumnSecTag : SecTagBase
    {
        public ColumnSecTag()
            : base(name: "Column Section Tag", nickname: "ColumnSec", description: "Display Column Section Tag", frameType: FrameType.Column)
        {
        }
        protected override Bitmap Icon => Properties.Resource.ColumnSection;
        public override Guid ComponentGuid => new Guid("63B2A2E1-A277-4ABA-B522-00D7969871C3");
    }
}
