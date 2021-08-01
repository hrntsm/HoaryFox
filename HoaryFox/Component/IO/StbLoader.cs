using System;
using Grasshopper.Kernel;
using STBDotNet.v202;

namespace HoaryFox.Component_v2.IO
{
    public class StbLoader : GH_Component
    {
        private string _path;
        public override GH_Exposure Exposure => GH_Exposure.primary;

        public StbLoader()
          : base("Load STB file", "Loader",
              "Read ST-Bridge file",
              "HoaryFox2", "IO")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("path", "path", "input ST-Bridge file path", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "output StbData", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            // 対象の stb の path を取得
            if (!dataAccess.GetData("path", ref _path)) { return; }

            var stbData = (ST_BRIDGE)STBDotNet.Serialization.Serializer.Deserialize(_path);
            dataAccess.SetData(0, stbData);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resource.LoadStb;
        public override Guid ComponentGuid => new Guid("C1E1CD82-9AC0-479C-A22F-DB7C44F3C77D");
    }
}
