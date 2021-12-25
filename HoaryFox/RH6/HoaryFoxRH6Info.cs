using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace HoaryFox
{
    public class HoaryFoxRH6Info : GH_AssemblyInfo
    {
        public override string Name => "HoaryFox";

        public override Bitmap Icon => HoaryFoxCommon.Properties.Resource.InfoIcon;

        public override string Description => "This Component read ST-Bridge file(.stb) and display its model data.";

        public override Guid Id => new Guid("093de648-746b-4b0b-85ef-495c6fb4514f");

        public override string AuthorName => "hrntsm";

        public override string AuthorContact => "contact@hrntsm.com";
    }
}
