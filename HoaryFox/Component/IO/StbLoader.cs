using System;
using System.Collections.Generic;
using System.Xml.Linq;

using Grasshopper.Kernel;
using HoaryFox.STB;
using Rhino.Geometry;

namespace HoaryFox.Component.IO
{
    public class StbLoader:GH_Component
    {
        private StbData _stbData;
        private string _path;
        private readonly double _lengthTolerance = GH_Component.DocumentTolerance();
        private readonly double _angleTolerance = GH_Component.DocumentAngleTolerance();
            
        public StbLoader()
          : base("Load stb data", "Loader", "Read ST-Bridge file and display", "HoaryFox", "IO")
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
            
            _stbData = new StbData(_path, _lengthTolerance, _angleTolerance);
            
            DA.SetData(0, _stbData);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resource.Brep;

        public override Guid ComponentGuid => new Guid("B8B7631C-BCAE-4549-95F7-1954D4781D24");
    }
}
