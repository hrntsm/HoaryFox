using System.Collections.Generic;

using STBDotNet.v202;

namespace KarambaConnect.K2S
{
    internal class K2SSecSteelItems
    {
        public List<StbSecFlatBar> SecFlatBars { get; set; }
        public List<StbSecRollBOX> SecRollBoxes { get; set; }
        public List<StbSecRollH> SecRollHs { get; set; }
        public List<StbSecRollT> SecRollTs { get; set; }
        public List<StbSecPipe> SecPipes { get; set; }

        public StbSecSteel ToStb()
        {
            return new StbSecSteel
            {
                StbSecFlatBar = SecFlatBars != null ? SecFlatBars.ToArray() : System.Array.Empty<StbSecFlatBar>(),
                StbSecRollBOX = SecRollBoxes != null ? SecRollBoxes.ToArray() : System.Array.Empty<StbSecRollBOX>(),
                StbSecRollH = SecRollHs != null ? SecRollHs.ToArray() : System.Array.Empty<StbSecRollH>(),
                StbSecRollT = SecRollTs != null ? SecRollTs.ToArray() : System.Array.Empty<StbSecRollT>(),
                StbSecPipe = SecPipes != null ? SecPipes.ToArray() : System.Array.Empty<StbSecPipe>(),
            };
        }
    }
}
