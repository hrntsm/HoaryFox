using System;
using Grasshopper.Kernel;
using Karamba.Models;
using Karamba.GHopper.Models;
using STBDotNet.Elements;
using STBDotNet.Serialization;

namespace karambaConnect.Component
{
    public class Karamba2Stb:GH_Component
    {
        private readonly string defaultOutPath =
            Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\model.stb";


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
            pManager.AddTextParameter("Path", "Path", "Output path", GH_ParamAccess.item, defaultOutPath);
            pManager.AddBooleanParameter("Out?", "Out?", "If it is true, output stb file.", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var path = string.Empty;
            var isOutput = false;
            var modelIn = new object();
            
            if (!DA.GetData(0, ref modelIn)) { return; }
            if (!DA.GetData(1, ref path)) { return; }
            if (!DA.GetData(2, ref isOutput)) { return; }

            var ghKModel = modelIn as GH_Model;
            if (ghKModel == null)
            {
                throw new ArgumentException("The input is not of type model!");
            }
            Model model = ghKModel.Value;
            
            if (isOutput)
            {
                StbElements elements = GetStbData(model);
                var sr = new Serializer();
                sr.Serialize(elements, path);
            }
        }

        private StbElements GetStbData(Model model)
        {
            var elem = new StbElements
            {
                Version = "1.4.00",
                Common = K2S.StbCommon.Set(),
                Model = K2S.StbModel.Set(model)
            };
            return elem;
        }


        // protected override Bitmap Icon => karambaConnect.Properties.Resource.ToKaramba;
        public override Guid ComponentGuid => new Guid("38296D06-E47A-403F-BFE8-00E873A99CF8");
    }
}
