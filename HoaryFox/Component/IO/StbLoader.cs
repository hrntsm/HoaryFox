using System;
using Grasshopper.Kernel;
using STBReader;

namespace HoaryFox.Component.IO
{
    public class StbLoader : GH_Component
    {
        private string _path;
        private readonly double _lengthTolerance = DocumentTolerance();
        private readonly double _angleTolerance = DocumentAngleTolerance();
        public override GH_Exposure Exposure => GH_Exposure.primary;

        public StbLoader()
          : base("Load STB file", "Loader", "Read ST-Bridge file and display", "HoaryFox", "IO")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("path", "path", "input ST-Bridge file path", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "output StbData", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // 対象の stb の pathを取得
            if (!DA.GetData("path", ref _path)) { return; }

            var stbData = new StbData(_path, _lengthTolerance, _angleTolerance);

            DA.SetData(0, stbData);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resource.LoadStb;
        public override Guid ComponentGuid => new Guid("B8B7631C-BCAE-4549-95F7-1954D4781D24");
    }
}
