using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace KarambaConnect
{
    public class KarambaConnectInfo : GH_AssemblyInfo
    {
        public override string Name => "HoaryFox(karambaConnect)";

        public override Bitmap Icon => null;

        public override string Description => "HoaryFox karamba connect extension";

        public override Guid Id => new Guid("ffa60c17-4050-4232-96db-011eccbd402d");

        public override string AuthorName => "hrntsm";

        public override string AuthorContact => "contact@hrntsm.com";
    }
}
