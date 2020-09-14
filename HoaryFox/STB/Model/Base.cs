using System.Collections.Generic;

namespace HoaryFox.STB.Model
{
    /// <summary>
    /// 位置・断面情報（節点・部材・階・軸）
    /// </summary>
    public class StbModel : StbBase
    {
        public override string Tag { get; } = "StbModel";
        public List<int> Id { get; } = new List<int>();
    }
}