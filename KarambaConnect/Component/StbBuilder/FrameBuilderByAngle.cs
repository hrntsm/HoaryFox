using System;
using System.Drawing;
using Grasshopper.Kernel;
using Karamba.GHopper.Models;
using Karamba.Models;
using KarambaConnect.K2S;

namespace KarambaConnect.Component.StbBuilder
{
    public class FrameBuilderByAngle:GH_Component
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;

        public FrameBuilderByAngle()
          : base("FrameBuilder by angle", "Frame by angle", "Convert Karamba model to StbModel & StbSection by elements angle.", "HoaryFox", "StbBuilder")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Model(), "Model", "Model", "Karamba model data", GH_ParamAccess.item);
            pManager.AddNumberParameter("Angle", "Angle", "Maximum angle of the column to the Z-axis", GH_ParamAccess.item, Math.PI / 4d);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Member", "Mem", "StbMember data", GH_ParamAccess.item);
            pManager.AddGenericParameter("Section", "Sec", "StbSection data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double colMaxAngle = 0;
            var modelIn = new object();
            if (!DA.GetData(0, ref modelIn)) { return; }
            if (!DA.GetData(1, ref colMaxAngle)) { return; }

            if (!(modelIn is GH_Model ghKModel))
            {
                throw new ArgumentException("The input is not Karamba3D model!");
            }
            Model kModel = ghKModel.Value;
            var stbModel = new StbModel(kModel);
            STBDotNet.Elements.StbModel.Model sModel = stbModel.SetByAngle(colMaxAngle);
            
            DA.SetData(0, sModel.Members);
            DA.SetDataList(1, sModel.Sections);
        }

        protected override Bitmap Icon => Properties.Resource.FrameBuilder;
        public override Guid ComponentGuid => new Guid("38296D06-E47A-403F-BFE8-00E873A99CF8");
    }
}
