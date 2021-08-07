using System.Collections.Generic;
using STBDotNet.v202;

namespace KarambaConnect.K2S
{
    internal class K2SSecSteelItems
    {
        public List<StbSecFlatBar> SecFlatBars { get; set; }
        public List<StbSecRollBOX> SecRollBOXes { get; set; }
        public List<StbSecRollH> SecRollHs { get; set; }
        public List<StbSecRollT> SecRollTs { get; set; }
        public List<StbSecPipe> SecPipes { get; set; }

        public StbSecSteel ToStb()
        {
            return new StbSecSteel
            {
                StbSecFlatBar = SecFlatBars.ToArray(),
                StbSecRollBOX = SecRollBOXes.ToArray(),
                StbSecRollH = SecRollHs.ToArray(),
                StbSecRollT = SecRollTs.ToArray(),
                StbSecPipe = SecPipes.ToArray()
            };
        }
    }
}
