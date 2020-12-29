using System;
using Grasshopper.Kernel;
using Karamba.Models;
using Karamba.GHopper.Models;
using karambaConnect.K2S;

namespace karambaConnect.Component
{
    public class Karamba2Stb:GH_Component
    {
        public Karamba2Stb()
          : base("Karamba to Stb", "K2S", "Convert Karamba model to ST-Bridge data.", "HoaryFox", "Export")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Model(), "Model", "Model", "Karamba model data", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Node", "Node", "StbNode Data", GH_ParamAccess.list);
            pManager.AddGenericParameter("Member", "Mem", "StbMember data", GH_ParamAccess.item);
            pManager.AddGenericParameter("Section", "Sec", "StbSection data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var modelIn = new object();
            
            if (!DA.GetData(0, ref modelIn)) { return; }

            var ghKModel = modelIn as GH_Model;
            if (ghKModel == null)
            {
                throw new ArgumentException("The input is not of type model!");
            }
            Model kModel = ghKModel.Value;
            STBDotNet.Elements.StbModel.Model sModel = StbModel.Set(kModel);
            
            DA.SetDataList(0, sModel.Nodes);
            DA.SetData(1, sModel.Members);
            DA.SetDataList(2, sModel.Sections);
        }

        // protected override Bitmap Icon => karambaConnect.Properties.Resource.ToKaramba;
        public override Guid ComponentGuid => new Guid("38296D06-E47A-403F-BFE8-00E873A99CF8");
    }
}
