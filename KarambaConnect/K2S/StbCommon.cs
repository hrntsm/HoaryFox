using System;
using STBDotNet.Elements.StbCommon;

namespace KarambaConnect.K2S
{
    public static class StbCommon
    {
        public static Common Set()
        {
            var common = new Common
            {
                AppName = "HoaryFox Stb Converter",
                ProjectName = "Grasshopper Karamba model",
                Guid = Guid.NewGuid().ToString("D")
            };

            return common;
        }
    }
}
