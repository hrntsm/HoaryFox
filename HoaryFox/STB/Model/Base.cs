using System.Collections.Generic;
using System.Xml.Linq;
using static HoaryFox.STB.StbData;

namespace HoaryFox.STB.Model
{
    /// <summary>
    /// 位置・断面情報（節点・部材・階・軸）
    /// </summary>
    public class StbModel : StbBase
    {
        public override string Tag { get; } = "StbModel";
        public List<int> Id { get; } = new List<int>();

        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
            Id.Add((int)stbElem.Attribute("id"));
        }
    }
}