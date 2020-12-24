using System;
using Grasshopper.Kernel;
using Karamba.GHopper.Models;
using Karamba.Models;
using STBDotNet;
using STBDotNet.Elements;

namespace karambaConnect.Component
{
    public class Karamba2Stb:GH_Component
    {
        public Karamba2Stb()
          : base("Karamba to Stb", "K2S", "Convert Karamba model to ST-Bridge data and output.", "HoaryFox", "IO")
        {
        }

        public override void ClearData()
        {
            base.ClearData();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Model(), "Model", "Model", "Karamba model data", GH_ParamAccess.item);
            pManager.AddTextParameter("Path", "Path", "Output path", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Out", "Out", "If it is true, output stb file.", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var model = new Model();
            var path = string.Empty;
            var isOutput = false;
            var obj = new object();
            
            if (!DA.GetData(0, ref obj)) { return; }
            if (!DA.GetData(1, ref path)) { return; }
            if (!DA.GetData(2, ref isOutput)) { return; }

            if (isOutput)
            {
                StbElements elements = GetStbData(model);
                var sr = new STBDotNet.Serialization.Serializer();
                sr.Serialize(elements, path);
            }
        }

        private StbElements GetStbData(Model model)
        {
            return new StbElements();
        }

        // protected override Bitmap Icon => karambaConnect.Properties.Resource.ToKaramba;
        public override Guid ComponentGuid => new Guid("38296D06-E47A-403F-BFE8-00E873A99CF8");
    }
}
